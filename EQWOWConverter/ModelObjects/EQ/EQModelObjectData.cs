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

namespace EQWOWConverter.Objects
{
    internal class EQModelObjectData
    {
        public AnimatedVerticies AnimatedVerticies { get; } = new AnimatedVerticies();
        public List<Vector3> Verticies { get; } = new List<Vector3>();
        public List<TextureCoordinates> TextureCoords { get; } = new List<TextureCoordinates>();
        public List<Vector3> Normals { get; } = new List<Vector3>();
        public List<TriangleFace> TriangleFaces { get; } = new List<TriangleFace>();
        public List<Material> Materials { get; } = new List<Material>();
        public List<Vector3> CollisionVerticies { get; } = new List<Vector3>();
        public List<TriangleFace> CollisionTriangleFaces { get; } = new List<TriangleFace>();
        private string MaterialListName = string.Empty;

        public void LoadDataFromDisk(string inputObjectName, string inputObjectFolder)
        {
            if (Directory.Exists(inputObjectFolder) == false)
            {
                Logger.WriteLine("- [" + inputObjectName + "]: ERROR - Could not find path at '" + inputObjectFolder + "'");
                return;
            }

            // Load the various blocks
            LoadRenderMeshData(inputObjectName, inputObjectFolder);
            LoadMaterialDataFromDisk(inputObjectName, inputObjectFolder);
            LoadCollisionMeshData(inputObjectName, inputObjectFolder);
        }

        private void LoadRenderMeshData(string inputObjectName, string inputObjectFolder)
        {
            Logger.WriteLine("- [" + inputObjectName + "]: Reading render mesh data...");
            string renderMeshFileName = Path.Combine(inputObjectFolder, "Meshes", inputObjectName + ".txt");
            if (File.Exists(renderMeshFileName) == false)
            {
                Logger.WriteLine("- [" + inputObjectName + "]: ERROR - Could not find render mesh file that should be at '" + renderMeshFileName + "'");
                return;
            }

            // Load the core data
            string inputData = File.ReadAllText(renderMeshFileName);
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
                        Logger.WriteLine("- [" + inputObjectName + "]: Error, material list name needs to be 2 components");
                        continue;
                    }
                    if (MaterialListName != string.Empty)
                    {
                        Logger.WriteLine("- [" + inputObjectName + "]: Error, a second material list was found");
                        continue;
                    }
                    MaterialListName = blocks[1];
                }

                // ad = Animation head
                else if (inputRow.StartsWith("ad"))
                {
                    string[] blocks = inputRow.Split(",");
                    if (blocks.Length != 2)
                    {
                        Logger.WriteLine("- [" + inputObjectName + "]: Error, animated head block was not 2 components");
                        continue;
                    }
                    if (AnimatedVerticies.FrameDelay != 0)
                    {
                        Logger.WriteLine("- [" + inputObjectName + "]: Error, there was already a read frame delay");
                        continue;
                    }
                    AnimatedVerticies.FrameDelay = int.Parse(blocks[1]);
                }

                // av = Animation verticies
                else if (inputRow.StartsWith("av"))
                {
                    string[] blocks = inputRow.Split(",");
                    if (blocks.Length != 5)
                    {
                        Logger.WriteLine("- [" + inputObjectName + "]: Error, animation verticies block was not 5 components");
                        continue;
                    }

                    int frameIndex = int.Parse(blocks[1]);
                    if (frameIndex >= AnimatedVerticies.GetFrameCount())
                        AnimatedVerticies.Frames.Add(new List<Vector3>());
                    float xPos = float.Parse(blocks[2]);
                    float zPos = float.Parse(blocks[3]);
                    float yPos = float.Parse(blocks[4]);
                    AnimatedVerticies.Frames[frameIndex].Add(new Vector3(xPos, yPos, zPos));
                }

