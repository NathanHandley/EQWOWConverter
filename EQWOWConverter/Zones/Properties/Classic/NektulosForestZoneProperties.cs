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
    internal class NektulosForestZoneProperties : ZoneProperties
    {
        public NektulosForestZoneProperties() : base()
        {
            SetBaseZoneProperties("nektulos", "Nektulos Forest", -259f, -1201f, -5f, 0, ZoneContinentType.Antonica);
            SetZonewideEnvironmentAsOutdoorsNoSky(57, 64, 50, ZoneFogType.Heavy, 1f);
            AddZoneLineBox("ecommons", 1569.311157f, 667.254028f, -21.531260f, ZoneLineOrientationType.East, -2666.610596f, -550.025208f, 31.661320f, -2707.922119f, -636.140076f, -22.031050f);
            AddZoneLineBox("lavastorm", -2075.322998f, -189.802826f, -19.598631f, ZoneLineOrientationType.North, 3164.449707f, 385.575653f, 151.052231f, 3094.654785f, 237.925507f, -19.999571f);
            AddZoneLineBox("neriaka", -0.739280f, 113.977829f, 28.000000f, ZoneLineOrientationType.South, 2270.015381f, -1092.749023f, 12.469000f, 2210.318115f, -1149.777344f, -0.499900f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", -229.968414f, 2123.114746f, -848.817383f, 945.974426f, -30.156151f, 200f); // West most part of river
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", -545.284668f, 946.974426f, -1134.071777f, -186.474213f, -30.156151f, 200f); // Middle part of river
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", -853.824585f, -185.474213f, -1614.697388f, -1905.550659f, -30.156151f, 200f); // East part of river
        }
    }
}
