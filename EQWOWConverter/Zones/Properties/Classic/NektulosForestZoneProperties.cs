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
    internal class NektulosForestZoneProperties : ZoneProperties
    {
        public NektulosForestZoneProperties() : base()
        {
            // TODO: Trees below the zone need to be deleted
            // TODO: More zone areas
            // TODO: BUG: Missing block at 2522.321289f, 677.535767f, -8.108790f, see reference
            // AddZoneArea("Neriak");
            // AddZoneAreaBox("Neriak", 2539.906494f, -707.843018f, 120.655533f, 2091.832031f, -1205.748169f, -28.141399f);

            // AddZoneArea("Lavastorm Path", "nektulos-00", "nektulos-00", true, "wind_lp2", "wind_lp2", 0.75f); // Lowered music volume a bit
            // AddZoneAreaBox("Lavastorm Path", 3581.946777f, 863.359070f, 281.967194f, 2433.607910f, 437.545807f, -117.007523f);
            // AddZoneAreaBox("Lavastorm Path", 3581.946777f, 863.359070f, 281.967194f, 2675.020264f, 170.838821f, -162.906296f);

            AddZoneLineBox("ecommons", 1569.311157f, 667.254028f, -21.531260f, ZoneLineOrientationType.East, -2666.610596f, -550.025208f, 31.661320f, -2707.922119f, -636.140076f, -22.031050f);
            AddZoneLineBox("lavastorm", -2075.322998f, -189.802826f, -19.598631f, ZoneLineOrientationType.North, 3164.449707f, 385.575653f, 151.052231f, 3094.654785f, 237.925507f, -19.999571f);
            AddZoneLineBox("neriaka", -0.739280f, 113.977829f, 28.000000f, ZoneLineOrientationType.East, 2270.015381f, -1092.749023f, 12.469000f, 2210.318115f, -1149.777344f, -0.499900f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", -229.968414f, 2123.114746f, -848.817383f, 945.974426f, -30.156151f, 200f); // West most part of river
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", -545.284668f, 946.974426f, -1134.071777f, -186.474213f, -30.156151f, 200f); // Middle part of river
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", -853.824585f, -185.474213f, -1614.697388f, -1905.550659f, -30.156151f, 200f); // East part of river
        }
    }
}
