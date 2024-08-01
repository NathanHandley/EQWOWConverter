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
    internal class KithicorForestZoneProperties : ZoneProperties
    {
        public KithicorForestZoneProperties() : base()
        {
            SetBaseZoneProperties("kithicor", "Kithicor Forest", 3828f, 1889f, 459f, 0, ZoneContinentType.Antonica);
            SetFogProperties(120, 140, 100, 10, 200);
            AddZoneLineBox("commons", 1032.412720f, 4154.744629f, -52.093071f, ZoneLineOrientationType.North, 1408.693237f, -1098.195190f, 55.470139f, 1378.633545f, -1153.891724f, -52.593639f);
            AddZoneLineBox("highpass", -980.394165f, 90.663696f, -0.000010f, ZoneLineOrientationType.North, 569.884521f, 4903.054199f, 742.436829f, 558.181274f, 4885.024414f, 689.404907f);
            AddZoneLineBox("rivervale", -371.955841f, -282.273224f, 0.000020f, ZoneLineOrientationType.North, 2028.557495f, 3831.161621f, 472.718994f, 2020.498779f, 3818.663086f, 461.750427f);
            AddLiquidPlaneZLevel(LiquidType.Water, "d_w1", 107.216621f, 463.531281f, -697.145447f, -773.156982f, -49.736340f, 200f);
        }
    }
}
