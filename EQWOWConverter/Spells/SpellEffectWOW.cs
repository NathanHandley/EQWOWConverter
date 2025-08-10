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
        private static Dictionary<string, Dictionary<string, float>> EffectValueBaselinesByEffectAndValueType = new Dictionary<string, Dictionary<string, float>>();

        public SpellWOWEffectType EffectType = SpellWOWEffectType.None;
        public Int32 EffectDieSides = 0;
        public float EffectRealPointsPerLevel = 0;
        private Int32 _EffectBasePoints = 0;
        public int EffectBasePoints
        {
            get { return _EffectBasePoints; }
        }
        public SpellMechanicType EffectMechanic = SpellMechanicType.None;
        public SpellWOWTargetType ImplicitTargetA = SpellWOWTargetType.Self;
        public SpellWOWTargetType ImplicitTargetB = SpellWOWTargetType.None;
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
                ImplicitTargetA = this.ImplicitTargetA,
                ImplicitTargetB = this.ImplicitTargetB,
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

        public void SetEffectAmountValues(int effectBasePoints, int effectMaxPoints, int spellLevel, SpellEQBaseValueFormulaType eqFormula, 
            int spellCastTimeInMS, bool useMax, string valueScalingFormulaName)
        {
            // Normalize the formula name
            valueScalingFormulaName = valueScalingFormulaName.ToLower().Trim();

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

            // Only work with positive values
            bool effectBasePointsWasNegative = false;
            if (effectBasePoints < 0)
            {
                effectBasePointsWasNegative = true;
                effectBasePoints *= -1;
            }
            if (effectMaxPoints < 0)
                effectMaxPoints *= -1;

            // Run base through a formula using a calculated value of the supplied spell level, instead of the player level
            _EffectBasePoints = Math.Abs(effectBasePoints);
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
                if (useMax == true)
                    _EffectBasePoints = effectMaxPoints;
                else 
                    _EffectBasePoints = Math.Min(EffectBasePoints, effectMaxPoints);
            }

            // Scale the value if it's a controlled type
            if (valueScalingFormulaName.Length > 0)
            {
                float beforeValue = _EffectBasePoints;
                if (valueScalingFormulaName.Contains("overtime"))
                    beforeValue = beforeValue / Convert.ToSingle(Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW);
                else if (valueScalingFormulaName.Contains("dps") || valueScalingFormulaName.Contains("hps"))
                    beforeValue = beforeValue / Math.Max((Convert.ToSingle(spellCastTimeInMS) * 0.001f), 1f); // No lower than 1 second for the calculation
                float afterValue = GetConvertedEqValueToWowValue(valueScalingFormulaName, beforeValue);
                float calculatedNewValue = Convert.ToSingle(_EffectBasePoints) * (afterValue / beforeValue);
                _EffectBasePoints = Math.Max(Convert.ToInt32(calculatedNewValue), 1);
                    
            }

            // Reverse the sign
            if (effectBasePointsWasNegative == true)
                _EffectBasePoints *= -1;
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

        private static void PopulateEffectValueBaselines()
        {
            string spellEffectValueBaselineFileName = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "SpellEffectValueBaselines.csv");
            Logger.WriteDebug("Populating Spell Effect Value Baselines list via file '" + spellEffectValueBaselineFileName + "'");
            List<string> spellEffectValueBaselineRows = FileTool.ReadAllStringLinesFromFile(spellEffectValueBaselineFileName, false, true);
            bool isFirstRow = true;
            List<string> effectValues = new List<string>();
            foreach (string row in spellEffectValueBaselineRows)
            {
                // Load the row
                string[] rowBlocks = row.Split("|");

                // If it's the first row, build out the effect values
                if (isFirstRow == true)
                {
                    foreach (string block in rowBlocks)
                    {
                        string lowerText = block.Trim().ToLower();
                        if (lowerText == "effect")
                            continue;
                        effectValues.Add(lowerText);
                    }

                    isFirstRow = false;
                    continue;
                }

                // Otherwise, load the effect values
                string effect = rowBlocks[0].Trim().ToLower();
                EffectValueBaselinesByEffectAndValueType.Add(effect, new Dictionary<string, float>());
                for (int i = 1; i < rowBlocks.Count(); i++)
                    EffectValueBaselinesByEffectAndValueType[effect].Add(effectValues[i - 1], float.Parse(rowBlocks[i]));
            }
        }

        private static float GetConvertedEqValueToWowValue(string effectName, float eqEffectValue)
        {
            // Read the file if haven't yet
            if (EffectValueBaselinesByEffectAndValueType.Count() == 0)
                PopulateEffectValueBaselines();

            // Get the effect values row row
            string effectNameLower = effectName.ToLower();
            if (EffectValueBaselinesByEffectAndValueType.ContainsKey(effectNameLower) == false)
            {
                Logger.WriteError("Could not pull effect value for effect '" + effectNameLower + "' as that effect wasn't in the SpellEffectValueBaselines");
                return eqEffectValue;
            }
            Dictionary<string, float> valuesForEffectType = EffectValueBaselinesByEffectAndValueType[effectNameLower];

            // Extract the values
            float valueEqLow = valuesForEffectType["eqlow"];
            float valueEqHigh = valuesForEffectType["eqhigh"];
            float valueWowLow = valuesForEffectType["wowlow"];
            float valueWoWHigh = valuesForEffectType["wowhigh60"];

            // Perform no calculation if any are 0
            if (valueEqLow == 0 || valueEqHigh == 0 || valueWowLow == 0 || valueWoWHigh == 0)
            {
                Logger.WriteDebug("Could not pull effect value for effect '" + effectNameLower + "' as that effect had a value with 0");
                return eqEffectValue;
            } 

            // Only operate with positive numbers
            bool flipValueSign = false;
            if (eqEffectValue < 0)
            {
                flipValueSign = true;
                eqEffectValue *= -1;
            }

            // Set a floor on the value
            eqEffectValue = MathF.Max(eqEffectValue, valueEqLow);

            // Calculate the value
            float normalizedModOfHigh = ((eqEffectValue - valueEqLow) / (valueEqHigh - valueEqLow));
            float calcBiasFactor = 1;
            if (normalizedModOfHigh < 1)
                calcBiasFactor = ((1 - normalizedModOfHigh) * (Configuration.SPELL_EFFECT_VALUE_LOW_BIAS_WEIGHT - 1)) + 1;
            float biasedNormalizedModOfHigh = normalizedModOfHigh / calcBiasFactor;
            float calculatedValue = (biasedNormalizedModOfHigh * valueWoWHigh) + ((1 - biasedNormalizedModOfHigh) * valueWowLow);

            // Flip the sign if needed
            if (flipValueSign == true)
                calculatedValue *= -1;

            return calculatedValue;
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

        public void OverrideBasePoints(int basePoints)
        {
            _EffectBasePoints = basePoints;
        }
    }
}
