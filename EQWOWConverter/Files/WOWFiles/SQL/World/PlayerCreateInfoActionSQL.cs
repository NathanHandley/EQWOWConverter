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
using EQWOWConverter.Common;

namespace EQWOWConverter.WOWFiles
{
    internal class PlayerCreateInfoActionSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("INSERT IGNORE INTO `playercreateinfo_action` (`race`, `class`, `button`, `action`, `type`) ");
            stringBuilder.AppendLine("SELECT `allrace`.`race`, `shared`.`class`, `shared`.`button`, `shared`.`action`, `shared`.`type` ");
            stringBuilder.Append("FROM (");
            bool isFirstRace = true;
            foreach (RaceType raceType in Enum.GetValues(typeof(RaceType)))
            {
                if (raceType == RaceType.All)
                    continue;
                if (isFirstRace == true)
                    stringBuilder.Append(string.Concat("SELECT ", Convert.ToInt32(raceType).ToString(), " AS `race`"));
                else
                    stringBuilder.Append(string.Concat(" UNION ALL SELECT ", Convert.ToInt32(raceType).ToString()));
                isFirstRace = false;
            }
            stringBuilder.AppendLine(") AS `allrace` ");
            stringBuilder.AppendLine("JOIN (SELECT `class`, `button`, `action`, `type` FROM `playercreateinfo_action` ");
            stringBuilder.AppendLine("      GROUP BY `class`, `button`, `action`, `type` HAVING COUNT(DISTINCT `race`) > 1) AS `shared`; ");
            return stringBuilder.ToString();
        }
    }
}
