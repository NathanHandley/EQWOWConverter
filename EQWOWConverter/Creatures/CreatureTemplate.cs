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

using EQWOWConverter.Common;

namespace EQWOWConverter.Creatures
{
    internal class CreatureTemplate
    {
        private static Dictionary<int, CreatureTemplate> CreatureTemplateListByEQID = new Dictionary<int, CreatureTemplate>();
        private static SortedDictionary<int, Dictionary<string, float>> StatBaselinesByLevels = new SortedDictionary<int, Dictionary<string, float>>();

        public int EQCreatureTemplateID = 0;
        public int WOWCreatureTemplateID = 0;
        public string Name = string.Empty; // Restrict to 100 characters
        public string SubName = string.Empty; // Restrict to 100 characters
        public int Level = 1;
        public CreatureRace Race = new CreatureRace();
        public int EQClass = 1;
        public int EQBodyType = 24; // This is common for the body type
        public int FaceID = 0;
        public int ColorTintID = 0;
        public float Size = 0f;
        public CreatureGenderType GenderType = CreatureGenderType.Neutral;
        public int TextureID = 0;
        public int HelmTextureID = 0;
        public CreatureModelTemplate? ModelTemplate = null;
        public int MerchantID = 0;
        public int EQLootTableID = 0;
        public int WOWLootID = 0;
        public int MoneyMinInCopper = 0;
        public int MoneyMaxInCopper = 0;
        public bool HasMana = false;
        public float HPMod = 1f;
        public float DamageMod = 1f;
        public CreatureRankType Rank = CreatureRankType.Normal;
        public int EQFactionID = 0;
        public int EQNPCFactionID = 0;
        public int WOWFactionTemplateID = 0;
        public List<CreatureFactionKillReward> CreatureFactionKillRewards = new List<CreatureFactionKillReward>();
        public float DetectionRange = 0;
        public bool CanAssist = false;
        public bool IsBanker = false;
        public ClassType ClassTrainerType = ClassType.None;
        public int GossipMenuID = 0;

        private static int CURRENT_SQL_CREATURE_GUID = Configuration.SQL_CREATURE_GUID_LOW;
        
        public static Dictionary<int, CreatureTemplate> GetCreatureTemplateListByEQID()
        {
            if (CreatureTemplateListByEQID.Count == 0)
                PopulateCreatureTemplateList();
            return CreatureTemplateListByEQID;
        }

        public static int GenerateCreatureSQLGUID()
        {
            int returnGUID = CURRENT_SQL_CREATURE_GUID;
            CURRENT_SQL_CREATURE_GUID++;
            return returnGUID;
        }

