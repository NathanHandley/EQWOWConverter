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
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Common
{
    internal class EQMapData
    {
        public List<Vector3> Verticies = new List<Vector3>();
        public List<TextureUv> TextureCoords = new List<TextureUv>();
        public List<Vector3> Normals = new List<Vector3>();
        public List<ColorRGBA> VertexColors = new List<ColorRGBA>();
        public List<TriangleFace> TriangleFaces = new List<TriangleFace>();
        public List<Material> Materials = new List<Material>();
        public string MaterialListName = string.Empty;

        public AxisAlignedBox BoundingBox = new AxisAlignedBox();
        public AxisAlignedBoxLR BoundingBoxLowRes = new AxisAlignedBoxLR();

        public List<EQMapData> TextureAlignedSubMeshes = new List<EQMapData>();

        public EQMapData()
        {

        }
        public EQMapData(string parentName, string inputData)
        {
            GenerateCompleteZoneMesh(parentName, inputData);
        }

        public EQMapData(string parentName, string inputData, List<Material> materials)
        {
            GenerateCompleteZoneMesh(parentName, inputData);
            Materials = materials;
            GenerateTextureAlignedSubMeshes(parentName);
        }

        private void GenerateCompleteZoneMesh(string parentName, string inputData)
        {
            string[] inputRows = inputData.Split(Environment.NewLine);
            foreach (string inputRow in inputRows)
            {
                // Nothing for blank lines
                if (inputRow.Length == 0)
                    continue;

                // # = comment
                else if (inputRow.StartsWith("#"))
                    continue;

                // ml = Material List
                else if (inputRow.StartsWith("ml"))
                {
                    string[] blocks = inputRow.Split(",");
                    if (blocks.Length != 2)
                    {
                        Logger.WriteLine("- [" + parentName + "]: Error, material list name needs to be 2 components");
                        continue;
                    }
                    if (MaterialListName != string.Empty)
                    {
                        Logger.WriteLine("- [" + parentName + "]: Error, a second material list was found");
                        continue;
                    }
                    MaterialListName = blocks[1];
                }

                // v = Verticies
                else if (inputRow.StartsWith("v"))
                {
                    string[] blocks = inputRow.Split(",");
                    if (blocks.Length != 4)
                    {
                        Logger.WriteLine("- [" + parentName + "]: Error, vertex block was not 4 components");
                        continue;
                    }
                    Vector3 vertex = new Vector3();
                    vertex.X = float.Parse(blocks[1]);
                    vertex.Z = float.Parse(blocks[2]);
                    vertex.Y = float.Parse(blocks[3]);
                    Verticies.Add(vertex);
                }

                // uv = Texture Coordinates
                else if (inputRow.StartsWith("uv"))
                {
                    string[] blocks = inputRow.Split(",");
                    if (blocks.Length != 3)
                    {
                        Logger.WriteLine("- [" + parentName + "]: Error, texture coordinate block was not 3 components");
                        continue;
                    }
                    TextureUv textureUv = new TextureUv();
                    textureUv.X = float.Parse(blocks[1]);
                    textureUv.Y = float.Parse(blocks[2]);
                    TextureCoords.Add(textureUv);
                }

                // n = Normal
                else if (inputRow.StartsWith("n"))
                {
                    string[] blocks = inputRow.Split(",");
                    if (blocks.Length != 4)
                    {
                        Logger.WriteLine("- [" + parentName + "]: Error, normals block was not 4 components");
                        continue;
                    }
                    Vector3 normal = new Vector3();
                    normal.X = float.Parse(blocks[1]);
                    normal.Y = float.Parse(blocks[2]);
                    normal.Z = float.Parse(blocks[3]);
                    Normals.Add(normal);
                }

                // c = Vertex Color
                else if (inputRow.StartsWith("c"))
                {
                    string[] blocks = inputRow.Split(",");
                    if (blocks.Length != 5)
                    {
                        Logger.WriteLine("- [" + parentName + "]: Error, vertex color block was not 5 components");
                        continue;
                    }
                    ColorRGBA color = new ColorRGBA();
                    color.B = byte.Parse(blocks[1]);
                    color.G = byte.Parse(blocks[2]);
                    color.R = byte.Parse(blocks[3]);
                    color.A = byte.Parse(blocks[4]);
                    VertexColors.Add(color);
                }

                // i = Indicies
                else if (inputRow.StartsWith("i"))
                {
                    string[] blocks = inputRow.Split(",");
                    if (blocks.Length != 5)
                    {
                        Logger.WriteLine("- [" + parentName + "]: Error,indicies block was not 5 components");
                        continue;
                    }
                    TriangleFace index = new TriangleFace();
                    index.MaterialIndex = int.Parse(blocks[1]);
                    // Reverse the culling rotation
                    index.V3 = int.Parse(blocks[2]);
                    index.V2 = int.Parse(blocks[3]);
                    index.V1 = int.Parse(blocks[4]);
                    TriangleFaces.Add(index);
                }

                else
                {
                    Logger.WriteLine("- [" + parentName + "]: Error, unknown line '" + inputRow + "'");
                }
            }
        }

        private void GenerateTextureAlignedSubMeshes(string parentName)
        {
            if (Materials.Count == 0)
            {
                Logger.WriteLine("- [" + parentName + "]: Could not generate sub meshes since there are no materials");
                return;
            }

            // Perform unique copy of faces into sub objects
            List<TriangleFace> facesToConsume = new List<TriangleFace>(TriangleFaces);
            while (facesToConsume.Count > 0)
            {
                List<int> facesToDelete = new List<int>();
                List<TriangleFace> newMeshTriangles = new List<TriangleFace>();

                // Iterate through and collect like faces
                int curMaterialIndex = -2;
                for (int i = 0; i < facesToConsume.Count; i++)
                {
                    // If there is no assigned working material index, grab it
                    if (curMaterialIndex == -2)
                    {
                        // If it's an invalid index, just delete it
                        if (facesToConsume[i].MaterialIndex < 0)
                        {
                            facesToDelete.Add(i);
                            break;
                        }
                        curMaterialIndex = facesToConsume[i].MaterialIndex;
                    }

                    // Capture like faces
                    if (facesToConsume[i].MaterialIndex == curMaterialIndex)
                    {
                        // TODO: Add data to mesh
                        newMeshTriangles.Add(facesToConsume[i]);
                        curMaterialIndex = facesToConsume[i].MaterialIndex;
                        facesToDelete.Add(i);
                    }
                }

                // Remove the faces
                for (int j = facesToDelete.Count - 1; j >= 0; j--)
                    facesToConsume.RemoveAt(j);

                // Save the mesh
                if (newMeshTriangles.Count > 0)
                {
                    // When creating the new mesh, the indicies of the faces to include need to be remapped because
                    // the related arrays will be subsets
                    EQMapData newZoneMesh = new EQMapData();
                    newZoneMesh.Materials = new List<Material>(Materials);
                    Dictionary<int, int> copiedIndiciesAndNewValues = new Dictionary<int, int>();
                    foreach(TriangleFace face in newMeshTriangles)
                    {
                        TriangleFace realignedFace = new TriangleFace();
                        if (copiedIndiciesAndNewValues.ContainsKey(face.V1) == true)
                        {
                            realignedFace.V1 = copiedIndiciesAndNewValues[face.V1];
                        }
                        else
                        {
                            realignedFace.V1 = newZoneMesh.Verticies.Count;
                            copiedIndiciesAndNewValues.Add(face.V1, realignedFace.V1);
                            newZoneMesh.Verticies.Add(Verticies[face.V1]);
                            newZoneMesh.Normals.Add(Normals[face.V1]);
                            newZoneMesh.VertexColors.Add(VertexColors[face.V1]);
                            newZoneMesh.TextureCoords.Add(TextureCoords[face.V1]);
                        }

                        if (copiedIndiciesAndNewValues.ContainsKey(face.V2) == true)
                        {
                            realignedFace.V2 = copiedIndiciesAndNewValues[face.V2];
                        }
                        else
                        {
                            realignedFace.V2 = newZoneMesh.Verticies.Count;
                            copiedIndiciesAndNewValues.Add(face.V2, realignedFace.V2);
                            newZoneMesh.Verticies.Add(Verticies[face.V2]);
                            newZoneMesh.Normals.Add(Normals[face.V2]);
                            newZoneMesh.VertexColors.Add(VertexColors[face.V2]);
                            newZoneMesh.TextureCoords.Add(TextureCoords[face.V2]);
                        }

                        if (copiedIndiciesAndNewValues.ContainsKey(face.V3) == true)
                        {
                            realignedFace.V3 = copiedIndiciesAndNewValues[face.V3];
                        }
                        else
                        {
                            realignedFace.V3 = newZoneMesh.Verticies.Count;
                            copiedIndiciesAndNewValues.Add(face.V3, realignedFace.V3);
                            newZoneMesh.Verticies.Add(Verticies[face.V3]);
                            newZoneMesh.Normals.Add(Normals[face.V3]);
                            newZoneMesh.VertexColors.Add(VertexColors[face.V3]);
                            newZoneMesh.TextureCoords.Add(TextureCoords[face.V3]);
                        }

                        realignedFace.MaterialIndex = face.MaterialIndex;
                        newZoneMesh.TriangleFaces.Add(realignedFace);
                    }
                    newZoneMesh.CalculateBoundingBox();
                    TextureAlignedSubMeshes.Add(newZoneMesh);
                }
                else
                    Logger.WriteLine("-[" + parentName + "]: Error: In the loop to generate TextureAlignedSubMeshes, there were no verticies added but a mesh was added.");
            }

            if (TextureAlignedSubMeshes.Count >= 1000)
            {
                Logger.WriteLine("-[" + parentName + "]: Error: More than 1000 sub meshes was generated, so WMO generation will fail...");
            }
        }

        public void CalculateBoundingBox()
        {
            foreach (Vector3 renderVert in Verticies)
            {
                if (renderVert.X < BoundingBox.BottomCorner.X)
                    BoundingBox.BottomCorner.X = renderVert.X;
                if (renderVert.Y < BoundingBox.BottomCorner.Y)
                    BoundingBox.BottomCorner.Y = renderVert.Y;
                if (renderVert.Z < BoundingBox.BottomCorner.Z)
                    BoundingBox.BottomCorner.Z = renderVert.Z;

                if (renderVert.X > BoundingBox.TopCorner.X)
                    BoundingBox.TopCorner.X = renderVert.X;
                if (renderVert.Y > BoundingBox.TopCorner.Y)
                    BoundingBox.TopCorner.Y = renderVert.Y;
                if (renderVert.Z > BoundingBox.TopCorner.Z)
                    BoundingBox.TopCorner.Z = renderVert.Z;
            }
            foreach (Vector3 collisionVert in Verticies)
            {
                if (collisionVert.X < BoundingBox.BottomCorner.X)
                    BoundingBox.BottomCorner.X = collisionVert.X;
                if (collisionVert.Y < BoundingBox.BottomCorner.Y)
                    BoundingBox.BottomCorner.Y = collisionVert.Y;
                if (collisionVert.Z < BoundingBox.BottomCorner.Z)
                    BoundingBox.BottomCorner.Z = collisionVert.Z;

                if (collisionVert.X > BoundingBox.TopCorner.X)
                    BoundingBox.TopCorner.X = collisionVert.X;
                if (collisionVert.Y > BoundingBox.TopCorner.Y)
                    BoundingBox.TopCorner.Y = collisionVert.Y;
                if (collisionVert.Z > BoundingBox.TopCorner.Z)
                    BoundingBox.TopCorner.Z = collisionVert.Z;
            }

            BoundingBoxLowRes = new AxisAlignedBoxLR(BoundingBox);
        }
    }
}
