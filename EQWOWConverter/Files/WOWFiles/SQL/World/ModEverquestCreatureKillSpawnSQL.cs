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

using System.Text;

namespace EQWOWConverter.WOWFiles
{
    internal class ModEverquestCreatureKillSpawnSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DROP TABLE IF EXISTS `mod_everquest_creature_kill_spawn`; ");
            stringBuilder.AppendLine("CREATE TABLE IF NOT EXISTS `mod_everquest_creature_kill_spawn` ( ");
            stringBuilder.AppendLine("`ID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`TriggerCreatureTemplateID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`TriggerTypeID` TINYINT(3) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`MapID` INT(10) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`ActionType` TINYINT(3) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`TargetCreatureTemplateID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`Chance` FLOAT NOT NULL DEFAULT '100', ");
            stringBuilder.AppendLine("`AltGroup` INT(10) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`AltID` INT(10) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`AltWeight` FLOAT NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`SpawnAtCorpse` TINYINT(3) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`PositionX` FLOAT NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`PositionY` FLOAT NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`PositionZ` FLOAT NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`Orientation` FLOAT NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`DelayMinMS` INT(10) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`DelayMaxMS` INT(10) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`OnlyIfNotAliveCreatureTemplateID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`RequireDeadCreatureTemplateIDs` VARCHAR(128) NOT NULL DEFAULT '', ");
            stringBuilder.AppendLine("`RequireAliveCreatureTemplateIDs` VARCHAR(128) NOT NULL DEFAULT '', ");
            stringBuilder.AppendLine("`AddToHateList` TINYINT(3) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`TriggerMinLevel` INT(10) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`TriggerMaxLevel` INT(10) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`RespawnTimeSec` INT(10) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`Comment` VARCHAR(128) NOT NULL DEFAULT '', ");
            stringBuilder.AppendLine("PRIMARY KEY (`ID`) USING BTREE ); ");
            return stringBuilder.ToString();
        }

        public void AddRow(int id, int triggerCreatureTemplateID, int triggerTypeID, int mapID, int actionType, int targetCreatureTemplateID,
            float chance, int altGroup, int altID, float altWeight, bool spawnAtCorpse, float positionX, float positionY,
            float positionZ, float orientation, int delayMinMS, int delayMaxMS, int onlyIfNotAliveCreatureTemplateID,
            string requireDeadCreatureTemplateIDs, string requireAliveCreatureTemplateIDs, bool addToHateList,
            int triggerMinLevel, int triggerMaxLevel, int respawnTimeSec, string comment)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("ID", id);
            newRow.AddInt("TriggerCreatureTemplateID", triggerCreatureTemplateID);
            newRow.AddInt("TriggerTypeID", triggerTypeID);
            newRow.AddInt("MapID", mapID);
            newRow.AddInt("ActionType", actionType);
            newRow.AddInt("TargetCreatureTemplateID", targetCreatureTemplateID);
            newRow.AddFloat("Chance", chance);
            newRow.AddInt("AltGroup", altGroup);
            newRow.AddInt("AltID", altID);
            newRow.AddFloat("AltWeight", altWeight);
            newRow.AddInt("SpawnAtCorpse", spawnAtCorpse ? 1 : 0);
            newRow.AddFloat("PositionX", positionX);
            newRow.AddFloat("PositionY", positionY);
            newRow.AddFloat("PositionZ", positionZ);
            newRow.AddFloat("Orientation", orientation);
            newRow.AddInt("DelayMinMS", delayMinMS);
            newRow.AddInt("DelayMaxMS", delayMaxMS);
            newRow.AddInt("OnlyIfNotAliveCreatureTemplateID", onlyIfNotAliveCreatureTemplateID);
            newRow.AddString("RequireDeadCreatureTemplateIDs", 128, requireDeadCreatureTemplateIDs);
            newRow.AddString("RequireAliveCreatureTemplateIDs", 128, requireAliveCreatureTemplateIDs);
            newRow.AddInt("AddToHateList", addToHateList ? 1 : 0);
            newRow.AddInt("TriggerMinLevel", triggerMinLevel);
            newRow.AddInt("TriggerMaxLevel", triggerMaxLevel);
            newRow.AddInt("RespawnTimeSec", respawnTimeSec);
            newRow.AddString("Comment", 128, comment);
            Rows.Add(newRow);
        }
    }
}
