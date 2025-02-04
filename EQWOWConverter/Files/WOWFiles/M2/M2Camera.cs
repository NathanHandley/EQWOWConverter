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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal class M2Camera : IOffsetByteSerializable
    {
        private UInt32 CameraType = 0; // 0 = portrait
        private float DiagonalFOV = 0.950022f;
        private float FarClip = 27.77778f;
        private float NearClip = 0.2222222f;
        private Vector3 Position = new Vector3();
        private Vector3 Target = new Vector3();

        public M2TrackSequences<SplineKey> TranslationPosition;
        public M2TrackSequences<SplineKey> TranslationTarget;
        public M2TrackSequences<M2Float> RollEffect;

        public M2Camera(Vector3 position, Vector3 target)
        {
            // Translation Position
            ObjectModelTrackSequences<SplineKey> translationPositionTrack = new ObjectModelTrackSequences<SplineKey>();
            translationPositionTrack.AddSequence();
            translationPositionTrack.AddValueToSequence(0, 0, new SplineKey());
            TranslationPosition = new M2TrackSequences<SplineKey>(translationPositionTrack);

            // Translation Target
            ObjectModelTrackSequences<SplineKey> translationTargetTrack = new ObjectModelTrackSequences<SplineKey>();
            translationTargetTrack.AddSequence();
            translationTargetTrack.AddValueToSequence(0, 0, new SplineKey());
            TranslationTarget = new M2TrackSequences<SplineKey>(translationTargetTrack);

            // RollEffect
            ObjectModelTrackSequences<M2Float> rollEffect = new ObjectModelTrackSequences<M2Float>();
            rollEffect.AddSequence();
            rollEffect.AddValueToSequence(0, 0, new M2Float(0f));
            RollEffect = new M2TrackSequences<M2Float>(rollEffect);

            // Set the position and target
            Position = position;
            Target = target;
        }

        public UInt32 GetHeaderSize()
        {
            UInt32 size = 0;
            size += 4; // CameraType
            size += 4; // DiagonalFOV
            size += 4; // FarClip
            size += 4; // NearClip
            size += TranslationPosition.GetHeaderSize();
            size += Position.GetBytesSize();
            size += TranslationTarget.GetHeaderSize();
            size += Target.GetBytesSize();
            size += RollEffect.GetHeaderSize();
            return size;
        }

        public List<byte> GetHeaderBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(CameraType));
            bytes.AddRange(BitConverter.GetBytes(DiagonalFOV));
            bytes.AddRange(BitConverter.GetBytes(FarClip));
            bytes.AddRange(BitConverter.GetBytes(NearClip));
            bytes.AddRange(TranslationPosition.GetHeaderBytes());
            bytes.AddRange(Position.ToBytes());
            bytes.AddRange(TranslationTarget.GetHeaderBytes());
            bytes.AddRange(Target.ToBytes());
            bytes.AddRange(RollEffect.GetHeaderBytes());
            return bytes;
        }

        public void AddDataBytes(ref List<byte> byteBuffer)
        {
            TranslationPosition.AddDataBytes(ref byteBuffer);
            TranslationTarget.AddDataBytes(ref byteBuffer);
            RollEffect.AddDataBytes(ref byteBuffer);
        }
    }
}
