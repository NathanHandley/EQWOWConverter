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
    internal class EastFreeportZoneProperties : ZoneProperties
    {
        public EastFreeportZoneProperties() : base()
        {
            // TODO: There is a boat that goes to ocean of tears (OOT)
            SetBaseZoneProperties("freporte", "East Freeport", -648f, -1097f, -52.2f, 0, ZoneContinentType.Antonica);
            SetFogProperties(230, 200, 200, 10, 450);
            AddZoneLineBox("nro", 4152.241699f, 905.000000f, -28.031219f, ZoneLineOrientationType.South, -1336.303711f, -98.602051f, 200.000000f, -1366.303711f, -138.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 885.000000f, -28.031000f, ZoneLineOrientationType.South, -1336.303711f, -138.602051f, 200.000000f, -1366.303711f, -158.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 865.000000f, -28.031231f, ZoneLineOrientationType.South, -1336.303711f, -158.602051f, 200.000000f, -1366.303711f, -178.602066f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 845.000000f, -28.031059f, ZoneLineOrientationType.South, -1336.303711f, -178.602066f, 200.000000f, -1366.303711f, -198.602066f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 825.000000f, -28.031099f, ZoneLineOrientationType.South, -1336.303711f, -198.602066f, 200.000000f, -1366.303711f, -218.602066f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 805.000000f, -28.031151f, ZoneLineOrientationType.South, -1336.303711f, -218.602066f, 200.000000f, -1366.303711f, -238.602066f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 785.000000f, -28.031059f, ZoneLineOrientationType.South, -1336.303711f, -238.602066f, 200.000000f, -1366.303711f, -258.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 765.000000f, -28.031019f, ZoneLineOrientationType.South, -1336.303711f, -258.602051f, 200.000000f, -1366.303711f, -278.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 745.000000f, -28.031080f, ZoneLineOrientationType.South, -1336.303711f, -278.602051f, 200.000000f, -1366.303711f, -298.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 725.000000f, -28.031040f, ZoneLineOrientationType.South, -1336.303711f, -298.602051f, 200.000000f, -1366.303711f, -318.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 705.000000f, -28.030741f, ZoneLineOrientationType.South, -1336.303711f, -318.602051f, 200.000000f, -1366.303711f, -338.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 685.000000f, -28.030991f, ZoneLineOrientationType.South, -1336.303711f, -338.602051f, 200.000000f, -1366.303711f, -358.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 665.000000f, -28.031130f, ZoneLineOrientationType.South, -1336.303711f, -358.602051f, 200.000000f, -1366.303711f, -378.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 645.000000f, -28.031090f, ZoneLineOrientationType.South, -1336.303711f, -378.602051f, 200.000000f, -1366.303711f, -398.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 625.000000f, -28.031290f, ZoneLineOrientationType.South, -1336.303711f, -398.602051f, 200.000000f, -1366.303711f, -418.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 605.000000f, -28.031000f, ZoneLineOrientationType.South, -1336.303711f, -418.602051f, 200.000000f, -1366.303711f, -438.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 585.000000f, -28.031130f, ZoneLineOrientationType.South, -1336.303711f, -438.602051f, 200.000000f, -1366.303711f, -458.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 565.000000f, -28.031040f, ZoneLineOrientationType.South, -1336.303711f, -458.602051f, 200.000000f, -1366.303711f, -478.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 545.000000f, -28.031281f, ZoneLineOrientationType.South, -1336.303711f, -478.602051f, 200.000000f, -1366.303711f, -498.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 525.000000f, -28.031019f, ZoneLineOrientationType.South, -1336.303711f, -498.602051f, 200.000000f, -1366.303711f, -518.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 505.000000f, -28.031050f, ZoneLineOrientationType.South, -1336.303711f, -518.602051f, 200.000000f, -1366.303711f, -538.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 485.000000f, -28.031130f, ZoneLineOrientationType.South, -1336.303711f, -538.602051f, 200.000000f, -1366.303711f, -558.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 465.000000f, -28.031019f, ZoneLineOrientationType.South, -1336.303711f, -558.602051f, 200.000000f, -1366.303711f, -578.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 445.000000f, -28.030939f, ZoneLineOrientationType.South, -1336.303711f, -578.602051f, 200.000000f, -1366.303711f, -598.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 425.000000f, -28.031099f, ZoneLineOrientationType.South, -1336.303711f, -598.602051f, 200.000000f, -1366.303711f, -618.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 405.000000f, -28.031139f, ZoneLineOrientationType.South, -1336.303711f, -618.602051f, 200.000000f, -1366.303711f, -638.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 385.000000f, -28.030809f, ZoneLineOrientationType.South, -1336.303711f, -638.602051f, 200.000000f, -1366.303711f, -658.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 365.000000f, -28.030390f, ZoneLineOrientationType.South, -1336.303711f, -658.602051f, 200.000000f, -1366.303711f, -678.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 345.000000f, -28.031111f, ZoneLineOrientationType.South, -1336.303711f, -678.602051f, 200.000000f, -1366.303711f, -698.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 325.000000f, -28.031120f, ZoneLineOrientationType.South, -1336.303711f, -698.602051f, 200.000000f, -1366.303711f, -718.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 305.000000f, -28.031040f, ZoneLineOrientationType.South, -1336.303711f, -718.602051f, 200.000000f, -1366.303711f, -738.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 285.000000f, -28.030560f, ZoneLineOrientationType.South, -1336.303711f, -738.602051f, 200.000000f, -1366.303711f, -758.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 265.000000f, -28.031080f, ZoneLineOrientationType.South, -1336.303711f, -758.602051f, 200.000000f, -1366.303711f, -778.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 245.000000f, -28.031219f, ZoneLineOrientationType.South, -1336.303711f, -778.602051f, 200.000000f, -1366.303711f, -798.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 225.000000f, -28.031000f, ZoneLineOrientationType.South, -1336.303711f, -798.602051f, 200.000000f, -1366.303711f, -818.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 205.000000f, -28.031170f, ZoneLineOrientationType.South, -1336.303711f, -818.602051f, 200.000000f, -1366.303711f, -838.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 185.000000f, -28.031040f, ZoneLineOrientationType.South, -1336.303711f, -838.602051f, 200.000000f, -1366.303711f, -858.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 165.000000f, -28.031130f, ZoneLineOrientationType.South, -1336.303711f, -858.602051f, 200.000000f, -1366.303711f, -878.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 145.000000f, -28.031010f, ZoneLineOrientationType.South, -1336.303711f, -878.602051f, 200.000000f, -1366.303711f, -898.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 125.000000f, -28.031120f, ZoneLineOrientationType.South, -1336.303711f, -898.602051f, 200.000000f, -1366.303711f, -918.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 105.000000f, -28.031000f, ZoneLineOrientationType.South, -1336.303711f, -918.602051f, 200.000000f, -1366.303711f, -938.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 85.000000f, -28.031050f, ZoneLineOrientationType.South, -1336.303711f, -938.602051f, 200.000000f, -1366.303711f, -958.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 65.000000f, -28.031080f, ZoneLineOrientationType.South, -1336.303711f, -958.602051f, 200.000000f, -1366.303711f, -978.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 45.000000f, -28.031080f, ZoneLineOrientationType.South, -1336.303711f, -978.602051f, 200.000000f, -1366.303711f, -998.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 25.000000f, -28.031071f, ZoneLineOrientationType.South, -1336.303711f, -998.602051f, 200.000000f, -1366.303711f, -1018.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 5.000000f, -28.031040f, ZoneLineOrientationType.South, -1336.303711f, -1018.602051f, 200.000000f, -1366.303711f, -1038.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, -15.000000f, -28.031050f, ZoneLineOrientationType.South, -1336.303711f, -1038.602051f, 200.000000f, -1366.303711f, -1058.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, -35.000000f, -28.031040f, ZoneLineOrientationType.South, -1336.303711f, -1058.602051f, 200.000000f, -1366.303711f, -1078.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, -55.000000f, -28.031031f, ZoneLineOrientationType.South, -1336.303711f, -1078.602051f, 200.000000f, -1366.303711f, -1098.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, -75.000000f, -28.031031f, ZoneLineOrientationType.South, -1336.303711f, -1098.602051f, 200.000000f, -1366.303711f, -1118.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, -95.000000f, -28.031179f, ZoneLineOrientationType.South, -1336.303711f, -1118.602051f, 200.000000f, -1366.303711f, -1138.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, -115.000000f, -28.031050f, ZoneLineOrientationType.South, -1336.303711f, -1138.602051f, 200.000000f, -1366.303711f, -1158.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, -135.000000f, -28.031250f, ZoneLineOrientationType.South, -1336.303711f, -1158.602051f, 200.000000f, -1366.303711f, -1178.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, -155.000000f, -28.031130f, ZoneLineOrientationType.South, -1336.303711f, -1178.602051f, 200.000000f, -1366.303711f, -1198.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, -175.000000f, -28.031000f, ZoneLineOrientationType.South, -1336.303711f, -1198.602051f, 200.000000f, -1366.303711f, -1218.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, -195.000000f, -28.031219f, ZoneLineOrientationType.South, -1336.303711f, -1218.602051f, 200.000000f, -1366.303711f, -1238.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, -215.000000f, -28.030870f, ZoneLineOrientationType.South, -1336.303711f, -1238.602051f, 200.000000f, -1366.303711f, -1258.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, -230.000000f, -28.031080f, ZoneLineOrientationType.South, -1336.303711f, -1258.602051f, 200.000000f, -1366.303711f, -1298.602051f, -100.000000f);
            AddZoneLineBox("freportw", -82.741112f, -951.859192f, -27.999960f, ZoneLineOrientationType.East, 462.006012f, -343.977173f, -0.311040f, 433.619080f, -420.036346f, -28.500010f);
            AddZoneLineBox("freportw", -392.460449f, -622.734680f, -28.000040f, ZoneLineOrientationType.North, 154.989761f, -55.088539f, 0.501000f, 93.445801f, -70.162163f, -28.499990f);
            AddZoneLineBox("freportw", -740.355530f, -1630.233276f, -97.968758f, ZoneLineOrientationType.South, -164.879898f, 350.068451f, -85.501068f, -196.100616f, 335.683228f, -98.468712f);
            AddLiquidPlaneZLevel(LiquidType.Water, "d_ow1", 1052.497925f, -1188.711670f, -1303.691772f, -2912.643799f, -69.966743f, 300f);
            AddLiquidPlaneZLevel(LiquidType.Water, "d_ow1", -405.173492f, -1016.167297f, -613.165588f, -1201.003418f, -69.966743f, 300f);
            AddLiquidPlaneZLevel(LiquidType.Water, "d_ow1", 391.193787f, -828.030029f, -177.646881f, -1195.420410f, -69.966743f, 300f);
            AddLiquidPlaneZLevel(LiquidType.Water, "d_ow1", -212.893402f, -1035.786987f, -483.346436f, -1188.991699f, -69.966743f, 300f);
            AddLiquidPlaneZLevel(LiquidType.Water, "d_ow1", -176.359528f, -962.933838f, -213.906982f, -1189.269043f, -69.966743f, 300f);
        }
    }
}
