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
    internal enum SpellEQFormulaType : int
    {
        BaseValue = 100,

        BaseAddLevelTimesMultiplier = 1,
        BaseDivideBy100 = 60,
        BaseAddHalfLevel = 101, // Base + Level / 2
        BaseAddLevel = 102,
        BaseAddLevelTimesTwo = 103,
        BaseAddLevelTimesThree = 104,
        BaseAddLevelTimesFour = 105,
        BaseAddLevelDivideTwo = 107, // Base + Level / 2 (same as BaseAddHalfLevel?)
        BaseAddLevelDivideThree = 108,
        BaseAddLevelDivideFour = 109,
        BaseAddLevelDivideFive = 110,
        BaseAddSixTimesLevelMinusSpellLevel = 111, // Base * (Level - Spell Level)
        BaseAddEightTimesLevelMinusSpellLevel = 112,
        BaseAddTenTimesLevelMinusSpellLevel = 113,
        BaseAddFifteenTimesLevelMinusSpellLevel = 114,
        BaseAddSixTimesLevelMinusSpellLevel2 = 115, // Same as BaseAddSixTimesLevelMinusSpellLevel?
        BaseAddEightTimesLevelMinusSpellLevel2 = 116,
        BaseAddTwelveTimesLevelMinusSpellLevel = 117,
        BaseAddTwentyTimesLevelMinusSpellLevel = 118,
        BaseAddLevelDivideEight = 119,
        BaseAddLevelDivideThree2 = 121, // Same as BaseAddLevelDivideThree?
        Splurt = 122, // What is this?
        RandomBetweenBaseAndMax = 123,
        EffectMa = 203 // What is this?
    }
}
