﻿//  Author: Nathan Handley (nathanhandley@protonmail.com)
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

namespace EQWOWConverter.ObjectModels
{
    internal class ObjectModelTrackSequenceValues<T> where T : IByteSerializable
    {
        public List<T> Values = new List<T>();
        public UInt32 DataOffset = 0;

        public void AddValue(T value)
        {
            Values.Add(value);
        }

        public UInt32 GetHeaderSize()
        {
            UInt32 size = 0;
            size += 4; // Number of elements
            size += 4; // Data offset
            return size;
        }

        public List<Byte> GetHeaderBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(Values.Count)));
            bytes.AddRange(BitConverter.GetBytes(DataOffset));
            return bytes;
        }

        public List<Byte> GetDataBytes()
        {
            if (Values.Count == 0)
                return new List<byte>();
            else
            {
                // Allocate an estimated size
                List<Byte> bytes = new List<Byte>(Convert.ToInt32(Values.Count * Values[0].GetBytesSize()));
                foreach (T value in Values)
                    bytes.AddRange(value.ToBytes());
                return bytes;
            }
        }
    }
}
