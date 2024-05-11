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
using System.Xml.Linq;
using EQWOWConverter.Common;

namespace EQWOWConverter.Zones
{
    internal class EQZoneData
    {
        public List<Vector3> Verticies { get; } = new List<Vector3>();
        public List<TextureUv> TextureCoords { get; } = new List<TextureUv>();
        public List<Vector3> Normals { get; } = new List<Vector3>();
        public List<ColorRGBA> VertexColors { get; } = new List<ColorRGBA>();
        public List<TriangleFace> TriangleFaces { get; } = new List<TriangleFace>();
        public List<Material> Materials { get; } = new List<Material>();
        public List<MapChunk> MapChunks { get; } = new List<MapChunk>(); // Potentially delete

        public ColorRGBA AmbientLight { get; } = new ColorRGBA();
        public List<LightInstance> LightInstances { get; } = new List<LightInstance>();
        public List<Vector3> CollisionVerticies { get; } = new List<Vector3>();
        public List<TriangleFace> CollisionTriangleFaces { get; } = new List<TriangleFace>();

        private string MaterialListName = string.Empty;

        public void LoadDataFromDisk(string inputZoneFolderName, string inputZoneFolderFullPath)
        {
            if (Directory.Exists(inputZoneFolderFullPath) == false)
            {
                Logger.WriteLine("- [" + inputZoneFolderName + "]: ERROR - Could not find path at '" + inputZoneFolderFullPath + "'");
                return;
            }

            // Load the various blocks
            LoadRenderMeshData(inputZoneFolderName, inputZoneFolderFullPath);
            LoadMaterialDataFromDisk(inputZoneFolderName, inputZoneFolderFullPath);            
            LoadCollisionMeshData(inputZoneFolderName, inputZoneFolderFullPath);
            LoadAmbientLightData(inputZoneFolderName, inputZoneFolderFullPath);
            LoadLightInstanceData(inputZoneFolderName, inputZoneFolderFullPath);
        }

        private void LoadRenderMeshData(string inputZoneFolderName, string inputZoneFolderFullPath)
        {
            Logger.WriteLine("- [" + inputZoneFolderName + "]: Reading render mesh data...");
            string renderMeshFileName = Path.Combine(inputZoneFolderFullPath, "Meshes", inputZoneFolderName + ".txt");
            if (File.Exists(renderMeshFileName) == false)
            {
                Logger.WriteLine("- [" + inputZoneFolderName + "]: ERROR - Could not find render mesh file that should be at '" + renderMeshFileName + "'");
                return;
            }

            // Load the core data
            string inputData = File.ReadAllText(renderMeshFileName);
            string[] inputRows = inputData.Split(Environment.NewLine);
            int curMapChunkID = 0;
            MapChunk curMapChunk = new MapChunk(curMapChunkID);
            MapChunks.Add(curMapChunk);
            bool lastRowVasVertex = true;
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
                        Logger.WriteLine("- [" + inputZoneFolderName + "]: Error, material list name needs to be 2 components");
                        continue;
                    }
                    if (MaterialListName != string.Empty)
                    {
                        Logger.WriteLine("- [" + inputZoneFolderName + "]: Error, a second material list was found");
                        continue;
                    }
                    MaterialListName = blocks[1];
                }

                // v = Verticies
                else if (inputRow.StartsWith("v"))
                {
                    if (lastRowVasVertex == false)
                    {
                        curMapChunkID++;
                        curMapChunk = new MapChunk(curMapChunkID);
                        MapChunks.Add(curMapChunk);
                        lastRowVasVertex = true;
                    }
                    string[] blocks = inputRow.Split(",");
                    if (blocks.Length != 4)
                    {
                        Logger.WriteLine("- [" + inputZoneFolderName + "]: Error, vertex block was not 4 components");
                        continue;
                    }
                    Vector3 vertex = new Vector3();
                    vertex.X = float.Parse(blocks[1]);
                    vertex.Z = float.Parse(blocks[2]);
                    vertex.Y = float.Parse(blocks[3]);
                    Verticies.Add(vertex);
                    curMapChunk.Verticies.Add(vertex);
                }

