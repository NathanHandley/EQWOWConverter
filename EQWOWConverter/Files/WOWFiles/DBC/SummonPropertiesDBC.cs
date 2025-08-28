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

using EQWOWConverter.Spells;

namespace EQWOWConverter.WOWFiles
{
    internal class SummonPropertiesDBC : DBCFile
    {
        private static int CURRENT_ID = Configuration.DBCID_SUMMONPROPERTIES_ID_START;
        private static readonly object SummonIDLock = new object();

        public void AddRow(int id, SpellPet spellPet)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(id); // ID
            newRow.AddInt32(1); // Control (1 = Guardian, 2 = Pet)
            newRow.AddInt32(0); // Faction
            newRow.AddInt32(1); // Title (1 = Pet, 2 = Guardian, 3 = Minion)
            newRow.AddInt32(0); // Slot
            newRow.AddInt32(GetFlags(spellPet)); // Flags
            Rows.Add(newRow);
        }

        private int GetFlags(SpellPet spellPet)
        {
            int flags = 0;
            flags |= 256; // Use Creature Level
            flags |= 4096; // Use Summoner Faction
            flags |= 131072; // Despawn on Summoner Logout
            if (spellPet.ControlType == SpellPetControlType.FullControl)
            {
                flags |= 524288; // Guardian Acts Like a Pet
            }
            else if (spellPet.ControlType == SpellPetControlType.NoControl || spellPet.ControlType == SpellPetControlType.NoAttackControl)
            {
                flags |= 2; // Help when Summoned in combat
            }
            return flags;
        }

        public static int GenerateUniqueID()
        {
            lock (SummonIDLock)
            {
                int curID = CURRENT_ID;
                CURRENT_ID++;
                return curID;
            }
        }
    }
}
