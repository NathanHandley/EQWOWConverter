//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2024 Nathan Handley
//  
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
        public Int16 NegChild; // Right side, negative
        public Int16 PosChild; // Left side, positive
        public UInt16 NumFaces; // Number of faces (found in WMO MOBR)
        public UInt32 FaceStartIndex; // first triangle (found in WMO MOBR)
        public float PlaneDistance;

        public BSPNode() { }
        public BSPNode(BSPNodeFlag singleFlag, short negChild, short posChild, ushort numFaces, uint faceStartIndex, float planeDistance)
        {
            Flags = singleFlag;
            NegChild = negChild;
            PosChild = posChild;
            NumFaces = numFaces;
            FaceStartIndex = faceStartIndex;
            PlaneDistance = planeDistance;
        }

        public List<byte> ToBytes()
        {
            List<byte> returnBytes = new List<byte>();
            returnBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(Flags)));
            returnBytes.AddRange(BitConverter.GetBytes(NegChild));
            returnBytes.AddRange(BitConverter.GetBytes(PosChild));
            returnBytes.AddRange(BitConverter.GetBytes(NumFaces));
            returnBytes.AddRange(BitConverter.GetBytes(FaceStartIndex));
            returnBytes.AddRange(BitConverter.GetBytes(PlaneDistance));
            return returnBytes;
        }
    }
}
