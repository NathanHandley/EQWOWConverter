//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2024 Nathan Handley
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using EQWOWConverter.Common;
using EQWOWConverter.ModelObjects.WOW;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.ModelObjects
{
    internal class ModelTrackSequences<T> where T : ByteSerializable
    {
        ModelAnimationInterpolationType InterpolationType = ModelAnimationInterpolationType.None;
        public UInt16 GlobalSequenceID = 65535;
        private List<ModelTrackSequenceTimestamps> Timestamps = new List<ModelTrackSequenceTimestamps>();
        private List<ModelTrackSequenceValues<T>> Values = new List<ModelTrackSequenceValues<T>>();
        public UInt32 DataOffset = 0;

        /// <summary>
        /// Adds a new track sequence, and returns the index
        /// </summary>
        public int AddSequence()
        {
            Timestamps.Add(new ModelTrackSequenceTimestamps());
            Values.Add(new ModelTrackSequenceValues<T>());
            return Timestamps.Count - 1;
        }

        /// <summary>
        /// Adds a value to the pair related for a sequence
        /// </summary>
        public void AddValueToSequence(int sequenceID, UInt32 timestamp, T value)
        {
            if (sequenceID >= Timestamps.Count)
            {
                Logger.WriteLine("ERROR AddValueToSequence out of range exception!");
                return;
            }
            Timestamps[sequenceID].AddTimestamp(timestamp);
            Values[sequenceID].AddValue(value);
        }

        public UInt32 GetHeaderSize()
        {
            UInt32 size = 0;
            size += 2;  // InterpolationType
            size += 2;  // GlobalSequenceID
            size += 4;  // Number of timestamps
            size += 4;  // Offset of timestamps
            size += 4;  // Number of values
            size += 4;  // Offset of values
            return size;
        }

        public List<byte> GetHeaderBytes()
        {
            // Calculate the offset for the timestamps and values
            UInt32 timestampOffset = DataOffset;
            UInt32 valuesDatOffset = timestampOffset;
            foreach (ModelTrackSequenceTimestamps timeStamps in Timestamps)
            {
                valuesDatOffset += timeStamps.GetHeaderSize();
                valuesDatOffset += timeStamps.GetDataSize();
            }

            // Write the data
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(InterpolationType)));
            bytes.AddRange(BitConverter.GetBytes(GlobalSequenceID));
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(Timestamps.Count)));
            bytes.AddRange(BitConverter.GetBytes(timestampOffset));
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(Values.Count)));
            bytes.AddRange(BitConverter.GetBytes(valuesDatOffset));
            return bytes;
        }

        public UInt32 GetByteSize()
        {
            UInt32 size = 0;
            size += GetHeaderSize();
            foreach (ModelTrackSequenceTimestamps timeStamps in Timestamps)
            {
                size += timeStamps.GetHeaderSize();
                size += timeStamps.GetDataSize();
            }
            foreach (ModelTrackSequenceValues<T> values in Values)
            {
                size += values.GetHeaderSize();
                size += values.GetDataSize();
            }
            return size;
        }

        public List<byte> GetDataBytes()
        {
            // Structure: [Timestamp Headers][Timestamp Data][Value Headers][Value Data]

            // Calculate the offsets
            UInt32 timestampHeaderStartOffset = DataOffset;
            UInt32 timestampDataStartOffset = timestampHeaderStartOffset;
            foreach (ModelTrackSequenceTimestamps timeStamps in Timestamps)
                timestampDataStartOffset += timeStamps.GetHeaderSize();
            UInt32 valuesHeaderStartOffset = timestampDataStartOffset;
            foreach (ModelTrackSequenceTimestamps timeStamps in Timestamps)
                valuesHeaderStartOffset += timeStamps.GetDataSize();
            UInt32 valuesDataStartOffset = valuesHeaderStartOffset;
            foreach (ModelTrackSequenceValues<T> values in Values)
                valuesDataStartOffset += values.GetDataSize();

            // Store the offsets
            UInt32 curTimestampDataOffset = timestampDataStartOffset;
            foreach (ModelTrackSequenceTimestamps timeStamps in Timestamps)
            {
                if (timeStamps.Timestamps.Count > 0)
                {
                    timeStamps.DataOffset = curTimestampDataOffset;
                    curTimestampDataOffset += timeStamps.GetDataSize();
                }
            }
            UInt32 curValueDataOffset = valuesDataStartOffset;
            foreach (ModelTrackSequenceValues<T> values in Values)
            {
                if (values.Values.Count > 0)
                {
                    values.DataOffset = curValueDataOffset;
                    curValueDataOffset += values.GetDataSize();
                }
            }

            // Build the byte array            
            List<byte> bytes = new List<byte>();
            foreach (ModelTrackSequenceTimestamps timeStamps in Timestamps)
                bytes.AddRange(timeStamps.GetHeaderBytes());
            foreach (ModelTrackSequenceTimestamps timeStamps in Timestamps)
                if (timeStamps.Timestamps.Count > 0)
                    bytes.AddRange(timeStamps.GetDataBytes());

            foreach (ModelTrackSequenceValues<T> values in Values)
                bytes.AddRange(values.GetHeaderBytes());
            foreach (ModelTrackSequenceValues<T> values in Values)
                if (values.Values.Count > 0)
                    bytes.AddRange(values.GetDataBytes());

            // Return the bytes
            return bytes;
        }
    }
}
