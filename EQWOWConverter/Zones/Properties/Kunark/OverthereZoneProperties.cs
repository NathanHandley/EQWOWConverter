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
            // Fix Z
            AddZoneLineBox("frontiermtns", 4213.145020f, 1414.458862f, 332.436798f, ZoneLineOrientationType.South,
                2076.854248f, -3983.190186f, 1054.334229f, 1141.867676f, -4890.632812f, 288.149506f);            
            AddZoneLineBox("skyfire", -1091.753296f, -4317.972656f, 36.877560f, ZoneLineOrientationType.West,
                4288.374023f, -961.218262f, 176.188766f, 4226.748535f, -1021.012146f, 84.486160f); // North
            AddZoneLineBox("skyfire", -1199.551514f, -4318.401367f, 38.063560f, ZoneLineOrientationType.West,
                4297.203125f, -1058.881836f, 176.188766f, 4227.503906f, -1124.245972f, 84.486160f); // South
            AddZoneLineBox("warslikswood", 232.026474f, 3877.180908f, 232.809433f, ZoneLineOrientationType.East,
                -3967.926025f, 553.270874f, 498.073242f, -4382.918945f, -323.305664f, 103.104813f);

            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1ot", 2079.233643f, -2777.568115f, 1818.492432f, -2979.514648f, -42.593590f, 30f); // Mud puddle (east)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1ot", -858.570618f, 2560.836914f, -1082.936523f, 2292.712646f, -58.593769f, 30f); // Mud puddle (west)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", 11259.541992f, 5907.908691f, 2288.494873f, -5216.059082f, -181.781113f, 500f); // Ocean
        }
    }
}
