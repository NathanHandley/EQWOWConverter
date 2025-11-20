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

namespace EQWOWConverter.Zones
{
    internal class ZonePropertiesDisplayMapLinkBox
    {
        public string LinkedZoneShortName = string.Empty;
        public float West;
        public float North;
        public float Width;
        public float Height;

        public ZonePropertiesDisplayMapLinkBox(string linkedZoneShortName, float west, float north, float east, float south)
        {
            west *= Configuration.GENERATE_WORLD_SCALE;
            north *= Configuration.GENERATE_WORLD_SCALE;
            east *= Configuration.GENERATE_WORLD_SCALE;
            south *= Configuration.GENERATE_WORLD_SCALE;

            LinkedZoneShortName = linkedZoneShortName;
            West = west;
            North = north;
            Width = west - east;
            Height = north - south;
        }
    }
}
