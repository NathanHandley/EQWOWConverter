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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.EQFiles
{
    internal class EQSkeleton
    {
        public class EQSkeletonBone
        {
            public string BoneName = string.Empty;
            public List<int> Children = new List<int>();
            public string MeshName = string.Empty;
            public string AlternateMeshName = string.Empty;
            public string ParticleCloudName = string.Empty;
        }

        public List<string> MeshNames = new List<string>();
        public List<string> SecondaryMeshNames = new List<string>();
        public List<EQSkeletonBone> BoneStructures = new List<EQSkeletonBone>();

        public bool LoadFromDisk(string fileFullPath)
        {
            Logger.WriteDetail(" - Reading EQ Skeleton Data from '" + fileFullPath + "'...");
            if (File.Exists(fileFullPath) == false)
            {
                Logger.WriteError("- Could not find EQ Skeleton file that should be at '" + fileFullPath + "'");
                return false;
            }

            // Load the core data
            string inputData = File.ReadAllText(fileFullPath);
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

            Logger.WriteDetail(" - Done reading EQ Skeleton Data from '" + fileFullPath + "'");
            return true;
        }
    }
}
