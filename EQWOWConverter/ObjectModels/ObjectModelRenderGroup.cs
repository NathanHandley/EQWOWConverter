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

using EQWOWConverter.Common;

namespace EQWOWConverter.ObjectModels
{
    internal class ObjectModelRenderGroup
    {
        public List<UInt16> BoneLookupIndices = new List<UInt16>();
        public UInt16 VertexStart = UInt16.MaxValue;
        public UInt16 VertexCount = 0;
        public UInt16 TriangleStart = UInt16.MaxValue;
        public UInt16 TriangleCount = 0;
        public UInt16 RootBone = 0;
        public UInt16 MaterialIndex = 0;
        public List<ObjectModelVertex> Vertices = new List<ObjectModelVertex>();
        public HashSet<int> VertexIndicies = new HashSet<int>();

        public ObjectModelRenderGroup()
        {

        }

        public ObjectModelRenderGroup(MeshData.MeshRenderGroup meshRenderGroup)
        {
            VertexStart = Convert.ToUInt16(meshRenderGroup.VertexStart);
            VertexCount = Convert.ToUInt16(meshRenderGroup.VertexCount);
            TriangleStart = Convert.ToUInt16(meshRenderGroup.TriangleStart);
            TriangleCount = Convert.ToUInt16(meshRenderGroup.TriangleCount);
        }
    }
}
