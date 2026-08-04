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

using EQWOWConverter.Items;
using EQWOWConverter.Spells;
using EQWOWConverter.WOWFiles;
using System.Text;

namespace EQWOWConverter.Tradeskills
{
    internal class TradeskillRecipe
    {
        private static Dictionary<TradeskillType, List<TradeskillRecipe>> RecipesByTradeskillType = new Dictionary<TradeskillType, List<TradeskillRecipe>>();
        private static List<TradeskillRecipe> AllRecipes = new List<TradeskillRecipe>();
        private static readonly object TradeskillLock = new object();
        private static Dictionary<string, UInt32> TotemIDsByItemName = new Dictionary<string, UInt32>();

        public int EQID;
        public int SpellID;
        public string Name = string.Empty;
        public TradeskillType Type;
        public int EQTradeskillID;
        public int SkillNeededEQ;
        public int TrivialEQ;
        public int SkillLineWOW;
        public int SkillRankNeededWOW;
        public int TrivialLowWOW;
        public int TrivialHighWOW;
        public int LearnCostInCopper;
        public Dictionary<int, int> ProducedItemCountsByWOWItemID = new Dictionary<int, int>();
        public Dictionary<int, int> ComponentItemCountsByWOWItemID = new Dictionary<int, int>();
        public bool DoReplaceContainer;
        public List<int> RequiredIWOWtemIDs = new List<int>();
        public List<int> CombinerWOWItemIDs = new List<int>();
        public ItemTemplate? ProducedFilledContainer = null;
        public UInt32 RequiredTotemID1 = 0;
        public UInt32 RequiredTotemID2 = 0;
        public int RequiredFocus = 0;
        public int ProducedMultiContainerWOWID = -1;

        public TradeskillRecipe(int spellID, int eQID, string name, TradeskillType type, int skillNeededEQ, int trivialEQ)
        {
            SpellID = spellID;
            EQID = eQID;
            Name = name;
            Type = type;
            SkillNeededEQ = skillNeededEQ;
            TrivialEQ = trivialEQ;
        }

        public static void RemoveRecipe(TradeskillRecipe recipe)
        {
            lock (TradeskillLock)
            {
                for (int i = AllRecipes.Count-1; i >= 0; i--)
                    if (AllRecipes[i] == recipe)
                        AllRecipes.RemoveAt(i);
                foreach (List<TradeskillRecipe> tradeskillRecipes in RecipesByTradeskillType.Values)
                {
                    for (int i = tradeskillRecipes.Count - 1; i >= 0; i--)
                        if (tradeskillRecipes[i] == recipe)
                            tradeskillRecipes.RemoveAt(i);
                }
            }
        }

        public static Dictionary<TradeskillType, List<TradeskillRecipe>> GetRecipesByTradeskillType()
        {
            lock (TradeskillLock)
            {
                if (RecipesByTradeskillType.Count == 0)
                {
                    Logger.WriteError("Must call PopulateTradeskillRecipes before trying to GetRecipesByTradeskillType");
                    return new Dictionary<TradeskillType, List<TradeskillRecipe>>();
                }
                else
                    return RecipesByTradeskillType;
            }
        }

        public static List<TradeskillRecipe> GetAllRecipes()
        {
            lock (TradeskillLock)
            {
                if (RecipesByTradeskillType.Count == 0)
                {
                    Logger.WriteError("Must call PopulateTradeskillRecipes before trying to GetRecipesByTradeskillType");
                    return new List<TradeskillRecipe>();
                }
                else
                    return AllRecipes;
            }
        }

        public static Dictionary<string, UInt32> GetTotemIDsByItemName()
        {
            lock (TradeskillLock)
            {
                return TotemIDsByItemName;
            }
        }

