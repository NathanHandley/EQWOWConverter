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
using EQWOWConverter.Common;

// Note: There's a client limit of 128 (wowdev.wiki)
namespace EQWOWConverter.Zones
{
    internal class ZonePortal
    {
        public List<Vector3> Vertices = new List<Vector3>();
        public Vector3 Normal = new Vector3();
        public float Distance = 0f;
        public UInt16 GroupIndex;

        public ZonePortal(UInt16 groupIndex)
        {
            GroupIndex = groupIndex;
        }

        public List<byte> ToBytesInfo(UInt16 startVertex)
        {
            List<byte> returnBytes = new List<byte>();
            returnBytes.AddRange(BitConverter.GetBytes(startVertex));
            returnBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(4)));
            returnBytes.AddRange(Normal.ToBytes());
            returnBytes.AddRange(BitConverter.GetBytes(Distance));
            return returnBytes;
        }

        public List<byte> ToBytesReferences(UInt16 portalIndex)
        {
            List<byte> returnBytes = new List<byte>();

            // Negative side (inside)
            returnBytes.AddRange(BitConverter.GetBytes(portalIndex));
            returnBytes.AddRange(BitConverter.GetBytes(GroupIndex));
            returnBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(-1)));
            returnBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(0))); // Filler

            // Positive side (outside)
            returnBytes.AddRange(BitConverter.GetBytes(portalIndex));
            returnBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(0)));
            returnBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(1)));
            returnBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(0))); // Filler

            return returnBytes;
        }
    }
}
