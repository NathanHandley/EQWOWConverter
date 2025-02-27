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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Creatures
{
    internal class CreatureSpawnGroup
    {
        private static Dictionary<int, CreatureSpawnGroup> SpawnGroupsByGroupID = new Dictionary<int, CreatureSpawnGroup>();

        public int ID = 0;
        public string Name = string.Empty;
        public int SpawnLimit = 0;

        public static Dictionary<int, CreatureSpawnGroup> GetSpawnGroupsByGroupID()
        {
            if (SpawnGroupsByGroupID.Count == 0)
                PopulateSpawnGroupList();
            return SpawnGroupsByGroupID;
        }

        private static void PopulateSpawnGroupList()
        {
            SpawnGroupsByGroupID.Clear();

            string spawnGroupsFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "SpawnGroups.csv");
            Logger.WriteDetail("Populating Spawn Group list via file '" + spawnGroupsFile + "'");
            string inputData = FileTool.ReadAllDataFromFile(spawnGroupsFile);
            string[] inputRows = inputData.Split(Environment.NewLine);
            if (inputRows.Length < 2)
            {
                Logger.WriteError("SpawnGroups list via file '" + spawnGroupsFile + "' did not have enough rows");
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
                string[] rowBlocks = row.Split("|");
                CreatureSpawnGroup newSpawnGroup = new CreatureSpawnGroup();
                newSpawnGroup.ID = Convert.ToInt32(rowBlocks[0]);
                newSpawnGroup.Name = rowBlocks[1];
                newSpawnGroup.SpawnLimit = Convert.ToInt32(rowBlocks[2]);
                SpawnGroupsByGroupID.Add(newSpawnGroup.ID, newSpawnGroup);
            }
        }
    }
}
