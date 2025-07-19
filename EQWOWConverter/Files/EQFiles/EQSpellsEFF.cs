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

namespace EQWOWConverter.EQFiles
{
    // Much of this structure data was from research from "Stolistic" on the Project Latern discord (posted 12/30/2022 in #lantern-general)
    internal class EQSpellsEFF
    {
        internal class SectionData
        {
            public int VisualEffectIndex = 0;
            public string[] SpriteNames = new string[3];
            public string TypeString = string.Empty;
            public int[] LocationIDs = new int[3]; // -1 = None(?), 0 = Body Center, 1 = Head, 2 = Right Hand, 3 = Left Hand, 4 = Right Foot, 5 = Left Foot, 6+ = Also center of body
            public int[] EmissionTypeIDs = new int[3]; // -1 = None, 0 = Hands, 1 = Cone to the right, 2 = Sphere around player, 3 = Disc on the ground, 4 = Column from the ground, 5 = Disc at the player center.  Note: 5 also gives a 2 and a 3 emitter.
            public string[] SpriteListNames = new string[12]; // Up to 12 sprites used for animating particles. Loop through until the end and start at begining. Porticles oriented in a circle, with each 30 degrees from the next for a total of 360 degrees.
            public int SpriteListEffect; // 1 = Projectile, 2 = Disc pulsating outward and then inward
            public int SoundID = 0; // Values look to be between 103-113. -1 for no sound
            public ColorRGBA[] EmitterColors = new ColorRGBA[3]; // BGRX formation, where X (otherwise alpha) is unused
            public float[] EmitterGravities = new float[3];
            public float[] EmitterSpawnXs = new float[3]; // #1 can be 1 or -1, #2 can be 1, #3 can be 1 or -1 (?)
            public float[] EmitterSpawnYs = new float[3]; // #1 can be 1 or -1, #2 can be -1, #3 can be 1 (?)
            public float[] EmitterSpawnZs = new float[3]; // #1 and #2 can be 1 or -1 or -2, #3 can be 1 or -1
            public float[] EmitterSpawnRadii = new float[3]; // Radius of the particles
            public float[] EmitterSpawnAngles = new float[3]; // Angle of the particles (unsure on orientation)
            public int[] EmitterSpawnLifespans = new int[3]; // Lifespawn of the particles in milliseconds
            public float[] EmitterSpawnVelocities = new float[3]; // Velocity of the particles
            public int[] EmitterSpawnRates = new int[3]; // Spawn rates of the particles (in what?)
            public float[] EmitterSpawnScale = new float[3]; // Scale of the particles
            public int[] UnknownData = new int[9];  // 9 unknown values that is always zero
            public float[] SpriteListUnknown = new float[12]; // Unsure.  Can be 10, 15, 20, or 30
            public short[] SpriteListCircularShifts = new short[12]; // How many rotational shifts particles will travel during animation.  15 shifts is one full rotation counterclockwise.  Negative goes clockwise.  Ignored if movement is stationary.
            public short[] SpriteListVerticalForces = new short[12]; // Verticle force applied to the particles.  -3 and +3 are common.  3 = Upper Top, 2 = Upper Middle, 1 = Upper Bottom, 0 = Player Center, -1 = Lower Top, -2 = Lower Middle, -3 = Lower Bottom
            public float[] SpriteListRadii = new float[12]; // Radius from playre's center for the particles
            public short[] SpriteListMovements = new short[12]; // Determines if the particle moves.  1 = Stationary, 2 = Moves
            public float[] SpriteListScales = new float[12]; // The scale of the particles

