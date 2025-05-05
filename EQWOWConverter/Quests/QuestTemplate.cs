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

using EQWOWConverter.Items;

namespace EQWOWConverter.Quests
{
    internal class QuestTemplate
    {
        static private List<QuestTemplate> QuestTemplates = new List<QuestTemplate>();
        static private readonly object QuestLock = new object();

        public int QuestIDWOW;
        public string Name = string.Empty;
        public string ZoneShortName = string.Empty;
        public string QuestgiverName = string.Empty;
        public List<int> QuestgiverWOWCreatureTemplateIDs = new List<int>();
        public bool HasMinimumFactionRequirement = false;
        public int QuestgiverWOWFactionID = 0;
        public int MinimumQuestgiverFactionValue = 0;
        public int RequiredItem1EQID;
        public int RequiredItem1WOWID = 0;
        public int RequiredItem1Count;
        public int RequiredItem2EQID;
        public int RequiredItem2WOWID = 0;
        public int RequiredItem2Count;
        public int RequiredItem3EQID;
        public int RequiredItem3WOWID = 0;
        public int RequiredItem3Count;
        public int RequiredItem4EQID;
        public int RequiredItem4WOWID = 0;
        public int RequiredItem4Count;
        public int RequiredItem5EQID;
        public int RequiredItem5WOWID = 0;
        public int RequiredItem5Count;
        public int RequiredItem6EQID;
        public int RequiredItem6WOWID = 0;
        public int RequiredItem6Count;
        public int RewardMoneyInCopper;
        public int RewardExperience;
        public int RewardItem1EQID;
        public int RewardItem1WOWID = 0;
        public int RewardItem1Count;
        public float RewardItem1Chance;
        public int RewardItem2EQID;
        public int RewardItem2WOWID = 0;
        public int RewardItem2Count;
        public float RewardItem2Chance;
        public int RewardItem3EQID;
        public int RewardItem3WOWID = 0;
        public int RewardItem3Count;
        public float RewardItem3Chance;
        // TODO: Faction
        // TODO: Attack player after turnin
        public string RequestText = string.Empty;
        public string RequestObjectiveText = string.Empty;
        public int AreaID = 0;
        private int NumOfObjectiveItemsAddedToText = 0;

        public static List<QuestTemplate> GetQuestTemplates()
        {
            lock (QuestLock)
            {
                if (QuestTemplates.Count == 0)
                    PopulateQuestTemplates();
                return QuestTemplates;
            }
        }

        private static int ConvertEQFactionValueToWoW(int eqFactionValue)
        {
            switch (eqFactionValue)
            {
                case -500:  return -2999;   // EQ Dubious => WOW Unfriendly
                case -100:  return -2999;   // EQ Apprehensive => WOW Unfriendly
                case 0:     return 0;       // EQ Indifferent => WOW Neutral
                case 100:   return 3000;    // EQ Amiable => WOW Friendly
                case 500:   return 6000;    // EQ Kindly => WOW Honored
                case 750:   return 12000;   // EQ Warmly => WOW Revered
                case 1100:  return 21000;   // EQ Ally => WOW Exalted
                default:
                    {
                        Logger.WriteError(string.Concat("Could not convert EQ faction value to WOW faction value as unhandled faction value of '", eqFactionValue, "' was provided"));
                        return 0;
                    }
            }
        }

        public void AddObjectiveItems(string itemName, int itemCount)
        {
            if (NumOfObjectiveItemsAddedToText != 0)
                RequestText += ", ";
            else
                RequestText = "Bring me ";

            if (itemCount > 1 && itemName.EndsWith("s") == false)
                RequestText = string.Concat(RequestText, itemCount, " ", itemName, "s");
            else
                RequestText = string.Concat(RequestText, itemCount, " ", itemName);
            NumOfObjectiveItemsAddedToText++;
        }

