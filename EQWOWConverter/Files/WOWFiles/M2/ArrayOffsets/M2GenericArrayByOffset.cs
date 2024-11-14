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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EQWOWConverter.WOWFiles
{
    internal class M2GenericArrayByOffset<T> : IOffsetByteSerializable where T : IByteSerializable
    {
        private UInt32 Count = 0;
        private UInt32 Offset = 0;
        public List<T> Elements = new List<T>();

        public void Add(T element)
        {
            Elements.Add(element);
            Count = Convert.ToUInt32(Elements.Count);
        }

        public void AddArray(List<T> elementArray)
        {
            for (int i = 0; i < elementArray.Count; i++)
                Add(elementArray[i]);
        }

        public void SetElementValue(int elementIndex, T value)
        {
            if (elementIndex >= Elements.Count)
                Logger.WriteError("Attempted to set an element in a M2GenericArrayByOffset that wasn't in the array");
            else
                Elements[elementIndex] = value;
        }

        public UInt32 GetHeaderSize()
        {
            UInt32 size = 4; // Count
            size += 4; // Offset
            return size;
        }

        public List<Byte> GetHeaderBytes()
        {
            List<byte> returnBytes = new List<byte>();
            returnBytes.AddRange(BitConverter.GetBytes(Count));
            returnBytes.AddRange(BitConverter.GetBytes(Offset));
            return returnBytes;
        }

        public void AddDataBytes(ref List<byte> byteBuffer)
        {
            if (Count == 0)
                return;
            Offset = Convert.ToUInt32(byteBuffer.Count);
            for (int i = 0; i < Elements.Count; ++i)
                byteBuffer.AddRange(Elements[i].ToBytes());
        }
    }
}
