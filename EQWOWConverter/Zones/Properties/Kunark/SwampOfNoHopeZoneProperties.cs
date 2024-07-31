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
    internal class SwampOfNoHopeZoneProperties : ZoneProperties
    {
        public SwampOfNoHopeZoneProperties() : base()
        {
            SetBaseZoneProperties("swampofnohope", "Swamp of No Hope", -1830f, -1259.9f, 27.1f, 0, ZoneContinent.Kunark);
            SetFogProperties(210, 200, 210, 60, 400);
            AddZoneLineBox("cabeast", -98.067253f, -657.688232f, 0.000070f, ZoneLineOrientationType.North,
                3172.572266f, 3068.755859f, 43.239300f, 3137.161865f, 3040.213135f, -0.374930f);
            AddZoneLineBox("cabeast", -280.219482f, -476.267853f, 0.000010f, ZoneLineOrientationType.West,
                2955.181396f, 3256.399658f, -0.375170f, 2955.181396f, 3256.399658f, -0.375170f);
        }
    }
}
