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

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using EQWOWConverter.Common;
using EQWOWConverter.ObjectModels;

namespace EQWOWConverter.WOWFiles
{
    internal class M2Skin
    {
        string Name = string.Empty;
        private M2SkinHeader Header = new M2SkinHeader();
        private List<byte> SkinBytes = new List<byte>();

        public M2Skin(ObjectModel wowObjectModel)
        {
            Name = wowObjectModel.Name;
            SkinBytes.Clear();
            List<byte> nonHeaderBytes = new List<byte>();
            int curOffset = Header.GetSize();

            // Indices
            List<byte> indicesBytes = GenerateIndicesBlock(wowObjectModel);
            Header.Indices.Set(Convert.ToUInt32(curOffset), Convert.ToUInt32(wowObjectModel.ModelVertices.Count));
            curOffset += indicesBytes.Count;
            nonHeaderBytes.AddRange(indicesBytes);

            // Triangles
            List<byte> triangleBytes = GenerateTrianglesBlock(wowObjectModel);
            Header.TriangleIndices.Set(Convert.ToUInt32(curOffset), Convert.ToUInt32(wowObjectModel.ModelTriangles.Count * 3));
            curOffset += triangleBytes.Count;
            nonHeaderBytes.AddRange(triangleBytes);

            // Bone Indices
            List<byte> boneIndicesBytes = GenerateBoneIndicesBlock(wowObjectModel);
            Header.BoneIndices.Set(Convert.ToUInt32(curOffset), Convert.ToUInt32(wowObjectModel.ModelVertices.Count));
            curOffset += boneIndicesBytes.Count;
            nonHeaderBytes.AddRange(boneIndicesBytes);

            // SubMeshes
            List<M2SkinTextureUnit> textureUnits;
            List<byte> subMeshesBytes = GenerateSubMeshesBlock(wowObjectModel, out textureUnits);
            Header.SubMeshes.Set(Convert.ToUInt32(curOffset), Convert.ToUInt32(textureUnits.Count));     // 1 sub mesh per texture unit
            curOffset += subMeshesBytes.Count;
            nonHeaderBytes.AddRange(subMeshesBytes);

            // Texture Units
            List<byte> textureUnitsBytes = GenerateTextureUnitsBlock(textureUnits);
            Header.TextureUnits.Set(Convert.ToUInt32(curOffset), Convert.ToUInt32(textureUnits.Count));
            nonHeaderBytes.AddRange(textureUnitsBytes);

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
                M2SkinBoneIndices curIndices = new M2SkinBoneIndices(modelVertex.BoneIndices);
                blockBytes.AddRange(curIndices.ToBytes());
            }
            return blockBytes;
        }

        /// <summary>
        /// SubMeshes
        /// </summary>
        private List<byte> GenerateSubMeshesBlock(ObjectModel modelObject, out List<M2SkinTextureUnit> textureUnits)
        {
            textureUnits = new List<M2SkinTextureUnit>();

            // Each material gets a new sub mesh and texture unit
            // Note: It's expected that triangles and vertices are sorted by texture already
            List<M2SkinSubMesh> subMeshes = new List<M2SkinSubMesh>();
            UInt16 materialIter = 0;
            foreach (ObjectModelMaterial material in modelObject.ModelMaterials)
            {
                // Find the geometry offsets related to this material
                int startTriangleIndex = -1;
                int numberOfTrianges = 0;
                int startVertexIndex = -1;
                HashSet<int> countedVertexIndices = new HashSet<int>();
                for (int i = 0; i < modelObject.ModelTriangles.Count; i++)
                {
                    if (modelObject.ModelTriangles[i].MaterialIndex == material.Material.Index)
                    {
                        if (startTriangleIndex == -1)
                            startTriangleIndex = i;
                        numberOfTrianges++;
                        if (startVertexIndex == -1)
                            startVertexIndex = modelObject.ModelTriangles[i].GetSmallestIndex();
                        if (countedVertexIndices.Contains(modelObject.ModelTriangles[i].V1) == false)
                            countedVertexIndices.Add(modelObject.ModelTriangles[i].V1);
                        if (countedVertexIndices.Contains(modelObject.ModelTriangles[i].V2) == false)
                            countedVertexIndices.Add(modelObject.ModelTriangles[i].V2);
                        if (countedVertexIndices.Contains(modelObject.ModelTriangles[i].V3) == false)
                            countedVertexIndices.Add(modelObject.ModelTriangles[i].V3);
                    }
                }
                if (startTriangleIndex == -1)
                    continue;
                int numberOfVertices = countedVertexIndices.Count;

                // Build the sub mesh
                M2SkinSubMesh curSubMesh = new M2SkinSubMesh(Convert.ToUInt16(startVertexIndex), Convert.ToUInt16(numberOfVertices), 
                    Convert.ToUInt16(startTriangleIndex * 3), Convert.ToUInt16(numberOfTrianges * 3), 
                    Convert.ToUInt16(modelObject.ModelBones.Count));
                List<ObjectModelVertex> subMeshVertices = new List<ObjectModelVertex>();
                foreach (int vertexIndex in countedVertexIndices)
                    subMeshVertices.Add(modelObject.ModelVertices[vertexIndex]);
                curSubMesh.CalculatePositionAndBoundingData(subMeshVertices);
                subMeshes.Add(curSubMesh);

                // Create a texture unit
                UInt16 transparencyLookupIndex =modelObject.ModelTextureTransparencyLookups[materialIter];
                M2SkinTextureUnit curTextureUnit = new M2SkinTextureUnit(materialIter, materialIter, materialIter, transparencyLookupIndex);
                textureUnits.Add(curTextureUnit);
                materialIter++;
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

        public void WriteToDisk(string outputFolderPath)
        {
            // Make the directory
            if (Directory.Exists(outputFolderPath) == false)
                FileTool.CreateBlankDirectory(outputFolderPath, true);

            // Create the skin
            string skinFileName = Path.Combine(outputFolderPath, Name + "00.skin");
            File.WriteAllBytes(skinFileName, SkinBytes.ToArray());
        }
    }
}
