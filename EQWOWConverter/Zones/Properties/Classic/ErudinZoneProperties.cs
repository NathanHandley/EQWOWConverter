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
    internal class ErudinZoneProperties : ZoneProperties
    {
        public ErudinZoneProperties() : base()
        {
            SetBaseZoneProperties("erudnext", "Erudin", -309.75f, 109.64f, 23.75f, 0, ZoneContinentType.Odus);
            SetFogProperties(200, 200, 220, 10, 550);
            AddZoneLineBox("tox", 2543.662842f, 297.415588f, -48.407711f, ZoneLineOrientationType.South, -1559.726196f, -175.747467f, -17.531000f, -1584.182617f, -211.414566f, -48.468529f);
            AddTeleportPad("erudnext", -1410.466431f, -184.863327f, 34.000938f, ZoneLineOrientationType.North, -1392.625977f, -254.981995f, -42.968651f, 6.0f);
            AddTeleportPad("erudnext", -1410.336670f, -310.649994f, -45.968410f, ZoneLineOrientationType.South, -1410.323730f, -310.856049f, 37.000172f, 6.0f);
            AddTeleportPad("erudnint", 711.753357f, 805.382568f, 18.000059f, ZoneLineOrientationType.East, -644.927917f, -183.935837f, 75.968788f, 6.0f);
            AddTeleportPad("erudnext", -396.419495f, -8.040450f, 38.000011f, ZoneLineOrientationType.North, -644.701294f, -278.909973f, 68.968857f, 6.0f);
            AddTeleportPad("erudnext", -773.795898f, -183.949753f, 50.968781f, ZoneLineOrientationType.North, -327.673065f, -1.365350f, 37.998589f, 11f);
            AddTeleportPad("erudnext", -773.795898f, -183.949753f, 50.968781f, ZoneLineOrientationType.North, -327.673065f, -8.182305f, 37.998589f, 11f);
            AddTeleportPad("erudnext", -773.795898f, -183.949753f, 50.968781f, ZoneLineOrientationType.North, -327.821869f, -14.999260f, 37.998589f, 11f);
            AddLiquidPlaneZLevel(LiquidType.Water, "d_ow1", 1400f, 64.162201f, -1207.356567f, -2698.789551f, -0.000090f, 500f); // Big Water
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", -679.434692f, -177.832794f, -695.453979f, -189.770065f, 55.968849f, 10f); // Main fountain, mid
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", -719.733948f, -172.185776f, -751.873840f, -195.509460f, 48.968899f, 10f); // Main fountain, lower pt 1
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", -724.286133f, -167.803345f, -747.216858f, -199.989166f, 48.968899f, 10f); // Main fountain, lower pt 2
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", -721.906250f, -170.235764f, -749.472107f, -197.832443f, 48.968899f, 10f); // Main fountain, lower pt 3
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", -283.326660f, 60.087742f, -364.447083f, -76.676491f, 29.000031f, 10f); // Inside towards dock, big pool
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", -106.275299f, 13.693090f, -117.689507f, -29.982969f, 38.000011f, 10f); // Inside towards dock, small pool pt 1
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", -117.689507f, 12.426710f, -119.835289f, -27.878361f, 38.000011f, 10f); // Inside towards dock, small pool pt 2
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", -119.835289f, 10.661210f, -121.552803f, -26.316320f, 38.000011f, 10f); // Inside towards dock, small pool pt 3
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", -121.552803f, 8.839670f, -123.378937f, -24.332081f, 38.000011f, 10f); // Inside towards dock, small pool pt 4
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", -123.378937f, 7.186920f, -125.058151f, -22.762091f, 38.000011f, 10f); // Inside towards dock, small pool pt 5
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_w1", -125.058151f, 5.446620f, -126.930794f, -20.884710f, 38.000011f, 10f); // Inside towards dock, small pool pt 6
        }
    }
}
