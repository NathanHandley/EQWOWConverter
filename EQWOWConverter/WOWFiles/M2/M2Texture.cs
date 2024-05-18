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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal class M2Texture
    {
        public ModelTexture Texture;
        public string FullTexturePath;

        public M2Texture(ModelTexture texture, string textureFolder)
        {
            Texture = texture;
            FullTexturePath = Path.Combine(textureFolder, texture.TextureName + ".blp\0");
        }

        //public List<byte> ToBytes()
        //{
        //    List<byte> bytes = new List<byte>();
        //    string fullPath = GenerateFullFileNameAndPath(modelTextureFolder) + "\0";
        //    bytes.AddRange(Encoding.ASCII.GetBytes(fullPath.ToUpper()));
        //    FileNameLength = Convert.ToUInt32(bytes.Count);
        //    FileNameOffset = Convert.ToUInt32(curOffset);
        //    curOffset += bytes.Count;
        //    return bytes;
        //}

        //public void AddToByteBuffer(ref List<byte> byteBuffer)
        //{
        //    byteBuffer.AddRange(Position.ToBytes());
        //    byteBuffer.AddRange(BoneWeights.ToArray());
        //    byteBuffer.AddRange(BoneIndicies.ToArray());
        //    byteBuffer.AddRange(Normal.ToBytes());
        //    byteBuffer.AddRange(Texture1TextureCoordinates.ToBytes());
        //    byteBuffer.AddRange(Texture2TextureCoordinates.ToBytes());
        //}
    }
}
