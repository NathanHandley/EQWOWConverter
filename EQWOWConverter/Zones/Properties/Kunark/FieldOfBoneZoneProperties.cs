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
            AddZoneLineBox("cabeast", 1359.3015f, -435.72766f, 0.000174f, ZoneLineOrientationType.West,
                -2541.8613f, 3747.162f, 50.543545f, -2570.4119f, 3699.717f, 3.5938148f);
            AddZoneLineBox("cabeast", 1179.1279f, -619.062f, 0.000174f, ZoneLineOrientationType.South,
                -2768.011f, 3545.4978f, 86.73899f, -2829.281f, 3514.2957f, 3.5937567f);

            // Fix Z
            AddZoneLineBox("emeraldjungle", -1901.190918f, 5196.096191f, -9.045620f, ZoneLineOrientationType.East,
                -1469.073242f, -1273.950073f, 272.566742f, -2379.717773f, -1835.171875f, -35.978790f);
            AddZoneLineBox("kaesora", 369.378082f, 39.647121f, 97.021637f, ZoneLineOrientationType.South,
                -1876.434204f, -27.546480f, -102.778427f, -1915.887939f, -154.983429f, -189.135834f);
            AddZoneLineBox("kurn", -281.924896f, 64.484741f, 0.555150f, ZoneLineOrientationType.West,
                491.488922f, 1084.042725f, 147.798172f, 442.394867f, 1041.152466f, 56.059269f); // West
            AddZoneLineBox("kurn", -281.990570f, -64.211090f, 0.319200f, ZoneLineOrientationType.East,
                504.123322f, 947.919067f, 115.329117f, 443.650513f, 923.330933f, 54.821972f); // East
            AddZoneLineBox("swampofnohope", 4919.936523f, 1160.701660f, 29.458639f, ZoneLineOrientationType.South,
                -3451.142822f, 1648.809570f, 343.569519f, -4156.723633f, 583.717896f, -66.526421f);
            AddZoneLineBox("warslikswood", 1572.644287f, -3552.959717f, 1.012120f, ZoneLineOrientationType.West,
                1801.588013f, 5128.291504f, 383.528931f, 886.039978f, 4616.152832f, -61.082981f);

            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", 4608.963867f, 4793.761230f, 1746.790283f, -1613.797729f, -179.124878f, 500f);
        
        }
    }
}
