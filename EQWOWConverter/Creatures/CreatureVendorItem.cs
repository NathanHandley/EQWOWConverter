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

namespace EQWOWConverter.Creatures
{
    internal class CreatureVendorItem
    {
        private static List<CreatureVendorItem> CreatureVendorItems = new List<CreatureVendorItem>();

        public int EQCreatureTemplateID = 0;
        public int EQItemID = 0;
        public int Slot = 0;
        public int FactionRequired = -1100; // Unsure what this is (found in merchantlist inside TAKP database). Looks like a packed flag, but why negative?
        public int Quantity = 0;

        public static List<CreatureVendorItem> GetCreatureVendorItems()
        {
            if (CreatureVendorItems.Count == 0)
                PopulateCreatureVendorItems();
            return CreatureVendorItems;
        }

        private static void PopulateCreatureVendorItems()
        {
            CreatureVendorItems.Clear();

            string creatureVendorItemsFile = Path.Combine(Configuration.CONFIG_PATH_ASSETS_FOLDER, "WorldData", "VendorItems.csv");
            Logger.WriteDetail("Populating Creature Vendor Items list via file '" + creatureVendorItemsFile + "'");
            string inputData = FileTool.ReadAllDataFromFile(creatureVendorItemsFile);
            string[] inputRows = inputData.Split(Environment.NewLine);
            if (inputRows.Length < 2)
            {
                Logger.WriteError("CreatureVenderItems list via file '" + creatureVendorItemsFile + "' did not have enough rows");
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
                CreatureVendorItem newItem = new CreatureVendorItem();
                newItem.EQCreatureTemplateID = int.Parse(rowBlocks[0]);
                newItem.Slot = int.Parse(rowBlocks[1]);
                newItem.EQItemID = int.Parse(rowBlocks[2]);
                newItem.FactionRequired = int.Parse(rowBlocks[3]);
                newItem.Quantity = int.Parse(rowBlocks[4]);

                // Only add if it's within this target expansion
                int minExpansion = int.Parse(rowBlocks[5]);
                int maxExpansion = int.Parse(rowBlocks[6]);
                if (minExpansion != -1 && minExpansion > Configuration.CONFIG_GENERATE_EQ_EXPANSION_ID)
                    continue;
                if (maxExpansion != -1 && maxExpansion < Configuration.CONFIG_GENERATE_EQ_EXPANSION_ID)
                    continue;
                CreatureVendorItems.Add(newItem);
            }
        }
    }
}
