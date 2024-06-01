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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EQWOWConverter.Common;
using EQWOWConverter.WOWFiles;

namespace EQWOWConverter.Zones
{
    internal class ZonePropertiesLineBox
    {
        public int AreaTriggerID;
        public string TargetZoneShortName = string.Empty;
        public Vector3 TargetZonePosition = new Vector3();
        public float TargetZoneOrientation = 0f;
        public Vector3 BoxPosition = new Vector3();
        public float BoxLength;
        public float BoxWidth;
        public float BoxHeight;
        public float BoxOrientation;

        public ZonePropertiesLineBox()
        {
            AreaTriggerID = AreaTriggerDBC.GetGeneratedAreaTriggerID();
        }
    }
}
