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

namespace EQWOWConverter.Creatures
{
    internal class CreatureSpawnInstance
    {
        private static Dictionary<int, CreatureSpawnInstance> SpawnInstanceListByID = new Dictionary<int, CreatureSpawnInstance>();

        public int ID = 0;
        public int SpawnGroupID = 0;
        public string ZoneShortName = string.Empty;
        public float SpawnXPosition = 0;
        public float SpawnYPosition = 0;
        public float SpawnZPosition = 0;
        public float Heading = 0;
        public int RespawnTimeInSeconds = 0;
        public int Variance = 0; // Unsure what this is
        public int PathGridID = 0;
        public int RoamRange = 0;

        public static Dictionary<int, CreatureSpawnInstance> GetSpawnInstanceListByID()
        {
            if (SpawnInstanceListByID.Count == 0)
                PopulateSpawnInstanceList();
            return SpawnInstanceListByID;
        }

        private static void PopulateSpawnInstanceList()
        {
            SpawnInstanceListByID.Clear();

            string spawnDetailsFile = Path.Combine(Configuration.CONFIG_PATH_ASSETS_FOLDER, "WorldData", "SpawnInstances.csv");
            Logger.WriteDetail("Populating Spawn Detail list via file '" + spawnDetailsFile + "'");
            string inputData = File.ReadAllText(spawnDetailsFile);
            string[] inputRows = inputData.Split(Environment.NewLine);
            if (inputRows.Length < 2)
            {
                Logger.WriteError("SpawnDetails list via file '" + spawnDetailsFile + "' did not have enough rows");
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
                CreatureSpawnInstance newSpawnDetail = new CreatureSpawnInstance();
                newSpawnDetail.ID = int.Parse(rowBlocks[0]);
                newSpawnDetail.SpawnGroupID = int.Parse(rowBlocks[1]);
                newSpawnDetail.ZoneShortName = rowBlocks[2];
                newSpawnDetail.SpawnXPosition = float.Parse(rowBlocks[3]);
                newSpawnDetail.SpawnYPosition = float.Parse(rowBlocks[4]);
                newSpawnDetail.SpawnZPosition = float.Parse(rowBlocks[5]);
                newSpawnDetail.Heading = float.Parse(rowBlocks[6]);
                newSpawnDetail.RespawnTimeInSeconds = int.Parse(rowBlocks[7]);
                newSpawnDetail.Variance = int.Parse(rowBlocks[8]);
                newSpawnDetail.PathGridID = int.Parse(rowBlocks[9]);
                newSpawnDetail.RoamRange = int.Parse(rowBlocks[10]);

                if (SpawnInstanceListByID.ContainsKey(newSpawnDetail.ID))
                {
                    Logger.WriteError("Spawn Detail list via file '" + spawnDetailsFile + "' has an duplicate row with id '" + newSpawnDetail.ID + "'");
                    continue;
                }
                SpawnInstanceListByID.Add(newSpawnDetail.ID, newSpawnDetail);
            }
        }
    }
}
