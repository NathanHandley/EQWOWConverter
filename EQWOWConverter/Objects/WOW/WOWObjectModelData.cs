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
        public List<Int16> ModelTextureTransparencyWeightsLookups = new List<Int16>();
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
            ModelTextureTransparencyWeightsLookups.Add(0);
            ModelTextureTransparencySequencesSet.Add(new ModelTrackSequences<Fixed16>());
            ModelTextureTransparencySequencesSet[0].AddValueToSequence(ModelTextureTransparencySequencesSet[0].AddSequence(), 0, new Fixed16(32767));
            ModelTextureTransparencySequencesSet.Add(new ModelTrackSequences<Fixed16>());
            ModelTextureTransparencySequencesSet[1].AddValueToSequence(ModelTextureTransparencySequencesSet[1].AddSequence(), 0, new Fixed16(32767));
            ModelReplaceableTextureLookups.Add(-1); // No replace lookup

            // Make one animation
            ModelAnimations.Add(new ModelAnimation());
            ModelAnimations[0].BoundingBox = new BoundingBox(BoundingBox);
            ModelAnimations[0].BoundingRadius = BoundingSphereRadius;
            //-------------------------------------------------------------------------------------------
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

        public void LoadFromZoneAnimatedMaterial(string name, Material material, List<TriangleFace> triangleFaces, List<Vector3> verticies, 
            List<Vector3> normals, List<ColorRGBA> vertexColors, List<TextureCoordinates> textureCoordinates)
        {
            // Save name and triangles
            Name = name;
            foreach(TriangleFace triangleFace  in triangleFaces)
                ModelTriangles.Add(triangleFace);

            // Generate modelverticies from geometry data
            if (verticies.Count != textureCoordinates.Count && verticies.Count != normals.Count)
            {
                Logger.WriteError("Failed to load wowobject from zone data since vertex count doesn't match texture coordinate count or normal count");
                return;
            }
            for (int i = 0; i < verticies.Count; i++)
            {
                ModelVertex newModelVertex = new ModelVertex();
                newModelVertex.Position.X = verticies[i].X;
                newModelVertex.Position.Y = verticies[i].Y;
                newModelVertex.Position.Z = verticies[i].Z;
                newModelVertex.Normal.X = normals[i].X;
                newModelVertex.Normal.Y = normals[i].Y;
                newModelVertex.Normal.Z = normals[i].Z;
                TextureCoordinates correctedCoordinates = material.GetCorrectedBaseCoordinates(textureCoordinates[i]);
                newModelVertex.Texture1TextureCoordinates.Y = correctedCoordinates.Y;
                newModelVertex.Texture1TextureCoordinates.X = correctedCoordinates.X;
                ModelVerticies.Add(newModelVertex);
            }

            // Process material
            ProcessMaterials(material);

            // Correct coordinates
            CorrectTextureCoordinates();

            // Build the bounding box (and no collision)
            CalculateBoundingBoxesAndRadii();

            // HARD CODED FOR STATIC --------------------------------------------------------------------
            // Create a base bone
            //AnimationSequenceIDLookups.Add(0); // Maps animations to the IDs in AnimationData.dbc - None for static
            ModelBones.Add(new ModelBone());
            ModelBoneKeyLookups.Add(-1);
            ModelBoneLookups.Add(0);
            ModelTextureTransparencyWeightsLookups.Add(0);
            ModelTextureTransparencySequencesSet.Add(new ModelTrackSequences<Fixed16>());
            ModelTextureTransparencySequencesSet[0].AddValueToSequence(ModelTextureTransparencySequencesSet[0].AddSequence(), 0, new Fixed16(32767));
            ModelTextureTransparencySequencesSet.Add(new ModelTrackSequences<Fixed16>());
            ModelTextureTransparencySequencesSet[1].AddValueToSequence(ModelTextureTransparencySequencesSet[1].AddSequence(), 0, new Fixed16(32767));
            ModelReplaceableTextureLookups.Add(-1); // No replace lookup

            // Make one animation
            ModelAnimations.Add(new ModelAnimation());
            ModelAnimations[0].BoundingBox = new BoundingBox(BoundingBox);
            ModelAnimations[0].BoundingRadius = BoundingSphereRadius;
            //-------------------------------------------------------------------------------------------
        }

        MaterialAnimationType DetermineMaterialAnimationType(Material material)
        {
            if (material.IsAnimated() == false)
                return MaterialAnimationType.None;

            // If there are oversized texture coordinates, it can only be handled by a texture transpacy animation
            for (int i = 0; i < ModelTriangles.Count; ++i)
            {
                // Save this triangle index if any of the texture coordinates are oversized
                if (ModelVerticies[ModelTriangles[i].V1].HasOversizedTexture1TextureCoordinates()
                    || ModelVerticies[ModelTriangles[i].V2].HasOversizedTexture1TextureCoordinates()
                    || ModelVerticies[ModelTriangles[i].V3].HasOversizedTexture1TextureCoordinates())
                {
                    return MaterialAnimationType.TextureTransparency;
                }
            }

            // If none are oversized, then transform is best
            return MaterialAnimationType.TextureTransform;
        }

        private void ProcessMaterials(params Material[] materials)
        {
            // Generate a model material per material
            Int16 curIndex = 0;
            foreach (Material material in materials)
            {
                if (material.TextureName != string.Empty)
                {
                    ModelTexture newModelTexture = new ModelTexture();
                    newModelTexture.TextureName = material.TextureName;
                    ModelTextures.Add(newModelTexture);
                    ModelMaterial newModelMaterial;
                    material.TextureAnimationType = DetermineMaterialAnimationType(material);
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

                    // Make animation frames if this is an animated texture by texture transform
                    if (material.TextureAnimationType == MaterialAnimationType.TextureTransform)
                    {
                        ModelTextureAnimation newAnimation = new ModelTextureAnimation();
                        newAnimation.TranslationTrack.InterpolationType = ModelAnimationInterpolationType.None;
                        int curSequenceID = newAnimation.TranslationTrack.AddSequence();

                        // For each frame, add a frame in the animation array
                        for(int i = 0; i < material.NumOfAnimationFrames(); ++i)
                        {
                            // Animations are spanning on the u (x)
                            uint curTimestamp = Convert.ToUInt32(i) * material.AnimationDelayMs;
                            Vector3 curTranslation = material.GetTranslationForAnimationFrame(i);
                            newAnimation.TranslationTrack.AddValueToSequence(curSequenceID, curTimestamp, curTranslation);
                        }

                        // Save the anim and reference the texture
                        GlobalLoopSequenceLimits.Add(Convert.ToUInt32(material.NumOfAnimationFrames()) * material.AnimationDelayMs);
                        newAnimation.TranslationTrack.GlobalSequenceID = Convert.ToUInt16(GlobalLoopSequenceLimits.Count - 1);
                        ModelTextureAnimations.Add(newAnimation);                        
                        ModelTextureAnimationLookup.Add(Convert.ToInt16(ModelTextureAnimations.Count - 1));
                    }
                    // No animation setting
                    else if (material.TextureAnimationType == MaterialAnimationType.None)
                    {
                        ModelTextureAnimationLookup.Add(-1);
                    }
                    else
                    {
                        Logger.WriteError("For object '" + Name + "' material '" + newModelMaterial.Material.Name + "' there is an unhandled texture animation type of '" + newModelMaterial.Material.TextureAnimationType .ToString() + "'");
                        ModelTextureAnimationLookup.Add(-1);
                    }

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

        private void CalculateBoundingBoxesAndRadii()
        {
            BoundingBox = BoundingBox.GenerateBoxFromVectors(ModelVerticies, Configuration.CONFIG_STATIC_OBJECT_MIN_BOUNDING_BOX_SIZE);
            BoundingSphereRadius = BoundingBox.FurthestPointDistanceFromCenter();
            CollisionBoundingBox = BoundingBox.GenerateBoxFromVectors(CollisionPositions, Configuration.CONFIG_EQTOWOW_ADDED_BOUNDARY_AMOUNT);
            CollisionSphereRaidus = CollisionBoundingBox.FurthestPointDistanceFromCenter();
        }
    }
}
