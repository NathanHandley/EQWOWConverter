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
using System.Drawing.Text;

namespace EQWOWConverter.ObjectModels
{
    internal class ObjectModelParticleEmitter
    {
        internal enum EmissionAttachLocation
        {
            Chest,
            Head,
            Hands, // Could be left and right hands, check
            Feet // Colud be left and right feet, check
        }

        internal enum EmissionSpawnPattern
        {
            None,
            FromHands, // Exits from the hands based on velocity
            ConeToRight, // Unsure what uses this
            SphereAroundPlayer, // Appears around the player starting at EmitterSpawnRadius distance from center
            DiscOnGround, // Disc around the player
            ColumnFromGround, // Around the player, generally going up (gravity)
            DiscPlayerCenter // Comes out from the center of the player        
        }

        public string SpriteFileNameNoExt = string.Empty;
        public EmissionAttachLocation EmissionLocation = EmissionAttachLocation.Chest;
        public EmissionSpawnPattern EmissionPattern = EmissionSpawnPattern.None;
        public float Gravity = 0;
        public int LifespanInMS = 0;
        public float Scale = 0;
        public float Velocity = 0;
        public int SpawnRate = 0;
        public int SpellVisualEffectNameDBCID;

        public void Load(EQSpellsEFF.SectionData effectSection, int effectIndex, int spellVisualEffectNameDBCID)
        {
            SpellVisualEffectNameDBCID = spellVisualEffectNameDBCID;

            // Calculate the location and pattern first since those are used in further calculations.
            EmissionLocation = GetEmissionAttachLocation(effectSection, effectIndex);
            EmissionPattern = GetEmissionSpawnPattern(effectSection, effectIndex);

            // Velocity
            Velocity = CalculateVelocity(effectSection, effectIndex, EmissionPattern);

            // Lifespan
            LifespanInMS = CalculateLifespanInMS(effectSection, effectIndex);

            float feetToMeterMod = 0.3048f;

            // Convert values
            SpriteFileNameNoExt = effectSection.SpriteNames[effectIndex].Replace("_SPRITE", "");
            Gravity = effectSection.EmitterGravities[effectIndex] * feetToMeterMod; // Change feet to meters
            Scale = effectSection.EmitterSpawnScale[effectIndex] * Configuration.GENERATE_WORLD_SCALE;
            
            //SpawnRate = effectSection.EmitterSpawnRates[effectIndex]; // Figure out this rate value, 
            SpawnRate = 10; // Temp
        }
        
        private float CalculateVelocity(EQSpellsEFF.SectionData effectSection, int effectIndex, EmissionSpawnPattern spawnPattern)
        {
            float sourceVelocity = effectSection.EmitterSpawnVelocities[effectIndex];

            // Hands always 'shoot out' unless there's a velocity, and then it's nothing
            if (spawnPattern == EmissionSpawnPattern.FromHands)
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

        private EmissionSpawnPattern GetEmissionSpawnPattern(EQSpellsEFF.SectionData effectSection, int effectIndex)
        {
            switch (effectSection.EmissionTypeIDs[effectIndex])
            {
                case 0: return EmissionSpawnPattern.FromHands;
                case 1: return EmissionSpawnPattern.ConeToRight;
                case 2: return EmissionSpawnPattern.SphereAroundPlayer;
                case 3: return EmissionSpawnPattern.DiscPlayerCenter;
                case 4: return EmissionSpawnPattern.ColumnFromGround;
                case 5: return EmissionSpawnPattern.DiscOnGround;
                default: return EmissionSpawnPattern.None;
            }
        }
        
        private EmissionAttachLocation GetEmissionAttachLocation(EQSpellsEFF.SectionData effectSection, int effectIndex)
        {
            switch (effectSection.EmissionTypeIDs[effectIndex])
            {
                case 0: return EmissionAttachLocation.Hands;
                case 1: // Fallthrough
                case 2:
                    {
                        switch (effectSection.LocationIDs[effectIndex])
                        {
                            case 1: return EmissionAttachLocation.Head;
                            case 2: return EmissionAttachLocation.Hands; // May need to be separate
                            case 3: return EmissionAttachLocation.Hands; // May need to be separate
                            case 4: return EmissionAttachLocation.Feet; // May need to be separate
                            case 5: return EmissionAttachLocation.Feet; // May need to be separate
                            default: return EmissionAttachLocation.Chest;
                        }
                    }
                case 3: return EmissionAttachLocation.Feet;
                case 4: return EmissionAttachLocation.Feet;
                case 5: return EmissionAttachLocation.Chest;
                default: return EmissionAttachLocation.Chest; // Default to chest
            }
        }
    }
}
