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

using EQWOWConverter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Files.WOWFiles
{
    internal class ZoneMusicDBC
    {
        public class Row
        {
            public int Id;
            public string SetName = string.Empty;
            public int SilenceIntervalMinDay = 180000; // Default (common in file)
            public int SilenceIntervalMinNight = 180000; // Default (common in file)
            public int SilenceIntervalMaxDay = 300000;  // Default (common in file)
            public int SilenceIntervalMaxNight = 300000; // Default (common in file)
            public int SoundEntryIDDay = 0;
            public int SoundEntryIDNight = 0;
        }

        List<Row> rows = new List<Row>();

        public void AddRow(int Id, string setName, int soundIDDay, int soundIDNight)
        {
            Row newRow = new Row();
            newRow.Id = Id;
            newRow.SetName = setName;
            newRow.SoundEntryIDDay = soundIDDay;
            newRow.SoundEntryIDNight = soundIDNight;
            rows.Add(newRow);
        }

        public void WriteToDisk(string baseFolderPath)
        {
            FileTool.CreateBlankDirectory(baseFolderPath, true);
            string fullFilePath = Path.Combine(baseFolderPath, "ZoneMusicDBC.csv");

            // Add each row of data (and header)
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("\"ID\",\"SetName\",\"SilenceintervalMin_1\",\"SilenceintervalMin_2\",\"SilenceintervalMax_1\",\"SilenceintervalMax_2\",\"Sounds_1\",\"Sounds_2\"");
            foreach (Row row in rows)
            {
                stringBuilder.Append("\"" + row.Id.ToString() + "\"");
                stringBuilder.Append(",\"" + row.SetName + "\"");
                stringBuilder.Append(",\"" + row.SilenceIntervalMinDay.ToString() + "\"");
                stringBuilder.Append(",\"" + row.SilenceIntervalMinNight.ToString() + "\"");
                stringBuilder.Append(",\"" + row.SilenceIntervalMaxDay.ToString() + "\"");
                stringBuilder.Append(",\"" + row.SilenceIntervalMaxNight.ToString() + "\"");
                stringBuilder.Append(",\"" + row.SoundEntryIDDay.ToString() + "\"");
                stringBuilder.AppendLine(",\"" + row.SoundEntryIDNight.ToString() + "\"");
            }

            // Output it
            File.WriteAllText(fullFilePath, stringBuilder.ToString());
        }
    }
}
