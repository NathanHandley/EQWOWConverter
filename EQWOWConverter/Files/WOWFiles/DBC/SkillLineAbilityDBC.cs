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

        public void AddRow(int id, SpellTemplate spellTemplate)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(id); // ID
            newRow.AddInt32(spellTemplate.SkillLine); // SkillLine
            newRow.AddInt32(spellTemplate.WOWSpellID); // Spell
            newRow.AddInt32(0); // RaceMask
            newRow.AddInt32(0); // ClassMask
            newRow.AddInt32(0); // ExcludeRace
            newRow.AddInt32(0); // ExcludeClass
            if (spellTemplate.TradeskillRecipe == null)
            {
                newRow.AddInt32(1); // MinSkillLineRank
                newRow.AddInt32(0); // SupercededBySpell
                newRow.AddInt32(0); // AcquireMethod (0 = learn by trainer)
                newRow.AddInt32(0); // TrivialSkillLineRankHigh
                newRow.AddInt32(0); // TrivialSkillLineRankLow
            }
            else
            {
                newRow.AddInt32(spellTemplate.TradeskillRecipe.SkillRankNeededWOW); // MinSkillLineRank
                newRow.AddInt32(0); // SupercededBySpell
                newRow.AddInt32(0); // AcquireMethod (0 = learn by trainer)
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

            // Update any skill references
            if (Configuration.PLAYER_SKILL_ENABLE_ALIGNED_ARMOR_TYPE_ON_ALL_CLASSES == true)
            {
                List<ClassWOWType> leatherClasses = PlayerClassMapping.GetWOWClassesEligibleForArmorType(ItemWOWArmorSubclassType.Leather).ToList();
                List<ClassWOWType> mailClasses = PlayerClassMapping.GetWOWClassesEligibleForArmorType(ItemWOWArmorSubclassType.Mail).ToList();
                List<ClassWOWType> plateClasses = PlayerClassMapping.GetWOWClassesEligibleForArmorType(ItemWOWArmorSubclassType.Plate).ToList();
                foreach (DBCRow row in Rows)
                {
                    DBCRow.DBCFieldInt32 skillLineField = (DBCRow.DBCFieldInt32)row.AddedFields[1];
                    DBCRow.DBCFieldInt32 classMaskField = (DBCRow.DBCFieldInt32)row.AddedFields[4];
                    switch (skillLineField.Value)
                    {
                        case 414: classMaskField = CalculateClassMask(leatherClasses); break; // Leather
                        case 413: classMaskField = CalculateClassMask(mailClasses); break; // Mail
                        case 293: classMaskField = CalculateClassMask(plateClasses); break; // Plate
                        default: break;
                    }
                }
            }
            if (Configuration.PLAYER_SKILL_ENABLE_SHIELDS_ON_ALL_CLASSES == true)
            {
                foreach (DBCRow row in Rows)
                {
                    DBCRow.DBCFieldInt32 skillLineField = (DBCRow.DBCFieldInt32)row.AddedFields[1];
                    if (skillLineField.Value == 433)
                        row.AddedFields[4] = new DBCRow.DBCFieldInt32(-1); // Set class mask to "all"
                }
            }
            if (Configuration.PLAYER_SKILL_ENABLE_ALIGNED_MELEE_WEAPON_SKILLS_ON_ALL_CLASSES == true)
            {
                List<ClassWOWType> axeOneHandClasses = PlayerClassMapping.GetWOWClassesEligibleForWeaponSubClass(ItemWOWWeaponSubclassType.AxeOneHand).ToList();
                List<ClassWOWType> axeTwoHandClasses = PlayerClassMapping.GetWOWClassesEligibleForWeaponSubClass(ItemWOWWeaponSubclassType.AxeTwoHand).ToList();
                List<ClassWOWType> maceOneHandClasses = PlayerClassMapping.GetWOWClassesEligibleForWeaponSubClass(ItemWOWWeaponSubclassType.MaceOneHand).ToList();
                List<ClassWOWType> maceTwoHandClasses = PlayerClassMapping.GetWOWClassesEligibleForWeaponSubClass(ItemWOWWeaponSubclassType.MaceTwoHand).ToList();
                List<ClassWOWType> polearmClasses = PlayerClassMapping.GetWOWClassesEligibleForWeaponSubClass(ItemWOWWeaponSubclassType.Polearm).ToList();
                List<ClassWOWType> swordOneHandClasses = PlayerClassMapping.GetWOWClassesEligibleForWeaponSubClass(ItemWOWWeaponSubclassType.SwordOneHand).ToList();
                List<ClassWOWType> swordTwoHandClasses = PlayerClassMapping.GetWOWClassesEligibleForWeaponSubClass(ItemWOWWeaponSubclassType.SwordTwoHand).ToList();
                List<ClassWOWType> staffClasses = PlayerClassMapping.GetWOWClassesEligibleForWeaponSubClass(ItemWOWWeaponSubclassType.Staff).ToList();
                List<ClassWOWType> fistWeaponClasses = PlayerClassMapping.GetWOWClassesEligibleForWeaponSubClass(ItemWOWWeaponSubclassType.FistWeapon).ToList();
                List<ClassWOWType> daggerClasses = PlayerClassMapping.GetWOWClassesEligibleForWeaponSubClass(ItemWOWWeaponSubclassType.Dagger).ToList();
                foreach (DBCRow row in Rows)
                {
                    DBCRow.DBCFieldInt32 skillLineField = (DBCRow.DBCFieldInt32)row.AddedFields[1];
                    DBCRow.DBCFieldInt32 classMaskField = (DBCRow.DBCFieldInt32)row.AddedFields[4];
                    switch (skillLineField.Value)
                    {
                        case 44: classMaskField = CalculateClassMask(axeOneHandClasses); break; // Axes
                        case 172: classMaskField = CalculateClassMask(axeTwoHandClasses); break; // Two-Handed Axes
                        case 54: classMaskField = CalculateClassMask(maceOneHandClasses); break; // Maces
                        case 160: classMaskField = CalculateClassMask(maceTwoHandClasses); break; // Two-Handed Maces
                        case 229: classMaskField = CalculateClassMask(polearmClasses); break; // Polearms
                        case 43: classMaskField = CalculateClassMask(swordOneHandClasses); break; // Swords
                        case 55: classMaskField = CalculateClassMask(swordTwoHandClasses); break; // Two-Handed Swords
                        case 136: classMaskField = CalculateClassMask(staffClasses); break; // Staves
                        case 473: classMaskField = CalculateClassMask(fistWeaponClasses); break; // Fist Weapons
                        case 173: classMaskField = CalculateClassMask(daggerClasses); break; // Daggers
                        default: break;
                    }
                }
            }
        }

        private DBCRow.DBCFieldInt32 CalculateClassMask(List<ClassWOWType> allowedClassTypes)
        {
            int allowableClass = 0;
            foreach (ClassWOWType classType in allowedClassTypes)
            {
                if (classType == ClassWOWType.All)
                    return new DBCRow.DBCFieldInt32(-1);
                allowableClass += Convert.ToInt32(Math.Pow(2, Convert.ToInt32(classType) - 1));
            }
            if (allowableClass == 0)
                allowableClass = -1;
            return new DBCRow.DBCFieldInt32(allowableClass);
        }
    }
}
