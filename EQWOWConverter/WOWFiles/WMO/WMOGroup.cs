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
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal class WMOGroup : WOWChunkedObject
    {
        public List<byte> GroupBytes = new List<byte>();

        public WMOGroup(WMORoot wmoRoot, WorldModelObject worldModelObject)
        {
            // MVER (Version) ---------------------------------------------------------------------
            GroupBytes.AddRange(GenerateMVERChunk());

            // MOGP (Container for all other chunks) ----------------------------------------------
            GroupBytes.AddRange(GenerateMOGPChunk(wmoRoot, worldModelObject));
        }

        /// <summary>
        /// MVER (Version)
        /// </summary>
        private List<byte> GenerateMVERChunk()
        {
            UInt32 version = 17;
            return WrapInChunk("MVER", BitConverter.GetBytes(version));
        }

        /// <summary>
        /// MOGP (Main container for all other chunks)
        /// </summary>
        private List<byte> GenerateMOGPChunk(WMORoot wmoRoot, WorldModelObject worldModelObject)
        {
            List<byte> chunkBytes = new List<byte>();

            // Group name offsets in MOGN
            chunkBytes.AddRange(BitConverter.GetBytes(wmoRoot.GroupNameOffset));
            chunkBytes.AddRange(BitConverter.GetBytes(wmoRoot.GroupNameDescriptiveOffset));

            // Flags
            UInt32 groupHeaderFlags = Convert.ToUInt32(WMOGroupFlags.IsOutdoors);
            groupHeaderFlags |= Convert.ToUInt32(WMOGroupFlags.HasBSPTree);
            if (worldModelObject.DoodadInstances.Count > 0)
                groupHeaderFlags |= Convert.ToUInt32(WMOGroupFlags.HasDoodads);
            chunkBytes.AddRange(BitConverter.GetBytes(groupHeaderFlags));

            // Bounding box
            chunkBytes.AddRange(worldModelObject.BoundingBox.ToBytesHighRes());

            // Portal references (zero for now)
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(0))); // First portal index
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(0))); // Number of portals

            // NOTE: Temp code in place. Making everything a single render batch for testing.
            // What exactly is "transbatchcount"?
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(0))); // transBatchCount
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(0))); // internalBatchCount
            // TODO: This count should split above/below based on internal and external batch details
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(worldModelObject.RenderBatches.Count()))); // externalBatchCount
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(0))); // padding/unknown

            // This fog Id list may be wrong, but hoping that 0 works
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0))); // 4 fog IDs that are all zero, I hope...

            // Liquid type (zero causes whole WMO to be underwater, but 15 seems to fix that)
            if (worldModelObject.WMOType == WorldModelObjectType.LiquidVolume)
                chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
            else
                chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(15)));

            // WMOGroupID (inside WMOAreaTable)
            chunkBytes.AddRange(BitConverter.GetBytes(worldModelObject.WMOGroupID));

            // Unknown values.  Hopefully not needed
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            // ------------------------------------------------------------------------------------
            // SUB CHUNKS
            // ------------------------------------------------------------------------------------
            // MOPY (Material info for triangles) -------------------------------------------------
            chunkBytes.AddRange(GenerateMOPYChunk(worldModelObject));

            // MOVI (MapObject Vertex Indicies) ---------------------------------------------------
            chunkBytes.AddRange(GenerateMOVIChunk(worldModelObject));

            // MOVT (Verticies) -------------------------------------------------------------------
            chunkBytes.AddRange(GenerateMOVTChunk(worldModelObject));

            // MONR (Normals) ---------------------------------------------------------------------
            chunkBytes.AddRange(GenerateMONRChunk(worldModelObject));

            // MOTV (Texture Coordinates) ---------------------------------------------------------
            chunkBytes.AddRange(GenerateMOTVChunk(worldModelObject));

            // MOBA (Render Batches) --------------------------------------------------------------
            chunkBytes.AddRange(GenerateMOBAChunk(worldModelObject));

            // MOLR (Light References) ------------------------------------------------------------
            //chunkBytes.AddRange(GenerateMOLRChunk(zone));

            // MODR (Doodad References) -----------------------------------------------------------
            if (worldModelObject.DoodadInstances.Count > 0)
                chunkBytes.AddRange(GenerateMODRChunk(worldModelObject));

            // MOBN (Nodes of the BSP tree) -------------------------------------------------------
            chunkBytes.AddRange(GenerateMOBNChunk(worldModelObject));

            // MOBR (Face / Triangle Incidies) ----------------------------------------------------
            chunkBytes.AddRange(GenerateMOBRChunk(worldModelObject));

            // MOCV (Vertex Colors) ---------------------------------------------------------------
            //chunkBytes.AddRange(GenerateMOCVChunk(zone));

            // MLIQ (Liquid/Water details) --------------------------------------------------------
            // - If HasWater flag

            // Note: There can be two MOTV and MOCV blocks depending on flags.  May need to factor for that

            return WrapInChunk("MOGP", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOPY (Material info for triangles)
        /// </summary>
        private List<byte> GenerateMOPYChunk(WorldModelObject worldModelObject)
        {
            List<byte> chunkBytes = new List<byte>();

            // One for each triangle
            foreach (TriangleFace polyIndexTriangle in worldModelObject.TriangleFaces)
            {
                WMOPolyMaterialFlags flags = 0;
                chunkBytes.Add(Convert.ToByte(flags));
                chunkBytes.Add(Convert.ToByte(polyIndexTriangle.MaterialIndex));
            }

            return WrapInChunk("MOPY", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOVI (MapObject Vertex Indicies)
        /// </summary>
        private List<byte> GenerateMOVIChunk(WorldModelObject worldModelObject)
        {
            List<byte> chunkBytes = new List<byte>();

            foreach(TriangleFace polyIndex in worldModelObject.TriangleFaces)
                chunkBytes.AddRange(polyIndex.ToBytes());

            return WrapInChunk("MOVI", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOVT (Verticies)
        /// </summary>
        private List<byte> GenerateMOVTChunk(WorldModelObject worldModelObject)
        {
            List<byte> chunkBytes = new List<byte>();

            foreach (Vector3 vertex in worldModelObject.Verticies)
                chunkBytes.AddRange(vertex.ToBytes());

            return WrapInChunk("MOVT", chunkBytes.ToArray());
        }

        /// <summary>
        /// MONR (Normals)
        /// </summary>
        private List<byte> GenerateMONRChunk(WorldModelObject worldModelObject)
        {
            List<byte> chunkBytes = new List<byte>();

            foreach (Vector3 normal in worldModelObject.Normals)
              chunkBytes.AddRange(normal.ToBytes());

            return WrapInChunk("MONR", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOTV (Texture Coordinates)
        /// </summary>
        private List<byte> GenerateMOTVChunk(WorldModelObject worldModelObject)
        {
            List<byte> chunkBytes = new List<byte>();

            foreach (TextureCoordinates textureCoords in worldModelObject.TextureCoords)
                chunkBytes.AddRange(textureCoords.ToBytes());

            return WrapInChunk("MOTV", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOBA (Render Batches)
        /// </summary>
        private List<byte> GenerateMOBAChunk(WorldModelObject worldModelObject)
        {
            List<byte> chunkBytes = new List<byte>();
            foreach (WorldModelRenderBatch renderBatch in worldModelObject.RenderBatches)
            {
                // Bounding Box
                chunkBytes.AddRange(renderBatch.BoundingBox.ToBytesLowRes());

                // Poly Start Index
                chunkBytes.AddRange(BitConverter.GetBytes(renderBatch.FirstTriangleFaceIndex));

                // Number of poly indexes
                chunkBytes.AddRange(BitConverter.GetBytes(renderBatch.NumOfFaceIndicies));

                // Vertex Start Index
                chunkBytes.AddRange(BitConverter.GetBytes(renderBatch.FirstVertexIndex));

                // Vertex End Index
                chunkBytes.AddRange(BitConverter.GetBytes(renderBatch.LastVertexIndex));

                // Byte padding (or unknown flag, unsure)
                chunkBytes.Add(0);

                // Index of the material
                chunkBytes.Add(renderBatch.MaterialIndex);
            }

            return WrapInChunk("MOBA", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOLR (Light References)
        /// Optional.  Only if it has lights
        /// </summary>
        private List<byte> GenerateMOLRChunk(WorldModelObject worldModelObject)
        {
            List<byte> chunkBytes = new List<byte>();

            // Intentionally blank for now

            return WrapInChunk("MOLR", chunkBytes.ToArray());
        }

        /// <summary>
        /// MODR (Doodad References)
        /// Optional.  If has Doodads
        /// </summary>
        private List<byte> GenerateMODRChunk(WorldModelObject worldModelObject)
        {
            List<byte> chunkBytes = new List<byte>();
            foreach (var doodadInstanceReference in worldModelObject.DoodadInstances)
                chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(doodadInstanceReference.Key)));
            return WrapInChunk("MODR", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOBN (Nodes of the BSP tree, collision)
        /// Optional.  If HasBSPTree flag.
        /// </summary>
        private List<byte> GenerateMOBNChunk(WorldModelObject worldModelObject)
        {
            List<byte> chunkBytes = new List<byte>();

            foreach (BSPNode node in worldModelObject.BSPTree.Nodes)
                chunkBytes.AddRange(node.ToBytes());

            return WrapInChunk("MOBN", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOBR (Face / Triangle Incidies)
        /// Optional.  If HasBSPTree flag.
        /// </summary>
        private List<byte> GenerateMOBRChunk(WorldModelObject worldModelObject)
        {
            List<byte> chunkBytes = new List<byte>();

            foreach(UInt32 faceIndex in worldModelObject.BSPTree.FaceTriangleIndicies)
                chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(faceIndex)));

            return WrapInChunk("MOBR", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOCV (Vertex Colors)
        /// Optional.  If HasVertexColor Flag
        /// </summary>
        private List<byte> GenerateMOCVChunk(WorldModelObject worldModelObject)
        {
            List<byte> chunkBytes = new List<byte>();

            // Intentionally blank for now

            return WrapInChunk("MOCV", chunkBytes.ToArray());
        }

        /// <summary>
        /// MLIQ (Liquid/Water details)
        /// Optional.  If HasWater flag
        /// </summary>
        private List<byte> GenerateMLIQChunk(WorldModelObject worldModelObject)
        {
            List<byte> chunkBytes = new List<byte>();

            // Intentionally blank for now

            return WrapInChunk("MLIQ", chunkBytes.ToArray());
        }
    }
}
