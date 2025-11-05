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
using EQWOWConverter.Zones;
using System.Text;

namespace EQWOWConverter.WOWFiles
{
    internal class ADT : WOWChunkedObject
    {
        private Zone ZoneObject;
        private string WMOFileName;
        private float ZoneBaseHeight;
        private List<byte> DataBytes;

        public ADT(Zone zone, string wmoFileName)
        {
            ZoneObject = zone;
            WMOFileName = wmoFileName;
            ZoneBaseHeight = 0f; // TODO: Map this to something, maybe from ZoneProperties

            DataBytes = GenerateDataBytes();
        }

        private List<byte> GenerateDataBytes()
        {
            List<byte> discardByteArray = new List<byte>();

            // Generate version chunk (MVER)
            List<byte> versionChunkBytes = GenerateMVERChunk();

            // Generate texture chunk (MTEX)
            // Using a placeholder texture since the map won't be rendered
            List<byte> textureChunkBytes = GenerateMTEXChunk("Tileset\\BurningStepps\\BurningSteppsRock02.blp\0");

            // Generate the M2 model list chunk (MMDX)
            // No objects, so making it a blank chunk
            List<byte> m2ModelChunkBytes = WrapInChunk("MMDX", discardByteArray.ToArray());

            // Generate offsets for M2 model list (MMID)
            // No objects, so making it a blank chunk
            List<byte> m2ModelOffsetChunkBytes = WrapInChunk("MMID", discardByteArray.ToArray());

            // Generate WMO file names (MWMO)
            List<byte> wmoNameChunkBytes = GenerateMWMOChunk(WMOFileName);

            // Generate offsets for WMO File names (WMID)
            List<byte> wmoNameOffsetBytes = GenerateMWIDChunk();

            // Generate model placement information (MDDF)
            List<byte> modelPlacementBytes = GenerateMDDFChunk();

            // Generate WMO placement information (MODF)
            List<byte> wmoPlacementInformationBytes = GenerateMODFChunk(ZoneObject);

            // Calculate the offsets of all of the chunks which are relative to the data block of the header (MHDR)
            int headerRelativeOffsetCursor = 64; // Header (HMD) data size minus the magic/size subheader
            int offsetMCIN = headerRelativeOffsetCursor;
            headerRelativeOffsetCursor += 4104; // Map Chunk Infos (MCIN) - 16bytes x 256 + 8 bytes (header)
            int offsetMTEX = headerRelativeOffsetCursor;
            headerRelativeOffsetCursor += textureChunkBytes.Count;
            int offsetMMDX = headerRelativeOffsetCursor;
            headerRelativeOffsetCursor += m2ModelChunkBytes.Count;
            int offsetMMID = headerRelativeOffsetCursor;
            headerRelativeOffsetCursor += m2ModelOffsetChunkBytes.Count;
            int offsetMWMO = headerRelativeOffsetCursor;
            headerRelativeOffsetCursor += wmoNameChunkBytes.Count;
            int offsetMWID = headerRelativeOffsetCursor;
            headerRelativeOffsetCursor += wmoNameOffsetBytes.Count;
            int offsetMDDF = headerRelativeOffsetCursor;
            headerRelativeOffsetCursor += modelPlacementBytes.Count;
            int offsetMODF = headerRelativeOffsetCursor;

            // Generate the header (MHDR)
            List<byte> headerBytes = GenerateMHDRChunk(offsetMCIN, offsetMTEX, offsetMMDX, offsetMMID, offsetMWMO, offsetMWID, offsetMDDF, offsetMODF);

            // Generate map chunks (MCNKs) and their info objects (MCIN)
            //List<ADTMapChunk> mapChunks = new List<ADTMapChunk>();
            List<ADTMapChunkInfo> mapChunkInfos = new List<ADTMapChunkInfo>();
            List<byte> mapChunkBytes = new List<byte>();
            int curMCNKOffset = headerRelativeOffsetCursor + headerBytes.Count + versionChunkBytes.Count + 8;
            for (UInt16 y = 0; y < 16; y++)
                for (UInt16 x = 0; x < 16; x++)
                {
                    // Make the chunk
                    ADTMapChunk curChunk = new ADTMapChunk(x, y, ZoneBaseHeight, ZoneObject.DefaultArea.DBCAreaTableID);
                    List<byte> curChunkBytes = curChunk.GetDataBytes();
                    mapChunkBytes.AddRange(curChunkBytes);

                    // Make the info and advance the cursor
                    ADTMapChunkInfo curChunkInfo = new ADTMapChunkInfo(curMCNKOffset, curChunkBytes.Count);
                    mapChunkInfos.Add(curChunkInfo);
                    curMCNKOffset += curChunkBytes.Count;
                }

            // Turn the chunk infos into a data block
            List<byte> mapChunkInfoBytes = GenerateMCINChunk(mapChunkInfos);

            // Combine all the data up into the final data block
            List<Byte> dataBytes = new List<Byte>();
            dataBytes.AddRange(versionChunkBytes);              // MVER
            dataBytes.AddRange(headerBytes);                    // MHDR
            dataBytes.AddRange(mapChunkInfoBytes);              // MCIN
            dataBytes.AddRange(textureChunkBytes);              // MTEX
            dataBytes.AddRange(m2ModelChunkBytes);              // MMDX
            dataBytes.AddRange(m2ModelOffsetChunkBytes);        // MMID
            dataBytes.AddRange(wmoNameChunkBytes);              // MWMO
            dataBytes.AddRange(wmoNameOffsetBytes);             // MWID
            dataBytes.AddRange(modelPlacementBytes);            // MDDF
            dataBytes.AddRange(wmoPlacementInformationBytes);   // MODF
            dataBytes.AddRange(mapChunkBytes);                  // MCNKs
            return dataBytes;
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
        private List<byte> GenerateMHDRChunk(int offsetMCIN, int offsetMTEX, int offsetMMDX, int offsetMMID, int offsetMWMO, 
            int offsetMWID, int offsetMDDF, int offsetMODF)
        {
            List<byte> chunkBytes = new List<byte>();

            // Flags (1 = contains MFBO, 2 = Northrend identifier) - Not used
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            // MCIN Offset
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(offsetMCIN)));

