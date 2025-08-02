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
    internal class SpellEffectWOW : IComparable<SpellEffectWOW>
    {
        public SpellWOWEffectType EffectType = SpellWOWEffectType.None;
        public Int32 EffectDieSides = 0;
        public float EffectRealPointsPerLevel = 0;
        public Int32 EffectBasePoints = 0;
        public SpellMechanicType EffectMechanic = SpellMechanicType.None;
        public UInt32 ImplicitTagetB = 0;
        public UInt32 EffectRadiusIndex = 0;
        public SpellWOWAuraType EffectAuraType = SpellWOWAuraType.None;
        public UInt32 EffectAuraPeriod = 0;
        public float EffectMultipleValue = 0;
        public UInt32 EffectChainTargets = 0;
        public UInt32 EffectItemType = 0;
        public int EffectMiscValueA = 0;
        public int EffectMiscValueB = 0;
        public string ActionDescription = string.Empty;
        public string AuraDescription = string.Empty;

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

        public SpellEffectWOW Clone()
        {
            return new SpellEffectWOW
            {
                EffectType = this.EffectType,
                EffectDieSides = this.EffectDieSides,
                EffectRealPointsPerLevel = this.EffectRealPointsPerLevel,
                EffectBasePoints = this.EffectBasePoints,
                EffectMechanic = this.EffectMechanic,
                ImplicitTagetB = this.ImplicitTagetB,
                EffectRadiusIndex = this.EffectRadiusIndex,
                EffectAuraType = this.EffectAuraType,
                EffectAuraPeriod = this.EffectAuraPeriod,
                EffectMultipleValue = this.EffectMultipleValue,
                EffectChainTargets = this.EffectChainTargets,
                EffectItemType = this.EffectItemType,
                EffectMiscValueA = this.EffectMiscValueA,
                EffectMiscValueB = this.EffectMiscValueB,
                ActionDescription = this.ActionDescription,
                AuraDescription = this.AuraDescription
            };
        }

        public bool IsAuraType()
        {
            if (EffectType == SpellWOWEffectType.ApplyAura)
                return true;
            if (EffectType == SpellWOWEffectType.ApplyAreaAuraParty)
                return true;
            if (EffectType == SpellWOWEffectType.ApplyAreaAuraEnemy)
                return true;
            if (EffectType == SpellWOWEffectType.ApplyAreaAuraFriend)
                return true;
            if (EffectType == SpellWOWEffectType.ApplyAreaAuraOwner)
                return true;
            if (EffectType == SpellWOWEffectType.ApplyAreaAuraPet)
                return true;
            if (EffectType == SpellWOWEffectType.ApplyAreaAuraRaid)
                return true;
            return false;
        }

        public int CompareTo(SpellEffectWOW? other)
        {
            // Null and auras should evaluate as greater to push them to the bottom of the list
            if (other == null)
                return 0;
            if (IsAuraType() == true && other.IsAuraType() == false)
                return 1;
            if (IsAuraType() == false && other.IsAuraType() == true)
                return -1;
            return 0;
        }
    }
}
