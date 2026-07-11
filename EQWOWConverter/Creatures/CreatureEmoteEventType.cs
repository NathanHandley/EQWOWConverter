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

namespace EQWOWConverter.Creatures
{
    // 0-8 match TAKP's EQ::constants::EmoteEventTypes. 9 "Killed" is merged into 5 "KilledPC" on load.
    // 10-11 are for ambient speech triggered by quest scripts
    internal enum CreatureEmoteEventType
    {
        LeaveCombat = 0,
        EnterCombat = 1,
        OnDeath = 2,
        AfterDeath = 3,
        Hailed = 4,
        KilledPC = 5,
        KilledNPC = 6,
        OnSpawn = 7,
        OnDespawn = 8,
        RandomTimer = 10,
        Proximity = 11
    }

    // Matches TAKP's EQ::constants::EmoteTypes
    internal enum CreatureEmoteType
    {
        Say = 0,
        Emote = 1,
        Shout = 2,
        Proximity = 3
    }
}
