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

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Common
{
    internal class SplineKey : IByteSerializable
    {
        public Vector3 Point = new Vector3();
        public Vector3 TanIn = new Vector3();
        public Vector3 TanOut = new Vector3();

        public UInt32 GetBytesSize()
        {
            return Point.GetBytesSize() + TanIn.GetBytesSize() + TanOut.GetBytesSize();
        }

        public List<byte> ToBytes()
        {
            List<byte> returnBytes = new List<byte>();
            returnBytes.AddRange(Point.ToBytes());
            returnBytes.AddRange(TanIn.ToBytes());
            returnBytes.AddRange(TanOut.ToBytes());
            return returnBytes;
        }

    }
}
