using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using EQWOWConverter.EQObjects;
using EQWOWConverter.Core;
using Warcraft.WMO;
using Warcraft.WMO.GroupFile;
using Vector3 = EQWOWConverter.Core.Vector3;

namespace EQWOWConverter
{
    internal class AssetConverter
    {
        public static bool ConvertEQZonesToWOW(string eqExportsCondensedPath)
        {
            // Temp for now, just use one zone

            // Make sure the root path exists
            if (Directory.Exists(eqExportsCondensedPath) == false)
            {
                Logger.WriteLine("ERROR - Condensed path of '" + eqExportsCondensedPath + "' does not exist.");
                Logger.WriteLine("Conversion Failed!");
                return false;
            }

            // For now, let's test by grabbing a specific zone.  west freeport
            string zonePath = Path.Combine(eqExportsCondensedPath, "zones", "freportw");
            return ConvertZone("freportw", zonePath);
        }

        private static bool ConvertZone(string zoneName, string zonePath)
        {
            Logger.WriteLine("Converting zone '" + zoneName + "' at '" + zonePath);
            if (Directory.Exists(zonePath) == false)
            {
                Logger.WriteLine("- [" + zoneName + "]: ERROR - Could not find path at '" + zonePath + "'");
                return false;
            }

            // Get the render mesh data
            Logger.WriteLine("- [" + zoneName + "]: Reading render mesh data...");
            string meshFileName = Path.Combine(zonePath, "Meshes", zoneName + ".txt");
            if (File.Exists(meshFileName) == false)
            {
                Logger.WriteLine("- [" + zoneName + "]: ERROR - Could not find mesh file that should be at '" + meshFileName + "'");
                return false;
            }

            Zone curZone = new Zone();
            using (var meshStreamReader = new StreamReader(meshFileName))
            {
                string? curLine;
                while ((curLine = meshStreamReader.ReadLine()) != null)
                {
                    // Nothing for blank lines
                    if (curLine.Length == 0)
                        continue;

                    // # = comment
                    else if (curLine.StartsWith("#"))
                        continue;

                    // ml = Material List
                    else if (curLine.StartsWith("ml"))
                    {
                        Logger.WriteLine("- [" + zoneName + "]: Skipped Material List line (NYI)");
                        continue;
                    }

                    // v = Verticies
                    else if (curLine.StartsWith("v"))
                    {
                        string[] blocks = curLine.Split(",");
                        if (blocks.Length != 4)
                        {
                            Logger.WriteLine("- [" + zoneName + "]: Error, render mesh vertex block was not 4 components");
                            continue;
                        }
                        Vector3 vertex = new Vector3();
                        vertex.X = float.Parse(blocks[1]);
                        vertex.Z = float.Parse(blocks[2]);
                        vertex.Y = float.Parse(blocks[3]);
                        curZone.RenderMesh.Verticies.Add(vertex);
                    }

                    // uv = Texture Coordinates
                    else if (curLine.StartsWith("uv"))
                    {
                        string[] blocks = curLine.Split(",");
                        if (blocks.Length != 3)
                        {
                            Logger.WriteLine("- [" + zoneName + "]: Error, render mesh texture coordinate block was not 3 components");
                            continue;
                        }
                        TextureUv textureUv = new TextureUv();
                        textureUv.X = float.Parse(blocks[1]);
                        textureUv.Y = float.Parse(blocks[2]);
                        curZone.RenderMesh.TextureCoords.Add(textureUv);
                    }

                    // n = Normal
                    else if (curLine.StartsWith("n"))
                    {
                        string[] blocks = curLine.Split(",");
                        if (blocks.Length != 4)
                        {
                            Logger.WriteLine("- [" + zoneName + "]: Error, render mesh normals block was not 4 components");
                            continue;
                        }
                        Vector3 normal = new Vector3();
                        normal.X = float.Parse(blocks[1]);
                        normal.Y = float.Parse(blocks[2]);
                        normal.Z = float.Parse(blocks[3]);
                        curZone.RenderMesh.Normals.Add(normal);
                    }

                    // c = Vertex Color
                    else if (curLine.StartsWith("c"))
                    {
                        string[] blocks = curLine.Split(",");
                        if (blocks.Length != 5)
                        {
                            Logger.WriteLine("- [" + zoneName + "]: Error, render mesh vertex color block was not 5 components");
                            continue;
                        }
                        ColorRGBA color = new ColorRGBA();
                        color.B = int.Parse(blocks[1]);
                        color.G = int.Parse(blocks[2]);
                        color.R = int.Parse(blocks[3]);
                        color.A = int.Parse(blocks[4]);
                        curZone.RenderMesh.VertexColors.Add(color);
                    }

                    // i = Indicies
                    else if (curLine.StartsWith("i"))
                    {
                        string[] blocks = curLine.Split(",");
                        if (blocks.Length != 5)
                        {
                            Logger.WriteLine("- [" + zoneName + "]: Error, render mesh indicies block was not 5 components");
                            continue;
                        }
                        PolyIndex index = new PolyIndex();
                        index.MaterialIndex = int.Parse(blocks[1]);
                        index.V1 = int.Parse(blocks[2]);
                        index.V2 = int.Parse(blocks[3]);
                        index.V3 = int.Parse(blocks[4]);
                        curZone.RenderMesh.Indicies.Add(index);
                    }

                    else
                    {
                        Logger.WriteLine("- [" + zoneName + "]: Error, render mesh unknown line '" + curLine + "'");
                    }
                }
            }

            // Get the collision mesh data
            Logger.WriteLine("- [" + zoneName + "]: Reading collision mesh data...");
            string collisionMeshFileName = Path.Combine(zonePath, "Meshes", zoneName + "_collision.txt");
            if (File.Exists(collisionMeshFileName) == false)
            {
                Logger.WriteLine("- [" + zoneName + "]: No collision mesh found.  Skipping");
            }
            else
            {
                using (var meshStreamReader = new StreamReader(collisionMeshFileName))
                {
                    string? curLine;
                    while ((curLine = meshStreamReader.ReadLine()) != null)
                    {
                        // Nothing for blank lines
                        if (curLine.Length == 0)
                            continue;

                        // # = comment
                        else if (curLine.StartsWith("#"))
                            continue;

                        // v = Verticies
                        else if (curLine.StartsWith("v"))
                        {
                            string[] blocks = curLine.Split(",");
                            if (blocks.Length != 4)
                            {
                                Logger.WriteLine("- [" + zoneName + "]: Error, collision mesh vertex block was not 4 components");
                                continue;
                            }
                            Vector3 vertex = new Vector3();
                            vertex.X = float.Parse(blocks[1]);
                            vertex.Z = float.Parse(blocks[2]);
                            vertex.Y = float.Parse(blocks[3]);
                            curZone.CollisionMesh.Verticies.Add(vertex);
                        }

                        // i = Indicies
                        else if (curLine.StartsWith("i"))
                        {
                            string[] blocks = curLine.Split(",");
                            if (blocks.Length != 5)
                            {
                                Logger.WriteLine("- [" + zoneName + "]: Error, collision mesh indicies block was not 5 components");
                                continue;
                            }
                            PolyIndex index = new PolyIndex();
                            index.MaterialIndex = int.Parse(blocks[1]);
                            index.V1 = int.Parse(blocks[2]);
                            index.V2 = int.Parse(blocks[3]);
                            index.V3 = int.Parse(blocks[4]);
                            curZone.CollisionMesh.Indicies.Add(index);
                        }

                        else
                        {
                            Logger.WriteLine("- [" + zoneName + "]: Error, collision mesh unknown line '" + curLine + "'");
                        }
                    }
                }
            }

            Logger.WriteLine("Conversion of '" + zoneName + "' complete");
            return true;
        }
    }
}
