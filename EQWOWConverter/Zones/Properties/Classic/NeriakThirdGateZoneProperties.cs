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
    internal class NeriakThirdGateZoneProperties : ZoneProperties
    {
        public NeriakThirdGateZoneProperties() : base()
        {
            SetBaseZoneProperties("neriakc", "Neriak Third Gate", -968.96f, 891.92f, -52.22f, 0, ZoneContinentType.Antonica);
            //AddValidMusicInstanceTrackIndexes(12, 13, 14, 16, 17);
            SetZonewideEnvironmentAsIndoors(35, 22, 59, ZoneFogType.Heavy);
            AddZoneLineBox("neriakb", 196.809433f, -853.183411f, -41.968700f, ZoneLineOrientationType.South, 203.418655f, -846.463745f, -31.531000f, 181.745132f, -860.847778f, -42.468739f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 564.595459f, -695.657227f, 331.376404f, -930.340698f, -69.968529f, 30f); // South area, water moat (bottom)
            AddQuadrilateralLiquidShapeZLevel(ZoneLiquidType.Water, "t50_w1", 515.853943f, -757.694885f, 501.844330f, -743.636108f,
                489.708313f, -755.678589f, 503.854492f, -769.786743f, -27.999990f, 50f, 1000f, 1000f, -1000f, -1000f, 0.5f); // South area, lowest spillover into moat (northwest)
            AddQuadrilateralLiquidShapeZLevel(ZoneLiquidType.Water, "t50_w1", 516.064209f, -865.767456f, 503.501434f, -853.133545f,
                489.493195f, -867.663330f, 501.900360f, -879.862610f, -27.999990f, 50f, 1000f, 1000f, -1000f, -1000f, 0.5f); // South area, lowest spillover into moat (northeast)                        
            AddQuadrilateralLiquidShapeZLevel(ZoneLiquidType.Water, "t50_w1", 405.875366f, -867.682312f, 391.754523f, -853.294373f,
                379.614746f, -865.668030f, 393.802002f, -879.829529f, -27.999990f, 50f, 1000f, 1000f, -1000f, -1000f, 0.5f); // South area, lowest spillover into moat (southeast)
            AddQuadrilateralLiquidShapeZLevel(ZoneLiquidType.Water, "t50_w1", 406.509827f, -755.673096f, 393.734772f, -743.447388f,
                379.510651f, -757.599976f, 391.854950f, -769.755066f, -27.999990f, 50f, 1000f, 1000f, -1000f, -1000f, 0.5f); // South area, lowest spillover into moat (southwest)
            AddOctagonLiquidShape(ZoneLiquidType.Water, "t50_w1", 503.118591f, 462.368011f, -756.297546f, -797.092651f, -769.704102f, -783.686096f,
                -769.717651f, -784.686462f, 489.802521f, 475.831116f, 489.802521f, 474.831116f, 3.000050f, 18f, 1f); // South area, second level pool (northwest)
            AddOctagonLiquidShape(ZoneLiquidType.Water, "t50_w1", 503.118591f, 462.368011f, -825.981628f, -867.293274f, -839.658630f, -853.651550f,
                -838.658630f, -853.651550f, 489.783752f, 474.854004f, 489.783752f, 475.854004f, 3.000050f, 18f, 1f); // South area, second level pool (northeast)
            AddOctagonLiquidShape(ZoneLiquidType.Water, "t50_w1", 433.082654f, 392.420123f, -756.679382f, -797.288135f, -769.794458f, -784.708374f,
                -769.894458f, -783.708374f, 419.758734f, 405.645306f, 420.858734f, 405.845306f, 3.000050f, 18f, 1f); // South area, second level pool (southwest)
            AddOctagonLiquidShape(ZoneLiquidType.Water, "t50_w1", 433.082654f, 392.420123f, -826.226501f, -867.087097f, -837.657227f, -853.264429f,
                -839.657227f, -853.664429f, 421.758734f, 405.645306f, 419.658734f, 405.845306f, 3.000050f, 18f, 1f); // South area, second level pool (southeast)
            AddOctagonLiquidShape(ZoneLiquidType.Water, "t50_w1", 475.315918f, 420.430237f, -784.233215f, -839.071289f, -797.651123f, -825.433545f,
                -797.651123f, -825.633545f, 461.791595f, 433.775452f, 461.591595f, 433.775452f, 17.000080f, 4.5f, 1f); // South area, top pool
            AddOctagonLiquidShape(ZoneLiquidType.Water, "t50_falls1", 463.792389f, 431.817810f, -795.708984f, -827.656067f, -807.690613f, -815.690063f,
                -807.690613f, -815.690063f, 451.802094f, 443.800415f, 451.802094f, 443.800415f, 60f, 47.5f, 0.31f); // South area, top waterfall
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 957.712585f, -801.579041f, 768.454346f, -973.101624f, -69.968651f, 50f); // South second area with eye
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 1292.375366f, -1573.922241f, 1241.988159f, -1711.024536f, -104.968109f, 50f); // Northwest area, water under bridge
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 1107.336060f, -1336.077759f, 932.822266f, -1799.186035f, -97.968651f, 50f); // East area, water around the building with bridges and docks (North)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 932.422266f, -1546.286987f, 696.481934f, -1799.186035f, -97.968651f, 50f); // East area, water around the building with bridges and docks (South)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 824.538452f, -1456.688843f, 798.816895f, -1468.245117f, -81.968712f, 10f); // Central area, dragon statue fountain
            AddLiquidPlaneZLevel(ZoneLiquidType.Blood, "t75_blood1", 1301.838745f, -1249.076172f, 1289.048096f, -1251.979248f, -39.968670f, 3f); // Blood at top of the red temple
        }
    }
}
