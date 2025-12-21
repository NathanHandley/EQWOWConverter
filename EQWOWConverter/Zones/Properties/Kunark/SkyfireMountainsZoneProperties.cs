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
            AddAlwaysBrightMaterial("d_lava001");

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

            // Isolated Northwest, west climb (lowest to highest, heading west then south)
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 1506.949585f, 4497.075684f, 1418.387207f, 4482.598145f, -190.995438f, -199.722076f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 1487.952148f, 4546.512695f, 1346.304321f, 4497.041504f, -190.535507f, -190.998886f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 1418.439575f, 4564.167969f, 1355.454346f, 4546.478027f, -181.374817f, -190.538940f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 1399.200684f, 4601.865234f, 1347.367188f, 4564.133301f, -181.192017f, -181.378265f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 1377.555786f, 4619.984375f, 1322.571167f, 4601.830566f, -172.064423f, -181.195465f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 1333.473755f, 4654.394531f, 1322.985229f, 4591.938477f, -172.737839f, -172.081833f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 1323.019775f, 4681.912109f, 1307.329590f, 4592.443359f, -158.104889f, -172.741287f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 1307.364136f, 4672.693848f, 1257.218872f, 4596.802734f, -157.593460f, -158.108322f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 1257.253296f, 4660.817871f, 1246.741089f, 4612.419922f, -149.748520f, -157.596909f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 1246.775635f, 4662.256836f, 1237.095459f, 4607.007812f, -144.778732f, -149.751968f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 1237.129883f, 4661.367676f, 1226.489868f, 4605.998047f, -130.155334f, -144.782181f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 1226.524414f, 4684.674805f, 1107.912720f, 4585.565918f, -129.611176f, -130.158783f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 1107.947266f, 4658.092285f, 1071.476318f, 4597.831055f, -116.488243f, -129.614624f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);

            // Isolated northwest, north climb (lowest to highest)
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 1579.358765f, 3369.519775f, 1567.883179f, 3306.472412f, -192.043564f, -199.722076f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 1589.581177f, 3365.045654f, 1579.324219f, 3312.437012f, -178.867462f, -192.047028f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 1599.620117f, 3363.174805f, 1589.546509f, 3316.744873f, -175.220627f, -178.870926f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 1609.105103f, 3360.018311f, 1599.585815f, 3317.540771f, -163.970291f, -175.224060f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 1619.063477f, 3357.179932f, 1609.070557f, 3316.386475f, -159.812241f, -163.973740f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 2237.645020f, 3922.502930f, 1619.028931f, 3237.258789f, -158.504456f, -159.815689f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 2268.272217f, 3568.202148f, 2235.512939f, 3534.352295f, -158.518997f, -158.503021f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 2273.855957f, 3581.293213f, 2235.992676f, 3568.167480f, -164.153244f, -158.522446f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 2281.045898f, 3589.132812f, 2238.288330f, 3581.258545f, -162.852570f, -164.156693f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 2277.224365f, 3599.367432f, 2237.447754f, 3589.098145f, -164.312149f, -162.856018f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 2275.352783f, 3612.639160f, 2237.202148f, 3599.333008f, -164.312347f, -164.315598f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 2285.157715f, 3661.178467f, 2242.595215f, 3612.604492f, -164.313904f, -164.315796f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 2280.112305f, 3669.000732f, 2233.740967f, 3661.144287f, -160.871429f, -164.317352f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 2280.949219f, 3682.055420f, 2235.098389f, 3668.966064f, -154.250015f, -160.874878f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 2309.260010f, 3747.352051f, 2231.468506f, 3682.020752f, -154.962830f, -154.253464f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 2318.678467f, 3750.248047f, 2305.783203f, 3717.216309f, -155.648514f, -154.857880f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 2327.495850f, 3749.142334f, 2318.644043f, 3720.817383f, -151.310715f, -155.651962f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 2338.319092f, 3749.029785f, 2327.461182f, 3724.771484f, -147.114273f, -151.314163f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 2360.703125f, 3758.143799f, 2338.284424f, 3712.792725f, -131.632538f, -147.117722f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 2392.374756f, 3764.060547f, 2360.668701f, 3702.202148f, -114.916840f, -131.635986f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);

            // Isolated northwest, flat bottom
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", 1804.764771f, 4485.881836f, 1028.035400f, 2160.111816f, -199.718567f, 250f);

            // West from middle pool, north incline westward
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 185.356033f, 1632.411011f, 146.063797f, 1612.303955f, -232.347137f, -249.409622f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 191.550156f, 1647.770874f, 132.914749f, 1632.376465f, -224.809937f, -232.350586f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 182.726883f, 1682.065918f, 130.554276f, 1647.736328f, -199.736877f, -224.813354f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 184.541107f, 1699.576782f, 137.737564f, 1682.031372f, -199.725113f, -199.740326f, ZoneLiquidSlantType.WestHighEastLow, 250f);

            // West from middle pool, south incline westward
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -228.366333f, 1618.229980f, -264.387482f, 1610.473633f, -243.045746f, -249.409546f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -228.380539f, 1629.680664f, -267.540802f, 1618.195312f, -234.239655f, -243.049179f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -231.563705f, 1634.396362f, -267.584900f, 1629.646240f, -231.540787f, -234.243103f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -230.290604f, 1646.928955f, -268.379028f, 1634.361938f, -225.572784f, -231.544205f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -233.736404f, 1660.068848f, -270.455353f, 1646.894409f, -217.694946f, -225.576233f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -219.862274f, 1667.211304f, -271.900055f, 1660.034180f, -213.190002f, -217.698380f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -219.370224f, 1681.911621f, -268.767120f, 1667.176758f, -202.577682f, -213.193466f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -215.797424f, 1708.101929f, -271.151855f, 1681.877075f, -199.718521f, -202.581146f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -206.331924f, 1783.360107f, -279.301331f, 1708.067383f, -199.718765f, -199.721954f, ZoneLiquidSlantType.WestHighEastLow, 250f);

            // West from middle pool, flat area
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", 297.959351f, 2960.479248f, -525.005676f, 1717.256104f, -199.718567f, 250f);

            // West from middle pool, final western area going up west
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -135.864883f, 2956.430176f, -199.925705f, 2941.139160f, -190.906799f, -199.253036f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -119.462013f, 2981.124268f, -203.893356f, 2956.395752f, -185.669144f, -190.910233f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -99.876221f, 3000.080322f, -186.827972f, 2981.089600f, -174.793365f, -185.672607f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -100.420441f, 3014.253174f, -191.521133f, 3000.045654f, -172.843735f, -174.796814f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -27.035999f, 3280.310303f, -195.458725f, 3014.218994f, -172.843613f, -172.847183f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -111.687752f, 3290.037354f, -176.047516f, 3280.276123f, -167.659134f, -172.847076f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -111.037529f, 3301.594971f, -171.297546f, 3290.002441f, -162.718369f, -167.662582f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -110.589546f, 3314.579834f, -175.580078f, 3301.560547f, -162.718567f, -162.721817f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -103.695282f, 3328.737061f, -171.894302f, 3314.545166f, -156.565353f, -162.722015f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -94.815514f, 3344.029053f, -181.535233f, 3328.702637f, -139.902939f, -156.568802f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -112.312477f, 3362.428711f, -174.143494f, 3343.994141f, -123.654373f, -139.906387f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -114.941490f, 3370.957764f, -170.932236f, 3362.394531f, -117.067673f, -123.657806f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -115.260696f, 3382.601807f, -175.168167f, 3370.923096f, -99.208038f, -117.071114f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -112.721649f, 3404.076172f, -182.174286f, 3382.567383f, -88.784492f, -99.211487f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -103.538589f, 3435.717773f, -285.022034f, 3404.041504f, -85.656013f, -88.787933f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -88.935081f, 3605.962646f, -306.378479f, 3435.683105f, -85.655617f, -85.659447f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -184.847061f, 3619.245850f, -256.784180f, 3605.927979f, -79.075851f, -85.659073f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -147.499664f, 3629.705811f, -237.604523f, 3619.211426f, -52.437408f, -79.079292f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -143.203629f, 3646.753906f, -215.821625f, 3629.671143f, -47.525730f, -52.440861f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -118.358414f, 3656.934082f, -209.811523f, 3646.719238f, -43.281059f, -47.529171f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -100.913780f, 3770.336914f, -215.081497f, 3656.899414f, -43.276890f, -43.284512f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -92.661430f, 3781.010254f, -184.886703f, 3770.302490f, -41.212330f, -43.280338f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -94.072327f, 3803.626465f, -182.830963f, 3780.975586f, -40.937279f, -41.215778f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -96.838440f, 3816.189209f, -176.388763f, 3803.592041f, -35.792229f, -40.940731f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -99.999901f, 3828.139404f, -175.732086f, 3816.154541f, -28.387939f, -35.795681f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -106.985603f, 3846.649902f, -172.448013f, 3828.104980f, -20.244671f, -28.391390f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -112.082268f, 3873.234131f, -175.338165f, 3846.615479f, -14.986900f, -20.248110f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -100.783234f, 3885.388916f, -179.088562f, 3873.199707f, -9.155550f, -14.990350f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -121.861862f, 3871.805664f, -167.520920f, 3859.306885f, -15.222460f, -17.181931f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -119.304779f, 3884.506348f, -162.768112f, 3871.771240f, -9.156050f, -15.225910f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -100.875458f, 3974.226807f, -171.522583f, 3884.471436f, -9.155640f, -9.159490f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -114.926376f, 3982.721680f, -168.794327f, 3974.192383f, -7.549100f, -9.159090f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -120.713287f, 4001.106445f, -167.195160f, 3982.687256f, -7.406050f, -7.552550f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -123.429543f, 4013.115479f, -166.671509f, 4001.072266f, -3.842970f, -7.409500f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -115.145218f, 4031.338867f, -169.346313f, 4013.081055f, 2.862070f, -3.846420f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -121.373497f, 4047.773438f, -171.007034f, 4031.304199f, 10.867440f, 2.858620f, ZoneLiquidSlantType.WestHighEastLow, 250f);

            // East from middle pool, climb east at the start
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 179.462265f, -176.535965f, 113.597397f, -198.304718f, -227.266724f, -249.409698f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 183.639587f, -198.270233f, 126.416389f, -219.646164f, -215.943954f, -227.270187f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 182.939240f, -219.611679f, 120.991402f, -231.401733f, -209.156219f, -215.947403f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 186.006424f, -231.367233f, 121.887627f, -273.039581f, -209.074493f, -209.159653f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 186.015213f, -273.005066f, 122.388657f, -284.387695f, -206.768265f, -209.077942f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 179.500214f, -284.353210f, 127.797684f, -292.032959f, -205.586914f, -206.771713f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 180.416718f, -291.998444f, 127.024940f, -310.519867f, -199.718719f, -205.590378f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", 183.099915f, -310.485382f, 130.514847f, -341.816406f, -199.716507f, -199.722153f, ZoneLiquidSlantType.EastHighWestLow, 250f);

            // East from middle, large flat area
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", 200.561615f, -316.720154f, -970.323547f, -3804.597656f, -199.715317f, 250f);

            // South from middle, climb south at the start
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -213.493576f, 154.825516f, -224.230698f, 87.617737f, -239.011703f, -249.409698f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -224.196213f, 152.292633f, -235.135986f, 103.417847f, -230.762360f, -239.015167f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -235.101486f, 146.103546f, -241.628036f, 98.970253f, -227.021667f, -230.765793f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -241.593567f, 151.004318f, -262.820709f, 99.587433f, -214.835281f, -227.025116f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -262.786224f, 154.804749f, -271.758301f, 100.977669f, -211.520706f, -214.838715f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -271.723816f, 155.293091f, -290.343689f, 101.396568f, -200.473679f, -211.524155f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -290.309204f, 164.487747f, -327.910583f, 104.162498f, -199.718765f, -200.477127f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);

            // South from middle, large flat
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -306.983246f, 164.048187f, -526.242371f, 30.712870f, -199.715317f, 250f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -526.142371f, 463.953613f, -3549.526123f, -1719.308960f, -199.715317f, 250f);

            // Southmost, up into dragon head
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -3544.243896f, -1644.605713f, -3557.204834f, -1686.366211f, -196.612473f, -199.440613f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -3557.170166f, -1649.538574f, -3566.607910f, -1691.911987f, -192.928818f, -196.615921f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Magma, "d_lava001", -3566.573486f, -1652.060059f, -3593.246826f, -1694.820435f, -192.132156f, -192.932266f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
        }
    }
}
