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

namespace EQWOWConverter.Common
{
    internal class Fog
    {
        public bool HasInfiniteRadius = false;
        public Vector3 Position = new Vector3();
        public float NearRadius = 0;
        public float FarRadius = 0;
        public float NormalStartScalar = 0.25f;         // Default is 0.25f
        public float NormalEnd = 444.4445f;             // Default is 444.4445f
        public ColorRGBA NormalColor = new ColorRGBA(255, 255, 255, 255);
        public float UnderwaterStartScalar = -0.5f;
        public float UnderwaterEnd = 222.2222f;
        public ColorRGBA UnderwaterColor = new ColorRGBA(255, 255, 255, 255);

        public List<byte> ToBytes()
        {
            List<byte> returnBytes = new List<byte>();
            returnBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(HasInfiniteRadius ? 0x01 : 0x00)));
            returnBytes.AddRange(Position.ToBytes());
            returnBytes.AddRange(BitConverter.GetBytes(NearRadius));
            returnBytes.AddRange(BitConverter.GetBytes(FarRadius));
            returnBytes.AddRange(BitConverter.GetBytes(NormalEnd));
            returnBytes.AddRange(BitConverter.GetBytes(NormalStartScalar));
            returnBytes.AddRange(NormalColor.ToBytesBGRA());
            returnBytes.AddRange(BitConverter.GetBytes(UnderwaterEnd));
            returnBytes.AddRange(BitConverter.GetBytes(UnderwaterStartScalar));
            returnBytes.AddRange(UnderwaterColor.ToBytesBGRA());
            return returnBytes;
        }
    }
}
