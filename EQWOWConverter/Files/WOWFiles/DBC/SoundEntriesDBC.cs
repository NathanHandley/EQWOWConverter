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
using static System.Net.WebRequestMethods;
using System.Xml.Linq;

namespace EQWOWConverter.WOWFiles
{
    internal class SoundEntriesDBC : DBCFile
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

        public void AddRow(Sound sound, string fileNameWithExt, string directory)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt(sound.DBCID);
            newRow.AddInt(Convert.ToInt32(sound.Type));
            newRow.AddString(sound.Name);
            newRow.AddString(fileNameWithExt);
            newRow.AddString(string.Empty); // FileName 2
            newRow.AddString(string.Empty); // FileName 3
            newRow.AddString(string.Empty); // FileName 4
            newRow.AddString(string.Empty); // FileName 5
            newRow.AddString(string.Empty); // FileName 6
            newRow.AddString(string.Empty); // FileName 7
            newRow.AddString(string.Empty); // FileName 8
            newRow.AddString(string.Empty); // FileName 9
            newRow.AddString(string.Empty); // FileName 10
            newRow.AddInt(1); // Frequency 1
            newRow.AddInt(1); // Frequency 2
            newRow.AddInt(1); // Frequency 3
            newRow.AddInt(1); // Frequency 4
            newRow.AddInt(1); // Frequency 5
            newRow.AddInt(1); // Frequency 6
            newRow.AddInt(1); // Frequency 7
            newRow.AddInt(1); // Frequency 8
            newRow.AddInt(1); // Frequency 9
            newRow.AddInt(1); // Frequency 10
            newRow.AddString(directory);
            newRow.AddFloat(sound.Volume);
            if (sound.Loop == true)
                newRow.AddPackedFlags(0x02000);
            else
                newRow.AddPackedFlags(0);
            newRow.AddFloat(sound.MinDistance);
            newRow.AddFloat(sound.DistanceCutoff);
            newRow.AddInt(2); // EAXDef
            newRow.AddInt(0); // SoundEntriesAdvancedID
            Rows.Add(newRow);
        }
    }
}
