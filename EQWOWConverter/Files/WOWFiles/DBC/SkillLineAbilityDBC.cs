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

        public HashSet<int> GetSpellIDsForSkillLines(HashSet<int> skillLineIDs)
        {
            HashSet<int> spellIDs = new HashSet<int>();
            foreach (DBCRow row in Rows)
            {
                if (row.AddedFields.Count < 3)
                    continue;
                if (row.AddedFields[1] is not DBCRow.DBCFieldInt32 skillLineField || row.AddedFields[2] is not DBCRow.DBCFieldInt32 spellField)
                    continue;
                if (skillLineIDs.Contains(skillLineField.Value) == true)
                    spellIDs.Add(spellField.Value);
            }
            return spellIDs;
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

            // Fill in the racial abilities that blizzard class-gated, as needed
            if (Configuration.PLAYER_ADD_MISSING_ALL_CLASS_RACIAL_ABILITIES == true)
                AddMissingRacialAbilityClasses();

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

        // Every wow class mask or'd together (class ID 10 does not exist, so 512 is skipped)
        private static readonly int ALL_CLASSES_MASK = 1535;

        private class RacialAbilityClassGap
        {
            public RacialAbilityClassGap(string name, int widenedSpellID, int addedClassMask, List<int> allVersionSpellIDs)
            {
                Name = name;
                WidenedSpellID = widenedSpellID;
                AddedClassMask = addedClassMask;
                AllVersionSpellIDs = allVersionSpellIDs;
            }

            public string Name;
            public int WidenedSpellID;
            public int AddedClassMask;
            public List<int> AllVersionSpellIDs;
        }

        // class masks: Warrior 1, Paladin 2, Hunter 4, Rogue 8, Priest 16, DeathKnight 32, Shaman 64, Mage 128, Warlock 256, Druid 1024
        private static List<RacialAbilityClassGap> GetRacialAbilityClassGaps()
        {
            return new List<RacialAbilityClassGap>()
            {
                // Orc, where 20572 is attack power only and 33702 is spell power only, so the both-halves version is used
                new RacialAbilityClassGap("Blood Fury", 33697, 1170, new List<int>() { 20572, 33702, 33697 }), // Paladin, Priest, Mage, Druid

                // Night Elf, which is one spell for all classes
                new RacialAbilityClassGap("Elusiveness", 21009, 450, new List<int>() { 21009 }), // Paladin, Shaman, Mage, Warlock

                // Blood Elf, where the version restore different power types (25046 energy, 50613 runic power), and the mana version is the one the left out classes actually run on
                new RacialAbilityClassGap("Arcane Torrent", 28730, 1089, new List<int>() { 25046, 28730, 50613 }), // Warrior, Shaman, Druid

                // Draenei, where the version used heals off whichever of spell power or attack power is higher
                new RacialAbilityClassGap("Gift of the Naaru", 59547, 1288, new List<int>() { 28880, 59542, 59543, 59544, 59545, 59547, 59548 }), // Rogue, Warlock, Druid

                // Draenei, where both version give the same melee and spell hit
                new RacialAbilityClassGap("Heroic Presence", 28878, 1288, new List<int>() { 6562, 28878 }), // Rogue, Warlock, Druid

                // Draenei, where every version is the same shadow resistance passive
                new RacialAbilityClassGap("Shadow Resistance", 59540, 1288, new List<int>() { 59221, 59535, 59536, 59538, 59539, 59540, 59541 }) // Rogue, Warlock, Druid
            };
        }

        private void AddMissingRacialAbilityClasses()
        {
            List<RacialAbilityClassGap> racialAbilityClassGaps = GetRacialAbilityClassGaps();
            Dictionary<int, int> classMaskAdditionsBySpellID = new Dictionary<int, int>();
            foreach (RacialAbilityClassGap racialAbilityClassGap in racialAbilityClassGaps)
                classMaskAdditionsBySpellID.Add(racialAbilityClassGap.WidenedSpellID, racialAbilityClassGap.AddedClassMask);
            HashSet<int> racialSkillLineIDs = new HashSet<int>() { 101, 124, 125, 126, 220, 733, 753, 754, 756, 760 };

            HashSet<int> updatedSpellIDs = new HashSet<int>();
            Dictionary<int, int> classMasksBySpellID = new Dictionary<int, int>();
            foreach (DBCRow row in Rows)
            {
                DBCRow.DBCFieldInt32 skillLineField = (DBCRow.DBCFieldInt32)row.AddedFields[1];
                if (racialSkillLineIDs.Contains(skillLineField.Value) == false)
                    continue;
                DBCRow.DBCFieldInt32 spellField = (DBCRow.DBCFieldInt32)row.AddedFields[2];
                DBCRow.DBCFieldInt32 classMaskField = (DBCRow.DBCFieldInt32)row.AddedFields[4];

                // A ClassMask of 0 already means 'every class', so leave it alone
                if (classMaskAdditionsBySpellID.ContainsKey(spellField.Value) == true)
                {
                    updatedSpellIDs.Add(spellField.Value);
                    if (classMaskField.Value != 0)
                        classMaskField.Value |= classMaskAdditionsBySpellID[spellField.Value];
                }

                // Hold what every class can reach afterwards, so the coverage check below has something to read
                int rowClassMask = classMaskField.Value == 0 ? ALL_CLASSES_MASK : classMaskField.Value;
                if (classMasksBySpellID.ContainsKey(spellField.Value) == false)
                    classMasksBySpellID.Add(spellField.Value, rowClassMask);
                else
                    classMasksBySpellID[spellField.Value] |= rowClassMask;
            }

            foreach (RacialAbilityClassGap racialAbilityClassGap in racialAbilityClassGaps)
            {
                if (updatedSpellIDs.Contains(racialAbilityClassGap.WidenedSpellID) == false)
                {
                    Logger.WriteError(string.Concat("SkillLineAbilityDBC could not find a racial ability row for spell ID '", racialAbilityClassGap.WidenedSpellID.ToString(), "' when filling in the missing all-class racial abilities"));
                    continue;
                }

                // All of the versions together have to reach every class, or some race and class pairing has no way to get the racial at all
                int coveredClassMask = 0;
                foreach (int versionSpellID in racialAbilityClassGap.AllVersionSpellIDs)
                    if (classMasksBySpellID.ContainsKey(versionSpellID) == true)
                        coveredClassMask |= classMasksBySpellID[versionSpellID];
                int uncoveredClassMask = ALL_CLASSES_MASK & ~coveredClassMask;
                if (uncoveredClassMask != 0)
                    Logger.WriteError(string.Concat("SkillLineAbilityDBC left racial ability '", racialAbilityClassGap.Name, "' unreachable for the classes in class mask '", uncoveredClassMask.ToString(), "'"));
            }
        }
    }
}
