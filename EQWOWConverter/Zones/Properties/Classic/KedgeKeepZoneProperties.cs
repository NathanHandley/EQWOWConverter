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
    internal class KedgeKeepZoneProperties : ZoneProperties
    {
        public KedgeKeepZoneProperties() : base()
        {
            SetBaseZoneProperties("kedge", "Kedge Keep", 99.96f, 14.02f, 31.75f, 0, ZoneContinentType.Faydwer);
            SetZonewideEnvironmentAsIndoors(66, 101, 134, ZoneFogType.Heavy, 170, 248, 248);
            OverrideVertexColorIntensity(0.5);
            AddZoneLineBox("cauldron", -1170.507080f, -1030.383179f, -315.376831f, ZoneLineOrientationType.East, 140.130951f, 14.514380f, 348.342682f, 119.745049f, -10.192420f, 299.375000f);
            SetIsCompletelyInLiquid(ZoneLiquidType.Water);
        }
    }
}
