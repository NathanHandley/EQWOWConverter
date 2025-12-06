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
using EQWOWConverter.ObjectModels;

namespace EQWOWConverter.WOWFiles
{
    internal class M2Skin
    {
        private M2SkinHeader Header = new M2SkinHeader();
        private List<byte> SkinBytes = new List<byte>();
        private int MaxBones = 0;

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

            // Set the bone counts
            if (MaxBones > 64)
                Header.BoneCountMax = 256;
            else if (MaxBones > 53)
                Header.BoneCountMax = 64;
            else if (MaxBones > 21)
                Header.BoneCountMax = 53;
            else
                Header.BoneCountMax = 21;

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
            foreach (ObjectModelVertex modelVertex in modelObject.ModelVertices)
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
                // One texture unit and sub mesh per render group, unless the related material is animated
                UInt16 curBoneLookupIndex = 0;
                for (int i = 0; i < modelObject.ModelRenderGroups.Count; i++)
                {
                    ObjectModelRenderGroup renderGroup = modelObject.ModelRenderGroups[i];
                    if (renderGroup.Vertices.Count == 0)
                        continue;
                    if (renderGroup.MaterialIndex >= 10000)
                        continue;
                    MaxBones = Math.Max(renderGroup.BoneLookupIndices.Count, MaxBones);


                    // Build the sub mesh
                    M2SkinSubMesh curSubMesh = new M2SkinSubMesh(renderGroup, curBoneLookupIndex);
                    curBoneLookupIndex += Convert.ToUInt16(renderGroup.BoneLookupIndices.Count);
                    subMeshes.Add(curSubMesh);

                    // Create a texture unit
                    UInt16 materialID = renderGroup.MaterialIndex;
                    UInt16 transparencyLookupIndex = modelObject.ModelTextureTransparencyLookups[Convert.ToInt32(materialID)];
                    M2SkinTextureUnit curTextureUnit = new M2SkinTextureUnit(Convert.ToUInt16(subMeshes.Count() - 1), materialID,
                        materialID, transparencyLookupIndex, -1);
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
            foreach (var textureUnit in textureUnits)
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
