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
using EQWOWConverter.ObjectModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal class M2Attachment : IOffsetByteSerializable
    {
        private ObjectModelAttachmentType AttachmentType;
        private UInt32 ParentBoneID;
        private Vector3 Position = new Vector3();
        M2TrackSequences<M2Int32> DataTrack;

        public M2Attachment(ObjectModelAttachmentType attachmentType, UInt32 parentBoneID)
        {
            AttachmentType = attachmentType;
            ParentBoneID = parentBoneID;

            ObjectModelTrackSequences<M2Int32> objectModelTrackSequences = new ObjectModelTrackSequences<M2Int32>();
            objectModelTrackSequences.AddSequence();
            objectModelTrackSequences.AddValueToSequence(0, 0, new M2Int32(1));
            DataTrack = new M2TrackSequences<M2Int32>(objectModelTrackSequences);
        }

        public UInt32 GetHeaderSize()
        {
            UInt32 size = 0;
            size += 4; // Attachment ID
            size += 4; // Parent Bone ID
            size += Position.GetBytesSize();
            size += DataTrack.GetHeaderSize();
            return size;
        }

        public List<byte> GetHeaderBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(AttachmentType)));
            bytes.AddRange(BitConverter.GetBytes(ParentBoneID));
            bytes.AddRange(Position.ToBytes());
            bytes.AddRange(DataTrack.GetHeaderBytes());
            return bytes;
        }

        public void AddDataBytes(ref List<byte> byteBuffer)
        {
            DataTrack.AddDataBytes(ref byteBuffer);
        }
    }
}
