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
        public SteamfontMountainsZoneProperties() : base()
        {
            // TODO: Look into possibly more zone areas
            // TODO: Darken the Minotaur Caves
            SetBaseZoneProperties("steamfont", "Steamfont Mountains", -272.86f, 159.86f, -21.4f, 0, ZoneContinentType.Faydwer);
            SetZonewideAmbienceSound("", "darkwds1");
            Enable2DSoundInstances("wind_lp4", "steamlp");

            AddZoneArea("The Windmills", "steamfont-02", "steamfont-02", false);
            AddZoneAreaBox("The Windmills", 668.587585f, -75.615662f, 409.540710f, -391.177246f, -1247.875854f, -228.816513f);

            AddZoneArea("Minotaur Caves", "steamfont-00", "steamfont-00");
            AddZoneAreaBox("Minotaur Caves", 1964.830078f, -1998.093628f, -48.734001f, 1027.718140f, -2531.045898f, -205.285553f);
            AddZoneAreaBox("Minotaur Caves", 1486.451416f, -1920.150513f, -3.271080f, 1161.790039f, -2492.401611f, -156.221054f);
            AddZoneAreaBox("Minotaur Caves", 1440.333862f, -1656.859253f, -33.397308f, 1086.998047f, -2033.821655f, -195.411057f);

            AddZoneArea("Ak'Anon", "steamfont-03", "steamfont-03");
            AddZoneAreaBox("Ak'Anon", -1465.521118f, 895.604370f, 37.269329f, -2252.618408f, 181.761765f, -302.858337f);

            AddZoneArea("Druid Stone Ring", "steamfont-01", "steamfont-01");
            AddZoneAreaBox("Druid Stone Ring", -1487.718384f, 1796.941772f, 76.132362f, -1984.688843f, 1419.929077f, -284.841370f);

            AddZoneArea("South Kobold Camp", "steamfont-05", "steamfont-05");
            AddZoneAreaBox("South Kobold Camp", -550.745239f, 1970.254395f, -17.606470f, -1025.218872f, 1508.614502f, -245.629196f);

            AddZoneArea("Dragon Skeleton", "steamfont-06", "steamfont-06", false);
            AddZoneAreaBox("Dragon Skeleton", -905.103760f, -333.562561f, 43.232040f, -1528.842773f, -1015.249390f, -195.124466f);

            if (Configuration.CONFIG_AUDIO_USE_ALTERNATE_TRACKS == true)
                AddZoneArea("North Kobold Camp", "steamfont-05", "steamfont-05");
            else
                AddZoneArea("North Kobold Camp");
            AddZoneAreaBox("North Kobold Camp", 1931.113281f, 1453.376221f, 88.200996f, 1453.425171f, 947.308777f, -200.573578f);

            SetZonewideEnvironmentAsOutdoorsWithSky(152, 152, 167, ZoneFogType.Medium, 0.75f, 1f);
            AddZoneLineBox("akanon", 57.052101f, -77.213501f, 0.000010f, ZoneLineOrientationType.South, -2064.9805f, 535.8183f, -98.656f, -2077.9038f, 521.43134f, -111.624886f);
            AddZoneLineBox("lfaydark", 930.675537f, -2166.410400f, -4.781320f, ZoneLineOrientationType.West, 608.013672f, 2214.515625f, 26.767950f, 559.319153f, 2202.571045f, -113.749878f);
        }
    }
}
