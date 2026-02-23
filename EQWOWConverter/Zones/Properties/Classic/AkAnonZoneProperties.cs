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
    internal class AkAnonZoneProperties : ZoneProperties
    {
        public AkAnonZoneProperties() : base()
        {
            AddZoneArea("Entry", "akanon-00", "akanon-00");
            AddZoneAreaBox("Entry", 257.142761f, 118.389160f, 1000, -131.215698f, -96.817497f, -1000f);
            
            AddZoneArea("Command", "akanon-00", "akanon-00");
            AddZoneAreaBox("Command", 840.922852f, -97.088692f, 1000, 713.662292f, -224.234146f, -1000f);
            
            AddZoneArea("Supply Area", "akanon-04", "akanon-04");
            AddZoneAreaBox("Supply Area", 1082.224976f, 231.194824f, 1000, 470.762787f, -420.297241f, -1000f);
            AddZoneAreaBox("Supply Area", 799.755920f, 231.194824f, 1000, 785.969543f, -381.862274f, -1000f);
            AddZoneAreaBox("Supply Area", 806.287354f, -415.003998f, 1000, 668.925293f, -447.226532f, -1000f);
            
            AddZoneArea("Barracks", "akanon-06", "akanon-06");
            AddZoneAreaBox("Barracks", 858.116882f, -422.322205f, 1000, 818.958008f, -483.524323f, -1000f);
            AddZoneAreaBox("Barracks", 1005.345703f, -438.836975f, 1000, 816.749207f, -507.056915f, -1000f);
            
            AddZoneArea("Market", "akanon-07", "akanon-07");
            AddZoneAreaBox("Market", 1394.073242f, -625.999939f, 1000, 1127.415894f, -837.516541f, -28.327950f);
            
            AddZoneArea("Library Mechanamagica", "akanon-02", "akanon-02");
            AddZoneAreaBox("Library Mechanamagica", 1149.214355f, -928.644714f, 1000, 1002.033630f, -1066.362427f, -1000f);
            
            AddZoneArea("Abbey of Deep Musing", "akanon-01", "akanon-01"); 
            AddZoneAreaBox("Abbey of Deep Musing", 1260.580566f, -492.048706f, 1000, 1159.140259f, -624.095093f, -1000f);
            AddZoneAreaBox("Abbey of Deep Musing", 1163.014404f, -498.962433f, 1000, 1144.748413f, -605.672791f, -1000f);
            AddZoneAreaBox("Abbey of Deep Musing", 1252.234985f, -483.473602f, 1000, 1165.459839f, -497.916351f, -1000f);
            AddZoneAreaBox("Abbey of Deep Musing", 1272.996582f, -505.813477f, 1000, 1250.005127f, -619.286072f, -1000f);
            
            AddZoneArea("The Falls");
            AddZoneAreaBox("The Falls", 1722.807495f, -91.651901f, 1000, 1223.065186f, -608.332092f, -1000);
            
            AddZoneArea("Lower Tunnels", "akanon-07", "akanon-07");
            AddZoneAreaBox("Lower Tunnels", 2110.136230f, -366.787415f, -160.268997f, 2016.313599f, -436.065186f, -1000);
            AddZoneAreaBox("Lower Tunnels", 2263.394287f, -344.190277f, -160.268997f, 2079.037109f, -653.487427f, -1000);
            AddZoneAreaBox("Lower Tunnels", 2261.485596f, -419.654907f, -160.268997f, 1961.612183f, -647.920593f, -1000);
            
            AddZoneArea("The Turbine", "", "", false, "spinnrlp", "spinnrlp");
            AddZoneAreaBox("The Turbine", 2036.529297f, -170.345245f, 1000, 1718.003540f, -462.474243f, -156.969238f);
            AddZoneAreaBox("The Turbine", 2097.180176f, -162.135498f, -16.548880f, 1947.567261f, -444.580750f, -1000f);
            
            AddZoneArea("Zoo", "akanon-06", "akanon-06");
            AddZoneAreaBox("Zoo", 1375.437500f, -726.707092f, -28.450760f, 1228.063110f, -839.659973f, -300);
            
            AddZoneLineBox("steamfont", -2059.579834f, 528.815857f, -111.126549f, ZoneLineOrientationType.North, 70.830750f, -69.220848f, 12.469000f, 60.770279f, -84.162193f, -0.500000f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0004", 718.719971f, 143.395935f, 517.753479f, -379.726532f, -28.999929f, 60f); // Large water pool, south outer
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0004", 1065.541504f, 141.236343f, 517.743479f, -31.595591f, -28.999929f, 60f); // Large water pool, west outer
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0004", 1191.483398f, -31.585591f, 992.221497f, -455.338409f, -28.999929f, 60f); // Large water pool, north outer
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0004", 879.112427f, -216.083237f, 718.729971f, -378.343597f, -28.999929f, 60f); // Large water pool, east outer
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0004", 943.169678f, -31.585591f, 718.709971f, -103.521957f, -28.999929f, 60f); // Large water pool, west large block not outer
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0004", 992.231497f, -88.385208f, 832.563293f, -279.458344f, -28.999929f, 60f); // Large water pool, north large block not outer
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0004", 992.241497f, -279.448344f, 920.245789f, -363.988678f, -28.999929f, 60f); // Large water pool, northeast corner section
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0004", 920.235789f, -320.646973f, 879.102427f, -366.217255f, -28.999929f, 60f); // Large water pool, small northeast outer
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0004", 724.633301f, -213.591019f, 717.220398f, -223.374084f, -28.999929f, 60f); // Large water pool, center cutout, SE corner section
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0004", 724.364746f, -98.908813f, 712.962463f, -106.535049f, -28.999929f, 60f); // Large water pool, center cutout, SW corner section
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0004", 840.269836f, -98.132057f, 827.901306f, -107.484932f, -28.999929f, 60f); // Large water pool, center cutout, NW corner section
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0004", 946.898315f, -29.885700f, 937.846619f, -35.895458f, -28.999929f, 60f); // Large water pool, nw cutout, sw coner section (middle)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0004", 950.656982f, -27.905569f, 936.563416f, -32.405380f, -28.999929f, 60f); // Large water pool, nw cutout, sw coner section (north)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0004", 944.902893f, -27.905569f, 936.563416f, -38.282459f, -28.999929f, 60f); // Large water pool, nw cutout, sw coner section (south)
            AddTrapezoidLiquidAxisAlignedZLevelShape(ZoneLiquidType.Water, "d_m0004", 503.825012f, 489.799225f, 0.645790f, -97.986412f, -13.937520f, -83.965424f, 174.745728f, 350f, 0.4f); // Entry waterfall, south
            AddTrapezoidLiquidAxisAlignedZLevelShape(ZoneLiquidType.Water, "d_m0004", 517.780579f, 503.815012f, 28.102119f, -126.010399f, 0.645790f, -97.986412f, 174.745728f, 350f, 0.4f); // Entry waterfall, north
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0004", 1393.839966f, -624.353882f, 1099.773682f, -687.787842f, -29.000010f, 50f); // Large to Medium connection channel, bridge and forge nearby
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0004", 1159.325195f, -600.300171f, 1069.149658f, -624.363882f, -29.000010f, 50f); // Large to Medium connection channel, bend around low internal 
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0004", 1147.023926f, -455.328409f, 1076.342651f, -600.290171f, -29.000010f, 50f); // Large to Medium connection channel, south run
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0004", 1162.622070f, -455.328409f, 1147.013926f, -488.690735f, -29.000010f, 50f); // Large to Medium connection channel, small corner
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0004", 1303.937500f, -615.752747f, 1249.870728f, -624.343882f, -29.000010f, 50f); // Large to Medium connection channel, flat to waterfall
            AddLiquidPlane(ZoneLiquidType.Water, "d_m0004", 1315.200073f, -609.897156f, 1268.035645f, -615.752747f, -29.000010f, -34.656818f, ZoneLiquidSlantType.EastHighWestLow, 150f, -1, "The Falls"); // Large to Medium channel, waterfall high
            AddLiquidPlane(ZoneLiquidType.Water, "d_m0004", 1319.235107f, -601.728088f, 1276.238281f, -609.898156f, -34.656818f, -42.569721f, ZoneLiquidSlantType.EastHighWestLow, 150f, -1, "The Falls"); // Large to Medium channel, waterfall low
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0004", 1385.424927f, -219.524216f, 1235.044922f, -601.738088f, -83.969673f, 200f);  // Angle water room, large west stretch
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0004", 1690.890137f, -222.218719f, 1583.315674f, -459.795074f, -125.937500f, 250f); // Angle water room, large lower area
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0004", 1749.439209f, -264.337097f, 1690.880137f, -323.689636f, -125.937500f, 250f); // Angle water room, outflow
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0004", 1583.325674f, -180.395126f, 1441.454224f, -337.889893f, -125.937500f, 250f); // Angle water room, large lower area west
            AddLiquidPlane(ZoneLiquidType.Water, "d_m0004", 1387.379883f, -219.524216f, 1385.414927f, -363.847931f, -83.968643f, -85.391907f, ZoneLiquidSlantType.SouthHighNorthLow, 250f); // Angle water room, nw first decline
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0004", 1399.420288f, -219.524216f, 1387.369883f, -363.827362f, -85.968513f, 250f); // Angle water room, nw first flat step down
            AddLiquidPlane(ZoneLiquidType.Water, "d_m0004", 1401.434204f, -219.524216f, 1399.410288f, -349.886871f, -85.968513f, -86.975113f, ZoneLiquidSlantType.SouthHighNorthLow, 250f); // Angle water room, nw second decline
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0004", 1413.430298f, -219.524216f, 1401.424204f, -349.881958f, -97.968422f, 250f); // Angle water room, nw second flat step down
            AddLiquidPlane(ZoneLiquidType.Water, "d_m0004", 1415.407471f, -219.524216f, 1413.420298f, -349.887268f, -97.968422f, -99.147186f, ZoneLiquidSlantType.SouthHighNorthLow, 250f); // Angle water room, nw third decline
            AddLiquidPlane(ZoneLiquidType.Water, "d_m0004", 1441.441650f, -206.769455f, 1415.397471f, -349.856812f, -110.013939f, -113.806763f, ZoneLiquidSlantType.SouthHighNorthLow, 250f); // Angle water room, nw 4th shelf with slight decline
            AddLiquidPlane(ZoneLiquidType.Water, "d_m0004", 1443.464224f, -206.769455f, 1441.431650f, -335.904602f, -119.955673f, -120.584892f, ZoneLiquidSlantType.SouthHighNorthLow, 250f); // Angle water room nw fourth decline
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0004", 1511.302856f, -503.851990f, 1384.338257f, -560.399841f, -83.968643f, 250f); // Angle water room, top layer NE square
            AddTriangleLiquidShapeSouthEdgeAligned(ZoneLiquidType.Water, "d_m0004", 1515.426270f, -504.053528f, 1385.414927f, -358.403595f, -504.195312f, -83.968643f, 250f, 1.5f); // Angle water room, top NE flat triangle
            AddQuadrilateralLiquidShapeZLevel(ZoneLiquidType.Water, "d_m0004", 1542.222852f, -501.844788f, 1401.386353f, -348.008789f, 1384.896362f, -363.390625f, 1521.530518f, -517.400146f, -85.968620f, 250f, 2000f, 3000f, -3000f, -2000f, 1.5f); // Angle water room, NE 2nd shelf
            AddQuadrilateralLiquidShapeZLevel(ZoneLiquidType.Water, "d_m0004", 1567.441406f, -499.122986f, 1414.456177f, -347.694214f, 1396.811646f, -349.537689f, 1540.378662f, -503.297974f, -97.968719f, 250f, 2000f, 2000f, -2000f, -2000f, 1.5f); // Angle water room, NE 3rd shelf
            AddQuadrilateralLiquidShapeZLevel(ZoneLiquidType.Water, "d_m0004", 1570.003418f, -473.291626f, 1432.528931f, -340.194733f, 1410.220215f, -349.923248f, 1566.189819f, -504.060883f, -113.5f, 250f, 5000f, 5000f, -5000f, -5000f, 1.5f); // Angle water room, NE 4th shelf half 1
            AddQuadrilateralLiquidShapeZLevel(ZoneLiquidType.Water, "d_m0004", 1583.776367f, -461.868988f, 1441.326538f, -334.384521f, 1428.510986f, -337.834198f, 1566.220581f, -478.165771f, -116.642921f, 250f, 2000f, 2000f, -2000f, -2000f, 1.5f); // Angle water room, NE 4th shelf half 2
            AddQuadrilateralLiquidShapeZLevel(ZoneLiquidType.Water, "d_m0004", 1585.881958f, -460.313110f, 1443.003540f, -334.342102f, 1441.548584f, -340.078186f, 1581.346069f, -466.686768f, -120f, 250f, 5000f, 5000f, -5000f, -5000f, 1.5f); // Angle water room, NE 5th shelf (small shelf)
            AddQuadrilateralLiquidShapeZLevel(ZoneLiquidType.Water, "d_m0004", 1583.547729f, -335.616241f, 1445.949463f, -335.960205f, 1441.000366f, -337.993347f, 1582.367554f, -462.521454f, -125.936363f, 250f, 2000f, 2000f, -2000f, -2000f, 1.5f); // Angle room bottom level SE corner
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0004", 1973.211548f, -193.271240f, 1749.429209f, -380.653412f, -126.937317f, 500f); // Outflow room, up to waterfall
            AddLiquidPlane(ZoneLiquidType.Water, "d_m0004", 1987.200806f, -264.239075f, 1973.201548f, -322.780212f, -126.937317f, -140.044510f, ZoneLiquidSlantType.SouthHighNorthLow, 500f); // Outfloom room, waterfall
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0004", 2086.414307f, -264.974457f, 1987.190806f, -322.667267f, -197.909174f, 500f); // Outfloow room, outflow
        }
    }
}
