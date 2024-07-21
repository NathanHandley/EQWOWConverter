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
    internal class PlaneOfHateZoneProperties : ZoneProperties
    {
        public PlaneOfHateZoneProperties()
        {
            // TODO: Need to identify a new zone in / zone out for this zone
            SetBaseZoneProperties("hateplane", "Plane of Hate", -353.08f, -374.8f, 3.75f, 0, ZoneContinent.Antonica);
            SetFogProperties(128, 128, 128, 30, 200);
        }
    }
}
