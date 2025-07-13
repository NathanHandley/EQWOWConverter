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

        public string SpriteFileNameNoExt = string.Empty;
        public EmissionAttachLocation EmissionLocation = EmissionAttachLocation.Chest;
        public float Gravity = 0;
        public int LifespanInMS = 0;
        public float Scale = 0;
        public float Velocity = 0;
        public int SpawnRate = 0;
        public int SpellVisualEffectNameDBCID;

        public void Load(EQSpellsEFF.SectionData effectSection, int effectIndex, int spellVisualEffectNameDBCID)
        {
            if (effectIndex > 2)
            {
                Logger.WriteError("Could not load ObjectModelParticleEmitter because effectIndex was > 2");
                return;
            }
            SpellVisualEffectNameDBCID = spellVisualEffectNameDBCID;

            // Convert values
            SpriteFileNameNoExt = effectSection.SpriteNames[effectIndex].Replace("_SPRITE", "");
            EmissionLocation = GetEmissionAttachLocation(effectSection, effectIndex);
            Gravity = effectSection.EmitterGravities[effectIndex];
            LifespanInMS = effectSection.EmitterSpawnLifespans[effectIndex];
            Scale = effectSection.EmitterSpawnScale[effectIndex];
            Velocity = effectSection.EmitterSpawnVelocities[effectIndex];
            //SpawnRate = effectSection.EmitterSpawnRates[effectIndex]; // Figure out this rate value, 
            SpawnRate = 10; // Temp

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
