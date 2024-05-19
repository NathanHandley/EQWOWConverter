//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2024 Nathan Handley
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

using EQWOWConverter.ModelObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal class M2Bone
    {
        public ModelBone Bone;

        public M2Bone(ModelBone modelBone)
        {
            Bone = modelBone;
        }

        public UInt32 GetHeaderSize()
        {
            UInt32 size = 0;
            size += 4; // KeyBoneID
            size += 4; // ModelBoneFlags
            size += 2; // ParentBone
            size += 2; // SubMeshID
            size += 4; // BoneNameCRC
            size += Bone.TranslationTrack.GetHeaderSize();
            size += Bone.RotationTrack.GetHeaderSize();
            size += Bone.ScaleTrack.GetHeaderSize();
            size += 12; // PivotPoint
            return size;
        }

        public List<byte> GetHeaderBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Bone.KeyBoneID));
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(Bone.Flags)));
            bytes.AddRange(BitConverter.GetBytes(Bone.ParentBone));
            bytes.AddRange(BitConverter.GetBytes(Bone.SubMeshID));
            bytes.AddRange(BitConverter.GetBytes(Bone.BoneNameCRC));
            bytes.AddRange(Bone.TranslationTrack.GetHeaderBytes());
            bytes.AddRange(Bone.RotationTrack.GetHeaderBytes());
            bytes.AddRange(Bone.ScaleTrack.GetHeaderBytes());
            bytes.AddRange(Bone.PivotPoint.ToBytes());
            return bytes;
        }

        public void AddDataToByteBufferAndUpdateHeader(UInt32 workingCursorOffset, ref List<Byte> byteBuffer)
        {
            Bone.TranslationTrack.AddDataToByteBufferAndUpdateHeader(ref workingCursorOffset, ref byteBuffer);
            Bone.RotationTrack.AddDataToByteBufferAndUpdateHeader(ref workingCursorOffset, ref byteBuffer);
            Bone.ScaleTrack.AddDataToByteBufferAndUpdateHeader(ref workingCursorOffset, ref byteBuffer);
        }

    }
}
