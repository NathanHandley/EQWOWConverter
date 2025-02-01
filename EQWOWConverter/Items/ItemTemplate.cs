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

using EQWOWConverter.Common;
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
        private static Dictionary<string, Dictionary<string, float>> StatBaselinesBySlotAndStat = new Dictionary<string, Dictionary<string, float>>();
        private static SortedDictionary<int, ItemTemplate> ItemTemplatesByEQDBID = new SortedDictionary<int, ItemTemplate>();
        private static int CURRENT_SQL_ITEMTEMPLATEENTRYID = Configuration.CONFIG_SQL_ITEM_TEMPLATE_ENTRY_START;

        public int EQItemID = 0;
        public int WOWEntryID = 0;
        public int ClassID = 0;
        public int SubClassID = 0;
        public string Name = string.Empty;
        public ItemWOWQuality Quality = ItemWOWQuality.Poor;
        public int DisplayID = 0;
        public int SheatheType = 0;
        public int Material = -1;
        public int BuyPriceInCopper = 0;
        public int SellPriceInCopper = 0;
        public int BagSlots = 0;
        public int StackSize = 1;
        public ItemWOWInventoryType InventoryType = ItemWOWInventoryType.NoEquip;
        public int WeaponMinDamage = 0;
        public int WeaponMaxDamage = 0;
        public int WeaponDelay = 0;
        public int EQClassMask = 32767;
        public int EQSlotMask = 0;
        public List<ClassType> AllowedClassTypes = new List<ClassType>();
        public List<(ItemWOWStatType, int)> StatValues = new List<(ItemWOWStatType, int)>();
        public int Armor = 0;

        public ItemTemplate()
        {
            WOWEntryID = CURRENT_SQL_ITEMTEMPLATEENTRYID;
            CURRENT_SQL_ITEMTEMPLATEENTRYID++;
        }

        public static SortedDictionary<int, ItemTemplate> GetItemTemplatesByEQDBIDs()
        {
            if (ItemTemplatesByEQDBID.Count == 0)
                PopulateItemTemplateListFromDisk();
            return ItemTemplatesByEQDBID;
        }

        private static float GetConvertedEqToWowStat(ItemWOWInventoryType itemSlot, string statName, float eqStatValue)
        {
            // Read the file if haven't yet
            if (StatBaselinesBySlotAndStat.Count() == 0)
                PopulateStatBaselinesBySlot();

            // Zero or negative stats give nothing back
            bool flipStatSign = false;
            if (eqStatValue <= 0)
            {
                flipStatSign = true;
                eqStatValue *= -1;
            }
                
            // Get the slot row
            string slotNameLower = itemSlot.ToString().ToLower();
            if (StatBaselinesBySlotAndStat.ContainsKey(slotNameLower) == false)
            {
                Logger.WriteError("Could not pull stat for slot '" + slotNameLower + "' as that slot wasn't in the ItemStatBaselines");
                return 0;
            }
            Dictionary<string, float> statsForSlot = StatBaselinesBySlotAndStat[slotNameLower];
            string statNameLower = statName.Trim().ToLower();

            // Pull "EqLow"
            string statNameEqLow = statNameLower + "eqlow";
            float statEqLow = 0;
            if (statsForSlot.ContainsKey(statNameEqLow) == false)
            {
                Logger.WriteError("Could not pull stat for slot '" + slotNameLower + "' as the column '" + statEqLow + "' did not exist in the ItemStatBaselines");
                return 0;
            }
            statEqLow = statsForSlot[statNameEqLow];

            // Pull "EqHigh"
            string statNameEqHigh = statNameLower + "eqhigh";
            float statEqHigh = 0;
            if (statsForSlot.ContainsKey(statNameEqHigh) == false)
            {
                Logger.WriteError("Could not pull stat for slot '" + slotNameLower + "' as the column '" + statEqHigh + "' did not exist in the ItemStatBaselines");
                return 0;
            }
            statEqHigh = statsForSlot[statNameEqHigh];

            // Pull "WowLow"
            string statNameWowLow = statNameLower + "wowlow";
            float statWowLow = 0;
            if (statsForSlot.ContainsKey(statNameWowLow) == false)
            {
                Logger.WriteError("Could not pull stat for slot '" + slotNameLower + "' as the column '" + statWowLow + "' did not exist in the ItemStatBaselines");
                return 0;
            }
            statWowLow = statsForSlot[statNameEqLow];

            // Pull "WowHigh"
            string statNameWowHigh = statNameLower + "wowhigh";
            float statWowHigh = 0;
            if (statsForSlot.ContainsKey(statNameWowHigh) == false)
            {
                Logger.WriteError("Could not pull stat for slot '" + slotNameLower + "' as the column '" + statWowHigh + "' did not exist in the ItemStatBaselines");
                return 0;
            }
            statWowHigh = statsForSlot[statNameWowHigh];

            // Ignore any rows that are without stat
            if (statEqHigh <= 0 || statEqLow <= 0 || statWowHigh <= 0 || statWowLow <= 0)
            {
                Logger.WriteDetail("Could not pull stat for slot '" + slotNameLower + "' as one of the 4 values was <= 0 from TtemStatBaselines");
                return 0;
            }

            // Maintain a minimum boundary
            if (eqStatValue < statEqLow)
                return statWowLow;

            // Calculate the stat
            float normalizedModOfHigh = ((eqStatValue - statEqLow) / (statEqHigh - statEqLow)) / Configuration.CONFIG_ITEMS_STATS_LOW_BIAS_WEIGHT;
            float calculatedStat = (normalizedModOfHigh * statWowHigh) + ((1 - normalizedModOfHigh) * statWowLow);

            // Flip the sign if needed
            if (flipStatSign == true)
                calculatedStat *= -1;

            return calculatedStat;
        }

        // TODO: May not need class/subclass
        private static void PopulateStats(ref ItemTemplate itemTemplate, ItemWOWInventoryType itemSlot, int classID, int subClassID, 
            int eqArmorClass, int eqStrength, int eqAgility, int eqCharisma, int eqDexterity, int eqIntelligence, int eqStamina, int eqWisdom,
            int eqHp, int eqMana, int eqResistPoison, int eqResistMagic, int eqResistDisease, int eqResistFire, int eqResistCold)
        {
            itemTemplate.StatValues.Clear();

            // Armor Class (can't process negatives for armor)
            if (eqArmorClass > 0)
                itemTemplate.Armor = Convert.ToInt32(GetConvertedEqToWowStat(itemSlot, "ac", eqArmorClass));

            // Strength
            if (eqStrength != 0)
                itemTemplate.StatValues.Add((ItemWOWStatType.Strength, Convert.ToInt32(GetConvertedEqToWowStat(itemSlot, "str", eqStrength))));

            // Agility
            // Note: The highest between Dex or Agl is used
            if (eqDexterity != 0 || eqAgility != 0)
            {
                int pickedStat = Math.Max(eqDexterity, eqAgility);
                if (pickedStat == 0) // One is zero, one is below zero
                    pickedStat = Math.Min(eqDexterity, eqAgility);
                itemTemplate.StatValues.Add((ItemWOWStatType.Agility, Convert.ToInt32(GetConvertedEqToWowStat(itemSlot, "agi", pickedStat))));
            }

            // Intelligence
            if (eqIntelligence != 0)
                itemTemplate.StatValues.Add((ItemWOWStatType.Intellect, Convert.ToInt32(GetConvertedEqToWowStat(itemSlot, "int", eqIntelligence))));

            // Stamina
            if (eqStamina != 0)
                itemTemplate.StatValues.Add((ItemWOWStatType.Stamina, Convert.ToInt32(GetConvertedEqToWowStat(itemSlot, "sta", eqStamina))));

            // Spirit
            // Note: This is converted from "Wisdom"
            if (eqWisdom != 0)
                itemTemplate.StatValues.Add((ItemWOWStatType.Spirit, Convert.ToInt32(GetConvertedEqToWowStat(itemSlot, "spr", eqWisdom))));

            // Hit (Charisma)
            // Note: This is being mapped to "hit".  There wasn't a better option, and it makes this stat useful on equipment
            if (eqCharisma != 0)
                itemTemplate.StatValues.Add((ItemWOWStatType.HitRating, Convert.ToInt32(GetConvertedEqToWowStat(itemSlot, "hitrating", eqCharisma))));

            // HP
            if (eqHp != 0)
                itemTemplate.StatValues.Add((ItemWOWStatType.Health, Convert.ToInt32(GetConvertedEqToWowStat(itemSlot, "hp", eqHp))));

            // Mana
            if (eqMana != 0)
                itemTemplate.StatValues.Add((ItemWOWStatType.Mana, Convert.ToInt32(GetConvertedEqToWowStat(itemSlot, "mp", eqMana))));

            // Resist Magic

            // Resist Nature

            // Resist Shadow

            // Resist Fire

            // Resist Cold

            // Block Value
        }

        private static ItemWOWQuality CalculateQuality(List<(ItemWOWStatType, int)> statValues)
        {
            if (statValues.Count > 0)
                return ItemWOWQuality.Uncommon;
            else
                return ItemWOWQuality.Common;
        }

        private enum WeaponIconImpliedType
        {
            BladeSlash,
            BladeStab,
            Axe,
            Polearm,
            BluntNonStaff,
            BluntStaff,
            Hand2Hand
        }

        private static WeaponIconImpliedType GetWeaponImpliedTypeFromIcon(int iconID)
        {
            switch (iconID)
            {
                case 19: return WeaponIconImpliedType.BladeSlash;
                case 47: return WeaponIconImpliedType.BluntNonStaff;
                case 59: return WeaponIconImpliedType.BluntNonStaff;
                case 67: return WeaponIconImpliedType.BluntNonStaff;
                case 68: return WeaponIconImpliedType.Axe;
                case 69: return WeaponIconImpliedType.Axe;
                case 73: return WeaponIconImpliedType.Axe;
                case 78: return WeaponIconImpliedType.BluntNonStaff;
                case 81: return WeaponIconImpliedType.BluntNonStaff;
                case 74: return WeaponIconImpliedType.BladeSlash;
                case 75: return WeaponIconImpliedType.BladeSlash;
                case 76: return WeaponIconImpliedType.BladeSlash;
                case 77: return WeaponIconImpliedType.BladeSlash;
                case 79: return WeaponIconImpliedType.BladeSlash; // Scythe icon
                case 80: return WeaponIconImpliedType.BladeSlash;
                case 88: return WeaponIconImpliedType.BladeSlash;
                case 89: return WeaponIconImpliedType.BladeSlash;
                case 90: return WeaponIconImpliedType.BladeSlash;
                case 91: return WeaponIconImpliedType.BladeSlash;
                case 92: return WeaponIconImpliedType.BladeSlash;
                case 101: return WeaponIconImpliedType.BluntStaff;
                case 102: return WeaponIconImpliedType.BluntStaff;
                case 103: return WeaponIconImpliedType.BladeSlash;
                case 104: return WeaponIconImpliedType.BladeSlash;
                case 105: return WeaponIconImpliedType.BladeSlash;
                case 186: return WeaponIconImpliedType.BluntStaff;
                case 236: return WeaponIconImpliedType.Polearm;
                case 237: return WeaponIconImpliedType.BluntNonStaff;
                case 238: return WeaponIconImpliedType.BluntNonStaff;
                case 240: return WeaponIconImpliedType.Polearm;
                case 241: return WeaponIconImpliedType.BluntNonStaff;
                case 242: return WeaponIconImpliedType.Polearm;
                case 249: return WeaponIconImpliedType.BluntStaff; // Fishing Pole
                case 262: return WeaponIconImpliedType.BladeSlash;
                case 263: return WeaponIconImpliedType.BladeSlash;
                case 268: return WeaponIconImpliedType.Axe;
                case 276: return WeaponIconImpliedType.Polearm;
                case 281: return WeaponIconImpliedType.BladeSlash; // Scythe icon
                case 309: return WeaponIconImpliedType.BluntStaff; // Potential wand
                case 310: return WeaponIconImpliedType.BluntStaff; // Potential wand
                case 311: return WeaponIconImpliedType.BluntStaff; // Potential wand
                case 321: return WeaponIconImpliedType.BluntNonStaff;
                case 322: return WeaponIconImpliedType.BluntNonStaff;
                case 347: return WeaponIconImpliedType.BladeSlash;
                case 382: return WeaponIconImpliedType.BladeSlash;
                case 387: return WeaponIconImpliedType.BluntStaff;
                case 388: return WeaponIconImpliedType.Axe;
                case 389: return WeaponIconImpliedType.BluntStaff;
                case 390: return WeaponIconImpliedType.BladeSlash;
                case 391: return WeaponIconImpliedType.BluntNonStaff;
                case 402: return WeaponIconImpliedType.Axe;
                case 403: return WeaponIconImpliedType.BluntNonStaff;
                case 473: return WeaponIconImpliedType.BluntNonStaff;
                case 475: return WeaponIconImpliedType.Hand2Hand;
                case 583: return WeaponIconImpliedType.BluntStaff;
                case 617: return WeaponIconImpliedType.BluntNonStaff;
                case 643: return WeaponIconImpliedType.BluntStaff; // Fishing Pole
                case 663: return WeaponIconImpliedType.Polearm;
                case 664: return WeaponIconImpliedType.Axe;
                case 665: return WeaponIconImpliedType.BladeSlash; // Might be stab
                case 666: return WeaponIconImpliedType.Axe;
                case 667: return WeaponIconImpliedType.Axe;
                case 668: return WeaponIconImpliedType.BladeSlash;
                case 669: return WeaponIconImpliedType.BladeSlash;
                case 670: return WeaponIconImpliedType.BladeSlash; // Might be stab, but probably slash
                case 671: return WeaponIconImpliedType.BladeSlash;
                case 672: return WeaponIconImpliedType.BluntNonStaff; // Might be a staff
                case 673: return WeaponIconImpliedType.BladeSlash;
                case 674: return WeaponIconImpliedType.BladeSlash;
                case 675: return WeaponIconImpliedType.BluntNonStaff;
                case 676: return WeaponIconImpliedType.BladeSlash;
                case 677: return WeaponIconImpliedType.BladeSlash;
                case 678: return WeaponIconImpliedType.BladeSlash;
                case 679: return WeaponIconImpliedType.Hand2Hand;
                case 680: return WeaponIconImpliedType.Hand2Hand;
                case 681: return WeaponIconImpliedType.Polearm; // May not be a polearm
                case 682: return WeaponIconImpliedType.Hand2Hand; // Could be dagger, but I recall this was for monk weapons
                case 683: return WeaponIconImpliedType.BladeStab; // Could also be slash
                case 684: return WeaponIconImpliedType.BladeSlash;
                case 685: return WeaponIconImpliedType.BladeSlash;
                case 686: return WeaponIconImpliedType.BladeSlash;
                case 687: return WeaponIconImpliedType.Polearm;
                case 688: return WeaponIconImpliedType.BluntNonStaff;
                case 689: return WeaponIconImpliedType.BluntNonStaff;
                case 690: return WeaponIconImpliedType.BladeSlash;
                case 691: return WeaponIconImpliedType.BladeSlash;
                case 692: return WeaponIconImpliedType.BladeSlash;
                case 693: return WeaponIconImpliedType.BladeSlash;
                case 694: return WeaponIconImpliedType.BluntNonStaff;
                case 695: return WeaponIconImpliedType.BladeSlash;
                case 711: return WeaponIconImpliedType.BladeSlash;
                case 712: return WeaponIconImpliedType.BladeSlash;
                case 713: return WeaponIconImpliedType.BluntStaff;
                case 714: return WeaponIconImpliedType.BladeStab; // Could be slash
                case 715: return WeaponIconImpliedType.BladeSlash; // Could be slash
                case 716: return WeaponIconImpliedType.Polearm;
                default: return WeaponIconImpliedType.BluntNonStaff; // Default to 'blunt' since it would probably be some odd object
            }
        }

        private static ItemWOWWeaponSubclassType GetWeaponSubclass(int eqItemID, int eqItemType, int iconID)
        {
            WeaponIconImpliedType iconWeaponImpliedType = GetWeaponImpliedTypeFromIcon(iconID);
            switch (eqItemType)
            {
                case 0: // 1h Slashing + Junk
                    {
                        switch (iconWeaponImpliedType)
                        {
                            case WeaponIconImpliedType.BluntNonStaff: return ItemWOWWeaponSubclassType.MaceOneHand;
                            case WeaponIconImpliedType.Hand2Hand: return ItemWOWWeaponSubclassType.FistWeapon;
                            case WeaponIconImpliedType.BladeStab: return ItemWOWWeaponSubclassType.Dagger;
                            case WeaponIconImpliedType.Axe: return ItemWOWWeaponSubclassType.AxeOneHand;
                            default: return ItemWOWWeaponSubclassType.SwordOneHand;
                        }
                    }
                case 1: // 2h Slashing
                    {
                        switch (iconWeaponImpliedType)
                        {
                            case WeaponIconImpliedType.Axe: return ItemWOWWeaponSubclassType.AxeTwoHand;
                            case WeaponIconImpliedType.Polearm: return ItemWOWWeaponSubclassType.Polearm;
                            default: return ItemWOWWeaponSubclassType.SwordTwoHand;
                        }
                    }
                case 2: // 1h Pierce
                    {
                        switch (iconWeaponImpliedType)
                        {
                            case WeaponIconImpliedType.Hand2Hand: return ItemWOWWeaponSubclassType.FistWeapon;
                            default: return ItemWOWWeaponSubclassType.Dagger;
                        }
                    }
                case 35: // 2h Pierce
                    {
                        return ItemWOWWeaponSubclassType.Polearm;
                    }
                case 3: // 1h Blunt
                    {
                        return ItemWOWWeaponSubclassType.MaceOneHand;
                    }
                case 4: // 2h Blunt
                    {
                        switch (iconWeaponImpliedType)
                        {
                            case WeaponIconImpliedType.BluntStaff: return ItemWOWWeaponSubclassType.Staff;
                            default: return ItemWOWWeaponSubclassType.MaceTwoHand;
                        }
                    }
                default:
                    {
                        Logger.WriteError("Unhandled iconID of `" + eqItemType + "` in GetWeaponSubclass");
                        return 0;
                    }
            }
        }

        private static ItemWOWInventoryType GetInventoryTypeFromSlotMask(int slotMask)
        {
            if (slotMask == 0)
                return 0;
            if (IsPackedSlotMask(ItemEQEquipSlotBitmaskType.Chest, slotMask))
                return ItemWOWInventoryType.Chest;
            if (IsPackedSlotMask(ItemEQEquipSlotBitmaskType.Hands, slotMask))
                return ItemWOWInventoryType.Hands;
            if (IsPackedSlotMask(ItemEQEquipSlotBitmaskType.Ear1, slotMask))
                return ItemWOWInventoryType.Trinket;
            if (IsPackedSlotMask(ItemEQEquipSlotBitmaskType.Ear2, slotMask))
                return ItemWOWInventoryType.Trinket;
            if (IsPackedSlotMask(ItemEQEquipSlotBitmaskType.Ring1, slotMask))
                return ItemWOWInventoryType.Finger;
            if (IsPackedSlotMask(ItemEQEquipSlotBitmaskType.Ring2, slotMask))
                return ItemWOWInventoryType.Finger;
            if (IsPackedSlotMask(ItemEQEquipSlotBitmaskType.Ammo, slotMask))
                return ItemWOWInventoryType.Ammo;
            if (IsPackedSlotMask(ItemEQEquipSlotBitmaskType.Feet, slotMask))
                return ItemWOWInventoryType.Feet;
            if (IsPackedSlotMask(ItemEQEquipSlotBitmaskType.Head, slotMask))
                return ItemWOWInventoryType.Head;
            if (IsPackedSlotMask(ItemEQEquipSlotBitmaskType.Face, slotMask))
                return ItemWOWInventoryType.Head;
            if (IsPackedSlotMask(ItemEQEquipSlotBitmaskType.Shoulder, slotMask))
                return ItemWOWInventoryType.Shoulder;
            if (IsPackedSlotMask(ItemEQEquipSlotBitmaskType.Arms, slotMask))
                return ItemWOWInventoryType.Shoulder;
            if (IsPackedSlotMask(ItemEQEquipSlotBitmaskType.Wrist1, slotMask))
                return ItemWOWInventoryType.Wrists;
            if (IsPackedSlotMask(ItemEQEquipSlotBitmaskType.Wrist2, slotMask))
                return ItemWOWInventoryType.Wrists;
            if (IsPackedSlotMask(ItemEQEquipSlotBitmaskType.Legs, slotMask))
                return ItemWOWInventoryType.Legs;
            if (IsPackedSlotMask(ItemEQEquipSlotBitmaskType.Waist, slotMask))
                return ItemWOWInventoryType.Waist;
            if (IsPackedSlotMask(ItemEQEquipSlotBitmaskType.Neck, slotMask))
                return ItemWOWInventoryType.Neck;
            if (IsPackedSlotMask(ItemEQEquipSlotBitmaskType.Back, slotMask))
                return ItemWOWInventoryType.Back;
            return ItemWOWInventoryType.NoEquip;
        }
        private static bool IsPackedSlotMask(ItemEQEquipSlotBitmaskType itemSlotBitmaskType, int slotMask)
        {
            if ((slotMask & Convert.ToInt32(itemSlotBitmaskType)) == Convert.ToInt32(itemSlotBitmaskType))
                return true;
            return false;
        }

        private static ItemWOWArmorSubclassType GetArmorSubclass(int classMask)
        {
            if (classMask == 0)
                return ItemWOWArmorSubclassType.Misc;
            if (classMask >= 32767)
                return ItemWOWArmorSubclassType.Cloth;
            if (IsPackedClassMask(ItemEQClassBitmaskType.Necromancer, classMask))
                return ItemWOWArmorSubclassType.Cloth;
            if (IsPackedClassMask(ItemEQClassBitmaskType.Wizard, classMask))
                return ItemWOWArmorSubclassType.Cloth;
            if (IsPackedClassMask(ItemEQClassBitmaskType.Magician, classMask))
                return ItemWOWArmorSubclassType.Cloth;
            if (IsPackedClassMask(ItemEQClassBitmaskType.Enchanter, classMask))
                return ItemWOWArmorSubclassType.Cloth;
            // Clerics can wear plate in EQ, but only make cleric gear cloth if it's cleric-only
            if (IsPackedClassMask(ItemEQClassBitmaskType.Cleric, classMask) &&
                IsPackedClassMask(ItemEQClassBitmaskType.Warrior, classMask) == false &&
                IsPackedClassMask(ItemEQClassBitmaskType.Paladin, classMask) == false &&
                IsPackedClassMask(ItemEQClassBitmaskType.ShadowKnight, classMask) == false)
                return ItemWOWArmorSubclassType.Cloth;
            if (IsPackedClassMask(ItemEQClassBitmaskType.Druid, classMask))
                return ItemWOWArmorSubclassType.Leather;
            if (IsPackedClassMask(ItemEQClassBitmaskType.Monk, classMask))
                return ItemWOWArmorSubclassType.Leather;
            if (IsPackedClassMask(ItemEQClassBitmaskType.Beastlord, classMask))
                return ItemWOWArmorSubclassType.Leather;
            if (IsPackedClassMask(ItemEQClassBitmaskType.Ranger, classMask))
                return ItemWOWArmorSubclassType.Mail;
            if (IsPackedClassMask(ItemEQClassBitmaskType.Shaman, classMask))
                return ItemWOWArmorSubclassType.Mail;
            // Note: EQ rogues can wear mail/chain, so this will be an issue in some cases
            if (IsPackedClassMask(ItemEQClassBitmaskType.Rogue, classMask))
                return ItemWOWArmorSubclassType.Leather;
            if (IsPackedClassMask(ItemEQClassBitmaskType.Warrior, classMask))
                return ItemWOWArmorSubclassType.Plate;
            if (IsPackedClassMask(ItemEQClassBitmaskType.Paladin, classMask))
                return ItemWOWArmorSubclassType.Plate;
            if (IsPackedClassMask(ItemEQClassBitmaskType.ShadowKnight, classMask))
                return ItemWOWArmorSubclassType.Plate;
            if (IsPackedClassMask(ItemEQClassBitmaskType.Bard, classMask))
                return ItemWOWArmorSubclassType.Plate;
            // Clerics can wear plate in EQ
            if (IsPackedClassMask(ItemEQClassBitmaskType.Cleric, classMask))
                return ItemWOWArmorSubclassType.Plate;
            return ItemWOWArmorSubclassType.Misc;
        }

        private static List<ClassType> GetClassTypesFromClassMask(int classMask, int classID, int subClassID)
        {
            List<ClassType> classTypes = new List<ClassType>();

            if (classMask <= 0 || classMask >= 32767)
            {
                classTypes.Add(ClassType.All);
                return classTypes;
            }

            if (IsPackedClassMask(ItemEQClassBitmaskType.Necromancer, classMask) || IsPackedClassMask(ItemEQClassBitmaskType.Enchanter, classMask))
                classTypes.Add(ClassType.Warlock);
            if (IsPackedClassMask(ItemEQClassBitmaskType.Wizard, classMask) || IsPackedClassMask(ItemEQClassBitmaskType.Magician, classMask))
                classTypes.Add(ClassType.Mage);
            if (IsPackedClassMask(ItemEQClassBitmaskType.Beastlord, classMask) || IsPackedClassMask(ItemEQClassBitmaskType.Ranger, classMask))
                classTypes.Add(ClassType.Hunter);
            if (IsPackedClassMask(ItemEQClassBitmaskType.Monk, classMask) || IsPackedClassMask(ItemEQClassBitmaskType.Rogue, classMask))
                classTypes.Add(ClassType.Rogue);
            if (IsPackedClassMask(ItemEQClassBitmaskType.Bard, classMask) || IsPackedClassMask(ItemEQClassBitmaskType.Warrior, classMask))
                classTypes.Add(ClassType.Warrior);
            if (IsPackedClassMask(ItemEQClassBitmaskType.Shaman, classMask))
                classTypes.Add(ClassType.Shaman);
            if (IsPackedClassMask(ItemEQClassBitmaskType.Druid, classMask))
                classTypes.Add(ClassType.Druid);
            if (IsPackedClassMask(ItemEQClassBitmaskType.Paladin, classMask))
                classTypes.Add(ClassType.Paladin);
            if (IsPackedClassMask(ItemEQClassBitmaskType.ShadowKnight, classMask))
                classTypes.Add(ClassType.DeathKnight);
            if (IsPackedClassMask(ItemEQClassBitmaskType.Cleric, classMask))
                classTypes.Add(ClassType.Priest);

            // If set, collapse common armors and weapons
            if (Configuration.CONFIG_ITEMS_ALLOW_ALL_CLASSES_ON_GENERIC_EQUIP == true)
            {
                // Weapon
                if (classID == 2)
                {
                    if (classTypes.Count >= 3)
                    {
                        classTypes.Clear();
                        classTypes.Add(ClassType.All);
                    }
                }

                // Armor
                else if (classID == 4)
                {
                    // Leather and higher
                    if (subClassID >= 2 && classTypes.Count >= 4)
                    {
                        classTypes.Clear();
                        classTypes.Add(ClassType.All);
                    }
                }
            }
            
            return classTypes;
        }

        private static bool IsPackedClassMask(ItemEQClassBitmaskType itemClassBitmaskType, int classMask)
        {
            if ((classMask & Convert.ToInt32(itemClassBitmaskType)) == Convert.ToInt32(itemClassBitmaskType))
                return true;
            return false;
        }

        static private void PopulateEquippableItemProperties(ref ItemTemplate itemTemplate, int eqItemType, int bagType, int classMask, int slotMask, int iconID)
        {
            switch (eqItemType)
            {
                case 0: // Catches a lot of items
                    {
                        if (bagType != 0)
                        {
                            itemTemplate.InventoryType = ItemWOWInventoryType.Bag;
                            switch (bagType)
                            {
                                case 2: // Quiver
                                    {
                                        itemTemplate.ClassID = 2;
                                        itemTemplate.SubClassID = 3;
                                        itemTemplate.InventoryType = ItemWOWInventoryType.Quiver;
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
                            // 1 Hand Slash => 1h Sword or Axe
                            itemTemplate.ClassID = 2;
                            itemTemplate.SubClassID = Convert.ToInt32(GetWeaponSubclass(itemTemplate.EQItemID, eqItemType, iconID));
                            itemTemplate.InventoryType = ItemWOWInventoryType.OneHand;
                        }
                    } break;
                case 1: // 2 Hand Slash => 2h Sword or Axe
                    {
                        itemTemplate.ClassID = 2;
                        itemTemplate.SubClassID = Convert.ToInt32(GetWeaponSubclass(itemTemplate.EQItemID, eqItemType, iconID));
                        itemTemplate.InventoryType = ItemWOWInventoryType.TwoHand;
                    } break;
                case 2: // 1 Hand Pierce => Dagger
                    {
                        itemTemplate.ClassID = 2;
                        itemTemplate.SubClassID = Convert.ToInt32(GetWeaponSubclass(itemTemplate.EQItemID, eqItemType, iconID));
                        itemTemplate.InventoryType = ItemWOWInventoryType.OneHand;
                    } break;
                case 35: // 2 Hand Pierce => Polearm
                    {
                        itemTemplate.ClassID = 2;
                        itemTemplate.SubClassID = Convert.ToInt32(GetWeaponSubclass(itemTemplate.EQItemID, eqItemType, iconID));
                        itemTemplate.InventoryType = ItemWOWInventoryType.TwoHand;
                    } break;
                case 3: // 1 Hand Blunt => 1H Mace
                    {
                        itemTemplate.ClassID = 2;
                        itemTemplate.SubClassID = Convert.ToInt32(GetWeaponSubclass(itemTemplate.EQItemID, eqItemType, iconID));
                        itemTemplate.InventoryType = ItemWOWInventoryType.OneHand;
                    } break;
                case 4: // 2 Hand Blunt => 2H Mace or Staff
                    {
                        itemTemplate.ClassID = 2;
                        itemTemplate.SubClassID = Convert.ToInt32(GetWeaponSubclass(itemTemplate.EQItemID, eqItemType, iconID));
                        itemTemplate.InventoryType = ItemWOWInventoryType.TwoHand;
                    } break;
                case 5: // Bow
                    {
                        itemTemplate.ClassID = 2;
                        itemTemplate.SubClassID = Convert.ToInt32(ItemWOWWeaponSubclassType.Bow);
                        itemTemplate.InventoryType = ItemWOWInventoryType.Ranged;
                    } break;
                case 7: // Thrown
                    {
                        itemTemplate.ClassID = 2;
                        itemTemplate.SubClassID = Convert.ToInt32(ItemWOWWeaponSubclassType.Thrown);
                        itemTemplate.InventoryType = ItemWOWInventoryType.Thrown;
                    } break;
                case 8: // Shield
                    {
                        itemTemplate.ClassID = 4;
                        itemTemplate.SubClassID = 6;
                        itemTemplate.InventoryType = ItemWOWInventoryType.Shield;
                    } break;
                case 10: // Armor
                    {
                        itemTemplate.ClassID = 4;
                        itemTemplate.SubClassID = Convert.ToInt32(GetArmorSubclass(classMask));
                        itemTemplate.InventoryType = GetInventoryTypeFromSlotMask(slotMask);
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
                        itemTemplate.InventoryType = ItemWOWInventoryType.Thrown;
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
                        itemTemplate.InventoryType = ItemWOWInventoryType.Ammo;
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
                        itemTemplate.InventoryType = ItemWOWInventoryType.Finger; // TODO: Neck vs Earring vs Ring
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
                        itemTemplate.InventoryType = ItemWOWInventoryType.TwoHand;
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
                        itemTemplate.InventoryType = ItemWOWInventoryType.OneHand;
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
            string itemsFileName = Path.Combine(Configuration.CONFIG_PATH_ASSETS_FOLDER, "WorldData", "ItemTemplates.csv");
            Logger.WriteDetail("Populating item templates via file '" + itemsFileName + "'");
            List<string> itemTemplateRows = FileTool.ReadAllStringLinesFromFile(itemsFileName, true, true);

            // Load all of the data
            foreach (string row in itemTemplateRows)
            {
                // Load the row
                string[] rowBlocks = row.Split("|");
                ItemTemplate newItemTemplate = new ItemTemplate();
                newItemTemplate.EQItemID = int.Parse(rowBlocks[0]);
                newItemTemplate.Name = rowBlocks[1];

                // Icon
                int iconID = int.Parse(rowBlocks[3]) - 500;
                string iconName = "INV_EQ_" + (iconID).ToString();
                ItemDisplayInfo itemDisplayInfo = ItemDisplayInfo.GetOrCreateItemDisplayInfo(iconName);
                newItemTemplate.DisplayID = itemDisplayInfo.DBCID;

                // Equippable Properties
                int itemType = int.Parse(rowBlocks[2]);
                int bagType = int.Parse(rowBlocks[7]);
                newItemTemplate.EQClassMask = int.Parse(rowBlocks[12]);
                newItemTemplate.EQSlotMask = int.Parse(rowBlocks[13]);
                PopulateEquippableItemProperties(ref newItemTemplate, itemType, bagType, newItemTemplate.EQClassMask, newItemTemplate.EQSlotMask, iconID);

                // Price
                newItemTemplate.BuyPriceInCopper = int.Parse(rowBlocks[4]);
                if (newItemTemplate.BuyPriceInCopper <= 0)
                    newItemTemplate.BuyPriceInCopper = 1;
                newItemTemplate.SellPriceInCopper = int.Max(Convert.ToInt32(Convert.ToDouble(newItemTemplate.BuyPriceInCopper) * 0.25), 1);

                // Other
                newItemTemplate.BagSlots = int.Parse(rowBlocks[8]);
                newItemTemplate.StackSize = int.Max(int.Parse(rowBlocks[9]), 1);
                newItemTemplate.AllowedClassTypes = GetClassTypesFromClassMask(newItemTemplate.EQClassMask, newItemTemplate.ClassID, newItemTemplate.SubClassID);

                // Calculate the weapon damage
                int damage = int.Parse(rowBlocks[10]);
                int delay = int.Parse(rowBlocks[11]) * 100;
                if (damage > 0 && newItemTemplate.ClassID == 2)
                {
                    float calcDps = GetConvertedEqToWowStat(newItemTemplate.InventoryType, "dps", damage);
                    if (calcDps != 0)
                    {
                        // Min/Max damage ranges are a +/- 20% 
                        newItemTemplate.WeaponMinDamage = Convert.ToInt32(Math.Round(calcDps * 0.8f));
                        newItemTemplate.WeaponMaxDamage = Convert.ToInt32(Math.Round(calcDps * 1.2f));

                        // Scale the delay
                        if (delay > 0)
                            newItemTemplate.WeaponDelay = Convert.ToInt32(Math.Round(Convert.ToSingle(delay) * (1 - Configuration.CONFIG_ITEMS_WEAPON_DELAY_REDUCTION_AMT)));
                    }
                }

                // Calculate stats
                int agility = int.Parse(rowBlocks[14]);
                int armorClass = int.Parse(rowBlocks[15]);
                int charisma = int.Parse(rowBlocks[16]);
                int dexterity = int.Parse(rowBlocks[17]);
                int intelligence = int.Parse(rowBlocks[18]);
                int stamina = int.Parse(rowBlocks[19]);
                int strength = int.Parse(rowBlocks[20]);
                int wisdom = int.Parse(rowBlocks[21]);
                int hp = int.Parse(rowBlocks[22]);
                int mana = int.Parse(rowBlocks[23]);
                int resistMagic = int.Parse(rowBlocks[24]);
                int resistDisease = int.Parse(rowBlocks[25]);
                int resistPoison = int.Parse(rowBlocks[26]);
                int resistCold = int.Parse(rowBlocks[27]);
                int resistFire = int.Parse(rowBlocks[28]);
                PopulateStats(ref newItemTemplate, newItemTemplate.InventoryType, newItemTemplate.ClassID, newItemTemplate.SubClassID,
                    armorClass, strength, agility, charisma, dexterity, intelligence, stamina, wisdom, hp, mana, resistPoison, resistMagic, 
                    resistDisease, resistFire, resistCold);

                // Set the quality
                newItemTemplate.Quality = CalculateQuality(newItemTemplate.StatValues);

                // Add
                if (ItemTemplatesByEQDBID.ContainsKey(newItemTemplate.EQItemID))
                {
                    Logger.WriteError("Items list via file '" + itemsFileName + "' has an duplicate row with EQItemID '" + newItemTemplate.EQItemID + "'");
                    continue;
                }
                ItemTemplatesByEQDBID.Add(newItemTemplate.EQItemID, newItemTemplate);
            }
        }

        private static void PopulateStatBaselinesBySlot()
        {
            string itemStatBaselineFile = Path.Combine(Configuration.CONFIG_PATH_ASSETS_FOLDER, "WorldData", "ItemStatBaselines.csv");
            Logger.WriteDetail("Populating Item Stat Baselines list via file '" + itemStatBaselineFile + "'");
            List<string> itemStatBaselineRows = FileTool.ReadAllStringLinesFromFile(itemStatBaselineFile, false, true);
            bool isFirstRow = true;
            List<string> stats = new List<string>();
            foreach (string row in itemStatBaselineRows)
            {
                // Load the row
                string[] rowBlocks = row.Split("|");

                // If it's the first row, build out the stats
                if (isFirstRow == true)
                {
                    foreach(string block in rowBlocks)
                    {
                        string lowerText = block.Trim().ToLower();
                        if (lowerText == "slot")
                            continue;
                        stats.Add(lowerText);
                    }

                    isFirstRow = false;
                    continue;
                }

                // Otherwise, load the stats
                string slot = rowBlocks[0].Trim().ToLower();
                StatBaselinesBySlotAndStat.Add(slot, new Dictionary<string, float>());
                for (int i = 1; i < rowBlocks.Count(); i++)
                    StatBaselinesBySlotAndStat[slot].Add(stats[i - 1], float.Parse(rowBlocks[i]));
            }
        }
    }
}
