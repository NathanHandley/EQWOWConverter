﻿//  Author: Nathan Handley (nathanhandley@protonmail.com)
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
using EQWOWConverter.ObjectModels;

namespace EQWOWConverter.WOWFiles
{
    internal class M2Skin
    {
        private M2SkinHeader Header = new M2SkinHeader();
        private List<byte> SkinBytes = new List<byte>();

        public M2Skin(ObjectModel wowObjectModel)
        {
            // No skins needed if there is no material data
            if (wowObjectModel.ModelMaterials.Count == 0)
            {
                SkinBytes.AddRange(Header.ToBytes());
                return;
            }

            List<byte> nonHeaderBytes = new List<byte>();
            int curOffset = Header.GetSize();

            // Indices
            List<byte> indicesBytes = GenerateIndicesBlock(wowObjectModel);
            Header.Indices.Set(Convert.ToUInt32(curOffset), Convert.ToUInt32(wowObjectModel.ModelVertices.Count));
            curOffset += indicesBytes.Count;
            nonHeaderBytes.AddRange(indicesBytes);
            AddBytesToAlign(ref nonHeaderBytes, ref curOffset, 16);

            // Triangles
            List<byte> triangleBytes = GenerateTrianglesBlock(wowObjectModel);
            Header.TriangleIndices.Set(Convert.ToUInt32(curOffset), Convert.ToUInt32(wowObjectModel.ModelTriangles.Count * 3));
            curOffset += triangleBytes.Count;
            nonHeaderBytes.AddRange(triangleBytes);
            AddBytesToAlign(ref nonHeaderBytes, ref curOffset, 16);

            // Bone Indices
            List<byte> boneIndicesBytes = GenerateBoneIndicesBlock(wowObjectModel);
            Header.BoneIndices.Set(Convert.ToUInt32(curOffset), Convert.ToUInt32(wowObjectModel.ModelVertices.Count));
            curOffset += boneIndicesBytes.Count;
            nonHeaderBytes.AddRange(boneIndicesBytes);
            AddBytesToAlign(ref nonHeaderBytes, ref curOffset, 16);

            // SubMeshes
            List<M2SkinTextureUnit> textureUnits;
            List<byte> subMeshesBytes = GenerateSubMeshesBlockAndTextureUnits(wowObjectModel, out textureUnits);
            Header.SubMeshes.Set(Convert.ToUInt32(curOffset), Convert.ToUInt32(textureUnits.Count));     // 1 sub mesh per texture unit
            curOffset += subMeshesBytes.Count;
            nonHeaderBytes.AddRange(subMeshesBytes);
            AddBytesToAlign(ref nonHeaderBytes, ref curOffset, 16);

            // Texture Units
            List<byte> textureUnitsBytes = GenerateTextureUnitsBlock(textureUnits);
            Header.TextureUnits.Set(Convert.ToUInt32(curOffset), Convert.ToUInt32(textureUnits.Count));
            nonHeaderBytes.AddRange(textureUnitsBytes);
            AddBytesToAlign(ref nonHeaderBytes, ref curOffset, 16);

            // Assemble the byte stream together, header first
            SkinBytes.AddRange(Header.ToBytes());
            SkinBytes.AddRange(nonHeaderBytes);
        }

        /// <summary>
        /// Indices
        /// </summary>
        private List<byte> GenerateIndicesBlock(ObjectModel modelObject)
        {
            List<byte> blockBytes = new List<byte>();
            for (UInt16 i = 0; i < modelObject.ModelVertices.Count; ++i)
                blockBytes.AddRange(BitConverter.GetBytes(i));
            return blockBytes;
        }

        /// <summary>
        /// Triangles
        /// </summary>
        private List<byte> GenerateTrianglesBlock(ObjectModel modelObject)
        {
            List<byte> blockBytes = new List<byte>();
            foreach (TriangleFace triangle in modelObject.ModelTriangles)
                blockBytes.AddRange(triangle.ToBytes());
            return blockBytes;
        }

        /// <summary>
        /// Bone Indices
        /// </summary>
        private List<byte> GenerateBoneIndicesBlock(ObjectModel modelObject)
        {
            List<byte> blockBytes = new List<byte>();
            foreach(ObjectModelVertex modelVertex in modelObject.ModelVertices)
            {
                M2SkinBoneIndices curIndices = new M2SkinBoneIndices(modelVertex.BoneIndicesLookup);
                blockBytes.AddRange(curIndices.ToBytes());
            }
            return blockBytes;
        }

