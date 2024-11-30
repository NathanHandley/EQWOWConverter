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
using EQWOWConverter.Creatures;
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

        public void LoadAllStaticObjectDataFromDisk(string inputObjectName, string inputObjectFolder, string inputMeshName)
        {
            if (Directory.Exists(inputObjectFolder) == false)
            {
                Logger.WriteError("- [" + inputObjectName + "]: Error - Could not find path at '" + inputObjectFolder + "'");
                return;
            }

            // Load the various blocks
            LoadRenderMeshData(inputObjectName, new List<string>() { inputMeshName }, inputObjectFolder);
            LoadMaterialDataFromDisk(MaterialListFileName, inputObjectFolder, 0);
            LoadCollisionMeshData(inputObjectName, inputObjectFolder);
        }

        public void LoadAllAnimateObjectDataFromDisk(string inputObjectName, string inputObjectFolder, CreatureModelTemplate creatureModelTemplate, 
            CreatureModelVariation creatureModelVariation)
        {
            if (Directory.Exists(inputObjectFolder) == false)
            {
                Logger.WriteError("- [" + inputObjectName + "]: Error - Could not find path at '" + inputObjectFolder + "'");
                return;
            }

            // Load skeleton and material data first
            LoadSkeletonData(inputObjectName, inputObjectFolder);

            // Determine what meshes to use for a given variation
            List<string> meshNames = new List<string>();
            if (creatureModelVariation.HelmTextureIndex == 0)
            {
                foreach (string meshName in SkeletonData.MeshNames)
                    meshNames.Add(meshName);
            }
            else
            {
                // Skip last primary if helm isn't zero
                // TODO: Check Fish (race 24) since it doesn't have a body mesh
                for (int i = 0; i < SkeletonData.MeshNames.Count - 1; i++)
                    meshNames.Add(SkeletonData.MeshNames[i]);
                if (meshNames.Count == 0 && SkeletonData.MeshNames.Count > 0)
                    meshNames.Add(SkeletonData.MeshNames[0]);                    

                // Enable the appropriate secondary
                int subMeshIndex = creatureModelVariation.HelmTextureIndex - 1;
                if (SkeletonData.SecondaryMeshNames.Count > subMeshIndex)
                    meshNames.Add(SkeletonData.SecondaryMeshNames[subMeshIndex]);
                else
                    Logger.WriteDetail("For object '" + inputObjectName + "', HelmTextureIndex was '" + creatureModelVariation.HelmTextureIndex + "' and secondarymeshcount was '" + SkeletonData.SecondaryMeshNames.Count + "'");
            }
            // This is a fix that seems to be required for Coldain's helm
            foreach (string meshName in SkeletonData.SecondaryMeshNames)
            {
                if (meshName == "cokhe01" && meshNames.Contains("cokhe01") == false)
                    meshNames.Add("cokhe01");
            }
            LoadRenderMeshData(inputObjectName, meshNames, inputObjectFolder);

            // Load the materials
            LoadMaterialDataFromDisk(MaterialListFileName, inputObjectFolder, creatureModelVariation.TextureIndex);

            // Load the rest
            LoadAnimationData(inputObjectName, inputObjectFolder, creatureModelTemplate.Race.GetAnimationSupplementNameForGender(creatureModelVariation.GenderType));
            LoadCollisionMeshData(inputObjectName, inputObjectFolder);
        }

        private void LoadRenderMeshData(string inputObjectName, List<string> inputMeshNames, string inputObjectFolder)
        {
            Logger.WriteDetail("- [" + inputObjectName + "]: Reading render mesh data...");
            foreach(string meshName in inputMeshNames)
            {
                string renderMeshFileName = Path.Combine(inputObjectFolder, "Meshes", meshName + ".txt");
                EQMesh eqMeshData = new EQMesh();
                if (eqMeshData.LoadFromDisk(renderMeshFileName) == false)
                {
                    Logger.WriteError("- [" + inputObjectName + "]: ERROR - Could not find render mesh file that should be at '" + renderMeshFileName + "'");
                    return;
                }

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

                // Save the mesh data and material list
                MeshData.AddMeshData(eqMeshData.Meshdata);

                if (MaterialListFileName == string.Empty)
                    MaterialListFileName = eqMeshData.MaterialListFileName;
                else if (MaterialListFileName != eqMeshData.MaterialListFileName)
                {
                    Logger.WriteError("- [" + inputObjectName + "]: ERROR - Mismatch material list file name provided changing '" + MaterialListFileName + "' to '" + eqMeshData.MaterialListFileName + "'");
                    MaterialListFileName = eqMeshData.MaterialListFileName;
                }
            }
        }

        private void LoadMaterialDataFromDisk(string inputObjectName, string inputObjectFolder, int materialIndex)
        {
            Logger.WriteDetail("- [" + inputObjectName + "]: Reading materials...");
            string materialListFileName = Path.Combine(inputObjectFolder, "MaterialLists", inputObjectName + ".txt");
            EQMaterialList materialListData = new EQMaterialList();
            if (materialListData.LoadFromDisk(materialListFileName) == false)
                Logger.WriteError("- [" + inputObjectName + "]: No material data found.");
            else
            {
                if (materialIndex >= materialListData.MaterialsByTextureVariation.Count)
                {
                    if (materialListData.MaterialsByTextureVariation.Count > 0)
                    {
                        Materials = materialListData.MaterialsByTextureVariation[0];
                        Logger.WriteDetail("- [" + inputObjectName + "]: materialIndex of value '" + materialIndex + "' exceeded count of MaterialsByTextureVariation of value '" + materialListData.MaterialsByTextureVariation.Count + "', so fell back to 0.");
                    }
                    else
                        Logger.WriteError("- [" + inputObjectName + "]: materialIndex of value '" + materialIndex + "' exceeded count of MaterialsByTextureVariation of value '" + materialListData.MaterialsByTextureVariation.Count + "'.");
                }
                else
                {
                    Materials = materialListData.MaterialsByTextureVariation[materialIndex];
                }
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

        private void LoadAnimationData(string inputObjectName, string inputObjectFolder, string animationSupplementalName)
        {
            Logger.WriteDetail("- [" + inputObjectName + "]: Reading animation data...");

            // Animations are split by animation name
            Animations.Clear();
            string animationFolder = Path.Combine(inputObjectFolder, "Animations");
            DirectoryInfo animationDirectoryInfo = new DirectoryInfo(animationFolder);
            FileInfo[] animationFileInfos = animationDirectoryInfo.GetFiles(inputObjectName + "_*.txt");
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
            if (animationSupplementalName.Length > 0)
            {
                animationFileInfos = animationDirectoryInfo.GetFiles(animationSupplementalName + "_*.txt");
                foreach (FileInfo animationFileInfo in animationFileInfos)
                {
                    string animationFileName = Path.GetFileNameWithoutExtension(animationFileInfo.FullName);
                    string animationName = animationFileName.Split("_")[1];

                    // Skip any already loaded
                    if (Animations.ContainsKey(animationName))
                        continue;

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
}
