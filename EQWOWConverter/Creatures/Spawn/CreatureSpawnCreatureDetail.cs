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
    internal class CreatureSpawnCreatureDetail
    {
        private static Dictionary<int, CreatureSpawnCreatureDetail> SpawnCreatureDetailList = new Dictionary<int, CreatureSpawnCreatureDetail>();

        public int ID = 0;
        public string Name = string.Empty; // Restrict to 100 characters
        public string SubName = string.Empty; // Restrict to 100 characters
        public int Level = 1;
        public int RaceID = 0;
        public int Class = 1;
        public int BodyType = 24; // This is common for the body type
        public int HP = 1;
        public CreatureGenderType GenderType = CreatureGenderType.Neutral;
        public int TextureID = 0;
        public int HelmTextureID = 0;

        public static Dictionary<int, CreatureSpawnCreatureDetail> GetSpawnCreatureDetailList()
        {
            if (SpawnCreatureDetailList.Count == 0)
                PopulateSpawnCreatureDetailsList();
            return SpawnCreatureDetailList;
        }

        private static void PopulateSpawnCreatureDetailsList()
        {
            SpawnCreatureDetailList.Clear();

            string spawnCreatureDetailsFile = Path.Combine(Configuration.CONFIG_PATH_ASSETS_FOLDER, "WorldData", "SpawnCreatureDetails.csv");
            Logger.WriteDetail("Populating Spawn Creature Detail list via file '" + spawnCreatureDetailsFile + "'");
            string inputData = File.ReadAllText(spawnCreatureDetailsFile);
            string[] inputRows = inputData.Split(Environment.NewLine);
            if (inputRows.Length < 2)
            {
                Logger.WriteError("SpawnCreatureDetails list via file '" + spawnCreatureDetailsFile + "' did not have enough rows");
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
                CreatureSpawnCreatureDetail newSpawnCreatureDetail = new CreatureSpawnCreatureDetail();
                newSpawnCreatureDetail.ID = int.Parse(rowBlocks[0]);
                newSpawnCreatureDetail.Name = rowBlocks[1];
                newSpawnCreatureDetail.SubName = rowBlocks[2];
                newSpawnCreatureDetail.Level = int.Parse(rowBlocks[3]);
                newSpawnCreatureDetail.RaceID = int.Parse(rowBlocks[4]);
                newSpawnCreatureDetail.Class = int.Parse(rowBlocks[5]);
                newSpawnCreatureDetail.BodyType = int.Parse(rowBlocks[6]);
                newSpawnCreatureDetail.HP = int.Parse(rowBlocks[7]);
                int genderID = int.Parse(rowBlocks[8]);
                switch (genderID)
                {
                    case 0: newSpawnCreatureDetail.GenderType = CreatureGenderType.Male; break;
                    case 1: newSpawnCreatureDetail.GenderType = CreatureGenderType.Female; break;
                    default: newSpawnCreatureDetail.GenderType = CreatureGenderType.Neutral; break;
                }
                newSpawnCreatureDetail.TextureID = int.Parse(rowBlocks[9]);
                newSpawnCreatureDetail.HelmTextureID = int.Parse(rowBlocks[10]);

                if (SpawnCreatureDetailList.ContainsKey(newSpawnCreatureDetail.ID))
                {
                    Logger.WriteError("Spawn Creature Detail list via file '" + spawnCreatureDetailsFile + "' has an duplicate row with id '" + newSpawnCreatureDetail.ID + "'");
                    continue;
                }
                SpawnCreatureDetailList.Add(newSpawnCreatureDetail.ID, newSpawnCreatureDetail);
            }
        }
    }
}
