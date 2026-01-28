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
    }
}
