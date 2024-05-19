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

using EQWOWConverter.ModelObjects;
using EQWOWConverter.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal class M2TextureArrayByOffset : IOffsetByteSerializable
    {
        private List<M2Texture> Textures = new List<M2Texture>();
        private string TextureFolder;
        private UInt32 Count = 0;
        private UInt32 Offset = 0;

        public M2TextureArrayByOffset(string textureFolder)
        {
            TextureFolder = textureFolder;
        }

        public void AddModelTextures(List<ModelTexture> modelTextures)
        {
            foreach(ModelTexture texture in modelTextures)
            {
                Textures.Add(new M2Texture(texture, TextureFolder));
            }
            Count = Convert.ToUInt32(Textures.Count);
        }

        public List<Byte> GetHeaderBytes()
        {
            List<byte> returnBytes = new List<byte>();
            returnBytes.AddRange(BitConverter.GetBytes(Count));
            returnBytes.AddRange(BitConverter.GetBytes(Offset));
            return returnBytes;
        }

        public void AddDataBytes(ref List<byte> byteBuffer)
        {
            Offset = Convert.ToUInt32(byteBuffer.Count);

            // Calculate space just for the metadata/headers
            UInt32 totalSubHeaderSpaceNeeded = 0;
            foreach (M2Texture texture in Textures)
                totalSubHeaderSpaceNeeded += texture.GetHeaderSize();

            // Generate the content sections (strings)
            UInt32 curOffset = Convert.ToUInt32(byteBuffer.Count + totalSubHeaderSpaceNeeded);
            List<byte> dataBytes = new List<byte>();
            foreach (M2Texture texture in Textures)
            {
                texture.FileNameOffset = curOffset;
                dataBytes.AddRange(Encoding.ASCII.GetBytes(texture.FullTexturePath.ToUpper()));
                curOffset += texture.FileNameLength;
            }

            // Generate the headers
            List<byte> headerBytes = new List<byte>();
            foreach (M2Texture texture in Textures)
                headerBytes.AddRange(texture.GetHeaderBytes());

            // Combine them
            byteBuffer.AddRange(headerBytes);
            byteBuffer.AddRange(dataBytes);
        }
    }
}
