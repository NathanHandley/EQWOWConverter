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
    internal class PlaneOfSkyZoneProperties : ZoneProperties
    {
        public PlaneOfSkyZoneProperties() : base()
        {
            // TODO: Add teleport pads
            SetBaseZoneProperties("airplane", "Plane of Sky", 542.45f, 1384.6f, -650f, 0, ZoneContinent.Antonica);
            SetFogProperties(0, 0, 0, 500, 2000);
            AddZoneLineBox("freporte", -363.75037f, -1778.4629f, 100f, ZoneLineOrientationType.West, 3000f, 3000f, -1000f, -3000f, -3000f, -1200f);
        }
    }
}
