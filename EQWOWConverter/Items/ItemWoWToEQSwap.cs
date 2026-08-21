//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2026 Nathan Handley
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

using EQWOWConverter.Common;

namespace EQWOWConverter.Items
{
    internal class ItemWoWToEQSwap
    {
        private static List<ItemWoWToEQSwap> ItemWoWToEQSwaps = new List<ItemWoWToEQSwap>();
        private static readonly object SwapLock = new object();

        public ItemWOWInventoryType InventoryType = ItemWOWInventoryType.NoEquip;
        public int ClassID = 0;
        public int SubClassID = 0;
        public ClassEQType EQClass = ClassEQType.None;
        public int WOWItemTemplateID = 0;

        public static List<ItemWoWToEQSwap> GetAllItemWoWToEQSwaps()
        {
            lock (SwapLock)
            {
                if (ItemWoWToEQSwaps.Count == 0)
                    PopulateItemWoWToEQSwaps();
                return ItemWoWToEQSwaps;
            }
        }

        private static void PopulateItemWoWToEQSwaps()
        {
            ItemWoWToEQSwaps.Clear();

            string swapFileName = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "ItemWoWToEQSwaps.csv");
            Logger.WriteDebug(string.Concat("Populating Item WoW To EQ Swaps via file '", swapFileName, "'"));
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(swapFileName, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                ItemWOWInventoryType inventoryType;
                if (TryGetInventoryTypeFromName(columns["InventoryType"].Trim(), out inventoryType) == false)
                {
                    Logger.WriteError("ItemWoWToEQSwap could not convert InventoryType '", columns["InventoryType"], "' into an inventory type, so the row was skipped");
                    continue;
                }

                ClassEQType eqClass;
                if (TryGetEQClassFromName(columns["EQClassID"].Trim(), out eqClass) == false)
                {
                    Logger.WriteError("ItemWoWToEQSwap could not convert EQClassID '", columns["EQClassID"], "' into an EQ class, so the row was skipped");
                    continue;
                }

                ItemWoWToEQSwap swap = new ItemWoWToEQSwap();
                swap.InventoryType = inventoryType;
                swap.ClassID = int.Parse(columns["ClassID"].Trim());
                swap.SubClassID = int.Parse(columns["SubClassID"].Trim());
                swap.EQClass = eqClass;
                swap.WOWItemTemplateID = int.Parse(columns["WOWItemTemplateID"].Trim());
                ItemWoWToEQSwaps.Add(swap);
            }
        }

        private static bool TryGetInventoryTypeFromName(string inventoryTypeName, out ItemWOWInventoryType inventoryType)
        {
            inventoryType = ItemWOWInventoryType.NoEquip;
            switch (inventoryTypeName.ToLower())
            {
                case "head": inventoryType = ItemWOWInventoryType.Head; return true;
                case "neck": inventoryType = ItemWOWInventoryType.Neck; return true;
                case "shoulder": inventoryType = ItemWOWInventoryType.Shoulder; return true;
                case "shirt": inventoryType = ItemWOWInventoryType.Shirt; return true;
                case "chest": inventoryType = ItemWOWInventoryType.Chest; return true;
                case "waist": inventoryType = ItemWOWInventoryType.Waist; return true;
                case "legs": inventoryType = ItemWOWInventoryType.Legs; return true;
                case "feet": inventoryType = ItemWOWInventoryType.Feet; return true;
                case "wrist":
                case "wrists": inventoryType = ItemWOWInventoryType.Wrists; return true;
                case "hands": inventoryType = ItemWOWInventoryType.Hands; return true;
                case "finger": inventoryType = ItemWOWInventoryType.Finger; return true;
                case "trinket": inventoryType = ItemWOWInventoryType.Trinket; return true;
                case "onehand": inventoryType = ItemWOWInventoryType.OneHand; return true;
                case "shield": inventoryType = ItemWOWInventoryType.Shield; return true;
                case "ranged": inventoryType = ItemWOWInventoryType.Ranged; return true;
                case "back": inventoryType = ItemWOWInventoryType.Back; return true;
                case "twohand": inventoryType = ItemWOWInventoryType.TwoHand; return true;
                case "tabard": inventoryType = ItemWOWInventoryType.Tabard; return true;
                case "robe": inventoryType = ItemWOWInventoryType.Robe; return true;
                case "mainhand": inventoryType = ItemWOWInventoryType.MainHand; return true;
                case "offhandweapon": inventoryType = ItemWOWInventoryType.OffHandWeapon; return true;
                case "heldinoffhand": inventoryType = ItemWOWInventoryType.HeldInOffHand; return true;
                case "thrown": inventoryType = ItemWOWInventoryType.Thrown; return true;
                case "rangedright": inventoryType = ItemWOWInventoryType.RangedRight; return true;
                case "relic": inventoryType = ItemWOWInventoryType.Relic; return true;
                default: return false;
            }
        }

        private static bool TryGetEQClassFromName(string eqClassName, out ClassEQType eqClass)
        {
            eqClass = ClassEQType.None;
            switch (eqClassName.ToLower())
            {
                case "bard": eqClass = ClassEQType.Bard; return true;
                case "cleric": eqClass = ClassEQType.Cleric; return true;
                case "druid": eqClass = ClassEQType.Druid; return true;
                case "enchanter": eqClass = ClassEQType.Enchanter; return true;
                case "magician": eqClass = ClassEQType.Magician; return true;
                case "monk": eqClass = ClassEQType.Monk; return true;
                case "necromancer": eqClass = ClassEQType.Necromancer; return true;
                case "paladin": eqClass = ClassEQType.Paladin; return true;
                case "ranger": eqClass = ClassEQType.Ranger; return true;
                case "rogue": eqClass = ClassEQType.Rogue; return true;
                case "shadowknight": eqClass = ClassEQType.ShadowKnight; return true;
                case "shaman": eqClass = ClassEQType.Shaman; return true;
                case "warrior": eqClass = ClassEQType.Warrior; return true;
                case "wizard": eqClass = ClassEQType.Wizard; return true;
                default: return false;
            }
        }
    }
}
