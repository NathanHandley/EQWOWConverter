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
    internal class SouthKaladimZoneProperties : ZoneProperties
    {
        public SouthKaladimZoneProperties() : base()
        {
            // TODO: Arena PVP
            SetBaseZoneProperties("kaladima", "South Kaladim", -2f, -18f, 3.75f, 0, ZoneContinentType.Faydwer);
            SetZonewideEnvironmentAsIndoors(31, 22, 09, ZoneFogType.Heavy);
            OverrideVertexColorIntensity(0.4);
            
            AddZoneArea("Main Gate", "kaladima-00", "kaladima-00", Configuration.CONFIG_AUDIO_MUSIC_DEFAULT_VOLUME, false);
            AddZoneAreaBox("Main Gate", 146.431183f, 62.509140f, 70.454086f, -109.933128f, -110.416473f, -10.129540f);

            AddZoneArea("Pub Kal", "kaladima-04", "kaladima-04");
            AddZoneAreaBox("Pub Kal", 118.635437f, 250.987381f, 20.040649f, 72.985977f, 228.588013f, -2.086410f);
            AddZoneAreaBox("Pub Kal", 94.753662f, 250.987381f, 20.040649f, 73.307312f, 205.085403f, -2.086410f);

            AddZoneArea("Irontoe's Eats", "kaladima-05", "kaladima-05");
            AddZoneAreaBox("Irontoe's Eats", 202.718903f, 334.681396f, 32.399288f, 169.567993f, 290.871521f, -5.210800f);

            AddZoneArea("Tanned Assets");
            AddZoneAreaBox("Tanned Assets", 179.209122f, 393.627869f, 56.327728f, 156.926529f, 361.014832f, -4.393900f);

            AddZoneArea("Redfist's Metal");
            AddZoneAreaBox("Redfist's Metal", 142.030411f, 406.263794f, 33.101601f, 119.972191f, 360.936554f, -3.876520f);

            AddZoneArea("Staff and Spear");
            AddZoneAreaBox("Staff and Spear", 154.675690f, 247.826767f, 28.332060f, 132.570541f, 204.328674f, -2.261180f);

            AddChildZoneArea("Gem Room", "Kaladim Castle", "kaladima-03", "kaladima-03");
            AddZoneAreaBox("Gem Room", -177.571320f, -339.534027f, 25.729700f, -219.530533f, -380.677124f, 2.961010f);

            AddZoneArea("Kaladim Castle", "kaladima-07", "kaladima-07");
            AddZoneAreaBox("Kaladim Castle", 140.091827f, -267.974731f, 104.200493f, -204.232773f, -444.398071f, -89.694290f);

            AddZoneArea("Castle Tunnel", "", "", 0f, false, "wind_lp2", "wind_lp2", 0.13931568f, 0.13931568f);
            AddZoneAreaBox("Castle Tunnel", 268.912476f, -276.845337f, 51.524540f, 225.391342f, -375.978149f, -13.769460f);
            AddZoneAreaBox("Castle Tunnel", 269.257782f, -301.463318f, 51.524540f, 137.570175f, -389.579071f, -2.326460f);

            AddZoneArea("East Tunnel", "", "", 0f, false, "wind_lp3", "wind_lp3", 0.13931568f, 0.13931568f);
            AddZoneAreaBox("East Tunnel", 249.309692f, -29.441420f, 53.632401f, 127.378166f, -117.933983f, -4.708280f);

            AddZoneArea("West Tunnel", "", "", 0f, false, "wind_lp2", "wind_lp2", 0.13931568f, 0.13931568f);
            AddZoneAreaBox("West Tunnel", 231.041077f, 191.201447f, 67.094452f, 129.096695f, 24.601440f, -7.553070f);
            AddZoneAreaBox("West Tunnel", 231.041077f, 191.201447f, 67.094452f, 73.621208f, 51.615170f, -7.553070f);

            AddZoneArea("Northwest Tunnel", "", "", 0f, false, "wind_lp4", "wind_lp4", 0.13931568f, 0.13931568f);
            AddZoneAreaBox("Northeast Tunnel", 499.900879f, 362.115509f, 64.797981f, 240.681122f, 219.488556f, -29.046940f);
            AddZoneAreaBox("Northeast Tunnel", 464.164642f, -184.274612f, 113.766930f, 270.746704f, -287.887421f, -15.605070f);

            AddZoneArea("Kaladim Arena", "kaladima-01", "kaladima-01");
            AddZoneAreaOctagonBox("Kaladim Arena", 153.392624f, 73.905548f, 345.278442f, 266.113373f, 320.122925f, 292.471558f, 317.966187f, 293.934387f,
                126.058197f, 101.773773f, 126.058197f, 101.773773f, 31.041071f, -21.248930f);

            AddZoneArea("Warrior's Hall", "kaladima-02", "kaladima-02");
            AddZoneAreaBox("Warrior's Hall", 98.731529f, 547.754944f, 68.954941f, -115.038628f, 321.714050f, -17.766130f);

            AddZoneLineBox("butcher", 3121.1667f, -179.98013f, 0.00088672107f, ZoneLineOrientationType.South, -66.545395f, 47.896313f, 14.469f, -85.64434f, 34.009415f, -0.49999186f);
            AddZoneLineBox("kaladimb", 409.332306f, 340.759308f, -24.000509f, ZoneLineOrientationType.North, 334.304260f, 252.005707f, 16.310989f, 317.203705f, 225.868561f, 0.608990f);
            AddZoneLineBox("kaladimb", 394.005920f, -270.823303f, 0.000210f, ZoneLineOrientationType.North, 414.648987f, -209.715607f, 22.469000f, 405.986603f, -280f, -0.499960f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0001", 268.664917f, -122.803329f, 170.169144f, -212.128967f, -1.999970f, 50f); // Big Water Area, northwest most near waterfall
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0001", 193.904186f, -210.322464f, 169.361099f, -233.385437f, -1.999970f, 50f); // Big Water Area, midsection
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0001", 170.362366f, -211.477966f, 44.062649f, -333.230957f, -1.999970f, 50f); // Big Water Area, southern
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0001", 88.341553f, 375.290588f, 86.891579f, 368.514465f, 1.000010f, 50f); // Pool near arena (starts north)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0001", 87.941553f, 375.690588f, 86.491579f, 368.114465f, 1.000010f, 50f); // Pool near arena
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0001", 86.541553f, 377.090588f, 85.091579f, 366.714465f, 1.000010f, 50f); // Pool near arena
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0001", 85.141553f, 378.490588f, 83.691579f, 365.314465f, 1.000010f, 50f); // Pool near arena
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0001", 83.741553f, 379.890588f, 82.291579f, 363.914465f, 1.000010f, 50f); // Pool near arena
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0001", 82.341553f, 381.290588f, 80.891579f, 362.514465f, 1.000010f, 50f); // Pool near arena
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0001", 80.941553f, 381.290588f, 79.491579f, 361.114465f, 1.000010f, 50f); // Pool near arena
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0001", 79.541553f, 381.290588f, 78.091579f, 359.714465f, 1.000010f, 50f); // Pool near arena
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0001", 78.141553f, 379.890588f, 76.691578f, 358.314465f, 1.000010f, 50f); // Pool near arena
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0001", 76.741553f, 378.490588f, 75.291578f, 356.914465f, 1.000010f, 50f); // Pool near arena
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0001", 75.341552f, 377.090588f, 73.891578f, 355.514465f, 1.000010f, 50f); // Pool near arena
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0001", 73.941552f, 375.690588f, 72.491578f, 355.514465f, 1.000010f, 50f); // Pool near arena
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0001", 72.541552f, 374.290588f, 71.091578f, 355.514465f, 1.000010f, 50f); // Pool near arena
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0001", 71.141552f, 372.890588f, 69.691578f, 355.514465f, 1.000010f, 50f); // Pool near arena
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0001", 69.741552f, 371.490588f, 68.291578f, 356.514465f, 1.000010f, 50f); // Pool near arena
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0001", 68.341552f, 370.090588f, 66.891578f, 357.914465f, 1.000010f, 50f); // Pool near arena
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0001", 66.941552f, 368.690588f, 65.491578f, 359.314465f, 1.000010f, 50f); // Pool near arena
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_m0001", 65.541552f, 367.290588f, 64.091578f, 360.714465f, 1.000010f, 50f); // Pool near arena
        }
    }
}