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
    internal class SouthKaranaZoneProperties : ZoneProperties
    {
        public SouthKaranaZoneProperties() : base()
        {
            SetBaseZoneProperties("southkarana", "Southern Plains of Karana", 1293.66f, 2346.69f, -5.77f, 0, ZoneContinentType.Antonica);
            SetZonewideAmbienceSound("", "darkwds1");
            Enable2DSoundInstances("wind_lp3", "wind_lp4", "wind_lp2");

            AddZoneArea("The Well", "southkarana-00", "southkarana-00", false);
            AddZoneAreaBox("The Well", 115.009888f, 21.298410f, 42.651821f, -204.788803f, -257.898560f, -18.088860f);

            AddZoneArea("North Bridge", "southkarana-01", "southkarana-01");
            AddZoneAreaBox("North Bridge", 3521.624756f, 1509.357910f, 188.557877f, 1616.884277f, 198.383865f, -324.273438f);

            AddZoneArea("Undead Ruins", "southkarana-02", "southkarana-02");
            AddZoneAreaBox("Undead Ruins", 1540.513428f, 1434.618286f, 138.008347f, 1151.730103f, 953.870422f, -38.318489f);

            AddZoneArea("Lair of the Splitpaw", "southkarana-02", "southkarana-02");
            AddZoneAreaBox("Lair of the Splitpaw", -2994.746338f, 900.005066f, 102.187469f, -3513.122314f, 599.014160f, -49.473679f);

            AddZoneArea("Cursed Wizard Stones", "southkarana-03", "southkarana-03");
            AddZoneAreaBox("Cursed Wizard Stones", -1933.267700f, -1528.290039f, 95.360359f, -2329.310059f, -1929.807251f, -48.178211f);

            AddZoneArea("Centaur Town", "southkarana-01", "southkarana-01");
            AddZoneAreaBox("Centaur Town", 700.278931f, -1955.581787f, 156.323242f, -349.100586f, -2948.299805f, -69.660492f);

            AddZoneArea("Hermit House", "southkarana-04", "southkarana-04");
            AddZoneAreaBox("Hermit House", -5188.768066f, -2496.931885f, 92.119423f, -5747.661621f, -2827.709473f, -22.400141f);

            AddZoneArea("Aviak Town", "southkarana-05", "southkarana-05");
            AddZoneAreaBox("Aviak Town", -6111.867676f, 2361.861816f, 277.681763f, -7366.000488f, 242.259964f, -123.776070f);

            AddZoneArea("Ruined Stone Ring");
            AddZoneAreaBox("Ruined Stone Ring", -5925.080078f, -3168.792725f, 84.413254f, -6296.206543f, -3541.133057f, -33.351131f);

            AddZoneLineBox("lakerathe", 4352.154297f, 1158.142578f, -0.000990f, ZoneLineOrientationType.South, -8555.652344f, 1180.041138f, 43.965542f, -8569.452148f, 1132.577637f, -0.499510f);
            AddZoneLineBox("northkarana", -4472.277344f, 1208.014893f, -34.406212f, ZoneLineOrientationType.North, 2900.742432f, 943.823730f, 17.628691f, 2821.058350f, 862.661682f, -36.353588f);
            AddZoneLineBox("paw", -103.683167f, 16.824860f, 0.000050f, ZoneLineOrientationType.East, -3110.107910f, 895.748901f, 2.515520f, -3126.174805f, 861.375854f, -12.438860f);
            AddQuadrilateralLiquidShapeZLevel(ZoneLiquidType.Water, "d_w1", -18.260389f, -104.704063f, -26.714720f, -96.223312f, -35.142010f, -104.840118f, -26.822020f, -113.371986f,
                3.656250f, 250f); // Water near center of map
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", 3847.334961f, 4207.039062f, 1675.559814f, -4671.479004f, -69.374458f, 250f); // Big north water area
        }
    }
}
