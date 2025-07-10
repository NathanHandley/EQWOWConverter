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
using EQWOWConverter.ObjectModels;

namespace EQWOWConverter.WOWFiles
{
    internal class M2ParticleEmitter : IOffsetByteSerializable
    {
        private int ID = -1; // Always -1
        private UInt32 Flags;
        private Vector3 RelativePosition = new Vector3();
        private UInt16 ParentBoneID = 0;
        private UInt16 TextureID = 0;
        private UInt32 GeometryModelLength = 0; // ?
        private UInt32 GeometryModelOffset = 0; // ?
        private UInt32 RecursionModelLength = 0; // ?
        private UInt32 RecursionModelOffset = 0; // ?
        private ObjectModelParticleBlendModeType BlendModeType = ObjectModelParticleBlendModeType.Opaque;
        private ObjectModelParticleEmitterType EmitterType = ObjectModelParticleEmitterType.Plane;
        private UInt16 ParticleIndex = 0; // Find where this references
        private byte ParticleType = 0; // 0 = Normal, 1 = Large (moonwell), 2 = Like 0 but for Tram(?)
        private byte HeadOrTail = 0; // 0 = Head (billboard square quad), 1 = Tail (Billboarded along the axis of motion and stretches in length based on speed), 2 = Both.
        private Int16 TextureTileRotation = 0; // Rotation for the texture tile, -1, 0, 1
        private UInt16 TextureDimensionsRows = 1; // For tiled textures
        private UInt16 TextureDimensionColumns = 1; // For tiled textures
        private M2TrackSequences<M2Float> EmissionSpeed = new M2TrackSequences<M2Float>(); // Base velocity of emitted particles
        private M2TrackSequences<M2Float> SpeedVariation = new M2TrackSequences<M2Float>(); // Amount of random varation in particle emission velocity
        private M2TrackSequences<M2Float> VerticalRange = new M2TrackSequences<M2Float>(); // Range 0 to pi. For plane generators, this is the maximum polar angle of the initial velocity, and 0 makes the velocity straight up (+z)
                                                                                                               // For sphere generators, this is the maximum elevation of the initial position, 0 makes the initial position entirely in the x-y plane (z=0)
        private M2TrackSequences<M2Float> HorizontalRange = new M2TrackSequences<M2Float>(); // Range 0 to 2 * pi.  For plane generators this is the maximum azimuth angle of the initial velocity, and 0 makes the velocity have no sideways (y axis) component.
                                                                                                                 // For sphere generators, this is the maximum azimuth angle of the initial position
        private M2TrackSequences<M2Float> Gravity = new M2TrackSequences<M2Float>(); // Sometimes a float, sometimes a compressed vector as defined by the Flags
        private M2TrackSequences<M2Float> Lifespan = new M2TrackSequences<M2Float>(); // Number of seconds a particle exists after creation
        private float LifespanVariation = 0; // LifespanVaration * random(-1, 1) is added to Lifespan
        private M2TrackSequences<M2Float> EmissionRate = new M2TrackSequences<M2Float>(); // What is this exactly?
        private float EmissionRateVariation = 0; // EmissionRateVariation * random(-1, 1) is added to EmissionRate
        private M2TrackSequences<M2Float> EmissionAreaLength = new M2TrackSequences<M2Float>(); // For plane this is width of the plane in x-axis, for Sphere it's the minimum radius
        private M2TrackSequences<M2Float> EmissionAreaWidth = new M2TrackSequences<M2Float>(); // For plane this is width of the plane in y-axis, for Sphere it's the maximum radius
        private M2TrackSequences<M2Float> ZSource = new M2TrackSequences<M2Float>(); // When > 0, initial particle velicoty is (position - (0, 0, ZSource)) normalized
        private M2TrackSequencesSimple<Vector3> ColorTrack = new M2TrackSequencesSimple<Vector3>(); // Value format is actually sets of RGB where they are float values from 0-255.  Consider different object here.
        private M2TrackSequencesSimple<M2UInt16> AlphaTrack = new M2TrackSequencesSimple<M2UInt16>(); // 0 - 32767
        private M2TrackSequencesSimple<Vector2> ScaleTrack = new M2TrackSequencesSimple<Vector2>();
        private Vector2 ScaleVariation = new Vector2(); // Percentage amount to randomly scale each particle
        private M2TrackSequencesSimple<M2UInt16> HeadCellTrack = new M2TrackSequencesSimple<M2UInt16>(); // Relates to intensity but unknown exactly on values.  0, 16, 17, 32 are values seen
        private M2TrackSequencesSimple<M2UInt16> TailCellTrack = new M2TrackSequencesSimple<M2UInt16>();
        private float TailLengthMultiplier = 0.1f; // Multiple for calculating tail particle length.  0.1 is seen a bit
        private float TwinkleSpeed = 18f; // Possibly a FPS number for twinkle. 18f is seen
        private float TwinklPercent = 1f; // Not 100% sure on this
        private Vector2 TwinkleRange = new Vector2(0.5f, 1.5f); // "Min and Max", but .5 and 1.5 is seen.  Adjust
        private float BurstMultiplier = 1f; // Unsure, but requires M2ParticleEmitterFlags.UseBurstMultiplier
        private float Drag = 0; // If no-zero, the particles slow down sooner than they would otherwise
        // TODO: Base spin




        public UInt32 GetHeaderSize()
        {
            UInt32 size = 0;
            // TODO
            return size;
        }

        public List<byte> GetHeaderBytes()
        {
            List<byte> bytes = new List<byte>();
            // TODO
            return bytes;
        }

        public void AddDataBytes(ref List<byte> byteBuffer)
        {
            // TODO
        }
    }
}
