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
    internal class CabilisWestZoneProperties : ZoneProperties
    {
        public CabilisWestZoneProperties() : base()
        {
            // TODO: Texture holes in the waterways

            SetZonewideEnvironmentAsOutdoorsNoSky(12, 12, 8, ZoneFogType.Heavy, 1f);

            AddZoneLineBox("cabeast", -28.278749f, 314.920990f, 0.000030f, ZoneLineOrientationType.South, -20.735310f, 322.030548f, 12.469000f, -33.827209f, 302.649109f, -0.499990f);
            AddZoneLineBox("cabeast", -28.944679f, 335.877106f, -24.860720f, ZoneLineOrientationType.South, -20.975170f, 350.067322f, 12.469000f, -41.966270f, 321.681580f, -42.468739f);
            AddZoneLineBox("cabeast", -28.406759f, 357.039429f, 0.000260f, ZoneLineOrientationType.South, -27.676720f, 364.034607f, 12.469000f, -49.269180f, 349.616089f, -0.500000f);
            AddZoneLineBox("warslikswood", -2253.605225f, -1121.567871f, 262.812622f, ZoneLineOrientationType.West, 887.849365f, 1192.889526f, 64.138229f, 857.462646f, 1153.048340f, -0.499980f);
            AddZoneLineBox("warslikswood", -2410.033447f, -934.157043f, 262.812653f, ZoneLineOrientationType.North, 739.584961f, 1343.662231f, 99.151367f, 698.854492f, 1313.275391f, -0.499970f);
            AddZoneLineBox("lakeofillomen", 6520.699707f, -6630.659180f, 35.093719f, ZoneLineOrientationType.South, -810.963440f, 783.879944f, 129.847549f, -860.993652f, 753.494934f, -0.500040f);
            AddZoneLineBox("lakeofillomen", 6331.367676f, -6786.975586f, 35.093800f, ZoneLineOrientationType.West, -971.431702f, 642.097107f, 166.406494f, -1001.788269f, 595.321289f, -0.499620f);

            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_cbw1", 422.922913f, 897.539062f, -220.803253f, 56.280869f, -13.999950f, 50f); // All center and north channels
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_cbw1", 407.932251f, 588.873962f, 362.916626f, 544.132324f, -0.999950f, 20f); // Small pool in the north, up high
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_cbw1", -147.919342f, 441.182068f, -479.545013f, 195.291428f, -13.999950f, 50f); // South connecting channels
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_cbw1", -369.980133f, 208.182693f, -497.524353f, 153.896286f, -13.999950f, 50f); // Southeast water areas
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_cbw1", -442.531891f, 210.270386f, -495.216644f, 143.655457f, -13.999950f, 15f); // Last high pool before pool step downs
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_cbw1", -441.499268f, 147.391403f, -493.871033f, 87.824089f, -27.999920f, 11f); // Mid-level pool connecting high and low
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_cbw1", -465.671326f, 91.795677f, -517.440674f, -41.573021f, -41.968731f, 67f); // Bottom pool with water tunnel going down
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_cbw1", -432.142548f, 35.032619f, -467.754150f, -17.948999f, -85.968727f, 100f); // Bottom pool outlet
            AddLiquidPlaneZLevel(ZoneLiquidType.Blood, "t75_b1", 322.494659f, 798.303650f, 292.494659f, 768.633362f, 12f, 20f); // Blood pool

            AddDiscardGeometryBox(26.142830f, 21.469500f, 61.976299f, -26.144300f, -23.115879f, -10.437380f); // 0 0 0 spawn room
        }
    }
}
