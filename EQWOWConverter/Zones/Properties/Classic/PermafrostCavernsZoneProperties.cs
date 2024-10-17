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

namespace EQWOWConverter.Zones.Properties
{
    internal class PermafrostCavernsZoneProperties : ZoneProperties
    {
        public PermafrostCavernsZoneProperties() : base()
        {
            // TODO: Add zone areas
            // TODO: Add breath
            SetBaseZoneProperties("permafrost", "Permafrost Caverns", 0f, 0f, 3.75f, 0, ZoneContinentType.Antonica);
            SetZonewideEnvironmentAsIndoors(30, 40, 60, ZoneFogType.Heavy);
            Enable2DSoundInstances("wind_lp2", "wind_lp4", "caveloop", "torch_lp");
            OverrideVertexColorIntensity(0.3);
            DisableSunlight();
            AddZoneLineBox("everfrost", 2019.599976f, -7040.121094f, -63.843819f, ZoneLineOrientationType.West, -39.775318f, 172.344788f, 38.435791f, -80.162201f, 102.044090f, -0.499990f);
        }
    }
}
