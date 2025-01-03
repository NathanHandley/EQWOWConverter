// Author: Nathan Handley(nathanhandley @protonmail.com)
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
using EQWOWConverter.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal class M2TextureAnimation
    {
        ObjectModelTextureAnimation TextureAnimation;
        public M2TrackSequences<Vector3> TranslationTrack;
        public M2TrackSequences<Quaternion> RotationTrack;
        public M2TrackSequences<Vector3> ScaleTrack;

        public M2TextureAnimation(ObjectModelTextureAnimation textureAnimation)
        {
            TextureAnimation = textureAnimation;
            TranslationTrack = new M2TrackSequences<Vector3>(textureAnimation.TranslationTrack);
            RotationTrack = new M2TrackSequences<Quaternion>(textureAnimation.RotationTrack);
            ScaleTrack = new M2TrackSequences<Vector3>(textureAnimation.ScaleTrack);
        }

        public UInt32 GetHeaderSize()
        {
            UInt32 size = 0;
            size += TextureAnimation.TranslationTrack.GetHeaderSize();
            size += TextureAnimation.RotationTrack.GetHeaderSize();
            size += TextureAnimation.ScaleTrack.GetHeaderSize();
            return size;
        }

        public List<byte> GetHeaderBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(TranslationTrack.GetHeaderBytes());
            bytes.AddRange(RotationTrack.GetHeaderBytes());
            bytes.AddRange(ScaleTrack.GetHeaderBytes());
            return bytes;
        }

        public void AddDataBytes(ref List<byte> byteBuffer)
        {
            TranslationTrack.AddDataBytes(ref byteBuffer);
            RotationTrack.AddDataBytes(ref byteBuffer);
            ScaleTrack.AddDataBytes(ref byteBuffer);
        }
    }
}
