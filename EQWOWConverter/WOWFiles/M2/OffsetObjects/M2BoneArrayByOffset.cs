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
using System.Xml.Linq;

namespace EQWOWConverter.WOWFiles
{
    internal class M2BoneArrayByOffset : OffsetByteSerializable
    {
        private UInt32 Count = 0;
        private UInt32 Offset = 0;
        private List<M2Bone> Bones = new List<M2Bone>();

        public void AddModelBones(List<ModelBone> bones)
        {
            foreach(ModelBone bone in bones)
            {
                Bones.Add(new M2Bone(bone));
            }
            Count = Convert.ToUInt32(Bones.Count);
        }

        public List<Byte> GetHeaderBytes()
        {
            List<byte> returnBytes = new List<byte>();
            returnBytes.AddRange(BitConverter.GetBytes(Count));
            returnBytes.AddRange(BitConverter.GetBytes(Offset));
            return returnBytes;
        }

        public void AddDataBytes(ref List<byte> byteBuffer)
        {
            // Update header bytes
            if (Count == 0)
            {
                Offset = 0;
                return;
            }
            Offset = Convert.ToUInt32(byteBuffer.Count);

            // Determine space needed for all bone headers and reserve it
            UInt32 allBoneSubHeaderSize = 0;
            foreach (M2Bone bone in Bones)
                allBoneSubHeaderSize += bone.GetHeaderSize();
            for (int i = 0; i < allBoneSubHeaderSize; i++)
                byteBuffer.Add(0);

            // Add all of the data
            foreach (M2Bone bone in Bones)
                bone.AddDataBytes(ref byteBuffer);

            // Write the header data
            List<byte> headerBytes = new List<byte>();
            foreach (M2Bone bone in Bones)
                headerBytes.AddRange(bone.GetHeaderBytes());
            for (int i = 0; i < allBoneSubHeaderSize; i++)
                byteBuffer[i + Convert.ToInt32(Offset)] = headerBytes[i];
        }
    }
}
