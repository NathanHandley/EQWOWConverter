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

using EQWOWConverter.Quests;

// $B - line break, $N - name, $R - race, $C - class, $Gmale:female;, %s = self

namespace EQWOWConverter.WOWFiles
{
    internal class QuestTemplateSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM quest_template WHERE `ID` >= " + Configuration.SQL_QUEST_TEMPLATE_ID_START.ToString() + " AND `ID` <= " + Configuration.SQL_QUEST_TEMPLATE_ID_END + ";";
        }

        public void AddRow(QuestTemplate questTemplate, int questID, string questName)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("ID", questID);
            newRow.AddInt("QuestType", 2);
            newRow.AddInt("QuestLevel", questTemplate.QuestLevel); // -1 = player's level will be used to calculate completion exp
            newRow.AddInt("MinLevel", 1);
            newRow.AddInt("QuestSortID", questTemplate.AreaID); // > 0 then the value is the Zone ID from AreaTable.dbc
            newRow.AddInt("QuestInfoID", 0); // References QuestInfo.dbc
            newRow.AddInt("SuggestedGroupNum", 0);
            newRow.AddInt("RequiredFactionId1", questTemplate.QuestgiverWOWFactionID);
            newRow.AddInt("RequiredFactionId2", 0);
            newRow.AddInt("RequiredFactionValue1", questTemplate.MinimumQuestgiverFactionValue);
            newRow.AddInt("RequiredFactionValue2", 0);
            newRow.AddInt("RewardNextQuest", 0);
            newRow.AddInt("RewardXPDifficulty", (questTemplate.RewardExperience > 0 && questTemplate.QuestLevel > 0) ? 3 : 0);
            newRow.AddInt("RewardMoney", questTemplate.RequiredMoneyInCopper > 0 ? (-1 * questTemplate.RequiredMoneyInCopper) : questTemplate.RewardMoneyInCopper);
            newRow.AddInt("RewardMoneyDifficulty", 0);
            newRow.AddInt("RewardDisplaySpell", 0);
            newRow.AddInt("RewardSpell", 0);
            newRow.AddInt("RewardHonor", 0);
            newRow.AddFloat("RewardKillHonor", 0);
            newRow.AddInt("StartItem", 0);
            newRow.AddInt("Flags", GetFlags());
            newRow.AddInt("RequiredPlayerKills", 0);
            newRow.AddInt("RewardItem1", questTemplate.RewardItemWOWIDs.Count > 0 ? questTemplate.RewardItemWOWIDs[0] : 0);
            newRow.AddInt("RewardAmount1", questTemplate.RewardItemWOWIDs.Count > 0 ? questTemplate.RewardItemCounts[0] : 0);
            newRow.AddInt("RewardItem2", questTemplate.RewardItemWOWIDs.Count > 1 ? questTemplate.RewardItemWOWIDs[1] : 0);
            newRow.AddInt("RewardAmount2", questTemplate.RewardItemWOWIDs.Count > 1 ? questTemplate.RewardItemCounts[1] : 0);
            newRow.AddInt("RewardItem3", questTemplate.RewardItemWOWIDs.Count > 2 ? questTemplate.RewardItemWOWIDs[2] : 0);
            newRow.AddInt("RewardAmount3", questTemplate.RewardItemWOWIDs.Count > 2 ? questTemplate.RewardItemCounts[2] : 0);
            newRow.AddInt("RewardItem4", questTemplate.RewardItemWOWIDs.Count > 3 ? questTemplate.RewardItemWOWIDs[3] : 0);
            newRow.AddInt("RewardAmount4", questTemplate.RewardItemWOWIDs.Count > 3 ? questTemplate.RewardItemCounts[3] : 0);
            newRow.AddInt("ItemDrop1", 0);
            newRow.AddInt("ItemDropQuantity1", 0);
            newRow.AddInt("ItemDrop2", 0);
            newRow.AddInt("ItemDropQuantity2", 0);
            newRow.AddInt("ItemDrop3", 0);
            newRow.AddInt("ItemDropQuantity3", 0);
            newRow.AddInt("ItemDrop4", 0);
            newRow.AddInt("ItemDropQuantity4", 0);
            newRow.AddInt("RewardChoiceItemID1", 0);
            newRow.AddInt("RewardChoiceItemQuantity1", 0);
            newRow.AddInt("RewardChoiceItemID2", 0);
            newRow.AddInt("RewardChoiceItemQuantity2", 0);
            newRow.AddInt("RewardChoiceItemID3", 0);
            newRow.AddInt("RewardChoiceItemQuantity3", 0);
            newRow.AddInt("RewardChoiceItemID4", 0);
            newRow.AddInt("RewardChoiceItemQuantity4", 0);
            newRow.AddInt("RewardChoiceItemID5", 0);
            newRow.AddInt("RewardChoiceItemQuantity5", 0);
            newRow.AddInt("RewardChoiceItemID6", 0);
            newRow.AddInt("RewardChoiceItemQuantity6", 0);
            newRow.AddInt("POIContinent", 0);
            newRow.AddFloat("POIx", 0);
            newRow.AddFloat("POIy", 0);
            newRow.AddInt("POIPriority", 0);
            newRow.AddInt("RewardTitle", 0);
            newRow.AddInt("RewardTalents", 0);
            newRow.AddInt("RewardArenaPoints", 0);
            newRow.AddInt("RewardFactionID1", 0);
            newRow.AddInt("RewardFactionValue1", 0);
            newRow.AddInt("RewardFactionOverride1", 0);
            newRow.AddInt("RewardFactionID2", 0);
            newRow.AddInt("RewardFactionValue2", 0);
            newRow.AddInt("RewardFactionOverride2", 0);
            newRow.AddInt("RewardFactionID3", 0);
            newRow.AddInt("RewardFactionValue3", 0);
            newRow.AddInt("RewardFactionOverride3", 0);
            newRow.AddInt("RewardFactionID4", 0);
            newRow.AddInt("RewardFactionValue4", 0);
            newRow.AddInt("RewardFactionOverride4", 0);
            newRow.AddInt("RewardFactionID5", 0);
            newRow.AddInt("RewardFactionValue5", 0);
            newRow.AddInt("RewardFactionOverride5", 0);
            newRow.AddInt("TimeAllowed", 0);
            newRow.AddInt("AllowableRaces", 0);
            newRow.AddString("LogTitle", questName);
            newRow.AddString("LogDescription", GenerateQuestDescription(questTemplate));
            newRow.AddString("QuestDescription", string.Empty);
            newRow.AddString("AreaDescription", string.Empty);
            newRow.AddString("QuestCompletionLog", GenerateQuestDescription(questTemplate));
            newRow.AddInt("RequiredNpcOrGo1", 0);
            newRow.AddInt("RequiredNpcOrGo2", 0);
            newRow.AddInt("RequiredNpcOrGo3", 0);
            newRow.AddInt("RequiredNpcOrGo4", 0);
            newRow.AddInt("RequiredNpcOrGoCount1", 0);
            newRow.AddInt("RequiredNpcOrGoCount2", 0);
            newRow.AddInt("RequiredNpcOrGoCount3", 0);
            newRow.AddInt("RequiredNpcOrGoCount4", 0);
            newRow.AddInt("RequiredItemId1", questTemplate.RequiredItem1WOWID);
            newRow.AddInt("RequiredItemId2", questTemplate.RequiredItem2WOWID);
            newRow.AddInt("RequiredItemId3", questTemplate.RequiredItem3WOWID);
            newRow.AddInt("RequiredItemId4", questTemplate.RequiredItem4WOWID);
            newRow.AddInt("RequiredItemId5", questTemplate.RequiredItem5WOWID);
            newRow.AddInt("RequiredItemId6", questTemplate.RequiredItem6WOWID);
            newRow.AddInt("RequiredItemCount1", questTemplate.RequiredItem1Count);
            newRow.AddInt("RequiredItemCount2", questTemplate.RequiredItem2Count);
            newRow.AddInt("RequiredItemCount3", questTemplate.RequiredItem3Count);
            newRow.AddInt("RequiredItemCount4", questTemplate.RequiredItem4Count);
            newRow.AddInt("RequiredItemCount5", questTemplate.RequiredItem5Count);
            newRow.AddInt("RequiredItemCount6", questTemplate.RequiredItem6Count);
            newRow.AddInt("Unknown0", 0);
            newRow.AddString("ObjectiveText1", "");
            newRow.AddString("ObjectiveText2", "");
            newRow.AddString("ObjectiveText3", "");
            newRow.AddString("ObjectiveText4", "");
            newRow.AddInt("VerifiedBuild", 12340);
            Rows.Add(newRow);
        }

        private int GetFlags()
        {
            int flags = 0;
            flags += 8; // QUEST_FLAGS_SHARABLE
            flags += 64; // QUEST_FLAGS_RAID (Can complete in raid)

            return flags;
        }

        private string GenerateQuestObjectives(QuestTemplate questTemplate)
        {
            return "Test Log Description";
        }

        private string GenerateQuestDescription(QuestTemplate questTemplate)
        {
            if (questTemplate.RequestText.Length > 0)
                return questTemplate.RequestText;
            else
                return "I am looking for some things, can you help?";
        }
    }
}
