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
    internal class ADT : WOWChunkedObject
    {
        public ADT()
        {
            // Version
            List<byte> versionBytes = GenerateMVERChunk();

            // Map Chunks
            // TODO:



            // MCIN (Map Chunk Bytes)
            List<byte> mapChunkInfoBytes = GenerateMCINChunk();

            // MHDR (Header)
            List<byte> headerBytes = GenerateMOHDChunk();
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
        private List<byte> GenerateMCINChunk()
        {
            List<ADTMapChunkInfo> mapChunkInfos = new List<ADTMapChunkInfo>();

            List<byte> chunkBytes = new List<byte>();
            foreach (ADTMapChunkInfo mapChunkInfo in mapChunkInfos)
                chunkBytes.AddRange(mapChunkInfo.ToBytes());

            return WrapInChunk("MCIN", chunkBytes.ToArray());
        }
    }
}
