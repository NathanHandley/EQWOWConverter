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
            public int HighlightSky;
            public float Glow;
        }

        private List<Row> rows = new List<Row>();

        public void AddRow(ZoneEnvironmentSettings.ZoneEnvironmentParameters environmentParameters)
        {
            Row newRow = new Row();
            newRow.Id = environmentParameters.DBCLightParamsID;
            newRow.Glow = environmentParameters.Glow;
            newRow.HighlightSky = environmentParameters.HighlightSky;
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
                stringBuilder.Append(",\"" + row.HighlightSky.ToString() + "\"");
                stringBuilder.Append(",\"" + row.LightSkyboxID.ToString() + "\"");
                stringBuilder.Append(",\"0\""); // CloudTypeID (always 0)
                stringBuilder.Append(",\"" + row.Glow.ToString() + "\"");
                stringBuilder.Append(",\"0.5\""); // Water Shallow Alpha
                stringBuilder.Append(",\"1\""); // Water Deep Alpha
                stringBuilder.Append(",\"0.75\""); // Ocean Shallow Alpha
                stringBuilder.AppendLine(",\"1\""); // Ocean Deep Alpha
            }

            // Output it
            File.WriteAllText(fullFilePath, stringBuilder.ToString());
        }
    }
}
