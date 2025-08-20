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

using System.Text;

namespace EQWOWConverter.Spells
{
    internal class SpellEffectWOW : IComparable<SpellEffectWOW>
    {
        private static Dictionary<string, Dictionary<string, float>> EffectValueBaselinesByEffectAndValueType = new Dictionary<string, Dictionary<string, float>>();

        public SpellWOWEffectType EffectType = SpellWOWEffectType.None;
        public Int32 EffectDieSides = 0;
        public float EffectRealPointsPerLevel = 0;
        public int EffectBasePoints = 0;
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
        public int CalcEffectLowLevelValue = 0;
        public int CalcEffectLowLevel = 0;
        public int CalcEffectHighLevelValue = 0;
        public int CalcEffectHighLevel = 0;

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
                AuraDescription = this.AuraDescription,
                CalcEffectLowLevelValue = this.CalcEffectLowLevelValue,
                CalcEffectLowLevel = this.CalcEffectLowLevel,
                CalcEffectHighLevelValue = this.CalcEffectHighLevelValue,
                CalcEffectHighLevel = this.CalcEffectHighLevel
            };
        }

        public int GetEffectAmountValueByLevel(int inputEffectBasePoints, int inputEffectMaxPoints, int spellInfluencingLevel, int unitInfluencingLevel, 
            SpellEQBaseValueFormulaType eqFormula, int spellCastTimeInMS, string valueScalingFormulaName, SpellEffectWOWConversionScaleType conversionScaleType)
        {
            // Only work with positive values
            bool effectBasePointsWasNegative = false;
            if (inputEffectBasePoints < 0)
            {
                effectBasePointsWasNegative = true;
                inputEffectBasePoints *= -1;
            }
            if (inputEffectMaxPoints < 0)
                inputEffectMaxPoints *= -1;

            // Run base through a formula using a calculated value of the supplied spell level, instead of the player level
            int calculatedEffectBasePoints = Math.Abs(inputEffectBasePoints);
            switch (eqFormula)
            {
                case SpellEQBaseValueFormulaType.BaseDivideBy100: calculatedEffectBasePoints = inputEffectBasePoints / 100; break;
                case SpellEQBaseValueFormulaType.BaseAddLevel: calculatedEffectBasePoints += unitInfluencingLevel; break;
                case SpellEQBaseValueFormulaType.BaseAddLevelTimesTwo: calculatedEffectBasePoints += (unitInfluencingLevel * 2); break;
                case SpellEQBaseValueFormulaType.BaseAddLevelTimesThree: calculatedEffectBasePoints += (unitInfluencingLevel * 3); break;
                case SpellEQBaseValueFormulaType.BaseAddLevelTimesFour: calculatedEffectBasePoints += (unitInfluencingLevel * 4); break;
                case SpellEQBaseValueFormulaType.BaseAddLevelDivideTwo: calculatedEffectBasePoints += Convert.ToInt32(Convert.ToSingle(unitInfluencingLevel) * 0.5f); break;
                case SpellEQBaseValueFormulaType.BaseAddLevelDivideThree: calculatedEffectBasePoints += Convert.ToInt32(Convert.ToSingle(unitInfluencingLevel) * 0.3333f); break;
                case SpellEQBaseValueFormulaType.BaseAddLevelDivideFour: calculatedEffectBasePoints += Convert.ToInt32(Convert.ToSingle(unitInfluencingLevel) * 0.25f); break;
                case SpellEQBaseValueFormulaType.BaseAddLevelDivideFive: calculatedEffectBasePoints += Convert.ToInt32(Convert.ToSingle(unitInfluencingLevel) * 0.20f); break;
                case SpellEQBaseValueFormulaType.BaseAddLevelDivideEight: calculatedEffectBasePoints += Convert.ToInt32(Convert.ToSingle(unitInfluencingLevel) * 0.125f); break;
                case SpellEQBaseValueFormulaType.BaseAddSixTimesLevelMinusSpellLevel: calculatedEffectBasePoints += 6 * (unitInfluencingLevel - spellInfluencingLevel); break;
                case SpellEQBaseValueFormulaType.BaseAddEightTimesLevelMinusSpellLevel: calculatedEffectBasePoints += 8 * (unitInfluencingLevel - spellInfluencingLevel); break;
                case SpellEQBaseValueFormulaType.BaseAddTenTimesLevelMinusSpellLevel: calculatedEffectBasePoints += 10 * (unitInfluencingLevel - spellInfluencingLevel); break;
                case SpellEQBaseValueFormulaType.BaseAddFifteenTimesLevelMinusSpellLevel: calculatedEffectBasePoints += 15 * (unitInfluencingLevel - spellInfluencingLevel); break;
                case SpellEQBaseValueFormulaType.BaseAddTwelveTimesLevelMinusSpellLevel: calculatedEffectBasePoints += 15 * (unitInfluencingLevel - spellInfluencingLevel); break;
                case SpellEQBaseValueFormulaType.BaseAddTwentyTimesLevelMinusSpellLevel: calculatedEffectBasePoints += 20 * (unitInfluencingLevel - spellInfluencingLevel); break;
                default: calculatedEffectBasePoints = Math.Max(inputEffectBasePoints, inputEffectMaxPoints); break;
            }

            // Enforce a maximum if it's set
            if (inputEffectMaxPoints != 0)
                calculatedEffectBasePoints = Math.Min(calculatedEffectBasePoints, inputEffectMaxPoints);

            // Scale the value if it's a controlled type
            if (valueScalingFormulaName.Length > 0)
            {
                float beforeValue = calculatedEffectBasePoints;
                if (conversionScaleType == SpellEffectWOWConversionScaleType.Periodic)
                    beforeValue /= Convert.ToSingle(Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_EQ);
                float normalizeMod = 0;
                if (conversionScaleType == SpellEffectWOWConversionScaleType.CastTime)
                {
                    // No lower than 1 second for the calculation
                    normalizeMod = 1 / Math.Max((Convert.ToSingle(spellCastTimeInMS) * 0.001f), 1f);
                    beforeValue *= normalizeMod;
                }
                float afterValue = GetConvertedEqValueToWowValue(valueScalingFormulaName, beforeValue);
                if (conversionScaleType == SpellEffectWOWConversionScaleType.CastTime)
                    afterValue /= normalizeMod;
                if (conversionScaleType == SpellEffectWOWConversionScaleType.Periodic)
                    afterValue *= Convert.ToSingle(Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW);
                calculatedEffectBasePoints = Math.Max(Convert.ToInt32(afterValue), 1);
            }

            // Reverse the sign, if needed
            if (effectBasePointsWasNegative == true)
                calculatedEffectBasePoints *= -1;
            return calculatedEffectBasePoints;
        }

        public void SetEffectAmountValues(int effectBasePoints, int effectMaxPoints, int spellLevel, SpellEQBaseValueFormulaType eqFormula, 
            int spellCastTimeInMS, string valueScalingFormulaName, SpellEffectWOWConversionScaleType conversionScaleType)
        {
            // Normalize the formula name
            valueScalingFormulaName = valueScalingFormulaName.ToLower().Trim();

            // Avoid underlevel calculations
            if (spellLevel < 0)
                spellLevel = 1;

            // Run the calculation on both ends of the level band, if relevant
            EffectRealPointsPerLevel = 0;
            if (eqFormula == SpellEQBaseValueFormulaType.BaseDivideBy100 || eqFormula == SpellEQBaseValueFormulaType.UnknownUseBaseOrMaxWhicheverHigher || Configuration.SPELL_EFFECT_USE_DYNAMIC_EFFECT_VALUES == false)
            {
                EffectBasePoints = GetEffectAmountValueByLevel(effectBasePoints, effectMaxPoints, spellLevel, spellLevel, eqFormula, spellCastTimeInMS,
                    valueScalingFormulaName, conversionScaleType);                
                CalcEffectLowLevelValue = EffectBasePoints;
                CalcEffectLowLevel = spellLevel;
                CalcEffectHighLevelValue = EffectBasePoints;                
                CalcEffectHighLevel = spellLevel;                
            }
            else
            {
                int calcMaxPoints = GetEffectAmountValueByLevel(effectMaxPoints, effectMaxPoints, spellLevel, spellLevel, eqFormula, spellCastTimeInMS,
                    valueScalingFormulaName, conversionScaleType);
                int endCalcLevel = Configuration.SPELL_EFFECT_CALC_STATS_FOR_MAX_LEVEL;
                EffectBasePoints = GetEffectAmountValueByLevel(effectBasePoints, effectMaxPoints, spellLevel, spellLevel, eqFormula, spellCastTimeInMS,
                    valueScalingFormulaName, conversionScaleType);
                CalcEffectLowLevelValue = EffectBasePoints;
                CalcEffectLowLevel = spellLevel;

                // Only calculate the larger band if the two levels aren't the same
                if (endCalcLevel <= spellLevel || EffectBasePoints == calcMaxPoints)
                {
                    CalcEffectHighLevelValue = EffectBasePoints;
                    CalcEffectHighLevel = spellLevel;
                }
                else
                {
                    // Calculate for every level in the gap as a spread until a max is hit
                    for (int curCalcLevel = spellLevel+1; curCalcLevel <= endCalcLevel; curCalcLevel++)
                    {
                        CalcEffectHighLevel = curCalcLevel;
                        CalcEffectHighLevelValue = GetEffectAmountValueByLevel(effectBasePoints, effectMaxPoints, spellLevel, curCalcLevel, eqFormula, spellCastTimeInMS,
                            valueScalingFormulaName, conversionScaleType);
                        if (CalcEffectHighLevelValue == calcMaxPoints)
                            curCalcLevel = endCalcLevel + 1;
                    }

                    float totalLevelSteps = CalcEffectHighLevel - spellLevel;
                    EffectRealPointsPerLevel = (Convert.ToSingle(CalcEffectHighLevelValue) - Convert.ToSingle(EffectBasePoints)) / totalLevelSteps;
                }
            }
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
            if (Configuration.SPELL_EFFECT_BALANCE_LEVEL_USE_60_VERSION == false)
                valueWoWHigh = valuesForEffectType["wowhigh80"];

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

        public string GetFormattedEffectActionString(bool addPercentSymbol)
        {
            StringBuilder stringBuilder = new StringBuilder();

            int lowValue = Math.Abs(CalcEffectLowLevelValue);
            int highValue = Math.Abs(CalcEffectHighLevelValue);

            // Simple return, no level difference
            if (lowValue == highValue)
            {
                stringBuilder.Append(lowValue);
                if (addPercentSymbol == true)
                    stringBuilder.Append("%");
            }

            // Band values
            else
            {
                stringBuilder.Append(lowValue.ToString());
                if (addPercentSymbol == true)
                    stringBuilder.Append("%");
                stringBuilder.Append(" (L");
                stringBuilder.Append(CalcEffectLowLevel.ToString());
                stringBuilder.Append(") to ");
                stringBuilder.Append(highValue.ToString());
                if (addPercentSymbol == true)
                    stringBuilder.Append("%");
                stringBuilder.Append(" (L");
                stringBuilder.Append(CalcEffectHighLevel.ToString());
                stringBuilder.Append(")");
            }

            return stringBuilder.ToString();
        }

        public string GetFormattedEffectAuraString(bool addPercentSymbol, string leadinTextIfSingleValue,
            string leadinTextIfBandValue)
        {
            // Nothing will get returned if there is a range/band
            if (CalcEffectLowLevelValue != CalcEffectHighLevelValue)
                return string.Empty;

            StringBuilder stringBuilder = new StringBuilder();
            int lowValue = Math.Abs(CalcEffectLowLevelValue);
            stringBuilder.Append(leadinTextIfSingleValue);
            stringBuilder.Append(lowValue);
            if (addPercentSymbol == true)
                stringBuilder.Append("%");

            return stringBuilder.ToString();
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
            EffectBasePoints = basePoints;
        }
    }
}
