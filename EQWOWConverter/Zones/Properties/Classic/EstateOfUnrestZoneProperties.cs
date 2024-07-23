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
    internal class EstateOfUnrestZoneProperties : ZoneProperties
    {
        public EstateOfUnrestZoneProperties()
        {
            SetBaseZoneProperties("unrest", "Estate of Unrest", 52f, -38f, 3.75f, 0, ZoneContinent.Faydwer);
            SetFogProperties(40, 10, 60, 10, 300);
            AddZoneLineBox("cauldron", -2014.301880f, -627.332886f, 90.001083f, ZoneLineOrientationType.North, 113.163170f, 340.068451f, 18.469000f, 72.315872f, 319.681549f, -0.500000f);
            AddOctagonLiquidShape(LiquidType.Water, "d_m0006", 296.937286f, 232.862274f, 36.855289f, 12.970670f, 32.835838f, 17.249861f, 32.835838f, 17.249861f,
                292.667419f, 236.893173f, 292.667419f, 236.893173f, 2.000010f, 25f, 0.4f); // West fountain
            AddOctagonLiquidShape(LiquidType.Water, "d_m0006", 296.937286f, 232.862274f, -13.138050f, -37.008369f, -17.099560f, -32.811298f, -17.099560f, -32.811298f,
                292.667419f, 236.893173f, 292.667419f, 236.893173f, 2.000010f, 25f, 0.4f); // East fountain
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", 684.855469f, -278.539734f, 662.342163f, -300.645325f, -1.999930f, 50f); // Inside the mound with a moat
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", 836.316162f, -197.394684f, 687.165100f, -304.020538f, -1.999930f, 50f); // Outside the mound with a moat north
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", 664.912354f, -197.394684f, 509.776093f, -304.020538f, -1.999930f, 50f); // Outside the mound with a moat south
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", 687.175100f, -197.394684f, 664.902354f, -273.965698f, -1.999930f, 50f); // Outside the mound with a moat center
        }
    }
}
