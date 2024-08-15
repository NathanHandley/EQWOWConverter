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

namespace EQWOWConverter.Files.WOWFiles
{
    internal class LightDBC
    {
        public class Row
        {
            public int Id;
            public int MapId;
            public float PositionX;
            public float PositionY;
            public float PositionZ;
            public float FalloffStart;
            public float FalloffEnd;
            public int ParamsClearID;
            public int ParamsClearWaterID;
            public int ParamsStormID;
            public int ParamsStormWaterID;

            // DBCIDs
            private static int CURRENT_LIGHTID = Configuration.CONFIG_DBCID_LIGHT_START;

            public Row()
            {
                Id = CURRENT_LIGHTID;
                CURRENT_LIGHTID++;
            }
        }

        private List<Row> rows = new List<Row>();

        public void AddRow(int mapId, ZoneEnvironmentSettings zoneEnvironmentSettings)
        {
            Row newRow = new Row();
            newRow.MapId = mapId;
            newRow.PositionX = zoneEnvironmentSettings.PositionX;
            newRow.PositionY = zoneEnvironmentSettings.PositionY;
            newRow.PositionZ = zoneEnvironmentSettings.PositionZ;
            newRow.FalloffStart = zoneEnvironmentSettings.FalloffStart;
            newRow.FalloffEnd = zoneEnvironmentSettings.FalloffEnd;
            newRow.ParamsClearID = zoneEnvironmentSettings.ParamatersClearWeather.DBCLightParamsID;
            newRow.ParamsClearWaterID = zoneEnvironmentSettings.ParamatersClearWeatherUnderwater.DBCLightParamsID;
            newRow.ParamsStormID = zoneEnvironmentSettings.ParamatersStormyWeather.DBCLightParamsID;
            newRow.ParamsStormWaterID = zoneEnvironmentSettings.ParamatersStormyWeatherUnderwater.DBCLightParamsID;
            rows.Add(newRow);
        }

        public void WriteToDisk(string baseFolderPath)
        {
            FileTool.CreateBlankDirectory(baseFolderPath, true);
            string fullFilePath = Path.Combine(baseFolderPath, "LightDBC.csv");

            // Add each row of data (and header)
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("\"ID\",\"ContinentID\",\"X\",\"Y\",\"Z\",\"FalloffStart\",\"FalloffEnd\",\"LightParamsID_1\",\"LightParamsID_2\",\"LightParamsID_3\",\"LightParamsID_4\",\"LightParamsID_5\",\"LightParamsID_6\",\"LightParamsID_7\",\"LightParamsID_8\"");
            foreach (Row row in rows)
            {
                stringBuilder.Append("\"" + row.Id.ToString() + "\"");
                stringBuilder.Append(",\"" + row.MapId.ToString() + "\"");
                stringBuilder.Append(",\"" + row.PositionX.ToString() + "\"");
                stringBuilder.Append(",\"" + row.PositionY.ToString() + "\"");
                stringBuilder.Append(",\"" + row.PositionZ.ToString() + "\"");
                stringBuilder.Append(",\"" + row.FalloffStart.ToString() + "\"");
                stringBuilder.Append(",\"" + row.FalloffEnd.ToString() + "\"");
                stringBuilder.Append(",\"" + row.ParamsClearID.ToString() + "\"");
                stringBuilder.Append(",\"" + row.ParamsClearWaterID.ToString() + "\"");
                stringBuilder.Append(",\"" + row.ParamsStormID.ToString() + "\"");
                stringBuilder.Append(",\"" + row.ParamsStormWaterID.ToString() + "\"");
                stringBuilder.Append(",\"4\"");
                stringBuilder.Append(",\"0\"");
                stringBuilder.Append(",\"0\"");
                stringBuilder.AppendLine(",\"0\"");
            }

            // Output it
            File.WriteAllText(fullFilePath, stringBuilder.ToString());
        }
    }
}
