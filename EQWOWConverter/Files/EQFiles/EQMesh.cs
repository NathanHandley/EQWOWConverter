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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.EQFiles
{
    internal class EQMesh
    {
        public MeshData Meshdata = new MeshData();
        public AnimatedVertices AnimatedVertices = new AnimatedVertices();
        public string MaterialListFileName = string.Empty;
        // TODO: Bones

        public bool LoadFromDisk(string fileFullPath)
        {
            Logger.WriteDetail(" - Reading EQ Mesh Data from '" + fileFullPath + "'...");
            if (File.Exists(fileFullPath) == false)
            {
                Logger.WriteError("- Could not find mesh file that should be at '" + fileFullPath + "'");
                return false;
            }

            // Load the core data
            string inputData = File.ReadAllText(fileFullPath);
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
                        Logger.WriteError("- MaterialList block must be 2 components");
                        continue;
                    }
                    MaterialListFileName = blocks[1];
                }   

                // b = Bones (?)
                else if (inputRow.StartsWith("b"))
                    continue; // Skip for now

                // v = Vertices
                else if (inputRow.StartsWith("v"))
                {
                    string[] blocks = inputRow.Split(",");
                    if (blocks.Length != 4)
                    {
                        Logger.WriteError("- Vertex block was not 4 components");
                        continue;
                    }
                    Vector3 vertex = new Vector3();
                    vertex.X = float.Parse(blocks[1]);
                    vertex.Z = float.Parse(blocks[2]);
                    vertex.Y = float.Parse(blocks[3]);
                    Meshdata.Vertices.Add(vertex);
                }

                // ad = Animation head
                else if (inputRow.StartsWith("ad"))
                {
                    string[] blocks = inputRow.Split(",");
                    if (blocks.Length != 2)
                    {
                        Logger.WriteError("- Animated head block was not 2 components");
                        continue;
                    }
                    if (AnimatedVertices.FrameDelay != 0)
                    {
                        Logger.WriteError("- There was already a read frame delay");
                        continue;
                    }
                    AnimatedVertices.FrameDelay = int.Parse(blocks[1]);
                }

                // av = Animation vertices
                else if (inputRow.StartsWith("av"))
                {
                    string[] blocks = inputRow.Split(",");
                    if (blocks.Length != 5)
                    {
                        Logger.WriteError("- Animation vertices block was not 5 components");
                        continue;
                    }

                    int frameIndex = int.Parse(blocks[1]);
                    if (frameIndex >= AnimatedVertices.GetFrameCount())
                        AnimatedVertices.Frames.Add(new List<Vector3>());
                    float xPos = float.Parse(blocks[2]);
                    float zPos = float.Parse(blocks[3]);
                    float yPos = float.Parse(blocks[4]);
                    AnimatedVertices.Frames[frameIndex].Add(new Vector3(xPos, yPos, zPos));
                }

                // uv = Texture Coordinates
                else if (inputRow.StartsWith("uv"))
                {
                    string[] blocks = inputRow.Split(",");
                    if (blocks.Length != 3)
                    {
                        Logger.WriteError("- Texture coordinate block was not 3 components");
                        continue;
                    }
                    TextureCoordinates textureUv = new TextureCoordinates();
                    textureUv.X = float.Parse(blocks[1]);
                    textureUv.Y = float.Parse(blocks[2]);
                    Meshdata.TextureCoordinates.Add(textureUv);
                }

                // n = Normal
                else if (inputRow.StartsWith("n"))
                {
                    string[] blocks = inputRow.Split(",");
                    if (blocks.Length != 4)
                    {
                        Logger.WriteError("- Normals block was not 4 components");
                        continue;
                    }
                    Vector3 normal = new Vector3();
                    normal.X = float.Parse(blocks[1]);
                    normal.Y = float.Parse(blocks[2]);
                    normal.Z = float.Parse(blocks[3]);
                    Meshdata.Normals.Add(normal);
                }

                // c = Vertex Color
                else if (inputRow.StartsWith("c"))
                {
                    string[] blocks = inputRow.Split(",");
                    if (blocks.Length != 5)
                    {
                        Logger.WriteError("- Vertex color block was not 5 components");
                        continue;
                    }
                    ColorRGBA color = new ColorRGBA();
                    color.B = byte.Parse(blocks[1]);
                    color.G = byte.Parse(blocks[2]);
                    color.R = byte.Parse(blocks[3]);
                    color.A = byte.Parse(blocks[4]);
                    Meshdata.VertexColors.Add(color);
                }

                // i = Indices
                else if (inputRow.StartsWith("i"))
                {
                    string[] blocks = inputRow.Split(",");
                    if (blocks.Length != 5)
                    {
                        Logger.WriteError("- Indices block was not 5 components");
                        continue;
                    }
                    TriangleFace index = new TriangleFace();
                    index.MaterialIndex = int.Parse(blocks[1]);
                    index.V1 = int.Parse(blocks[2]);
                    index.V2 = int.Parse(blocks[3]);
                    index.V3 = int.Parse(blocks[4]);
                    Meshdata.TriangleFaces.Add(index);
                }

                else
                {
                    Logger.WriteError("- Unknown line '" + inputRow + "'");
                }
            }

            Logger.WriteDetail(" - Done reading EQ Mesh Data from '" + fileFullPath + "'");
            return true;
        }
    }
}
