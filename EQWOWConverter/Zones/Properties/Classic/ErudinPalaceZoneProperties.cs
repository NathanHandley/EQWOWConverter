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
    internal class ErudinPalaceZoneProperties : ZoneProperties
    {
        public ErudinPalaceZoneProperties()
        {
            // TODO: Lots of 'garbage' zone material to delete
            SetBaseZoneProperties("erudnint", "Erudin Palace", 807f, 712f, 22f, 0, ZoneContinent.Odus);
            SetFogProperties(0, 0, 0, 0, 0);
            AddTeleportPad("erudnext", -773.795898f, -183.949753f, 50.968781f, ZoneLineOrientationType.North, 711.744934f, 806.283813f, 5.000060f, 12.0f);
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_eruw1", 764.244263f, 770.935730f, 659.586243f, 716.210144f, 35.000069f, 5f); // Top pool 1
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_eruw1", 760.352295f, 773.629150f, 661.691772f, 714.828491f, 35.000069f, 5f); // Top pool 2
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_eruw1", 756.832825f, 778.366150f, 666.796204f, 708.935974f, 35.000069f, 5f); // Top pool 3
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_eruw1", 754.576050f, 780.455627f, 668.782837f, 707.070862f, 35.000069f, 5f); // Top pool 4
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_eruw1", 734.305359f, 704.889282f, 689.243225f, 685.480774f, 2.000090f, 5f); // Mid pool 1
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_eruw1", 732.363098f, 706.830750f, 691.173462f, 683.539062f, 2.000090f, 5f); // Mid pool 1
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_eruw1", 730.505737f, 708.781738f, 693.223389f, 681.413147f, 2.000090f, 5f); // Mid pool 1
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_eruw1", 728.763733f, 710.812073f, 695.437622f, 679.204224f, 2.000090f, 5f); // Mid pool 1
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_eruw1", 726.807678f, 713.138184f, 697.743591f, 677.025757f, 2.000090f, 5f); // Mid pool 1
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_eruw1", 724.575378f, 715.400269f, 699.801758f, 674.766052f, 2.000090f, 5f); // Mid pool 1
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_eruw1", 723.000488f, 717.042480f, 701.335938f, 673.376221f, 2.000090f, 5f); // Mid pool 1
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_eruw1", 721.408508f, 718.618103f, 701.335938f, 673.376221f, 2.000090f, 5f); // Mid pool 1
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_eruw1", 863.735901f, 709.248230f, 559.081116f, 698.252502f, -15.999990f, 5f); // Bottom channel
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_eruw1", 762.302856f, 716.458130f, 661.015869f, 690.985657f, -15.999990f, 5f); // Bottom Pool 1
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_eruw1", 734.070862f, 720.952454f, 689.432556f, 686.778503f, -15.999990f, 5f); // Bottom Pool 1
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_eruw1", 729.625732f, 725.127869f, 693.982971f, 681.921570f, -15.999990f, 5f); // Bottom Pool 2
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_eruw1", 725.435303f, 729.403625f, 698.276855f, 677.747437f, -15.999990f, 5f); // Bottom Pool 3
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_eruw1", 721.138245f, 733.511414f, 702.827820f, 674.229980f, -15.999990f, 5f); // Bottom Pool 4
        }
    }
}
