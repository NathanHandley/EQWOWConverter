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
    internal class SpellPetAurasSQL : SQLFile
    {
        private HashSet<(int, int, int)> AddedSpellEffectPets = new HashSet<(int, int, int)>();

        public override string DeleteRowSQL()
        {
            // Stock talents (like Master Demonologist) also get rows here for the generated pets, so rows keyed to a generated pet go too
            return "DELETE FROM `spell_pet_auras` WHERE (`spell` >= " + Configuration.DBCID_SPELL_ID_START.ToString() + " AND `spell` <= " + Configuration.DBCID_SPELL_ID_END + ")"
                + " OR (`pet` >= " + Configuration.SQL_CREATURETEMPLATE_ENTRY_LOW.ToString() + " AND `pet` <= " + Configuration.SQL_CREATURETEMPLATE_ENTRY_HIGH + ");";
        }

        public void AddRow(int ownerSpellID, int ownerSpellEffectIndex, int petCreatureTemplateID, int petAuraSpellID)
        {
            // Multiple spells can summon the same pet creature, so only add each pet once
            if (AddedSpellEffectPets.Add((ownerSpellID, ownerSpellEffectIndex, petCreatureTemplateID)) == false)
                return;

            SQLRow newRow = new SQLRow();
            newRow.AddInt("spell", ownerSpellID);
            newRow.AddInt("effectId", ownerSpellEffectIndex);
            newRow.AddInt("pet", petCreatureTemplateID);
            newRow.AddInt("aura", petAuraSpellID);
            Rows.Add(newRow);
        }

        public void AddMasterDemonologistRowsForPet(int petCreatureTemplateID)
        {
            // Master Demonologist hands each warlock demon its own aura by creature entry, and a generated pet gets the Felguard's (more damage done, less damage taken)
            AddRow(23785, 0, petCreatureTemplateID, 35702); // Rank 1
            AddRow(23822, 0, petCreatureTemplateID, 35703); // Rank 2
            AddRow(23823, 0, petCreatureTemplateID, 35704); // Rank 3
            AddRow(23824, 0, petCreatureTemplateID, 35705); // Rank 4
            AddRow(23825, 0, petCreatureTemplateID, 35706); // Rank 5
        }
    }
}
