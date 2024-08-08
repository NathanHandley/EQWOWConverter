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
    internal class FeerrottZoneProperties : ZoneProperties 
    {
        public FeerrottZoneProperties() : base()
        {
            // Stump causes a clash at 281.88 281.34 -3.04, the small jutted side
            SetBaseZoneProperties("feerrott", "The Feerrott", 902.6f, 1091.7f, 28f, 0, ZoneContinentType.Antonica);
            SetZonewideEnvironmentAsOutdoors(8, 25, 0, ZoneFogType.Heavy, false, 1, 0.7f, 0.5f, 0.1f);
            OverrideVertexColorIntensity(0.4);
            DisableSunlight();
            AddZoneLineBox("cazicthule", 55.471420f, -67.975937f, 0.000000f, ZoneLineOrientationType.North, -1469.255859f, -100.275429f, 58.405380f, -1499.662231f, -120.661491f, 47.437580f);
            AddZoneLineBox("oggok", -373.311127f, -102.846184f, -0.000000f, ZoneLineOrientationType.North, 1700.901245f, 832.210693f, 110.609047f, 1669.091797f, 786.900452f, 56.781330f);
            AddZoneLineBox("innothule", -1120.934570f, 1876.716309f, -12.343200f, ZoneLineOrientationType.East, -1053.738770f, -3064.860107f, 34.236019f, -1118.701904f, -3134.157959f, -12.843610f);
            AddZoneLineBox("rathemtn", 654.660095f, -3116.889893f, 0.000320f, ZoneLineOrientationType.North, 391.610870f, 3485.147949f, 64.094902f, 348.915161f, 3365.229736f, -0.499940f);
            AddZoneLineBox("fearplane", -817.476501f, 1032.365723f, 102.129517f, ZoneLineOrientationType.South, -2374.944580f, 2635.523193f, 98.296158f, -2399.710449f, 2569.650391f, 18.406269f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_sw1", 2104.413330f, 2316.766602f, 541.289795f, 976.981201f, -29.031210f, 300f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_sw1", 542f, 1206.450073f, 226.887222f, 633.791870f, -29.031210f, 300f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_sw1", 543.668091f, 1291.754883f, 440.032959f, 1204.328369f, -29.031210f, 300f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_sw1", 230.289062f, 982.076294f, -109.899048f, 285.150513f, -29.031210f, 300f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_sw1", -108.000000f, 938.000000f, -2141.682129f, 565.552185f, -29.031210f, 300f); // West Fork
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_sw1", 4.483450f, 299.117706f, -527.667542f, -29.172649f, -29.031210f, 300f); // East Fork
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_sw1", -97.385689f, 488.924164f, -294.640381f, 284.096161f, -29.031210f, 300f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_sw1", -250.212585f, -27.550091f, -1067.875122f, -492.329712f, -29.031210f, 300f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_sw1", -663.865784f, -488.732147f, -1322.731934f, -681.698486f, -29.031210f, 300f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_sw1", -906.003662f, -678.693909f, -2143.735596f, -965.550171f, -29.031210f, 300f);
        }
    }
}
