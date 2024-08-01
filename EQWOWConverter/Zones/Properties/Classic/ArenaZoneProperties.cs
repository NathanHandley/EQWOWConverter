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
    internal class ArenaZoneProperties : ZoneProperties
    {
        public ArenaZoneProperties() : base()
        {
            SetBaseZoneProperties("arena", "The Arena", 460.9f, -41.4f, -7.38f, 0, ZoneContinentType.Antonica);
            SetFogProperties(100, 100, 100, 10, 1500);
            AddZoneLineBox("lakerathe", 2345.1172f, 2692.0679f, 92.193184f, ZoneLineOrientationType.East, -44.28722f, -845.03625f, 45.75025f, -74.66106f, -871.419f, 7.0403852f);
        }
    }
}