        public static void PopulateTradeskillRecipes(SortedDictionary<int, ItemTemplate> itemTemplatesByEQDBID)
        {
            lock (TradeskillLock)
            {
                // Clear if already loaded
                if (RecipesByTradeskillType.Count > 0)
                {
                    Logger.WriteError("Calling PopulateTradeskillRecipes twice");
                    RecipesByTradeskillType.Clear();
                }

                // Load the recipes
                string tradeskillRecipesFilePath = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "TradeskillRecipes.csv");
                Logger.WriteDebug(string.Concat("Populating tradeskill recipes via file '", tradeskillRecipesFilePath, "'"));
                List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(tradeskillRecipesFilePath, "|");
                foreach (Dictionary<string, string> columns in rows)
                {
                    // Skip if not eligible to generate
                    if (columns["enabled"] == "0")
                        continue;
                    int minExpansionID = int.Parse(columns["min_expansion"]);
                    if (minExpansionID > Configuration.GENERATE_EQ_EXPANSION_ID_TRADESKILLS)
                        continue;

                    // Load the recipe
                    int spellID = int.Parse(columns["wow_spellID"]);
                    int eqID = int.Parse(columns["eq_recipeID"]);
                    string name = columns["name"];
                    int eqSkillNeeded = int.Parse(columns["eq_skill_needed"]);
                    int eqTrivial = int.Parse(columns["eq_trivial"]);
                    int eqTradeskillID = int.Parse(columns["eq_tradeskillID"]);
                    TradeskillType type = ConvertTradeskillType(eqTradeskillID);
                    if (type == TradeskillType.Unknown)
                    {
                        Logger.WriteDebug(string.Concat("Skipping tradeskill item with name '", name, "' as the tradeskill type is Unknown"));
                        continue;
                    }

                    // Assign the focus
                    int focusID = 0;
                    switch (type)
                    {
                        case TradeskillType.Blacksmithing: focusID = 3; break;
                        case TradeskillType.Cooking: focusID = 4; break;
                        default: break; // Do Nothing
                    }
                    TradeskillRecipe recipe = new TradeskillRecipe(spellID, eqID, name, type, eqSkillNeeded, eqTrivial);
                    recipe.EQTradeskillID = eqTradeskillID;
                    recipe.RequiredFocus = focusID;
                    recipe.DoReplaceContainer = columns["replace_container"] == "0" ? false : true;
                    bool itemLookupFailed = false;
                    for (int i = 0; i < 4; i++)
                    {
                        string producedEQItemIDString = columns[string.Concat("produced_eqid_", i)];
                        if (producedEQItemIDString.Trim().Length > 0)
                        {
                            int producedEQItemID = int.Parse(producedEQItemIDString);
                            if (itemTemplatesByEQDBID.ContainsKey(producedEQItemID) == false)
                            {
                                Logger.WriteError(string.Concat("Tried to add a tradeskill produced item with EQ Id of ", producedEQItemID, " but it did not exist"));
                                itemLookupFailed = true;
                                continue;
                            }
                            int producedWOWItemID = itemTemplatesByEQDBID[producedEQItemID].WOWEntryID;
                            itemTemplatesByEQDBID[producedEQItemID].NumOfTradeskillsThatCreateIt++;
                            int producedItemCount = int.Parse(columns[string.Concat("produced_count_", i)]);
                            if (recipe.ProducedItemCountsByWOWItemID.ContainsKey(producedWOWItemID) == true)
                                recipe.ProducedItemCountsByWOWItemID[producedWOWItemID] += producedItemCount;
                            else
                                recipe.ProducedItemCountsByWOWItemID.Add(producedWOWItemID, producedItemCount);
                        }
                    }
                    for (int i = 0; i < 8; i++)
                    {
                        string componentEQItemIDString = columns[string.Concat("component_eqid_", i)];
                        if (componentEQItemIDString.Trim().Length > 0)
                        {
                            int componentEQItemID = int.Parse(componentEQItemIDString);
                            if (itemTemplatesByEQDBID.ContainsKey(componentEQItemID) == false)
                            {
                                Logger.WriteError(string.Concat("Tried to add a tradeskill component item with EQ Id of ", componentEQItemID, " but it did not exist"));
                                itemLookupFailed = true;
                                continue;
                            }
                            int componentWOWItemID = itemTemplatesByEQDBID[componentEQItemID].WOWEntryID;
                            int componentItemCount = int.Parse(columns[string.Concat("component_count_", i)]);
                            if (recipe.ComponentItemCountsByWOWItemID.ContainsKey(componentWOWItemID) == true)
                                recipe.ComponentItemCountsByWOWItemID[componentWOWItemID] += componentItemCount;
                            else
                                recipe.ComponentItemCountsByWOWItemID.Add(componentWOWItemID, componentItemCount);
                        }
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        string requiredEQItemIDString = columns[string.Concat("required_eqid_", i)];
                        if (requiredEQItemIDString.Trim().Length > 0)
                        {
                            int requiredEQItemID = int.Parse(requiredEQItemIDString);
                            if (itemTemplatesByEQDBID.ContainsKey(requiredEQItemID) == false)
                            {
                                Logger.WriteError(string.Concat("Tried to add a tradeskill required item with EQ Id of ", requiredEQItemID, " but it did not exist"));
                                itemLookupFailed = true;
                                continue;
                            }
                            int requiredWOWItemID = itemTemplatesByEQDBID[requiredEQItemID].WOWEntryID;
                            if (recipe.RequiredIWOWtemIDs.Contains(requiredWOWItemID) == false)
                                recipe.RequiredIWOWtemIDs.Add(requiredWOWItemID);
                            string itemName = itemTemplatesByEQDBID[requiredEQItemID].Name;
                            if (TotemIDsByItemName.ContainsKey(itemName) == false)
                                TotemIDsByItemName.Add(itemName, Convert.ToUInt32(IDGenerationTool.GenerateID("TotemCategoryID", itemName)));
                            if (i == 0)
                                recipe.RequiredTotemID1 = TotemIDsByItemName[itemName];
                            else
                                recipe.RequiredTotemID2 = TotemIDsByItemName[itemName];
                            itemTemplatesByEQDBID[requiredEQItemID].TotemDBCID = Convert.ToInt32(TotemIDsByItemName[itemName]);
                        }
                    }
                    if (type == TradeskillType.None)
                    {
                        int containerItemEQID = int.Parse(columns["container_eqid_0"]);
                        if (containerItemEQID == -1)
                        {
                            foreach (int componentWOWItemID in recipe.ComponentItemCountsByWOWItemID.Keys)
                                recipe.CombinerWOWItemIDs.Add(componentWOWItemID);
                        }
                        else
                        {
                            if (itemTemplatesByEQDBID.ContainsKey(containerItemEQID) == false)
                            {
                                Logger.WriteError(string.Concat("Tried to add a 'none' combiner item with EQ Id of ", containerItemEQID, " but it did not exist"));
                                itemLookupFailed = true;
                                continue;
                            }
                            recipe.CombinerWOWItemIDs.Add(itemTemplatesByEQDBID[containerItemEQID].WOWEntryID);
                        }
                    }
                    if (itemLookupFailed == true)
                        continue;

                    // Don't create the recipe if there aren't any components
                    if (recipe.ComponentItemCountsByWOWItemID.Count() == 0)
                    {
                        Logger.WriteDebug(string.Concat("There were no components for recipe '", recipe.EQID, "', skipping"));
                        continue;
                    }

                    if (type == TradeskillType.Engineering && recipe.RequiredTotemID1 == 0)
                    {
                        if (recipe.EQTradeskillID == 64) // fletching
                            recipe.RequiredTotemID1 = Convert.ToUInt32(Configuration.TRADESKILL_TOTEM_CATEGORY_DBCID_ENGINEERING_FLETCHING);
                        else
                            recipe.RequiredTotemID1 = Convert.ToUInt32(Configuration.TRADESKILL_TOTEM_CATEGORY_DBCID_ENGINEERING_TOOLBOX);
                    }   
                    if (type == TradeskillType.Tailoring && recipe.RequiredTotemID1 == 0)
                        recipe.RequiredTotemID1 = Convert.ToUInt32(Configuration.TRADESKILL_TOTEM_CATEGORY_DBCID_TAILORING);
                    if (type == TradeskillType.Jewelcrafting && recipe.RequiredTotemID1 == 0)
                        recipe.RequiredTotemID1 = Convert.ToUInt32(Configuration.TRADESKILL_TOTEM_CATEGORY_DBCID_JEWELCRAFTING);
                    if (type == TradeskillType.Alchemy && recipe.RequiredTotemID1 == 0)
                        recipe.RequiredTotemID1 = Convert.ToUInt32(Configuration.TRADESKILL_TOTEM_CATEGORY_DBCID_ALCHEMY);
                    recipe.ProducedMultiContainerWOWID = int.Parse(columns["produced_multi_container_wowid"]);

                    // Generate WOW values
                    PopulateWOWSkillLevelsAndLine(recipe);
                    recipe.LearnCostInCopper = CalculateCostInCopper(recipe.SkillRankNeededWOW);

                    // Add it
                    if (RecipesByTradeskillType.ContainsKey(type) == false)
                        RecipesByTradeskillType.Add(type, new List<TradeskillRecipe>());
                    RecipesByTradeskillType[type].Add(recipe);
                    AllRecipes.Add(recipe);
                }
            }            
        }

