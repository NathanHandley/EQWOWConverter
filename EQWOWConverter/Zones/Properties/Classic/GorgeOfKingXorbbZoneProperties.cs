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
    internal class GorgeOfKingXorbbZoneProperties : ZoneProperties
    {
        public GorgeOfKingXorbbZoneProperties()
        {
            SetBaseZoneProperties("beholder", "Gorge of King Xorbb", -21.44f, -512.23f, 45.13f, 0, ZoneContinent.Antonica);
            SetFogProperties(240, 180, 150, 10, 600);
            AddZoneLineBox("runnyeye", -111.2879f, -12.021315f, 0.000001f, ZoneLineOrientationType.East, 911.07355f, -1858.2123f, 15.469f, 894.673f, -1878.7317f, 0.50007594f);
            AddZoneLineBox("eastkarana", 3094.991f, -2315.8328f, 23.849806f, ZoneLineOrientationType.South, -1443.1708f, -542.2266f, 423.58218f, -1665.6155f, -747.2683f, 13f);
        }
    }
}
