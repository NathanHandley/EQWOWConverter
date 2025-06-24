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
    internal class CreatureSpellList
    {
        private static List<CreatureSpellList> CreatureSpellLists = new List<CreatureSpellList>();
        private static readonly object CreatureSpellListLock = new object();
        public int ID;
        public string Name = string.Empty;
        public int ParentListID;
        public int AttackProcID;
        public int ProcChance;

        public static List<CreatureSpellList> GetCreatureSpellLists()
        {
            lock (CreatureSpellListLock)
            {
                if (CreatureSpellLists.Count == 0)
                    PopulateCreatureSpellLists();
                return CreatureSpellLists;
            }
        }

        private static void PopulateCreatureSpellLists()
        {
            string spellListsFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "CreatureSpellLists.csv");
            Logger.WriteDebug("Populating creature spell lists via file '" + spellListsFile + "'");
            List<Dictionary<string, string>> spellListsRows = FileTool.ReadAllRowsFromFileWithHeader(spellListsFile, "|");
            foreach (Dictionary<string, string> columns in spellListsRows)
            {
                // Load the row
                CreatureSpellList newList = new CreatureSpellList();
                newList.ID = int.Parse(columns["id"]);
                newList.Name = columns["name"];
                newList.ParentListID = int.Parse(columns["parent_list"]);
                newList.AttackProcID = int.Parse(columns["attack_proc"]);
                newList.ProcChance = int.Parse(columns["proc_chance"]);
                CreatureSpellLists.Add(newList);
            }
        }
    }
}
