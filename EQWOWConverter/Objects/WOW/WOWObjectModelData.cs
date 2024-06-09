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
        public List<UInt16> ModelTextureTransparencyLookups = new List<UInt16>();
        public SortedDictionary<int, ModelTrackSequences<Fixed16>> ModelTextureTransparencySequenceSetByMaterialIndex = new SortedDictionary<int, ModelTrackSequences<Fixed16>>();
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

        // TODO: Vertex Colors
        public void Load(string name, List<Material> initialMaterials, List<TriangleFace> triangleFaces, List<Vector3> verticies,
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
            {
                ApplyEQToWoWGeometryTranslations(ref triangleFaces, ref verticies, ref textureCoordinates);
                List<TextureCoordinates> discardCoordinates = new List<TextureCoordinates>();
                ApplyEQToWoWGeometryTranslations(ref collisionTriangleFaces, ref collisionVerticies, ref discardCoordinates);
            }

            // Process materials
            List<Material> expandedMaterials = new List<Material>();
            foreach (Material material in initialMaterials)
                expandedMaterials.Add(new Material(material));
            foreach(Material material in initialMaterials)
            {
                // If animated, expand out into additional materials with additional geometry
                if (material.IsAnimated() == true)
                {
                    if (material.TextureNames.Count <= 1)
                    {
                        Logger.WriteError("Material '" + material.Name + "' in object '" + Name + "' was marked as animated, but had only one texture.");
                        return;
                    }

                    // Build the new materials and animation properties for this material
                    List<Material> curAnimationMaterials;
                    ExpandAnimatedMaterialAndAddAnimationProperties(material, ref expandedMaterials, out curAnimationMaterials);

                    // Add the additional geometry for the new frames
                    AddGeometryForExpandedMaterialFrames(curAnimationMaterials, ref triangleFaces, ref verticies, ref normals, ref textureCoordinates);
                }
                // If static, build single-frame animation properties
                else
                {
                    // Make a 'blank' animation for this material/texture, since it's static
                    ModelTextureTransparencySequenceSetByMaterialIndex[Convert.ToInt32(material.Index)] = new ModelTrackSequences<Fixed16>();
                    ModelTextureTransparencySequenceSetByMaterialIndex[Convert.ToInt32(material.Index)].AddSequence();
                    ModelTextureTransparencySequenceSetByMaterialIndex[Convert.ToInt32(material.Index)].AddValueToSequence(0, 0, new Fixed16(32767));
                    ModelTextureTransparencyLookups.Add(Convert.ToUInt16(ModelTextureTransparencyLookups.Count));
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
            BuildModelMaterialsFromMaterials(expandedMaterials.ToArray());

            // Correct any texture coordinates
            CorrectTextureCoordinates();

            // Process the rest
            ProcessCollisionData(collisionVerticies, collisionTriangleFaces);
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

        public void ApplyEQToWoWGeometryTranslations(ref List<TriangleFace> triangleFaces, ref List<Vector3> verticies,
            ref List<TextureCoordinates> textureCoordinates)
        {
            // Change face indices for winding over differences
            foreach (TriangleFace eqFace in triangleFaces)
            {
                int swapFaceVertexIndex = eqFace.V3;
                eqFace.V3 = eqFace.V1;
                eqFace.V1 = swapFaceVertexIndex;
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

        private void BuildModelMaterialsFromMaterials(params Material[] materials)
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

        private void ProcessCollisionData(List<Vector3> collisionVerticies, List<TriangleFace> collisionTriangleFaces)
        {
            // Purge prior data
            CollisionPositions.Clear();
            CollisionFaceNormals.Clear();
            CollisionTriangles.Clear();

            // Store positions, factoring for world scailing and rotation around Z axis
            foreach (Vector3 collisionVertex in collisionVerticies)
                CollisionPositions.Add(new Vector3(collisionVertex));

            // Store triangle indicies, ignoring 'blank' ones that have the same value 3x
            foreach (TriangleFace collisionTriangle in collisionTriangleFaces)
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
                //Vector3 normal = new Vector3(normalizedNormalSystem.X * -1, normalizedNormalSystem.Y * -1, normalizedNormalSystem.Z * -1);
                Vector3 normal = new Vector3(normalizedNormalSystem.X, normalizedNormalSystem.Y, normalizedNormalSystem.Z);
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

        private void ExpandAnimatedMaterialAndAddAnimationProperties(Material initialMaterial, ref List<Material> expandedMaterials, 
            out List<Material> curAnimationMaterials)
        {
            // Create a unique material and animation frame for every material
            curAnimationMaterials = new List<Material>();
            UInt32 curAnimationTimestamp = 0;
            for (int textureIter = 0; textureIter < initialMaterial.TextureNames.Count; ++textureIter)
            {
                // Update the material values if it's the first in the chain, otherwise create a new one
                string curMaterialName = initialMaterial.Name + "Anim_" + textureIter;
                Material curMaterial;
                int curMaterialIndex;
                if (textureIter == 0)
                {
                    initialMaterial.Name = curMaterialName;
                    curMaterial = initialMaterial;
                    curMaterialIndex = Convert.ToInt32(initialMaterial.Index);
                }
                else
                {
                    UInt32 newMaterialIndex = GetUniqueMaterialIDFromMaterials(expandedMaterials);
                    List<string> newMaterialTextureName = new List<string>() { initialMaterial.TextureNames[textureIter] };
                    Material newAnimationMaterial = new Material(curMaterialName, newMaterialIndex, initialMaterial.MaterialType, newMaterialTextureName,
                        initialMaterial.AnimationDelayMs, initialMaterial.TextureWidth, initialMaterial.TextureHeight);
                    curMaterial = newAnimationMaterial;
                    expandedMaterials.Add(curMaterial);
                    curMaterialIndex = Convert.ToInt32(newMaterialIndex);
                }

                // Create the new transparency animation for this frame
                ModelTrackSequences<Fixed16> newAnimation = new ModelTrackSequences<Fixed16>();
                newAnimation.InterpolationType = ModelAnimationInterpolationType.None;
                newAnimation.GlobalSequenceID = Convert.ToUInt16(GlobalLoopSequenceLimits.Count);
                int curSequenceId = newAnimation.AddSequence();

                // Add a blank (transparent) frame to this animation for every frame that already exists, and add a blank to those others
                for (int i = 0; i < curAnimationMaterials.Count; ++i)
                {
                    newAnimation.AddValueToSequence(0, Convert.ToUInt32(i) * initialMaterial.AnimationDelayMs, new Fixed16(0));
                    ModelTextureTransparencySequenceSetByMaterialIndex[Convert.ToInt32(curAnimationMaterials[i].Index)].AddValueToSequence(0, curAnimationTimestamp, new Fixed16(0));
                }

                // Add this shown (non-transparent) frame
                newAnimation.AddValueToSequence(0, curAnimationTimestamp, new Fixed16(Int16.MaxValue));

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

        private void AddGeometryForExpandedMaterialFrames(List<Material> frameMaterials, ref List<TriangleFace> triangleFaces,
            ref List<Vector3> verticies, ref List<Vector3> normals, ref List<TextureCoordinates> textureCoordinates)
        {
            for (int i = 1; i < frameMaterials.Count; i++)
            {
                // Create new triangles
                List<TriangleFace> newTriangleFaces = new List<TriangleFace>();

                // Determine what the min vertex index is for the triangles, as well as capture the reference indicies for vertex copies
                int minSourceTriangleVertexIndex = -1;
                int maxSourceTriangleVertexIndex = -1;
                foreach (TriangleFace triangleFace in triangleFaces)
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
                    Logger.WriteError("Could not find any triangle face verticies for material '" + frameMaterials[0].Name + "' in object '" + Name + "'");
                    return;
                }

                // Create new triangles using the min identified earlier
                int newVertexIndexStartOffsetAdd = verticies.Count - minSourceTriangleVertexIndex;
                foreach (TriangleFace triangleFace in triangleFaces)
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
                    triangleFaces.Add(triangleFace);

                // Create new geometry data                        
                for (int vi = minSourceTriangleVertexIndex; vi <= maxSourceTriangleVertexIndex; ++vi)
                {
                    verticies.Add(new Vector3(verticies[vi]));
                    normals.Add(new Vector3(normals[vi]));
                    textureCoordinates.Add(new TextureCoordinates(textureCoordinates[vi]));
                }
            }
        }

        private UInt32 GetUniqueMaterialIDFromMaterials(List<Material> materials)
        {
            UInt32 highestExistingID = 0;
            foreach(Material material in materials)
                if (material.Index > highestExistingID)
                    highestExistingID = material.Index;
            return highestExistingID+1;
        }
    }
}
