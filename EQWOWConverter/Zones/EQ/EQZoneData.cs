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
using EQWOWConverter.EQFiles;

namespace EQWOWConverter.Zones
{
    internal class EQZoneData
    {
        private bool IsLoaded = false;
        private string MaterialListFileName = string.Empty;
        public List<Vector3> Verticies = new List<Vector3>();
        public List<TextureCoordinates> TextureCoords = new List<TextureCoordinates>();
        public List<Vector3> Normals = new List<Vector3>();
        public List<ColorRGBA> VertexColors = new List<ColorRGBA>();
        public List<TriangleFace> TriangleFaces = new List<TriangleFace>();
        public List<Material> Materials = new List<Material>();

        public ColorRGBA AmbientLight = new ColorRGBA();
        public List<LightInstance> LightInstances = new List<LightInstance>();
        public List<ObjectInstance> ObjectInstances = new List<ObjectInstance>();
        public List<Vector3> CollisionVerticies = new List<Vector3>();
        public List<TriangleFace> CollisionTriangleFaces = new List<TriangleFace>();

        private string MaterialListName = string.Empty;

        public void LoadDataFromDisk(string inputZoneFolderName, string inputZoneFolderFullPath)
        {
            if (IsLoaded == true)
                return;
            if (Directory.Exists(inputZoneFolderFullPath) == false)
            {
                Logger.WriteError("- [" + inputZoneFolderName + "]: ERROR - Could not find path at '" + inputZoneFolderFullPath + "'");
                return;
            }

            // Load the various blocks
            LoadRenderMeshData(inputZoneFolderName, inputZoneFolderFullPath);
            LoadMaterialDataFromDisk(inputZoneFolderName, inputZoneFolderFullPath);            
            LoadCollisionMeshData(inputZoneFolderName, inputZoneFolderFullPath);
            LoadAmbientLightData(inputZoneFolderName, inputZoneFolderFullPath);
            LoadLightInstanceData(inputZoneFolderName, inputZoneFolderFullPath);
            LoadObjectInstanceData(inputZoneFolderName, inputZoneFolderFullPath);
            IsLoaded = true;
        }

        private void LoadRenderMeshData(string inputZoneFolderName, string inputZoneFolderFullPath)
        {
            Logger.WriteDetail("- [" + inputZoneFolderName + "]: Reading render mesh data...");
            string renderMeshFileName = Path.Combine(inputZoneFolderFullPath, "Meshes", inputZoneFolderName + ".txt");
            EQMesh meshData = new EQMesh();
            if (meshData.LoadFromDisk(renderMeshFileName) == false)
            {
                Logger.WriteError("- [" + inputZoneFolderName + "]: ERROR - Could not find render mesh file that should be at '" + renderMeshFileName + "'");
                return;
            }
            Verticies = meshData.Verticies;
            Normals = meshData.Normals;
            TextureCoords = meshData.TextureCoordinates;
            TriangleFaces = meshData.TriangleFaces;
            VertexColors = meshData.VertexColors;
            MaterialListFileName = meshData.MaterialListFileName;
        }

