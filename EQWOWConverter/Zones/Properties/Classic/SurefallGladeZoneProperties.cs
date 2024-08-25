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
    internal class SurefallGladeZoneProperties : ZoneProperties
    {
        public SurefallGladeZoneProperties() : base()
        {
            SetBaseZoneProperties("qrg", "Surefall Glade", 136.9f, -65.9f, 4f, 0, ZoneContinentType.Antonica);
            //AddValidMusicInstanceTrackIndexes(2, 3);
            SetZonewideEnvironmentAsOutdoorsNoSky(114, 111, 116, ZoneFogType.Heavy, 1f);
            DisableSunlight();
            AddZoneLineBox("qeytoqrg", 5180.557617f, 161.911987f, -6.594880f, ZoneLineOrientationType.West, -623.557495f, 168.640945f, 0.500030f, -639.942505f, 150.659027f, -0.499970f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 264.136719f, 37.288700f, 48.358829f, -182.936539f, -3.999990f, 100f); // Pool around house on stilts
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 688.963257f, -248.454926f, 141.856928f, -567.358032f, -1.000000f, 100f); // Cave water, high
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 141.866928f, -420.606171f, 89.630241f, -567.358032f, -3.999990f, 100f); // Cave water, low
        }
    }
}
