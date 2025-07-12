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

namespace EQWOWConverter.WOWFiles
{
    internal class M2TrackSequencesSimple<T> where T : IByteSerializable
    {
        public UInt32 TimestampsOffset = 0;
        public List<UInt16> Timestamps = new List<UInt16>();
        public UInt32 ValuesOffset = 0;
        public List<T> Values = new List<T>();

        public void AddTimeStep(UInt16 timestamp, T value)
        {
            Timestamps.Add(timestamp);
            Values.Add(value);
        }

        public UInt32 GetHeaderSize()
        {
            UInt32 size = 0;
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
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(Timestamps.Count)));
            bytes.AddRange(BitConverter.GetBytes(TimestampsOffset));
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(Values.Count)));
            bytes.AddRange(BitConverter.GetBytes(ValuesOffset));
            return bytes;
        }

        public void AddDataBytes(ref List<byte> byteBuffer)
        {
            // Don't add anything if there's no data to add
            if (Timestamps.Count == 0)
                return;

            // Add timestep data
            TimestampsOffset = Convert.ToUInt32(byteBuffer.Count);
            foreach (UInt16 timestamp in Timestamps)
                byteBuffer.AddRange(BitConverter.GetBytes(timestamp));
            
            // Align memory
            AddBytesToAlign(ref byteBuffer, 16);

            // Add value data
            ValuesOffset = Convert.ToUInt32(byteBuffer.Count);
            foreach(T value in Values)
                byteBuffer.AddRange(value.ToBytes());

            // Align memory
            AddBytesToAlign(ref byteBuffer, 16);
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
