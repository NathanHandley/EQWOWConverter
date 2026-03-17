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

namespace EQWOWConverter
{
    internal class CreatureSpawnEntry
    {
        private static List<CreatureSpawnEntry> SpawnEntryList = new List<CreatureSpawnEntry>();
        private static readonly object SpawnLock = new object();

        public int SpawnGroupID = 0;
        public int EQCreatureTemplateID = 0;
        public int Chance = 100;

        public static List<CreatureSpawnEntry> GetSpawnEntryList()
        {
            lock (SpawnLock)
            {
                if (SpawnEntryList.Count == 0)
                    PopulateSpawnEntryList();
                return SpawnEntryList;
            }
        }

        private static void PopulateSpawnEntryList()
        {
            SpawnEntryList.Clear();

            string spawnEntriesFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "SpawnEntries.csv");
            Logger.WriteDebug("Populating Spawn Entry list via file '" + spawnEntriesFile + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(spawnEntriesFile, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                // Skip any invalid expansion rows
                if (Configuration.CREATURE_SPAWN_AND_WAYPOINT_DEBUG_MODE == false)
                {
                    int minExpansion = int.Parse(columns["min_expansion"]);
                    int maxExpansion = int.Parse(columns["max_expansion"]);
                    if (minExpansion != -1 && minExpansion > Configuration.GENERATE_EQ_EXPANSION_ID_GENERAL)
                        continue;
                    if (maxExpansion != -1 && maxExpansion < Configuration.GENERATE_EQ_EXPANSION_ID_GENERAL)
                        continue;
                }

                CreatureSpawnEntry newSpawnEntry = new CreatureSpawnEntry();
                newSpawnEntry.SpawnGroupID = int.Parse(columns["spawngroupID"]);
                newSpawnEntry.EQCreatureTemplateID = int.Parse(columns["npcID"]);
                newSpawnEntry.Chance = int.Parse(columns["chance"]);
                SpawnEntryList.Add(newSpawnEntry);
            }
        }
    }
}
