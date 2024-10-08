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
    internal class CastleMistmooreZoneProperties : ZoneProperties
    {
        public CastleMistmooreZoneProperties() : base()
        {
            // TODO: Lift in "The Tower"
            SetBaseZoneProperties("mistmoore", "Castle Mistmoore", 123f, -295f, -177f, 0, ZoneContinentType.Faydwer);

            AddZoneArea("Main Entryway", "mistmoore-03", "mistmoore-03", Configuration.CONFIG_AUDIO_MUSIC_DEFAULT_VOLUME, false);
            AddZoneAreaBox("Main Entryway", 52.558922f, 64.554611f, -168.251938f, -54.312000f, -70.595573f, -204.147720f);
            AddZoneAreaBox("Main Entryway", -26.757799f, 55.103039f, -166.467682f, -73.198334f, -57.687698f, -195.256317f);
            
            AddZoneArea("Castle Front", "mistmoore-01", "mistmoore-01", Configuration.CONFIG_AUDIO_MUSIC_DEFAULT_VOLUME, true, "", "night", 0f, 0.2786121f);
            AddZoneAreaBox("Castle Front", 324.605652f, 280.881042f, -95.237747f, 38.168800f, -204.299667f, -290f);
            
            AddZoneArea("Tomb");
            AddZoneAreaBox("Tomb", 9.834270f, 399.232208f, -222.168137f, -36.118179f, 356.207245f, -240.511887f);
            
            AddZoneArea("Graveyard", "", "", Configuration.CONFIG_AUDIO_MUSIC_DEFAULT_VOLUME, false, "", "night", 0f, 0.2786121f);
            AddZoneAreaBox("Graveyard", 161.105804f, 480.375092f, -140.617172f, -61.580879f, 328.872833f, -239.916077f);
            AddZoneAreaBox("Graveyard", 161.105804f, 480.375092f, -140.617172f, 20.498360f, 310.267181f, -239.916077f);
            AddZoneAreaBox("Graveyard", 161.105804f, 480.375092f, -140.617172f, -71.843201f, 336.520477f, -239.916077f);
            
            AddZoneArea("The Tower");
            AddZoneAreaBox("The Tower", -173.731277f, 401.491638f, -39.359699f, -290.964081f, 275.052490f, -177.201447f);
            
            AddZoneArea("Outer Pass", "", "", Configuration.CONFIG_AUDIO_MUSIC_DEFAULT_VOLUME, false, "", "night", 0f, 0.2786121f);
            AddZoneAreaBox("Outer Pass", 328.328461f, 661.384521f, -45.296520f, 7.925330f, 217.048187f, -280.445526f);
            AddZoneAreaBox("Outer Pass", 328.328461f, 661.384521f, -45.296520f, -469.359589f, 422.729919f, -280.445526f);
            AddZoneAreaBox("Outer Pass", -93.997177f, 613.809998f, -49.347141f, -469.359589f, 326.295868f, -280.445526f);
            AddZoneAreaBox("Outer Pass", -105.815468f, 613.809998f, -49.347141f, -469.359589f, 298.835388f, -280.445526f);
            AddZoneAreaBox("Outer Pass", -140.026962f, 613.809998f, -205.869247f, -469.359589f, 262.462738f, -280.445526f);
            AddZoneAreaBox("Outer Pass", -277.957703f, 613.809998f, -177.411514f, -469.359589f, 102.643059f, -280.445526f);
            AddZoneAreaBox("Outer Pass", -288.164612f, 613.809998f, -172.534561f, -469.359589f, 94.337547f, -280.445526f);
            
            AddZoneArea("Courtyard", "", "", Configuration.CONFIG_AUDIO_MUSIC_DEFAULT_VOLUME, false, "", "night", 0f, 0.2786121f);
            AddZoneAreaBox("Courtyard", 35.284790f, 205.371857f, -148.481659f, -199.885864f, 109.646828f, -198.810364f);
            AddZoneAreaBox("Courtyard", 35.284790f, 205.371857f, -148.481659f, -30.348749f, 61.634281f, -201.658279f);
            
            AddZoneArea("Crypt");
            AddZoneAreaBox("Crypt", -288.638336f, 80.434486f, -152.270264f, -365.138214f, 6.775110f, -204.384033f);
            
            AddZoneArea("Kitchen");
            AddZoneAreaBox("Kitchen", -224.878784f, -75.612762f, -137.195847f, -289.675049f, -141.804581f, -193);
            
            AddZoneArea("Throne Room");
            AddZoneAreaBox("Throne Room", -94.612473f, 48.470699f, -132.045151f, -168.778702f, -48.582161f, -164.867599f);
            
            AddZoneArea("Drawing Room");
            AddZoneAreaBox("Drawing Room", -95.448441f, 49.007591f, -171.129974f, -192.991455f, -48.696281f, -200.231613f);
            
            AddZoneArea("Library");
            AddZoneAreaBox("Library", -48.864670f, 101.219513f, -133.596344f, -99.474808f, 63.765011f, -165.961929f);
            
            AddZoneArea("Dining Hall");
            AddZoneAreaBox("Dining Hall", -95.846069f, -69.394127f, -167.270004f, -181.243073f, -156.531387f, -194.436142f);
            
            SetZonewideEnvironmentAsIndoors(30, 0, 60, ZoneFogType.Heavy);
            DisableSunlight();
            AddZoneLineBox("lfaydark", -1166.805908f, 3263.892578f, 0.000850f, ZoneLineOrientationType.East, -279.682556f, 141.644180f, -78.362358f, -339.412628f, 108.218033f, -182.437500f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", -207.922714f, 518.252563f, -409.548492f, 418.513092f, -237.916143f, 100f); // Entry pool, big
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 2.439530f, 95.290939f, -14.319680f, 84.488243f, -194.937485f, 5f); // Courtyard Pool Base 1
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 0.899840f, 96.857147f, -12.823690f, 83.049080f, -194.937485f, 5f); // Courtyard Pool Base 2
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", -0.571870f, 98.305656f, -11.297530f, 81.506592f, -194.937485f, 5f); // Courtyard Pool Base 3
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", -2.049700f, 90.969017f, -9.905360f, 89.007263f, -189.882553f, 10f); // Courtyard Pool Fountain N and S
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", -5.059470f, 93.924767f, -6.917720f, 86.123688f, -189.882553f, 10f); // Courtyard Pool Fountain W and E
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", -314.182220f, 77.662148f, -329.567352f, 14.306520f, -191.936478f, 10f); // Coffin room
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", -170.154343f, 95.783401f, -181.624146f, 84.045723f, -161.937485f, 5f); // small pool with steps
        }
    }
}
