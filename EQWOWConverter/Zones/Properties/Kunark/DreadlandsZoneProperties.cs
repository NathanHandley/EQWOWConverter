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
            AddZoneLineBox("burningwood", -4247.9624f, -712.7452f, 245.30704f, ZoneLineOrientationType.North,
                3057.91f, -414.8485f, 319.16867f, 2988.2588f, -1083.3096f, 240.4023f);

            // Fix Z
            AddZoneLineBox("karnor", 115.772827f, 341.605042f, 0.434650f, ZoneLineOrientationType.East,
                -1986.169312f, 720.839783f, 63.532600f, -2042.756836f, 675.693604f, 18.183969f); // Karnor North entry
            AddZoneLineBox("karnor", -81.997353f, 341.945251f, 2.062710f, ZoneLineOrientationType.East,
                -1985.006226f, 521.480408f, 65.162521f, -2037.955078f, 477.249451f, 18.468460f); // Karnor South entry
            AddZoneLineBox("frontiermtns", -3835.836182f, -3291.410645f, -348.273071f, ZoneLineOrientationType.East,
                -3442.503906f, 2417.939941f, 183.325562f, -3551.589355f, 2262.425049f, 126.868477f);
            AddZoneLineBox("firiona", 240.615234f, 6047.389648f, -80.769928f, ZoneLineOrientationType.South,
                -6036.449707f, 315.327850f, 184.674255f, -6161.772949f, 269.808441f, 129.150375f);
        }
    }
}
