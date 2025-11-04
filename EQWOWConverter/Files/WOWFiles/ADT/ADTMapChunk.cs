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

namespace EQWOWConverter.WOWFiles
{
    internal class ADTMapChunk : WOWChunkedObject
    {
        private UInt32 IndexX;
        private UInt32 IndexY;
        private float HeightLevel;
        private UInt32 AreaID;
        private List<byte> MCVTBytes = new List<byte>();
        private List<byte> MCNRBytes = new List<byte>();
        private List<byte> MCLYBytes = new List<byte>();
        private List<byte> MCRFBytes = new List<byte>();
        private List<byte> MCALBytes = new List<byte>();
        private List<byte> MCSHBytes = new List<byte>();

        public ADTMapChunk(UInt32 xIndex, UInt32 yIndex, float baseHeight, UInt32 areaID)
        {
            IndexX = xIndex;
            IndexY = yIndex;
            HeightLevel = baseHeight;
            AreaID = areaID;

            // MCVT (Height values)
            // Making it flat
            for (int i = 0; i < 145; i++)
                MCVTBytes.AddRange(BitConverter.GetBytes(baseHeight));

            // MCNR (Height normals)
            // Making everything (0,0,127)
            for (int i = 0; i < 145; i++)
            {
                MCNRBytes.Add(0);
                MCNRBytes.Add(0);
                MCNRBytes.Add(127);
            }

            // MCLY (Texture layer definitions)
            // None, since we won't render it (Texture ID + Flags + EffectID + AlphaOffset)
            MCLYBytes = WrapInChunk("MCLY", MCLYBytes.ToArray());

            // MCRF?

            // MCAL?

            // MCSH: 64 bytes of 0
            MCSHBytes.AddRange(new byte[64]);
        }

        public List<byte> GetChunkDataBytes()
        {
            List<byte> chunkBytes = new List<byte>();

            // Initial header section
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0))); // Flags (none)
            chunkBytes.AddRange(BitConverter.GetBytes(IndexX));
            chunkBytes.AddRange(BitConverter.GetBytes(IndexY));
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0))); // Number of Layers
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0))); // Number of doodad references

            // Offset starts at 128 due to the header data
            uint curOffset = 128;

            UInt32 MCVTOffset = curOffset;
            chunkBytes.AddRange(BitConverter.GetBytes(MCVTOffset));

            UInt32 MCNROffset = MCVTOffset + (uint)MCVTBytes.Count;
            chunkBytes.AddRange(BitConverter.GetBytes(MCNROffset));

            UInt32 MCLYOffset = MCNROffset + (uint)MCNRBytes.Count;
            chunkBytes.AddRange(BitConverter.GetBytes(MCLYOffset));

            UInt32 MCRFOffset = MCLYOffset + (uint)MCLYBytes.Count;
            chunkBytes.AddRange(BitConverter.GetBytes(MCRFOffset));

            UInt32 MCALOffset = MCRFOffset + (uint)MCRFBytes.Count;
            chunkBytes.AddRange(BitConverter.GetBytes(MCALOffset));

            UInt32 MCALSize = (uint)MCALBytes.Count;
            chunkBytes.AddRange(BitConverter.GetBytes(MCALSize));

            UInt32 MCSHOffset = MCALOffset + MCALSize;
            chunkBytes.AddRange(BitConverter.GetBytes(MCSHOffset));

            UInt32 MCSHSize = (uint)MCSHBytes.Count;
            chunkBytes.AddRange(BitConverter.GetBytes(MCSHSize));

            chunkBytes.AddRange(BitConverter.GetBytes(AreaID));
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0))); // Number of Map Object References
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0))); // Number of holes (TODO: Consider making map all holes?)

            byte[] reallyLowResTextureMap = new byte[16];
            chunkBytes.AddRange(reallyLowResTextureMap);

            byte[] predTex = new byte[8];
            chunkBytes.AddRange(predTex);

            byte[] noEffectDoodad = new byte[8];
            chunkBytes.AddRange(noEffectDoodad);

            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0))); // MCSE (Sound Emitters) - Offset
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0))); // MCSE (Sound Emitters) - Size
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0))); // MCLQ Offset (not used)
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(8))); // MCLQ Size, set to 8 when not used (which then uses MH20 chunk)
            chunkBytes.AddRange(BitConverter.GetBytes(HeightLevel));
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0))); // Unknown / Pad
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0))); // MCLV Offset (?)
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0))); // Unknown / Pad

            // TODO: More padding for header?

            // Add subchunk data
            chunkBytes.AddRange(MCVTBytes);
            chunkBytes.AddRange(MCNRBytes);
            chunkBytes.AddRange(MCLYBytes);
            chunkBytes.AddRange(MCRFBytes);
            chunkBytes.AddRange(MCALBytes);
            chunkBytes.AddRange(MCSHBytes);

            return chunkBytes;
        }
    }
}
