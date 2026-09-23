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
    internal class ModEverquestIllusionGenericItemDisplaySQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DROP TABLE IF EXISTS `mod_everquest_illusion_generic_item_display`; ");
            stringBuilder.AppendLine("CREATE TABLE IF NOT EXISTS `mod_everquest_illusion_generic_item_display` ( ");
            stringBuilder.AppendLine("`ChrRaceID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`Gender` TINYINT(3) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`InventoryType` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`ArmorSubClass` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`NativeItemDisplayInfoID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("PRIMARY KEY (`ChrRaceID`, `Gender`, `InventoryType`, `ArmorSubClass`) USING BTREE ); ");
            return stringBuilder.ToString();
        }

        public void AddRow(int chrRaceID, int gender, int inventoryType, int armorSubClass, int nativeItemDisplayInfoID)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("ChrRaceID", chrRaceID);
            newRow.AddInt("Gender", gender);
            newRow.AddInt("InventoryType", inventoryType);
            newRow.AddInt("ArmorSubClass", armorSubClass);
            newRow.AddInt("NativeItemDisplayInfoID", nativeItemDisplayInfoID);
            Rows.Add(newRow);
        }
    }
}
