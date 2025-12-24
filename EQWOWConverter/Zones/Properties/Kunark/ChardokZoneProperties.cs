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
            SetZonewideEnvironmentAsIndoors(26, 26, 26, ZoneFogType.Medium);
            OverrideVertexColorIntensity(0.4);

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
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_charpoop1", -169.871796f, 40.012051f, -229.886078f, 29.965820f, -165.752747f, 300f); // Bottom waterfall, eastmost straight part
            AddTrapezoidLiquidAxisAlignedZLevelShape(ZoneLiquidType.Water, "t75_charpoop1", -149.936691f, -170.134033f, 30.012831f, 19.987000f, 39.970360f, 30.001060f, -165.752747f, 300f, 0.4f); // Bottom waterfall, segment towards north 1
            AddTrapezoidLiquidAxisAlignedZLevelShape(ZoneLiquidType.Water, "t75_charpoop1", -119.924599f, -149.974518f, -0.001660f, -10.007690f, 30.296391f, 20.338970f, -165.752747f, 300f, 0.4f); // Bottom waterfall, segment towards north 2
            AddTrapezoidLiquidAxisAlignedZLevelShape(ZoneLiquidType.Water, "t75_charpoop1", -99.727631f, -119.934599f, -9.842380f, -20.124001f, 0.010350f, -10.002630f, -165.752747f, 300f, 0.4f); // Bottom waterfall, segment towards north 3
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_charpoop1", 104.317886f, 92.410110f, -97.481148f, -15.401990f, -309.875000f, 200f);  // Bottom waterfall room, north segment
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_charpoop1", 11.186770f, 92.410110f, -235.288635f, 39.989948f, -309.875000f, 200f);  // Bottom waterfall room, north segment 2 (more south)
            AddTrapezoidLiquidAxisAlignedZLevelShape(ZoneLiquidType.Water, "t75_charpoop1", -96.311836f, -120.171806f, 36.136162f, -11.942400f, 36.136162f, -0.897420f, -309.875000f, 200f, 0.4f);  // Bottom waterfall room, north part east waterfall edge
            AddTrapezoidLiquidAxisAlignedZLevelShape(ZoneLiquidType.Water, "t75_charpoop1", -119.640533f, -149.998077f, 83.592430f, -0.925950f, 83.592430f, 29.794000f, -309.875000f, 200f, 0.4f);  // Bottom waterfall room, north part east 1 south waterfall edge
            AddTrapezoidLiquidAxisAlignedZLevelShape(ZoneLiquidType.Water, "t75_charpoop1", -148.821350f, -170.195084f, 85.823959f, 29.007650f, 85.823959f, 39.469650f, -309.875000f, 200f, 0.4f);  // Bottom waterfall room, north part east 2 south waterfall edge
            AddTrapezoidLiquidAxisAlignedZLevelShape(ZoneLiquidType.Water, "t75_charpoop1", -149.470047f, -170.117493f, 20.457099f, -49.469860f, 30.297831f, -49.469860f, -309.875000f, 200f, 0.4f);  // Bottom waterfall room, south part west 1 north waterfall edge
            AddTrapezoidLiquidAxisAlignedZLevelShape(ZoneLiquidType.Water, "t75_charpoop1", -119.754639f, -150.053299f, -9.202310f, -43.145031f, 21.217360f, -43.145031f, -309.875000f, 200f, 0.4f);  // Bottom waterfall room, south part west 2 north waterfall edge
            AddTrapezoidLiquidAxisAlignedZLevelShape(ZoneLiquidType.Water, "t75_charpoop1", -99.266296f, -121.175636f, -19.818331f, -41.579128f, -7.578530f, -41.579128f, -309.875000f, 200f, 0.4f);  // Bottom waterfall room, south part west 3 north waterfall edge
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_charpoop1", -169.889145f, 30.030069f, -262.887299f, -50.600559f, -309.875000f, 200f);  // Bottom waterfall room, south segment (north part of south area)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_charpoop1", -90.736572f, -20.900070f, -262.887299f, -50.600559f, -309.875000f, 200f);  // Bottom waterfall room, south segment (south part)
        }
    }
}
