//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2025 Nathan Handley
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
    internal class HowlingStonesZoneProperties : ZoneProperties
    {
        public HowlingStonesZoneProperties() : base()
        {
            AddZoneLineBox("overthere", -83.674156f, 825.339172f, -506.624969f, ZoneLineOrientationType.East, // Consider moving to the stone?
                -2.369990f, -722.148010f, 34.469002f, -15.200140f, -734.827942f, 17.500111f);
        }
    }
}
