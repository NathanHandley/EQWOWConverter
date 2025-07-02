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
using EQWOWConverter.Items;

namespace EQWOWConverter.WOWFiles
{
    internal class ItemTemplateSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM `item_template` WHERE `entry` >= " + Configuration.SQL_ITEM_TEMPLATE_ENTRY_START + " AND `entry` <= " + Configuration.SQL_ITEM_TEMPLATE_ENTRY_END + ";";
        }

        public void AddRow(ItemTemplate itemTemplate)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("entry", itemTemplate.WOWEntryID);
            newRow.AddInt("class", itemTemplate.ClassID);
            newRow.AddInt("subclass", itemTemplate.SubClassID);
            newRow.AddInt("SoundOverrideSubclass", -1);
            newRow.AddString("name", 255, itemTemplate.Name);
            if (itemTemplate.ItemDisplayInfo != null)
                newRow.AddInt("displayid", itemTemplate.ItemDisplayInfo.ItemDisplayInfoDBCID);
            else
                newRow.AddInt("displayid", 0);
            newRow.AddInt("Quality", Convert.ToInt32(itemTemplate.Quality));
            newRow.AddInt("Flags", GetFlags(itemTemplate));
            newRow.AddInt("FlagsExtra", 0);
            newRow.AddInt("BuyCount", itemTemplate.BuyCount);
            newRow.AddInt("BuyPrice", itemTemplate.BuyPriceInCopper);
            newRow.AddInt("SellPrice", itemTemplate.SellPriceInCopper);
            newRow.AddInt("InventoryType", Convert.ToInt32(itemTemplate.InventoryType));
            newRow.AddInt("AllowableClass", CalculateAllowableClasses(itemTemplate));
            newRow.AddInt("AllowableRace", -1);
            newRow.AddInt("ItemLevel", 500);
            newRow.AddInt("RequiredLevel", 1);
            newRow.AddInt("RequiredSkill", 0);
            newRow.AddInt("RequiredSkillRank", 0);
            newRow.AddInt("requiredspell", 0);
            newRow.AddInt("requiredhonorrank", 0);
            newRow.AddInt("RequiredCityRank", 0);
            newRow.AddInt("RequiredReputationFaction", 0);
            newRow.AddInt("RequiredReputationRank", 0);
            newRow.AddInt("maxcount", 0);
            newRow.AddInt("stackable", itemTemplate.StackSize);
            newRow.AddInt("ContainerSlots", itemTemplate.BagSlots);
            newRow.AddInt("StatsCount", itemTemplate.StatValues.Count);
            for (int i = 1; i <= 10; i++)
            {
                string curStatTypeFieldName = "stat_type" + i.ToString();
                int curStatTypeFieldValue = 0;
                string curStatValueFieldName = "stat_value" + i.ToString();
                int curStatValueFieldValue = 0;
                if (i <= itemTemplate.StatValues.Count)
                {
                    curStatTypeFieldValue = Convert.ToInt32(itemTemplate.StatValues[i-1].Item1);
                    curStatValueFieldValue = itemTemplate.StatValues[i-1].Item2;
                }
                newRow.AddInt(curStatTypeFieldName, curStatTypeFieldValue);
                newRow.AddInt(curStatValueFieldName, curStatValueFieldValue);
            }
            newRow.AddInt("ScalingStatDistribution", 0);
            newRow.AddInt("ScalingStatValue", 0);
            newRow.AddInt("dmg_min1", itemTemplate.WeaponMinDamage);
            newRow.AddInt("dmg_max1", itemTemplate.WeaponMaxDamage);
            newRow.AddInt("dmg_type1", 0);
            newRow.AddInt("dmg_min2", 0);
            newRow.AddInt("dmg_max2", 0);
            newRow.AddInt("dmg_type2", 0);
            newRow.AddInt("armor", itemTemplate.Armor);
            newRow.AddInt("holy_res", 0);
            newRow.AddInt("fire_res", itemTemplate.FireResist);
            newRow.AddInt("nature_res", itemTemplate.NatureResist);
            newRow.AddInt("frost_res", itemTemplate.FrostResist);
            newRow.AddInt("shadow_res", itemTemplate.ShadowResist);
            newRow.AddInt("arcane_res", itemTemplate.ArcaneResist);
            newRow.AddInt("delay", itemTemplate.WeaponDelay);
            if (itemTemplate.ClassID == 2 && itemTemplate.SubClassID == 2) // bow
            {
                newRow.AddInt("ammo_type", 2);
                newRow.AddFloat("RangedModRange", 100);
            }
            else
            {
                newRow.AddInt("ammo_type", 0);
                newRow.AddFloat("RangedModRange", 0);
            }
            if (itemTemplate.DoesTeachSpell == true && itemTemplate.WOWSpellID1 != 0)
            {
                newRow.AddInt("spellid_1", 483); // "Learning" wow spell ID
                newRow.AddInt("spelltrigger_1", 0);
                newRow.AddInt("spellcharges_1", -1);
                newRow.AddFloat("spellppmRate_1", -1);
                newRow.AddInt("spellcooldown_1", -1);
                newRow.AddInt("spellcategory_1", 0);
                newRow.AddInt("spellcategorycooldown_1", -1);
                newRow.AddInt("spellid_2", itemTemplate.WOWSpellID1);
                newRow.AddInt("spelltrigger_2", 6); // Learn Spell ID
                newRow.AddInt("spellcharges_2", 0);
                newRow.AddFloat("spellppmRate_2", 0);
                newRow.AddInt("spellcooldown_2", -1);
                newRow.AddInt("spellcategory_2", 0);
                newRow.AddInt("spellcategorycooldown_2", -1);
            }
            else
            {
                newRow.AddInt("spellid_1", itemTemplate.WOWSpellID1);
                newRow.AddInt("spelltrigger_1", 0);
                newRow.AddInt("spellcharges_1", 0);
                newRow.AddFloat("spellppmRate_1", 0);
                newRow.AddInt("spellcooldown_1", itemTemplate.SpellCooldown1);
                newRow.AddInt("spellcategory_1", itemTemplate.SpellCategory1);
                newRow.AddInt("spellcategorycooldown_1", itemTemplate.SpellCategoryCooldown1);
                newRow.AddInt("spellid_2", 0);
                newRow.AddInt("spelltrigger_2", 0);
                newRow.AddInt("spellcharges_2", 0);
                newRow.AddFloat("spellppmRate_2", 0);
                newRow.AddInt("spellcooldown_2", -1);
                newRow.AddInt("spellcategory_2", 0);
                newRow.AddInt("spellcategorycooldown_2", -1);
            }
            newRow.AddInt("spellid_3", 0);
            newRow.AddInt("spelltrigger_3", 0);
            newRow.AddInt("spellcharges_3", 0);
            newRow.AddFloat("spellppmRate_3", 0);
            newRow.AddInt("spellcooldown_3", -1);
            newRow.AddInt("spellcategory_3", 0);
            newRow.AddInt("spellcategorycooldown_3", -1);
            newRow.AddInt("spellid_4", 0);
            newRow.AddInt("spelltrigger_4", 0);
            newRow.AddInt("spellcharges_4", 0);
            newRow.AddFloat("spellppmRate_4", 0);
            newRow.AddInt("spellcooldown_4", -1);
            newRow.AddInt("spellcategory_4", 0);
            newRow.AddInt("spellcategorycooldown_4", -1);
            newRow.AddInt("spellid_5", 0);
            newRow.AddInt("spelltrigger_5", 0);
            newRow.AddInt("spellcharges_5", 0);
            newRow.AddFloat("spellppmRate_5", 0);
            newRow.AddInt("spellcooldown_5", -1);
            newRow.AddInt("spellcategory_5", 0);
            newRow.AddInt("spellcategorycooldown_5", -1);
            newRow.AddInt("bonding", itemTemplate.IsNoDrop == true ? 1 : 0);
            newRow.AddString("description", 255, itemTemplate.Description);
            newRow.AddInt("PageText", 0);
            newRow.AddInt("LanguageID", 0);
            newRow.AddInt("PageMaterial", 0);
            newRow.AddInt("startquest", 0);
            newRow.AddInt("lockid", 0);
            newRow.AddInt("Material", itemTemplate.WOWItemMaterialType);
            newRow.AddInt("sheath", itemTemplate.SheatheType);
            newRow.AddInt("RandomProperty", 0);
            newRow.AddInt("RandomSuffix", 0);
            newRow.AddInt("block", itemTemplate.Block);
            newRow.AddInt("itemset", 0);
            newRow.AddInt("MaxDurability", 0);
            newRow.AddInt("area", 0);
            newRow.AddInt("Map", 0);
            newRow.AddInt("BagFamily", 0);
            newRow.AddInt("TotemCategory", itemTemplate.TotemDBCID);
            newRow.AddInt("socketColor_1", 0);
            newRow.AddInt("socketContent_1", 0);
            newRow.AddInt("socketColor_2", 0);
            newRow.AddInt("socketContent_2", 0);
            newRow.AddInt("socketColor_3", 0);
            newRow.AddInt("socketContent_3", 0);
            newRow.AddInt("socketBonus", 0);
            newRow.AddInt("GemProperties", 0);
            newRow.AddInt("RequiredDisenchantSkill", -1);
            newRow.AddFloat("ArmorDamageModifier", 0);
            newRow.AddInt("duration", 0);
            newRow.AddInt("ItemLimitCategory", 0);
            newRow.AddInt("HolidayId", 0);
            newRow.AddString("ScriptName", 64, string.Empty);
            newRow.AddInt("DisenchantID", 0);
            newRow.AddInt("FoodType", itemTemplate.FoodType); // For pets: 1 - Meat, 2 - Fish, 3 - Cheese, 4 - Bread, 5 - Fungus, 6 - fruit, 7 - Raw Meat, 8 - Raw Fish
            newRow.AddInt("minMoneyLoot", 0);
            newRow.AddInt("maxMoneyLoot", 0);
            newRow.AddInt("flagsCustom", 0);
            newRow.AddInt("VerifiedBuild", 12340);
            Rows.Add(newRow);
        }

        private int CalculateAllowableClasses(ItemTemplate itemTemplate)
        {
            int allowableClass = 0;
            foreach (ClassType classType in itemTemplate.AllowedClassTypes)
            {
                if (classType == ClassType.All)
                    return -1;
                allowableClass += Convert.ToInt32(Math.Pow(2, Convert.ToInt32(classType) - 1));
            }
            if (allowableClass == 0)
                allowableClass = -1;
            return allowableClass;
        }

        private int GetFlags(ItemTemplate itemTemplate)
        {
            int flags = 0;
            if (itemTemplate.DoesVanishOnLogout == true)
                flags += 2;
            if (itemTemplate.CanBeOpened == true)
                flags += 4;
            if (itemTemplate.DoesTeachSpell == true && itemTemplate.WOWSpellID1 != 0)
                flags += 64; // ITEM_FLAG_PLAYERCAST
            return flags;
        }
    }
}
