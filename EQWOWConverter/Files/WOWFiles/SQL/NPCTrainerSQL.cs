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

namespace EQWOWConverter.WOWFiles
{
    internal class NPCTrainerSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM `npc_trainer` WHERE `ID` >= " + Configuration.CONFIG_SQL_NPCTRAINER_ID_START.ToString() + " AND `ID` <= " + Configuration.CONFIG_SQL_NPCTRAINER_ID_END + ";";
        }

        public void AddRow(int lineID, SpellClassTrainerAbility trainerAbility)
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
    }
}
