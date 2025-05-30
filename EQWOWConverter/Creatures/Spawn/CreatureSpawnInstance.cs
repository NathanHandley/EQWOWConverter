﻿//  Author: Nathan Handley (nathanhandley@protonmail.com)
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
    internal class CreatureSpawnInstance
    {
        private static Dictionary<int, CreatureSpawnInstance> SpawnInstanceListByID = new Dictionary<int, CreatureSpawnInstance>();

        public int ID = 0;
        public int SpawnGroupID = 0;
        public string ZoneShortName = string.Empty;
        public float SpawnXPosition = 0;
        public float SpawnYPosition = 0;
        public float SpawnZPosition = 0;
        public float Orientation = 0;
        public int RespawnTimeInSeconds = 0;
        public int Variance = 0; // TODO: Figure out what this is...
        public int PathGridID = 0;
        public bool SpawnDay;
        public bool SpawnNight;

        public int MapID = 0;
        public int AreaID = 0;
        private List<CreaturePathGridEntry> PathGridEntries = new List<CreaturePathGridEntry>();

        public static Dictionary<int, CreatureSpawnInstance> GetSpawnInstanceListByID()
        {
            if (SpawnInstanceListByID.Count == 0)
                PopulateSpawnInstanceList();
            return SpawnInstanceListByID;
        }

        public void SetPathGridEntries(List<CreaturePathGridEntry> pathGridEntries)
        {
            if (pathGridEntries.Count == 0)
                return;

            // Sort the records first
            pathGridEntries.Sort();

            // Find the nearest grid node
            int nearestPathGridEntryIndex = 0;
            float nearestDistance = 5000f;
            for (int i = 0; i < pathGridEntries.Count; i++)
            {
                CreaturePathGridEntry creaturePathGridEntry = pathGridEntries[i];
                float dx = SpawnXPosition - creaturePathGridEntry.NodeX;
                float dy = SpawnYPosition - creaturePathGridEntry.NodeY;
                float dz = SpawnZPosition - creaturePathGridEntry.NodeZ;
                float calcDistance = MathF.Sqrt(dx * dx + dy * dy + dz * dz);
                if (calcDistance < nearestDistance)
                {
                    nearestDistance = calcDistance;
                    nearestPathGridEntryIndex = i;
                }
            }

            // Build a grid node list starting from the nearest
            if (nearestPathGridEntryIndex == 0)
            {
                foreach (CreaturePathGridEntry creaturePathGridEntry in pathGridEntries)
                    PathGridEntries.Add(new CreaturePathGridEntry(creaturePathGridEntry));
            }
            else
            {
                int curEntryNumber = 1;
                for (int i = nearestPathGridEntryIndex; i < pathGridEntries.Count; ++i)
                {
                    CreaturePathGridEntry creaturePathGridEntry = new CreaturePathGridEntry(pathGridEntries[i]);
                    creaturePathGridEntry.Number = curEntryNumber;
                    curEntryNumber++;
                    PathGridEntries.Add(creaturePathGridEntry);
                }
                for (int i = 0; i < pathGridEntries.Count - PathGridEntries.Count; ++i)
                {
                    CreaturePathGridEntry creaturePathGridEntry = new CreaturePathGridEntry(pathGridEntries[i]);
                    creaturePathGridEntry.Number = curEntryNumber;
                    curEntryNumber++;
                    PathGridEntries.Add(creaturePathGridEntry);
                }
            }
        }

        public List<CreaturePathGridEntry> GetPathGridEntries()
        {
            return PathGridEntries;
        }

        private static void PopulateSpawnInstanceList()
        {
            string spawnDetailsFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "SpawnInstances.csv");
            Logger.WriteDebug("Populating Spawn Detail list via file '" + spawnDetailsFile + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(spawnDetailsFile, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                // Skip any invalid expansion rows
                int minExpansion = int.Parse(columns["min_expansion"]);
                int maxExpansion = int.Parse(columns["max_expansion"]);
                if (minExpansion != -1 && minExpansion > Configuration.GENERATE_EQ_EXPANSION_ID_GENERAL)
                    continue;
                if (maxExpansion != -1 && maxExpansion < Configuration.GENERATE_EQ_EXPANSION_ID_GENERAL)
                    continue;

                // TODO: figure out really where this should be content wise
                string contentFlagsDisabled = columns["content_flags_disabled"];
                if (contentFlagsDisabled.Trim() == "OldPlane_Hate_Sky")
                    continue;

                // Load
                CreatureSpawnInstance newSpawnDetail = new CreatureSpawnInstance();
                newSpawnDetail.ID = int.Parse(columns["eqid"]);
                newSpawnDetail.SpawnGroupID = int.Parse(columns["spawngroupid"]);
                newSpawnDetail.ZoneShortName = columns["zone"];
                float spawnX = float.Parse(columns["x"]);
                float spawnY = float.Parse(columns["y"]);
                float spawnZ = float.Parse(columns["z"]);
                float heading = float.Parse(columns["heading"]);
                newSpawnDetail.RespawnTimeInSeconds = int.Parse(columns["respawntime"]);
                newSpawnDetail.Variance = int.Parse(columns["variance"]);
                newSpawnDetail.PathGridID = int.Parse(columns["pathgrid"]);
                newSpawnDetail.SpawnDay = int.Parse(columns["spawn_day"]) == 1 ? true : false;
                newSpawnDetail.SpawnNight = int.Parse(columns["spawn_night"]) == 1 ? true : false;

                // Get orientation from heading. EQ uses 0-256 range, and can be 2x that (512) and then convert to degrees and then radians
                float modHeading = 0;
                if (heading != 0)
                    modHeading = heading / (256f / 360f);
                newSpawnDetail.Orientation = modHeading * Convert.ToSingle(Math.PI / 180);

                // Modify by world scale
                // IMPORTANT: The X and Y data was swapped in the SpawnInstances.CSV due to orientation differences between EQ and WoW
                newSpawnDetail.SpawnXPosition = spawnX * Configuration.GENERATE_WORLD_SCALE;
                newSpawnDetail.SpawnYPosition = spawnY * Configuration.GENERATE_WORLD_SCALE;
                newSpawnDetail.SpawnZPosition = spawnZ * Configuration.GENERATE_WORLD_SCALE;

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
