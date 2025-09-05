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
    internal struct SpellDuration : IEquatable<SpellDuration>
    {
        public int BaseDurationInMS = 0;
        public int DurationInMSPerLevel = 0;
        public int MaxDurationInMS = 0;
        public int MinLevel = 0;
        public int MaxLevel = 0;
        public bool IsInfinite = false;

        public SpellDuration() { }

        public int GetBuffDurationForLevel(int level)
        {
            if (level == 0 || level >= MaxLevel)
                return MaxDurationInMS;
            if (level <= MinLevel)
                return BaseDurationInMS;
            int calcMSForLevel = BaseDurationInMS + ((MinLevel - level) * DurationInMSPerLevel);
            if (calcMSForLevel > MaxDurationInMS)
                return MaxDurationInMS;
            else
                return calcMSForLevel;
        }

        public void SetFixedDuration(int durationInMS)
        {
            BaseDurationInMS = durationInMS;
            MaxDurationInMS = durationInMS;
            DurationInMSPerLevel = 0;
            MinLevel = 0;
            MaxLevel = 0;
        }

        public void CalculateAndSetAuraDuration(int spellLevel, int eqBuffDurationFormula, int maxBuffDurationInTicks,
            bool isModelChangeSize, bool isBardSongCastersAura)
        {
            // Default for model change size spells
            if (eqBuffDurationFormula == 0 && isModelChangeSize == true)
            {
                BaseDurationInMS = Configuration.SPELL_MODEL_SIZE_CHANGE_EFFECT_DEFAULT_TIME_IN_MS;
                MaxDurationInMS = Configuration.SPELL_MODEL_SIZE_CHANGE_EFFECT_DEFAULT_TIME_IN_MS;
                return;
            }

            if (eqBuffDurationFormula <= 0)
                return;

            // Normalize the max duration into MS
            int maxBuffDurationInMS = maxBuffDurationInTicks * Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_EQ * 1000;

            // Prevent underflow on level
            if (spellLevel < 1)
                spellLevel = 1;

            // Handle population based on the type of formula
            if (isBardSongCastersAura == true)
            {
                IsInfinite = true;
                return;
            }
            else if (eqBuffDurationFormula == 3600)
            {
                int calcMS = 21600000; // 6 hours is the default
                if (maxBuffDurationInMS != 0)
                    calcMS = maxBuffDurationInMS * Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_EQ * 1000;
                BaseDurationInMS = calcMS;
                MaxDurationInMS = calcMS;
            }
            else if (eqBuffDurationFormula == 4)
            {
                // 50 ticks or max
                int calcMS = 50 * Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_EQ * 1000;
                if (maxBuffDurationInMS > 0)
                    calcMS = Math.Max(calcMS, maxBuffDurationInMS);
                BaseDurationInMS = calcMS;
                MaxDurationInMS = calcMS;
            }
            else if (eqBuffDurationFormula == 5)
            {
                // 2 ticks or max
                int calcMS = 2 * Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_EQ * 1000;
                if (maxBuffDurationInMS > 0)
                    calcMS = Math.Max(calcMS, maxBuffDurationInMS);
                BaseDurationInMS = calcMS;
                MaxDurationInMS = calcMS;
            }
            else if (eqBuffDurationFormula == 50)
            {
                IsInfinite = true;
                return;
            }
            else
            {
                if (maxBuffDurationInTicks > 0)
                    MaxDurationInMS = maxBuffDurationInMS;
                // These are dynamic formulas
                int endCalcLevel = Configuration.SPELL_EFFECT_CALC_STATS_FOR_MAX_LEVEL;
                BaseDurationInMS = CalcAuraDurationInMSForLevel(spellLevel, eqBuffDurationFormula);
                if (MaxDurationInMS > 0 && BaseDurationInMS > MaxDurationInMS)
                    BaseDurationInMS = MaxDurationInMS;
                if (BaseDurationInMS < MaxDurationInMS)
                {
                    MinLevel = spellLevel;
                    for (int curCalcLevel = spellLevel + 1; curCalcLevel <= endCalcLevel; curCalcLevel++)
                    {
                        MaxLevel = curCalcLevel;
                        MaxDurationInMS = CalcAuraDurationInMSForLevel(curCalcLevel, eqBuffDurationFormula);
                        if (maxBuffDurationInMS > 0)
                            MaxDurationInMS = Math.Min(MaxDurationInMS, maxBuffDurationInMS);
                        if (MaxDurationInMS == maxBuffDurationInMS)
                            curCalcLevel = endCalcLevel + 1;
                    }

                    int totalLevelSteps = MaxLevel - spellLevel;
                    DurationInMSPerLevel = (MaxDurationInMS - BaseDurationInMS) / totalLevelSteps;
                }
            }
        }

        private int CalcAuraDurationInMSForLevel(int curLevel, int eqBuffDurationFormula)
        {
            // Different logic per formula
            int curDurationInTicks = 0;
            switch (eqBuffDurationFormula)
            {
                case 1: curDurationInTicks = curLevel / 2; break;
                case 2:
                    {
                        if (curLevel > 3)
                            curDurationInTicks = (curLevel / 2) + 5;
                        else
                            curDurationInTicks = 6;
                    }
                    break;
                case 3: curDurationInTicks = 30 * curLevel; break;
                case 6: curDurationInTicks = (curLevel / 2) + 2; break;
                case 7: curDurationInTicks = curLevel; break;
                case 8: curDurationInTicks = curLevel + 10; break;
                case 9: curDurationInTicks = (curLevel * 2) + 10; break;
                case 10: curDurationInTicks = (curLevel * 3) + 10; break;
                case 11: curDurationInTicks = ((curLevel + 3) * 30); break;
                case 50: curDurationInTicks = 0; break; // Unlimited
                default:
                    {
                        Logger.WriteError("Unhandled type of eqBuffDurationFormula of ", eqBuffDurationFormula.ToString());
                        return 0;
                    }
            }
            return curDurationInTicks * 1000 * Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_EQ;
        }

        public string GetTimeText()
        {
            StringBuilder timeSB = new StringBuilder();

            timeSB.Append(GetTimeTextFragmentForMS(BaseDurationInMS));

            if (Configuration.SPELL_EFFECT_USE_DYNAMIC_AURA_DURATIONS == true 
                && MinLevel != MaxLevel
                && DurationInMSPerLevel != 0
                && MaxDurationInMS != BaseDurationInMS)
            {
                timeSB.Append(" (L");
                timeSB.Append(MinLevel.ToString());
                timeSB.Append(") to ");
                timeSB.Append(GetTimeTextFragmentForMS(MaxDurationInMS));
                timeSB.Append(" (L");
                timeSB.Append(MaxLevel.ToString());
                timeSB.Append(")");
            }

            return timeSB.ToString();
        }

        private string GetTimeTextFragmentForMS(int ms)
        {
            int inputSeconds = ms / 1000;
            if (inputSeconds <= 0)
                return string.Empty;

            StringBuilder timeSB = new StringBuilder();
            int hours = inputSeconds / 3600;
            if (hours > 0)
            {
                timeSB.Append(hours);
                timeSB.Append(" hour");
                if (hours != 1)
                    timeSB.Append("s");
            }
            int minutes = (inputSeconds % 3600) / 60;
            if (minutes > 0)
            {
                if (hours > 0)
                    timeSB.Append(" ");
                timeSB.Append(minutes);
                timeSB.Append(" minute");
                if (minutes != 1)
                    timeSB.Append("s");
            }
            int seconds = (inputSeconds % 60);
            if (seconds > 0)
            {
                if (hours > 0 || minutes > 0)
                    timeSB.Append(" ");
                timeSB.Append(seconds);
                timeSB.Append(" second");
                if (seconds != 1)
                    timeSB.Append("s");
            }
            return timeSB.ToString();
        }

        public override bool Equals(object? obj)
        {
            return obj is SpellDuration other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BaseDurationInMS, DurationInMSPerLevel, MaxDurationInMS, MinLevel, MaxLevel);
        }

        public bool Equals(SpellDuration other)
        {
            return BaseDurationInMS == other.BaseDurationInMS
                && DurationInMSPerLevel == other.DurationInMSPerLevel
                && MaxDurationInMS == other.MaxDurationInMS
                && MinLevel == other.MinLevel
                && MaxLevel == other.MaxLevel;
        }

        public static bool operator ==(SpellDuration left, SpellDuration right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SpellDuration left, SpellDuration right)
        {
            return !left.Equals(right);
        }
    }
}
