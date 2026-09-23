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
    internal class ModEverquestIllusionCharacterSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DROP TABLE IF EXISTS `mod_everquest_illusion_character`; ");
            stringBuilder.AppendLine("CREATE TABLE IF NOT EXISTS `mod_everquest_illusion_character` ( ");
            stringBuilder.AppendLine("`EQRaceID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`Gender` TINYINT(3) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`ChrRaceID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`DisplayID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`AltDisplayID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`FaceCount` INT(10) NOT NULL DEFAULT '1', ");
            stringBuilder.AppendLine("`IsRobeCapable` TINYINT(3) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`Scale` FLOAT NOT NULL DEFAULT '1', ");
            stringBuilder.AppendLine("PRIMARY KEY (`EQRaceID`, `Gender`) USING BTREE ); ");
            return stringBuilder.ToString();
        }

        public void AddRow(int eqRaceID, int gender, int chrRaceID, int displayID, int altDisplayID, int faceCount, int isRobeCapable, float scale)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("EQRaceID", eqRaceID);
            newRow.AddInt("Gender", gender);
            newRow.AddInt("ChrRaceID", chrRaceID);
            newRow.AddInt("DisplayID", displayID);
            newRow.AddInt("AltDisplayID", altDisplayID);
            newRow.AddInt("FaceCount", faceCount);
            newRow.AddInt("IsRobeCapable", isRobeCapable);
            newRow.AddFloat("Scale", scale); // Object scale the mod applies with the display (the display rows themselves stay at scale 1)
            Rows.Add(newRow);
        }
    }
}
