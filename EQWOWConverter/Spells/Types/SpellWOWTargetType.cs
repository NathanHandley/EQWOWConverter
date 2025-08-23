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

// Scraped from wowwiki on 2025/02/25, which took from TrinityCore
// TARGET_FLAG_NONE            = 0x00000000,
// TARGET_FLAG_UNUSED_1        = 0x00000001,               // not used
// TARGET_FLAG_UNIT            = 0x00000002,               // pguid
// TARGET_FLAG_UNIT_RAID       = 0x00000004,               // not sent, used to validate target (if raid member)
// TARGET_FLAG_UNIT_PARTY      = 0x00000008,               // not sent, used to validate target (if party member)
// TARGET_FLAG_ITEM            = 0x00000010,               // pguid
// TARGET_FLAG_SOURCE_LOCATION = 0x00000020,               // pguid, 3 float
// TARGET_FLAG_DEST_LOCATION   = 0x00000040,               // pguid, 3 float
// TARGET_FLAG_UNIT_ENEMY      = 0x00000080,               // not sent, used to validate target (if enemy)
// TARGET_FLAG_UNIT_ALLY       = 0x00000100,               // not sent, used to validate target (if ally) - Used by teaching spells
// TARGET_FLAG_CORPSE_ENEMY    = 0x00000200,               // pguid
// TARGET_FLAG_UNIT_DEAD       = 0x00000400,               // not sent, used to validate target (if dead creature)
// TARGET_FLAG_GAMEOBJECT      = 0x00000800,               // pguid, used with TARGET_GAMEOBJECT_TARGET
// TARGET_FLAG_TRADE_ITEM      = 0x00001000,               // pguid
// TARGET_FLAG_STRING          = 0x00002000,               // string
// TARGET_FLAG_GAMEOBJECT_ITEM = 0x00004000,               // not sent, used with TARGET_GAMEOBJECT_ITEM_TARGET
// TARGET_FLAG_CORPSE_ALLY     = 0x00008000,               // pguid
// TARGET_FLAG_UNIT_MINIPET    = 0x00010000,               // pguid, used to validate target (if non combat pet)
// TARGET_FLAG_GLYPH_SLOT      = 0x00020000,               // used in glyph spells
// TARGET_FLAG_DEST_TARGET     = 0x00040000,               // sometimes appears with DEST_TARGET spells (may appear or not for a given spell)
// TARGET_FLAG_UNUSED20        = 0x00080000,               // uint32 counter, loop { vec3 - screen position (?), guid }, not used so far
// TARGET_FLAG_UNIT_PASSENGER  = 0x00100000,               // guessed, used to validate target (if vehicle passenger)

namespace EQWOWConverter.Spells
{
    internal enum SpellWOWTargetType : int
    {
        None = 0, // This isn't actually a wow target type, but might be corpse?
        Self = 1,
        Pet = 5,
        TargetEnemy = 6,
        AreaAroundCasterTargetingEnemies = 15,
        AreaAroundTargetEnemy = 16,
        TeleportLocationFromDB = 17,
        CasterParty = 20,
        TargetFriendly = 21,
        AreaAroundCaster = 22,
        AreaAroundTargetAlly = 31,
        TargetParty = 35,
        TargetAny = 63
    }
}
