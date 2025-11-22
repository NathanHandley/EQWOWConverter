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
        private UInt32 ChunkXIndex;
        private UInt32 ChunkYIndex;
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
        private UInt32 MCSHOffset = 0;
        private List<byte> MCSHBytes = new List<byte>();

        public ADTMapChunk(UInt32 tileXIndex, UInt32 tileYIndex, UInt32 chunkXIndex, UInt32 chunkYIndex, float baseHeight, UInt32 areaID)
        {
            ChunkXIndex = chunkXIndex;
            ChunkYIndex = chunkYIndex;
            AreaID = areaID;

            // MCVT (Height values)
            // Making it flat
            for (int i = 0; i < 145; i++)
                MCVTBytes.AddRange(BitConverter.GetBytes(0));
            MCVTBytes = WrapInChunk("MCVT", MCVTBytes.ToArray());

            // MCNR (Height normals)
            // Making everything (0,0,127)
            for (int i = 0; i < 145; i++)
            {
                MCNRBytes.Add(0);
                MCNRBytes.Add(0);
                MCNRBytes.Add(127);
            }
            // This padding is unknown what it's for
            List<byte> unknownPadding = new List<byte> { 0, 112, 245, 18, 0, 8, 0, 0, 0, 84, 245, 18, 0 };
            MCNRBytes.AddRange(unknownPadding.ToArray());
            MCNRBytes = WrapInChunk("MCNR", MCNRBytes.ToArray());

            // MCLY (Texture layer definitions)
            // None, since we won't render it (Texture ID + Flags + EffectID + AlphaOffset)
            MCLYBytes = WrapInChunk("MCLY", MCLYBytes.ToArray());

            // MCRF (References?)
            MCRFBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
            MCRFBytes = WrapInChunk("MCRF", MCRFBytes.ToArray());

            // MCAL (Alpha Maps for additional texture layers)
            // None
            MCALBytes = WrapInChunk("MCAL", MCALBytes.ToArray());

            // MCSH (shadow map)
            MCSHBytes.AddRange(new byte[512]);
            MCSHBytes = WrapInChunk("MCSH", MCSHBytes.ToArray());

            // MSCE (Sound Emitters)
            // None
            MCSEBytes = WrapInChunk("MCSE", MCSEBytes.ToArray());

            // MCLQ (Liquids)
            // None
            MCLQBytes = WrapInChunk("MCLQ", MCLQBytes.ToArray());

            // Calculate position
            // Chunks in a tile span 0-533.3333 on each axis
            // tileIndex between 31 and 32 is zero, with lower going positive
            // X <=> Y (seems x/y are inverted)
            float step = 100f / 3f; // Ensures proper repeating 33.33333....
            float startY = (32f - tileYIndex) * (16f * step);
            float startX = (32f - tileXIndex) * (16f * step);
            float xPosition = startX + (chunkXIndex * step * -1f);
            float yPosition = startY + (chunkYIndex * step * -1f);
            float zPosition = baseHeight;
            Position = new Vector3(yPosition, xPosition, zPosition); // Intentionally inverted x/y
        }

        private List<byte> GetHeaderBytes()
        {
            List<byte> headerBytes = new List<byte>();

            // Flags (none)
            headerBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            // IndexX
            headerBytes.AddRange(BitConverter.GetBytes(ChunkXIndex));

            // IndexY
            headerBytes.AddRange(BitConverter.GetBytes(ChunkYIndex));

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

            // Offset into shadow data
            headerBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(MCSHOffset)));

            // Size of shadow
            headerBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(512)));

            // AreaID
            headerBytes.AddRange(BitConverter.GetBytes(AreaID));

            // Number of Map Object References
            headerBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(1)));

            // Number of holes (not using terrain, so just making it a hole) unless in creature debug mode
            if (Configuration.CREATURE_SPAWN_AND_WAYPOINT_DEBUG_MODE == true)
                headerBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
            else
                headerBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(65535)));

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

            // Size of Liquids (Setting to 8 has it not use MCLQ, and fallback to MH20)
            headerBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(8)));

            // Position
            headerBytes.AddRange(Position.ToBytes());

            // Offset into MCCV (not using that block)
            headerBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            // Two unused blocks
            headerBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
            headerBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            return headerBytes;
        }

        private List<byte> GetNonHeaderBytes()
        {
            List<byte> nonHeaderBytes = new List<byte>();

            // Offset starts at +136 (+128 chunk header, +8 magic header)
            UInt32 nonHeaderStartOffset = 136;

            // Height data
            MCVTOffset = nonHeaderStartOffset;
            nonHeaderBytes.AddRange(MCVTBytes);

            // Normals data
            MCNROffset = MCVTOffset + (UInt32)MCVTBytes.Count; 
            nonHeaderBytes.AddRange(MCNRBytes);

            // Layer data
            MCLYOffset = MCNROffset + (UInt32)MCNRBytes.Count;
            nonHeaderBytes.AddRange(MCLYBytes);

            // Doodad reference data
            MCRFOffset = MCLYOffset + (UInt32)MCLYBytes.Count;
            nonHeaderBytes.AddRange(MCRFBytes);

            // Shadow data
            MCSHOffset = MCRFOffset + (UInt32)MCRFBytes.Count;
            nonHeaderBytes.AddRange(MCSHBytes);

            // Layer alpha map data
            MCALOffset = MCSHOffset + (UInt32)MCSHBytes.Count;
            nonHeaderBytes.AddRange(MCALBytes);

            // Liquid data
            MCLQOffset = MCALOffset + (UInt32)MCALBytes.Count;
            nonHeaderBytes.AddRange(MCLQBytes);

            // Sound emitter data
            MCSEOffset = MCLQOffset + (UInt32)MCLQBytes.Count;
            nonHeaderBytes.AddRange(MCSEBytes);

            return nonHeaderBytes;
        }

        public List<byte> GetDataBytes()
        {
            List<Byte> dataBytes = new List<Byte>();

            // Must generate the body bytes first since that sets the offsets in the header
            List<byte> bodyBytes = GetNonHeaderBytes();
            List<byte> headerBytes = GetHeaderBytes();

            dataBytes.AddRange(headerBytes.ToArray());
            dataBytes.AddRange(bodyBytes.ToArray());

            return WrapInChunk("MCNK", dataBytes.ToArray());
        }
    }
}