        private static void PopulateQuestTemplates()
        {
            string questTemplateFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "QuestTemplates.csv");
            Logger.WriteDebug(string.Concat("Populating quest templates via file '", questTemplateFile, "'"));
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(questTemplateFile, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                // Skip invalid expansions
                if (int.Parse(columns["min_expansion"]) > Configuration.GENERATE_EQ_EXPANSION_ID_GENERAL)
                    continue;

                // Load the row
                QuestTemplate newQuestTemplate = new QuestTemplate();
                newQuestTemplate.QuestIDWOW = int.Parse(columns["wow_questid"]);
                newQuestTemplate.Name = columns["quest_name"];
                newQuestTemplate.ZoneShortName = columns["zone_shortname"];
                newQuestTemplate.QuestgiverName = columns["questgiver_name"];
                int minRep = int.Parse(columns["req_repmin"]);
                newQuestTemplate.MinimumQuestgiverFactionValue = minRep == -1 ? 0 : ConvertEQFactionValueToWoW(minRep);
                newQuestTemplate.HasMinimumFactionRequirement = minRep == -1 ? false : true;
                newQuestTemplate.RequiredItem1EQID = int.Parse(columns["req_item_id1"]);
                newQuestTemplate.RequiredItem1Count = int.Parse(columns["req_item_count1"]);
                newQuestTemplate.RequiredItem2EQID = int.Parse(columns["req_item_id2"]);
                newQuestTemplate.RequiredItem2Count = int.Parse(columns["req_item_count2"]);
                newQuestTemplate.RequiredItem3EQID = int.Parse(columns["req_item_id3"]);
                newQuestTemplate.RequiredItem3Count = int.Parse(columns["req_item_count3"]);
                newQuestTemplate.RequiredItem4EQID = int.Parse(columns["req_item_id4"]);
                newQuestTemplate.RequiredItem4Count = int.Parse(columns["req_item_count4"]);
                newQuestTemplate.RequiredItem5EQID = int.Parse(columns["req_item_id5"]);
                newQuestTemplate.RequiredItem5Count = int.Parse(columns["req_item_count5"]);
                newQuestTemplate.RequiredItem6EQID = int.Parse(columns["req_item_id6"]);
                newQuestTemplate.RequiredItem6Count = int.Parse(columns["req_item_count6"]);
                newQuestTemplate.RewardMoneyInCopper = int.Parse(columns["reward_money"]);
                newQuestTemplate.RewardExperience = int.Parse(columns["reward_exp"]);
                newQuestTemplate.RewardItem1EQID = int.Parse(columns["reward_item_ID1"]);
                newQuestTemplate.RewardItem1Count = int.Parse(columns["reward_item_count1"]);
                newQuestTemplate.RewardItem1Chance = float.Parse(columns["reward_item_chance1"]);
                newQuestTemplate.RewardItem2EQID = int.Parse(columns["reward_item_ID2"]);
                newQuestTemplate.RewardItem2Count = int.Parse(columns["reward_item_count2"]);
                newQuestTemplate.RewardItem2Chance = float.Parse(columns["reward_item_chance2"]);
                newQuestTemplate.RewardItem3EQID = int.Parse(columns["reward_item_ID3"]);
                newQuestTemplate.RewardItem3Count = int.Parse(columns["reward_item_count3"]);
                newQuestTemplate.RewardItem3Chance = float.Parse(columns["reward_item_chance3"]);
                //newQuestTemplate.RequestText = columns["request_text"]; Ignoring for now
                QuestTemplates.Add(newQuestTemplate);
            }
        }

