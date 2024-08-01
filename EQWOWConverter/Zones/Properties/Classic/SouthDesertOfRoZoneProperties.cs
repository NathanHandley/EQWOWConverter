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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Zones.Properties
{
    internal class SouthDesertOfRoZoneProperties : ZoneProperties
    {
        public SouthDesertOfRoZoneProperties() : base()
        {
            SetBaseZoneProperties("sro", "Southern Desert of Ro", 286f, 1265f, 79f, 0, ZoneContinentType.Antonica);
            SetFogProperties(250, 250, 180, 10, 800);
            AddZoneLineBox("innothule", 2537.843262f, 1157.335449f, -28.670191f, ZoneLineOrientationType.South, -3172.916504f, 1030f, 38.835121f, -3225.501709f, 1057.282593f, -30f);
            AddZoneLineBox("oasis", -1859.231567f, 182.460098f, 2.406740f, ZoneLineOrientationType.North, 1526.327637f, 9.256500f, 131.793716f, 1478.424438f, 292.955048f, 1.148580f);
        }
    }
}
