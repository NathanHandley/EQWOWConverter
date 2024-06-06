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
using EQWOWConverter.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal class WDL : WOWChunkedObject
    {
        public List<byte> ObjectBytes = new List<byte>();
        private string BaseFileName;

        // TODO: This is where the heightmap will go for rendering 'in the distance'
        public WDL(Zone zone)
        {
            BaseFileName = zone.ShortName;

            // MVER (Version) ---------------------------------------------------------------------
            ObjectBytes.AddRange(GenerateMVERChunk(zone));

            // MWMO (WMO Filenames in the map) ----------------------------------------------------
            ObjectBytes.AddRange(GenerateMWMOChunk(zone));

            // MWID (List of indexes into the MWMO chunk) -----------------------------------------
            ObjectBytes.AddRange(GenerateMWIDChunk(zone));

            // MODF (Placement Information) -------------------------------------------------------
            ObjectBytes.AddRange(GenerateMODFChunk(zone));

            // MAOF (Map Area Offset) -------------------------------------------------------------
            // Watch this, Stockades has a lot of blank data here
            ObjectBytes.AddRange(GenerateMAOFChunk(zone));
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
        /// MWMO (WMO Filenames in the map)
        /// </summary>
        private List<byte> GenerateMWMOChunk(Zone zone)
        {
            List<byte> chunkBytes = new List<byte>();

            // Intentionally blank for now

            return WrapInChunk("MWMO", chunkBytes.ToArray());
        }

        /// <summary>
        /// MWID (List of indexes into the MWMO chunk)
        /// </summary>
        private List<byte> GenerateMWIDChunk(Zone zone)
        {
            List<byte> chunkBytes = new List<byte>();

            // Intentionally blank for now

            return WrapInChunk("MWID", chunkBytes.ToArray());
        }

        /// <summary>
        /// MODF (Placement Information)
        /// </summary>
        private List<byte> GenerateMODFChunk(Zone zone)
        {
            List<byte> chunkBytes = new List<byte>();

            // Intentionally blank for now

            return WrapInChunk("MODF", chunkBytes.ToArray());
        }

        /// <summary>
        /// MAOF (Map Area Offset)
        /// </summary>
        private List<byte> GenerateMAOFChunk(Zone zone)
        {
            List<byte> chunkBytes = new List<byte>();

            // Map Area Offsets, set to zero for all 4096 (64*64)
            for (int mapX = 0; mapX < 64; ++mapX)
            {
                for (int mapY = 0; mapY < 64; ++mapY)
                {
                    // Since this is a WMO-based map, blank seems okay...
                    chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
                }
            }

            return WrapInChunk("MAOF", chunkBytes.ToArray());
        }

        public void WriteToDisk(string baseFolderPath)
        {
            string folderToWrite = Path.Combine(baseFolderPath, "World", "Maps", "EQ_" + BaseFileName);
            FileTool.CreateBlankDirectory(folderToWrite, true);
            string fullFilePath = Path.Combine(folderToWrite, "EQ_" + BaseFileName + ".wdl");
            File.WriteAllBytes(fullFilePath, ObjectBytes.ToArray());
        }
    }
}
