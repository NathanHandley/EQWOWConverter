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
        private float ZoneFloorHeight;
        private List<byte> DataBytes;
        private UInt32 TileXIndex = 0;
        private UInt32 TileYIndex = 0;
        public List<string> DoodadPathStrings = new List<string>();
        public Dictionary<string, int> DoodadPathStringOffsets = new Dictionary<string, int>();
        private List<ZoneDoodadInstance> TileDoodadInstances = new List<ZoneDoodadInstance>();
        private static readonly object UNIQUE_MODEL_ID_LOCK = new object();
        private static readonly UInt32 UNIQUE_MODEL_ID_START = 1000000;
        private static UInt32 CUR_UNIQUE_MODEL_ID = UNIQUE_MODEL_ID_START;

        public ADT(Zone zone, string wmoFileName, UInt32 tileXIndex, UInt32 tileYIndex, float zoneFloorHeight,
            string relativeStaticDoodadsFolder, string relativeZoneObjectsFolder, UInt32 uniqueWMOID)
        {
            ZoneObject = zone;
            WMOFileName = wmoFileName;
            ZoneFloorHeight = zoneFloorHeight;
            TileXIndex = tileXIndex;
            TileYIndex = tileYIndex;

            // Add doodads only if trying to generate minimaps
            if (Configuration.ZONE_MINIMAP_GENERATION_MODE_ENABLED == true)
            {
                // Filter doodads that belong in this tile, factoring for the change in coordinate systems
                float tileLength = 1600f / 3f; // Comes out to 533.333 repeat, doing the math here to make it be as exact as possible
                float tileMinX = tileXIndex * tileLength - 32 * tileLength;
                float tileMinY = tileYIndex * tileLength - 32 * tileLength;
                float tileMaxX = tileMinX + tileLength;
                float tileMaxY = tileMinY + tileLength;
                Dictionary<string, int> doodadPathStringIndicesByPathString = new Dictionary<string, int>();
                foreach (var doodadInstance in zone.DoodadInstances)
                {
                    //float doodadXInWorldSpace = doodadInstance.Position.Y * -1;
                    //float doodadYInWorldSpace = doodadInstance.Position.X * -1;

                    // Math wasn't working out, so enabling for all
                    //if (doodadXInWorldSpace >= tileMinX && doodadXInWorldSpace < tileMaxX &&
                    //    doodadYInWorldSpace >= tileMinY && doodadYInWorldSpace < tileMaxY)
                    //{
                        TileDoodadInstances.Add(doodadInstance);
                        string doodadPath = GenerateAndGetPathForDoodad(doodadInstance, relativeStaticDoodadsFolder, relativeZoneObjectsFolder);
                        if (doodadPathStringIndicesByPathString.ContainsKey(doodadPath) == false)
                        {
                            doodadPathStringIndicesByPathString.Add(doodadPath, DoodadPathStrings.Count);
                            DoodadPathStrings.Add(doodadPath);
                        }
                        doodadInstance.ADTObjectNameIndex = Convert.ToUInt32(doodadPathStringIndicesByPathString[doodadPath]);
                    //}
                }
            }
            DataBytes = GenerateDataBytes(tileXIndex, tileYIndex, uniqueWMOID);
        }

        public static UInt32 GenerateUniqueModelID()
        {
            lock (UNIQUE_MODEL_ID_LOCK)
            {
                UInt32 generatedID = CUR_UNIQUE_MODEL_ID;
                CUR_UNIQUE_MODEL_ID++;
                return generatedID;
            }
        }

        private List<byte> GenerateDataBytes(UInt32 tileXIndex, UInt32 tileYIndex, UInt32 uniqueWMOID)
        {
            List<byte> discardByteArray = new List<byte>();

            // Generate version chunk (MVER)
            List<byte> versionChunkBytes = GenerateMVERChunk();

            // Generate texture chunk (MTEX)
            // Using a placeholder texture since the map won't be rendered
            List<byte> textureChunkBytes = GenerateMTEXChunk("Tileset\\BurningStepps\\BurningSteppsRock02.blp\0");

            // Generate the M2 model list chunk (MMDX)
            List<byte> m2ModelChunkBytes = GenerateMMDXChunk();

            // Generate offsets for M2 model list (MMID)
            List<byte> m2ModelOffsetChunkBytes = GenerateMMIDChunk();

            // Generate WMO file names (MWMO)
            List<byte> wmoNameChunkBytes = GenerateMWMOChunk(WMOFileName);

            // Generate offsets for WMO File names (WMID)
            List<byte> wmoNameOffsetBytes = GenerateMWIDChunk();

            // Generate model placement information (MDDF)
            List<byte> modelPlacementBytes = GenerateMDDFChunk();

            // Generate WMO placement information (MODF)
            List<byte> wmoPlacementInformationBytes = GenerateMODFChunk(ZoneObject, uniqueWMOID);

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
            List<ADTMapChunkInfo> mapChunkInfos = new List<ADTMapChunkInfo>();
            List<byte> mapChunkBytes = new List<byte>();
            int curMCNKOffset = headerRelativeOffsetCursor + wmoPlacementInformationBytes.Count + versionChunkBytes.Count + 8;
            for (UInt16 y = 0; y < 16; y++)
                for (UInt16 x = 0; x < 16; x++)
                {
                    // Make the chunk
                    ADTMapChunk curChunk = new ADTMapChunk(tileXIndex, tileYIndex, x, y, ZoneFloorHeight, ZoneObject.DefaultArea.DBCAreaTableID,
                        TileDoodadInstances);
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
        /// MMDX (M2 model list chunk)
        /// </summary>
        private List<byte> GenerateMMDXChunk()
        {
            List<byte> chunkBytes = new List<byte>();
            foreach (string modelPathString in DoodadPathStrings)
            {
                DoodadPathStringOffsets.Add(modelPathString, chunkBytes.Count);
                chunkBytes.AddRange(Encoding.ASCII.GetBytes(modelPathString));
            }
            return WrapInChunk("MMDX", chunkBytes.ToArray());
        }

        /// <summary>
        /// MMID (M2 model list name offsets chunk)
        /// </summary>
        private List<byte> GenerateMMIDChunk()
        {
            List<byte> chunkBytes = new List<byte>();
            foreach (string modelPathString in DoodadPathStrings)
                chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(DoodadPathStringOffsets[modelPathString])));
            return WrapInChunk("MMID", chunkBytes.ToArray());
        }

        /// <summary>
        /// MWMO (WMO file names)
        /// </summary>
        private List<byte> GenerateMWMOChunk(string wmoFileName)
        {
            List<byte> chunkBytes = new List<byte>();
            chunkBytes.AddRange(Encoding.ASCII.GetBytes(wmoFileName.ToUpper() + "\0"));
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
            List<byte> chunkBytes = new List<byte>();
            foreach (ZoneDoodadInstance doodadInstance in TileDoodadInstances)
                chunkBytes.AddRange(doodadInstance.ToBytesForADT(GenerateUniqueModelID()));
            return WrapInChunk("MDDF", chunkBytes.ToArray());
        }

        /// <summary>
        /// MODF (WMO placement information)
        /// </summary>
        private List<byte> GenerateMODFChunk(Zone zone, UInt32 uniqueWMOID)
        {
            List<byte> chunkBytes = new List<byte>();

            // If there's an orientation issue, it could be that this matrix will need to map to world coordinates...
            // ID.  Unsure what this is exactly, so setting to zero for now
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            // Unique ID
            chunkBytes.AddRange(BitConverter.GetBytes(uniqueWMOID));

            // Position
            // Note that WMOs have to be translated to map space, which has a different coordinate system and origin point
            float centerPointValue = 51200f / 3f; // 64 x 533.333/2
            Vector3 positionVector = new Vector3(centerPointValue, 0, centerPointValue);
            chunkBytes.AddRange(positionVector.ToBytes());

            // Rotation - Set zero now, and maybe mess with later.  Format is ABC not XYZ....
            Vector3 rotation = new Vector3();
            chunkBytes.AddRange(rotation.ToBytes());

            // Bounding Box (Upper Extents then Lower Extents)
            // This extreme box ensures that it's always loaded on any sized map
            BoundingBox extremeBoundingBox = new BoundingBox(-1000000f, -1000000f, -1000000f, 1000000f, 1000000f, 1000000f);
            chunkBytes.AddRange(extremeBoundingBox.ToBytesForADT(new Vector3()));
            //chunkBytes.AddRange(zone.BoundingBox.ToBytesForADT(positionVector));

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

        private string GenerateAndGetPathForDoodad(ZoneDoodadInstance doodadInstance, string relativeStaticDoodadsFolder, string relativeZoneObjectsFolder)
        {
            string objectName = doodadInstance.ObjectName;
            string objectFullPath = string.Empty;
            if (doodadInstance.DoodadType == ZoneDoodadInstanceType.StaticObject)
                objectFullPath = Path.Combine(relativeStaticDoodadsFolder, objectName, objectName + ".MDX" + "\0").ToUpper();
            else if (doodadInstance.DoodadType == ZoneDoodadInstanceType.ZoneMaterial || doodadInstance.DoodadType == ZoneDoodadInstanceType.SoundInstance)
                objectFullPath = Path.Combine(relativeZoneObjectsFolder, objectName, objectName + ".MDX" + "\0").ToUpper();
            else
                Logger.WriteError("Unhandled type of doodad instance '" + doodadInstance.DoodadType.ToString() + "' for doodad name '" + doodadInstance.ObjectName + "'");
            return objectFullPath;
        }

        public void WriteToDisk(string baseFolderPath)
        {
            string folderToWrite = Path.Combine(baseFolderPath, "World", "Maps", "EQ_" + ZoneObject.ShortName);
            FileTool.CreateBlankDirectory(folderToWrite, true);
            string fullFilePath = Path.Combine(folderToWrite, string.Concat("EQ_", ZoneObject.ShortName, "_", TileXIndex, "_", TileYIndex, ".adt"));
            File.WriteAllBytes(fullFilePath, DataBytes.ToArray());
        }
    }
}