        public static void ClampProducedItemSellPricesToComponentCosts(SortedDictionary<int, ItemTemplate> itemTemplatesByWOWEntryID)
        {
            lock (TradeskillLock)
            {
                foreach (TradeskillRecipe recipe in AllRecipes)
                {
                    // Calculate what the components would cost to buy from a vendor at max reputation discount
                    long componentCostInCopper = 0;
                    foreach (var component in recipe.ComponentItemCountsByWOWItemID)
                    {
                        if (itemTemplatesByWOWEntryID.ContainsKey(component.Key) == false)
                            continue;
                        ItemTemplate componentItemTemplate = itemTemplatesByWOWEntryID[component.Key];
                        int perUnitBuyPriceInCopper;
                        if (componentItemTemplate.OverrideStackBuyPriceInCopper > 0)
                            perUnitBuyPriceInCopper = componentItemTemplate.OverrideStackBuyPriceInCopper / Math.Max(componentItemTemplate.BuyCount, 1);
                        else
                            perUnitBuyPriceInCopper = componentItemTemplate.BuyPriceInCopper;
                        int discountedPerUnitBuyPriceInCopper = Convert.ToInt32(Math.Floor(perUnitBuyPriceInCopper * 0.8)); // Max rep gives 20% price discount
                        componentCostInCopper += (long)discountedPerUnitBuyPriceInCopper * component.Value;
                    }

                    // Calculate what the produced items would sell to a vendor for, considering the most valuable form of each
                    // (a clicky item's bag can be opened for the inner item, and slotshiftable items can shift into variant copies)
                    long producedSellValueInCopper = 0;
                    foreach (var produced in recipe.ProducedItemCountsByWOWItemID)
                    {
                        if (itemTemplatesByWOWEntryID.ContainsKey(produced.Key) == false)
                            continue;
                        producedSellValueInCopper += CalculateMaxSellValueInCopper(itemTemplatesByWOWEntryID[produced.Key], itemTemplatesByWOWEntryID) * produced.Value;
                    }
                    if (producedSellValueInCopper <= componentCostInCopper)
                        continue;

                    // Selling the products would profit over buying the components, so lower the produced sell prices to match the component cost
                    HashSet<int> loweredWOWItemIDs = new HashSet<int>();
                    foreach (var produced in recipe.ProducedItemCountsByWOWItemID)
                    {
                        if (itemTemplatesByWOWEntryID.ContainsKey(produced.Key) == false)
                            continue;
                        LowerSellPriceOfItemAndDerivedItems(itemTemplatesByWOWEntryID[produced.Key], itemTemplatesByWOWEntryID, recipe,
                            componentCostInCopper, producedSellValueInCopper, loweredWOWItemIDs);
                    }
                }
            }
        }

