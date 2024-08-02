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
    internal class LightParamsDBC
    {
        public class Row
        {
            public int Id;
            public int LightSkyboxID = 0;
            public float Glow;
        }

        private List<Row> rows = new List<Row>();

        public void AddRow(ZoneEnvironmentSettings.ZoneEnvironmentParameters environmentParameters)
        {
            Row newRow = new Row();
            newRow.Id = environmentParameters.DBCLightParamsID;
            newRow.Glow = environmentParameters.Glow;
            rows.Add(newRow);
        }

        public void WriteToDisk(string baseFolderPath)
        {
            FileTool.CreateBlankDirectory(baseFolderPath, true);
            string fullFilePath = Path.Combine(baseFolderPath, "LightParamsDBC.csv");

            // Add each row of data
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Row row in rows)
            {
                stringBuilder.Append("\"" + row.Id.ToString() + "\"");
                stringBuilder.Append(",\"0\"");
                stringBuilder.Append(",\"" + row.LightSkyboxID.ToString() + "\"");
                stringBuilder.Append(",\"0\"");
                stringBuilder.Append(",\"" + row.Glow.ToString() + "\"");
                stringBuilder.Append(",\"0.5\"");
                stringBuilder.Append(",\"1\"");
                stringBuilder.Append(",\"1\"");
                stringBuilder.AppendLine(",\"1\"");
            }

            // Output it
            File.WriteAllText(fullFilePath, stringBuilder.ToString());
        }
    }
}
