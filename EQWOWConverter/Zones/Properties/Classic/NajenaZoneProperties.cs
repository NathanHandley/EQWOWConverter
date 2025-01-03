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
    internal class NajenaZoneProperties : ZoneProperties
    {
        public NajenaZoneProperties() : base()
        {
            // TODO: Add a key system
            SetBaseZoneProperties("najena", "Najena", -22.6f, 229.1f, -41.8f, 0, ZoneContinentType.Antonica);
            SetZonewideEnvironmentAsIndoors(10, 10, 10, ZoneFogType.Heavy);
            DisableSunlight();
            OverrideVertexColorIntensity(0.4);

            AddZoneArea("Najena's Room");
            AddZoneAreaBox("Najena's Room", 381.038269f, 145.555283f, 25.470989f, 290.597717f, 78.611732f, -3.714120f);
            AddZoneAreaBox("Najena's Room", 354.981506f, 87.133827f, 18.075880f, 334.488220f, 68.708702f, -2.803370f);

            AddZoneArea("Outer Cells", "najena-05", "najena-05");
            AddZoneAreaBox("Outer Cells", 188.322433f, 263.384155f, 22.455700f, 56.767941f, 156.196060f, -2.675950f);
            AddZoneAreaBox("Outer Cells", 144.808136f, 308.840302f, 20.609859f, 40.894390f, 242.812607f, -4.665660f);
            AddZoneAreaBox("Outer Cells", 153.959641f, 370.154694f, 17.217840f, 94.792511f, 285.069122f, -6.889870f);
            AddZoneAreaBox("Outer Cells", 111.206787f, 356.432190f, 18.995781f, 69.502853f, 324.381866f, -5.414920f);

            AddZoneArea("Inner Caves", "najena-01", "najena-01");
            AddZoneAreaBox("Inner Caves", 552.575256f, 264.871124f, 53.369011f, 366.090240f, -140.405533f, -87.243607f);
            AddZoneAreaBox("Inner Caves", 399.670868f, 125.590057f, 38.050861f, 42.449169f, -111.452927f, -66.999542f);
            AddZoneAreaBox("Inner Caves", 56.121181f, -138.746948f, -33.216862f, 97.280510f, -71.216118f, 21.620979f);
            AddZoneAreaBox("Inner Caves", 116.320702f, -101.356247f, 40.723801f, 61.063919f, -117.019707f, -22.047810f);
            AddZoneAreaBox("Inner Caves", 187.795578f, -53.726349f, 58.342331f, 150.526154f, -163.406158f, -23.697611f);

            if (Configuration.CONFIG_AUDIO_USE_ALTERNATE_TRACKS == true)
                AddZoneArea("Prison", "najena-00", "najena-00");
            else
                AddZoneArea("Prison");
            AddZoneAreaBox("Prison", 21.234501f, 25.369350f, -14.850650f, -205.015564f, -312.356842f, -53.671120f);

            if (Configuration.CONFIG_AUDIO_USE_ALTERNATE_TRACKS == true)
                AddZoneArea("Slaughter Halls", "najena-06", "najena-06");
            else
                AddZoneArea("Slaughter Halls");
            AddZoneAreaBox("Slaughter Halls", 375.946014f, 366.574188f, 30.997030f, 148.481201f, 282.367737f, -45);
            AddZoneAreaBox("Slaughter Halls", 375.946014f, 281.867737f, 30.997030f, 42.580471f, 157.239090f, -45);
            AddZoneAreaBox("Slaughter Halls", 378.851807f, 307.444977f, -1.1f, 24.694099f, 125.745117f, -30.448811f);

            if (Configuration.CONFIG_AUDIO_USE_ALTERNATE_TRACKS == true)
                AddZoneArea("Torture Halls", "najena-02", "najena-02");
            else
                AddZoneArea("Torture Halls");
            AddZoneAreaBox("Torture Halls", 230.296432f, -94.949051f, 51.312790f, -206.064926f, -368.088531f, -54.869240f);
            AddZoneAreaBox("Torture Halls", -17.631281f, -43.657410f, 33.118420f, -84.742889f, -168.686264f, -9.989460f);

            AddZoneArea("Main Entry", "najena-07", "najena-07");
            AddZoneAreaBox("Main Entry", 59.568878f, 237.314423f, 43.435940f, -123.449753f, 30.747480f, -4.009430f);
            AddZoneAreaBox("Main Entry", 52.602741f, 37.922661f, 23.360029f, -28.192450f, -100.173920f, -4f);

            AddZoneLineBox("lavastorm", -937.992371f, -1044.653320f, 12.625020f, ZoneLineOrientationType.West, 0.193110f, 929.818542f, 48.437752f, -30.192530f, 883.758789f, -0.499850f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Blood, "t75_b1", 352.940308f, 213.064240f, 308.154907f, 178.544678f, -28.999861f, 5f); // Blood pool under bridge
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_w1", 271.931427f, 65.983147f, 182.312454f, -28.775570f, -16.000010f, 30f); // East / Upper water
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_w1", 253.706802f, 122.485786f, 184.148834f, 61.766312f, -21.999990f, 30f); // West / Lower water
            AddLiquidPlaneZLevel(ZoneLiquidType.Blood, "t75_b1", 126.516800f, -139.600418f, 110.629539f, -155.153397f, 0.000040f, 10f); // Blood pool with spikes
        }
    }
}
