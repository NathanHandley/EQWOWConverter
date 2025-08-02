//  Author: Nathan Handley(nathanhandley@protonmail.com)
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

namespace EQWOWConverter.Spells
{
    internal enum SpellMechanicType : UInt32
    {
        None = 0,
        Charmed = 1,
        Disoriented = 2,
        Disarmed = 3,
        Distracted = 4,
        Fleeing = 5,
        Gripped = 6,
        Rooted = 7,
        Slowed = 8,
        Silenced = 9,
        Asleep = 10,
        Snared = 11,
        Stunned = 12,
        Frozen = 13,
        Incapacitated = 14,
        Bleeding = 15,
        Healing = 16,
        Polymorphed = 17,
        Banished = 18,
        Shielded = 19,
        Shackled = 20,
        Mounted = 21,
        Infected = 22,
        Turned = 23,
        Horrified = 24,
        Invulnerable = 25,
        Interrupted = 26,
        Dazed = 27,
        Discovery = 28,
        // Invulnerable = 29, Duplicate?  See SpellMechanic.dbc
        Sapped = 30,
        Enraged = 31
    }
}
