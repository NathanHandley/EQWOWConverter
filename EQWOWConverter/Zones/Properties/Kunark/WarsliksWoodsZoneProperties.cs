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
    internal class WarsliksWoodsZoneProperties : ZoneProperties
    {
        public WarsliksWoodsZoneProperties() : base()
        {
            AddZoneLineBox("cabwest", 870.207581f, 1143.751831f, 0.000020f, ZoneLineOrientationType.East,
                -2237.151123f, -1135.133423f, 381.612640f, -2268.348633f, -1180.958496f, 262.312653f);
            AddZoneLineBox("cabwest", 688.666626f, 1327.751099f, 0.000030f, ZoneLineOrientationType.South,
                -2420.843750f, -917.836975f, 399.112671f, -2473.554932f, -946.380981f, 262.313660f);

            // Ocean
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", 7868.704590f, 7324.179199f, 685.372620f, -6605.181641f, -217.343323f, 500f);

            // River mouth to river source
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", 704.767456f, 302.805481f, 679.463501f, 229.940414f, -210.159805f, -217.097382f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", 679.497925f, 313.543152f, 670.920349f, 243.223053f, -208.409500f, -210.163254f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", 670.954834f, 365.387665f, 644.602173f, 248.104843f, -208.406235f, -208.412933f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", 644.636658f, 358.672852f, 636.972839f, 286.180603f, -206.417175f, -208.409683f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", 637.007324f, 362.292358f, 620.531189f, 311.610565f, -204.176254f, -206.420624f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", 620.565674f, 377.671692f, 605.337952f, 325.525085f, -191.184753f, -204.179718f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", 605.372437f, 392.302521f, 589.220215f, 328.553986f, -192.436707f, -191.188202f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", 589.254700f, 389.002563f, 579.878174f, 332.921295f, -188.934616f, -192.440125f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", 579.912598f, 427.373077f, 571.169495f, 341.747131f, -182.937439f, -188.938065f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", 571.203918f, 574.874268f, 404.952515f, 351.231171f, -182.937485f, -182.940887f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", 404.987000f, 575.204102f, 375.484985f, 523.505859f, -182.913025f, -182.940933f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", 375.519531f, 571.579102f, 365.039276f, 523.631958f, -180.223526f, -182.916473f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", 365.073761f, 572.939453f, 346.508667f, 527.370911f, -180.218613f, -180.226959f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", 346.543213f, 569.709229f, 335.205231f, 525.251892f, -173.937454f, -180.222076f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", 335.239716f, 574.303528f, 325.941254f, 528.081238f, -173.937485f, -173.940903f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", 325.975739f, 573.668518f, 315.017883f, 533.258911f, -169.093704f, -173.940933f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", 315.052399f, 573.648682f, 305.500641f, 532.631042f, -169.093704f, -169.097153f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", 305.535126f, 576.796265f, 295.163269f, 533.724304f, -163.275742f, -169.097153f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", 295.197754f, 581.569824f, 289.428589f, 538.148621f, -162.312454f, -163.279190f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", 289.463074f, 580.005493f, 281.725891f, 540.889282f, -162.312469f, -162.315903f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);          
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", 281.725891f, 582.998413f, 241.191254f, 531.754211f, -162.312469f, -162.315903f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", 241.225754f, 568.726990f, 231.555267f, 531.754211f, -160.221817f, -162.315918f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", 231.589767f, 574.990662f, 216.572433f, 530.088440f, -157.874954f, -160.225266f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", 216.606918f, 578.487671f, 205.044876f, 529.884888f, -112.879471f, -157.878403f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", 205.079361f, 573.777527f, 194.712112f, 534.438232f, -108.088966f, -112.882919f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", 194.746597f, 588.682251f, -14.475600f, 521.705750f, -107.562447f, -108.092422f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -14.441110f, 616.416504f, -376.197906f, 524.901245f, -107.562447f, -107.565903f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);

            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -315.825775f, 525.557556f, -371.743988f, 465.496735f, -107.561760f, -107.565842f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -315.878082f, 465.531189f, -363.158600f, 455.709015f, -105.901100f, -107.565208f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -314.313812f, 455.743469f, -365.419708f, 445.119629f, -101.529831f, -105.904549f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -313.795929f, 445.154083f, -367.472534f, 435.639893f, -101.531181f, -101.533272f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -310.127014f, 435.674347f, -370.063843f, 425.099640f, -90.473602f, -101.534630f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -318.397247f, 425.134125f, -371.390289f, 415.625366f, -90.468719f, -90.477043f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -319.995850f, 415.659882f, -379.917786f, 405.119965f, -59.499962f, -90.472168f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -315.355743f, 405.154449f, -377.490631f, 302.894135f, -59.499962f, -59.503399f, ZoneLiquidSlantType.EastHighWestLow, 250f);

            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -346.598907f, 306.571716f, -449.523560f, 226.718857f, -59.500011f, -59.508301f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -449.489105f, 282.413940f, -511.560211f, 224.318207f, -59.499981f, -59.503448f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -511.525726f, 283.747101f, -595.152527f, 222.114075f, -59.499962f, -59.503422f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -595.118042f, 479.045532f, -792.414185f, 234.463638f, -59.499870f, -59.503410f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -792.379761f, 578.744324f, -894.940857f, 429.388916f, -59.499790f, -59.503319f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -894.906311f, 583.923645f, -1055.393311f, 519.830444f, -59.359810f, -59.503239f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1055.358765f, 589.524414f, -1067.858398f, 526.792053f, -54.975101f, -59.363258f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1067.823853f, 585.774170f, -1084.786133f, 526.559692f, -52.777760f, -54.978550f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1084.751709f, 584.345764f, -1089.249878f, 532.447388f, -49.798801f, -52.781212f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1089.215332f, 581.570557f, -1094.822388f, 532.461731f, -46.968731f, -49.802238f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1094.787842f, 585.936646f, -1184.210083f, 518.626038f, -46.968658f, -46.972179f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1184.175537f, 577.019531f, -1194.551758f, 532.942139f, -25.123119f, -46.972111f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1194.517212f, 585.218872f, -1217.900391f, 527.410583f, -15.016360f, -25.126570f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1217.865845f, 585.358337f, -1229.205200f, 533.259094f, -10.360330f, -15.019810f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1229.170776f, 584.676880f, -1243.440796f, 528.491272f, -9.624850f, -10.363770f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1243.406250f, 570.277039f, -1254.647217f, 529.814697f, 34.606998f, -9.628300f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1254.612671f, 571.369873f, -1262.354004f, 526.555359f, 37.411739f, 34.603550f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1262.319458f, 574.577393f, -1275.003540f, 527.295105f, 41.086262f, 37.408291f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1274.968994f, 577.730835f, -1284.410767f, 529.900635f, 43.344109f, 41.082809f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1284.376343f, 576.702148f, -1294.202759f, 532.300842f, 45.103580f, 43.340672f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1294.168213f, 592.463928f, -1310.274902f, 536.219666f, 44.775532f, 45.100128f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);

            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1308.000977f, 597.286133f, -1322.024780f, 540.681274f, 45.125031f, 44.686691f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1321.990234f, 606.339539f, -1330.894287f, 547.110901f, 45.125061f, 45.121578f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1330.859741f, 635.456543f, -1342.016357f, 550.660217f, 45.125290f, 45.121609f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1341.981812f, 643.174683f, -1349.490601f, 556.543091f, 45.125031f, 45.121841f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1349.456055f, 659.029053f, -1356.845947f, 570.060669f, 45.125149f, 45.121578f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1356.811401f, 661.656921f, -1365.419800f, 578.343750f, 45.125011f, 45.121700f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1365.385498f, 667.350342f, -1374.748047f, 611.796387f, 45.125069f, 45.121559f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1374.713501f, 678.625183f, -1393.913818f, 622.907104f, 45.125092f, 45.121620f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1393.879150f, 702.193787f, -1417.733276f, 634.356995f, 45.125210f, 45.121651f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1417.698730f, 774.691589f, -1463.971558f, 645.944153f, 45.125111f, 45.121761f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1463.937012f, 888.661255f, -1594.121948f, 678.847412f, 45.126541f, 45.121658f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1594.087402f, 884.915894f, -1683.037964f, 817.912292f, 45.125290f, 45.123089f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1683.003418f, 878.329895f, -1693.628540f, 832.646301f, 49.872002f, 45.121841f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1693.593872f, 877.568176f, -1712.830078f, 835.208740f, 55.405899f, 49.868549f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1712.795532f, 884.042786f, -1726.702026f, 830.872864f, 55.393539f, 55.402451f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1726.667725f, 882.721069f, -1734.607788f, 834.504333f, 57.939732f, 55.390091f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1734.573486f, 874.857544f, -1744.087402f, 830.380188f, 60.156410f, 57.936291f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1744.052856f, 870.656372f, -1759.820801f, 829.789307f, 62.068958f, 60.152962f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1759.786255f, 874.558350f, -1771.297974f, 830.797119f, 64.533783f, 62.065521f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1771.263306f, 881.620422f, -1783.334106f, 835.913452f, 66.019722f, 64.530327f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1783.299561f, 878.762085f, -1794.438354f, 835.720581f, 72.971779f, 66.016281f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1794.404053f, 890.505310f, -1807.737549f, 836.741028f, 74.323730f, 72.968330f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1807.703247f, 972.878235f, -1896.468018f, 842.674744f, 74.593849f, 74.320282f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);

            // River source lake
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "tau_stwt01", -1892.384521f, 1100.665283f, -2186.869873f, 746.563660f, 74.593903f, 40f);
        
        }
    }
}
