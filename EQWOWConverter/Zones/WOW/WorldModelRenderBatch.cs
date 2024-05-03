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

namespace EQWOWConverter.Zones
{
    internal class WorldModelRenderBatch
    {
        public AxisAlignedBoxLR BoundingBoxLowRes = new AxisAlignedBoxLR();
        public byte MaterialIndex = 0;
        public UInt32 FirstTriangleFaceIndex = 0;
        public UInt16 NumOfFaceIndicies = 0;
        public UInt16 FirstVertexIndex = 0;
        public UInt16 LastVertexIndex = 0;

        public WorldModelRenderBatch(byte materialIndex, uint firstTriangleFaceIndex, ushort numOfFaceIndicies, 
            ushort firstVertexIndex, ushort lastVertexIndex, List<Vector3> worldModelObjectVerticies)
        {
            MaterialIndex = materialIndex;
            FirstTriangleFaceIndex = firstTriangleFaceIndex;
            NumOfFaceIndicies = numOfFaceIndicies;
            FirstVertexIndex = firstVertexIndex;
            LastVertexIndex = lastVertexIndex;
            CalculateBoundingBox(worldModelObjectVerticies);
        }

        private void CalculateBoundingBox(List<Vector3> verticies)
        {
            AxisAlignedBox boundingBox = new AxisAlignedBox();
            foreach (Vector3 renderVert in verticies)
            {
                if (renderVert.X < boundingBox.BottomCorner.X)
                    boundingBox.BottomCorner.X = renderVert.X;
                if (renderVert.Y < boundingBox.BottomCorner.Y)
                    boundingBox.BottomCorner.Y = renderVert.Y;
                if (renderVert.Z < boundingBox.BottomCorner.Z)
                    boundingBox.BottomCorner.Z = renderVert.Z;

                if (renderVert.X > boundingBox.TopCorner.X)
                    boundingBox.TopCorner.X = renderVert.X;
                if (renderVert.Y > boundingBox.TopCorner.Y)
                    boundingBox.TopCorner.Y = renderVert.Y;
                if (renderVert.Z > boundingBox.TopCorner.Z)
                    boundingBox.TopCorner.Z = renderVert.Z;
            }
            BoundingBoxLowRes = new AxisAlignedBoxLR(boundingBox);
        }
    }
}
