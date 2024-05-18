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

using EQWOWConverter.Common;
using EQWOWConverter.ModelObjects.WOW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.ModelObjects
{
    internal class ModelBone
    {
        public Int32 KeyBoneID = -1;
        public ModelBoneFlags Flags = 0;
        public Int16 ParentBone = -1; // Why is this Int16 instead of Int32?
        public UInt16 SubMeshID = 0;
        public UInt32 BoneNameCRC = 0;
        public ModelTrackSequences<Vector3> TranslationTrack = new ModelTrackSequences<Vector3>();
        public ModelTrackSequences<Quaternion> RotationTrack = new ModelTrackSequences<Quaternion>();
        public ModelTrackSequences<Vector3> ScaleTrack = new ModelTrackSequences<Vector3>();
        public Vector3 PivotPoint = new Vector3();

        public ModelBone()
        {
            //// TESTING ONLY!!!!!!!!
            //TranslationTrack.AddValueToSequence(TranslationTrack.AddSequence(), 1, new Vector3(1, 1, 1));
            //RotationTrack.AddValueToSequence(RotationTrack.AddSequence(), 2, new Quaternion(2, 2, 2, 2));
            //RotationTrack.AddValueToSequence(0, 3, new Quaternion(3, 3, 3, 3));
            //RotationTrack.AddValueToSequence(RotationTrack.AddSequence(), 4, new Quaternion(4, 4, 4, 4));
            // REMOVE ME
            // REMOVE ME
            // REMOVE ME
            // REMOVE ME
        }

        public UInt32 GetBytesSize()
        {
            UInt32 size = 0;
            size += 4; // KeyBoneID
            size += 4; // ModelBoneFlags
            size += 2; // ParentBone
            size += 2; // SubMeshID
            size += 4; // BoneNameCRC
            size += TranslationTrack.GetBytesSize();
            size += RotationTrack.GetBytesSize();
            size += ScaleTrack.GetBytesSize();
            size += 12; // PivotPoint
            return size;
        }

        public UInt32 GetHeaderSize()
        {
            UInt32 size = 0;
            size += 4; // KeyBoneID
            size += 4; // ModelBoneFlags
            size += 2; // ParentBone
            size += 2; // SubMeshID
            size += 4; // BoneNameCRC
            size += TranslationTrack.GetHeaderSize();
            size += RotationTrack.GetHeaderSize();
            size += ScaleTrack.GetHeaderSize();
            size += 12; // PivotPoint
            return size;
        }

        public List<byte> GetHeaderBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(KeyBoneID));
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(Flags)));
            bytes.AddRange(BitConverter.GetBytes(ParentBone));
            bytes.AddRange(BitConverter.GetBytes(SubMeshID));
            bytes.AddRange(BitConverter.GetBytes(BoneNameCRC));
            bytes.AddRange(TranslationTrack.GetHeaderBytes());
            bytes.AddRange(RotationTrack.GetHeaderBytes());
            bytes.AddRange(ScaleTrack.GetHeaderBytes());
            bytes.AddRange(PivotPoint.ToBytes());
            return bytes;
        }

        public void AddDataToByteBufferAndUpdateHeader(ref UInt32 workingCursorOffset, ref List<Byte> byteBuffer)
        {
            TranslationTrack.AddDataToByteBufferAndUpdateHeader(ref workingCursorOffset, ref byteBuffer);
            RotationTrack.AddDataToByteBufferAndUpdateHeader(ref workingCursorOffset, ref byteBuffer);
            ScaleTrack.AddDataToByteBufferAndUpdateHeader(ref workingCursorOffset, ref byteBuffer);
        }
    }
}
