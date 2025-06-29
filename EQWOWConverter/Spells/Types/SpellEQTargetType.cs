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
    internal enum SpellEQTargetType : int
    {
        LineOfSight = 1,
        GroupV1 = 3,
        PointBlankAreaOfEffect = 4,
        Single = 5,
        Self = 6,
        TargetedAreaOfEffect = 8,
        Animal = 9,
        Undead = 10,
        Summoned = 11,
        LifeTap = 13, 
        Pet = 14, 
        Corpse = 15,
        Plant = 16, 
        UberGiants = 17,
        UberDragons = 18,
        TargetedAreaOfEffectLifeTap = 20,
        AreaOfEffectUndead = 24,
        AreaOfEffectSummoned = 25, // May not be needed, as "Rebuke Summoned" may not be in use
        Groupv2 = 41
    }
}
