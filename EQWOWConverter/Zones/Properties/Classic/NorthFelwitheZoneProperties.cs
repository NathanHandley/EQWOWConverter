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
    internal class NorthFelwitheZoneProperties : ZoneProperties
    {
        public NorthFelwitheZoneProperties() : base()
        {
            // TODO: More zone areas
            Enable2DSoundInstances("wind_lp2", "wind_lp3", "wind_lp4");
            SetZonewideMusic("felwithea-00", "felwithea-00", true);

            AddZoneArea("Entry", "felwithea-01", "felwithea-01", false);
            AddZoneAreaBox("Entry", 117.707916f, 462.981293f, 186.264618f, -86.893219f, 30.036381f, -100f);

            if (Configuration.AUDIO_USE_ALTERNATE_TRACKS == false)
                AddZoneArea("Water Outlook", "felwithea-02", "felwithea-02");
            else
                AddZoneArea("Water Outlook");
            AddZoneAreaBox("Water Outlook", 392.955811f, -536.260681f, 44.567032f, 74.084091f, -926.284851f, -77.346931f);
            AddZoneAreaBox("Water Outlook", 122.281776f, -638.404175f, 3.277470f, -162.649353f, -939.232300f, -119.733437f);

            SetZonewideEnvironmentAsOutdoorsWithSky(58, 75, 58, ZoneFogType.Medium, 0.5f, 1f);
            AddZoneLineBox("felwitheb", 251.268646f, -832.815125f, -13.999020f, ZoneLineOrientationType.North, 364.650452f, -711.921509f, -1.531000f, 342.316345f, -727.911865f, -14.499750f);
            AddZoneLineBox("gfaydark", -1931.678101f, -2613.879639f, 20.406450f, ZoneLineOrientationType.West, 56.161152f, 242.410782f, 26.469000f, 27.806530f, 193.596893f, -0.500000f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_agua1", 137.808594f, -34.700352f, 31.669941f, -209.193726f, -14.01f, 300f); // West pool
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_agua1", 42.782681f, -41.832130f, -32.244209f, -154.059235f, -14.01f, 300f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_agua1", -27.781460f, -41.832870f, -130.866898f, -209.193726f, -14.01f, 300f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_agua1", 305.541901f, -629.289001f, -168.728821f, -835.178894f, -27.997999f, 300f); // East pool
        }
    }
}
