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

namespace EQWOWConverter.WOWFiles
{
    internal class QuestTemplateSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM quest_template WHERE `ID` >= " + Configuration.SQL_QUEST_TEMPLATE_ID_START.ToString() + " AND `ID` <= " + Configuration.SQL_QUEST_TEMPLATE_ID_END + ";";
        }

        public void AddRow()
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("ID", 0);
            newRow.AddInt("QuestType", 2);
            newRow.AddInt("QuestLevel", 1);
            newRow.AddInt("MinLevel", 0);
            newRow.AddInt("QuestSortID", 0);
            newRow.AddInt("QuestInfoID", 0);
            newRow.AddInt("SuggestedGroupNum", 0);
            newRow.AddInt("RequiredFactionId1", 0);
            newRow.AddInt("RequiredFactionId2", 0);
            newRow.AddInt("RequiredFactionValue1", 0);
            newRow.AddInt("RequiredFactionValue2", 0);
            newRow.AddInt("RewardNextQuest", 0);
            newRow.AddInt("RewardXPDifficulty", 0);
            newRow.AddInt("RewardMoney", 0);
            newRow.AddInt("RewardMoneyDifficulty", 0);
            newRow.AddInt("RewardDisplaySpell", 0);
            newRow.AddInt("RewardSpell", 0);
            newRow.AddInt("RewardHonor", 0);
            newRow.AddFloat("RewardKillHonor", 0);
            newRow.AddInt("StartItem", 0);
            newRow.AddInt("Flags", 0);
            newRow.AddInt("RequiredPlayerKills", 0);
            newRow.AddInt("RewardItem1", 0);
            newRow.AddInt("RewardAmount1", 0);
            newRow.AddInt("RewardItem2", 0);
            newRow.AddInt("RewardAmount2", 0);
            newRow.AddInt("RewardItem3", 0);
            newRow.AddInt("RewardAmount3", 0);
            newRow.AddInt("RewardItem4", 0);
            newRow.AddInt("RewardAmount4", 0);
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
            newRow.AddString("LogTitle", "");
            newRow.AddString("LogDescription", "");
            newRow.AddString("QuestDescription", "");
            newRow.AddString("AreaDescription", "");
            newRow.AddString("QuestCompletionLog", "");
            newRow.AddInt("RequiredNpcOrGo1", 0);
            newRow.AddInt("RequiredNpcOrGo2", 0);
            newRow.AddInt("RequiredNpcOrGo3", 0);
            newRow.AddInt("RequiredNpcOrGo4", 0);
            newRow.AddInt("RequiredNpcOrGoCount1", 0);
            newRow.AddInt("RequiredNpcOrGoCount2", 0);
            newRow.AddInt("RequiredNpcOrGoCount3", 0);
            newRow.AddInt("RequiredNpcOrGoCount4", 0);
            newRow.AddInt("RequiredItemId1", 0);
            newRow.AddInt("RequiredItemId2", 0);
            newRow.AddInt("RequiredItemId3", 0);
            newRow.AddInt("RequiredItemId4", 0);
            newRow.AddInt("RequiredItemId5", 0);
            newRow.AddInt("RequiredItemId6", 0);
            newRow.AddInt("RequiredItemCount1", 0);
            newRow.AddInt("RequiredItemCount2", 0);
            newRow.AddInt("RequiredItemCount3", 0);
            newRow.AddInt("RequiredItemCount4", 0);
            newRow.AddInt("RequiredItemCount5", 0);
            newRow.AddInt("RequiredItemCount6", 0);
            newRow.AddInt("Unknown0", 0);
            newRow.AddString("ObjectiveText1", "");
            newRow.AddString("ObjectiveText2", "");
            newRow.AddString("ObjectiveText3", "");
            newRow.AddString("ObjectiveText4", "");
            newRow.AddInt("VerifiedBuild", 12340);
            Rows.Add(newRow);
        }
    }
}
