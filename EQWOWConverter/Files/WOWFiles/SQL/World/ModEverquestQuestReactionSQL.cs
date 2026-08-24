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
    internal class ModEverquestQuestReactionSQL : SQLFile
    {
        private static int CUR_ID = 0;

        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DROP TABLE IF EXISTS `mod_everquest_quest_reaction`; ");
            stringBuilder.AppendLine("CREATE TABLE IF NOT EXISTS `mod_everquest_quest_reaction` ( ");
            stringBuilder.AppendLine("`ID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`QuestTemplateID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`ReactionType` INT(10) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`UsePlayerX` TINYINT(3) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`UsePlayerY` TINYINT(3) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`UsePlayerZ` TINYINT(3) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`AddedPlayerX` FLOAT NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`AddedPlayerY` FLOAT NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`UsePlayerOrientation` TINYINT(3) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`PositionX` FLOAT NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`PositionY` FLOAT NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`PositionZ` FLOAT NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`Orientation` FLOAT NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`CreatureTemplateID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`QuestgiverCreatureTemplateID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`DelayInMS` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`SayText` VARCHAR(512) NOT NULL DEFAULT '', ");
            stringBuilder.AppendLine("`UseNpcX` TINYINT(3) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`UseNpcY` TINYINT(3) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`UseNpcZ` TINYINT(3) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`UseNpcOrientation` TINYINT(3) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`MovementIsRun` TINYINT(3) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`FiresOnArrival` TINYINT(3) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("PRIMARY KEY(`ID`) USING BTREE ); ");
            return stringBuilder.ToString();
        }

        public void AddRow(int questTemplateID, int questgiverCreatureTemplateID, QuestReaction questReaction)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("ID", CUR_ID);
            newRow.AddInt("QuestTemplateID", questTemplateID);
            newRow.AddInt("ReactionType", (Int32)questReaction.ReactionType);
            newRow.AddInt("UsePlayerX", questReaction.UsePlayerX == false ? 0 : 1);
            newRow.AddInt("UsePlayerY", questReaction.UsePlayerY == false ? 0 : 1);
            newRow.AddInt("UsePlayerZ", questReaction.UsePlayerZ == false ? 0 : 1);
            newRow.AddFloat("AddedPlayerX", questReaction.AddedX);
            newRow.AddFloat("AddedPlayerY", questReaction.AddedY);
            newRow.AddInt("UsePlayerOrientation", questReaction.UsePlayerHeading == false ? 0 : 1);
            newRow.AddFloat("PositionX", questReaction.PositionX);
            newRow.AddFloat("PositionY", questReaction.PositionY);
            newRow.AddFloat("PositionZ", questReaction.PositionZ);
            newRow.AddFloat("Orientation", questReaction.WOWOrientation);
            newRow.AddInt("CreatureTemplateID", questReaction.CreatureWOWID);
            newRow.AddInt("QuestgiverCreatureTemplateID", questgiverCreatureTemplateID);
            newRow.AddInt("DelayInMS", questReaction.DelayInMS);
            newRow.AddString("SayText", 512, questReaction.ReactionValue);
            newRow.AddInt("UseNpcX", questReaction.UseNpcX == false ? 0 : 1);
            newRow.AddInt("UseNpcY", questReaction.UseNpcY == false ? 0 : 1);
            newRow.AddInt("UseNpcZ", questReaction.UseNpcZ == false ? 0 : 1);
            newRow.AddInt("UseNpcOrientation", questReaction.UseNpcHeading == false ? 0 : 1);
            newRow.AddInt("MovementIsRun", questReaction.MovementIsRun == false ? 0 : 1);
            newRow.AddInt("FiresOnArrival", questReaction.FiresOnArrival == false ? 0 : 1);
            Rows.Add(newRow);

            CUR_ID++;
        }
    }
}
