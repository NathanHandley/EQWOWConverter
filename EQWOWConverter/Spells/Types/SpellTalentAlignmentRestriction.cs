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
    internal enum SpellTalentAlignmentRestriction
    {
        None = 0,
        DirectDamage = 1,           // Spell has a direct (non-periodic) damage or health leech effect
        AreaDamage = 2,             // The damage lands in an area (targeted or caster-centered)
        PointBlankAreaDamage = 3,   // The damage area is centered on the caster
        DirectHeal = 4,             // Spell has a direct (non-periodic) heal effect
        SingleTargetDirectHeal = 5, // Direct heal that lands on one target (no group or area heal targets)
        AreaHeal = 6,               // Heal that lands on a group or area
        PeriodicHeal = 7,           // Spell has a heal over time effect
        PeriodicDamage = 8,         // Spell has a damage over time effect
        Cure = 9,                   // Spell dispels, or cures poison / disease / curse
        StrengthDebuff = 10,        // The effect the modifier targets is a strength reduction
        PetSummon = 11              // Spell summons a pet
    }
}
