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
    internal class SouthQeynosZoneProperties : ZoneProperties
    {
        public SouthQeynosZoneProperties() : base()
        {
            // TODO: Boat to Erudes Crossing
            SetBaseZoneProperties("qeynos", "South Qeynos", 186.46f, 14.29f, 3.75f, 0, ZoneContinentType.Antonica);
            AddZoneLineBox("qcat", 342.931549f, -174.301727f, 20.630989f, ZoneLineOrientationType.South, 280.068512f, -545.588013f, -130.403152f, 265.713104f, -559.974731f, -173.822586f);
            AddZoneLineBox("qcat", 215.878342f, -307.922394f, -41.968761f, ZoneLineOrientationType.East, -139.744003f, -621.613892f, -15.531000f, -156.270111f, -644.556519f, -28.499399f);
            AddZoneLineBox("qcat", 231.812836f, -63.370232f, 37.164181f, ZoneLineOrientationType.South, 182.130966f, -475.619812f, -112.237106f, 167.745056f, -490.005890f, -150.894775f);
            AddZoneLineBox("qcat", -175.662354f, 267.444336f, -77.394470f, ZoneLineOrientationType.East, -181.744064f, 46.723820f, -85.501877f, -196.098679f, 25.135241f, -98.468697f);
            AddZoneLineBox("qeynos2", -28.415890f, 357.134766f, -1.000000f, ZoneLineOrientationType.North, 602.557495f, -68.215889f, 18.468479f, 595.287170f, -84.163147f, -1.499980f);
            AddZoneLineBox("qeynos2", -153.796173f, -6.780330f, -1.000000f, ZoneLineOrientationType.North, 476.619080f, -430.347809f, 18.467279f, 468.250488f, -448.006866f, -1.499980f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", 601.443359f, 1175.225586f, -797.834167f, -113.296204f, -19.999929f, 150f); // Ocean and ocean tunnel
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0001", 431.629883f, -296.355560f, 393.577087f, -334.192322f, -1.999930f, 5f); // North pond near the clock
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0001", 228.634933f, -374.583069f, 140.408615f, -512.894714f, -1.999940f, 350f); // South well and tunnel (just SW of east tunnel)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0001", 372.959717f, -523.751526f, 264.531525f, -562.092468f, -4.000000f, 350f); // Northeast Well and tunnel
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0001", -156.018356f, -430.961548f, -170.753387f, -451.301483f, -1.999930f, 5f); // Indoor pond in the Rainbringer building in the SE
        }
    }
}
