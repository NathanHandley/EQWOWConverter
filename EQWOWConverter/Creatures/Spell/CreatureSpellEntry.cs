//  Author: Nathan Handley (nathanhandley@protonmail.com)
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

namespace EQWOWConverter.Creatures
{
    internal struct CreatureSpellEntry : IComparable<CreatureSpellEntry>
    {
        private static SortedDictionary<int, List<CreatureSpellEntry>> CreatureSpellEntriesByListID = new SortedDictionary<int, List<CreatureSpellEntry>>();
        private static readonly object CreatureSpellEntryLock = new object();
        public int ID;
        public int CreatureSpellListID;
        public int EQSpellID;
        public int TypeFlags;
        public int MinLevel;
        public int MaxLevel;
        public int ManaCost;
        public int OriginalRecastDelayInMS;
        public int CalculatedMinimumDelayInMS;
        public int BuffDurationInMS;
        public int Priority;

        public static SortedDictionary<int, List<CreatureSpellEntry>> GetCreatureSpellEntriesByListID()
        {
            lock (CreatureSpellEntryLock)
            {
                if (CreatureSpellEntriesByListID.Count == 0)
                    PopulateCreatureSpellEntries();
                return CreatureSpellEntriesByListID;
            }
        }

        private static void PopulateCreatureSpellEntries()
        {
            string spellEntriesFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "CreatureSpellEntries.csv");
            Logger.WriteDebug("Populating creature spell entries via file '" + spellEntriesFile + "'");
            List<Dictionary<string, string>> spellEntryRows = FileTool.ReadAllRowsFromFileWithHeader(spellEntriesFile, "|");
            foreach (Dictionary<string, string> columns in spellEntryRows)
            {
                // Skip any invalid rows
                int minExpansion = int.Parse(columns["min_expansion"]);
                if (minExpansion > Configuration.GENERATE_EQ_EXPANSION_ID_GENERAL)
                    continue;

                // Load the row
                CreatureSpellEntry newSpellEntry = new CreatureSpellEntry();
                newSpellEntry.ID = int.Parse(columns["id"]);
                newSpellEntry.CreatureSpellListID = int.Parse(columns["creature_spell_list_id"]);
                newSpellEntry.EQSpellID = int.Parse(columns["eq_spell_id"]);
                newSpellEntry.TypeFlags = int.Parse(columns["type"]);
                newSpellEntry.MinLevel = int.Parse(columns["minlevel"]);
                newSpellEntry.MaxLevel = int.Parse(columns["maxlevel"]);
                newSpellEntry.ManaCost = int.Parse(columns["manacost"]);
                newSpellEntry.OriginalRecastDelayInMS = int.Parse(columns["recast_delay"]) * 1000;
                if (newSpellEntry.OriginalRecastDelayInMS < 0)
                    newSpellEntry.OriginalRecastDelayInMS = 0;
                newSpellEntry.CalculatedMinimumDelayInMS = newSpellEntry.OriginalRecastDelayInMS;
                newSpellEntry.Priority = int.Parse(columns["priority"]);

                // Add it
                if (CreatureSpellEntriesByListID.ContainsKey(newSpellEntry.CreatureSpellListID) == false)
                    CreatureSpellEntriesByListID.Add(newSpellEntry.CreatureSpellListID, new List<CreatureSpellEntry>());
                CreatureSpellEntriesByListID[newSpellEntry.CreatureSpellListID].Add(newSpellEntry);
            }
        }

