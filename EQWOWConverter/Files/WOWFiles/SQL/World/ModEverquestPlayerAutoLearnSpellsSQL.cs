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
    internal class ModEverquestPlayerAutoLearnSpellsSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DROP TABLE IF EXISTS `mod_everquest_playerautolearnspells`; ");
            stringBuilder.AppendLine("CREATE TABLE IF NOT EXISTS `mod_everquest_playerautolearnspells` ( ");
            stringBuilder.AppendLine("`eqclass` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',");
            stringBuilder.AppendLine("`race` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',");
            stringBuilder.AppendLine("`spell` INT(10) UNSIGNED NOT NULL DEFAULT '0',");
            stringBuilder.AppendLine("PRIMARY KEY (`class`, `race`, `spell`) USING BTREE); ");
            return stringBuilder.ToString();
        }

        // raceID of 0 means the spell is learned by the class regardless of race
        public void AddRow(ClassEQType eqClassType, RaceType raceType, int spellID)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("eqclass", (int)eqClassType);
            newRow.AddInt("race", (int)raceType);
            newRow.AddInt("spell", spellID);
            Rows.Add(newRow);
        }
    }
}
