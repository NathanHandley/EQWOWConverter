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

namespace EQWOWConverter.Zones.Properties
{
    internal class NorthFelwitheZoneProperties : ZoneProperties
    {
        public NorthFelwitheZoneProperties() : base()
        {
            SetBaseZoneProperties("felwithea", "Northern Felwithe", 94f, -25f, 3.75f, 0, ZoneContinentType.Faydwer);
            SetFogProperties(100, 130, 100, 10, 300);
            AddZoneLineBox("felwitheb", 251.268646f, -832.815125f, -13.999020f, ZoneLineOrientationType.North, 364.650452f, -711.921509f, -1.531000f, 342.316345f, -727.911865f, -14.499750f);
            AddZoneLineBox("gfaydark", -1931.678101f, -2613.879639f, 20.406450f, ZoneLineOrientationType.West, 56.161152f, 242.410782f, 26.469000f, 27.806530f, 193.596893f, -0.500000f);
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", 137.808594f, -34.700352f, 31.669941f, -209.193726f, -14.01f, 300f); // West pool
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", 42.782681f, -41.832130f, -32.244209f, -154.059235f, -14.01f, 300f);
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", -27.781460f, -41.832870f, -130.866898f, -209.193726f, -14.01f, 300f);
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", 305.541901f, -629.289001f, -168.728821f, -835.178894f, -27.997999f, 300f); // East pool
        }
    }
}
