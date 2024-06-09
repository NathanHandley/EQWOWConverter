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
            textureUnits = new List<M2SkinTextureUnit>();

            if (modelObject.Name == "ZO_freportw_t25_m0004")
            {
                int x = 5;
                int y = 5;
            }

            // Each material gets a new sub mesh and texture unit
            // Note: It's expected that triangles and verticies are sorted by texture already
            List<M2SkinSubMesh> subMeshes = new List<M2SkinSubMesh>();
            UInt16 materialIter = 0;
            foreach (ModelMaterial material in modelObject.ModelMaterials)
            {
                // Find the geometry offsets related to this material
                int startTriangleIndex = -1;
                int numberOfTrianges = 0;
                int startVertexIndex = -1;
                HashSet<int> countedVertexIndicies = new HashSet<int>();
                for (int i = 0; i < modelObject.ModelTriangles.Count; i++)
                {
                    if (modelObject.ModelTriangles[i].MaterialIndex == material.Material.Index)
                    {
                        if (startTriangleIndex == -1)
                            startTriangleIndex = i;
                        numberOfTrianges++;
                        if (startVertexIndex == -1)
                            startVertexIndex = modelObject.ModelTriangles[i].GetSmallestIndex();
                        if (countedVertexIndicies.Contains(modelObject.ModelTriangles[i].V1) == false)
                            countedVertexIndicies.Add(modelObject.ModelTriangles[i].V1);
                        if (countedVertexIndicies.Contains(modelObject.ModelTriangles[i].V2) == false)
                            countedVertexIndicies.Add(modelObject.ModelTriangles[i].V2);
                        if (countedVertexIndicies.Contains(modelObject.ModelTriangles[i].V3) == false)
                            countedVertexIndicies.Add(modelObject.ModelTriangles[i].V3);
                    }
                }
                if (startTriangleIndex == -1)
                    continue;
                int numberOfVerticies = countedVertexIndicies.Count;

                // Build the sub mesh
                M2SkinSubMesh curSubMesh = new M2SkinSubMesh(Convert.ToUInt16(startVertexIndex), Convert.ToUInt16(numberOfVerticies), 
                    Convert.ToUInt16(startTriangleIndex * 3), Convert.ToUInt16(numberOfTrianges * 3));
                List<ModelVertex> subMeshVerticies = new List<ModelVertex>();
                foreach (int vertexIndex in countedVertexIndicies)
                    subMeshVerticies.Add(modelObject.ModelVerticies[vertexIndex]);
                curSubMesh.CalculatePositionAndBoundingData(subMeshVerticies);
                subMeshes.Add(curSubMesh);

                // Create a texture unit
                M2SkinTextureUnit curTextureUnit = new M2SkinTextureUnit(materialIter, materialIter, materialIter, materialIter);
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
