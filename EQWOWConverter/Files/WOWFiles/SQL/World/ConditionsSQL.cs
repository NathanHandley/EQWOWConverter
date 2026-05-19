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

using EQWOWConverter.Common;
using System.Text;

namespace EQWOWConverter.WOWFiles
{
    internal class ConditionsSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DELETE FROM `conditions` WHERE COMMENT like 'EQ %';");
            return stringBuilder.ToString();
        }

        public void AddRowForMenuOptionRaceRestriction(int gossipMenuID, int gossipMenuOptionID, List<RaceType> raceTypes, string comment, int elseGroupID = 0)
        {
            // Race value is a powers of 2, zero index
            int raceMask = 0;
            foreach (RaceType raceType in raceTypes)
                raceMask += (int)(Math.Pow(2, ((int)raceType - 1)));

            SQLRow newRow = new SQLRow();
            newRow.AddInt("SourceTypeOrReferenceId", 15); // CONDITION_SOURCE_TYPE_GOSSIP_MENU_OPTION
            newRow.AddInt("SourceGroup", gossipMenuID);
            newRow.AddInt("SourceEntry", gossipMenuOptionID);
            newRow.AddInt("SourceId", 0);
            newRow.AddInt("ElseGroup", elseGroupID);
            newRow.AddInt("ConditionTypeOrReference", 16); // CONDITION_RACE
            newRow.AddInt("ConditionTarget", 0);
            newRow.AddInt("ConditionValue1", raceMask);
            newRow.AddInt("ConditionValue2", 0);
            newRow.AddInt("ConditionValue3", 0);
            newRow.AddInt("NegativeCondition", 0);
            newRow.AddInt("ErrorType", 0);
            newRow.AddInt("ErrorTextId", 0);
            newRow.AddString("ScriptName", 64, string.Empty);
            newRow.AddString("Comment", 255, "EQ " + comment);
            Rows.Add(newRow);
        }

        public void AddRowForMenuOptionClassRestriction(int gossipMenuID, int gossipMenuOptionID, List<ClassWOWType> classTypes, string comment, int elseGroupID = 0, bool negativeCondition = false)
        {
            // Class value is a powers of 2, zero index
            int classMask = 0;
            foreach (ClassWOWType classType in classTypes)
                classMask += (int)(Math.Pow(2, ((int)classType - 1)));

            SQLRow newRow = new SQLRow();
            newRow.AddInt("SourceTypeOrReferenceId", 15); // CONDITION_SOURCE_TYPE_GOSSIP_MENU_OPTION
            newRow.AddInt("SourceGroup", gossipMenuID);
            newRow.AddInt("SourceEntry", gossipMenuOptionID);
            newRow.AddInt("SourceId", 0);
            newRow.AddInt("ElseGroup", elseGroupID);
            newRow.AddInt("ConditionTypeOrReference", 15); // CONDITION_CLASS
            newRow.AddInt("ConditionTarget", 0);
            newRow.AddInt("ConditionValue1", classMask);
            newRow.AddInt("ConditionValue2", 0);
            newRow.AddInt("ConditionValue3", 0);
            newRow.AddInt("NegativeCondition", negativeCondition == true ? 1 : 0);
            newRow.AddInt("ErrorType", 0);
            newRow.AddInt("ErrorTextId", 0);
            newRow.AddString("ScriptName", 64, string.Empty);
            newRow.AddString("Comment", 255, "EQ " + comment);
            Rows.Add(newRow);
        }

        public void AddRowForMenuOptionAuraExistsRestriction(int gossipMenuID, int gossipMenuOptionID, int auraSpellTemplateID, string comment, int elseGroupID = 0)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("SourceTypeOrReferenceId", 15); // CONDITION_SOURCE_TYPE_GOSSIP_MENU_OPTION
            newRow.AddInt("SourceGroup", gossipMenuID);
            newRow.AddInt("SourceEntry", gossipMenuOptionID);
            newRow.AddInt("SourceId", 0);
            newRow.AddInt("ElseGroup", elseGroupID);
            newRow.AddInt("ConditionTypeOrReference", 1); // CONDITION_AURA
            newRow.AddInt("ConditionTarget", 0);
            newRow.AddInt("ConditionValue1", auraSpellTemplateID);
            newRow.AddInt("ConditionValue2", 0);
            newRow.AddInt("ConditionValue3", 0);
            newRow.AddInt("NegativeCondition", 1); // Setting to "1" means this will fire when the player does NOT have this aura
            newRow.AddInt("ErrorType", 0);
            newRow.AddInt("ErrorTextId", 0);
            newRow.AddString("ScriptName", 64, string.Empty);
            newRow.AddString("Comment", 255, "EQ " + comment);
            Rows.Add(newRow);
        }

        public void AddRowForMenuRestrictionIfAura(int gossipMenuID, int npcTextID, int auraSpellID, string comment, bool negativeCondition = false)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("SourceTypeOrReferenceId", 14); // CONDITION_SOURCE_TYPE_GOSSIP_MENU
            newRow.AddInt("SourceGroup", gossipMenuID);
            newRow.AddInt("SourceEntry", npcTextID);
            newRow.AddInt("SourceId", 0);
            newRow.AddInt("ElseGroup", 0);
            newRow.AddInt("ConditionTypeOrReference", 1); // CONDITION_AURA
            newRow.AddInt("ConditionTarget", 0);
            newRow.AddInt("ConditionValue1", auraSpellID);
            newRow.AddInt("ConditionValue2", 0);
            newRow.AddInt("ConditionValue3", 0);
            newRow.AddInt("NegativeCondition", negativeCondition == true ? 1 : 0);
            newRow.AddInt("ErrorType", 0);
            newRow.AddInt("ErrorTextId", 0);
            newRow.AddString("ScriptName", 64, string.Empty);
            newRow.AddString("Comment", 255, "EQ " + comment);
            Rows.Add(newRow);
        }
    }
}
