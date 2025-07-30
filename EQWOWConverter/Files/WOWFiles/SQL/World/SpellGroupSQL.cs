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

namespace EQWOWConverter.WOWFiles
{
    internal class SpellGroupSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM spell_group WHERE `id` >= " + Configuration.SQL_SPELL_GROUP_ID_START.ToString() + " AND `id` <= " + Configuration.SQL_SPELL_GROUP_ID_END + ";";
        }

        public void AddRow(int id, int wowSpellID)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("id", id);
            newRow.AddInt("spell_id", wowSpellID);
            newRow.AddInt("special_flag", 0); // SPELL_GROUP_SPECIAL_FLAG_FORCED_STRONGEST
            Rows.Add(newRow);
        }
    }
}
