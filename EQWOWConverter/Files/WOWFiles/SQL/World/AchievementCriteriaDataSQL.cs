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

namespace EQWOWConverter.WOWFiles
{
    internal class AchievementCriteriaDataSQL : SQLFile
    {
        public static int DATA_TYPE_MAP_ID = 20; // Criteria only progresses while the player is on value1's map

        public override string DeleteRowSQL()
        {
            return string.Concat("DELETE FROM achievement_criteria_data WHERE `criteria_id` >= ", Configuration.DBCID_ACHIEVEMENTCRITERIA_ID_START.ToString(), " AND `criteria_id` <= ", Configuration.DBCID_ACHIEVEMENTCRITERIA_ID_END.ToString(), ";");
        }

        // Every kill criteria needs at least one of these rows or the core never progresses it
        public void AddRow(int criteriaID, int dataType, int value1, int value2)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("criteria_id", criteriaID);
            newRow.AddInt("type", dataType);
            newRow.AddInt("value1", value1);
            newRow.AddInt("value2", value2);
            newRow.AddString("ScriptName", 64, string.Empty);
            Rows.Add(newRow);
        }
    }
}
