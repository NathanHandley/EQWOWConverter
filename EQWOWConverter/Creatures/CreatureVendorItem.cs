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
        private static Dictionary<int, List<CreatureVendorItem>> CreatureVendorItemsByMerchantID = new Dictionary<int, List<CreatureVendorItem>>();

        public int MerchantID = 0;
        public int EQItemID = 0;
        public int WOWItemID = 0;
        public int Slot = 0;
        public int FactionRequired = -1100; // Unsure how to read this exactly (found in merchantlist inside TAKP database). Looks like a packed flag, but why negative?
        public int Quantity = 0;

        public static Dictionary<int, List<CreatureVendorItem>> GetCreatureVendorItemsByMerchantIDs()
        {
            if (CreatureVendorItemsByMerchantID.Count == 0)
                PopulateCreatureVendorItems();
            return CreatureVendorItemsByMerchantID;
        }

        private static void PopulateCreatureVendorItems()
        {
            string creatureVendorItemsFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "VendorItems.csv");
            Logger.WriteDebug("Populating Creature Vendor Items list via file '" + creatureVendorItemsFile + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(creatureVendorItemsFile, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                // Skip disabled
                if (int.Parse(columns["enabled"]) == 0)
                    continue;

                // Only add if it's within this target expansion
                int minExpansion = int.Parse(columns["min_expansion"]);
                int maxExpansion = int.Parse(columns["max_expansion"]);
                if (minExpansion != -1 && minExpansion > Configuration.GENERATE_EQ_EXPANSION_ID_GENERAL)
                    continue;
                if (maxExpansion != -1 && maxExpansion < Configuration.GENERATE_EQ_EXPANSION_ID_GENERAL)
                    continue;

                // Add the row
                CreatureVendorItem newVendorItem = new CreatureVendorItem();
                newVendorItem.MerchantID = int.Parse(columns["merchantid"]);
                newVendorItem.Slot = int.Parse(columns["slot"]);
                newVendorItem.EQItemID = int.Parse(columns["eq_itemid"]);
                newVendorItem.WOWItemID = int.Parse(columns["wow_itemid"]);
                newVendorItem.FactionRequired = int.Parse(columns["faction_required"]);
                newVendorItem.Quantity = int.Parse(columns["quantity"]);
                if (CreatureVendorItemsByMerchantID.ContainsKey(newVendorItem.MerchantID) == false)
                    CreatureVendorItemsByMerchantID.Add(newVendorItem.MerchantID, new List<CreatureVendorItem>());
                CreatureVendorItemsByMerchantID[newVendorItem.MerchantID].Add(newVendorItem);
            }
        }
    }
}
