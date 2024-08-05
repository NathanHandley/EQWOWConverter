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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Zones.Properties
{
    internal class EastCommonsZoneProperties : ZoneProperties
    {
        public EastCommonsZoneProperties() : base()
        {
            SetBaseZoneProperties("ecommons", "East Commonlands", -1485f, 9.2f, -51f, 0, ZoneContinentType.Antonica);
            AddZoneLineBox("nro", 2033.690186f, 1875.838257f, 0.000120f, ZoneLineOrientationType.East, -3004.062744f, -1183.421265f, 28.469000f, -3087.551270f, -1212.701660f, -0.499900f);
            AddZoneLineBox("nektulos", -2686.337891f, -529.951477f, -21.531050f, ZoneLineOrientationType.West, 1591.733643f, 696.248291f, 23.553110f, 1554.580811f, 679.187378f, -22.031260f);
            AddZoneLineBox("commons", 1158.570435f, -1599.747314f, 6.933440f, ZoneLineOrientationType.West, 1168.777954f, 5131.237793f, 200.000000f, 1148.777954f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 1138.570435f, -1599.747314f, 0.914720f, ZoneLineOrientationType.West, 1148.777954f, 5131.237793f, 200.000000f, 1128.777954f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 1118.570435f, -1599.747314f, -5.104220f, ZoneLineOrientationType.West, 1128.777954f, 5131.237793f, 200.000000f, 1108.777954f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 1098.570435f, -1599.747314f, -11.122930f, ZoneLineOrientationType.West, 1108.777954f, 5131.237793f, 200.000000f, 1088.777954f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 1078.570435f, -1599.747314f, -17.141710f, ZoneLineOrientationType.West, 1088.777954f, 5131.237793f, 200.000000f, 1068.777954f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 1058.570435f, -1599.747314f, -23.122900f, ZoneLineOrientationType.West, 1068.777954f, 5131.237793f, 200.000000f, 1048.777954f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 1038.570435f, -1599.747314f, -29.554140f, ZoneLineOrientationType.West, 1048.777954f, 5131.237793f, 200.000000f, 1028.777954f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 1018.570374f, -1599.747314f, -35.985538f, ZoneLineOrientationType.West, 1028.777954f, 5131.237793f, 200.000000f, 1008.777893f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 998.570374f, -1599.747314f, -42.416641f, ZoneLineOrientationType.West, 1008.777893f, 5131.237793f, 200.000000f, 988.777893f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 978.570374f, -1599.747314f, -48.847809f, ZoneLineOrientationType.West, 988.777893f, 5131.237793f, 200.000000f, 968.777893f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 958.570374f, -1599.747314f, -55.186001f, ZoneLineOrientationType.West, 968.777893f, 5131.237793f, 200.000000f, 948.777893f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 938.570374f, -1599.747314f, -55.184929f, ZoneLineOrientationType.West, 948.777893f, 5131.237793f, 200.000000f, 928.777893f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 918.570374f, -1599.747314f, -55.185928f, ZoneLineOrientationType.West, 928.777893f, 5131.237793f, 200.000000f, 908.777893f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 898.570374f, -1599.747314f, -55.186050f, ZoneLineOrientationType.West, 908.777893f, 5131.237793f, 200.000000f, 888.777893f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 878.570374f, -1599.747314f, -55.186008f, ZoneLineOrientationType.West, 888.777893f, 5131.237793f, 200.000000f, 868.777893f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 858.570374f, -1599.747314f, -55.167400f, ZoneLineOrientationType.West, 868.777893f, 5131.237793f, 200.000000f, 848.777893f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 838.570374f, -1599.747314f, -51.761181f, ZoneLineOrientationType.West, 848.777893f, 5131.237793f, 200.000000f, 828.777893f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 818.570374f, -1599.747314f, -48.354771f, ZoneLineOrientationType.West, 828.777893f, 5131.237793f, 200.000000f, 808.777893f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 798.570374f, -1599.747314f, -44.948689f, ZoneLineOrientationType.West, 808.777893f, 5131.237793f, 200.000000f, 788.777893f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 778.570374f, -1599.747314f, -41.542381f, ZoneLineOrientationType.West, 788.777893f, 5131.237793f, 200.000000f, 768.777893f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 758.570374f, -1599.747314f, -38.165630f, ZoneLineOrientationType.West, 768.777893f, 5131.237793f, 200.000000f, 748.777893f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 738.570374f, -1599.747314f, -36.147209f, ZoneLineOrientationType.West, 748.777893f, 5131.237793f, 200.000000f, 728.777893f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 718.570374f, -1599.747314f, -34.128349f, ZoneLineOrientationType.West, 728.777893f, 5131.237793f, 200.000000f, 708.777893f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 698.570374f, -1599.747314f, -32.109650f, ZoneLineOrientationType.West, 708.777893f, 5131.237793f, 200.000000f, 688.777893f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 678.570374f, -1599.747314f, -30.090839f, ZoneLineOrientationType.West, 688.777893f, 5131.237793f, 200.000000f, 668.777954f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 658.570435f, -1599.747314f, -28.141970f, ZoneLineOrientationType.West, 668.777954f, 5131.237793f, 200.000000f, 648.777954f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 638.570435f, -1599.747314f, -30.156441f, ZoneLineOrientationType.West, 648.777954f, 5131.237793f, 200.000000f, 628.777954f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 618.570435f, -1599.747314f, -32.170738f, ZoneLineOrientationType.West, 628.777954f, 5131.237793f, 200.000000f, 608.777954f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 598.570435f, -1599.747314f, -34.185089f, ZoneLineOrientationType.West, 608.777954f, 5131.237793f, 200.000000f, 588.777954f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 578.570435f, -1599.747314f, -36.207230f, ZoneLineOrientationType.West, 588.777954f, 5131.237793f, 200.000000f, 568.777954f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 558.570435f, -1599.747314f, -38.249901f, ZoneLineOrientationType.West, 568.777954f, 5131.237793f, 200.000000f, 548.777954f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 538.570435f, -1599.747314f, -41.656132f, ZoneLineOrientationType.West, 548.777954f, 5131.237793f, 200.000000f, 528.777954f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 518.570435f, -1599.747314f, -45.062489f, ZoneLineOrientationType.West, 528.777954f, 5131.237793f, 200.000000f, 508.777954f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 498.570435f, -1599.747314f, -48.468410f, ZoneLineOrientationType.West, 508.777954f, 5131.237793f, 200.000000f, 488.777954f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 478.570435f, -1599.747314f, -51.874989f, ZoneLineOrientationType.West, 488.777954f, 5131.237793f, 200.000000f, 468.777954f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 458.570435f, -1599.747314f, -55.217369f, ZoneLineOrientationType.West, 468.777954f, 5131.237793f, 200.000000f, 448.777954f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 438.570435f, -1599.747314f, -55.218510f, ZoneLineOrientationType.West, 448.777954f, 5131.237793f, 200.000000f, 428.777954f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 418.570435f, -1599.747314f, -55.217442f, ZoneLineOrientationType.West, 428.777954f, 5131.237793f, 200.000000f, 408.777954f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 398.570435f, -1599.747314f, -55.218632f, ZoneLineOrientationType.West, 408.777954f, 5131.237793f, 200.000000f, 388.777954f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 378.570435f, -1599.747314f, -55.218700f, ZoneLineOrientationType.West, 388.777954f, 5131.237793f, 200.000000f, 368.777954f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 358.570435f, -1599.747314f, -55.218700f, ZoneLineOrientationType.West, 368.777954f, 5131.237793f, 200.000000f, 348.777954f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 338.570435f, -1599.747314f, -55.218449f, ZoneLineOrientationType.West, 348.777954f, 5131.237793f, 200.000000f, 328.777954f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 318.570435f, -1599.747314f, -55.218658f, ZoneLineOrientationType.West, 328.777954f, 5131.237793f, 200.000000f, 308.777954f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 298.570435f, -1599.747314f, -55.218651f, ZoneLineOrientationType.West, 308.777954f, 5131.237793f, 200.000000f, 288.777954f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 278.570435f, -1599.747314f, -55.217579f, ZoneLineOrientationType.West, 288.777954f, 5131.237793f, 200.000000f, 268.777954f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 258.570435f, -1599.747314f, -55.218681f, ZoneLineOrientationType.West, 268.777954f, 5131.237793f, 200.000000f, 248.777954f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 238.570435f, -1599.747314f, -55.218658f, ZoneLineOrientationType.West, 248.777954f, 5131.237793f, 200.000000f, 228.777954f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 218.570435f, -1599.747314f, -55.218658f, ZoneLineOrientationType.West, 228.777954f, 5131.237793f, 200.000000f, 208.777954f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 198.570435f, -1599.747314f, -55.218418f, ZoneLineOrientationType.West, 208.777954f, 5131.237793f, 200.000000f, 188.777954f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 178.570435f, -1599.747314f, -55.218670f, ZoneLineOrientationType.West, 188.777954f, 5131.237793f, 200.000000f, 168.777954f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 158.570435f, -1599.747314f, -55.271351f, ZoneLineOrientationType.West, 168.777954f, 5131.237793f, 200.000000f, 148.777954f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 138.570435f, -1599.747314f, -57.446281f, ZoneLineOrientationType.West, 148.777954f, 5131.237793f, 200.000000f, 128.777954f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 118.570427f, -1599.747314f, -59.621250f, ZoneLineOrientationType.West, 128.777954f, 5131.237793f, 200.000000f, 108.777946f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 98.570427f, -1599.747314f, -61.796280f, ZoneLineOrientationType.West, 108.777946f, 5131.237793f, 200.000000f, 88.777946f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 78.570442f, -1599.747314f, -63.971249f, ZoneLineOrientationType.West, 88.777946f, 5131.237793f, 200.000000f, 68.777962f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 58.570438f, -1599.747314f, -66.141312f, ZoneLineOrientationType.West, 68.777962f, 5131.237793f, 200.000000f, 48.777962f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 38.570438f, -1599.747314f, -67.787682f, ZoneLineOrientationType.West, 48.777962f, 5131.237793f, 200.000000f, 28.777960f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", 18.570440f, -1599.747314f, -69.434021f, ZoneLineOrientationType.West, 28.777960f, 5131.237793f, 200.000000f, 8.777960f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -1.429570f, -1599.747314f, -71.086342f, ZoneLineOrientationType.West, 8.777960f, 5131.237793f, 200.000000f, -11.222040f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -21.429569f, -1599.747314f, -72.738922f, ZoneLineOrientationType.West, -11.222040f, 5131.237793f, 200.000000f, -31.222040f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -41.429569f, -1599.747314f, -74.325768f, ZoneLineOrientationType.West, -31.222040f, 5131.237793f, 200.000000f, -51.222038f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -61.429569f, -1599.747314f, -72.675987f, ZoneLineOrientationType.West, -51.222038f, 5131.237793f, 200.000000f, -71.222038f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -81.429573f, -1599.747314f, -71.025978f, ZoneLineOrientationType.West, -71.222038f, 5131.237793f, 200.000000f, -91.222038f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -101.429573f, -1599.747314f, -69.375992f, ZoneLineOrientationType.West, -91.222038f, 5131.237793f, 200.000000f, -111.222038f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -121.429573f, -1599.747314f, -67.725998f, ZoneLineOrientationType.West, -111.222038f, 5131.237793f, 200.000000f, -131.222046f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -141.429565f, -1599.747314f, -66.053642f, ZoneLineOrientationType.West, -131.222046f, 5131.237793f, 200.000000f, -151.222046f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -161.429565f, -1599.747314f, -63.878689f, ZoneLineOrientationType.West, -151.222046f, 5131.237793f, 200.000000f, -171.222046f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -181.429565f, -1599.747314f, -61.703491f, ZoneLineOrientationType.West, -171.222046f, 5131.237793f, 200.000000f, -191.222046f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -201.235886f, -1594.215820f, -58.965080f, ZoneLineOrientationType.West, -191.222046f, 5131.237793f, 200.000000f, -211.222046f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -221.429565f, -1596.641235f, -57.032490f, ZoneLineOrientationType.West, -211.222046f, 5131.237793f, 200.000000f, -231.222046f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -241.429565f, -1596.641235f, -55.300110f, ZoneLineOrientationType.West, -231.222046f, 5131.237793f, 200.000000f, -251.222046f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -261.429565f, -1596.641235f, -59.739422f, ZoneLineOrientationType.West, -251.222046f, 5131.237793f, 200.000000f, -271.222046f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -281.429565f, -1596.641235f, -64.194572f, ZoneLineOrientationType.West, -271.222046f, 5131.237793f, 200.000000f, -291.222046f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -301.429565f, -1596.641235f, -68.656754f, ZoneLineOrientationType.West, -291.222046f, 5131.237793f, 200.000000f, -311.222046f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -321.429565f, -1596.641235f, -73.119377f, ZoneLineOrientationType.West, -311.222046f, 5131.237793f, 200.000000f, -331.222046f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -341.429565f, -1596.641235f, -77.264351f, ZoneLineOrientationType.West, -331.222046f, 5131.237793f, 200.000000f, -351.222046f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -361.429565f, -1596.641235f, -79.907959f, ZoneLineOrientationType.West, -351.222046f, 5131.237793f, 200.000000f, -371.222046f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -381.429565f, -1596.641235f, -82.551781f, ZoneLineOrientationType.West, -371.222046f, 5131.237793f, 200.000000f, -391.222046f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -401.429565f, -1596.641235f, -85.195572f, ZoneLineOrientationType.West, -391.222046f, 5131.237793f, 200.000000f, -411.222046f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -421.429565f, -1596.641235f, -87.839287f, ZoneLineOrientationType.West, -411.222046f, 5131.237793f, 200.000000f, -431.222046f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -441.429565f, -1596.641235f, -90.279900f, ZoneLineOrientationType.West, -431.222046f, 5131.237793f, 200.000000f, -451.222046f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -461.429565f, -1596.641235f, -87.635872f, ZoneLineOrientationType.West, -451.222046f, 5131.237793f, 200.000000f, -471.222046f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -481.429565f, -1596.641235f, -84.992378f, ZoneLineOrientationType.West, -471.222046f, 5131.237793f, 200.000000f, -491.222046f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -501.429565f, -1596.641235f, -82.348679f, ZoneLineOrientationType.West, -491.222046f, 5131.237793f, 200.000000f, -511.222046f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -521.429565f, -1596.641235f, -79.704811f, ZoneLineOrientationType.West, -511.222046f, 5131.237793f, 200.000000f, -531.222046f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -541.429565f, -1596.641235f, -77.134331f, ZoneLineOrientationType.West, -531.222046f, 5131.237793f, 200.000000f, -551.222046f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -561.429565f, -1596.641235f, -72.686951f, ZoneLineOrientationType.West, -551.222046f, 5131.237793f, 200.000000f, -571.222046f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -581.429565f, -1596.641235f, -68.228188f, ZoneLineOrientationType.West, -571.222046f, 5131.237793f, 200.000000f, -591.222046f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -601.429565f, -1596.641235f, -63.769482f, ZoneLineOrientationType.West, -591.222046f, 5131.237793f, 200.000000f, -611.222046f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -621.429565f, -1596.641235f, -59.310661f, ZoneLineOrientationType.West, -611.222046f, 5131.237793f, 200.000000f, -631.222046f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -641.429565f, -1596.641235f, -54.353611f, ZoneLineOrientationType.West, -631.222046f, 5131.237793f, 200.000000f, -651.222046f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -661.429565f, -1596.641235f, -42.966110f, ZoneLineOrientationType.West, -651.222046f, 5131.237793f, 200.000000f, -671.222107f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -681.429626f, -1596.641235f, -31.578560f, ZoneLineOrientationType.West, -671.222107f, 5131.237793f, 200.000000f, -691.222107f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -701.429626f, -1596.641235f, -20.191050f, ZoneLineOrientationType.West, -691.222107f, 5131.237793f, 200.000000f, -711.222107f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -721.429626f, -1596.641235f, -8.803610f, ZoneLineOrientationType.West, -711.222107f, 5131.237793f, 200.000000f, -731.222107f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -741.429626f, -1596.641235f, 2.213060f, ZoneLineOrientationType.West, -731.222107f, 5131.237793f, 200.000000f, -751.222107f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -761.429626f, -1596.641235f, 8.973560f, ZoneLineOrientationType.West, -751.222107f, 5131.237793f, 200.000000f, -771.222107f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -781.429626f, -1596.641235f, 15.734280f, ZoneLineOrientationType.West, -771.222107f, 5131.237793f, 200.000000f, -791.222107f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -801.429626f, -1596.641235f, 22.494841f, ZoneLineOrientationType.West, -791.222107f, 5131.237793f, 200.000000f, -811.222107f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -821.429626f, -1596.641235f, 29.255489f, ZoneLineOrientationType.West, -811.222107f, 5131.237793f, 200.000000f, -831.222107f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -841.429626f, -1596.641235f, 34.901020f, ZoneLineOrientationType.West, -831.222107f, 5131.237793f, 200.000000f, -851.222107f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -861.429626f, -1596.641235f, 28.144600f, ZoneLineOrientationType.West, -851.222107f, 5131.237793f, 200.000000f, -871.222107f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -881.429626f, -1596.641235f, 21.388330f, ZoneLineOrientationType.West, -871.222107f, 5131.237793f, 200.000000f, -891.222107f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -901.429626f, -1596.641235f, 14.632090f, ZoneLineOrientationType.West, -891.222107f, 5131.237793f, 200.000000f, -911.222107f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -921.429626f, -1596.641235f, 7.875830f, ZoneLineOrientationType.West, -911.222107f, 5131.237793f, 200.000000f, -931.222107f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -941.429626f, -1596.641235f, 0.970300f, ZoneLineOrientationType.West, -931.222107f, 5131.237793f, 200.000000f, -951.222107f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -961.429626f, -1596.641235f, -0.273860f, ZoneLineOrientationType.West, -951.222107f, 5131.237793f, 200.000000f, -971.222107f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -981.429626f, -1596.641235f, -1.517720f, ZoneLineOrientationType.West, -971.222107f, 5131.237793f, 200.000000f, -991.222107f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -1001.429626f, -1596.641235f, -2.761340f, ZoneLineOrientationType.West, -991.222107f, 5131.237793f, 200.000000f, -1011.222107f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -1021.429626f, -1596.641235f, -4.005160f, ZoneLineOrientationType.West, -1011.222107f, 5131.237793f, 200.000000f, -1031.222046f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -1041.429565f, -1596.641235f, -4.855840f, ZoneLineOrientationType.West, -1031.222046f, 5131.237793f, 200.000000f, -1051.222046f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -1061.429565f, -1596.641235f, 2.797780f, ZoneLineOrientationType.West, -1051.222046f, 5131.237793f, 200.000000f, -1071.222046f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -1081.429565f, -1596.641235f, 10.450930f, ZoneLineOrientationType.West, -1071.222046f, 5131.237793f, 200.000000f, -1091.222046f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -1101.429565f, -1596.641235f, 18.104589f, ZoneLineOrientationType.West, -1091.222046f, 5131.237793f, 200.000000f, -1111.222046f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -1121.429565f, -1596.641235f, 25.758080f, ZoneLineOrientationType.West, -1111.222046f, 5131.237793f, 200.000000f, -1131.222046f, 5101.237793f, -100.000000f);
            AddZoneLineBox("commons", -1141.061279f, -1596.579956f, 33.031120f, ZoneLineOrientationType.West, -1131.222046f, 5131.237793f, 200.000000f, -1151.222046f, 5101.237793f, -100.000000f);
            AddZoneLineBox("freportw", 577.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 587.000000f, -1600.000000f, 200.000000f, 567.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", 557.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 567.000000f, -1600.000000f, 200.000000f, 547.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", 537.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 547.000000f, -1600.000000f, 200.000000f, 527.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", 517.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 527.000000f, -1600.000000f, 200.000000f, 507.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", 497.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 507.000000f, -1600.000000f, 200.000000f, 487.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", 477.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 487.000000f, -1600.000000f, 200.000000f, 467.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", 457.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 467.000000f, -1600.000000f, 200.000000f, 447.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", 437.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 447.000000f, -1600.000000f, 200.000000f, 427.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", 417.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 427.000000f, -1600.000000f, 200.000000f, 407.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", 397.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 407.000000f, -1600.000000f, 200.000000f, 387.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", 377.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 387.000000f, -1600.000000f, 200.000000f, 367.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", 357.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 367.000000f, -1600.000000f, 200.000000f, 347.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", 337.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 347.000000f, -1600.000000f, 200.000000f, 327.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", 317.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 327.000000f, -1600.000000f, 200.000000f, 307.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", 297.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 307.000000f, -1600.000000f, 200.000000f, 287.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", 277.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 287.000000f, -1600.000000f, 200.000000f, 267.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", 257.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 267.000000f, -1600.000000f, 200.000000f, 247.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", 237.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 247.000000f, -1600.000000f, 200.000000f, 227.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", 217.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 227.000000f, -1600.000000f, 200.000000f, 207.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", 197.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 207.000000f, -1600.000000f, 200.000000f, 187.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", 177.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 187.000000f, -1600.000000f, 200.000000f, 167.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", 157.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 167.000000f, -1600.000000f, 200.000000f, 147.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", 137.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 147.000000f, -1600.000000f, 200.000000f, 127.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", 117.772163f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 127.000000f, -1600.000000f, 200.000000f, 107.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", 97.772163f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 107.000000f, -1600.000000f, 200.000000f, 87.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", 77.772163f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 87.000000f, -1600.000000f, 200.000000f, 67.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", 57.772160f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 67.000000f, -1600.000000f, 200.000000f, 47.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", 37.772160f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 47.000000f, -1600.000000f, 200.000000f, 27.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", 17.772160f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 27.000000f, -1600.000000f, 200.000000f, 7.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", -2.227840f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 7.000000f, -1600.000000f, 200.000000f, -13.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", -22.227850f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -13.000000f, -1600.000000f, 200.000000f, -33.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", -42.227852f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -33.000000f, -1600.000000f, 200.000000f, -53.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", -62.227852f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -53.000000f, -1600.000000f, 200.000000f, -73.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", -82.227852f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -73.000000f, -1600.000000f, 200.000000f, -93.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", -102.227837f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -93.000000f, -1600.000000f, 200.000000f, -113.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", -122.227837f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -113.000000f, -1600.000000f, 200.000000f, -133.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", -142.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -133.000000f, -1600.000000f, 200.000000f, -153.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", -162.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -153.000000f, -1600.000000f, 200.000000f, -173.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", -182.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -173.000000f, -1600.000000f, 200.000000f, -193.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", -202.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -193.000000f, -1600.000000f, 200.000000f, -213.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", -222.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -213.000000f, -1600.000000f, 200.000000f, -233.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", -242.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -233.000000f, -1600.000000f, 200.000000f, -253.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", -262.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -253.000000f, -1600.000000f, 200.000000f, -273.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", -282.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -273.000000f, -1600.000000f, 200.000000f, -293.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", -302.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -293.000000f, -1600.000000f, 200.000000f, -313.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", -322.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -313.000000f, -1600.000000f, 200.000000f, -333.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", -342.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -333.000000f, -1600.000000f, 200.000000f, -353.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", -362.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -353.000000f, -1600.000000f, 200.000000f, -373.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", -382.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -373.000000f, -1600.000000f, 200.000000f, -393.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", -402.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -393.000000f, -1600.000000f, 200.000000f, -413.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", -422.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -413.000000f, -1600.000000f, 200.000000f, -433.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", -442.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -433.000000f, -1600.000000f, 200.000000f, -453.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", -462.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -453.000000f, -1600.000000f, 200.000000f, -473.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", -482.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -473.000000f, -1600.000000f, 200.000000f, -493.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", -502.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -493.000000f, -1600.000000f, 200.000000f, -513.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", -522.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -513.000000f, -1600.000000f, 200.000000f, -533.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", -542.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -533.000000f, -1600.000000f, 200.000000f, -553.000000f, -1630.000000f, -100.000000f);
            AddZoneLineBox("freportw", -562.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -553.000000f, -1600.000000f, 200.000000f, -593.000000f, -1630.000000f, -100.000000f);
        }
    }
}
