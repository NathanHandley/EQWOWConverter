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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Common
{
    internal class BSPNode
    {
        public BSPNodeFlag Flags;
        public Int16 ChildANodeIndex = -1; // Right side, negative
        public Int16 ChildBNodeIndex = -1; // Left side, positive
        public UInt16 NumFaces = 0; // Number of faces (found in WMO MOBR)
        public UInt32 FaceStartIndex = 0; // first triangle index (found in WMO MOBR)
        public float PlaneDistance = 0;
        public int Depth;

        // Related to tree generation
        public bool TreeGenHaveMoreToProcess = false;
        public BoundingBox TreeGenBoundingBox = new BoundingBox();
        public List<UInt32> TreeGenFaceIndices = new List<UInt32>();

        public BSPNode(int depth)
        {
            Depth = depth;
        }

        public BSPNode(bool haveMoreToProcess, BoundingBox boundingBox, List<UInt32> faceIndices, int depth)
        {
            TreeGenHaveMoreToProcess = haveMoreToProcess;
            TreeGenFaceIndices = new List<uint>(faceIndices);
            TreeGenBoundingBox = new BoundingBox(boundingBox);
            Depth = depth;
        }

        public void SetValues(BSPNodeFlag singleFlag, short childANodeIndex, short childBNodeIndex, ushort numFaces, uint faceStartIndex, float planeDistance)
        {
            Flags = singleFlag;
            ChildANodeIndex = childANodeIndex;
            ChildBNodeIndex = childBNodeIndex;
            NumFaces = numFaces;
            FaceStartIndex = faceStartIndex;
            PlaneDistance = planeDistance;
        }

        public void ClearTreeGenData()
        {
            TreeGenHaveMoreToProcess = false;
            TreeGenBoundingBox = new BoundingBox();
            TreeGenFaceIndices.Clear();
        }

        public List<byte> ToBytes()
        {
            List<byte> returnBytes = new List<byte>();
            returnBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(Flags)));
            returnBytes.AddRange(BitConverter.GetBytes(ChildANodeIndex));
            returnBytes.AddRange(BitConverter.GetBytes(ChildBNodeIndex));
            returnBytes.AddRange(BitConverter.GetBytes(NumFaces));
            returnBytes.AddRange(BitConverter.GetBytes(FaceStartIndex));
            returnBytes.AddRange(BitConverter.GetBytes(PlaneDistance));
            return returnBytes;
        }
    }
}
