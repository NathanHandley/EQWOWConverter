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
    internal class CastleMistmooreZoneProperties : ZoneProperties
    {
        public CastleMistmooreZoneProperties() : base()
        {
            SetBaseZoneProperties("mistmoore", "Castle Mistmoore", 123f, -295f, -177f, 0, ZoneContinentType.Faydwer);
            SetFogProperties(60, 30, 90, 10, 250);
            AddZoneLineBox("lfaydark", -1166.805908f, 3263.892578f, 0.000850f, ZoneLineOrientationType.East, -279.682556f, 141.644180f, -78.362358f, -339.412628f, 108.218033f, -182.437500f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", -207.922714f, 518.252563f, -409.548492f, 418.513092f, -237.916143f, 100f); // Entry pool, big
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 2.439530f, 95.290939f, -14.319680f, 84.488243f, -194.937485f, 5f); // Courtyard Pool Base 1
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 0.899840f, 96.857147f, -12.823690f, 83.049080f, -194.937485f, 5f); // Courtyard Pool Base 2
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", -0.571870f, 98.305656f, -11.297530f, 81.506592f, -194.937485f, 5f); // Courtyard Pool Base 3
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", -2.049700f, 90.969017f, -9.905360f, 89.007263f, -189.882553f, 10f); // Courtyard Pool Fountain N and S
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", -5.059470f, 93.924767f, -6.917720f, 86.123688f, -189.882553f, 10f); // Courtyard Pool Fountain W and E
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", -314.182220f, 77.662148f, -329.567352f, 14.306520f, -191.936478f, 10f); // Coffin room
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", -170.154343f, 95.783401f, -181.624146f, 84.045723f, -161.937485f, 5f); // small pool with steps
        }
    }
}
