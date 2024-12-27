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
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EQWOWConverter.EQFiles.EQMesh;

namespace EQWOWConverter.ObjectModels
{
    internal class ObjectModelEQData
    {
        public MeshData MeshData = new MeshData();
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

        public void LoadAllAnimateObjectDataFromDisk(string inputObjectName, string inputObjectFolder, CreatureModelTemplate creatureModelTemplate)
        {
            if (Directory.Exists(inputObjectFolder) == false)
            {
                Logger.WriteError("- [" + inputObjectName + "]: Error - Could not find path at '" + inputObjectFolder + "'");
                return;
            }

            // Load skeleton
            LoadSkeletonData(inputObjectName, inputObjectFolder);

            // Delete body mesh data for Dervish
            int raceID = creatureModelTemplate.Race.ID;
            if (raceID == 100)
                SkeletonData.MeshNames.Remove("der");

            // Determine what mesh names to use for a given variation
            List<string> meshNames = new List<string>();
            if (creatureModelTemplate.HelmTextureIndex == 0)
            {
                foreach (string meshName in SkeletonData.MeshNames)
                    meshNames.Add(meshName);
            }
            else
            {
                // Collect all of the body and helm textures
                List<string> bodyTextureNames = new List<string>();
                List<string> helmTextureNames = new List<string>();
                foreach (string meshName in SkeletonData.MeshNames)
                {
                    if (meshName.Contains("he0"))
                        helmTextureNames.Add(meshName);
                    else 
                        bodyTextureNames.Add(meshName);
                }
                foreach (string meshName in SkeletonData.SecondaryMeshNames)
                {
                    if (meshName.Contains("he0"))
                        helmTextureNames.Add(meshName);
                    else
                        bodyTextureNames.Add(meshName);
                }

                // Handle out-of-bounds
                if (creatureModelTemplate.HelmTextureIndex >= helmTextureNames.Count)
                {
                    foreach (string meshName in SkeletonData.MeshNames)
                        meshNames.Add(meshName);
                }
                else
                {
                    if (bodyTextureNames.Count > 0)
                        meshNames.Add(bodyTextureNames[0]);
                    meshNames.Add(helmTextureNames[creatureModelTemplate.HelmTextureIndex]);
                }
            }

            // Fix coldain's helm
            foreach (string meshName in SkeletonData.SecondaryMeshNames)
            {
                if (meshName == "cokhe01" && meshNames.Contains("cokhe01") == false)
                    meshNames.Add("cokhe01");
            }

            // Special mesh logic for 'eye'
            if (meshNames.Count == 0 && inputObjectName.ToUpper() == "EYE")
                meshNames.Add("eye");

            // For robe-capable races, swap the chest geometry
            if (creatureModelTemplate.TextureIndex >= 10 && (raceID == 1 || raceID == 3 || raceID == 5 || raceID == 6 || raceID == 12 || raceID == 128))
            {
                meshNames.Remove(inputObjectName.ToLower());
                meshNames.Add(string.Concat(inputObjectName.ToLower(), "01"));
            }

            // Load the render meshes
            LoadRenderMeshData(inputObjectName, meshNames, inputObjectFolder);

            // Load the materials, with special logic for invisible man
            if (creatureModelTemplate.Race.ID == 127)
            {
                EQMaterialList materialListData = new EQMaterialList();
                materialListData.LoadForInvisibleMan();
                Materials = materialListData.MaterialsByTextureVariation[0];
            }
            else
                LoadMaterialDataFromDisk(MaterialListFileName, inputObjectFolder, creatureModelTemplate.TextureIndex);

            // For multi-face races, swap the faces
            if (creatureModelTemplate.FaceIndex > 0 && (raceID < 13 || raceID == 70 || raceID == 128 || raceID == 130))
            {
                string faceTextureStart = string.Concat(inputObjectName, "he00");
                faceTextureStart = faceTextureStart.ToLower();
                foreach (Material material in Materials)
                {
                    if (material.TextureNames.Count > 0 && material.TextureNames[0].ToLower().StartsWith(faceTextureStart))
                    {
                        char curTextureLastID = material.TextureNames[0].Last();
                        string newFaceTextureName = string.Concat(faceTextureStart, creatureModelTemplate.FaceIndex.ToString(), curTextureLastID);

                        // Only switch if that texture exists
                        if (File.Exists(Path.Combine(Configuration.CONFIG_PATH_EQEXPORTSCONDITIONED_FOLDER, "characters", "Textures", newFaceTextureName + ".blp")))
                            material.TextureNames[0] = newFaceTextureName;
                    }
                }
            }

            // For robe-capable races, swap the textures
            if (creatureModelTemplate.TextureIndex >= 10 && (raceID == 1 || raceID == 3 || raceID == 5 || raceID == 6 || raceID == 12 || raceID == 128))
            {
                // Calculate what body robe graphics to use
                int robeIndex = creatureModelTemplate.TextureIndex - 6;

                // Body graphics
                if (robeIndex >= 0 && robeIndex <= 10)
                {
                    string replaceFromText = "clk04";
                    string replaceToText;
                    if (robeIndex == 10)
                        replaceToText = "clk10";
                    else
                        replaceToText = string.Concat("clk0", robeIndex.ToString());

                    foreach (Material material in Materials)
                        if (material.TextureNames.Count > 0 && material.TextureNames[0].StartsWith(replaceFromText))
                            material.TextureNames[0] = material.TextureNames[0].Replace(replaceFromText, replaceToText);
                }

                // Head graphics (erudite only)
                if (raceID == 3 && (robeIndex >= 0 && robeIndex <= 10))
                {
                    string replaceFromText = string.Concat("clk", inputObjectName.ToLower(), "06");
                    string replaceToText;
                    if (robeIndex == 10)
                        replaceToText = "clk1006";
                    else
                        replaceToText = string.Concat("clk0", robeIndex.ToString(), "06");

                    foreach (Material material in Materials)
                        if (material.TextureNames.Count > 0 && material.TextureNames[0].StartsWith(replaceFromText))
                            material.TextureNames[0] = material.TextureNames[0].Replace(replaceFromText, replaceToText);
                }
            }

            // Load the rest
            LoadAnimationData(inputObjectName, inputObjectFolder, creatureModelTemplate.Race.GetAnimationSupplementNameForGender(creatureModelTemplate.GenderType));
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
