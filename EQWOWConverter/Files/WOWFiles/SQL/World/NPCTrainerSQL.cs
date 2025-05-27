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

using EQWOWConverter.Spells;
using EQWOWConverter.Tradeskills;
using System.Text;

namespace EQWOWConverter.WOWFiles
{
    internal class NPCTrainerSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DELETE FROM `npc_trainer` WHERE `ID` >= " + Configuration.SQL_NPCTRAINER_ID_START.ToString() + " AND `ID` <= " + Configuration.SQL_NPCTRAINER_ID_END + ";");
            stringBuilder.AppendLine("DELETE FROM `npc_trainer` WHERE `ID` >= " + Configuration.SQL_CREATURETEMPLATE_ENTRY_LOW.ToString() + " AND `ID` <= " + Configuration.SQL_CREATURETEMPLATE_ENTRY_HIGH + ";");
            return stringBuilder.ToString();
        }

        public void AddRowForTrainerReference(int spellLinesID, int creatureTemplateEntryID)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("ID", creatureTemplateEntryID);
            newRow.AddInt("SpellID", spellLinesID * -1); // Making it a negative number forces a reference group lookup at the end
            newRow.AddInt("MoneyCost", 0);
            newRow.AddInt("ReqSkillLine", 0);
            newRow.AddInt("ReqSkillRank", 0);
            newRow.AddInt("ReqLevel", 0);
            newRow.AddInt("ReqSpell", 0);
            Rows.Add(newRow);
        }

        public void AddRowForTrainerAbility(int lineID, SpellTrainerAbility trainerAbility)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("ID", lineID);
            newRow.AddInt("SpellID", trainerAbility.SpellID);
            newRow.AddInt("MoneyCost", trainerAbility.MoneyCost);
            newRow.AddInt("ReqSkillLine", trainerAbility.ReqSkillLine);
            newRow.AddInt("ReqSkillRank", trainerAbility.ReqSkillRank);
            newRow.AddInt("ReqLevel", trainerAbility.ReqLevel);
            newRow.AddInt("ReqSpell", trainerAbility.ReqSpellID);
            Rows.Add(newRow);
        }

        public void AddRowManual(int id, int spellID, int moneyCost, int reqSkillLine, int reqSkillRank, int reqLevel, int reqSpell)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("ID", id);
            newRow.AddInt("SpellID", spellID);
            newRow.AddInt("MoneyCost", moneyCost);
            newRow.AddInt("ReqSkillLine", reqSkillLine);
            newRow.AddInt("ReqSkillRank", reqSkillRank);
            newRow.AddInt("ReqLevel", reqLevel);
            newRow.AddInt("ReqSpell", reqSpell);
            Rows.Add(newRow);
        }

        public void AddDevelopmentSkillsForTradeskill(int lineID, TradeskillType tradeskillType)
        {
            switch (tradeskillType)
            {
                case TradeskillType.Alchemy:
                    {
                        AddRowManual(lineID, 2275, 10, 0, 0, 5, 0);
                        AddRowManual(lineID, 2280, 500, 171, 50, 10, 0);
                        AddRowManual(lineID, 3465, 5000, 171, 125, 20, 0);
                        AddRowManual(lineID, 11612, 50000, 171, 200, 35, 0);
                        AddRowManual(lineID, 28597, 100000, 171, 275, 50, 0);
                        AddRowManual(lineID, 51303, 350000, 171, 350, 65, 0);
                    }
                    break;
                case TradeskillType.Blacksmithing:
                    {
                        AddRowManual(lineID, 2020, 10, 0, 0, 5, 0);
                        AddRowManual(lineID, 2021, 500, 164, 50, 10, 0);
                        AddRowManual(lineID, 3539, 5000, 164, 125, 20, 0);
                        AddRowManual(lineID, 9786, 50000, 164, 200, 35, 0);
                        AddRowManual(lineID, 29845, 100000, 164, 275, 50, 0);
                        AddRowManual(lineID, 51298, 350000, 164, 350, 65, 0);
                    }
                    break;
                case TradeskillType.Cooking:
                    {
                        AddRowManual(lineID, 2551, 100, 0, 0, 5, 0);
                        AddRowManual(lineID, 3412, 500, 185, 50, 10, 0);
                        AddRowManual(lineID, 18261, 25000, 185, 200, 35, 0);
                        AddRowManual(lineID, 54257, 1000, 185, 125, 20, 0);
                        AddRowManual(lineID, 54256, 100000, 185, 275, 50, 0);
                        AddRowManual(lineID, 51295, 350000, 185, 350, 65, 0);
                    }
                    break;
                case TradeskillType.Engineering:
                    {
                        AddRowManual(lineID, 4039, 10, 0, 0, 5, 0);
                        AddRowManual(lineID, 4040, 500, 202, 50, 10, 0);
                        AddRowManual(lineID, 4041, 5000, 202, 125, 20, 0);
                        AddRowManual(lineID, 12657, 50000, 202, 200, 35, 0);
                        AddRowManual(lineID, 30351, 100000, 202, 275, 50, 0);
                        AddRowManual(lineID, 61464, 350000, 202, 350, 65, 0);
                    }
                    break;
                case TradeskillType.Jewelcrafting:
                    {
                        AddRowManual(lineID, 25245, 10, 0, 0, 5, 0);
                        AddRowManual(lineID, 25246, 500, 755, 50, 10, 0);
                        AddRowManual(lineID, 28896, 5000, 755, 125, 20, 0);
                        AddRowManual(lineID, 28899, 50000, 755, 200, 35, 0);
                        AddRowManual(lineID, 28901, 100000, 755, 275, 50, 0);
                        AddRowManual(lineID, 51310, 350000, 755, 350, 65, 0);
                    }
                    break;
                case TradeskillType.Inscription:
                    {
                        AddRowManual(lineID, 45375, 10, 0, 0, 5, 0);
                        AddRowManual(lineID, 45376, 950, 773, 50, 10, 0);
                        AddRowManual(lineID, 45377, 4750, 773, 125, 20, 0);
                        AddRowManual(lineID, 45378, 47500, 773, 200, 35, 0);
                        AddRowManual(lineID, 45379, 100000, 773, 275, 50, 0);
                        AddRowManual(lineID, 45380, 350000, 773, 350, 65, 0);
                    }
                    break;
                case TradeskillType.Tailoring:
                    {
                        AddRowManual(lineID, 3911, 10, 0, 0, 5, 0);
                        AddRowManual(lineID, 3912, 500, 197, 50, 10, 0);
                        AddRowManual(lineID, 3913, 5000, 197, 125, 20, 0);
                        AddRowManual(lineID, 12181, 50000, 197, 200, 35, 0);
                        AddRowManual(lineID, 26791, 100000, 197, 275, 50, 0);
                        AddRowManual(lineID, 51308, 350000, 197, 350, 65, 0);
                    }
                    break;
                case TradeskillType.Enchanting:
                    {
                        AddRowManual(lineID, 7414, 10, 0, 0, 5, 0);
                        AddRowManual(lineID, 7415, 500, 333, 50, 10, 0);
                        AddRowManual(lineID, 7416, 5000, 333, 125, 20, 0);
                        AddRowManual(lineID, 13921, 50000, 333, 200, 35, 0);
                        AddRowManual(lineID, 28030, 100000, 333, 275, 50, 0);
                        AddRowManual(lineID, 51312, 350000, 333, 350, 65, 0);
                    }
                    break;
                default:
                    {
                        Logger.WriteError("AddDevelopmentSkillsForTradeskill called for unhandled tradeskill type");
                    } break;
            }
        }
    }
}
