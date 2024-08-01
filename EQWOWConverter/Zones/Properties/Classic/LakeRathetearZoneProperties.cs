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
    internal class LakeRathetearZoneProperties : ZoneProperties
    {
        public LakeRathetearZoneProperties() : base()
        {
            SetBaseZoneProperties("lakerathe", "Lake Rathetear", 1213f, 4183f, 4f, 0, ZoneContinentType.Antonica);
            SetFogProperties(200, 200, 220, 10, 800);
            AddZoneLineBox("arena", -56.940857f, -835.9014f, 7.882746f, ZoneLineOrientationType.West, 2360.1794f, 2708.7017f, 130.344f, 2329.8247f, 2699.243f, 92.11265f);
            AddZoneLineBox("southkarana", -8541.681641f, 1158.678223f, 0.000370f, ZoneLineOrientationType.North, 4392.966797f, 1200f, 38.467892f, 4366.503906f, 1132.421143f, -0.500990f);
            AddZoneLineBox("rathemtn", 3533.836426f, 2945.927734f, -3.874240f, ZoneLineOrientationType.North, 2647.961426f, -2217.051025f, 62.953671f, 2538.218994f, -2290.238770f, 1.250070f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", 3984.728027f, 2968.631104f, -1043.543091f, -1923.546997f, -44.155720f, 400f);
        }
    }
}
