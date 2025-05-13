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

namespace EQWOWConverter.Tradeskills
{
    internal class TradeskillRecipe
    {
        static private Dictionary<TradeskillType, List<TradeskillRecipe>> RecipesByTradeskillType = new Dictionary<TradeskillType, List<TradeskillRecipe>>();
        private static readonly object TradeskillLock = new object();

        public int SpellID;
        public int EQID;
        public string Name = string.Empty;
        public TradeskillType Type;
        public int SkillNeededEQ;
        public int TrivialEQ;
        public Dictionary<int, int> ProducedItemCountsByItemID = new Dictionary<int, int>();
        public Dictionary<int, int> ComponentItemCountsByItemID = new Dictionary<int, int>();
        public int SkillNeededWOW;
        public int TrivialLowWOW;
        public int TrivialHighWOW;
        public int LearnCostInCopper;

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

        public static void PopulateTradeskillRecipes(SortedDictionary<int, ItemTemplate> itemTemplatesByEQDBID)
        {
            // Load in the recipes first
            string tradeskillRecipesFilePath = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "TradeskillRecipes.csv");
            Logger.WriteDebug(string.Concat("Populating tradeskill recipes via file '", tradeskillRecipesFilePath, "'"));
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(tradeskillRecipesFilePath, "|");
            Dictionary<int, TradeskillRecipe> candidateRecipesByEQID = new Dictionary<int, TradeskillRecipe>();
            foreach (Dictionary<string, string> columns in rows)
            {
                // Skip if not generating the higher expansions
                int minExpansionID = int.Parse(columns["min_expansion"]);
                if (minExpansionID > Configuration.GENERATE_EQ_EXPANSION_ID_GENERAL)
                    continue;

                // Create the tradeskill recipe
                int spellID = int.Parse(columns["spellid_wow"]);
                int eqID = int.Parse(columns["id_eq"]);
                string name = columns["name"];
                int eqSkillNeeded = int.Parse(columns["skillneeded"]);
                int eqTrivial = int.Parse(columns["trivial"]);
                TradeskillType type = ConvertTradeskillType(int.Parse(columns["tradeskill"]));
                if (type == TradeskillType.None)
                {
                    Logger.WriteDebug(string.Concat("Skipping tradeskill item with name '", name, "' as the tradeskill type is Unknown"));
                    continue;
                }

                // Add it as a candidate
                TradeskillRecipe newRecipe = new TradeskillRecipe(spellID, eqID, name, type, eqSkillNeeded, eqTrivial);
                candidateRecipesByEQID.Add(eqID, newRecipe);
            }

            // Load in the recipe items
            string tradeskillRecipeItemsFilePath = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "TradeskillRecipeItems.csv");
            Logger.WriteDebug(string.Concat("Populating tradeskill recipe items via file '", tradeskillRecipeItemsFilePath, "'"));
            rows = FileTool.ReadAllRowsFromFileWithHeader(tradeskillRecipeItemsFilePath, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                int recipeEQID = int.Parse(columns["recipe_id"]);
                int itemID = int.Parse(columns["item_id"]);
                int successCount = int.Parse(columns["successcount"]);
                int componentCount = int.Parse(columns["componentcount"]);

                // Valid items only
                if (itemTemplatesByEQDBID.ContainsKey(itemID) == false)
                {
                    Logger.WriteDebug(string.Concat("Skipping recipe item with eqid ", itemID, " since that item ID is not known"));
                    continue;
                }
                if (candidateRecipesByEQID.ContainsKey(recipeEQID) == false)
                {
                    Logger.WriteDebug(string.Concat("Skipping recipe item with recipe id ", recipeEQID, " since that recipe ID is not known"));
                    continue;
                }

                // Attach to the related templates
                TradeskillRecipe curRecipe = candidateRecipesByEQID[recipeEQID];
                if (successCount > 0)
                {
                    if (curRecipe.ProducedItemCountsByItemID.ContainsKey(itemID) == false)
                        curRecipe.ProducedItemCountsByItemID.Add(itemID, successCount);
                    else
                        curRecipe.ProducedItemCountsByItemID[itemID] += successCount;
                }
                if (componentCount > 0)
                {
                    if (curRecipe.ComponentItemCountsByItemID.ContainsKey(itemID) == false)
                        curRecipe.ComponentItemCountsByItemID.Add(itemID, componentCount);
                    else
                        curRecipe.ComponentItemCountsByItemID[itemID] += componentCount;
                }

                // Generate WOW values
                PopulateWOWSkillLevels(curRecipe);
            }

            // Save them in the list
            foreach (var tradeskillRecipe in candidateRecipesByEQID)
            {
                if (tradeskillRecipe.Value.ProducedItemCountsByItemID.Count == 0)
                {
                    Logger.WriteDebug(string.Concat("Skipping recipe id ", tradeskillRecipe.Key, " since there are no produced items"));
                    continue;
                }
                if (tradeskillRecipe.Value.ComponentItemCountsByItemID.Count == 0)
                {
                    Logger.WriteDebug(string.Concat("Skipping recipe id ", tradeskillRecipe.Key, " since there are no component items"));
                    continue;
                }
                if (RecipesByTradeskillType.ContainsKey(tradeskillRecipe.Value.Type) == false)
                    RecipesByTradeskillType.Add(tradeskillRecipe.Value.Type, new List<TradeskillRecipe>());
                RecipesByTradeskillType[tradeskillRecipe.Value.Type].Add(tradeskillRecipe.Value);
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
                case 100: return TradeskillType.Enchanting; // Spell from enchanters
                case 75: return TradeskillType.None;
                default:
                    {
                        Logger.WriteError(string.Concat("Invalid tradeskill type of '", eqTradeskillTypeID, "'"));
                        return TradeskillType.None;
                    }
            }
        }

        private static void PopulateWOWSkillLevels(TradeskillRecipe tradeskillRecipe)
        {
            // Base skill levels entirely off the "trivial" value
            tradeskillRecipe.SkillNeededWOW = Convert.ToInt32(tradeskillRecipe.SkillNeededEQ * Configuration.TRADESKILLS_CONVERSION_MOD);
            tradeskillRecipe.TrivialLowWOW = tradeskillRecipe.SkillNeededWOW + Configuration.TRADESKILLS_SKILL_TIER_DISTANCE;
            tradeskillRecipe.TrivialHighWOW = tradeskillRecipe.TrivialHighWOW + Configuration.TRADESKILLS_SKILL_TIER_DISTANCE;
        }
    }
}
