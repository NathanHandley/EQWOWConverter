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
    internal class CityOfMistZoneProperties : ZoneProperties
    {
        public CityOfMistZoneProperties()
        {
            // TODO: Any in-zone teleports?
            SetBaseZoneProperties("citymist", "City of Mist", -734f, 28f, 3.75f, 0, ZoneContinent.Kunark);
            SetFogProperties(90, 110, 60, 50, 275);
            AddZoneLineBox("emeraldjungle", 0.121500f, -774.691650f, 0.000000f, ZoneLineOrientationType.West,
                309.691193f, -1730.243408f, -300.343506f, 291.030334f, -1789.959473f, -335.468658f);
        }
    }
}
