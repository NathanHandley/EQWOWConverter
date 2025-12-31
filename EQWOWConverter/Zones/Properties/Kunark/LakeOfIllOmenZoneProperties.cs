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
    internal class LakeOfIllOmenZoneProperties : ZoneProperties
    {
        public LakeOfIllOmenZoneProperties() : base()
        {
            SetZonewideAmbienceSound("", "darkwds1");
            Enable2DSoundInstances("wind_lp3", "wind_lp4", "lakelap3");

            AddZoneLineBox("cabwest", -802.654480f, 767.458740f, -0.000070f, ZoneLineOrientationType.North, 6577.715820f, -6613.837891f, 145.213730f, 6533.130859f, -6645.066895f, 34.593719f);
            AddZoneLineBox("cabwest", -985.943787f, 584.806458f, 0.000380f, ZoneLineOrientationType.East, 6344.193848f, -6799.043945f, 182.103806f, 6315.685547f, -6843.227051f, 34.595600f);
            AddZoneLineBox("firiona", 1292.149292f, 3306.101807f, 183.356964f, ZoneLineOrientationType.East,
                1431.185181f, -4380.426758f, 487.776611f, 662.696167f, -4773.780273f, 190.810226f);
            AddZoneLineBox("frontiermtns", -660.689392f, -4141.462402f, 109.009789f, ZoneLineOrientationType.West,
                -114.581352f, 3693.796875f, 366.001709f, -891.873901f, 3351.493652f, 4.848970f);
            AddZoneLineBox("warslikswood", -3657.177002f, 1188.529663f, 50.093948f, ZoneLineOrientationType.West,
                4400.304688f, 1339.878052f, 205.636200f, 4284.637695f, 1054.167725f, 28.188250f);

            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", 1502.120117f, 1338.126587f, -2497.938965f, -1861.968262f, 65.187592f, 400f); // Middle lake

            // River Start - Climb north
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1057.156494f, -1526.031250f, 1046.186157f, -1568.453613f, 76.780998f, 65.334740f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1066.649658f, -1521.955566f, 1057.121948f, -1568.549805f, 76.784126f, 76.777550f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1076.824097f, -1519.179199f, 1066.615112f, -1569.547485f, 80.597839f, 76.780693f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1086.842896f, -1516.144775f, 1076.789673f, -1567.664429f, 83.023109f, 80.594391f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1096.760742f, -1516.949463f, 1086.808350f, -1561.331421f, 83.843063f, 83.019661f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1105.867920f, -1519.828003f, 1096.726196f, -1567.141968f, 86.238419f, 83.839607f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1126.663452f, -1516.984375f, 1105.833374f, -1566.271851f, 94.151321f, 86.234970f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1136.972534f, -1522.680420f, 1126.628906f, -1567.579956f, 94.250252f, 94.147873f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1146.936646f, -1523.908691f, 1136.937988f, -1567.909302f, 97.464706f, 94.246811f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1170.584106f, -1523.551636f, 1146.902100f, -1575.275024f, 97.468987f, 97.461273f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1197.110962f, -1509.715942f, 1170.549683f, -1579.910522f, 100.907097f, 97.465538f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1206.902222f, -1517.064941f, 1197.076416f, -1563.296509f, 103.656517f, 100.903664f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1231.722168f, -1514.631226f, 1206.867798f, -1574.010254f, 106.120491f, 103.653084f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1246.729614f, -1523.603638f, 1231.687622f, -1569.901978f, 106.158882f, 106.117050f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1258.524048f, -1519.658691f, 1246.695068f, -1569.262695f, 108.923119f, 106.155441f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1268.454224f, -1521.962769f, 1258.489502f, -1568.633179f, 110.597733f, 108.919670f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1277.989014f, -1522.005127f, 1268.419678f, -1565.929443f, 110.625092f, 110.594276f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1299.003662f, -1514.345459f, 1277.954468f, -1567.513062f, 122.218842f, 110.621643f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1308.349121f, -1516.845703f, 1298.969116f, -1563.387695f, 122.218758f, 122.215393f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1328.595581f, -1515.814453f, 1308.314575f, -1564.971069f, 129.947830f, 122.215317f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1338.067749f, -1521.082275f, 1328.561157f, -1569.135254f, 129.968811f, 129.944382f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1348.735718f, -1525.098389f, 1338.033203f, -1566.092773f, 132.027603f, 129.965378f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1368.290039f, -1508.830200f, 1348.701172f, -1573.425903f, 133.667709f, 132.024170f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1395.081299f, -1512.130615f, 1368.255493f, -1571.455688f, 136.605453f, 133.664261f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1407.491089f, -1519.840210f, 1395.046753f, -1559.152588f, 137.065903f, 136.602005f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1423.321777f, -1515.364502f, 1407.456421f, -1557.912354f, 140.687592f, 137.062454f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1511.767822f, -1511.982300f, 1423.287231f, -1594.153809f, 140.785187f, 140.684143f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);

            // River - Climb East to the smaller pond
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1516.639526f, -1593.098999f, 1475.529175f, -1606.193726f, 143.579330f, 140.684113f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1526.462891f, -1606.159180f, 1471.671875f, -1655.633301f, 143.593964f, 143.575882f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1528.309814f, -1655.598755f, 1476.245605f, -1676.088745f, 151.295792f, 143.590515f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1524.292480f, -1676.054443f, 1471.291992f, -1701.217896f, 151.424042f, 151.292343f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1518.215942f, -1701.183350f, 1466.221069f, -1716.196167f, 155.187607f, 151.420593f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1518.393066f, -1716.161621f, 1467.084595f, -1732.052612f, 156.314560f, 155.184158f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1525.939941f, -1732.018066f, 1469.795654f, -1755.823975f, 160.443863f, 156.311111f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1525.480591f, -1755.789429f, 1474.599487f, -1770.614380f, 165.089249f, 160.440430f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1522.018188f, -1770.579834f, 1470.630615f, -1786.682129f, 166.753403f, 165.085800f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1518.323242f, -1786.647705f, 1470.012207f, -1795.344116f, 166.722275f, 166.749954f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1517.777588f, -1795.309570f, 1470.660034f, -1807.325806f, 169.509262f, 166.718826f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1520.187256f, -1807.291260f, 1472.258423f, -1826.778687f, 169.562561f, 169.505814f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1526.617065f, -1826.744141f, 1473.507324f, -1847.689575f, 174.843857f, 169.559113f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1523.251953f, -1847.655029f, 1480.025635f, -1856.589966f, 174.843933f, 174.840393f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1523.785889f, -1856.555420f, 1473.140137f, -1877.911255f, 182.595596f, 174.840500f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1521.499634f, -1877.876587f, 1473.242676f, -1897.704956f, 182.648743f, 182.592133f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1517.983643f, -1897.670410f, 1471.300537f, -1912.363892f, 184.881332f, 182.645294f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1519.310547f, -1912.329224f, 1465.721069f, -1927.143677f, 185.877441f, 184.877899f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1528.332520f, -1927.109131f, 1463.965332f, -1996.879639f, 192.216858f, 185.873993f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1787.785400f, -1996.845337f, 1473.899414f, -2195.024658f, 192.281525f, 192.213394f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1729.038940f, -2194.990234f, 1673.592529f, -2207.724854f, 195.245987f, 192.278076f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1730.143555f, -2207.690186f, 1668.770996f, -2256.136719f, 195.250107f, 195.242538f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1729.055786f, -2256.102051f, 1669.691650f, -2277.569336f, 203.000107f, 195.246658f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1724.791504f, -2277.534912f, 1661.539307f, -2297.830078f, 203.000107f, 202.996658f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1733.182983f, -2297.795654f, 1662.803467f, -2356.951660f, 212.010864f, 202.996658f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1731.478271f, -2356.916992f, 1670.169800f, -2396.611572f, 212.062607f, 212.007446f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1719.684692f, -2396.577148f, 1671.941162f, -2409.284912f, 214.931946f, 212.059143f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1730.658081f, -2409.250488f, 1670.918457f, -2457.908691f, 214.937683f, 214.928482f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1725.784058f, -2457.874268f, 1676.013062f, -2479.897705f, 222.690903f, 214.934235f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1726.740845f, -2479.863281f, 1662.028687f, -2523.035156f, 222.707458f, 222.687454f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1727.153809f, -2523.000732f, 1665.306763f, -2550.450439f, 226.033127f, 222.704025f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1733.770752f, -2550.416016f, 1665.003662f, -2597.162842f, 226.031967f, 226.029678f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1718.252808f, -2597.128174f, 1672.392212f, -2608.752441f, 231.000580f, 226.028503f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1719.572266f, -2608.718018f, 1675.859619f, -2617.291260f, 231.000046f, 230.997147f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1720.808105f, -2617.256836f, 1675.131958f, -2628.294678f, 234.500916f, 230.996582f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1726.093750f, -2628.260254f, 1674.633179f, -2697.706787f, 240.000214f, 234.497467f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1727.597168f, -2697.672607f, 1660.036743f, -2787.904297f, 239.995834f, 239.996765f, ZoneLiquidSlantType.EastHighWestLow, 250f);

            // Pond
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 2160.357422f, -2787.869873f, 1569.326050f, -3408.995605f, 239.826523f, 239.992401f, ZoneLiquidSlantType.EastHighWestLow, 250f);

            // River from pond, going east
            // TODO: Look into this, may have issues with plane angle
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1813.692139f, -3408.960938f, 1695.484009f, -3457.455322f, 239.844315f, 239.823074f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1755.496460f, -3457.420898f, 1687.469727f, -3473.864014f, 239.843964f, 239.840897f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1733.052856f, -3473.829346f, 1678.287720f, -3494.605713f, 239.964401f, 239.840515f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1733.524048f, -3494.571289f, 1676.670288f, -3507.121582f, 239.718933f, 239.960953f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1727.096802f, -3507.087158f, 1667.950562f, -3615.213379f, 239.603073f, 239.715500f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1715.344116f, -3615.178955f, 1666.947144f, -3635.630615f, 234.218933f, 239.599640f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1715.584595f, -3635.596191f, 1666.356445f, -3665.436279f, 234.020706f, 234.215500f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1715.612061f, -3665.401611f, 1671.523560f, -3675.562988f, 230.719238f, 234.017242f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1715.953003f, -3675.528320f, 1671.647705f, -3685.122314f, 230.513214f, 230.715775f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1715.353394f, -3685.087646f, 1674.402344f, -3694.684570f, 225.750259f, 230.509766f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1717.225586f, -3694.650146f, 1670.632812f, -3742.989014f, 225.750137f, 225.746826f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1722.477539f, -3742.954590f, 1669.822510f, -3758.243164f, 223.534027f, 225.746674f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1723.755859f, -3758.208740f, 1673.554077f, -3774.631348f, 222.406464f, 223.530579f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1721.692749f, -3774.596680f, 1670.244507f, -3814.375732f, 222.256287f, 222.403015f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1713.609253f, -3814.341553f, 1665.611572f, -3833.578125f, 214.851135f, 222.252823f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1720.002930f, -3833.543701f, 1666.068970f, -3884.402832f, 214.524582f, 214.847687f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1717.357422f, -3884.368408f, 1672.623413f, -3896.118164f, 211.780746f, 214.521133f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1717.979004f, -3896.083740f, 1645.276123f, -3963.036377f, 211.955261f, 211.777298f, ZoneLiquidSlantType.WestHighEastLow, 250f);

            // Remaining river
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1613.246826f, -3928.399902f, 1266.356323f, -4396.597168f, 211.687607f, 211.684494f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1314.427612f, -4396.562988f, 1266.572510f, -4416.723145f, 211.605606f, 211.684158f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1313.520630f, -4416.688965f, 1268.381348f, -4436.162598f, 206.250275f, 211.602142f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1317.944946f, -4436.127930f, 1266.869263f, -4446.029785f, 206.156296f, 206.246841f, ZoneLiquidSlantType.WestHighEastLow, 250f);

            // Added bits of river
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_stwt01", 1647.470215f, -3925.617920f, 1612.418457f, -3997.034180f, 211.687607f, 20f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1705.320215f, -3814.825195f, 1670.027100f, -3833.773193f, 214.669403f, 222.305893f, ZoneLiquidSlantType.WestHighEastLow, 250f);
        }
    }
}
