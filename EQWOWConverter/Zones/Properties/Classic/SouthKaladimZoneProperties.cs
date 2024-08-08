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
            SetBaseZoneProperties("kaladima", "South Kaladim", -2f, -18f, 3.75f, 0, ZoneContinentType.Faydwer);
            SetZonewideEnvironmentAsIndoors(31, 22, 09, ZoneFogType.Heavy);
            OverrideVertexColorIntensity(0.5);
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
