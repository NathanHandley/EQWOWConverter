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

using EQWOWConverter.Common;
using EQWOWConverter.EQFiles;
using EQWOWConverter.Spells;

namespace EQWOWConverter.ObjectModels
{
    internal class ObjectModelParticleEmitter
    {
        public string SpriteSheetFileNameNoExt = string.Empty;
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

        public void Load(EQSpellsEFF.EFFSourceSectionData effectSection, int effectIndex, SpellVisualEmitterSpawnPatternType emitterPatternOverride = SpellVisualEmitterSpawnPatternType.None)
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

            // Sprite sheet name
            SpriteSheetFileNameNoExt = GetSpriteSheetName(effectSection.SpriteNames[effectIndex], effectSection.EmitterColors[effectIndex]);

            // Gravity
            Gravity = CalculateGravity(effectSection, effectIndex, EmissionPattern);

            // Scale
            Scale = CalculateScale(effectSection, effectIndex);

            // Spawn rate
            SpawnRate = CalculateSpawnRate(effectSection, effectIndex, EmissionPattern);
        }

        private string GetSpriteSheetName(string eqSpellsEFFSpriteName, ColorRGBA colorRGBA)
        {
            if (eqSpellsEFFSpriteName.Length == 0)
                return string.Empty;

            string spriteSheetFileNameNoExt = eqSpellsEFFSpriteName;
            if ((colorRGBA.R != 0 && colorRGBA.R != 255) || (colorRGBA.G != 0 && colorRGBA.G != 255) || (colorRGBA.B != 0 && colorRGBA.B != 255))
            {
                string colorString = string.Concat("_c_", colorRGBA.R.ToString(), "_", colorRGBA.G.ToString(), "_", colorRGBA.B.ToString(), "_");
                spriteSheetFileNameNoExt = string.Concat(spriteSheetFileNameNoExt, colorString);
            }
            spriteSheetFileNameNoExt = string.Concat(spriteSheetFileNameNoExt, "Sheet");
            return spriteSheetFileNameNoExt;
        }

        private float CalculateScale(EQSpellsEFF.EFFSourceSectionData effectSection, int effectIndex)
        {
            float scale = Math.Max(effectSection.EmitterSpawnScale[effectIndex], Configuration.SPELLS_EFFECT_PARTICLE_SIZE_SCALE_MIN);
            return scale * Configuration.SPELLS_EFFECT_PARTICLE_SIZE_SCALE_MOD;
        }

        private float CalculateRadius(EQSpellsEFF.EFFSourceSectionData effectSection, int effectIndex, SpellVisualEmitterSpawnPatternType emissionPattern)
        {
            float eqRadius = effectSection.EmitterSpawnRadii[effectIndex];

            // Enforce any defaults
            if (eqRadius == 0)
            {
                if (emissionPattern == SpellVisualEmitterSpawnPatternType.SphereAroundUnit)
                    eqRadius = 6;
                else if (emissionPattern == SpellVisualEmitterSpawnPatternType.DiscAroundUnitCenter)
                    eqRadius = 8;
                else if (emissionPattern == SpellVisualEmitterSpawnPatternType.DiscOnGround)
                    eqRadius = 5;
                else if (emissionPattern == SpellVisualEmitterSpawnPatternType.SphereAwayFromPlayer)
                    eqRadius = 3;
            }

            // Scale against the world
            return eqRadius * Configuration.SPELLS_EFFECT_DISTANCE_SCALE_MOD;
        }

        private float CalculateGravity(EQSpellsEFF.EFFSourceSectionData effectSection, int effectIndex, SpellVisualEmitterSpawnPatternType emissionPattern)
        {
            float eqGravity = effectSection.EmitterGravities[effectIndex];

            // Disc's have a default gravity seemingly (see Spirit of Wolf (effect 7))
            if (emissionPattern == SpellVisualEmitterSpawnPatternType.DiscAroundUnitCenter && eqGravity == 0)
                eqGravity = 6;

            return eqGravity * Configuration.SPELLS_EFFECT_DISTANCE_SCALE_MOD;
        }

