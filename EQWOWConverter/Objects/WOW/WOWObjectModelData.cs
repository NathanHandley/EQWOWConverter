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
using EQWOWConverter.ModelObjects;
using EQWOWConverter.WOWFiles;
using EQWOWConverter.Zones;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Objects
{
    internal class WOWObjectModelData
    {
        public string Name = string.Empty;
        public List<UInt32> GlobalLoopSequenceLimits = new List<UInt32>();
        public List<ModelAnimation> ModelAnimations = new List<ModelAnimation>();
        public List<Int16> AnimationSequenceIDLookups = new List<Int16>();
        public List<ModelVertex> ModelVerticies = new List<ModelVertex>();
        public List<ModelBone> ModelBones = new List<ModelBone>();
        public List<ModelTextureAnimation> ModelTextureAnimations = new List<ModelTextureAnimation>();
        public List<Int16> ModelBoneKeyLookups = new List<Int16>();
        public List<Int16> ModelBoneLookups = new List<Int16>();
        public List<ModelMaterial> ModelMaterials = new List<ModelMaterial>();
        public List<ModelTexture> ModelTextures = new List<ModelTexture>();
        public List<Int16> ModelTextureLookups = new List<Int16>();
        public List<Int16> ModelTextureMappingLookups = new List<Int16>();
        public List<Int16> ModelReplaceableTextureLookups = new List<Int16>();
        public List<UInt16> ModelTextureTransparencyWeightsLookups = new List<UInt16>();
        public List<ModelTrackSequences<Fixed16>> ModelTextureTransparencySequencesSet = new List<ModelTrackSequences<Fixed16>>();
        public List<Int16> ModelTextureAnimationLookup = new List<Int16>();
        public List<UInt16> ModelSecondTextureMaterialOverrides = new List<UInt16>();
        public List<TriangleFace> ModelTriangles = new List<TriangleFace>();
        public BoundingBox BoundingBox = new BoundingBox();
        public float BoundingSphereRadius = 0f;

        public List<Vector3> CollisionPositions = new List<Vector3>();
        public List<Vector3> CollisionFaceNormals = new List<Vector3>();
        public List<TriangleFace> CollisionTriangles = new List<TriangleFace>();
        public BoundingBox CollisionBoundingBox = new BoundingBox();
        public float CollisionSphereRaidus = 0f;

        public WOWObjectModelData()
        {

        }

        public UInt16 GetTextureLookupIndexForMaterial(int materialID)
        {
            // For now, it's a 1 to 1 match
            return Convert.ToUInt16(materialID);
        }

        public UInt16 GetTextureTransparencyLookupIndexForMaterial(int materialID)
        {
            // Array should match indicies
            return ModelTextureTransparencyWeightsLookups[materialID];
        }

        // TODO: Vertex Colors
        public void Load(string name, List<Material> materials, List<TriangleFace> triangleFaces, List<Vector3> verticies,
            List<Vector3> normals, List<ColorRGBA> vertexColors, List<TextureCoordinates> textureCoordinates,
            List<Vector3> collisionVerticies, List<TriangleFace> collisionTriangleFaces, bool isFromRawEQObject)
        {
            // Save Name
            Name = name;

            // Control for bad objects
            if (verticies.Count != textureCoordinates.Count && verticies.Count != normals.Count)
            {
                Logger.WriteError("Failed to load wowobject named '" + Name + "' since vertex count doesn't match texture coordinate count or normal count");
                return;
            }

            // Sort the geometry
            // TODO: Vertex Colors
            SortGeometry(ref triangleFaces, ref verticies, ref normals, ref textureCoordinates);

            // Perform EQ->WoW translations if this is coming from a raw EQ object
            if (isFromRawEQObject == true)
                ApplyEQToWoWGeometryTranslations(ref triangleFaces, ref verticies, ref textureCoordinates);

            // "Flatten" animated materials by producing new materials with replicating geometry on a per texture basis, and creating texture animations
            List<Material> flattenedMaterials = new List<Material>();
            foreach(Material originalMaterial in materials)
            {
                // If animated, flatten the material and replicate geometry
                if (originalMaterial.IsAnimated() == true)
                {
                    // "Flatten" the material by making copies on a per-texture basis
                    List<Material> curAnimationSubMaterials = new List<Material>();
                    UInt32 curAnimationTimestamp = 0;
                    foreach (string textureName in originalMaterial.TextureNames)
                    {
                        // Create a new material just for this texture
                        string curMaterialName = originalMaterial.Name + "Anim_" + curAnimationSubMaterials.Count;
                        List<string> curMaterialTextureName = new List<string>() { textureName };
                        UInt32 materialIndex = Convert.ToUInt32(flattenedMaterials.Count);
                        Material newAnimationMaterial = new Material(curMaterialName, materialIndex, originalMaterial.MaterialType, curMaterialTextureName,
                            0, originalMaterial.TextureWidth, originalMaterial.TextureHeight);

                        // Create the new transparency animation for this frame
                        ModelTrackSequences<Fixed16> newAnimation = new ModelTrackSequences<Fixed16>();
                        newAnimation.InterpolationType = ModelAnimationInterpolationType.None;
                        newAnimation.GlobalSequenceID = Convert.ToUInt16(GlobalLoopSequenceLimits.Count);
                        int curSequenceId = newAnimation.AddSequence();

                        // Add a blank (transparent) frame to this animation for every frame that already exists, and add a blank to those others
                        for (int i = 0; i < ModelTextureTransparencySequencesSet.Count; ++i)
                        {
                            newAnimation.AddValueToSequence(0, Convert.ToUInt32(i) * originalMaterial.AnimationDelayMs, new Fixed16(0));
                            ModelTextureTransparencySequencesSet[i].AddValueToSequence(0, curAnimationTimestamp, new Fixed16(0));
                        }

                        // Add this shown (non-transparent) frame
                        newAnimation.AddValueToSequence(0, curAnimationTimestamp, new Fixed16(Int16.MaxValue));

                        // Add this animation and the texture lookup, which should match current count
                        ModelTextureTransparencySequencesSet.Add(newAnimation);
                        ModelTextureTransparencyWeightsLookups.Add(Convert.ToUInt16(ModelTextureTransparencySequencesSet.Count - 1));
                        curAnimationTimestamp += originalMaterial.AnimationDelayMs;

                        // Save this material
                        curAnimationSubMaterials.Add(newAnimationMaterial);
                        flattenedMaterials.Add(newAnimationMaterial);
                    }

                    // Save this global sequence
                    GlobalLoopSequenceLimits.Add(Convert.ToUInt32(originalMaterial.NumOfAnimationFrames()) * originalMaterial.AnimationDelayMs);

                    // Make appropriate geometry copies for the animated frames
                    for (int i = 1; i < curAnimationSubMaterials.Count; i++)
                    {
                        // Create new triangles
                        List<TriangleFace> newTriangleFaces = new List<TriangleFace>();
                        int newVertexIndexStartOffset = verticies.Count;
                        int referenceVertexStartOffset = -1;
                        int referenceVertexEndOffset = -1;
                        foreach (TriangleFace triangleFace in triangleFaces)
                        {
                            if (triangleFace.MaterialIndex != originalMaterial.Index)
                                continue;
                            TriangleFace newTriangleFace = new TriangleFace(triangleFace);
                            newTriangleFace.V1 += newVertexIndexStartOffset;
                            newTriangleFace.V2 += newVertexIndexStartOffset;
                            newTriangleFace.V3 += newVertexIndexStartOffset;
                            newTriangleFace.MaterialIndex = Convert.ToInt32(curAnimationSubMaterials[i].Index);
                            newTriangleFaces.Add(newTriangleFace);

                            // Store the vertex offsets to be used in the next section
                            if (referenceVertexStartOffset == -1 || triangleFace.GetSmallestIndex() < referenceVertexStartOffset)
                                referenceVertexStartOffset = triangleFace.GetSmallestIndex();
                            if (referenceVertexEndOffset == -1 || triangleFace.GetLargestIndex() > referenceVertexEndOffset)
                                referenceVertexEndOffset = triangleFace.GetLargestIndex();
                        }
                        foreach (TriangleFace triangleFace in newTriangleFaces)
                            triangleFaces.Add(triangleFace);

                        // Create new geometry data                        
                        for(int vi = referenceVertexStartOffset; vi <= referenceVertexEndOffset; ++vi)
                        {
                            verticies.Add(new Vector3(verticies[vi]));
                            normals.Add(new Vector3(normals[vi]));
                            textureCoordinates.Add(new TextureCoordinates(textureCoordinates[vi]));
                        }
                    }

                    // Update references in the root animation frame
                    foreach (TriangleFace triangleFace in triangleFaces)
                    {
                        if (triangleFace.MaterialIndex != originalMaterial.Index)
                            continue;
                        triangleFace.MaterialIndex = Convert.ToInt32(curAnimationSubMaterials[0].Index);
                    }
                }
                else
                {   
                    // Make a new material to reflect the updated index
                    Material newMaterial = new Material(originalMaterial);
                    newMaterial.Index = Convert.ToUInt32(flattenedMaterials.Count);
                    flattenedMaterials.Add(newMaterial);

                    // Update old face material index references
                    foreach (TriangleFace face in triangleFaces)
                        if (face.MaterialIndex == originalMaterial.Index)
                            face.MaterialIndex = Convert.ToInt32(newMaterial.Index);

                    // Make a 'blank' animation for this material/texture, since it's static
                    int newTransSeqSetIndex = ModelTextureTransparencySequencesSet.Count;
                    ModelTextureTransparencySequencesSet.Add(new ModelTrackSequences<Fixed16>());
                    ModelTextureTransparencySequencesSet[newTransSeqSetIndex].AddSequence();
                    ModelTextureTransparencySequencesSet[newTransSeqSetIndex].AddValueToSequence(0, 0, new Fixed16(32767));
                    ModelTextureTransparencyWeightsLookups.Add(Convert.ToUInt16(ModelTextureTransparencySequencesSet.Count - 1));
                }
            }

            // Save the geometry data
            foreach (TriangleFace face in triangleFaces)
                ModelTriangles.Add(new TriangleFace(face));
            for (int i = 0; i < verticies.Count; i++)
            {
                ModelVertex newModelVertex = new ModelVertex();
                newModelVertex.Position = new Vector3(verticies[i]);
                newModelVertex.Normal = new Vector3(normals[i]);
                newModelVertex.Texture1TextureCoordinates = new TextureCoordinates(textureCoordinates[i]);
                ModelVerticies.Add(newModelVertex);
            }

            // Process materials
            ProcessMaterials(flattenedMaterials.ToArray());

            // Correct any texture coordinates
            CorrectTextureCoordinates();

            // Process the rest
            //ProcessCollisionData(verticies, triangleFaces, collisionVerticies, collisionTriangleFaces);
            CalculateBoundingBoxesAndRadii();

            // HARD CODED FOR STATIC --------------------------------------------------------------------
            // Create a base bone
            //AnimationSequenceIDLookups.Add(0); // Maps animations to the IDs in AnimationData.dbc - None for static
            ModelBones.Add(new ModelBone());
            ModelBoneKeyLookups.Add(-1);
            ModelBoneLookups.Add(0);
            ModelReplaceableTextureLookups.Add(-1); // No replace lookup

            // Make one animation (standing)
            ModelAnimations.Add(new ModelAnimation());
            ModelAnimations[0].BoundingBox = new BoundingBox(BoundingBox);
            ModelAnimations[0].BoundingRadius = BoundingSphereRadius;
            //-------------------------------------------------------------------------------------------
        }

        // Note: Only working for static for now, but more to come
        public void LoadFromEQObject(string name, EQModelObjectData eqObject)
        {
            // Save Name
            Name = name;

            // Change face orientation for culling differences between EQ and WoW
            foreach (TriangleFace eqFace in eqObject.TriangleFaces)
            {
                TriangleFace newFace = new TriangleFace();
                newFace.MaterialIndex = eqFace.MaterialIndex;

                // Rotate the verticies for culling differences
                newFace.V1 = eqFace.V3;
                newFace.V2 = eqFace.V2;
                newFace.V3 = eqFace.V1;

                // Add it
                ModelTriangles.Add(newFace);
            }

            if (eqObject.Verticies.Count != eqObject.TextureCoords.Count && eqObject.Verticies.Count != eqObject.Normals.Count)
            {
                Logger.WriteError("Failed to load wowobject from eqobject named '" + name + "' since vertex count doesn't match texture coordinate count or normal count");
                return;
            }

            // Read in all the verticies
            for (int i = 0; i < eqObject.Verticies.Count; i++)
            {
                ModelVertex newModelVertex = new ModelVertex();

                // Read vertex, and account for world scale and rotate around the z axis 180 degrees
                Vector3 curVertex = eqObject.Verticies[i];
                newModelVertex.Position.X = curVertex.X * Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
                newModelVertex.Position.X = -newModelVertex.Position.X;
                newModelVertex.Position.Y = curVertex.Y * Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
                newModelVertex.Position.Y = -newModelVertex.Position.Y;
                newModelVertex.Position.Z = curVertex.Z * Configuration.CONFIG_EQTOWOW_WORLD_SCALE;

                // Read texture coordinates, and factor for mapping differences between EQ and WoW
                TextureCoordinates curTextureCoordinates = eqObject.TextureCoords[i];
                newModelVertex.Texture1TextureCoordinates.X = curTextureCoordinates.X;
                newModelVertex.Texture1TextureCoordinates.Y = -1 * curTextureCoordinates.Y;

                // Read normals
                Vector3 curNormal = eqObject.Normals[i];
                newModelVertex.Normal.X = curNormal.X;
                newModelVertex.Normal.Y = curNormal.Y;
                newModelVertex.Normal.Z = curNormal.Z;

                ModelVerticies.Add(newModelVertex);
            }

            // Process materials
            foreach(Material material in eqObject.Materials)
                ModelTextureTransparencyWeightsLookups.Add(0);
            ProcessMaterials(eqObject.Materials.ToArray());

            // Correct any coordinates
            CorrectTextureCoordinates();

            // Process the rest
            ProcessCollisionData(eqObject.Verticies, eqObject.TriangleFaces, eqObject.CollisionVerticies, eqObject.CollisionTriangleFaces);
            SortGeometry();
            CalculateBoundingBoxesAndRadii();

            // HARD CODED FOR STATIC --------------------------------------------------------------------
            // Create a base bone
            //AnimationSequenceIDLookups.Add(0); // Maps animations to the IDs in AnimationData.dbc - None for static
            ModelBones.Add(new ModelBone());
            ModelBoneKeyLookups.Add(-1);
            ModelBoneLookups.Add(0);
            ModelTextureTransparencySequencesSet.Add(new ModelTrackSequences<Fixed16>());
            ModelTextureTransparencySequencesSet[0].AddValueToSequence(ModelTextureTransparencySequencesSet[0].AddSequence(), 0, new Fixed16(32767));
            ModelReplaceableTextureLookups.Add(-1); // No replace lookup

            // Make one animation
            ModelAnimations.Add(new ModelAnimation());
            ModelAnimations[0].BoundingBox = new BoundingBox(BoundingBox);
            ModelAnimations[0].BoundingRadius = BoundingSphereRadius;
            //-------------------------------------------------------------------------------------------
        }

        public void LoadFromZoneAnimatedMaterial(string name, Material material, List<TriangleFace> triangleFaces, List<Vector3> verticies,
            List<Vector3> normals, List<ColorRGBA> vertexColors, List<TextureCoordinates> textureCoordinates)
        {
            Load(name, new List<Material> { material }, triangleFaces, verticies, normals, vertexColors, textureCoordinates,
                new List<Vector3>(), new List<TriangleFace>(), false);
        //    // Control for bad objects
        //    if (verticies.Count != textureCoordinates.Count && verticies.Count != normals.Count)
        //    {
        //        Logger.WriteError("Failed to load wowobject from zone data since vertex count doesn't match texture coordinate count or normal count");
        //        return;
        //    }
        //    if (material.TextureNames.Count <= 1)
        //    {
        //        Logger.WriteError("Failed to load wowobject from zone data since there was only one texture");
        //        return;
        //    }

        //    // Generate geometry on a per material basis to fake the animation with duplicate transparent textures
        //    List<Material> animationFrameMaterials = new List<Material>();
        //    UInt32 curAnimationTimestamp = 0;
        //    foreach (string textureName in material.TextureNames)
        //    {
        //        // Each texture gets a new unique material, and a copy of all the geometry
        //        string curMaterialName = material.Name + "Anim_" + animationFrameMaterials.Count;
        //        List<string> curMaterialTextureName = new List<string>() { textureName };
        //        UInt32 materialIndex = Convert.ToUInt32(animationFrameMaterials.Count);
        //        Material newAnimationMaterial = new Material(curMaterialName, materialIndex, material.MaterialType, curMaterialTextureName,
        //            0, material.TextureWidth, material.TextureHeight);

        //        // Add this frame's triangles and verticies, accounting for the new material
        //        foreach (TriangleFace triangleFace in triangleFaces)
        //        {
        //            TriangleFace newTriangleFace = new TriangleFace(triangleFace);
        //            newTriangleFace.V1 += (animationFrameMaterials.Count * verticies.Count);
        //            newTriangleFace.V2 += (animationFrameMaterials.Count * verticies.Count);
        //            newTriangleFace.V3 += (animationFrameMaterials.Count * verticies.Count);
        //            newTriangleFace.MaterialIndex = Convert.ToInt32(newAnimationMaterial.Index);
        //            ModelTriangles.Add(newTriangleFace);
        //        }
        //        for (int i = 0; i < verticies.Count; i++)
        //        {
        //            ModelVertex newModelVertex = new ModelVertex();
        //            newModelVertex.Position.X = verticies[i].X;
        //            newModelVertex.Position.Y = verticies[i].Y;
        //            newModelVertex.Position.Z = verticies[i].Z;
        //            newModelVertex.Normal.X = normals[i].X;
        //            newModelVertex.Normal.Y = normals[i].Y;
        //            newModelVertex.Normal.Z = normals[i].Z;
        //            TextureCoordinates correctedCoordinates = material.GetCorrectedBaseCoordinates(textureCoordinates[i]);
        //            newModelVertex.Texture1TextureCoordinates.X = correctedCoordinates.X;
        //            newModelVertex.Texture1TextureCoordinates.Y = correctedCoordinates.Y;
        //            ModelVerticies.Add(newModelVertex);
        //        }

        //        //// Create the new transparency "animation"
        //        ModelTrackSequences<Fixed16> newAnimation = new ModelTrackSequences<Fixed16>();
        //        newAnimation.InterpolationType = ModelAnimationInterpolationType.None;
        //        newAnimation.GlobalSequenceID = 0;
        //        int curSequenceId = newAnimation.AddSequence();

        //        // Add a blank (transparent) frame to this animation for every frame that already exists, and add a blank to those others
        //        for (int i = 0; i < ModelTextureTransparencySequencesSet.Count; ++i)
        //        {
        //            newAnimation.AddValueToSequence(0, Convert.ToUInt32(i) * material.AnimationDelayMs, new Fixed16(0));
        //            ModelTextureTransparencySequencesSet[i].AddValueToSequence(0, curAnimationTimestamp, new Fixed16(0));
        //        }

        //        // Add this shown (non-transparent) frame
        //        newAnimation.AddValueToSequence(0, curAnimationTimestamp, new Fixed16(Int16.MaxValue));

        //        // Add this animation and the texture lookup, which should match current count
        //        ModelTextureTransparencySequencesSet.Add(newAnimation);
        //        ModelTextureTransparencyWeightsLookups.Add(Convert.ToUInt16(ModelTextureTransparencySequencesSet.Count - 1));
        //        curAnimationTimestamp += material.AnimationDelayMs;

        //        animationFrameMaterials.Add(newAnimationMaterial);
        //    }

        //    // Save name and triangles
        //    Name = name;

        //    // Add the global loop sequence to contain this
        //    GlobalLoopSequenceLimits.Add(Convert.ToUInt32(material.NumOfAnimationFrames()) * material.AnimationDelayMs);

        //    // Process material
        //    ProcessMaterials(animationFrameMaterials.ToArray());

        //    // Correct coordinates
        //    CorrectTextureCoordinates();

        //    // Build the bounding box (and no collision)
        //    CalculateBoundingBoxesAndRadii();

        //    // HARD CODED FOR STATIC --------------------------------------------------------------------
        //    // Create a base bone
        //    //AnimationSequenceIDLookups.Add(0); // Maps animations to the IDs in AnimationData.dbc - None for static
        //    ModelBones.Add(new ModelBone());
        //    ModelBoneKeyLookups.Add(-1);
        //    ModelBoneLookups.Add(0);
        //    ModelReplaceableTextureLookups.Add(-1); // No replace lookup

        //    // Make one animation
        //    ModelAnimations.Add(new ModelAnimation());
        //    ModelAnimations[0].BoundingBox = new BoundingBox(BoundingBox);
        //    ModelAnimations[0].BoundingRadius = BoundingSphereRadius;
        //    //-------------------------------------------------------------------------------------------
        }

        public void ApplyEQToWoWGeometryTranslations(ref List<TriangleFace> triangleFaces, ref List<Vector3> verticies,
            ref List<TextureCoordinates> textureCoordinates)
        {
            // Change face indices for winding over differences
            foreach (TriangleFace eqFace in triangleFaces)
            {
                int swapFaceVertexIndex = eqFace.V3;
                eqFace.V3 = eqFace.V1;
                eqFace.V1 = eqFace.V3;
            }
            // Perform vertex world scaling and 180 Z-Axis degree rotation
            foreach (Vector3 eqVertex in verticies)
            {
                eqVertex.X *= -1 * Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
                eqVertex.Y *= -1 * Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
                eqVertex.Z *= Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            }
            // Flip Y on the texture coordinates
            foreach (TextureCoordinates eqTextureCoordinates in textureCoordinates)
            {
                eqTextureCoordinates.Y *= -1;
            }
        }

        public void CorrectTextureCoordinates()
        {
            HashSet<int> textureCoordRemappedVertexIndicies = new HashSet<int>();
            foreach (TriangleFace triangleFace in ModelTriangles)
            {
                if (triangleFace.MaterialIndex < ModelMaterials.Count)
                {
                    if (textureCoordRemappedVertexIndicies.Contains(triangleFace.V1) == false)
                    {
                        TextureCoordinates correctedCoordinates = ModelMaterials[triangleFace.MaterialIndex].Material.GetCorrectedBaseCoordinates(ModelVerticies[triangleFace.V1].Texture1TextureCoordinates);
                        ModelVerticies[triangleFace.V1].Texture1TextureCoordinates.X = correctedCoordinates.X;
                        ModelVerticies[triangleFace.V1].Texture1TextureCoordinates.Y = correctedCoordinates.Y;
                        textureCoordRemappedVertexIndicies.Add(triangleFace.V1);
                    }
                    if (textureCoordRemappedVertexIndicies.Contains(triangleFace.V2) == false)
                    {

                        TextureCoordinates correctedCoordinates = ModelMaterials[triangleFace.MaterialIndex].Material.GetCorrectedBaseCoordinates(ModelVerticies[triangleFace.V2].Texture1TextureCoordinates);
                        ModelVerticies[triangleFace.V2].Texture1TextureCoordinates.X = correctedCoordinates.X;
                        ModelVerticies[triangleFace.V2].Texture1TextureCoordinates.Y = correctedCoordinates.Y;
                        textureCoordRemappedVertexIndicies.Add(triangleFace.V2);
                    }
                    if (textureCoordRemappedVertexIndicies.Contains(triangleFace.V3) == false)
                    {

                        TextureCoordinates correctedCoordinates = ModelMaterials[triangleFace.MaterialIndex].Material.GetCorrectedBaseCoordinates(ModelVerticies[triangleFace.V3].Texture1TextureCoordinates);
                        ModelVerticies[triangleFace.V3].Texture1TextureCoordinates.X = correctedCoordinates.X;
                        ModelVerticies[triangleFace.V3].Texture1TextureCoordinates.Y = correctedCoordinates.Y;
                        textureCoordRemappedVertexIndicies.Add(triangleFace.V3);
                    }
                }
            }
        }

        private void ProcessMaterials(params Material[] materials)
        {
            // Generate a model material per material
            Int16 curIndex = 0;
            foreach (Material material in materials)
            {
                // TODO: Account for more textures once texture animation is fully implemented
                if (material.TextureNames.Count > 0)
                {
                    ModelTexture newModelTexture = new ModelTexture();
                    // TODO: Account for more textures once texture animation is fully implemented
                    newModelTexture.TextureName = material.TextureNames[0];
                    ModelTextures.Add(newModelTexture);
                    ModelMaterial newModelMaterial;
                    switch (material.MaterialType)
                    {
                        case MaterialType.TransparentAdditive:
                        case MaterialType.TransparentAdditiveUnlit:
                        case MaterialType.TransparentAdditiveUnlitSkydome:
                            {
                                newModelMaterial = new ModelMaterial(material, ModelMaterialBlendType.Add);
                            }
                            break;
                        case MaterialType.Transparent25Percent:
                        case MaterialType.Transparent75Percent:
                        case MaterialType.Transparent50Percent:
                        case MaterialType.TransparentMasked:
                            {
                                newModelMaterial = new ModelMaterial(material, ModelMaterialBlendType.Alpha);
                            }
                            break;
                        default:
                            {
                                newModelMaterial = new ModelMaterial(material, ModelMaterialBlendType.Opaque);
                            }
                            break;
                    }
                    ModelMaterials.Add(newModelMaterial);
                    ModelTextureAnimationLookup.Add(-1);
                    ModelTextureLookups.Add(curIndex);
                    ModelTextureMappingLookups.Add(curIndex);
                    ++curIndex;
                }
            }
        }

        private void ProcessCollisionData(List<Vector3> verticies, List<TriangleFace> triangleFaces,
            List<Vector3> collisionVerticies, List<TriangleFace> collisionTriangleFaces)
        {
            // Purge prior data
            CollisionPositions.Clear();
            CollisionFaceNormals.Clear();
            CollisionTriangles.Clear();

            // If there was no collision data then the whole object is collidable otherwise use the collision data
            List<Vector3> collisionPositions = new List<Vector3>();
            List<TriangleFace> collisionTriangles = new List<TriangleFace>();
            if (collisionVerticies.Count == 0)
            {
                collisionPositions = verticies;
                collisionTriangles = triangleFaces;
            }
            else
            {
                collisionPositions = collisionVerticies;
                collisionTriangles = collisionTriangleFaces;
            }

            // Store positions, factoring for world scailing and rotation around Z axis
            foreach (Vector3 collisionVertex in collisionPositions)
            {
                Vector3 curVertex = new Vector3();
                curVertex.X = collisionVertex.X * Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
                curVertex.Y = collisionVertex.Y * Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
                curVertex.Z = collisionVertex.Z * Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
                curVertex.X = -curVertex.X;
                curVertex.Y = -curVertex.Y;
                CollisionPositions.Add(new Vector3(curVertex));
            }

            // Store triangle indicies, ignoring 'blank' ones that have the same value 3x
            foreach (TriangleFace collisionTriangle in collisionTriangles)
                if (collisionTriangle.V1 != collisionTriangle.V2)
                    CollisionTriangles.Add(new TriangleFace(collisionTriangle));

            // Calculate normals using the triangles provided
            foreach (TriangleFace collisionTriangle in CollisionTriangles)
            {
                // Grab related verticies
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

                // Invert the normal due to winding order difference
                Vector3 normal = new Vector3(normalizedNormalSystem.X * -1, normalizedNormalSystem.Y * -1, normalizedNormalSystem.Z * -1);
                CollisionFaceNormals.Add(normal);
            }
        }

        private void SortGeometry()
        {
            ModelTriangles.Sort();

            // Reorder the verticies / texcoords / normals / vertcolors to match the sorted triangle faces
            List<ModelVertex> sortedVerticies = new List<ModelVertex>();
            List<TriangleFace> sortedTriangleFaces = new List<TriangleFace>();
            Dictionary<int, int> oldNewVertexIndicies = new Dictionary<int, int>();
            for (int i = 0; i < ModelTriangles.Count; i++)
            {
                TriangleFace curTriangleFace = ModelTriangles[i];

                // Face vertex 1
                if (oldNewVertexIndicies.ContainsKey(curTriangleFace.V1))
                {
                    // This index was aready remapped
                    curTriangleFace.V1 = oldNewVertexIndicies[curTriangleFace.V1];
                }
                else
                {
                    // Store new mapping
                    int oldVertIndex = curTriangleFace.V1;
                    int newVertIndex = sortedVerticies.Count;
                    oldNewVertexIndicies.Add(oldVertIndex, newVertIndex);
                    curTriangleFace.V1 = newVertIndex;

                    // Add verticies
                    sortedVerticies.Add(ModelVerticies[oldVertIndex]);
                }

                // Face vertex 2
                if (oldNewVertexIndicies.ContainsKey(curTriangleFace.V2))
                {
                    // This index was aready remapped
                    curTriangleFace.V2 = oldNewVertexIndicies[curTriangleFace.V2];
                }
                else
                {
                    // Store new mapping
                    int oldVertIndex = curTriangleFace.V2;
                    int newVertIndex = sortedVerticies.Count;
                    oldNewVertexIndicies.Add(oldVertIndex, newVertIndex);
                    curTriangleFace.V2 = newVertIndex;

                    // Add verticies
                    sortedVerticies.Add(ModelVerticies[oldVertIndex]);
                }

                // Face vertex 3
                if (oldNewVertexIndicies.ContainsKey(curTriangleFace.V3))
                {
                    // This index was aready remapped
                    curTriangleFace.V3 = oldNewVertexIndicies[curTriangleFace.V3];
                }
                else
                {
                    // Store new mapping
                    int oldVertIndex = curTriangleFace.V3;
                    int newVertIndex = sortedVerticies.Count;
                    oldNewVertexIndicies.Add(oldVertIndex, newVertIndex);
                    curTriangleFace.V3 = newVertIndex;

                    // Add verticies
                    sortedVerticies.Add(ModelVerticies[oldVertIndex]);
                }

                // Save this updated triangle
                sortedTriangleFaces.Add(curTriangleFace);
            }

            // Save the sorted values
            ModelVerticies = sortedVerticies;
            ModelTriangles = sortedTriangleFaces;
        }

        private void SortGeometry(ref List<TriangleFace> triangleFaces, ref List<Vector3> verticies,
            ref List<Vector3> normals, ref List<TextureCoordinates> textureCoordinates)
        {
            // Sort triangles first
            triangleFaces.Sort();

            // Reorder the verticies / texcoords / normals / to match the sorted triangle faces
            List<Vector3> sortedVerticies = new List<Vector3>();
            List<Vector3> sortedNormals = new List<Vector3>();
            List<TextureCoordinates> sortedTextureCoordinates = new List<TextureCoordinates>();
            List<TriangleFace> sortedTriangleFaces = new List<TriangleFace>();
            Dictionary<int, int> oldNewVertexIndicies = new Dictionary<int, int>();
            for (int i = 0; i < triangleFaces.Count; i++)
            {
                TriangleFace curTriangleFace = triangleFaces[i];

                // Face vertex 1
                if (oldNewVertexIndicies.ContainsKey(curTriangleFace.V1))
                {
                    // This index was aready remapped
                    curTriangleFace.V1 = oldNewVertexIndicies[curTriangleFace.V1];
                }
                else
                {
                    // Store new mapping
                    int oldVertIndex = curTriangleFace.V1;
                    int newVertIndex = sortedVerticies.Count;
                    oldNewVertexIndicies.Add(oldVertIndex, newVertIndex);
                    curTriangleFace.V1 = newVertIndex;

                    // Add verticies
                    sortedVerticies.Add(verticies[oldVertIndex]);
                    sortedNormals.Add(normals[oldVertIndex]);
                    sortedTextureCoordinates.Add(textureCoordinates[oldVertIndex]);
                }

                // Face vertex 2
                if (oldNewVertexIndicies.ContainsKey(curTriangleFace.V2))
                {
                    // This index was aready remapped
                    curTriangleFace.V2 = oldNewVertexIndicies[curTriangleFace.V2];
                }
                else
                {
                    // Store new mapping
                    int oldVertIndex = curTriangleFace.V2;
                    int newVertIndex = sortedVerticies.Count;
                    oldNewVertexIndicies.Add(oldVertIndex, newVertIndex);
                    curTriangleFace.V2 = newVertIndex;

                    // Add verticies
                    sortedVerticies.Add(verticies[oldVertIndex]);
                    sortedNormals.Add(normals[oldVertIndex]);
                    sortedTextureCoordinates.Add(textureCoordinates[oldVertIndex]);
                }

                // Face vertex 3
                if (oldNewVertexIndicies.ContainsKey(curTriangleFace.V3))
                {
                    // This index was aready remapped
                    curTriangleFace.V3 = oldNewVertexIndicies[curTriangleFace.V3];
                }
                else
                {
                    // Store new mapping
                    int oldVertIndex = curTriangleFace.V3;
                    int newVertIndex = sortedVerticies.Count;
                    oldNewVertexIndicies.Add(oldVertIndex, newVertIndex);
                    curTriangleFace.V3 = newVertIndex;

                    // Add verticies
                    sortedVerticies.Add(verticies[oldVertIndex]);
                    sortedNormals.Add(normals[oldVertIndex]);
                    sortedTextureCoordinates.Add(textureCoordinates[oldVertIndex]);
                }

                // Save this updated triangle
                sortedTriangleFaces.Add(curTriangleFace);
            }

            // Save the sorted values
            triangleFaces = sortedTriangleFaces;
            verticies = sortedVerticies;
            normals = sortedNormals;
            textureCoordinates = sortedTextureCoordinates;
        }

        private void CalculateBoundingBoxesAndRadii()
        {
            BoundingBox = BoundingBox.GenerateBoxFromVectors(ModelVerticies, Configuration.CONFIG_STATIC_OBJECT_MIN_BOUNDING_BOX_SIZE);
            BoundingSphereRadius = BoundingBox.FurthestPointDistanceFromCenter();
            CollisionBoundingBox = BoundingBox.GenerateBoxFromVectors(CollisionPositions, Configuration.CONFIG_EQTOWOW_ADDED_BOUNDARY_AMOUNT);
            CollisionSphereRaidus = CollisionBoundingBox.FurthestPointDistanceFromCenter();
        }
    }
}
