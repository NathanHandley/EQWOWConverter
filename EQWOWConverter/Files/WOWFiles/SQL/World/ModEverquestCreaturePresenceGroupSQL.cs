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
    internal class ModEverquestCreaturePresenceGroupSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DROP TABLE IF EXISTS `mod_everquest_creature_presence_group`; ");
            stringBuilder.AppendLine("CREATE TABLE IF NOT EXISTS `mod_everquest_creature_presence_group` ( ");
            stringBuilder.AppendLine("`ID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`MapID` INT(10) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`PrimaryCreatureTemplateID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`OtherCreatureTemplateID1` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`OtherCreatureTemplateID2` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`OtherCreatureTemplateID3` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`OtherCreatureTemplateID4` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`CheckIntervalMS` INT(10) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`Comment` VARCHAR(128) NOT NULL DEFAULT '', ");
            stringBuilder.AppendLine("PRIMARY KEY (`ID`) USING BTREE ); ");
            return stringBuilder.ToString();
        }

        public void AddRow(int id, int mapID, int primaryCreatureTemplateID, List<int> otherCreatureTemplateIDs, int checkIntervalMS, string comment)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("ID", id);
            newRow.AddInt("MapID", mapID);
            newRow.AddInt("PrimaryCreatureTemplateID", primaryCreatureTemplateID);
            newRow.AddInt("OtherCreatureTemplateID1", otherCreatureTemplateIDs.Count > 0 ? otherCreatureTemplateIDs[0] : 0);
            newRow.AddInt("OtherCreatureTemplateID2", otherCreatureTemplateIDs.Count > 1 ? otherCreatureTemplateIDs[1] : 0);
            newRow.AddInt("OtherCreatureTemplateID3", otherCreatureTemplateIDs.Count > 2 ? otherCreatureTemplateIDs[2] : 0);
            newRow.AddInt("OtherCreatureTemplateID4", otherCreatureTemplateIDs.Count > 3 ? otherCreatureTemplateIDs[3] : 0);
            newRow.AddInt("CheckIntervalMS", checkIntervalMS);
            newRow.AddString("Comment", 128, comment);
            Rows.Add(newRow);
        }
    }
}
