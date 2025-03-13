//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2025 Nathan Handley
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

namespace EQWOWConverter.WOWFiles
{
    internal class SoundEntriesDBC : DBCFile
    {
        public void AddRow(Sound sound, string fileNameWithExt, string directory)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(sound.DBCID);
            newRow.AddInt32(Convert.ToInt32(sound.Type));
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
            newRow.AddInt32(1); // Frequency 1
            newRow.AddInt32(0); // Frequency 2
            newRow.AddInt32(0); // Frequency 3
            newRow.AddInt32(0); // Frequency 4
            newRow.AddInt32(0); // Frequency 5
            newRow.AddInt32(0); // Frequency 6
            newRow.AddInt32(0); // Frequency 7
            newRow.AddInt32(0); // Frequency 8
            newRow.AddInt32(0); // Frequency 9
            newRow.AddInt32(0); // Frequency 10
            newRow.AddString(directory);
            newRow.AddFloat(sound.GetVolume());
            int flags = 0;
            if (sound.Loop == true)
                flags |= 0x200;
            //if (sound.NoOverlap == true)
            //    flags |= 0x0020;
            newRow.AddPackedFlags(flags);
            newRow.AddFloat(sound.MinDistance);
            newRow.AddFloat(sound.DistanceCutoff);
            newRow.AddInt32(2);
            newRow.AddInt32(0); // SoundEntriesAdvancedID
            Rows.Add(newRow);
        }
    }
}
