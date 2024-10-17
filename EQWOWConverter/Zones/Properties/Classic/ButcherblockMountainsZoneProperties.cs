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
    internal class ButcherblockMountainsZoneProperties : ZoneProperties
    {
        public ButcherblockMountainsZoneProperties() : base()
        {
            // TODO: Add more zone areas

            // Note: There should be a boat to Firiona Vie [Timorous Deep] (NYI) and a boat to Freeport [Ocean of Tears] (NYI)
            SetBaseZoneProperties("butcher", "Butcherblock Mountains", -700f, 2550f, 2.9f, 0, ZoneContinentType.Faydwer);
            
            AddZoneArea("Kaladim Entrance", "butcher-01", "butcher-01");
            AddZoneAreaBox("Kaladim Entrance", 3046.260742f, 334.174408f, 335.738495f, 2679.131836f, -575.568665f, -52.174461f);
            
            AddZoneArea("Skeleton Point", "butcher-00", "butcher-00");
            AddZoneAreaBox("Skeleton Point", -1232.642456f, 931.783386f, 414.072693f, -1783.278076f, 681.203186f, -24.140810f);
            
            AddZoneArea("Faydwer Port");
            AddZoneAreaBox("Faydwer Port", 1551.215576f, 3391.010010f, 101.499222f, 723.625549f, 2465.903320f, -182.151886f);
            
            AddZoneArea("Chess Board");
            AddZoneAreaBox("Chess Board", 1302.726196f, -1978.077881f, 337.046906f, 281.085815f, -3012.630127f, -118.726242f);
            
            Enable2DSoundInstances("torch_lp", "wind_lp4", "darkwds2", "wind_lp2", "darkwds1");
            AddZoneLineBox("kaladima", -60.207775f, 41.798244f, 0.0010997541f, ZoneLineOrientationType.North, 3145.1406f, -173.6824f, 14.468006f, 3128.918f, -186.06715f, -0.4991133f);
            AddZoneLineBox("gfaydark", -1563.382568f, 2626.150391f, -0.126430f, ZoneLineOrientationType.North, -1180.5581f, -3073.2896f, 67.52528f, -1218.3838f, -3150f, -0.4993223f);
            AddZoneLineBox("cauldron", 2853.7092f, 264.44955f, 469.3444f, ZoneLineOrientationType.South, -2937.8154f, -317.8051f, 45.09004f, -2957.5332f, -351.47528f, -0.49968797f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", 3407.252686f, 5015.645996f, -2835.589355f, 2882.903320f, -12.718050f, 500f);
        }
    }
}
