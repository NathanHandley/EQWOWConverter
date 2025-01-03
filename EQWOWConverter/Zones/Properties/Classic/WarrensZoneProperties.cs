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
    internal class WarrensZoneProperties : ZoneProperties
    {
        public WarrensZoneProperties() : base()
        {
            // TODO: Add zone areas
            SetBaseZoneProperties("warrens", "The Warrens", -930f, 748f, -37.22f, 0, ZoneContinentType.Odus);
            SetZonewideEnvironmentAsIndoors(10, 10, 10, ZoneFogType.Heavy);
            OverrideVertexColorIntensity(0.4f);
            DisableSunlight();
            AddZoneLineBox("paineel", 721.470337f, -881.333069f, -36.999989f, ZoneLineOrientationType.South, 734.975342f, -874.463745f, -26.531000f, 713.524292f, -888.849182f, -37.499908f);
            AddZoneLineBox("stonebrunt", -3720.441895f, 2921.923096f, -39.687389f, ZoneLineOrientationType.South, -119.277092f, 1159.723511f, -93.500740f, -145.652954f, 1130f, -111.468353f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 617.556641f, 317.333496f, 545.656250f, 247.194733f, -114.968719f, 100f); // Pool
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 866.417175f, -456.959717f, 712.530212f, -537.631897f, -80.968719f, 100f); // Channel, north top
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 596.772156f, -537.912302f, 545.353271f, -589.962708f, -80.968719f, 100f); // Channel, north bottom (east protusion)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 714.974060f, -483.080292f, 223.336105f, -537.922302f, -80.978719f, 100f); // Channel, run through south (west side)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 545.971436f, -537.104858f, 223.336105f, -558.051575f, -80.978719f, 100f); // Channel, run through south (east side)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 347.002655f, -558.337036f, 234.729202f, -624.938477f, -80.978719f, 100f); // Channel, southeast past the gate
        }
    }
}
