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
using EQWOWConverter.ObjectModels.Properties;
using EQWOWConverter.Zones;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EQWOWConverter.ObjectModels
{
    internal class ObjectModel
    {
        public string Name = string.Empty;
        public ObjectModelType ModelType;
        public ObjectModelEQData EQObjectModelData = new ObjectModelEQData();
        public ObjectModelProperties Properties = new ObjectModelProperties();
        public List<UInt32> GlobalLoopSequenceLimits = new List<UInt32>();
        public List<ObjectModelAnimation> ModelAnimations = new List<ObjectModelAnimation>();
        public List<Int16> AnimationLookups = new List<Int16>();
        public List<ObjectModelVertex> ModelVertices = new List<ObjectModelVertex>();
        public List<ObjectModelBone> ModelBones = new List<ObjectModelBone>();
        public List<ObjectModelTextureAnimation> ModelTextureAnimations = new List<ObjectModelTextureAnimation>();
        public List<Int16> ModelBoneKeyLookups = new List<Int16>();
        public SortedDictionary<int, List<Int16>> BoneLookupsByMaterialIndex = new SortedDictionary<int, List<Int16>>();
        public List<ObjectModelMaterial> ModelMaterials = new List<ObjectModelMaterial>();
        public List<ObjectModelTexture> ModelTextures = new List<ObjectModelTexture>();
        public List<Int16> ModelTextureLookups = new List<Int16>();
        public List<Int16> ModelTextureMappingLookups = new List<Int16>();
        public List<Int16> ModelReplaceableTextureLookups = new List<Int16>();
        public List<UInt16> ModelTextureTransparencyLookups = new List<UInt16>();
        public SortedDictionary<int, ObjectModelTrackSequences<Fixed16>> ModelTextureTransparencySequenceSetByMaterialIndex = new SortedDictionary<int, ObjectModelTrackSequences<Fixed16>>();
        public List<Int16> ModelTextureAnimationLookup = new List<Int16>();
        public List<UInt16> ModelSecondTextureMaterialOverrides = new List<UInt16>();
        public List<TriangleFace> ModelTriangles = new List<TriangleFace>();
        public BoundingBox BoundingBox = new BoundingBox();
        public float BoundingSphereRadius = 0f;
        public Sound? SoundIdleLoop = null;

        public List<Vector3> CollisionPositions = new List<Vector3>();
        public List<Vector3> CollisionFaceNormals = new List<Vector3>();
        public List<TriangleFace> CollisionTriangles = new List<TriangleFace>();
        public BoundingBox CollisionBoundingBox = new BoundingBox();
        public float CollisionSphereRaidus = 0f;

        public ObjectModel(string name, ObjectModelProperties objectProperties, ObjectModelType modelType)
        {
            Name = name;
            Properties = objectProperties;
            ModelType = modelType;
        }

        public void LoadEQObjectData(string inputRootFolder)
        {
            // Clear any old data
            EQObjectModelData = new ObjectModelEQData();

            // Load based on type
            if (ModelType == ObjectModelType.Skeletal)
                EQObjectModelData.LoadDataFromDisk(Name, inputRootFolder, true);
            else
                EQObjectModelData.LoadDataFromDisk(Name, inputRootFolder, false);
        }

        public void PopulateObjectModelFromEQObjectModelData(int textureVariationIndex, float skeletonLiftHeight = 0, float modelScale = 1)
        {
            if (EQObjectModelData.CollisionVertices.Count == 0)
                Load(Name, EQObjectModelData.MaterialsByTextureVariation[textureVariationIndex], EQObjectModelData.MeshData, new List<Vector3>(), new List<TriangleFace>(), skeletonLiftHeight, modelScale);
            else
                Load(Name, EQObjectModelData.MaterialsByTextureVariation[textureVariationIndex], EQObjectModelData.MeshData, EQObjectModelData.CollisionVertices, EQObjectModelData.CollisionTriangleFaces, skeletonLiftHeight, modelScale);
        }

        // TODO: Vertex Colors
        public void Load(string name, List<Material> initialMaterials, MeshData meshData, List<Vector3> collisionVertices,
            List<TriangleFace> collisionTriangleFaces, float skeletonLiftHeight = 0, float modelScale = 1)
        {
            // Save Name
            Name = name;

            // Control for bad objects
            if (meshData.Vertices.Count != meshData.TextureCoordinates.Count && meshData.Vertices.Count != meshData.Normals.Count)
            {
                Logger.WriteError("Failed to load wowobject named '" + Name + "' since vertex count doesn't match texture coordinate count or normal count");
                return;
            }

            // Sort the geometry
            meshData.SortDataByMaterial();

            // Perform EQ->WoW translations if this is coming from a raw EQ object
            if (ModelType == ObjectModelType.Skeletal || ModelType == ObjectModelType.SimpleDoodad)
            {
                // Regular
                meshData.ApplyEQToWoWGeometryTranslationsAndWorldScale(ModelType != ObjectModelType.Skeletal, modelScale);

                // If there is any collision data, also translate that too
                if (collisionVertices.Count > 0)
                {
                    MeshData collisionMeshData = new MeshData();
                    collisionMeshData.TriangleFaces = collisionTriangleFaces;
                    collisionMeshData.Vertices = collisionVertices;
                    collisionMeshData.ApplyEQToWoWGeometryTranslationsAndWorldScale(ModelType != ObjectModelType.Skeletal, modelScale);
                    collisionTriangleFaces = collisionMeshData.TriangleFaces;
                    collisionVertices = collisionMeshData.Vertices;
                }
            }

            // Collision data
            ProcessCollisionData(meshData, initialMaterials, collisionVertices, collisionTriangleFaces);

            // Process materials
            ProcessMaterials(initialMaterials, ref meshData);

            // Create model vertices
            GenerateModelVertices(meshData, collisionVertices, collisionTriangleFaces);

            // Correct any texture coordinates
            CorrectTextureCoordinates();

            // Process the rest
            CalculateBoundingBoxesAndRadii();

            // No texture replacement lookup (yet)
            ModelReplaceableTextureLookups.Add(-1);

            // Build the bones and animation structures
            ProcessBonesAndAnimation(ModelMaterials, ModelVertices, ModelTriangles, skeletonLiftHeight);
        }

        private void ProcessBonesAndAnimation(List<ObjectModelMaterial> modelMaterials, List<ObjectModelVertex> modelVertices, List<TriangleFace> modelTriangles, float skeletonLiftHeight)
        {
            // Static types
            if (ModelType != ObjectModelType.Skeletal || EQObjectModelData.Animations.Count == 0)
            {
                ModelBoneKeyLookups.Add(-1);

                if (ModelType == ObjectModelType.Skeletal)
                    Logger.WriteError("Object named '" + Name + "' is skeletal but has no animations, so loading as static");

                // Create a base bone
                AnimationLookups.Add(0); // Maps animations to the IDs in AnimationData.dbc - None for static
                ModelBones.Add(new ObjectModelBone());

                // Make one animation (standing)
                ModelAnimations.Add(new ObjectModelAnimation());
                ModelAnimations[0].BoundingBox = new BoundingBox(BoundingBox);
                ModelAnimations[0].BoundingRadius = BoundingSphereRadius;
            }

            // Skeletal
            else
            {
                // Grab the 'pos' animation, which should be the base pose
                Animation pickedAnimation = new Animation("",AnimationType.Stand, EQAnimationType.Unknown, 0, 0);
                foreach (var animation in EQObjectModelData.Animations)
                    if (animation.Key == "pos")
                        pickedAnimation = animation.Value;
                if (pickedAnimation.AnimationFrames.Count == 0)
                {
                    Logger.WriteError("Could not pull skeleton information for object '" + Name + "' as there was no 'pos' animation");
                    ModelBones.Add(new ObjectModelBone());
                    ModelAnimations.Add(new ObjectModelAnimation());
                    ModelAnimations[0].BoundingBox = new BoundingBox(BoundingBox);
                    ModelAnimations[0].BoundingRadius = BoundingSphereRadius;
                    return;
                }

                // Block out 27 key bones with blank
                for (int i = 0; i < 27; i++)
                    ModelBoneKeyLookups.Add(-1);

                // Create bones by reading this pos animation
                for (int i = 0; i < pickedAnimation.AnimationFrames.Count; i++)
                {
                    ObjectModelBone curBone = new ObjectModelBone();
                    curBone.BoneNameEQ = pickedAnimation.AnimationFrames[i].GetBoneName();
                    curBone.ParentBoneNameEQ = pickedAnimation.AnimationFrames[i].GetParentBoneName();
                    for (Int16 j = 0; j < ModelBones.Count; j++)
                        if (ModelBones[j].BoneNameEQ == curBone.ParentBoneNameEQ)
                            curBone.ParentBone = j;
                    if (curBone.BoneNameEQ == "root")
                    {
                        curBone.ScaleTrack.InterpolationType = ObjectModelAnimationInterpolationType.None;
                        curBone.RotationTrack.InterpolationType = ObjectModelAnimationInterpolationType.None;
                        curBone.TranslationTrack.InterpolationType = ObjectModelAnimationInterpolationType.None;
                    }
                    else
                    {
                        curBone.ScaleTrack.InterpolationType = ObjectModelAnimationInterpolationType.Linear;
                        curBone.RotationTrack.InterpolationType = ObjectModelAnimationInterpolationType.Linear;
                        curBone.TranslationTrack.InterpolationType = ObjectModelAnimationInterpolationType.Linear;
                    }
                    curBone.KeyBoneID = Convert.ToInt32(GetKeyBoneTypeForEQBone(curBone.BoneNameEQ));
                    if (curBone.KeyBoneID != -1)
                        ModelBoneKeyLookups[curBone.KeyBoneID] = Convert.ToInt16(ModelBones.Count);
                    ModelBones.Add(curBone);
                }

                // Create animations
                int curSequenceID = -1;
                foreach (var animation in EQObjectModelData.Animations)
                {
                    Logger.WriteDetail("Adding animation of eq type '" + animation.Key + "' and wow type '" + animation.Value.AnimationType.ToString() + "' to object '" + Name + "'");

                    // Create the base animation object
                    ObjectModelAnimation newAnimation = new ObjectModelAnimation();
                    newAnimation.DurationInMS = Convert.ToUInt32(animation.Value.TotalTimeInMS);
                    newAnimation.AnimationType = animation.Value.AnimationType;
                    newAnimation.EQAnimationType = animation.Value.EQAnimationType;
                    newAnimation.BoundingBox = new BoundingBox(BoundingBox);
                    newAnimation.BoundingRadius = BoundingSphereRadius;
                    newAnimation.AliasNext = Convert.ToUInt16(ModelAnimations.Count); // The next animation is itself, so it's a loop
                    ModelAnimations.Add(newAnimation);

                    // If this has more than one frame, reduce the duration by 1 frame to allow for proper looping
                    if (animation.Value.FrameCount > 1)
                        newAnimation.DurationInMS -= Convert.ToUInt32(animation.Value.AnimationFrames[0].FramesMS);

                    // Save the lookup
                    SetAnimationLookup(newAnimation.AnimationType, Convert.ToInt16(ModelAnimations.Count - 1));

                    // Create an animation track sequence for each bone
                    curSequenceID++;
                    foreach(ObjectModelBone bone in ModelBones)
                    {
                        bone.ScaleTrack.AddSequence();
                        bone.RotationTrack.AddSequence();
                        bone.TranslationTrack.AddSequence();
                    }

                    // Add the animation-bone transformations to the bone objects for each frame
                    Dictionary<string, int> curTimestampsByBoneName = new Dictionary<string, int>();
                    foreach(Animation.BoneAnimationFrame animationFrame in animation.Value.AnimationFrames)
                    {
                        ObjectModelBone curBone = GetBoneWithName(animationFrame.GetBoneName());

                        // Root has just one frame
                        if (animationFrame.GetBoneName() == "root")
                        {
                            curBone.ScaleTrack.AddValueToSequence(curSequenceID, 0, new Vector3(1, 1, 1));
                            curBone.RotationTrack.AddValueToSequence(curSequenceID, 0, new QuaternionShort());
                            curBone.TranslationTrack.AddValueToSequence(curSequenceID, 0, new Vector3(0, 0, 0));
                        }
                        // Regular bones append the animation frame
                        else
                        {
                            // Format and transform the animation frame values from EQ to WoW
                            Vector3 frameTranslation = new Vector3(animationFrame.XPosition * Configuration.CONFIG_GENERATE_WORLD_SCALE,
                                                                   animationFrame.YPosition * Configuration.CONFIG_GENERATE_WORLD_SCALE,
                                                                   animationFrame.ZPosition * Configuration.CONFIG_GENERATE_WORLD_SCALE);
                            Vector3 frameScale = new Vector3(animationFrame.Scale, animationFrame.Scale, animationFrame.Scale);
                            QuaternionShort frameRotation;
                            frameRotation = new QuaternionShort(-animationFrame.XRotation,
                                                                -animationFrame.YRotation,
                                                                -animationFrame.ZRotation,
                                                                animationFrame.WRotation);
                            frameRotation.RecalculateToShortest();

                            // For bones that connect to root, add the height mod
                            if (curBone.ParentBoneNameEQ == "root")
                                frameTranslation.Z += skeletonLiftHeight;

                            // Calculate the frame start time
                            UInt32 curTimestamp = 0;
                            if (curTimestampsByBoneName.ContainsKey(animationFrame.GetBoneName()) == false)
                                curTimestampsByBoneName.Add(animationFrame.GetBoneName(), 0);
                            else
                                curTimestamp = Convert.ToUInt32(curTimestampsByBoneName[animationFrame.GetBoneName()]);

                            // Add the values
                            curBone.ScaleTrack.AddValueToSequence(curSequenceID, curTimestamp, frameScale);
                            curBone.RotationTrack.AddValueToSequence(curSequenceID, curTimestamp, frameRotation);
                            curBone.TranslationTrack.AddValueToSequence(curSequenceID, curTimestamp, frameTranslation);

                            // Increment the frame start for next
                            curTimestampsByBoneName[animationFrame.GetBoneName()] += animationFrame.FramesMS;
                        }
                    }
                }

                // Create bone lookups on a per submesh basis (which are grouped by material)
                for (int curMaterialIndex = 0; curMaterialIndex < modelMaterials.Count; curMaterialIndex++)
                {
                    List<Int16> curMaterialBoneIndices = new List<Int16>();
                    foreach(TriangleFace modelTriangle in modelTriangles)
                    {
                        if (modelTriangle.MaterialIndex == curMaterialIndex)
                        {
                            if (curMaterialBoneIndices.Contains(modelVertices[modelTriangle.V1].BoneIndicesTrue[0]) == false)
                                curMaterialBoneIndices.Add(modelVertices[modelTriangle.V1].BoneIndicesTrue[0]);
                            if (curMaterialBoneIndices.Contains(modelVertices[modelTriangle.V1].BoneIndicesTrue[1]) == false)
                                curMaterialBoneIndices.Add(modelVertices[modelTriangle.V1].BoneIndicesTrue[1]);
                            if (curMaterialBoneIndices.Contains(modelVertices[modelTriangle.V1].BoneIndicesTrue[2]) == false)
                                curMaterialBoneIndices.Add(modelVertices[modelTriangle.V1].BoneIndicesTrue[2]);
                        }
                    }
                    if (curMaterialBoneIndices.Count > 0)
                    {
                        BoneLookupsByMaterialIndex.Add(curMaterialIndex, curMaterialBoneIndices);
                    }
                }

                // Update bone indices to reflect the lookup values
                foreach(TriangleFace modelTriangle in modelTriangles)
                {
                    if (BoneLookupsByMaterialIndex.ContainsKey(modelTriangle.MaterialIndex) == false)
                    {
                        Logger.WriteError("Object '" + Name + "' could not find BoneLookup by material index of '" + modelTriangle.MaterialIndex + "'");
                        continue;
                    }

                    List<Int16> curBoneLookups = BoneLookupsByMaterialIndex[modelTriangle.MaterialIndex];
                    for (int i = 0; i < curBoneLookups.Count; i++)
                    {
                        if (ModelVertices[modelTriangle.V1].BoneIndicesTrue[0] == curBoneLookups[i])
                            ModelVertices[modelTriangle.V1].BoneIndicesLookup[0] = Convert.ToByte(i);
                        if (ModelVertices[modelTriangle.V2].BoneIndicesTrue[0] == curBoneLookups[i])
                            ModelVertices[modelTriangle.V2].BoneIndicesLookup[0] = Convert.ToByte(i);
                        if (ModelVertices[modelTriangle.V3].BoneIndicesTrue[0] == curBoneLookups[i])
                            ModelVertices[modelTriangle.V3].BoneIndicesLookup[0] = Convert.ToByte(i);
                    }                    
                }
            }

            // TODO: Put this somewhere else / change this
            if (GlobalLoopSequenceLimits.Count == 0)
                GlobalLoopSequenceLimits.Add(0);
        }

        private KeyBoneType GetKeyBoneTypeForEQBone(string eqBoneName)
        {
            // TODO: Need to add more key bone types
            switch (eqBoneName)
            {
                case "root": return KeyBoneType.Root;
                case "he": return KeyBoneType.Head;
                default: return KeyBoneType.None;
            }
        }

        private void SetAnimationLookup(AnimationType animationType, Int16 animationID)
        {
            // Expand the list if needed
            UInt16 curAnimationtypeValue = Convert.ToUInt16(animationType);
            if (curAnimationtypeValue >= AnimationLookups.Count)
                for (int i = AnimationLookups.Count; i <= curAnimationtypeValue; i++)
                    AnimationLookups.Add(-1);

            // Set the reference in the list
            AnimationLookups[curAnimationtypeValue] = animationID;
        }
        
        private ObjectModelBone GetBoneWithName(string name)
        {
            foreach (ObjectModelBone bone in ModelBones)
                if (bone.BoneNameEQ == name)
                    return bone;

            Logger.WriteError("No bone named '" + name + "' for object '" + Name + "'");
            return new ObjectModelBone();
        }

        private void GenerateModelVertices(MeshData meshData, List<Vector3> collisionVertices, List<TriangleFace> collisionTriangleFaces)
        {
            if (Configuration.CONFIG_OBJECT_STATIC_RENDER_AS_COLLISION == true && (ModelType == ObjectModelType.Skeletal || ModelType == ObjectModelType.SimpleDoodad))
            {
                foreach (TriangleFace face in collisionTriangleFaces)
                    ModelTriangles.Add(new TriangleFace(face));
                for (int i = 0; i < collisionVertices.Count; i++)
                {
                    ObjectModelVertex newModelVertex = new ObjectModelVertex();
                    newModelVertex.Position = new Vector3(collisionVertices[i]);
                    newModelVertex.Normal = new Vector3(0, 0, 0);
                    newModelVertex.Texture1TextureCoordinates = new TextureCoordinates(0f, 1f);
                    newModelVertex.BoneIndicesTrue[0] = 0;
                    ModelVertices.Add(newModelVertex);
                }
            }
            else
            {
                foreach (TriangleFace face in meshData.TriangleFaces)
                    ModelTriangles.Add(new TriangleFace(face));
                for (int i = 0; i < meshData.Vertices.Count; i++)
                {
                    ObjectModelVertex newModelVertex = new ObjectModelVertex();
                    newModelVertex.Position = new Vector3(meshData.Vertices[i]);
                    newModelVertex.Normal = new Vector3(meshData.Normals[i]);
                    newModelVertex.Texture1TextureCoordinates = new TextureCoordinates(meshData.TextureCoordinates[i]);
                    if (meshData.BoneIDs.Count > 0)
                        newModelVertex.BoneIndicesTrue[0] = meshData.BoneIDs[i];
                    ModelVertices.Add(newModelVertex);
                }
            }
        }

        private void ProcessMaterials(List<Material> initialMaterials, ref MeshData meshData)
        {
            List<Material> expandedMaterials = new List<Material>();
            foreach (Material material in initialMaterials)
            {
                // Mark exception materials that should always be bright
                if (Properties.AlwaysBrightMaterialsByName.Contains(material.Name))
                    material.AlwaysBrightOverride = true;

                // Save on the exception list
                expandedMaterials.Add(new Material(material));
            }
            foreach (Material material in initialMaterials)
            {
                // If animated, expand out into additional materials with additional geometry
                if (material.IsAnimated() == true)
                {
                    if (material.TextureNames.Count <= 1)
                    {
                        Logger.WriteError("Material '" + material.UniqueName + "' in object '" + Name + "' was marked as animated, but had only one texture.");
                        return;
                    }

                    // Build the new materials and animation properties for this material
                    List<Material> curAnimationMaterials;
                    ExpandAnimatedMaterialAndAddAnimationProperties(material, ref expandedMaterials, out curAnimationMaterials);

                    // Add the additional geometry for the new frames
                    AddGeometryForExpandedMaterialFrames(curAnimationMaterials, ref meshData);
                }
                // If static, build single-frame animation properties
                else
                {
                    // Make a 'blank' animation for this material/texture, since it's static
                    ModelTextureTransparencySequenceSetByMaterialIndex[Convert.ToInt32(material.Index)] = new ObjectModelTrackSequences<Fixed16>();
                    ModelTextureTransparencySequenceSetByMaterialIndex[Convert.ToInt32(material.Index)].AddSequence();
                    ModelTextureTransparencySequenceSetByMaterialIndex[Convert.ToInt32(material.Index)].AddValueToSequence(0, 0, new Fixed16(material.GetTransparencyValue()));
                    ModelTextureTransparencyLookups.Add(Convert.ToUInt16(ModelTextureTransparencyLookups.Count));
                }
            }

            // Generate a model material per material
            Int16 curIndex = 0;
            foreach (Material material in expandedMaterials)
            {
                if (material.TextureNames.Count > 0)
                {
                    ObjectModelTexture newModelTexture = new ObjectModelTexture();
                    newModelTexture.TextureName = material.TextureNames[0];
                    ModelTextures.Add(newModelTexture);
                    ObjectModelMaterial newModelMaterial;
                    if (Properties.AlphaBlendMaterialsByName.Contains(material.Name))
                    {
                        newModelMaterial = new ObjectModelMaterial(material, ObjectModelMaterialBlendType.Alpha);
                    }
                    else
                    {
                        switch (material.MaterialType)
                        {
                            case MaterialType.TransparentAdditive:
                            case MaterialType.TransparentAdditiveUnlit:
                            case MaterialType.TransparentAdditiveUnlitSkydome:
                                {
                                    newModelMaterial = new ObjectModelMaterial(material, ObjectModelMaterialBlendType.Add);
                                }
                                break;
                            case MaterialType.Transparent25Percent:
                            case MaterialType.Transparent75Percent:
                            case MaterialType.Transparent50Percent:
                            case MaterialType.TransparentMasked:
                                {
                                    newModelMaterial = new ObjectModelMaterial(material, ObjectModelMaterialBlendType.Alpha_Key);
                                }
                                break;
                            default:
                                {
                                    newModelMaterial = new ObjectModelMaterial(material, ObjectModelMaterialBlendType.Opaque);
                                }
                                break;
                        }
                    }
                    ModelMaterials.Add(newModelMaterial);
                    ModelTextureAnimationLookup.Add(-1);
                    ModelTextureLookups.Add(curIndex);
                    ModelTextureMappingLookups.Add(curIndex);
                    ++curIndex;
                }
            }
        }

        private void ApplyCustomCollision(ObjectModelCustomCollisionType customCollisionType, ref List<Vector3> collisionVertices, ref List<TriangleFace> collisionTriangleFaces)
        {
            switch (customCollisionType)
            {
                case ObjectModelCustomCollisionType.Ladder:
                    {
                        // Determine the boundary box
                        BoundingBox workingBoundingBox = BoundingBox.GenerateBoxFromVectors(collisionVertices, 0.01f);

                        // Control for world scaling
                        float extendDistance = Configuration.CONFIG_OBJECT_STATIC_LADDER_EXTEND_DISTANCE * Configuration.CONFIG_GENERATE_WORLD_SCALE;
                        float stepDistance = Configuration.CONFIG_OBJECT_STATIC_LADDER_STEP_DISTANCE * Configuration.CONFIG_GENERATE_WORLD_SCALE;

                        // Purge the existing collision data
                        collisionVertices.Clear();
                        collisionTriangleFaces.Clear();

                        // Build steps, extending outword from the longer side
                        float stepHighX = workingBoundingBox.TopCorner.X + extendDistance;
                        float stepMidX = workingBoundingBox.TopCorner.X - (workingBoundingBox.GetXDistance() * 0.5f);
                        float stepLowX = workingBoundingBox.BottomCorner.X - extendDistance;
                        float stepHighY = workingBoundingBox.TopCorner.Y;
                        float stepMidY = workingBoundingBox.TopCorner.Y - (workingBoundingBox.GetYDistance() * 0.5f);
                        float stepLowY = workingBoundingBox.BottomCorner.Y;

                        // Build 'steps' by adding collision quads angled away from the center jutting out the longer sides (makes an upside-down V shape)
                        for (float curZ = workingBoundingBox.BottomCorner.Z; curZ <= workingBoundingBox.TopCorner.Z; curZ += stepDistance)
                        {
                            // 2-step Angle (splits from the middle, angles down and away)
                            int stepStartVert = collisionVertices.Count;
                            collisionVertices.Add(new Vector3(stepMidX, stepHighY, curZ));
                            collisionVertices.Add(new Vector3(stepMidX, stepLowY, curZ));
                            collisionVertices.Add(new Vector3(stepLowX, stepLowY, curZ - (stepDistance * 3)));
                            collisionVertices.Add(new Vector3(stepLowX, stepHighY, curZ - (stepDistance * 3)));
                            collisionTriangleFaces.Add(new TriangleFace(0, stepStartVert + 1, stepStartVert, stepStartVert + 3));
                            collisionTriangleFaces.Add(new TriangleFace(0, stepStartVert + 1, stepStartVert + 3, stepStartVert + 2));

                            stepStartVert = collisionVertices.Count;
                            collisionVertices.Add(new Vector3(stepHighX, stepHighY, curZ - (stepDistance * 3)));
                            collisionVertices.Add(new Vector3(stepHighX, stepLowY, curZ - (stepDistance * 3)));
                            collisionVertices.Add(new Vector3(stepMidX, stepLowY, curZ));
                            collisionVertices.Add(new Vector3(stepMidX, stepHighY, curZ));
                            collisionTriangleFaces.Add(new TriangleFace(0, stepStartVert + 1, stepStartVert, stepStartVert + 3));
                            collisionTriangleFaces.Add(new TriangleFace(0, stepStartVert + 1, stepStartVert + 3, stepStartVert + 2));
                        }

                        // Add collision walls on the sides
                        float highX = workingBoundingBox.TopCorner.X;
                        float lowX = workingBoundingBox.BottomCorner.X;
                        float highY = workingBoundingBox.TopCorner.Y;
                        float lowY = workingBoundingBox.BottomCorner.Y;
                        float highZ = workingBoundingBox.TopCorner.Z;
                        float lowZ = workingBoundingBox.BottomCorner.Z;

                        // Reduce size of short sides to make you step more 'inside' the ladder
                        highX -= workingBoundingBox.GetXDistance() * 0.2f;
                        lowX += workingBoundingBox.GetXDistance() * 0.2f;

                        // Side 1 (side)
                        int wallStartVert = collisionVertices.Count;
                        collisionVertices.Add(new Vector3(highX, lowY, highZ));
                        collisionVertices.Add(new Vector3(highX, lowY, lowZ));
                        collisionVertices.Add(new Vector3(lowX, lowY, lowZ));
                        collisionVertices.Add(new Vector3(lowX, lowY, highZ));
                        collisionTriangleFaces.Add(new TriangleFace(0, wallStartVert + 1, wallStartVert, wallStartVert + 3));
                        collisionTriangleFaces.Add(new TriangleFace(0, wallStartVert + 1, wallStartVert + 3, wallStartVert + 2));

                        // Side 2 (side)
                        wallStartVert = collisionVertices.Count;
                        collisionVertices.Add(new Vector3(highX, highY, highZ));
                        collisionVertices.Add(new Vector3(lowX, highY, highZ));
                        collisionVertices.Add(new Vector3(lowX, highY, lowZ));
                        collisionVertices.Add(new Vector3(highX, highY, lowZ));
                        collisionTriangleFaces.Add(new TriangleFace(0, wallStartVert + 1, wallStartVert, wallStartVert + 3));
                        collisionTriangleFaces.Add(new TriangleFace(0, wallStartVert + 1, wallStartVert + 3, wallStartVert + 2));

                        // Side 3
                        wallStartVert = collisionVertices.Count;
                        collisionVertices.Add(new Vector3(highX, highY, highZ));
                        collisionVertices.Add(new Vector3(highX, highY, lowZ));
                        collisionVertices.Add(new Vector3(highX, lowY, lowZ));
                        collisionVertices.Add(new Vector3(highX, lowY, highZ));
                        collisionTriangleFaces.Add(new TriangleFace(0, wallStartVert + 1, wallStartVert, wallStartVert + 3));
                        collisionTriangleFaces.Add(new TriangleFace(0, wallStartVert + 1, wallStartVert + 3, wallStartVert + 2));

                        // Side 4
                        wallStartVert = collisionVertices.Count;
                        collisionVertices.Add(new Vector3(lowX, highY, highZ));
                        collisionVertices.Add(new Vector3(lowX, lowY, highZ));
                        collisionVertices.Add(new Vector3(lowX, lowY, lowZ));
                        collisionVertices.Add(new Vector3(lowX, highY, lowZ));
                        collisionTriangleFaces.Add(new TriangleFace(0, wallStartVert + 1, wallStartVert, wallStartVert + 3));
                        collisionTriangleFaces.Add(new TriangleFace(0, wallStartVert + 1, wallStartVert + 3, wallStartVert + 2));
                    }
                    break;
                default:
                    {
                        Logger.WriteError("ApplyCustomCollision has unhandled custom collision type of '" + customCollisionType + "'");
                    }
                    break;
            }
        }

        public void CorrectTextureCoordinates()
        {
            HashSet<int> textureCoordRemappedVertexIndices = new HashSet<int>();
            foreach (TriangleFace triangleFace in ModelTriangles)
            {
                if (triangleFace.MaterialIndex < ModelMaterials.Count)
                {
                    if (textureCoordRemappedVertexIndices.Contains(triangleFace.V1) == false)
                    {
                        TextureCoordinates correctedCoordinates = ModelMaterials[triangleFace.MaterialIndex].Material.GetCorrectedBaseCoordinates(ModelVertices[triangleFace.V1].Texture1TextureCoordinates);
                        ModelVertices[triangleFace.V1].Texture1TextureCoordinates.X = correctedCoordinates.X;
                        ModelVertices[triangleFace.V1].Texture1TextureCoordinates.Y = correctedCoordinates.Y;
                        textureCoordRemappedVertexIndices.Add(triangleFace.V1);
                    }
                    if (textureCoordRemappedVertexIndices.Contains(triangleFace.V2) == false)
                    {

                        TextureCoordinates correctedCoordinates = ModelMaterials[triangleFace.MaterialIndex].Material.GetCorrectedBaseCoordinates(ModelVertices[triangleFace.V2].Texture1TextureCoordinates);
                        ModelVertices[triangleFace.V2].Texture1TextureCoordinates.X = correctedCoordinates.X;
                        ModelVertices[triangleFace.V2].Texture1TextureCoordinates.Y = correctedCoordinates.Y;
                        textureCoordRemappedVertexIndices.Add(triangleFace.V2);
                    }
                    if (textureCoordRemappedVertexIndices.Contains(triangleFace.V3) == false)
                    {

                        TextureCoordinates correctedCoordinates = ModelMaterials[triangleFace.MaterialIndex].Material.GetCorrectedBaseCoordinates(ModelVertices[triangleFace.V3].Texture1TextureCoordinates);
                        ModelVertices[triangleFace.V3].Texture1TextureCoordinates.X = correctedCoordinates.X;
                        ModelVertices[triangleFace.V3].Texture1TextureCoordinates.Y = correctedCoordinates.Y;
                        textureCoordRemappedVertexIndices.Add(triangleFace.V3);
                    }
                }
            }
        }

        private void ProcessCollisionData(MeshData meshData, List<Material> materials, List<Vector3> collisionVertices, List<TriangleFace> collisionTriangleFaces)
        {
            // Purge prior data
            CollisionPositions.Clear();
            CollisionFaceNormals.Clear();
            CollisionTriangles.Clear();

            // Generate collision data if there is none and it's from an EQ object
            if (collisionVertices.Count == 0 && (ModelType == ObjectModelType.Skeletal || ModelType == ObjectModelType.SimpleDoodad))
            {
                // Take any non-transparent material geometry and use that to build a mesh
                Dictionary<UInt32, Material> foundMaterials = new Dictionary<UInt32, Material>();
                foreach (TriangleFace face in meshData.TriangleFaces)
                {
                    Material curMaterial = new Material();
                    bool materialFound = false;
                    foreach (Material material in materials)
                    {
                        if (face.MaterialIndex == material.Index)
                        {
                            curMaterial = material;
                            materialFound = true;
                            break;
                        }
                    }
                    if (materialFound == false)
                    {
                        Logger.WriteError("Attempted to build collision data for object '" + Name + "', but could not find material with ID '" + face.MaterialIndex + "'");
                        continue;
                    }
                    if (curMaterial.HasTransparency() == true)
                        continue;
                    if (foundMaterials.ContainsKey(curMaterial.Index) == true)
                        continue;
                    foundMaterials.Add(curMaterial.Index, curMaterial);
                }

                // Build the collision data
                if (foundMaterials.Count > 0)
                {
                    MeshData collisionMesh = meshData.GetMeshDataForMaterials(foundMaterials.Values.ToList().ToArray());
                    foreach (TriangleFace face in collisionMesh.TriangleFaces)
                        collisionTriangleFaces.Add(new TriangleFace(face));
                    foreach (Vector3 position in collisionMesh.Vertices)
                        collisionVertices.Add(new Vector3(position));
                }
            }

            // Apply any custom collision data
            if (Properties.CustomCollisionType != ObjectModelCustomCollisionType.None)
                ApplyCustomCollision(Properties.CustomCollisionType, ref collisionVertices, ref collisionTriangleFaces);

            // Store positions, factoring for world scailing and rotation around Z axis
            foreach (Vector3 collisionVertex in collisionVertices)
                CollisionPositions.Add(new Vector3(collisionVertex));

            // Store triangle indices, ignoring 'blank' ones that have the same value 3x
            foreach (TriangleFace collisionTriangle in collisionTriangleFaces)
                if (collisionTriangle.V1 != collisionTriangle.V2)
                    CollisionTriangles.Add(new TriangleFace(collisionTriangle));

            // Calculate normals using the triangles provided
            foreach (TriangleFace collisionTriangle in CollisionTriangles)
            {
                // Grab related vertices
                Vector3 vertex1 = CollisionPositions[collisionTriangle.V1];
                Vector3 vertex2 = CollisionPositions[collisionTriangle.V2];
                Vector3 vertex3 = CollisionPositions[collisionTriangle.V3];

                // Calculate two edges
                Vector3 edge1 = vertex2 - vertex1;
                Vector3 edge2 = vertex3 - vertex1;

                // Cross product determise the vector, then normalize (using C# libraries to save coding time)
                System.Numerics.Vector3 edge1System = new System.Numerics.Vector3(edge1.X, edge1.Y, edge1.Z);
                System.Numerics.Vector3 edge2System = new System.Numerics.Vector3(edge2.X, edge2.Y, edge2.Z);
                System.Numerics.Vector3 normalSystem = System.Numerics.Vector3.Cross(edge1System, edge2System);
                System.Numerics.Vector3 normalizedNormalSystem = System.Numerics.Vector3.Normalize(normalSystem);

                // Remove NaNs
                if (float.IsNaN(normalizedNormalSystem.X))
                    normalizedNormalSystem.X = 0;
                if (float.IsNaN(normalizedNormalSystem.Y))
                    normalizedNormalSystem.Y = 0;
                if (float.IsNaN(normalizedNormalSystem.Z))
                    normalizedNormalSystem.Z = 0;

                // Invert the normal due to winding order difference
                Vector3 normal = new Vector3(normalizedNormalSystem.X, normalizedNormalSystem.Y, normalizedNormalSystem.Z);
                CollisionFaceNormals.Add(normal);
            }
        }

        private void CalculateBoundingBoxesAndRadii()
        {
            BoundingBox = BoundingBox.GenerateBoxFromVectors(ModelVertices, Configuration.CONFIG_OBJECT_STATIC_MIN_BOUNDING_BOX_SIZE);
            BoundingSphereRadius = BoundingBox.FurthestPointDistanceFromCenter();
            CollisionBoundingBox = BoundingBox.GenerateBoxFromVectors(CollisionPositions, Configuration.CONFIG_GENERATE_ADDED_BOUNDARY_AMOUNT);
            CollisionSphereRaidus = CollisionBoundingBox.FurthestPointDistanceFromCenter();
        }

        private void ExpandAnimatedMaterialAndAddAnimationProperties(Material initialMaterial, ref List<Material> expandedMaterials,
            out List<Material> curAnimationMaterials)
        {
            // Create a unique material and animation frame for every material
            curAnimationMaterials = new List<Material>();
            UInt32 curAnimationTimestamp = 0;
            for (int textureIter = 0; textureIter < initialMaterial.TextureNames.Count; ++textureIter)
            {
                // Update the material values if it's the first in the chain, otherwise create a new one
                string curMaterialName = initialMaterial.UniqueName + "Anim_" + textureIter;
                Material curMaterial;
                int curMaterialIndex;
                if (textureIter == 0)
                {
                    initialMaterial.UniqueName = curMaterialName;
                    curMaterial = initialMaterial;
                    curMaterialIndex = Convert.ToInt32(initialMaterial.Index);
                }
                else
                {
                    UInt32 newMaterialIndex = GetUniqueMaterialIDFromMaterials(expandedMaterials);
                    List<string> newMaterialTextureName = new List<string>() { initialMaterial.TextureNames[textureIter] };
                    Material newAnimationMaterial = new Material(curMaterialName, initialMaterial.UniqueName, newMaterialIndex, initialMaterial.MaterialType,
                        newMaterialTextureName, initialMaterial.AnimationDelayMs, initialMaterial.TextureWidth, initialMaterial.TextureHeight, initialMaterial.AlwaysBrightOverride);
                    curMaterial = newAnimationMaterial;
                    expandedMaterials.Add(curMaterial);
                    curMaterialIndex = Convert.ToInt32(newMaterialIndex);
                }

                // Create the new transparency animation for this frame
                ObjectModelTrackSequences<Fixed16> newAnimation = new ObjectModelTrackSequences<Fixed16>();
                newAnimation.InterpolationType = ObjectModelAnimationInterpolationType.None;
                newAnimation.GlobalSequenceID = Convert.ToUInt16(GlobalLoopSequenceLimits.Count);
                int curSequenceId = newAnimation.AddSequence();

                // Add a blank (transparent) frame to this animation for every frame that already exists, and add a blank to those others
                for (int i = 0; i < curAnimationMaterials.Count; ++i)
                {
                    newAnimation.AddValueToSequence(0, Convert.ToUInt32(i) * initialMaterial.AnimationDelayMs, new Fixed16(0));
                    ModelTextureTransparencySequenceSetByMaterialIndex[Convert.ToInt32(curAnimationMaterials[i].Index)].AddValueToSequence(0, curAnimationTimestamp, new Fixed16(0));
                }

                // Add this shown (non-transparent) frame
                newAnimation.AddValueToSequence(0, curAnimationTimestamp, new Fixed16(curMaterial.GetTransparencyValue()));

                // Add this animation and the texture lookup, which should match current count
                ModelTextureTransparencySequenceSetByMaterialIndex[curMaterialIndex] = newAnimation;
                ModelTextureTransparencyLookups.Add(Convert.ToUInt16(ModelTextureTransparencyLookups.Count));
                curAnimationTimestamp += initialMaterial.AnimationDelayMs;
                curAnimationMaterials.Add(curMaterial);
            }

            // Reduce the texture list for the first material to one
            curAnimationMaterials[0].TextureNames = new List<string>() { initialMaterial.TextureNames[0] };

            // Save this global sequence so that it loops
            GlobalLoopSequenceLimits.Add(Convert.ToUInt32(curAnimationMaterials.Count) * initialMaterial.AnimationDelayMs);
        }

        private void AddGeometryForExpandedMaterialFrames(List<Material> frameMaterials, ref MeshData meshData)
        {
            for (int i = 1; i < frameMaterials.Count; i++)
            {
                // Create new triangles
                List<TriangleFace> newTriangleFaces = new List<TriangleFace>();

                // Determine what the min vertex index is for the triangles, as well as capture the reference indices for vertex copies
                int minSourceTriangleVertexIndex = -1;
                int maxSourceTriangleVertexIndex = -1;
                foreach (TriangleFace triangleFace in meshData.TriangleFaces)
                {
                    if (triangleFace.MaterialIndex != frameMaterials[0].Index)
                        continue;

                    // Store the vertex offsets to be used in the next section
                    if (minSourceTriangleVertexIndex == -1 || triangleFace.GetSmallestIndex() < minSourceTriangleVertexIndex)
                        minSourceTriangleVertexIndex = triangleFace.GetSmallestIndex();
                    if (maxSourceTriangleVertexIndex == -1 || triangleFace.GetLargestIndex() > maxSourceTriangleVertexIndex)
                        maxSourceTriangleVertexIndex = triangleFace.GetLargestIndex();
                }
                if (minSourceTriangleVertexIndex == -1)
                {
                    Logger.WriteError("Could not find any triangle face vertices for material '" + frameMaterials[0].UniqueName + "' in object '" + Name + "'");
                    return;
                }

                // Create new triangles using the min identified earlier
                int newVertexIndexStartOffsetAdd = meshData.Vertices.Count - minSourceTriangleVertexIndex;
                foreach (TriangleFace triangleFace in meshData.TriangleFaces)
                {
                    if (triangleFace.MaterialIndex != frameMaterials[0].Index)
                        continue;
                    TriangleFace newTriangleFace = new TriangleFace(triangleFace);
                    newTriangleFace.V1 += newVertexIndexStartOffsetAdd;
                    newTriangleFace.V2 += newVertexIndexStartOffsetAdd;
                    newTriangleFace.V3 += newVertexIndexStartOffsetAdd;
                    newTriangleFace.MaterialIndex = Convert.ToInt32(frameMaterials[i].Index);
                    newTriangleFaces.Add(newTriangleFace);
                }
                foreach (TriangleFace triangleFace in newTriangleFaces)
                    meshData.TriangleFaces.Add(triangleFace);

                // Create new geometry data                        
                for (int vi = minSourceTriangleVertexIndex; vi <= maxSourceTriangleVertexIndex; ++vi)
                {
                    meshData.Vertices.Add(new Vector3(meshData.Vertices[vi]));
                    meshData.Normals.Add(new Vector3(meshData.Normals[vi]));
                    meshData.TextureCoordinates.Add(new TextureCoordinates(meshData.TextureCoordinates[vi]));
                }
            }
        }

        private UInt32 GetUniqueMaterialIDFromMaterials(List<Material> materials)
        {
            UInt32 highestExistingID = 0;
            foreach (Material material in materials)
                if (material.Index > highestExistingID)
                    highestExistingID = material.Index;
            return highestExistingID + 1;
        }
    }
}
