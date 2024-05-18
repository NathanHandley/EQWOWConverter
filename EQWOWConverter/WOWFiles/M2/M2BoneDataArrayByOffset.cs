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
using EQWOWConverter.ModelObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal class M2BoneDataArrayByOffset
    {
        private UInt32 Count = 0;
        private UInt32 Offset = 0;
        private List<ModelBone> Bones = new List<ModelBone>();

        public void AddBones(List<ModelBone> bones)
        {
            Bones = bones;
        }

        public List<Byte> GetHeaderBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Count));
            if (Count == 0)
                bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
            else
                bytes.AddRange(BitConverter.GetBytes(Offset));
            return bytes;
        }

        // The data is arranged such that all bone headers are grouped together, then the data space is after
        public void AddDataToByteBufferAndUpdateHeader(ref UInt32 workingCursorOffset, ref List<Byte> byteBuffer)
        {
            // Update header values
            Offset = workingCursorOffset;
            Count = Convert.ToUInt32(Bones.Count);
            if (Count == 0)
                return;            

            // Reserve the space for all of the individual bone header data
            UInt32 allBoneSubHeaderSize = 0;
            foreach (ModelBone bone in Bones)
                allBoneSubHeaderSize += bone.GetHeaderSize();
            
            // Generate the data for each bone onto the byte buffer
            List<byte> dataForAllBonesNonHeader = new List<byte>();
            UInt32 workingSubCursorOffset = allBoneSubHeaderSize + workingCursorOffset;
            foreach (ModelBone bone in Bones)
                bone.AddDataToByteBufferAndUpdateHeader(ref workingSubCursorOffset, ref dataForAllBonesNonHeader);

            // Generate the header data and add to bytebuffer
            foreach (ModelBone bone in Bones)
                byteBuffer.AddRange(bone.GetHeaderBytes());
            workingCursorOffset += allBoneSubHeaderSize;

            // Add the actual data
            byteBuffer.AddRange(dataForAllBonesNonHeader);

            // Update cursor
            workingCursorOffset += Convert.ToUInt32(dataForAllBonesNonHeader.Count);
        }
    }
}
