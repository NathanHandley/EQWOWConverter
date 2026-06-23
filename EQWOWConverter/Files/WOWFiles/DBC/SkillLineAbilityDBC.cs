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
using EQWOWConverter.Player;
using EQWOWConverter.Spells;
using EQWOWConverter.Items;

namespace EQWOWConverter.WOWFiles
{
    internal class SkillLineAbilityDBC : DBCFile
    {
        private static int CUR_SKILLLINEABILITY_DBCID = Configuration.DBCID_SKILLLINEABILITY_ID_START;

        public void AddRow(int id, int skillLineID, int spellTemplateID, int acquireMethodID)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(id); // ID
            newRow.AddInt32(skillLineID); // SkillLine
            newRow.AddInt32(spellTemplateID); // Spell
            newRow.AddInt32(0); // RaceMask
            newRow.AddInt32(0); // ClassMask
            newRow.AddInt32(0); // ExcludeRace
            newRow.AddInt32(0); // ExcludeClass
            newRow.AddInt32(1); // MinSkillLineRank
            newRow.AddInt32(0); // SupercededBySpell
            newRow.AddInt32(acquireMethodID); // AcquireMethod (0 = learn by trainer, 1 = learned on skill value, 2 = learned on skill learn)
            newRow.AddInt32(0); // TrivialSkillLineRankHigh
            newRow.AddInt32(0); // TrivialSkillLineRankLow
            newRow.AddInt32(0); // CharacterPoints1
            newRow.AddInt32(0); // CharacterPoints2

            newRow.SortValue1 = skillLineID;
            newRow.SortValue2 = id;

            Rows.Add(newRow);
        }

        public void AddRow(int id, SpellTemplate spellTemplate, int spellTemplateID, int acquireMethodID)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(id); // ID
            newRow.AddInt32(spellTemplate.SkillLine); // SkillLine
            newRow.AddInt32(spellTemplateID); // Spell
            newRow.AddInt32(0); // RaceMask
            newRow.AddInt32(0); // ClassMask
            newRow.AddInt32(0); // ExcludeRace
            newRow.AddInt32(0); // ExcludeClass
            if (spellTemplate.TradeskillRecipe == null)
            {
                newRow.AddInt32(1); // MinSkillLineRank
                newRow.AddInt32(0); // SupercededBySpell
                newRow.AddInt32(acquireMethodID); // AcquireMethod (0 = learn by trainer, 1 = learned on skill value, 2 = learned on skill learn)
                newRow.AddInt32(0); // TrivialSkillLineRankHigh
                newRow.AddInt32(0); // TrivialSkillLineRankLow
            }
            else
            {
                newRow.AddInt32(spellTemplate.TradeskillRecipe.SkillRankNeededWOW); // MinSkillLineRank
                newRow.AddInt32(0); // SupercededBySpell
                newRow.AddInt32(acquireMethodID); // AcquireMethod (0 = learn by trainer, 1 = learned on skill value, 2 = learned on skill learn)
                newRow.AddInt32(spellTemplate.TradeskillRecipe.TrivialHighWOW); // TrivialSkillLineRankHigh
                newRow.AddInt32(spellTemplate.TradeskillRecipe.TrivialLowWOW); // TrivialSkillLineRankLow
            }
            newRow.AddInt32(0); // CharacterPoints1
            newRow.AddInt32(0); // CharacterPoints2

            newRow.SortValue1 = spellTemplate.SkillLine;
            newRow.SortValue2 = id;
            
            Rows.Add(newRow);
        }

        public static int GenerateID()
        {
            int spellID = CUR_SKILLLINEABILITY_DBCID;
            CUR_SKILLLINEABILITY_DBCID++;
            return spellID;
        }

        protected override void OnPostLoadDataFromDisk()
        {
            // Convert any raw data rows to actual data rows (which should be all of them)
            foreach (DBCRow row in Rows)
            {
                // This shouldn't be possible, but control for it just in case
                if (row.SourceRawBytes.Count == 0)
                {
                    Logger.WriteError("SkillLineAbilityDBC had no source raw bytes when converting a row in OnPostLoadDataFromDisk");
                    continue;
                }

                // Fill every field
                int byteCursor = 0;

                row.AddIntFromSourceRawBytes(ref byteCursor); // ID
                row.AddIntFromSourceRawBytes(ref byteCursor); // SkillLine
                row.AddIntFromSourceRawBytes(ref byteCursor); // Spell
                row.AddIntFromSourceRawBytes(ref byteCursor); // RaceMask
                row.AddIntFromSourceRawBytes(ref byteCursor); // ClassMask
                row.AddIntFromSourceRawBytes(ref byteCursor); // ExcludeRace
                row.AddIntFromSourceRawBytes(ref byteCursor); // ExcludeClass
                row.AddIntFromSourceRawBytes(ref byteCursor); // MinSkillLineRank
                row.AddIntFromSourceRawBytes(ref byteCursor); // SupercededBySpell
                row.AddIntFromSourceRawBytes(ref byteCursor); // AcquireMethod
                row.AddIntFromSourceRawBytes(ref byteCursor); // TrivialSkillLineRankHigh
                row.AddIntFromSourceRawBytes(ref byteCursor); // TrivialSkillLineRankLow
                row.AddIntFromSourceRawBytes(ref byteCursor); // CharacterPoints1
                row.AddIntFromSourceRawBytes(ref byteCursor); // CharacterPoints2

                row.SortValue1 = ((DBCRow.DBCFieldInt32)row.AddedFields[1]).Value; // SkillLine
                row.SortValue2 = ((DBCRow.DBCFieldInt32)row.AddedFields[0]).Value; // ID

                // Purge raw data
                row.SourceRawBytes.Clear();
            }

            // Update default learned DK abilities, as needed
            if (Configuration.PLAYER_DEATHKNIGHT_START_LIKE_OTHER_CLASSES == true)
            {
                foreach (DBCRow row in Rows)
                {
                    DBCRow.DBCFieldInt32 skillLineField = (DBCRow.DBCFieldInt32)row.AddedFields[1];
                    DBCRow.DBCFieldInt32 acquireMethodField = (DBCRow.DBCFieldInt32)row.AddedFields[9];
                    switch (skillLineField.Value)
                    {
                        case 770: acquireMethodField.Value = 0; break; // Blood
                        case 771: acquireMethodField.Value = 0; break; // Frost
                        case 772: acquireMethodField.Value = 0; break; // Unholy
                        default: break;
                    }
                }
            }
        }
    }
}
