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
    internal class KaesoraZoneProperties : ZoneProperties
    {
        public KaesoraZoneProperties() : base()
        {
            SetZonewideEnvironmentAsIndoors(26, 26, 26, ZoneFogType.Medium);
            OverrideVertexColorIntensity(0.4);

            // Fix Z
            AddZoneLineBox("fieldofbone", -1893.204956f, -182.732758f, -130.822327f, ZoneLineOrientationType.East,
                425.008514f, 52.377048f, 141.641708f, 389.831665f, 23.726810f, 92.980698f);

            AddDiscardGeometryBox(19.384069f, 22.773911f, 39.682812f, -20.191750f, -27.478559f, -7.144740f); // 0 0 0 cat room
        }
    }
}
