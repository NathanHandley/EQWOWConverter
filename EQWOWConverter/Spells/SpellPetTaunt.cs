//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2026 Nathan Handley
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

using EQWOWConverter.WOWFiles;

namespace EQWOWConverter.Spells
{
    internal class SpellPetTauntRank
    {
        public int Rank = 0;
        public int WOWSpellID = 0;
        public int LearnLevel = 0; // The level the Voidwalker learned this rank at, and the level a flagged EQ pet learns it at
        public int MaxScalingLevel = 0;
        public int ThreatAmount = 0;
        public float ThreatAmountPerLevel = 0f;
        public int ManaCost = 0;
        public int HitChanceReductionPercent = 0; // Only the later Suffering ranks carry this, and it's zero everywhere else

        public SpellPetTauntRank(int rank, int wowSpellID, int learnLevel, int maxScalingLevel, int threatAmount, float threatAmountPerLevel, int manaCost,
            int hitChanceReductionPercent)
        {
            Rank = rank;
            WOWSpellID = wowSpellID;
            LearnLevel = learnLevel;
            MaxScalingLevel = maxScalingLevel;
            ThreatAmount = threatAmount;
            ThreatAmountPerLevel = threatAmountPerLevel;
            ManaCost = manaCost;
            HitChanceReductionPercent = hitChanceReductionPercent;
        }
    }

    internal class SpellPetTaunt
    {
        public const int HIT_CHANCE_REDUCTION_DURATION_IN_MS = 15000; // Matches SpellDuration.dbc row 8, which the later Suffering ranks use

        private static readonly object SpellPetTauntLock = new object();
        private static List<SpellPetTauntRank> SingleTauntRanks = new List<SpellPetTauntRank>();
        private static List<SpellPetTauntRank> MultiTauntRanks = new List<SpellPetTauntRank>();

        // Cloned from Torment, whose ranks the Voidwalker picked up every ten levels starting at 10
        public static List<SpellPetTauntRank> GetSingleTauntRanks()
        {
            lock (SpellPetTauntLock)
            {
                if (SingleTauntRanks.Count == 0)
                {
                    int firstSpellID = Configuration.SPELL_PET_TAUNT_SPELL_ID_START;
                    SingleTauntRanks.Add(new SpellPetTauntRank(1, firstSpellID + 0, 10, 15, 45, 2f, 15, 0));
                    SingleTauntRanks.Add(new SpellPetTauntRank(2, firstSpellID + 1, 20, 25, 75, 2f, 30, 0));
                    SingleTauntRanks.Add(new SpellPetTauntRank(3, firstSpellID + 2, 30, 35, 125, 2f, 50, 0));
                    SingleTauntRanks.Add(new SpellPetTauntRank(4, firstSpellID + 3, 40, 45, 215, 2f, 70, 0));
                    SingleTauntRanks.Add(new SpellPetTauntRank(5, firstSpellID + 4, 50, 55, 300, 2f, 90, 0));
                    SingleTauntRanks.Add(new SpellPetTauntRank(6, firstSpellID + 5, 60, 65, 395, 2f, 110, 0));
                    SingleTauntRanks.Add(new SpellPetTauntRank(7, firstSpellID + 6, 70, 75, 632, 2f, 130, 0));
                    SingleTauntRanks.Add(new SpellPetTauntRank(8, firstSpellID + 7, 80, 85, 1175, 2f, 165, 0));
                }
                return SingleTauntRanks;
            }
        }

        // Cloned from Suffering, which the Voidwalker learned on its own uneven schedule, and which only starts reducing hit chance at rank 5
        public static List<SpellPetTauntRank> GetMultiTauntRanks()
        {
            lock (SpellPetTauntLock)
            {
                if (MultiTauntRanks.Count == 0)
                {
                    int firstSpellID = Configuration.SPELL_PET_AREATAUNT_SPELL_ID_START;
                    MultiTauntRanks.Add(new SpellPetTauntRank(1, firstSpellID + 0, 24, 34, 150, 0f, 100, 0));
                    MultiTauntRanks.Add(new SpellPetTauntRank(2, firstSpellID + 1, 36, 46, 300, 0f, 165, 0));
                    MultiTauntRanks.Add(new SpellPetTauntRank(3, firstSpellID + 2, 48, 58, 450, 0f, 230, 0));
                    MultiTauntRanks.Add(new SpellPetTauntRank(4, firstSpellID + 3, 60, 70, 600, 0f, 285, 0));
                    MultiTauntRanks.Add(new SpellPetTauntRank(5, firstSpellID + 4, 63, 70, 645, 2f, 305, 10));
                    MultiTauntRanks.Add(new SpellPetTauntRank(6, firstSpellID + 5, 69, 74, 885, 2f, 355, 10));
                    MultiTauntRanks.Add(new SpellPetTauntRank(7, firstSpellID + 6, 75, 79, 1400, 2f, 355, 10));
                    MultiTauntRanks.Add(new SpellPetTauntRank(8, firstSpellID + 7, 80, 84, 1675, 2f, 355, 10));
                }
                return MultiTauntRanks;
            }
        }

        public static int GetSingleTauntSkillLineID()
        {
            return IDGenerationTool.GenerateID("SkillLineID", "pettaunt");
        }

        public static int GetMultiTauntSkillLineID()
        {
            return IDGenerationTool.GenerateID("SkillLineID", "petareataunt");
        }

        public static int GetSingleTauntSpellCategoryID()
        {
            return SpellCategoryDBC.GenerateDBCID("pettaunt");
        }

        public static int GetMultiTauntSpellCategoryID()
        {
            return SpellCategoryDBC.GenerateDBCID("petareataunt");
        }
    }
}
