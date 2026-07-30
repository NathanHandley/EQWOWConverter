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
    internal class ModEverquestTalentExclusionSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DROP TABLE IF EXISTS `mod_everquest_talent_exclusion`; ");
            stringBuilder.AppendLine("CREATE TABLE IF NOT EXISTS `mod_everquest_talent_exclusion` ( ");
            stringBuilder.AppendLine("`TalentRank1SpellID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`WOWClassName` VARCHAR(24) NOT NULL DEFAULT '', ");
            stringBuilder.AppendLine("`TalentName` VARCHAR(64) NOT NULL DEFAULT '', ");
            stringBuilder.AppendLine("`Reason` VARCHAR(512) NOT NULL DEFAULT '', ");
            stringBuilder.AppendLine("PRIMARY KEY (`TalentRank1SpellID`) USING BTREE); ");
            return stringBuilder.ToString();
        }

        public void AddRow(int talentRank1SpellID, string wowClassName, string talentName, string reason)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("TalentRank1SpellID", talentRank1SpellID);
            newRow.AddString("WOWClassName", 24, wowClassName);
            newRow.AddString("TalentName", 64, talentName);
            newRow.AddString("Reason", 512, reason);
            Rows.Add(newRow);
        }
    }
}
