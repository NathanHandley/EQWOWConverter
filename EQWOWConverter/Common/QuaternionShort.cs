﻿//  Author: Nathan Handley (nathanhandley@protonmail.com)
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

namespace EQWOWConverter.Common
{
    // This is a 'short' version of the quaternion that is used for bones
    internal class QuaternionShort : IByteSerializable
    {
        public float X = 0;
        public float Y = 0;
        public float Z = 0;
        public float W = 0;

        public QuaternionShort() { }
        public QuaternionShort(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        private UInt16 GetFloatAsShort(float value)
        {
            return Convert.ToUInt16((value + 1f) * 32767);
        }

        public UInt32 GetBytesSize()
        {
            return 8;
        }

        public List<Byte> ToBytes()
        {
            List<Byte> bytes = new List<Byte>();
            bytes.AddRange(BitConverter.GetBytes(GetFloatAsShort(X)));
            bytes.AddRange(BitConverter.GetBytes(GetFloatAsShort(Y)));
            bytes.AddRange(BitConverter.GetBytes(GetFloatAsShort(Z)));
            bytes.AddRange(BitConverter.GetBytes(GetFloatAsShort(W)));
            return bytes;
        }
    }
}