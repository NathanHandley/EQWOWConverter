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

namespace EQWOWConverter.WOWFiles
{
    internal class QuestTemplateAddonSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM quest_template_addon WHERE `ID` >= " + Configuration.SQL_QUEST_TEMPLATE_ID_START.ToString() + " AND `ID` <= " + Configuration.SQL_QUEST_TEMPLATE_ID_END + ";";
        }

        public void AddRow(QuestTemplate questTemplate, int curQuestTemplateID, int prevQuestTemplateID, bool isRepeatable)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("ID", curQuestTemplateID);
            newRow.AddInt("MaxLevel", 0);
            newRow.AddInt("AllowableClasses", 0);
            newRow.AddInt("SourceSpellID", 0);
            newRow.AddInt("PrevQuestID", prevQuestTemplateID);
            newRow.AddInt("NextQuestID", 0);
            newRow.AddInt("ExclusiveGroup", 0);
            newRow.AddInt("RewardMailTemplateID", 0);
            newRow.AddInt("RewardMailDelay", 0);
            newRow.AddInt("RequiredSkillID", 0);
            newRow.AddInt("RequiredSkillPoints", 0);
            newRow.AddInt("RequiredMinRepFaction", 0);
            newRow.AddInt("RequiredMaxRepFaction", 0);
            newRow.AddInt("RequiredMinRepValue", 0);
            newRow.AddInt("RequiredMaxRepValue", 0);
            newRow.AddInt("ProvidedItemCount", 0);
            newRow.AddInt("SpecialFlags", isRepeatable == true ? 1 : 0); // QUEST_SPECIAL_FLAGS_REPEATABLE
            Rows.Add(newRow);
        }
    }
}
