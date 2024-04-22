using EQWOWConverter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.EQObjects
{
    internal class Zone
    {
        public string Name = string.Empty;

        public Mesh RenderMesh = new Mesh();
        public Mesh CollisionMesh = new Mesh();

        public ColorRGB AmbientLight = new ColorRGB();
        public List<LightInstance> LightInstances = new List<LightInstance>();
        public List<Material> Materials = new List<Material>();

        public Zone(string name, string zoneFolder)
        {
            // Store name
            Name = name;

            if (Directory.Exists(zoneFolder) == false)
            {
                Logger.WriteLine("- [" + Name + "]: ERROR - Could not find path at '" + zoneFolder + "'");
                return;
            }

            // Get the render mesh data
            Logger.WriteLine("- [" + Name + "]: Reading render mesh data...");
            string renderMeshFileName = Path.Combine(zoneFolder, "Meshes", Name + ".txt");
            if (File.Exists(renderMeshFileName) == false)
                Logger.WriteLine("- [" + Name + "]: ERROR - Could not find render mesh file that should be at '" + renderMeshFileName + "'");
            else
                RenderMesh = new Mesh(name, File.ReadAllText(renderMeshFileName));

            // Get the collision mesh data
            Logger.WriteLine("- [" + Name + "]: Reading collision mesh data...");
            string collisionMeshFileName = Path.Combine(zoneFolder, "Meshes", Name + "_collision.txt");
            string collisionMeshData = string.Empty;
            if (File.Exists(collisionMeshFileName) == false)
                Logger.WriteLine("- [" + Name + "]: No collision mesh found.");
            else
                CollisionMesh = new Mesh(name, File.ReadAllText(collisionMeshFileName));

            // Get the ambient light
            Logger.WriteLine("- [" + Name + "]: Reading ambiant light data...");
            string ambientLightFileName = Path.Combine(zoneFolder, "ambient_light.txt");
            if (File.Exists(ambientLightFileName) == false)
                Logger.WriteLine("- [" + Name + "]: No ambient light data found.");
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
                                Logger.WriteLine("- [" + Name + "]: Error, ambiant light data must be in 3 components");
                                continue;
                            }
                            AmbientLight.R = int.Parse(blocks[0]);
                            AmbientLight.G = int.Parse(blocks[1]);
                            AmbientLight.B = int.Parse(blocks[2]);
                        }
                    }
                }
            }

            // Get the light instances
            Logger.WriteLine("- [" + Name + "]: Reading light instances...");
            string lightInstancesFileName = Path.Combine(zoneFolder, "light_instances.txt");
            if (File.Exists(lightInstancesFileName) == false)
                Logger.WriteLine("- [" + Name + "]: No light instance data found.");
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
                                Logger.WriteLine("- [" + Name + "]: Error, light instance data is 7 components");
                                continue;
                            }
                            LightInstance newLightInstance = new LightInstance();
                            newLightInstance.Position.X = float.Parse(blocks[0]);
                            newLightInstance.Position.Y = float.Parse(blocks[1]);
                            newLightInstance.Position.Z = float.Parse(blocks[2]);
                            newLightInstance.Radius = float.Parse(blocks[3]);
                            newLightInstance.Color.R = float.Parse(blocks[4]);
                            newLightInstance.Color.G = float.Parse(blocks[4]);
                            newLightInstance.Color.B = float.Parse(blocks[4]);
                            LightInstances.Add(newLightInstance);
                        }
                    }
                }
            }

            // Get the materials
            Logger.WriteLine("- [" + Name + "]: Reading materials...");
            string materialListFileName = Path.Combine(zoneFolder, "MaterialLists", Name + ".txt");
            if (File.Exists(materialListFileName) == false)
                Logger.WriteLine("- [" + Name + "]: No material data found.");
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
                                Logger.WriteLine("- [" + Name + "]: Error, material data is 3 components");
                                continue;
                            }
                            Material newMaterial = new Material();
                            newMaterial.Index = uint.Parse(blocks[0]);
                            newMaterial.AnimationDelayMs = uint.Parse(blocks[2]);

                            // Texture block
                            string[] textureBlock = blocks[1].Split(":");
                            newMaterial.Name = textureBlock[0];
                            for (int i = 1; i < textureBlock.Length; i++)
                                newMaterial.AnimationTextures.Add(textureBlock[i]);
                            Materials.Add(newMaterial);
                        }
                    }
                }
            }
        }
    }
}
