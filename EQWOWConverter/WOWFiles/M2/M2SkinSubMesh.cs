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
using EQWOWConverter.Common;
using EQWOWConverter.ModelObjects;
using EQWOWConverter.Objects;

namespace EQWOWConverter.WOWFiles
{
    internal class M2SkinSubMesh
    {
        public UInt32 SkinSectionID = 0; // There's actually much more to this, but 0 will probably work for this project
        // I saw reference to "Level" in a few places, but I don't see it in memory analysis of a .skin file such as CaveMineSpiderPillar0100.skin
        public UInt16 VertexStart = 0;
        public UInt16 VertexCount = 0;
        public UInt16 StartTriangleIndex = 0;
        public UInt16 TriangleCount = 0;
        public UInt16 BoneCount = 1;
        public UInt16 BoneLookupIndex = 0;
        public UInt16 NumOfBonesInfluencing = 1; // Max number of bones needed at one time?  How does this differ from Bone Count?
        public Vector3 AverageVertexCenterPosition = new Vector3(); // Average position between all verts
        public Vector3 BoundingBoxCenterPosition = new Vector3(); // Center point from a bounding box wrapped around the verticies
        public float BoundingBoxFurthestVertexDistanceFromCenter = 0;   // Probably too long of a name, but I'll forget it otherwise
    
        public M2SkinSubMesh(WOWObjectModelData objectModelData)
        {
            VertexCount = Convert.ToUInt16(objectModelData.ModelVerticies.Count);
            TriangleCount = Convert.ToUInt16(objectModelData.ModelTriangles.Count);

            // Calculate the average vertex center position
            float totalX = 0;
            float totalY = 0;
            float totalZ = 0;
            foreach(ModelVertex vertex in objectModelData.ModelVerticies)
            {
                totalX += vertex.Position.X;
                totalY += vertex.Position.Y;
                totalZ += vertex.Position.Z;
            }
            AverageVertexCenterPosition.X = totalX / objectModelData.ModelVerticies.Count;
            AverageVertexCenterPosition.Y = totalY / objectModelData.ModelVerticies.Count;
            AverageVertexCenterPosition.Z = totalZ / objectModelData.ModelVerticies.Count;

            // Bounding box
            BoundingBoxCenterPosition = objectModelData.BoundingBox.GetCenter();
            BoundingBoxFurthestVertexDistanceFromCenter = objectModelData.BoundingBox.FurthestPointDistanceFromCenter();

            // TODO: Add specifics around bone counts for animated
        }

        public List<byte> ToBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(SkinSectionID));
            bytes.AddRange(BitConverter.GetBytes(VertexStart));
            bytes.AddRange(BitConverter.GetBytes(VertexCount));
            bytes.AddRange(BitConverter.GetBytes(StartTriangleIndex));
            bytes.AddRange(BitConverter.GetBytes(TriangleCount));
            bytes.AddRange(BitConverter.GetBytes(BoneCount));
            bytes.AddRange(BitConverter.GetBytes(BoneLookupIndex));
            bytes.AddRange(BitConverter.GetBytes(NumOfBonesInfluencing));
            bytes.AddRange(AverageVertexCenterPosition.ToBytes());
            bytes.AddRange(BoundingBoxCenterPosition.ToBytes());
            bytes.AddRange(BitConverter.GetBytes(BoundingBoxFurthestVertexDistanceFromCenter));
            return bytes;
        }
    }
}
