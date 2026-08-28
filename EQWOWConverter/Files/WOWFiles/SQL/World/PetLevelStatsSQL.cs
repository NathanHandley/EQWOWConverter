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

using EQWOWConverter.Creatures;
using EQWOWConverter.Spells;

namespace EQWOWConverter.WOWFiles
{
    internal class PetLevelStatsSQL : SQLFile
    {
        private HashSet<int> AddedCreatureTemplateIDs = new HashSet<int>();

        public override string DeleteRowSQL()
        {
            return "DELETE FROM `pet_levelstats` WHERE `creature_entry` >= " + Configuration.SQL_CREATURETEMPLATE_ENTRY_LOW.ToString() + " AND `creature_entry` <= " + Configuration.SQL_CREATURETEMPLATE_ENTRY_HIGH + ";";
        }

        public void AddRowsForCreatureTemplate(CreatureTemplate creatureTemplate)
        {
            // Multiple spells can summon the same pet creature, so only the first pass generates the rows
            if (AddedCreatureTemplateIDs.Contains(creatureTemplate.WOWCreatureTemplateID) == true)
                return;
            if (creatureTemplate.PetTypeName.Length == 0)
            {
                Logger.WriteError("Pet creature template ", creatureTemplate.WOWCreatureTemplateID.ToString(), " (", creatureTemplate.Name, ") has no pet type, so no pet level stats were generated");
                return;
            }
            SortedDictionary<int, SpellPetPowerTierLevelStats>? statsByLevel = SpellPetPowerTierLevelStats.GetStatsByLevelForTypeName(creatureTemplate.PetTypeName);
            if (statsByLevel == null)
                return;
            AddedCreatureTemplateIDs.Add(creatureTemplate.WOWCreatureTemplateID);

            // The tier damage values assume the reference swing time, so rescale them for pets that swing slower or faster
            float damageMod = 1f;
            if (creatureTemplate.AttackTime > 0 && Configuration.CREATURE_PET_POWER_TIER_REFERENCE_ATTACK_TIME_IN_MS > 0)
                damageMod = Convert.ToSingle(creatureTemplate.AttackTime) / Convert.ToSingle(Configuration.CREATURE_PET_POWER_TIER_REFERENCE_ATTACK_TIME_IN_MS);

            foreach (SpellPetPowerTierLevelStats levelStats in statsByLevel.Values)
            {
                if (levelStats.Level > Configuration.CREATURE_PET_POWER_TIER_MAX_LEVEL)
                    continue;
                SQLRow newRow = new SQLRow();
                newRow.AddInt("creature_entry", creatureTemplate.WOWCreatureTemplateID);
                newRow.AddInt("level", levelStats.Level);
                newRow.AddInt("hp", levelStats.Health);
                newRow.AddInt("mana", levelStats.Mana);
                newRow.AddInt("armor", levelStats.Armor);
                newRow.AddInt("str", levelStats.Strength);
                newRow.AddInt("agi", levelStats.Agility);
                newRow.AddInt("sta", levelStats.Stamina);
                newRow.AddInt("inte", levelStats.Intellect);
                newRow.AddInt("spi", levelStats.Spirit);
                newRow.AddInt("min_dmg", Math.Max(1, Convert.ToInt32(MathF.Round(levelStats.MinDamage * damageMod))));
                newRow.AddInt("max_dmg", Math.Max(1, Convert.ToInt32(MathF.Round(levelStats.MaxDamage * damageMod))));
                Rows.Add(newRow);
            }
        }
    }
}
