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
        public class ModelBoneTimeTrack<SerializableObject>
        {
            ModelAnimationInterpolationType InterpolationType = ModelAnimationInterpolationType.None;
            public UInt16 GlobalSequenceID = 65535;
            public UInt32 TimestampsOffset = 0;
            public UInt32 ValuesOffset = 0;
            public List<UInt32> Timestamps = new List<UInt32>();
            public List<ByteSerializable> Values = new List<ByteSerializable>();

            public List<byte> TrackMetaToBytes()
            {
                List<byte> bytes = new List<byte>();
                bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(InterpolationType)));
                bytes.AddRange(BitConverter.GetBytes(GlobalSequenceID));
                bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(Timestamps.Count)));
                bytes.AddRange(BitConverter.GetBytes(TimestampsOffset));
                bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(Values.Count)));
                bytes.AddRange(BitConverter.GetBytes(ValuesOffset));
                return bytes;
            }
            public List<byte> TimestampsToBytes(ref int curOffset)
            {
                TimestampsOffset = Convert.ToUInt32(curOffset);
                List<byte> bytes = new List<byte>();
                foreach (UInt32 timestamp in Timestamps)
                    bytes.AddRange(BitConverter.GetBytes(timestamp));
                curOffset += bytes.Count;
                return bytes;
            }
            public List<byte> ValuesToBytes(ref int curOffset)
            {
                ValuesOffset = Convert.ToUInt32(curOffset);
                List<byte> bytes = new List<byte>();
                foreach (ByteSerializable value in Values)
                    bytes.AddRange(value.ToBytes());
                curOffset += bytes.Count;
                return bytes;
            }
            public int GetMetaSize()
            {
                int size = 0;
                size += 2; // InterpolationType
                size += 2; // GlobalSequenceID
                size += 4; // TimestampsCount
                size += 4; // TimestampsOffset
                size += 4; // ValuesCount
                size += 4; // ValuesOffset
                return size;
            }
        }

        public Int32 KeyBoneID = -1;
        public ModelBoneFlags Flags = 0;
        public Int16 ParentBone = -1; // Why is this Int16 instead of Int32?
        public UInt16 SubMeshID = 0;
        public UInt32 BoneNameCRC = 300463684;  // Looks like this isn't used, so copied value in CaveMineSpiderPillar01. Revisit
        public ModelBoneTimeTrack<Vector3> TranslationTrack = new ModelBoneTimeTrack<Vector3>();
        public ModelBoneTimeTrack<Quaternion> RotationTrack = new ModelBoneTimeTrack<Quaternion>();
        public ModelBoneTimeTrack<Vector3> ScaleTrack = new ModelBoneTimeTrack<Vector3>();
        public Vector3 PivotPoint = new Vector3();

        public ModelBone()
        {
        }

        public void AddTranslation(UInt32 timestamp, Vector3 translation)
        {
            TranslationTrack.Timestamps.Add(timestamp);
            TranslationTrack.Values.Add(translation);
        }

        public void AddRotation(UInt32 timestamp, Quaternion rotation)
        {
            RotationTrack.Timestamps.Add(timestamp);
            RotationTrack.Values.Add(rotation);
        }

        public void AddScale(UInt32 timestamp, Vector3 scale)
        {
            ScaleTrack.Timestamps.Add(timestamp);
            ScaleTrack.Values.Add(scale);
        }

        private int GetMetaSize()
        {
            int size = 0;
            size += 4; // KeyBoneID
            size += 4; // ModelBoneFlags
            size += 2; // ParentBone
            size += 2; // SubMeshID
            size += 4; // BoneNameCRC
            size += TranslationTrack.GetMetaSize();
            size += RotationTrack.GetMetaSize();
            size += ScaleTrack.GetMetaSize();
            size += 12; // PivotPoint
            return size;
        }

        public List<byte> ToBytes(ref int curOffset)
        {
            List<byte> tracksBytes = new List<byte>();
            curOffset += GetMetaSize();
            tracksBytes.AddRange(TranslationTrack.TimestampsToBytes(ref curOffset));
            tracksBytes.AddRange(TranslationTrack.ValuesToBytes(ref curOffset));
            tracksBytes.AddRange(RotationTrack.TimestampsToBytes(ref curOffset));
            tracksBytes.AddRange(RotationTrack.ValuesToBytes(ref curOffset));
            tracksBytes.AddRange(ScaleTrack.TimestampsToBytes(ref curOffset));
            tracksBytes.AddRange(ScaleTrack.ValuesToBytes(ref curOffset));

            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(KeyBoneID));
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(Flags)));
            bytes.AddRange(BitConverter.GetBytes(ParentBone));
            bytes.AddRange(BitConverter.GetBytes(SubMeshID));
            bytes.AddRange(BitConverter.GetBytes(BoneNameCRC));
            bytes.AddRange(TranslationTrack.TrackMetaToBytes());
            bytes.AddRange(RotationTrack.TrackMetaToBytes());
            bytes.AddRange(ScaleTrack.TrackMetaToBytes());
            bytes.AddRange(PivotPoint.ToBytes());
            bytes.AddRange(tracksBytes);
            return bytes;
        }
    }
}
