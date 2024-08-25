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
    internal class NagafensLairZoneProperties : ZoneProperties
    {
        public NagafensLairZoneProperties() : base()
        {
            SetBaseZoneProperties("soldungb", "Nagafen's Lair", -262.7f, -423.99f, -108.22f, 0, ZoneContinentType.Antonica);
            //AddValidMusicInstanceTrackIndexes(0, 1, 2, 4, 5, 7);
            SetZonewideEnvironmentAsIndoors(180, 30, 30, ZoneFogType.Heavy);
            OverrideVertexColorIntensity(0.3);
            DisableSunlight();
            AddZoneLineBox("lavastorm", 909.788574f, 484.493713f, 51.688461f, ZoneLineOrientationType.North, -399.037048f, -259.033051f, -101.499748f, -410.349213f, -270.100739f, -112.467888f);
            AddZoneLineBox("soldunga", -166.265717f, -572.437744f, 15.365680f, ZoneLineOrientationType.West, -158.744904f, -540.487061f, 35.468010f, -173.130875f, -581.914795f, 13.500010f);
            AddZoneLineBox("soldunga", -299.452057f, -508.939087f, 22.000681f, ZoneLineOrientationType.South, -285.998016f, -499.999878f, 32.469002f, -300.556580f, -514.974731f, 21.500320f);
            AddZoneLineBox("soldunga", -364.548492f, -391.234467f, 8.000000f, ZoneLineOrientationType.West, -350.692047f, -379.634491f, 19.469000f, -371.417236f, -399.513031f, 7.500610f);
            AddZoneLineBox("soldunga", -1082.416138f, -549.995605f, -4.000000f, ZoneLineOrientationType.East, -1076.369019f, -535.692444f, 15.149850f, -1089.673218f, -559.933105f, -4.499780f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_lava001", -139.902847f, -376.441772f, -173.771774f, -411.930511f, -16.999981f, 150f); // Oil Pool
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -1166.785400f, -667.862488f, -1412.326294f, -986.085388f, 73.968903f, 150f); // Nagafen's Pool
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -1143.028687f, -604.321421f, -1195.789917f, -667.872488f, 73.968903f, 150f); // Path towards Nagafen's Pool (nearest)
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -1132.814819f, -567.222534f, -1174.485229f, -604.341421f, 73.968903f, 150f); // Path towards Nagafen's Pool
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -1113.380005f, -523.927429f, -1160.635132f, -567.232534f, 73.968903f, 150f); // Path towards Nagafen's Pool
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -1114.478638f, -440.522186f, -1160.635132f, -523.937429f, 73.968903f, 11.25f); // Path towards Nagafen's Pool (top of waterfall)
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -1155.879150f, -353.806427f, -1363.432739f, -569.984619f, -15.000010f, 150f); // Area below lava flow under Nagafen
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -998.797852f, -395.365641f, -1056.736938f, -469.923004f, -49.968731f, 150f); // Lava steps west of Nagafen, 2nd step plane (east)
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -1012.745972f, -367.435913f, -1056.736938f, -395.385641f, -49.968731f, 150f); // Lava steps west of Nagafen, 2nd step plane (west)
            AddQuadrilateralLiquidShapeZLevel(ZoneLiquidType.Magma, "d_lava001", -1020.252136f, -369.291748f, -1027.900391f, -354.708851f, -1052.254028f, -368.784038f,
                -1049.278198f, -377.636841f, -49.968731f, 150f, 2000f, 2000f, -2000f, -2000f, 0.5f); // Lava steps west of Nagafen, 2nd step (top of lavafall)
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -957.797424f, -463.603119f, -1030.056519f, -546.555298f, -17.999990f, 150f); // Lava steps west of Nagafen, top step plane east
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -988.444214f, -451.760712f, -1003.117676f, -470.173279f, -17.999990f, 150f); // Lava steps west of Nagafen, top step plane west
            AddQuadrilateralLiquidShapeZLevel(ZoneLiquidType.Magma, "d_lava001", -990.568665f, -454.285675f, -1003.253906f, -451.589722f, -1019.192810f, -463.566467f,
                -1008.867859f, -464.364014f, -17.999990f, 150f, 2000f, 2000f, -2000f, -2000f, 0.5f); // Lava steps west of Nagafen, top step waterfall top
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -1013.488708f, -239.818726f, -1086.548828f, -372.778351f, -71.968651f, 150f); // Lava steps west of Nagafen, bottom eastmost
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -975.216736f, -137.976318f, -1013.580933f, -264.936951f, -71.968651f, 150f); // Lava steps west of Nagafen, bottom middle
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -953.617432f, -74.198563f, -974.440491f, -185.962540f, -71.968651f, 150f); // Lava steps west of Nagafen, bottom southeast
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -973.749146f, -109.691994f, -997.505066f, -193.307343f, -71.968651f, 150f); // Lava steps west of Nagafen, south coming up to other intersection
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -975.342407f, -96.718460f, -990.840393f, -110.728691f, -71.968651f, 150f); // Lava steps west of Nagafen, near intersection
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -967.319946f, -95.079277f, -979.230469f, -116.164574f, -71.968651f, 150f); // Lava steps west of Nagafen, near intersection bottom of lavafall
            AddQuadrilateralLiquidShapeZLevel(ZoneLiquidType.Magma, "d_lava001", -969.702698f, -82.734779f, -974.341309f, -74.098640f, -987.817749f, -96.341690f,
                -979.366150f, -100.475906f, -71.968651f, 150f, 2000f, 2000f, -2000f, -2000f, 0.5f); // Lava steps west of Nagafen, near intersection at bottom of lava fall
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -1077.559692f, -135.729828f, -1389.664673f, -382.311005f, -67.968674f, 150f); // Lava path south of Nagafen, eastmost
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -1010.905090f, -49.302589f, -1080.569692f, -185.783417f, -67.968674f, 150f); // Lava path south of Nagafen, center block
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -994.834534f, -49.302589f, -1010.915090f, -117.912903f, -67.968674f, 150f); // Lava path south of Nagafen, near top lava fall
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -988.720825f, -49.302589f, -994.844534f, -102.415977f, -67.968674f, 150f); // Lava path south of Nagafen, near top lava fall
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -972.454163f, -49.302589f, -988.730825f, -72.405312f, -67.968674f, 150f); // Lava path south of Nagafen, near top lava fall
            AddQuadrilateralLiquidShapeZLevel(ZoneLiquidType.Magma, "d_lava001", -972.271851f, -72.494850f, -990.341980f, -66.949249f,
                -994.710083f, -91.504433f, -989.396301f, -97.818367f, -67.968674f, 150f, 2000f, 2000f, -2000f, -2000f, 0.5f); // Lava path south of Nagafen, lava fall into next step down
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -834.602905f, 8.966730f, -953.106262f, -111.683937f, -88.968590f, 150f); // Lava west middle area, full between two lava falls
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -447.087067f, 287.395966f, -840.904663f, -140.857407f, -108.968658f, 150f); // Lava west and northwest
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -477.091125f, -138.475662f, -534.154968f, -274.647156f, -115.968674f, 150f); // Lava north connecting west and east
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -507.929535f, -273.177063f, -650.492126f, -433.339966f, -97.968658f, 150f); // Lava north east lower with the large pool
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -651.626282f, -368.762909f, -722.261108f, -477.929626f, -15.999950f, 150f); // Lava east upper middle, majority segment
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -637.853333f, -428.289948f, -722.261108f, -477.929626f, -15.999950f, 150f); // Lava east upper middle, minority segment
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -331.496796f, -549.121582f, -399.097748f, -619.549866f, -13.999960f, 150f); // Lava north east upper 
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -370.305115f, -522.029358f, -388.071930f, -549.131582f, -13.999960f, 150f); // Lava north east upper pt 2
            AddQuadrilateralLiquidShapeZLevel(ZoneLiquidType.Magma, "d_lava001", -378.460907f, -528.554138f, -386.896851f, -523.995361f, -396.291107f, -545.855591f,
                -384.069855f, -557.535339f, -13.999960f, 150f, 2000f, 2000f, -2000f, -2000f, 0.5f); // Lava north east top of falls
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -386.336121f, -404.101440f, -462.968964f, -540.660461f, -32.999962f, 17.45f); // Lava north east mid
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -463.904785f, -366.310272f, -539.539490f, -448.649292f, -51.968731f, 19.5f); // Lava north east bottom
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -537.387085f, -379.438049f, -545.856812f, -393.651611f, -51.968731f, 19.5f); // Lava north east bottom near falls
            AddQuadrilateralLiquidShapeZLevel(ZoneLiquidType.Magma, "d_lava001", -535.108704f, -378.957306f, -546.292969f, -367.877228f, -551.902222f, -374.825226f,
                -545.501892f, -397.644989f, -51.968731f, 19.5f, 2000f, 2000f, -2000f, -2000f, 0.4f); // Lave north top of falls
            AddAlwaysBrightMaterial("d_lava001");
            AddAlwaysBrightMaterial("d_lavafall1");
            AddAlwaysBrightMaterial("d_m0002");
            AddAlwaysBrightMaterial("d_m0008");
            AddAlwaysBrightMaterial("d_underlava1");
        }
    }
}
