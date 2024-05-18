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
        public UInt32 FileNameLength = 0;
        public UInt32 FileNameOffset = 0;

        public M2Texture(ModelTexture texture, string textureFolder)
        {
            Texture = texture;
            FullTexturePath = Path.Combine(textureFolder, texture.TextureName + ".blp\0");
            FileNameLength = Convert.ToUInt32(FullTexturePath.Length);
        }

        public UInt32 GetHeaderSize()
        {
            UInt32 size = 0;
            size += 4;  // Type
            size += 4;  // WrapType
            size += 4;  // Texture Name Length
            size += 4;  // Texture Offset
            return size;
        }

        public List<byte> GetHeaderBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(Texture.Type)));
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(Texture.WrapType)));
            bytes.AddRange(BitConverter.GetBytes(FileNameLength));
            bytes.AddRange(BitConverter.GetBytes(FileNameOffset));
            return bytes;
        }

        public List<Byte> GetDataBytes(ref UInt32 curOffset)
        {
            List<byte> bytes = new List<byte>();


            return bytes;
        }

        //public List<byte> ToBytes()
        //{
        //    List<byte> bytes = new List<byte>();
        //    string fullPath = GenerateFullFileNameAndPath(modelTextureFolder) + "\0";
        //    bytes.AddRange();
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
