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
    internal class HighKeepZoneProperties : ZoneProperties
    {
        public HighKeepZoneProperties() : base()
        {
            SetBaseZoneProperties("highkeep", "High Keep", 88f, -16f, 4f, 0, ZoneContinentType.Antonica);
            SetFogProperties(0, 0, 0, 0, 0);
            AddZoneLineBox("highpass", 62.824429f, -112.595383f, 0.000000f, ZoneLineOrientationType.West, 70.162773f, 126.130470f, 12.469000f, 55.775291f, 104.252892f, -0.499970f);
            AddZoneLineBox("highpass", -90.567039f, -112.659950f, -0.000010f, ZoneLineOrientationType.West, -82.355392f, 112.775299f, 12.469000f, -98.161209f, 104.758774f, -0.500000f);
        }
    }
}
