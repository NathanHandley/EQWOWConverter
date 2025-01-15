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
using System.Threading.Tasks;

namespace EQWOWConverter.Items
{
    internal class ItemLootDrop
    {
        private static Dictionary<int, ItemLootDrop> ItemLootDropsByEQID = new Dictionary<int, ItemLootDrop>();

        public int EQID = 0;
        public string Name = string.Empty;
        public List<ItemLootDropEntry> ItemLootDropEntries = new List<ItemLootDropEntry>();

        public static Dictionary<int, ItemLootDrop> GetItemLootDropsByEQID()
        {
            if (ItemLootDropsByEQID.Count == 0)
                PopulateItemLootDropDataFromDisk();
            return ItemLootDropsByEQID;
        }

        private static void PopulateItemLootDropDataFromDisk()
        {
            // Populate the loot drops
            string itemLootDropsFileName = Path.Combine(Configuration.CONFIG_PATH_ASSETS_FOLDER, "WorldData", "ItemLootDrops.csv");
            Logger.WriteDetail("Populating Item Loot Drops via file '" + itemLootDropsFileName + "'");
            List<string> lootDropsInputRows = FileTool.ReadAllStringLinesFromFile(itemLootDropsFileName, true, true);
            foreach (string row in lootDropsInputRows)
            {
                string[] rowBlocks = row.Split("|");

                // Skip invalid expansion rows
                int minExpansion = int.Parse(rowBlocks[2]);
                int maxExpansion = int.Parse(rowBlocks[3]);
                if (minExpansion != -1 && minExpansion > Configuration.CONFIG_GENERATE_EQ_EXPANSION_ID)
                    continue;
                if (maxExpansion != -1 && maxExpansion < Configuration.CONFIG_GENERATE_EQ_EXPANSION_ID)
                    continue;

                // Create the item loot drop object
                ItemLootDrop curLootDropTable = new ItemLootDrop();
                curLootDropTable.EQID = int.Parse(rowBlocks[0]);
                curLootDropTable.Name = rowBlocks[1];
                ItemLootDropsByEQID.Add(curLootDropTable.EQID, curLootDropTable);
            }

            // Populate the entries, and map them to loot drops
            string itemLootDropEntriesFileName = Path.Combine(Configuration.CONFIG_PATH_ASSETS_FOLDER, "WorldData", "ItemLootDropEntries.csv");
            Logger.WriteDetail("Populating Item Loot Drop Entries via file '" + itemLootDropEntriesFileName + "'");
            List<string> lootDropEntryInputRows = FileTool.ReadAllStringLinesFromFile(itemLootDropEntriesFileName, true, true);
            foreach (string row in lootDropEntryInputRows)
            {
                string[] rowBlocks = row.Split("|");

                // Create the entry record
                ItemLootDropEntry curLootDropEntry = new ItemLootDropEntry();
                curLootDropEntry.LootDropID = int.Parse(rowBlocks[0]);
                curLootDropEntry.ItemIDEQ = int.Parse(rowBlocks[1]);
                curLootDropEntry.ItemCharges = int.Parse(rowBlocks[2]);
                curLootDropEntry.EquipItem = int.Parse(rowBlocks[3]);
                curLootDropEntry.Chance = float.Parse(rowBlocks[4]);
                curLootDropEntry.Multiplier = int.Parse(rowBlocks[5]);
                curLootDropEntry.DisabledChance = float.Parse(rowBlocks[6]);

                // Map it to a table
                if (ItemLootDropsByEQID.ContainsKey(curLootDropEntry.LootDropID) == false)
                {
                    Logger.WriteDetail("LootDropEntry could not find a valid LootDrop with ID '" + curLootDropEntry.LootDropID + "', which probably means it's for a different expansion");
                    continue;
                }
                else
                    ItemLootDropsByEQID[curLootDropEntry.LootDropID].ItemLootDropEntries.Add(curLootDropEntry);
            }
        }
    }
}
