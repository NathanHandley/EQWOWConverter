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
using EQWOWConverter.Creatures;
using EQWOWConverter.EQFiles;

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

        public void LoadObjectDataFromDisk(string inputObjectName, string inputObjectFolder, CreatureModelTemplate? creatureModelTemplate, 
            string meshAndSkeletonNameOverrideForNonCreature = "")
        {
            if (Directory.Exists(inputObjectFolder) == false)
            {
                Logger.WriteError("- [" + inputObjectName + "]: Error - Could not find path at '" + inputObjectFolder + "'");
                return;
            }

            // Load skeleton, if possible
            string skeletonName = inputObjectName;
            if (meshAndSkeletonNameOverrideForNonCreature != "")
                skeletonName = meshAndSkeletonNameOverrideForNonCreature;
            string skeletonFileName = Path.Combine(inputObjectFolder, "Skeletons", skeletonName + ".txt");
            if (File.Exists(skeletonFileName))
                LoadSkeletonData(skeletonName, inputObjectFolder);

            if (creatureModelTemplate != null)
            {
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
                Dictionary<string, byte> meshNamesInDictionary = new Dictionary<string, byte>();
                foreach (string meshName in meshNames)
                    meshNamesInDictionary.Add(meshName, 0);
                LoadRenderMeshData(inputObjectName, meshNamesInDictionary, inputObjectFolder);

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
                            if (File.Exists(Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "characters", "Textures", newFaceTextureName + ".blp")))
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
                string animationSupplimentName = string.Empty;
                if (creatureModelTemplate != null)
                    animationSupplimentName = creatureModelTemplate.Race.Skeleton2Name;

                LoadAnimationData(inputObjectName, inputObjectFolder, animationSupplimentName);

                // Load collision
                LoadCollisionMeshData(inputObjectName, meshNamesInDictionary.Keys.ToList(), inputObjectFolder);
            }
            else
            {
                // Load render meshes....
                Dictionary<string, byte> meshBoneIndexByName = new Dictionary<string, byte>();
                for (byte i = 0; i < SkeletonData.BoneStructures.Count; i++)
                    if (SkeletonData.BoneStructures[i].MeshName != string.Empty)
                        meshBoneIndexByName.Add(SkeletonData.BoneStructures[i].MeshName, i);
                if (meshAndSkeletonNameOverrideForNonCreature != string.Empty)
                    meshBoneIndexByName.Add(meshAndSkeletonNameOverrideForNonCreature, 0);
                LoadRenderMeshData(inputObjectName, meshBoneIndexByName, inputObjectFolder);

                // Load Materials
                LoadMaterialDataFromDisk(MaterialListFileName, inputObjectFolder, 0);

                // Load the rest
                // If this object uses animated vertices, then it should have a skeleton and animation generated
                // TODO: Make this work with > 256 vertex frames
                int numOfFilledAVs = 0;
                foreach (var avFrames in MeshData.AnimatedVertexFramesByVertexIndex)
                    if (avFrames.VertexOffsetFrames.Count > 0)
                        numOfFilledAVs++;
                if (numOfFilledAVs >= 255)
                    Logger.WriteDetail("Object '" + inputObjectName + "' has animated vertices but has a frame count of " + MeshData.AnimatedVertexFramesByVertexIndex.Count + " so it will not be animated by vertices");
                if (MeshData.AnimatedVertexFramesByVertexIndex.Count > 0 && numOfFilledAVs < 255)
                {
                    ConvertAnimatedVerticesToSkeleton(inputObjectName);
                    GenerateAnimationFromAnimatedVertexSkeleton(inputObjectName, MeshData);
                }
                else
                    LoadAnimationData(inputObjectName, inputObjectFolder, string.Empty);

                // Load collision
                LoadCollisionMeshData(inputObjectName, meshBoneIndexByName.Keys.ToList(), inputObjectFolder);
            }
        }

        private void LoadRenderMeshData(string inputObjectName, Dictionary<string, byte> meshNamesByBoneIndex, string inputObjectFolder)
        {
            Logger.WriteDetail("- [" + inputObjectName + "]: Reading render mesh data...");
            foreach (var meshNameByBoneIndex in meshNamesByBoneIndex)
            {
                // Load this mesh
                string renderMeshFileName = Path.Combine(inputObjectFolder, "Meshes", meshNameByBoneIndex.Key + ".txt");
                EQMesh eqMeshData = new EQMesh();
                if (eqMeshData.LoadFromDisk(renderMeshFileName) == false)
                {
                    Logger.WriteError("- [" + inputObjectName + "]: ERROR - Could not find render mesh file that should be at '" + renderMeshFileName + "'");
                    return;
                }

                // Associate bone references
                if (eqMeshData.Bones.Count > 0)
                {
                    for (int i = 0; i < eqMeshData.Meshdata.Vertices.Count; i++)
                    {
                        byte curBoneID = 0;
                        foreach (EQMesh.BoneReference bone in eqMeshData.Bones)
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
                else
                {
                    for (int i = 0; i < eqMeshData.Meshdata.Vertices.Count; i++)
                        eqMeshData.Meshdata.BoneIDs.Add(meshNameByBoneIndex.Value);
                }

                // Save the mesh data into the larger mesh data
                MeshData.AddMeshData(eqMeshData.Meshdata);

                // Save material list
                if (MaterialListFileName == string.Empty)
                    MaterialListFileName = eqMeshData.MaterialListFileName;
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

        // TODO: Make this work with multiple meshes
        private void LoadCollisionMeshData(string inputObjectName, List<string> meshNames, string inputObjectFolder)
        {
            Logger.WriteDetail("- [" + inputObjectName + "]: Reading collision mesh data...");
            MeshData collisionMeshData = new MeshData();
            foreach (string meshName in meshNames)
            {
                string collisionMeshFileName = Path.Combine(inputObjectFolder, "Meshes", meshName + "_collision.txt");
                if (File.Exists(collisionMeshFileName) == false)
                {
                    Logger.WriteDetail("- [" + inputObjectName + "]: No collision mesh found, skipping.");
                    continue;
                }
                EQMesh meshData = new EQMesh();
                if (meshData.LoadFromDisk(collisionMeshFileName) == false)
                {
                    Logger.WriteError("- [" + inputObjectName + "]: Error loading collision mesh at '" + collisionMeshFileName + "'");
                    continue;
                }
                collisionMeshData.AddMeshData(meshData.Meshdata);
            }
            CollisionTriangleFaces = collisionMeshData.TriangleFaces;
            CollisionVertices = collisionMeshData.Vertices;
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

        private void ConvertAnimatedVerticesToSkeleton(string inputObjectName)
        {
            Logger.WriteDetail("- [" + inputObjectName + "]: Converting animated vertices to skeleton data...");
            SkeletonData = new EQSkeleton();
            SkeletonData.LoadFromAnimatedVerticesData(ref MeshData);
        }

        private void GenerateAnimationFromAnimatedVertexSkeleton(string inputObjectName, MeshData meshData)
        {
            Logger.WriteDetail("- [" + inputObjectName + "]: Generating an animation based on an animated vertex skeleton...");
            if (Animations.Count > 0)
            {
                Logger.WriteError("- [" + inputObjectName + "]: Failed to generate the animation since animations already existed.");
                return;
            }
            if (meshData.AnimatedVertexFramesByVertexIndex.Count == 0)
            {
                Logger.WriteError("- [" + inputObjectName + "]: Failed to generate the animation since there are no animated vertices.");
                return;
            }

            // Create the new animation
            int totalTime = 0;
            for (int i = 0; i < meshData.AnimatedVertexFramesByVertexIndex.Count; i++)
            {
                if (meshData.AnimatedVertexFramesByVertexIndex[i].VertexOffsetFrames.Count > 0)
                {
                    totalTime = meshData.AnimatedVerticesDelayInMS * meshData.AnimatedVertexFramesByVertexIndex[i].VertexOffsetFrames.Count;
                    break;
                }
            }
            int totalFrameCount = meshData.AnimatedVertexFramesByVertexIndex[0].VertexOffsetFrames.Count;
            Animation newAnimation = new Animation("o01", AnimationType.Stand, EQAnimationType.o01StandIdle, totalFrameCount, totalTime);

            // Build the root bone frame (always 1 at the start)
            Animation.BoneAnimationFrame rootBoneFrame = new Animation.BoneAnimationFrame();
            rootBoneFrame.BoneFullNameInPath = "root";
            rootBoneFrame.FrameIndex = 0;
            rootBoneFrame.XPosition = 0;
            rootBoneFrame.ZPosition = 0;
            rootBoneFrame.YPosition = 0;
            rootBoneFrame.XRotation = 0;
            rootBoneFrame.ZRotation = 0;
            rootBoneFrame.YRotation = 0;
            rootBoneFrame.WRotation = 1;
            rootBoneFrame.Scale = 1;
            rootBoneFrame.FramesMS = meshData.AnimatedVerticesDelayInMS;
            newAnimation.AnimationFrames.Add(rootBoneFrame);

            // Build the following frames
            for (int vertexIndex = 0; vertexIndex < meshData.AnimatedVertexFramesByVertexIndex.Count; vertexIndex++)
            {
                AnimatedVertexFrames vertexFrames = meshData.AnimatedVertexFramesByVertexIndex[vertexIndex];
                string boneFullName = "root/v" + vertexIndex;
                for (int frameIndex = 0; frameIndex < vertexFrames.VertexOffsetFrames.Count; frameIndex++)
                {
                    Vector3 curPosOffset = vertexFrames.VertexOffsetFrames[frameIndex];
                    Animation.BoneAnimationFrame curFrame = new Animation.BoneAnimationFrame();
                    curFrame.BoneFullNameInPath = boneFullName;
                    curFrame.FrameIndex = frameIndex;
                    curFrame.XPosition = curPosOffset.X * -1; // Rotate around Z axis
                    curFrame.ZPosition = curPosOffset.Z;
                    curFrame.YPosition = curPosOffset.Y * -1; // Rotate around Z axis
                    curFrame.XRotation = 0;
                    curFrame.ZRotation = 0;
                    curFrame.YRotation = 0;
                    curFrame.WRotation = 1;
                    curFrame.Scale = 1;
                    curFrame.FramesMS = meshData.AnimatedVerticesDelayInMS;
                    newAnimation.AnimationFrames.Add(curFrame);
                }
            }

            // Save it
            Animations.Add("pos", newAnimation);
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
