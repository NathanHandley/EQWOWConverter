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
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal class AreaTableDBC
    {
        public class Row : IComparable<Row>
        {
            private static int CURRENT_AREABIT = Configuration.CONFIG_DBCID_AREATABLE_AREABIT_START;

            public int Id;
            public int ContinentID = 724;
            public int ParentAreaID = 0;
            public int AreaBit = 0;
            public int Flags = 0;
            public int ZoneMusic = 0;
            public int IntroSound = 0;
            public int ExplorationLevel = 0;
            public string AreaName = string.Empty;       

            public Row()
            {
                AreaBit = CURRENT_AREABIT;
                CURRENT_AREABIT++;
            }

            public int CompareTo(Row? otherRow)
            {
                if (otherRow == null)
                    return -1;
                if (Id == otherRow.Id)
                    return 0;
                else if (Id < otherRow.Id)
                    return -1;
                else
                    return 1;
            }
        }

        private List<Row> rows = new List<Row>();

        public void AddRow(int id, int parentAreaID, ZoneAreaMusic? areaMusic, string areaName)
        {
            Row newRow = new Row();
            newRow.Id = id;
            newRow.ParentAreaID = parentAreaID;
            if (areaMusic != null)
                newRow.ZoneMusic = areaMusic.DBCID;
            else
                newRow.ZoneMusic = 0;
            newRow.AreaName = areaName;
            rows.Add(newRow);
        }

        public void WriteToDisk(string baseFolderPath)
        {
            // Sort first
            rows.Sort();

            FileTool.CreateBlankDirectory(baseFolderPath, true);
            string fullFilePath = Path.Combine(baseFolderPath, "AreaTableDBC.csv");

            // Add each row of data (and header)
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("\"ID\",\"ContinentID\",\"ParentAreaID\",\"AreaBit\",\"Flags\",\"SoundProviderPref\",\"SoundProviderPrefUnderwater\",\"AmbienceID\",\"ZoneMusic\",\"IntroSound\",\"ExplorationLevel\",\"AreaName_Lang_enUS\",\"AreaName_Lang_enGB\",\"AreaName_Lang_koKR\",\"AreaName_Lang_frFR\",\"AreaName_Lang_deDE\",\"AreaName_Lang_enCN\",\"AreaName_Lang_zhCN\",\"AreaName_Lang_enTW\",\"AreaName_Lang_zhTW\",\"AreaName_Lang_esES\",\"AreaName_Lang_esMX\",\"AreaName_Lang_ruRU\",\"AreaName_Lang_ptPT\",\"AreaName_Lang_ptBR\",\"AreaName_Lang_itIT\",\"AreaName_Lang_Unk\",\"AreaName_Lang_Mask\",\"FactionGroupMask\",\"LiquidTypeID_1\",\"LiquidTypeID_2\",\"LiquidTypeID_3\",\"LiquidTypeID_4\",\"MinElevation\",\"Ambient_Multiplier\",\"Lightid\"");
            foreach(Row row in rows)
            {
                stringBuilder.Append("\"" + row.Id.ToString() + "\"");
                stringBuilder.Append(",\"" + row.ContinentID.ToString() + "\"");
                stringBuilder.Append(",\"" + row.ParentAreaID.ToString() + "\"");
                stringBuilder.Append(",\"" + row.AreaBit.ToString() + "\"");
                stringBuilder.Append(",\"" + row.Flags.ToString() + "\"");
                stringBuilder.Append(",\"0\",\"0\",\"0\"");
                stringBuilder.Append(",\"" + row.ZoneMusic.ToString() + "\"");
                stringBuilder.Append(",\"" + row.IntroSound.ToString() + "\"");
                stringBuilder.Append(",\"" + row.ExplorationLevel.ToString() + "\"");
                stringBuilder.Append(",\"" + row.AreaName + "\"");
                stringBuilder.AppendLine(",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"16712190\",\"0\",\"0\",\"0\",\"0\",\"0\",\"-5000\",\"0\",\"0\"");
            }

            // Output it
            File.WriteAllText(fullFilePath, stringBuilder.ToString());
        }
    }

}
