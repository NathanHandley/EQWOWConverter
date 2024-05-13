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

using EQWOWConverter.WOWFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.ModelObjects
{
    internal class ModelTexture
    {
        public ModelTextureType Type = ModelTextureType.Hardcoded;
        public ModelTextureWrapType WrapType = ModelTextureWrapType.None;
        public string TextureName = string.Empty;
        private UInt32 FileNameLength = 0;
        private UInt32 FileNameOffset = 0;

        public ModelTexture()
        {
            // TESTING ////////////////////////////////////////////////
            Type = ModelTextureType.CreatureSkin1;
            WrapType = ModelTextureWrapType.XY;
            TextureName = "SomeTestingTextureName";
            ////////////////////////////////////////////////////////////
        }

        private string GenerateFullFileNameAndPath(string modelTextureFolder)
        {
            string fullFilePath = Path.Combine(modelTextureFolder, TextureName + ".blp");
            return fullFilePath;
        }

        public int GetMetaSize()
        {
            int size = 0;
            size += 4;  // Type
            size += 4;  // WrapType
            size += 4;  // Texture Name Length
            size += 4;  // Texture Offset
            return size;
        }

        public List<byte> ToBytesMeta()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(Type)));
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(WrapType)));
            bytes.AddRange(BitConverter.GetBytes(FileNameLength));
            bytes.AddRange(BitConverter.GetBytes(FileNameOffset));
            return bytes;
        }

        public List<byte> ToBytesData(string modelTextureFolder, ref int curOffset)
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(Encoding.ASCII.GetBytes(GenerateFullFileNameAndPath(modelTextureFolder) + "\0"));
            FileNameLength = Convert.ToUInt32(bytes.Count);
            FileNameOffset = Convert.ToUInt32(curOffset);
            curOffset += bytes.Count;            
            return bytes;
        }
    }
}
