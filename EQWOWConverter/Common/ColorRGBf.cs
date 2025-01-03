//  Author: Nathan Handley (nathanhandley@protonmail.com)
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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Common
{
    internal class ColorRGBf : IByteSerializable
    {
        public float R = 0;
        public float G = 0;
        public float B = 0;

        public ColorRGBf(int r, int g, int b)
        {
            R = Convert.ToSingle(r) / 255f;
            G = Convert.ToSingle(g) / 255f;
            B = Convert.ToSingle(b) / 255f;
        }
        
        public ColorRGBf(ColorRGBf color)
        {
            R = color.R;
            G = color.G;
            B = color.B;
        }

        public UInt32 GetBytesSize()
        {
            return 12;
        }

        public List<byte> ToBytes()
        {
            List<byte> returnBytes = new List<byte>();
            returnBytes.AddRange(BitConverter.GetBytes(R));
            returnBytes.AddRange(BitConverter.GetBytes(G));
            returnBytes.AddRange(BitConverter.GetBytes(B));
            return returnBytes;
        }
    }
}
