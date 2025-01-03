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

using EQWOWConverter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Zones.Properties
{
    internal class RunnyeyeCitadelZoneProperties : ZoneProperties
    {
        public RunnyeyeCitadelZoneProperties() : base()
        {
            // TODO: Add zone areas
            SetBaseZoneProperties("runnyeye", "Runnyeye Citadel", -21.85f, -108.88f, 3.75f, 0, ZoneContinentType.Antonica);
            SetZonewideEnvironmentAsIndoors(26, 52, 9, ZoneFogType.Heavy);
            OverrideVertexColorIntensity(0.4f);
            DisableSunlight();
            Enable2DSoundInstances("slmeloop", "slmestrm");

            AddZoneArea("Upper Pools", "runnyeye-00", "runnyeye-00");
            AddZoneAreaBox("Upper Pools", 228.738785f, 134.024902f, 28.439110f, -49.520500f, 36.699299f, -19.277330f);
            AddZoneAreaBox("Upper Pools", 220.338425f, 141.395691f, 24.374371f, 103.887222f, 33.761250f, -21.572861f);
            AddZoneAreaBox("Upper Pools", 120.590248f, 136.948044f, 21.733240f, -44.972610f, -34.389851f, -15.163530f);
            AddZoneAreaBox("Upper Pools", 79.259552f, 35.387619f, 52.870529f, -22.043350f, -110.600601f, -16.695601f);

            AddZoneArea("Eye Pool", "runnyeye-01", "runnyeye-01");
            AddZoneAreaBox("Eye Pool", 155.185547f, -59.430408f, -20.543591f, 53.687580f, -145.624680f, -67.274200f);

            AddZoneArea("Blood Room", "runnyeye-02", "runnyeye-00");
            AddZoneAreaBox("Blood Room", 35.168320f, 72.665810f, -18.138041f, -59f, -68.910408f, -63.175690f);
            AddZoneAreaBox("Blood Room", 35.168320f, 48.297878f, -18.138041f, -59f, -52.180302f, -63.175690f);
            AddZoneAreaBox("Blood Room", -58.263672f, 40.957069f, -46.533749f, -76f, -37.296391f, -50f);

            AddZoneLineBox("beholder", 903.2041f, -1850.1808f, 1.0001143f, ZoneLineOrientationType.West, -102.775955f, 12.901143f, 15.468005f, -119.129944f, -8.304958f, -0.49999338f);
            AddZoneLineBox("misty", -816.631531f, 1427.580444f, -10.751390f, ZoneLineOrientationType.North, 271.099457f, 170f, 15.469000f, 250.497299f, 135.744324f, 0.501060f);
            AddOctagonLiquidShape(ZoneLiquidType.Water, "t50_agua1", 6.060460f, -4.080790f, 5.092600f, -5.076620f, 2.997610f, -2.994190f, 2.997610f, -2.994190f,
                4.004600f, -1.993800f, 4.004600f, -1.993800f, -128.937500f, 100f); // Bottom well entry
            AddLiquidVolume(ZoneLiquidType.Water, 20.882460f, 26.487289f, -28.736370f, -28.261860f, -148.937500f, -172.924393f); // Bottom well outlet into the lowest path area
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_agua1", 294.765076f, 106.255119f, 6.004130f, -258.832855f, -134.937424f, 150f); // North lower area
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_agua1", -4.007230f, 20.985340f, -278.851288f, -264.851929f, -134.937424f, 150f); // South lower area
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_agua1", 6.014130f, 32.763168f, -4.017230f, 5.014490f, -134.937424f, 150f);  // West lower area 
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_agua1", 6.014130f, -5.003880f, -4.017230f, -264.851929f, -134.937424f, 150f);  // East lower area
            AddLiquidPlaneZLevel(ZoneLiquidType.GreenWater, "t25_slime1", 209.147537f, 125.676064f, 183.607407f, 75.932487f, -1.999960f, 12f);// Top green pool
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_agua1", 24.071569f, 43.730431f, 2.673260f, -60.384258f, -2.000000f, 12f); // Top water pool
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_agua1", 188.957626f, 182.237473f, -134.247452f, 85.751991f, -50.968750f, 13.2f); // Second level water, north and west
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_agua1", -75.618561f, 182.247473f, -160.054581f, -108.079483f, -50.968750f, 13.2f); // Second level water, south and east                        
            AddLiquidPlaneZLevel(ZoneLiquidType.GreenWater, "t25_slime1", 109.272560f, -84.608208f, 72.763344f, -102.594093f, -63.968750f, 6.1f); // 3rd Level Slime - Entry with the small 'shelf' inside and steam
            AddLiquidPlaneZLevel(ZoneLiquidType.GreenWater, "t25_slime1", 102.162209f, -84.790802f, 72.763344f, -104.623718f, -63.968750f, 47.5f); // 3rd Level Slime - Tunnel under shelf and steam
            AddLiquidPlaneZLevel(ZoneLiquidType.GreenWater, "t25_slime1", 240.866287f, 195.361618f, 111.132767f, 31.060631f, -99.968750f, 12f); // 3rd Level Slime - Northwest corner
            AddLiquidPlaneZLevel(ZoneLiquidType.GreenWater, "t25_slime1", 382.286346f, 31.070631f, 130.300415f, -143.681931f, -99.968750f, 12f); // 3rd Level Slime - Northeast corner
            AddLiquidPlaneZLevel(ZoneLiquidType.GreenWater, "t25_slime1", 130.310415f, -102.566994f, 32.193481f, -143.681931f, -99.968750f, 12f); // 3rd Level Slime - East center, in front of the steam-topped down channel
            AddLiquidPlaneZLevel(ZoneLiquidType.GreenWater, "t25_slime1", -177.592316f, -66.531921f, -190.357666f, -79.460022f, -85.978727f, 25.5f); // 3rd Level Slime - Southmost false floor channel
            AddLiquidPlaneZLevel(ZoneLiquidType.GreenWater, "t25_slime1", -190.347666f, -49.662540f, -204.915298f, -92.592140f, -99.968750f, 12f); // 3rd Level Slime - South of southmost floor channel
            AddLiquidPlaneZLevel(ZoneLiquidType.GreenWater, "t25_slime1", -177.592316f, -49.672540f, -190.357666f, -66.541921f, -99.968750f, 12f); // 3rd Level Slime - West of southmost floor channel
            AddLiquidPlaneZLevel(ZoneLiquidType.GreenWater, "t25_slime1", -177.592316f, -79.450022f, -190.357666f, -92.602140f, -99.968750f, 12f); // 3rd Level Slime - East of southmost floor channel
            AddLiquidPlaneZLevel(ZoneLiquidType.GreenWater, "t25_slime1", -65.935371f, -49.672540f, -177.602316f, -122.502213f, -99.968750f, 12f); // 3rd Level Slime - Nearest north of southmost floor channel
            AddLiquidPlaneZLevel(ZoneLiquidType.GreenWater, "t25_slime1", -1.994960f, -100.078423f, -65.945371f, -134.621674f, -99.968750f, 12f); // 3rd Level Slime - Between two inlets, just south of close stairs
            AddLiquidPlaneZLevel(ZoneLiquidType.GreenWater, "t25_slime1", 32.363239f, -103.054077f, -1.984960f, -134.381683f, -99.968750f, 12f); // 3rd Level Slime - Between two inlets, closer to the stairs
            AddLiquidPlaneZLevel(ZoneLiquidType.GreenWater, "t25_slime1", 33.061699f, 42.747589f, -37.459019f, -45.247662f, -93.968742f, 12f); // 3rd Level Slime - mid-step up pool
            AddAlwaysBrightMaterial("d_herthwall");
            AddAlwaysBrightMaterial("d_hearth1");
        }
    }
}
