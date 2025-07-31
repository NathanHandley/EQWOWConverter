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
    internal enum SpellEQEffectType : int
    {
        None = 254,

        CurrentHitPoints = 0, // value of Negative = Damage and Positive = Heal. 
        ArmorClass = 1,
        Attack = 2,
        MovementSpeed = 3,
        //Strength = 4,
        //Dexterity = 5,
        //Agility = 6,
        //Stamina = 7,
        //Intelligence = 8,
        //Wisdom = 9,
        //Charisma = 10,
        //AttackSpeed = 11,
        //Invisibility = 12,
        //SeeInvisibility = 13,
        //WaterBreathing = 14,
        //CurrentMana = 15,
        //NPCFrenzy = 16, // ?
        //NPCAwareness = 17, // ?
        //Pacify = 18, // aka: Lull
        //ModFaction = 19,
        //Blind = 20,
        //Stun = 21,
        TotalHP = 69,
        CurrentHitPointsOnce = 79,
        // TODO: Like 500 more...
    }
}
