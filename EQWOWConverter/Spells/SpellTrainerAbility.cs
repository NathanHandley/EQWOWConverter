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
using EQWOWConverter.Tradeskills;

namespace EQWOWConverter.Spells
{
    internal class SpellTrainerAbility
    {
        private static Dictionary<ClassType, List<SpellTrainerAbility>> ClassTrainerAbilitiesByClassType = new Dictionary<ClassType, List<SpellTrainerAbility>>();
        private static Dictionary<TradeskillType, List<SpellTrainerAbility>> TradeskillTrainerAbilitiesByTradeskillType = new Dictionary<TradeskillType, List<SpellTrainerAbility>>();
        private static readonly object SpellTrainerReadLock = new object();
        private static readonly object SpellTrainerWriteLock = new object();

        public ClassType ClassType = ClassType.None;
        public TradeskillType TradeskillType = TradeskillType.None;
        public TradeskillRecipe? TradeskillRecipe = null;
        public int SpellID = 0;
        public int MoneyCost = 0;
        public int ReqSkillLine = 0;
        public int ReqSkillRank = 0;
        public int ReqLevel = 0;
        public int ReqSpellID = 0;

        public static void RemoveTrainerAbilityUsingTradeskillRecipe(TradeskillRecipe recipe)
        {
            lock (SpellTrainerReadLock)
            {
                int numOfRecipes = TradeskillTrainerAbilitiesByTradeskillType[recipe.Type].Count;
                for (int i = numOfRecipes-1; i >= 0; i--)
                {
                    if (TradeskillTrainerAbilitiesByTradeskillType[recipe.Type][i].TradeskillRecipe == recipe)
                    {
                        TradeskillTrainerAbilitiesByTradeskillType[recipe.Type].RemoveAt(i);
                        return;
                    }
                }
            }
        }

        public static List<SpellTrainerAbility> GetTrainerSpellsForTradeskill(TradeskillType tradeskillType)
        {
            lock (SpellTrainerReadLock)
            {
                if (ClassTrainerAbilitiesByClassType.Count == 0)
                    PopulateClassTrainerAbilities();
                return TradeskillTrainerAbilitiesByTradeskillType[tradeskillType];
            }
        }

        public static List<SpellTrainerAbility> GetTrainerSpellsForClass(ClassType classType)
        {
            lock (SpellTrainerReadLock)
            {
                if (ClassTrainerAbilitiesByClassType.Count == 0)
                    PopulateClassTrainerAbilities();
                return ClassTrainerAbilitiesByClassType[classType];
            }
        }

        public static void PopulateTradeskillAbilities(Dictionary<TradeskillType, List<TradeskillRecipe>> recipesByTradeskillType)
        {
            lock (SpellTrainerWriteLock)
            {
                // Add spells for each recipe
                foreach (TradeskillType tradeskillType in recipesByTradeskillType.Keys)
                {
                    if (tradeskillType == TradeskillType.Unknown || tradeskillType == TradeskillType.None)
                        continue;
                    foreach (TradeskillRecipe recipe in recipesByTradeskillType[tradeskillType])
                    {
                        SpellTrainerAbility curTrainerRecipeAbility = new SpellTrainerAbility();
                        curTrainerRecipeAbility.TradeskillType = tradeskillType;
                        curTrainerRecipeAbility.SpellID = recipe.SpellID;
                        curTrainerRecipeAbility.MoneyCost = recipe.LearnCostInCopper;
                        curTrainerRecipeAbility.ReqSkillLine = recipe.SkillLineWOW;
                        curTrainerRecipeAbility.ReqSkillRank = recipe.SkillRankNeededWOW;
                        curTrainerRecipeAbility.SpellID = recipe.SpellID;
                        curTrainerRecipeAbility.TradeskillRecipe = recipe;
                        if (TradeskillTrainerAbilitiesByTradeskillType.ContainsKey(recipe.Type) == false)
                            TradeskillTrainerAbilitiesByTradeskillType.Add(recipe.Type, new List<SpellTrainerAbility>());
                        TradeskillTrainerAbilitiesByTradeskillType[recipe.Type].Add(curTrainerRecipeAbility);
                    }
                }
            }
        }

        private static void PopulateClassTrainerAbilities()
        {
            lock (SpellTrainerWriteLock)
            {
                // Read in the trainer spell list
                string classTrainerSpellsFileName = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "SpellClassTrainerSpells.csv");
                Logger.WriteDebug("Populating class trainer spells via file '" + classTrainerSpellsFileName + "'");
                List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(classTrainerSpellsFileName, "|");
                foreach (Dictionary<string, string> columns in rows)
                {
                    SpellTrainerAbility curTrainerClassAbility = new SpellTrainerAbility();
                    curTrainerClassAbility.ClassType = (ClassType)int.Parse(columns["ClassID"]);
                    curTrainerClassAbility.SpellID = int.Parse(columns["SpellID"]);
                    curTrainerClassAbility.MoneyCost = int.Parse(columns["MoneyCost"]);
                    curTrainerClassAbility.ReqSkillLine = int.Parse(columns["ReqSkillLine"]);
                    curTrainerClassAbility.ReqSkillRank = int.Parse(columns["ReqSkillRank"]);
                    curTrainerClassAbility.ReqLevel = int.Parse(columns["ReqLevel"]);
                    curTrainerClassAbility.ReqSpellID = int.Parse(columns["ReqSpellID"]);
                    if (ClassTrainerAbilitiesByClassType.ContainsKey(curTrainerClassAbility.ClassType) == false)
                        ClassTrainerAbilitiesByClassType.Add(curTrainerClassAbility.ClassType, new List<SpellTrainerAbility>());
                    ClassTrainerAbilitiesByClassType[curTrainerClassAbility.ClassType].Add(curTrainerClassAbility);
                }
            }            
        }
    }
}
