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
    internal class NajenaZoneProperties : ZoneProperties
    {
        public NajenaZoneProperties() : base()
        {
            SetBaseZoneProperties("najena", "Najena", -22.6f, 229.1f, -41.8f, 0, ZoneContinentType.Antonica);
            AddValidMusicInstanceTrackIndexes(1, 5, 7);
            SetZonewideEnvironmentAsIndoors(10, 10, 10, ZoneFogType.Heavy);
            DisableSunlight();
            OverrideVertexColorIntensity(0.4);
            AddZoneLineBox("lavastorm", -937.992371f, -1044.653320f, 12.625020f, ZoneLineOrientationType.West, 0.193110f, 929.818542f, 48.437752f, -30.192530f, 883.758789f, -0.499850f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Blood, "t75_b1", 352.940308f, 213.064240f, 308.154907f, 178.544678f, -28.999861f, 5f); // Blood pool under bridge
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_w1", 271.931427f, 65.983147f, 182.312454f, -28.775570f, -16.000010f, 30f); // East / Upper water
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_w1", 253.706802f, 122.485786f, 184.148834f, 61.766312f, -21.999990f, 30f); // West / Lower water
            AddLiquidPlaneZLevel(ZoneLiquidType.Blood, "t75_b1", 126.516800f, -139.600418f, 110.629539f, -155.153397f, 0.000040f, 10f); // Blood pool with spikes
        }
    }
}
