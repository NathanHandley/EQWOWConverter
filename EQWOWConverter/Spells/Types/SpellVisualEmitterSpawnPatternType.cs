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
    internal enum SpellVisualEmitterSpawnPatternType
    {
        None,
        FromHands, // Exits from the hands based on velocity
        SphereAwayFromPlayer, // Unsure what uses this
        SphereAroundUnit, // Appears around the player starting at EmitterSpawnRadius distance from center
        DiscOnGround, // Disc around the player
        ColumnFromGround, // Around the player, generally going up (gravity)
        DiscAroundUnitCenter, // Comes out from the center of the player
        DiscAboveUnit,
        ColumnFromAbove
    }
}
