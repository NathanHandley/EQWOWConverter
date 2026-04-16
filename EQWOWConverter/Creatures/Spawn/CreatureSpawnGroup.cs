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

namespace EQWOWConverter.Creatures
{
    internal class CreatureSpawnGroup
    {
        private static Dictionary<int, CreatureSpawnGroup> SpawnGroupsByGroupID = new Dictionary<int, CreatureSpawnGroup>();

        public int ID = 0;
        public string Name = string.Empty;
        public int SpawnLimit = 0;
        public float RoamMinX = 0;
        public float RoamMaxX = 0;
        public float RoamMinY = 0;
        public float RoamMaxY = 0;
        public int RoamMaxDelayInMS = 0;
        public int RoamMinDelayInMS = 0;
        public int SpawnZoneEventID = 0;
        public int DespawnZoneEventID = 0;
        public string ZoneShortName = string.Empty;

        public bool DoesRoam()
        {
            if (RoamMinX != 0 || RoamMaxX != 0 || RoamMinY != 0 || RoamMaxY != 0)
                return true;
            return false;
        }

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
                newSpawnGroup.ZoneShortName = columns["zone"];
                newSpawnGroup.Name = columns["name"];
                newSpawnGroup.SpawnLimit = Convert.ToInt32(columns["spawn_limit"]);
                newSpawnGroup.SpawnZoneEventID = int.Parse(columns["spawn_zone_event_id"]);
                newSpawnGroup.DespawnZoneEventID = int.Parse(columns["despawn_zone_event_id"]);

                // Roam data
                newSpawnGroup.RoamMaxX = float.Parse(columns["max_x"]) * Configuration.GENERATE_WORLD_SCALE;
                newSpawnGroup.RoamMinX = float.Parse(columns["min_x"]) * Configuration.GENERATE_WORLD_SCALE;
                newSpawnGroup.RoamMaxY = float.Parse(columns["max_y"]) * Configuration.GENERATE_WORLD_SCALE;
                newSpawnGroup.RoamMinY = float.Parse(columns["min_y"]) * Configuration.GENERATE_WORLD_SCALE;
                newSpawnGroup.RoamMaxDelayInMS = int.Parse(columns["delay"]);
                newSpawnGroup.RoamMinDelayInMS = int.Parse(columns["mindelay"]);
                if (newSpawnGroup.RoamMaxDelayInMS < newSpawnGroup.RoamMinDelayInMS)
                    newSpawnGroup.RoamMaxDelayInMS = newSpawnGroup.RoamMinDelayInMS;

                SpawnGroupsByGroupID.Add(newSpawnGroup.ID, newSpawnGroup);
            }
        }
    }
}
