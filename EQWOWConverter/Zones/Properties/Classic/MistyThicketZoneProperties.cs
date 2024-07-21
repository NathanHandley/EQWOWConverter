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
    internal class MistyThicketZoneProperties : ZoneProperties
    {
        public MistyThicketZoneProperties()
        {
            SetBaseZoneProperties("misty", "Misty Thicket", 0f, 0f, 2.43f, 0, ZoneContinent.Antonica);
            SetFogProperties(100, 120, 50, 10, 500);
            AddZoneLineBox("runnyeye", 231.871094f, 150.141602f, 1.001060f, ZoneLineOrientationType.South, -826.740295f, 1443.224487f, 3.532040f, -840.195496f, 1415.736206f, -11.249970f);
            AddZoneLineBox("rivervale", -83.344131f, 97.216301f, -0.000000f, ZoneLineOrientationType.East, 418.880157f, -2588.360596f, 11.531500f, 394.495270f, -2613.019531f, -11.249990f);
        }
    }
}
