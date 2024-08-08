//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2024 Nathan Handley
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
    internal class CrushboneZoneProperties : ZoneProperties
    {
        public CrushboneZoneProperties() : base()
        {
            SetBaseZoneProperties("crushbone", "Crushbone", 158f, -644f, 4f, 0, ZoneContinentType.Faydwer);
            SetZonewideEnvironmentAsOutdoors(20, 20, 120, ZoneFogType.Heavy, true, 0.5f, 1.0f, 1.0f, 0.25f);
            AddZoneLineBox("gfaydark", 2561.247803f, -52.142502f, 15.843880f, ZoneLineOrientationType.South, -640.919861f, 187.129715f, 39.221329f, -732.241028f, 141.981308f, -0.500000f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0002", 257.281067f, 826.939575f, -280.881683f, 567.900513f, -34.999939f, 150f); // NW water plane
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0002", -252.334946f, 658.722595f, -608.327148f, 420.405396f, -11.999990f, 50f); // SW water plane (higher up)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0002", -252.334946f, 658.722595f, -608.327148f, 420.405396f, -11.999990f, 50f); // Upper river towards keep circle (southmore)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0002", -71.529442f, 479f, -258.593109f, 392.106171f, -11.999990f, 50f); // Upper river towards keep circle
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0002", -53.215691f, 430.083923f, -73.401512f, 319.778778f, -11.999990f, 50f); // Upper river towards keep circle, side part 1
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0002", 4.901690f, 343.037506f, -60.139000f, 290.808258f, -11.999990f, 50f); // Upper river towards keep circle, side part 2
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0002", 325.689392f, 292.879547f, -56.076481f, 87.385933f, -11.999990f, 50f); // West river area around keep
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0002", 301.031555f, 90.831619f, 223.354294f, 27.758240f, -11.999990f, 50f); // Northwest river area around keep
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0002", 270.780273f, 28.266529f, 212.974243f, -8.258000f, -11.999990f, 50f); // North east river area around keep 1
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0002", 252.122513f, -7.610770f, 192.806335f, -42.189678f, -11.999990f, 50f); // North east river area around keep 2
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0002", 234.143616f, -41.506008f, 145.427017f, -68.262947f, -11.999990f, 50f); // North east river area around keep 3
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0002", 213.689957f, -67.187302f, 126.517197f, -96.146927f, -11.999990f, 50f); // North east river area around keep 4
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0002", 171.805862f, -95.562683f, 128.086807f, -144.639587f, -11.999990f, 50f); // North east river area around keep 5
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0002", 146.060959f, -59.722210f, -8.454320f, -174.799026f, -11.999990f, 50f); // East river area around keep
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0002", 26.958090f, 94.326317f, -61.008018f, -107.154488f, -11.999990f, 50f); // South river area around keep
            AddLiquidPlaneZLevel(ZoneLiquidType.Blood, "t50_m0003", 84.625970f, 0.410270f, 72.905113f, -13.311320f, 15.000010f, 150f); // Blood in south keep
            AddLiquidPlaneZLevel(ZoneLiquidType.Blood, "t50_m0003", 84.321714f, 1.77798f, 72.905113f, -13.311320f, 15.000010f, 150f); // Blood in south keep
            AddLiquidPlaneZLevel(ZoneLiquidType.Blood, "t50_m0003", 84.017458f, 3.145690f, 72.905113f, -13.311320f, 15.000010f, 150f); // Blood in south keep
            AddLiquidPlaneZLevel(ZoneLiquidType.Blood, "t50_m0003", 83.61664f, 4.93148f, 72.905113f, -13.311320f, 15.000010f, 150f); // Blood in south keep
            AddLiquidPlaneZLevel(ZoneLiquidType.Blood, "t50_m0003", 83.215822f, 6.717270f, 72.905113f, -13.311320f, 15.000010f, 150f); // Blood in south keep
            AddLiquidPlaneZLevel(ZoneLiquidType.Blood, "t50_m0003", 82.831182f, 8.457405f, 72.905113f, -13.311320f, 15.000010f, 150f); // Blood in south keep
            AddLiquidPlaneZLevel(ZoneLiquidType.Blood, "t50_m0003", 82.446542f, 10.197540f, 72.905113f, -13.311320f, 15.000010f, 150f); // Blood in south keep
            AddLiquidPlaneZLevel(ZoneLiquidType.Blood, "t50_m0003", 82.0374695f, 12.029025f, 72.905113f, -13.311320f, 15.000010f, 150f); // Blood in south keep
            AddLiquidPlaneZLevel(ZoneLiquidType.Blood, "t50_m0003", 81.628397f, 13.860510f, 72.905113f, -13.311320f, 15.000010f, 150f); // Blood in south keep
            AddLiquidPlaneZLevel(ZoneLiquidType.Blood, "t50_m0003", 81.343359f, 15.144805f, 72.905113f, -13.311320f, 15.000010f, 150f); // Blood in south keep
            AddLiquidPlaneZLevel(ZoneLiquidType.Blood, "t50_m0003", 81.058321f, 16.429100f, 72.905113f, -13.311320f, 15.000010f, 150f); // Blood in south keep
            AddLiquidPlaneZLevel(ZoneLiquidType.Blood, "t50_m0003", 80.760172f, 17.803990f, 72.905113f, -13.311320f, 15.000010f, 150f); // Blood in south keep
            AddLiquidPlaneZLevel(ZoneLiquidType.Blood, "t50_m0003", 84.415932f, -13.300000f, 72.905113f, -16.12f, 15.000010f, 150f); // Blood in south keep
            AddLiquidPlaneZLevel(ZoneLiquidType.Blood, "t50_m0003", 83.292168f, -16.110729f, 72.905113f, -18.55f, 15.000010f, 150f); // Blood in south keep
            AddLiquidPlaneZLevel(ZoneLiquidType.Blood, "t50_m0003", 82.313110f, -18.549540f, 72.905113f, -21.76f, 15.000010f, 150f); // Blood in south keep
            AddLiquidPlaneZLevel(ZoneLiquidType.Blood, "t50_m0003", 81.036003f, -21.751711f, 72.905113f, -24.25f, 15.000010f, 150f); // Blood in south keep
        }
    }
}