        public bool MapWOWItemIDs(SortedDictionary<int, ItemTemplate> itemTemplatesByEQDBID)
        {
            if (RequiredItem1EQID != -1)
            {
                if (itemTemplatesByEQDBID.ContainsKey(RequiredItem1EQID) == false)
                {
                    Logger.WriteDebug(string.Concat("Quest '", Name, "' (", QuestIDWOW, ") could not be mapped as the EQItemID '", RequiredItem1EQID, "' did not exist"));
                    return false;
                }
                RequiredItem1WOWID = itemTemplatesByEQDBID[RequiredItem1EQID].WOWEntryID;
                AddObjectiveItems(itemTemplatesByEQDBID[RequiredItem1EQID].Name, RequiredItem1Count);
            }
            if (RequiredItem2EQID != -1)
            {
                if (itemTemplatesByEQDBID.ContainsKey(RequiredItem2EQID) == false)
                {
                    Logger.WriteDebug(string.Concat("Quest '", Name, "' (", QuestIDWOW, ") could not be mapped as the EQItemID '", RequiredItem2EQID, "' did not exist"));
                    return false;
                }
                RequiredItem2WOWID = itemTemplatesByEQDBID[RequiredItem2EQID].WOWEntryID;
                AddObjectiveItems(itemTemplatesByEQDBID[RequiredItem2EQID].Name, RequiredItem2Count);
            }
            if (RequiredItem3EQID != -1)
            {
                if (itemTemplatesByEQDBID.ContainsKey(RequiredItem3EQID) == false)
                {
                    Logger.WriteDebug(string.Concat("Quest '", Name, "' (", QuestIDWOW, ") could not be mapped as the EQItemID '", RequiredItem3EQID, "' did not exist"));
                    return false;
                }
                RequiredItem3WOWID = itemTemplatesByEQDBID[RequiredItem3EQID].WOWEntryID;
                AddObjectiveItems(itemTemplatesByEQDBID[RequiredItem3EQID].Name, RequiredItem3Count);
            }
            if (RequiredItem4EQID != -1)
            {
                if (itemTemplatesByEQDBID.ContainsKey(RequiredItem4EQID) == false)
                {
                    Logger.WriteDebug(string.Concat("Quest '", Name, "' (", QuestIDWOW, ") could not be mapped as the EQItemID '", RequiredItem4EQID, "' did not exist"));
                    return false;
                }
                RequiredItem4WOWID = itemTemplatesByEQDBID[RequiredItem4EQID].WOWEntryID;
                AddObjectiveItems(itemTemplatesByEQDBID[RequiredItem4EQID].Name, RequiredItem4Count);
            }
            if (RequiredItem5EQID != -1)
            {
                if (itemTemplatesByEQDBID.ContainsKey(RequiredItem5EQID) == false)
                {
                    Logger.WriteDebug(string.Concat("Quest '", Name, "' (", QuestIDWOW, ") could not be mapped as the EQItemID '", RequiredItem5EQID, "' did not exist"));
                    return false;
                }
                RequiredItem5WOWID = itemTemplatesByEQDBID[RequiredItem5EQID].WOWEntryID;
                AddObjectiveItems(itemTemplatesByEQDBID[RequiredItem5EQID].Name, RequiredItem5Count);
            }
            if (RequiredItem6EQID != -1)
            {
                if (itemTemplatesByEQDBID.ContainsKey(RequiredItem6EQID) == false)
                {
                    Logger.WriteDebug(string.Concat("Quest '", Name, "' (", QuestIDWOW, ") could not be mapped as the EQItemID '", RequiredItem6EQID, "' did not exist"));
                    return false;
                }
                RequiredItem6WOWID = itemTemplatesByEQDBID[RequiredItem6EQID].WOWEntryID;
                AddObjectiveItems(itemTemplatesByEQDBID[RequiredItem6EQID].Name, RequiredItem6Count);
            }

            if (RewardItem1EQID != -1)
            {
                if (itemTemplatesByEQDBID.ContainsKey(RewardItem1EQID) == false)
                {
                    Logger.WriteDebug(string.Concat("Quest '", Name, "' (", QuestIDWOW, ") could not be mapped as the EQItemID '", RewardItem1EQID, "' did not exist"));
                    return false;
                }
                RewardItem1WOWID = itemTemplatesByEQDBID[RewardItem1EQID].WOWEntryID;
            }
            if (RewardItem2EQID != -1)
            {
                if (itemTemplatesByEQDBID.ContainsKey(RewardItem2EQID) == false)
                {
                    Logger.WriteDebug(string.Concat("Quest '", Name, "' (", QuestIDWOW, ") could not be mapped as the EQItemID '", RewardItem2EQID, "' did not exist"));
                    return false;
                }
                RewardItem2WOWID = itemTemplatesByEQDBID[RewardItem2EQID].WOWEntryID;
            }
            if (RewardItem3EQID != -1)
            {
                if (itemTemplatesByEQDBID.ContainsKey(RewardItem3EQID) == false)
                {
                    Logger.WriteDebug(string.Concat("Quest '", Name, "' (", QuestIDWOW, ") could not be mapped as the EQItemID '", RewardItem3EQID, "' did not exist"));
                    return false;
                }
                RewardItem3WOWID = itemTemplatesByEQDBID[RewardItem3EQID].WOWEntryID;
            }

            return true;
        }
    }
}
