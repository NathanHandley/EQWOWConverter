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
    internal class NorthDesertOfRoZoneProperties : ZoneProperties
    {
        public NorthDesertOfRoZoneProperties() : base()
        {
            // Add more zone areas
            SetBaseZoneProperties("nro", "Northern Desert of Ro", 299.12f, 3537.9f, -24.5f, 0, ZoneContinentType.Antonica);
            Enable2DSoundInstances("wind_lp4", "wind_lp2", "wind_lp3");

            AddZoneArea("Field Camp", "nro-03", "nro-03", true, "", "night");
            AddZoneAreaBox("Field Camp", 3560.148438f, 583.294373f, 43.290260f, 3067.353516f, -159.970612f, -61.339088f);

            AddZoneArea("Fishing Village", "nro-02", "nro-02");
            AddZoneAreaBox("Fishing Village", 2463.193359f, -670.508850f, 91.106056f, 1987.107544f, -1045.745361f, -122.241547f);

            AddZoneArea("Haunted Ruins", "nro-04", "nro-04");
            AddZoneAreaBox("Haunted Ruins", 547.317871f, 569.168884f, 90.754204f, 285.208160f, 296.018585f, -77.585022f);

            AddZoneArea("Freeport Field", "", "", false, "", "night");
            AddZoneAreaBox("Freeport Field", 4867.109375f, 1739.585815f, 415.479492f, 2493.980713f, -1006.311584f, -119.600151f);

            AddZoneArea("Commons Tunnel");
            AddZoneAreaBox("Commons Tunnel", 4107.152832f, 4106.853027f, 152.150986f, 1893.781738f, 1889.344116f, -43.641651f);
            AddZoneAreaBox("Commons Tunnel", 2089.186279f, 1911.286377f, 58.350510f, 1809.794556f, 1460.291260f, -43.495338f);

            AddZoneArea("Desert", "nro-00", "nro-00");
            AddZoneAreaBox("Desert", 2503.120117f, 3843.550293f, 427.859924f, -2788.978027f, -1005.688293f, -222.964447f);

            AddZoneLineBox("freporte", -1316.303711f, -128.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 935.000000f, 200.000000f, 4172.241699f, 895.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -148.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 895.000000f, 200.000000f, 4172.241699f, 875.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -168.602066f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 875.000000f, 200.000000f, 4172.241699f, 855.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -188.602066f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 855.000000f, 200.000000f, 4172.241699f, 835.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -208.602066f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 835.000000f, 200.000000f, 4172.241699f, 815.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -228.602066f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 815.000000f, 200.000000f, 4172.241699f, 795.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -248.602066f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 795.000000f, 200.000000f, 4172.241699f, 775.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -268.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 775.000000f, 200.000000f, 4172.241699f, 755.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -288.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 755.000000f, 200.000000f, 4172.241699f, 735.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -308.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 735.000000f, 200.000000f, 4172.241699f, 715.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -328.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 715.000000f, 200.000000f, 4172.241699f, 695.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -348.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 695.000000f, 200.000000f, 4172.241699f, 675.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -368.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 675.000000f, 200.000000f, 4172.241699f, 655.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -388.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 655.000000f, 200.000000f, 4172.241699f, 635.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -408.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 635.000000f, 200.000000f, 4172.241699f, 615.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -428.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 615.000000f, 200.000000f, 4172.241699f, 595.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -448.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 595.000000f, 200.000000f, 4172.241699f, 575.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -468.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 575.000000f, 200.000000f, 4172.241699f, 555.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -488.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 555.000000f, 200.000000f, 4172.241699f, 535.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -508.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 535.000000f, 200.000000f, 4172.241699f, 515.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -528.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 515.000000f, 200.000000f, 4172.241699f, 495.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -548.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 495.000000f, 200.000000f, 4172.241699f, 475.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -568.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 475.000000f, 200.000000f, 4172.241699f, 455.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -588.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 455.000000f, 200.000000f, 4172.241699f, 435.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -608.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 435.000000f, 200.000000f, 4172.241699f, 415.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -628.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 415.000000f, 200.000000f, 4172.241699f, 395.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -648.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 395.000000f, 200.000000f, 4172.241699f, 375.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -668.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 375.000000f, 200.000000f, 4172.241699f, 355.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -688.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 355.000000f, 200.000000f, 4172.241699f, 335.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -708.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 335.000000f, 200.000000f, 4172.241699f, 315.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -728.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 315.000000f, 200.000000f, 4172.241699f, 295.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -748.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 295.000000f, 200.000000f, 4172.241699f, 275.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -768.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 275.000000f, 200.000000f, 4172.241699f, 255.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -788.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 255.000000f, 200.000000f, 4172.241699f, 235.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -808.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 235.000000f, 200.000000f, 4172.241699f, 215.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -828.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 215.000000f, 200.000000f, 4172.241699f, 195.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -848.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 195.000000f, 200.000000f, 4172.241699f, 175.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -868.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 175.000000f, 200.000000f, 4172.241699f, 155.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -888.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 155.000000f, 200.000000f, 4172.241699f, 135.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -908.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 135.000000f, 200.000000f, 4172.241699f, 115.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -928.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 115.000000f, 200.000000f, 4172.241699f, 95.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -948.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 95.000000f, 200.000000f, 4172.241699f, 75.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -968.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 75.000000f, 200.000000f, 4172.241699f, 55.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -988.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 55.000000f, 200.000000f, 4172.241699f, 35.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -1008.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 35.000000f, 200.000000f, 4172.241699f, 15.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -1028.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 15.000000f, 200.000000f, 4172.241699f, -5.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -1048.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, -5.000000f, 200.000000f, 4172.241699f, -25.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -1068.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, -25.000000f, 200.000000f, 4172.241699f, -45.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -1088.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, -45.000000f, 200.000000f, 4172.241699f, -65.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -1108.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, -65.000000f, 200.000000f, 4172.241699f, -85.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -1128.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, -85.000000f, 200.000000f, 4172.241699f, -105.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -1148.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, -105.000000f, 200.000000f, 4172.241699f, -125.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -1168.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, -125.000000f, 200.000000f, 4172.241699f, -145.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -1188.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, -145.000000f, 200.000000f, 4172.241699f, -165.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -1208.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, -165.000000f, 200.000000f, 4172.241699f, -185.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -1228.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, -185.000000f, 200.000000f, 4172.241699f, -205.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -1248.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, -205.000000f, 200.000000f, 4172.241699f, -225.000000f, -100.000000f);
            AddZoneLineBox("freporte", -1316.303711f, -1263.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, -225.000000f, 200.000000f, 4172.241699f, -265.000000f, -100.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 1239.252441f, 21.636360f, ZoneLineOrientationType.South, -1878f, 1500.229980f, 300.000000f, -1900f, 1231.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 1219.252441f, 22.805220f, ZoneLineOrientationType.South, -1878f, 1231.229980f, 300.000000f, -1900f, 1211.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 1199.252441f, 23.974270f, ZoneLineOrientationType.South, -1878f, 1211.229980f, 300.000000f, -1900f, 1191.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 1179.252441f, 25.147091f, ZoneLineOrientationType.South, -1878f, 1191.229980f, 300.000000f, -1900f, 1171.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 1159.252441f, 25.584009f, ZoneLineOrientationType.South, -1878f, 1171.229980f, 300.000000f, -1900f, 1151.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 1139.252441f, 25.984921f, ZoneLineOrientationType.South, -1878f, 1151.229980f, 300.000000f, -1900f, 1131.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 1119.252441f, 26.372841f, ZoneLineOrientationType.South, -1878f, 1131.229980f, 300.000000f, -1900f, 1111.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 1099.252441f, 26.760839f, ZoneLineOrientationType.South, -1878f, 1111.229980f, 300.000000f, -1900f, 1091.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 1079.252441f, 27.148649f, ZoneLineOrientationType.South, -1878f, 1091.229980f, 300.000000f, -1900f, 1071.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 1059.252441f, 27.167200f, ZoneLineOrientationType.South, -1878f, 1071.229980f, 300.000000f, -1900f, 1051.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 1039.252441f, 26.993990f, ZoneLineOrientationType.South, -1878f, 1051.229980f, 300.000000f, -1900f, 1031.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 1019.252502f, 26.600121f, ZoneLineOrientationType.South, -1878f, 1031.229980f, 300.000000f, -1900f, 1011.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 999.252502f, 26.203390f, ZoneLineOrientationType.South, -1878f, 1011.229980f, 300.000000f, -1900f, 991.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 979.252502f, 25.806890f, ZoneLineOrientationType.South, -1878f, 991.229980f, 300.000000f, -1900f, 971.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 959.252502f, 25.788059f, ZoneLineOrientationType.South, -1878f, 971.229980f, 300.000000f, -1900f, 951.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 939.252502f, 24.566401f, ZoneLineOrientationType.South, -1878f, 951.229980f, 300.000000f, -1900f, 931.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 919.252502f, 21.840040f, ZoneLineOrientationType.South, -1878f, 931.229980f, 300.000000f, -1900f, 911.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 899.252502f, 19.114100f, ZoneLineOrientationType.South, -1878f, 911.229980f, 300.000000f, -1900f, 891.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 879.252502f, 16.380091f, ZoneLineOrientationType.South, -1878f, 891.229980f, 300.000000f, -1900f, 871.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 859.252502f, 16.249411f, ZoneLineOrientationType.South, -1878f, 871.229980f, 300.000000f, -1900f, 851.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 839.252502f, 15.835400f, ZoneLineOrientationType.South, -1878f, 851.229980f, 300.000000f, -1900f, 831.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 819.252502f, 14.938530f, ZoneLineOrientationType.South, -1878f, 831.229980f, 300.000000f, -1900f, 811.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 799.252502f, 14.043360f, ZoneLineOrientationType.South, -1878f, 811.229980f, 300.000000f, -1900f, 791.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 779.252502f, 13.148440f, ZoneLineOrientationType.South, -1878f, 791.229980f, 300.000000f, -1900f, 771.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 759.252502f, 11.505960f, ZoneLineOrientationType.South, -1878f, 771.229980f, 300.000000f, -1900f, 751.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 739.252502f, 10.579040f, ZoneLineOrientationType.South, -1878f, 751.229980f, 300.000000f, -1900f, 731.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 719.252502f, 10.579120f, ZoneLineOrientationType.South, -1878f, 731.229980f, 300.000000f, -1900f, 711.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 699.252502f, 10.579510f, ZoneLineOrientationType.South, -1878f, 711.229980f, 300.000000f, -1900f, 691.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 679.252502f, 10.579230f, ZoneLineOrientationType.South, -1878f, 691.229980f, 300.000000f, -1900f, 671.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 659.252441f, 4.989020f, ZoneLineOrientationType.South, -1878f, 671.229980f, 300.000000f, -1900f, 651.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 639.252441f, 1.750520f, ZoneLineOrientationType.South, -1878f, 651.229980f, 300.000000f, -1900f, 631.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 619.252441f, 1.750570f, ZoneLineOrientationType.South, -1878f, 631.229980f, 300.000000f, -1900f, 611.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 599.252441f, 1.750080f, ZoneLineOrientationType.South, -1878f, 611.229980f, 300.000000f, -1900f, 591.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 579.252441f, 1.750100f, ZoneLineOrientationType.South, -1878f, 591.229980f, 300.000000f, -1900f, 571.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 559.252441f, 1.750250f, ZoneLineOrientationType.South, -1878f, 571.229980f, 300.000000f, -1900f, 551.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 539.252441f, 1.407590f, ZoneLineOrientationType.South, -1878f, 551.229980f, 300.000000f, -1900f, 531.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 519.252441f, 0.632410f, ZoneLineOrientationType.South, -1878f, 531.229980f, 300.000000f, -1900f, 511.230011f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 499.252441f, -0.142320f, ZoneLineOrientationType.South, -1878f, 511.230011f, 300.000000f, -1900f, 491.230011f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 479.252441f, -0.921910f, ZoneLineOrientationType.South, -1878f, 491.230011f, 300.000000f, -1900f, 471.230011f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 459.252441f, -0.588930f, ZoneLineOrientationType.South, -1878f, 471.230011f, 300.000000f, -1900f, 451.230011f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 439.252441f, -2.299280f, ZoneLineOrientationType.South, -1878f, 451.230011f, 300.000000f, -1900f, 431.230011f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 419.252441f, -6.598600f, ZoneLineOrientationType.South, -1878f, 431.230011f, 300.000000f, -1900f, 411.230011f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 399.252441f, -10.900240f, ZoneLineOrientationType.South, -1878f, 411.230011f, 300.000000f, -1900f, 391.230011f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 379.252441f, -15.201820f, ZoneLineOrientationType.South, -1878f, 391.230011f, 300.000000f, -1900f, 371.230011f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 359.252441f, -13.067110f, ZoneLineOrientationType.South, -1878f, 371.230011f, 300.000000f, -1900f, 351.230011f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 339.252441f, -11.130530f, ZoneLineOrientationType.South, -1878f, 351.230011f, 300.000000f, -1900f, 331.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 319.252441f, -9.835140f, ZoneLineOrientationType.South, -1878f, 331.229980f, 300.000000f, -1900f, 311.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 299.252441f, -8.539930f, ZoneLineOrientationType.South, -1878f, 311.229980f, 300.000000f, -1900f, 291.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 279.252441f, -7.249940f, ZoneLineOrientationType.South, -1878f, 291.229980f, 300.000000f, -1900f, 271.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 259.252441f, -4.595450f, ZoneLineOrientationType.South, -1878f, 271.229980f, 300.000000f, -1900f, 251.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 239.252441f, 1.599700f, ZoneLineOrientationType.South, -1878f, 251.229980f, 300.000000f, -1900f, 231.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 219.252441f, 12.023810f, ZoneLineOrientationType.South, -1878f, 231.229980f, 300.000000f, -1900f, 211.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 199.252441f, 22.448219f, ZoneLineOrientationType.South, -1878f, 211.229980f, 300.000000f, -1900f, 191.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 179.252441f, 32.874660f, ZoneLineOrientationType.South, -1878f, 191.229980f, 300.000000f, -1900f, 171.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 159.252441f, 38.845650f, ZoneLineOrientationType.South, -1878f, 171.229980f, 300.000000f, -1900f, 151.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 139.252441f, 42.769871f, ZoneLineOrientationType.South, -1878f, 151.229980f, 300.000000f, -1900f, 131.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 119.252441f, 44.451469f, ZoneLineOrientationType.South, -1878f, 131.229980f, 300.000000f, -1900f, 111.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 99.252441f, 46.133499f, ZoneLineOrientationType.South, -1878f, 111.229980f, 300.000000f, -1900f, 91.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 79.252441f, 47.815441f, ZoneLineOrientationType.South, -1878f, 91.229980f, 300.000000f, -1900f, 71.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 59.252441f, 47.895851f, ZoneLineOrientationType.South, -1878f, 71.229980f, 300.000000f, -1900f, 51.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 39.252441f, 47.666088f, ZoneLineOrientationType.South, -1878f, 51.229980f, 300.000000f, -1900f, 31.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, 19.252439f, 47.153721f, ZoneLineOrientationType.South, -1878f, 31.229980f, 300.000000f, -1900f, 11.229980f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -0.747560f, 46.641392f, ZoneLineOrientationType.South, -1878f, 11.229980f, 300.000000f, -1900f, -8.770020f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -20.747561f, 46.129292f, ZoneLineOrientationType.South, -1878f, -8.770020f, 300.000000f, -1900f, -28.770020f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -40.747559f, 46.222469f, ZoneLineOrientationType.South, -1878f, -28.770020f, 300.000000f, -1900f, -48.770020f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -60.747559f, 45.824760f, ZoneLineOrientationType.South, -1878f, -48.770020f, 300.000000f, -1900f, -68.770020f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -80.747559f, 44.768810f, ZoneLineOrientationType.South, -1878f, -68.770020f, 300.000000f, -1900f, -88.770020f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -100.747559f, 43.713421f, ZoneLineOrientationType.South, -1878f, -88.770020f, 300.000000f, -1900f, -108.770020f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -120.747559f, 42.660069f, ZoneLineOrientationType.South, -1878f, -108.770020f, 300.000000f, -1900f, -128.770020f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -140.747559f, 43.350559f, ZoneLineOrientationType.South, -1878f, -128.770020f, 300.000000f, -1900f, -148.770020f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -160.747559f, 43.900349f, ZoneLineOrientationType.South, -1878f, -148.770020f, 300.000000f, -1900f, -168.770020f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -180.747559f, 44.170551f, ZoneLineOrientationType.South, -1878f, -168.770020f, 300.000000f, -1900f, -188.770020f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -200.747559f, 44.441021f, ZoneLineOrientationType.South, -1878f, -188.770020f, 300.000000f, -1900f, -208.770020f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -220.747559f, 44.711781f, ZoneLineOrientationType.South, -1878f, -208.770020f, 300.000000f, -1900f, -228.770020f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -240.747559f, 44.724800f, ZoneLineOrientationType.South, -1878f, -228.770020f, 300.000000f, -1900f, -248.770020f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -260.747559f, 44.551331f, ZoneLineOrientationType.South, -1878f, -248.770020f, 300.000000f, -1900f, -268.770020f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -280.747559f, 44.164749f, ZoneLineOrientationType.South, -1878f, -268.770020f, 300.000000f, -1900f, -288.770020f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -300.747559f, 43.778198f, ZoneLineOrientationType.South, -1878f, -288.770020f, 300.000000f, -1900f, -308.770020f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -320.747559f, 43.390808f, ZoneLineOrientationType.South, -1878f, -308.770020f, 300.000000f, -1900f, -328.770020f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -340.747559f, 42.884041f, ZoneLineOrientationType.South, -1878f, -328.770020f, 300.000000f, -1900f, -348.769989f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -360.747559f, 42.472370f, ZoneLineOrientationType.South, -1878f, -348.769989f, 300.000000f, -1900f, -368.769989f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -380.747559f, 42.204800f, ZoneLineOrientationType.South, -1878f, -368.769989f, 300.000000f, -1900f, -388.769989f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -400.747559f, 41.936619f, ZoneLineOrientationType.South, -1878f, -388.769989f, 300.000000f, -1900f, -408.769989f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -420.747559f, 41.664650f, ZoneLineOrientationType.South, -1878f, -408.769989f, 300.000000f, -1900f, -428.769989f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -440.747559f, 39.041630f, ZoneLineOrientationType.South, -1878f, -428.769989f, 300.000000f, -1900f, -448.769989f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -460.747559f, 35.604820f, ZoneLineOrientationType.South, -1878f, -448.769989f, 300.000000f, -1900f, -468.769989f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -480.747559f, 31.312210f, ZoneLineOrientationType.South, -1878f, -468.769989f, 300.000000f, -1900f, -488.769989f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -500.747559f, 27.020281f, ZoneLineOrientationType.South, -1878f, -488.769989f, 300.000000f, -1900f, -508.769989f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -520.747559f, 22.728399f, ZoneLineOrientationType.South, -1878f, -508.769989f, 300.000000f, -1900f, -528.770020f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -540.747559f, 17.421190f, ZoneLineOrientationType.South, -1878f, -528.770020f, 300.000000f, -1900f, -548.770020f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -560.747559f, 13.013620f, ZoneLineOrientationType.South, -1878f, -548.770020f, 300.000000f, -1900f, -568.770020f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -580.747559f, 9.775330f, ZoneLineOrientationType.South, -1878f, -568.770020f, 300.000000f, -1900f, -588.770020f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -600.747559f, 6.536830f, ZoneLineOrientationType.South, -1878f, -588.770020f, 300.000000f, -1900f, -608.770020f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -620.747559f, 3.294970f, ZoneLineOrientationType.South, -1878f, -608.770020f, 300.000000f, -1900f, -628.770020f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -640.747559f, 2.011570f, ZoneLineOrientationType.South, -1878f, -628.770020f, 300.000000f, -1900f, -648.770020f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -660.747559f, 1.708660f, ZoneLineOrientationType.South, -1878f, -648.770020f, 300.000000f, -1900f, -668.770020f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -680.747498f, 2.490760f, ZoneLineOrientationType.South, -1878f, -668.770020f, 300.000000f, -1900f, -688.770020f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -700.747498f, 3.273800f, ZoneLineOrientationType.South, -1878f, -688.770020f, 300.000000f, -1900f, -708.770020f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -720.747498f, 4.056780f, ZoneLineOrientationType.South, -1878f, -708.770020f, 300.000000f, -1900f, -728.770020f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -740.747498f, 1.383310f, ZoneLineOrientationType.South, -1878f, -728.770020f, 300.000000f, -1900f, -748.770020f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -760.747498f, -1.239650f, ZoneLineOrientationType.South, -1878f, -748.770020f, 300.000000f, -1900f, -768.770020f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -780.747498f, -3.587200f, ZoneLineOrientationType.South, -1878f, -768.770020f, 300.000000f, -1900f, -788.770020f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -800.747498f, -5.934350f, ZoneLineOrientationType.South, -1878f, -788.770020f, 300.000000f, -1900f, -808.770020f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -820.747498f, -6.406100f, ZoneLineOrientationType.South, -1878f, -808.770020f, 300.000000f, -1900f, -828.770020f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -840.747498f, -6.406000f, ZoneLineOrientationType.South, -1878f, -828.770020f, 300.000000f, -1900f, -848.770020f, -200.000000f);
            AddZoneLineBox("oasis", 2540.233154f, -860.747498f, -6.406110f, ZoneLineOrientationType.South, -1878f, -848.770020f, 300.000000f, -1900f, -1200.770020f, -200.000000f);
            AddZoneLineBox("ecommons", -3023.223633f, -1147.192261f, 0.000050f, ZoneLineOrientationType.West, 2077.083984f, 1928.101074f, 28.065140f, 2007.522705f, 1900.196045f, -0.499880f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", 2513.325439f, -761.260254f, -2522.613525f, -1661.002563f, -6.406400f, 100f);
        }
    }
}
