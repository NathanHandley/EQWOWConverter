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
        private static SortedDictionary<int, ItemTemplate> ItemTemplatesByEQDBID = new SortedDictionary<int, ItemTemplate>();
        private static int CURRENT_SQL_ITEMTEMPLATEENTRYID = Configuration.CONFIG_SQL_ITEM_TEMPLATE_ENTRY_START;

        public int EQItemID = 0;
        public int WOWEntryID = 0;
        public int ClassID = 0;
        public int SubClassID = 0;
        public string Name = string.Empty;
        public int DisplayID = 0;
        public int SheatheType = 0;
        public int Material = -1;
        public int BuyPriceInCopper = 0;
        public int SellPriceInCopper = 0;
        public int BagSlots = 0;
        public int StackSize = 1;
        public ItemInventoryType InventoryType = ItemInventoryType.NoEquip;

        public ItemTemplate()
        {
            WOWEntryID = CURRENT_SQL_ITEMTEMPLATEENTRYID;
            CURRENT_SQL_ITEMTEMPLATEENTRYID++;
        }

        static public SortedDictionary<int, ItemTemplate> GetItemTemplatesByEQDBIDs()
        {
            if (ItemTemplatesByEQDBID.Count == 0)
                PopulateItemTemplateListFromDisk();
            return ItemTemplatesByEQDBID;
        }

        static private void PopulateEquippableItemProperties(ref ItemTemplate itemTemplate, int eqItemType, int bagType)
        {
            switch (eqItemType)
            {
                case 0: // Catches a lot of items
                    {
                        if (bagType != 0)
                        {
                            itemTemplate.InventoryType = ItemInventoryType.Bag;
                            switch (bagType)
                            {
                                case 2: // Quiver
                                    {
                                        itemTemplate.ClassID = 2;
                                        itemTemplate.SubClassID = 3;
                                        itemTemplate.InventoryType = ItemInventoryType.Quiver;
                                    } break;
                                case 10: // Toolbox => Engineering Bag
                                    {
                                        itemTemplate.ClassID = 1;
                                        itemTemplate.SubClassID = 4;
                                    } break;
                                case 16: // Sewing Kit => Leatherworking Bag (TODO: Add Tailoring Bag?)
                                    {
                                        itemTemplate.ClassID = 1;
                                        itemTemplate.SubClassID = 7;
                                    } break;
                                default: // Normal Bag
                                    {
                                        itemTemplate.ClassID = 1;
                                        itemTemplate.SubClassID = 0;
                                    } break;
                            }
                            return;
                        }
                        else
                        {
                            // 1 Hand Slash => 1h Sword
                            // TODO: Axe
                            itemTemplate.ClassID = 2;
                            itemTemplate.SubClassID = 7;
                            itemTemplate.InventoryType = ItemInventoryType.OneHand;
                        }
                    } break;
                case 1: // 2 Hand Slash => 2h Sword
                    {
                        // TODO: Axe
                        itemTemplate.ClassID = 2;
                        itemTemplate.SubClassID = 8;
                        itemTemplate.InventoryType = ItemInventoryType.TwoHand;
                    } break;
                case 2: // 1 Hand Pierce => Dagger
                    {
                        itemTemplate.ClassID = 2;
                        itemTemplate.SubClassID = 15;
                        itemTemplate.InventoryType = ItemInventoryType.OneHand;
                    } break;
                case 35: // 2 Hand Pierce => Polearm
                    {
                        itemTemplate.ClassID = 2;
                        itemTemplate.SubClassID = 6;
                        itemTemplate.InventoryType = ItemInventoryType.TwoHand;
                    } break;
                case 3: // 1 Hand Blunt => 1H Mace
                    {
                        itemTemplate.ClassID = 2;
                        itemTemplate.SubClassID = 4;
                        itemTemplate.InventoryType = ItemInventoryType.OneHand;
                    } break;
                case 4: // 2 Hand Blunt => 2H Mace
                    {
                        itemTemplate.ClassID = 2;
                        itemTemplate.SubClassID = 5;
                        itemTemplate.InventoryType = ItemInventoryType.TwoHand;
                        // TODO: Staves
                    } break;
                case 5: // Bow
                    {
                        itemTemplate.ClassID = 2;
                        itemTemplate.SubClassID = 2;
                        itemTemplate.InventoryType = ItemInventoryType.Ranged;
                    } break;
                case 7: // Thrown
                    {
                        itemTemplate.ClassID = 2;
                        itemTemplate.SubClassID = 16;
                        itemTemplate.InventoryType = ItemInventoryType.Thrown;
                    } break;
                case 8: // Shield
                    {
                        itemTemplate.ClassID = 4;
                        itemTemplate.SubClassID = 6;
                        itemTemplate.InventoryType = ItemInventoryType.Shield;
                    } break;
                case 10: // Armor
                    {
                        itemTemplate.ClassID = 4;
                        itemTemplate.SubClassID = 1;
                        itemTemplate.InventoryType = ItemInventoryType.Chest; // TEMP
                        // TODO: Slot
                        // TODO: Armor Type
                    } break;
                case 11: // Misc
                    {
                        itemTemplate.ClassID = 14;
                        itemTemplate.SubClassID = 4;
                    } break;
                case 12: // Lockpick
                    {
                        itemTemplate.ClassID = 13;
                        itemTemplate.SubClassID = 1;
                    } break;
                case 14: // Food => Food and Drink
                    {
                        itemTemplate.ClassID = 0;
                        itemTemplate.SubClassID = 5;
                    } break;
                case 15: // Drink => Food and Drink
                    {
                        itemTemplate.ClassID = 0;
                        itemTemplate.SubClassID = 5;
                    } break;
                case 16: // Light Source => Misc
                    {
                        itemTemplate.ClassID = 14;
                        itemTemplate.SubClassID = 4;
                    } break;
                case 17: // Stackable => Misc
                    {
                        itemTemplate.ClassID = 14;
                        itemTemplate.SubClassID = 4;
                    } break;
                case 18: // Bandage
                    {
                        itemTemplate.ClassID = 0;
                        itemTemplate.SubClassID = 7;
                    } break;
                case 19: // Throwing (2)
                    {
                        itemTemplate.ClassID = 2;
                        itemTemplate.SubClassID = 16;
                        itemTemplate.InventoryType = ItemInventoryType.Thrown;
                    } break;
                case 20: // Spell/Tome => Book
                    {
                        itemTemplate.ClassID = 9;
                        itemTemplate.SubClassID = 0;
                    } break;
                case 21: // Potion
                    {
                        itemTemplate.ClassID = 0;
                        itemTemplate.SubClassID = 1;
                    } break;
                case 23: // Wind Instrument => Misc
                    {
                        itemTemplate.ClassID = 14;
                        itemTemplate.SubClassID = 4;
                    } break;
                case 24: // String Instrument => Misc
                    {
                        itemTemplate.ClassID = 14;
                        itemTemplate.SubClassID = 4;
                    } break;
                case 25: // Brass Instrument => Misc
                    {
                        itemTemplate.ClassID = 14;
                        itemTemplate.SubClassID = 4;
                    } break;
                case 26: // Drum Instrument => Misc
                    {
                        itemTemplate.ClassID = 14;
                        itemTemplate.SubClassID = 4;
                    } break;
                case 27: // Arrow
                    {
                        itemTemplate.ClassID = 6;
                        itemTemplate.SubClassID = 2;
                        itemTemplate.InventoryType = ItemInventoryType.Ammo;
                    } break;
                case 28: // Other Consumable => Generic Consumable
                    {
                        itemTemplate.ClassID = 0;
                        itemTemplate.SubClassID = 0;
                    } break;
                case 29: // Jewelry => Misc Armor
                    {
                        itemTemplate.ClassID = 4;
                        itemTemplate.SubClassID = 0;
                        itemTemplate.InventoryType = ItemInventoryType.Finger; // TODO: Neck vs Earring vs Ring
                    } break;
                case 30: // Skull => Misc
                    {
                        itemTemplate.ClassID = 14;
                        itemTemplate.SubClassID = 4;
                    } break;
                case 31: // Tome => Book
                    {
                        itemTemplate.ClassID = 9;
                        itemTemplate.SubClassID = 0;
                    } break;
                case 32: // Note => Misc
                    {
                        itemTemplate.ClassID = 14;
                        itemTemplate.SubClassID = 4;
                    } break;
                case 33: // Key
                    {
                        itemTemplate.ClassID = 13;
                        itemTemplate.SubClassID = 0;
                    } break;
                case 34: // Coin => Misc
                    {
                        itemTemplate.ClassID = 14;
                        itemTemplate.SubClassID = 4;
                    } break;
                case 36: // Fishing Pole
                    {
                        itemTemplate.ClassID = 2;
                        itemTemplate.SubClassID = 20;
                        itemTemplate.InventoryType = ItemInventoryType.TwoHand;
                    } break;
                case 37: // Fishing Bait => Devices (like other fishing bait)
                    {
                        itemTemplate.ClassID = 7;
                        itemTemplate.SubClassID = 3;
                    } break;
                case 38: // Alcohol => Food and Drink
                    {
                        itemTemplate.ClassID = 0;
                        itemTemplate.SubClassID = 5;
                    } break;
                case 39: // Key2 => Key (Quest only keys?)
                    {
                        itemTemplate.ClassID = 13;
                        itemTemplate.SubClassID = 0;
                    } break;
                case 40: // Compass => Misc
                    {
                        itemTemplate.ClassID = 14;
                        itemTemplate.SubClassID = 4;
                    } break;
                case 42: // Poison => Consumable other (like rogue poison)
                    {
                        itemTemplate.ClassID = 0;
                        itemTemplate.SubClassID = 8;
                    } break;
                case 45: // Hand2Hand => Fist Weapon
                    {
                        itemTemplate.ClassID = 2;
                        itemTemplate.SubClassID = 13;
                        itemTemplate.InventoryType = ItemInventoryType.OneHand;
                    } break;
                case 50: // Singing => Misc
                    {
                        itemTemplate.ClassID = 14;
                        itemTemplate.SubClassID = 4;
                    } break;
                case 51: // All Instruments => Misc
                    {
                        itemTemplate.ClassID = 14;
                        itemTemplate.SubClassID = 4;
                    } break;
                case 52: // Charm => Misc
                    {
                        itemTemplate.ClassID = 14;
                        itemTemplate.SubClassID = 4;
                    } break;
                case 54: // Augment => Misc
                    {
                        itemTemplate.ClassID = 14;
                        itemTemplate.SubClassID = 4;
                    } break;
                case 55: // Augment Solvent => Misc
                    {
                        itemTemplate.ClassID = 14;
                        itemTemplate.SubClassID = 4;
                    } break;
                case 56: // Augment Distill => Misc
                    {
                        itemTemplate.ClassID = 14;
                        itemTemplate.SubClassID = 4;
                    } break;
                default:
                    {
                        Logger.WriteError("Unhandled item class type for eqItemType '" + eqItemType + "'");
                        itemTemplate.ClassID = 0;
                        itemTemplate.SubClassID = 0;
                    } break;
            }
        }

        static public void PopulateItemTemplateListFromDisk()
        {
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
                newItemTemplate.EQItemID = int.Parse(rowBlocks[0]);
                newItemTemplate.Name = rowBlocks[1];

                // Equippable Properties
                int itemType = int.Parse(rowBlocks[2]);
                int bagType = int.Parse(rowBlocks[7]);
                PopulateEquippableItemProperties(ref newItemTemplate, itemType, bagType);

                // Icon
                int iconID = int.Parse(rowBlocks[3]);
                string iconName = "INV_EQ_" + (iconID - 500).ToString();
                ItemDisplayInfo itemDisplayInfo = ItemDisplayInfo.GetOrCreateItemDisplayInfo(iconName);
                newItemTemplate.DisplayID = itemDisplayInfo.DBCID;

                // Price
                newItemTemplate.BuyPriceInCopper = int.Parse(rowBlocks[4]);
                if (newItemTemplate.BuyPriceInCopper <= 0)
                    newItemTemplate.BuyPriceInCopper = 1;
                newItemTemplate.SellPriceInCopper = int.Max(Convert.ToInt32(Convert.ToDouble(newItemTemplate.BuyPriceInCopper) * 0.25), 1);

                // Other
                newItemTemplate.BagSlots = int.Parse(rowBlocks[8]);
                newItemTemplate.StackSize = int.Max(int.Parse(rowBlocks[9]), 1);

                // Add
                if (ItemTemplatesByEQDBID.ContainsKey(newItemTemplate.EQItemID))
                {
                    Logger.WriteError("Items list via file '" + itemsFileName + "' has an duplicate row with EQItemID '" + newItemTemplate.EQItemID + "'");
                    continue;
                }
                ItemTemplatesByEQDBID.Add(newItemTemplate.EQItemID, newItemTemplate);
            }
        }
    }
}
