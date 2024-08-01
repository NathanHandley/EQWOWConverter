//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2024 Nathan Handley
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

using EQWOWConverter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Zones.Properties
{
    internal class WestCommonsZoneProperties : ZoneProperties
    {
        public WestCommonsZoneProperties() : base()
        {
            SetBaseZoneProperties("commons", "West Commonlands", -1334.24f, 209.57f, -51.47f, 0, ZoneContinentType.Antonica);
            SetFogProperties(200, 200, 220, 10, 800);
            AddZoneLineBox("befallen", -67.97337f, 34.777237f, 0.0009409825f, ZoneLineOrientationType.South, -1161.5278f, 603.6031f, -29.81225f, -1176.3967f, 588.6972f, -42.781216f);
            AddZoneLineBox("kithicor", 1361.589966f, -1139.802246f, -52.093639f, ZoneLineOrientationType.South, 1026.848022f, 4180.347168f, 6.000250f, 987.942383f, 4119.968750f, -52.593540f);
            AddZoneLineBox("ecommons", 1158.777954f, 5081.237793f, 17.410320f, ZoneLineOrientationType.East, 1168.570435f, -1619.747314f, 200.000000f, 1148.570435f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 1138.777954f, 5081.237793f, 2.592890f, ZoneLineOrientationType.East, 1148.570435f, -1619.747314f, 200.000000f, 1128.570435f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 1118.777954f, 5081.237793f, -3.419240f, ZoneLineOrientationType.East, 1128.570435f, -1619.747314f, 200.000000f, 1108.570435f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 1098.777954f, 5081.237793f, -9.438890f, ZoneLineOrientationType.East, 1108.570435f, -1619.747314f, 200.000000f, 1088.570435f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 1078.777954f, 5081.237793f, -15.464940f, ZoneLineOrientationType.East, 1088.570435f, -1619.747314f, 200.000000f, 1068.570435f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 1058.777954f, 5081.237793f, -21.449539f, ZoneLineOrientationType.East, 1068.570435f, -1619.747314f, 200.000000f, 1048.570435f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 1038.777954f, 5081.237793f, -22.050051f, ZoneLineOrientationType.East, 1048.570435f, -1619.747314f, 200.000000f, 1028.570435f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 1018.777893f, 5081.237793f, -28.476311f, ZoneLineOrientationType.East, 1028.570435f, -1619.747314f, 200.000000f, 1008.570374f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 998.777893f, 5081.237793f, -34.902721f, ZoneLineOrientationType.East, 1008.570374f, -1619.747314f, 200.000000f, 988.570374f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 978.777893f, 5081.237793f, -41.336720f, ZoneLineOrientationType.East, 988.570374f, -1619.747314f, 200.000000f, 968.570374f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 958.777893f, 5081.237793f, -47.761551f, ZoneLineOrientationType.East, 968.570374f, -1619.747314f, 200.000000f, 948.570374f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 938.777893f, 5081.237793f, -50.970051f, ZoneLineOrientationType.East, 948.570374f, -1619.747314f, 200.000000f, 928.570374f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 918.777893f, 5081.237793f, -50.969872f, ZoneLineOrientationType.East, 928.570374f, -1619.747314f, 200.000000f, 908.570374f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 898.777893f, 5081.237793f, -50.970100f, ZoneLineOrientationType.East, 908.570374f, -1619.747314f, 200.000000f, 888.570374f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 878.777893f, 5081.237793f, -50.970169f, ZoneLineOrientationType.East, 888.570374f, -1619.747314f, 200.000000f, 868.570374f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 858.777893f, 5081.237793f, -50.987450f, ZoneLineOrientationType.East, 868.570374f, -1619.747314f, 200.000000f, 848.570374f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 838.777893f, 5081.237793f, -53.242039f, ZoneLineOrientationType.East, 848.570374f, -1619.747314f, 200.000000f, 828.570374f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 818.777893f, 5081.237793f, -49.842571f, ZoneLineOrientationType.East, 828.570374f, -1619.747314f, 200.000000f, 808.570374f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 798.777893f, 5081.237793f, -46.443378f, ZoneLineOrientationType.East, 808.570374f, -1619.747314f, 200.000000f, 788.570374f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 778.777893f, 5081.237793f, -43.043781f, ZoneLineOrientationType.East, 788.570374f, -1619.747314f, 200.000000f, 768.570374f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 758.777893f, 5081.237793f, -39.655670f, ZoneLineOrientationType.East, 768.570374f, -1619.747314f, 200.000000f, 748.570374f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 738.777893f, 5081.237793f, -38.168171f, ZoneLineOrientationType.East, 748.570374f, -1619.747314f, 200.000000f, 728.570374f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 718.777893f, 5081.237793f, -36.143929f, ZoneLineOrientationType.East, 728.570374f, -1619.747314f, 200.000000f, 708.570374f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 698.777893f, 5081.237793f, -34.119610f, ZoneLineOrientationType.East, 708.570374f, -1619.747314f, 200.000000f, 688.570374f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 678.777893f, 5081.237793f, -32.100288f, ZoneLineOrientationType.East, 688.570374f, -1619.747314f, 200.000000f, 668.570435f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 658.777954f, 5081.237793f, -30.106050f, ZoneLineOrientationType.East, 668.570435f, -1619.747314f, 200.000000f, 648.570435f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 638.777954f, 5081.237793f, -31.591780f, ZoneLineOrientationType.East, 648.570435f, -1619.747314f, 200.000000f, 628.570435f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 618.777954f, 5081.237793f, -33.615952f, ZoneLineOrientationType.East, 628.570435f, -1619.747314f, 200.000000f, 608.570435f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 598.777954f, 5081.237793f, -35.639420f, ZoneLineOrientationType.East, 608.570435f, -1619.747314f, 200.000000f, 588.570435f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 578.777954f, 5081.237793f, -37.663071f, ZoneLineOrientationType.East, 588.570435f, -1619.747314f, 200.000000f, 568.570435f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 558.777954f, 5081.237793f, -39.682640f, ZoneLineOrientationType.East, 568.570435f, -1619.747314f, 200.000000f, 548.570435f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 538.777954f, 5081.237793f, -41.615669f, ZoneLineOrientationType.East, 548.570435f, -1619.747314f, 200.000000f, 528.570435f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 518.777954f, 5081.237793f, -45.022900f, ZoneLineOrientationType.East, 528.570435f, -1619.747314f, 200.000000f, 508.570435f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 498.777954f, 5081.237793f, -48.430302f, ZoneLineOrientationType.East, 508.570435f, -1619.747314f, 200.000000f, 488.570435f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 478.777954f, 5081.237793f, -51.837990f, ZoneLineOrientationType.East, 488.570435f, -1619.747314f, 200.000000f, 468.570435f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 458.777954f, 5081.237793f, -55.218479f, ZoneLineOrientationType.East, 468.570435f, -1619.747314f, 200.000000f, 448.570435f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 438.777954f, 5081.237793f, -55.218769f, ZoneLineOrientationType.East, 448.570435f, -1619.747314f, 200.000000f, 428.570435f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 418.777954f, 5081.237793f, -55.218441f, ZoneLineOrientationType.East, 428.570435f, -1619.747314f, 200.000000f, 408.570435f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 398.777954f, 5081.237793f, -55.218472f, ZoneLineOrientationType.East, 408.570435f, -1619.747314f, 200.000000f, 388.570435f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 378.777954f, 5081.237793f, -55.218540f, ZoneLineOrientationType.East, 388.570435f, -1619.747314f, 200.000000f, 368.570435f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 358.777954f, 5081.237793f, -55.218479f, ZoneLineOrientationType.East, 368.570435f, -1619.747314f, 200.000000f, 348.570435f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 338.777954f, 5081.237793f, -55.217411f, ZoneLineOrientationType.East, 348.570435f, -1619.747314f, 200.000000f, 328.570435f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 318.777954f, 5081.237793f, -55.218578f, ZoneLineOrientationType.East, 328.570435f, -1619.747314f, 200.000000f, 308.570435f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 298.777954f, 5081.237793f, -55.218540f, ZoneLineOrientationType.East, 308.570435f, -1619.747314f, 200.000000f, 288.570435f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 278.777954f, 5081.237793f, -55.218761f, ZoneLineOrientationType.East, 288.570435f, -1619.747314f, 200.000000f, 268.570435f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 258.777954f, 5081.237793f, -55.218281f, ZoneLineOrientationType.East, 268.570435f, -1619.747314f, 200.000000f, 248.570435f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 238.777954f, 5081.237793f, -55.218632f, ZoneLineOrientationType.East, 248.570435f, -1619.747314f, 200.000000f, 228.570435f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 218.777954f, 5081.237793f, -55.218319f, ZoneLineOrientationType.East, 228.570435f, -1619.747314f, 200.000000f, 208.570435f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 198.777954f, 5081.237793f, -55.218620f, ZoneLineOrientationType.East, 208.570435f, -1619.747314f, 200.000000f, 188.570435f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 178.777954f, 5081.237793f, -55.218521f, ZoneLineOrientationType.East, 188.570435f, -1619.747314f, 200.000000f, 168.570435f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 158.777954f, 5081.237793f, -55.248749f, ZoneLineOrientationType.East, 168.570435f, -1619.747314f, 200.000000f, 148.570435f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 138.777954f, 5081.237793f, -59.047550f, ZoneLineOrientationType.East, 148.570435f, -1619.747314f, 200.000000f, 128.570435f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 118.777946f, 5081.237793f, -61.217190f, ZoneLineOrientationType.East, 128.570435f, -1619.747314f, 200.000000f, 108.570427f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 98.777946f, 5081.237793f, -63.389259f, ZoneLineOrientationType.East, 108.570427f, -1619.747314f, 200.000000f, 88.570427f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 78.777962f, 5081.237793f, -65.572212f, ZoneLineOrientationType.East, 88.570427f, -1619.747314f, 200.000000f, 68.570442f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 58.777962f, 5081.237793f, -67.756332f, ZoneLineOrientationType.East, 68.570442f, -1619.747314f, 200.000000f, 48.570438f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 28.777962f, 5081.237793f, -67.756332f, ZoneLineOrientationType.East, 48.570438f, -1619.747314f, 200.000000f, 28.570440f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", 18.777960f, 5081.237793f, -71.677933f, ZoneLineOrientationType.East, 28.570440f, -1619.747314f, 200.000000f, 8.570430f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", -1.222040f, 5081.237793f, -73.329102f, ZoneLineOrientationType.East, 8.570430f, -1619.747314f, 200.000000f, -11.429570f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", -21.222040f, 5081.237793f, -74.980499f, ZoneLineOrientationType.East, -11.429570f, -1619.747314f, 200.000000f, -31.429569f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", -41.222038f, 5081.237793f, -76.600540f, ZoneLineOrientationType.East, -31.429569f, -1619.747314f, 200.000000f, -51.429569f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", -61.222038f, 5081.237793f, -74.328918f, ZoneLineOrientationType.East, -51.429569f, -1619.747314f, 200.000000f, -71.429573f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", -81.222038f, 5081.237793f, -72.677887f, ZoneLineOrientationType.East, -71.429573f, -1619.747314f, 200.000000f, -91.429573f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", -101.222038f, 5081.237793f, -71.026268f, ZoneLineOrientationType.East, -91.429573f, -1619.747314f, 200.000000f, -111.429573f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", -121.222038f, 5081.237793f, -69.375511f, ZoneLineOrientationType.East, -111.429573f, -1619.747314f, 200.000000f, -131.429565f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", -141.222046f, 5081.237793f, -67.707352f, ZoneLineOrientationType.East, -131.429565f, -1619.747314f, 200.000000f, -151.429565f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", -161.222046f, 5081.237793f, -63.881512f, ZoneLineOrientationType.East, -151.429565f, -1619.747314f, 200.000000f, -171.429565f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", -181.222046f, 5081.237793f, -61.701981f, ZoneLineOrientationType.East, -171.429565f, -1619.747314f, 200.000000f, -191.429565f, -1649.747314f, -100.000000f);
            AddZoneLineBox("ecommons", -201.222046f, 5081.237793f, -59.530651f, ZoneLineOrientationType.East, -191.429565f, -1616.641235f, 200.000000f, -211.429565f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -221.222046f, 5081.237793f, -57.366020f, ZoneLineOrientationType.East, -211.429565f, -1616.641235f, 200.000000f, -231.429565f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -241.222046f, 5081.237793f, -55.238590f, ZoneLineOrientationType.East, -231.429565f, -1616.641235f, 200.000000f, -251.429565f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -261.222046f, 5081.237793f, -57.779572f, ZoneLineOrientationType.East, -251.429565f, -1616.641235f, 200.000000f, -271.429565f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -281.222046f, 5081.237793f, -62.237400f, ZoneLineOrientationType.East, -271.429565f, -1616.641235f, 200.000000f, -291.429565f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -301.222046f, 5081.237793f, -66.696938f, ZoneLineOrientationType.East, -291.429565f, -1616.641235f, 200.000000f, -311.429565f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -321.222046f, 5081.237793f, -71.156601f, ZoneLineOrientationType.East, -311.429565f, -1616.641235f, 200.000000f, -331.429565f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -341.222046f, 5081.237793f, -75.596397f, ZoneLineOrientationType.East, -331.429565f, -1616.641235f, 200.000000f, -351.429565f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -361.222046f, 5081.237793f, -77.522232f, ZoneLineOrientationType.East, -351.429565f, -1616.641235f, 200.000000f, -371.429565f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -381.222046f, 5081.237793f, -80.169937f, ZoneLineOrientationType.East, -371.429565f, -1616.641235f, 200.000000f, -391.429565f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -401.222046f, 5081.237793f, -82.816628f, ZoneLineOrientationType.East, -391.429565f, -1616.641235f, 200.000000f, -411.429565f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -421.222046f, 5081.237793f, -85.463211f, ZoneLineOrientationType.East, -411.429565f, -1616.641235f, 200.000000f, -431.429565f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -441.222046f, 5081.237793f, -88.071243f, ZoneLineOrientationType.East, -431.429565f, -1616.641235f, 200.000000f, -451.429565f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -461.222046f, 5081.237793f, -86.145203f, ZoneLineOrientationType.East, -451.429565f, -1616.641235f, 200.000000f, -471.429565f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -481.222046f, 5081.237793f, -83.486809f, ZoneLineOrientationType.East, -471.429565f, -1616.641235f, 200.000000f, -491.429565f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -501.222046f, 5081.237793f, -80.836227f, ZoneLineOrientationType.East, -491.429565f, -1616.641235f, 200.000000f, -511.429565f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -521.222046f, 5081.237793f, -78.198151f, ZoneLineOrientationType.East, -511.429565f, -1616.641235f, 200.000000f, -531.429565f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -541.222046f, 5081.237793f, -75.560722f, ZoneLineOrientationType.East, -531.429565f, -1616.641235f, 200.000000f, -551.429565f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -561.222046f, 5081.237793f, -72.992142f, ZoneLineOrientationType.East, -551.429565f, -1616.641235f, 200.000000f, -571.429565f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -581.222046f, 5081.237793f, -68.537811f, ZoneLineOrientationType.East, -571.429565f, -1616.641235f, 200.000000f, -591.429565f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -601.222046f, 5081.237793f, -64.086182f, ZoneLineOrientationType.East, -591.429565f, -1616.641235f, 200.000000f, -611.429565f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -621.222046f, 5081.237793f, -59.634548f, ZoneLineOrientationType.East, -611.429565f, -1616.641235f, 200.000000f, -631.429565f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -641.222046f, 5081.237793f, -55.166950f, ZoneLineOrientationType.East, -631.429565f, -1616.641235f, 200.000000f, -651.429565f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -661.222046f, 5081.237793f, -48.647518f, ZoneLineOrientationType.East, -651.429565f, -1616.641235f, 200.000000f, -671.429626f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -681.222107f, 5081.237793f, -37.260860f, ZoneLineOrientationType.East, -671.429626f, -1616.641235f, 200.000000f, -691.429626f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -701.222107f, 5081.237793f, -25.872950f, ZoneLineOrientationType.East, -691.429626f, -1616.641235f, 200.000000f, -711.429626f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -721.222107f, 5081.237793f, -14.485130f, ZoneLineOrientationType.East, -711.429626f, -1616.641235f, 200.000000f, -731.429626f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -741.222107f, 5081.237793f, -3.148240f, ZoneLineOrientationType.East, -731.429626f, -1616.641235f, 200.000000f, -751.429626f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -761.222107f, 5081.237793f, 1.779870f, ZoneLineOrientationType.East, -751.429626f, -1616.641235f, 200.000000f, -771.429626f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -781.222107f, 5081.237793f, 8.531090f, ZoneLineOrientationType.East, -771.429626f, -1616.641235f, 200.000000f, -791.429626f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -801.222107f, 5081.237793f, 15.288440f, ZoneLineOrientationType.East, -791.429626f, -1616.641235f, 200.000000f, -811.429626f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -821.222107f, 5081.237793f, 22.050070f, ZoneLineOrientationType.East, -811.429626f, -1616.641235f, 200.000000f, -831.429626f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -841.222107f, 5081.237793f, 28.719170f, ZoneLineOrientationType.East, -831.429626f, -1616.641235f, 200.000000f, -851.429626f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -861.222107f, 5081.237793f, 23.791540f, ZoneLineOrientationType.East, -851.429626f, -1616.641235f, 200.000000f, -871.429626f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -881.222107f, 5081.237793f, 17.033131f, ZoneLineOrientationType.East, -871.429626f, -1616.641235f, 200.000000f, -891.429626f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -901.222107f, 5081.237793f, 10.274550f, ZoneLineOrientationType.East, -891.429626f, -1616.641235f, 200.000000f, -911.429626f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -921.222107f, 5081.237793f, 3.515960f, ZoneLineOrientationType.East, -911.429626f, -1616.641235f, 200.000000f, -931.429626f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -941.222107f, 5081.237793f, -3.184360f, ZoneLineOrientationType.East, -931.429626f, -1616.641235f, 200.000000f, -951.429626f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -961.222107f, 5081.237793f, -2.896860f, ZoneLineOrientationType.East, -951.429626f, -1616.641235f, 200.000000f, -971.429626f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -981.222107f, 5081.237793f, -4.134420f, ZoneLineOrientationType.East, -971.429626f, -1616.641235f, 200.000000f, -991.429626f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -1001.222107f, 5081.237793f, -5.372180f, ZoneLineOrientationType.East, -991.429626f, -1616.641235f, 200.000000f, -1011.429626f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -1021.222107f, 5081.237793f, -6.611620f, ZoneLineOrientationType.East, -1011.429626f, -1616.641235f, 200.000000f, -1031.429565f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -1041.222046f, 5081.237793f, -7.814110f, ZoneLineOrientationType.East, -1031.429565f, -1616.641235f, 200.000000f, -1051.429565f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -1061.222046f, 5081.237793f, -1.553180f, ZoneLineOrientationType.East, -1051.429565f, -1616.641235f, 200.000000f, -1071.429565f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -1081.222046f, 5081.237793f, 6.107420f, ZoneLineOrientationType.East, -1071.429565f, -1616.641235f, 200.000000f, -1091.429565f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -1101.222046f, 5081.237793f, 13.762880f, ZoneLineOrientationType.East, -1091.429565f, -1616.641235f, 200.000000f, -1111.429565f, -1646.641235f, -100.000000f);
            AddZoneLineBox("ecommons", -1121.222046f, 5081.237793f, 21.418200f, ZoneLineOrientationType.East, -1111.429565f, -1616.641235f, 200.000000f, -1151.429565f, -1646.641235f, -100.000000f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", 377.188812f, 3223.834473f, -460.310211f, 2200.130127f, -66.088470f, 500f);
        }
    }
}
