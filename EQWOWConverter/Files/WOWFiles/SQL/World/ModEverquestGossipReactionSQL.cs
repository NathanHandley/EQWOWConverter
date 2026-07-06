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

using EQWOWConverter.Quests;
using System.Text;

namespace EQWOWConverter.WOWFiles
{
    internal class ModEverquestGossipReactionSQL : SQLFile
    {
        private static int CUR_ID = 0;

        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DROP TABLE IF EXISTS `mod_everquest_gossip_reaction`; ");
            stringBuilder.AppendLine("CREATE TABLE IF NOT EXISTS `mod_everquest_gossip_reaction` ( ");
            stringBuilder.AppendLine("`ID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`GossipCreatureTemplateID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`NpcTextID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`OptionID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`OptionText` VARCHAR(255) NOT NULL DEFAULT '', ");
            stringBuilder.AppendLine("`ReactionType` INT(10) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`SayText` VARCHAR(512) NOT NULL DEFAULT '', ");
            stringBuilder.AppendLine("`TargetCreatureTemplateID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`UsePlayerX` TINYINT(3) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`UsePlayerY` TINYINT(3) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`UsePlayerZ` TINYINT(3) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`AddedPlayerX` FLOAT NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`AddedPlayerY` FLOAT NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`UsePlayerOrientation` TINYINT(3) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`UseNpcX` TINYINT(3) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`UseNpcY` TINYINT(3) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`UseNpcZ` TINYINT(3) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`UseNpcOrientation` TINYINT(3) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`PositionX` FLOAT NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`PositionY` FLOAT NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`PositionZ` FLOAT NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`Orientation` FLOAT NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`DelayInMS` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("PRIMARY KEY(`ID`) USING BTREE ); ");
            return stringBuilder.ToString();
        }

        public void AddRow(int gossipCreatureTemplateID, int npcTextID, QuestGossipReaction gossipReaction, int targetCreatureTemplateID)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("ID", CUR_ID);
            newRow.AddInt("GossipCreatureTemplateID", gossipCreatureTemplateID);
            newRow.AddInt("NpcTextID", npcTextID);
            newRow.AddInt("OptionID", gossipReaction.OptionID);
            newRow.AddString("OptionText", 255, gossipReaction.OptionText);
            newRow.AddInt("ReactionType", (Int32)gossipReaction.ReactionType);
            newRow.AddString("SayText", 512, gossipReaction.ReactionValue);
            newRow.AddInt("TargetCreatureTemplateID", targetCreatureTemplateID);
            newRow.AddInt("UsePlayerX", gossipReaction.UsePlayerX == false ? 0 : 1);
            newRow.AddInt("UsePlayerY", gossipReaction.UsePlayerY == false ? 0 : 1);
            newRow.AddInt("UsePlayerZ", gossipReaction.UsePlayerZ == false ? 0 : 1);
            newRow.AddFloat("AddedPlayerX", gossipReaction.AddedX);
            newRow.AddFloat("AddedPlayerY", gossipReaction.AddedY);
            newRow.AddInt("UsePlayerOrientation", gossipReaction.UsePlayerHeading == false ? 0 : 1);
            newRow.AddInt("UseNpcX", gossipReaction.UseNpcX == false ? 0 : 1);
            newRow.AddInt("UseNpcY", gossipReaction.UseNpcY == false ? 0 : 1);
            newRow.AddInt("UseNpcZ", gossipReaction.UseNpcZ == false ? 0 : 1);
            newRow.AddInt("UseNpcOrientation", gossipReaction.UseNpcHeading == false ? 0 : 1);
            newRow.AddFloat("PositionX", gossipReaction.PositionX);
            newRow.AddFloat("PositionY", gossipReaction.PositionY);
            newRow.AddFloat("PositionZ", gossipReaction.PositionZ);
            newRow.AddFloat("Orientation", gossipReaction.WOWOrientation);
            newRow.AddInt("DelayInMS", gossipReaction.DelayInMS);
            Rows.Add(newRow);

            CUR_ID++;
        }
    }
}
