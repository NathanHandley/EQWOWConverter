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
    internal class CreatureSpawnGroup
    {
        private static Dictionary<int, CreatureSpawnGroup> SpawnGroupsByGroupID = new Dictionary<int, CreatureSpawnGroup>();

        public int ID = 0;
        public string Name = string.Empty;
        public int SpawnLimit = 0;
        public float RoamDistance = 0;

        public static Dictionary<int, CreatureSpawnGroup> GetSpawnGroupsByGroupID()
        {
            if (SpawnGroupsByGroupID.Count == 0)
                PopulateSpawnGroupList();
            return SpawnGroupsByGroupID;
        }

        private static void PopulateSpawnGroupList()
        {
            string spawnGroupsFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "SpawnGroups.csv");
            Logger.WriteDebug("Populating Spawn Group list via file '" + spawnGroupsFile + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(spawnGroupsFile, "|");
            foreach (Dictionary<string, string> columns in rows)
            { 
                // Load the row
                CreatureSpawnGroup newSpawnGroup = new CreatureSpawnGroup();
                newSpawnGroup.ID = Convert.ToInt32(columns["id"]);
                newSpawnGroup.Name = columns["name"];
                newSpawnGroup.SpawnLimit = Convert.ToInt32(columns["spawn_limit"]);

                // Calculate the wander distance
                float maxX = float.Parse(columns["max_x"]);
                float minX = float.Parse(columns["min_x"]);
                float maxY = float.Parse(columns["max_y"]);
                float minY = float.Parse(columns["min_y"]);
                float xDistance = maxX - minX;
                float yDistance = maxY - minY;
                float minDistance = (MathF.Min(xDistance, yDistance) / 2) * Configuration.GENERATE_WORLD_SCALE;
                if (minDistance > 1)
                    newSpawnGroup.RoamDistance = minDistance;

                SpawnGroupsByGroupID.Add(newSpawnGroup.ID, newSpawnGroup);
            }
        }
    }
}
