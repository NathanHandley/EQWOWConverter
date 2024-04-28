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
    internal class ZoneMesh
    {
        public List<Vector3> Verticies = new List<Vector3>();
        public List<TextureUv> TextureCoords = new List<TextureUv>();
        public List<Vector3> Normals = new List<Vector3>();
        public List<ColorRGBA> VertexColors = new List<ColorRGBA>();
        public List<PolyIndex> Indicies = new List<PolyIndex>();
        public string MaterialListName = string.Empty;
        
        public ZoneMesh()
        {

        }
        public ZoneMesh(string parentName, string inputData)
        {
            GenerateCompleteZoneMesh(parentName, inputData);
            GenerateZoneSubMeshes(parentName, inputData);
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
                    PolyIndex index = new PolyIndex();
                    index.MaterialIndex = int.Parse(blocks[1]);
                    index.V1 = int.Parse(blocks[2]);
                    index.V2 = int.Parse(blocks[3]);
                    index.V3 = int.Parse(blocks[4]);
                    Indicies.Add(index);
                }

                else
                {
                    Logger.WriteLine("- [" + parentName + "]: Error, unknown line '" + inputRow + "'");
                }
            }
        }
        private void GenerateZoneSubMeshes(string parentName, string inputData)
        {
            // TODO: Break the zone into 1000x1000 x/y position chunks so larger zones can be loaded.
        }
    }
}
