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
    internal class ErudsCrossingZoneProperties : ZoneProperties
    {
        public ErudsCrossingZoneProperties()
        {
            // TODO: There's a boat that connects to erudnext and qeynos (south)
            SetBaseZoneProperties("erudsxing", "Erud's Crossing", 795f, -1766.9f, 12.36f, 0, ZoneContinent.Odus);
            SetFogProperties(200, 200, 220, 10, 800);
            AddLiquidPlaneZLevel(LiquidType.Water, "d_w1", 3050.016846f, 5036.591309f, -4999.445801f, -3051.582520f, -20.062160f, 500);
            AddDisabledMaterialCollisionByNames("d_w1");
        }
    }
}
