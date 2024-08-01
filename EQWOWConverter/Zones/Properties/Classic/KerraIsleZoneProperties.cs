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
    internal class KerraIsleZoneProperties : ZoneProperties
    {
        public KerraIsleZoneProperties() : base()
        {
            SetBaseZoneProperties("kerraridge", "Kerra Isle", -859.97f, 474.96f, 23.75f, 0, ZoneContinentType.Odus);
            SetFogProperties(220, 220, 200, 10, 600);
            AddZoneLineBox("tox", -510.562134f, 2635.008545f, -38.249962f, ZoneLineOrientationType.East, 430.005493f, -948.882141f, 38.436760f, 399.657959f, -979.802734f, 19.500050f);
            AddLiquidPlaneZLevel(LiquidType.Water, "d_agua1", 921.841675f, -298.145691f, -803.680969f, -652.802063f, -0.009930f, 100f); // North and Eastern section
            AddLiquidPlaneZLevel(LiquidType.Water, "d_agua1", 105.104973f, 487.097412f, -463.912354f, -299.266937f, -0.009930f, 100f); // Main lake, north
            AddLiquidPlaneZLevel(LiquidType.Water, "d_agua1", -463.825409f, 451.519562f, -803.680969f, -107.275681f, -0.009930f, 100f); // Main lake, west
            AddLiquidPlaneZLevel(LiquidType.Water, "d_agua1", -548.308899f, -105.716133f, -803.680969f, -299.282684f, -0.009930f, 100f); // Main lake, south
        }
    }
}
