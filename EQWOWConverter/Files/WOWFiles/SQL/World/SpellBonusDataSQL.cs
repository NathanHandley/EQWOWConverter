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

namespace EQWOWConverter.WOWFiles
{
    internal class SpellBonusDataSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM spell_bonus_data WHERE `entry` >= " + Configuration.DBCID_SPELL_ID_START.ToString() + " AND `entry` <= " + Configuration.DBCID_SPELL_ID_END + ";";
        }

        public void AddRow(int spellTemplateID, string comment)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("entry", spellTemplateID);
            newRow.AddFloat("direct_bonus", Configuration.SPELL_DEFAULT_SPELL_POWER_INFLUENCE_PERCENT);
            newRow.AddFloat("dot_bonus", Configuration.SPELL_DEFAULT_SPELL_POWER_INFLUENCE_PERCENT);
            newRow.AddFloat("ap_bonus", 0);
            newRow.AddFloat("ap_dot_bonus", 0);
            newRow.AddString("comments", comment);
            Rows.Add(newRow);
        }
    }
}
