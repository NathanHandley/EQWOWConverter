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
using EQWOWConverter.WOWFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.ModelObjects
{
    internal class ModelVertex
    {
        public Vector3 Position = new Vector3();
        public List<byte> BoneWeights = new List<byte>(new byte[4]);    // Any more than 4 elements will be ignored
        public List<byte> BoneIndicies = new List<byte>(new byte[4]);   // Any more than 4 elements will be ignored
        public Vector3 Normal = new Vector3();
        public TextureCoordinates Texture1TextureCoordinates = new TextureCoordinates();
        public TextureCoordinates Texture2TextureCoordinates = new TextureCoordinates();

        public ModelVertex()
        {
            // First bone weight is always max by default
            BoneWeights[0] = 255;
        }

        //public UInt32 GetBytesSize()
        //{
        //    UInt32 size = 0;
        //    size += 12;// Position
        //    size += 4; // BoneWeights
        //    size += 4; // BoneIndicies
        //    size += 12;// Normal
        //    size += 8; // Texture1TextureCoordinates
        //    size += 8; // Texture2TextureCoordinates
        //    return size;
        //}

        //public List<byte> ToBytes()
        //{
        //    List<byte> bytes = new List<byte>();
        //    bytes.AddRange(Position.ToBytes());
        //    bytes.AddRange(BoneWeights.ToArray());
        //    bytes.AddRange(BoneIndicies.ToArray());
        //    bytes.AddRange(Normal.ToBytes());
        //    bytes.AddRange(Texture1TextureCoordinates.ToBytes());
        //    bytes.AddRange(Texture2TextureCoordinates.ToBytes());
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