        private static long CalculateMaxSellValueInCopper(ItemTemplate itemTemplate, SortedDictionary<int, ItemTemplate> itemTemplatesByWOWEntryID)
        {
            long maxSellValueInCopper = itemTemplate.SellPriceInCopper;
            foreach (int slotshiftWOWItemID in itemTemplate.SlotshiftWOWIDsBySlot.Values)
                if (itemTemplatesByWOWEntryID.ContainsKey(slotshiftWOWItemID) == true)
                    maxSellValueInCopper = Math.Max(maxSellValueInCopper, itemTemplatesByWOWEntryID[slotshiftWOWItemID].SellPriceInCopper);
            if (itemTemplate.ContainedItems.Count > 0)
            {
                long containedSellValueInCopper = 0;
                foreach (var containedItem in itemTemplate.ContainedItems)
                    if (containedItem.itemTemplateIDWOW != itemTemplate.WOWEntryID && itemTemplatesByWOWEntryID.ContainsKey(containedItem.itemTemplateIDWOW) == true)
                        containedSellValueInCopper += CalculateMaxSellValueInCopper(itemTemplatesByWOWEntryID[containedItem.itemTemplateIDWOW], itemTemplatesByWOWEntryID) * containedItem.count;
                maxSellValueInCopper = Math.Max(maxSellValueInCopper, containedSellValueInCopper);
            }
            return maxSellValueInCopper;
        }

