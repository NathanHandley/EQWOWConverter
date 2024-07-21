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
    internal class BefallenZoneProperties : ZoneProperties
    {
        public BefallenZoneProperties()
        {
            SetBaseZoneProperties("befallen", "Befallen", 35.22f, -75.27f, 2.19f, 0, ZoneContinent.Antonica);
            SetFogProperties(30, 30, 90, 10, 175);
            AddZoneLineBox("commons", -1155.6317f, 596.3344f, -42.280308f, ZoneLineOrientationType.North, -49.9417f, 42.162197f, 12.469f, -63.428658f, 27.86666f, -0.5000006f);
        }
    }
}
