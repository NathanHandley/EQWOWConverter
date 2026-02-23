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
    internal class OldSebilisZoneProperties : ZoneProperties
    {
        public OldSebilisZoneProperties() : base()
        {
            AddZoneLineBox("trakanon", -1628.483887f, -4764.273926f, -476.531189f, ZoneLineOrientationType.South, -90.748070f, -65.824310f, -16.669720f, -102.438210f, -78.004112f, -37.766270f, "Top exit teleporter");
            AddZoneLineBox("trakanon", -1628.483887f, -4764.273926f, -476.531189f, ZoneLineOrientationType.South, -669.088440f, -3.538890f, -152.304398f, -679.783447f, -15.525920f, -180.885757f, "Bottom exit teleporter");

            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_greenagua1", -1791.199951f, -292.427155f, -2109.412842f, -539.693359f, -197.906174f, 400f); // Trakanon's room
            AddLiquidPlaneZLevel(ZoneLiquidType.Blood, "d_b1", -1057.424805f, -661.983887f, -1156.666626f, -721.446960f, -125.937408f, 200f); // East blood room
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_greenagua1", -1033.014404f, 224.964600f, -1096.677979f, 135.804688f, -183.937439f, 200f); // Green Maze, Center
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_greenagua1", -768.731018f, 119.613373f, -886.085693f, -29.514400f, -181.937500f, 200f); // Green Maze, North
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_greenagua1", -1212.925293f, 88.140152f, -1258.094971f, 32.734772f, -181.937454f, 200f); // Green Maze, Small south pond with a rock
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_greenagua1", -812.376404f, 251.724243f, -921.374695f, 27.815760f, -61.968712f, 50f); // Temple entry above green maze
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_greenagua1", -266.255402f, 671.745178f, -453.683960f, 460.135956f, -71.968628f, 500f); // NW water, top area
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_greenagua1", -433.265350f, 767.345764f, -600.455383f, 630.779968f, -89.968697f, 500f); // NW water, bottom area
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_greenagua1", -252.250076f, 292.368134f, -581.209351f, -57.572189f, -181.937500f, 500f); // Lower cells water, connects 3 areas
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_greenagua1", -165.978638f, 275.321472f, -409.955811f, -146.305618f, -89.968742f, 58f); // Entry water
            AddOctagonLiquidShape(ZoneLiquidType.Water, "t75_greenagua1", -307.839233f, -321.949921f, -127.787514f, -141.970428f, -131.876724f, -137.983200f, -131.876724f, -137.983200f, -311.723114f, -318.011536f, -311.723114f, -318.011536f, -81.743187f, 500f); // Entry to lower water connect, east
            AddOctagonLiquidShape(ZoneLiquidType.Water, "t75_greenagua1", -181.800720f, -196.028885f, 268.020630f, 253.804016f, 264.039856f, 257.771210f, 264.039856f, 257.771210f, -185.870132f, -192.072296f, -185.870132f, -192.072296f, -81.743187f, 500f); // Entry to lower water connect, west
        }
    }
}
