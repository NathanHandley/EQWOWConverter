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

namespace EQWOWConverter.Zones.Properties
{
    internal class DreadlandsZoneProperties : ZoneProperties
    {
        public DreadlandsZoneProperties() : base()
        {
            SetZonewideEnvironmentAsOutdoorsWithSky(95, 89, 75, ZoneFogType.Clear, 0.5f, 1f);

            AddZoneLineBox("burningwood", -4247.9624f, -712.7452f, 245.30704f, ZoneLineOrientationType.North,
                3057.91f, -414.8485f, 319.16867f, 2988.2588f, -1083.3096f, 240.4023f);
        }
    }
}
