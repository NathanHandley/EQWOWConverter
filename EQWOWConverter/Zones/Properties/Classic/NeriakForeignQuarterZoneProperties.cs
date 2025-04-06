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
    internal class NeriakForeignQuarterZoneProperties : ZoneProperties
    {
        public NeriakForeignQuarterZoneProperties() : base()
        {
            // TODO: Add more zone areas
            SetZonewideEnvironmentAsIndoors(35, 22, 59, ZoneFogType.Heavy);

            AddZoneArea("Guard House", "neriaka-02", "neriaka-02");
            AddZoneAreaBox("Guard House", 13.058960f, -84.772346f, 28.154770f, -12.747060f, -128.748932f, -2.394260f);
            AddZoneAreaBox("Guard House", 38.669739f, -98.807129f, 15.071800f, -39.769070f, -127.396881f, -4.807590f);

            AddZoneArea("Front Gate", "neriaka-00", "neriaka-00");
            AddZoneAreaBox("Front Gate", 217.287186f, 249.349365f, 90.845581f, -319.956879f, -28.907511f, -6.688130f);
            AddZoneAreaBox("Front Gate", 53.041210f, 0.962250f, 84.192307f, -55.847149f, -139.796371f, -29.330940f);

            AddZoneArea("Commons Path", "neriaka-01", "neriaka-01");
            AddZoneAreaBox("Commons Path", 39.398472f, -154.950089f, 35.158119f, -49.895748f, -497.731720f, -54.652771f);

            AddZoneLineBox("nektulos", 2294.104980f, -1105.768066f, 0.000190f, ZoneLineOrientationType.North, 27.909149f, 168.129883f, 40.197109f, -14.193390f, 134.459396f, 27.500010f);
            AddZoneLineBox("neriakb", 83.471588f, -404.715454f, -14.000000f, ZoneLineOrientationType.East, 98.162216f, -339.024811f, 12.469000f, 64.296402f, -406.345734f, -14.499970f);
            AddZoneLineBox("neriakb", -237.565536f, -455.336853f, 14.000020f, ZoneLineOrientationType.North, -223.713684f, -447.620209f, 26.468010f, -238.099426f, -489.933807f, 13.500010f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_m0000", -305.656586f, -143.895905f, -323.945435f, -145.418015f, 3.000080f, 10f); // West pool
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_m0000", -305.656586f, -143.895905f, -322.545435f, -146.818015f, 3.000080f, 10f); // West pool
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_m0000", -305.656586f, -143.895905f, -321.145435f, -148.218015f, 3.000080f, 10f); // West pool
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_m0000", -305.656586f, -143.895905f, -319.745435f, -149.618015f, 3.000080f, 10f); // West pool
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_m0000", -305.656586f, -143.895905f, -318.345435f, -151.018015f, 3.000080f, 10f); // West pool
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_m0000", -305.656586f, -143.895905f, -316.945435f, -152.418015f, 3.000080f, 10f); // West pool
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_m0000", -305.656586f, -143.895905f, -315.545435f, -153.818015f, 3.000080f, 10f); // West pool
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_m0000", -305.656586f, -143.895905f, -314.145435f, -155.218015f, 3.000080f, 10f); // West pool
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_m0000", -305.656586f, -143.895905f, -312.745435f, -156.618015f, 3.000080f, 10f); // West pool
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_m0000", -305.656586f, -143.895905f, -311.345435f, -158.018015f, 3.000080f, 10f); // West pool
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_m0000", -305.656586f, -143.895905f, -309.945435f, -159.418015f, 3.000080f, 10f); // West pool
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_m0000", -305.656586f, -143.895905f, -308.545435f, -160.818015f, 3.000080f, 10f); // West pool
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_m0000", -305.656586f, -143.895905f, -307.145435f, -162.218015f, 3.000080f, 10f); // West pool
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_m0000", -305.656586f, -143.895905f, -305.745435f, -163.618015f, 3.000080f, 10f); // West pool
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t25_m0001", 108.254227f, -139.586319f, 89.659752f, -142.491455f, 2.000000f, 10f); // Center fountain
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t25_m0001", 107.154227f, -138.486319f, 90.759752f, -143.591455f, 2.000000f, 10f); // Center fountain
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t25_m0001", 106.054227f, -137.386319f, 91.859752f, -144.691455f, 2.000000f, 10f); // Center fountain
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t25_m0001", 104.954227f, -136.286319f, 92.959752f, -145.791455f, 2.000000f, 10f); // Center fountain
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t25_m0001", 103.854227f, -135.186319f, 94.059752f, -146.891455f, 2.000000f, 10f); // Center fountain
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t25_m0001", 102.754227f, -134.086319f, 95.159752f, -147.991455f, 2.000000f, 10f); // Center fountain
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t25_m0001", 101.654227f, -132.986319f, 96.259752f, -149.091455f, 2.000000f, 10f); // Center fountain
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t25_m0001", 100.554227f, -131.886319f, 97.359752f, -150.191455f, 2.000000f, 10f); // Center fountain
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t25_m0001", 99.454227f, -130.786319f, 98.459752f, -151.291455f, 2.000000f, 10f); // Center fountain
        }
    }
}
