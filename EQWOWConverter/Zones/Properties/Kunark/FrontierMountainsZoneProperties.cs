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
    internal class FrontierMountainsZoneProperties : ZoneProperties
    {
        public FrontierMountainsZoneProperties() : base()
        {
            AddZoneLineBox("burningwood", -2965.3167f, -4515.809f, -51.462868f, ZoneLineOrientationType.West,
                -2312.331f, 4184.5947f, -433.798f, -2418.7312f, 4063.2607f, -472.19543f);

            // Fix Z
            AddZoneLineBox("dreadlands", 2360.625732f, -3406.440674f, 137.522598f, ZoneLineOrientationType.West,
                -3201.033691f, -3734.184326f, -313.857239f, -3265.977051f, -3868.127441f, -354.653534f);
            AddZoneLineBox("droga", 1410.232056f, 376.219910f, 0.776180f, ZoneLineOrientationType.South,
                2982.568115f, 3269.139404f, 359.866669f, 2914.926758f, 3213.493164f, 336.742157f);
            AddZoneLineBox("nurga", -2220.826904f, -1798.888184f, 1.368590f, ZoneLineOrientationType.North,
                -475.103729f, -2604.649170f, -475.357574f, -556.315918f, -2669.104492f, -506.329834f);
        }
    }
}
