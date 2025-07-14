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
            Radius = effectSection.EmitterSpawnRadii[effectIndex] * Configuration.GENERATE_WORLD_SCALE;
            
            // Convert values
            SpriteFileNameNoExt = effectSection.SpriteNames[effectIndex].Replace("_SPRITE", "");
            
            float feetToMeterMod = 0.3048f;
            Gravity = effectSection.EmitterGravities[effectIndex] * feetToMeterMod; // Change feet to meters (?)

            // Factor world scale for spell scale
            Scale = (effectSection.EmitterSpawnScale[effectIndex] * Configuration.GENERATE_WORLD_SCALE);
            
            //SpawnRate = effectSection.EmitterSpawnRates[effectIndex]; // Figure out this rate value, 
            SpawnRate = 10; // Temp
        }
        
        private float CalculateVelocity(EQSpellsEFF.SectionData effectSection, int effectIndex, SpellVisualEmitterSpawnPatternType spawnPattern)
        {
            float sourceVelocity = effectSection.EmitterSpawnVelocities[effectIndex];

            // Hands always 'shoot out' unless there's a velocity, and then it's nothing
            if (spawnPattern == SpellVisualEmitterSpawnPatternType.FromHands)
            {
                if (sourceVelocity == 0)
                    return -1f; // This is about right
                else
                    return 0f;
            }            

            // Default is just use the defined velocity
            return sourceVelocity;
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
                case 3: return SpellVisualEmitterSpawnPatternType.DiscPlayerCenter;
                case 4: return SpellVisualEmitterSpawnPatternType.ColumnFromGround;
                case 5: return SpellVisualEmitterSpawnPatternType.DiscOnGround;
                default: return SpellVisualEmitterSpawnPatternType.None;
            }
        }
        
        private SpellEmitterModelAttachLocationType GetEmissionAttachLocation(EQSpellsEFF.SectionData effectSection, int effectIndex)
        {
            switch (effectSection.EmissionTypeIDs[effectIndex])
            {
                case 0: return SpellEmitterModelAttachLocationType.Hands;
                case 1: // Fallthrough
                case 2:
                    {
                        switch (effectSection.LocationIDs[effectIndex])
                        {
                            case 1: return SpellEmitterModelAttachLocationType.Head;
                            case 2: return SpellEmitterModelAttachLocationType.Hands; // May need to be separate
                            case 3: return SpellEmitterModelAttachLocationType.Hands; // May need to be separate
                            case 4: return SpellEmitterModelAttachLocationType.Feet; // May need to be separate
                            case 5: return SpellEmitterModelAttachLocationType.Feet; // May need to be separate
                            default: return SpellEmitterModelAttachLocationType.Chest;
                        }
                    }
                case 3: return SpellEmitterModelAttachLocationType.Feet;
                case 4: return SpellEmitterModelAttachLocationType.Feet;
                case 5: return SpellEmitterModelAttachLocationType.Chest;
                default: return SpellEmitterModelAttachLocationType.Chest; // Default to chest
            }
        }
    }
}
