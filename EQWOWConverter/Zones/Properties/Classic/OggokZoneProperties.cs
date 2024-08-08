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
    internal class OggokZoneProperties : ZoneProperties
    {
        public OggokZoneProperties() : base()
        {
            SetBaseZoneProperties("oggok", "Oggok", -99f, -345f, 4f, 0, ZoneContinentType.Antonica);
            SetZonewideEnvironmentAsOutdoorsNoSky(88, 94, 54, ZoneFogType.Heavy, 1f);
            DisableSunlight();
            AddZoneLineBox("feerrott", 1652.742065f, 811.823181f, 57.281330f, ZoneLineOrientationType.South, -399.834625f, -77.776642f, 56.437752f, -462.005951f, -120.130768f, -0.500000f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_oggokwater1", 200.303665f, 192.993454f, -183.825287f, -191.923065f, -62.968739f, 200f); // Water around the blood/sludge arena at 0,0,0
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_oggokusludge1", 52.597210f, 51.597240f, -52.363419f, -52.416752f, -46.968750f, 15f); // Bloody sludge in the  arena at 0,0,0
        }
    }
}
