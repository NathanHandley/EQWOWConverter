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
using EQWOWConverter.Items;

namespace EQWOWConverter.WOWFiles
{
    internal class ItemTemplateSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM `item_template` WHERE `entry` >= " + Configuration.CONFIG_SQL_ITEM_TEMPLATE_ENTRY_START + " AND `entry` <= " + Configuration.CONFIG_SQL_ITEM_TEMPLATE_ENTRY_END + ";";
        }

        public void AddRow(ItemTemplate itemTemplate)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("entry", itemTemplate.WOWEntryID);
            newRow.AddInt("class", itemTemplate.ClassID);
            newRow.AddInt("subclass", itemTemplate.SubClassID);
            newRow.AddInt("SoundOverrideSubclass", -1);
            newRow.AddString("name", 255, itemTemplate.Name);
            newRow.AddInt("displayid", itemTemplate.DisplayID);
            newRow.AddInt("Quality", 1);
            newRow.AddInt("Flags", 0);
            newRow.AddInt("FlagsExtra", 0);
            newRow.AddInt("BuyCount", 1);
            newRow.AddInt("BuyPrice", itemTemplate.BuyPriceInCopper);
            newRow.AddInt("SellPrice", itemTemplate.SellPriceInCopper);
            newRow.AddInt("InventoryType", Convert.ToInt32(itemTemplate.InventoryType));
            newRow.AddInt("AllowableClass", -1);
            newRow.AddInt("AllowableRace", -1);
            newRow.AddInt("ItemLevel", 0);
            newRow.AddInt("RequiredLevel", 0);
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
            newRow.AddInt("StatsCount", 0);
            newRow.AddInt("stat_type1", 0);
            newRow.AddInt("stat_value1", 0);
            newRow.AddInt("stat_type2", 0);
            newRow.AddInt("stat_value2", 0);
            newRow.AddInt("stat_type3", 0);
            newRow.AddInt("stat_value3", 0);
            newRow.AddInt("stat_type4", 0);
            newRow.AddInt("stat_value4", 0);
            newRow.AddInt("stat_type5", 0);
            newRow.AddInt("stat_value5", 0);
            newRow.AddInt("stat_type6", 0);
            newRow.AddInt("stat_value6", 0);
            newRow.AddInt("stat_type7", 0);
            newRow.AddInt("stat_value7", 0);
            newRow.AddInt("stat_type8", 0);
            newRow.AddInt("stat_value8", 0);
            newRow.AddInt("stat_type9", 0);
            newRow.AddInt("stat_value9", 0);
            newRow.AddInt("stat_type10", 0);
            newRow.AddInt("stat_value10", 0);
            newRow.AddInt("ScalingStatDistribution", 0);
            newRow.AddInt("ScalingStatValue", 0);
            newRow.AddInt("dmg_min1", 0);
            newRow.AddInt("dmg_max1", 0);
            newRow.AddInt("dmg_type1", 0);
            newRow.AddInt("dmg_min2", 0);
            newRow.AddInt("dmg_max2", 0);
            newRow.AddInt("dmg_type2", 0);
            newRow.AddInt("armor", 0);
            newRow.AddInt("holy_res", null);
            newRow.AddInt("fire_res", null);
            newRow.AddInt("nature_res", null);
            newRow.AddInt("frost_res", null);
            newRow.AddInt("shadow_res", null);
            newRow.AddInt("arcane_res", null);
            newRow.AddInt("delay", 1000);
            newRow.AddInt("ammo_type", 0);
            newRow.AddFloat("RangedModRange", 0);
            newRow.AddInt("spellid_1", 0);
            newRow.AddInt("spelltrigger_1", 0);
            newRow.AddInt("spellcharges_1", 0);
            newRow.AddFloat("spellppmRate_1", 0);
            newRow.AddInt("spellcooldown_1", -1);
            newRow.AddInt("spellcategory_1", 0);
            newRow.AddInt("spellcategorycooldown_1", -1);
            newRow.AddInt("spellid_2", 0);
            newRow.AddInt("spelltrigger_2", 0);
            newRow.AddInt("spellcharges_2", 0);
            newRow.AddFloat("spellppmRate_2", 0);
            newRow.AddInt("spellcooldown_2", -1);
            newRow.AddInt("spellcategory_2", 0);
            newRow.AddInt("spellcategorycooldown_2", -1);
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
            newRow.AddInt("bonding", 0);
            newRow.AddString("description", 255, string.Empty);
            newRow.AddInt("PageText", 0);
            newRow.AddInt("LanguageID", 0);
            newRow.AddInt("PageMaterial", 0);
            newRow.AddInt("startquest", 0);
            newRow.AddInt("lockid", 0);
            newRow.AddInt("Material", itemTemplate.Material);
            newRow.AddInt("sheath", itemTemplate.SheatheType);
            newRow.AddInt("RandomProperty", 0);
            newRow.AddInt("RandomSuffix", 0);
            newRow.AddInt("block", 0);
            newRow.AddInt("itemset", 0);
            newRow.AddInt("MaxDurability", 0);
            newRow.AddInt("area", 0);
            newRow.AddInt("Map", 0);
            newRow.AddInt("BagFamily", 0);
            newRow.AddInt("TotemCategory", 0);
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
            newRow.AddInt("FoodType", 0);
            newRow.AddInt("minMoneyLoot", 0);
            newRow.AddInt("maxMoneyLoot", 0);
            newRow.AddInt("flagsCustom", 0);
            newRow.AddInt("VerifiedBuild", 12340);
            Rows.Add(newRow);
        }
    }
}
