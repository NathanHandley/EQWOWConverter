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
    internal class WestFreeportZoneProperties : ZoneProperties
    {
        public WestFreeportZoneProperties() : base()
        {
            // TODO: Gaps in the geometry at -325.65 -797.158 -32
            SetBaseZoneProperties("freportw", "West Freeport", 181f, 335f, -24f, 0, ZoneContinentType.Antonica);
            SetZonewideEnvironmentAsOutdoors();
            AddZoneLineBox("ecommons", 577.000000f, -1580.000000f, -54.468632f, ZoneLineOrientationType.West, 587.772156f, 841.873230f, 200.000000f, 567.772156f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", 557.000000f, -1580.000000f, -54.468632f, ZoneLineOrientationType.West, 567.772156f, 841.873230f, 200.000000f, 547.772156f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", 537.000000f, -1580.000000f, -54.468670f, ZoneLineOrientationType.West, 547.772156f, 841.873230f, 200.000000f, 527.772156f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", 517.000000f, -1580.000000f, -54.468670f, ZoneLineOrientationType.West, 527.772156f, 841.873230f, 200.000000f, 507.772156f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", 497.000000f, -1580.000000f, -54.468658f, ZoneLineOrientationType.West, 507.772156f, 841.873230f, 200.000000f, 487.772156f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", 477.000000f, -1580.000000f, -54.468620f, ZoneLineOrientationType.West, 487.772156f, 841.873230f, 200.000000f, 467.772156f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", 457.000000f, -1580.000000f, -54.468651f, ZoneLineOrientationType.West, 467.772156f, 841.873230f, 200.000000f, 447.772156f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", 437.000000f, -1580.000000f, -54.468689f, ZoneLineOrientationType.West, 447.772156f, 841.873230f, 200.000000f, 427.772156f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", 417.000000f, -1580.000000f, -54.467621f, ZoneLineOrientationType.West, 427.772156f, 841.873230f, 200.000000f, 407.772156f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", 397.000000f, -1580.000000f, -54.468639f, ZoneLineOrientationType.West, 407.772156f, 841.873230f, 200.000000f, 387.772156f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", 377.000000f, -1580.000000f, -54.468578f, ZoneLineOrientationType.West, 387.772156f, 841.873230f, 200.000000f, 367.772156f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", 357.000000f, -1580.000000f, -52.021858f, ZoneLineOrientationType.West, 367.772156f, 841.873230f, 200.000000f, 347.772156f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", 337.000000f, -1580.000000f, -54.468632f, ZoneLineOrientationType.West, 347.772156f, 841.873230f, 200.000000f, 327.772156f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", 317.000000f, -1580.000000f, -54.468632f, ZoneLineOrientationType.West, 327.772156f, 841.873230f, 200.000000f, 307.772156f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", 297.000000f, -1580.000000f, -54.467548f, ZoneLineOrientationType.West, 307.772156f, 841.873230f, 200.000000f, 287.772156f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", 277.000000f, -1580.000000f, -54.468739f, ZoneLineOrientationType.West, 287.772156f, 841.873230f, 200.000000f, 267.772156f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", 257.000000f, -1580.000000f, -54.468639f, ZoneLineOrientationType.West, 267.772156f, 841.873230f, 200.000000f, 247.772156f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", 237.000000f, -1580.000000f, -54.468578f, ZoneLineOrientationType.West, 247.772156f, 841.873230f, 200.000000f, 227.772156f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", 217.000000f, -1580.000000f, -54.467510f, ZoneLineOrientationType.West, 227.772156f, 841.873230f, 200.000000f, 207.772156f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", 197.000000f, -1580.000000f, -54.468491f, ZoneLineOrientationType.West, 207.772156f, 841.873230f, 200.000000f, 187.772156f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", 177.000000f, -1580.000000f, -54.468739f, ZoneLineOrientationType.West, 187.772156f, 841.873230f, 200.000000f, 167.772156f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", 157.000000f, -1580.000000f, -54.468601f, ZoneLineOrientationType.West, 167.772156f, 841.873230f, 200.000000f, 147.772156f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", 137.000000f, -1580.000000f, -54.468670f, ZoneLineOrientationType.West, 147.772156f, 841.873230f, 200.000000f, 127.772163f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", 117.000000f, -1580.000000f, -54.468521f, ZoneLineOrientationType.West, 127.772163f, 841.873230f, 200.000000f, 107.772163f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", 97.000000f, -1580.000000f, -54.467442f, ZoneLineOrientationType.West, 107.772163f, 841.873230f, 200.000000f, 87.772163f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", 77.000000f, -1580.000000f, -54.468540f, ZoneLineOrientationType.West, 87.772163f, 841.873230f, 200.000000f, 67.772163f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", 57.000000f, -1580.000000f, -54.468601f, ZoneLineOrientationType.West, 67.772163f, 841.873230f, 200.000000f, 47.772160f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", 37.000000f, -1580.000000f, -54.468658f, ZoneLineOrientationType.West, 47.772160f, 841.873230f, 200.000000f, 27.772160f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", 17.000000f, -1580.000000f, -54.468601f, ZoneLineOrientationType.West, 27.772160f, 841.873230f, 200.000000f, 7.772160f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", -3.000000f, -1580.000000f, -54.468632f, ZoneLineOrientationType.West, 7.772160f, 841.873230f, 200.000000f, -12.227850f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", -23.000000f, -1580.000000f, -54.468712f, ZoneLineOrientationType.West, -12.227840f, 841.873230f, 200.000000f, -32.227852f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", -43.000000f, -1580.000000f, -54.468632f, ZoneLineOrientationType.West, -32.227852f, 841.873230f, 200.000000f, -52.227852f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", -63.000000f, -1580.000000f, -54.468658f, ZoneLineOrientationType.West, -52.227852f, 841.873230f, 200.000000f, -72.227852f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", -83.000000f, -1580.000000f, -54.468632f, ZoneLineOrientationType.West, -72.227852f, 841.873230f, 200.000000f, -92.227837f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", -103.000000f, -1580.000000f, -54.468651f, ZoneLineOrientationType.West, -92.227837f, 841.873230f, 200.000000f, -112.227837f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", -123.000000f, -1580.000000f, -54.468601f, ZoneLineOrientationType.West, -112.227837f, 841.873230f, 200.000000f, -132.227844f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", -143.000000f, -1580.000000f, -54.468540f, ZoneLineOrientationType.West, -132.227844f, 841.873230f, 200.000000f, -152.227844f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", -163.000000f, -1580.000000f, -54.468601f, ZoneLineOrientationType.West, -152.227844f, 841.873230f, 200.000000f, -172.227844f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", -183.000000f, -1580.000000f, -54.468410f, ZoneLineOrientationType.West, -172.227844f, 841.873230f, 200.000000f, -192.227844f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", -203.000000f, -1580.000000f, -54.468632f, ZoneLineOrientationType.West, -192.227844f, 841.873230f, 200.000000f, -212.227844f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", -223.000000f, -1580.000000f, -54.468441f, ZoneLineOrientationType.West, -212.227844f, 841.873230f, 200.000000f, -232.227844f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", -243.000000f, -1580.000000f, -54.468639f, ZoneLineOrientationType.West, -232.227844f, 841.873230f, 200.000000f, -252.227844f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", -263.000000f, -1580.000000f, -54.467571f, ZoneLineOrientationType.West, -252.227844f, 841.873230f, 200.000000f, -272.227844f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", -283.000000f, -1580.000000f, -54.468670f, ZoneLineOrientationType.West, -272.227844f, 841.873230f, 200.000000f, -292.227844f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", -303.000000f, -1580.000000f, -54.468670f, ZoneLineOrientationType.West, -292.227844f, 841.873230f, 200.000000f, -312.227844f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", -323.000000f, -1580.000000f, -54.468700f, ZoneLineOrientationType.West, -312.227844f, 841.873230f, 200.000000f, -332.227844f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", -343.000000f, -1580.000000f, -54.468681f, ZoneLineOrientationType.West, -332.227844f, 841.873230f, 200.000000f, -352.227844f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", -363.000000f, -1580.000000f, -54.468712f, ZoneLineOrientationType.West, -352.227844f, 841.873230f, 200.000000f, -372.227844f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", -383.000000f, -1580.000000f, -54.468632f, ZoneLineOrientationType.West, -372.227844f, 841.873230f, 200.000000f, -392.227844f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", -403.000000f, -1580.000000f, -54.468670f, ZoneLineOrientationType.West, -392.227844f, 841.873230f, 200.000000f, -412.227844f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", -423.000000f, -1580.000000f, -54.468658f, ZoneLineOrientationType.West, -412.227844f, 841.873230f, 200.000000f, -432.227844f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", -443.000000f, -1580.000000f, -54.468658f, ZoneLineOrientationType.West, -432.227844f, 841.873230f, 200.000000f, -452.227844f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", -463.000000f, -1580.000000f, -54.468719f, ZoneLineOrientationType.West, -452.227844f, 841.873230f, 200.000000f, -472.227844f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", -483.000000f, -1580.000000f, -54.467651f, ZoneLineOrientationType.West, -472.227844f, 841.873230f, 200.000000f, -492.227844f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", -503.000000f, -1580.000000f, -54.468670f, ZoneLineOrientationType.West, -492.227844f, 841.873230f, 200.000000f, -512.227844f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", -523.000000f, -1580.000000f, -54.468712f, ZoneLineOrientationType.West, -512.227844f, 841.873230f, 200.000000f, -532.227844f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", -543.000000f, -1580.000000f, -54.468632f, ZoneLineOrientationType.West, -532.227844f, 841.873230f, 200.000000f, -552.227844f, 811.873230f, -100.000000f);
            AddZoneLineBox("ecommons", -563.000000f, -1580.000000f, -54.468601f, ZoneLineOrientationType.West, -552.227844f, 841.873230f, 200.000000f, -592.227844f, 811.873230f, -100.000000f);
            AddZoneLineBox("freporte", 447.153564f, -338.147949f, -27.999809f, ZoneLineOrientationType.West, -68.449120f, -867.598389f, 0.500060f, -98.163116f, -942.147095f, -28.499960f);
            AddZoneLineBox("freporte", 84.504356f, -62.710468f, -27.999990f, ZoneLineOrientationType.South, -401.208527f, -615.295532f, 0.940550f, -462.005890f, -629.942322f, -28.499929f);
            AddZoneLineBox("freporte", -154.759933f, 342.932068f, -97.968491f, ZoneLineOrientationType.North, -699.512451f, -1623.163940f, -85.500092f, -734.874329f, -1637.536377f, -98.468758f);
            AddZoneLineBox("freportn", 370.832550f, 726.575989f, -13.999940f, ZoneLineOrientationType.West, 1597.559326f, -249.103378f, 12.469000f, 1581.182617f, -270.030487f, -0.499950f);
            AddZoneLineBox("freportn", -2.907860f, -440.593567f, -20.999920f, ZoneLineOrientationType.North, 758.867493f, -571.213928f, -12.532780f, 742.214172f, -587.942444f, -50f);
            AddZoneLineBox("freportn", -408.099854f, 489.939026f, -13.999160f, ZoneLineOrientationType.North, 265.374237f, -110.341187f, 14.500020f, 221.547180f, -140.130920f, -14.500000f);
            AddZoneLineBox("freportn", -366.081055f, -82.489418f, -28.000010f, ZoneLineOrientationType.North, 307.515747f, -684.160217f, 0.500130f, 265.184326f, -713.913147f, -28.499969f);
            AddTeleportPad("freportw", 146.800308f, -681.771179f, -12.999480f, ZoneLineOrientationType.East, 97.993584f, -657.753784f, -40.968651f, 7.7f);
            AddTeleportPad("freportw", 12.084580f, -655.863647f, -54.968719f, ZoneLineOrientationType.North, 157.920013f, -715.959045f, -12.000000f, 7.7f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 799.806030f, -498.484985f, 657.738413f, -891.634094f, -20.999941f, 10f); // North tunnels from fork to exits
            AddLiquidPlane(ZoneLiquidType.Water, "t50_w1", 657.748413f, -727.049255f, 629.859070f, -742.386902f, -21.606260f, -32.01f, ZoneLiquidSlantType.NorthHighSouthLow, 10f); // North water slant near the fork in the tunnels
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 629.869070f, -727.049255f, 405.834116f, -869.017700f, -32.01f, 10f); // North area between slanting tunnels
            AddLiquidPlane(ZoneLiquidType.Water, "t50_w1", 405.844116f, -725.139465f, 293.876282f, -743.528076f, -32.01f, -69.578629f, ZoneLiquidSlantType.NorthHighSouthLow, 10f); // Large water slant behind the magic guilds
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 405.834116f, -842.651917f, -129.254303f, -884.942749f, -32.01f, 10f); // Channel that runs next to the magic guild, that goes into the tunnel
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 293.886282f, -654.427490f, 19.920610f, -827.145630f, -69.968712f, 100f); // Water around the mage guild and into the channel
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", -102.443680f, -745.005737f, -142.103119f, -842.661917f, -32.01f, 100f); // Channel water south of the mage area, into the gate and around the bend
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 111.530647f, -303.077606f, 76.707130f, -336.678467f, -33.999981f, 30f); // Small pool
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", -142.113119f, -731.640381f, -338.503693f, -797.679382f, -32.01f, 20f); // Channel heading back south into more tunnels and the stairs
            AddLiquidPlane(ZoneLiquidType.Water, "t50_w1", -321.548035f, -797.669382f, -336.199432f, -853.656311f, -32.7f, -47.968361f, ZoneLiquidSlantType.WestHighEastLow, 20f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", -321.254303f, -853.646311f, -363.852386f, -896.863098f, -47.968739f, 20f); // South bend between two declines
            AddLiquidPlane(ZoneLiquidType.Water, "t50_w1", -363.842386f, -880.658325f, -419.860931f, -896.736633f, -48.525330f, -69.968788f, ZoneLiquidSlantType.NorthHighSouthLow, 20f); // Southeastmost decline waterway
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 919.481262f, 163.751633f, 757.783447f, -11.916170f, -56.968739f, 50f); // North hidden pathway
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", -419.396210f, -863.264893f, -652.994568f, -962.689810f, -69.968712f, 150f); // Far southeast underground pathway, west part
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", -558.365784f, -962.679810f, -652.994568f, -1040.707886f, -69.968712f, 150f); // Far southeast underground pathway, south part
        }
    }
}
