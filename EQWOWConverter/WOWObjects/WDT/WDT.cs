//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2024 Nathan Handley
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWObjects
{
    internal class WDT : WOWChunkedObject
    {
        public List<byte> ObjectBytes = new List<byte>();
        private string BaseFileName;

        public WDT(Zone zone, string wmoFileName)
        {
            BaseFileName = zone.Name;

            // MVER (Version) ---------------------------------------------------------------------
            ObjectBytes.AddRange(GenerateMVERChunk(zone));

            // MPHD (Header) ----------------------------------------------------------------------
            ObjectBytes.AddRange(GenerateMPHDChunk(zone));

            // MAIN (Map Tile Table) --------------------------------------------------------------
            ObjectBytes.AddRange(GenerateMAINChunk(zone));

            // MWMO (Main WMO lookup) -------------------------------------------------------------
            ObjectBytes.AddRange(GenerateMWMOChunk(zone, wmoFileName));

            // MODF (WMO placement information) ---------------------------------------------------
            ObjectBytes.AddRange(GenerateMODFChunk(zone));
        }

        /// <summary>
        /// MVER (Version)
        /// </summary>
        private List<byte> GenerateMVERChunk(Zone zone)
        {
            UInt32 version = 18;
            return WrapInChunk("MVER", BitConverter.GetBytes(version));
        }

        /// <summary>
        /// MPHD (Header)
        /// </summary>
        private List<byte> GenerateMPHDChunk(Zone zone)
        {
            List<byte> chunkBytes = new List<byte>();

            // Flags
            UInt32 flags = GetPackedFlags(Convert.ToUInt32(WDTHeaderFlags.HasGlobalMapObject));
            chunkBytes.AddRange(BitConverter.GetBytes(flags));

            // Unknown / Padding
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0))); // "Something", hopefully blank is fine
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0))); // Unused 1
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0))); // Unused 2
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0))); // Unused 3
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0))); // Unused 4
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0))); // Unused 5
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0))); // Unused 6

            return WrapInChunk("MPHD", chunkBytes.ToArray());
        }

        /// <summary>
        /// MAIN (Map Tile Table)
        /// </summary>
        private List<byte> GenerateMAINChunk(Zone zone)
        {
            List<byte> chunkBytes = new List<byte>();

            for (int mapX = 0; mapX < 64; ++mapX)
            {
                for (int mapY = 0; mapY < 64; ++mapY)
                {
                    // Since this is a WMO-based map, blank seems okay...
                    chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
                    chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
                }
            }

            return WrapInChunk("MAIN", chunkBytes.ToArray());
        }

        /// <summary>
        /// MWMO (Main WMO lookup)
        /// </summary>
        private List<byte> GenerateMWMOChunk(Zone zone, string wmoFileName)
        {
            List<byte> chunkBytes = new List<byte>();

            // Write out the wmo root file name
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

            // Bounding Box... again?
            chunkBytes.AddRange(zone.BoundingBox.ToBytes());

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

        public void WriteToDisk(string baseFolderPath)
        {
            string folderToWrite = Path.Combine(baseFolderPath, "World", "Everquest", BaseFileName);
            FileTool.CreateBlankDirectory(folderToWrite);
            string fullFilePath = Path.Combine(folderToWrite, BaseFileName + "wdt");
            File.WriteAllBytes(fullFilePath, ObjectBytes.ToArray());
        }
    }
}
