//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2026 Nathan Handley
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

using EQWOWConverter.Spells;
using EQWOWConverter.Tradeskills;
using System.Text;

namespace EQWOWConverter.WOWFiles
{
    internal class TrainerSpellSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DELETE FROM `trainer_spell` WHERE `TrainerId` >= " + Configuration.SQL_TRAINER_ID_START.ToString() + " AND `TrainerId` <= " + Configuration.SQL_TRAINER_ID_END + ";");
            return stringBuilder.ToString();
        }

        public void AddRow(int trainerID, SpellTrainerAbility trainerAbility)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("TrainerId", trainerID);
            newRow.AddInt("SpellId", trainerAbility.SpellID);
            newRow.AddInt("MoneyCost", trainerAbility.MoneyCost);
            newRow.AddInt("ReqSkillLine", trainerAbility.ReqSkillLine);
            newRow.AddInt("ReqSkillRank", trainerAbility.ReqSkillRank);
            newRow.AddInt("ReqAbility1", trainerAbility.ReqSpellID);
            newRow.AddInt("ReqAbility2", 0);
            newRow.AddInt("ReqAbility3", 0);
            newRow.AddInt("ReqLevel", trainerAbility.ReqLevel);
            newRow.AddInt("VerifiedBuild", 0);
            Rows.Add(newRow);
        }

        public void AddRow(int trainerID, int spellID, int moneyCost, int reqSkillLine, int reqSkillRank, int reqLevel, int reqSpell)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("TrainerId", trainerID);
            newRow.AddInt("SpellId", spellID);
            newRow.AddInt("MoneyCost", moneyCost);
            newRow.AddInt("ReqSkillLine", reqSkillLine);
            newRow.AddInt("ReqSkillRank", reqSkillRank);
            newRow.AddInt("ReqAbility1", reqSpell);
            newRow.AddInt("ReqAbility2", 0);
            newRow.AddInt("ReqAbility3", 0);
            newRow.AddInt("ReqLevel", reqLevel);
            newRow.AddInt("VerifiedBuild", 0);
            Rows.Add(newRow);
        }

        public void AddRiderSkills(int trainerID)
        {
            AddRow(trainerID, 33388, 40000, 762, 0, 20, 0);
            AddRow(trainerID, 33391, 500000, 762, 75, 40, 0);
            if (Configuration.CREATURE_RIDING_TRAINERS_ALSO_TEACH_FLY == true)
            {
                AddRow(trainerID, 34090, 2500000, 762, 150, 60, 0);
                AddRow(trainerID, 34091, 50000000, 762, 225, 70, 0);
            }
        }

        public void AddDevelopmentSkillsForTradeskill(int trainerID, TradeskillType tradeskillType)
        {
            switch (tradeskillType)
            {
                case TradeskillType.Alchemy:
                    {
                        AddRow(trainerID, 2275, 10, 0, 0, 5, 0);
                        AddRow(trainerID, 2280, 500, 171, 50, 10, 0);
                        AddRow(trainerID, 3465, 5000, 171, 125, 20, 0);
                        AddRow(trainerID, 11612, 50000, 171, 200, 35, 0);
                        AddRow(trainerID, 28597, 100000, 171, 275, 50, 0);
                        AddRow(trainerID, 51303, 350000, 171, 350, 65, 0);
                    }
                    break;
                case TradeskillType.Blacksmithing:
                    {
                        AddRow(trainerID, 2020, 10, 0, 0, 5, 0);
                        AddRow(trainerID, 2021, 500, 164, 50, 10, 0);
                        AddRow(trainerID, 3539, 5000, 164, 125, 20, 0);
                        AddRow(trainerID, 9786, 50000, 164, 200, 35, 0);
                        AddRow(trainerID, 29845, 100000, 164, 275, 50, 0);
                        AddRow(trainerID, 51298, 350000, 164, 350, 65, 0);
                    }
                    break;
                case TradeskillType.Cooking:
                    {
                        AddRow(trainerID, 2551, 100, 0, 0, 5, 0);
                        AddRow(trainerID, 3412, 500, 185, 50, 10, 0);
                        AddRow(trainerID, 18261, 25000, 185, 200, 35, 0);
                        AddRow(trainerID, 54257, 1000, 185, 125, 20, 0);
                        AddRow(trainerID, 54256, 100000, 185, 275, 50, 0);
                        AddRow(trainerID, 51295, 350000, 185, 350, 65, 0);
                    }
                    break;
                case TradeskillType.Engineering:
                    {
                        AddRow(trainerID, 4039, 10, 0, 0, 5, 0);
                        AddRow(trainerID, 4040, 500, 202, 50, 10, 0);
                        AddRow(trainerID, 4041, 5000, 202, 125, 20, 0);
                        AddRow(trainerID, 12657, 50000, 202, 200, 35, 0);
                        AddRow(trainerID, 30351, 100000, 202, 275, 50, 0);
                        AddRow(trainerID, 61464, 350000, 202, 350, 65, 0);
                    }
                    break;
                case TradeskillType.Jewelcrafting:
                    {
                        AddRow(trainerID, 25245, 10, 0, 0, 5, 0);
                        AddRow(trainerID, 25246, 500, 755, 50, 10, 0);
                        AddRow(trainerID, 28896, 5000, 755, 125, 20, 0);
                        AddRow(trainerID, 28899, 50000, 755, 200, 35, 0);
                        AddRow(trainerID, 28901, 100000, 755, 275, 50, 0);
                        AddRow(trainerID, 51310, 350000, 755, 350, 65, 0);
                    }
                    break;
                case TradeskillType.Inscription:
                    {
                        AddRow(trainerID, 45375, 10, 0, 0, 5, 0);
                        AddRow(trainerID, 45376, 950, 773, 50, 10, 0);
                        AddRow(trainerID, 45377, 4750, 773, 125, 20, 0);
                        AddRow(trainerID, 45378, 47500, 773, 200, 35, 0);
                        AddRow(trainerID, 45379, 100000, 773, 275, 50, 0);
                        AddRow(trainerID, 45380, 350000, 773, 350, 65, 0);
                    }
                    break;
                case TradeskillType.Tailoring:
                    { 
                        AddRow(trainerID, 3911, 10, 0, 0, 5, 0);
                        AddRow(trainerID, 3912, 500, 197, 50, 10, 0);
                        AddRow(trainerID, 3913, 5000, 197, 125, 20, 0);
                        AddRow(trainerID, 12181, 50000, 197, 200, 35, 0);
                        AddRow(trainerID, 26791, 100000, 197, 275, 50, 0);
                        AddRow(trainerID, 51308, 350000, 197, 350, 65, 0);
                    }
                    break;
                case TradeskillType.Enchanting:
                    {
                        AddRow(trainerID, 7414, 10, 0, 0, 5, 0);
                        AddRow(trainerID, 7415, 500, 333, 50, 10, 0);
                        AddRow(trainerID, 7416, 5000, 333, 125, 20, 0);
                        AddRow(trainerID, 13921, 50000, 333, 200, 35, 0);
                        AddRow(trainerID, 28030, 100000, 333, 275, 50, 0);
                        AddRow(trainerID, 51312, 350000, 333, 350, 65, 0);
                    }
                    break;
                default:
                    {
                        Logger.WriteError("AddDevelopmentSkillsForTradeskill called for unhandled tradeskill type");
                    }
                    break;
            }
        }
    }
}
