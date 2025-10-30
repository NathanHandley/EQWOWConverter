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
using EQWOWConverter.Items;

namespace EQWOWConverter.Quests
{
    internal class QuestTemplate
    {
        internal class QuestItemReference
        {
            public int itemIDEQ;
            public int itemIDWOW = 0;
            public int itemCount;
        }

        private static List<QuestTemplate> QuestTemplates = new List<QuestTemplate>();
        private static readonly object QuestLock = new object();

        public int QuestIDWOW;
        public string Name = string.Empty;
        public string ZoneShortName = string.Empty;
        public string QuestgiverName = string.Empty;
        public List<int> QuestgiverWOWCreatureTemplateIDs = new List<int>();
        public bool HasMinimumFactionRequirement = false;
        public int QuestgiverWOWFactionID = 0;
        public int MinimumQuestgiverFactionValue = 0;
        public int QuestLevel = -1;
        public int RequiredMoneyInCopper = 0;
        public List<QuestItemReference> RequiredItems = new List<QuestItemReference>();
        public int RewardMoneyInCopper;
        public int RewardExperience;
        public List<int> RewardItemEQIDs = new List<int>();
        public List<int> RewardItemWOWIDs = new List<int>();
        public List<int> RewardItemCounts = new List<int>();
        public List<float> RewardItemChances = new List<float>();
        // TODO: Faction
        public string RequestText = string.Empty;
        public string RequestObjectiveText = string.Empty;
        public int AreaID = 0;
        public List<QuestReaction> Reactions = new List<QuestReaction>();
        private int NumOfObjectiveItemsAddedToText = 0;
        public List<QuestCompletionFactionReward> questCompletionFactionRewards = new List<QuestCompletionFactionReward>();
        public ItemTemplate? RandomAwardContainerItemTemplate = null;
        public bool HasInvalidItems = false;
        public bool IsValidQuest = true;
        public int MultiRewardContainerWOWItemID = -1;

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

