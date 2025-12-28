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
    internal class DalnirZoneProperties : ZoneProperties
    {
        public DalnirZoneProperties() : base()
        {
            // Test/Update Z
            AddZoneLineBox("warslikswood", 2581.582275f, 4587.688965f, -244.136932f, ZoneLineOrientationType.West,
                23.855680f, 178.400238f, 17.282030f, -18.799231f, 82.444389f, -12.233050f); // Main walk entry
            AddZoneLineBox("warslikswood", 2581.582275f, 4587.688965f, -244.136932f, ZoneLineOrientationType.West,
                -334.306122f, -67.596291f, -166.470016f, -353.670563f, -90.821373f, -187.702927f); // Telepad at bottom

            SetZonewideEnvironmentAsIndoors(20, 10, 25, ZoneFogType.Medium);
            OverrideVertexColorIntensity(0.4);

            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_w1", 113.215393f, -718.826904f, 82.814590f, -778.763916f, -83.968681f, 10f); // Upper east pool with a checker rim
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_w1", 4.930180f, -745.163025f, -18.033091f, -752.353027f, -193.937424f, 10f); // Lower east pool that is a plus shape, north-south
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_w1", -3.185510f, -737.162292f, -10.629220f, -760.183655f, -193.937424f, 10f); // Lower east pool that is a plus shape, west-east
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_w1", 352.562042f, -12.688230f, 109.452881f, -406.200043f, -182.937485f, 10f); // Large west cave water area plus a little room near it
        }
    }
}