                // v = Verticies
                else if (inputRow.StartsWith("v"))
                {
                    string[] blocks = inputRow.Split(",");
                    if (blocks.Length != 4)
                    {
                        Logger.WriteLine("- [" + inputObjectName + "]: Error, vertex block was not 4 components");
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
                        Logger.WriteLine("- [" + inputObjectName + "]: Error, texture coordinate block was not 3 components");
                        continue;
                    }
                    TextureCoordinates textureUv = new TextureCoordinates();
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
                        Logger.WriteLine("- [" + inputObjectName + "]: Error, normals block was not 4 components");
                        continue;
                    }
                    Vector3 normal = new Vector3();
                    normal.X = float.Parse(blocks[1]);
                    normal.Y = float.Parse(blocks[2]);
                    normal.Z = float.Parse(blocks[3]);
                    Normals.Add(normal);
                }

                // i = Indicies
                else if (inputRow.StartsWith("i"))
                {
                    string[] blocks = inputRow.Split(",");
                    if (blocks.Length != 5)
                    {
                        Logger.WriteLine("- [" + inputObjectName + "]: Error,indicies block was not 5 components");
                        continue;
                    }
                    TriangleFace index = new TriangleFace();
                    index.MaterialIndex = int.Parse(blocks[1]);
                    index.V1 = int.Parse(blocks[2]);
                    index.V2 = int.Parse(blocks[3]);
                    index.V3 = int.Parse(blocks[4]);
                    TriangleFaces.Add(index);
                }

                else
                {
                    Logger.WriteLine("- [" + inputObjectName + "]: Error, unknown line '" + inputRow + "'");
                }
            }
        }
        private void LoadMaterialDataFromDisk(string inputObjectName, string inputObjectFolder)
        {
            Logger.WriteLine("- [" + inputObjectName + "]: Reading materials...");
            string materialListFileName = Path.Combine(inputObjectFolder, "MaterialLists", inputObjectName + ".txt");
            if (File.Exists(materialListFileName) == false)
                Logger.WriteLine("- [" + inputObjectName + "]: No material data found.");
            else
            {
                using (var materialListReader = new StreamReader(materialListFileName))
                {
                    string? curLine;
                    while ((curLine = materialListReader.ReadLine()) != null)
                    {
                        // Nothing for blank lines
                        if (curLine.Length == 0)
                            continue;

                        // # = comment
                        else if (curLine.StartsWith("#"))
                            continue;

                        // 3-blocks is a material instance
                        else
                        {
                            string[] blocks = curLine.Split(",");
                            if (blocks.Length != 3)
                            {
                                Logger.WriteLine("- [" + inputObjectName + "]: Error, material data is 3 components");
                                continue;
                            }
                            Material newMaterial = new Material(blocks[1]);
                            newMaterial.Index = uint.Parse(blocks[0]);
                            newMaterial.AnimationDelayMs = uint.Parse(blocks[2]);

                            // Texture block
                            string[] textureBlock = blocks[1].Split(":");
                            newMaterial.Name = textureBlock[0];
                            for (int i = 1; i < textureBlock.Length; i++)
                            {
                                newMaterial.AnimationTextures.Add(textureBlock[i]);
                            }
                            Materials.Add(newMaterial);
                        }
                    }
                }
            }
        }
        private void LoadCollisionMeshData(string inputObjectName, string inputObjectFolder)
        {
            // Get the collision mesh data
            Logger.WriteLine("- [" + inputObjectName + "]: Reading collision mesh data...");
            string collisionMeshFileName = Path.Combine(inputObjectFolder, "Meshes", inputObjectName + "_collision.txt");
            string collisionMeshData = string.Empty;
            if (File.Exists(collisionMeshFileName) == false)
            {
                Logger.WriteLine("- [" + inputObjectName + "]: No collision mesh found.");
                return;
            }

            // Load the collision data
            string inputData = File.ReadAllText(collisionMeshFileName);
            string[] inputRows = inputData.Split(Environment.NewLine);
            foreach (string inputRow in inputRows)
            {
                // Nothing for blank lines
                if (inputRow.Length == 0)
                    continue;

                // # = comment
                else if (inputRow.StartsWith("#"))
                    continue;

                // v = Verticies
                else if (inputRow.StartsWith("v"))
                {
                    string[] blocks = inputRow.Split(",");
                    if (blocks.Length != 4)
                    {
                        Logger.WriteLine("- [" + inputObjectName + "]: Error, vertex block was not 4 components");
                        continue;
                    }
                    Vector3 vertex = new Vector3();
                    vertex.X = float.Parse(blocks[1]);
                    vertex.Z = float.Parse(blocks[2]);
                    vertex.Y = float.Parse(blocks[3]);
                    CollisionVerticies.Add(vertex);
                }

                // i = Indicies
                else if (inputRow.StartsWith("i"))
                {
                    string[] blocks = inputRow.Split(",");
                    if (blocks.Length != 5)
                    {
                        Logger.WriteLine("- [" + inputObjectName + "]: Error,indicies block was not 5 components");
                        continue;
                    }
                    TriangleFace index = new TriangleFace();
                    index.MaterialIndex = int.Parse(blocks[1]);
                    index.V1 = int.Parse(blocks[2]);
                    index.V2 = int.Parse(blocks[3]);
                    index.V3 = int.Parse(blocks[4]);
                    CollisionTriangleFaces.Add(index);
                }

                else
                {
                    Logger.WriteLine("- [" + inputObjectName + "]: Error, unknown line '" + inputRow + "'");
                }
            }
        }
    }
}
