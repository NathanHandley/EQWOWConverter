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

using EQWOWConverter.Common;
using System.Text;

namespace EQWOWConverter.WOWFiles
{
    internal class ModEverquestIllusionObjectSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DROP TABLE IF EXISTS `mod_everquest_illusion_object`; ");
            stringBuilder.AppendLine("CREATE TABLE IF NOT EXISTS `mod_everquest_illusion_object` ( ");
            stringBuilder.AppendLine("`ID` INT(10) UNSIGNED NOT NULL DEFAULT 0, ");
            stringBuilder.AppendLine("`MapID` INT(10) UNSIGNED NOT NULL DEFAULT 0, ");
            stringBuilder.AppendLine("`X` FLOAT NOT NULL DEFAULT 0, ");
            stringBuilder.AppendLine("`Y` FLOAT NOT NULL DEFAULT 0, ");
            stringBuilder.AppendLine("`Z` FLOAT NOT NULL DEFAULT 0, ");
            stringBuilder.AppendLine("`DisplayID` INT(10) UNSIGNED NOT NULL DEFAULT 0, ");
            stringBuilder.AppendLine("`IsTree` TINYINT(3) UNSIGNED NOT NULL DEFAULT 0, ");
            stringBuilder.AppendLine("`ObjectName` VARCHAR(100) NOT NULL DEFAULT '', ");
            stringBuilder.AppendLine("PRIMARY KEY (`ID`) USING BTREE, ");
            stringBuilder.AppendLine("INDEX `idx_map` (`MapID`) USING BTREE); ");
            return stringBuilder.ToString();
        }

        public void AddRow(int id, int mapID, Vector3 position, int displayID, bool isTree, string objectName)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("ID", id);
            newRow.AddInt("MapID", mapID);
            newRow.AddFloat("X", position.X);
            newRow.AddFloat("Y", position.Y);
            newRow.AddFloat("Z", position.Z);
            newRow.AddInt("DisplayID", displayID);
            newRow.AddInt("IsTree", isTree ? 1 : 0);
            newRow.AddString("ObjectName", 100, objectName);
            Rows.Add(newRow);
        }
    }
}
