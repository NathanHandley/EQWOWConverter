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
            AddValidMusicInstanceTrackIndexes(0, 1, 2, 3, 4, 5);
            AddZoneLineBox("lakerathe", 4352.154297f, 1158.142578f, -0.000990f, ZoneLineOrientationType.South, -8555.652344f, 1180.041138f, 43.965542f, -8569.452148f, 1132.577637f, -0.499510f);
            AddZoneLineBox("northkarana", -4472.277344f, 1208.014893f, -34.406212f, ZoneLineOrientationType.North, 2900.742432f, 943.823730f, 17.628691f, 2821.058350f, 862.661682f, -36.353588f);
            AddZoneLineBox("paw", -103.683167f, 16.824860f, 0.000050f, ZoneLineOrientationType.East, -3110.107910f, 895.748901f, 2.515520f, -3126.174805f, 861.375854f, -12.438860f);
            AddQuadrilateralLiquidShapeZLevel(ZoneLiquidType.Water, "d_w1", -18.260389f, -104.704063f, -26.714720f, -96.223312f, -35.142010f, -104.840118f, -26.822020f, -113.371986f,
                3.656250f, 250f); // Water near center of map
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", 3847.334961f, 4207.039062f, 1675.559814f, -4671.479004f, -69.374458f, 250f); // Big north water area
        }
    }
}
