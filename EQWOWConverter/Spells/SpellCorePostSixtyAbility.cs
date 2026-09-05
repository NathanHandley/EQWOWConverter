//  Author: Nathan Handley(nathanhandley@protonmail.com)
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
    internal static class SpellCorePostSixtyAbility
    {
        public const int REDUCED_TO_LEVEL = 60;

        private static readonly List<int> ReducedFirstRankSpellIDs = new List<int>()
        {
            30451, // Arcane Blast (Mage, rank 1, was 64)
            30455, // Ice Lance (Mage, rank 1, was 66)
            32645, // Envenom (Rogue, rank 1, was 62)
            51723, // Fan of Knives (Rogue, was 80)
            49020, // Obliterate (DeathKnight, rank 1, was 61)
            51505, // Lava Burst (Shaman, rank 1, was 75)
            62078, // Swipe (Cat) (Druid, rank 1, was 71)
        };

        public static List<int> GetReducedFirstRankSpellIDs()
        {
            return new List<int>(ReducedFirstRankSpellIDs);
        }

        public static bool IsReducedFirstRankSpellID(int spellID)
        {
            return ReducedFirstRankSpellIDs.Contains(spellID);
        }
    }
}
