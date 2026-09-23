//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2026 Nathan Handley
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

namespace EQWOWConverter.WOWFiles
{
    internal class ChrRacesDBC : DBCFile
    {
        // ChrRaces.dbc in 3.3.5 has 69 fields per row (276 bytes).  The new rows for EQ races are byte clones of the Human row with
        // targeted fields patched, so every client-required field stays valid without this converter modeling the full layout
        private const int RECORD_SIZE_BYTES = 276;
        private const int FIELD_OFFSET_ID = 0;
        private const int FIELD_OFFSET_MALE_DISPLAY_ID = 16;   // Field 4
        private const int FIELD_OFFSET_FEMALE_DISPLAY_ID = 20; // Field 5
        private const int FIELD_OFFSET_CLIENT_FILE_STRING = 44; // Field 11 (the client resolves Character\{ClientFileString}\ models with it)
        private const int FIELD_OFFSET_NAME_LANG_ENUS = 56;    // Field 14 (name_lang english offset)
        private const int SOURCE_ROW_RACE_ID_HUMAN = 1;

        public void AddRowClonedFromHuman(int raceID, string raceName, string clientFileString, int maleDisplayInfoID, int femaleDisplayInfoID)
        {
            // Find the Human row to clone
            DBCRow? humanRow = null;
            foreach (DBCRow row in Rows)
            {
                if (row.SourceRawBytes.Count >= 4 && BitConverter.ToInt32(row.SourceRawBytes.ToArray(), 0) == SOURCE_ROW_RACE_ID_HUMAN)
                {
                    humanRow = row;
                    break;
                }
            }
            if (humanRow == null || humanRow.SourceRawBytes.Count != RECORD_SIZE_BYTES)
            {
                Logger.WriteError(string.Concat("ChrRacesDBC could not find a valid Human row to clone, so race '", raceID.ToString(), "' was not added"));
                return;
            }

            // Clone and patch it
            byte[] newRowBytes = humanRow.SourceRawBytes.ToArray();
            WriteInt32IntoBytes(newRowBytes, FIELD_OFFSET_ID, raceID);
            // Flag 0x1 = not playable: every stock race kept off the character creation screen (Goblin, Naga, Vrykul, etc)
            const int FIELD_OFFSET_FLAGS = 4;
            WriteInt32IntoBytes(newRowBytes, FIELD_OFFSET_FLAGS, BitConverter.ToInt32(newRowBytes, FIELD_OFFSET_FLAGS) | 0x1);
            WriteInt32IntoBytes(newRowBytes, FIELD_OFFSET_MALE_DISPLAY_ID, maleDisplayInfoID);
            WriteInt32IntoBytes(newRowBytes, FIELD_OFFSET_FEMALE_DISPLAY_ID, femaleDisplayInfoID);
            WriteInt32IntoBytes(newRowBytes, FIELD_OFFSET_CLIENT_FILE_STRING, PutStringInStringBlockAndGetOffset(clientFileString));
            WriteInt32IntoBytes(newRowBytes, FIELD_OFFSET_NAME_LANG_ENUS, PutStringInStringBlockAndGetOffset(raceName));

            DBCRow newRow = new DBCRow();
            newRow.SourceRawBytes.AddRange(newRowBytes);
            newRow.SortValue1 = raceID;
            Rows.Add(newRow);
        }

        private static void WriteInt32IntoBytes(byte[] targetBytes, int offset, int value)
        {
            byte[] valueBytes = BitConverter.GetBytes(value);
            for (int i = 0; i < 4; i++)
                targetBytes[offset + i] = valueBytes[i];
        }

        protected override void OnPostLoadDataFromDisk()
        {
            // Rows stay as raw bytes, but they need sort values so the stock rows keep a stable order against the added rows
            foreach (DBCRow row in Rows)
            {
                if (row.SourceRawBytes.Count >= 4)
                    row.SortValue1 = BitConverter.ToInt32(row.SourceRawBytes.ToArray(), 0);
            }
        }
    }
}