        public bool AreRequiredItemsPlayerObtainable(SortedDictionary<int, ItemTemplate> itemTemplatesByWOWEntryID)
        {
            foreach (QuestItemReference requiredItem in RequiredItems)
            {
                if (requiredItem.itemIDWOW == 0)
                    continue;
                if (itemTemplatesByWOWEntryID.ContainsKey(requiredItem.itemIDWOW) == false)
                    return false;
                if (itemTemplatesByWOWEntryID[requiredItem.itemIDWOW].IsPlayerObtainable() == false)
                    return false;
            }
            return true;
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
            // Load the reactions
            string questReactionsFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "QuestReactions.csv");
            Logger.WriteDebug(string.Concat("Loading quest reactions via file '", questReactionsFile, "'"));
            Dictionary<int, List<QuestReaction>> reactionsByQuestID = new Dictionary<int, List<QuestReaction>>();
            List<Dictionary<string, string>> reactionRows = FileTool.ReadAllRowsFromFileWithHeader(questReactionsFile, "|");
            foreach (Dictionary<string, string> columns in reactionRows)
            {
                QuestReaction reaction = new QuestReaction();
                int questID = int.Parse(columns["wow_questid"]);
                string reactionTypeString = columns["reaction_type"];
                string reactionValue1 = columns["reaction_value1"];
                switch (reactionTypeString)
                {
                    case "attackplayer":
                        {
                            reaction.type = QuestReactionType.AttackPlayer;
                        } break;
                    case "despawn":
                        {
                            reaction.type = QuestReactionType.Despawn;
                            if (reactionValue1 == "self")
                                reaction.TargetSelf = true;
                            else
                                reaction.TargetCreatureEQID = int.Parse(reactionValue1);
                        } break;
                    case "emote":
                        {
                            reaction.type = QuestReactionType.Emote;
                            reaction.ReactionValue = reactionValue1;
                        } break;
                    case "say":
                        {
                            reaction.type = QuestReactionType.Say;
                            reaction.ReactionValue = reactionValue1;
                        } break;
                    case "spawn":
                        {
                            reaction.type = QuestReactionType.Spawn;
                            reaction.TargetCreatureEQID = int.Parse(reactionValue1);
                            string reactionValue2 = columns["reaction_value2"];
                            if (reactionValue2 == "playerX")
                                reaction.UsePlayerX = true;
                            else
                                reaction.X = ParseTool.ParseFloat(reactionValue2, 0);
                            string reactionValue3 = columns["reaction_value3"];
                            if (reactionValue3 == "playerY")
                                reaction.UsePlayerY = true;
                            else
                                reaction.Y = ParseTool.ParseFloat(reactionValue3, 0);
                            string reactionValue4 = columns["reaction_value4"];
                            if (reactionValue4 == "playerZ")
                                reaction.UsePlayerZ = true;
                            else
                                reaction.Z = ParseTool.ParseFloat(reactionValue4, 0);
                            string reactionValue5 = columns["reaction_value5"];
                            if (reactionValue5 == "playerHeading")
                                reaction.UsePlayerHeading = true;
                            else
                                reaction.Heading = ParseTool.ParseFloat(reactionValue5, 0); // TODO: Convert this
                            string reactionValue6 = columns["reaction_value6"];
                            if (reactionValue6.Length > 0)
                                reaction.AddedX = ParseTool.ParseFloat(reactionValue6, 0);
                            string reactionValue7 = columns["reaction_value7"];
                            if (reactionValue7.Length > 0)
                                reaction.AddedY = ParseTool.ParseFloat(reactionValue7, 0);
                        } break;
                    case "spawnunique":
                        {
                            reaction.type = QuestReactionType.SpawnUnique;
                            reaction.TargetCreatureEQID = int.Parse(reactionValue1);
                            string reactionValue2 = columns["reaction_value2"];
                            if (reactionValue2 == "playerX")
                                reaction.UsePlayerX = true;
                            else
                                reaction.X = ParseTool.ParseFloat(reactionValue2, 0);
                            string reactionValue3 = columns["reaction_value3"];
                            if (reactionValue3 == "playerY")
                                reaction.UsePlayerY = true;
                            else
                                reaction.Y = ParseTool.ParseFloat(reactionValue3, 0);
                            string reactionValue4 = columns["reaction_value4"];
                            if (reactionValue4 == "playerZ")
                                reaction.UsePlayerZ = true;
                            else
                                reaction.Z = ParseTool.ParseFloat(reactionValue4, 0);
                            string reactionValue5 = columns["reaction_value5"];
                            if (reactionValue5 == "playerHeading")
                                reaction.UsePlayerHeading = true;
                            else
                                reaction.Heading = ParseTool.ParseFloat(reactionValue5, 0); // TODO: Convert this
                            string reactionValue6 = columns["reaction_value6"];
                            if (reactionValue6.Length > 0)
                                reaction.AddedX = ParseTool.ParseFloat(reactionValue6, 0);
                            string reactionValue7 = columns["reaction_value7"];
                            if (reactionValue7.Length > 0)
                                reaction.AddedY = ParseTool.ParseFloat(reactionValue7, 0);
                        }
                        break;
                    case "yell":
                        {
                            reaction.type = QuestReactionType.Yell;
                            reaction.ReactionValue = reactionValue1;
                        }
                        break;
                    default:
                        {
                            Logger.WriteError(string.Concat("Unhandled reaction type of '", reactionTypeString, "'"));
                            continue;
                        }
                }
                if (reactionsByQuestID.ContainsKey(questID) == false)
                    reactionsByQuestID.Add(questID, new List<QuestReaction>());
                reactionsByQuestID[questID].Add(reaction);
            }