            // MTEX Offset
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(offsetMTEX)));

            // MMDX Offset
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(offsetMMDX)));
            
            // MMID Offset
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(offsetMMID)));
            
            // MWMO Offset
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(offsetMWMO)));
            
            // MWID Offset
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(offsetMWID)));
            
            // MDDF Offset
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(offsetMDDF)));
            
            // MODF Offset
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(offsetMODF)));
            
            // MFBO Offset (none)
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
            
            // MH20 Offset (none)
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
            
            // MTXF Offset (none)
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            // Padding to bring up to 64 bytes (not including chunk header)
            chunkBytes.AddRange(new byte[16]);

            return WrapInChunk("MHDR", chunkBytes.ToArray());
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
        /// MWID (WMO file name offsets)
        /// </summary>
        private List<byte> GenerateMWIDChunk()
        {
            // Offsets are relative to the start of the MWMO, so "0" works
            List<byte> chunkBytes = new List<byte>();
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
            return WrapInChunk("MWID", chunkBytes.ToArray());
        }

        /// <summary>
        /// MDDF (Model placement information)
        /// </summary>
        private List<byte> GenerateMDDFChunk()
        {
            // No models, so no data
            List<byte> chunkBytes = new List<byte>();
            return WrapInChunk("MDDF", chunkBytes.ToArray());
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

            // Unique ID.
            chunkBytes.AddRange(BitConverter.GetBytes((zone.MODFIdentifier)));

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

        public void WriteToDisk(string baseFolderPath, int tileX, int tileY)
        {
            string folderToWrite = Path.Combine(baseFolderPath, "World", "Maps", "EQ_" + ZoneObject.ShortName);
            FileTool.CreateBlankDirectory(folderToWrite, true);
            string fullFilePath = Path.Combine(folderToWrite, string.Concat("EQ_", ZoneObject.ShortName, "_", tileX, "_", tileY, ".adt"));
            File.WriteAllBytes(fullFilePath, DataBytes.ToArray());
        }
    }
}
