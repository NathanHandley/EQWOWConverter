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

using EQWOWConverter.Creatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Common
{
    internal class StatBaselines
    {
        private static SortedDictionary<int, Dictionary<string, float>> StatBaselinesByLevels = new SortedDictionary<int, Dictionary<string, float>>();

        private static void PopulateStatBaselinesByLevel()
        {
            string statBaselineFile = Path.Combine(Configuration.CONFIG_PATH_ASSETS_FOLDER, "WorldData", "StatBaselines.csv");
            Logger.WriteDetail("Populating Stat Baselines list via file '" + statBaselineFile + "'");
            List<string> statBaselineRows = FileTool.ReadAllStringLinesFromFile(statBaselineFile, true, true);
            foreach (string row in statBaselineRows)
            {
                // Load the row
                string[] rowBlocks = row.Split("|");
                int level = int.Parse(rowBlocks[0]);
                StatBaselinesByLevels.Add(level, new Dictionary<string, float>());

                // Creature HP
                int creatureHP = int.Parse(rowBlocks[1]);
                StatBaselinesByLevels[level].Add("creaturehp", creatureHP);

                // Creature avg damage
                float creatureAvgDMG = float.Parse(rowBlocks[2]);
                StatBaselinesByLevels[level].Add("creatureavgdmg", creatureAvgDMG);

                // Item - Armor: Chest
                int itemArmorChest = int.Parse(rowBlocks[3]);
                StatBaselinesByLevels[level].Add("itemarmorchest", itemArmorChest);
            }
        }

        public static float GetStatMod(string statName, int level, float eqStatValue, CreatureRankType creatureRank = CreatureRankType.Normal)
        {
            if (StatBaselinesByLevels.Count == 0)
                PopulateStatBaselinesByLevel();
            if (level == 0)
                return 1;
            if (eqStatValue < 1)
                return 1;

            // Determine the boundaries and adds
            float modAdd;
            float modMin;
            float modMax;
            float modSet;
            switch (statName)
            {
                case "creaturehp":
                    {
                        modAdd = Configuration.CONFIG_CREATURE_STAT_MOD_HP_ADD;
                        modMin = Configuration.CONFIG_CREATURE_STAT_MOD_HP_MIN;
                        modMax = GetValueForCreatureRank(creatureRank, Configuration.CONFIG_CREATURE_STAT_MOD_HP_MAX_NORMAL,
                            -1, -1, -1, Configuration.CONFIG_CREATURE_STAT_MOD_HP_MAX_RARE);
                        modSet = GetValueForCreatureRank(creatureRank, -1, Configuration.CONFIG_CREATURE_STAT_MOD_HP_SET_ELITE,
                            Configuration.CONFIG_CREATURE_STAT_MOD_HP_SET_ELITERARE, Configuration.CONFIG_CREATURE_STAT_MOD_HP_SET_BOSS, -1);
                    }
                    break;
                case "creatureavgdmg":
                    {
                        modAdd = Configuration.CONFIG_CREATURE_STAT_MOD_AVGDMG_ADD;
                        modMin = Configuration.CONFIG_CREATURE_STAT_MOD_AVGDMG_MIN;
                        modMax = GetValueForCreatureRank(creatureRank, Configuration.CONFIG_CREATURE_STAT_MOD_AVGDMG_MAX_NORMAL,
                            -1, -1, -1, Configuration.CONFIG_CREATURE_STAT_MOD_AVGDMG_MAX_RARE);
                        modSet = GetValueForCreatureRank(creatureRank, -1, Configuration.CONFIG_CREATURE_STAT_MOD_AVGDMG_SET_ELITE,
                            Configuration.CONFIG_CREATURE_STAT_MOD_AVGDMG_SET_ELITERARE, Configuration.CONFIG_CREATURE_STAT_MOD_AVGDMG_SET_BOSS, -1);
                    }
                    break;
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
                    if (levelLow == level)
                        break;
                    else
                        continue;
                }
                if (recordLevel <= level)
                {
                    levelLow = recordLevel;
                    statLow = recordStat;
                }
                if (level <= recordLevel)
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
            if (level == levelLow)
                statRelative = statLow;
            else if (level == levelHigh)
                statRelative = statHigh;
            else
            {
                float normalLevelRelative = (Convert.ToSingle(level - levelLow) / Convert.ToSingle(levelHigh - levelLow));
                statRelative = normalLevelRelative * statHigh + ((1 - normalLevelRelative) * statLow);
            }
            float genStatValue = eqStatValue / statRelative;

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

        private static float GetValueForCreatureRank(CreatureRankType creatureRank, float normalValue, float eliteValue, float eliteRareValue,
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
                        Logger.WriteError("GetValueForRank failed since rank '" + creatureRank + "' was not defined");
                        return 1;
                    }
            }
        }
    }
}
