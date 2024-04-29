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
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWObjects
{
    internal class WDL : WOWChunkedObject
    {
        public List<byte> ObjectBytes = new List<byte>();
        private string BaseFileName;

        // TODO: This is where the heightmap will go for rendering 'in the distance'
        public WDL(Zone zone)
        {
            BaseFileName = zone.Name;

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

            // Blank padding for now
            chunkBytes.AddRange(Encoding.ASCII.GetBytes("\0\0\0\0"));

            return WrapInChunk("MWMO", chunkBytes.ToArray());
        }

        /// <summary>
        /// MWID (List of indexes into the MWMO chunk)
        /// </summary>
        private List<byte> GenerateMWIDChunk(Zone zone)
        {
            List<byte> chunkBytes = new List<byte>();

            // Blank padding for now
            chunkBytes.AddRange(Encoding.ASCII.GetBytes("\0\0\0\0"));

            return WrapInChunk("MWID", chunkBytes.ToArray());
        }

        /// <summary>
        /// MODF (Placement Information)
        /// </summary>
        private List<byte> GenerateMODFChunk(Zone zone)
        {
            List<byte> chunkBytes = new List<byte>();

            // Blank padding for now
            chunkBytes.AddRange(Encoding.ASCII.GetBytes("\0\0\0\0"));

            return WrapInChunk("MODF", chunkBytes.ToArray());
        }

        /// <summary>
        /// MAOF (Map Area Offset)
        /// </summary>
        private List<byte> GenerateMAOFChunk(Zone zone)
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

            return WrapInChunk("MAOF", chunkBytes.ToArray());
        }

        public void WriteToDisk(string baseFolderPath)
        {
            string folderToWrite = Path.Combine(baseFolderPath, "World", "EQ_" + BaseFileName);
            FileTool.CreateBlankDirectory(folderToWrite, true);
            string fullFilePath = Path.Combine(folderToWrite, BaseFileName + ".wdl");
            File.WriteAllBytes(fullFilePath, ObjectBytes.ToArray());
        }
    }
}
