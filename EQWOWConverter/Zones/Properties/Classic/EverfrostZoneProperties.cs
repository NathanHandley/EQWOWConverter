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
    internal class EverfrostZoneProperties : ZoneProperties
    {
        public EverfrostZoneProperties() : base()
        {
            // TODO: Add zone areas
            SetBaseZoneProperties("everfrost", "Everfrost", 682.74f, 3139.01f, -60.16f, 0, ZoneContinentType.Antonica);
            SetZonewideEnvironmentAsOutdoorsNoSky(144, 165, 183, ZoneFogType.Heavy, 1f);
            DisableSunlight();
            SetZonewideAmbienceSound("wind_lp2", "wind_lp4", 0.13931568f, 0.098401114f);
            AddZoneLineBox("blackburrow", 64.26508f, -340.1918f, 0.00073920796f, ZoneLineOrientationType.South, -3054.6953f, -515.55963f, -99.7185f, -3094.8235f, -547f, -113.68753f);
            AddZoneLineBox("halas", -647.768616f, -75.159027f, 0.000020f, ZoneLineOrientationType.North, 3756.428467f, 397.611786f, 38.469002f, 3706.500488f, 347.150665f, -0.499760f);
            AddZoneLineBox("permafrost", -61.690048f, 84.215889f, 0.000010f, ZoneLineOrientationType.East, 2040.192261f, -7055.080078f, -8.999750f, 1989.364502f, -7120.806641f, -64.344040f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wat1", 3592.137207f, -3781.852539f, 1687.589478f, -4645.904785f, -105.843300f, 350f); // NW water that goes into cave
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wat1", 3312.257568f, -5144.483887f, 2050.186523f, -5937.699219f, -105.843300f, 350f); // NE water that runs into a mountain line
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wat1", 2050.196523f, -4937.517578f, 1608.196289f, -5580.368652f, -105.843300f, 350f); // NE water path, 2nd leg from north
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wat1", 1687.825806f, -4247.805176f, -1085.845327f, -5472.050781f, -105.843300f, 350f); // South water path that connects to north
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wat1", -1085.835327f, -4139.526367f, -4905.087891f, -4658.822266f, -105.843300f, 350f); // South water path end
        }
    }
}
