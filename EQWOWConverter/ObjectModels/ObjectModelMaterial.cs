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

namespace EQWOWConverter.ObjectModels
{
    internal class ObjectModelMaterial : IByteSerializable
    {
        public ObjectModelMaterialFlag Flags = ObjectModelMaterialFlag.None;
        public ObjectModelMaterialBlendType BlendingMode;
        public Material Material;
        public Int16 ColorIndex = -1;

        public ObjectModelMaterial(Material material, ObjectModelMaterialBlendType blendType)
        {
            Material = new Material(material);
            BlendingMode = blendType;
            if (blendType == ObjectModelMaterialBlendType.Add || material.AlwaysBrightOverride == true)
                Flags |= ObjectModelMaterialFlag.Unlit;
            if (blendType == ObjectModelMaterialBlendType.Add || material.GetTransparencyValue() != Int16.MaxValue)
                Flags |= ObjectModelMaterialFlag.DepthWrite;
        }

        public UInt32 GetBytesSize()
        {
            return 4;
        }

        public List<byte> ToBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(Flags)));
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(BlendingMode)));
            return bytes;
        }
    }
}
