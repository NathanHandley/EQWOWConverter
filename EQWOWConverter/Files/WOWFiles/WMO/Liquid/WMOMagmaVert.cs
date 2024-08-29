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

namespace EQWOWConverter.WOWFiles
{
    internal class WMOMagmaVert
    {
        public Int16 UIndex = 0; // Index into TextureCoordinates[].X
        public Int16 VIndex = 0; // Index into TextureCoordinates[].Y
        public float Height = 0; // Z Value

        public WMOMagmaVert()
        {

        }

        public WMOMagmaVert(Int16 uIndex, Int16 vIndex, float height)
        {
            UIndex = uIndex;
            VIndex = vIndex;
            Height = height;
        }
        
        public List<byte> ToBytes()
        {
            List<byte> returnBytes = new List<byte>();
            returnBytes.AddRange(BitConverter.GetBytes(UIndex));
            returnBytes.AddRange(BitConverter.GetBytes(VIndex));
            returnBytes.AddRange(BitConverter.GetBytes(Height));
            return returnBytes;
        }
    }
}
