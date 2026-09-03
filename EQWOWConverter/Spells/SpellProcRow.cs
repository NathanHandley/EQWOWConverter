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
    internal class SpellProcRow
    {
        public int ProcFlags = 0;
        public int SpellTypeMask = 0;
        public int SpellPhaseMask = 0;
        public int HitMask = 0;
        public int AttributesMask = 0;
        public int CooldownInMS = 0;
        public int ChancePercent = 0; // 0 leaves the roll to the spell's own ProcChance

        public SpellProcRow(int procFlags, int spellTypeMask, int spellPhaseMask, int hitMask, int cooldownInMS, int chancePercent)
        {
            ProcFlags = procFlags;
            SpellTypeMask = spellTypeMask;
            SpellPhaseMask = spellPhaseMask;
            HitMask = hitMask;
            CooldownInMS = cooldownInMS;
            ChancePercent = chancePercent;
        }
    }
}
