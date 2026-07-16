//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2026 Nathan Handley
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
    internal class CreatureKillSpawn
    {
        private static List<CreatureKillSpawn> KillSpawnList = new List<CreatureKillSpawn>();
        private static HashSet<int> SpawnedTargetEQCreatureTemplateIDs = new HashSet<int>();
        private static readonly object KillSpawnLock = new object();

        public int ID;
        public string ZoneShortName = string.Empty;
        public int TriggerEQCreatureTemplateID;
        public CreatureKillSpawnTriggerType TriggerType = CreatureKillSpawnTriggerType.Death;
        public CreatureKillSpawnActionType ActionType = CreatureKillSpawnActionType.Spawn;
        public int TargetEQCreatureTemplateID;
        public float Chance = 100;
        public int AltGroup;
        public int AltID;
        public float AltWeight;
        public bool SpawnAtCorpse;
        public float XPosition;
        public float YPosition;
        public float ZPosition;
        public float Orientation;
        public int DelayMinMS;
        public int DelayMaxMS;
        public int OnlyIfNotAliveEQCreatureTemplateID;
        public List<int> RequireDeadEQCreatureTemplateIDs = new List<int>();
        public List<int> RequireAliveEQCreatureTemplateIDs = new List<int>();
        public bool AddToHateList;
        public int TriggerMinLevel;
        public int TriggerMaxLevel;
        public int RespawnTimeInSec;
        public string Comment = string.Empty;

        public static List<CreatureKillSpawn> GetKillSpawnList()
        {
            lock (KillSpawnLock)
            {
                if (KillSpawnList.Count == 0)
                    PopulateKillSpawnList();
                return KillSpawnList;
            }
        }

        // Targets of kill-triggered spawns are valid creatures even without static spawn zones
        public static bool IsKillSpawnTarget(int eqCreatureTemplateID)
        {
            lock (KillSpawnLock)
            {
                if (KillSpawnList.Count == 0)
                    PopulateKillSpawnList();
                return SpawnedTargetEQCreatureTemplateIDs.Contains(eqCreatureTemplateID);
            }
        }

        private static void PopulateKillSpawnList()
        {
            string killSpawnsFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "CreatureKillSpawns.csv");
            Logger.WriteDebug("Populating Creature Kill Spawn list via file '" + killSpawnsFile + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(killSpawnsFile, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                // Skip any invalid expansion rows
                int minExpansion = int.Parse(columns["min_expansion"]);
                int maxExpansion = int.Parse(columns["max_expansion"]);
                if (minExpansion != -1 && minExpansion > Configuration.GENERATE_EQ_EXPANSION_ID_GENERAL)
                    continue;
                if (maxExpansion != -1 && maxExpansion < Configuration.GENERATE_EQ_EXPANSION_ID_GENERAL)
                    continue;

                CreatureKillSpawn newKillSpawn = new CreatureKillSpawn();
                newKillSpawn.ID = int.Parse(columns["id"]);
                newKillSpawn.ZoneShortName = columns["zone"].ToLower().Trim();
                newKillSpawn.TriggerEQCreatureTemplateID = int.Parse(columns["trigger_eq_npc_id"]);
                switch (columns["trigger_event"].ToLower().Trim())
                {
                    case "death": newKillSpawn.TriggerType = CreatureKillSpawnTriggerType.Death; break;
                    case "combat": newKillSpawn.TriggerType = CreatureKillSpawnTriggerType.Combat; break;
                    case "evade": newKillSpawn.TriggerType = CreatureKillSpawnTriggerType.Evade; break;
                    case "ooctimer": newKillSpawn.TriggerType = CreatureKillSpawnTriggerType.OutOfCombatTimer; break;
                    default:
                        {
                            Logger.WriteError("CreatureKillSpawn row '" + newKillSpawn.ID + "' has unknown trigger_event '" + columns["trigger_event"] + "'");
                            continue;
                        }
                }
                switch (columns["action"].ToLower().Trim())
                {
                    case "spawn": newKillSpawn.ActionType = CreatureKillSpawnActionType.Spawn; break;
                    case "despawn": newKillSpawn.ActionType = CreatureKillSpawnActionType.Despawn; break;
                    case "respawnself": newKillSpawn.ActionType = CreatureKillSpawnActionType.RespawnSelf; break;
                    case "respawntarget": newKillSpawn.ActionType = CreatureKillSpawnActionType.RespawnTarget; break;
                    default:
                        {
                            Logger.WriteError("CreatureKillSpawn row '" + newKillSpawn.ID + "' has unknown action '" + columns["action"] + "'");
                            continue;
                        }
                }
                newKillSpawn.TargetEQCreatureTemplateID = int.Parse(columns["target_eq_npc_id"]);
                newKillSpawn.Chance = float.Parse(columns["chance"]);
                newKillSpawn.AltGroup = int.Parse(columns["alt_group"]);
                newKillSpawn.AltID = int.Parse(columns["alt_id"]);
                newKillSpawn.AltWeight = float.Parse(columns["alt_weight"]);
                newKillSpawn.SpawnAtCorpse = columns["at_corpse"].Trim() == "1";

                // X and Y were pre-swapped in the CSV due to orientation differences between EQ and WoW
                newKillSpawn.XPosition = float.Parse(columns["x"]) * Configuration.GENERATE_WORLD_SCALE;
                newKillSpawn.YPosition = float.Parse(columns["y"]) * Configuration.GENERATE_WORLD_SCALE;
                newKillSpawn.ZPosition = float.Parse(columns["z"]) * Configuration.GENERATE_WORLD_SCALE;

                // Get orientation from heading. EQ uses 0-256 range, and can be 2x that (512) and then convert to degrees and then radians
                float heading = float.Parse(columns["heading"]);
                float modHeading = 0;
                if (heading != 0)
                    modHeading = heading / (256f / 360f);
                newKillSpawn.Orientation = modHeading * Convert.ToSingle(Math.PI / 180);

                newKillSpawn.DelayMinMS = int.Parse(columns["delay_min_ms"]);
                newKillSpawn.DelayMaxMS = int.Parse(columns["delay_max_ms"]);
                newKillSpawn.OnlyIfNotAliveEQCreatureTemplateID = int.Parse(columns["only_if_not_alive"]);
                foreach (string idString in columns["require_dead_eq_npc_ids"].Split(',', StringSplitOptions.RemoveEmptyEntries))
                    newKillSpawn.RequireDeadEQCreatureTemplateIDs.Add(int.Parse(idString));
                foreach (string idString in columns["require_alive_eq_npc_ids"].Split(',', StringSplitOptions.RemoveEmptyEntries))
                    newKillSpawn.RequireAliveEQCreatureTemplateIDs.Add(int.Parse(idString));
                newKillSpawn.AddToHateList = columns["add_to_hate"].Trim() == "1";
                newKillSpawn.TriggerMinLevel = int.Parse(columns["trigger_min_level"]);
                newKillSpawn.TriggerMaxLevel = int.Parse(columns["trigger_max_level"]);

                // A respawn time of -1 means to use the raid boss respawn window
                // TODO: Have a lookup for boss creatures so it uses the right respawn max time (could be raid trash)
                newKillSpawn.RespawnTimeInSec = int.Parse(columns["respawn_time_sec"]);
                if (newKillSpawn.RespawnTimeInSec == -1)
                    newKillSpawn.RespawnTimeInSec = Configuration.CREATURE_RAID_BOSS_RESPAWN_MAX_TIME_IN_SEC;
                newKillSpawn.Comment = columns["comment"];
                KillSpawnList.Add(newKillSpawn);

                if (newKillSpawn.ActionType == CreatureKillSpawnActionType.Spawn && newKillSpawn.TargetEQCreatureTemplateID > 0)
                    SpawnedTargetEQCreatureTemplateIDs.Add(newKillSpawn.TargetEQCreatureTemplateID);
            }
        }
    }
}