        // Try to match general cast priority picking as referenced from TAKP mob_ai.cpp AICastSpell
        public static int GetCombatSpellEventChance(int eqSpellTypeFlags, int priority)
        {
            // Priority 0 spells always cast (raid bosses have these)
            if (priority <= 0)
                return 100;

            // Non-nuke spellcast type chance (direct damage spells are always 100%)
            int typeChance = 100;
            if ((eqSpellTypeFlags & 4) == 4) typeChance = Configuration.CREATURE_SPELL_COMBAT_ROOT_CAST_CHANCE;
            else if ((eqSpellTypeFlags & 128) == 128) typeChance = Configuration.CREATURE_SPELL_COMBAT_SNARE_CAST_CHANCE;
            else if ((eqSpellTypeFlags & 256) == 256) typeChance = Configuration.CREATURE_SPELL_COMBAT_DOT_CAST_CHANCE;
            else if ((eqSpellTypeFlags & 512) == 512) typeChance = Configuration.CREATURE_SPELL_COMBAT_DISPEL_CAST_CHANCE;
            else if ((eqSpellTypeFlags & 2048) == 2048) typeChance = Configuration.CREATURE_SPELL_COMBAT_MEZ_CAST_CHANCE;
            else if ((eqSpellTypeFlags & 4096) == 4096) typeChance = Configuration.CREATURE_SPELL_COMBAT_CHARM_CAST_CHANCE;
            else if ((eqSpellTypeFlags & 8192) == 8192) typeChance = Configuration.CREATURE_SPELL_COMBAT_SLOW_CAST_CHANCE;
            else if ((eqSpellTypeFlags & 16384) == 16384) typeChance = Configuration.CREATURE_SPELL_COMBAT_DEBUFF_CAST_CHANCE;
            else if ((eqSpellTypeFlags & 64) == 64) typeChance = Configuration.CREATURE_SPELL_COMBAT_LIFETAP_CAST_CHANCE;

            // Priority-order preference to drop less priority ones off
            int priorityChance = 100;
            if (priority > Configuration.CREATURE_SPELL_COMBAT_PRIORITY_PRIMARY_THRESHOLD)
                priorityChance = Math.Max(Configuration.CREATURE_SPELL_COMBAT_PRIORITY_CHANCE_MIN, 100 - ((priority - Configuration.CREATURE_SPELL_COMBAT_PRIORITY_PRIMARY_THRESHOLD) * Configuration.CREATURE_SPELL_COMBAT_PRIORITY_CHANCE_STEP));
            return Math.Min(typeChance, priorityChance);
        }

        // TAKP's chance that one engaged autocast check casts this spell if it is off recast: the NPC's detrimental gate, then the spell type's roll
        public static float GetBossEQPassChancePerCheck(int eqSpellTypeFlags, int eqClass, int eqSpellCount)
        {
            // TAKP roll_mod: NPCs with more spells roll each one less often (dispel alone ignores it)
            int rollMod = 0;
            if (eqSpellCount < 4)
                rollMod = 10;
            else if (eqSpellCount > 9)
                rollMod = -10;
            else if (eqSpellCount > 6)
                rollMod = -5;

            // TAKP detrimental gate: hybrids (paladin 3, ranger 4, shadow knight 5, bard 8, beastlord 15) almost never cast detrimental spells
            int detrimentalChance = Configuration.CREATURE_SPELL_BOSS_EQ_DETRIMENTAL_CHANCE;
            if (eqClass == 3 || eqClass == 4 || eqClass == 5 || eqClass == 8 || eqClass == 15)
                detrimentalChance = Configuration.CREATURE_SPELL_BOSS_EQ_DETRIMENTAL_CHANCE_HYBRID;

            int typeChance = Configuration.CREATURE_SPELL_BOSS_EQ_NUKE_CAST_CHANCE;
            if ((eqSpellTypeFlags & 4) == 4) typeChance = Configuration.CREATURE_SPELL_COMBAT_ROOT_CAST_CHANCE;
            else if ((eqSpellTypeFlags & 128) == 128) typeChance = Configuration.CREATURE_SPELL_COMBAT_SNARE_CAST_CHANCE;
            else if ((eqSpellTypeFlags & 256) == 256) typeChance = Configuration.CREATURE_SPELL_COMBAT_DOT_CAST_CHANCE;
            else if ((eqSpellTypeFlags & 512) == 512) typeChance = Configuration.CREATURE_SPELL_COMBAT_DISPEL_CAST_CHANCE;
            else if ((eqSpellTypeFlags & 2048) == 2048) typeChance = Configuration.CREATURE_SPELL_COMBAT_MEZ_CAST_CHANCE;
            else if ((eqSpellTypeFlags & 4096) == 4096) typeChance = Configuration.CREATURE_SPELL_COMBAT_CHARM_CAST_CHANCE;
            else if ((eqSpellTypeFlags & 8192) == 8192) typeChance = Configuration.CREATURE_SPELL_COMBAT_SLOW_CAST_CHANCE;
            else if ((eqSpellTypeFlags & 16384) == 16384) typeChance = Configuration.CREATURE_SPELL_COMBAT_DEBUFF_CAST_CHANCE;
            else if ((eqSpellTypeFlags & 64) == 64) typeChance = Configuration.CREATURE_SPELL_COMBAT_LIFETAP_CAST_CHANCE;
            if ((eqSpellTypeFlags & 512) != 512)
                typeChance += rollMod;

            return (Math.Clamp(detrimentalChance, 1, 100) / 100f) * (Math.Clamp(typeChance, 1, 100) / 100f);
        }

