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
    internal class InnothuleSwampZoneProperties : ZoneProperties
    {
        public InnothuleSwampZoneProperties()
        {
            SetBaseZoneProperties("innothule", "Innothule Swamp", -588f, -2192f, -25f, 0, ZoneContinent.Antonica);
            SetFogProperties(170, 160, 90, 10, 500);
            AddZoneLineBox("feerrott", -1020.344177f, -3092.292236f, -12.343540f, ZoneLineOrientationType.North, -1110.918945f, 1900.790283f, 9.191510f, -1156.486450f, 1899.104858f, -12.843200f);
            AddZoneLineBox("grobb", -179.500046f, 39.101452f, -0.000000f, ZoneLineOrientationType.West, -2781.871094f, -625.318726f, -16.126810f, -2804.662109f, -646.227112f, -35.062538f);
            AddZoneLineBox("guktop", -62.457378f, 42.394871f, 0.000010f, ZoneLineOrientationType.East, 150.598709f, -828.381348f, 0.967340f, 136.212891f, -843.098694f, -11.999980f);
            AddZoneLineBox("sro", -3168.635742f, 1032.933105f, -26.814310f, ZoneLineOrientationType.North, 2800f, 1250f, 19.084551f, 2554.791748f, 1120f, -35f);
            AddLiquidPlaneZLevel(LiquidType.Water, "t75_sw1", 2322.104736f, 1750.974365f, -2615.490234f, -670.816833f, -31.624689f, 50f);
            AddDisabledMaterialCollisionByNames("t75_sw1");
        }
    }
}
