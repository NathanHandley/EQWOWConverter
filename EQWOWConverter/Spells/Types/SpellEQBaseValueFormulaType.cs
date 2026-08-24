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

// Note: Any raw formula from 1 to 99 (except the 60 below) has no enum member, and is instead carried as itself by casting the raw formula number into this type
// EQ treats those as "base + (level * the formula number)", so the number is the per-level multiplier and there's no way to name them all

namespace EQWOWConverter.Spells
{
    internal enum SpellEQBaseValueFormulaType : int
    {
        UnknownUseBaseOrMaxWhicheverHigher = 0,
        BaseValue = 100,
        BaseDivideBy100 = 60,
        BaseAddLevel = 102,
        BaseAddLevelTimesTwo = 103,
        BaseAddLevelTimesThree = 104,
        BaseAddLevelTimesFour = 105,
        BaseAddLevelDivideTwo = 107, // Base + Level / 2 (Same as 101)
        BaseAddLevelDivideThree = 108, // (Same as 121)
        BaseAddLevelDivideFour = 109,
        BaseAddLevelDivideFive = 110,

        BaseAddSixTimesLevelMinusSpellLevel = 111, // Base * (Level - Spell Level), (Same as 115)
        BaseAddEightTimesLevelMinusSpellLevel = 112, // (Same as 116)
        BaseAddTenTimesLevelMinusSpellLevel = 113,
        BaseAddFifteenTimesLevelMinusSpellLevel = 114,
        BaseAddTwelveTimesLevelMinusSpellLevel = 117,
        BaseAddTwentyTimesLevelMinusSpellLevel = 118,
        
        BaseAddLevelDivideEight = 119,
        //Splurt = 122, // What is this?
        //RandomBetweenBaseAndMax = 123, never used on Velious and below
        //EffectMa = 203 // What is this? Unknown?
    }
}
