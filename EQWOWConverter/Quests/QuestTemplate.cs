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

namespace EQWOWConverter.Quests
{
    internal class QuestTemplate
    {
        static private List<QuestTemplate> QuestTemplates = new List<QuestTemplate>();
        static private readonly object QuestLock = new object();

        public int QuestID;
        public string Name = string.Empty;
        public string ZoneShortName = string.Empty;
        public string QuestgiverName = string.Empty;
        public int? MinimumQuestgiverFactionValue = null;
        public int RequiredItem1EQID;
        public int RequiredItem2EQID;
        public int RequiredItem3EQID;
        public int RequiredItem4EQID;
        public int RequiredItem5EQID;
        public int RequiredItem6EQID;
        public int RewardMoneyInCopper;
        public int RewardExperience;
        public int RewardItem1ID;
        public int RewardItem1Count;
        public float RewardItem1Chance;
        public int RewardItem2ID;
        public int RewardItem2Count;
        public float RewardItem2Chance;
        public int RewardItem3ID;
        public int RewardItem3Count;
        public float RewardItem3Chance;
        // TODO: Faction
        // TODO: Attack player after turnin
        public string RequestText = string.Empty;

        public static List<QuestTemplate> GetQuestTemplates()
        {
            lock (QuestLock)
            {
                if (QuestTemplates.Count == 0)
                    PopulateQuestTemplates();
                return QuestTemplates;
            }
        }

        private static void PopulateQuestTemplates()
        {
            string questTemplateFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "QuestTemplates.csv");
            Logger.WriteDebug(string.Concat("Populating quest templates via file '", questTemplateFile, "'"));
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(questTemplateFile, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                // Skip invalid expansions
                if (int.Parse(columns["min_expansion"]) > Configuration.GENERATE_EQ_EXPANSION_ID)
                    continue;

                // Load the row
                QuestTemplate newQuestTemplate = new QuestTemplate();
                newQuestTemplate.QuestID = int.Parse(columns["wow_questid"]);
                newQuestTemplate.Name = columns["quest_name"];
                newQuestTemplate.ZoneShortName = columns["zone_shortname"];
                newQuestTemplate.QuestgiverName = columns["questgiver_name"];
                int minRep = int.Parse(columns["req_repmin"]);
                newQuestTemplate.MinimumQuestgiverFactionValue = minRep == -1 ? null : minRep;
                newQuestTemplate.RequiredItem1EQID = int.Parse(columns["req_item1"]);
                newQuestTemplate.RequiredItem2EQID = int.Parse(columns["req_item2"]);
                newQuestTemplate.RequiredItem3EQID = int.Parse(columns["req_item3"]);
                newQuestTemplate.RequiredItem4EQID = int.Parse(columns["req_item4"]);
                newQuestTemplate.RequiredItem5EQID = int.Parse(columns["req_item5"]);
                newQuestTemplate.RequiredItem6EQID = int.Parse(columns["req_item6"]);
                newQuestTemplate.RewardMoneyInCopper = int.Parse(columns["reward_money"]);
                newQuestTemplate.RewardExperience = int.Parse(columns["reward_exp"]);
                newQuestTemplate.RewardItem1ID = int.Parse(columns["reward_item_ID1"]);
                newQuestTemplate.RewardItem1Count = int.Parse(columns["reward_item_count1"]);
                newQuestTemplate.RewardItem1Chance = float.Parse(columns["reward_item_chance1"]);
                newQuestTemplate.RewardItem2ID = int.Parse(columns["reward_item_ID2"]);
                newQuestTemplate.RewardItem2Count = int.Parse(columns["reward_item_count2"]);
                newQuestTemplate.RewardItem2Chance = float.Parse(columns["reward_item_chance2"]);
                newQuestTemplate.RewardItem3ID = int.Parse(columns["reward_item_ID3"]);
                newQuestTemplate.RewardItem3Count = int.Parse(columns["reward_item_count3"]);
                newQuestTemplate.RewardItem3Chance = float.Parse(columns["reward_item_chance3"]);
                newQuestTemplate.RequestText = columns["request_text"];
                QuestTemplates.Add(newQuestTemplate);
            }
        }
    }
}
