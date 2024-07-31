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
    internal class QeynosHillsZoneProperties : ZoneProperties
    {
        public QeynosHillsZoneProperties() : base()
        {
            SetBaseZoneProperties("qeytoqrg", "Qeynos Hills", 196.7f, 5100.9f, -1f, 0, ZoneContinent.Antonica);
            SetFogProperties(0, 0, 0, 500, 2000);
            AddZoneLineBox("blackburrow", -163.06775f, 29.47728f, 0.000014f, ZoneLineOrientationType.West, 3442.5054f, -1124.6694f, 11.548047f, 3424.3691f, -1135.8118f, -0.4999545f);
            AddZoneLineBox("qey2hh1", 16.735029f, -634.390564f, -7.000000f, ZoneLineOrientationType.East, 1511.327637f, -2226.687500f, 67.544861f, 1317.927246f, -2336.197754f, -4.843780f);
            AddZoneLineBox("qeynos2", 1480.794556f, 1099.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 1109.592896f, 200.000000f, -360.758850f, 1089.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 1079.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 1089.592896f, 200.000000f, -360.758850f, 1069.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 1059.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 1069.592896f, 200.000000f, -360.758850f, 1049.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 1039.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 1049.592896f, 200.000000f, -360.758850f, 1029.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 1019.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 1029.592896f, 200.000000f, -360.758850f, 1009.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 999.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 1009.592896f, 200.000000f, -360.758850f, 989.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 979.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 989.592896f, 200.000000f, -360.758850f, 969.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 959.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 969.592896f, 200.000000f, -360.758850f, 949.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 939.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 949.592896f, 200.000000f, -360.758850f, 929.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 919.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 929.592896f, 200.000000f, -360.758850f, 909.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 899.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 909.592896f, 200.000000f, -360.758850f, 889.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 879.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 889.592896f, 200.000000f, -360.758850f, 869.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 859.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 869.592896f, 200.000000f, -360.758850f, 849.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 839.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 849.592896f, 200.000000f, -360.758850f, 829.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 819.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 829.592896f, 200.000000f, -360.758850f, 809.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 799.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 809.592896f, 200.000000f, -360.758850f, 789.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 779.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 789.592896f, 200.000000f, -360.758850f, 769.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 759.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 769.592896f, 200.000000f, -360.758850f, 749.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 739.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 749.592896f, 200.000000f, -360.758850f, 729.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 719.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 729.592896f, 200.000000f, -360.758850f, 709.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 699.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 709.592896f, 200.000000f, -360.758850f, 689.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 679.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 689.592896f, 200.000000f, -360.758850f, 669.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 659.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 669.592896f, 200.000000f, -360.758850f, 649.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 639.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 649.592896f, 200.000000f, -360.758850f, 629.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 619.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 629.592896f, 200.000000f, -360.758850f, 609.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 599.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 609.592896f, 200.000000f, -360.758850f, 589.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 579.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 589.592896f, 200.000000f, -360.758850f, 569.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 559.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 569.592896f, 200.000000f, -360.758850f, 549.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 539.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 549.592896f, 200.000000f, -360.758850f, 529.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 519.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 529.592896f, 200.000000f, -360.758850f, 509.592865f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 499.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 509.592865f, 200.000000f, -360.758850f, 489.592865f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 479.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 489.592865f, 200.000000f, -360.758850f, 469.592865f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 459.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 469.592865f, 200.000000f, -360.758850f, 449.592865f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 439.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 449.592865f, 200.000000f, -360.758850f, 429.592865f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 419.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 429.592865f, 200.000000f, -360.758850f, 409.592865f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 399.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 409.592865f, 200.000000f, -360.758850f, 389.592865f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 379.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 389.592865f, 200.000000f, -360.758850f, 369.592865f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 359.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 369.592865f, 200.000000f, -360.758850f, 349.592865f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 339.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 349.592865f, 200.000000f, -360.758850f, 329.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 319.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 329.592896f, 200.000000f, -360.758850f, 309.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 299.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 309.592896f, 200.000000f, -360.758850f, 289.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 279.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 289.592896f, 200.000000f, -360.758850f, 269.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 259.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 269.592896f, 200.000000f, -360.758850f, 249.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 239.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 249.592896f, 200.000000f, -360.758850f, 229.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 219.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 229.592896f, 200.000000f, -360.758850f, 209.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 199.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 209.592896f, 200.000000f, -360.758850f, 189.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 179.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 189.592896f, 200.000000f, -360.758850f, 169.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 159.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 169.592896f, 200.000000f, -360.758850f, 149.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 139.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 149.592896f, 200.000000f, -360.758850f, 129.592896f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 119.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 129.592896f, 200.000000f, -360.758850f, 109.592903f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 99.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 109.592903f, 200.000000f, -360.758850f, 89.592903f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 79.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 89.592903f, 200.000000f, -360.758850f, 69.592903f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 59.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 69.592903f, 200.000000f, -360.758850f, 49.592899f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 39.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 49.592899f, 200.000000f, -360.758850f, 29.592899f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, 19.153200f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 29.592899f, 200.000000f, -360.758850f, 9.592900f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -0.846800f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 9.592900f, 200.000000f, -360.758850f, -10.407100f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -20.846800f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -10.407100f, 200.000000f, -360.758850f, -30.407110f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -40.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -30.407110f, 200.000000f, -360.758850f, -50.407108f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -60.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -50.407108f, 200.000000f, -360.758850f, -70.407112f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -80.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -70.407112f, 200.000000f, -360.758850f, -90.407097f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -100.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -90.407097f, 200.000000f, -360.758850f, -110.407097f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -120.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -110.407097f, 200.000000f, -360.758850f, -130.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -140.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -130.407104f, 200.000000f, -360.758850f, -150.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -160.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -150.407104f, 200.000000f, -360.758850f, -170.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -180.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -170.407104f, 200.000000f, -360.758850f, -190.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -200.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -190.407104f, 200.000000f, -360.758850f, -210.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -220.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -210.407104f, 200.000000f, -360.758850f, -230.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -240.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -230.407104f, 200.000000f, -360.758850f, -250.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -260.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -250.407104f, 200.000000f, -360.758850f, -270.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -280.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -270.407104f, 200.000000f, -360.758850f, -290.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -300.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -290.407104f, 200.000000f, -360.758850f, -310.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -320.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -310.407104f, 200.000000f, -360.758850f, -330.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -340.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -330.407104f, 200.000000f, -360.758850f, -350.407135f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -360.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -350.407135f, 200.000000f, -360.758850f, -370.407135f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -380.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -370.407135f, 200.000000f, -360.758850f, -390.407135f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -400.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -390.407135f, 200.000000f, -360.758850f, -410.407135f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -420.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -410.407135f, 200.000000f, -360.758850f, -430.407135f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -440.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -430.407135f, 200.000000f, -360.758850f, -450.407135f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -460.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -450.407135f, 200.000000f, -360.758850f, -470.407135f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -480.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -470.407135f, 200.000000f, -360.758850f, -490.407135f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -500.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -490.407135f, 200.000000f, -360.758850f, -510.407135f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -520.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -510.407135f, 200.000000f, -360.758850f, -530.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -540.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -530.407104f, 200.000000f, -360.758850f, -550.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -560.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -550.407104f, 200.000000f, -360.758850f, -570.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -580.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -570.407104f, 200.000000f, -360.758850f, -590.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -600.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -590.407104f, 200.000000f, -360.758850f, -610.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -620.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -610.407104f, 200.000000f, -360.758850f, -630.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -640.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -630.407104f, 200.000000f, -360.758850f, -650.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -660.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -650.407104f, 200.000000f, -360.758850f, -670.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -680.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -670.407104f, 200.000000f, -360.758850f, -690.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -700.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -690.407104f, 200.000000f, -360.758850f, -710.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -720.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -710.407104f, 200.000000f, -360.758850f, -730.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -740.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -730.407104f, 200.000000f, -360.758850f, -750.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -760.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -750.407104f, 200.000000f, -360.758850f, -770.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -780.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -770.407104f, 200.000000f, -360.758850f, -790.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -800.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -790.407104f, 200.000000f, -360.758850f, -810.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -820.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -810.407104f, 200.000000f, -360.758850f, -830.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -840.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -830.407104f, 200.000000f, -360.758850f, -850.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -860.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -850.407104f, 200.000000f, -360.758850f, -870.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -880.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -870.407104f, 200.000000f, -360.758850f, -890.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -900.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -890.407104f, 200.000000f, -360.758850f, -910.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -920.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -910.407104f, 200.000000f, -360.758850f, -930.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -940.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -930.407104f, 200.000000f, -360.758850f, -950.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -960.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -950.407104f, 200.000000f, -360.758850f, -970.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -980.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -970.407104f, 200.000000f, -360.758850f, -990.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -1000.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -990.407104f, 200.000000f, -360.758850f, -1010.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -1020.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -1010.407104f, 200.000000f, -360.758850f, -1030.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -1040.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -1030.407104f, 200.000000f, -360.758850f, -1050.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -1060.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -1050.407104f, 200.000000f, -360.758850f, -1070.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -1080.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -1070.407104f, 200.000000f, -360.758850f, -1090.407104f, -100.000000f);
            AddZoneLineBox("qeynos2", 1480.794556f, -1099f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -1090.407104f, 200.000000f, -360.758850f, -1150f, -100.000000f);
            AddZoneLineBox("qrg", -631.004761f, 137.129745f, 0.000030f, ZoneLineOrientationType.East, 5189.661133f, 143.432114f, 7.875250f, 5173.275391f, 103.197861f, -7.093250f);
            AddLiquidPlaneZLevel(LiquidType.Water, "d_w1", 4913.403320f, 1147.879028f, 3488.182617f, 574.132324f, -24.749750f, 150f);
        }
    }
}
