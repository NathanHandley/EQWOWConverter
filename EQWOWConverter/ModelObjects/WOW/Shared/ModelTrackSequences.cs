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

        // Data is arranged as follows: [TimestampHeaders][ValueHeaders][TimestampData][ValueData]
        public void AddDataAndUpdateOffsets(ref List<byte> boneDataSpace, UInt32 dataSpaceStartOffset)
        {
            // Don't add anything if there's no data to add
            if (Timestamps.Count == 0)
                return;

            // Determine first available data offset
            UInt32 firstUnusedBoneDataSpaceOffset = Convert.ToUInt32(boneDataSpace.Count) + dataSpaceStartOffset;
            // Get the size of the header data to know where the start of timestampdata can begin
            UInt32 sizeOfHeaderData = 0;
            TimestampsOffset = firstUnusedBoneDataSpaceOffset;
            foreach (ModelTrackSequenceTimestamps sequenceTimestamps in Timestamps)
                sizeOfHeaderData += sequenceTimestamps.GetHeaderSize();
            ValuesOffset = TimestampsOffset + sizeOfHeaderData;
            foreach (ModelTrackSequenceValues<T> sequenceValues in Values)
                sizeOfHeaderData += sequenceValues.GetHeaderSize();
            UInt32 curUnusedDataBytesStartOffset = firstUnusedBoneDataSpaceOffset + sizeOfHeaderData;

            // Collect timestamp data bytes and update timestamp header offsets accordingly
            List<byte> timestampDataBytes = new List<byte>();
            foreach (ModelTrackSequenceTimestamps sequenceTimestamps in Timestamps)
            {
                sequenceTimestamps.DataOffset = curUnusedDataBytesStartOffset;
                timestampDataBytes.AddRange(sequenceTimestamps.GetDataBytes());
                curUnusedDataBytesStartOffset += sequenceTimestamps.GetDataSize();
            }

            // Collect timestamp headers
            List<byte> timestampHeaderBytes = new List<byte>();
            foreach (ModelTrackSequenceTimestamps sequenceTimestamps in Timestamps)
                timestampHeaderBytes.AddRange(sequenceTimestamps.GetHeaderBytes());

            // Collect value data bytes and update timestamp header offsets accordingly
            List<byte> valueDataBytes = new List<byte>();
            foreach (ModelTrackSequenceValues<T> sequenceValues in Values)
            {
                sequenceValues.DataOffset = curUnusedDataBytesStartOffset;
                valueDataBytes.AddRange(sequenceValues.GetDataBytes());
                curUnusedDataBytesStartOffset += sequenceValues.GetDataSize();
            }

            // Collect value headers
            List<byte> valueHeaderBytes = new List<byte>();
            foreach (ModelTrackSequenceValues<T> sequenceValues in Values)
                timestampHeaderBytes.AddRange(sequenceValues.GetHeaderBytes());

            // Combine the data together and return it
            boneDataSpace.AddRange(timestampHeaderBytes);
            boneDataSpace.AddRange(valueHeaderBytes);
            boneDataSpace.AddRange(timestampDataBytes);
            boneDataSpace.AddRange(valueDataBytes);
        }
    }
}
