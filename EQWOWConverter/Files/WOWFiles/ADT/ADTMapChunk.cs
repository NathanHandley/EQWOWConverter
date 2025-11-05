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

namespace EQWOWConverter.WOWFiles
{
    internal class ADTMapChunk : WOWChunkedObject
    {
        private UInt32 IndexX;
        private UInt32 IndexY;
        private float HeightLevel;
        private UInt32 AreaID;
        private Vector3 Position;
        private UInt32 MCVTOffset = 0;
        private List<byte> MCVTBytes = new List<byte>();
        private UInt32 MCNROffset = 0;
        private List<byte> MCNRBytes = new List<byte>();
        private UInt32 MCLYOffset = 0;
        private List<byte> MCLYBytes = new List<byte>();
        private UInt32 MCRFOffset = 0;
        private List<byte> MCRFBytes = new List<byte>();
        private UInt32 MCALOffset = 0;
        private List<byte> MCALBytes = new List<byte>();
        private UInt32 MCLQOffset = 0;
        private List<byte> MCLQBytes = new List<byte>();
        private UInt32 MCSEOffset = 0;
        private List<byte> MCSEBytes = new List<byte>();

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

            // MCRF (References?)
            // None
            MCRFBytes = WrapInChunk("MCRF", MCLYBytes.ToArray());

            // MCAL (Alpha Maps for additional texture layers)
            // None
            MCALBytes = WrapInChunk("MCAL", MCALBytes.ToArray());

            //// MCSH: 64 bytes of 0
            //MCSHBytes.AddRange(new byte[64]);

            // MSCE (Sound Emitters)
            // None
            MCSEBytes = WrapInChunk("MCSE", MCSEBytes.ToArray());

            // MCLQ (Liquids)
            // None
            MCLQBytes = WrapInChunk("MCLQ", MCLQBytes.ToArray());

            // TODO: Figure out how to calculate position
            // MonasteryInstances_29_28.adt has row[0] mcnk[0] as 2133.333, 1600, 0
            // MonasteryInstances_29_28.adt has row[0] mcnk[1] as 2133.333, 1566.667, 0
            // MonasteryInstances_29_28.adt has row[0] mcnk[2] as 2133.333, 1533.333, 0
            // MonasteryInstances_29_28.adt has row[1] mcnk[0] as 2100, 1600, 0
            // MonasteryInstances_29_28.adt has row[2] mcnk[1] as 2100, 1566.667, 0
            // MonasteryInstances_29_28.adt has row[3] mcnk[2] as 2100, 1533.333, 0
            // ...
            // MonasteryInstances_29_28.adt has row[15] mcnk[15] as 1633.333, 1100, 0
            Position = new Vector3(0, 0, 0);
        }

        public List<byte> GetHeaderBytes()
        {
            List<byte> headerBytes = new List<byte>();

            // Flags (none)
            headerBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            // IndexX
            headerBytes.AddRange(BitConverter.GetBytes(IndexX));

            // IndexY
            headerBytes.AddRange(BitConverter.GetBytes(IndexY));

            // Number of Layers
            headerBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            // Number of doodad references
            headerBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            // Offset into height data
            headerBytes.AddRange(BitConverter.GetBytes(MCVTOffset));

            // Offset into normal data
            headerBytes.AddRange(BitConverter.GetBytes(MCNROffset));

            // Offset into layer data
            headerBytes.AddRange(BitConverter.GetBytes(MCLYOffset));

            // Offset into doodad reference data
            headerBytes.AddRange(BitConverter.GetBytes(MCRFOffset));

            // Offset into layer alpha map data
            headerBytes.AddRange(BitConverter.GetBytes(MCALOffset));

            // Size of alpha (8 because blank)
            headerBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(8)));

            // Offset into shadow data (making zero because it doesn't exist - May cause issue)
            headerBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            // Size of shadow (should be zero, but I see 512 in MonasteryInstances_29_28.adt even though the flag is blank)
            headerBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            // AreaID
            headerBytes.AddRange(BitConverter.GetBytes(AreaID));

            // Number of Map Object References
            headerBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            // Number of holes (should this be 0 and the whole map is holes?)
            headerBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            // 4 Unknown values
            headerBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
            headerBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
            headerBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
            headerBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            // "PredTex" (determines the detail doodads will show).  Not using it
            headerBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            // No Effect Doodads (disable doodads with 1, ignore it)
            headerBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            // Offset into Sound emitters
            headerBytes.AddRange(BitConverter.GetBytes(MCSEOffset));

            // Number of sound emitters (forcing zero)
            headerBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            // Offset into liquids section
            headerBytes.AddRange(BitConverter.GetBytes(MCLQOffset));

            // Position
            headerBytes.AddRange(Position.ToBytes());

            // Offset into MCCV (not using that block)
            headerBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            // Two unused blocks
            headerBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
            headerBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            return headerBytes;
        }



        private List<byte> GetNonHeaderBytes(UInt32 mapChunkStartOffset)
        {
            List<byte> nonHeaderBytes = new List<byte>();

            // Offset starts at +128 due to the header data size
            UInt32 nonHeaderStartOffset = 128 + mapChunkStartOffset;

            // Height data
            MCVTOffset = nonHeaderStartOffset;
            nonHeaderBytes.AddRange(MCVTBytes);

            // Normals data
            MCNROffset = MCVTOffset + (uint)MCVTBytes.Count; 
            nonHeaderBytes.AddRange(MCNRBytes);

            // Layer data
            MCLYOffset = MCNROffset + (uint)MCNRBytes.Count;
            nonHeaderBytes.AddRange(MCLYBytes);

            // Doodad reference data
            MCRFOffset = MCLYOffset + (uint)MCLYBytes.Count;
            nonHeaderBytes.AddRange(MCRFBytes);

            // Note: MonasteryInstances_29_28.adt has MCSH data block here, but flag is 0.  Revisit if issues.

            // Layer alpha map data
            MCALOffset = MCRFOffset + (uint)MCRFBytes.Count;
            nonHeaderBytes.AddRange(MCALBytes);

            // Liquid data
            MCLQOffset = MCALOffset + (uint)MCRFBytes.Count;
            nonHeaderBytes.AddRange(MCLQBytes);

            // Sound emitter data
            MCSEOffset = MCLQOffset + (uint)MCRFBytes.Count;
            nonHeaderBytes.AddRange(MCSEBytes);

            return nonHeaderBytes;
        }




        public List<byte> GetDataBytes()
        {
            List<>
            //List<byte> chunkBytes = new List<byte>();
            List<byte> nonHeaderBytes = new List<byte>();

            ///////////////////
            // Non-Header bytes

                        // Initial header section
            
            

            

            
            

            

            

            UInt32 MCALSize = (uint)MCALBytes.Count;
            chunkBytes.AddRange(BitConverter.GetBytes(MCALSize));

            UInt32 MCSHOffset = MCALOffset + MCALSize;
            chunkBytes.AddRange(BitConverter.GetBytes(MCSHOffset));

            UInt32 MCSHSize = (uint)MCSHBytes.Count;
            chunkBytes.AddRange(BitConverter.GetBytes(MCSHSize));

            chunkBytes.AddRange(BitConverter.GetBytes(AreaID));
            

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
