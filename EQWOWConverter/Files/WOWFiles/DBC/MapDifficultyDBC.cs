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
            string fullFilePath = Path.Combine(baseFolderPath, "MapDifficultyDBC.csv");

            // Add each row of data (and header)
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("\"ID\",\"MapID\",\"Difficulty\",\"Message_Lang_enUS\",\"Message_Lang_enGB\",\"Message_Lang_koKR\",\"Message_Lang_frFR\",\"Message_Lang_deDE\",\"Message_Lang_enCN\",\"Message_Lang_zhCN\",\"Message_Lang_enTW\",\"Message_Lang_zhTW\",\"Message_Lang_esES\",\"Message_Lang_esMX\",\"Message_Lang_ruRU\",\"Message_Lang_ptPT\",\"Message_Lang_ptBR\",\"Message_Lang_itIT\",\"Message_Lang_Unk\",\"Message_Lang_Mask\",\"RaidDuration\",\"MaxPlayers\",\"Difficultystring\"");
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
