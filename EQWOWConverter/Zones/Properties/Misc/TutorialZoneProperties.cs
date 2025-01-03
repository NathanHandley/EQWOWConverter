//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2025 Nathan Handley
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
    internal class TutorialZoneProperties : ZoneProperties
    {
        public TutorialZoneProperties() : base()
        {
            SetBaseZoneProperties("tutorial", "Tutorial", 0f, 0f, 0f, 0, ZoneContinentType.Development);
            SetZonewideAmbienceSound("", "night");
            Enable2DSoundInstances("wind_lp4");
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_w1", 99.116943f, 98.680382f, -101.066818f, -103.535660f, -14.000000f, 350f); // Middle circle of water
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_w1", -316.934570f, -120.476463f, -436.932068f, -189.277786f, -0.999990f, 350f); // Waterfall area
        }
    }
}