        /// <summary>
        /// SubMeshes
        /// </summary>
        private List<byte> GenerateSubMeshesBlockAndTextureUnits(ObjectModel modelObject, out List<M2SkinTextureUnit> textureUnits)
        {
            List<M2SkinSubMesh> subMeshes = new List<M2SkinSubMesh>();
            textureUnits = new List<M2SkinTextureUnit>();

            // Only build the mesh if this object has rendering enabled
            if (modelObject.Properties.RenderingEnabled == true)
            {
                // Build a list of material-to-vertex for later calculations
                List<int> vertexMaterialIDs = new List<int>();
                for (int vertexIndex = 0; vertexIndex < modelObject.ModelVertices.Count; vertexIndex++)
                    vertexMaterialIDs.Add(-1);
                foreach (TriangleFace modelTriangle in modelObject.ModelTriangles)
                {
                    vertexMaterialIDs[modelTriangle.V1] = modelTriangle.MaterialIndex;
                    vertexMaterialIDs[modelTriangle.V2] = modelTriangle.MaterialIndex;
                    vertexMaterialIDs[modelTriangle.V3] = modelTriangle.MaterialIndex;
                }

                // Each material gets a new sub mesh and texture unit
                // Note: It's expected that triangles and vertices are sorted by texture already
                for (UInt16 materialIter = 0; materialIter < modelObject.ModelMaterials.Count; materialIter++)
                {
                    ObjectModelMaterial curModelMaterial = modelObject.ModelMaterials[materialIter];
                    int curMaterialIndex = Convert.ToInt32(curModelMaterial.Material.Index);

                    // Count number of triangles and find starting offset
                    int startTriangleIndex = -1;
                    int numberOfTrianges = 0;
                    for (int triangleIndex = 0; triangleIndex < modelObject.ModelTriangles.Count; triangleIndex++)
                    {
                        TriangleFace curTriangle = modelObject.ModelTriangles[triangleIndex];
                        if (curTriangle.MaterialIndex == curMaterialIndex)
                        {
                            if (startTriangleIndex == -1)
                                startTriangleIndex = triangleIndex;
                            numberOfTrianges++;
                        }
                    }

                    // If there were no triangles with this material, there there's no reason to make a mesh for it
                    if (startTriangleIndex == -1)
                        continue;

                    // Count the number of vertices and find starting offset, and save them for later
                    int startVertexIndex = -1;
                    int numberOfVertices = 0;
                    List<ObjectModelVertex> subMeshVertices = new List<ObjectModelVertex>();
                    for (int vertexIndex = 0; vertexIndex < vertexMaterialIDs.Count; vertexIndex++)
                    {
                        int vertexMaterialID = vertexMaterialIDs[vertexIndex];
                        if (vertexMaterialID == curMaterialIndex)
                        {
                            if (startVertexIndex == -1)
                                startVertexIndex = vertexIndex;
                            numberOfVertices++;
                            subMeshVertices.Add(modelObject.ModelVertices[vertexIndex]);
                        }
                    }

                    // Count the number of bones and find the starting offset
                    UInt16 startBoneIndex = 0;
                    UInt16 numberOfBones = 0;
                    foreach (var boneLookupByMaterialIndex in modelObject.BoneLookupsByMaterialIndex)
                    {
                        if (boneLookupByMaterialIndex.Key != curMaterialIndex)
                            startBoneIndex += Convert.ToUInt16(boneLookupByMaterialIndex.Value.Count);
                        else
                        {
                            numberOfBones = Convert.ToUInt16(boneLookupByMaterialIndex.Value.Count);
                            break;
                        }
                    }

                    // Some models don't have bones, so control for that
                    if (numberOfBones == 0)
                        numberOfBones = 1;

                    // Build the sub mesh
                    M2SkinSubMesh curSubMesh = new M2SkinSubMesh(Convert.ToUInt16(startVertexIndex), Convert.ToUInt16(numberOfVertices),
                        Convert.ToUInt16(startTriangleIndex * 3), Convert.ToUInt16(numberOfTrianges * 3), numberOfBones, startBoneIndex);
                    curSubMesh.CalculatePositionAndBoundingData(subMeshVertices);
                    subMeshes.Add(curSubMesh);

                    // Create a texture unit
                    UInt16 transparencyLookupIndex = modelObject.ModelTextureTransparencyLookups[materialIter];
                    M2SkinTextureUnit curTextureUnit = new M2SkinTextureUnit(materialIter, materialIter, materialIter, transparencyLookupIndex,
                        curModelMaterial.ColorIndex);
                    textureUnits.Add(curTextureUnit);
                }
            }            

            List<byte> blockBytes = new List<byte>();
            foreach (M2SkinSubMesh subMesh in subMeshes)
                blockBytes.AddRange(subMesh.ToBytes());
            return blockBytes;
        }

        /// <summary>
        /// Texture Units
        /// </summary>
        private List<byte> GenerateTextureUnitsBlock(List<M2SkinTextureUnit> textureUnits)
        {
            List<byte> blockBytes = new List<byte>();
            foreach(var textureUnit in textureUnits)
                blockBytes.AddRange(textureUnit.ToBytes());
            return blockBytes;
        }

        public void WriteToDisk(string fileName, string outputFolderPath)
        {
            // Make the directory
            if (Directory.Exists(outputFolderPath) == false)
                FileTool.CreateBlankDirectory(outputFolderPath, true);

            // Create the skin
            string skinFileName = Path.Combine(outputFolderPath, fileName + "00.skin");
            File.WriteAllBytes(skinFileName, SkinBytes.ToArray());
        }

        private void AddBytesToAlign(ref List<byte> byteBuffer, ref int offset, int byteAlignMultiplier)
        {
            int bytesModified = byteAlignMultiplier - (byteBuffer.Count % byteAlignMultiplier);
            if (bytesModified == byteAlignMultiplier)
            {
                bytesModified = 0;
                return;
            }
            byteBuffer.AddRange(new byte[bytesModified]);
            offset += bytesModified;
        }
    }
}
