﻿//  Author: Nathan Handley (nathanhandley@protonmail.com)
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

namespace EQWOWConverter.EQFiles
{
    internal class EQMesh
    {
        public struct BoneReference
        {
            public byte KeyBoneID = 0;
            public int VertStart = 0;
            public int VertCount = 0;

            public BoneReference() { }
        }

        public MeshData Meshdata = new MeshData();
        public string MaterialListFileName = string.Empty;       
        public List<BoneReference> Bones = new List<BoneReference>();

        public EQMesh() { }

        public EQMesh(EQMesh other)
        {
            Meshdata = new MeshData(other.Meshdata);
            MaterialListFileName = other.MaterialListFileName;
            Bones = new List<BoneReference>(other.Bones);
        }

        public bool LoadFromDisk(string fileFullPath)
        {
            Logger.WriteDebug(" - Reading EQ Mesh Data from '" + fileFullPath + "'...");
            if (File.Exists(fileFullPath) == false)
            {
                Logger.WriteError("- Could not find mesh file that should be at '" + fileFullPath + "'");
                return false;
            }

            // Load the core data
            int curAnimatedVertexFrame = -1;
            int curAnimatedVertexIndex = -1;
            string inputData = FileTool.ReadAllDataFromFile(fileFullPath);
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

                // b = Bones
                else if (inputRow.StartsWith("b"))
                {
                    string[] blocks = inputRow.Split(",");
                    if (blocks.Length != 4)
                    {
                        Logger.WriteError("- Bones block must be 4 components");
                        continue;
                    }

                    BoneReference boneReference = new BoneReference();
                    boneReference.KeyBoneID = byte.Parse(blocks[1]);
                    boneReference.VertStart = int.Parse(blocks[2]);
                    boneReference.VertCount = int.Parse(blocks[3]);
                    Bones.Add(boneReference);
                }

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
                    if (Meshdata.AnimatedVerticesDelayInMS != 0)
                    {
                        Logger.WriteError("- There was already a read frame delay");
                        continue;
                    }
                    Meshdata.AnimatedVerticesDelayInMS = int.Parse(blocks[1]);
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
                    float xPos = float.Parse(blocks[2]);
                    float zPos = float.Parse(blocks[3]);
                    float yPos = float.Parse(blocks[4]);                    
                    if (frameIndex != curAnimatedVertexFrame)
                    {
                        curAnimatedVertexFrame = frameIndex;
                        curAnimatedVertexIndex = 0;
                    }
                    else
                        curAnimatedVertexIndex++;
                    if (frameIndex == 0)
                        Meshdata.AnimatedVertexFramesByVertexIndex.Add(new AnimatedVertexFrames());
                    Meshdata.AnimatedVertexFramesByVertexIndex[curAnimatedVertexIndex].VertexOffsetFrames.Add(new Vector3(xPos, yPos, zPos));
                    Meshdata.AnimatedVertexFramesByVertexIndex[curAnimatedVertexIndex].VertexOffsetFramesInStringLiteral.Add(new AnimatedVertexFrames.Vector3StringLiteral(blocks[2], blocks[4], blocks[3]));
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

            Meshdata.DeleteInvalidTriangles();
            if (Meshdata.AnimatedVertexFramesByVertexIndex.Count > 0)
                if (Meshdata.AnimatedVertexFramesByVertexIndex.Count != Meshdata.Vertices.Count)
                {
                    Logger.WriteError("EQMesh loading issue for mesh '" + fileFullPath + "' Animated Vertex Frames did not match Vertices count");
                    return false;
                }

            // Clean up animated vertex information
            if (Meshdata.AnimatedVertexFramesByVertexIndex.Count > 0)
            {
                // Reduce all amounts by the actual vertex locations
                for (int i = 0; i < Meshdata.AnimatedVertexFramesByVertexIndex.Count; i++)
                {
                    for (int j = 0; j < Meshdata.AnimatedVertexFramesByVertexIndex[i].VertexOffsetFrames.Count; j++)
                    {
                        Vector3 newVector = new Vector3(
                            Meshdata.AnimatedVertexFramesByVertexIndex[i].VertexOffsetFrames[j].X - Meshdata.Vertices[i].X,
                            Meshdata.AnimatedVertexFramesByVertexIndex[i].VertexOffsetFrames[j].Y - Meshdata.Vertices[i].Y,
                            Meshdata.AnimatedVertexFramesByVertexIndex[i].VertexOffsetFrames[j].Z - Meshdata.Vertices[i].Z);
                        Meshdata.AnimatedVertexFramesByVertexIndex[i].VertexOffsetFrames[j] = newVector;
                    }
                }

                // Remove frames that are zeroed but not the first
                List<int> framesToDelete = new List<int>();
                for (int i = 1; i < Meshdata.AnimatedVertexFramesByVertexIndex[0].VertexOffsetFramesInStringLiteral.Count; i++)
                {
                    bool hasNonZero = false;
                    for (int j = 0; j < Meshdata.AnimatedVertexFramesByVertexIndex.Count; j++)
                    {   
                        if (MathF.Abs(Meshdata.AnimatedVertexFramesByVertexIndex[j].VertexOffsetFrames[i].X) > float.Epsilon ||
                            MathF.Abs(Meshdata.AnimatedVertexFramesByVertexIndex[j].VertexOffsetFrames[i].Y) > float.Epsilon ||
                            MathF.Abs(Meshdata.AnimatedVertexFramesByVertexIndex[j].VertexOffsetFrames[i].Z) > float.Epsilon)
                        {
                            hasNonZero = true;
                            break;
                        }
                    }
                    if (hasNonZero == false)
                        framesToDelete.Add(i);
                }
                for (int i = framesToDelete.Count - 1; i >= 0; i--)
                {
                    for (int j = 0; j < Meshdata.AnimatedVertexFramesByVertexIndex.Count; j++)
                    {
                        Meshdata.AnimatedVertexFramesByVertexIndex[j].VertexOffsetFrames.RemoveAt(framesToDelete[i]);
                        Meshdata.AnimatedVertexFramesByVertexIndex[j].VertexOffsetFramesInStringLiteral.RemoveAt(framesToDelete[i]);
                    }
                }

                // Anywhere there is no movement at all, purge that list of offsets
                for (int i = 0; i < Meshdata.AnimatedVertexFramesByVertexIndex.Count; i++)
                {
                    bool allTheSame = true;
                    string xValue = string.Empty;
                    string yValue = string.Empty;
                    string zValue = string.Empty;
                    for (int j = 0; j < Meshdata.AnimatedVertexFramesByVertexIndex[i].VertexOffsetFramesInStringLiteral.Count; ++j)
                    {
                        if (j == 0)
                        {
                            xValue = Meshdata.AnimatedVertexFramesByVertexIndex[i].VertexOffsetFramesInStringLiteral[j].XString;
                            yValue = Meshdata.AnimatedVertexFramesByVertexIndex[i].VertexOffsetFramesInStringLiteral[j].YString;
                            zValue = Meshdata.AnimatedVertexFramesByVertexIndex[i].VertexOffsetFramesInStringLiteral[j].ZString;
                        }
                        else if (Meshdata.AnimatedVertexFramesByVertexIndex[i].VertexOffsetFramesInStringLiteral[j].XString != xValue ||
                            Meshdata.AnimatedVertexFramesByVertexIndex[i].VertexOffsetFramesInStringLiteral[j].YString != yValue ||
                            Meshdata.AnimatedVertexFramesByVertexIndex[i].VertexOffsetFramesInStringLiteral[j].ZString != zValue)
                        {
                            allTheSame = false;
                            break;
                        }
                    }
                    if (allTheSame == true)
                    {
                        Meshdata.AnimatedVertexFramesByVertexIndex[i].VertexOffsetFrames.Clear();
                        Meshdata.AnimatedVertexFramesByVertexIndex[i].VertexOffsetFramesInStringLiteral.Clear();
                    }
                }
            }

            Logger.WriteDebug(" - Done reading EQ Mesh Data from '" + fileFullPath + "'");
            return true;
        }
    }
}
