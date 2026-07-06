//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2024 Nathan Handley
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

namespace EQWOWConverter.WOWFiles
{
    internal class CreatureModelInfoSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM creature_model_info WHERE `DisplayID` >= " + Configuration.DBCID_CREATUREDISPLAYINFO_ID_START.ToString() + " AND `DisplayID` <= " + Configuration.DBCID_CREATUREDISPLAYINFO_ID_END.ToString() + ";";
        }

        public void AddRow(int displayID, int gender)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("DisplayID", displayID); // Reference to CreatureDisplayInfo.dbc
            newRow.AddFloat("BoundingRadius", 0); // Not currently used (?)
            newRow.AddFloat("CombatReach", 1.5f * (Configuration.GENERATE_EQUIPMENT_SCALE / Configuration.GENERATE_CREATURE_SCALE)); // Core multiplies this by unit scale which carries some baked in scaling, so take it out
            newRow.AddInt("Gender", gender); // 0: Male, 1: Female, 2: None/Neutral
            newRow.AddInt("DisplayID_Other_Gender", 0); // Record that relates to the 'other' gender ID (if male, this is the female row...)
            Rows.Add(newRow);
        }
    }
}
