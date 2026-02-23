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
            AddZoneLineBox("burningwood", -4247.9624f, -712.7452f, 245.30704f, ZoneLineOrientationType.North, 4832.997559f, 42.463902f, 1087.840820f, 2864.077393f, -1472.783569f, 40.859489f);
            AddZoneLineBox("karnor", 115.772827f, 341.605042f, 0f, ZoneLineOrientationType.East, 720.839783f, -1986.169312f, 63.532600f, 675.693604f, -2042.756836f, 18.183969f, "Karnor North entry");
            AddZoneLineBox("karnor", -81.997353f, 341.945251f, 0f, ZoneLineOrientationType.East, 521.480408f, -1985.006226f, 65.162521f, 477.249451f, -2037.955078f, 18.468460f, "Karnor South entry");
            AddZoneLineBox("firiona", 240.615234f, 6047.389648f, -82.966827f, ZoneLineOrientationType.South, 315.327850f, -6036.449707f, 184.674255f, 269.808441f, -6161.772949f, 129.150375f);
            AddZoneLineBox("frontiermtns", -3835.836182f, -3291.410645f, -350.405426f, ZoneLineOrientationType.East, 2417.939941f, -3442.503906f, 183.325562f, 2262.425049f, -3551.589355f, 126.868477f);
        }
    }
}
