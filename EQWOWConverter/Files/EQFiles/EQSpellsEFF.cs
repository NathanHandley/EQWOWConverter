using EQWOWConverter.Common;

namespace EQWOWConverter.EQFiles
{
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

    // Much of this structure data was from research from "Stolistic" on the Project Latern discord
    internal class EQSpellsEFF
    {
        internal class SectionData
        {
            public string[] SpriteNames = new string[3];
            public string Type = string.Empty;
            public int[] LocationIDs = new int[3]; // 0 = Body Center, 1 = Head, 2 = Right Hand, 3 = Left Hand, 4 = Right Foot, 5 = Left Foot, 6 = Also center of body
            public int[] EmissionTypeIDs = new int[3]; // -1 = None, 0 = Hands, 1 = Cone to the right, 2 = Sphere around player, 3 = Disc on the ground, 4 = Column from the ground, 5 = Disc at the player center
            public string[] SpriteListNames = new string[12]; // Up to 12 sprites used for animating particles. Loop through until the end and start at begining. Porticles oriented in a circle, with each 30 degrees from the next for a total of 360 degrees.
            public int SpriteListEffect; // 1 = Projectile, 2 = Disc pulsating outward and then inward
            public int Flags = 0; // (zero based) Bit3 = Rain Effect, Bit5 and 6 are always 1.  7-15 are unused.  Others are unknown.
            public ColorRGBA[] EmitterColors = new ColorRGBA[3]; // BGRX formation, where X (otherwise alpha) is unused
            public float[] EmitterGravities = new float[3];
            public float[] EmitterSpawnXs = new float[3]; // #1 can be 1 or -1, #2 can be 1, #3 can be 1 or -1 (?)
            public float[] EmitterSpawnYs = new float[3]; // #1 can be 1 or -1, #2 can be -1, #3 can be 1 (?)
            public float[] EmitterSpawnZs = new float[3]; // #1 and #2 can be 1 or -1 or -2, #3 can be 1 or -1
            public float[] EmitterSpawnRadiuses = new float[3]; // Radius of the particles
            public float[] EmitterSpawnAngles = new float[3]; // Angle of the particles (unsure on orientation)
            public int[] EmitterSpawnLifespans = new int[3]; // Lifespawn of the particles in milliseconds
            public float[] EmitterSpawnVelocities = new float[3]; // Velocity of the particles
            public int[] EmitterSpawnRates = new int[3]; // Spawn rates of the particles (in what?)
            public float[] EmitterSpawnScale = new float[3]; // Scale of the particles
            public int[] UnknownData = new int[9];  // 9 unknown values that is always zero
            public float[] SpriteListUnknown = new float[12]; // Unsure.  Can be 10, 15, 20, or 30
            public short[] SpriteListCircularShifts = new short[12]; // How many rotational shifts particles will travel during animation.  15 shifts is one full rotation counterclockwise.  Negative goes clockwise.  Ignored if movement is stationary.
            public short[] SpriteListVerticalForcees = new short[12]; // Verticle force applied to the particles.  -3 and +3 are common.  3 = Upper Top, 2 = Upper Middle, 1 = Upper Bottom, 0 = Player Center, -1 = Lower Top, -2 = Lower Middle, -3 = Lower Bottom
            public float[] SpriteListRadii = new float[12]; // Radius from playre's center for the particles
            public short[] SpriteListMovements = new short[12]; // Determines if the particle moves.  1 = Stationary, 2 = Moves
            public float[] SpriteListScales = new float[12]; // The scale of the particles
        }

        internal class SpellEffect
        {
            public int Field01;
            public int Field02;
            public SectionData[] SectionDatas = new SectionData[3]; // Always 3, sometimes blank
        }        

        public bool LoadFromDisk(string fileFullPath)
        {
            Logger.WriteDebug(" - Reading Sound Effects Data from '" + fileFullPath + "'...");
            if (File.Exists(fileFullPath) == false)
            {
                Logger.WriteError("- Could not find Sound Effects file that should be at '" + fileFullPath + "'");
                return false;
            }

            // Load in all the data
            List<byte> fileBytes = FileTool.GetFileBytes(fileFullPath);



            return true;
        }
    }
}
