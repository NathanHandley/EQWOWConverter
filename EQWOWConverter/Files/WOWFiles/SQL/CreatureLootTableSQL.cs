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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using EQWOWConverter.Items;

namespace EQWOWConverter.WOWFiles
{
    internal class CreatureLootTableSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM creature_loot_template WHERE `entry` >= " + Configuration.CONFIG_SQL_CREATURETEMPLATE_ENTRY_LOW.ToString() + " AND `entry` <= " + Configuration.CONFIG_SQL_CREATURETEMPLATE_ENTRY_HIGH + ";";
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
            newRow.AddInt("GroupId", 0);
            newRow.AddInt("MinCount", itemLootTemplate.MinCount);
            newRow.AddInt("MaxCount", itemLootTemplate.MaxCount);
            newRow.AddString("Comment", 255, itemLootTemplate.Comment);
            Rows.Add(newRow);
        }
    }
}
