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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal enum WMOPolyMaterialFlags : byte
    {
        Unknown1            = 0x01,
        NoCameraCollide     = 0x02,
        Detail              = 0x04,
        NoCollision         = 0x08, // Reads as collision, but wowdev says it turns off rendering of water ripple...?
        Hint                = 0x10,
        Render              = 0x20,
        CullObjects         = 0x40,
        CollideHit          = 0x80
    }
}