            public void LoadFromBytes(List<byte> bytes, ref int byteCursor)
            {
                // TODO: The 12 element ones are actually 4 blocks of 3, so break into parts
                for (int i = 0; i < 3; i++)
                    SpriteNames[i] = ByteTool.ReadStringFromBytes(bytes, ref byteCursor, 32);
                TypeString = ByteTool.ReadStringFromBytes(bytes, ref byteCursor, 32);
                for (int i = 0; i < 3; i++)
                    LocationIDs[i] = ByteTool.ReadInt32FromBytes(bytes, ref byteCursor);
                for (int i = 0; i < 3; i++)
                    EmissionTypeIDs[i] = ByteTool.ReadInt32FromBytes(bytes, ref byteCursor);
                for (int i = 0; i < 12; i++)
                    SpriteListNames[i] = ByteTool.ReadStringFromBytes(bytes, ref byteCursor, 32);
                SpriteListEffect = ByteTool.ReadInt32FromBytes(bytes, ref byteCursor);
                SoundID = ByteTool.ReadInt32FromBytes(bytes, ref byteCursor);
                for (int i = 0; i < 3; i++)
                    EmitterColors[i] = ByteTool.ReadColorBGRAFromBytes(bytes, ref byteCursor);
                for (int i = 0; i < 3; i++)
                    EmitterGravities[i] = ByteTool.ReadFloatFromBytes(bytes, ref byteCursor);
                for (int i = 0; i < 3; i++)
                {
                    EmitterSpawnXs[i] = ByteTool.ReadFloatFromBytes(bytes, ref byteCursor);
                    EmitterSpawnYs[i] = ByteTool.ReadFloatFromBytes(bytes, ref byteCursor);
                    EmitterSpawnZs[i] = ByteTool.ReadFloatFromBytes(bytes, ref byteCursor);
                }
                for (int i = 0; i < 3; i++)
                    EmitterSpawnRadii[i] = ByteTool.ReadFloatFromBytes(bytes, ref byteCursor);
                for (int i = 0; i < 3; i++)
                    EmitterSpawnAngles[i] = ByteTool.ReadFloatFromBytes(bytes, ref byteCursor);
                for (int i = 0; i < 3; i++)
                    EmitterSpawnLifespans[i] = ByteTool.ReadInt32FromBytes(bytes, ref byteCursor);
                for (int i = 0; i < 3; i++)
                    EmitterSpawnVelocities[i] = ByteTool.ReadFloatFromBytes(bytes, ref byteCursor);
                for (int i = 0; i < 3; i++)
                    EmitterSpawnRates[i] = ByteTool.ReadInt32FromBytes(bytes, ref byteCursor);
                for (int i = 0; i < 3; i++)
                    EmitterSpawnScale[i] = ByteTool.ReadFloatFromBytes(bytes, ref byteCursor);
                for (int i = 0; i < 9; i++)
                    UnknownData[i] = ByteTool.ReadInt32FromBytes(bytes, ref byteCursor);
                for (int i = 0; i < 12; i++)
                    SpriteListUnknown[i] = ByteTool.ReadFloatFromBytes(bytes, ref byteCursor);
                for (int i = 0; i < 12; i++)
                    SpriteListCircularShifts[i] = ByteTool.ReadInt16FromBytes(bytes, ref byteCursor);
                for (int i = 0; i < 12; i++)
                    SpriteListVerticalForces[i] = ByteTool.ReadInt16FromBytes(bytes, ref byteCursor);
                for (int i = 0; i < 12; i++)
                    SpriteListRadii[i] = ByteTool.ReadFloatFromBytes(bytes, ref byteCursor);
                for (int i = 0; i < 12; i++)
                    SpriteListMovements[i] = ByteTool.ReadInt16FromBytes(bytes, ref byteCursor);
                for (int i = 0; i < 12; i++)
                    SpriteListScales[i] = ByteTool.ReadFloatFromBytes(bytes, ref byteCursor);
            }
        }

        internal class EQSpellEffect
        {
            public int Field01;
            public int Field02;
            public int VisualEffectIndex = 0;
            public SectionData[] SectionDatas = new SectionData[3]; // Always 3, sometimes blank
        }        

        public List<EQSpellEffect> SpellEffects = new List<EQSpellEffect>();

        public bool LoadFromDisk(string fileFullPath)
        {
            Logger.WriteDebug(" - Reading Spell Effects Data from '", fileFullPath, "'...");
            if (File.Exists(fileFullPath) == false)
            {
                Logger.WriteError("- Could not find Spell Effects file that should be at '", fileFullPath, "'");
                return false;
            }

            // Load in all the data
            List<byte> fileBytes = FileTool.GetFileBytes(fileFullPath);

            //  There are 255 spell effects in the file, but looks like only 52 (0-51) are populated with anything
            int byteCursor = 0;
            for (int i = 0; i < 52; i++)
            {
                EQSpellEffect curEffect = new EQSpellEffect();
                curEffect.Field01 = ByteTool.ReadInt32FromBytes(fileBytes, ref byteCursor);
                curEffect.Field02 = ByteTool.ReadInt32FromBytes(fileBytes, ref byteCursor);
                curEffect.VisualEffectIndex = i;

                // Always three sections
                for (int j = 0; j < 3; j++)
                {
                    curEffect.SectionDatas[j] = new SectionData();
                    curEffect.SectionDatas[j].VisualEffectIndex = i;
                    curEffect.SectionDatas[j].LoadFromBytes(fileBytes, ref byteCursor);
                }

                SpellEffects.Add(curEffect);
            }

            Logger.WriteDebug(" - Done reading Spell Effects from '", fileFullPath, "'");
            return true;
        }
    }
}
