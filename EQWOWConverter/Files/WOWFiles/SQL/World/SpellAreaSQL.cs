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
    internal class SpellAreaSQL : SQLFile
    {
        public const int SPELL_ID_GHOST = 8326;                     // The aura a player has while released as a ghost
        public const int SPELL_ID_GHOST_MOUNT_FLYING = 55164;       // Swift Spectral Gryphon
        public const int SPELL_ID_GHOST_MOUNT_FLYING_WISP = 55173;  // Swift Flying Wisp
        private const int RACEMASK_ALL_EXCEPT_NIGHTELF = 65527;
        private const int RACEMASK_NIGHTELF_ONLY = 8;

        public override string DeleteRowSQL()
        {
            return "DELETE FROM spell_area WHERE `area` >= " + Configuration.DBCID_AREATABLE_ID_START.ToString() + " AND `area` <= " + Configuration.DBCID_AREATABLE_ID_END.ToString() + ";";
        }

        public void AddRowsForFlyingGhostArea(int areaID)
        {
            // Need both rows so that night elves (which are wisps when dead) also work
            AddRow(SPELL_ID_GHOST_MOUNT_FLYING, areaID, SPELL_ID_GHOST, RACEMASK_ALL_EXCEPT_NIGHTELF);
            AddRow(SPELL_ID_GHOST_MOUNT_FLYING_WISP, areaID, SPELL_ID_GHOST, RACEMASK_NIGHTELF_ONLY);
        }

        private void AddRow(int spellID, int areaID, int auraSpellID, int raceMask)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("spell", spellID);
            newRow.AddInt("area", areaID);
            newRow.AddInt("quest_start", 0);
            newRow.AddInt("quest_end", 0);
            newRow.AddInt("aura_spell", auraSpellID);
            newRow.AddInt("racemask", raceMask);
            newRow.AddInt("gender", 2); // 2 is 'any gender'
            newRow.AddInt("autocast", 1);
            newRow.AddInt("quest_start_status", 64);
            newRow.AddInt("quest_end_status", 11);
            Rows.Add(newRow);
        }
    }
}