        private static void PopulateCreatureTemplateList()
        {
            // Grab the baselines
            PopulateStatBaselinesByLevel();

            // Load all of the creature data
            string creatureTemplatesFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "CreatureTemplates.csv");
            Logger.WriteDebug("Populating Creature Template list via file '" + creatureTemplatesFile + "'");           
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(creatureTemplatesFile, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                // Load the row
                CreatureTemplate newCreatureTemplate = new CreatureTemplate();
                newCreatureTemplate.EQCreatureTemplateID = int.Parse(columns["eq_id"]);
                newCreatureTemplate.WOWCreatureTemplateID = int.Parse(columns["wow_id"]);
                if (newCreatureTemplate.WOWCreatureTemplateID < Configuration.SQL_CREATURETEMPLATE_ENTRY_LOW || newCreatureTemplate.WOWCreatureTemplateID > Configuration.SQL_CREATURETEMPLATE_ENTRY_HIGH)
                    Logger.WriteError("Creature template with EQ id of '' had a wow id of '', but that's outside th ebounds of CREATURETEMPLATE_ENTRY_LOW and CREATURETEMPLATE_ENTRY_HIGH.  SQL deletes will not catch everything");
                newCreatureTemplate.Rank = (CreatureRankType)int.Parse(columns["rank"]);
                newCreatureTemplate.Name = columns["name"].Replace('_', ' ');
                newCreatureTemplate.SubName = columns["lastname"].Replace('_', ' ');
                newCreatureTemplate.Level = int.Max(int.Parse(columns["level"]), 1);
                int raceID = int.Parse(columns["race"]);
                if (raceID == 0)
                {
                    Logger.WriteDebug("Creature template had race of 0, so falling back to 1 (Human)");
                    raceID = 1;
                }
                newCreatureTemplate.EQBodyType = int.Parse(columns["bodytype"]);
                newCreatureTemplate.Size = float.Parse(columns["size"]);
                if (newCreatureTemplate.Size <= 0)
                {
                    Logger.WriteDebug("CreatureTemplate with size of zero or less detected with name '" + newCreatureTemplate.Name + "', so setting to 1");
                    newCreatureTemplate.Size = 1;
                }
                int genderID = int.Parse(columns["gender"]);
                switch (genderID)
                {
                    case 0: newCreatureTemplate.GenderType = CreatureGenderType.Male; break;
                    case 1: newCreatureTemplate.GenderType = CreatureGenderType.Female; break;
                    default: newCreatureTemplate.GenderType = CreatureGenderType.Neutral; break;
                }
                newCreatureTemplate.TextureID = int.Parse(columns["texture"]);
                newCreatureTemplate.HelmTextureID = int.Parse(columns["helmtexture"]);
                newCreatureTemplate.FaceID = int.Parse(columns["face"]);
                if (newCreatureTemplate.FaceID > 9)
                {
                    Logger.WriteDebug("CreatureTemplate with face ID greater than 9 detected, so setting to 0");
                    newCreatureTemplate.FaceID = 0;
                }
                newCreatureTemplate.EQLootTableID = int.Parse(columns["loottable_id"]);
                newCreatureTemplate.MerchantID = int.Parse(columns["merchant_id"]);
                newCreatureTemplate.ColorTintID = int.Parse(columns["armortint_id"]);
                newCreatureTemplate.HasMana = (int.Parse(columns["mana"]) > 0);
                newCreatureTemplate.HPMod = GetStatMod("hp", newCreatureTemplate.Level, newCreatureTemplate.Rank, float.Parse(columns["hp"]));
                newCreatureTemplate.DamageMod = GetStatMod("avgdmg", newCreatureTemplate.Level, newCreatureTemplate.Rank, float.Parse(columns["avgdmg"]));
                float detectionRange = float.Parse(columns["aggroradius"]);
                if (detectionRange > 0)
                    newCreatureTemplate.DetectionRange = detectionRange * Configuration.GENERATE_WORLD_SCALE;
                else
                    newCreatureTemplate.DetectionRange = Configuration.CREATURE_DEFAULT_DETECTION_RANGE;
                newCreatureTemplate.EQClass = int.Parse(columns["class"]);
                ProcessEQClass(ref newCreatureTemplate, newCreatureTemplate.EQClass);              

                // Special logic for a few variations of kobolds, which look wrong if not adjusted
                if (raceID == 48)
                {
                    if  (newCreatureTemplate.TextureID == 2 || (newCreatureTemplate.TextureID == 1 && newCreatureTemplate.HelmTextureID == 0))
                    {
                        newCreatureTemplate.TextureID = 0;
                        newCreatureTemplate.HelmTextureID = 0;
                        newCreatureTemplate.FaceID = 0;
                    }
                }

                // Grab the race
                List<CreatureRace> allRaces = CreatureRace.GetAllCreatureRaces();
                CreatureRace? race = null;
                foreach (CreatureRace curRace in allRaces)
                {
                    if (curRace.ID == raceID && curRace.Gender == newCreatureTemplate.GenderType && curRace.VariantID == 0)
                    {
                        race = curRace;
                        break;
                    }
                }

                if (race == null)
                {
                    Logger.WriteError("No valid race found that matches raceID '" + raceID + "' and gender '" + newCreatureTemplate.GenderType + "'");
                    continue;
                }
                else
                {
                    // Make sure there's a skeleton
                    if (race.SkeletonName.Trim().Length == 0)
                    {
                        Logger.WriteDebug("Creature Template with name '" + newCreatureTemplate.Name + "' with race ID of '" + raceID + "' has no skeletons, so skipping");
                        continue;
                    }
                    newCreatureTemplate.Race = race;
                }

                // Add ID if debugging for it is true
                if (Configuration.CREATURE_ADD_ENTITY_ID_TO_NAME == true)
                    newCreatureTemplate.Name = newCreatureTemplate.Name + " " + newCreatureTemplate.EQCreatureTemplateID.ToString();
                //newCreatureTemplate.Name = newCreatureTemplate.Name + " R" + newCreatureTemplate.Race.EQCreatureTemplateID + "-G" + Convert.ToInt32(newCreatureTemplate.GenderType).ToString() + "-V" + newCreatureTemplate.Race.VariantID;

                // Reputation / Faction
                newCreatureTemplate.EQFactionID = int.Parse(columns["faction_id"]);
                newCreatureTemplate.EQNPCFactionID = int.Parse(columns["npc_faction_id"]);
                newCreatureTemplate.WOWFactionTemplateID = CreatureFaction.GetWOWFactionTemplateIDForEQFactionID(newCreatureTemplate.EQFactionID);
                newCreatureTemplate.CanAssist = CreatureFaction.CanFactionAssistPlayer(newCreatureTemplate.EQFactionID);
                foreach (CreatureFactionKillReward factionKillReward in CreatureFaction.GetCreatureFactionKillRewards(newCreatureTemplate.EQNPCFactionID))
                    newCreatureTemplate.CreatureFactionKillRewards.Add(factionKillReward);

                // Must be a unique record
                if (CreatureTemplateListByEQID.ContainsKey(newCreatureTemplate.EQCreatureTemplateID))
                {
                    Logger.WriteError("Creature Template list via file '" + creatureTemplatesFile + "' has an duplicate row with id '" + newCreatureTemplate.EQCreatureTemplateID + "'");
                    continue;
                }

                CreatureTemplateListByEQID.Add(newCreatureTemplate.EQCreatureTemplateID, newCreatureTemplate);
            }
        }

