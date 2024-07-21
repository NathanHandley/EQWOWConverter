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
    internal class EastKaranaZoneProperties : ZoneProperties
    {
        public EastKaranaZoneProperties()
        {
            SetBaseZoneProperties("eastkarana", "Eastern Plains of Karana", 0f, 0f, 3.5f, 0, ZoneContinent.Antonica);
            SetFogProperties(200, 200, 220, 10, 800);
            AddZoneLineBox("beholder", -1385.247f, -659.5757f, 60.639446f, ZoneLineOrientationType.North, 3388.710449f, -2134.555420f, 322.495361f, 3160.392090f, -2401.121826f, -100);
            AddZoneLineBox("northkarana", 10.664860f, -3093.490234f, -37.343510f, ZoneLineOrientationType.West, 38.202431f, 1198.431396f, 32.241810f, -13.265930f, 1182.535156f, -37.843681f);
            AddZoneLineBox("highpass", -1014.530701f, 112.901894f, -0.000030f, ZoneLineOrientationType.East, -3062.753662f, -8301.240234f, 737.270081f, -3082.371826f, -8324.481445f, 689.406372f);
            AddLiquidPlaneZLevel(LiquidType.Water, "d_w1", 3007.819092f, 1837.666504f, -3782.756836f, 551.661438f, -74.156052f, 500f);
            AddLiquidPlaneZLevel(LiquidType.Water, "d_w1", -3772.680420f, 1837.766504f, -5798.433105f, -4512.786133f, -74.156052f, 500f);
            AddDisabledMaterialCollisionByNames("d_w1");
        }
    }
}
