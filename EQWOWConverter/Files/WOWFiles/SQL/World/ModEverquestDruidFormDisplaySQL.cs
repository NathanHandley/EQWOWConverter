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
    internal class ModEverquestDruidFormDisplaySQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DROP TABLE IF EXISTS `mod_everquest_druid_form_display`; ");
            stringBuilder.AppendLine("CREATE TABLE IF NOT EXISTS `mod_everquest_druid_form_display` ( ");
            stringBuilder.AppendLine("`FormType` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0' COMMENT '1 bear (and dire bear), 2 cat, 3 travel, 4 tree of life, 5 moonkin', ");
            stringBuilder.AppendLine("`OptionID` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0' COMMENT 'Which of the choices for that form this is.  Option 0 is never listed, since it means leave the display the core picked alone', ");
            stringBuilder.AppendLine("`DisplayID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`DisplayScale` FLOAT NOT NULL DEFAULT '1' COMMENT 'Object scale to wear the display at, which is how a Norrath form keeps the size its creature spawns at', ");
            stringBuilder.AppendLine("PRIMARY KEY (`FormType`, `OptionID`) USING BTREE ); ");
            return stringBuilder.ToString();
        }

        public void AddRow(int formType, int optionID, int displayID, float displayScale)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("FormType", formType);
            newRow.AddInt("OptionID", optionID);
            newRow.AddInt("DisplayID", displayID);
            newRow.AddFloat("DisplayScale", displayScale);
            Rows.Add(newRow);
        }
    }
}
