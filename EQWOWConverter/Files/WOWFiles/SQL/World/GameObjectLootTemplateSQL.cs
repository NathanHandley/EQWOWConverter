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
    internal class GameObjectLootTemplateSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM gameobject_loot_template WHERE `entry` >= " + Configuration.SQL_GAMEOBJECTTEMPLATE_ID_START.ToString() + " AND `entry` <= " + Configuration.SQL_GAMEOBJECTTEMPLATE_ID_END + ";";
        }

        public void AddRow(int gameObjectTemplateID, int containedItemTemplateID, string comment)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("Entry", gameObjectTemplateID);
            newRow.AddInt("Item", containedItemTemplateID);
            newRow.AddInt("Reference", 0);
            newRow.AddFloat("Chance", 100);
            newRow.AddInt("QuestRequired", 0);
            newRow.AddInt("LootMode", 1);
            newRow.AddInt("GroupId", 0);
            newRow.AddInt("MinCount", 1);
            newRow.AddInt("MaxCount", 1);
            newRow.AddString("Comment", 255, comment);
            Rows.Add(newRow);
        }
    }
}
