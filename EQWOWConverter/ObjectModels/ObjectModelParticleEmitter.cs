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

        public void Load(EQSpellsEFF.EFFSpellEmitter effEmitter, SpellVisualEmitterSpawnPatternType emitterPatternOverride = SpellVisualEmitterSpawnPatternType.None)
        {
            VisualEffectIndex = effEmitter.VisualEffectIndex;

            // Calculate the location and pattern first since those are used in further calculations.
            if (emitterPatternOverride == SpellVisualEmitterSpawnPatternType.None)
                EmissionPattern = GetEmissionSpawnPattern(effEmitter.EmissionTypeID, effEmitter.SpawnZ);
            else
                EmissionPattern = emitterPatternOverride;
            EmissionLocation = GetEmissionAttachLocation(effEmitter.LocationID, EmissionPattern);

            // Velocity
            Velocity = CalculateVelocity(effEmitter.Velocity, EmissionPattern);

            // Lifespan
            LifespanInMS = CalculateLifespanInMS(effEmitter.ParticleLifespan);

            // Radius
            Radius = CalculateRadius(effEmitter.Radius, EmissionPattern);

            // Sprite sheet name
            SpriteSheetFileNameNoExt = GetSpriteSheetName(effEmitter.SpriteName, effEmitter.Color);

            // Gravity
            Gravity = CalculateGravity(effEmitter.Gravity, EmissionPattern);

            // Scale
            Scale = CalculateScale(effEmitter.Scale);

            // Spawn rate
            SpawnRate = CalculateSpawnRate(effEmitter.SpawnRate, EmissionPattern);
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

        private float CalculateScale(float eqSpawnScale)
        {
            float scale = Math.Max(eqSpawnScale, Configuration.SPELLS_EFFECT_PARTICLE_SIZE_SCALE_MIN);
            return scale * Configuration.SPELLS_EFFECT_PARTICLE_SIZE_SCALE_MOD;
        }

        private float CalculateRadius(float eqRadius, SpellVisualEmitterSpawnPatternType emissionPattern)
        {
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

        private float CalculateGravity(float eqGravity, SpellVisualEmitterSpawnPatternType emissionPattern)
        {
            // Disc's have a default gravity seemingly (see Spirit of Wolf (effect 7))
            if (emissionPattern == SpellVisualEmitterSpawnPatternType.DiscAroundUnitCenter && eqGravity == 0)
                eqGravity = 6;

            // For columns coming from above, flip the gravity (why? Blizzard looks wrong without it...)
            if (emissionPattern == SpellVisualEmitterSpawnPatternType.ColumnFromAbove)
                eqGravity *= -1;

            return eqGravity * Configuration.SPELLS_EFFECT_DISTANCE_SCALE_MOD;
        }

        private int CalculateSpawnRate(int eqSpawnRate, SpellVisualEmitterSpawnPatternType emissionPattern)
        {
            // Defaults
            if (eqSpawnRate == 0)
            {
                switch (emissionPattern)
                {
                    case SpellVisualEmitterSpawnPatternType.SphereAroundUnit: eqSpawnRate = Configuration.SPELL_EMITTER_SPAWN_RATE_SPHERE_DEFAULT; break;
                    default: eqSpawnRate = Configuration.SPELL_EMITTER_SPAWN_RATE_OTHER_DEFAULT; break;
                }
            }

            // Minimums
            switch (emissionPattern)
            {
                case SpellVisualEmitterSpawnPatternType.SphereAroundUnit: eqSpawnRate = Math.Max(eqSpawnRate, Configuration.SPELL_EMITTER_SPAWN_RATE_SPHERE_MINIMUM); break;
                case SpellVisualEmitterSpawnPatternType.FromHands: eqSpawnRate = Math.Max(eqSpawnRate, 25); break;
                case SpellVisualEmitterSpawnPatternType.DiscAroundUnitCenter: eqSpawnRate = Math.Max(eqSpawnRate, 25); break;
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

            return Convert.ToInt32(Convert.ToSingle(eqSpawnRate) * spawnRateMod);
        }

        private float CalculateVelocity(float eqVelocity, SpellVisualEmitterSpawnPatternType spawnPattern)
        {
            if (eqVelocity == 0)
            {
                switch (spawnPattern)
                {
                    case SpellVisualEmitterSpawnPatternType.FromHands: eqVelocity = 3f; break;
                    case SpellVisualEmitterSpawnPatternType.DiscAroundUnitCenter: eqVelocity = 6f; break;
                    default: break;
                }
            }

            // Particles from hands or outer sphere need a reverse of their velocity
            if (spawnPattern == SpellVisualEmitterSpawnPatternType.FromHands)
                eqVelocity *= -1;
            if (spawnPattern == SpellVisualEmitterSpawnPatternType.SphereAwayFromPlayer)
                eqVelocity *= -1;

            return eqVelocity * Configuration.SPELLS_EFFECT_DISTANCE_SCALE_MOD;
        }

        private int CalculateLifespanInMS(int eqLifespanInMS)
        {
            // Default to a second if no lifespan
            if (eqLifespanInMS == 0)
                return Convert.ToInt32(1000f * Configuration.SPELLS_EFFECT_PARTICLE_LIFESPAN_TIME_MOD);

            return Convert.ToInt32(Convert.ToSingle(eqLifespanInMS) * Configuration.SPELLS_EFFECT_PARTICLE_LIFESPAN_TIME_MOD);
        }

        private SpellVisualEmitterSpawnPatternType GetEmissionSpawnPattern(int eqEmissionTypeID, float eqZPosition)
        {
            switch (eqEmissionTypeID)
            {
                case 0: return SpellVisualEmitterSpawnPatternType.FromHands;
                case 1: return SpellVisualEmitterSpawnPatternType.SphereAwayFromPlayer;
                case 2: return SpellVisualEmitterSpawnPatternType.SphereAroundUnit;
                case 3:
                    {
                        if (eqZPosition < 0)
                            return SpellVisualEmitterSpawnPatternType.DiscAboveUnit;
                        else
                            return SpellVisualEmitterSpawnPatternType.DiscOnGround;
                    }
                case 4: 
                    {
                        if (eqZPosition < 0)
                            return SpellVisualEmitterSpawnPatternType.ColumnFromAbove;
                        else
                            return SpellVisualEmitterSpawnPatternType.ColumnFromGround;
                    }
                case 5: return SpellVisualEmitterSpawnPatternType.DiscAroundUnitCenter;
                default: return SpellVisualEmitterSpawnPatternType.None;
            }
        }

        private SpellEmitterModelAttachLocationType GetEmissionAttachLocation(int eqLocationID, SpellVisualEmitterSpawnPatternType emissionPattern)
        {
            // Emission type overrides any setting on attach location
            switch (emissionPattern)
            {
                case SpellVisualEmitterSpawnPatternType.FromHands: return SpellEmitterModelAttachLocationType.Hands;
                case SpellVisualEmitterSpawnPatternType.ColumnFromGround: return SpellEmitterModelAttachLocationType.Feet;
                case SpellVisualEmitterSpawnPatternType.DiscOnGround: return SpellEmitterModelAttachLocationType.Feet;
                case SpellVisualEmitterSpawnPatternType.DiscAboveUnit: return SpellEmitterModelAttachLocationType.Head;
                case SpellVisualEmitterSpawnPatternType.ColumnFromAbove: return SpellEmitterModelAttachLocationType.Head;
                default: break;
            }

            // Otherwise, use location ID
            switch (eqLocationID)
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
