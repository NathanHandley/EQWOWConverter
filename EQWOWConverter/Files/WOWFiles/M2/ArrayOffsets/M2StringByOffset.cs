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

using System.Text;

namespace EQWOWConverter.WOWFiles
{
    internal class M2StringByOffset : IOffsetByteSerializable
    {
        private UInt32 Length = 0;
        private UInt32 Offset = 0;
        public string StringValue;

        public M2StringByOffset(string stringValue)
        {
            StringValue = stringValue + "\0";
            Length = Convert.ToUInt32(StringValue.Length);
        }

        public void AddToByteBuffer(ref List<byte> byteBuffer)
        {
            Offset = Convert.ToUInt32(byteBuffer.Count);
            byteBuffer.AddRange(Encoding.ASCII.GetBytes(StringValue));
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
            returnBytes.AddRange(BitConverter.GetBytes(Length));
            returnBytes.AddRange(BitConverter.GetBytes(Offset));
            return returnBytes;
        }

        public void AddDataBytes(ref List<byte> byteBuffer)
        {
            Offset = Convert.ToUInt32(byteBuffer.Count);
            byteBuffer.AddRange(Encoding.ASCII.GetBytes(StringValue));
        }
    }
}
