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

namespace EQWOWConverter.Spells
{
    internal static class MovementCastSpellRegistry
    {
        private static readonly object RegistryLock = new object();
        private static SortedSet<int> MovementCastSnaredSpellIDs = new SortedSet<int>();

        public static void RegisterMovementCastSnaredSpellID(int wowSpellID)
        {
            if (wowSpellID <= 0)
                return;
            lock (RegistryLock)
                MovementCastSnaredSpellIDs.Add(wowSpellID);
        }

        // Sorted, so the generated sql is in the same order on every run
        public static List<int> GetMovementCastSnaredSpellIDs()
        {
            lock (RegistryLock)
                return new List<int>(MovementCastSnaredSpellIDs);
        }
    }
}
