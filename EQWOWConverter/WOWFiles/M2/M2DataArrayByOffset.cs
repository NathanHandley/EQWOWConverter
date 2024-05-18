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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal class M2DataArrayByOffset<T> where T : ByteSerializable
    {
        private UInt32 Count = 0;
        private UInt32 Offset = 0;
        private List<T> Data = new List<T>();

        public void AddDataElement(T dataElement)
        {
            Data.Add(dataElement);
        }

        public void AddDataArray(List<T> dataArray)
        {
            Data = dataArray;
        }

        public List<Byte> GetHeaderBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Count));
            if (Count == 0)
                bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
            else
                bytes.AddRange(BitConverter.GetBytes(Offset));
            return bytes;
        }

        public void AddDataToByteBufferAndUpdateHeader(ref UInt32 workingCursorOffset, ref List<Byte> byteBuffer)
        {
            // Update header values
            Offset = workingCursorOffset;
            Count =  Convert.ToUInt32(Data.Count);

            // Generate the data block
            List<byte> dataBytes = new List<byte>();
            foreach (T t in Data)
                dataBytes.AddRange(t.ToBytes());
            byteBuffer.AddRange(dataBytes);
            
            // Update cursor
            workingCursorOffset += Convert.ToUInt32(dataBytes.Count);
        }
    }
}