        private static void ProcessEQClass(ref CreatureTemplate creatureTemplate, int eqClass)
        {
            switch(eqClass)
            {
                case 20: // Warrior GM
                    {
                        creatureTemplate.ClassTrainerType = ClassType.Warrior;
                        creatureTemplate.SubName = "Warrior Trainer";
                    } break;
                case 21: // Cleric GM
                    {
                        creatureTemplate.ClassTrainerType = ClassType.Priest;
                        creatureTemplate.SubName = "Priest Trainer";
                    } break;
                case 22: // Paladin GM
                    {
                        creatureTemplate.ClassTrainerType = ClassType.Paladin;
                        creatureTemplate.SubName = "Paladin Trainer";
                    } break;
                case 23: // RangerGM
                    {
                        creatureTemplate.ClassTrainerType = ClassType.Hunter;
                        creatureTemplate.SubName = "Hunter Trainer";
                    } break;
                case 24: // ShadowKnight GM
                    {
                        creatureTemplate.ClassTrainerType = ClassType.DeathKnight;
                        creatureTemplate.SubName = "Death Knight Trainer";
                    } break;
                case 25: // Druid GM
                    {
                        creatureTemplate.ClassTrainerType = ClassType.Druid;
                        creatureTemplate.SubName = "Druid Trainer";
                    } break;
                case 26: // Monk GM
                    {
                        creatureTemplate.ClassTrainerType = ClassType.Rogue;
                        creatureTemplate.SubName = "Rogue Trainer";
                    } break;
                case 27: // Bard GM
                    {
                        creatureTemplate.ClassTrainerType = ClassType.Warrior;
                        creatureTemplate.SubName = "Warrior Trainer";
                    } break;
                case 28: // Rogue GM
                    {
                        creatureTemplate.ClassTrainerType = ClassType.Rogue;
                        creatureTemplate.SubName = "Rogue Trainer";
                    } break;
                case 29: // Shaman GM
                    {
                        creatureTemplate.ClassTrainerType = ClassType.Shaman;
                        creatureTemplate.SubName = "Shaman Trainer";
                    } break;
                case 30: // Necromancer GM
                    {
                        creatureTemplate.ClassTrainerType = ClassType.Warlock;
                        creatureTemplate.SubName = "Warlock Trainer";
                    } break;
                case 31: // Wizard GM
                    {
                        creatureTemplate.ClassTrainerType = ClassType.Mage;
                        creatureTemplate.SubName = "Mage Trainer";
                    } break;
                case 32: // Mage GM
                    {
                        creatureTemplate.ClassTrainerType = ClassType.Mage;
                        creatureTemplate.SubName = "Mage Trainer";
                    } break;
                case 33: // Enchanter GM
                    {
                        creatureTemplate.ClassTrainerType = ClassType.Warlock;
                        creatureTemplate.SubName = "Warlock Trainer";
                    } break;
                case 34: // Beastlord GM
                    {
                        creatureTemplate.ClassTrainerType = ClassType.Hunter;
                        creatureTemplate.SubName = "Hunter Trainer";
                    } break;
                case 40: // Banker
                    {
                        creatureTemplate.IsBanker = true;
                    } break;
                default:
                    {
                        // Do nothing
                    } break;
            }                
        }

