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

namespace EQWOWConverter.WOWFiles
{
    internal class M2SkinHeader
    {
        private string TokenMagic = "SKIN";
        public M2SkinHeaderElement Indices = new M2SkinHeaderElement();
        public M2SkinHeaderElement TriangleIndices = new M2SkinHeaderElement(); // This will be 3x the number of triangle records
        public M2SkinHeaderElement BoneIndices = new M2SkinHeaderElement(); // I've also seen this as 'properties' for some reason.  Relates to Vertices
        public M2SkinHeaderElement SubMeshes = new M2SkinHeaderElement();
        public M2SkinHeaderElement TextureUnits = new M2SkinHeaderElement(); // "Batches"?
        private UInt32 BoneCountMax = 21;   // Values seem to be 21, 53, 64, 256

        public List<byte> ToBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(Encoding.ASCII.GetBytes(TokenMagic));
            bytes.AddRange(Indices.ToBytes());
            bytes.AddRange(TriangleIndices.ToBytes());
            bytes.AddRange(BoneIndices.ToBytes());
            bytes.AddRange(SubMeshes.ToBytes());
            bytes.AddRange(TextureUnits.ToBytes());
            bytes.AddRange(BitConverter.GetBytes(BoneCountMax));
            return bytes;
        }

        public int GetSize()
        {
            int size = 0;
            size += 4;  // TokenMagic
            size += 8;  // Indices
            size += 8;  // Triangles
            size += 8;  // BoneIndices
            size += 8;  // SubMeshes
            size += 8;  // TextureUnits
            size += 4;  // BoneCountMax
            return size;
        }
    }
}
