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
    internal class PlayerCreateInfoSQL : SQLFile
    {
        public bool FillGapsFromExistingRows = false;

        public override string DeleteRowSQL()
        {
            if (FillGapsFromExistingRows == false)
                return "DELETE FROM `playercreateinfo`;";

            // No rows were generated, so the missing pairings copy the lowest numbered class row that race already has
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("INSERT IGNORE INTO `playercreateinfo` (`race`, `class`, `map`, `zone`, `position_x`, `position_y`, `position_z`, `orientation`) ");
            stringBuilder.AppendLine("SELECT `basrow`.`race`, `allclass`.`class`, `basrow`.`map`, `basrow`.`zone`, `basrow`.`position_x`, `basrow`.`position_y`, `basrow`.`position_z`, `basrow`.`orientation` ");
            stringBuilder.AppendLine("FROM (SELECT `race`, MIN(`class`) AS `baseclass` FROM `playercreateinfo` GROUP BY `race`) AS `basclass` ");
            stringBuilder.AppendLine("JOIN `playercreateinfo` AS `basrow` ON `basrow`.`race` = `basclass`.`race` AND `basrow`.`class` = `basclass`.`baseclass` ");
            stringBuilder.AppendLine("JOIN (SELECT DISTINCT `class` FROM `playercreateinfo`) AS `allclass`; ");
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
