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
        static private Dictionary<TradeskillType, TradeskillRecipe> RecipesByTradeskillType = new Dictionary<TradeskillType, TradeskillRecipe>();
        private static readonly object TradeskillLock = new object();

        public int SpellID;
        public int EQID;
        public string Name = string.Empty;
        public TradeskillType Type;
        public int SkillNeededEQ;
        public int TrivialEQ;

        public TradeskillRecipe(int spellID, int eQID, string name, TradeskillType type, int skillNeededEQ, int trivialEQ)
        {
            SpellID = spellID;
            EQID = eQID;
            Name = name;
            Type = type;
            SkillNeededEQ = skillNeededEQ;
            TrivialEQ = trivialEQ;
        }

        public static Dictionary<TradeskillType, TradeskillRecipe> GetRecipesByTradeskillType()
        {
            lock (TradeskillLock)
            {
                if (RecipesByTradeskillType.Count == 0)
                {
                    Logger.WriteError("Must call PopulateTradeskillRecipes before trying to GetRecipesByTradeskillType");
                    return new Dictionary<TradeskillType, TradeskillRecipe>();
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
            foreach (Dictionary<string, string> columns in rows)
            {
                // TODO:
            }

            // Load in the recipe items
            string tradeskillRecipeItemsFilePath = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "TradeskillRecipeItems.csv");
            Logger.WriteDebug(string.Concat("Populating tradeskill recipe items via file '", tradeskillRecipeItemsFilePath, "'"));
            rows = FileTool.ReadAllRowsFromFileWithHeader(tradeskillRecipeItemsFilePath, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                // TODO:
            }

            // Save them in the list
            // TODO:
        }
    }
}
