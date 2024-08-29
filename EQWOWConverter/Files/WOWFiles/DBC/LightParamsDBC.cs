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

using EQWOWConverter.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal class LightParamsDBC : DBCFile
    {
        public void AddRow(ZoneEnvironmentSettings.ZoneEnvironmentParameters environmentParameters)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt(environmentParameters.DBCLightParamsID);
            newRow.AddInt(environmentParameters.HighlightSky);
            newRow.AddInt(0); // Light Skybox ID
            newRow.AddInt(0); // Cloud Type ID
            newRow.AddFloat(environmentParameters.Glow);
            newRow.AddFloat(0.5f); // Water Shallow Alpha
            newRow.AddFloat(1f); // Water Deep Alpha
            newRow.AddFloat(0.75f); // Ocean Shallow Alpha
            newRow.AddFloat(1f); // Ocean Deep Alpha
            Rows.Add(newRow);
        }
    }
}
