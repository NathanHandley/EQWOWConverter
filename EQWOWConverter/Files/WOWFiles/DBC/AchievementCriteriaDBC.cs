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
    internal class AchievementCriteriaDBC : DBCFile
    {
        private static int CRITERIA_TYPE_KILL_CREATURE = 0;

        public void AddRowForKillCreature(int criteriaID, int achievementID, int creatureTemplateWOWID, string description, int uiOrder)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(criteriaID); // ID
            newRow.AddInt32(achievementID); // Achievement_Id (Achievement.dbc)
            newRow.AddInt32(CRITERIA_TYPE_KILL_CREATURE); // Type
            newRow.AddInt32(creatureTemplateWOWID); // Asset_Id (creature_template entry)
            newRow.AddInt32(1); // Quantity (kills required)
            newRow.AddInt32(0); // Start_Event
            newRow.AddInt32(0); // Start_Asset
            newRow.AddInt32(0); // Fail_Event
            newRow.AddInt32(0); // Fail_Asset
            newRow.AddStringLang(description); // Description
            newRow.AddInt32(0); // Flags (0 = show as a plain checklist entry)
            newRow.AddInt32(0); // Timer_Start_Event
            newRow.AddInt32(0); // Timer_Asset_Id
            newRow.AddInt32(0); // Timer_Time
            newRow.AddInt32(uiOrder); // Ui_Order
            Rows.Add(newRow);
        }
    }
}
