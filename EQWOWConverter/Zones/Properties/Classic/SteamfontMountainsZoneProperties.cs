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
    internal class SteamfontMountainsZoneProperties : ZoneProperties
    {
        public SteamfontMountainsZoneProperties()
        {
            SetBaseZoneProperties("steamfont", "Steamfont Mountains", -272.86f, 159.86f, -21.4f, 0, ZoneContinent.Faydwer);
            SetFogProperties(200, 200, 220, 10, 800);
            AddZoneLineBox("akanon", 57.052101f, -77.213501f, 0.000010f, ZoneLineOrientationType.South, -2064.9805f, 535.8183f, -98.656f, -2077.9038f, 521.43134f, -111.624886f);
            AddZoneLineBox("lfaydark", 930.675537f, -2166.410400f, -4.781320f, ZoneLineOrientationType.West, 608.013672f, 2214.515625f, 26.767950f, 559.319153f, 2202.571045f, -113.749878f);
        }
    }
}
