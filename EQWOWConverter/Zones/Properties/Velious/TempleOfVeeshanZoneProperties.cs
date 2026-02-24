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
    internal class TempleOfVeeshanZoneProperties : ZoneProperties
    {
        public TempleOfVeeshanZoneProperties() : base()
        {
            //AddValidMusicInstanceTrackIndexes(0, 2, 3);
            AddDiscardGeometryBox(24.609921f, 49.312328f, 41.225788f, -29.206511f, -14.658490f, -19.866989f, "Tele-in box"); // Tele-in box
        }
    }
}
