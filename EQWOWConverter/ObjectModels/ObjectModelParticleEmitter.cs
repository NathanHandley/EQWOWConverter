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

using EQWOWConverter.EQFiles;
using EQWOWConverter.Spells;

namespace EQWOWConverter.ObjectModels
{
    internal class ObjectModelParticleEmitter
    {
        public string SpriteFileNameNoExt = string.Empty;
        public SpellEmitterModelAttachLocationType EmissionLocation = SpellEmitterModelAttachLocationType.Chest;
        public SpellVisualEmitterSpawnPatternType EmissionPattern = SpellVisualEmitterSpawnPatternType.None;
        public float Gravity = 0;
        public int LifespanInMS = 0;
        public float Scale = 0;
        public float Velocity = 0;
        public int SpawnRate = 0;
        public float Radius = 0;
        public int TextureID = 0;

        public void Load(EQSpellsEFF.SectionData effectSection, int effectIndex)
        {
            // Calculate the location and pattern first since those are used in further calculations.
            EmissionLocation = GetEmissionAttachLocation(effectSection, effectIndex);
            EmissionPattern = GetEmissionSpawnPattern(effectSection, effectIndex);

            // Velocity
            Velocity = CalculateVelocity(effectSection, effectIndex, EmissionPattern);

            // Lifespan
            LifespanInMS = CalculateLifespanInMS(effectSection, effectIndex);

            // Radius
            Radius = CalculateRadius(effectSection, effectIndex, EmissionPattern);
            
            // Convert values
            SpriteFileNameNoExt = effectSection.SpriteNames[effectIndex].Replace("_SPRITE", "");
            
            // Convert feet to meters
            float feetToMeterMod = 0.3048f;
            Gravity = effectSection.EmitterGravities[effectIndex] * feetToMeterMod;

            // Factor world scale for spell scale
            Scale = (effectSection.EmitterSpawnScale[effectIndex] * Configuration.GENERATE_WORLD_SCALE);
            
            // Spawn rate
            SpawnRate = CalculateSpawnRate(effectSection, effectIndex, EmissionPattern);
        }

        private float CalculateRadius(EQSpellsEFF.SectionData effectSection, int effectIndex, SpellVisualEmitterSpawnPatternType emissionPattern)
        {
            float eqRadius = effectSection.EmitterSpawnRadii[effectIndex];

            // Enforce any defaults
            if (eqRadius == 0)
            {
                if (emissionPattern == SpellVisualEmitterSpawnPatternType.SphereAroundUnit)
                    eqRadius = 6;
                else if (emissionPattern == SpellVisualEmitterSpawnPatternType.DiscAroundUnitCenter)
                    eqRadius = 6;
            }

            // Scale against the world
            return eqRadius * Configuration.GENERATE_WORLD_SCALE;
        }

        private int CalculateSpawnRate(EQSpellsEFF.SectionData effectSection, int effectIndex, SpellVisualEmitterSpawnPatternType emissionPattern)
        {
            switch (emissionPattern)
            {
                case SpellVisualEmitterSpawnPatternType.FromHands: return 25;
                case SpellVisualEmitterSpawnPatternType.SphereAroundUnit: return 60;
                default: return effectSection.EmitterSpawnRates[effectIndex];
            }
        }

        private float CalculateVelocity(EQSpellsEFF.SectionData effectSection, int effectIndex, SpellVisualEmitterSpawnPatternType spawnPattern)
        {
            float sourceVelocity = effectSection.EmitterSpawnVelocities[effectIndex] * Configuration.GENERATE_WORLD_SCALE;

            switch (spawnPattern)
            {
                case SpellVisualEmitterSpawnPatternType.FromHands:
                    {
                        if (sourceVelocity == 0)
                            return -1f; // This is about right
                        else
                            return 0f;
                    }
                case SpellVisualEmitterSpawnPatternType.SphereAroundUnit: // fallthrough
                default:return sourceVelocity;
            }
        }

        private int CalculateLifespanInMS(EQSpellsEFF.SectionData effectSection, int effectIndex)
        {
            // Default to a second if no lifespan
            if (effectSection.EmitterSpawnLifespans[effectIndex] == 0)
                return 1000;

            // Default to what's provided
            else
                return effectSection.EmitterSpawnLifespans[effectIndex];
        }

        private SpellVisualEmitterSpawnPatternType GetEmissionSpawnPattern(EQSpellsEFF.SectionData effectSection, int effectIndex)
        {
            switch (effectSection.EmissionTypeIDs[effectIndex])
            {
                case 0: return SpellVisualEmitterSpawnPatternType.FromHands;
                case 1: return SpellVisualEmitterSpawnPatternType.ConeToRight;
                case 2: return SpellVisualEmitterSpawnPatternType.SphereAroundUnit;
                case 3: return SpellVisualEmitterSpawnPatternType.DiscOnGround;
                case 4: return SpellVisualEmitterSpawnPatternType.ColumnFromGround;
                case 5: return SpellVisualEmitterSpawnPatternType.DiscAroundUnitCenter;
                default: return SpellVisualEmitterSpawnPatternType.None;
            }
        }

        private SpellEmitterModelAttachLocationType GetEmissionAttachLocation(EQSpellsEFF.SectionData effectSection, int effectIndex)
        {
            // Emission type overrides any setting on attach location
            switch (effectSection.EmissionTypeIDs[effectIndex])
            {
                case 0: return SpellEmitterModelAttachLocationType.Hands;
                case 3: return SpellEmitterModelAttachLocationType.Feet;
                case 4: return SpellEmitterModelAttachLocationType.Feet;
                default: break;
            }

            // Otherwise, use location ID
            switch (effectSection.LocationIDs[effectIndex])
            {
                case 0: return SpellEmitterModelAttachLocationType.Chest;
                case 1: return SpellEmitterModelAttachLocationType.Head;
                case 2: return SpellEmitterModelAttachLocationType.Hands;
                case 3: return SpellEmitterModelAttachLocationType.Hands;
                case 4: return SpellEmitterModelAttachLocationType.Feet;
                case 5: return SpellEmitterModelAttachLocationType.Feet;
                default: return SpellEmitterModelAttachLocationType.Chest;
            }
        }
    }
}
