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

using EQWOWConverter.Items;

namespace EQWOWConverter.WOWFiles
{
    internal class CreatureLootTemplateSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            // Warning: Do not use item as the delete criteria as there are reference table entries in the 90039-91016 and 190003-191016 ranges
            return "DELETE FROM creature_loot_template WHERE `entry` >= " + Configuration.SQL_CREATURETEMPLATE_ENTRY_LOW.ToString() + " AND `entry` <= " + Configuration.SQL_CREATURETEMPLATE_ENTRY_HIGH + ";";
        }

        public void AddRow(ItemLootTemplate itemLootTemplate)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("Entry", itemLootTemplate.CreatureTemplateEntryID);
            newRow.AddInt("Item", itemLootTemplate.ItemTemplateEntryID);
            newRow.AddInt("Reference", 0);
            newRow.AddFloat("Chance", itemLootTemplate.Chance);
            if (itemLootTemplate.QuestRequired == true)
                newRow.AddInt("QuestRequired", 1);
            else
                newRow.AddInt("QuestRequired", 0);
            newRow.AddInt("LootMode", 1);
            newRow.AddInt("GroupId", itemLootTemplate.GroupID);
            newRow.AddInt("MinCount", itemLootTemplate.MinCount);
            newRow.AddInt("MaxCount", itemLootTemplate.MaxCount);
            newRow.AddString("Comment", 255, itemLootTemplate.Comment);
            Rows.Add(newRow);
        }
    }
}
