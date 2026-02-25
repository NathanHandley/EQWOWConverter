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
    internal class FieldOfBoneZoneProperties : ZoneProperties
    {
        public FieldOfBoneZoneProperties() : base()
        {
            // AddZoneLineBox("cabeast", 1359.3015f, -435.72766f, 0.000174f, ZoneLineOrientationType.West, -2541.8613f, 3747.162f, 50.543545f, -2570.4119f, 3699.717f, 3.5938148f);
            // AddZoneLineBox("cabeast", 1179.1279f, -619.062f, 0.000174f, ZoneLineOrientationType.South, -2768.011f, 3545.4978f, 86.73899f, -2829.281f, 3514.2957f, 3.5937567f);
            // AddZoneLineBox("kurn", -281.924896f, 64.484741f, 0f, ZoneLineOrientationType.West, 491.488922f, 1084.042725f, 147.798172f, 442.394867f, 1041.152466f, 56.059269f, "West");
            // AddZoneLineBox("kurn", -281.990570f, -64.211090f, 0f, ZoneLineOrientationType.East, 504.123322f, 947.919067f, 115.329117f, 443.650513f, 923.330933f, 54.821972f, "East");
            // AddZoneLineBox("kaesora", 369.378082f, 39.647121f, 95.968773f, ZoneLineOrientationType.South, -1876.434204f, -27.546480f, -102.778427f, -1915.887939f, -154.983429f, -189.135834f);
            // AddZoneLineBox("emeraldjungle", -1248.687012f, 4822.778809f, -10.562540f, ZoneLineOrientationType.East, -728.203369f, -810.171387f, 292.050262f, -1687.132812f, -1426.814819f, -240.731918f);
            // AddZoneLineBox("warslikswood", 1695.040771f, -3486.741699f, 1.675020f, ZoneLineOrientationType.West, 1750.624023f, 4812.213379f, 611.543274f, 771.424744f, 3729.381348f, -231.297836f);
            // AddZoneLineBox("swampofnohope", 4577.290527f, 1101.923096f, 58.393021f, ZoneLineOrientationType.South, -2649.325439f, 1160.931030f, 396.166199f, -3491.827881f, -7.119500f, -122.874771f);

            // AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", 4608.963867f, 4793.761230f, 1746.790283f, -1613.797729f, -179.124878f, 500f);

            AddDiscardGeometryBox(-3801.098633f, 2171.918945f, 1147.039185f, -6046.062012f, 159.970657f, -312.337341f, "Extra geometery into swamp of no hope"); // Extra geometery into swamp of no hope
            AddDiscardGeometryBox(3985.778809f, 4981.827148f, -183.742142f, 3193.421387f, 4778.954590f, -365.933014f, "Small geometry sticking out in map on west side of ocean"); // Small geometry sticking out in map on west side of ocean
        }
    }
}
