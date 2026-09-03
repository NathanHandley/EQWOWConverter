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
    internal class ModEverquestQuestFactionRequirementSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DROP TABLE IF EXISTS `mod_everquest_quest_faction_requirement`; ");
            stringBuilder.AppendLine("CREATE TABLE IF NOT EXISTS `mod_everquest_quest_faction_requirement` ( ");
            stringBuilder.AppendLine("`QuestTemplateID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`FactionID` SMALLINT(5) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`MinimumFactionRank` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("PRIMARY KEY(`QuestTemplateID`) USING BTREE ); ");
            return stringBuilder.ToString();
        }

        public void AddRow(int questTemplateID, int factionID, int minimumFactionRank)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("QuestTemplateID", questTemplateID);
            newRow.AddInt("FactionID", factionID);
            newRow.AddInt("MinimumFactionRank", minimumFactionRank);
            Rows.Add(newRow);
        }
    }
}
