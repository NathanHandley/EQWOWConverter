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
    internal class PlaneOfFearZoneProperties : ZoneProperties
    {
        public PlaneOfFearZoneProperties()
        {
            SetBaseZoneProperties("fearplane", "Plane of Fear", 1282.09f, -1139.03f, 1.67f, 0, ZoneContinent.Antonica);
            SetFogProperties(255, 50, 10, 10, 1000);
            AddZoneLineBox("feerrott", -2347.395752f, 2604.589111f, 10.280410f, ZoneLineOrientationType.North, -790.410828f, 1052.103638f, 150.821121f, -803.796631f, 1015.684509f, 105.875198f);
        }
    }
}
