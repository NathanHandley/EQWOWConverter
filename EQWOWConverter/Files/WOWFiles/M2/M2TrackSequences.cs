//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2025 Nathan Handley
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
using EQWOWConverter.ObjectModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal class M2TrackSequences<T> where T : IByteSerializable
    {
        public ObjectModelTrackSequences<T> TrackSequences;
        public UInt32 TimestampsOffset = 0;
        public UInt32 ValuesOffset = 0;

        public M2TrackSequences()
        {
            TrackSequences = new ObjectModelTrackSequences<T>();
        }

        public M2TrackSequences(ObjectModelTrackSequences<T> trackSequences)
        {
            TrackSequences = trackSequences;
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
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(TrackSequences.InterpolationType)));
            bytes.AddRange(BitConverter.GetBytes(TrackSequences.GlobalSequenceID));
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(TrackSequences.Timestamps.Count)));
            bytes.AddRange(BitConverter.GetBytes(TimestampsOffset));
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(TrackSequences.Values.Count)));
            bytes.AddRange(BitConverter.GetBytes(ValuesOffset));
            return bytes;
        }

        public void AddDataBytes(ref List<byte> byteBuffer)
        {
            // Don't add anything if there's no data to add
            if (TrackSequences.Timestamps.Count == 0)
                return;

            // Set space aside for timestamp headers
            TimestampsOffset = Convert.ToUInt32(byteBuffer.Count);
            UInt32 timestampHeaderBlockSize = 0;
            foreach (ObjectModelTrackSequenceTimestamps timestamp in TrackSequences.Timestamps)
                timestampHeaderBlockSize += timestamp.GetHeaderSize();

            // Set space aside for the value headers
            ValuesOffset = TimestampsOffset + timestampHeaderBlockSize;
            UInt32 valueHeaderBlockSize = 0;
            foreach (ObjectModelTrackSequenceValues<T> values in TrackSequences.Values)
                valueHeaderBlockSize += values.GetHeaderSize();

            // Reserve the space for all headers in the byte buffer
            UInt32 totalSubHeaderSpaceToReserve = timestampHeaderBlockSize + valueHeaderBlockSize;
            byteBuffer.AddRange(new byte[totalSubHeaderSpaceToReserve]);

            // Add timestamp data
            foreach (ObjectModelTrackSequenceTimestamps timestamp in TrackSequences.Timestamps)
            {
                timestamp.DataOffset = Convert.ToUInt32(byteBuffer.Count);
                byteBuffer.AddRange(timestamp.GetDataBytes());

                // Align memory
                AddBytesToAlign(ref byteBuffer, 16);
            }

            // Add value data
            foreach (ObjectModelTrackSequenceValues<T> values in TrackSequences.Values)
            {
                values.DataOffset = Convert.ToUInt32(byteBuffer.Count);
                byteBuffer.AddRange(values.GetDataBytes());

                // Align memory
                AddBytesToAlign(ref byteBuffer, 16);
            }

            // Write the track header data
            List<byte> trackHeaderBytes = new List<byte>();
            foreach (ObjectModelTrackSequenceTimestamps timestamp in TrackSequences.Timestamps)
                trackHeaderBytes.AddRange(timestamp.GetHeaderBytes());
            foreach (ObjectModelTrackSequenceValues<T> values in TrackSequences.Values)
                trackHeaderBytes.AddRange(values.GetHeaderBytes());
            for (int i = 0; i < totalSubHeaderSpaceToReserve; i++)
                byteBuffer[i + Convert.ToInt32(TimestampsOffset)] = trackHeaderBytes[i];
        }

        private void AddBytesToAlign(ref List<byte> byteBuffer, int byteAlignMultiplier)
        {
            int bytesToAdd = byteAlignMultiplier - (byteBuffer.Count % byteAlignMultiplier);
            if (bytesToAdd == byteAlignMultiplier)
                return;
            byteBuffer.AddRange(new byte[bytesToAdd]);
        }
    }
}