                // uv = Texture Coordinates
                else if (inputRow.StartsWith("uv"))
                {
                    string[] blocks = inputRow.Split(",");
                    if (blocks.Length != 3)
                    {
                        Logger.WriteLine("- [" + inputZoneFolderName + "]: Error, texture coordinate block was not 3 components");
                        continue;
                    }
                    TextureUv textureUv = new TextureUv();
                    textureUv.X = float.Parse(blocks[1]);
                    textureUv.Y = float.Parse(blocks[2]);
                    TextureCoords.Add(textureUv);
                    curMapChunk.TextureCoords.Add(textureUv);
                    lastRowVasVertex = false;
                }

                // n = Normal
                else if (inputRow.StartsWith("n"))
                {
                    string[] blocks = inputRow.Split(",");
                    if (blocks.Length != 4)
                    {
                        Logger.WriteLine("- [" + inputZoneFolderName + "]: Error, normals block was not 4 components");
                        continue;
                    }
                    Vector3 normal = new Vector3();
                    normal.X = float.Parse(blocks[1]);
                    normal.Y = float.Parse(blocks[2]);
                    normal.Z = float.Parse(blocks[3]);
                    Normals.Add(normal);
                    curMapChunk.Normals.Add(normal);
                    lastRowVasVertex = false;
                }

                // c = Vertex Color
                else if (inputRow.StartsWith("c"))
                {
                    string[] blocks = inputRow.Split(",");
                    if (blocks.Length != 5)
                    {
                        Logger.WriteLine("- [" + inputZoneFolderName + "]: Error, vertex color block was not 5 components");
                        continue;
                    }
                    ColorRGBA color = new ColorRGBA();
                    color.B = byte.Parse(blocks[1]);
                    color.G = byte.Parse(blocks[2]);
                    color.R = byte.Parse(blocks[3]);
                    color.A = byte.Parse(blocks[4]);
                    VertexColors.Add(color);
                    curMapChunk.VertexColors.Add(color);
                    lastRowVasVertex = false;
                }

                // i = Indicies
                else if (inputRow.StartsWith("i"))
                {
                    string[] blocks = inputRow.Split(",");
                    if (blocks.Length != 5)
                    {
                        Logger.WriteLine("- [" + inputZoneFolderName + "]: Error,indicies block was not 5 components");
                        continue;
                    }
                    TriangleFace index = new TriangleFace();
                    index.MaterialIndex = int.Parse(blocks[1]);
                    index.V1 = int.Parse(blocks[2]);
                    index.V2 = int.Parse(blocks[3]);
                    index.V3 = int.Parse(blocks[4]);
                    TriangleFaces.Add(index);
                    curMapChunk.TriangleFaces.Add(index);
                    lastRowVasVertex = false;
                }

