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

namespace EQWOWConverter.Quests
{
    internal class QuestExperience
    {
        private static Dictionary<int, int[]> WOWQuestExperienceByLevel = new Dictionary<int, int[]>();
        private static readonly object QuestExperienceLock = new object();
        private static readonly int[] WOW_EXPERIENCE_TO_COMPLETE_LEVEL = {400, 900, 1400, 2100, 2800, 3600, 4500, 5400, 6500, 7600, 8700, 9800, 11000, 12300, 13600, 15000, 16400, 17800, 19300, 20800, 
            22400, 24000, 25500, 27200, 28900, 30500, 32200, 33900, 36300, 38800, 41600, 44600, 48000, 51400, 55000, 58700, 62400, 66200, 70200, 74300, 78500, 82800, 87100, 91600, 96300, 101000, 105800,
            110700, 115700, 120900, 126100, 131500, 137000, 142500, 148200, 154000, 159900, 165800, 172000, 290000 };

        public static int GetRewardXPDifficulty(int questLevel, int rewardExperienceEQ)
        {
            if (rewardExperienceEQ <= 0)
                return 0;

            // -1 level quests scale to the player's level on the server, so award the smallest tier
            if (questLevel < 1)
                return 1;
            if (questLevel > 60)
                questLevel = 60;

            // Determine what fraction of the quest level's experience the reward was worth in EQ, honoring the EQ rule that a single quest reward can not exceed a fraction of the level's experience
            double eqExperienceForLevel = GetEQExperienceToCompleteLevel(questLevel);
            double cappedRewardExperienceEQ = Math.Min(rewardExperienceEQ, eqExperienceForLevel * Configuration.QUESTS_EXP_EQ_REWARD_LEVEL_FRACTION_CAP);
            double levelFraction = cappedRewardExperienceEQ / eqExperienceForLevel;

            // Award the difficulty tier closest to that same fraction of the level's experience in WOW
            int[] experienceByDifficulty = GetWOWQuestExperienceByLevel()[questLevel];
            double targetWOWExperience = levelFraction * WOW_EXPERIENCE_TO_COMPLETE_LEVEL[questLevel - 1];
            int closestDifficulty = 1;
            double closestDistance = Math.Abs(experienceByDifficulty[0] - targetWOWExperience);
            for (int difficulty = 2; difficulty <= 8; difficulty++)
            {
                double curDistance = Math.Abs(experienceByDifficulty[difficulty - 1] - targetWOWExperience);
                if (curDistance < closestDistance)
                {
                    closestDistance = curDistance;
                    closestDifficulty = difficulty;
                }
            }
            return closestDifficulty;
        }

        public static int GetLowestLevelWithProportionalReward(int rewardExperienceEQ)
        {
            for (int level = 1; level < 60; level++)
            {
                double eqExperienceForLevel = GetEQExperienceToCompleteLevel(level);
                if (rewardExperienceEQ <= eqExperienceForLevel * Configuration.QUESTS_EXP_EQ_REWARD_LEVEL_FRACTION_CAP)
                    return level;
            }
            return 60;
        }

        private static double GetEQExperienceToCompleteLevel(int level)
        {
            return GetEQTotalExperienceForLevel(level + 1) - GetEQTotalExperienceForLevel(level);
        }

        private static double GetEQTotalExperienceForLevel(int level)
        {
            int adjustedLevel = level - 1;
            return Convert.ToDouble(adjustedLevel * adjustedLevel * adjustedLevel) * 1000d;
        }

        private static Dictionary<int, int[]> GetWOWQuestExperienceByLevel()
        {
            lock (QuestExperienceLock)
            {
                if (WOWQuestExperienceByLevel.Count == 0)
                    PopulateWOWQuestExperienceByLevel();
                return WOWQuestExperienceByLevel;
            }
        }

        private static void PopulateWOWQuestExperienceByLevel()
        {
            WOWQuestExperienceByLevel.Clear();
            string questExperienceFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "QuestRewardExperience.csv");
            Logger.WriteDebug(string.Concat("Populating quest reward experience via file '", questExperienceFile, "'"));
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(questExperienceFile, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                int level = int.Parse(columns["Level"]);
                int[] experienceByDifficulty = new int[8];
                for (int difficulty = 1; difficulty <= 8; difficulty++)
                    experienceByDifficulty[difficulty - 1] = int.Parse(columns[string.Concat("Difficulty", difficulty)]);

                if (WOWQuestExperienceByLevel.ContainsKey(level) == true)
                {
                    Logger.WriteError(string.Concat("Failed to read in a quest reward experience row since there was a duplicate row with level ", level));
                    continue;
                }
                WOWQuestExperienceByLevel.Add(level, experienceByDifficulty);
            }
        }
    }
}
