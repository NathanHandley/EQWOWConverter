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
    internal class IcewellKeepZoneProperties : ZoneProperties
    {
        public IcewellKeepZoneProperties() : base()
        {
            //AddValidMusicInstanceTrackIndexes(0, 1);

            AddDiscardGeometryBox(13.033260f, 17.105900f, 43.301620f, -16.485941f, -12.658110f, -4.878450f, "Tele-in room"); // Tele-in room
        }
    }
}
