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
    internal class QeynosAqueductsZoneProperties : ZoneProperties
    {
        public QeynosAqueductsZoneProperties() : base()
        {
            // TODO: Add more zone areas
            // TODO: Secret pot to Tox
            SetBaseZoneProperties("qcat", "Qeynos Aqueduct System", -315f, 214f, -38f, 0, ZoneContinentType.Antonica);
            SetZonewideEnvironmentAsIndoors(11, 36, 28, ZoneFogType.Medium);
            OverrideVertexColorIntensity(0.4);
            SetZonewideMusic("qcat-00", "qcat-00", true);

            AddZoneArea("Plague Bringer Tunnels", "qcat-01", "qcat-01");
            AddZoneAreaBox("Plague Bringer Tunnels", 489.128876f, -212.308807f, 196.282455f, 144.117630f, -698.131470f, -43.918282f);
            AddZoneAreaBox("Plague Bringer Tunnels", 517.676575f, 51.978771f, -18.997459f, 346.800323f, -256.628723f, -43.918282f);
            AddZoneAreaBox("Plague Bringer Tunnels", 478.528412f, 69.817322f, -26.608040f, 338.702545f, -2.257140f, -43.966591f);
            AddZoneAreaBox("Plague Bringer Tunnels", 409.858490f, 83.903488f, -21.663639f, 338.558014f, -66.726021f, -43.301041f);
            AddZoneAreaBox("Plague Bringer Tunnels", 347.182587f, 55.822350f, -22.146070f, 308.153931f, -62.604069f, -43.623081f);
            AddZoneAreaBox("Plague Bringer Tunnels", 398.436371f, -549.158813f, -41.968761f, 319.451172f, -620.778687f, -60.968689f);

            AddZoneArea("Crow's Way", "qcat-02", "qcat-02");
            AddZoneAreaBox("Crow's Way", 769.653625f, 82.470329f, -23.267820f, 610.306030f, 8.595240f, -48.048168f);
            AddZoneAreaBox("Crow's Way", 763.063782f, 85.606148f, -28.013861f, 610.306030f, 8.595240f, -48.048168f);
            AddZoneAreaBox("Crow's Way", 703.082947f, 101.457939f, -25.315540f, 610.306030f, 8.595240f, -48.048168f);
            AddZoneAreaBox("Crow's Way", 663.495361f, 160.923462f, -30.006010f, 610.306030f, 8.595240f, -48.048168f);

            AddZoneLineBox("qeynos2", 301.114655f, -161.613953f, -63.449379f, ZoneLineOrientationType.North, 1063.755981f, -41.776192f, 262.653015f, 1049.400269f, -56.161572f, 215f);
            AddZoneLineBox("qeynos2", 187.834518f, 341.638733f, -87.992813f, ZoneLineOrientationType.West, 895.849731f, 224.099167f, 61.134998f, 881.462830f, 209.713654f, 39.701309f);
            AddZoneLineBox("qeynos2", 175.706985f, 97.447029f, -41.968739f, ZoneLineOrientationType.West, 645.282104f, 140.744995f, -29.533369f, 629.556519f, 111.476059f, -42.468731f);
            AddZoneLineBox("qeynos", 273.965027f, -553.077881f, -107.459396f, ZoneLineOrientationType.West, 350.068390f, -167.744415f, 73.340729f, 335.681610f, -182.130219f, 42.217911f);
            AddZoneLineBox("qeynos", -147.118271f, -602.695679f, -27.999969f, ZoneLineOrientationType.West, 224.098785f, -251.712753f, -29.532080f, 209.713898f, -294.294037f, -42.468761f);
            AddZoneLineBox("qeynos", 174.606125f, -482.169281f, -81.858551f, ZoneLineOrientationType.North, 238.099686f, -55.775669f, 89.590897f, 223.712814f, -70.160507f, 55.560329f);
            AddZoneLineBox("qeynos", -188.236420f, 78.551971f, -91.583946f, ZoneLineOrientationType.West, -167.744095f, 314.996979f, -71.500740f, -182.130966f, 286.822327f, -84.468750f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_w1", 1066.685913f, -40.094090f, 1043.013306f, -62.600571f, 450f, 600f); // Water Column - North (and a little swim-under)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_w1", 1043.003306f, -40.094090f, 1005.895813f, -67.779869f, -42.968609f, 150f); // Small water section leading to the north water column
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_w1", 897.805542f, 224.793762f, 873.986084f, 202.529007f, 300f, 450f); // Water Column - North West (and a little swim-under)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_w1", 897.805542f, 202.539007f, 850.501831f, 164.009399f, -42.968609f, 150f); // Small water section leading to the north west water column
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_w1", 925.001343f, 211.993073f, 674.584167f, 5.217260f, -42.968609f, 21f); // Northwest large top area
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_w1", 841.040100f, 5.227260f, 656.927429f, -57.250038f, -42.968609f, 21f); // Northeast large top area
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_w1", 674.594167f, 211.993073f, 626.825195f, 120.706902f, -42.968609f, 21f); // Sliver connecting top northwest and southwest
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_w1", 626.835195f, 522.180908f, -227.728653f, -13.182240f, -42.968609f, 150f); // Southwest area, through dock exit
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_w1", 352.625366f, -165.435913f, 331.080566f, -182.326920f, 200.603394f, 350f); // Water Column - East
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_w1", 331.090566f, -77.152657f, 78.252281f, -218.270767f, -42.968609f, 150f); // Southeast area, leaving only a sliver in far se
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_w1", 117.543510f, -218.260767f, 77.043922f, -226.367447f, -42.968609f, 150f); // Tiny part in the SE
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_w1", 241.891846f, -53.101780f, 215.503647f, -77.142657f, 200.603394f, 350f); // Water Column, - South
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_w1", 241.891846f, -13.172240f, 215.503647f, -53.111780f, -42.968609f, 150f); // West connecting side of the south water column
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_w1", 215.513647f, -13.172240f, 78.252281f, -218.280767f, -42.968609f, 150f);// Center south water
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_w1", 686.540344f, 99.223663f, 642.067749f, 12.217740f, -84.968674f, 100f); // Lower water area with the shark sign
            AddLiquidPlaneZLevel(ZoneLiquidType.Blood, "d_b1", 379.903473f, -557.136230f, 334.749023f, -604.327087f, -42.968700f, 50f); // Blood pool
        }
    }
}
