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

using System.Text;

namespace EQWOWConverter.WOWFiles
{
    internal class ModEverquestPlayerCreateInfoSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DROP TABLE IF EXISTS `mod_everquest_playercreateinfo`; ");
            stringBuilder.AppendLine("CREATE TABLE IF NOT EXISTS `mod_everquest_playercreateinfo` ( ");
            stringBuilder.AppendLine("`race` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',");
            stringBuilder.AppendLine("`class` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',");
            stringBuilder.AppendLine("`map` SMALLINT(5) UNSIGNED NOT NULL DEFAULT '0',");
            stringBuilder.AppendLine("`zone` INT(10) UNSIGNED NOT NULL DEFAULT '0',");
            stringBuilder.AppendLine("`position_x` FLOAT NOT NULL DEFAULT '0',");
            stringBuilder.AppendLine("`position_y` FLOAT NOT NULL DEFAULT '0',");
            stringBuilder.AppendLine("`position_z` FLOAT NOT NULL DEFAULT '0',");
            stringBuilder.AppendLine("`orientation` FLOAT NOT NULL DEFAULT '0',");
            stringBuilder.AppendLine("PRIMARY KEY (`race`, `class`) USING BTREE); ");
            return stringBuilder.ToString();
        }

        public void AddRow(int raceID, int classID, int mapID, int areaID, float xPosition, float yPosition, float zPosition, float orientation)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("race", raceID);
            newRow.AddInt("class", classID);
            newRow.AddInt("map", mapID);
            newRow.AddInt("zone", areaID);
            newRow.AddFloat("position_x", xPosition);
            newRow.AddFloat("position_y", yPosition);
            newRow.AddFloat("position_z", zPosition);
            newRow.AddFloat("orientation", orientation);
            Rows.Add(newRow);
        }
    }
}
