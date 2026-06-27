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

namespace EQWOWConverter.Creatures
{
    internal struct CreatureSpellEntry : IComparable<CreatureSpellEntry>
    {
        private static SortedDictionary<int, List<CreatureSpellEntry>> CreatureSpellEntriesByListID = new SortedDictionary<int, List<CreatureSpellEntry>>();
        private static readonly object CreatureSpellEntryLock = new object();
        public int ID;
        public int CreatureSpellListID;
        public int EQSpellID;
        public int TypeFlags;
        public int MinLevel;
        public int MaxLevel;
        public int ManaCost;
        public int OriginalRecastDelayInMS;
        public int CalculatedMinimumDelayInMS;
        public int BuffDurationInMS;
        public int Priority;

        public static SortedDictionary<int, List<CreatureSpellEntry>> GetCreatureSpellEntriesByListID()
        {
            lock (CreatureSpellEntryLock)
            {
                if (CreatureSpellEntriesByListID.Count == 0)
                    PopulateCreatureSpellEntries();
                return CreatureSpellEntriesByListID;
            }
        }

        private static void PopulateCreatureSpellEntries()
        {
            string spellEntriesFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "CreatureSpellEntries.csv");
            Logger.WriteDebug("Populating creature spell entries via file '" + spellEntriesFile + "'");
            List<Dictionary<string, string>> spellEntryRows = FileTool.ReadAllRowsFromFileWithHeader(spellEntriesFile, "|");
            foreach (Dictionary<string, string> columns in spellEntryRows)
            {
                // Skip any invalid rows
                int minExpansion = int.Parse(columns["min_expansion"]);
                if (minExpansion > Configuration.GENERATE_EQ_EXPANSION_ID_GENERAL)
                    continue;

                // Load the row
                CreatureSpellEntry newSpellEntry = new CreatureSpellEntry();
                newSpellEntry.ID = int.Parse(columns["id"]);
                newSpellEntry.CreatureSpellListID = int.Parse(columns["creature_spell_list_id"]);
                newSpellEntry.EQSpellID = int.Parse(columns["eq_spell_id"]);
                newSpellEntry.TypeFlags = int.Parse(columns["type"]);
                newSpellEntry.MinLevel = int.Parse(columns["minlevel"]);
                newSpellEntry.MaxLevel = int.Parse(columns["maxlevel"]);
                newSpellEntry.ManaCost = int.Parse(columns["manacost"]);
                newSpellEntry.OriginalRecastDelayInMS = int.Parse(columns["recast_delay"]) * 1000;
                if (newSpellEntry.OriginalRecastDelayInMS < 0)
                    newSpellEntry.OriginalRecastDelayInMS = 0;
                newSpellEntry.CalculatedMinimumDelayInMS = newSpellEntry.OriginalRecastDelayInMS;
                newSpellEntry.Priority = int.Parse(columns["priority"]);

                // Add it
                if (CreatureSpellEntriesByListID.ContainsKey(newSpellEntry.CreatureSpellListID) == false)
                    CreatureSpellEntriesByListID.Add(newSpellEntry.CreatureSpellListID, new List<CreatureSpellEntry>());
                CreatureSpellEntriesByListID[newSpellEntry.CreatureSpellListID].Add(newSpellEntry);
            }
        }

        // Try to match general cast priority picking as referenced from TAKP mob_ai.cpp AICastSpell
        public static int GetCombatSpellEventChance(int eqSpellTypeFlags, int priority)
        {
            // Priority 0 spells always cast (raid bosses have these)
            if (priority <= 0)
                return 100;

            // Non-nuke spellcast type chance (direct damage spells are always 100%)
            int typeChance = 100;
            if ((eqSpellTypeFlags & 4) == 4) typeChance = Configuration.CREATURE_SPELL_COMBAT_ROOT_CAST_CHANCE;
            else if ((eqSpellTypeFlags & 128) == 128) typeChance = Configuration.CREATURE_SPELL_COMBAT_SNARE_CAST_CHANCE;
            else if ((eqSpellTypeFlags & 256) == 256) typeChance = Configuration.CREATURE_SPELL_COMBAT_DOT_CAST_CHANCE;
            else if ((eqSpellTypeFlags & 512) == 512) typeChance = Configuration.CREATURE_SPELL_COMBAT_DISPEL_CAST_CHANCE;
            else if ((eqSpellTypeFlags & 2048) == 2048) typeChance = Configuration.CREATURE_SPELL_COMBAT_MEZ_CAST_CHANCE;
            else if ((eqSpellTypeFlags & 4096) == 4096) typeChance = Configuration.CREATURE_SPELL_COMBAT_CHARM_CAST_CHANCE;
            else if ((eqSpellTypeFlags & 8192) == 8192) typeChance = Configuration.CREATURE_SPELL_COMBAT_SLOW_CAST_CHANCE;
            else if ((eqSpellTypeFlags & 16384) == 16384) typeChance = Configuration.CREATURE_SPELL_COMBAT_DEBUFF_CAST_CHANCE;
            else if ((eqSpellTypeFlags & 64) == 64) typeChance = Configuration.CREATURE_SPELL_COMBAT_LIFETAP_CAST_CHANCE;

            // Priority-order preference to drop less priority ones off
            int priorityChance = 100;
            if (priority > Configuration.CREATURE_SPELL_COMBAT_PRIORITY_PRIMARY_THRESHOLD)
                priorityChance = Math.Max(Configuration.CREATURE_SPELL_COMBAT_PRIORITY_CHANCE_MIN, 100 - ((priority - Configuration.CREATURE_SPELL_COMBAT_PRIORITY_PRIMARY_THRESHOLD) * Configuration.CREATURE_SPELL_COMBAT_PRIORITY_CHANCE_STEP));
            return Math.Min(typeChance, priorityChance);
        }

        public int CompareTo(CreatureSpellEntry other)
        {
            // Proper way to do this is to sort by "Priority", however doing that will cause much less
            // spell variety in the case that a high priority spell with a faster recast delay will
            // almayst only get used after 1 step-through of the spell list (in WOW).  TODO: Customize
            // later to get better spell sorting.
            //return Priority.CompareTo(other.Priority);
            int minDelayComparison = CalculatedMinimumDelayInMS.CompareTo(other.CalculatedMinimumDelayInMS);
            if (minDelayComparison == 0)
                return Priority.CompareTo(other.Priority);
            else
                return minDelayComparison;
        }
    }
}
