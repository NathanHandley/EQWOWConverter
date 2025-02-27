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
    internal class SkillLineAbilityDBC : DBCFile
    {
        private static int CUR_SPELL_DBCID = Configuration.DBCID_SKILLLINEABILITY_ID_START;

        public void AddRow(int id, int skillLineID, int spellID)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(id); // ID
            newRow.AddInt32(skillLineID); // SkillLine
            newRow.AddInt32(spellID); // Spell
            newRow.AddInt32(0); // RaceMask
            newRow.AddInt32(0); // ClassMask
            newRow.AddInt32(0); // ExcludeRace
            newRow.AddInt32(0); // ExcludeClass
            newRow.AddInt32(1); // MinSkillLineRank
            newRow.AddInt32(0); // SupercededBySpell
            newRow.AddInt32(0); // AcquireMethod (0 = learn by trainer)
            newRow.AddInt32(0); // TrivialSkillLineRankHigh
            newRow.AddInt32(0); // TrivialSkillLineRankLow
            newRow.AddInt32(0); // CharacterPoints1
            newRow.AddInt32(0); // CharacterPoints2
            Rows.Add(newRow);
        }

        public static int GenerateID()
        {
            int spellID = CUR_SPELL_DBCID;
            CUR_SPELL_DBCID++;
            return spellID;
        }
    }
}
