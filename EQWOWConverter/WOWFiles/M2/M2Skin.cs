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

        public M2Skin(ModelObject modelObject)
        {
            Name = modelObject.Name;
            SkinBytes.Clear();
            List<byte> nonHeaderBytes = new List<byte>();
            int curOffset = Header.GetSize();

            // Indicies
            List<byte> indiciesBytes = GenerateIndiciesBlock(modelObject.WOWModelObjectData);
            Header.Indicies.Set(Convert.ToUInt32(curOffset), Convert.ToUInt32(modelObject.WOWModelObjectData.ModelVerticies.Count));
            curOffset += indiciesBytes.Count;
            nonHeaderBytes.AddRange(indiciesBytes);

            // Triangles
            List<byte> triangleBytes = GenerateIndiciesBlock(modelObject.WOWModelObjectData);
            Header.TriangleIndicies.Set(Convert.ToUInt32(curOffset), Convert.ToUInt32(modelObject.WOWModelObjectData.ModelTriangles.Count));
            curOffset += triangleBytes.Count;
            nonHeaderBytes.AddRange(triangleBytes);

            // Bone Indicies
            List<byte> boneIndiciesBytes = GenerateBoneIndiciesBlock(modelObject.WOWModelObjectData);
            Header.BoneIndicies.Set(Convert.ToUInt32(curOffset), Convert.ToUInt32(modelObject.WOWModelObjectData.ModelVerticies.Count));
            curOffset += boneIndiciesBytes.Count;
            nonHeaderBytes.AddRange(boneIndiciesBytes);

            // SubMeshes
            List<byte> subMeshesBytes = GenerateSubMeshesBlock(modelObject.WOWModelObjectData);
            Header.SubMeshes.Set(Convert.ToUInt32(curOffset), Convert.ToUInt32(1));     // Set to 1 for now
            curOffset += subMeshesBytes.Count;
            nonHeaderBytes.AddRange(subMeshesBytes);

            // Texture Units
            List<byte> textureUnitsBytes = GenerateTextureUnitsBlock(modelObject.WOWModelObjectData);
            Header.TextureUnits.Set(Convert.ToUInt32(curOffset), Convert.ToUInt32(1));  // Set to 1 for now
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
        private List<byte> GenerateSubMeshesBlock(WOWObjectModelData modelObject)
        {
            // One mesh per material
            M2SkinSubMesh subMesh = new M2SkinSubMesh(modelObject);
            return subMesh.ToBytes();
        }

        /// <summary>
        /// Texture Units
        /// </summary>
        private List<byte> GenerateTextureUnitsBlock(WOWObjectModelData modelObject)
        {
            List<byte> blockBytes = new List<byte>();
            
            return blockBytes;
        }

        public void WriteToDisk(string outputFolderPath)
        {
            // Make the directory
            if (Directory.Exists(outputFolderPath))
                Directory.Delete(outputFolderPath, true);
            FileTool.CreateBlankDirectory(outputFolderPath, true);

            // Create the skin
            string skinFileName = Path.Combine(outputFolderPath, Name + ".skin");
            File.WriteAllBytes(skinFileName, SkinBytes.ToArray());
        }
    }
}
