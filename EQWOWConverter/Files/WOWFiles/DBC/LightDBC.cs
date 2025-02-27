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

using EQWOWConverter.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EQWOWConverter.WOWFiles
{
    internal class LightDBC : DBCFile
    {
        // DBCIDs
        private static int CURRENT_LIGHTID = Configuration.DBCID_LIGHT_ID_START;

        public void AddRow(int mapId, ZoneEnvironmentSettings zoneEnvironmentSettings)
        {
            // Generate unique Light ID
            int lightID = CURRENT_LIGHTID;
            CURRENT_LIGHTID++;

            DBCRow newRow = new DBCRow();
            newRow.AddInt32(lightID);
            newRow.AddInt32(mapId); // "ContinentID" (map ID)
            newRow.AddFloat(zoneEnvironmentSettings.PositionX);
            newRow.AddFloat(zoneEnvironmentSettings.PositionY);
            newRow.AddFloat(zoneEnvironmentSettings.PositionZ);
            newRow.AddFloat(zoneEnvironmentSettings.FalloffStart);
            newRow.AddFloat(zoneEnvironmentSettings.FalloffEnd);
            newRow.AddInt32(zoneEnvironmentSettings.ParamatersClearWeather.DBCLightParamsID);
            newRow.AddInt32(zoneEnvironmentSettings.ParamatersClearWeatherUnderwater.DBCLightParamsID);
            newRow.AddInt32(zoneEnvironmentSettings.ParamatersStormyWeather.DBCLightParamsID);
            newRow.AddInt32(zoneEnvironmentSettings.ParamatersStormyWeatherUnderwater.DBCLightParamsID);
            newRow.AddInt32(4); // Death Parameters ID
            newRow.AddInt32(0); // Unknown 1
            newRow.AddInt32(0); // Unknown 2
            newRow.AddInt32(0); // Unknown 3
            Rows.Add(newRow);
        }
    }
}
