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
        private static Dictionary<int, List<CreatureSpellEntry>> CreatureSpellEntriesByListID = new Dictionary<int, List<CreatureSpellEntry>>();
        private static readonly object CreatureSpellEntryLock = new object();
        public int ID;
        public int CreatureSpellListID;
        public int EQSpellID;
        public int TypeID;
        public int MinLevel;
        public int MaxLevel;
        public int ManaCost;
        public int OriginalRecastDelayInMS;
        public int CalculatedMinimumRecastDelayInMS;
        public int Priority;

        public static Dictionary<int, List<CreatureSpellEntry>> GetCreatureSpellEntriesByListID()
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
                newSpellEntry.TypeID = int.Parse(columns["type"]);
                newSpellEntry.MinLevel = int.Parse(columns["minlevel"]);
                newSpellEntry.MaxLevel = int.Parse(columns["maxlevel"]);
                newSpellEntry.ManaCost = int.Parse(columns["manacost"]);
                newSpellEntry.OriginalRecastDelayInMS = int.Parse(columns["recast_delay"]) * 1000;
                if (newSpellEntry.OriginalRecastDelayInMS < 0)
                    newSpellEntry.OriginalRecastDelayInMS = 0;
                newSpellEntry.CalculatedMinimumRecastDelayInMS = newSpellEntry.OriginalRecastDelayInMS;
                newSpellEntry.Priority = int.Parse(columns["priority"]);

                // Add it
                if (CreatureSpellEntriesByListID.ContainsKey(newSpellEntry.CreatureSpellListID) == false)
                    CreatureSpellEntriesByListID.Add(newSpellEntry.CreatureSpellListID, new List<CreatureSpellEntry>());
                CreatureSpellEntriesByListID[newSpellEntry.CreatureSpellListID].Add(newSpellEntry);
            }
        }

        public bool DoCastInCombat()
        {
            return true;
        }

        public int CompareTo(CreatureSpellEntry other)
        {
            // Proper way to do this is to sort by "Priority", however doing that will cause much less
            // spell variety in the case that a high priority spell with a faster recast delay will
            // almayst only get used after 1 step-through of the spell list (in WOW).  TODO: Customize
            // later to get better spell sorting.
            //return Priority.CompareTo(other.Priority);
            return CalculatedMinimumRecastDelayInMS.CompareTo(other.CalculatedMinimumRecastDelayInMS);
        }
    }
}
