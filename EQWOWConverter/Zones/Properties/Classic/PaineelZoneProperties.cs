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
    internal class PaineelZoneProperties : ZoneProperties
    {
        public PaineelZoneProperties()
        {
            // TODO: "Lift" near the hole that goes up and down
            // TODO: Teleporters after zone collision is mapped (4 glass pads connecting making 2 bounces?  What about red area?)
            SetBaseZoneProperties("paineel", "Paineel", 200f, 800f, 3.39f, 0, ZoneContinent.Odus);
            SetFogProperties(150, 150, 150, 200, 850);
            AddZoneLineBox("hole", 633.865723f, -942.076172f, -93.062523f, ZoneLineOrientationType.North, 640.945190f, -935.434082f, -87.500748f, 605.060547f, -947.819336f, -98.468681f);
            AddZoneLineBox("hole", 645.839417f, 246.516739f, -327.142517f, ZoneLineOrientationType.North, 932.554138f, 434.162994f, -151.438705f, 242.766006f, 88.558594f, -332.241425f);
            AddZoneLineBox("tox", -2592.465576f, -418.976410f, -45.092499f, ZoneLineOrientationType.North, 872.845337f, 187.732834f, 17.467010f, 831.879700f, 133.200150f, -1.499920f);
            AddZoneLineBox("warrens", 740.468933f, -881.437256f, -36.999771f, ZoneLineOrientationType.North, 748.881470f, -874.462463f, -26.586519f, 732.721863f, -888.849670f, -37.499989f);
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", 666.567322f, -801.855347f, 534.581116f, -984.564148f, -69.978620f, 50f); // Water path where you swim to the hole
            AddOctagonLiquidShape(LiquidType.Water, "t50_w1", 726.081848f, 701.436951f, -792.323425f, -817.091125f, -797.694214f, -811.679016f,
                -797.694214f, -811.679016f, 720.692749f, 706.710144f, 720.692749f, 706.710144f, -25.999870f, 50f); // Fountain inside on way to The Hole water path - Top
            AddQuadrilateralLiquidShapeZLevel(LiquidType.Water, "t50_w1", 708.056885f, -814.862244f, 701.712341f, -810.206421f, 699.726074f, -811.770020f,
                706.642822f, -818.681946f, -25.999870f, 50f); // Fountain inside on the way to The Hole water path - Waterfall
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", 712.035706f, -802.286377f, 676.356934f, -843.325256f, -29.000031f, 50f); // Fountain inside on the way to The Hole water path - Bottom
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", 685.144226f, 556.225159f, 674.213623f, 549.373779f, -97.968712f, 20f); // From The Hole to the City, indoor square pools (top)
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", 725.872559f, 555.932251f, 714.881287f, 548.928711f, -109.968750f, 20f); // From The Hole to the City, indoor square pools (middle)
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", 765.141846f, 556.267395f, 752.281494f, 549.298767f, -121.937477f, 20f); // From The Hole to the City, indoor square pools (bottom)
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", 957.460938f, 424.865753f, 945.598877f, 413.575684f, -121.937462f, 20f); // Indoor corner square pool near The Hole path
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", 880.495972f, 558.883911f, 868.833496f, 512.006042f, -125.937469f, 20f); // Lowest Courtayrd - Long pool with 2 spires in it, in outdoor se courtyard and neareast path to The Hole
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", 838.568909f, 705.510559f, 812.499207f, 679.591309f, -125.937363f, 20f); // Lowest Courtyard - Square pool with crecent stone shaped pillars around it, west of the long pool above
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", 824.789429f, 817.671753f, 819.851135f, 784.782349f, -83.968697f, 10f); // Mid-Tier Courtyard, area with 3 rectangle pools (east pool)
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", 824.759705f, 845.550476f, 784.788269f, 833.887817f, -83.968697f, 10f); // Mid-Tier Courtyard, area with 3 rectangle pools (center pool)
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", 824.622253f, 894.515381f, 819.658081f, 861.692383f, -83.968697f, 10f); // Mid-Tier Courtyard, area with 3 rectangle pools (west pool)
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", 909.135376f, 769.169189f, 903.211121f, 763.264648f, -83.968613f, 10f); // Mid-Tier Courtyard, area with 2 square pools (west pool)
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", 909.092896f, 748.146851f, 903.232666f, 742.302795f, -83.968613f, 10f); // Mid-Tier Courtyard, area with 2 square pools (east pool)
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", 1202.749756f, 698.624268f, 1148.437622f, 672.841980f, -41.968418f, 10f); // Highest courtyard, Long gold-rim rectangle pool in front of the north palace
            AddOctagonLiquidShape(LiquidType.Water, "t50_w1", 1287.200928f, 1259.772583f, 699.460327f, 671.911438f, 692.744812f, 678.758545f,
                692.744812f, 678.758545f, 1280.479858f, 1266.512207f, 1280.479858f, 1266.512207f, -41.968681f, 20f, 0.5f); // Northmost palace, entry level pool with 4 pillars in it
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", 790.090332f, 891.959412f, 743.220886f, 883.353577f, -83.978712f, 10f); // Building next to 3 rectangle pools, west fountain base
            AddOctagonLiquidShape(LiquidType.Water, "t50_w1", 751.686829f, 747.712585f, 889.624023f, 885.685974f, 888.655579f, 886.656189f,
                888.655579f, 886.656189f, 750.697754f, 748.686584f, 750.697754f, 748.686584f, -65.891022f, 50f); // Building next to 3 rectangle pools, west fountain spout water
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", 789.903381f, 795.782776f, 743.417603f, 787.371277f, -83.978712f, 10f); // Building next to 3 rectangle pools, east fountain base
            AddOctagonLiquidShape(LiquidType.Water, "t50_w1", 751.686829f, 747.712585f, 793.683228f, 789.702271f, 792.686829f, 790.690613f,
                792.686829f, 790.690613f, 750.697754f, 748.686584f, 750.697754f, 748.686584f, -65.891022f, 50f); // Building next to 3 rectangle pools, east fountain spout water
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", 963.091919f, 933.947144f, 959.424377f, 930.356323f, -65.978742f, 5f); // NW red roof house, bedroom water 1
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", 940.828613f, 869.948547f, 937.455994f, 866.379211f, -65.978742f, 5f); // NW red roof house, bedroom water 2
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", 1001.873413f, 894.942810f, 998.371216f, 891.451416f, -65.978742f, 5f); // NW red roof house, bedroom water 3
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", 1002.055908f, 848.042725f, 998.262268f, 844.184570f, -65.978742f, 5f); // NW red roof house, bedroom water 4
        }
    }
}