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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Files.WOWFiles
{
    internal class WMOLiquidTile
    {
        public LiquidType LiquidType = LiquidType.None; // byte
        public byte Unknown1 = 0;
        public byte Unknown2 = 0;
        public byte IsFishable = 1;    // Make all fishable by default
        public byte Shared = 0;        // Unsure on purpose

        public WMOLiquidTile()
        {
        
        }

        public WMOLiquidTile(LiquidType type, byte unknown1, byte unknown2, byte isFishable, byte shared)
        {
            LiquidType = type;
            Unknown1 = unknown1;
            Unknown2 = unknown2;
            IsFishable = isFishable;
            Shared = shared;
        }

        public List<byte> ToBytes()
        {
            List<byte> returnBytes = new List<byte>();
            returnBytes.Add(Convert.ToByte(LiquidType));
            returnBytes.Add(Unknown1);
            returnBytes.Add(Unknown2);
            returnBytes.Add(IsFishable);
            returnBytes.Add(Shared);
            return returnBytes;
        }
    }
}
