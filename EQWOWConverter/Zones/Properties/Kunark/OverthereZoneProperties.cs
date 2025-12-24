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
    internal class OverthereZoneProperties : ZoneProperties
    {
        public OverthereZoneProperties() : base()
        {
            SetZonewideEnvironmentAsOutdoorsWithSky(95, 89, 75, ZoneFogType.Clear, 0.5f, 1f);

            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1ot", 2079.233643f, -2777.568115f, 1818.492432f, -2979.514648f, -42.593590f, 30f); // Mud puddle (east)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1ot", -858.570618f, 2560.836914f, -1082.936523f, 2292.712646f, -58.593769f, 30f); // Mud puddle (west)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", 11259.541992f, 5907.908691f, 2288.494873f, -5216.059082f, -181.781113f, 500f); // Ocean
        }
    }
}
