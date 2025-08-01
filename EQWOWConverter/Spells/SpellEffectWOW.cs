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
    internal class SpellEffectWOW : IEquatable<SpellEffectWOW>, IComparable<SpellEffectWOW>
    {
        public SpellWOWEffectType EffectType = SpellWOWEffectType.None;
        public Int32 EffectDieSides = 0;
        public float EffectRealPointsPerLevel = 0;
        public Int32 EffectBasePoints = 0;
        public UInt32 EffectMechanic = 0;
        public UInt32 ImplicitTagetB = 0;
        public UInt32 EffectRadiusIndex = 0;
        public SpellWOWAuraType EffectAuraType = SpellWOWAuraType.None;
        public UInt32 EffectAuraPeriod = 0;
        public float EffectMultipleValue = 0;
        public UInt32 EffectChainTargets = 0;
        public UInt32 EffectItemType = 0;
        public int EffectMiscValueA = 0;
        public int EffectMiscValueB = 0;

        public SpellEffectWOW() { }

        public SpellEffectWOW(SpellWOWEffectType effectType, SpellWOWAuraType effectAuraType, uint effectAuraPeriod, uint effectItemType, int effectDieSides, int effectBasePoints, 
            int effectMiscValueA, int effectMiscValueB)
        {
            EffectType = effectType;
            EffectAuraType = effectAuraType;
            EffectAuraPeriod = effectAuraPeriod;
            EffectItemType = effectItemType;
            EffectDieSides = effectDieSides;
            EffectBasePoints = effectBasePoints;
            EffectMiscValueA = effectMiscValueA;
            EffectMiscValueB = effectMiscValueB;
        }

        public int CompareTo(SpellEffectWOW? other)
        {
            // Null and auras should evaluate as 'higher' (bottom)
            if (other == null)
                return 1;
            if (EffectType == SpellWOWEffectType.ApplyAura)
                return 1;
            if (EffectType == SpellWOWEffectType.ApplyAreaAuraParty)
                return 1;
            if (EffectType == SpellWOWEffectType.ApplyAreaAuraEnemy)
                return 1;
            if (EffectType == SpellWOWEffectType.ApplyAreaAuraFriend)
                return 1;
            if (EffectType == SpellWOWEffectType.ApplyAreaAuraOwner)
                return 1;
            if (EffectType == SpellWOWEffectType.ApplyAreaAuraPet)
                return 1;
            if (EffectType == SpellWOWEffectType.ApplyAreaAuraRaid)
                return 1;
            return 0;
        }

        public bool Equals(SpellEffectWOW? other)
        {
            if (other == null)
                return false;
            return EffectType == other.EffectType &&
                   EffectDieSides == other.EffectDieSides &&
                   EffectRealPointsPerLevel == other.EffectRealPointsPerLevel &&
                   EffectBasePoints == other.EffectBasePoints &&
                   EffectMechanic == other.EffectMechanic &&
                   ImplicitTagetB == other.ImplicitTagetB &&
                   EffectRadiusIndex == other.EffectRadiusIndex &&
                   EffectAuraType == other.EffectAuraType &&
                   EffectAuraPeriod == other.EffectAuraPeriod &&
                   EffectMultipleValue == other.EffectMultipleValue &&
                   EffectChainTargets == other.EffectChainTargets &&
                   EffectItemType == other.EffectItemType &&
                   EffectMiscValueA == other.EffectMiscValueA &&
                   EffectMiscValueB == other.EffectMiscValueB;
        }
    }
}