        private int CalculateSpawnRate(EQSpellsEFF.EFFSourceSectionData effectSection, int effectIndex, SpellVisualEmitterSpawnPatternType emissionPattern)
        {
            int spawnRate = effectSection.EmitterSpawnRates[effectIndex];

            // Defaults
            if (spawnRate == 0)
            {
                switch (emissionPattern)
                {
                    case SpellVisualEmitterSpawnPatternType.SphereAroundUnit: spawnRate = Configuration.SPELL_EMITTER_SPAWN_RATE_SPHERE_DEFAULT; break;
                    default: spawnRate = Configuration.SPELL_EMITTER_SPAWN_RATE_OTHER_DEFAULT; break;
                }
            }

            // Minimums
            switch (emissionPattern)
            {
                case SpellVisualEmitterSpawnPatternType.SphereAroundUnit: spawnRate = Math.Max(spawnRate, Configuration.SPELL_EMITTER_SPAWN_RATE_SPHERE_MINIMUM); break;
                case SpellVisualEmitterSpawnPatternType.FromHands: spawnRate = Math.Max(spawnRate, 25); break;
                case SpellVisualEmitterSpawnPatternType.DiscAroundUnitCenter: spawnRate = Math.Max(spawnRate, 25); break;
                default: break;
            }

            // Multipliers
            float spawnRateMod = 0;
            switch (emissionPattern)
            {
                case SpellVisualEmitterSpawnPatternType.SphereAroundUnit: spawnRateMod = Configuration.SPELL_EMITTER_SPAWN_RATE_SPHERE_MOD; break;
                case SpellVisualEmitterSpawnPatternType.DiscAroundUnitCenter: // fallthrough
                case SpellVisualEmitterSpawnPatternType.DiscOnGround: spawnRateMod = Configuration.SPELL_EMITTER_SPAWN_RATE_DISC_MOD; break;
                default: spawnRateMod = Configuration.SPELL_EMITTER_SPAWN_RATE_SPHERE_MOD; break;
            }

            return Convert.ToInt32(Convert.ToSingle(spawnRate) * spawnRateMod);
        }

        private float CalculateVelocity(EQSpellsEFF.EFFSourceSectionData effectSection, int effectIndex, SpellVisualEmitterSpawnPatternType spawnPattern)
        {
            float velocity = effectSection.EmitterSpawnVelocities[effectIndex];
            if (velocity == 0)
            {
                switch (spawnPattern)
                {
                    case SpellVisualEmitterSpawnPatternType.FromHands: velocity = 3f; break;
                    case SpellVisualEmitterSpawnPatternType.DiscAroundUnitCenter: velocity = 6f; break;
                    default: break;
                }
            }

            // Particles from hands or outer sphere need a reverse of their velocity
            if (spawnPattern == SpellVisualEmitterSpawnPatternType.FromHands)
                velocity *= -1;
            if (spawnPattern == SpellVisualEmitterSpawnPatternType.SphereAwayFromPlayer)
                velocity *= -1;

            return velocity * Configuration.SPELLS_EFFECT_DISTANCE_SCALE_MOD;
        }

        private int CalculateLifespanInMS(EQSpellsEFF.EFFSourceSectionData effectSection, int effectIndex)
        {
            // Default to a second if no lifespan
            if (effectSection.EmitterSpawnLifespans[effectIndex] == 0)
                return Convert.ToInt32(1000f * Configuration.SPELLS_EFFECT_PARTICLE_LIFESPAN_TIME_MOD);

            return Convert.ToInt32(Convert.ToSingle(effectSection.EmitterSpawnLifespans[effectIndex]) * Configuration.SPELLS_EFFECT_PARTICLE_LIFESPAN_TIME_MOD);
        }

        private SpellVisualEmitterSpawnPatternType GetEmissionSpawnPattern(EQSpellsEFF.EFFSourceSectionData effectSection, int effectIndex)
        {
            switch (effectSection.EmissionTypeIDs[effectIndex])
            {
                case 0: return SpellVisualEmitterSpawnPatternType.FromHands;
                case 1: return SpellVisualEmitterSpawnPatternType.SphereAwayFromPlayer;
                case 2: return SpellVisualEmitterSpawnPatternType.SphereAroundUnit;
                case 3: return SpellVisualEmitterSpawnPatternType.DiscOnGround;
                case 4: return SpellVisualEmitterSpawnPatternType.ColumnFromGround;
                case 5: return SpellVisualEmitterSpawnPatternType.DiscAroundUnitCenter;
                default: return SpellVisualEmitterSpawnPatternType.None;
            }
        }

        private SpellEmitterModelAttachLocationType GetEmissionAttachLocation(EQSpellsEFF.EFFSourceSectionData effectSection, int effectIndex, 
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
