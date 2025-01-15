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
    internal class ItemLootTable
    {
        private static Dictionary<int, ItemLootTable> ItemLootTablesByEQID = new Dictionary<int, ItemLootTable>();

        public int EQID = 0;
        public string Name = string.Empty;
        public int MinMoney = 0;
        public int MaxMoney = 0;
        public int AverageMoney = 0;
        public List<ItemLootTableEntry> ItemLootTableEntries = new List<ItemLootTableEntry>();

        public static Dictionary<int, ItemLootTable> GetItemLootTablesByEQID()
        {
            if (ItemLootTablesByEQID.Count == 0)
                PopulateItemLootTableDataFromDisk();
            return ItemLootTablesByEQID;
        }

        private static void PopulateItemLootTableDataFromDisk()
        {
            // Populate the loot tables
            string itemLootTablesFileName = Path.Combine(Configuration.CONFIG_PATH_ASSETS_FOLDER, "WorldData", "ItemLootTables.csv");
            Logger.WriteDetail("Populating Item Loot Tables via file '" + itemLootTablesFileName + "'");
            List<string> lootTableInputRows = FileTool.ReadAllStringLinesFromFile(itemLootTablesFileName, true, true);
            foreach (string row in lootTableInputRows)
            {
                string[] rowBlocks = row.Split("|");

                // Skip invalid expansion rows
                int minExpansion = int.Parse(rowBlocks[6]);
                int maxExpansion = int.Parse(rowBlocks[7]);
                if (minExpansion != -1 && minExpansion > Configuration.CONFIG_GENERATE_EQ_EXPANSION_ID)
                    continue;
                if (maxExpansion != -1 && maxExpansion < Configuration.CONFIG_GENERATE_EQ_EXPANSION_ID)
                    continue;

                // Create the item loot table object
                ItemLootTable curItemLootTable = new ItemLootTable();
                curItemLootTable.EQID = int.Parse(rowBlocks[0]);
                curItemLootTable.Name = rowBlocks[1];
                curItemLootTable.MinMoney = int.Parse(rowBlocks[2]);
                curItemLootTable.MaxMoney = int.Parse(rowBlocks[3]);
                curItemLootTable.AverageMoney = int.Parse(rowBlocks[4]);
                ItemLootTablesByEQID.Add(curItemLootTable.EQID, curItemLootTable);
            }

            // Populate the entries, and map them to loot tables
            string itemLootTableEntriesFileName = Path.Combine(Configuration.CONFIG_PATH_ASSETS_FOLDER, "WorldData", "ItemLootTableEntries.csv");
            Logger.WriteDetail("Populating Item Loot Table Entries via file '" + itemLootTableEntriesFileName + "'");
            List<string> lootTableEntryInputRows = FileTool.ReadAllStringLinesFromFile(itemLootTableEntriesFileName, true, true);
            foreach (string row in lootTableEntryInputRows)
            {
                string[] rowBlocks = row.Split("|");

                // Create the entry record
                ItemLootTableEntry curLootTableEntry = new ItemLootTableEntry();
                curLootTableEntry.LootTableID = int.Parse(rowBlocks[0]);
                curLootTableEntry.LootDropID = int.Parse(rowBlocks[1]);
                curLootTableEntry.Multiplier = int.Parse(rowBlocks[2]);
                curLootTableEntry.Probability = int.Parse(rowBlocks[3]);
                curLootTableEntry.DropLimit = int.Parse(rowBlocks[4]);
                curLootTableEntry.MinDrop = int.Parse(rowBlocks[5]);
                curLootTableEntry.MultiplierMin = int.Parse(rowBlocks[6]);

                // Map it to a table
                if (ItemLootTablesByEQID.ContainsKey(curLootTableEntry.LootTableID) == false)
                {
                    Logger.WriteError("LootTableEntry could not find LootTable with ID '" + curLootTableEntry.LootTableID + "'");
                    continue;
                }
                else
                    ItemLootTablesByEQID[curLootTableEntry.LootTableID].ItemLootTableEntries.Add(curLootTableEntry);
            }
        }
    }
}
