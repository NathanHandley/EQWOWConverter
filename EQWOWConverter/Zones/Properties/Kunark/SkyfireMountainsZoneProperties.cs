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
    internal class SkyfireMountainsZoneProperties : ZoneProperties
    {
        public SkyfireMountainsZoneProperties() : base()
        {
            AddZoneLineBox("burningwood", 5087.0146f, 1740.0859f, -163.56395f, ZoneLineOrientationType.South, -5623.817f, 1910.7054f, -56.840195f, -5703.1704f, 1580.5497f, -164.28036f); // Zone-in had no geometery

            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", 3761.557861f, 3470.733398f, 3268.780273f, 3050.660889f, 208.125198f, 300f); // North lava crater, north part
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", 3614.604736f, 3515.087158f, 3084.947754f, 3062.883789f, 208.125198f, 300f); // North lava crater, south part
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", 1390.400757f, 1879.098877f, -1002.334717f, -450.889465f, -249.406174f, 500f); // Large center crater
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", 1619.781494f, -1157.938477f, 1007.738647f, -1789.466064f, -2.093660f, 500f); // East crater
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -1899.353271f, 1773.006714f, -2462.694580f, 1210.469849f, 54.875099f, 500f); // Southwest crater

            // North NorthWest river, climbing south to dragon head
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 4122.767578f, 2848.519287f, 4105.233887f, 2812.736328f, -127.543694f, -135.220276f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 4105.268555f, 2851.568359f, 4095.123535f, 2811.029785f, -123.764618f, -127.547127f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 4095.157959f, 2854.532715f, 4064.891113f, 2810.141113f, -91.600288f, -123.768066f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 4064.925537f, 2843.386475f, 4054.918945f, 2807.244873f, -84.548462f, -91.603737f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);

            // North NorthWest river under the south climb to dragon head, flat
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", 4526.216309f, 2862.261963f, 4088.063477f, 2610.507080f, -135.218597f, 200f);

            // North Northwest river, climb between highest flat and middle flat
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 4590.627930f, 2561.269043f, 4552.329102f, 2549.357178f, -176.390427f, -184.223267f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 4588.431641f, 2575.642822f, 4557.763184f, 2561.234619f, -164.206207f, -176.393875f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 4586.958984f, 2597.056885f, 4537.929199f, 2575.608398f, -151.069397f, -164.209641f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 4579.996582f, 2617.265625f, 4531.307617f, 2597.022461f, -135.218506f, -151.072845f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 4547.081543f, 2626.501221f, 4521.767578f, 2617.231201f, -135.218430f, -135.221939f, ZoneLiquidSlantType.WestHighEastLow, 250f);

            // North Northwest river, middle flat
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", 4799.291016f, 2591.093994f, 4583.361328f, 1992.584595f, -184.187180f, 250f);

            // North Northwest river, climb between lowest and middle flat
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 4754.747559f, 1955.175171f, 4706.935059f, 1941.257080f, -190.411255f, -199.722000f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 4774.478027f, 1983.695068f, 4714.352051f, 1955.140747f, -185.366241f, -190.414703f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 4805.272949f, 2031.548218f, 4715.747070f, 1983.660522f, -184.184814f, -185.369675f, ZoneLiquidSlantType.WestHighEastLow, 250f);

            // North Northwest river, lowest flat (connects at junction)
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", 4759.243164f, 1958.178589f, 3103.476562f, 936.244263f, -199.718567f, 250f);

            // North Northeast flat (connects at junction)
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", 3897.764160f, 968.982788f, 2306.562256f, -1492.043335f, -199.718567f, 250f);

            // North Northeast, climb south to dragon head
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 2310.685791f, -1444.841431f, 2298.043701f, -1491.484497f, -194.311005f, -199.243347f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 2298.078369f, -1430.706299f, 2276.827637f, -1497.013794f, -182.602234f, -194.314453f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 2276.862305f, -1419.767334f, 2265.499268f, -1504.969482f, -179.167526f, -182.605698f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 2265.533691f, -1422.499634f, 2231.972656f, -1521.020508f, -178.211456f, -179.170959f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);

            // North from big pool, flat
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", 3183.667236f, 1472.633911f, 1208.061401f, 508.424438f, -199.718567f, 250f);

            // Big pool into north river
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 1138.029541f, 553.655151f, 1120.569214f, 515.174255f, -230.343185f, -249.409698f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 1153.882690f, 557.377441f, 1137.994995f, 518.926575f, -217.445374f, -230.346634f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 1168.381592f, 559.179993f, 1153.848145f, 518.309082f, -207.944763f, -217.448837f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 1208.743774f, 560.667603f, 1168.347046f, 517.773376f, -199.627274f, -207.948196f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);






        }
    }
}