        // Expected wait, on top of each spell's recast, that TAKP's autocast loop puts between a raid boss's non-priority-0 combat spells.  TAKP runs
        // one check about every 1.4 seconds while the NPC is idle (none while casting or during the recovery after a cast), rolls the detrimental
        // gate once per check and casts the FIRST eligible spell whose type roll passes, so a boss's spells share one gate instead of each rolling
        // on its own.  Treating them independently makes a boss with many spells chain cast.  Solved as a fixed point over: how often each spell is
        // off recast, the share of the gate each gets, and the fraction of time the NPC is idle.  Checked against a direct simulation of the TAKP
        // loop for Severilous (level 60 shaman list): one cast every ~14 seconds and ~36% of the time casting, where the simulation gives ~14 and ~38%
        public static int[] CalculateBossEQRollWaitsInMS(float[] passChancePerCheck, int[] recastDelayInMS, int[] castTimeInMS)
        {
            int spellCount = passChancePerCheck.Length;
            float[] waitsInMS = new float[spellCount];
            float[] availableFraction = new float[spellCount];
            for (int i = 0; i < spellCount; i++)
                availableFraction[i] = 1f;
            float busyFraction = 0f;
            float checkIntervalInMS = Configuration.CREATURE_SPELL_BOSS_EQ_AUTOCAST_CHECK_IN_MS;
            float postCastRecoveryInMS = Configuration.CREATURE_SPELL_BOSS_EQ_POST_CAST_RECOVERY_IN_MS;

            for (int iteration = 0; iteration < 200; iteration++)
            {
                // Chance that some spell casts on a check, spread across the spells in proportion to their own pass chance
                float noneCastChance = 1f;
                float passChanceSum = 0f;
                for (int i = 0; i < spellCount; i++)
                {
                    float weightedPassChance = availableFraction[i] * passChancePerCheck[i];
                    noneCastChance *= 1f - weightedPassChance;
                    passChanceSum += weightedPassChance;
                }
                float shareFactor = passChanceSum > 0f ? (1f - noneCastChance) / passChanceSum : 1f;
                float idleFraction = Math.Max(0.05f, 1f - busyFraction);

                float newBusyFraction = 0f;
                for (int i = 0; i < spellCount; i++)
                {
                    float effectivePassChance = Math.Max(0.0001f, passChancePerCheck[i] * shareFactor);
                    waitsInMS[i] = checkIntervalInMS * ((1f / effectivePassChance) - 1f) / idleFraction;
                    float cycleInMS = Math.Max(1f, recastDelayInMS[i] + waitsInMS[i]);
                    availableFraction[i] = waitsInMS[i] / cycleInMS;
                    newBusyFraction += (castTimeInMS[i] + postCastRecoveryInMS) / cycleInMS;
                }

                // Damped so the busy/idle feedback settles instead of oscillating
                busyFraction = (0.5f * busyFraction) + (0.5f * Math.Min(newBusyFraction, 0.95f));
            }

            int[] waitsOut = new int[spellCount];
            for (int i = 0; i < spellCount; i++)
                waitsOut[i] = Convert.ToInt32(Math.Min(waitsInMS[i], 3600000f));
            return waitsOut;
        }

        public int CompareTo(CreatureSpellEntry other)
        {
            // Proper way to do this is to sort by "Priority", however doing that will cause much less
            // spell variety in the case that a high priority spell with a faster recast delay will
            // almayst only get used after 1 step-through of the spell list (in WOW).  TODO: Customize
            // later to get better spell sorting.
            //return Priority.CompareTo(other.Priority);
            int minDelayComparison = CalculatedMinimumDelayInMS.CompareTo(other.CalculatedMinimumDelayInMS);
            if (minDelayComparison == 0)
                return Priority.CompareTo(other.Priority);
            else
                return minDelayComparison;
        }
    }
}
