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

namespace EQWOWConverter.Zones.WOW
{
    internal class WorldModelObjectDoodadInstance
    {
        public string ObjectName = string.Empty;
        public UInt32 ObjectNameOffset = 0;
        public DoodadInstanceFlags Flags = DoodadInstanceFlags.AcceptProjectedTexture;
        public Vector3 Position = new Vector3();
        public Quaternion Orientation = new Quaternion();
        public float Scale = 0.0f;
        public ColorBGRA Color = new ColorBGRA();

        public List<byte> ToBytes()
        {
            List<byte> returnBytes = new List<byte>();

            // The flags and name offset share a UInt32, effectively making it UInt24 and UInt8
            returnBytes.AddRange(BitConverter.GetBytes(ObjectNameOffset));
            returnBytes[3] = Convert.ToByte(Flags);
            returnBytes.AddRange(Position.ToBytes());
            returnBytes.AddRange(Orientation.ToBytes());
            returnBytes.AddRange(BitConverter.GetBytes(Scale));
            returnBytes.AddRange(Color.ToBytes());
            return returnBytes;
        }
    }
}
