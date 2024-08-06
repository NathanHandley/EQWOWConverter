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
    internal class HalasZoneProperties : ZoneProperties
    {
        public HalasZoneProperties() : base()
        {
            // TODO: Boat that goes back and forth
            SetBaseZoneProperties("halas", "Halas", 0f, 0f, 3.75f, 0, ZoneContinentType.Antonica);
            SetZonewideEnvironmentAsOutdoors(200, 230, 255, ZoneFogType.Heavy, false, 1f, 1f, 0.5f);
            DisableSunlight();
            AddZoneLineBox("everfrost", 3682.792725f, 372.904633f, 0.000240f, ZoneLineOrientationType.South, -664.463196f, -50.776440f, 37.469002f, -744.483093f, -101.162247f, -0.499990f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_agua1", -16.822701f, 195.248566f, -464.163391f, -189.505676f, -2.999970f, 250f); // Pool at zone line
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_agua1", 199.293900f, -199.563965f, 177.719742f, -224.856445f, -0.999970f, 250f); // Well
        }
    }
}
