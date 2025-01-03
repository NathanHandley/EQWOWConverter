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

using EQWOWConverter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal class WMOWaterVert
    {
        public byte Flow1 = 0;
        public byte Flow2 = 0;
        public byte Flow1Percent = 0;
        public byte Unused = 0;
        public float Height = 0;       // Z Value

        public WMOWaterVert()
        {

        }

        public WMOWaterVert(byte flow1, byte flow2, byte flow1Percent, byte unused, float height)
        {
            Flow1 = flow1;
            Flow2 = flow2;
            Flow1Percent = flow1Percent;
            Unused = unused;
            Height = height;
        }


        public List<byte> ToBytes()
        {
            List<byte> returnBytes = new List<byte>();
            returnBytes.Add(Flow1);
            returnBytes.Add(Flow2);
            returnBytes.Add(Flow1Percent);
            returnBytes.Add(Unused);
            returnBytes.AddRange(BitConverter.GetBytes(Height));
            return returnBytes;
        }
    }
}
