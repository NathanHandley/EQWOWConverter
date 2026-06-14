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

using EQWOWConverter.Items;
using System.Text;

namespace EQWOWConverter.WOWFiles
{
    internal class ModEverquestCreatureLootSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DROP TABLE IF EXISTS `mod_everquest_creature_loot`; ");
            stringBuilder.AppendLine("CREATE TABLE IF NOT EXISTS `mod_everquest_creature_loot` ( ");
            stringBuilder.AppendLine("`ID` BIGINT(20) UNSIGNED NOT NULL AUTO_INCREMENT, ");
            stringBuilder.AppendLine("`CreatureTemplateID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`LootGroupID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`GroupMultiplier` INT(10) UNSIGNED NOT NULL DEFAULT '1', ");
            stringBuilder.AppendLine("`GroupProbability` FLOAT NOT NULL DEFAULT '100', ");
            stringBuilder.AppendLine("`DropLimit` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`MinDrop` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`ItemTemplateID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`Chance` FLOAT NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`ItemMultiplier` INT(10) UNSIGNED NOT NULL DEFAULT '1', ");
            stringBuilder.AppendLine("`ItemCharges` INT(10) UNSIGNED NOT NULL DEFAULT '1', ");
            stringBuilder.AppendLine("PRIMARY KEY (`ID`) USING BTREE, ");
            stringBuilder.AppendLine("KEY `idx_creature` (`CreatureTemplateID`) USING BTREE ); ");
            return stringBuilder.ToString();
        }

        public void AddRow(CreatureLootEntry lootEntry)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("CreatureTemplateID", lootEntry.CreatureTemplateEntryID);
            newRow.AddInt("LootGroupID", lootEntry.LootGroupID);
            newRow.AddInt("GroupMultiplier", lootEntry.GroupMultiplier);
            newRow.AddFloat("GroupProbability", lootEntry.GroupProbability);
            newRow.AddInt("DropLimit", lootEntry.DropLimit);
            newRow.AddInt("MinDrop", lootEntry.MinDrop);
            newRow.AddInt("ItemTemplateID", lootEntry.ItemTemplateEntryID);
            newRow.AddFloat("Chance", lootEntry.Chance);
            newRow.AddInt("ItemMultiplier", lootEntry.ItemMultiplier);
            newRow.AddInt("ItemCharges", lootEntry.ItemCharges);
            Rows.Add(newRow);
        }
    }
}
