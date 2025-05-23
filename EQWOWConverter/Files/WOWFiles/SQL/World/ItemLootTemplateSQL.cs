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

using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace EQWOWConverter.WOWFiles
{
    internal class ItemLootTemplateSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM `item_loot_template` WHERE `Entry` >= " + Configuration.SQL_ITEM_TEMPLATE_ENTRY_START + " AND `Entry` <= " + Configuration.SQL_ITEM_TEMPLATE_ENTRY_END + ";";
        }

        public void AddRow(int containerItemTemplateID, int containedItemTemplateID, int groupID, float chance, int count, string comment)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("Entry", containerItemTemplateID);
            newRow.AddInt("Item", containedItemTemplateID);
            newRow.AddInt("Reference", 0);
            newRow.AddFloat("Chance", chance);
            newRow.AddInt("QuestRequired", 0);
            newRow.AddInt("LootMode", 1);
            newRow.AddInt("GroupId", groupID);
            newRow.AddInt("MinCount", count);
            newRow.AddInt("MaxCount", count);
            newRow.AddString("Comment", 255, comment);
            Rows.Add(newRow);
        }
    }
}
