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
        private static Dictionary<ClassType, int> ClassTrainerSpellsReferenceLineIDsByClassType = new Dictionary<ClassType, int>();
        private static Dictionary<ClassType, List<SpellTrainerAbility>> ClassTrainerAbilitiesByClassType = new Dictionary<ClassType, List<SpellTrainerAbility>>();
        private static Dictionary<TradeskillType, int> TradeskillSpellsReferenceLineIDsByTradeskillType = new Dictionary<TradeskillType, int>();
        private static Dictionary<TradeskillType, List<SpellTrainerAbility>> TradeskillTrainerAbilitiesByTradeskillType = new Dictionary<TradeskillType, List<SpellTrainerAbility>>();
        private static readonly object SpellTrainerReadLock = new object();
        private static readonly object SpellTrainerWriteLock = new object();
        private static int CURRENT_NPCTRAINER_ID = Configuration.SQL_NPCTRAINER_ID_START;

        public ClassType ClassType = ClassType.None;
        public TradeskillType TradeskillType = TradeskillType.None;
        public TradeskillRecipe? TradeskillRecipe = null;
        public int SpellID = 0;
        public int MoneyCost = 0;
        public int ReqSkillLine = 0;
        public int ReqSkillRank = 0;
        public int ReqLevel = 0;
        public int ReqSpellID = 0;

        public static List<SpellTrainerAbility> GetTrainerSpellsForClass(ClassType classType)
        {
            lock (SpellTrainerReadLock)
            {
                if (ClassTrainerAbilitiesByClassType.Count == 0)
                    PopulateClassTrainerAbilities();
                return ClassTrainerAbilitiesByClassType[classType];
            }
        }

        public static List<SpellTrainerAbility> GetTrainerSpellsForTradeskill(TradeskillType tradeskillType)
        {
            lock (SpellTrainerReadLock)
            {
                if (TradeskillSpellsReferenceLineIDsByTradeskillType.Count == 0)
                {
                    Logger.WriteError("GetTrainerSpellsForTradeskill called before PopulateTradeskillAbilities");
                    return new List<SpellTrainerAbility>();
                }
                return TradeskillTrainerAbilitiesByTradeskillType[tradeskillType];
            }
        }

        public static int GetTrainerSpellsReferenceLineIDForWOWClassTrainer(ClassType trainerClassType)
        {
            lock (SpellTrainerReadLock)
            {
                if (ClassTrainerSpellsReferenceLineIDsByClassType.Count == 0)
                    PopulateClassTrainerAbilities();
                return ClassTrainerSpellsReferenceLineIDsByClassType[trainerClassType];
            }
        }

        public static int GetTrainerSpellsReferenceLineIDForWOWTradeskillTrainer(TradeskillType trainerTradeskillType)
        {
            lock (SpellTrainerReadLock)
            {
                if (TradeskillSpellsReferenceLineIDsByTradeskillType.Count == 0)
                {
                    Logger.WriteError("GetTrainerSpellsReferenceLineIDForWOWTradeskillTrainer called before PopulateTradeskillAbilities");
                    return 0;
                }
                return TradeskillSpellsReferenceLineIDsByTradeskillType[trainerTradeskillType];
            }
        }

        public static void PopulateTradeskillAbilities(Dictionary<TradeskillType, List<TradeskillRecipe>> recipesByTradeskillType)
        {
            lock (SpellTrainerWriteLock)
            {
                if (TradeskillSpellsReferenceLineIDsByTradeskillType.Count != 0)
                {
                    Logger.WriteError("Attempted to PopulateTradeskillAbilities twice");
                    return;
                }

                // Generate the IDs on a per-tradeskill basis                
                foreach (TradeskillType tradeskillType in Enum.GetValues(typeof(TradeskillType)))
                {
                    if (tradeskillType == TradeskillType.Unknown || tradeskillType == TradeskillType.None)
                        continue;
                    TradeskillSpellsReferenceLineIDsByTradeskillType.Add(tradeskillType, CURRENT_NPCTRAINER_ID);
                    CURRENT_NPCTRAINER_ID++;
                }

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
                // Generate the IDs on a per-class basis                
                foreach (ClassType classType in Enum.GetValues(typeof(ClassType)))
                {
                    if (classType == ClassType.All || classType == ClassType.None)
                        continue;
                    ClassTrainerSpellsReferenceLineIDsByClassType.Add(classType, CURRENT_NPCTRAINER_ID);
                    CURRENT_NPCTRAINER_ID++;
                }

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

                // Assign the bind and gate spells
                List<int> casterClassIDs = new List<int> { 5, 7, 8, 9, 11 };
                List<int> meleeClassIDs = new List<int> { 1, 2, 3, 4, 6 };
                if (Configuration.SPELLS_GATE_CASTER_LEARN_LEVEL > 0)
                {
                    foreach (int classTypeID in casterClassIDs)
                    {
                        SpellTrainerAbility curSpell = new SpellTrainerAbility();
                        curSpell.ClassType = (ClassType)classTypeID;
                        curSpell.SpellID = Configuration.SPELLS_GATE_SPELLDBC_ID;
                        curSpell.MoneyCost = Configuration.SPELLS_GATE_SPELL_LEARN_COST;
                        curSpell.ReqLevel = Configuration.SPELLS_GATE_CASTER_LEARN_LEVEL;
                        ClassTrainerAbilitiesByClassType[curSpell.ClassType].Add(curSpell);
                    }
                }
                if (Configuration.SPELLS_GATE_MELEE_LEARN_LEVEL > 0)
                {
                    foreach (int classTypeID in meleeClassIDs)
                    {
                        SpellTrainerAbility curSpell = new SpellTrainerAbility();
                        curSpell.ClassType = (ClassType)classTypeID;
                        curSpell.SpellID = Configuration.SPELLS_GATE_SPELLDBC_ID;
                        curSpell.MoneyCost = Configuration.SPELLS_GATE_SPELL_LEARN_COST;
                        curSpell.ReqLevel = Configuration.SPELLS_GATE_MELEE_LEARN_LEVEL;
                        ClassTrainerAbilitiesByClassType[curSpell.ClassType].Add(curSpell);
                    }
                }
                int bindSpellID = Configuration.SPELLS_BINDANY_SPELLDBC_ID;
                if (Configuration.SPELLS_BIND_CASTER_LEARN_LEVEL > 0 && Configuration.SPELLS_BIND_MELEE_LEARN_LEVEL > 0)
                    bindSpellID = Configuration.SPELLS_BINDSELF_SPELLDBC_ID;
                if (Configuration.SPELLS_BIND_CASTER_LEARN_LEVEL > 0)
                {
                    foreach (int classTypeID in casterClassIDs)
                    {
                        SpellTrainerAbility curSpell = new SpellTrainerAbility();
                        curSpell.ClassType = (ClassType)classTypeID;
                        curSpell.SpellID = bindSpellID;
                        curSpell.MoneyCost = Configuration.SPELLS_BIND_SPELL_LEARN_COST;
                        curSpell.ReqLevel = Configuration.SPELLS_BIND_CASTER_LEARN_LEVEL;
                        ClassTrainerAbilitiesByClassType[curSpell.ClassType].Add(curSpell);
                    }
                }
                if (Configuration.SPELLS_BIND_MELEE_LEARN_LEVEL > 0)
                {
                    foreach (int classTypeID in meleeClassIDs)
                    {
                        SpellTrainerAbility curSpell = new SpellTrainerAbility();
                        curSpell.ClassType = (ClassType)classTypeID;
                        curSpell.SpellID = bindSpellID;
                        curSpell.MoneyCost = Configuration.SPELLS_BIND_SPELL_LEARN_COST;
                        curSpell.ReqLevel = Configuration.SPELLS_BIND_MELEE_LEARN_LEVEL;
                        ClassTrainerAbilitiesByClassType[curSpell.ClassType].Add(curSpell);
                    }
                }
            }            
        }
    }
}
