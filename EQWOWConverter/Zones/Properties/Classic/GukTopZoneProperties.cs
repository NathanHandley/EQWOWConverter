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

using EQWOWConverter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Zones.Properties
{
    internal class GukTopZoneProperties : ZoneProperties
    {
        public GukTopZoneProperties() : base()
        {
            // TODO: Add zone areas
            // TODO: Large areas to 'delete' that you couldn't access anyway
            // "gap" that falls down at 931 -2 -125
            SetZonewideEnvironmentAsIndoors(40, 45, 20, ZoneFogType.Heavy);
            OverrideVertexColorIntensity(0.4);
            Enable2DSoundInstances("caveloop");
            AddZoneLineBox("gukbottom", 1154.039917f, 670.316589f, -93.968727f, ZoneLineOrientationType.West, 1122.238281f, 644.556519f, -77.740372f, 1105.369995f, 629.647583f, -95.468483f);
            AddZoneLineBox("gukbottom", 1665.729126f, -107.982651f, -102.307808f, ZoneLineOrientationType.East, 1623.884277f, 142.214523f, -60f, 1563.454590f, 117.747520f, -110f);
            AddZoneLineBox("gukbottom", 1493.752930f, -1.347960f, -91.878059f, ZoneLineOrientationType.East, 1575f, -205.689896f, -80f, 1550.644043f, -155.016663f, -115f);
            AddZoneLineBox("gukbottom", 1195.318848f, -209.319427f, -83.968781f, ZoneLineOrientationType.East, 1203.724121f, -205.721344f, -71.500748f, 1185.029785f, -210.714722f, -84.468697f);
            AddZoneLineBox("innothule", 144.032776f, -821.548645f, -11.500000f, ZoneLineOrientationType.West, -53.083141f, 56.776360f, 12.469000f, -70.161201f, 49.388599f, -0.499990f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 1307.711426f, 603.854858f, 1187.823120f, 515.279236f, -84.978567f, 250f); // NW water area, SW big area
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 1685.380859f, 645.511963f, 1307.665771f, 546.273987f, -84.978567f, 250f); // NW water area, NW strip
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 1682.266846f, 546.283987f, 1410.800537f, 431.357605f, -84.978567f, 250f); // NW water area, big NW area
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 1685.662598f, 431.367605f, 1562.658691f, 39.609310f, -84.978567f, 250f); // NW water area, big NW area
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 212.592926f, -475.280548f, 195.210266f, -490.740234f, -29.999950f, 20f); // East entrance water column pool
            AddLiquidCylinder(ZoneLiquidType.Water, "t75_wguk1", 203.9f, -482.75f, 6.1f, 5f, 100f, 209.9f, -476.75f, 197.9f, -488.75f, 0.3f); // East entrance water column
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 212.628433f, -397.376251f, 195.367966f, -412.369354f, -29.999950f, 20f); // West entrance water column pool
            AddLiquidCylinder(ZoneLiquidType.Water, "t75_wguk1", 203.924454f, -404.837677f, 6.1f, 5f, 100f, 209.9f, -398.843170f, 197.9f, -410.842773f, 0.3f); // West entrance water column
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_b1", 246.447037f, -437.361053f, 241.367645f, -450.286865f, -16.999960f, 5f); // Entrance blood pool north of the cylinders
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 200.426575f, -431.506012f, 175.060501f, -456.532654f, -26.999969f, 20f); // Entrance center water pool
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 240.192154f, 209.908585f, 194.384766f, 94.247452f, -99.968727f, 30f); // SW lower cross channel, east of the supply water
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 239.087723f, 226.498566f, 223.923782f, 209.918585f, -99.968727f, 30f); // SW lower cross channel, north of the supply water
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 209.909714f, 226.498566f, 194.427032f, 209.918585f, -99.968727f, 30f); // SW lower cross channel, south of the supply water
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 223.933782f, 226.498566f, 209.899714f, 209.918585f, -86.321083f, 35f); // SW lower cross channel, supply water
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 106.478844f, -232.286987f, 41.983528f, -312.500885f, -15.01f, 10f); // SE upper water, SE corner
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 241.403152f, -74.023491f, 81.349022f, -276.385834f, -15.01f, 10f); // SE upper water, center section
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 169.428894f, -33.182129f, 128.714828f, -79.019577f, -15.01f, 10f); // SE upper water, west section
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 1577.224609f, -73.646111f, 1483.882446f, -255.995224f, -74.968628f, 40f); // SE upper water channel, far east part
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 1577.224609f, 87.159378f, 1483.882446f, -73.636111f, -84.968628f, 30f); // NE upper water channel, just west of east
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 1512.381714f, 146.524994f, 1451.948120f, 87.149378f, -84.968628f, 30f); // NE upper water channel, connecting bend into the donut-shaped water
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 1456.517334f, 179.028976f, 1445.645151f, 83.424301f, -84.968628f, 30f); // NE upper water, donut-shaped water small NE corner
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 1445.655151f, 197.438278f, 1357.730347f, 83.424301f, -84.968628f, 30f); // NE upper water, donut-shaped water north part
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 1357.740347f, 189.231293f, 1352.229614f, 105.706291f, -84.968628f, 30f); // NE upper water, donut-shaped water south part
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 1352.239614f, 172.073120f, 1348.799194f, 121.710793f, -84.968628f, 30f); // NE upper water, donut-shaped water south tip
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 1357.740347f, 97.982384f, 1353.750854f, 83.966904f, -57.917839f, 100f); // Line from donut-shaped room, northmost
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 1353.760854f, 97.363823f, 1348.921387f, 81.360802f, -57.917839f, 100f); // Line from donut-shaped room, 2nd from north
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 1348.931387f, 97.363823f, 1337.776123f, 75.474953f, -57.917839f, 100f); // Line from donut-shaped room, 3rd from north
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 1337.786123f, 97.363823f, 1324.008423f, 66.580017f, -57.917839f, 100f); // Line from donut-shaped room, 4rd from north
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 1324.018423f, 102.602692f, 1298.516479f, 62.382092f, -57.917839f, 100f); // Line from donut-shaped room, 5rd from north
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 1298.526479f, 134.895538f, 1221.172363f, 59.532509f, -57.917839f, 100f); // Line from donut-shaped room, 6rd from north
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 1221.182363f, 195.551834f, 1125.511841f, 98.888092f, -57.917839f, 100f); // Line from donut-shaped room, 7rd from north
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 1125.521841f, 244.858887f, 871.282898f, 143.744797f, -57.917839f, 100f); // Line from donut-shaped room, 8rd from north
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 872.315918f, 230.108475f, 700.687256f, 126.528900f, -84.968590f, 200f); // North large pool, north part - West
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 786.314941f, 126.538900f, 700.687256f, 12.964880f, -84.968590f, 200f); // North large pool, north part - East
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 700.697256f, 158.223709f, 673.311707f, 12.964880f, -84.968590f, 200f); // North large pool, 2nd down from north
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 673.321707f, 144.480103f, 652.276367f, 12.964880f, -84.968590f, 200f); // North large pool, 3nd down from north
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 652.286367f, 113.902138f, 558.421631f, 12.964880f, -84.968590f, 200f); // North large pool, 4nd down from north
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 558.431631f, 66.634956f, 544.656250f, 12.964880f, -84.968590f, 200f); // North large pool, south part
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 942.176819f, 36.788380f, 785.693787f, -79.683601f, -125.937363f, 200f); // Lower east center, north part
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 786.420532f, -25.840429f, 769.242310f, -44.140461f, -153.947454f, 200f); // Lower east center, outlet south
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 1119.130249f, 335.003723f, 1092.139893f, 308.971710f, -82.968727f, 5f); // Square pool with stairs all around it and 4 pillars
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 253.336533f, 152.714752f, 138.750122f, 67.294472f, -0.999990f, 7f); // Southwest Upper, westmost before the waterfall
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 251.963242f, 67.304472f, 237.871613f, 41.969238f, -0.999990f, 55f); // Southwest Upper, westmost waterfall
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 311.874786f, 43.261978f, 207.781754f, -59.968731f, -27.999990f, 22f); // Southwest lower area with ring and pillars, center
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 233.094543f, -33.407860f, 194.514984f, -73.351349f, -27.999990f, 22f); // Southwest lower area with ring and pillars, southeast
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 209.908493f, 0.003580f, 179.352478f, -20.191879f, -0.01f, 200f); // Southwest upper, southeast waterfall area (west waterfall)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 209.908493f, -20.181879f, 179.352478f, -99.504303f, -0.01f, 5f); // Southwest upper, southeast waterfall area (before waterfall)
            AddQuadrilateralLiquidShapeZLevel(ZoneLiquidType.Water, "t75_wguk1", 223.918457f, -28.026270f, 209.902557f, -14.027710f, 209f, -33.400539f, 223.737534f, -31.665899f, -0.01f, 200f); // Southwest upper, southeast waterfall area (waterfall angle)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 217.842773f, -22.133650f, 207.968323f, -37.862080f, -0.01f, 200f); // Southwest upper, southeast waterfall area (waterfall gap (on top))
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 824.671265f, 460.855042f, 812.857971f, 448.846191f, -68.968689f, 5); // Water square with high rise fire braizer
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 399.586334f, -59.966660f, 293.230927f, -114.731056f, -41.968712f, 9f); // Center middle east  (ne lower from ring with pillers)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 434.903229f, 98.151421f, 311.871765f, -59.956660f, -41.968712f, 9f); // Center middle north  (ne lower from ring with pillers)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 508.331848f, 186.730713f, 377.217804f, 82.592552f, -41.968712f, 9f); // Center middle northwest  (ne lower from ring with pillers)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_wguk1", 451.951996f, -13.112000f, 363.141907f, -127.284462f, -84.968719f, 30f); // Center middle low
        }
    }
}
