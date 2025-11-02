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
using System.Text;

namespace EQWOWConverter.WOWFiles
{
    internal class ModEverquestCreatureSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DROP TABLE IF EXISTS `mod_everquest_creature`; ");
            stringBuilder.AppendLine("CREATE TABLE IF NOT EXISTS `mod_everquest_creature` ( ");
            stringBuilder.AppendLine("`CreatureTemplateID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`CanShowHeldLootItems` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`MainhandHeldItemTemplateID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`OffhandHeldItemTemplateID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("PRIMARY KEY (`CreatureTemplateID`) USING BTREE ); ");
            return stringBuilder.ToString();
        }

        public void AddRow(int creatureTemplateID, bool canShowHeldLootItems, int mainhandHeldVisualItemID, int offhandHeldVisualItemID)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("CreatureTemplateID", creatureTemplateID);
            newRow.AddInt("CanShowHeldLootItems", canShowHeldLootItems == true ? 1 : 0);
            newRow.AddInt("MainhandHeldItemTemplateID", mainhandHeldVisualItemID);
            newRow.AddInt("OffhandHeldItemTemplateID", offhandHeldVisualItemID);
            Rows.Add(newRow);
        }
    }
}
