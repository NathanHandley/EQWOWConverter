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

using EQWOWConverter.Items;

namespace EQWOWConverter.WOWFiles
{
    internal class CreatureEquipTemplateSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM creature_equip_template WHERE `CreatureID` >= " + Configuration.SQL_CREATURE_GUID_LOW.ToString() + " AND `CreatureID` <= " + Configuration.SQL_CREATURE_GUID_HIGH + ";";
        }

        public void AddRow(int creatureGUID, int mainHandItemID)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("CreatureID", creatureGUID);
            newRow.AddInt("ID", 1);
            newRow.AddInt("ItemID1", mainHandItemID);
            newRow.AddInt("ItemID2", 0);
            newRow.AddInt("ItemID3", 0);
            Rows.Add(newRow);
        }
    }
}