        private static void LowerSellPriceOfItemAndDerivedItems(ItemTemplate itemTemplate, SortedDictionary<int, ItemTemplate> itemTemplatesByWOWEntryID,
            TradeskillRecipe recipe, long componentCostInCopper, long producedSellValueInCopper, HashSet<int> loweredWOWItemIDs)
        {
            if (loweredWOWItemIDs.Contains(itemTemplate.WOWEntryID) == true)
                return;
            loweredWOWItemIDs.Add(itemTemplate.WOWEntryID);
            if (itemTemplate.SellPriceInCopper > 0)
            {
                int cappedSellPriceInCopper = Convert.ToInt32(((long)itemTemplate.SellPriceInCopper * componentCostInCopper) / producedSellValueInCopper);
                Logger.WriteDebug(string.Concat("Recipe '", recipe.Name, "' (eqid ", recipe.EQID, ") sells for more than its components cost, lowering sell price of produced item '",
                    itemTemplate.Name, "' from ", itemTemplate.SellPriceInCopper, " to ", cappedSellPriceInCopper, " copper"));
                itemTemplate.SellPriceInCopper = cappedSellPriceInCopper;
            }
            foreach (int slotshiftWOWItemID in itemTemplate.SlotshiftWOWIDsBySlot.Values)
                if (itemTemplatesByWOWEntryID.ContainsKey(slotshiftWOWItemID) == true)
                    LowerSellPriceOfItemAndDerivedItems(itemTemplatesByWOWEntryID[slotshiftWOWItemID], itemTemplatesByWOWEntryID, recipe,
                        componentCostInCopper, producedSellValueInCopper, loweredWOWItemIDs);
            foreach (var containedItem in itemTemplate.ContainedItems)
                if (itemTemplatesByWOWEntryID.ContainsKey(containedItem.itemTemplateIDWOW) == true)
                    LowerSellPriceOfItemAndDerivedItems(itemTemplatesByWOWEntryID[containedItem.itemTemplateIDWOW], itemTemplatesByWOWEntryID, recipe,
                        componentCostInCopper, producedSellValueInCopper, loweredWOWItemIDs);
        }

