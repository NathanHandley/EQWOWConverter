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

using EQWOWConverter.Quests;
using System.Text;

namespace EQWOWConverter.WOWFiles
{
    internal class ModEverquestQuestCompleteReputationSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DROP TABLE IF EXISTS `mod_everquest_quest_complete_reputation`; ");
            stringBuilder.AppendLine("CREATE TABLE IF NOT EXISTS `mod_everquest_quest_complete_reputation` ( ");
            stringBuilder.AppendLine("`QuestTemplateID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`SortOrder` TINYINT(3) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`FactionID` SMALLINT(5) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`CompletionRewardValue` INT(10) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("PRIMARY KEY(`QuestTemplateID`, `SortOrder`) USING BTREE ); ");
            return stringBuilder.ToString();
        }

        public void AddRow(int questTemplateID, QuestCompletionFactionReward completionFactionReward)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("QuestTemplateID", questTemplateID);
            newRow.AddInt("SortOrder", completionFactionReward.SortOrder);
            newRow.AddInt("FactionID", completionFactionReward.WOWFactionID);
            newRow.AddInt("CompletionRewardValue", completionFactionReward.CompletionRewardValue);
            Rows.Add(newRow);
        }
    }
}
