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

namespace EQWOWConverter.WOWFiles
{
    internal class ADTMapChunkInfo
    {
        public UInt32 Offset = 0;
        public UInt32 Size = 0;
        public UInt32 Flags = 0; // Always zero
        public UInt32 AsyncID = 0; // Always zero

        public ADTMapChunkInfo(int offset, int size)
        {
            Offset = Convert.ToUInt32(offset);
            Size = Convert.ToUInt32(size);
        }

        public List<Byte> ToBytes()
        {
            List<Byte> bytes = new List<Byte>();
            bytes.AddRange(BitConverter.GetBytes(Offset));
            bytes.AddRange(BitConverter.GetBytes(Size));
            bytes.AddRange(BitConverter.GetBytes(Flags));
            bytes.AddRange(BitConverter.GetBytes(AsyncID));
            return bytes;
        }
    }
}
