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
using EQWOWConverter.EQFiles;
using EQWOWConverter.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EQWOWConverter.EQFiles.EQMesh;

namespace EQWOWConverter.ObjectModels
{
    internal class ObjectModelEQData
    {
        public MeshData MeshData = new MeshData();
        public AnimatedVertices AnimatedVertices = new AnimatedVertices(); // TODO: May not be in use, consider deleting
        public List<Material> Materials = new List<Material>();
        public List<Vector3> CollisionVertices = new List<Vector3>();
        public Dictionary<string, Animation> Animations = new Dictionary<string, Animation>();
        public List<TriangleFace> CollisionTriangleFaces = new List<TriangleFace>();
        private string MaterialListFileName = string.Empty;
        public EQSkeleton SkeletonData = new EQSkeleton();

        public void LoadDataFromDisk(string inputObjectName, string inputObjectFolder, bool isSkeletal)
        {
            if (Directory.Exists(inputObjectFolder) == false)
            {
                Logger.WriteError("- [" + inputObjectName + "]: Error - Could not find path at '" + inputObjectFolder + "'");
                return;
            }

            // Load the various blocks
            if (isSkeletal == true)
            {
                LoadSkeletonData(inputObjectName, inputObjectFolder);
                LoadRenderMeshData(inputObjectName, inputObjectFolder); // TODO: Need to be able to load multiple meshes
                LoadMaterialDataFromDisk(MaterialListFileName, inputObjectFolder);
                LoadAnimationData(inputObjectName, inputObjectFolder);
                LoadCollisionMeshData(inputObjectName, inputObjectFolder);
            }
            else
            {
                LoadRenderMeshData(inputObjectName, inputObjectFolder);
                LoadMaterialDataFromDisk(MaterialListFileName, inputObjectFolder);
                LoadCollisionMeshData(inputObjectName, inputObjectFolder);
            }            
        }

        private void LoadRenderMeshData(string inputObjectName, string inputObjectFolder)
        {
            Logger.WriteDetail("- [" + inputObjectName + "]: Reading render mesh data...");
            string renderMeshFileName = Path.Combine(inputObjectFolder, "Meshes", inputObjectName + ".txt");
            EQMesh eqMeshData = new EQMesh();
            if (eqMeshData.LoadFromDisk(renderMeshFileName) == false)
            {
                Logger.WriteError("- [" + inputObjectName + "]: ERROR - Could not find render mesh file that should be at '" + renderMeshFileName + "'");
                return;
            }
            MeshData = eqMeshData.Meshdata;
            MaterialListFileName = eqMeshData.MaterialListFileName;

            // Bone references
            if (eqMeshData.Bones.Count > 0)
            {
                for (int i = 0; i < eqMeshData.Meshdata.Vertices.Count; i++)
                {
                    byte curBoneID = 0;
                    foreach (BoneReference bone in eqMeshData.Bones)
                    {
                        if (i >= bone.VertStart && (i < (bone.VertStart + bone.VertCount)))
                        {
                            curBoneID = bone.KeyBoneID;
                            break;
                        }
                    }
                    eqMeshData.Meshdata.BoneIDs.Add(curBoneID);
                }
            }
        }

        private void LoadMaterialDataFromDisk(string inputObjectName, string inputObjectFolder)
        {
            Logger.WriteDetail("- [" + inputObjectName + "]: Reading materials...");
            string materialListFileName = Path.Combine(inputObjectFolder, "MaterialLists", inputObjectName + ".txt");
            EQMaterialList materialListData = new EQMaterialList();
            if (materialListData.LoadFromDisk(materialListFileName) == false)
                Logger.WriteDetail("- [" + inputObjectName + "]: No material data found.");
            else
            {
                Materials = materialListData.Materials;
            }
        }

        private void LoadCollisionMeshData(string inputObjectName, string inputObjectFolder)
        {
            Logger.WriteDetail("- [" + inputObjectName + "]: Reading collision mesh data...");
            string collisionMeshFileName = Path.Combine(inputObjectFolder, "Meshes", inputObjectName + "_collision.txt");
            if (File.Exists(collisionMeshFileName) == false)
            {
                Logger.WriteDetail("- [" + inputObjectName + "]: No collision mesh found, skipping for zone.");
                return;
            }
            EQMesh meshData = new EQMesh();
            if (meshData.LoadFromDisk(collisionMeshFileName) == false)
            {
                Logger.WriteError("- [" + inputObjectName + "]: Error loading collision mesh at '" + collisionMeshFileName + "'");
                return;
            }
            CollisionTriangleFaces = meshData.Meshdata.TriangleFaces;
            CollisionVertices = meshData.Meshdata.Vertices;
        }

        private void LoadSkeletonData(string inputObjectName, string inputObjectFolder)
        {
            Logger.WriteDetail("- [" + inputObjectName + "]: Reading skeleton data...");
            string skeletonFileName = Path.Combine(inputObjectFolder, "Skeletons", inputObjectName + ".txt");
            SkeletonData = new EQSkeleton();
            if (SkeletonData.LoadFromDisk(skeletonFileName) == false)
            {
                Logger.WriteError("- [" + inputObjectName + "]: Issue loading skeleton data that should be at '" + skeletonFileName + "'");
                return;
            }
        }

        private void LoadAnimationData(string inputObjectName, string inputObjectFolder)
        {
            Logger.WriteDetail("- [" + inputObjectName + "]: Reading animation data...");

            // Animations are split by animation name
            Animations.Clear();
            string animationFolder = Path.Combine(inputObjectFolder, "Animations");
            DirectoryInfo animationDirectoryInfo = new DirectoryInfo(animationFolder);
            FileInfo[] animationFileInfos = animationDirectoryInfo.GetFiles(inputObjectName + "*.txt");
            foreach(FileInfo animationFileInfo in animationFileInfos)
            {
                string animationFileName = Path.GetFileNameWithoutExtension(animationFileInfo.FullName);
                string animationName = animationFileName.Split("_")[1];
                EQAnimation curEQAnimation = new EQAnimation();
                if (curEQAnimation.LoadFromDisk(animationFileInfo.FullName))
                {                    
                    Animations.Add(animationName, curEQAnimation.Animation);
                }
                else
                {
                    Logger.WriteError("- [" + inputObjectName + "]: Could not load animation data that should be at '" + animationFileName + "'");
                }
            }
        }
    }
}
