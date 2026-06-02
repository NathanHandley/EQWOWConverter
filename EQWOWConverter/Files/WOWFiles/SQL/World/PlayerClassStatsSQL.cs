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

namespace EQWOWConverter.WOWFiles
{
    internal class PlayerClassStatsSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM player_class_stats WHERE `Class` = 6 AND `Level` < 55;"; // Remove any pre-55 DeathKnight stats
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
