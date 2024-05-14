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

namespace EQWOWConverter.WOWFiles
{
    internal class M2SkinHeader
    {
        private string TokenMagic = "SKIN";
        public M2HeaderElement Indicies = new M2HeaderElement();
        public M2HeaderElement TriangleIndicies = new M2HeaderElement(); // This will be 3x the number of triangle records
        public M2HeaderElement BoneIndicies = new M2HeaderElement(); // I've also seen this as 'properties' for some reason.  Relates to Verticies
        public M2HeaderElement SubMeshes = new M2HeaderElement();
        public M2HeaderElement TextureUnits = new M2HeaderElement(); // "Batches"?
        private UInt32 BoneCountMax = 21;   // Values seem to be 21, 53, 64, 256

        public List<byte> ToBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(Encoding.ASCII.GetBytes(TokenMagic));
            bytes.AddRange(Indicies.ToBytes());
            bytes.AddRange(TriangleIndicies.ToBytes());
            bytes.AddRange(BoneIndicies.ToBytes());
            bytes.AddRange(SubMeshes.ToBytes());
            bytes.AddRange(TextureUnits.ToBytes());
            bytes.AddRange(BitConverter.GetBytes(BoneCountMax));
            return bytes;
        }

        public int GetSize()
        {
            int size = 0;
            size += 4;  // TokenMagic
            size += 8;  // Indicies
            size += 8;  // Triangles
            size += 8;  // BoneIndicies
            size += 8;  // SubMeshes
            size += 8;  // TextureUnits
            size += 4;  // BoneCountMax
            return size;
        }
    }
}
