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
    internal class WMOAreaTableDBC
    {
        public class Row
        {
            private static int CURRENT_ID = 52000; // Reserving 52000-60000

            public int ID;
            public int WMOID;
            public int WMOGroupID;
            public int AmbienceID = 0;
            public int ZoneMusic = 0;
            public int IntroSound = 0;
            public int Flags = 0;
            public int AreaTableID = 0;
            public string AreaName = string.Empty;

            public Row()
            {
                ID = CURRENT_ID;
                CURRENT_ID++;
            }
        }

        List<Row> rows = new List<Row>();

        public void AddRow(int wmoID, int wmoGroupID, int areaTableID, string areaName)
        {
            Row newRow = new Row();
            newRow.WMOID = wmoID;
            newRow.WMOGroupID = wmoGroupID;
            newRow.AreaTableID = areaTableID;
            newRow.AreaName = areaName;
            rows.Add(newRow);
        }

        public void WriteToDisk(string baseFolderPath)
        {
            FileTool.CreateBlankDirectory(baseFolderPath, true);
            string fullFilePath = Path.Combine(baseFolderPath, "WMOAreaTableDBC.csv");

            // Add each row of data
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Row row in rows)
            {
                stringBuilder.Append("\"" + row.ID.ToString() + "\"");
                stringBuilder.Append(",\"" + row.WMOID.ToString() + "\"");
                stringBuilder.Append(",\"0\"");
                stringBuilder.Append(",\"" + row.WMOGroupID.ToString() + "\"");
                stringBuilder.Append(",\"0\",\"0\"");
                stringBuilder.Append(",\"" + row.AmbienceID.ToString() + "\"");
                stringBuilder.Append(",\"" + row.ZoneMusic.ToString() + "\"");
                stringBuilder.Append(",\"" + row.IntroSound.ToString() + "\"");
                stringBuilder.Append(",\"" + row.Flags.ToString() + "\"");
                stringBuilder.Append(",\"" + row.AreaTableID.ToString() + "\"");
                stringBuilder.Append(",\"" + row.AreaName + "\"");
                stringBuilder.AppendLine(",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"16712190\"");              
            }

            // Output it
            File.WriteAllText(fullFilePath, stringBuilder.ToString());
        }               
    }
}
