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

using EQWOWConverter.ObjectModels;
using EQWOWConverter.Zones;

namespace EQWOWConverter.WOWFiles
{
    internal class M2TextureAnimationArrayByOffset : IOffsetByteSerializable
    {
        private UInt32 Count = 0;
        private UInt32 Offset = 0;
        private List<M2TextureAnimation> TextureAnimations = new List<M2TextureAnimation>();

        public void AddModelTextureAnimations(List<ObjectModelTextureAnimation> textureAnimations)
        {
            foreach (ObjectModelTextureAnimation textureAnimation in textureAnimations)
            {
                TextureAnimations.Add(new M2TextureAnimation(textureAnimation));
            }
            Count = Convert.ToUInt32(TextureAnimations.Count);
        }

        public UInt32 GetHeaderSize()
        {
            UInt32 size = 4; // Count
            size += 4; // Offset
            return size;
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

            // Determine space needed for all texture animation headers and reserve it
            UInt32 allTextureAnimSubHeaderSize = 0;
            foreach (M2TextureAnimation textureAnimation in TextureAnimations)
                allTextureAnimSubHeaderSize += textureAnimation.GetHeaderSize();
            for (int i = 0; i < allTextureAnimSubHeaderSize; i++)
                byteBuffer.Add(0);

            // Add all of the data
            foreach (M2TextureAnimation textureAnimation in TextureAnimations)
                textureAnimation.AddDataBytes(ref byteBuffer);

            // Write the header data
            List<byte> headerBytes = new List<byte>();
            foreach (M2TextureAnimation textureAnimation in TextureAnimations)
                headerBytes.AddRange(textureAnimation.GetHeaderBytes());
            for (int i = 0; i < allTextureAnimSubHeaderSize; i++)
                byteBuffer[i + Convert.ToInt32(Offset)] = headerBytes[i];
        }
    }
}
