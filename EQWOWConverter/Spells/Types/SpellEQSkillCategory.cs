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

namespace EQWOWConverter.Spells
{
    internal enum SpellEQSkillCategory : int
    {
        Unknown = 0, // This is actually 1HBlunt
        Abjuration = 4,
        Alteration = 5,
        Brass = 12,
        Conjuration = 14,
        Defense = 15,
        Divination = 18,
        Evocation = 24,
        Offense = 33,
        Singing = 41,
        Stringed = 49,
        TigerClaw = 52, // Seriously?
        Wind = 54,
        Percussion = 70
    }
}
