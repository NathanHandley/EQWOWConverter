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
    internal class ChardokZoneProperties : ZoneProperties
    {
        public ChardokZoneProperties() : base()
        {
            AddZoneLineBox("burningwood", 7357.6494f, -4147.4604f, -235.93742f, ZoneLineOrientationType.North, -20.012981f, 879.84973f, 137.60643f, -70.907234f, 839.5071f, 99.46923f);
            AddZoneLineBox("burningwood", 7357.6494f, -4147.4604f, -235.93742f, ZoneLineOrientationType.North, 220.71272f, 895.73254f, 138.4065f, 157.77734f, 839.54913f, 99.468735f);

            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_charpoop1", 470.925812f, -398.946869f, 416.236481f, -481.242920f, -291.874908f, 50f); // Bottom SW pool, north part
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_charpoop1", 417.318481f, -412.656433f, 394.293762f, -513.047058f, -291.874908f, 50f); // Bottom SW pool, center part (north subpart)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_charpoop1", 394.398865f, -426.423615f, 381.498444f, -522.285645f, -291.874908f, 50f); // Bottom SW pool, center part (south subpart)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_charpoop1", 382.958008f, -437.959229f, 339.838165f, -522.285645f, -291.874908f, 50f); // Bottom SW pool, south part

            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_charpoop1", -240.193314f, 303.477509f, -436.053192f, 209.909393f, -129.937515f, 118f); // Top waterfall area, pool west
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_charpoop1", -240.193314f, 199.901260f, -436.053192f, 135.210648f, -129.937515f, 118f); // Top waterfall area, pool east
            AddLiquidVolume(ZoneLiquidType.Water, -240.193314f, 209.909393f, -436.053192f, 199.901260f, -125f, -250f); // Top waterfall area, pool middle
            AddLiquidVolume(ZoneLiquidType.Water, -202.769562f, 227.863815f, -229.939880f, 29.993500f, -152.712448f, -244.023239f); // Connecting channel between waterfall areas
            AddLiquidVolume(ZoneLiquidType.Water, -229.082047f, 227.863815f, -248.472107f, 195.453674f, -152.712448f, -244.023239f); // Connecting channel between waterfall areas - Small gap fill
            AddLiquidVolume(ZoneLiquidType.Water, -282.392853f, 209.909393f, -321.810089f, 199.901260f, -33.046249f, -182.316559f); // Top waterfall north waterfall
            AddLiquidVolume(ZoneLiquidType.Water, -337.887238f, 209.909393f, -373.652466f, 199.901260f, -33.046249f, -182.316559f); // Top waterfall south waterfall
            AddLiquidVolume(ZoneLiquidType.Water, -317.450999f, 209.909393f, -323.235504f, 199.901260f, -33.046249f, -96.968697f); // Top waterfall above door frame - Step 3 North
            AddLiquidVolume(ZoneLiquidType.Water, -323.225504f, 209.909393f, -325.929657f, 199.901260f, -33.046249f, -95.775993f); // Top waterfall above door frame - Step 2 North
            AddLiquidVolume(ZoneLiquidType.Water, -325.919657f, 209.909393f, -327.696167f, 199.901260f, -33.046249f, -93.414017f); // Top waterfall above door frame - Step 1 North
            AddLiquidVolume(ZoneLiquidType.Water, -327.696167f, 209.909393f, -331.944275f, 199.901260f, -33.046249f, -91.779327f); // Top waterfall above peak of door frame
            AddLiquidVolume(ZoneLiquidType.Water, -331.944275f, 209.909393f, -333.985300f, 199.901260f, -33.046249f, -93.573936f); // Top waterfall above door frame - Step 1 South
            AddLiquidVolume(ZoneLiquidType.Water, -333.995300f, 209.909393f, -336.289561f, 199.901260f, -33.046249f, -95.590172f); // Top waterfall above door frame - Step 2 South
            AddLiquidVolume(ZoneLiquidType.Water, -336.299561f, 209.909393f, -341.824473f, 199.901260f, -33.046249f, -96.968697f); // Top waterfall above door frame - Step 3 South

            // Lower surface = -309.875000
        }
    }
}
