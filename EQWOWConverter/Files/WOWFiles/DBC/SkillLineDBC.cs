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

using EQWOWConverter.Spells;

namespace EQWOWConverter.WOWFiles
{
    internal class SkillLineDBC : DBCFile
    {
        private static int CUR_ID = Configuration.DBCID_SKILLLINE_ID_START;
        private static Dictionary<SpellEQSkillCategory, int> SkillLineIDBySkillCategory = new Dictionary<SpellEQSkillCategory, int>();
        private static readonly object ID_LOCK = new object();

        public void AddRow(int id, string name, int spellIconID)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(id); // ID
            newRow.AddInt32(7); // CategoryID (7 = class)
            newRow.AddInt32(0); // SkillCostsID
            newRow.AddStringLang(name); // DisplayName
            newRow.AddStringLang(""); // Description
            newRow.AddInt32(spellIconID); // SpellIconID
            newRow.AddStringLang(""); // AlternateVerb
            newRow.AddInt32(0); // CanLink

            newRow.SortValue1 = id;

            Rows.Add(newRow);
        }

        protected override void OnPostLoadDataFromDisk()
        {
            // Convert any raw data rows to actual data rows
            foreach (DBCRow row in Rows)
            {
                // This shouldn't be possible, but control for it just in case
                if (row.SourceRawBytes.Count == 0)
                {
                    Logger.WriteError("SkillLineDBC had no source raw bytes when converting a row in OnPostLoadDataFromDisk");
                    continue;
                }

                // Fill every field
                int byteCursor = 0;
                row.AddIntFromSourceRawBytes(ref byteCursor); // ID
                row.AddIntFromSourceRawBytes(ref byteCursor); // CategoryID
                row.AddIntFromSourceRawBytes(ref byteCursor); // SkillCostsID
                row.AddStringLangFromSourceRawBytes(ref byteCursor, StringBlock); // DisplayName
                row.AddStringLangFromSourceRawBytes(ref byteCursor, StringBlock); // Description
                row.AddIntFromSourceRawBytes(ref byteCursor); // SpellIconID
                row.AddStringLangFromSourceRawBytes(ref byteCursor, StringBlock); // AlternateVerb
                row.AddIntFromSourceRawBytes(ref byteCursor); // CanLink

                row.SortValue1 = ((DBCRow.DBCFieldInt32)row.AddedFields[0]).Value; // ID

                // Purge raw data
                row.SourceRawBytes.Clear();
            }
        }

        public static int GetIDForSkillCatagory(SpellEQSkillCategory skillCategory)
        {
            return GetAllSkillLineIDsBySkillCategory()[skillCategory];
        }

        public static Dictionary<SpellEQSkillCategory, int> GetAllSkillLineIDsBySkillCategory()
        {
            if (SkillLineIDBySkillCategory.Count == 0)
            {
                foreach (SpellEQSkillCategory category in Enum.GetValues<SpellEQSkillCategory>())
                {
                    if (category != SpellEQSkillCategory.Unknown)
                        SkillLineIDBySkillCategory.Add(category, GenerateID());
                }
            }
            return SkillLineIDBySkillCategory;
        }

        private static int GenerateID()
        {
            int newID = CUR_ID;
            CUR_ID++;
            return CUR_ID;
        }
    }
}
