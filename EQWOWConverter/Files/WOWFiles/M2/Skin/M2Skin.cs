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
using EQWOWConverter.ModelObjects;
using EQWOWConverter.Objects;

namespace EQWOWConverter.WOWFiles
{
    internal class M2Skin
    {
        string Name = string.Empty;
        private M2SkinHeader Header = new M2SkinHeader();
        private List<byte> SkinBytes = new List<byte>();

        public M2Skin(WOWObjectModelData wowModelObject)
        {
            Name = wowModelObject.Name;
            SkinBytes.Clear();
            List<byte> nonHeaderBytes = new List<byte>();
            int curOffset = Header.GetSize();

            // Indicies
            List<byte> indiciesBytes = GenerateIndiciesBlock(wowModelObject);
            Header.Indicies.Set(Convert.ToUInt32(curOffset), Convert.ToUInt32(wowModelObject.ModelVerticies.Count));
            curOffset += indiciesBytes.Count;
            nonHeaderBytes.AddRange(indiciesBytes);

            // Triangles
            List<byte> triangleBytes = GenerateTrianglesBlock(wowModelObject);
            Header.TriangleIndicies.Set(Convert.ToUInt32(curOffset), Convert.ToUInt32(wowModelObject.ModelTriangles.Count * 3));
            curOffset += triangleBytes.Count;
            nonHeaderBytes.AddRange(triangleBytes);

            // Bone Indicies
            List<byte> boneIndiciesBytes = GenerateBoneIndiciesBlock(wowModelObject);
            Header.BoneIndicies.Set(Convert.ToUInt32(curOffset), Convert.ToUInt32(wowModelObject.ModelVerticies.Count));
            curOffset += boneIndiciesBytes.Count;
            nonHeaderBytes.AddRange(boneIndiciesBytes);

            // SubMeshes
            List<M2SkinTextureUnit> textureUnits;
            List<byte> subMeshesBytes = GenerateSubMeshesBlock(wowModelObject, out textureUnits);
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
        /// Indicies
        /// </summary>
        private List<byte> GenerateIndiciesBlock(WOWObjectModelData modelObject)
        {
            List<byte> blockBytes = new List<byte>();
            for (UInt16 i = 0; i < modelObject.ModelVerticies.Count; ++i)
                blockBytes.AddRange(BitConverter.GetBytes(i));
            return blockBytes;
        }

        /// <summary>
        /// Triangles
        /// </summary>
        private List<byte> GenerateTrianglesBlock(WOWObjectModelData modelObject)
        {
            List<byte> blockBytes = new List<byte>();
            foreach (TriangleFace triangle in modelObject.ModelTriangles)
                blockBytes.AddRange(triangle.ToBytes());
            return blockBytes;
        }

        /// <summary>
        /// Bone Indicies
        /// </summary>
        private List<byte> GenerateBoneIndiciesBlock(WOWObjectModelData modelObject)
        {
            List<byte> blockBytes = new List<byte>();
            foreach(ModelVertex modelVertex in modelObject.ModelVerticies)
            {
                M2SkinBoneIndicies curIndicies = new M2SkinBoneIndicies(modelVertex.BoneIndicies);
                blockBytes.AddRange(curIndicies.ToBytes());
            }
            return blockBytes;
        }

        /// <summary>
        /// SubMeshes
        /// </summary>
        private List<byte> GenerateSubMeshesBlock(WOWObjectModelData modelObject, out List<M2SkinTextureUnit> textureUnits)
        {
            // Isolate triangles per material.  They should be presorted by material ID
            SortedDictionary<int, List<TriangleFace>> trianglesPerMaterial = new SortedDictionary<int, List<TriangleFace>>();
            foreach(TriangleFace triangle in modelObject.ModelTriangles)
            {
                if (trianglesPerMaterial.ContainsKey(triangle.MaterialIndex) == false)
                    trianglesPerMaterial.Add(triangle.MaterialIndex, new List<TriangleFace>());
                trianglesPerMaterial[triangle.MaterialIndex].Add(triangle);
            }

            // Each material gets a new sub mesh and texture unit
            textureUnits = new List<M2SkinTextureUnit>();
            Int16 curIndex = 0;
            List<M2SkinSubMesh> subMeshes = new List<M2SkinSubMesh>();
            UInt16 curStartVertex = 0;
            UInt16 curStartTriangleIndex = 0;
            foreach (var trianglesForMaterial in trianglesPerMaterial)
            {
                // Sub Mesh
                UInt16 vertexStart = curStartVertex;
                UInt16 startTriangleIndex = curStartTriangleIndex;
                UInt16 triangleIndexCount = Convert.ToUInt16(trianglesForMaterial.Value.Count * 3);
                SortedDictionary<int, ModelVertex> verticiesByIndex = new SortedDictionary<int, ModelVertex>();
                foreach(TriangleFace triangleFace in trianglesForMaterial.Value)
                {
                    if (verticiesByIndex.ContainsKey(triangleFace.V1) == false)
                        verticiesByIndex.Add(triangleFace.V1, modelObject.ModelVerticies[triangleFace.V1]);
                    if (verticiesByIndex.ContainsKey(triangleFace.V2) == false)
                        verticiesByIndex.Add(triangleFace.V2, modelObject.ModelVerticies[triangleFace.V2]);
                    if (verticiesByIndex.ContainsKey(triangleFace.V3) == false)
                        verticiesByIndex.Add(triangleFace.V3, modelObject.ModelVerticies[triangleFace.V3]);
                }
                UInt16 vertexCount = Convert.ToUInt16(verticiesByIndex.Count);
                M2SkinSubMesh curSubMesh = new M2SkinSubMesh(vertexStart, vertexCount, startTriangleIndex, triangleIndexCount);
                List<ModelVertex> subMeshVerticies = new List<ModelVertex>();
                foreach (var modelVertexByIndex in verticiesByIndex)
                    subMeshVerticies.Add(modelVertexByIndex.Value);
                curSubMesh.CalculatePositionAndBoundingData(subMeshVerticies);
                subMeshes.Add(curSubMesh);

                // Increase start values for future sub meshes
                curStartTriangleIndex += triangleIndexCount;
                curStartVertex += vertexCount;

                // Texture Unit
                UInt16 textureLookupID = modelObject.GetTextureLookupIndexForMaterial(curIndex);
                M2SkinTextureUnit curTextureUnit = new M2SkinTextureUnit(Convert.ToUInt16(curIndex), Convert.ToUInt16(trianglesForMaterial.Key), textureLookupID);
                textureUnits.Add(curTextureUnit);

                curIndex++;
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
