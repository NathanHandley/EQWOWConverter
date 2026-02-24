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
    internal class PlaneOfMischiefZoneProperties : ZoneProperties
    {
        public PlaneOfMischiefZoneProperties() : base()
        {
            AddDiscardGeometryBox(730.961975f, -702.425354f, 383.608917f, 590.080750f, -846.646484f, 308.057251f, " Floating purple curtians"); //  Floating purple curtians
            AddDiscardGeometryBox(329.129425f, -379.572754f, 233.762146f, 329.129425f, -379.572754f, 233.762146f, "White walls and floating purple liquid room near the middle"); // White walls and floating purple liquid room near the middle
            AddDiscardGeometryBox(458.106323f, -380.777161f, 239.702957f, 338.721161f, -438.332611f, 104.364090f, "Tele-in room in the underside"); // Tele-in room in the underside
        }
    }
}