        private static void PopulateStatBaselinesByLevel()
        {
            string creatureStatBaselineFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "CreatureStatBaselines.csv");
            Logger.WriteDebug("Populating Creature Stat Baselines list via file '" + creatureStatBaselineFile + "'");
            List<string> creatureStatBaselineRows = FileTool.ReadAllStringLinesFromFile(creatureStatBaselineFile, true, true);
            foreach (string row in creatureStatBaselineRows)
            {
                // Load the row
                string[] rowBlocks = row.Split("|");
                int level = int.Parse(rowBlocks[0]);
                int hp = int.Parse(rowBlocks[1]);
                float avgDMG = float.Parse(rowBlocks[2]);

                // Create the baseline record
                StatBaselinesByLevels.Add(level, new Dictionary<string, float>());
                StatBaselinesByLevels[level].Add("hp", hp);
                StatBaselinesByLevels[level].Add("avgdmg", avgDMG);
            }
        }

        private static float GetStatMod(string statName, int creatureLevel, CreatureRankType creatureRank, float creatureStatValue)
        {
            if (creatureLevel == 0)
                return 1;
            if (creatureStatValue < 1)
                return 1;

            // Determine the boundaries and adds
            float modAdd;
            float modMin;
            float modMax;
            float modSet;
            switch(statName)
            {
                case "hp":
                    {
                        modAdd = Configuration.CREATURE_STAT_MOD_HP_ADD;
                        modMin = Configuration.CREATURE_STAT_MOD_HP_MIN;
                        modMax = GetValueForRank(creatureRank, Configuration.CREATURE_STAT_MOD_HP_MAX_NORMAL,
                            -1, -1, -1, Configuration.CREATURE_STAT_MOD_HP_MAX_RARE);
                        modSet = GetValueForRank(creatureRank, -1, Configuration.CREATURE_STAT_MOD_HP_SET_ELITE,
                            Configuration.CREATURE_STAT_MOD_HP_SET_ELITERARE, Configuration.CREATURE_STAT_MOD_HP_SET_BOSS, -1);
                    } break;
                case "avgdmg":
                    {
                        modAdd = Configuration.CREATURE_STAT_MOD_AVGDMG_ADD;
                        modMin = Configuration.CREATURE_STAT_MOD_AVGDMG_MIN;
                        modMax = GetValueForRank(creatureRank, Configuration.CREATURE_STAT_MOD_AVGDMG_MAX_NORMAL,
                            -1, -1, -1, Configuration.CREATURE_STAT_MOD_AVGDMG_MAX_RARE);
                        modSet = GetValueForRank(creatureRank, -1, Configuration.CREATURE_STAT_MOD_AVGDMG_SET_ELITE,
                            Configuration.CREATURE_STAT_MOD_AVGDMG_SET_ELITERARE, Configuration.CREATURE_STAT_MOD_AVGDMG_SET_BOSS, -1);
                    } break;
                default:
                    {
                        Logger.WriteError("GetStatMod failed due to unhandled stat name of '" + statName + "'");
                        return 1;
                    }
            }

            // Calculate the specific baseline to use based on range
            int levelLow = -1;
            int levelHigh = -1;
            float statLow = -1;
            float statHigh = -1;
            foreach (var statBaselineForLevel in StatBaselinesByLevels)
            {
                // Grab the stat for this level row
                if (statBaselineForLevel.Value.ContainsKey(statName) == false)
                {
                    Logger.WriteError("Error in GetStatMod as stat name '" + statName + "' did not exist");
                    return 1;
                }
                int recordLevel = statBaselineForLevel.Key;
                float recordStat = statBaselineForLevel.Value[statName];

                // Store based on current bounds
                if (levelLow == -1)
                {
                    levelLow = recordLevel;
                    levelHigh = recordLevel;
                    statLow = recordStat;
                    statHigh = recordStat;
                    if (levelLow == creatureLevel)
                        break;
                    else
                        continue;
                }
                if (recordLevel <= creatureLevel)
                {
                    levelLow = recordLevel;
                    statLow = recordStat;
                }
                if (creatureLevel <= recordLevel)
                {
                    levelHigh = recordLevel;
                    statHigh = recordStat;
                    break;
                }
            }
            if (levelLow == -1 || levelHigh == -1 || statLow == -1 || statHigh == -1)
            {
                Logger.WriteError("GetStatMod failed as one of the range caps was -1");
                return 1;
            }

            // Generate a stat mod
            float statRelative;
            if (creatureLevel == levelLow)
                statRelative = statLow;
            else if (creatureLevel == levelHigh)
                statRelative = statHigh;
            else
            {
                float normalLevelRelative = (Convert.ToSingle(creatureLevel - levelLow) / Convert.ToSingle(levelHigh - levelLow));
                statRelative = normalLevelRelative * statHigh + ((1 - normalLevelRelative) * statLow);
            }
            float genStatValue = creatureStatValue / statRelative;

            // Apply adds and limits
            genStatValue += modAdd;
            if (modMin != -1)
                genStatValue = MathF.Max(genStatValue, modMin);
            if (modMax != -1)
                genStatValue = MathF.Min(genStatValue, modMax);
            if (modSet != -1)
                genStatValue = modSet;
            return genStatValue;
        }

        private static float GetValueForRank(CreatureRankType creatureRank, float normalValue, float eliteValue, float eliteRareValue,
            float bossValue, float rareValue)
        {
            switch (creatureRank)
            {
                case CreatureRankType.Normal: return normalValue;
                case CreatureRankType.Elite: return eliteValue;
                case CreatureRankType.Boss: return bossValue;
                case CreatureRankType.Rare: return rareValue;
                case CreatureRankType.EliteRare: return eliteRareValue;
                default:
                    {
                        Logger.WriteError("GeTValueForRank failed since rank '" + creatureRank + "' was not defined");
                        return 1;
                    }
            }
        }
    }
}
