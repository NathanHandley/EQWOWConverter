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
        // TODO: HERE (Particle Color Replaceable?)

        public UInt32 GetHeaderSize()
        {
            UInt32 size = 0;
            // TODO
            return size;
        }

        public List<byte> GetHeaderBytes()
        {
            List<byte> bytes = new List<byte>();
            // TODO
            return bytes;
        }

        public void AddDataBytes(ref List<byte> byteBuffer)
        {
            // TODO
        }
    }
}
