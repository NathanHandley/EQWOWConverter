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
            // TODO: Port Authority lamp is missing
            // TODO: Boat to Erudes Crossing
            // TODO: Make arena PVP
            // TODO: Night sounds
            SetBaseZoneProperties("qeynos", "South Qeynos", 186.46f, 14.29f, 3.75f, 0, ZoneContinentType.Antonica);
            //AddValidMusicInstanceTrackIndexes(0, 1, 4, 5, 7, 8, 9, 11, 12);                      
            AddZoneArea("Mermaid's Lure", 55.699829f, -28.153431f, 110f, -27.963150f, -69.853050f, -1.5f, "qeynos-07", "qeynos-07");
            AddZoneArea("Mermaid's Lure", 27.911350f, -28.153431f, 110f, -27.963150f, -111.824409f, -1.5f, "qeynos-07", "qeynos-07");
            AddChildZoneArea("Port Authority", "Qeynos Port", -112.286743f, -14.296410f, 110f, -181.730515f, -41.458660f, -1.5f);
            AddChildZoneArea("Port Authority", "Qeynos Port", -140.093994f, -14.296410f, 110f, -181.730515f, -69.877022f, -1.5f);
            AddZoneArea("Qeynos Port", 603.145630f, 1178.116821f, 110f, -818.473145f, -27.126450f, -200, "qeynos-11", "qeynos-11");
            AddZoneArea("Qeynos Port", 68.179031f, 1178.116821f, 110f, -818.473145f, -161.125412f, -200, "qeynos-11", "qeynos-11");
            AddZoneArea("Qeynos Port", 125.205193f, 1178.116821f, 110f, -818.473145f, -110.587914f, -200, "qeynos-11", "qeynos-11");
            AddZoneArea("Lion's Mane Tavern", 320.800293f, -138.393097f, 12.885730f, 238.945557f, -209.187393f, -3.097620f, "qeynos-08", "qeynos-08");
            AddChildZoneArea("Fhara's Leather and Thread", "Portside Market", 419.278473f, -70.400070f, 110f, 392.324066f, -97.543221f, -1.5f);
            AddChildZoneArea("Lion's Mane Inn", "Portside Market", 376.389526f, -56.460201f, 110f, 224.466034f, -139.712448f, -1.5f);
            AddChildZoneArea("Lion's Mane Inn", "Portside Market", 321.789978f, -138.242661f, 110f, 238.646545f, -208.137894f, -1.5f);
            AddZoneArea("Portside Market", 451.076477f, -24.364201f, 110f, 42.974331f, -115.627251f, -2.5f, "qeynos-01", "qeynos-01");
            AddZoneArea("Portside Market", 392.009552f, -24.364201f, 110f, 55.030651f, -253.412476f, -2.5f, "qeynos-01", "qeynos-01");
            AddZoneArea("The Wind Spirit's Song", 561.063110f, -97.356247f, 68.774757f, 458.601990f, -191.984634f, -3.556920f, "qeynos-05", "qeynos-01");
            AddZoneArea("The Tin Soldier", 544.479919f, -29.543560f, 68.774757f, 491.071747f, -68.705101f, -3.556920f);
            AddZoneArea("The Tin Soldier", 544.479919f, -29.543560f, 68.774757f, 505.489563f, -96.758728f, -3.556920f);


            AddZoneArea("Shrine of The Burning Prince", 363.724792f, -573.181641f, 57.943581f, 272.187714f, -711.572693f, -4.857540f, "qeynos-04", "qeynos-04");
            AddZoneArea("The Herb Jar", 334.422058f, -547.265747f, 41.242401f, 295.249237f, -574.166077f, -3.855790f);
            AddZoneArea("Office of the People", 337.030853f, -476.784546f, 36.715111f, 309.043182f, -516.782898f, -1.584190f);
            AddZoneArea("Qeynos Hold", 350.490814f, -412.891876f, 53.599510f, 306.851196f, -460.808105f, -5.409760f);












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
