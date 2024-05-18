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
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.ModelObjects
{
    internal class ModelTrackSequences<T>
    {
        ModelAnimationInterpolationType InterpolationType = ModelAnimationInterpolationType.None;
        public UInt16 GlobalSequenceID = 65535;
        private List<ModelTrackSequenceTimestamps> Timestamps = new List<ModelTrackSequenceTimestamps>();
        private List<ModelTrackSequenceValues<T>> Values = new List<ModelTrackSequenceValues<T>>();
        public UInt32 TimestampsOffset = 0;
        public UInt32 ValuesOffset = 0;

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

        public UInt32 GetCount()
        {
            return Convert.ToUInt32(Timestamps.Count);
        }

        public UInt32 GetBytesSize()
        {
            UInt32 size = 0;
            size += 2;  // InterpolationType
            size += 2;  // GlobalSequenceID
            foreach (ModelTrackSequenceTimestamps timestamp in Timestamps)
            {
                size += timestamp.GetHeaderSize();
                size += timestamp.GetDataSize();
            }
            foreach (ModelTrackSequenceValues<T> value in Values)
            {
                size += value.GetHeaderSize();
                size += value.GetDataSize();
            }
            size += 4;  // Number of timestamps
            size += 4;  // Offset of timestamps
            size += 4;  // Number of values
            size += 4;  // Offset of values
            return size;
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
            // Write the data
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(InterpolationType)));
            bytes.AddRange(BitConverter.GetBytes(GlobalSequenceID));
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(Timestamps.Count)));
            bytes.AddRange(BitConverter.GetBytes(TimestampsOffset));
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(Values.Count)));
            bytes.AddRange(BitConverter.GetBytes(ValuesOffset));
            return bytes;
        }

        public void AddDataToByteBufferAndUpdateHeader(ref UInt32 workingCursorOffset, ref List<Byte> byteBuffer)
        {
            // Don't add anything if there's no data to add
            if (Timestamps.Count == 0)
                return;

            // Calculate space for timestamp headers
            UInt32 allTimestampHeaderSize = 0;
            foreach (ModelTrackSequenceTimestamps timestampSequence in Timestamps)
                allTimestampHeaderSize += timestampSequence.GetHeaderSize();

            // Calculate space for value headers
            UInt32 allValueHeadersSize = 0;
            foreach (ModelTrackSequenceValues<T> modelTrackSequenceValues in Values)
                allValueHeadersSize += modelTrackSequenceValues.GetHeaderSize();

            // Collect all the data, updating headers along the way
            UInt32 dataCursorOffset = workingCursorOffset + allTimestampHeaderSize + allValueHeadersSize;
            List<byte> allValueAndTimestampDataBytes = new List<byte>();
            foreach (ModelTrackSequenceTimestamps timestampSequence in Timestamps)
            {
                timestampSequence.DataOffset = dataCursorOffset;
                allValueAndTimestampDataBytes.AddRange(timestampSequence.GetDataBytes());
                dataCursorOffset += timestampSequence.GetDataSize();
            }
            foreach (ModelTrackSequenceValues<T> modelTrackSequenceValues in Values)
            {
                modelTrackSequenceValues.DataOffset = dataCursorOffset;
                allValueAndTimestampDataBytes.AddRange(modelTrackSequenceValues.GetDataBytes());
                dataCursorOffset += modelTrackSequenceValues.GetDataSize();
            }

            // Collect all of the timestamp and value headers
            List<byte> allValueAndTimestampSubheaderBytes = new List<byte>();
            foreach (ModelTrackSequenceTimestamps timestampSequence in Timestamps)
                allValueAndTimestampSubheaderBytes.AddRange(timestampSequence.GetHeaderBytes());
            foreach (ModelTrackSequenceValues<T> modelTrackSequenceValues in Values)
                allValueAndTimestampSubheaderBytes.AddRange(modelTrackSequenceValues.GetHeaderBytes());

            // Stick data together and update the offset
            byteBuffer.AddRange(allValueAndTimestampSubheaderBytes);
            byteBuffer.AddRange(allValueAndTimestampDataBytes);
            workingCursorOffset += Convert.ToUInt32(allValueAndTimestampSubheaderBytes.Count + allValueAndTimestampDataBytes.Count);
        }

        public List<byte> ToBytes()
        {
            List<byte> bytes = new List<byte>();
            return bytes;
        }
    }
}
