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
    internal class CreatureFamilyDBC : DBCFile
    {
        public void AddRowForPetSkillLines(int id, string name, int firstSkillLineID, int secondSkillLineID)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(id); // ID
            newRow.AddFloat(1f); // MinScale
            newRow.AddInt32(1); // MinScaleLevel
            newRow.AddFloat(1f); // MaxScale
            newRow.AddInt32(60); // MaxScaleLevel
            newRow.AddInt32(firstSkillLineID); // SkillLine1
            newRow.AddInt32(secondSkillLineID); // SkillLine2
            newRow.AddInt32(0); // PetFoodMask (no diet, so these can never be fed)
            newRow.AddInt32(-1); // PetTalentType (-1 blocks the pet talent UI, matching the warlock minion families)
            newRow.AddInt32(-1); // CategoryEnumID
            newRow.AddStringLang(name); // Name
            newRow.AddString(string.Empty); // IconFile

            newRow.SortValue1 = id;

            Rows.Add(newRow);
        }

        protected override void OnPostLoadDataFromDisk()
        {
            // Convert any raw data rows to actual data rows (which should be all of them)
            foreach (DBCRow row in Rows)
            {
                // This shouldn't be possible, but control for it just in case
                if (row.SourceRawBytes.Count == 0)
                {
                    Logger.WriteError("CreatureFamilyDBC had no source raw bytes when converting a row in OnPostLoadDataFromDisk");
                    continue;
                }

                // Fill every field
                int byteCursor = 0;
                row.AddIntFromSourceRawBytes(ref byteCursor); // ID
                row.AddFloatFromSourceRawBytes(ref byteCursor); // MinScale
                row.AddIntFromSourceRawBytes(ref byteCursor); // MinScaleLevel
                row.AddFloatFromSourceRawBytes(ref byteCursor); // MaxScale
                row.AddIntFromSourceRawBytes(ref byteCursor); // MaxScaleLevel
                row.AddIntFromSourceRawBytes(ref byteCursor); // SkillLine1
                row.AddIntFromSourceRawBytes(ref byteCursor); // SkillLine2
                row.AddIntFromSourceRawBytes(ref byteCursor); // PetFoodMask
                row.AddIntFromSourceRawBytes(ref byteCursor); // PetTalentType
                row.AddIntFromSourceRawBytes(ref byteCursor); // CategoryEnumID
                row.AddStringLangFromSourceRawBytes(ref byteCursor, StringBlock); // Name
                row.AddStringFromSourceRawBytes(ref byteCursor, StringBlock); // IconFile

                row.SortValue1 = ((DBCRow.DBCFieldInt32)row.AddedFields[0]).Value; // ID

                // Purge raw data
                row.SourceRawBytes.Clear();
            }
        }
    }
}
