//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2024 Nathan Handley
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter
{
    internal class CreatureSpawnEntry
    {
        private static List<CreatureSpawnEntry> SpawnEntryList = new List<CreatureSpawnEntry>();

        public int SpawnGroupID = 0;
        public int CreatureTemplateID = 0;
        public int Chance = 100;

        public static List<CreatureSpawnEntry> GetSpawnEntryList()
        {
            if (SpawnEntryList.Count == 0)
                PopulateSpawnEntryList();
            return SpawnEntryList;
        }

        private static void PopulateSpawnEntryList()
        {
            SpawnEntryList.Clear();

            string spawnEntriesFile = Path.Combine(Configuration.CONFIG_PATH_ASSETS_FOLDER, "WorldData", "SpawnEntries.csv");
            Logger.WriteDetail("Populating Spawn Entry list via file '" + spawnEntriesFile + "'");
            string inputData = File.ReadAllText(spawnEntriesFile);
            string[] inputRows = inputData.Split(Environment.NewLine);
            if (inputRows.Length < 2)
            {
                Logger.WriteError("SpawnEntry list via file '" + spawnEntriesFile + "' did not have enough rows");
                return;
            }

            // Load all of the data
            bool headerRow = true;
            foreach (string row in inputRows)
            {
                // Handle first row
                if (headerRow == true)
                {
                    headerRow = false;
                    continue;
                }

                // Skip blank rows
                if (row.Trim().Length == 0)
                    continue;

                // Load the row
                string[] rowBlocks = row.Split(",");
                CreatureSpawnEntry newSpawnEntry = new CreatureSpawnEntry();
                newSpawnEntry.SpawnGroupID = int.Parse(rowBlocks[0]);
                newSpawnEntry.CreatureTemplateID = int.Parse(rowBlocks[1]);
                newSpawnEntry.Chance = int.Parse(rowBlocks[2]);

                // Skip any invalid expansion rows
                int minExpansion = int.Parse(rowBlocks[5]);
                int maxExpansion = int.Parse(rowBlocks[6]);
                if (minExpansion != -1 && minExpansion > Configuration.CONFIG_GENERATE_EQ_EXPANSION_ID)
                    continue;
                if (maxExpansion != -1 && maxExpansion < Configuration.CONFIG_GENERATE_EQ_EXPANSION_ID)
                    continue;

                SpawnEntryList.Add(newSpawnEntry);
            }
        }
    }
}
