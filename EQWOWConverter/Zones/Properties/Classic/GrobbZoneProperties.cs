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
    internal class GrobbZoneProperties : ZoneProperties
    {
        public GrobbZoneProperties()
        {
            SetBaseZoneProperties("grobb", "Grobb", 0f, -100f, 4f, 0, ZoneContinent.Antonica);
            SetFogProperties(0, 0, 0, 500, 2000);
            AddZoneLineBox("innothule", -2795.355469f, -654.658081f, -34.562538f, ZoneLineOrientationType.East, -169.745117f, 26.887341f, 28.469000f, -192.243027f, 9.193430f, -0.499990f);
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_gwater1", 152.433304f, 36.971439f, 94.668137f, -120.081017f, -1.999990f, 20f); // South river, near ent
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_gwater1", 346.868958f, -111.392517f, 177.726746f, -316.939789f, -1.999990f, 20f); // River in first section after south
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_gwater1", 749.410645f, -317.407654f, 617.161682f, -386.517914f, -17.999981f, 100f); // North pond with dock
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_gwater1", 137.184448f, -415.296997f, 63.208019f, -511.666992f, 38.000061f, 100f); // Inside pool next to south blood 2, east part
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_gwater1", 111.996399f, -410.413666f, 65.629272f, -415.726196f, 38.000061f, 100f); // Inside pool next to south blood 1, west part
            AddLiquidPlaneZLevel(LiquidType.Blood, "t75_m0006", 587.856934f, -837.093262f, 431.030212f, -970.739502f, 0.000010f, 100f); // East blood pool
            AddLiquidPlaneZLevel(LiquidType.Blood, "t75_m0006", 50.250309f, -430.840881f, -3.798120f, -485.456055f, 48.968781f, 100f); // South blood pool
            AddDisabledMaterialCollisionByNames("t50_gwater1", "t75_underwater1", "t50_m0003", "t75_m0006", "t75_m0007", "d_b1", "d_m0005");
        }
    }
}
