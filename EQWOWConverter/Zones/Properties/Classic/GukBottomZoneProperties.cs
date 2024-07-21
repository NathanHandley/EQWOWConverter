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
    internal class GukBottomZoneProperties : ZoneProperties
    {
        public GukBottomZoneProperties()
        {
            // TODO: Bug - Clear ceiling in north tunnel at 1510 -459 -136
            // TODO: Bug - Blood collision issue.  If d_m0014 is collision enabled, the floor under some blood pools will be passthrough and fall through the map
            // TODO: Ladders
            SetBaseZoneProperties("gukbottom", "Ruins of Old Guk", -217f, 1197f, -81.78f, 0, ZoneContinent.Antonica);
            SetFogProperties(50, 45, 20, 10, 140);
            AddZoneLineBox("guktop", 1113.605835f, 617.183350f, -88.333542f, ZoneLineOrientationType.East, 1161.719360f, 662.774170f, -81.499748f, 1143.830933f, 656.943542f, -110f);
            AddZoneLineBox("innothule", 144.032776f, -821.548645f, -11.500000f, ZoneLineOrientationType.West, -123.419243f, 84.161140f, -225.437256f, -140f, 69.775558f, -238.406235f);
            AddZoneLineBox("guktop", 1620.083008f, 181.952133f, -88.660629f, ZoneLineOrientationType.West, 1675.066772f, -37.624660f, -70f, 1648.329590f, -92.907097f, -138.851685f);
            AddZoneLineBox("guktop", 1555.745972f, -121.623947f, -91.073799f, ZoneLineOrientationType.West, 1506.506348f, 73.868462f, -80f, 1485.213745f, 14.151250f, -105f);
            AddZoneLineBox("guktop", 1196.247681f, -197.502167f, -83.967888f, ZoneLineOrientationType.West, 1203.723999f, -181.743942f, -71.499748f, 1189.337769f, -204.274963f, -84.468781f);
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_wguk1", 1708.509888f, 272.930328f, 1451.755371f, -243.919006f, -64.278084f, 100f); // North water tunnels and their exits, no surface
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_wguk1", 1661.019897f, -243.909006f, 1507.664429f, -404.428986f, -84.968712f, 100f); // North water tunnels, tunnel to surface connection
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_wguk1", 1581.377319f, -404.428986f, 1553.373291f, -433.808990f, -84.968712f, 200f); // North water tunnels, waterfall
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_wguk1", 1584.068481f, -418.669525f, 1581.367319f, -433.808990f, -97.978613f, 200f); // North water tunnels, north waterfall under back area
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_wguk1", 1553.383291f, -418.669525f, 1551.182861f, -433.808990f, -111.968597f, 200f); // North water tunnels, north waterfall under back area
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_wguk1", 1602.481323f, -433.798990f, 1426.489136f, -494.352844f, -125.937469f, 200f); // Lower north water tunnels, bottom of waterfall and into tunnel
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_wguk1", 1426.499136f, -415.512909f, 1064.963013f, -541.815674f, -140.937408f, 200f);  // Lower north water tunnels, 1 step south of the base of the waterfall
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_wguk1", 1064.973013f, -496.693756f, 973.019653f, -542.443848f, -140.937408f, 33.5f); // Lower north water tunnels, first intersection from waterfall
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_wguk1", 991.544373f, -542.433848f, 963.857788f, -634.893738f, -142.640216f, 33f);// Lower north water tunnels, after intersection bend 'under' the overwalk
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_wguk1", 1045.445801f, -603.926086f, 991.534373f, -632.068359f, -140.937485f, 33f);// Lower north water tunnels, end of the bend minus very small end
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_wguk1", 1040.555786f, -598.379211f, 1032.234619f, -603.936086f, -140.937485f, 33f);// Lower north water tunnels, end of the bend nub
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_wguk1", 1165.004395f, -626.927185f, 1116.223389f, -675.938538f, -126.937431f, 55f); // Northeast Water Column over the small blood pool
            AddLiquidPlaneZLevel(LiquidType.Blood, "d_m0014", 1148.422241f, -642.838013f, 1132.825806f, -658.476074f, -195.916113f, 10f); // Northeast blood pool under the water column
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_wguk1", 894.365784f, -602.669128f, 882.813538f, -614.657959f, -208.906174f, 10f); // Tiny pool under the north water bend
            AddLiquidPlaneZLevel(LiquidType.Blood, "d_m0014", 1076.109741f, -742.590942f, 1064.633545f, -754.634644f, -195.906204f, 10f); // Non-sinking blood pool
            AddLiquidPlaneZLevel(LiquidType.Blood, "d_m0014", 894.537354f, -742.840332f, 882.744385f, -754.630371f, -194.937439f, 10f); // Center blood pool in north pathway
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_wguk1", -10.082610f, -124.179993f, -51.616791f, -170.448273f, -238.906235f, 20f); // South area cresent pool
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_wguk1", 125.095451f, -76.734489f, 99.050468f, -104.962547f, -236.906235f, 50f); // South area pool with green curtains
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_wguk1", 407.424347f, -83.456360f, 378.318146f, -145.874329f, -237.916235f, 50f); // South area intersection of water, north part
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_wguk1", 378.328146f, -90.287163f, 370.236725f, -145.874329f, -237.916235f, 50f); // South area intersection of water, next step south of north
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_wguk1", 374.337250f, -97.065567f, 312.977081f, -145.874329f, -237.916235f, 50f); // South area intersection of water, south portion
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_wguk1", 640.710022f, -332.082092f, 487.965851f, -508.010468f, -210.906219f, 100f); // Center large water pool with bridges
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_wguk1", 905.084900f, 610.934448f, 780.849976f, 339.636108f, -223.906143f, 150f); // Large west waterways, lower area - SW area
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_wguk1", 874.308472f, 339.646108f, 780.849976f, 247.501633f, -223.906143f, 150f); // Large west waterways, lower area - SE area
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_wguk1", 958.589355f, 306.636627f, 874.298472f, 245.511917f, -223.906143f, 150f); // Large west waterways, lower area - SE area
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_wguk1", 1164.159058f, 333.159912f, 958.579355f, 245.511917f, -223.906143f, 150f); // Large west waterways, lawer area - NE area
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_wguk1", 1164.159058f, 439.369568f, 1007.847412f, 333.149912f, -223.906143f, 150f); // Large west waterways, lawer area - W area
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_wguk1", 926.917358f, 719.690857f, 850.220581f, 612.003601f, -167.937393f, 350f); // West upper waterfall lead-up
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_wguk1", 896.710388f, 620.336731f, 856.116089f, 603.203186f, -167.937393f, 350f); // West upper waterfall block-in near the waterfall line
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_wguk1", 896.710388f, 603.158020f, 859.755066f, 595.460449f, -167.937393f, 350f); // West upper waterfall block-in near the waterfall line
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_wguk1", 896.710388f, 595.470449f, 864.465759f, 591.912842f, -167.937393f, 350f); // West upper waterfall block-in near the waterfall line
            AddQuadrilateralLiquidShapeZLevel(LiquidType.Water, "t50_wguk1", 896.710388f, 587.925903f, 895.350647f, 599.822693f, 873.723572f, 587.733887f,
                883.672058f, 585.779114f, -167.937393f, 350f); // West upper waterfall, north part
            AddQuadrilateralLiquidShapeZLevel(LiquidType.Water, "t50_wguk1", 879.013733f, 589.615356f, 873.382874f, 596.898560f, 863.668030f, 591.743713f,
                873.733572f, 587.733887f, -167.937393f, 350f); // West upper waterfall, east part
            AddQuadrilateralLiquidShapeZLevel(LiquidType.Water, "t50_wguk1", 867.500549f, 593.786804f, 857.799255f, 606.497314f, 853.164795f, 602.243958f, 863.647827f,
                591.752808f, -167.937393f, 350f); // West upper waterfall, south part
            AddDisabledMaterialCollisionByNames("t50_wguk1", "t75_m0000", "t50_gukfalls1", "t75_m0001", "t75_m0002", "t50_m0004", "t75_m0013", "d_m0015", "t25_smke1"); // d_m0014 = blood and used for some surfaces
        }
    }
}
