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
            AddZoneLineBox("burningwood", -2965.3167f, -4515.809f, -51.462868f, ZoneLineOrientationType.West, -2312.331f, 4184.5947f, -433.798f, -2418.7312f, 4063.2607f, -472.19543f);
            AddZoneLineBox("dreadlands", 2360.625732f, -3406.440674f, 135.156448f, ZoneLineOrientationType.West, -3734.184326f, -3201.033691f, -313.857239f, -3868.127441f, -3265.977051f, -354.653534f);
            AddZoneLineBox("droga", 1410.232056f, 376.219910f, 0f, ZoneLineOrientationType.South, 3269.139404f, 2982.568115f, 359.866669f, 3213.493164f, 2914.926758f, 336.742157f);
            AddZoneLineBox("nurga", -2220.826904f, -1798.888184f, 0f, ZoneLineOrientationType.North, -2604.649170f, -475.103729f, -475.357574f, -2669.104492f, -556.315918f, -506.329834f);
            AddZoneLineBox("lakeofillomen", -430.327026f, 3112.575684f, 110.214043f, ZoneLineOrientationType.East, -406.436157f, -4319.731445f, 476.974548f, -932.939514f, -4843.364746f, 35.422649f);
            AddZoneLineBox("overthere", -3839.407471f, 1460.350830f, 312.285889f, ZoneLineOrientationType.North, 5587.589844f, 2008.723511f, 1066.942993f, 4569.660156f, 813.669495f, 238.345154f);

            AddDiscardGeometryBox(8489.225586f, 2753.701172f, 1165.576416f, 6490.531250f, 361.942871f, 165.254105f, "Overthere zone line"); // Overthere zone line
            AddDiscardGeometryBox(6638.459473f, 2376.372314f, 1292.566162f, 5230.738770f, 517.452332f, 102.275459f, "More Overthere zone line"); // More Overthere zone line
        }
    }
}
