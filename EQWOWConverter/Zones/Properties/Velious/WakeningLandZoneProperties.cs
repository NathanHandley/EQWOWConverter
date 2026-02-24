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
    internal class WakeningLandZoneProperties : ZoneProperties
    {
        public WakeningLandZoneProperties() : base()
        {
            //AddValidMusicInstanceTrackIndexes(0, 1);

            AddDiscardGeometryBox(5312.046875f, 5181.260742f, 391.377960f, 3551.554199f, -5015.944824f, -366.186066f, "Northern edge"); // Northern edge
        }
    }
}
