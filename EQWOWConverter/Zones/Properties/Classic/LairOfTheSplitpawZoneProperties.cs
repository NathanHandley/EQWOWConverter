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
    internal class LairOfTheSplitpawZoneProperties : ZoneProperties
    {
        public LairOfTheSplitpawZoneProperties()
        {
            SetBaseZoneProperties("paw", "Lair of the Splitpaw", -7.9f, -79.3f, 4f, 0, ZoneContinent.Antonica);
            SetFogProperties(30, 25, 10, 10, 180);
            AddZoneLineBox("southkarana", -3118.534424f, 908.824341f, -11.938860f, ZoneLineOrientationType.West, -95.775307f, 64.159088f, 14.467540f, -112.163208f, 29.530199f, -0.499950f);
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", 1171.132324f, 159.264618f, 1098.533813f, 89.405617f, -5.999900f, 100f); // Western water (higher) - Northmost
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", 1098.633813f, 198.262665f, 931.602183f, 82.047173f, -5.999900f, 100f); // Western water (higher) - Southmost
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", 931.612183f, 223.869247f, 679.216797f, 20.784540f, -31.999950f, 100f); // Western water (lower) - Southwest
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", 918.777344f, 20.794540f, 784.150146f, -90.593658f, -31.999950f, 100f); // Western water (lower) - Eastern
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", 711.465454f, -170.612061f, 618.156555f, -243.180054f, -0.009000f, 100f); // Southeast Water, upper column and part swim under
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", 723.807068f, -112.854370f, 661.784668f, -173.061676f, -40.978719f, 100f); // Southeast Water, middle
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", 727.727112f, 19.525690f, 616.720337f, -117.570297f, -40.978719f, 100f); // Southeast Water, southwest
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", 769.085815f, -69.982788f, 727.627112f, -117.570297f, -40.978719f, 100f); // Southeast Water, southwest
        }
    }
}
