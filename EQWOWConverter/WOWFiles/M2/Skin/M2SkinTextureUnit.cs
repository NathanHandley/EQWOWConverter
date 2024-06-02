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
    internal class M2SkinTextureUnit
    {
        public byte Flags = Convert.ToByte(M2SkinTextureUnitFlags.Static);
        public sbyte PriorityPlane = 0; // ?
        public UInt16 ShaderID = 0;
        public UInt16 SkinSectionIndex = 0;
        public UInt16 GeosetIndex = 0;
        public Int16 ColorIndex = -1;
        public UInt16 MaterialIndex = 0;
        public UInt16 MaterialLayer = 0; // Caps at 7
        public UInt16 TextureCount = 1;
        public UInt16 TextureLookupIndex = 0;
        public UInt16 TextureMappingLookupIndex = 0;
        public UInt16 TextureTransparencyLookupIndex = 0;
        public UInt16 TextureTransformationsLookupIndex = 0;
        
        public M2SkinTextureUnit(UInt16 subMeshID, UInt16 materialIndex, UInt16 textureLookupIndex)
        {
            MaterialIndex = materialIndex;
            TextureLookupIndex = textureLookupIndex;
            SkinSectionIndex = subMeshID;
            GeosetIndex = subMeshID; // I'm not 100% sure why this is the same as submeshID, but data in WOTLK skins seem to do that
        }

        public List<byte> ToBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(Flags);
            bytes.Add(Convert.ToByte(PriorityPlane));
            bytes.AddRange(BitConverter.GetBytes(ShaderID));
            bytes.AddRange(BitConverter.GetBytes(SkinSectionIndex));
            bytes.AddRange(BitConverter.GetBytes(GeosetIndex));
            bytes.AddRange(BitConverter.GetBytes(ColorIndex));
            bytes.AddRange(BitConverter.GetBytes(MaterialIndex));
            bytes.AddRange(BitConverter.GetBytes(MaterialLayer));
            bytes.AddRange(BitConverter.GetBytes(TextureCount));
            bytes.AddRange(BitConverter.GetBytes(TextureLookupIndex));
            bytes.AddRange(BitConverter.GetBytes(TextureMappingLookupIndex));
            bytes.AddRange(BitConverter.GetBytes(TextureTransparencyLookupIndex));
            bytes.AddRange(BitConverter.GetBytes(TextureTransformationsLookupIndex));
            return bytes;
        }
    }
}
