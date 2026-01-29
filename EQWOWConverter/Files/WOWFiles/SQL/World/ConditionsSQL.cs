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
            stringBuilder.AppendLine("DELETE FROM `conditions` WHERE `SourceTypeOrReferenceID` = 15 AND `SourceGroup` >= " + Configuration.SQL_GOSSIPMENU_MENUID_START.ToString() + " AND `SourceGroup` <= " + Configuration.SQL_GOSSIPMENU_MENUID_END.ToString() + ";");
            return stringBuilder.ToString();
        }

        public void AddRowForMenuOptionClassRestriction(int gossipMenuID, int gossipMenuOptionID, ClassType classType, string comment)
        {
            // Class value is a powers of 2, zero index
            int classValue = (int)(Math.Pow(2, ((int)classType - 1)));

            SQLRow newRow = new SQLRow();
            newRow.AddInt("SourceTypeOrReferenceId", 15); // CONDITION_SOURCE_TYPE_GOSSIP_MENU_OPTION
            newRow.AddInt("SourceGroup", gossipMenuID);
            newRow.AddInt("SourceEntry", gossipMenuOptionID);
            newRow.AddInt("SourceId", 0);
            newRow.AddInt("ElseGroup", 0);
            newRow.AddInt("ConditionTypeOrReference", 15); // CONDITION_CLASS
            newRow.AddInt("ConditionTarget", 0);
            newRow.AddInt("ConditionValue1", classValue);
            newRow.AddInt("ConditionValue2", 0);
            newRow.AddInt("ConditionValue3", 0);
            newRow.AddInt("NegativeCondition", 0);
            newRow.AddInt("ErrorType", 0);
            newRow.AddInt("ErrorTextId", 0);
            newRow.AddString("ScriptName", 64, string.Empty);
            newRow.AddString("Comment", 255, comment);
            Rows.Add(newRow);
        }
    }
}
