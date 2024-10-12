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
            // TODO: A few unmapped zone areas (bandit camps)
            SetBaseZoneProperties("lfaydark", "Lesser Faydark", -1769.93f, -108.08f, -1.11f, 0, ZoneContinentType.Faydwer);
            SetZonewideEnvironmentAsOutdoorsNoSky(140, 155, 122, ZoneFogType.Heavy, 1f);
            DisableSunlight();
            SetZonewideAmbienceSound("", "night", 0, 0.2786121f);
            Enable2DSoundInstances("wind_lp4", "darkwds1");

            AddZoneArea("The Monument", "lfaydark-02", "lfaydark-02", Configuration.CONFIG_AUDIO_MUSIC_DEFAULT_VOLUME, false);
            AddZoneAreaBox("The Monument", -233.679443f, -1672.593140f, 200f, -554.176331f, -2018.064331f, -200f);

            AddZoneArea("Gnome Camp", "lfaydark-01", "lfaydark-01");
            AddZoneAreaBox("Gnome Camp", 1188.955688f, -1539.963867f, 200f, 897.718750f, -1840.254517f, -200f);

            AddZoneArea("Wood Elf Outpost", "lfaydark-05", "lfaydark-05");
            AddZoneAreaBox("Wood Elf Outpost", -129.669449f, 1357.105591f, 200f, -481.225159f, 1105.776978f, -200f);
                        
            AddZoneArea("East Orc Camp", "lfaydark-03", "lfaydark-03");
            AddZoneAreaBox("East Orc Camp", -27.457170f, -444.073669f, 200f, -428.689728f, -772.286011f, -200f);

            AddZoneArea("East Central Orc Camp", "lfaydark-03", "lfaydark-03");
            AddZoneAreaBox("East Central Orc Camp", 354.572327f, 472.336548f, 200f, 100.183243f, 165.389740f, -200f);
            
            AddZoneArea("Brownie Compound", "lfaydark-01", "lfaydark-01");
            AddZoneAreaBox("Brownie Compound", 1896.794189f, 3232.993164f, 200f, 1649.393066f, 2987.864990f, -200f);

            AddZoneArea("Faerie Village", "lfaydark-01", "lfaydark-01");
            AddZoneAreaBox("Faerie Village", 1287.373413f, 3750.674805f, 200f, 899.058655f, 3440.435547f, -200f);

            AddZoneArea("West Orc Camp", "lfaydark-03", "lfaydark-03");
            AddZoneAreaBox("West Orc Camp", 131.619629f, 3587.164062f, 200f, -185.258087f, 3263.642578f, -200f);

            AddZoneArea("West Central Orc Camp", "lfaydark-03", "lfaydark-03");
            AddZoneAreaBox("West Central Orc Camp", -460.964111f, 1828.944458f, 200f, -693.051270f, 1478.141235f, -200f);

            AddZoneArea("Abandoned Stone Ring", "lfaydark-04", "lfaydark-04");
            AddZoneAreaBox("Abandoned Stone Ring", -616.388428f, 3332.793701f, 200f, -922.309570f, 2986.999023f, -200f);
                        
            AddZoneLineBox("gfaydark", -2612.000732f, -1113.000000f, 0.000290f, ZoneLineOrientationType.North, 2195.666504f, -1174.378906f, 67.384300f, 2176.618164f, -1215.322021f, -0.499960f);
            AddZoneLineBox("mistmoore", -295.757965f, 160.095764f, -181.936813f, ZoneLineOrientationType.West, -1153.577759f, 3291.550049f, 110.469002f, -1182.255737f, 3372.130859f, -0.499820f);
            AddZoneLineBox("steamfont", 590.807617f, 2193.784424f, -113.249947f, ZoneLineOrientationType.East, 940.560425f, -2182.093262f, 77.329933f, 889.527710f, -2186.912109f, -5.281170f);
        }
    }
}
