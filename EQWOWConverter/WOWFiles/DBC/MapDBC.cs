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

namespace EQWOWConverter.WOWFiles
{
    internal class MapDBC
    {
        public class Row
        {
            public int Id;
            public string Directory = string.Empty;
            public int InstanceType = 0;// 0 = All, 1 = Group Only, 2 = Raid, 3 = Battleground
            public int Flags = 0;       // Unsure what this is, looks mostly 0 or 1
            public string MapName = string.Empty;
            public int AreaTableID;
            public int LoadingScreenID = 254; // Temporary Assignment
        }

        List<Row> rows = new List<Row>();

        public void AddRow(int id, string directory, string mapName, int areaTableID)
        {
            Row newRow = new Row();
            newRow.Id = id;
            newRow.Directory = directory;
            newRow.AreaTableID = areaTableID;
            newRow.MapName = mapName;
            rows.Add(newRow);
        }

        public void WriteToDisk(string baseFolderPath)
        {
            FileTool.CreateBlankDirectory(baseFolderPath, true);
            string fullFilePath = Path.Combine(baseFolderPath, "Map.csv");

            // Add each row of data
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Row row in rows)
            {
                stringBuilder.Append("\"" + row.Id.ToString() + "\"");
                stringBuilder.Append(",\"" + row.Directory + "\"");
                stringBuilder.Append(",\"" + row.InstanceType.ToString() + "\"");
                stringBuilder.Append(",\"" + row.Flags.ToString() + "\"");
                stringBuilder.Append(",\"0\"");
                stringBuilder.Append(",\"" + row.MapName + "\"");
                stringBuilder.Append(",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"16712190\"");
                stringBuilder.Append(",\"" + row.AreaTableID.ToString() + "\"");
                stringBuilder.Append(",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"16712188\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"16712188\"");
                stringBuilder.Append(",\"" + row.LoadingScreenID.ToString() + "\"");
                stringBuilder.AppendLine(",\"1\",\"0\",\"0\",\"0\",\"-1\",\"2\",\"0\",\"0\"");
            }

            // Output it
            File.WriteAllText(fullFilePath, stringBuilder.ToString());
        }
    }
}
