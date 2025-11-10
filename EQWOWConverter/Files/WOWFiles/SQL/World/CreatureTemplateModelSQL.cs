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

using System.Text;

namespace EQWOWConverter.WOWFiles
{
    internal class CreatureTemplateModelSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("DELETE FROM creature_template_model WHERE `CreatureID` >= " + Configuration.SQL_CREATURETEMPLATE_DEBUG_ENTRY_LOW.ToString() + " AND `CreatureID` <= " + Configuration.SQL_CREATURETEMPLATE_DEBUG_ENTRY_HIGH + ";");
            sb.Append("DELETE FROM creature_template_model WHERE `CreatureID` >= " + Configuration.SQL_CREATURETEMPLATE_ENTRY_LOW.ToString() + " AND `CreatureID` <= " + Configuration.SQL_CREATURETEMPLATE_ENTRY_HIGH + ";");
            return sb.ToString();
        }

        public void AddRow(int creatureTemplateID, int creatureDisplayID, float displayScale)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("CreatureID", creatureTemplateID);
            newRow.AddInt("Idx", 0);
            newRow.AddInt("CreatureDisplayID", creatureDisplayID); // CreatureDisplayInfo.dbc reference
            newRow.AddFloat("DisplayScale", displayScale);
            newRow.AddFloat("Probability", 1);
            newRow.AddInt("VerifiedBuild", 12340);
            Rows.Add(newRow);
        }
    }
}
