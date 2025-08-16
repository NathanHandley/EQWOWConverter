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

namespace EQWOWConverter.WOWFiles
{
    internal class SpellItemEnchantmentDBC : DBCFile
    {
        private static int CURRENT_ID = Configuration.DBCID_SPELLITEMENCHANTMENT_ID_START;

        public void AddRowForRogueWeaponProc(int id, int spellID, int procRate, string name)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(id); // ID
            newRow.AddInt32(0); // Charges
            newRow.AddInt32(1); // Effect1 (Comes from SpellDispelType.dbc, and 1 means MAGIC)
            newRow.AddInt32(0); // Effect2
            newRow.AddInt32(0); // Effect3
            newRow.AddInt32(procRate); // EffectPointsMin1
            newRow.AddInt32(0); // EffectPointsMin2
            newRow.AddInt32(0); // EffectPointsMin3
            newRow.AddInt32(procRate); // EffectPointsMax1
            newRow.AddInt32(0); // EffectPointsMax2
            newRow.AddInt32(0); // EffectPointsMax3
            newRow.AddInt32(spellID); // EffectArg1
            newRow.AddInt32(0); // EffectArg2
            newRow.AddInt32(0); // EffectArg3
            newRow.AddStringLang(name); // Name
            newRow.AddInt32(Configuration.SPELLS_ENCHANT_ROGUE_POISON_ENCHANT_EFFECT_VISUAL_ID); // ItemVisual (Matches other rogue poisons, probably good to give it the visual)
            newRow.AddInt32(9); // Flags (Matches other rogue poisons, so not 100% sure what it means)
            newRow.AddInt32(0); // Src_ItemID
            newRow.AddInt32(0); // Condition_Id
            newRow.AddInt32(0); // RequiredSkillID
            newRow.AddInt32(0); // RequiredSkillRank
            newRow.AddInt32(0); // MinLevel
            Rows.Add(newRow);
        }

        public static int GenerateUniqueID()
        {
            int uniqueID = CURRENT_ID;
            CURRENT_ID++;
            return uniqueID;
        }
    }
}