        private static TradeskillType ConvertTradeskillType(int eqTradeskillTypeID)
        {
            switch (eqTradeskillTypeID)
            {
                case 55: return TradeskillType.Cooking; // Fishing
                case 56: return TradeskillType.Alchemy; // Make Poison
                case 57: return TradeskillType.Engineering; // Tinkering
                case 58: return TradeskillType.Inscription; // Research
                case 59: return TradeskillType.Alchemy; // Alchemy
                case 60: return TradeskillType.Cooking; // Baking
                case 61: return TradeskillType.Tailoring; // Tailoring
                case 63: return TradeskillType.Blacksmithing; // Blacksmithing
                case 64: return TradeskillType.Engineering; // Fletching
                case 65: return TradeskillType.Cooking; // Brewing
                case 68: return TradeskillType.Jewelcrafting; // Jewerly Making
                case 69: return TradeskillType.Blacksmithing; // Pottery
                case 100: return TradeskillType.Enchanting; // Added enchanters
                case 75: return TradeskillType.None;
                default:
                    {
                        Logger.WriteError(string.Concat("Invalid tradeskill type of '", eqTradeskillTypeID, "'"));
                        return TradeskillType.Unknown;
                    }
            }
        }

        private static void PopulateWOWSkillLevelsAndLine(TradeskillRecipe tradeskillRecipe)
        {
            int maxSkillLevel = 450;
            float conversionMod = Configuration.TRADESKILLS_CONVERSION_MOD_80;
            if (Configuration.GENERATE_REBALANCE_CONTENT_TO_LEVEL_80 == false)
            {
                maxSkillLevel = 300;
                conversionMod = Configuration.TRADESKILLS_CONVERSION_MOD_60;
            }

            // Skill Line
            switch (tradeskillRecipe.Type)
            {
                case TradeskillType.Alchemy:        tradeskillRecipe.SkillLineWOW = 171; break;
                case TradeskillType.Blacksmithing:  tradeskillRecipe.SkillLineWOW = 164; break;
                case TradeskillType.Cooking:        tradeskillRecipe.SkillLineWOW = 185; break;
                case TradeskillType.Engineering:    tradeskillRecipe.SkillLineWOW = 202; break;
                case TradeskillType.Jewelcrafting:  tradeskillRecipe.SkillLineWOW = 755; break;
                case TradeskillType.Inscription:    tradeskillRecipe.SkillLineWOW = 773; break;
                case TradeskillType.Tailoring:      tradeskillRecipe.SkillLineWOW = 197; break;
                case TradeskillType.Enchanting:     tradeskillRecipe.SkillLineWOW = 333; break;
                default:
                    {
                        return; // Nothing for non-handled
                    }
            }

            // Skill Level
            if (tradeskillRecipe.SkillNeededEQ > 1)
            {
                tradeskillRecipe.SkillRankNeededWOW = Math.Min(Math.Max(Convert.ToInt32(tradeskillRecipe.SkillNeededEQ * conversionMod), 1), maxSkillLevel);
                tradeskillRecipe.TrivialLowWOW = Math.Min(tradeskillRecipe.SkillRankNeededWOW + Configuration.TRADESKILLS_SKILL_TIER_DISTANCE_LOW, maxSkillLevel);
                tradeskillRecipe.TrivialHighWOW = Math.Min(tradeskillRecipe.SkillRankNeededWOW + Configuration.TRADESKILLS_SKILL_TIER_DISTANCE_HIGH, maxSkillLevel);
            }
            else
            {
                tradeskillRecipe.TrivialHighWOW = Math.Min(Math.Max(Convert.ToInt32(tradeskillRecipe.TrivialEQ * conversionMod), 1), maxSkillLevel);
                tradeskillRecipe.TrivialLowWOW = Math.Min(Math.Max(tradeskillRecipe.TrivialHighWOW - Configuration.TRADESKILLS_SKILL_TIER_DISTANCE_LOW, 1), maxSkillLevel);
                tradeskillRecipe.SkillRankNeededWOW = Math.Min(Math.Max(tradeskillRecipe.TrivialLowWOW - Configuration.TRADESKILLS_SKILL_TIER_DISTANCE_HIGH, 1), maxSkillLevel);
            }
        }

        private static int GetLinearInterpolatedValue(int sourceValue, int sourceMin, int sourceMax, int targetMin, int targetMax)
        {
            return targetMin + (sourceValue - sourceMin) * (targetMax - targetMin) / (sourceMax - sourceMin);
        }

