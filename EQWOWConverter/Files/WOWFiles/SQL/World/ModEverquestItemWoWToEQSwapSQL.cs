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
    internal class ModEverquestItemWoWToEQSwapSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DROP TABLE IF EXISTS `mod_everquest_item_wow_to_eq_swap`; ");
            stringBuilder.AppendLine("CREATE TABLE IF NOT EXISTS `mod_everquest_item_wow_to_eq_swap` ( ");
            stringBuilder.AppendLine("`InventoryType` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`ItemClassID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`ItemSubClassID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`EQClassID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`ItemTemplateID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`ItemDisplayID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("PRIMARY KEY (`InventoryType`, `ItemClassID`, `ItemSubClassID`, `EQClassID`, `ItemTemplateID`) USING BTREE ); ");
            return stringBuilder.ToString();
        }

        public void AddRow(int inventoryType, int itemClassID, int itemSubClassID, int eqClassID, int itemTemplateID, int itemDisplayID)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("InventoryType", inventoryType);
            newRow.AddInt("ItemClassID", itemClassID);
            newRow.AddInt("ItemSubClassID", itemSubClassID);
            newRow.AddInt("EQClassID", eqClassID);
            newRow.AddInt("ItemTemplateID", itemTemplateID);
            newRow.AddInt("ItemDisplayID", itemDisplayID);
            Rows.Add(newRow);
        }
    }
}
