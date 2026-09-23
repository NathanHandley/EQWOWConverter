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
    internal class CharSectionsDBC : DBCFile
    {
        // Sections the client composes character visuals from
        public const int SECTION_BASE_SKIN = 0;
        public const int SECTION_FACE = 1;
        public const int SECTION_FACIAL_HAIR = 2;
        public const int SECTION_HAIR = 3;
        public const int SECTION_UNDERWEAR = 4;

        // Matches the flags on stock playable race rows.  Face rows are the exception: no stock face row ever carries the 0x10 bit (they are 1, 5, or 8), and face rows with 0x10 set are skipped by the client's face
        // section lookup, which leaves the composite's face regions unblitted (a featureless fill-colored head)
        private const int SECTION_FLAGS = 17;
        private const int SECTION_FLAGS_FACE = 1;

        public void AddRow(int sectionID, int raceID, int sexID, int baseSection, string texture1, string texture2, string texture3,
            int variationIndex, int colorIndex)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(sectionID); // ID
            newRow.AddInt32(raceID); // RaceID (ChrRaces.dbc)
            newRow.AddInt32(sexID); // SexID (0 = male, 1 = female)
            newRow.AddInt32(baseSection); // BaseSection (0 base skin, 1 face, 2 facial hair, 3 hair, 4 underwear)
            newRow.AddString(texture1); // TextureName 1
            newRow.AddString(texture2); // TextureName 2
            newRow.AddString(texture3); // TextureName 3
            if (baseSection == SECTION_FACE)
                newRow.AddInt32(SECTION_FLAGS_FACE); // Flags
            else
                newRow.AddInt32(SECTION_FLAGS); // Flags
            newRow.AddInt32(variationIndex); // VariationIndex (skin/hair/face selection byte)
            newRow.AddInt32(colorIndex); // ColorIndex (color selection byte)
            newRow.SortValue1 = 1; // After all stock rows
            newRow.SortValue2 = sectionID; // IDs are banded per (race, sex, section), so this keeps every key contiguous
            Rows.Add(newRow);
        }

        protected override void OnPostLoadDataFromDisk()
        {
            // The client indexes this table by scanning rows in FILE order: for each (race, sex, section) key it sizes the variation array from the contiguous run holding the key's last row,
            // then writes every row of that key at its variation index.  The stock file is grouped one run per key but is NOT in ID order, so re-sorting stock rows by ID splits the keys and
            // makes the client overflow those arrays (intermittent heap corruption -> startup crash at 0x0055F519).  Stock rows therefore keep their original file order and the added rows go after them, contiguous per key.
            for (int i = 0; i < Rows.Count; i++)
            {
                Rows[i].SortValue1 = 0;
                Rows[i].SortValue2 = i;
            }
        }
    }
}
