//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2025 Nathan Handley
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
            // TODO: Add more zone areas
            SetZonewideAmbienceSound("", "darkwds1");
            Enable2DSoundInstances("wind_lp3", "wind_lp2", "wind_lp4");

            AddZoneArea("Desert", "", "", false, "", "silence");
            AddZoneAreaBox("Desert", 2655.683594f, 1829.046021f, 396.438385f, -924.193787f, -1038.153442f, -238.829483f);

            AddZoneLineBox("innothule", 2537.843262f, 1157.335449f, -28.670191f, ZoneLineOrientationType.South, -3172.916504f, 1030f, 38.835121f, -3225.501709f, 1057.282593f, -30f);
            AddZoneLineBox("oasis", -1859.231567f, 182.460098f, 2.406740f, ZoneLineOrientationType.North, 1526.327637f, 9.256500f, 131.793716f, 1478.424438f, 292.955048f, 1.148580f);
        }
    }
}
