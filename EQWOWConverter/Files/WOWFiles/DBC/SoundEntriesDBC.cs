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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Files.WOWFiles
{
    internal class SoundEntriesDBC
    {
        public class Row
        {
            public int Id;
            public int SoundType;
            public string Name = string.Empty;
            public string FileName = string.Empty;
            public string DirectoryBase = string.Empty;
            public float Volumefloat = 1f;
            public int Flags = 0; // For now, only Looping (0x02000) might be used
            public float MinDistance = 0f;
            public float DistanceCutoff = 0f;
        }

        List<Row> rows = new List<Row>();

        public void AddRow(Sound sound, string directory)
        {
            Row newRow = new Row();
            newRow.Id = sound.Id;
            newRow.SoundType = Convert.ToInt32(sound.Type);
            newRow.Name = sound.Name;
            newRow.FileName = sound.AudioFileName;
            newRow.DirectoryBase = directory;
            newRow.Volumefloat = sound.Volume;
            if (sound.Loop == true)
                newRow.Flags = 0x02000;
            else
                newRow.Flags = 0;
            newRow.MinDistance = sound.MinDistance;
            newRow.DistanceCutoff = sound.DistanceCutoff;
            rows.Add(newRow);
        }

        public void WriteToDisk(string baseFolderPath)
        {
            FileTool.CreateBlankDirectory(baseFolderPath, true);
            string fullFilePath = Path.Combine(baseFolderPath, "SoundEntriesDBC.csv");

            // Add each row of data (and header)
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("\"ID\",\"SoundType\",\"Name\",\"File_1\",\"File_2\",\"File_3\",\"File_4\",\"File_5\",\"File_6\",\"File_7\",\"File_8\",\"File_9\",\"File_10\",\"Freq_1\",\"Freq_2\",\"Freq_3\",\"Freq_4\",\"Freq_5\",\"Freq_6\",\"Freq_7\",\"Freq_8\",\"Freq_9\",\"Freq_10\",\"DirectoryBase\",\"Volumefloat\",\"Flags\",\"MinDistance\",\"DistanceCutoff\",\"EAXDef\",\"SoundEntriesAdvancedID\"");
            foreach (Row row in rows)
            {
                stringBuilder.Append("\"" + row.Id.ToString() + "\"");
                stringBuilder.Append(",\"" + row.SoundType.ToString() + "\"");
                stringBuilder.Append(",\"" + row.Name + "\"");
                stringBuilder.Append(",\"" + row.FileName + "\"");
                stringBuilder.Append(",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"1\",\"1\",\"1\",\"1\",\"1\",\"1\",\"1\",\"1\",\"1\",\"1\"");
                stringBuilder.Append(",\"" + row.DirectoryBase + "\"");
                stringBuilder.Append(",\"" + row.Volumefloat.ToString() + "\"");
                stringBuilder.Append(",\"" + row.Flags.ToString() + "\"");
                stringBuilder.Append(",\"" + row.MinDistance.ToString() + "\"");
                stringBuilder.Append(",\"" + row.DistanceCutoff.ToString() + "\"");
                stringBuilder.AppendLine(",\"2\",\"0\"");
            }

            // Output it
            File.WriteAllText(fullFilePath, stringBuilder.ToString());
        }
    }
}
