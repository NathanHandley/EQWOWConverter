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
    // Note: These are defines in the mod-everquest file EverQuest.h, so values must match
    internal enum SpellDummyType
    {
        None = 0,
        BindSelf = 1,
        BindAny = 2,
        Gate = 3,
        BardFocusBrass = 4,
        BardFocusString = 5,
        BardFocusWind = 6,
        BardFocusPercussion = 7,
        BardFocusAll = 8,
        BardSongEnemyArea = 9,
        BardSongFriendlyParty = 10,
        BardSongSelf = 11,
        BardSongEnemySingle = 12,
        BardSongFriendlySingle = 13,
        BardSongAnySingle = 14,
        IllusionParent = 15,
        SummonPet = 16,
    }
}
