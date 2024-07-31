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
    internal class ChardokZoneProperties : ZoneProperties
    {
        public ChardokZoneProperties() : base()
        {
            SetBaseZoneProperties("chardok", "Chardok", 859f, 119f, 106f, 0, ZoneContinent.Kunark);
            SetFogProperties(90, 53, 6, 30, 300);
            AddZoneLineBox("burningwood", 7357.6494f, -4147.4604f, -235.93742f, ZoneLineOrientationType.North,
                -20.012981f, 879.84973f, 137.60643f, -70.907234f, 839.5071f, 99.46923f);
            AddZoneLineBox("burningwood", 7357.6494f, -4147.4604f, -235.93742f, ZoneLineOrientationType.North,
                220.71272f, 895.73254f, 138.4065f, 157.77734f, 839.54913f, 99.468735f);
        }
    }
}
