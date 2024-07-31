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
    internal class MapDifficultyDBC
    {
        public class Row
        {
            public int Id;
            public int MapID;
            public int Diffculty = 0; // Not sure 100%
            public string RejectionMessage = string.Empty;
            public int RaidDuration = 0; // Most raids are 86400 or 604800
            public int MaxPlayers = 0;
            public string Diffcultystring = string.Empty;

            public Row()
            {

            }
        }

        List<Row> rows = new List<Row>();

        public void AddRow(int mapID, int mapDifficultyID)
        {
            Row newRow = new Row();
            newRow.MapID = mapID;
            newRow.Id = mapDifficultyID;
            rows.Add(newRow);
        }

        public void WriteToDisk(string baseFolderPath)
        {
            FileTool.CreateBlankDirectory(baseFolderPath, true);
            string fullFilePath = Path.Combine(baseFolderPath, "MapDifficulty.csv");

            // Add each row of data
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Row row in rows)
            {
                stringBuilder.Append("\"" + row.Id.ToString() + "\"");
                stringBuilder.Append(",\"" + row.MapID.ToString() + "\"");
                stringBuilder.Append(",\"" + row.Diffculty.ToString() + "\"");
                stringBuilder.Append(",\"" + row.RejectionMessage + "\"");
                stringBuilder.Append(",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"16712190\"");
                stringBuilder.Append(",\"" + row.RaidDuration.ToString() + "\"");
                stringBuilder.Append(",\"" + row.MaxPlayers.ToString() + "\"");
                stringBuilder.AppendLine(",\"" + row.Diffcultystring + "\"");
            }

            // Output it
            File.WriteAllText(fullFilePath, stringBuilder.ToString());
        }
    }
}
