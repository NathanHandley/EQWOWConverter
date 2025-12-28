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

namespace EQWOWConverter.Zones.Properties
{
    internal class CabilisEastZoneProperties : ZoneProperties
    {
        public CabilisEastZoneProperties() : base()
        {
            SetZonewideEnvironmentAsOutdoorsNoSky(12, 12, 8, ZoneFogType.Heavy, 1f);

            // Retest
            AddZoneLineBox("swampofnohope", 2971.826660f, 3241.556885f, 0.125060f, ZoneLineOrientationType.East,
                -32.775379f,  -628.916870f, 175.343414f, -196.997025f, -687.232239f, -12.133560f);
            AddZoneLineBox("swampofnohope", 3115.613770f, 3062.930908f, 0.125170f, ZoneLineOrientationType.South,
                 -211.574753f, -411.067505f, 241.294952f,  -340.382843f, -496.183960f, -13.999000f);

            AddZoneLineBox("fieldofbone", -2557.7278f, 3688.0273f, 4.093815f, ZoneLineOrientationType.East, 1377.6309f, -455.81412f, 97.201485f, 1346.7754f, -497.1183f, -0.49989557f);
            AddZoneLineBox("fieldofbone", -2747.7383f, 3530.195f, 4.093984f, ZoneLineOrientationType.North, 1236.0558f, -605.5564f, 128.95297f, 1192.7297f, -635.9432f, -0.4994932f);
            AddZoneLineBox("cabwest", -13.886450f, 314.975342f, 0.000000f, ZoneLineOrientationType.North, -3.434140f, 322.059662f, 12.469000f, -21.590590f, 307.681549f, -0.499970f);
            AddZoneLineBox("cabwest", -13.976320f, 338.086029f, -24.860001f, ZoneLineOrientationType.North, -6.287930f, 350.279877f, 12.469000f, -18.972679f, 321.680542f, -42.468731f);
            AddZoneLineBox("cabwest", -14.334330f, 371.205414f, 0.000030f, ZoneLineOrientationType.North, -13.192510f, 378.441284f, 12.469000f, -21.719170f, 349.526428f, -0.499940f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_cbw1", 968.333923f, 13.227140f, 596.166870f, -297.584686f, -14, 45f); // Main water area, north section (and lots of center)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_cbw1", 633.157593f, 599.122559f, -352.483093f, -568.373352f, -14, 45f); // Main water area + west/south channel
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_cbw1", 728.487732f, -255.757553f, -321.543762f, -563.571838f, -14, 45f); // Main water area + more south channel, some east channel
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_cbw1", 607.391602f, -424.373993f, 396.922333f, -1025.532593f, -14, 45f); // East water tunnels off the main area
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_cbw1", 873.178711f, -495.658478f, 821.317810f, -551.591064f, -125.937424f, 30f); // Eastern area sphere-shaped carve out room small pond
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_cbw1", -348.487427f, -39.968380f, -496.760773f, -213.096375f, -0.999940f, 20f); // South area wading pools

            AddDiscardGeometryBox(-352.861359f, 68.610992f, -6.312210f, -522.594238f, -20.493219f, -108.501801f); // Rock and some water below the map
            AddDiscardGeometryBox(-360.150452f, 194.734756f, 135.514343f, -503.430725f, 105.218224f, -43.974831f); // Two out-of-bounds water strips
        }
    }
}
