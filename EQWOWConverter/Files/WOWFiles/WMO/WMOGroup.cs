﻿//  Author: Nathan Handley (nathanhandley@protonmail.com)
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
using EQWOWConverter.Files.WOWFiles;
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

        public WMOGroup(WMORoot wmoRoot, ZoneObjectModel worldObjectModel)
        {
            // MVER (Version) ---------------------------------------------------------------------
            GroupBytes.AddRange(GenerateMVERChunk());

            // MOGP (Container for all other chunks) ----------------------------------------------
            GroupBytes.AddRange(GenerateMOGPChunk(wmoRoot, worldObjectModel));
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
        private List<byte> GenerateMOGPChunk(WMORoot wmoRoot, ZoneObjectModel worldObjectModel)
        {
            List<byte> chunkBytes = new List<byte>();

            // Group name offsets in MOGN
            chunkBytes.AddRange(BitConverter.GetBytes(wmoRoot.GroupNameOffset));
            chunkBytes.AddRange(BitConverter.GetBytes(wmoRoot.GroupNameDescriptiveOffset));

            // Flags
            chunkBytes.AddRange(BitConverter.GetBytes(worldObjectModel.GenerateWMOHeaderFlags()));

            // Bounding box
            chunkBytes.AddRange(worldObjectModel.BoundingBox.ToBytesHighRes());

            // Portal references (zero for now)
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(0))); // First portal index
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(0))); // Number of portals

            // Render batches
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(0))); // transBatchCount ("transition" blend light from ext and int)
            if (worldObjectModel.IsExterior == true)
            {
                chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(0))); // internalBatchCount
                chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(worldObjectModel.RenderBatches.Count()))); // externalBatchCount
            }
            else
            {
                chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(worldObjectModel.RenderBatches.Count()))); // internalBatchCount
                chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(0))); // externalBatchCount
            }
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(0))); // padding/unknown

            // This fog Id list may be wrong, but hoping that 0 works
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0))); // 4 fog IDs that are all zero, I hope...

            // Liquid type (zero causes whole WMO to be underwater, but 15 seems to fix that)
            if (worldObjectModel.LiquidType != ZoneLiquidType.None)
            {
                if (Configuration.CONFIG_EQTOWOW_ZONE_LIQUID_SHOW_TRUE_SURFACE == true)
                {
                    // If set, show the 'actual' water surface
                    switch (worldObjectModel.LiquidType)
                    {
                        case ZoneLiquidType.Water:      chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(13))); break;
                        case ZoneLiquidType.Blood:      chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(13))); break;
                        case ZoneLiquidType.GreenWater: chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(13))); break;
                        case ZoneLiquidType.Magma:      chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(19))); break;
                        case ZoneLiquidType.Slime:      chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(20))); break;
                        default:                    chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(13))); break;
                    }
                }
                else
                    chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(worldObjectModel.LiquidType)));
            }
            else
                chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(15)));

            // WMOGroupID (inside WMOAreaTable)
            chunkBytes.AddRange(BitConverter.GetBytes(worldObjectModel.WMOGroupID));

            // Unknown values.  Hopefully not needed
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            // ------------------------------------------------------------------------------------
            // SUB CHUNKS
            // ------------------------------------------------------------------------------------
            // MOPY (Material info for triangles) -------------------------------------------------
            chunkBytes.AddRange(GenerateMOPYChunk(worldObjectModel));

            // MOVI (MapObject Vertex Indices) ---------------------------------------------------
            chunkBytes.AddRange(GenerateMOVIChunk(worldObjectModel));

            // MOVT (Vertices) -------------------------------------------------------------------
            chunkBytes.AddRange(GenerateMOVTChunk(worldObjectModel));

            // MONR (Normals) ---------------------------------------------------------------------
            chunkBytes.AddRange(GenerateMONRChunk(worldObjectModel));

            // MOTV (Texture Coordinates) ---------------------------------------------------------
            chunkBytes.AddRange(GenerateMOTVChunk(worldObjectModel));

            // MOBA (Render Batches) --------------------------------------------------------------
            chunkBytes.AddRange(GenerateMOBAChunk(worldObjectModel));

            // MOLR (Light References) ------------------------------------------------------------
            if (worldObjectModel.LightInstanceIDs.Count > 0)
                chunkBytes.AddRange(GenerateMOLRChunk(worldObjectModel));

            // MODR (Doodad References) -----------------------------------------------------------
            if (worldObjectModel.DoodadInstances.Count > 0)
            chunkBytes.AddRange(GenerateMODRChunk(worldObjectModel));

            // MOBN (Nodes of the BSP tree) -------------------------------------------------------
            chunkBytes.AddRange(GenerateMOBNChunk(worldObjectModel));

            // MOBR (Face / Triangle Incidies) ----------------------------------------------------
            chunkBytes.AddRange(GenerateMOBRChunk(worldObjectModel));

            // MOCV (Vertex Colors) ---------------------------------------------------------------
            if (worldObjectModel.WMOType == ZoneObjectModelType.Rendered && worldObjectModel.MeshData.VertexColors.Count > 0)
                chunkBytes.AddRange(GenerateMOCVChunk(worldObjectModel));

            // MLIQ (Liquid/Water details) --------------------------------------------------------
            // If it's a liquid volume, not having a MLIQ causes the whole area to be liquid
            if (worldObjectModel.IsCompletelyInLiquid == false)
                chunkBytes.AddRange(GenerateMLIQChunk(worldObjectModel));

            // Note: There can be two MOTV and MOCV blocks depending on flags.  May need to factor for that

            return WrapInChunk("MOGP", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOPY (Material info for triangles)
        /// </summary>
        private List<byte> GenerateMOPYChunk(ZoneObjectModel worldObjectModel)
        {
            List<byte> chunkBytes = new List<byte>();

            // One for each triangle
            foreach (TriangleFace polyIndexTriangle in worldObjectModel.MeshData.TriangleFaces)
            {
                WMOPolyMaterialFlags flags = 0;
                chunkBytes.Add(Convert.ToByte(flags));

                // Set 0xFF for non-renderable materials
                if ((worldObjectModel.WMOType == ZoneObjectModelType.CollisionSimple || worldObjectModel.WMOType == ZoneObjectModelType.CollisionWithAudio) 
                    || worldObjectModel.Materials[polyIndexTriangle.MaterialIndex].IsRenderable() == false)
                    chunkBytes.Add(Convert.ToByte(0xFF));
                else
                    chunkBytes.Add(Convert.ToByte(polyIndexTriangle.MaterialIndex));
            }

            return WrapInChunk("MOPY", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOVI (MapObject Vertex Indices)
        /// </summary>
        private List<byte> GenerateMOVIChunk(ZoneObjectModel worldObjectModel)
        {
            List<byte> chunkBytes = new List<byte>();

            foreach(TriangleFace polyIndex in worldObjectModel.MeshData.TriangleFaces)
                chunkBytes.AddRange(polyIndex.ToBytes());

            return WrapInChunk("MOVI", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOVT (Vertices)
        /// </summary>
        private List<byte> GenerateMOVTChunk(ZoneObjectModel worldObjectModel)
        {
            List<byte> chunkBytes = new List<byte>();

            foreach (Vector3 vertex in worldObjectModel.MeshData.Vertices)
                chunkBytes.AddRange(vertex.ToBytes());

            return WrapInChunk("MOVT", chunkBytes.ToArray());
        }

        /// <summary>
        /// MONR (Normals)
        /// </summary>
        private List<byte> GenerateMONRChunk(ZoneObjectModel worldObjectModel)
        {
            List<byte> chunkBytes = new List<byte>();

            foreach (Vector3 normal in worldObjectModel.MeshData.Normals)
              chunkBytes.AddRange(normal.ToBytes());

            return WrapInChunk("MONR", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOTV (Texture Coordinates)
        /// </summary>
        private List<byte> GenerateMOTVChunk(ZoneObjectModel worldObjectModel)
        {
            List<byte> chunkBytes = new List<byte>();

            foreach (TextureCoordinates textureCoords in worldObjectModel.MeshData.TextureCoordinates)
                chunkBytes.AddRange(textureCoords.ToBytes());

            return WrapInChunk("MOTV", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOBA (Render Batches)
        /// </summary>
        private List<byte> GenerateMOBAChunk(ZoneObjectModel worldObjectModel)
        {
            List<byte> chunkBytes = new List<byte>();
            foreach (ZoneRenderBatch renderBatch in worldObjectModel.RenderBatches)
            {
                // Bounding Box
                chunkBytes.AddRange(renderBatch.BoundingBox.ToBytesLowRes());

                // Poly Start Index
                chunkBytes.AddRange(BitConverter.GetBytes(renderBatch.FirstTriangleFaceIndex));

                // Number of poly indexes
                chunkBytes.AddRange(BitConverter.GetBytes(renderBatch.NumOfFaceIndices));

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
        private List<byte> GenerateMOLRChunk(ZoneObjectModel worldObjectModel)
        {
            List<byte> chunkBytes = new List<byte>();
            foreach (UInt16 lightInstanceID in worldObjectModel.LightInstanceIDs)
                chunkBytes.AddRange(BitConverter.GetBytes(lightInstanceID));
            return WrapInChunk("MOLR", chunkBytes.ToArray());
        }

        /// <summary>
        /// MODR (Doodad References)
        /// Optional.  If has Doodads
        /// </summary>
        private List<byte> GenerateMODRChunk(ZoneObjectModel worldObjectModel)
        {
            List<byte> chunkBytes = new List<byte>();
            foreach (var doodadInstanceReference in worldObjectModel.DoodadInstances)
                chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(doodadInstanceReference.Key)));
            return WrapInChunk("MODR", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOBN (Nodes of the BSP tree, collision)
        /// Optional.  If HasBSPTree flag.
        /// </summary>
        private List<byte> GenerateMOBNChunk(ZoneObjectModel worldObjectModel)
        {
            List<byte> chunkBytes = new List<byte>();

            foreach (BSPNode node in worldObjectModel.BSPTree.Nodes)
                chunkBytes.AddRange(node.ToBytes());

            return WrapInChunk("MOBN", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOBR (Face / Triangle Incidies)
        /// Optional.  If HasBSPTree flag.
        /// </summary>
        private List<byte> GenerateMOBRChunk(ZoneObjectModel worldObjectModel)
        {
            List<byte> chunkBytes = new List<byte>();

            foreach(UInt32 faceIndex in worldObjectModel.BSPTree.FaceTriangleIndices)
                chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(faceIndex)));

            return WrapInChunk("MOBR", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOCV (Vertex Colors)
        /// Optional.  If HasVertexColor Flag
        /// </summary>
        private List<byte> GenerateMOCVChunk(ZoneObjectModel worldObjectModel)
        {
            List<byte> chunkBytes = new List<byte>();
            foreach (ColorRGBA vertexColor in worldObjectModel.MeshData.VertexColors)
                chunkBytes.AddRange(vertexColor.ToBytesBGRA());
            return WrapInChunk("MOCV", chunkBytes.ToArray());
        }

        /// <summary>
        /// MLIQ (Liquid/Water details)
        /// Optional.  If HasWater flag
        /// </summary>
        private List<byte> GenerateMLIQChunk(ZoneObjectModel worldObjectModel)
        {
            List<byte> chunkBytes = new List<byte>();

            // Build the liquid object head
            WMOLiquid liquid = new WMOLiquid();
            liquid.MaterialID = Convert.ToUInt16(worldObjectModel.LiquidMaterial.Index);
            if (worldObjectModel.LiquidType != ZoneLiquidType.Magma && worldObjectModel.LiquidType != ZoneLiquidType.Slime)
                liquid.TileFlags.Add(WMOLiquidFlags.IsFishable);

            // Different logic based on type
            switch (worldObjectModel.WMOType)
            {
                case ZoneObjectModelType.LiquidPlane:
                    {
                        ZoneLiquidPlane liquidPlane = worldObjectModel.LiquidPlane;

                        // The corner position of a liquid is the SE corner due to it being in world coordinate space, not model space
                        liquid.CornerPosition = new Vector3();
                        liquid.CornerPosition.X = liquidPlane.SECornerXY.X;
                        liquid.CornerPosition.Y = liquidPlane.SECornerXY.Y;
                        liquid.CornerPosition.Z = 0f;

                        // Calculate tiles
                        float xDistance = worldObjectModel.BoundingBox.GetXDistance();
                        float yDistance = worldObjectModel.BoundingBox.GetYDistance();
                        liquid.XTileCount = Convert.ToInt32(Math.Round(xDistance / 4.1666625f, MidpointRounding.AwayFromZero)) + 1;
                        liquid.YTileCount = Convert.ToInt32(Math.Round(yDistance / 4.1666625f, MidpointRounding.AwayFromZero)) + 1;
                        liquid.XVertexCount = liquid.XTileCount + 1;
                        liquid.YVertexCount = liquid.YTileCount + 1;

                        // Build the tile data.  Z-Axis aligned can build it quicker
                        if (liquidPlane.SlantType == ZoneLiquidSlantType.None)
                        {
                            float zHeight = liquidPlane.HighZ;
                            for (int y = 0; y < liquid.YVertexCount; y++)
                            {
                                for (int x = 0; x < liquid.XVertexCount; x++)
                                { 
                                    switch (worldObjectModel.LiquidType)
                                    {
                                        case ZoneLiquidType.Water:
                                        case ZoneLiquidType.Blood:
                                        case ZoneLiquidType.GreenWater:
                                        case ZoneLiquidType.Slime:
                                            {
                                                liquid.WaterVerts.Add(new WMOWaterVert(0, 0, 0, 0, zHeight));
                                            } break;
                                        case ZoneLiquidType.Magma:
                                            {
                                                liquid.MagmaVerts.Add(new WMOMagmaVert(0, 0, zHeight));
                                            } break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            float xSlope = 0f;
                            float ySlope = 0f;
                            float zDrop = liquidPlane.HighZ - liquidPlane.LowZ;
                            float seZHeight = liquidPlane.HighZ;
                            switch (liquidPlane.SlantType) // Walk 'up' from south to north
                            {
                                case ZoneLiquidSlantType.None: break; // Intentionally blank
                                case ZoneLiquidSlantType.NorthHighSouthLow:
                                    {
                                        xSlope = -(zDrop / xDistance);
                                        seZHeight = liquidPlane.HighZ;
                                    } break;
                                case ZoneLiquidSlantType.WestHighEastLow:
                                    {
                                        ySlope = -(zDrop / yDistance);
                                        seZHeight = liquidPlane.HighZ;
                                    } break;
                                case ZoneLiquidSlantType.EastHighWestLow:
                                    {
                                        ySlope = zDrop / yDistance;
                                        seZHeight = liquidPlane.LowZ;
                                    } break;
                                case ZoneLiquidSlantType.SouthHighNorthLow:
                                    {
                                        xSlope = zDrop / xDistance;
                                        seZHeight = liquidPlane.LowZ;
                                    } break;
                                default:
                                    {
                                        Logger.WriteError("Unhandled LiquidPlane SlantType of '" + liquidPlane.SlantType +"'.  Plane will be flat.");
                                    } break;
                            }

                            for (int y = 0; y < liquid.YVertexCount; y++)
                            {
                                for (int x = 0; x < liquid.XVertexCount; x++)
                                {
                                    float curZHeight = (x * 4.1666625f * xSlope) + (y * 4.1666625f * ySlope) + seZHeight;
                                    switch (worldObjectModel.LiquidType)
                                    {
                                        case ZoneLiquidType.Water:
                                        case ZoneLiquidType.Blood:
                                        case ZoneLiquidType.GreenWater:
                                        case ZoneLiquidType.Slime:
                                            {
                                                liquid.WaterVerts.Add(new WMOWaterVert(0, 0, 0, 0, curZHeight));
                                            } break;
                                        case ZoneLiquidType.Magma:
                                            {
                                                liquid.MagmaVerts.Add(new WMOMagmaVert(0, 0, curZHeight));
                                            } break;
                                    }
                                }
                            }
                        }
                    } break;
                case ZoneObjectModelType.LiquidVolume:
                    {
                        ZoneLiquidPlane liquidPlane = worldObjectModel.LiquidPlane;

                        // The corner position of a liquid is the SE corner due to it being in world coordinate space, not model space
                        liquid.CornerPosition = new Vector3();
                        liquid.CornerPosition.X = liquidPlane.SECornerXY.X;
                        liquid.CornerPosition.Y = liquidPlane.SECornerXY.Y;
                        liquid.CornerPosition.Z = 0f;

                        // Calculate tiles
                        float xDistance = worldObjectModel.BoundingBox.GetXDistance();
                        float yDistance = worldObjectModel.BoundingBox.GetYDistance();
                        liquid.XTileCount = Convert.ToInt32(Math.Round(xDistance / 4.1666625f, MidpointRounding.AwayFromZero)) + 1;
                        liquid.YTileCount = Convert.ToInt32(Math.Round(yDistance / 4.1666625f, MidpointRounding.AwayFromZero)) + 1;
                        liquid.XVertexCount = liquid.XTileCount + 1;
                        liquid.YVertexCount = liquid.YTileCount + 1;

                        // Build the tile data.  Z-Axis aligned can build it quicker
                        if (liquidPlane.SlantType == ZoneLiquidSlantType.None)
                        {
                            float zHeight = liquidPlane.HighZ + 1000f;
                            for (int y = 0; y < liquid.YVertexCount; y++)
                            {
                                for (int x = 0; x < liquid.XVertexCount; x++)
                                {
                                    switch (worldObjectModel.LiquidType)
                                    {
                                        case ZoneLiquidType.Water:
                                        case ZoneLiquidType.Blood:
                                        case ZoneLiquidType.GreenWater:
                                        case ZoneLiquidType.Slime:
                                            {
                                                liquid.WaterVerts.Add(new WMOWaterVert(0, 0, 0, 0, zHeight));
                                            }
                                            break;
                                        case ZoneLiquidType.Magma:
                                            {
                                                liquid.MagmaVerts.Add(new WMOMagmaVert(0, 0, zHeight));
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                    } break;
                default:
                    {
                        // Create just a single water block and put in the middle of nowhere
                        liquid.CornerPosition = new Vector3();
                        liquid.CornerPosition.X = 0f;
                        liquid.CornerPosition.Y = 0f;
                        liquid.CornerPosition.Z = 0f;
                        liquid.XTileCount = 1;
                        liquid.YTileCount = 1;
                        liquid.XVertexCount = 2;
                        liquid.YVertexCount = 2;
                        liquid.WaterVerts.Add(new WMOWaterVert(0, 0, 0, 0, -1000f));
                        liquid.WaterVerts.Add(new WMOWaterVert(0, 0, 0, 0, -1000f));
                        liquid.WaterVerts.Add(new WMOWaterVert(0, 0, 0, 0, -1000f));
                        liquid.WaterVerts.Add(new WMOWaterVert(0, 0, 0, 0, -1000f));
                    } break;
            }
            chunkBytes.AddRange(liquid.ToBytes());
            return WrapInChunk("MLIQ", chunkBytes.ToArray());
        }
    }
}
