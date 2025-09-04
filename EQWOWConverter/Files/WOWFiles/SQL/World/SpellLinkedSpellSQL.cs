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
    internal class SpellLinkedSpellSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM spell_linked_spell WHERE `spell_trigger` >= " + Configuration.DBCID_SPELL_ID_START.ToString() + " AND `spell_trigger` <= " + Configuration.DBCID_SPELL_ID_END + ";";
        }

        public void AddRowForAuraTrigger(int triggeringSpellID, int triggeredSpellID, string comment)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("spell_trigger", triggeringSpellID);
            newRow.AddInt("spell_effect", triggeredSpellID);
            newRow.AddInt("type", 2); // 2 with a positive spell_effect value means 'add or remove aura in response to this'
            newRow.AddString("comment", comment);
            Rows.Add(newRow);
        }

        public void AddRowForHitTrigger(int triggeringSpellID, int triggeredSpellID, string comment)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("spell_trigger", triggeringSpellID);
            newRow.AddInt("spell_effect", triggeredSpellID);
            newRow.AddInt("type", 1); // 1 is a spell hit
            newRow.AddString("comment", comment);
            Rows.Add(newRow);
        }
    }
}
