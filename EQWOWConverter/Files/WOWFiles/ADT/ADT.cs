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

using System.Text;
using EQWOWConverter.Common;
using EQWOWConverter.Zones;

namespace EQWOWConverter.WOWFiles
{
    internal class ADT : WOWChunkedObject
    {
        public ADT(Zone zone, string wmoFileName)
        {
            // MVER (Version)
            List<byte> versionBytes = GenerateMVERChunk();

            // MTEX (Textures) - Placeholder Texture
            List<byte> textureChunkBytes = GenerateMTEXChunk("Tileset\\BurningStepps\\BurningSteppsRock02.blp\0");

            // MWMO (WMO file names)
            List<byte> wmoNameChunkBytes = GenerateMWMOChunk(wmoFileName);

            // MODF (WMO placement information)
            List<byte> wmoPlacementInformationBytes = GenerateMODFChunk(zone);

            // MCIN (Map Chunk Infos)
            List<ADTMapChunkInfo> mapChunkInfos = new List<ADTMapChunkInfo>();
            List<byte> mapChunkInfoBytes = GenerateMCINChunk(mapChunkInfos);

            // MCNK (Map Chunks)
            float zoneBaseHeight = 0f; // TODO: Map this to something, maybe from ZoneProperties
            List<ADTMapChunk> mapChunks = new List<ADTMapChunk>();
            for (UInt16 y = 0; y < 16; y++)
                for (UInt16 x = 0; x < 16; x++)
                    mapChunks.Add(new ADTMapChunk(x, y, zoneBaseHeight, zone.DefaultArea.DBCAreaTableID));

            // MHDR (Header)
            List<byte> headerBytes = GenerateMOHDChunk(); // TODO
        }

        /// <summary>
        /// MVER (Version)
        /// </summary>
        private List<byte> GenerateMVERChunk()
        {
            UInt32 version = 18;
            return WrapInChunk("MVER", BitConverter.GetBytes(version));
        }

        /// <summary>
        /// MHDR (Header) - Totals 72 bytes
        /// </summary>
        private List<byte> GenerateMOHDChunk()
        {
            List<byte> chunkBytes = new List<byte>();

            // Flags (1 = contains MFBO, 2 = Northrend identifier) - Not used
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            // MCIN Offset
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            // MTEX Offset
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            // MMDX Offset
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
            
            // MMID Offset
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
            
            // MWMO Offset
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
            
            // MWID Offset
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
            
            // MDDF Offset
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
            
            // MODF Offset
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
            
            // MFBO Offset
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
            
            // MH20 Offset
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
            
            // MTXF Offset
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            // Padding to bring up to 64 bytes (not including chunk header)
            chunkBytes.AddRange(new byte[16]);

            return WrapInChunk("MOHD", chunkBytes.ToArray());
        }

        /// <summary>
        /// MCIN (Map Chunk Infos)
        /// </summary>
        private List<byte> GenerateMCINChunk(List<ADTMapChunkInfo> mapChunkInfos)
        {
            List<byte> chunkBytes = new List<byte>();
            foreach (ADTMapChunkInfo mapChunkInfo in mapChunkInfos)
                chunkBytes.AddRange(mapChunkInfo.ToBytes());

            return WrapInChunk("MCIN", chunkBytes.ToArray());
        }

        /// <summary>
        /// MTEX (Textures Chunk)
        /// </summary>
        private List<byte> GenerateMTEXChunk(string dummyTextureFullPath)
        {
            List<byte> chunkBytes = new List<byte>();
            chunkBytes.AddRange(Encoding.ASCII.GetBytes(dummyTextureFullPath));
            return WrapInChunk("MTEX", chunkBytes.ToArray());
        }

        /// <summary>
        /// MWMO (WMO file names)
        /// </summary>
        private List<byte> GenerateMWMOChunk(string wmoFileName)
        {
            List<byte> chunkBytes = new List<byte>();
            chunkBytes.AddRange(Encoding.ASCII.GetBytes(wmoFileName + "\0"));
            return WrapInChunk("MWMO", chunkBytes.ToArray());
        }

        /// <summary>
        /// MODF (WMO placement information)
        /// </summary>
        private List<byte> GenerateMODFChunk(Zone zone)
        {
            List<byte> chunkBytes = new List<byte>();

            // If there's an orientation issue, it could be that this matrix will need to map to world coordinates...
            // ID.  Unsure what this is exactly, so setting to zero for now
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            // Unique ID.  Not sure if used, but see references of it to -1
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToInt32(-1)));

            // Position - Set zero now, and maybe mess with later
            Vector3 positionVector = new Vector3();
            chunkBytes.AddRange(positionVector.ToBytes());

            // Rotation - Set zero now, and maybe mess with later.  Format is ABC not XYZ....
            Vector3 rotation = new Vector3();
            chunkBytes.AddRange(rotation.ToBytes());

            // Bounding Box (Upper Extents then Lower Extents)
            chunkBytes.AddRange(zone.BoundingBox.ToBytesForWDT());

            // Flags - I don't think any are relevant, so zeroing it out (IsDestructible = 1, UsesLOD = 2)
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(0)));

            // DoodadSet - None for now
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(0)));

            // NameSet - Unsure on purpose
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(0)));

            // Unsure / Unused?
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(0)));

            return WrapInChunk("MODF", chunkBytes.ToArray());
        }
    }
}
