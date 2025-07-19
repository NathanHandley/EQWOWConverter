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
        public int VisualEffectIndex = 0;
        public float Gravity = 0;
        public int LifespanInMS = 0;
        public float Scale = 0;
        public float Velocity = 0;
        public int SpawnRate = 0;
        public float Radius = 0;
        public int TextureID = 0;

        public void Load(EQSpellsEFF.SectionData effectSection, int effectIndex, SpellVisualEmitterSpawnPatternType emitterPatternOverride = SpellVisualEmitterSpawnPatternType.None)
        {
            VisualEffectIndex = effectSection.VisualEffectIndex;

            // Calculate the location and pattern first since those are used in further calculations.
            if (emitterPatternOverride == SpellVisualEmitterSpawnPatternType.None)
                EmissionPattern = GetEmissionSpawnPattern(effectSection, effectIndex);
            else
                EmissionPattern = emitterPatternOverride;
            EmissionLocation = GetEmissionAttachLocation(effectSection, effectIndex, EmissionPattern);

            // Velocity
            Velocity = CalculateVelocity(effectSection, effectIndex, EmissionPattern);

            // Lifespan
            LifespanInMS = CalculateLifespanInMS(effectSection, effectIndex);

            // Radius
            Radius = CalculateRadius(effectSection, effectIndex, EmissionPattern);
            
            // Some sprits have _SPRITE and some don't, remove if they do
            SpriteFileNameNoExt = effectSection.SpriteNames[effectIndex].Replace("_SPRITE", "");

            // Gravity
            Gravity = CalculateGravity(effectSection, effectIndex);

            // Scale
            Scale = (effectSection.EmitterSpawnScale[effectIndex] * Configuration.SPELLS_EFFECT_PARTICLE_SIZE_SCALE_MOD);
            
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
            return eqRadius * Configuration.SPELLS_EFFECT_DISTANCE_SCALE_MOD;
        }

        private float CalculateGravity(EQSpellsEFF.SectionData effectSection, int effectIndex)
        {
            // TODO: Is there a default gravity for some emission patterns or locations, like "disc at player center"?
            // - See Spirit of Wolf (effect 7). It seems the particles should be traveling to the base of the feet despite
            // attaching to the body center
            return effectSection.EmitterGravities[effectIndex] * Configuration.SPELLS_EFFECT_DISTANCE_SCALE_MOD;
        }

        private int CalculateSpawnRate(EQSpellsEFF.SectionData effectSection, int effectIndex, SpellVisualEmitterSpawnPatternType emissionPattern)
        {
            int eqSpawnRate = effectSection.EmitterSpawnRates[effectIndex];
            int calcSpawnRate = eqSpawnRate;

            // Unsure why these minimums are needed, but many spells feel 'off' without them
            switch (emissionPattern)
            {
                case SpellVisualEmitterSpawnPatternType.FromHands: calcSpawnRate = Math.Max(eqSpawnRate, 25); break;
                case SpellVisualEmitterSpawnPatternType.SphereAroundUnit: calcSpawnRate = Math.Max(eqSpawnRate, 60); break;
                case SpellVisualEmitterSpawnPatternType.DiscAroundUnitCenter: calcSpawnRate = Math.Max(eqSpawnRate, 25); break;
                default: break;
            }

            return Convert.ToInt32(Convert.ToSingle(calcSpawnRate) * Configuration.SPELL_EMITTER_SPAWN_RATE_MOD);
        }

        private float CalculateVelocity(EQSpellsEFF.SectionData effectSection, int effectIndex, SpellVisualEmitterSpawnPatternType spawnPattern)
        {
            float sourceVelocity = effectSection.EmitterSpawnVelocities[effectIndex] * Configuration.SPELLS_EFFECT_DISTANCE_SCALE_MOD;

            switch (spawnPattern)
            {
                case SpellVisualEmitterSpawnPatternType.FromHands:
                    {
                        if (sourceVelocity == 0)
                            return -1; // Default does seem to be about 1 (negative for reversing direction)
                        else
                            return -1 * sourceVelocity;
                    }
                case SpellVisualEmitterSpawnPatternType.SphereAroundUnit: // fallthrough
                default:return sourceVelocity;
            }
        }

        private int CalculateLifespanInMS(EQSpellsEFF.SectionData effectSection, int effectIndex)
        {
            // Default to a second if no lifespan
            if (effectSection.EmitterSpawnLifespans[effectIndex] == 0)
                return Convert.ToInt32(1000f * Configuration.SPELLS_EFFECT_PARTICLE_LIFESPAN_TIME_MOD);

            // Default to what's provided
            else
                return Convert.ToInt32(Convert.ToSingle(effectSection.EmitterSpawnLifespans[effectIndex]) * Configuration.SPELLS_EFFECT_PARTICLE_LIFESPAN_TIME_MOD);
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

        private SpellEmitterModelAttachLocationType GetEmissionAttachLocation(EQSpellsEFF.SectionData effectSection, int effectIndex, 
            SpellVisualEmitterSpawnPatternType emissionPattern)
        {
            // Emission type overrides any setting on attach location
            switch (emissionPattern)
            {
                case SpellVisualEmitterSpawnPatternType.FromHands: return SpellEmitterModelAttachLocationType.Hands;
                case SpellVisualEmitterSpawnPatternType.ColumnFromGround: return SpellEmitterModelAttachLocationType.Feet;
                case SpellVisualEmitterSpawnPatternType.DiscOnGround: return SpellEmitterModelAttachLocationType.Feet;
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
