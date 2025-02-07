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

using EQWOWConverter.Creatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal class ModEverquestCreatureOnkillReputationSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM mod_everquest_creature_onkill_reputation;";
        }

        public void AddRow(int creatureTemplateID, CreatureFactionKillReward creatureFactionKillReward)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("CreatureTemplateID", creatureTemplateID);
            newRow.AddInt("SortOrder", creatureFactionKillReward.SortOrder);
            newRow.AddInt("FactionID", creatureFactionKillReward.WOWFactionID);
            newRow.AddInt("KillRewardValue", creatureFactionKillReward.KillRewardValue);
            Rows.Add(newRow);
        }
    }
}
