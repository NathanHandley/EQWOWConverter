//  Author: Nathan Handley (nathanhandley@protonmail.com)
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
    internal class EQSkeleton
    {
        public struct EQSkeletonBone
        {
            public string BoneName = string.Empty;
            public List<int> Children = new List<int>();
            public string MeshName = string.Empty;
            public string AlternateMeshName = string.Empty;
            public string ParticleCloudName = string.Empty;

            public EQSkeletonBone() { }
        }

        public List<string> MeshNames = new List<string>();
        public List<string> SecondaryMeshNames = new List<string>();
        public List<EQSkeletonBone> BoneStructures = new List<EQSkeletonBone>();

        public EQSkeleton() { }
        public EQSkeleton(EQSkeleton other)
        {
            MeshNames = new List<string>(other.MeshNames.Count);
            MeshNames.AddRange(other.MeshNames);
            SecondaryMeshNames = new List<string>(other.SecondaryMeshNames.Count);
            SecondaryMeshNames.AddRange(other.SecondaryMeshNames);
            BoneStructures = new List<EQSkeletonBone>(other.BoneStructures.Count);
            BoneStructures.AddRange(other.BoneStructures);
        }

        public void LoadFromAnimatedVerticesData(ref MeshData meshData)
        {
            // Clear existing bones
            meshData.BoneIDs.Clear();
            BoneStructures.Clear();
            MeshNames.Clear();
            SecondaryMeshNames.Clear();

            // There's always a root bone, and all animated vertices are children
            EQSkeletonBone rootBone = new EQSkeletonBone();
            rootBone.BoneName = "root";
            rootBone.MeshName = string.Empty;
            rootBone.AlternateMeshName = string.Empty;
            rootBone.ParticleCloudName = string.Empty;
            BoneStructures.Add(rootBone);

            // Every animated vertex gets a bone, and assign the bone ID to the mesh data accordingly
            int curBoneID = 1;
            for (int i = 0; i < meshData.AnimatedVertexFramesByVertexIndex.Count; i++)
            {
                if (meshData.AnimatedVertexFramesByVertexIndex[i].VertexOffsetFramesInStringLiteral.Count > 0)
                {
                    EQSkeletonBone vertexBone = new EQSkeletonBone();
                    vertexBone.BoneName = "v" + i;
                    vertexBone.MeshName = string.Empty;
                    vertexBone.AlternateMeshName = string.Empty;
                    vertexBone.ParticleCloudName = string.Empty;
                    BoneStructures.Add(vertexBone);
                    meshData.BoneIDs.Add(Convert.ToByte(curBoneID));
                    rootBone.Children.Add(curBoneID);
                    curBoneID++;
                }
                else
                    meshData.BoneIDs.Add(0);
            }
        }

        public bool LoadFromDisk(string fileFullPath)
        {
            Logger.WriteDebug(" - Reading EQ Skeleton Data from '" + fileFullPath + "'...");
            if (File.Exists(fileFullPath) == false)
            {
                Logger.WriteError("- Could not find EQ Skeleton file that should be at '" + fileFullPath + "'");
                return false;
            }

            // Load the core data
            string inputData = FileTool.ReadAllDataFromFile(fileFullPath);
            string[] inputRows = inputData.Split(Environment.NewLine);

            // Make sure there is the minimum number of rows
            if (inputRows.Length < 5)
            {
                Logger.WriteError("- Could not load EQ Skeleton file because there were less than 5 rows at '" + fileFullPath + "'");
                return false;
            }

            foreach (string inputRow in inputRows)
            {
                // Nothing for blank lines
                if (inputRow.Length == 0)
                    continue;

                // # = comment
                else if (inputRow.StartsWith("#"))
                    continue;

                // Get the blocks for this row
                string[] blocks = inputRow.Split(",");
                if (blocks.Length == 0)
                    continue;

                // Mesh list
                if (blocks[0] == "meshes")
                {
                    for (int i = 1; i < blocks.Length; i++)
                        MeshNames.Add(blocks[i]);                    
                    continue;
                }
                
                // Secondary mesh list
                if (blocks[0] == "secondary_meshes")
                {
                    for (int i = 1; i < blocks.Length; i++)
                        SecondaryMeshNames.Add(blocks[i]);
                    continue;
                }

                // This is a bone struct
                if (blocks.Length != 5)
                {
                    Logger.WriteError("EQ Skeleton Bone Struct data must be 5 components");
                    continue;
                }

                EQSkeletonBone boneStruct = new EQSkeletonBone();
                boneStruct.BoneName = blocks[0];
                string[] children = blocks[1].Split(";");
                foreach (string child in children)
                    if (child != string.Empty)
                        boneStruct.Children.Add(int.Parse(child));
                boneStruct.MeshName = blocks[2];
                boneStruct.AlternateMeshName = blocks[3];
                boneStruct.ParticleCloudName = blocks[4];
                BoneStructures.Add(boneStruct);
            }

            Logger.WriteDebug(" - Done reading EQ Skeleton Data from '" + fileFullPath + "'");
            return true;
        }
    }
}
