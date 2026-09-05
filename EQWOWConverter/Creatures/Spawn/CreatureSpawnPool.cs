//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2024 Nathan Handley
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

using EQWOWConverter.Events;

namespace EQWOWConverter.Creatures
{
    internal class CreatureSpawnPool
    {
        public List<CreatureSpawnInstance> CreatureSpawnInstances = new List<CreatureSpawnInstance>();
        public List<CreatureTemplate> CreatureTemplates = new List<CreatureTemplate>();
        public List<int> CreatureTemplateChances = new List<int>();
        public int SpawnLimit = 0;
        private const int FULL_SPAWN_CHANCE = 100;
        public CreatureSpawnGroup SpawnGroup = new CreatureSpawnGroup();
        public GameEvent? LinkedSpawnGameEvent = null;
        public GameEvent? LinkedDespawnGameEvent = null;

        public CreatureSpawnPool(CreatureSpawnGroup creatureSpawnGroup)
        {
            SpawnLimit = creatureSpawnGroup.SpawnLimit;
            SpawnGroup = creatureSpawnGroup;
        }

        public void AddCreatureTemplate(CreatureTemplate creatureTemplate, int chance)
        {
            CreatureTemplates.Add(creatureTemplate);
            CreatureTemplateChances.Add(chance);
        }

        public CreatureSpawnPool? CreateCopyWithoutRaidCreatures()
        {
            return CreateCopyWithFilteredCreatures(false, true);
        }

        public CreatureSpawnPool? CreateCopyWithOnlyRaidCreatures()
        {
            return CreateCopyWithFilteredCreatures(true, true);
        }

        private CreatureSpawnPool? CreateCopyWithFilteredCreatures(bool keepRaidCreatures, bool rescaleChancesToFillPool)
        {
            CreatureSpawnPool filteredSpawnPool = new CreatureSpawnPool(SpawnGroup);
            filteredSpawnPool.SpawnLimit = SpawnLimit;
            filteredSpawnPool.LinkedSpawnGameEvent = LinkedSpawnGameEvent;
            filteredSpawnPool.LinkedDespawnGameEvent = LinkedDespawnGameEvent;
            filteredSpawnPool.CreatureSpawnInstances = new List<CreatureSpawnInstance>(CreatureSpawnInstances);
            for (int i = 0; i < CreatureTemplates.Count; i++)
            {
                if (CreatureTemplates[i].IsRaidCreature() != keepRaidCreatures)
                    continue;
                int chance = i < CreatureTemplateChances.Count ? CreatureTemplateChances[i] : 0;
                filteredSpawnPool.AddCreatureTemplate(CreatureTemplates[i], chance);
            }
            if (filteredSpawnPool.CreatureTemplates.Count == 0)
                return null;
            if (rescaleChancesToFillPool == true)
                filteredSpawnPool.RescaleCreatureTemplateChancesToFillPool();
            return filteredSpawnPool;
        }

        private void RescaleCreatureTemplateChancesToFillPool()
        {
            int totalChance = 0;
            foreach (int chance in CreatureTemplateChances)
                if (chance > 0)
                    totalChance += chance;
            if (totalChance <= 0 || totalChance >= FULL_SPAWN_CHANCE)
                return;

            int highestChanceIndex = -1;
            int assignedChance = 0;
            for (int i = 0; i < CreatureTemplateChances.Count; i++)
            {
                if (CreatureTemplateChances[i] <= 0)
                    continue;
                if (highestChanceIndex == -1 || CreatureTemplateChances[i] > CreatureTemplateChances[highestChanceIndex])
                    highestChanceIndex = i;
                CreatureTemplateChances[i] = Convert.ToInt32(Math.Floor(Convert.ToSingle(CreatureTemplateChances[i]) * Convert.ToSingle(FULL_SPAWN_CHANCE) / Convert.ToSingle(totalChance)));
                assignedChance += CreatureTemplateChances[i];
            }

            // Rounding loss goes to the most common candidate, so the pool always adds back up to a full roll
            if (highestChanceIndex != -1 && assignedChance < FULL_SPAWN_CHANCE)
                CreatureTemplateChances[highestChanceIndex] += FULL_SPAWN_CHANCE - assignedChance;
        }

        public int GetMaxSpawnCount()
        {
            if (SpawnLimit == 0)
                return CreatureSpawnInstances.Count;
            else
                return SpawnLimit;
        }

        public void AddSpawnInstance(CreatureSpawnInstance creatureSpawnInstance)
        {
            CreatureSpawnInstances.Add(creatureSpawnInstance);
        }

        public CreatureTemplate GetMostRareCreatureTemplate()
        {
            CreatureTemplate mostRareTemplate = CreatureTemplates[0];
            int mostRareChance = 0;
            for (int i = 0; i < CreatureTemplates.Count; i++)
            {
                int chance = i < CreatureTemplateChances.Count ? CreatureTemplateChances[i] : 0;
                if (chance <= 0)
                    continue;
                if (mostRareChance <= 0 || chance < mostRareChance)
                {
                    mostRareChance = chance;
                    mostRareTemplate = CreatureTemplates[i];
                }
            }
            return mostRareTemplate;
        }

        public bool DoChancesAddTo100()
        {
            int totalChance = 0;
            foreach (int chance in CreatureTemplateChances)
                totalChance += chance;
            if (totalChance == 100)
                return true;
            else
                return false;
        }

        public void BalanceChancesTo100()
        {
            int totalChance = 0;
            foreach (int chance in CreatureTemplateChances)
                totalChance += chance;
            int adjustmentAmount = 100 - totalChance;
            int adjustmentStep = 1;
            if (adjustmentAmount < 0)
                adjustmentStep = -1;
            while(true)
            {
                for (int i = 0; i < CreatureTemplateChances.Count; i++)
                {
                    if (CreatureTemplateChances[i] + adjustmentStep > 0)
                    {
                        CreatureTemplateChances[i] += adjustmentStep;
                        adjustmentAmount -= adjustmentStep;
                        if (adjustmentAmount == 0)
                            return;
                    }
                }
            }
        }
    }
}
