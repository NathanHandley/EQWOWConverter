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
    internal class SkyfireMountainsZoneProperties : ZoneProperties
    {
        public SkyfireMountainsZoneProperties() : base()
        {
            SetBaseZoneProperties("skyfire", "Skyfire Mountains", -3931.32f, -1139.25f, 39.76f, 0, ZoneContinent.Kunark);
            SetFogProperties(235, 200, 200, 200, 600);
            AddZoneLineBox("burningwood", 5087.0146f, 1740.0859f, -163.56395f, ZoneLineOrientationType.South,
                -5623.817f, 1910.7054f, -56.840195f, -5703.1704f, 1580.5497f, -164.28036f); // Zone-in had no geometery
        }
    }
}
