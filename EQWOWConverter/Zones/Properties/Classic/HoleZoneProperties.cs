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
    internal class HoleZoneProperties : ZoneProperties
    {
        public HoleZoneProperties() : base()
        {
            // TODO: Bad object at 117 164 -349 (1x scale)
            // TODO: Forge in first building when coming in needs damage zone
            SetBaseZoneProperties("hole", "The Hole", -1049.98f, 640.04f, -77.22f, 0, ZoneContinentType.Odus);
            SetZonewideEnvironmentAsIndoors(10, 10, 10, ZoneFogType.Heavy);
            DisableSunlight();
            OverrideVertexColorIntensity(0.5);
            AddZoneLineBox("paineel", 588.502197f, -941.292969f, -93.159729f, ZoneLineOrientationType.South, 608.765930f, -935.432007f, -82.499748f, 580.660583f, -947.818420f, -98.468742f);
            AddZoneLineBox("neriakc", 480.001648f, -809.905090f, -55.968712f, ZoneLineOrientationType.North, 75.090286f, 356.037201f, -375.374756f, 67.145378f, 341.312317f, -386.343719f);
            AddZoneLineBox("paineel", 588.502197f, -941.292969f, -93.159729f, ZoneLineOrientationType.South, 55.819328f, 375.380615f, -375.374756f, 41.268639f, 367.254913f, -386.343750f);
            AddZoneLineBox("erudnext", -1552.149292f, -184.036606f, -47.968700f, ZoneLineOrientationType.North, 52.396881f, 326.834320f, -375.374756f, 37.806911f, 318.680603f, -386.343323f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 531.560303f, 436.702850f, 320.959839f, 82.405937f, -349.875000f, 50f); // Top water pond in the large pit
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 711.456055f, -908.072327f, 558.995667f, -1057.308350f, -83.968697f, 25f); // Entry (very top)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 462.969482f, -474.541412f, 377.558228f, -565.965576f, -195.916235f, 5f); // Shallow pool in big chaimber after entry
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 509.425354f, -165.892136f, 483.220215f, -179.785797f, -196.906174f, 100f); // Uppermost area 'funnel' water passage to lower area, top part
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 505.832306f, -175.224014f, 485.240601f, -206.390945f, -223.906174f, 75f); // Uppermost area 'funnel' water passage to lower area, bottom part
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 209.731720f, -138.017502f, 163.095779f, -212.544235f, -251.906219f, 25f); // Pool in path between first and second big areas                        
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 1.113530f, -202.140320f, -14.945060f, -232.141266f, -279.874908f, 15f); // Square Pool in second big area courtyard
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", -25.828911f, -25.686939f, -84.617897f, -56.631119f, -349.885000f, 10f); // Sqare pool that spills out in front of "The Slab"
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 120.324043f, -12.449350f, 103.312157f, -29.569731f, -349.875000f, 10f); // Sqare pool NE in second big area courtyard
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 15.977700f, 211.934479f, -16.027411f, 180.204727f, -307.875000f, 10f); // Square pool in front of big structure in second big area
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 112.899963f, 155.009628f, 96.903931f, 138.989075f, -349.874908f, 10f); // Square pool in north courtyard in second big area
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 189.843674f, 330.021423f, 160.873276f, 292.664459f, -349.885000f, 10f); // Square pool that spills out in nw area of second big area
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 35.880600f, 442.089081f, 19.900930f, 425.785156f, -349.874969f, 10f); // Square pool in w area of second big area
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 467.147797f, -243.486542f, 462.428894f, -248.417877f, -144.937469f, 4f); // Forge quench pool, first large building 1
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 463.205841f, -239.396255f, 458.280426f, -252.315720f, -144.937469f, 4f); // Forge quench pool, first large building 2
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", -82.058830f, 267.751709f, -84.660431f, 263.921936f, -289.784912f, 10f); // Fountain in large building in second area, waterfall
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", -59.591331f, 270.360718f, -84.663223f, 261.381836f, -307.885000f, 2f); // Fountain in large building in second area, pool
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 272.590942f, 871.876770f, 209.191193f, 796.871338f, -476.812469f, 20f); // West path pond
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 958.613790f, 870.819803f, 834.961090f, 733.737690f, -856.655883f, 20f); // Two pond with a bridge in the last run-down towards the bottom
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 917.357890f, 535.846457f, 889.587403f, 513.903960f, -572.781220f, 5f); // Orange/Yellow dome building internal square body of water                        
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 582.975820f, 409.538777f, 510.567933f, 264.886170f, -896.656190f, 10f); // Very bottom pond, bottom of waterfall 1 (middle)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 605.389660f, 387.356210f, 578.504487f, 330.563707f, -896.656190f, 10f); // Very bottom pond, bottom of waterfall 2
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 594.486337f, 331.230373f, 510.567933f, 279.507370f, -896.656190f, 10f); // Very bottom pond, bottom of waterfall 3
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 560.783283f, 266.736960f, 510.567933f, 243.898263f, -896.656190f, 10f); // Very bottom pond, bottom of waterfall 4
        }
    }
}