                else
                {
                    Logger.WriteLine("- [" + inputZoneFolderName + "]: Error, unknown line '" + inputRow + "'");
                }
            }
        }
        private void LoadMaterialDataFromDisk(string inputZoneFolderName, string inputZoneFolderFullPath)
        {
            Logger.WriteLine("- [" + inputZoneFolderName + "]: Reading materials...");
            string materialListFileName = Path.Combine(inputZoneFolderFullPath, "MaterialLists", inputZoneFolderName + ".txt");
            if (File.Exists(materialListFileName) == false)
                Logger.WriteLine("- [" + inputZoneFolderName + "]: No material data found.");
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
                                Logger.WriteLine("- [" + inputZoneFolderName + "]: Error, material data is 3 components");
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
        private void LoadCollisionMeshData(string inputZoneFolderName, string inputZoneFolderFullPath)
        {
            // Get the collision mesh data
            Logger.WriteLine("- [" + inputZoneFolderName + "]: Reading collision mesh data...");
            string collisionMeshFileName = Path.Combine(inputZoneFolderFullPath, "Meshes", inputZoneFolderName + "_collision.txt");
            string collisionMeshData = string.Empty;
            if (File.Exists(collisionMeshFileName) == false)
            {
                Logger.WriteLine("- [" + inputZoneFolderName + "]: No collision mesh found.");
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
                        Logger.WriteLine("- [" + inputZoneFolderName + "]: Error, vertex block was not 4 components");
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
                        Logger.WriteLine("- [" + inputZoneFolderName + "]: Error,indicies block was not 5 components");
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
                    Logger.WriteLine("- [" + inputZoneFolderName + "]: Error, unknown line '" + inputRow + "'");
                }
            }
        }
        private void LoadAmbientLightData(string inputZoneFolderName, string inputZoneFolderFullPath)
        {
            // Get the ambient light
            Logger.WriteLine("- [" + inputZoneFolderName + "]: Reading ambiant light data...");
            string ambientLightFileName = Path.Combine(inputZoneFolderFullPath, "ambient_light.txt");
            if (File.Exists(ambientLightFileName) == false)
                Logger.WriteLine("- [" + inputZoneFolderName + "]: No ambient light data found.");
            else
            {
                using (var ambiantlightReader = new StreamReader(ambientLightFileName))
                {
                    string? curLine;
                    while ((curLine = ambiantlightReader.ReadLine()) != null)
                    {
                        // Nothing for blank lines
                        if (curLine.Length == 0)
                            continue;

                        // # = comment
                        else if (curLine.StartsWith("#"))
                            continue;

                        // 3-block is the light
                        else
                        {
                            string[] blocks = curLine.Split(",");
                            if (blocks.Length != 3)
                            {
                                Logger.WriteLine("- [" + inputZoneFolderName + "]: Error, ambiant light data must be in 3 components");
                                continue;
                            }
                            AmbientLight.R = byte.Parse(blocks[0]);
                            AmbientLight.G = byte.Parse(blocks[1]);
                            AmbientLight.B = byte.Parse(blocks[2]);
                        }
                    }
                }
            }
        }
        private void LoadLightInstanceData(string inputZoneFolder, string inputZoneFolderFullPath)
        {
            // Get the light instances
            Logger.WriteLine("- [" + inputZoneFolder + "]: Reading light instances...");
            string lightInstancesFileName = Path.Combine(inputZoneFolderFullPath, "light_instances.txt");
            if (File.Exists(lightInstancesFileName) == false)
                Logger.WriteLine("- [" + inputZoneFolder + "]: No light instance data found.");
            else
            {
                using (var lightInstancesReader = new StreamReader(lightInstancesFileName))
                {
                    string? curLine;
                    while ((curLine = lightInstancesReader.ReadLine()) != null)
                    {
                        // Nothing for blank lines
                        if (curLine.Length == 0)
                            continue;

                        // # = comment
                        else if (curLine.StartsWith("#"))
                            continue;

                        // 7-blocks is a light instance
                        else
                        {
                            string[] blocks = curLine.Split(",");
                            if (blocks.Length != 7)
                            {
                                Logger.WriteLine("- [" + inputZoneFolder + "]: Error, light instance data is 7 components");
                                continue;
                            }
                            LightInstance newLightInstance = new LightInstance();
                            newLightInstance.Position.X = float.Parse(blocks[0]);
                            newLightInstance.Position.Y = float.Parse(blocks[1]);
                            newLightInstance.Position.Z = float.Parse(blocks[2]);
                            newLightInstance.Radius = float.Parse(blocks[3]);
                            newLightInstance.Color.R = float.Parse(blocks[4]);
                            newLightInstance.Color.G = float.Parse(blocks[5]);
                            newLightInstance.Color.B = float.Parse(blocks[6]);
                            LightInstances.Add(newLightInstance);
                        }
                    }
                }
            }
        }
    }
}
