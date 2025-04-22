//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2025 Nathan Handley
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
    internal class CreatureQuestStarterSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM creature_queststarter WHERE `quest` >= " + Configuration.SQL_QUEST_TEMPLATE_ID_START.ToString() + " AND `quest` <= " + Configuration.SQL_QUEST_TEMPLATE_ID_END + ";";
        }

        public void AddRow(int questTemplateID, int creatureTemplateID)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("id", creatureTemplateID);
            newRow.AddInt("quest", questTemplateID);
            Rows.Add(newRow);
        }
    }
}
