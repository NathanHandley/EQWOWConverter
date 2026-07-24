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
    internal class ModEverquestFactionSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DROP TABLE IF EXISTS `mod_everquest_faction`; ");
            stringBuilder.AppendLine("CREATE TABLE IF NOT EXISTS `mod_everquest_faction` ( ");
            stringBuilder.AppendLine("`FactionTemplateID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`FactionID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`BaseAlignment` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`PredominantEQRaceID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`WillDefendFriendlyPlayers` TINYINT UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`DefendersWillAttackToDefendPlayer` TINYINT UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`DefendCombatFactionTemplateID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("PRIMARY KEY (`FactionTemplateID`) USING BTREE); ");
            return stringBuilder.ToString();
        }

        public void AddRow(int factionTemplateID, int factionID, int baseAlignment, int predominantEQRaceID, bool willDefendFriendlyPlayers, bool defendersWillAttackToDefendPlayer, int defendCombatFactionTemplateID)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("FactionTemplateID", factionTemplateID);
            newRow.AddInt("FactionID", factionID);
            newRow.AddInt("BaseAlignment", baseAlignment);
            newRow.AddInt("PredominantEQRaceID", predominantEQRaceID);
            newRow.AddInt("WillDefendFriendlyPlayers", willDefendFriendlyPlayers == true ? 1 : 0);
            newRow.AddInt("DefendersWillAttackToDefendPlayer", defendersWillAttackToDefendPlayer == true ? 1 : 0);
            newRow.AddInt("DefendCombatFactionTemplateID", defendCombatFactionTemplateID);
            Rows.Add(newRow);
        }
    }
}
