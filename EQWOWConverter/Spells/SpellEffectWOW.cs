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
        private Int32 _EffectBasePoints = 0;
        public int EffectBasePoints
        {
            get { return _EffectBasePoints; }
        }
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
            _EffectBasePoints = effectBasePoints;
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
                _EffectBasePoints = this.EffectBasePoints,
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

        public void SetEffectAmountValues(int effectBasePoints, int effectMaxPoints, int spellLevel, SpellEQBaseValueFormulaType eqFormula, bool useMax)
        {
            // Avoid underlevel calculations
            if (spellLevel < 0)
                spellLevel = 0;

            // Cap max based on spell tiers if there is no max
            if (useMax == true && effectMaxPoints == 0)
            {
                if (spellLevel < 60)
                    spellLevel = 60;
                else if (spellLevel < 70)
                    spellLevel = 70;
            }

            // Flip spell level to negative for the calculation if points are negative, since it adds 'down'
            if (effectMaxPoints < 0)
                spellLevel *= -1;

            // Run base through a formula using a calculated value of the supplied spell level, instead of the player level
            _EffectBasePoints = effectBasePoints;
            switch (eqFormula)
            {
                case SpellEQBaseValueFormulaType.BaseDivideBy100: _EffectBasePoints = effectBasePoints / 100; break;
                case SpellEQBaseValueFormulaType.BaseAddLevel: _EffectBasePoints += spellLevel; break;
                case SpellEQBaseValueFormulaType.BaseAddLevelTimesTwo: _EffectBasePoints += (spellLevel * 2); break;
                case SpellEQBaseValueFormulaType.BaseAddLevelTimesThree: _EffectBasePoints += (spellLevel * 3); break;
                case SpellEQBaseValueFormulaType.BaseAddLevelTimesFour: _EffectBasePoints += (spellLevel * 4); break;
                case SpellEQBaseValueFormulaType.BaseAddLevelDivideTwo: _EffectBasePoints += Convert.ToInt32(Convert.ToSingle(spellLevel) * 0.5f); break;
                case SpellEQBaseValueFormulaType.BaseAddLevelDivideThree: _EffectBasePoints += Convert.ToInt32(Convert.ToSingle(spellLevel) * 0.3333f); break;
                case SpellEQBaseValueFormulaType.BaseAddLevelDivideFour: _EffectBasePoints += Convert.ToInt32(Convert.ToSingle(spellLevel) * 0.25f); break;
                case SpellEQBaseValueFormulaType.BaseAddLevelDivideFive: _EffectBasePoints += Convert.ToInt32(Convert.ToSingle(spellLevel) * 0.20f); break;
                case SpellEQBaseValueFormulaType.BaseAddLevelDivideEight: _EffectBasePoints += Convert.ToInt32(Convert.ToSingle(spellLevel) * 0.125f); break;
                default: break;
            }

            // Enforce a maximum if it's set
            if (effectMaxPoints != 0)
            {
                // Flip sign if needed on max
                if ((EffectBasePoints < 0 && effectMaxPoints > 0) || (EffectBasePoints > 0 && effectMaxPoints < 0))
                    effectMaxPoints *= -1;
                if (useMax == true)
                    _EffectBasePoints = effectMaxPoints;
                else if (EffectBasePoints < 0)
                    _EffectBasePoints = Math.Max(EffectBasePoints, effectMaxPoints);
                else
                    _EffectBasePoints = Math.Min(EffectBasePoints, effectMaxPoints);                
            }

            // Disabled this way because this makes both spell tooltips impossible to generate (without being very complicated and messy) due to
            // spell effects having to spill over to multiple auras, and it also makes it extremely hard to balance spells
            //_EffectBasePoints = effectBasePoints;
            //switch (eqFormula)
            //{
            //    case SpellEQBaseValueFormulaType.BaseDivideBy100: _EffectBasePoints = effectBasePoints / 100; break;
            //    case SpellEQBaseValueFormulaType.BaseAddLevel: EffectRealPointsPerLevel = 1; break;
            //    case SpellEQBaseValueFormulaType.BaseAddLevelTimesTwo: EffectRealPointsPerLevel = 2; break;
            //    case SpellEQBaseValueFormulaType.BaseAddLevelTimesThree: EffectRealPointsPerLevel = 3; break;
            //    case SpellEQBaseValueFormulaType.BaseAddLevelTimesFour: EffectRealPointsPerLevel = 4; break;
            //    case SpellEQBaseValueFormulaType.BaseAddLevelDivideTwo: EffectRealPointsPerLevel = 0.5f; break;
            //    case SpellEQBaseValueFormulaType.BaseAddLevelDivideThree: EffectRealPointsPerLevel = 0.3333f; break;
            //    case SpellEQBaseValueFormulaType.BaseAddLevelDivideFour: EffectRealPointsPerLevel = 0.25f; break;
            //    case SpellEQBaseValueFormulaType.BaseAddLevelDivideFive: EffectRealPointsPerLevel = 0.20f; break;
            //    case SpellEQBaseValueFormulaType.BaseAddSixTimesLevelMinusSpellLevel: EffectRealPointsPerLevel = 6; break;
            //    case SpellEQBaseValueFormulaType.BaseAddEightTimesLevelMinusSpellLevel: EffectRealPointsPerLevel = 6; break;
            //    case SpellEQBaseValueFormulaType.BaseAddTenTimesLevelMinusSpellLevel: EffectRealPointsPerLevel = 10; break;
            //    case SpellEQBaseValueFormulaType.BaseAddFifteenTimesLevelMinusSpellLevel: EffectRealPointsPerLevel = 15; break;
            //    case SpellEQBaseValueFormulaType.BaseAddTwelveTimesLevelMinusSpellLevel: EffectRealPointsPerLevel = 12; break;
            //    case SpellEQBaseValueFormulaType.BaseAddTwentyTimesLevelMinusSpellLevel: EffectRealPointsPerLevel = 20; break;
            //    case SpellEQBaseValueFormulaType.BaseAddLevelDivideEight: EffectRealPointsPerLevel = 0.125f; break;
            //    case SpellEQBaseValueFormulaType.BaseValue: // Fallthrough
            //    case SpellEQBaseValueFormulaType.BaseAddLevelTimesMultiplier: // Fallthrough
            //    default: break;
            //}
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
