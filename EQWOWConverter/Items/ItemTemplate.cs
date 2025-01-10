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

using EQWOWConverter.Creatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Items
{
    internal class ItemTemplate
    {
        public static SortedDictionary<int, ItemTemplate> ItemTemplatesByEQDBID = new SortedDictionary<int, ItemTemplate>();
        private static int CURRENT_SQL_ITEMTEMPLATEENTRYID = Configuration.CONFIG_SQL_ITEM_TEMPLATE_ENTRY_START;

        public int EQID = 0;
        public int EntryID = 0;
        public int ClassID = 0;
        public int SubClassID = 0;
        public string Name = string.Empty;
        public int DisplayID = 0;
        public int InventoryType = 0;
        public int SheatheType = 0;
        public int Material = -1;

        public ItemTemplate()
        {
            EntryID = CURRENT_SQL_ITEMTEMPLATEENTRYID;
            CURRENT_SQL_ITEMTEMPLATEENTRYID++;
        }

        static public void PopulateItemTemplateListFromDisk()
        {
            ItemTemplatesByEQDBID.Clear();

            // Load in item data
            string itemsFileName = Path.Combine(Configuration.CONFIG_PATH_ASSETS_FOLDER, "WorldData", "Items.csv");
            Logger.WriteDetail("Populating item templates via file '" + itemsFileName + "'");
            string inputData = FileTool.ReadAllDataFromFile(itemsFileName);
            string[] inputRows = inputData.Split(Environment.NewLine);
            if (inputRows.Length < 2)
            {
                Logger.WriteError("Items list via file '" + itemsFileName + "' did not have enough lines");
                return;
            }

            // Load all of the data
            bool headerRow = true;
            foreach (string row in inputRows)
            {
                // Handle first row
                if (headerRow == true)
                {
                    headerRow = false;
                    continue;
                }

                // Skip blank rows
                if (row.Trim().Length == 0)
                    continue;

                // Load the row
                string[] rowBlocks = row.Split("|");
                ItemTemplate newItemTemplate = new ItemTemplate();
                newItemTemplate.EQID = int.Parse(rowBlocks[0]);
                newItemTemplate.Name = rowBlocks[1];

                // Icon
                int iconID = int.Parse(rowBlocks[2]);
                string iconName = "INV_EQ_" + (iconID - 500).ToString();
                ItemDisplayInfo itemDisplayInfo = ItemDisplayInfo.GetOrCreateItemDisplayInfo(iconName);
                newItemTemplate.DisplayID = itemDisplayInfo.DBCID;

                // Add
                if (ItemTemplatesByEQDBID.ContainsKey(newItemTemplate.EntryID))
                {
                    Logger.WriteError("Items list via file '" + itemsFileName + "' has an duplicate row with id '" + newItemTemplate.EntryID + "'");
                    continue;
                }
                ItemTemplatesByEQDBID.Add(newItemTemplate.EntryID, newItemTemplate);
            }
        }
    }
}
