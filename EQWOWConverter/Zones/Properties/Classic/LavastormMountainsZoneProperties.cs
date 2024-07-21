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
    internal class LavastormMountainsZoneProperties : ZoneProperties
    {
        public LavastormMountainsZoneProperties()
        {
            SetBaseZoneProperties("lavastorm", "Lavastorm Mountains", 153.45f, -1842.79f, -16.37f, 0, ZoneContinent.Antonica);
            SetFogProperties(255, 50, 10, 10, 800);
            AddZoneLineBox("soltemple", 255.801758f, 55.676472f, -0.999090f, ZoneLineOrientationType.North, 1381.412109f, 336.848877f, 156.155457f, 1356.645630f, 324.494568f, 145.188034f);
            AddZoneLineBox("soldunga", -449.347717f, -524.520508f, 69.968758f, ZoneLineOrientationType.South, 784.385925f, 231.909103f, 139.531494f, 763.337830f, 221.400375f, 126.562843f);
            AddZoneLineBox("soldungb", -419.581055f, -264.690491f, -111.967888f, ZoneLineOrientationType.South, 901.472107f, 489.983673f, 62.156502f, 880.400269f, 479.244751f, 51.187592f);
            AddZoneLineBox("najena", -16.450621f, 870.293030f, 0.000150f, ZoneLineOrientationType.East, -921.776184f, -1060.107300f, 61.094002f, -961.185852f, -1075.276733f, 12.125720f);
            AddZoneLineBox("nektulos", 3052.935791f, 312.635284f, -19.294090f, ZoneLineOrientationType.South, -2100.800537f, -115.948547f, 129.457657f, -2171.145996f, -253.399704f, -20.001289f);
            AddLiquidPlaneZLevel(LiquidType.Magma, "d_lava001", 1329.520508f, 1471.655151f, -1583.654907f, -1024.369141f, -11.905970f, 300f);
            AddDisabledMaterialCollisionByNames("d_lava001");
        }
    }
}
