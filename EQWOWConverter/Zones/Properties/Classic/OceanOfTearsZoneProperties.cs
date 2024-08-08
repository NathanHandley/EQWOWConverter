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
    internal class OceanOfTearsZoneProperties : ZoneProperties
    {
        public OceanOfTearsZoneProperties() : base()
        {
            // TODO: Boat connecting east freeport and butcherblock
            SetBaseZoneProperties("oot", "Ocean of Tears", -9200f, 390f, 6f, 0, ZoneContinentType.Antonica);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", 3854.056396f, 11660.213867f, -5955.389648f, -11718.996094f, -20.063040f, 300f);
        }
    }
}