            // Load quest templates
            string questTemplateFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "QuestTemplates.csv");
            Logger.WriteDebug(string.Concat("Populating quest templates via file '", questTemplateFile, "'"));
            List<Dictionary<string, string>> questRows = FileTool.ReadAllRowsFromFileWithHeader(questTemplateFile, "|");
            foreach (Dictionary<string, string> columns in questRows)
            {
                // Skip invalid expansions and disabled quests
                if (int.Parse(columns["min_expansion"]) > Configuration.GENERATE_EQ_EXPANSION_ID_GENERAL)
                    continue;
                if (int.Parse(columns["enabled"]) == 0)
                    continue;

                // Load the row
                QuestTemplate newQuestTemplate = new QuestTemplate();
                newQuestTemplate.QuestIDWOW = int.Parse(columns["wow_questid"]);
                newQuestTemplate.ZoneShortName = columns["zone_shortname"];
                newQuestTemplate.QuestgiverName = columns["questgiver_name"];
                newQuestTemplate.QuestLevel = int.Parse(columns["level"]);
                int minRep = int.Parse(columns["req_repmin"]);
                newQuestTemplate.MinimumQuestgiverFactionValue = minRep == -1 ? 0 : ConvertEQFactionValueToWoW(minRep);
                newQuestTemplate.HasMinimumFactionRequirement = minRep == -1 ? false : true;
                newQuestTemplate.RequiredMoneyInCopper = int.Parse(columns["req_copper"]);
                newQuestTemplate.RequiredItems.Add(new QuestItemReference());
                newQuestTemplate.RequiredItems[0].itemIDEQ = int.Parse(columns["req_item_id1"]);
                newQuestTemplate.RequiredItems[0].itemCount = int.Parse(columns["req_item_count1"]);
                newQuestTemplate.RequiredItems.Add(new QuestItemReference());
                newQuestTemplate.RequiredItems[1].itemIDEQ = int.Parse(columns["req_item_id2"]);
                newQuestTemplate.RequiredItems[1].itemCount = int.Parse(columns["req_item_count2"]);
                newQuestTemplate.RequiredItems.Add(new QuestItemReference());
                newQuestTemplate.RequiredItems[2].itemIDEQ = int.Parse(columns["req_item_id3"]);
                newQuestTemplate.RequiredItems[2].itemCount = int.Parse(columns["req_item_count3"]);
                newQuestTemplate.RequiredItems.Add(new QuestItemReference());
                newQuestTemplate.RequiredItems[3].itemIDEQ = int.Parse(columns["req_item_id4"]);
                newQuestTemplate.RequiredItems[3].itemCount = int.Parse(columns["req_item_count4"]);
                newQuestTemplate.RequiredItems.Add(new QuestItemReference());
                newQuestTemplate.RequiredItems[4].itemIDEQ = int.Parse(columns["req_item_id5"]);
                newQuestTemplate.RequiredItems[4].itemCount = int.Parse(columns["req_item_count5"]);
                newQuestTemplate.RequiredItems.Add(new QuestItemReference());
                newQuestTemplate.RequiredItems[5].itemIDEQ = int.Parse(columns["req_item_id6"]);
                newQuestTemplate.RequiredItems[5].itemCount = int.Parse(columns["req_item_count6"]);
                newQuestTemplate.RewardMoneyInCopper = int.Parse(columns["reward_money"]);
                newQuestTemplate.RewardExperience = int.Parse(columns["reward_exp"]);
                for (int i = 1; i <= 38; i++)
                {
                    int rewardItemID = int.Parse(columns[string.Concat("reward_item_ID", i)]);
                    if (rewardItemID == -1)
                        break;

                    newQuestTemplate.RewardItemEQIDs.Add(rewardItemID);
                    newQuestTemplate.RewardItemCounts.Add(int.Parse(columns[string.Concat("reward_item_count", i)]));
                    newQuestTemplate.RewardItemChances.Add(int.Parse(columns[string.Concat("reward_item_chance", i)]));
                }
                List<int> rewardFactionEQIDs = new List<int>();
                List<int> rewardFactionValues = new List<int>();
                for (int i = 1; i <= 6; i++)
                {
                    rewardFactionEQIDs.Add(int.Parse(columns[string.Concat("reward_faction", i, "ID")]));
                    rewardFactionValues.Add(int.Parse(columns[string.Concat("reward_faction", i, "Amt")]));
                }
                newQuestTemplate.questCompletionFactionRewards = GetCompletionFactionRewards(rewardFactionEQIDs, rewardFactionValues);
                string questName = columns["quest_name"];
                string firstRewardItem = columns["reward_item1_name"];
                newQuestTemplate.Name = GetOrGenerateName(questName, newQuestTemplate.QuestgiverName, newQuestTemplate.QuestIDWOW, firstRewardItem, newQuestTemplate.RewardItemCounts);
                //newQuestTemplate.RequestText = columns["request_text"]; Ignoring for now
                if (reactionsByQuestID.ContainsKey(newQuestTemplate.QuestIDWOW))
                {
                    foreach (QuestReaction reaction in reactionsByQuestID[newQuestTemplate.QuestIDWOW])
                        newQuestTemplate.Reactions.Add(reaction);
                }
                newQuestTemplate.MultiRewardContainerWOWItemID = int.Parse(columns["reward_container_wowid"]);
                QuestTemplates.Add(newQuestTemplate);
            }
        }

        private static string GetOrGenerateName(string questName, string questGiverName, int questID, string firstRewardItemName, List<int> rewardItemCounts)
        {
            if (questName.Length != 0)
                return questName;
            if (rewardItemCounts.Count == 1 && rewardItemCounts[0] == 1)
                return firstRewardItemName;
            return String.Concat(questGiverName, " Quest (", questID, ")");
        }

        private static List<QuestCompletionFactionReward> GetCompletionFactionRewards(List<int> eqFactionIDs, List<int> factionValues)
        {
            // Generate the initial faction rewards based on what the quest is configured to give
            List<QuestCompletionFactionReward> initialFactionRewards = new List<QuestCompletionFactionReward>();
            for (int i = 0; i < eqFactionIDs.Count; i++)
            {
                // Stop if we hit -1s since that's a nothing, and nothing will follow
                if (eqFactionIDs[i] == -1)
                    break;

                QuestCompletionFactionReward curQuestFactionReward = new QuestCompletionFactionReward();
                curQuestFactionReward.EQFactionID = eqFactionIDs[i];
                curQuestFactionReward.WOWFactionID = CreatureFaction.GetWOWFactionIDForEQFactionID(eqFactionIDs[i]);
                curQuestFactionReward.CompletionRewardValue = factionValues[i] * Configuration.CREATURE_REP_REWARD_MULTIPLIER;
                initialFactionRewards.Add(curQuestFactionReward);
            }

            // Collapse/condense same factions since factions are merged up in this project
            List<QuestCompletionFactionReward> collapsedFactionRewards = new List<QuestCompletionFactionReward>();
            foreach (QuestCompletionFactionReward initialFactionReward in initialFactionRewards)
            {
                bool rewardFound = false;
                for (int i = collapsedFactionRewards.Count - 1; i >= 0; i--)
                {
                    // Take the highest or lowest faction reward
                    QuestCompletionFactionReward candidateRewardStage = collapsedFactionRewards[i];
                    if (candidateRewardStage.WOWFactionID == initialFactionReward.WOWFactionID)
                    {
                        if (Math.Abs(initialFactionReward.CompletionRewardValue) > Math.Abs(candidateRewardStage.CompletionRewardValue))
                            collapsedFactionRewards.RemoveAt(i);
                        else
                        {
                            rewardFound = true;
                            continue;
                        }
                    }
                }
                if (rewardFound == false)
                    collapsedFactionRewards.Add(initialFactionReward);
            }

            // Store and sort the list and set the sort order for rewards
            collapsedFactionRewards.Sort();
            for (int i = 0; i < collapsedFactionRewards.Count; i++)
                collapsedFactionRewards[i].SortOrder = i + 1;

            return collapsedFactionRewards;
        }

        public bool MapWOWItemIDs(SortedDictionary<int, ItemTemplate> itemTemplatesByEQDBID)
        {
            // Required Items
            for (int i = 0; i < RequiredItems.Count; i++)
            {
                if (RequiredItems[i].itemIDEQ == -1)
                    continue;
                if (itemTemplatesByEQDBID.ContainsKey(RequiredItems[i].itemIDEQ) == false)
                {
                    Logger.WriteDebug(string.Concat("Quest '", Name, "' (", QuestIDWOW, ") could not be mapped as the EQItemID '", RequiredItems[i].itemIDEQ, "' did not exist"));
                    HasInvalidItems = true;
                    return false;
                }
                RequiredItems[i].itemIDWOW = itemTemplatesByEQDBID[RequiredItems[i].itemIDEQ].WOWEntryID;
                AddObjectiveItems(itemTemplatesByEQDBID[RequiredItems[i].itemIDEQ].Name, RequiredItems[i].itemCount);
            }

            // Reward items
            for (int i = 0; i < RewardItemEQIDs.Count; i++)
            {
                if (itemTemplatesByEQDBID.ContainsKey(RewardItemEQIDs[i]) == false)
                {
                    Logger.WriteDebug(string.Concat("Quest '", Name, "' (", QuestIDWOW, ") could not be mapped as the EQItemID '", RewardItemEQIDs[i], "' did not exist"));
                    HasInvalidItems = true;
                    return false;
                }
                RewardItemWOWIDs.Add(itemTemplatesByEQDBID[RewardItemEQIDs[i]].WOWEntryID);
            }
            return true;
        }
    }
}
