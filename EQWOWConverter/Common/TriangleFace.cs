﻿//  Author: Nathan Handley (nathanhandley@protonmail.com)
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
    internal class TriangleFace : IComparable, IEquatable<TriangleFace>
    {
        public int MaterialIndex;
        public int V1;
        public int V2;
        public int V3;

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

        public bool ContainsIndex(SortedSet<int> indicies)
        {
            foreach (int i in indicies)
                if (ContainsIndex(i) == true)
                    return true;

            return false;
        }

        public bool SharesIndexWith(TriangleFace otherFace)
        {
            if (V1 == otherFace.V1 || V1 == otherFace.V2 || V1 == otherFace.V3)
                return true;
            if (V2 == otherFace.V1 || V2 == otherFace.V2 || V2 == otherFace.V3)
                return true;
            if (V3 == otherFace.V1 || V3 == otherFace.V2 || V3 == otherFace.V3)
                return true;

            return false;
        }

        public int CompareTo(object? obj)
        {
            if (obj == null) return 1;
            TriangleFace? otherTriangle = obj as TriangleFace;
            if (otherTriangle != null)
                return this.MaterialIndex.CompareTo(otherTriangle.MaterialIndex);
            else
                throw new ArgumentException("Object is not a TriangleFace");
        }

        public bool Equals(TriangleFace other)
        {
            if (V1 != other.V1) return false;
            if (V2 != other.V2) return false;
            if (V3 != other.V3) return false;
            if (MaterialIndex != other.MaterialIndex) return false;
            return true;
        }
    }
}