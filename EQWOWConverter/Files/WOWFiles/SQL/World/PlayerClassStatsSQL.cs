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
    internal class PlayerClassStatsSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DELETE FROM player_class_stats WHERE `Class` = 6 AND `Level` < 55;"); // Remove any pre-55 DeathKnight stats

            // Give mana to Warrior, Rogue, and DK
            if (Configuration.PLAYER_STAT_BASEMANA_FILL_DONOR_CLASS_ID > 0)
            {
                stringBuilder.Append("UPDATE `player_class_stats` AS `target` JOIN `player_class_stats` AS `donor` ON `donor`.`Class` = ");
                stringBuilder.Append(Configuration.PLAYER_STAT_BASEMANA_FILL_DONOR_CLASS_ID);
                stringBuilder.AppendLine(" AND `donor`.`Level` = `target`.`Level` SET `target`.`BaseMana` = `donor`.`BaseMana` WHERE `target`.`BaseMana` = 0 AND `donor`.`BaseMana` > 0;");
            }
            return stringBuilder.ToString();
        }

        public void AddRow(ClassWOWType classType, int level, int baseHP, int baseMana, int strength, int agility, int stamina, int intellect, int spirit)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("Class", (int)classType);
            newRow.AddInt("Level", level);
            newRow.AddInt("BaseHP", baseHP);
            newRow.AddInt("BaseMana", baseMana);
            newRow.AddInt("Strength", strength);
            newRow.AddInt("Agility", agility);
            newRow.AddInt("Stamina", stamina);
            newRow.AddInt("Intellect", intellect);
            newRow.AddInt("Spirit", spirit);
            Rows.Add(newRow);
        }
    }
}
