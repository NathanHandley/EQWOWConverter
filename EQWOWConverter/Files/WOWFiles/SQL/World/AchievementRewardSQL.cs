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
    internal class AchievementRewardSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM achievement_reward WHERE `ID` >= " + Configuration.DBCID_ACHIEVEMENT_ID_START.ToString() + " AND `ID` <= " + Configuration.DBCID_ACHIEVEMENT_ID_END.ToString() + ";";
        }

        public void AddRow(int achievementID, int itemID, int senderCreatureTemplateID, string subject, string body)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("ID", achievementID);
            newRow.AddInt("TitleA", 0);
            newRow.AddInt("TitleH", 0);
            newRow.AddInt("ItemID", itemID);
            newRow.AddInt("Sender", senderCreatureTemplateID);
            newRow.AddString("Subject", 255, subject);
            newRow.AddString("Body", 500, body);
            newRow.AddInt("MailTemplateID", 0);
            Rows.Add(newRow);
        }
    }
}
