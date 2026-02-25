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
    internal class ErudsCrossingZoneProperties : ZoneProperties
    {
        public ErudsCrossingZoneProperties() : base()
        {
            // AddZoneArea("Fishing Camp", "erudsxing-01", "erudsxing-01", true, "", "", 0.8f); // Music was too loud
            // AddZoneAreaBox("Fishing Camp", -1387.844849f, 1039.357910f, 99.225990f, -1675.524048f, 651.345337f, -107.792427f);

            // AddZoneArea("Shipwreck");
            // AddZoneAreaBox("Shipwreck", -1022.513428f, 2311.508545f, 169.701233f, -1509.418091f, 1992.726685f, -313.425903f);

            // AddZoneArea("Island West", "erudsxing-00", "erudsxing-00");
            // AddZoneAreaBox("Island West", -624.765015f, 1917.588013f, 213.023804f, -1934.694458f, 1432.076538f, -111.334282f);

            // AddZoneArea("Island");
            // AddZoneAreaBox("Island", -356.611389f, 2056.785889f, 388.188568f, -2012.822754f, 514.036499f, -340.110535f);

            // AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", 3050.016846f, 5036.591309f, -758.372131f, -3051.582520f, -20.062160f, 500); // North
            // AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", 3050.016846f, 5036.591309f, -4999.445801f, 1787.470825f, -20.062160f, 500); // West
            // AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", -1693.079224f, 5036.591309f, -4999.445801f, -3051.582520f, -20.062160f, 500); // South
            // AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", 3050.016846f, 796.321411f, -4999.445801f, -3051.582520f, -20.062160f, 500); // East

            AddDiscardGeometryBox(3278.904541f, -3049.615723f, -8.505950f, -5080.248047f, -3092.001221f, -365.908600f, "East Edge"); // East Edge
            AddDiscardGeometryBox(-4998.655273f, 5111.410645f, -16.397030f, -5241.749512f, -3074.986572f, -335.999634f, "South Edge"); // South Edge
            AddDiscardGeometryBox(3332.018311f, 5252.545898f, -19.575190f, -5004.250488f, 5036.571289f, -396.631653f, "West Edge"); // West Edge
        }
    }
}
