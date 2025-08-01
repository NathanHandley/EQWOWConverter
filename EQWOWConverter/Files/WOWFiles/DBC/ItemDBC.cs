﻿//  Author: Nathan Handley (nathanhandley@protonmail.com)
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
    internal class ItemDBC : DBCFile
    {
        private HashSet<int> insertedItemTemplateEntryIDs = new HashSet<int>();

        public void AddRow(ItemTemplate itemTemplate, int itemTemplateEntryID)
        {
            // Prevent double-add
            if (insertedItemTemplateEntryIDs.Contains(itemTemplateEntryID))
                return;
            insertedItemTemplateEntryIDs.Add(itemTemplateEntryID);

            DBCRow newRow = new DBCRow();
            newRow.AddInt32(itemTemplateEntryID);
            newRow.AddInt32(itemTemplate.ClassID);
            newRow.AddInt32(itemTemplate.SubClassID);
            newRow.AddInt32(-1); // Sound Override SubclassID
            newRow.AddInt32(itemTemplate.WOWItemMaterialType);
            if (itemTemplate.ItemDisplayInfo != null)
                newRow.AddInt32(itemTemplate.ItemDisplayInfo.ItemDisplayInfoDBCID);
            else
                newRow.AddInt32(0);
            newRow.AddInt32(Convert.ToInt32(itemTemplate.InventoryType));
            newRow.AddInt32(itemTemplate.SheatheType);
            Rows.Add(newRow);
        }
    }
}
