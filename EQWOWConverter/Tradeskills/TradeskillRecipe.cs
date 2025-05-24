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
using System.Text;

namespace EQWOWConverter.Tradeskills
{
    internal class TradeskillRecipe
    {
        static private Dictionary<TradeskillType, List<TradeskillRecipe>> RecipesByTradeskillType = new Dictionary<TradeskillType, List<TradeskillRecipe>>();
        static private List<TradeskillRecipe> AllRecipes = new List<TradeskillRecipe>();
        private static readonly object TradeskillReadLock = new object();
        private static readonly object TradeskillWriteLock = new object();

        public int EQID;
        public int SpellID;
        public string Name = string.Empty;
        public TradeskillType Type;
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

        public TradeskillRecipe(int spellID, int eQID, string name, TradeskillType type, int skillNeededEQ, int trivialEQ)
        {
            SpellID = spellID;
            EQID = eQID;
            Name = name;
            Type = type;
            SkillNeededEQ = skillNeededEQ;
            TrivialEQ = trivialEQ;
        }

        public static Dictionary<TradeskillType, List<TradeskillRecipe>> GetRecipesByTradeskillType()
        {
            lock (TradeskillReadLock)
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
            lock (TradeskillReadLock)
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

        public static void PopulateTradeskillRecipes(SortedDictionary<int, ItemTemplate> itemTemplatesByEQDBID)
        {
            lock (TradeskillWriteLock)
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
                    TradeskillType type = ConvertTradeskillType(int.Parse(columns["eq_tradeskillID"]));
                    if (type == TradeskillType.Unknown)
                    {
                        Logger.WriteDebug(string.Concat("Skipping tradeskill item with name '", name, "' as the tradeskill type is Unknown"));
                        continue;
                    }
                    TradeskillRecipe recipe = new TradeskillRecipe(spellID, eqID, name, type, eqSkillNeeded, eqTrivial);
                    recipe.DoReplaceContainer = columns["replace_container"] == "0" ? false : true;
                    recipe.LearnCostInCopper = 1;
                    for (int i = 0; i < 4; i++)
                    {
                        string producedEQItemIDString = columns[string.Concat("produced_eqid_", i)];
                        if (producedEQItemIDString.Trim().Length > 0)
                        {
                            int producedEQItemID = int.Parse(producedEQItemIDString);
                            if (itemTemplatesByEQDBID.ContainsKey(producedEQItemID) == false)
                            {
                                Logger.WriteError(string.Concat("Tried to add a tradeskill produced item with EQ Id of ", producedEQItemID, " but it did not exist"));
                                continue;
                            }
                            int producedWOWItemID = itemTemplatesByEQDBID[producedEQItemID].WOWEntryID;
                            itemTemplatesByEQDBID[producedEQItemID].IsMadeByTradeskill = true;
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
                                continue;
                            }
                            int requiredWOWItemID = itemTemplatesByEQDBID[requiredEQItemID].WOWEntryID;
                            if (recipe.RequiredIWOWtemIDs.Contains(requiredWOWItemID) == false)
                                recipe.RequiredIWOWtemIDs.Add(requiredWOWItemID);
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
                                continue;
                            }
                            recipe.CombinerWOWItemIDs.Add(itemTemplatesByEQDBID[containerItemEQID].WOWEntryID);
                        }
                    }

                    // Generate WOW values
                    PopulateWOWSkillLevelsAndLine(recipe);

                    // Add it
                    if (RecipesByTradeskillType.ContainsKey(type) == false)
                        RecipesByTradeskillType.Add(type, new List<TradeskillRecipe>());
                    RecipesByTradeskillType[type].Add(recipe);
                    AllRecipes.Add(recipe);
                }
            }            
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
            switch(tradeskillRecipe.Type)
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

            // Base skill levels entirely off the "trivial" value
            tradeskillRecipe.SkillRankNeededWOW = Math.Max(Convert.ToInt32(tradeskillRecipe.SkillNeededEQ * Configuration.TRADESKILLS_CONVERSION_MOD), 1);
            tradeskillRecipe.TrivialLowWOW = tradeskillRecipe.SkillRankNeededWOW + Configuration.TRADESKILLS_SKILL_TIER_DISTANCE;
            tradeskillRecipe.TrivialHighWOW = tradeskillRecipe.TrivialHighWOW + Configuration.TRADESKILLS_SKILL_TIER_DISTANCE;
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
                spellDescriptionSB.Append(string.Concat(" ", item.Value, " ", componentItemTemplate.Name));
                if (item.Value > 1)
                    spellDescriptionSB.Append("s");
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
                case TradeskillType.Engineering: spellTemplate.SpellVisualID1 = 3182; break; // Same as "Enchant 2H Weapon - Minor Impact"
                case TradeskillType.Jewelcrafting: spellTemplate.SpellVisualID1 = 7374; break; // Same as "Heavy Jade Ring"
                case TradeskillType.Inscription: spellTemplate.SpellVisualID1 = 10130; break; // Same as "Ink of the Sea"
                case TradeskillType.Tailoring: spellTemplate.SpellVisualID1 = 1168; break; // Same as "Bolt of Linen Cloth"
                case TradeskillType.Enchanting: spellTemplate.SpellVisualID1 = 2641; break; // Same as "Flying Tiger Goggles"
                default: spellTemplate.SpellVisualID1 = 1168; break; // Same as "Join map fragments" for the Tanaris treasure map
            }
        }
    }
}
