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
    internal class NorthKaladimZoneProperties : ZoneProperties
    {
        public NorthKaladimZoneProperties() : base()
        {
            SetBaseZoneProperties("kaladimb", "North Kaladim", -267f, 414f, 3.75f, 0, ZoneContinentType.Faydwer);
            SetFogProperties(70, 50, 20, 10, 175);
            AddZoneLineBox("kaladima", 306.093964f, 231.490326f, 0.020500f, ZoneLineOrientationType.South, 394.649292f, 346.066956f, -1.531000f, 397.138519f, 312.694366f, -24.499941f);
            AddZoneLineBox("kaladima", 393.919128f, -263.472565f, 0.000040f, ZoneLineOrientationType.South, 384.053192f, -259.715820f, 22.414330f, 373.654907f, -272.101318f, -0.499970f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_agua1", 789.445374f, 379.175079f, 736.143677f, 226.058517f, -75.968742f, 50f); // NW Rail Pool
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_agua1", 1203.536499f, 188.962967f, 1120.689331f, 76.613777f, 22.000019f, 50f); // Outside north temple area
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_agua1", 628.413330f, -26.542490f, 443.050323f, -200.405060f, -3.999960f, 50f); // Large dock area, north
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_agua1", 460.282043f, -42.519150f, 330.709229f, -153.390045f, -3.999960f, 50f); // Large dock area, south
        }
    }
}
