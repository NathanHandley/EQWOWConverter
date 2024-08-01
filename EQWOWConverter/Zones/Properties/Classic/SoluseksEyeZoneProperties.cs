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
    internal class SoluseksEyeZoneProperties : ZoneProperties
    {
        public SoluseksEyeZoneProperties() : base()
        {
            // TODO: Drawbridges at -156.465775f, -289.006134f, 15.660960f
            SetBaseZoneProperties("soldunga", "Solusek's Eye", -485.77f, -476.04f, 73.72f, 0, ZoneContinentType.Antonica);
            IsExteriorByDefault = false;
            SetFogProperties(180, 30, 30, 10, 100);
            AddZoneLineBox("lavastorm", 792.794373f, 226.540787f, 127.062599f, ZoneLineOrientationType.North, -429.005951f, -518.254517f, 82.437752f, -440.369812f, -529.974792f, 69.468758f); // Works
            AddZoneLineBox("soldungb", -165.640060f, -595.953247f, 14.000010f, ZoneLineOrientationType.East, -158.745377f, -582.988464f, 25.937571f, -173.130524f, -600.847412f, 13.500000f);
            AddZoneLineBox("soldungb", -275.436981f, -507.896454f, 22.000071f, ZoneLineOrientationType.North, -267.713684f, -499.620789f, 32.469002f, -286.491913f, -514.974121f, 21.500681f);
            AddZoneLineBox("soldungb", -364.385254f, -406.963348f, 8.000610f, ZoneLineOrientationType.East, -357.650269f, -400.866211f, 18.469000f, -377.148956f, -409.100372f, 6.500000f);
            AddZoneLineBox("soldungb", -1081.872803f, -525.067810f, -3.999330f, ZoneLineOrientationType.West, -1068.534180f, -521.974548f, 8.468000f, -1087.309937f, -541.027771f, -4.500000f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -959.703491f, -446.716766f, -1042.347900f, -549.781860f, -17.999990f, 150f); // South Lava - West Segment (lower)
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -982.687439f, -546.702393f, -1021.077576f, -573.826233f, 10.000040f, 150f); // South Lava - EastSouth Segment (southern upper)
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -881.593018f, -522.548462f, -1002.407898f, -596.958862f, 10.000040f, 150f); // South Lava - Northern Segment (upper)
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -584.446777f, -393.949585f, -714.652649f, -488.567871f, -15.999960f, 150f); // Main Lava - Most South west section (low)
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -463.831726f, -426.711334f, -583.792542f, -618.261658f, 29.000031f, 150f); // Main Lava - South west just above the very southwest part
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -462.499054f, -617.938416f, -629.354370f, -1045.443115f, 38.000019f, 150f); // Main Lava - South east area
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -444.441345f, -930.922058f, -462.522614f, -1025.180786f, 38.000019f, 150f); // Main Lava - South east area moving up towards east
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -325.039154f, -1004.851257f, -444.451345f, -1029.814087f, 38.000019f, 150f); // Main Lava - South east area moving up towards east (2nd from south)
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -356.387146f, -980.918030f, -444.451345f, -1023.871582f, 38.000019f, 150f); // Main Lava - Starting east area from south
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -273.261810f, -993.091187f, -310.648773f, -1033.765259f, 38.000019f, 150f); // Main Lava, connection to east area
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -310.648773f, -1007.396484f, -320.207825f, -1030.357422f, 38.000019f, 150f); // Main Lava, south east area moving up towards east (2nd north/east)
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -318.475647f, -1005.488586f, -327.968658f, -1031.960938f, 38.000019f, 150f); // Main Lava, south east area moving up towards east (3rd north/east)
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -321.059143f, -1004.023926f, -329.228455f, -1009.762573f, 38.000019f, 150f); // Main Lava, south east area moving up towards east (little corner nub)
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -349.973083f, -998.862610f, -364.150787f, -1013.383667f, 38.000019f, 150f); // Main Lava, south east area moving up towards east (little corner nub (more south))
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -163.275757f, -1032.966431f, -189.657944f, -1065.715454f, 44.968781f, 150f); // Northeast offshoot (probably can't actually get up here)
            AddOctagonLiquidShape(ZoneLiquidType.Magma, "d_lava001", -119.976532f, -132.940643f, -941.631226f, -954.618103f, -945.635254f, -950.622864f,
                -945.635254f, -950.622864f, -123.916550f, -128.932663f, -123.916550f, -128.932663f, 70.643982f, 300f); // North Lava Pillar                     
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -112.599098f, -934.069336f, -218.988007f, -1020.933228f, 45.968830f, 150f); // North connecting tunnels to the north lava pillar
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -189.409256f, -996.889221f, -273.904785f, -1053.176514f, 38.000019f, 150f); // Main Lava - Northeast area
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -218.673187f, -882.159790f, -278.829376f, -997.477417f, 38.000019f, 150f); // Main Lava - North area
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -333.911011f, -519.980408f, -400.758453f, -611.953796f, -13.999950f, 150f); // Main Lava - Northwest lower
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -276.853088f, -684.851990f, -412.576324f, -943.044861f, 20.000010f, 150f); // Main Lava - East area
            AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_lava001", -348.883087f, -610.981750f, -382.591187f, -943.044861f, 10.000240f, 150f); // Main Lava - West area
        }
    }
}