        private static int CalculateCostInCopper(int requiredSkillRank)
        {
            if (requiredSkillRank <= 50)
                return GetLinearInterpolatedValue(requiredSkillRank, 1, 50, Configuration.TRADESKILL_LEARN_COST_AT_1, Configuration.TRADESKILL_LEARN_COST_AT_50);
            else if (requiredSkillRank <= 100)
                return GetLinearInterpolatedValue(requiredSkillRank, 50, 100, Configuration.TRADESKILL_LEARN_COST_AT_50, Configuration.TRADESKILL_LEARN_COST_AT_100);
            else if (requiredSkillRank <= 200)
                return GetLinearInterpolatedValue(requiredSkillRank, 100, 200, Configuration.TRADESKILL_LEARN_COST_AT_100, Configuration.TRADESKILL_LEARN_COST_AT_200);
            else if (requiredSkillRank <= 300)
                return GetLinearInterpolatedValue(requiredSkillRank, 200, 300, Configuration.TRADESKILL_LEARN_COST_AT_200, Configuration.TRADESKILL_LEARN_COST_AT_300);
            else
                return GetLinearInterpolatedValue(requiredSkillRank, 300, 450, Configuration.TRADESKILL_LEARN_COST_AT_300, Configuration.TRADESKILL_LEARN_COST_AT_450);
        }

        public string GetGeneratedDescription(SortedDictionary<int, ItemTemplate> itemTemplatesByWOWEntryID)
        {
            StringBuilder spellDescriptionSB = new StringBuilder();
            spellDescriptionSB.Append("Combine");
            bool isFirstItem = true;
            foreach (var item in ComponentItemCountsByWOWItemID)
            {
                ItemTemplate componentItemTemplate = itemTemplatesByWOWEntryID[item.Key];
                if (isFirstItem == true)
                    isFirstItem = false;
                else
                    spellDescriptionSB.Append(",");
                spellDescriptionSB.Append(string.Concat(" ", item.Value, " x ", componentItemTemplate.Name));
            }
            spellDescriptionSB.Append(" to make");
            isFirstItem = true;
            foreach (var item in ProducedItemCountsByWOWItemID)
            {
                ItemTemplate producedItemTemplate = itemTemplatesByWOWEntryID[item.Key];
                if (isFirstItem == true)
                    isFirstItem = false;
                else
                    spellDescriptionSB.Append(" and");
                spellDescriptionSB.Append(string.Concat(" ", item.Value, " ", producedItemTemplate.Name));
            }
            return spellDescriptionSB.ToString();
        }

        public void SetSpellVisualData(SpellTemplate spellTemplate)
        {
            // Animation / visual effect
            switch (Type)
            {
                case TradeskillType.Alchemy: spellTemplate.SpellVisualID1 = 92; break; // Same as "Potion of Wild Magic"
                case TradeskillType.Blacksmithing: spellTemplate.SpellVisualID1 = 395; break; // Same as "Imperial Plate Helm"
                case TradeskillType.Cooking: spellTemplate.SpellVisualID1 = 3881; break; // Same as "Blood Sausage"
                case TradeskillType.Engineering: spellTemplate.SpellVisualID1 = 2641; break; // Same as "Flying Tiger Goggles"
                case TradeskillType.Jewelcrafting: spellTemplate.SpellVisualID1 = 7374; break; // Same as "Heavy Jade Ring"
                case TradeskillType.Inscription: spellTemplate.SpellVisualID1 = 10130; break; // Same as "Ink of the Sea"
                case TradeskillType.Tailoring: spellTemplate.SpellVisualID1 = 1168; break; // Same as "Bolt of Linen Cloth"
                case TradeskillType.Enchanting: spellTemplate.SpellVisualID1 = 3182; break; // Same as "Enchant 2H Weapon - Minor Impact"
                default: spellTemplate.SpellVisualID1 = 1168; break; // Same as "Join map fragments" for the Tanaris treasure map
            }
        }
    }
}
