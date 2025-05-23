﻿//  Author: Nathan Handley (nathanhandley@protonmail.com)
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

namespace EQWOWConverter.Spells
{
    internal class SpellClassTrainerAbility
    {
        private static Dictionary<ClassType, int> TrainerSpellIDsByClassType = new Dictionary<ClassType, int>();
        private static Dictionary<ClassType, List<SpellClassTrainerAbility>> TrainerAbilitiesByClassType = new Dictionary<ClassType, List<SpellClassTrainerAbility>>();

        public ClassType Class = ClassType.None;
        public int SpellID = 0;
        public int MoneyCost = 0;
        public int ReqSkillLine = 0;
        public int ReqSkillRank = 0;
        public int ReqLevel = 0;
        public int ReqSpellID = 0;

        public static List<SpellClassTrainerAbility> GetTrainerSpellsForClass(ClassType classType)
        {
            if (TrainerAbilitiesByClassType.Count == 0)
                PopulateClassTrainerAbilities();
            return TrainerAbilitiesByClassType[classType];
        }

        public static int GetTrainerSpellsIDForWOWClassTrainer(ClassType trainerClassType)
        {
            if (TrainerSpellIDsByClassType.Count == 0)
                PopulateClassTrainerAbilities();
            return TrainerSpellIDsByClassType[trainerClassType];
        }

        private static void PopulateClassTrainerAbilities()
        {
            // Generate the IDs on a per-class basis
            int curID = Configuration.SQL_NPCTRAINER_ID_START;
            foreach (ClassType classType in Enum.GetValues(typeof(ClassType)))
            {
                if (classType == ClassType.All || classType == ClassType.None)
                    continue;
                TrainerSpellIDsByClassType.Add(classType, curID);
                curID++;
            }

            // Read in the spell list
            string classTrainerSpellsFileName = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "SpellClassTrainerSpells.csv");
            Logger.WriteDebug("Populating class trainer spells via file '" + classTrainerSpellsFileName + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(classTrainerSpellsFileName, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                SpellClassTrainerAbility curSpell = new SpellClassTrainerAbility();
                curSpell.Class = (ClassType)int.Parse(columns["ClassID"]);
                curSpell.SpellID = int.Parse(columns["SpellID"]);
                curSpell.MoneyCost = int.Parse(columns["MoneyCost"]);
                curSpell.ReqSkillLine = int.Parse(columns["ReqSkillLine"]);
                curSpell.ReqSkillRank = int.Parse(columns["ReqSkillRank"]);
                curSpell.ReqLevel = int.Parse(columns["ReqLevel"]);
                curSpell.ReqSpellID = int.Parse(columns["ReqSpellID"]);
                if (TrainerAbilitiesByClassType.ContainsKey(curSpell.Class) == false)
                    TrainerAbilitiesByClassType.Add(curSpell.Class, new List<SpellClassTrainerAbility>());
                TrainerAbilitiesByClassType[curSpell.Class].Add(curSpell);
            }

            // Assign the bind and gate spells
            List<int> casterClassIDs = new List<int> { 5, 7, 8, 9, 11 };
            List<int> meleeClassIDs = new List<int> { 1, 2, 3, 4, 6 };
            if (Configuration.SPELLS_GATE_CASTER_LEARN_LEVEL > 0)
            {
                foreach(int classTypeID in casterClassIDs)
                {
                    SpellClassTrainerAbility curSpell = new SpellClassTrainerAbility();
                    curSpell.Class = (ClassType)classTypeID;
                    curSpell.SpellID = Configuration.SPELLS_GATE_SPELLDBC_ID;
                    curSpell.MoneyCost = Configuration.SPELLS_GATE_SPELL_LEARN_COST;
                    curSpell.ReqLevel = Configuration.SPELLS_GATE_CASTER_LEARN_LEVEL;
                    TrainerAbilitiesByClassType[curSpell.Class].Add(curSpell);
                }
            }
            if (Configuration.SPELLS_GATE_MELEE_LEARN_LEVEL > 0)
            {
                foreach (int classTypeID in meleeClassIDs)
                {
                    SpellClassTrainerAbility curSpell = new SpellClassTrainerAbility();
                    curSpell.Class = (ClassType)classTypeID;
                    curSpell.SpellID = Configuration.SPELLS_GATE_SPELLDBC_ID;
                    curSpell.MoneyCost = Configuration.SPELLS_GATE_SPELL_LEARN_COST;
                    curSpell.ReqLevel = Configuration.SPELLS_GATE_MELEE_LEARN_LEVEL;
                    TrainerAbilitiesByClassType[curSpell.Class].Add(curSpell);
                }
            }
            int bindSpellID = Configuration.SPELLS_BINDANY_SPELLDBC_ID;
            if (Configuration.SPELLS_BIND_CASTER_LEARN_LEVEL > 0 && Configuration.SPELLS_BIND_MELEE_LEARN_LEVEL > 0)
                bindSpellID = Configuration.SPELLS_BINDSELF_SPELLDBC_ID;
            if (Configuration.SPELLS_BIND_CASTER_LEARN_LEVEL > 0)
            {
                foreach (int classTypeID in casterClassIDs)
                {
                    SpellClassTrainerAbility curSpell = new SpellClassTrainerAbility();
                    curSpell.Class = (ClassType)classTypeID;
                    curSpell.SpellID = bindSpellID;
                    curSpell.MoneyCost = Configuration.SPELLS_BIND_SPELL_LEARN_COST;
                    curSpell.ReqLevel = Configuration.SPELLS_BIND_CASTER_LEARN_LEVEL;
                    TrainerAbilitiesByClassType[curSpell.Class].Add(curSpell);
                }
            }
            if (Configuration.SPELLS_BIND_MELEE_LEARN_LEVEL > 0)
            {
                foreach (int classTypeID in meleeClassIDs)
                {
                    SpellClassTrainerAbility curSpell = new SpellClassTrainerAbility();
                    curSpell.Class = (ClassType)classTypeID;
                    curSpell.SpellID = bindSpellID;
                    curSpell.MoneyCost = Configuration.SPELLS_BIND_SPELL_LEARN_COST;
                    curSpell.ReqLevel = Configuration.SPELLS_BIND_MELEE_LEARN_LEVEL;
                    TrainerAbilitiesByClassType[curSpell.Class].Add(curSpell);
                }
            }
        }
    }
}
