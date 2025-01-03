﻿//  Author: Nathan Handley (nathanhandley@protonmail.com)
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
using EQWOWConverter.WOWFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.ObjectModels
{
    internal class ObjectModelVertex : IByteSerializable
    {
        public Vector3 Position = new Vector3();
        public List<byte> BoneWeights = new List<byte>(new byte[4]);        // Any more than 4 elements will be ignored
        public List<byte> BoneIndicesTrue = new List<byte>(new byte[4]);    // Bone indexes compared to the bone list
        public List<byte> BoneIndicesLookup = new List<byte>(new byte[4]);  // Bone indexes compared to the bone lookup table
        public Vector3 Normal = new Vector3();
        public TextureCoordinates Texture1TextureCoordinates = new TextureCoordinates();
        public TextureCoordinates Texture2TextureCoordinates = new TextureCoordinates();

        public ObjectModelVertex()
        {
            // First bone weight is always max by default
            BoneWeights[0] = 255;
        }

        public UInt32 GetBytesSize()
        {
            UInt32 size = 0;
            size += 12;// Position
            size += 4; // BoneWeights
            size += 4; // BoneIndices
            size += 12;// Normal
            size += 8; // Texture1TextureCoordinates
            size += 8; // Texture2TextureCoordinates
            return size;
        }

        public List<byte> ToBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(Position.ToBytes());
            bytes.AddRange(BoneWeights.ToArray());
            bytes.AddRange(BoneIndicesTrue.ToArray());
            bytes.AddRange(Normal.ToBytes());
            bytes.AddRange(Texture1TextureCoordinates.ToBytes());
            bytes.AddRange(Texture2TextureCoordinates.ToBytes());
            return bytes;
        }
    }
}
