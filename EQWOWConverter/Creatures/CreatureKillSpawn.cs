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
        private static Dictionary<string, HashSet<int>> HailTriggerEQCreatureTemplateIDsByZoneShortName = new Dictionary<string, HashSet<int>>();
        private static readonly object KillSpawnLock = new object();

        public const int MAX_REQUIRE_DEAD_CREATURE_TEMPLATES = 3;

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
        public bool RespawnUsesRaidLockout; // The raid boss respawn window, which inside a raid instance becomes the instance lockout
        public float TriggerRadius;
        public int SpellEQID;
        public int PathGridID = -1;
        public int PathGridStartNode = -1;
        public int PathGridEndNode = -1;
        public CreatureKillSpawnArrivalBehaviorType ArrivalBehavior = CreatureKillSpawnArrivalBehaviorType.None;
        public bool IsPersistent; // The spawn stands in for a real spawn point, so it never times out
        public string Text = string.Empty;
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

        // Creatures that react to being hailed need to be talkable, the same as ones with a gossip menu
        public static Dictionary<string, HashSet<int>> GetHailTriggerEQCreatureTemplateIDsByZoneShortName()
        {
            lock (KillSpawnLock)
            {
                if (KillSpawnList.Count == 0)
                    PopulateKillSpawnList();
                return HailTriggerEQCreatureTemplateIDsByZoneShortName;
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
                    case "spawn": newKillSpawn.TriggerType = CreatureKillSpawnTriggerType.Spawn; break;
                    case "proximity": newKillSpawn.TriggerType = CreatureKillSpawnTriggerType.Proximity; break;
                    case "hail": newKillSpawn.TriggerType = CreatureKillSpawnTriggerType.Hail; break;
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
                    case "say": newKillSpawn.ActionType = CreatureKillSpawnActionType.Say; break;
                    case "emote": newKillSpawn.ActionType = CreatureKillSpawnActionType.Emote; break;
                    case "yell": newKillSpawn.ActionType = CreatureKillSpawnActionType.Yell; break;
                    case "attackplayer": newKillSpawn.ActionType = CreatureKillSpawnActionType.AttackPlayer; break;
                    case "walkgrid": newKillSpawn.ActionType = CreatureKillSpawnActionType.WalkGrid; break;
                    case "castspell": newKillSpawn.ActionType = CreatureKillSpawnActionType.CastSpell; break;
                    case "moveto": newKillSpawn.ActionType = CreatureKillSpawnActionType.MoveTo; break;
                    case "follow": newKillSpawn.ActionType = CreatureKillSpawnActionType.Follow; break;
                    case "root": newKillSpawn.ActionType = CreatureKillSpawnActionType.Root; break;
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
                for (int i = 1; i <= MAX_REQUIRE_DEAD_CREATURE_TEMPLATES; i++)
                {
                    string idString = columns[string.Concat("require_dead_eq_npc_id_", i)].Trim();
                    if (idString.Length == 0)
                        continue;
                    newKillSpawn.RequireDeadEQCreatureTemplateIDs.Add(int.Parse(idString));
                }
                foreach (string idString in columns["require_alive_eq_npc_ids"].Split(',', StringSplitOptions.RemoveEmptyEntries))
                    newKillSpawn.RequireAliveEQCreatureTemplateIDs.Add(int.Parse(idString));
                newKillSpawn.AddToHateList = columns["add_to_hate"].Trim() == "1";
                newKillSpawn.TriggerMinLevel = int.Parse(columns["trigger_min_level"]);
                newKillSpawn.TriggerMaxLevel = int.Parse(columns["trigger_max_level"]);

                // A respawn time of -1 means to use the raid boss respawn window
                // TODO: Have a lookup for boss creatures so it uses the right respawn max time (could be raid trash)
                newKillSpawn.RespawnTimeInSec = int.Parse(columns["respawn_time_sec"]);
                if (newKillSpawn.RespawnTimeInSec == -1)
                {
                    newKillSpawn.RespawnTimeInSec = Configuration.CREATURE_RAID_BOSS_RESPAWN_CENTER_IN_SEC;
                    newKillSpawn.RespawnUsesRaidLockout = true;
                }
                newKillSpawn.Comment = columns["comment"];

                // The radius is an EQ distance
                string triggerRadiusString = columns["trigger_radius"].Trim();
                if (triggerRadiusString.Length > 0)
                    newKillSpawn.TriggerRadius = float.Parse(triggerRadiusString) * Configuration.GENERATE_WORLD_SCALE;
                if (newKillSpawn.TriggerType == CreatureKillSpawnTriggerType.Proximity && newKillSpawn.TriggerRadius <= 0)
                {
                    Logger.WriteError("CreatureKillSpawn row '" + newKillSpawn.ID + "' has a proximity trigger with no trigger_radius");
                    continue;
                }
                string spellEQIDString = columns["spell_eq_id"].Trim();
                if (spellEQIDString.Length > 0)
                    newKillSpawn.SpellEQID = int.Parse(spellEQIDString);
                if (newKillSpawn.ActionType == CreatureKillSpawnActionType.CastSpell && newKillSpawn.SpellEQID <= 0)
                {
                    Logger.WriteError("CreatureKillSpawn row '" + newKillSpawn.ID + "' is a castspell with no spell_eq_id");
                    continue;
                }
                string pathGridString = columns["path_grid"].Trim();
                if (pathGridString.Length > 0)
                {
                    int gridID;
                    int startNode;
                    int endNode;
                    if (Quests.QuestReaction.TryParseWalkGridValue(pathGridString, out gridID, out startNode, out endNode) == false)
                    {
                        Logger.WriteError("CreatureKillSpawn row '" + newKillSpawn.ID + "' has an unreadable path_grid of '" + pathGridString + "'");
                        continue;
                    }
                    newKillSpawn.PathGridID = gridID;
                    newKillSpawn.PathGridStartNode = startNode;
                    newKillSpawn.PathGridEndNode = endNode;
                }
                if (newKillSpawn.ActionType == CreatureKillSpawnActionType.WalkGrid && newKillSpawn.PathGridID == -1)
                {
                    Logger.WriteError("CreatureKillSpawn row '" + newKillSpawn.ID + "' is a walkgrid with no path_grid");
                    continue;
                }
                switch (columns["arrival_behavior"].ToLower().Trim())
                {
                    case "": newKillSpawn.ArrivalBehavior = CreatureKillSpawnArrivalBehaviorType.None; break;
                    case "sit": newKillSpawn.ArrivalBehavior = CreatureKillSpawnArrivalBehaviorType.Sit; break;
                    case "despawn": newKillSpawn.ArrivalBehavior = CreatureKillSpawnArrivalBehaviorType.Despawn; break;
                    default:
                        {
                            Logger.WriteError("CreatureKillSpawn row '" + newKillSpawn.ID + "' has unknown arrival_behavior '" + columns["arrival_behavior"] + "'");
                            continue;
                        }
                }
                newKillSpawn.IsPersistent = columns["persistent"].Trim() == "1";
                newKillSpawn.Text = columns["text"].Trim().Replace("\"", "'");
                if ((newKillSpawn.ActionType == CreatureKillSpawnActionType.Say || newKillSpawn.ActionType == CreatureKillSpawnActionType.Emote
                    || newKillSpawn.ActionType == CreatureKillSpawnActionType.Yell) && newKillSpawn.Text.Length == 0)
                {
                    Logger.WriteError("CreatureKillSpawn row '" + newKillSpawn.ID + "' speaks but has no text");
                    continue;
                }
                KillSpawnList.Add(newKillSpawn);

                if (newKillSpawn.TriggerType == CreatureKillSpawnTriggerType.Hail)
                {
                    if (HailTriggerEQCreatureTemplateIDsByZoneShortName.ContainsKey(newKillSpawn.ZoneShortName) == false)
                        HailTriggerEQCreatureTemplateIDsByZoneShortName.Add(newKillSpawn.ZoneShortName, new HashSet<int>());
                    HailTriggerEQCreatureTemplateIDsByZoneShortName[newKillSpawn.ZoneShortName].Add(newKillSpawn.TriggerEQCreatureTemplateID);
                }

                if (newKillSpawn.ActionType == CreatureKillSpawnActionType.Spawn && newKillSpawn.TargetEQCreatureTemplateID > 0)
                    SpawnedTargetEQCreatureTemplateIDs.Add(newKillSpawn.TargetEQCreatureTemplateID);
            }
        }
    }
}
