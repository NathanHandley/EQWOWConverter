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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Zones.Properties
{
    internal class LesserFaydarkZoneProperties : ZoneProperties
    {
        public LesserFaydarkZoneProperties() : base()
        {
            SetBaseZoneProperties("lfaydark", "Lesser Faydark", -1769.93f, -108.08f, -1.11f, 0, ZoneContinentType.Faydwer);
            SetZonewideEnvironmentAsOutdoors(20, 50, 0, ZoneFogType.Heavy, false, 1f, 1f, 1f, 0.5f);
            DisableSunlight();
            AddZoneLineBox("gfaydark", -2612.000732f, -1113.000000f, 0.000290f, ZoneLineOrientationType.North, 2195.666504f, -1174.378906f, 67.384300f, 2176.618164f, -1215.322021f, -0.499960f);
            AddZoneLineBox("mistmoore", -295.757965f, 160.095764f, -181.936813f, ZoneLineOrientationType.West, -1153.577759f, 3291.550049f, 110.469002f, -1182.255737f, 3372.130859f, -0.499820f);
            AddZoneLineBox("steamfont", 590.807617f, 2193.784424f, -113.249947f, ZoneLineOrientationType.East, 940.560425f, -2182.093262f, 77.329933f, 889.527710f, -2186.912109f, -5.281170f);
        }
    }
}
