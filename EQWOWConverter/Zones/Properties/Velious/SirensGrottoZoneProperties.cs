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
    internal class SirensGrottoZoneProperties : ZoneProperties
    {
        public SirensGrottoZoneProperties() : base()
        {
            AddZoneLineBox("cobaltscar", 1584.026611f, 1626.080811f, 62.937771f, ZoneLineOrientationType.South, -600.025208f, 83.160942f, -72.500954f, -625.650818f, 62.775249f, -97.468727f);
        }
    }
}
