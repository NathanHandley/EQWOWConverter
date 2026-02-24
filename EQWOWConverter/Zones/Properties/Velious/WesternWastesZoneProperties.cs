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
    internal class WesternWastesZoneProperties : ZoneProperties
    {
        public WesternWastesZoneProperties() : base()
        {
            AddDiscardGeometryBox(-6907.262695f, 7068.784668f, -223.119186f, -7047.204590f, -6294.494629f, -441.381866f, "South ocean paremeter"); // South ocean paremeter
            AddDiscardGeometryBox(7345.539062f, 7049.416992f, -274.974518f, -7073.721191f, 6972.919922f, -521.189514f, "West ocean paremeter"); // West ocean paremeter
            AddDiscardGeometryBox(7350.208008f, -4984.016602f, -282.146088f, -7002.995117f, -6198.897949f, -689.360107f, "East ocean parameter"); // East ocean parameter
        }
    }
}
