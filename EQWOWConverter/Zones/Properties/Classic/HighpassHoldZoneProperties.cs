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
    internal class HighpassHoldZoneProperties : ZoneProperties
    {
        public HighpassHoldZoneProperties() : base()
        {
            SetBaseZoneProperties("highpass", "Highpass Hold", -104f, -14f, 4f, 0, ZoneContinentType.Antonica);
            SetFogProperties(200, 200, 200, 10, 400);
            AddZoneLineBox("eastkarana", -3069.264893f, -8291.038086f, 689.907410f, ZoneLineOrientationType.West, -1000.400269f, 153.409576f, 25.578859f, -1021.786133f, 121.336189f, -0.500030f);
            AddZoneLineBox("highkeep", -90.607208f, 98.531219f, 0.000000f, ZoneLineOrientationType.East, -83.776314f, -118.791763f, 12.469000f, -98.162193f, -140.129593f, -0.500000f);
            AddZoneLineBox("highkeep", 62.486179f, 97.604347f, 0.000030f, ZoneLineOrientationType.East, 70.161171f, -118.140022f, 12.469000f, 53.453629f, -126.744057f, -0.500010f);
            AddZoneLineBox("kithicor", 552.036682f, 4892.523438f, 689.904907f, ZoneLineOrientationType.South, -986.189697f, 98.161331f, 38.800350f, -1007.820984f, 83.809853f, -0.499890f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", -52.058632f, 488.075775f, -396.379669f, 248.035797f, -0.999960f, 100f);
        }
    }
}
