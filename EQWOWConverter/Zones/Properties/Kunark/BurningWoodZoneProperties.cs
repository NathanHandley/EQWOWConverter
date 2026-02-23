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
    internal class BurningWoodZoneProperties : ZoneProperties
    {
        public BurningWoodZoneProperties() : base()
        {
            AddZoneLineBox("skyfire", -5439.923f, 1772.0016f, -162.23752f, ZoneLineOrientationType.North, 5302.864258f, 1900f, -110.222794f, 5118.025879f, 1400.658936f, -170.772964f);
            AddZoneLineBox("chardok", -2.107779f, 858.89545f, 99.968796f, ZoneLineOrientationType.North, 7339.295f, -4128.179f, -197.4685f, 7297.6826f, -4168.443f, -236.43733f);
            AddZoneLineBox("frontiermtns", -2404.3184f, 4043.9534f, -457.12997f, ZoneLineOrientationType.East, -2845.6619f, -4620.881f, 5.9380794f, -2976.7217f, -4548.8438f, -48.21263f);
            AddZoneLineBox("dreadlands", 2797.4214f, -814.58563f, 195.71893f, ZoneLineOrientationType.South, -4431.6133f, -509.9672f, 457.6962f, -4849.6196f, -1052.6932f, 176.06267f);

            AddDiscardGeometryBox(-5612.707031f, 282.552094f, 617.565186f, -6199.320312f, -1748.371826f, -61.609772f); // Dreadlands zone line
        }
    }
}
