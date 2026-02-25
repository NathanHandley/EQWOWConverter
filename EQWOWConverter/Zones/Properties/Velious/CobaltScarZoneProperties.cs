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

namespace EQWOWConverter.Zones.Properties
{
    internal class CobaltScarZoneProperties : ZoneProperties
    {
        public CobaltScarZoneProperties() : base()
        {
            // AddZoneLineBox("sirens", -595.916992f, 73.038841f, -96.968727f, ZoneLineOrientationType.North, 1604.295898f, 1636.723511f, 87.406502f, 1588.378052f, 1616.337891f, 62.437771f);

            AddDiscardGeometryBox(2097.724854f, -1996.453125f, 100.228210f, -1827.655151f, -6000f, -246.045593f, "Northeast water"); // Northeast water
        }
    }
}
