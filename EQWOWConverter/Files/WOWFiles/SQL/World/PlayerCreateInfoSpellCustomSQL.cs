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
    internal class PlayerCreateInfoSpellCustomSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM playercreateinfo_spell_custom WHERE `Spell` >= " + Configuration.DBCID_SPELL_ID_START.ToString() + " AND `Spell` <= " + Configuration.DBCID_SPELL_ID_GENERATED_START + ";";
        }

        public void AddRow(int spellID, string note)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("racemask", 0);
            newRow.AddInt("classmask", 0);
            newRow.AddInt("Spell", spellID);
            newRow.AddString("Note", 255, note);
            Rows.Add(newRow);
        }
    }
}
