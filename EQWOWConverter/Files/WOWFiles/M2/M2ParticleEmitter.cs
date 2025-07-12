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
using Org.BouncyCastle.Utilities;
using System.Collections.Generic;
using System.Drawing;

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
        private float BaseSpin = 0; // Initial rotation of a particle quad
        private float BaseSpinVariation = 0; // Random variation on base spin
        private float Spin = 0; // Rotation of the particle quad per second
        private float SpinVariation = 0; // Random variation on spin
        private Vector3 TumbleMin = new Vector3();
        private Vector3 TumbleMax = new Vector3();
        private Vector3 WindVector = new Vector3();
        private float WindTime = 0;
        private float FollowSpeed1 = 0;
        private float FollowScale1 = 0;
        private float FollowSpeed2 = 0;
        private float FollowScale2 = 0;
        private UInt32 SplinePointCount = 0; // These two are actually a track, but not implementing unless we use splines
        private UInt32 SplinePointOffset = 0;
        private M2TrackSequences<M2Char> EnabledIn = new M2TrackSequences<M2Char>(); // May not be needed for anything.  Purpose not clear

        public M2ParticleEmitter(ObjectModelParticleEmitter objectModelParticleEmitter, UInt16 textureID)
        {
            TextureID = textureID;
            EmissionSpeed.TrackSequences.AddSequence();
            EmissionSpeed.TrackSequences.AddValueToLastSequence(0, new M2Float(objectModelParticleEmitter.Velocity));
            ScaleTrack.TrackSequences.AddSequence();
            ScaleTrack.TrackSequences.AddValueToLastSequence(0, new Vector2(objectModelParticleEmitter.Scale, objectModelParticleEmitter.Scale));
            EmissionRate.TrackSequences.AddSequence();
            EmissionRate.TrackSequences.AddValueToLastSequence(0, new M2Float(objectModelParticleEmitter.SpawnRate));
            Gravity.TrackSequences.AddSequence();
            Gravity.TrackSequences.AddValueToLastSequence(0, new M2Float(objectModelParticleEmitter.Gravity));
        }

        public UInt32 GetHeaderSize()
        {
            UInt32 size = 0;
            size += 4; // ID
            size += 4; // Flags
            size += RelativePosition.GetBytesSize();
            size += 2; // ParentBoneID
            size += 2; // TextureID
            size += 4; // GeometryModelLength
            size += 4; // GeometryModelOffset
            size += 4; // RecursionModelLength
            size += 4; // RecursionModelOffset
            size += 1; // BlendModeType
            size += 1; // EmitterType
            size += 2; // ParticleIndex
            size += 1; // ParticleType
            size += 1; // HeadOrTail
            size += 2; // TextureTileRotation
            size += 2; // TextureDimensionsRows
            size += 2; // TextureDimensionColumns
            size += EmissionSpeed.GetHeaderSize();
            size += SpeedVariation.GetHeaderSize();
            size += VerticalRange.GetHeaderSize();
            size += HorizontalRange.GetHeaderSize();
            size += Gravity.GetHeaderSize();
            size += Lifespan.GetHeaderSize();
            size += 4; // LifespanVariation
            size += EmissionRate.GetHeaderSize();
            size += 4; // EmissionRateVariation
            size += EmissionAreaLength.GetHeaderSize();
            size += EmissionAreaWidth.GetHeaderSize();
            size += ZSource.GetHeaderSize();
            size += ColorTrack.GetHeaderSize();
            size += AlphaTrack.GetHeaderSize();
            size += ScaleTrack.GetHeaderSize();
            size += ScaleVariation.GetBytesSize();
            size += HeadCellTrack.GetHeaderSize();
            size += TailCellTrack.GetHeaderSize();
            size += 4; // TailLengthMultiplier
            size += 4; // TwinkleSpeed
            size += 4; // TwinklPercent
            size += TwinkleRange.GetBytesSize();
            size += 4; // BurstMultiplier
            size += 4; // Drag
            size += 4; // BaseSpin
            size += 4; // BaseSpinVariation
            size += 4; // Spin
            size += 4; // SpinVariation
            size += TumbleMin.GetBytesSize();
            size += TumbleMax.GetBytesSize();
            size += WindVector.GetBytesSize();
            size += 4; // WindTime
            size += 4; // FollowSpeed1
            size += 4; // FollowScale1
            size += 4; // FollowSpeed2
            size += 4; // FollowScale2
            size += 4; // SplinePointCount
            size += 4; // SplinePointOffset
            size += EnabledIn.GetHeaderSize();
            return size;
        }

        public List<byte> GetHeaderBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(ID));
            bytes.AddRange(BitConverter.GetBytes(Flags));
            bytes.AddRange(RelativePosition.ToBytes());
            bytes.AddRange(BitConverter.GetBytes(ParentBoneID));
            bytes.AddRange(BitConverter.GetBytes(TextureID));
            bytes.AddRange(BitConverter.GetBytes(GeometryModelLength));
            bytes.AddRange(BitConverter.GetBytes(GeometryModelOffset));
            bytes.AddRange(BitConverter.GetBytes(RecursionModelLength));
            bytes.AddRange(BitConverter.GetBytes(RecursionModelOffset));
            bytes.Add(Convert.ToByte(BlendModeType));
            bytes.Add(Convert.ToByte(EmitterType));
            bytes.AddRange(BitConverter.GetBytes(ParticleIndex));
            bytes.Add(ParticleType);
            bytes.Add(HeadOrTail);
            bytes.AddRange(BitConverter.GetBytes(TextureTileRotation));
            bytes.AddRange(BitConverter.GetBytes(TextureDimensionsRows));
            bytes.AddRange(BitConverter.GetBytes(TextureDimensionColumns));
            bytes.AddRange(EmissionSpeed.GetHeaderBytes());
            bytes.AddRange(SpeedVariation.GetHeaderBytes());
            bytes.AddRange(VerticalRange.GetHeaderBytes());
            bytes.AddRange(HorizontalRange.GetHeaderBytes());
            bytes.AddRange(Gravity.GetHeaderBytes());
            bytes.AddRange(Lifespan.GetHeaderBytes());
            bytes.AddRange(BitConverter.GetBytes(LifespanVariation));
            bytes.AddRange(EmissionRate.GetHeaderBytes());
            bytes.AddRange(BitConverter.GetBytes(EmissionRateVariation));
            bytes.AddRange(EmissionAreaLength.GetHeaderBytes());
            bytes.AddRange(EmissionAreaWidth.GetHeaderBytes());
            bytes.AddRange(ZSource.GetHeaderBytes());
            bytes.AddRange(ColorTrack.GetHeaderBytes());
            bytes.AddRange(AlphaTrack.GetHeaderBytes());
            bytes.AddRange(ScaleTrack.GetHeaderBytes());
            bytes.AddRange(ScaleVariation.ToBytes());
            bytes.AddRange(HeadCellTrack.GetHeaderBytes());
            bytes.AddRange(TailCellTrack.GetHeaderBytes());
            bytes.AddRange(BitConverter.GetBytes(TailLengthMultiplier));
            bytes.AddRange(BitConverter.GetBytes(TwinkleSpeed));
            bytes.AddRange(BitConverter.GetBytes(TwinklPercent));
            bytes.AddRange(TwinkleRange.ToBytes());
            bytes.AddRange(BitConverter.GetBytes(BurstMultiplier));
            bytes.AddRange(BitConverter.GetBytes(Drag));
            bytes.AddRange(BitConverter.GetBytes(BaseSpin));
            bytes.AddRange(BitConverter.GetBytes(BaseSpinVariation));
            bytes.AddRange(BitConverter.GetBytes(Spin));
            bytes.AddRange(BitConverter.GetBytes(SpinVariation));
            bytes.AddRange(TumbleMin.ToBytes());
            bytes.AddRange(TumbleMax.ToBytes());
            bytes.AddRange(WindVector.ToBytes());
            bytes.AddRange(BitConverter.GetBytes(WindTime));
            bytes.AddRange(BitConverter.GetBytes(FollowSpeed1));
            bytes.AddRange(BitConverter.GetBytes(FollowScale1));
            bytes.AddRange(BitConverter.GetBytes(FollowSpeed2));
            bytes.AddRange(BitConverter.GetBytes(FollowScale2));
            bytes.AddRange(BitConverter.GetBytes(SplinePointCount));
            bytes.AddRange(BitConverter.GetBytes(SplinePointOffset));
            bytes.AddRange(EnabledIn.GetHeaderBytes());
            return bytes;
        }

        public void AddDataBytes(ref List<byte> byteBuffer)
        {
            EmissionSpeed.AddDataBytes(ref byteBuffer);
            SpeedVariation.AddDataBytes(ref byteBuffer);
            VerticalRange.AddDataBytes(ref byteBuffer);
            HorizontalRange.AddDataBytes(ref byteBuffer);
            Gravity.AddDataBytes(ref byteBuffer);
            Lifespan.AddDataBytes(ref byteBuffer);
            EmissionRate.AddDataBytes(ref byteBuffer);
            EmissionAreaLength.AddDataBytes(ref byteBuffer);
            EmissionAreaWidth.AddDataBytes(ref byteBuffer);
            ZSource.AddDataBytes(ref byteBuffer);
            ColorTrack.AddDataBytes(ref byteBuffer);
            AlphaTrack.AddDataBytes(ref byteBuffer);
            ScaleTrack.AddDataBytes(ref byteBuffer);
            HeadCellTrack.AddDataBytes(ref byteBuffer);
            TailCellTrack.AddDataBytes(ref byteBuffer);
            EnabledIn.AddDataBytes(ref byteBuffer);
        }
    }
}