        private void LoadMaterialDataFromDisk(string inputZoneFolderName, string inputZoneFolderFullPath)
        {
            Logger.WriteDetail("- [" + inputZoneFolderName + "]: Reading materials...");
            if (MaterialListFileName == string.Empty)
            {
                Logger.WriteError("- [" + inputZoneFolderName + "]: No material file name found");
                return;
            }
            string materialListFileName = Path.Combine(inputZoneFolderFullPath, "MaterialLists", MaterialListFileName + ".txt");
            EQMaterialList materialListData = new EQMaterialList();
            if (materialListData.LoadFromDisk(materialListFileName) == false)
                Logger.WriteDetail("- [" + inputZoneFolderName + "]: No material data found.");
            else
            {
                Materials = materialListData.Materials;
            }
        }
        private void LoadCollisionMeshData(string inputZoneFolderName, string inputZoneFolderFullPath)
        {
            // Get the collision mesh data
            Logger.WriteDetail("- [" + inputZoneFolderName + "]: Reading collision mesh data...");
            string collisionMeshFileName = Path.Combine(inputZoneFolderFullPath, "Meshes", inputZoneFolderName + "_collision.txt");
            string collisionMeshData = string.Empty;
            if (File.Exists(collisionMeshFileName) == false)
            {
                Logger.WriteDetail("- [" + inputZoneFolderName + "]: No collision mesh found.");
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
                        Logger.WriteError("- [" + inputZoneFolderName + "]: Error, vertex block was not 4 components");
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
                        Logger.WriteError("- [" + inputZoneFolderName + "]: Error,indicies block was not 5 components");
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
                    Logger.WriteError("- [" + inputZoneFolderName + "]: Error, unknown line '" + inputRow + "'");
                }
            }
        }
        private void LoadAmbientLightData(string inputZoneFolderName, string inputZoneFolderFullPath)
        {
            // Get the ambient light
            Logger.WriteDetail("- [" + inputZoneFolderName + "]: Reading ambiant light data...");
            string ambientLightFileName = Path.Combine(inputZoneFolderFullPath, "ambient_light.txt");
            if (File.Exists(ambientLightFileName) == false)
                Logger.WriteDetail("- [" + inputZoneFolderName + "]: No ambient light data found.");
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
                                Logger.WriteError("- [" + inputZoneFolderName + "]: Error, ambiant light data must be in 3 components");
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

        private void LoadObjectInstanceData(string inputZoneFolder, string inputZoneFolderFullPath)
        {
            // Get the object instances
            Logger.WriteDetail("- [" + inputZoneFolder + "]: Reading object instances...");
            string objectInstancesFileName = Path.Combine(inputZoneFolderFullPath, "object_instances.txt");
            if (File.Exists(objectInstancesFileName) == false)
                Logger.WriteDetail("- [" + inputZoneFolder + "]: No object instance data found.");
            else
            {
                using (var objectInstancesReader = new StreamReader(objectInstancesFileName))
                {
                    string? curLine;
                    while ((curLine = objectInstancesReader.ReadLine()) != null)
                    {
                        // Nothing for blank lines
                        if (curLine.Length == 0)
                            continue;

                        // # = comment
                        else if (curLine.StartsWith("#"))
                            continue;

                        //11-blocks is an object instance
                        else
                        {
                            string[] blocks = curLine.Split(",");
                            if (blocks.Length != 11)
                            {
                                Logger.WriteError("- [" + inputZoneFolder + "]: Error, object instance data is 7 components");
                                continue;
                            }

                            ObjectInstance newObjectInstance = new ObjectInstance();
                            newObjectInstance.ModelName = blocks[0];
                            newObjectInstance.Position.X = float.Parse(blocks[1]);
                            newObjectInstance.Position.Y = float.Parse(blocks[2]);
                            newObjectInstance.Position.Z = float.Parse(blocks[3]);
                            newObjectInstance.Rotation.X = float.Parse(blocks[4]);
                            newObjectInstance.Rotation.Y = float.Parse(blocks[5]);
                            newObjectInstance.Rotation.Z = float.Parse(blocks[6]);
                            newObjectInstance.Scale.X = float.Parse(blocks[7]);
                            newObjectInstance.Scale.Y = float.Parse(blocks[8]);
                            newObjectInstance.Scale.Z = float.Parse(blocks[9]);
                            newObjectInstance.ColorIndex = Int32.Parse(blocks[10]);
                            ObjectInstances.Add(newObjectInstance);
                        }
                    }
                }
            }
        }

        private void LoadLightInstanceData(string inputZoneFolder, string inputZoneFolderFullPath)
        {
            // Get the light instances
            Logger.WriteDetail("- [" + inputZoneFolder + "]: Reading light instances...");
            string lightInstancesFileName = Path.Combine(inputZoneFolderFullPath, "light_instances.txt");
            if (File.Exists(lightInstancesFileName) == false)
                Logger.WriteDetail("- [" + inputZoneFolder + "]: No light instance data found.");
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
                                Logger.WriteError("- [" + inputZoneFolder + "]: Error, light instance data is 7 components");
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
