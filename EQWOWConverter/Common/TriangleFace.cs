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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Common
{
    internal class TriangleFace : IByteSerializable
    {
        public int MaterialIndex;
        public int V1;
        public int V2;
        public int V3;

        public TriangleFace()
        { }

        public TriangleFace(TriangleFace face)
        {
            MaterialIndex = face.MaterialIndex;
            V1 = face.V1;
            V2 = face.V2;
            V3 = face.V3;
        }

        public UInt16 GetSmallestIndex()
        {
            int smallestIndex = V1;
            if (V2 < smallestIndex)
                smallestIndex = V2;
            if (V3 < smallestIndex)
                smallestIndex = V3;
            return Convert.ToUInt16(smallestIndex);
        }

        public UInt16 GetLargestIndex()
        {
            int largestIndex = V1;
            if (V2 > largestIndex)
                largestIndex = V2;
            if (V3 > largestIndex)
                largestIndex = V3;
            return Convert.ToUInt16(largestIndex);
        }

        public UInt32 GetBytesSize()
        {
            return 6; // 2 bytes per index, 3 indicies
        }

        public List<byte> ToBytes()
        {
            List<byte> returnBytes = new List<byte>();
            returnBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(V1)));
            returnBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(V2)));
            returnBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(V3)));
            return returnBytes;
        }

        public bool ContainsIndex(int index)
        {
            if (V1 == index || V2 == index || V3 == index)
                return true;
            return false;
        }
    }
}
