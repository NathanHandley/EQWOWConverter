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
            public int InstanceType = 0;// 0 = All, 1 = Group Only, 2 = Raid, 3 = Battleground, 4 = PVP?
            public int Flags = 0;       // Unsure what this is, looks mostly 0 or 1
            public string MapName = string.Empty;
            public int AreaTableID;
            public int LoadingScreenID;
        }

        List<Row> rows = new List<Row>();

        public void AddRow(int id, string directory, string mapName, int areaTableID, int loadingScreenID)
        {
            Row newRow = new Row();
            newRow.Id = id;
            newRow.Directory = directory;
            newRow.AreaTableID = areaTableID;
            newRow.MapName = mapName;
            newRow.LoadingScreenID = loadingScreenID;
            rows.Add(newRow);
        }

        public void WriteToDiskForPatch(string baseFolderPath)
        {
            FileTool.CreateBlankDirectory(baseFolderPath, true);
            string fullFilePath = Path.Combine(baseFolderPath, "MapDBC.csv");

            // Add each row of data (and header)
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("\"ID\",\"Directory\",\"InstanceType\",\"Flags\",\"PVP\",\"MapName_Lang_enUS\",\"MapName_Lang_enGB\",\"MapName_Lang_koKR\",\"MapName_Lang_frFR\",\"MapName_Lang_deDE\",\"MapName_Lang_enCN\",\"MapName_Lang_zhCN\",\"MapName_Lang_enTW\",\"MapName_Lang_zhTW\",\"MapName_Lang_esES\",\"MapName_Lang_esMX\",\"MapName_Lang_ruRU\",\"MapName_Lang_ptPT\",\"MapName_Lang_ptBR\",\"MapName_Lang_itIT\",\"MapName_Lang_Unk\",\"MapName_Lang_Mask\",\"AreaTableID\",\"MapDescription0_Lang_enUS\",\"MapDescription0_Lang_enGB\",\"MapDescription0_Lang_koKR\",\"MapDescription0_Lang_frFR\",\"MapDescription0_Lang_deDE\",\"MapDescription0_Lang_enCN\",\"MapDescription0_Lang_zhCN\",\"MapDescription0_Lang_enTW\",\"MapDescription0_Lang_zhTW\",\"MapDescription0_Lang_esES\",\"MapDescription0_Lang_esMX\",\"MapDescription0_Lang_ruRU\",\"MapDescription0_Lang_ptPT\",\"MapDescription0_Lang_ptBR\",\"MapDescription0_Lang_itIT\",\"MapDescription0_Lang_Unk\",\"MapDescription0_Lang_Mask\",\"MapDescription1_Lang_enUS\",\"MapDescription1_Lang_enGB\",\"MapDescription1_Lang_koKR\",\"MapDescription1_Lang_frFR\",\"MapDescription1_Lang_deDE\",\"MapDescription1_Lang_enCN\",\"MapDescription1_Lang_zhCN\",\"MapDescription1_Lang_enTW\",\"MapDescription1_Lang_zhTW\",\"MapDescription1_Lang_esES\",\"MapDescription1_Lang_esMX\",\"MapDescription1_Lang_ruRU\",\"MapDescription1_Lang_ptPT\",\"MapDescription1_Lang_ptBR\",\"MapDescription1_Lang_itIT\",\"MapDescription1_Lang_Unk\",\"MapDescription1_Lang_Mask\",\"LoadingScreenID\",\"MinimapIconScale\",\"CorpseMapID\",\"CorpseX\",\"CorpseY\",\"TimeOfDayOverride\",\"ExpansionID\",\"RaidOffset\",\"MaxPlayers\"");
            foreach (Row row in rows)
            {
                stringBuilder.Append("\"" + row.Id.ToString() + "\"");
                stringBuilder.Append(",\"" + row.Directory + "\"");
                stringBuilder.Append(",\"" + row.InstanceType.ToString() + "\"");
                stringBuilder.Append(",\"" + row.Flags.ToString() + "\"");
                stringBuilder.Append(",\"0\"");
                stringBuilder.Append(",\"" + row.MapName + "\"");
                stringBuilder.Append(",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"16712190\"");
                stringBuilder.Append(",\"0\""); // Override AreaTableID in patch version to 0 so music will play from wmoareatable
                stringBuilder.Append(",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"16712188\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"16712188\"");
                stringBuilder.Append(",\"" + row.LoadingScreenID.ToString() + "\"");
                stringBuilder.AppendLine(",\"1\",\"0\",\"0\",\"0\",\"-1\",\"2\",\"0\",\"0\"");
            }

            // Output it
            File.WriteAllText(fullFilePath, stringBuilder.ToString());
        }

        public void WriteToDiskForServer(string baseFolderPath)
        {
            FileTool.CreateBlankDirectory(baseFolderPath, true);
            string fullFilePath = Path.Combine(baseFolderPath, "MapDBC.csv");

            // Add each row of data (and header)
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("\"ID\",\"Directory\",\"InstanceType\",\"Flags\",\"PVP\",\"MapName_Lang_enUS\",\"MapName_Lang_enGB\",\"MapName_Lang_koKR\",\"MapName_Lang_frFR\",\"MapName_Lang_deDE\",\"MapName_Lang_enCN\",\"MapName_Lang_zhCN\",\"MapName_Lang_enTW\",\"MapName_Lang_zhTW\",\"MapName_Lang_esES\",\"MapName_Lang_esMX\",\"MapName_Lang_ruRU\",\"MapName_Lang_ptPT\",\"MapName_Lang_ptBR\",\"MapName_Lang_itIT\",\"MapName_Lang_Unk\",\"MapName_Lang_Mask\",\"AreaTableID\",\"MapDescription0_Lang_enUS\",\"MapDescription0_Lang_enGB\",\"MapDescription0_Lang_koKR\",\"MapDescription0_Lang_frFR\",\"MapDescription0_Lang_deDE\",\"MapDescription0_Lang_enCN\",\"MapDescription0_Lang_zhCN\",\"MapDescription0_Lang_enTW\",\"MapDescription0_Lang_zhTW\",\"MapDescription0_Lang_esES\",\"MapDescription0_Lang_esMX\",\"MapDescription0_Lang_ruRU\",\"MapDescription0_Lang_ptPT\",\"MapDescription0_Lang_ptBR\",\"MapDescription0_Lang_itIT\",\"MapDescription0_Lang_Unk\",\"MapDescription0_Lang_Mask\",\"MapDescription1_Lang_enUS\",\"MapDescription1_Lang_enGB\",\"MapDescription1_Lang_koKR\",\"MapDescription1_Lang_frFR\",\"MapDescription1_Lang_deDE\",\"MapDescription1_Lang_enCN\",\"MapDescription1_Lang_zhCN\",\"MapDescription1_Lang_enTW\",\"MapDescription1_Lang_zhTW\",\"MapDescription1_Lang_esES\",\"MapDescription1_Lang_esMX\",\"MapDescription1_Lang_ruRU\",\"MapDescription1_Lang_ptPT\",\"MapDescription1_Lang_ptBR\",\"MapDescription1_Lang_itIT\",\"MapDescription1_Lang_Unk\",\"MapDescription1_Lang_Mask\",\"LoadingScreenID\",\"MinimapIconScale\",\"CorpseMapID\",\"CorpseX\",\"CorpseY\",\"TimeOfDayOverride\",\"ExpansionID\",\"RaidOffset\",\"MaxPlayers\"");
            foreach (Row row in rows)
            {
                stringBuilder.Append("\"" + row.Id.ToString() + "\"");
                stringBuilder.Append(",\"" + row.Directory + "\"");
                stringBuilder.Append(",\"" + row.InstanceType.ToString() + "\"");
                stringBuilder.Append(",\"" + row.Flags.ToString() + "\"");
                stringBuilder.Append(",\"0\"");
                stringBuilder.Append(",\"" + row.MapName + "\"");
                stringBuilder.Append(",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"16712190\"");
                stringBuilder.Append(",\"" + row.AreaTableID.ToString() + "\""); // Use areatable here on server version so the map name will show up in the character screen
                stringBuilder.Append(",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"16712188\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"16712188\"");
                stringBuilder.Append(",\"" + row.LoadingScreenID.ToString() + "\"");
                stringBuilder.AppendLine(",\"1\",\"0\",\"0\",\"0\",\"-1\",\"2\",\"0\",\"0\"");
            }

            // Output it
            File.WriteAllText(fullFilePath, stringBuilder.ToString());
        }
    }
}
