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
            // TODO: Clip water surface geometry near Dalnir

            SetZonewideEnvironmentAsOutdoorsNoSky(26, 26, 26, ZoneFogType.Heavy, 1f);

            SetZonewideAmbienceSound("", "darkwds1");
            Enable2DSoundInstances("wtr_pool", "streamsm", "wind_lp2", "wind_lp4");

            AddZoneLineBox("cabwest", 870.207581f, 1143.751831f, 0.000020f, ZoneLineOrientationType.East,
                -2237.151123f, -1135.133423f, 381.612640f, -2268.348633f, -1180.958496f, 262.312653f);
            AddZoneLineBox("cabwest", 688.666626f, 1327.751099f, 0.000030f, ZoneLineOrientationType.South,
                -2420.843750f, -917.836975f, 399.112671f, -2473.554932f, -946.380981f, 262.313660f);
            AddZoneLineBox("lakeofillomen", 4240.515625f, 1077.258179f, 47.625381f, ZoneLineOrientationType.East,
                -3611.486084f, 1122.609375f, 128.471436f, -3852.555420f, 1033.167114f, 30.334721f);
            AddZoneLineBox("overthere", 62.135712f, -3927.562988f, 225.718140f, ZoneLineOrientationType.West,
                727.563416f, 4580.014648f, 696.519043f, -408.334595f, 3995.653076f, 201.359283f);
            AddZoneLineBox("dalnir", 6.630970f, 62.347679f, 0.004290f, ZoneLineOrientationType.East,
                2592.124023f, 4573.925293f, -223.179184f, 2541.819824f, 4508.052246f, -250.936874f);
            AddZoneLineBox("fieldofbone", 1413.743408f, 3554.795898f, -0.695750f, ZoneLineOrientationType.East,
                1922.704956f, -3688.215576f, 919.834229f, 830.668396f, -4323.236816f, -194.401077f);

            // Ocean
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", 7868.704590f, 3807.709961f, 685.372620f, -6605.181641f, -217.343323f, 500f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", 7868.704590f, 7324.179199f, 3210.743896f, 3807.709961f, -217.343323f, 500f);

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

            // River source lake (North)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "tau_stwt01", -1892.384521f, 1100.665283f, -2186.869873f, 746.563660f, 74.593903f, 40f);

            // West river fork towards other source
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -27.414471f, 706.799377f, -152.214462f, 604.894714f, -107.562447f, -107.565941f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -120.170860f, 805.514709f, -258.724731f, 706.764954f, -107.510979f, -107.565903f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -226.907379f, 815.770691f, -265.902313f, 805.480225f, -100.028198f, -107.514427f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -225.687698f, 825.346130f, -268.661652f, 815.736267f, -100.031143f, -100.031647f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -217.750000f, 844.889709f, -265.119873f, 825.311707f, -100.031227f, -100.034576f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -216.608673f, 855.154541f, -258.499115f, 844.855225f, -98.676804f, -100.034683f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -215.974579f, 863.737976f, -262.591522f, 855.120056f, -97.757637f, -98.680252f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -220.821777f, 879.495911f, -272.251648f, 863.703491f, -96.686409f, -97.761093f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -225.746140f, 904.571777f, -272.513336f, 879.461426f, -96.159393f, -96.689850f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -221.035507f, 915.650818f, -263.818726f, 904.537170f, -62.073601f, -96.162842f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -223.750076f, 924.821533f, -261.734344f, 915.616211f, -60.556431f, -62.077049f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -223.474854f, 932.602173f, -261.101410f, 924.787048f, -60.468739f, -60.559872f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -220.040680f, 966.709900f, -258.665894f, 932.567627f, -60.464790f, -60.472191f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -216.815033f, 1005.648804f, -260.423187f, 966.675354f, -60.468761f, -60.468231f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -223.427643f, 1030.810059f, -294.995148f, 1005.614319f, -60.468719f, -60.472210f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -237.966156f, 1132.851318f, -355.408386f, 1030.775513f, -60.468739f, -60.472160f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -316.002441f, 1315.138672f, -449.388947f, 1132.816895f, -60.468449f, -60.472179f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "tau_stwt01", -345.432312f, 1172.962769f, -372.350372f, 1064.579834f, -60.468449f, 20f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -422.484192f, 1326.021606f, -463.285004f, 1315.104126f, -55.723679f, -60.471901f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -415.861389f, 1335.558350f, -462.104553f, 1325.987183f, -55.715618f, -55.727131f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -417.307892f, 1345.812622f, -456.978638f, 1335.523926f, -49.609322f, -55.719070f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -413.662109f, 1355.037598f, -459.700562f, 1345.778076f, -49.437450f, -49.612759f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -416.339081f, 1366.172852f, -461.049133f, 1355.003174f, -40.403191f, -49.440899f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -420.436920f, 1375.311523f, -461.805908f, 1366.138184f, -37.796249f, -40.406639f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -420.434448f, 1395.586670f, -467.023285f, 1375.276978f, -37.562420f, -37.799702f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -422.745148f, 1405.499878f, -464.144989f, 1395.552368f, -13.968750f, -37.565868f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -421.937225f, 1520.663208f, -576.014099f, 1405.465332f, -13.968770f, -13.972190f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -514.812622f, 1546.175537f, -569.856079f, 1520.628662f, -8.806310f, -13.972210f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -518.733521f, 1555.156006f, -559.203003f, 1546.140991f, -8.749940f, -8.809750f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -518.674927f, 1562.488525f, -558.083557f, 1555.121582f, -6.680810f, -8.753390f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -520.374390f, 1569.227417f, -560.858826f, 1562.454102f, -4.686770f, -6.684260f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -523.669495f, 1586.358765f, -572.718750f, 1569.192749f, 1.750180f, -4.690220f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -525.130310f, 1605.459229f, -567.945557f, 1586.324219f, 2.175770f, 1.746730f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -521.239929f, 1616.828125f, -562.930603f, 1605.424683f, 46.406502f, 2.172320f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -522.247131f, 1705.970093f, -561.119934f, 1616.793701f, 46.406410f, 46.403049f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -523.364868f, 1729.020752f, -600.156738f, 1703.660767f, 46.406380f, 46.402962f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -520.880798f, 1800.784302f, -665.782288f, 1728.986450f, 46.406368f, 46.402931f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -612.705627f, 1914.041870f, -779.615417f, 1800.749756f, 46.406380f, 46.402931f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -722.117188f, 1930.019653f, -766.534180f, 1914.007568f, 49.469608f, 46.402931f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -714.514099f, 1953.468140f, -760.111938f, 1929.985107f, 49.500092f, 49.466160f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -718.517029f, 1963.208008f, -759.150269f, 1953.433594f, 52.661320f, 49.496639f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -718.768311f, 1968.720337f, -761.502136f, 1958.990723f, 53.465488f, 51.245350f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -724.179016f, 1988.292114f, -771.600159f, 1968.685913f, 53.500439f, 53.462029f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -725.225769f, 1998.330688f, -765.295044f, 1988.257812f, 56.401138f, 53.496990f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -723.096558f, 2004.257812f, -763.787659f, 1998.296143f, 58.687981f, 56.397690f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -721.510620f, 2013.591064f, -764.009338f, 2004.223145f, 58.687881f, 58.684540f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -720.796936f, 2024.457275f, -762.091370f, 2013.556641f, 64.281273f, 58.684441f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -715.540344f, 2034.191895f, -761.373291f, 2024.422729f, 64.281487f, 64.277817f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -716.834534f, 2044.367554f, -756.757141f, 2034.157471f, 70.723892f, 64.278038f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -716.405273f, 2054.405029f, -756.534973f, 2044.333008f, 72.689148f, 70.720444f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -715.785828f, 2063.016846f, -762.722534f, 2054.370605f, 72.689110f, 72.685707f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -721.462158f, 2074.762939f, -758.891785f, 2062.982178f, 120.909348f, 72.685661f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -721.354187f, 2078.802979f, -759.789917f, 2074.728271f, 120.906677f, 120.905899f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -721.260803f, 2104.583496f, -761.440796f, 2078.768555f, 120.906464f, 120.903236f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -723.248413f, 2127.444824f, -785.046692f, 2104.549072f, 120.906563f, 120.903023f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -719.368774f, 2290.792480f, -872.910706f, 2127.410400f, 120.906479f, 120.903107f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -827.965942f, 2297.087891f, -866.310791f, 2290.757812f, 124.671753f, 120.903030f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -822.790649f, 2305.396484f, -864.018372f, 2297.053223f, 128.284058f, 124.668297f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -820.419373f, 2315.105225f, -864.747070f, 2305.361816f, 128.281464f, 128.280609f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -819.945129f, 2325.168701f, -862.020081f, 2315.070801f, 145.783890f, 128.278015f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -813.678650f, 2334.593262f, -860.416138f, 2325.134033f, 145.781738f, 145.780441f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -817.109070f, 2345.493408f, -854.106812f, 2334.558838f, 186.103638f, 145.778290f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -815.398132f, 2355.703125f, -856.781433f, 2345.458740f, 190.215103f, 186.100174f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -814.774475f, 2363.976807f, -863.355652f, 2355.668457f, 190.219086f, 190.211655f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -821.984314f, 2375.545166f, -858.569214f, 2363.942139f, 252.545303f, 190.215637f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -823.273621f, 2379.215332f, -859.307434f, 2375.510742f, 254.414719f, 252.541855f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -821.985291f, 2384.937500f, -861.717773f, 2379.180664f, 257.342163f, 254.411285f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -822.949219f, 2393.757812f, -861.296509f, 2384.902832f, 258.880585f, 257.338715f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -820.885376f, 2405.168457f, -864.924500f, 2393.723389f, 259.093872f, 258.877136f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -820.658691f, 2431.860596f, -881.016052f, 2405.134033f, 259.094055f, 259.090424f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -836.887573f, 2445.992432f, -910.259705f, 2431.826172f, 259.093903f, 259.090607f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -848.017456f, 2459.572998f, -941.312134f, 2445.957764f, 259.093933f, 259.090454f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -870.832703f, 2470.810547f, -950.259338f, 2459.538574f, 259.093933f, 259.090485f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -876.236694f, 2486.879639f, -962.386169f, 2470.776123f, 259.093872f, 259.090485f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -907.231812f, 2504.147217f, -965.018555f, 2486.845215f, 259.093872f, 259.090424f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -916.952942f, 2528.577881f, -984.448486f, 2504.112793f, 259.093719f, 259.090424f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -923.715149f, 2605.364014f, -1064.445435f, 2528.543457f, 259.093933f, 259.090271f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1007.477051f, 2689.345215f, -1084.583374f, 2605.329590f, 259.153015f, 259.090485f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1023.615173f, 2696.107422f, -1069.622681f, 2689.310791f, 260.033783f, 259.149567f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1020.405579f, 2703.693848f, -1063.686768f, 2696.072998f, 260.855072f, 260.030334f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1019.003784f, 2754.469971f, -1055.544434f, 2743.094971f, 351.679565f, 305.574188f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1016.366028f, 2764.209961f, -1057.838745f, 2754.435547f, 368.657318f, 351.676117f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1022.489014f, 2714.520752f, -1062.773682f, 2703.659180f, 294.039429f, 260.851624f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1021.727173f, 2724.788086f, -1059.733521f, 2714.486328f, 301.796295f, 294.035980f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1021.632935f, 2728.901855f, -1060.654175f, 2724.753662f, 303.449188f, 301.792877f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1017.801147f, 2734.520020f, -1058.376343f, 2728.867432f, 305.733032f, 303.445740f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1018.883179f, 2743.709961f, -1053.795288f, 2734.485352f, 305.549530f, 305.729584f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1018.350159f, 2764.216553f, -1054.000000f, 2743.675293f, 368.656189f, 305.546082f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1020.894348f, 2774.180908f, -1057.801147f, 2764.182129f, 368.656494f, 368.652771f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1020.448486f, 2784.659180f, -1059.880859f, 2774.146240f, 375.968750f, 368.653046f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1019.543335f, 2804.967041f, -1066.005493f, 2784.624756f, 375.971405f, 375.965332f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1024.047363f, 2828.675293f, -1083.464355f, 2804.932373f, 375.968933f, 375.967957f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1030.311401f, 2905.270508f, -1167.345215f, 2828.640625f, 375.968933f, 375.965485f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1098.391357f, 3104.788574f, -1177.072266f, 2905.236084f, 375.968872f, 375.965485f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1113.995605f, 3404.364746f, -1369.252930f, 3104.753906f, 375.968933f, 375.965454f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1320.643799f, 3414.109863f, -1362.615845f, 3404.330322f, 380.383545f, 375.965485f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1320.627075f, 3434.867188f, -1361.509644f, 3414.075439f, 423.877655f, 380.380127f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1317.242554f, 3444.922607f, -1354.662231f, 3434.832520f, 430.093933f, 423.874146f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1316.865601f, 3453.939209f, -1354.634033f, 3444.888428f, 430.093811f, 430.090485f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1317.462402f, 3464.447266f, -1358.769409f, 3453.904541f, 474.099640f, 430.090363f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1319.008545f, 3474.912598f, -1358.766235f, 3464.413086f, 482.533234f, 474.096161f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1320.520996f, 3483.813965f, -1361.686768f, 3474.878174f, 482.531372f, 482.529755f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1321.744751f, 3494.530273f, -1364.147949f, 3483.779541f, 529.847168f, 482.527924f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1323.547485f, 3505.064209f, -1361.622925f, 3494.496094f, 535.844238f, 529.843689f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1320.510742f, 3526.257812f, -1364.139893f, 3505.030029f, 535.843750f, 535.840759f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1313.221680f, 3627.685547f, -1380.145630f, 3526.223633f, 535.843933f, 535.840271f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1312.015503f, 3756.432861f, -1382.423340f, 3627.650635f, 535.843933f, 535.840515f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1319.832764f, 3906.741211f, -1383.997803f, 3756.398682f, 535.870972f, 535.840515f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1316.570923f, 3942.244873f, -1397.807495f, 3906.706299f, 535.875122f, 535.867493f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "tau_stwt01", -1324.020630f, 4181.326172f, -1680.196533f, 3942.210449f, 535.871643f, 535.871643f, ZoneLiquidSlantType.WestHighEastLow, 250f);

            AddDiscardGeometryBox(3457.450928f, -2906.989990f,  1407.824463f, 765.621033f, -4829.969238f, 445.377960f); // East floating trees
            AddDiscardGeometryBox(2791.680664f, 4618.571777f, 940.455688f, 2595.156006f, 4286.382324f, 424.159668f); // West floating tree
        }
    }
}

