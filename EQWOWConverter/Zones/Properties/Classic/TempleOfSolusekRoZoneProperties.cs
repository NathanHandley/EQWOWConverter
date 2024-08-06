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
    internal class TempleOfSolusekRoZoneProperties : ZoneProperties
    {
        public TempleOfSolusekRoZoneProperties() : base()
        {
            SetBaseZoneProperties("soltemple", "Temple of Solusek Ro", 7.5f, 268.8f, 3f, 0, ZoneContinentType.Antonica);
            SetZonewideEnvironmentAsIndoors(180, 5, 5, ZoneFogType.Heavy);
            OverrideVertexColorIntensity(0.3);
            AddZoneLineBox("lavastorm", 1346.515381f, 330.955505f, 146.188034f, ZoneLineOrientationType.South, 244.129364f, 62.161572f, 9.468010f, 219.713821f, 44.408550f, -1.500000f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_m0002", 499.080811f, 32.964199f, 472.570038f, -48.240250f, 5.000010f, 12.6f);   // Top area, lowest lava
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_m0002", 519.093750f, -58.637852f, 474.333313f, -114.066833f, 12.000010f, 15f);   // Top area, east upper level (east side)
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_m0002", 516.087341f, -34.205761f, 498.917908f, -58.666969f, 12.000010f, 15f);   // Top area, east upper level (west side)
            AddQuadrilateralLiquidShapeZLevel(ZoneLiquidType.Magma, "d_lava001", 505.731720f, -42.152409f, 499.536438f, -38.081902f, 484.816589f, -50.759651f,
                495.153717f, -62.718128f, 12.000010f, 15f, 2000f, 2000f, -2000f, -2000f, 0.4f); // Top area, east fall
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_m0002", 501.026611f, 57.315898f, 480.985962f, 51.630980f, 8.000030f, 14.5f); // Top area, west 1st level up near NW
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_m0002", 491.430985f, 59.392169f, 480.985962f, 57.105898f, 8.000030f, 14.5f); // Top area, west 1st level up near NW
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_m0002", 488.882629f, 60.999130f, 484.416931f, 58.957699f, 8.000030f, 14.5f); // Top area, west 1st level up near NW
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_m0002", 487.431763f, 62.665852f, 484.416931f, 60.989130f, 8.000030f, 14.5f); // Top area, west 1st level up near NW
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_m0002", 501.026611f, 51.640980f, 479.441040f, 33.969700f, 8.000030f, 14.5f); // Top area, west 1st level up near NW
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_m0002", 482.425110f, 37.876530f, 478.034607f, 33.969700f, 8.000030f, 14.5f); // Top area, west 1st level up near NW
            AddQuadrilateralLiquidShapeZLevel(ZoneLiquidType.Magma, "d_m0002", 484.653290f, 37.431648f, 479.832001f, 45.032742f, 475.742035f, 34.063629f,
                475.808105f, 32.345268f, 8.000030f, 14.5f, 2000f, 2000f, -2000f, -2000f, 0.4f); // Top area, west 1st level up
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_m0002", 557.422913f, 127.655708f, 485.443909f, 63.568630f, 12.000010f, 50f); // Top area, west 2st level up (west)
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_m0002", 514.152710f, 66.235321f, 498.986969f, 52.200989f, 12.000010f, 50f); // Top area, west 2st level up (west)
            AddQuadrilateralLiquidShapeZLevel(ZoneLiquidType.Magma, "d_m0002", 503.979767f, 59.812340f, 494.726990f, 70.472641f, 485.481201f, 63.863422f,
                499.118988f, 52.208672f, 12.000010f, 50f, 2000f, 2000f, -2000f, -2000f, 0.5f); // Top area, west 2nd level lava fall
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_m0002", 509.190308f, 33.766731f, 452.649292f, -25.908581f, -52.968712f, 50f); // Bottom area pool
            AddOctagonLiquidShape(ZoneLiquidType.Magma, "d_m0002", 489.820374f, 476.808990f, 13.012480f, -0.013940f, 9.020610f, 3.978410f,
                9.020610f, 3.978410f, 485.784668f, 480.833710f, 485.784668f, 480.833710f, 5.000010f, 90f);
        }
    }
}
