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

// Nathan: Lots of as-is Claude notes in here, as there was a lot of nuances in the model conversion for player models.  Meant to be read in combination with ObjectModelCharacterComposite

using EQWOWConverter.Common;
using EQWOWConverter.Creatures;
using EQWOWConverter.EQFiles;
using System.Collections.Concurrent;

namespace EQWOWConverter.ObjectModels
{
    internal class ObjectModelEQData
    {
        private static readonly ConcurrentDictionary<string, EQMesh> CachedRenderMeshByFileName = new ConcurrentDictionary<string, EQMesh>();
        private static readonly ConcurrentDictionary<string, EQSkeleton> CachedSkeletonDataByFileName = new ConcurrentDictionary<string, EQSkeleton>();
        private static readonly ConcurrentDictionary<string, EQParticleCloud> CachedParticleCloudDataByFileName = new ConcurrentDictionary<string, EQParticleCloud>();
        private static readonly ConcurrentDictionary<string, EQMaterialList> CachedMaterialListByKey = new ConcurrentDictionary<string, EQMaterialList>(); // Keyed by <file>|<customMaterialListLine>
        private static readonly ConcurrentDictionary<string, EQMesh> CachedCollisionMeshByFileName = new ConcurrentDictionary<string, EQMesh>();

        public MeshData MeshData = new MeshData();
        public List<Material> Materials = new List<Material>();
        public ObjectModelCharacterComposite? CharacterComposite = null;
        public List<Vector3> CollisionVertices = new List<Vector3>();
        public Dictionary<string, Animation> Animations = new Dictionary<string, Animation>();
        public List<TriangleFace> CollisionTriangleFaces = new List<TriangleFace>();
        private string MaterialListFileName = string.Empty;
        public EQSkeleton SkeletonData = new EQSkeleton();
        public Dictionary<string, EQParticleCloud> ParticleCloudsByName = new Dictionary<string, EQParticleCloud>();

        public void LoadObjectDataFromDisk(string name, ObjectModelProperties objectProperties, string eqInputObjectFileName, string inputObjectFolder, CreatureModelTemplate? creatureModelTemplate = null)
        {
            if (Directory.Exists(inputObjectFolder) == false)
            {
                Logger.WriteError("- [" + name + "]: Error - Could not find path at '" + inputObjectFolder + "'");
                return;
            }

            // Load skeleton, if possible
            string skeletonFileName = Path.Combine(inputObjectFolder, "Skeletons", eqInputObjectFileName + ".txt");
            if (File.Exists(skeletonFileName))
                LoadSkeletonData(eqInputObjectFileName, inputObjectFolder);

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
                if (meshNames.Count == 0 && eqInputObjectFileName.ToLower() == "eye")
                    meshNames.Add("eye");

                // For robe-capable races, swap the chest geometry
                if (creatureModelTemplate.TextureIndex >= 10 && creatureModelTemplate.TextureIndex < 24 && (raceID == 1 || raceID == 3 || raceID == 5 || raceID == 6 || raceID == 12 || raceID == 128))
                {
                    meshNames.Remove(eqInputObjectFileName.ToLower());
                    meshNames.Add(string.Concat(eqInputObjectFileName.ToLower(), "01"));
                }

                // Load the render meshes.  Player character versions also track which mesh each vertex came from, for the composite mapping, and carry the helmed head meshes too (as hair style geosets an NPC display or a worn helm can select)
                List<byte>? characterMeshContextByVertexIndex = null;
                if (creatureModelTemplate.IsPlayerCharacterVersion == true)
                {
                    characterMeshContextByVertexIndex = new List<byte>();
                    foreach (string secondaryMeshName in SkeletonData.SecondaryMeshNames)
                    {
                        string secondaryMeshNameLower = secondaryMeshName.ToLower();
                        // Only the three helmed heads (he01-he03).  Some races carry further head variants (gnome he04 is a mage cap) which have no hair style geoset of their own and would otherwise merge into the bare head
                        bool isHelmedHeadMesh = secondaryMeshNameLower.EndsWith("he01") || secondaryMeshNameLower.EndsWith("he02") || secondaryMeshNameLower.EndsWith("he03");
                        if (secondaryMeshNameLower.StartsWith(eqInputObjectFileName.ToLower() + "he0") == true && isHelmedHeadMesh == true && meshNames.Contains(secondaryMeshName) == false)
                            meshNames.Add(secondaryMeshName);
                    }
                }
                Dictionary<string, byte> meshNamesInDictionary = new Dictionary<string, byte>();
                foreach (string meshName in meshNames)
                {
                    byte meshBoneIndex = 0;
                    for (byte i = 0; i < SkeletonData.BoneStructures.Count; i++)
                    {
                        if (SkeletonData.BoneStructures[i].MeshName == meshName)
                        {
                            meshBoneIndex = i;
                            break;
                        }
                    }
                    meshNamesInDictionary.Add(meshName, meshBoneIndex);
                }
                LoadRenderMeshData(name, meshNamesInDictionary, inputObjectFolder, characterMeshContextByVertexIndex);

                // Load the materials, with special logic for invisible man
                if (creatureModelTemplate.Race.ID == 127)
                {
                    EQMaterialList materialListData = new EQMaterialList();
                    materialListData.LoadForInvisibleMan();
                    Materials = materialListData.MaterialsByTextureVariation[0];
                }
                else
                    LoadMaterialDataFromDisk(MaterialListFileName, inputObjectFolder, creatureModelTemplate.TextureIndex, objectProperties.CustomMaterialListLine);

                // For multi-face races, swap the faces
                if (creatureModelTemplate.FaceIndex > 0 && (raceID < 13 || raceID == 70 || raceID == 128 || raceID == 130))
                {
                    string faceTextureStart = string.Concat(eqInputObjectFileName, "he00");
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
                if (creatureModelTemplate.TextureIndex >= 10 && creatureModelTemplate.TextureIndex < 24 && (raceID == 1 || raceID == 3 || raceID == 5 || raceID == 6 || raceID == 12 || raceID == 128))
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
                        string replaceFromText = string.Concat("clk", eqInputObjectFileName.ToLower(), "06");
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

                // Player character versions get the robe skirt geometry, mixed-mesh material splitting, and the composite texture mapping
                if (creatureModelTemplate.IsPlayerCharacterVersion == true && characterMeshContextByVertexIndex != null)
                {
                    if (raceID == 1 || raceID == 3 || raceID == 5 || raceID == 6 || raceID == 12 || raceID == 128)
                        AppendRobeMeshData(name, eqInputObjectFileName.ToLower(), inputObjectFolder, characterMeshContextByVertexIndex);
                    InsetWholeTextureCoordinates();
                    Dictionary<int, float[]> seamPhaseByMaterialIndex = CalculateTileSeamPhases(eqInputObjectFileName.ToLower());
                    SplitTrianglesAtUVTileBoundaries(eqInputObjectFileName.ToLower(), characterMeshContextByVertexIndex, seamPhaseByMaterialIndex);
                    FoldMeshUVsPerTriangle(characterMeshContextByVertexIndex, seamPhaseByMaterialIndex);
                    SplitMaterialsSpanningMeshContexts(name, characterMeshContextByVertexIndex);
                    CharacterComposite = ObjectModelCharacterComposite.BuildForCharacterModel(eqInputObjectFileName, MeshData, Materials,
                        SkeletonData, characterMeshContextByVertexIndex);
                }

                // Load the rest
                string animationSupplimentName = string.Empty;
                if (creatureModelTemplate != null)
                    animationSupplimentName = creatureModelTemplate.Race.Skeleton2Name;

                LoadAnimationData(name, eqInputObjectFileName, inputObjectFolder, animationSupplimentName);

                // Load collision
                LoadCollisionMeshData(name, meshNamesInDictionary.Keys.ToList(), inputObjectFolder);
            }
            else
            {
                // Load render meshes....
                Dictionary<string, byte> meshBoneIndexByName = new Dictionary<string, byte>();
                for (byte i = 0; i < SkeletonData.BoneStructures.Count; i++)
                {
                    if (SkeletonData.BoneStructures[i].MeshName != string.Empty)
                    {
                        if (SkeletonData.BoneStructures[i].AlternateMeshName != string.Empty)
                            Logger.WriteError("- [" + name + "]: Error - A bone had both a mesh and alternate mesh, so alternate is discarded");
                        meshBoneIndexByName.Add(SkeletonData.BoneStructures[i].MeshName, i);
                    }
                    if (SkeletonData.BoneStructures[i].AlternateMeshName != string.Empty)
                    {
                        meshBoneIndexByName.Add(SkeletonData.BoneStructures[i].AlternateMeshName, i);
                    }
                }
                foreach (string meshName in SkeletonData.MeshNames)
                    if (meshBoneIndexByName.ContainsKey(meshName) == false)
                        meshBoneIndexByName.Add(meshName, 0);
                foreach (string meshName in SkeletonData.SecondaryMeshNames)
                    if (meshBoneIndexByName.ContainsKey(meshName) == false)
                        meshBoneIndexByName.Add(meshName, 0);
                if (meshBoneIndexByName.Count == 0)
                    meshBoneIndexByName.Add(eqInputObjectFileName, 0);
                LoadRenderMeshData(name, meshBoneIndexByName, inputObjectFolder);

                // Load Materials
                LoadMaterialDataFromDisk(MaterialListFileName, inputObjectFolder, 0, objectProperties.CustomMaterialListLine);

                // Load the rest
                // If this object uses animated vertices, then it should have a skeleton and animation generated
                if (MeshData.AnimatedVertexFramesByVertexIndex.Count > 0)
                {
                    ConvertAnimatedVerticesToSkeleton(name);
                    GenerateAnimationFromAnimatedVertexSkeleton(name, MeshData);
                }
                else
                    LoadAnimationData(name, eqInputObjectFileName, inputObjectFolder, string.Empty);

                // Load collision
                LoadCollisionMeshData(name, meshBoneIndexByName.Keys.ToList(), inputObjectFolder);
            }

            // Load any particle clouds
            HashSet<string> particleCloudNames = new HashSet<string>();
            foreach (EQSkeleton.EQSkeletonBone bone in SkeletonData.BoneStructures)
                if (bone.ParticleCloudName.Length > 0)
                    particleCloudNames.Add(bone.ParticleCloudName);
            LoadParticleCloudData(name, particleCloudNames.ToList(), inputObjectFolder);
        }

        private void LoadRenderMeshData(string inputObjectName, Dictionary<string, byte> meshNamesByBoneIndex, string inputObjectFolder,  List<byte>? meshContextByVertexIndexOut = null)
        {
            Logger.WriteDebug("- [" + inputObjectName + "]: Reading render mesh data...");
            foreach (var meshNameByBoneIndex in meshNamesByBoneIndex)
            {
                // Load this mesh, if needed
                string renderMeshFileName = Path.Combine(inputObjectFolder, "Meshes", meshNameByBoneIndex.Key + ".txt");
                EQMesh eqMeshData = new EQMesh();
                if (CachedRenderMeshByFileName.TryGetValue(renderMeshFileName, out EQMesh? cachedRenderMesh) == true)
                    eqMeshData = new EQMesh(cachedRenderMesh);
                else if (eqMeshData.LoadFromDisk(renderMeshFileName) == true)
                    CachedRenderMeshByFileName.TryAdd(renderMeshFileName, new EQMesh(eqMeshData));
                else
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

                // Track what mesh these vertices came from, if requested (used by player character versions)
                if (meshContextByVertexIndexOut != null)
                {
                    byte meshContext = ObjectModelCharacterComposite.MESH_CONTEXT_BODY;
                    string meshNameLower = meshNameByBoneIndex.Key.ToLower();
                    int helmMarkerIndex = meshNameLower.LastIndexOf("he0");
                    if (helmMarkerIndex >= 0 && helmMarkerIndex + 3 < meshNameLower.Length && char.IsDigit(meshNameLower[helmMarkerIndex + 3]) == true)
                    {
                        int helmIndex = meshNameLower[helmMarkerIndex + 3] - '0';
                        if (helmIndex >= 1 && helmIndex <= 3)
                            meshContext = Convert.ToByte(ObjectModelCharacterComposite.MESH_CONTEXT_HELM_HEAD_BASE + helmIndex);
                        else
                            meshContext = ObjectModelCharacterComposite.MESH_CONTEXT_HEAD;
                    }
                    for (int i = 0; i < eqMeshData.Meshdata.Vertices.Count; i++)
                        meshContextByVertexIndexOut.Add(meshContext);
                }

                // Save material list
                if (MaterialListFileName == string.Empty)
                    MaterialListFileName = eqMeshData.MaterialListFileName;
            }
        }

        private void AppendRobeMeshData(string inputObjectName, string skeletonNameLower, string inputObjectFolder, List<byte> meshContextByVertexIndexOut)
        {
            string robeMeshName = string.Concat(skeletonNameLower, "01");
            string robeMeshFileName = Path.Combine(inputObjectFolder, "Meshes", robeMeshName + ".txt");
            if (File.Exists(robeMeshFileName) == false)
            {
                Logger.WriteDebug("- [" + inputObjectName + "]: No robe mesh found at '" + robeMeshFileName + "', so no robe geometry");
                return;
            }
            EQMesh robeMeshData = new EQMesh();
            if (CachedRenderMeshByFileName.TryGetValue(robeMeshFileName, out EQMesh? cachedRobeMesh) == true)
                robeMeshData = new EQMesh(cachedRobeMesh);
            else if (robeMeshData.LoadFromDisk(robeMeshFileName) == true)
                CachedRenderMeshByFileName.TryAdd(robeMeshFileName, new EQMesh(robeMeshData));
            else
            {
                Logger.WriteError("- [" + inputObjectName + "]: ERROR - Could not load robe mesh file at '" + robeMeshFileName + "'");
                return;
            }

            // Associate bone references, same as LoadRenderMeshData
            if (robeMeshData.Bones.Count > 0)
            {
                for (int i = 0; i < robeMeshData.Meshdata.Vertices.Count; i++)
                {
                    byte curBoneID = 0;
                    foreach (EQMesh.BoneReference bone in robeMeshData.Bones)
                    {
                        if (i >= bone.VertStart && (i < (bone.VertStart + bone.VertCount)))
                        {
                            curBoneID = bone.KeyBoneID;
                            break;
                        }
                    }
                    robeMeshData.Meshdata.BoneIDs.Add(curBoneID);
                }
            }
            else
            {
                for (int i = 0; i < robeMeshData.Meshdata.Vertices.Count; i++)
                    robeMeshData.Meshdata.BoneIDs.Add(0);
            }

            // Every material the robe mesh uses, its hands included, are shaped to meet the robe's sleeves (they reach further up the wrist than the base body's hands), so the robe shows them and hides the base hands
            HashSet<int> usedMaterialIndexes = new HashSet<int>();
            foreach (TriangleFace triangleFace in robeMeshData.Meshdata.TriangleFaces)
                usedMaterialIndexes.Add(triangleFace.MaterialIndex);
            List<Material> robeMaterials = new List<Material>();
            foreach (Material material in Materials)
            {
                if (material.TextureNames.Count == 0 || usedMaterialIndexes.Contains(Convert.ToInt32(material.Index)) == false)
                    continue;
                robeMaterials.Add(material);
            }
            if (robeMaterials.Count == 0)
            {
                Logger.WriteDebug("- [" + inputObjectName + "]: Robe mesh had no robe materials, so no robe geometry");
                return;
            }

            MeshData robeGeometry = robeMeshData.Meshdata.GetMeshDataForMaterials(robeMaterials.ToArray());
            MeshData.AddMeshData(robeGeometry);
            for (int i = 0; i < robeGeometry.Vertices.Count; i++)
                meshContextByVertexIndexOut.Add(ObjectModelCharacterComposite.MESH_CONTEXT_ROBE);
        }

        // A material can appear in more than one mesh context (body, head, helmed heads, robe) with different UV islands (example: hufhe0021
        // on the head and the body, the face texture on every head mesh, or the iksar forearm on the body and the robe mesh).  Each context
        // binds to its own composite layout and geoset, so the triangles of every context beyond the first get a cloned material
        // EQ pieces wrap their textures around the mesh: the gnome face texture repeats two and a half times around the head as one
        // connected surface.  Rendering wraps, but the composite bake needs a flat window, and a window several tiles wide only fits its
        // block shrunk (the gnome face landed at 62% of its texels).  Shifting every triangle by whole tiles onto one tile leaves each
        // triangle's mapping identical (a whole-tile shift samples the same texels) and collapses the repeats onto one copy of the art
        // (see FoldMeshUVsPerTriangle)
        // Cuts every triangle whose UVs cross a tile boundary along that boundary, so each triangle lies within one tile of its texture.
        // EQ meshes wrap a texture more than once (the barbarian legs run 1.4 tiles across and 1.9 down); the composite cannot wrap a
        // piece, so those tiles had to be unrolled into it (178x249 texels for a 128x128 texture), which no block can hold: the legs
        // packed at 39%.  Cut, every piece needs one tile.  A vertex made on a cut edge is shared by both sides (no crack).  A triangle
        // whose corners sit on different bones spans a joint and is never cut: its edges render as straight lines between the two
        // bones' transforms of the endpoints, and no vertex placed along such an edge can follow that line (blended between the bones
        // it bows outward as the joint bends, the dark elf's elbow grew a disc; on one bone it kinks).  Left whole, such a triangle
        // reaches past the tile and stretches the window (the high elf legs to 1.27 tiles, the robes to 1.2-1.3), so the tile boundary
        // itself is placed, per texture and axis, where no (or the fewest) joint-spanning triangles cross it: mid-thigh rather than at
        // the knee (see CalculateTileSeamPhases).  The fold that follows shifts each side onto the window's tile and duplicates the
        // cut vertices, and the composite's wrapped margins carry the art across the cut
        private const float UV_CUT_EPSILON = 1f / 4096f;

        // The old pre-baked models (the reference look) had every texture coordinate lying exactly on 0, 1 or -1 moved half a texel
        // inward (Material.GetCorrectedBaseCoordinates): a mesh edge on the texture's edge samples the edge texel itself rather than
        // blending it with the wrapped far column.  The gnome's sternum, mirrored at u = 1, is a dark groove that way and a bright
        // line without it.  Applied here to the mesh's own vertices only, before the tile cut: the cut vertices that follow must stay
        // exactly on the seam so the client's bilinear sample blends across it (an inset there hardened the half elf's bicep seam)
        private void InsetWholeTextureCoordinates()
        {
            if (MeshData.TextureCoordinates.Count != MeshData.Vertices.Count)
                return;
            Dictionary<int, Material> materialByIndex = new Dictionary<int, Material>();
            foreach (Material material in Materials)
                materialByIndex[Convert.ToInt32(material.Index)] = material;
            Dictionary<int, int> materialIndexByVertexIndex = new Dictionary<int, int>();
            foreach (TriangleFace face in MeshData.TriangleFaces)
            {
                if (materialIndexByVertexIndex.ContainsKey(face.V1) == false) materialIndexByVertexIndex[face.V1] = face.MaterialIndex;
                if (materialIndexByVertexIndex.ContainsKey(face.V2) == false) materialIndexByVertexIndex[face.V2] = face.MaterialIndex;
                if (materialIndexByVertexIndex.ContainsKey(face.V3) == false) materialIndexByVertexIndex[face.V3] = face.MaterialIndex;
            }
            int insetCount = 0;
            for (int i = 0; i < MeshData.TextureCoordinates.Count; i++)
            {
                int materialIndex;
                Material? material;
                if (materialIndexByVertexIndex.TryGetValue(i, out materialIndex) == false || materialByIndex.TryGetValue(materialIndex, out material) == false)
                    continue;
                float halfTexelU = 0.5f / Math.Max(1, material.TextureWidth);
                float halfTexelV = 0.5f / Math.Max(1, material.TextureHeight);
                TextureCoordinates uv = MeshData.TextureCoordinates[i];
                float u = InsetWholeCoordinate(uv.X, halfTexelU);
                float v = InsetWholeCoordinate(uv.Y, halfTexelV);
                if (u != uv.X || v != uv.Y)
                {
                    MeshData.TextureCoordinates[i] = new TextureCoordinates(u, v);
                    insetCount++;
                }
            }
            if (insetCount > 0)
                Logger.WriteDebug(string.Concat("Character model moved ", insetCount.ToString(), " texture coordinates on a whole number half a texel inward"));
        }

        private static float InsetWholeCoordinate(float value, float halfTexel)
        {
            if (value == 0f)
                return halfTexel;
            if (value == 1f)
                return 1f - halfTexel;
            if (value == -1f)
                return -1f + halfTexel;
            return value;
        }

        // The tile boundary ("seam") position on each axis, in tiles from the texture's own edge and on its texel grid, per material
        // index.  Materials sharing a texture share the seam (the composite gives them one window).  Of every texel position the one
        // whose folded window spans the fewest texels wins, then the one where the art is most continuous across the cut (a piece
        // packed without a margin shows the cut hard, see ObjectModelCharacterComposite.ApplyUVRemap), then the fewest cut triangles,
        // then the lowest position
        private Dictionary<int, float[]> CalculateTileSeamPhases(string skeletonName)
        {
            Dictionary<int, float[]> seamPhaseByMaterialIndex = new Dictionary<int, float[]>();
            if (MeshData.TextureCoordinates.Count != MeshData.Vertices.Count)
                return seamPhaseByMaterialIndex;
            Dictionary<int, Material> materialByIndex = new Dictionary<int, Material>();
            foreach (Material material in Materials)
                materialByIndex[Convert.ToInt32(material.Index)] = material;
            Dictionary<string, List<TriangleFace>> facesByTextureName = new Dictionary<string, List<TriangleFace>>();
            Dictionary<string, List<int>> materialIndexesByTextureName = new Dictionary<string, List<int>>();
            foreach (TriangleFace face in MeshData.TriangleFaces)
            {
                Material? material;
                if (materialByIndex.TryGetValue(face.MaterialIndex, out material) == false || material.TextureNames.Count == 0)
                    continue;
                if (face.V1 >= MeshData.Vertices.Count || face.V2 >= MeshData.Vertices.Count || face.V3 >= MeshData.Vertices.Count)
                    continue;
                string textureName = material.TextureNames[0].ToLower();
                if (facesByTextureName.ContainsKey(textureName) == false)
                {
                    facesByTextureName.Add(textureName, new List<TriangleFace>());
                    materialIndexesByTextureName.Add(textureName, new List<int>());
                }
                facesByTextureName[textureName].Add(face);
                if (materialIndexesByTextureName[textureName].Contains(face.MaterialIndex) == false)
                    materialIndexesByTextureName[textureName].Add(face.MaterialIndex);
            }
            foreach (var facesByTexture in facesByTextureName)
            {
                Material firstMaterial = materialByIndex[materialIndexesByTextureName[facesByTexture.Key][0]];
                int texelsU = firstMaterial.TextureWidth > 0 ? firstMaterial.TextureWidth : 64;
                int texelsV = firstMaterial.TextureHeight > 0 ? firstMaterial.TextureHeight : 64;
                float[] discontinuityByColumn;
                float[] discontinuityByRow;
                ObjectModelCharacterComposite.CalculateSeamDiscontinuity(facesByTexture.Key, skeletonName, texelsU, texelsV, out discontinuityByColumn, out discontinuityByRow);
                int windowTexelsU, windowTexelsV;
                float phaseU = ChooseSeamPhase(facesByTexture.Value, 0, texelsU, discontinuityByColumn, out windowTexelsU);
                float phaseV = ChooseSeamPhase(facesByTexture.Value, 1, texelsV, discontinuityByRow, out windowTexelsV);
                float[] phase = new float[6] { phaseU, phaseV, windowTexelsU, windowTexelsV, texelsU, texelsV };
                foreach (int materialIndex in materialIndexesByTextureName[facesByTexture.Key])
                {
                    seamPhaseByMaterialIndex[materialIndex] = phase;
                    materialByIndex[materialIndex].TileSeamPhaseU = phaseU;
                    materialByIndex[materialIndex].TileSeamPhaseV = phaseV;
                }
                Logger.WriteDebug(string.Concat("Character model '", skeletonName, "' texture '", facesByTexture.Key, "' seams at texel ", Convert.ToInt32(Math.Round(phaseU * texelsU)).ToString(),
                    " of ", texelsU.ToString(), " (window ", windowTexelsU.ToString(), " texels) by ", Convert.ToInt32(Math.Round(phaseV * texelsV)).ToString(), " of ", texelsV.ToString(),
                    " (window ", windowTexelsV.ToString(), " texels)"));
            }
            return seamPhaseByMaterialIndex;
        }

        private float CalculateFaceArea(TriangleFace face)
        {
            if (face.V1 >= MeshData.Vertices.Count || face.V2 >= MeshData.Vertices.Count || face.V3 >= MeshData.Vertices.Count)
                return 0f;
            Vector3 a = MeshData.Vertices[face.V1];
            Vector3 b = MeshData.Vertices[face.V2];
            Vector3 c = MeshData.Vertices[face.V3];
            double abx = b.X - a.X, aby = b.Y - a.Y, abz = b.Z - a.Z;
            double acx = c.X - a.X, acy = c.Y - a.Y, acz = c.Z - a.Z;
            double cx = (aby * acz) - (abz * acy);
            double cy = (abz * acx) - (abx * acz);
            double cz = (abx * acy) - (aby * acx);
            return Convert.ToSingle(0.5 * Math.Sqrt((cx * cx) + (cy * cy) + (cz * cz)));
        }

        // A triangle whose UVs span well over a tile on the axis: the texture repeats across it densely enough to average to a smear
        // (the barbarian kilt's side flap shows 1.5 repeats of the pleats, flat on the old models too).  Just over a tile (the high
        // elf's rear skirt panel at 1.09) the pattern still reads and the triangle stays in the main piece at full resolution
        private const float SMEAR_FACE_MIN_EXTENT_TILES = 1.25f;
        private bool IsSmearFace(TriangleFace face, int axis)
        {
            return GetFaceExtent(face, axis) >= SMEAR_FACE_MIN_EXTENT_TILES - UV_CUT_EPSILON;
        }

        private float GetFaceExtent(TriangleFace face, int axis)
        {
            float a = GetUVComponent(face.V1, axis);
            float b = GetUVComponent(face.V2, axis);
            float c = GetUVComponent(face.V3, axis);
            return Math.Max(a, Math.Max(b, c)) - Math.Min(a, Math.Min(b, c));
        }

        private bool IsJointSpanningFace(TriangleFace face)
        {
            int highestCorner = Math.Max(face.V1, Math.Max(face.V2, face.V3));
            if (MeshData.BoneIDs.Count <= highestCorner)
                return false;
            return MeshData.BoneIDs[face.V1] != MeshData.BoneIDs[face.V2] || MeshData.BoneIDs[face.V1] != MeshData.BoneIDs[face.V3];
        }

        // See CalculateTileSeamPhases.  The window each candidate seam gives is worked out the way the cut and the fold would leave it:
        // a triangle that gets cut fills its window from the first piece's start to the far edge and from the near edge to the last
        // piece's end; one left whole folds by its centroid and may reach past either edge
        private float ChooseSeamPhase(List<TriangleFace> faces, int axis, int texels, float[] discontinuityByTexel, out int bestWindowTexels)
        {
            List<float[]> faceRanges = new List<float[]>(); // min, max, centroid, 1 when the face may be cut
            foreach (TriangleFace face in faces)
            {
                float a = GetUVComponent(face.V1, axis);
                float b = GetUVComponent(face.V2, axis);
                float c = GetUVComponent(face.V3, axis);
                bool jointSpanning = IsJointSpanningFace(face);
                // A joint-spanning triangle that spans a whole tile or more on this axis always goes to its own piece (see
                // SplitSeamPieces), so it does not weigh on the seam's position
                if (jointSpanning == true && IsSmearFace(face, axis) == true)
                    continue;
                faceRanges.Add(new float[4] { Math.Min(a, Math.Min(b, c)), Math.Max(a, Math.Max(b, c)), (a + b + c) / 3f, jointSpanning ? 0f : 1f });
            }
            texels = Math.Max(1, texels);
            int bestTexel = 0;
            bestWindowTexels = int.MaxValue;
            float bestDiscontinuity = float.MaxValue;
            int bestCuts = int.MaxValue;
            for (int texel = 0; texel < texels; texel++)
            {
                float phase = Convert.ToSingle(texel) / texels;
                float lowest = float.MaxValue;
                float highest = float.MinValue;
                int cuts = 0;
                foreach (float[] range in faceRanges)
                {
                    float min = range[0] - phase;
                    float max = range[1] - phase;
                    float centroid = range[2] - phase;
                    int firstTile = Convert.ToInt32(Math.Floor(min + UV_CUT_EPSILON));
                    int lastTile = Convert.ToInt32(Math.Floor(max - UV_CUT_EPSILON));
                    if (range[3] > 0 && lastTile > firstTile)
                    {
                        cuts++;
                        lowest = Math.Min(lowest, Math.Min(min - firstTile, 0f));
                        highest = Math.Max(highest, Math.Max(max - lastTile, 1f));
                    }
                    else if (lastTile > firstTile)
                    {
                        // A joint-spanning crosser stays whole and folds to the side of its last boundary that leaves less of it
                        // outside the tile (see FoldMeshUVsPerTriangle)
                        float below = lastTile - min;
                        float above = max - lastTile;
                        if (above <= below)
                        {
                            lowest = Math.Min(lowest, min - (lastTile - 1));
                            highest = Math.Max(highest, 1f + above);
                        }
                        else
                        {
                            lowest = Math.Min(lowest, -below);
                            highest = Math.Max(highest, max - lastTile);
                        }
                    }
                    else
                    {
                        int shift = Convert.ToInt32(Math.Floor(centroid));
                        lowest = Math.Min(lowest, min - shift);
                        highest = Math.Max(highest, max - shift);
                    }
                }
                if (lowest == float.MaxValue)
                    continue;
                int windowTexels = Convert.ToInt32(Math.Ceiling((highest * texels) - 0.001f)) - Convert.ToInt32(Math.Floor((lowest * texels) + 0.001f));
                float discontinuity = (discontinuityByTexel != null && texel < discontinuityByTexel.Length) ? discontinuityByTexel[texel] : 0f;
                // Same window: the position cutting the fewest triangles wins (a mirrored or open axis, the gnome's chest or the human
                // chest's neck-to-belt axis, has a position that cuts nothing or nearly nothing, and must keep it: a sliver cut off the
                // centre line lands at the far edge and the open edges blend into each other), then the most continuous art
                bool better = windowTexels < bestWindowTexels;
                if (windowTexels == bestWindowTexels)
                {
                    if (cuts != bestCuts)
                        better = (cuts < bestCuts);
                    else if (discontinuity < bestDiscontinuity - 0.0001f)
                        better = true;
                }
                if (better == true)
                {
                    bestTexel = texel;
                    bestWindowTexels = windowTexels;
                    bestDiscontinuity = discontinuity;
                    bestCuts = cuts;
                }
            }
            if (bestWindowTexels == int.MaxValue)
                bestWindowTexels = 0;
            return Convert.ToSingle(bestTexel) / texels;
        }

        // Per material: seam phase U and V (tiles), the folded window in texels U and V, and the texture's texels U and V
        private static float[] GetSeamPhase(Dictionary<int, float[]> seamPhaseByMaterialIndex, int materialIndex)
        {
            float[]? phase;
            if (seamPhaseByMaterialIndex.TryGetValue(materialIndex, out phase) == true)
                return phase;
            return new float[6] { 0f, 0f, 64f, 64f, 64f, 64f };
        }

        // Suffix of the clone material holding a texture's joint-spanning triangles that cross its seam on one axis (see
        // SplitTrianglesAtUVTileBoundaries): 'u' or 'v' for the axis, so the composite folds and windows them on their own
        public const string SEAM_PIECE_MATERIAL_SUFFIX = "_seam";
        private const float SEAM_PIECE_MARGIN_TEXELS = 4f;      // The composite's edge margin, for costing a seam piece
        private const int SEAM_PIECE_MIN_OVERSHOOT_TEXELS = 4;  // Less overshoot than this stays in the main piece
        private const float SEAM_PIECE_REDUCED_MIN_OVERSHOOT_TILES = 0.2f; // A seam piece below full resolution only for this much overshoot
        private const float SEAM_PIECE_REDUCED_MAX_TRIANGLE_BODY_SHARE = 0.003f; // ... and only when no triangle covers more of the whole body than this
        private const float SEAM_PIECE_REDUCED_MAX_TOTAL_BODY_SHARE = 0.05f;     // ... nor all of them together more than this

        private void SplitTrianglesAtUVTileBoundaries(string skeletonName, List<byte> meshContextByVertexIndex, Dictionary<int, float[]> seamPhaseByMaterialIndex)
        {
            if (MeshData.TextureCoordinates.Count != MeshData.Vertices.Count)
                return;
            while (MeshData.SecondaryBoneIDs.Count < MeshData.Vertices.Count)
            {
                MeshData.SecondaryBoneIDs.Add(0);
                MeshData.SecondaryBoneWeights.Add(0);
            }
            Dictionary<string, int> cutVertexIndexByKey = new Dictionary<string, int>();
            List<TriangleFace> outputFaces = new List<TriangleFace>();
            int startingVertexCount = MeshData.Vertices.Count;
            int cutFaceCount = 0;
            int jointSpanningFaceCount = 0;
            List<int[]> jointCrossers = new List<int[]>(); // output face index, material index, crosses U (1/0), crosses V (1/0)
            foreach (TriangleFace face in MeshData.TriangleFaces)
            {
                int[] corners = new int[3] { face.V1, face.V2, face.V3 };
                if (corners[0] >= startingVertexCount || corners[1] >= startingVertexCount || corners[2] >= startingVertexCount)
                {
                    outputFaces.Add(face);
                    continue;
                }
                float minU = float.MaxValue, maxU = float.MinValue, minV = float.MaxValue, maxV = float.MinValue;
                foreach (int corner in corners)
                {
                    TextureCoordinates uv = MeshData.TextureCoordinates[corner];
                    minU = Math.Min(minU, uv.X); maxU = Math.Max(maxU, uv.X);
                    minV = Math.Min(minV, uv.Y); maxV = Math.Max(maxV, uv.Y);
                }
                float[] seamPhase = GetSeamPhase(seamPhaseByMaterialIndex, face.MaterialIndex);
                List<float> uBoundaries = GetCrossedTileBoundaries(minU, maxU, seamPhase[0]);
                List<float> vBoundaries = GetCrossedTileBoundaries(minV, maxV, seamPhase[1]);
                if (uBoundaries.Count == 0 && vBoundaries.Count == 0)
                {
                    outputFaces.Add(face);
                    continue;
                }
                if (IsJointSpanningFace(face) == true)
                {
                    jointCrossers.Add(new int[4] { outputFaces.Count, face.MaterialIndex, uBoundaries.Count > 0 ? 1 : 0, vBoundaries.Count > 0 ? 1 : 0 });
                    outputFaces.Add(face);
                    jointSpanningFaceCount++;
                    continue;
                }
                List<List<int>> polygons = new List<List<int>>();
                polygons.Add(new List<int>(corners));
                foreach (float boundary in uBoundaries)
                    polygons = SplitPolygonsAtTileBoundary(polygons, 0, boundary, cutVertexIndexByKey, meshContextByVertexIndex);
                foreach (float boundary in vBoundaries)
                    polygons = SplitPolygonsAtTileBoundary(polygons, 1, boundary, cutVertexIndexByKey, meshContextByVertexIndex);
                foreach (List<int> polygon in polygons)
                    for (int i = 1; i + 1 < polygon.Count; i++)
                        outputFaces.Add(new TriangleFace(face.MaterialIndex, polygon[0], polygon[i], polygon[i + 1]));
                cutFaceCount++;
            }
            MeshData.TriangleFaces = outputFaces;
            if (cutFaceCount > 0 || jointSpanningFaceCount > 0)
                Logger.WriteDebug(string.Concat("Character model tile cut split ", cutFaceCount.ToString(), " triangles crossing texture tile boundaries, adding ",
                    (MeshData.Vertices.Count - startingVertexCount).ToString(), " vertices; ", jointSpanningFaceCount.ToString(), " joint-spanning triangles left whole"));
            SplitSeamPieces(skeletonName, jointCrossers, seamPhaseByMaterialIndex);
        }

        // The joint-spanning triangles left whole across a seam still reach past the tile, stretching the window by the widest of
        // them on that axis: a band the full height of the piece for a handful of triangles at the joint rings (12 texels of a robe's
        // 128, 33 on the erudite's).  Where those triangles cluster (one or two per ring along a seam), they move to a clone of the
        // material, folded onto the seam so their window is small (the seam's neighbourhood by the rings' extent), and the main
        // piece is exactly one tile.  The move is made only where the clone's piece costs less room than the band it saves
        private void SplitSeamPieces(string skeletonName, List<int[]> jointCrossers, Dictionary<int, float[]> seamPhaseByMaterialIndex)
        {
            if (jointCrossers.Count == 0)
                return;
            // The robe hood ('clkerm06': robe cloth on the head) has one reserved corner in the composite, which a second piece of it
            // would share; it stays whole
            string hoodTexturePrefix = string.Concat("clk", skeletonName.ToLower());
            float bodySurface = 0f;
            foreach (TriangleFace face in MeshData.TriangleFaces)
                bodySurface += CalculateFaceArea(face);
            UInt32 nextMaterialIndex = 0;
            foreach (Material material in Materials)
                nextMaterialIndex = Math.Max(nextMaterialIndex, material.Index + 1);
            List<Material> seamMaterials = new List<Material>();
            HashSet<int> movedFaceIndexes = new HashSet<int>();

            // Smear triangles first (see IsSmearFace): no seam avoids a triangle that spans more than a tile, and left in the main piece
            // it alone stretched the barbarian's kilt window to 1.49 tiles.  Its repeats average to a smear on the old models too, so a
            // quarter resolution loses nothing
            for (int axis = 0; axis < 2; axis++)
            {
                Dictionary<int, List<int>> smearFaceIndexesByMaterialIndex = new Dictionary<int, List<int>>();
                foreach (int[] crosser in jointCrossers)
                {
                    if (movedFaceIndexes.Contains(crosser[0]) == true || IsSmearFace(MeshData.TriangleFaces[crosser[0]], axis) == false)
                        continue;
                    if (smearFaceIndexesByMaterialIndex.ContainsKey(crosser[1]) == false)
                        smearFaceIndexesByMaterialIndex.Add(crosser[1], new List<int>());
                    smearFaceIndexesByMaterialIndex[crosser[1]].Add(crosser[0]);
                }
                foreach (var smearFaceIndexes in smearFaceIndexesByMaterialIndex)
                {
                    Material? sourceMaterial = null;
                    foreach (Material material in Materials)
                        if (Convert.ToInt32(material.Index) == smearFaceIndexes.Key)
                            sourceMaterial = material;
                    if (sourceMaterial == null)
                        continue;
                    if (sourceMaterial.TextureNames.Count > 0 && sourceMaterial.TextureNames[0].ToLower().StartsWith(hoodTexturePrefix) == true)
                        continue;
                    float widestExtent = 0f;
                    foreach (int faceIndex in smearFaceIndexes.Value)
                        widestExtent = Math.Max(widestExtent, GetFaceExtent(MeshData.TriangleFaces[faceIndex], axis));
                    float pieceScale = 0.25f;
                    Material cloneMaterial = new Material(sourceMaterial);
                    cloneMaterial.Index = nextMaterialIndex;
                    nextMaterialIndex++;
                    string suffix = string.Concat(SEAM_PIECE_MATERIAL_SUFFIX, axis == 0 ? "u" : "v", Convert.ToInt32(Math.Round(pieceScale * 100f)).ToString());
                    cloneMaterial.UniqueName = string.Concat(sourceMaterial.UniqueName, suffix);
                    cloneMaterial.Name = string.Concat(sourceMaterial.Name, suffix);
                    seamMaterials.Add(cloneMaterial);
                    seamPhaseByMaterialIndex[Convert.ToInt32(cloneMaterial.Index)] = GetSeamPhase(seamPhaseByMaterialIndex, smearFaceIndexes.Key);
                    foreach (int faceIndex in smearFaceIndexes.Value)
                    {
                        TriangleFace face = MeshData.TriangleFaces[faceIndex];
                        MeshData.TriangleFaces[faceIndex] = new TriangleFace(Convert.ToInt32(cloneMaterial.Index), face.V1, face.V2, face.V3);
                        movedFaceIndexes.Add(faceIndex);
                    }
                    Logger.WriteDebug(string.Concat("Character model material '", sourceMaterial.UniqueName, "' moves ", smearFaceIndexes.Value.Count.ToString(), " triangles spanning ",
                        widestExtent.ToString("0.##"), " tiles on ", axis == 0 ? "U" : "V", " to '", cloneMaterial.UniqueName, "'"));
                }
            }

            for (int axis = 0; axis < 2; axis++)
            {
                Dictionary<int, List<int>> crosserFaceIndexesByMaterialIndex = new Dictionary<int, List<int>>();
                foreach (int[] crosser in jointCrossers)
                {
                    if (movedFaceIndexes.Contains(crosser[0]) == true)
                        continue;
                    if (crosser[2 + axis] == 0)
                        continue;
                    if (crosserFaceIndexesByMaterialIndex.ContainsKey(crosser[1]) == false)
                        crosserFaceIndexesByMaterialIndex.Add(crosser[1], new List<int>());
                    crosserFaceIndexesByMaterialIndex[crosser[1]].Add(crosser[0]);
                }
                foreach (var crosserFaceIndexes in crosserFaceIndexesByMaterialIndex)
                {
                    float[] seamPhase = GetSeamPhase(seamPhaseByMaterialIndex, crosserFaceIndexes.Key);
                    int otherAxis = 1 - axis;
                    float texelsA = seamPhase[4 + axis];
                    float texelsB = seamPhase[4 + otherAxis];
                    float overshootTexels = seamPhase[2 + axis] - texelsA;
                    if (overshootTexels < SEAM_PIECE_MIN_OVERSHOOT_TEXELS)
                        continue;
                    // The clone's window: the crossers folded onto the seam on this axis (see FoldMeshUVsPerTriangle) and by centroid
                    // on the other
                    float minA = float.MaxValue, maxA = float.MinValue, minB = float.MaxValue, maxB = float.MinValue;
                    foreach (int faceIndex in crosserFaceIndexes.Value)
                    {
                        TriangleFace face = MeshData.TriangleFaces[faceIndex];
                        int[] corners = new int[3] { face.V1, face.V2, face.V3 };
                        float centroidA = (GetUVComponent(corners[0], axis) + GetUVComponent(corners[1], axis) + GetUVComponent(corners[2], axis)) / 3f;
                        float centroidB = (GetUVComponent(corners[0], otherAxis) + GetUVComponent(corners[1], otherAxis) + GetUVComponent(corners[2], otherAxis)) / 3f;
                        float shiftA = -Convert.ToSingle(Math.Floor(centroidA - seamPhase[axis] - 0.5f));
                        float shiftB = -Convert.ToSingle(Math.Floor(centroidB - seamPhase[otherAxis]));
                        foreach (int corner in corners)
                        {
                            minA = Math.Min(minA, GetUVComponent(corner, axis) + shiftA);
                            maxA = Math.Max(maxA, GetUVComponent(corner, axis) + shiftA);
                            minB = Math.Min(minB, GetUVComponent(corner, otherAxis) + shiftB);
                            maxB = Math.Max(maxB, GetUVComponent(corner, otherAxis) + shiftB);
                        }
                    }
                    // The piece at full resolution where that costs less than the band; where the overshoot is large (the barbarian's
                    // kilt triangles span 0.4 tile from pelvis to knee), at half or a quarter: those triangles stretch across the joint
                    // and blur there anyway, and the band would cost every other piece of the block far more
                    float bandArea = overshootTexels * seamPhase[2 + otherAxis];
                    // Below full resolution the triangles show the art blurred, so that is only for small triangles (the rings of the
                    // iksar's tail: the largest 0.2% of the body), never the barbarian's kilt (0.7%) or the high elf's inner thigh (1.3%),
                    // where the blurred patch is the size of a hand
                    float crosserSurface = 0f;
                    float largestCrosserSurface = 0f;
                    foreach (int faceIndex in crosserFaceIndexes.Value)
                    {
                        float faceSurface = CalculateFaceArea(MeshData.TriangleFaces[faceIndex]);
                        crosserSurface += faceSurface;
                        largestCrosserSurface = Math.Max(largestCrosserSurface, faceSurface);
                    }
                    bool reducedAllowed = overshootTexels >= SEAM_PIECE_REDUCED_MIN_OVERSHOOT_TILES * texelsA && bodySurface > 0
                        && largestCrosserSurface <= bodySurface * SEAM_PIECE_REDUCED_MAX_TRIANGLE_BODY_SHARE && crosserSurface <= bodySurface * SEAM_PIECE_REDUCED_MAX_TOTAL_BODY_SHARE;
                    float pieceScale = 0f;
                    foreach (float candidateScale in new float[3] { 1f, 0.5f, 0.25f })
                    {
                        if (candidateScale < 1f && reducedAllowed == false)
                            break;
                        float pieceArea = (((maxA - minA) * texelsA * candidateScale) + (2 * SEAM_PIECE_MARGIN_TEXELS)) * (((maxB - minB) * texelsB * candidateScale) + (2 * SEAM_PIECE_MARGIN_TEXELS));
                        if (pieceArea < bandArea * 0.8f)
                        {
                            pieceScale = candidateScale;
                            break;
                        }
                    }
                    if (pieceScale <= 0f)
                        continue;
                    Material? sourceMaterial = null;
                    foreach (Material material in Materials)
                        if (Convert.ToInt32(material.Index) == crosserFaceIndexes.Key)
                            sourceMaterial = material;
                    if (sourceMaterial == null)
                        continue;
                    if (sourceMaterial.TextureNames.Count > 0 && sourceMaterial.TextureNames[0].ToLower().StartsWith(hoodTexturePrefix) == true)
                        continue;
                    Material cloneMaterial = new Material(sourceMaterial);
                    cloneMaterial.Index = nextMaterialIndex;
                    nextMaterialIndex++;
                    string suffix = string.Concat(SEAM_PIECE_MATERIAL_SUFFIX, axis == 0 ? "u" : "v", Convert.ToInt32(Math.Round(pieceScale * 100f)).ToString(), "r");
                    cloneMaterial.UniqueName = string.Concat(sourceMaterial.UniqueName, suffix);
                    cloneMaterial.Name = string.Concat(sourceMaterial.Name, suffix);
                    seamMaterials.Add(cloneMaterial);
                    seamPhaseByMaterialIndex[Convert.ToInt32(cloneMaterial.Index)] = seamPhase;
                    foreach (int faceIndex in crosserFaceIndexes.Value)
                    {
                        TriangleFace face = MeshData.TriangleFaces[faceIndex];
                        MeshData.TriangleFaces[faceIndex] = new TriangleFace(Convert.ToInt32(cloneMaterial.Index), face.V1, face.V2, face.V3);
                        movedFaceIndexes.Add(faceIndex);
                    }
                    Logger.WriteDebug(string.Concat("Character model material '", sourceMaterial.UniqueName, "' moves ", crosserFaceIndexes.Value.Count.ToString(), " joint-spanning triangles across its ",
                        axis == 0 ? "U" : "V", " seam to '", cloneMaterial.UniqueName, "' (", overshootTexels.ToString("0"), " texels of overshoot for a ",
                        Math.Ceiling((maxA - minA) * texelsA).ToString("0"), "x", Math.Ceiling((maxB - minB) * texelsB).ToString("0"), " piece at ", pieceScale.ToString("0.##"), ")"));
                }
            }
            Materials.AddRange(seamMaterials);
        }

        // Tile boundaries (the seam phase plus a whole number) strictly inside (min, max)
        private static List<float> GetCrossedTileBoundaries(float min, float max, float seamPhase)
        {
            List<float> boundaries = new List<float>();
            int first = Convert.ToInt32(Math.Floor(min - seamPhase)) + 1;
            int last = Convert.ToInt32(Math.Ceiling(max - seamPhase)) - 1;
            for (int tile = first; tile <= last; tile++)
            {
                float boundary = seamPhase + tile;
                if (boundary > min + UV_CUT_EPSILON && boundary < max - UV_CUT_EPSILON)
                    boundaries.Add(boundary);
            }
            return boundaries;
        }

        // Splits each (convex) polygon along one boundary line into its low and high sides, both kept; vertices on the line belong to both
        private List<List<int>> SplitPolygonsAtTileBoundary(List<List<int>> polygons, int axis, float boundary, Dictionary<string, int> cutVertexIndexByKey,
            List<byte> meshContextByVertexIndex)
        {
            List<List<int>> result = new List<List<int>>();
            foreach (List<int> polygon in polygons)
            {
                List<int> lowSide = new List<int>();
                List<int> highSide = new List<int>();
                bool anyStrictlyLow = false;
                bool anyStrictlyHigh = false;
                for (int i = 0; i < polygon.Count; i++)
                {
                    int a = polygon[i];
                    int b = polygon[(i + 1) % polygon.Count];
                    float ca = GetUVComponent(a, axis) - boundary;
                    float cb = GetUVComponent(b, axis) - boundary;
                    if (ca <= UV_CUT_EPSILON)
                        lowSide.Add(a);
                    if (ca >= -UV_CUT_EPSILON)
                        highSide.Add(a);
                    if (ca < -UV_CUT_EPSILON)
                        anyStrictlyLow = true;
                    if (ca > UV_CUT_EPSILON)
                        anyStrictlyHigh = true;
                    if ((ca < -UV_CUT_EPSILON && cb > UV_CUT_EPSILON) || (ca > UV_CUT_EPSILON && cb < -UV_CUT_EPSILON))
                    {
                        int cutVertexIndex = GetOrCreateCutVertex(a, b, axis, boundary, ca / (ca - cb), cutVertexIndexByKey, meshContextByVertexIndex);
                        lowSide.Add(cutVertexIndex);
                        highSide.Add(cutVertexIndex);
                    }
                }
                if (lowSide.Count >= 3 && anyStrictlyLow == true)
                    result.Add(lowSide);
                if (highSide.Count >= 3 && anyStrictlyHigh == true)
                    result.Add(highSide);
            }
            return result;
        }

        private float GetUVComponent(int vertexIndex, int axis)
        {
            if (axis == 0)
                return MeshData.TextureCoordinates[vertexIndex].X;
            return MeshData.TextureCoordinates[vertexIndex].Y;
        }

        // The vertex where edge a-b crosses the boundary (t along a to b), shared by every triangle using that edge
        private int GetOrCreateCutVertex(int a, int b, int axis, float boundary, float t, Dictionary<string, int> cutVertexIndexByKey, List<byte> meshContextByVertexIndex)
        {
            int low = Math.Min(a, b);
            int high = Math.Max(a, b);
            float tFromLow = (a == low) ? t : 1f - t;
            string key = string.Concat(low.ToString(), "|", high.ToString(), "|", axis.ToString(), "|", boundary.ToString("R"));
            int existingIndex;
            if (cutVertexIndexByKey.TryGetValue(key, out existingIndex) == true)
                return existingIndex;

            int newIndex = MeshData.Vertices.Count;
            Vector3 positionLow = MeshData.Vertices[low];
            Vector3 positionHigh = MeshData.Vertices[high];
            MeshData.Vertices.Add(new Vector3(Lerp(positionLow.X, positionHigh.X, tFromLow), Lerp(positionLow.Y, positionHigh.Y, tFromLow), Lerp(positionLow.Z, positionHigh.Z, tFromLow)));
            if (MeshData.Normals.Count > high)
            {
                Vector3 normalLow = MeshData.Normals[low];
                Vector3 normalHigh = MeshData.Normals[high];
                float nx = Lerp(normalLow.X, normalHigh.X, tFromLow);
                float ny = Lerp(normalLow.Y, normalHigh.Y, tFromLow);
                float nz = Lerp(normalLow.Z, normalHigh.Z, tFromLow);
                float length = Convert.ToSingle(Math.Sqrt((nx * nx) + (ny * ny) + (nz * nz)));
                if (length > 0.00001f)
                {
                    nx /= length; ny /= length; nz /= length;
                }
                MeshData.Normals.Add(new Vector3(nx, ny, nz));
            }
            TextureCoordinates uvLow = MeshData.TextureCoordinates[low];
            TextureCoordinates uvHigh = MeshData.TextureCoordinates[high];
            float u = (axis == 0) ? boundary : Lerp(uvLow.X, uvHigh.X, tFromLow);
            float v = (axis == 1) ? boundary : Lerp(uvLow.Y, uvHigh.Y, tFromLow);
            MeshData.TextureCoordinates.Add(new TextureCoordinates(u, v));
            if (MeshData.VertexColors.Count > high)
            {
                ColorRGBA colorLow = MeshData.VertexColors[low];
                ColorRGBA colorHigh = MeshData.VertexColors[high];
                MeshData.VertexColors.Add(new ColorRGBA(LerpByte(colorLow.R, colorHigh.R, tFromLow), LerpByte(colorLow.G, colorHigh.G, tFromLow),
                    LerpByte(colorLow.B, colorHigh.B, tFromLow), LerpByte(colorLow.A, colorHigh.A, tFromLow)));
            }
            bool nearLow = tFromLow < 0.5f;
            if (MeshData.BoneIDs.Count > high)
            {
                UInt16 boneLow = MeshData.BoneIDs[low];
                UInt16 boneHigh = MeshData.BoneIDs[high];
                MeshData.BoneIDs.Add(nearLow ? boneLow : boneHigh);
                if (boneLow == boneHigh)
                {
                    MeshData.SecondaryBoneIDs.Add(0);
                    MeshData.SecondaryBoneWeights.Add(0);
                }
                else
                {
                    // The farther endpoint's bone gets the remaining share (at most half)
                    float otherShare = nearLow ? tFromLow : (1f - tFromLow);
                    MeshData.SecondaryBoneIDs.Add(nearLow ? boneHigh : boneLow);
                    MeshData.SecondaryBoneWeights.Add(Convert.ToByte(Math.Round(otherShare * 255f)));
                }
            }
            else
            {
                MeshData.SecondaryBoneIDs.Add(0);
                MeshData.SecondaryBoneWeights.Add(0);
            }
            if (MeshData.AnimatedVertexFramesByVertexIndex.Count > high)
                MeshData.AnimatedVertexFramesByVertexIndex.Add(MeshData.AnimatedVertexFramesByVertexIndex[nearLow ? low : high]);
            if (meshContextByVertexIndex.Count > high)
                meshContextByVertexIndex.Add(meshContextByVertexIndex[low]);
            cutVertexIndexByKey.Add(key, newIndex);
            return newIndex;
        }

        private static float Lerp(float from, float to, float t)
        {
            return from + ((to - from) * t);
        }

        private static byte LerpByte(byte from, byte to, float t)
        {
            return Convert.ToByte(Math.Max(0, Math.Min(255, Math.Round(from + ((to - from) * t)))));
        }

        // Shifts every triangle by whole tiles onto its texture's window tile (the one starting at the seam phase, see
        // CalculateTileSeamPhases), by its centroid.  A vertex shared by triangles that need different shifts is duplicated, one copy
        // per shift, across every per-vertex list
        private void FoldMeshUVsPerTriangle(List<byte> meshContextByVertexIndex, Dictionary<int, float[]> seamPhaseByMaterialIndex)
        {
            int originalVertexCount = MeshData.Vertices.Count;
            if (MeshData.TextureCoordinates.Count != originalVertexCount)
                return;
            List<float[]> rawUVsByVertexIndex = new List<float[]>();
            for (int i = 0; i < originalVertexCount; i++)
                rawUVsByVertexIndex.Add(new float[2] { MeshData.TextureCoordinates[i].X, MeshData.TextureCoordinates[i].Y });

            Dictionary<string, int> vertexIndexByShiftKey = new Dictionary<string, int>();
            HashSet<int> shiftedInPlaceVertexIndexes = new HashSet<int>();
            Dictionary<int, int> seamAxisByMaterialIndex = new Dictionary<int, int>(); // Seam piece clones (see SplitSeamPieces): the axis they straddle
            foreach (Material material in Materials)
            {
                if (material.UniqueName.Contains(string.Concat(SEAM_PIECE_MATERIAL_SUFFIX, "u")) == true)
                    seamAxisByMaterialIndex[Convert.ToInt32(material.Index)] = 0;
                else if (material.UniqueName.Contains(string.Concat(SEAM_PIECE_MATERIAL_SUFFIX, "v")) == true)
                    seamAxisByMaterialIndex[Convert.ToInt32(material.Index)] = 1;
            }
            int duplicatedVertexCount = 0;
            for (int triangleIndex = 0; triangleIndex < MeshData.TriangleFaces.Count; triangleIndex++)
            {
                TriangleFace triangleFace = MeshData.TriangleFaces[triangleIndex];
                int[] cornerVertexIndexes = new int[3] { triangleFace.V1, triangleFace.V2, triangleFace.V3 };
                if (cornerVertexIndexes[0] >= originalVertexCount || cornerVertexIndexes[1] >= originalVertexCount || cornerVertexIndexes[2] >= originalVertexCount)
                    continue;
                float[] seamPhase = GetSeamPhase(seamPhaseByMaterialIndex, triangleFace.MaterialIndex);
                float centroidU = (rawUVsByVertexIndex[cornerVertexIndexes[0]][0] + rawUVsByVertexIndex[cornerVertexIndexes[1]][0] + rawUVsByVertexIndex[cornerVertexIndexes[2]][0]) / 3f;
                float centroidV = (rawUVsByVertexIndex[cornerVertexIndexes[0]][1] + rawUVsByVertexIndex[cornerVertexIndexes[1]][1] + rawUVsByVertexIndex[cornerVertexIndexes[2]][1]) / 3f;
                int shiftU = -Convert.ToInt32(Math.Floor(centroidU - seamPhase[0]));
                int shiftV = -Convert.ToInt32(Math.Floor(centroidV - seamPhase[1]));
                // A triangle still straddling a boundary here spans a joint and was left whole (see SplitTrianglesAtUVTileBoundaries):
                // it folds to the side of its last boundary that leaves less of it outside the tile, so the window grows by the smaller
                // part rather than by whichever side its centroid happens to miss (the barbarian's kilt window fell from 1.49 to 1.09
                // tiles).  A seam piece's triangles instead all fold so their centroids sit within half a tile of the seam's far edge,
                // so they land together (by centroid alone half would fold to the near edge, a tile apart)
                int seamAxis;
                if (seamAxisByMaterialIndex.TryGetValue(triangleFace.MaterialIndex, out seamAxis) == true)
                {
                    if (seamAxis == 0)
                        shiftU = -Convert.ToInt32(Math.Floor(centroidU - seamPhase[0] - 0.5f));
                    else
                        shiftV = -Convert.ToInt32(Math.Floor(centroidV - seamPhase[1] - 0.5f));
                }
                else
                {
                    shiftU = GetStraddlingShift(rawUVsByVertexIndex, cornerVertexIndexes, 0, seamPhase[0], shiftU);
                    shiftV = GetStraddlingShift(rawUVsByVertexIndex, cornerVertexIndexes, 1, seamPhase[1], shiftV);
                }

                for (int corner = 0; corner < 3; corner++)
                {
                    int originalVertexIndex = cornerVertexIndexes[corner];
                    string shiftKey = string.Concat(originalVertexIndex.ToString(), "|", shiftU.ToString(), "|", shiftV.ToString());
                    int targetVertexIndex;
                    if (vertexIndexByShiftKey.TryGetValue(shiftKey, out targetVertexIndex) == false)
                    {
                        float[] rawUV = rawUVsByVertexIndex[originalVertexIndex];
                        if (shiftedInPlaceVertexIndexes.Contains(originalVertexIndex) == false)
                        {
                            // First shift this vertex takes: applied in place
                            shiftedInPlaceVertexIndexes.Add(originalVertexIndex);
                            MeshData.TextureCoordinates[originalVertexIndex] = new TextureCoordinates(rawUV[0] + shiftU, rawUV[1] + shiftV);
                            targetVertexIndex = originalVertexIndex;
                        }
                        else
                        {
                            // Another shift of a vertex already placed: a duplicate carries it
                            targetVertexIndex = MeshData.Vertices.Count;
                            MeshData.Vertices.Add(new Vector3(MeshData.Vertices[originalVertexIndex]));
                            if (MeshData.Normals.Count > originalVertexIndex)
                                MeshData.Normals.Add(new Vector3(MeshData.Normals[originalVertexIndex]));
                            MeshData.TextureCoordinates.Add(new TextureCoordinates(rawUV[0] + shiftU, rawUV[1] + shiftV));
                            if (MeshData.BoneIDs.Count > originalVertexIndex)
                                MeshData.BoneIDs.Add(MeshData.BoneIDs[originalVertexIndex]);
                            if (MeshData.SecondaryBoneIDs.Count > originalVertexIndex)
                            {
                                MeshData.SecondaryBoneIDs.Add(MeshData.SecondaryBoneIDs[originalVertexIndex]);
                                MeshData.SecondaryBoneWeights.Add(MeshData.SecondaryBoneWeights[originalVertexIndex]);
                            }
                            if (MeshData.VertexColors.Count > originalVertexIndex)
                                MeshData.VertexColors.Add(new ColorRGBA(MeshData.VertexColors[originalVertexIndex]));
                            if (MeshData.AnimatedVertexFramesByVertexIndex.Count > originalVertexIndex)
                                MeshData.AnimatedVertexFramesByVertexIndex.Add(MeshData.AnimatedVertexFramesByVertexIndex[originalVertexIndex]);
                            if (meshContextByVertexIndex.Count > originalVertexIndex)
                                meshContextByVertexIndex.Add(meshContextByVertexIndex[originalVertexIndex]);
                            duplicatedVertexCount++;
                        }
                        vertexIndexByShiftKey.Add(shiftKey, targetVertexIndex);
                    }
                    cornerVertexIndexes[corner] = targetVertexIndex;
                }
                MeshData.TriangleFaces[triangleIndex] = new TriangleFace(triangleFace.MaterialIndex, cornerVertexIndexes[0], cornerVertexIndexes[1], cornerVertexIndexes[2]);
            }
            if (duplicatedVertexCount > 0)
                Logger.WriteDebug(string.Concat("Character model UV fold duplicated ", duplicatedVertexCount.ToString(), " seam vertices onto ", originalVertexCount.ToString(), " originals"));
        }

        // The whole-tile shift for a triangle straddling a tile boundary on this axis (see FoldMeshUVsPerTriangle); the given shift
        // when it straddles none
        private static int GetStraddlingShift(List<float[]> rawUVsByVertexIndex, int[] cornerVertexIndexes, int axis, float seamPhase, int centroidShift)
        {
            float min = float.MaxValue;
            float max = float.MinValue;
            foreach (int cornerVertexIndex in cornerVertexIndexes)
            {
                min = Math.Min(min, rawUVsByVertexIndex[cornerVertexIndex][axis] - seamPhase);
                max = Math.Max(max, rawUVsByVertexIndex[cornerVertexIndex][axis] - seamPhase);
            }
            int firstTile = Convert.ToInt32(Math.Floor(min + UV_CUT_EPSILON));
            int lastTile = Convert.ToInt32(Math.Floor(max - UV_CUT_EPSILON));
            if (lastTile <= firstTile)
                return centroidShift;
            float below = lastTile - min;
            float above = max - lastTile;
            return (above <= below) ? -(lastTile - 1) : -lastTile;
        }

        private void SplitMaterialsSpanningMeshContexts(string inputObjectName, List<byte> meshContextByVertexIndex)
        {
            UInt32 nextMaterialIndex = 0;
            foreach (Material material in Materials)
                nextMaterialIndex = Math.Max(nextMaterialIndex, material.Index + 1);

            // Head-mesh extent (head mesh vertices are in the head bone's space, so they get their own bounds), for spotting sliver
            // triangles: some head meshes carry a stray triangle of a body material running from the crown to the neck, such as the erudite
            // male's foot and leg materials.  Those are export junk and get dropped below
            Vector3 headMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 headMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            for (int i = 0; i < MeshData.Vertices.Count && i < meshContextByVertexIndex.Count; i++)
            {
                if (ObjectModelCharacterComposite.IsHeadMeshContext(meshContextByVertexIndex[i]) == false)
                    continue;
                Vector3 vertex = MeshData.Vertices[i];
                headMin.X = Math.Min(headMin.X, vertex.X); headMin.Y = Math.Min(headMin.Y, vertex.Y); headMin.Z = Math.Min(headMin.Z, vertex.Z);
                headMax.X = Math.Max(headMax.X, vertex.X); headMax.Y = Math.Max(headMax.Y, vertex.Y); headMax.Z = Math.Max(headMax.Z, vertex.Z);
            }
            float headTallestSpan = Math.Max(headMax.X - headMin.X, Math.Max(headMax.Y - headMin.Y, headMax.Z - headMin.Z));

            // Context priority for which one keeps the original material: bare head, helmed heads, body, robe
            byte[] contextPriority = new byte[] { ObjectModelCharacterComposite.MESH_CONTEXT_HEAD, ObjectModelCharacterComposite.MESH_CONTEXT_HELM_HEAD_BASE + 1,
                ObjectModelCharacterComposite.MESH_CONTEXT_HELM_HEAD_BASE + 2, ObjectModelCharacterComposite.MESH_CONTEXT_HELM_HEAD_BASE + 3,
                ObjectModelCharacterComposite.MESH_CONTEXT_BODY, ObjectModelCharacterComposite.MESH_CONTEXT_ROBE };

            List<Material> materialsToAdd = new List<Material>();
            foreach (Material material in Materials)
            {
                if (material.TextureNames.Count == 0)
                    continue;

                // Determine what mesh contexts this material's triangles are in
                HashSet<byte> contexts = GetMeshContextsForMaterial(material, meshContextByVertexIndex);
                bool hasHeadContext = false;
                bool hasNonHeadContext = false;
                foreach (byte context in contexts)
                {
                    if (ObjectModelCharacterComposite.IsHeadMeshContext(context) == true)
                        hasHeadContext = true;
                    else
                        hasNonHeadContext = true;
                }
                if (hasHeadContext == true && hasNonHeadContext == true)
                {
                    // Drop the head-mesh sliver triangles of this material (any that span over half the head mesh on some axis).  Small
                    // body-texture pieces on a head mesh stay: the high elf female's hair bow is a scrap of her leg texture
                    for (int i = MeshData.TriangleFaces.Count - 1; i >= 0; i--)
                    {
                        TriangleFace triangleFace = MeshData.TriangleFaces[i];
                        if (triangleFace.MaterialIndex != Convert.ToInt32(material.Index))
                            continue;
                        if (triangleFace.V1 >= meshContextByVertexIndex.Count)
                            continue;
                        if (ObjectModelCharacterComposite.IsHeadMeshContext(meshContextByVertexIndex[triangleFace.V1]) == false)
                            continue;
                        if (IsSliverTriangle(triangleFace, headTallestSpan) == true)
                        {
                            Logger.WriteDebug("- [" + inputObjectName + "]: Dropped a head-mesh sliver triangle of body material '" + material.UniqueName + "'");
                            MeshData.TriangleFaces.RemoveAt(i);
                        }
                    }
                    contexts = GetMeshContextsForMaterial(material, meshContextByVertexIndex);
                }
                if (contexts.Count < 2)
                    continue;

                // The highest priority context keeps the original material, the rest get clones
                byte keptContext = ObjectModelCharacterComposite.MESH_CONTEXT_BODY;
                foreach (byte context in contextPriority)
                {
                    if (contexts.Contains(context) == true)
                    {
                        keptContext = context;
                        break;
                    }
                }
                foreach (byte context in contextPriority)
                {
                    if (context == keptContext || contexts.Contains(context) == false)
                        continue;
                    string nameSuffix = "_bodysplit";
                    if (context == ObjectModelCharacterComposite.MESH_CONTEXT_ROBE)
                        nameSuffix = "_robesplit";
                    else if (ObjectModelCharacterComposite.IsHeadMeshContext(context) == true)
                        nameSuffix = string.Concat("_head", ObjectModelCharacterComposite.GetHelmIndexForMeshContext(context).ToString(), "split");
                    CloneMaterialForMeshContext(inputObjectName, material, nameSuffix, context, meshContextByVertexIndex, materialsToAdd, ref nextMaterialIndex);
                }
            }
            foreach (Material materialToAdd in materialsToAdd)
                Materials.Add(materialToAdd);
        }

        private HashSet<byte> GetMeshContextsForMaterial(Material material, List<byte> meshContextByVertexIndex)
        {
            HashSet<byte> contexts = new HashSet<byte>();
            foreach (TriangleFace triangleFace in MeshData.TriangleFaces)
            {
                if (triangleFace.MaterialIndex != Convert.ToInt32(material.Index))
                    continue;
                if (triangleFace.V1 >= meshContextByVertexIndex.Count)
                    continue;
                contexts.Add(meshContextByVertexIndex[triangleFace.V1]);
            }
            return contexts;
        }

        private void CloneMaterialForMeshContext(string inputObjectName, Material material, string nameSuffix, byte meshContext, List<byte> meshContextByVertexIndex,
            List<Material> materialsToAdd, ref UInt32 nextMaterialIndex)
        {
            Material cloneMaterial = new Material(material);
            cloneMaterial.Index = nextMaterialIndex;
            nextMaterialIndex++;
            cloneMaterial.UniqueName = string.Concat(material.UniqueName, nameSuffix);
            cloneMaterial.Name = string.Concat(material.Name, nameSuffix);
            materialsToAdd.Add(cloneMaterial);
            Logger.WriteDebug("- [" + inputObjectName + "]: Material '" + material.UniqueName + "' spans mesh contexts, so the '" + nameSuffix + "' part was split off");
            for (int i = 0; i < MeshData.TriangleFaces.Count; i++)
            {
                TriangleFace triangleFace = MeshData.TriangleFaces[i];
                if (triangleFace.MaterialIndex != Convert.ToInt32(material.Index))
                    continue;
                if (triangleFace.V1 >= meshContextByVertexIndex.Count)
                    continue;
                if (meshContextByVertexIndex[triangleFace.V1] != meshContext)
                    continue;
                MeshData.TriangleFaces[i] = new TriangleFace(Convert.ToInt32(cloneMaterial.Index), triangleFace.V1, triangleFace.V2, triangleFace.V3);
            }
        }

        // A triangle spanning over half of the mesh's tallest extent on some axis is a sliver, not character geometry
        private bool IsSliverTriangle(TriangleFace triangleFace, float meshTallestSpan)
        {
            if (triangleFace.V1 >= MeshData.Vertices.Count || triangleFace.V2 >= MeshData.Vertices.Count || triangleFace.V3 >= MeshData.Vertices.Count)
                return false;
            if (meshTallestSpan <= 0)
                return false;
            Vector3 v1 = MeshData.Vertices[triangleFace.V1];
            Vector3 v2 = MeshData.Vertices[triangleFace.V2];
            Vector3 v3 = MeshData.Vertices[triangleFace.V3];
            float spanX = Math.Max(v1.X, Math.Max(v2.X, v3.X)) - Math.Min(v1.X, Math.Min(v2.X, v3.X));
            float spanY = Math.Max(v1.Y, Math.Max(v2.Y, v3.Y)) - Math.Min(v1.Y, Math.Min(v2.Y, v3.Y));
            float spanZ = Math.Max(v1.Z, Math.Max(v2.Z, v3.Z)) - Math.Min(v1.Z, Math.Min(v2.Z, v3.Z));
            return Math.Max(spanX, Math.Max(spanY, spanZ)) > meshTallestSpan * 0.5f;
        }

        private void LoadMaterialDataFromDisk(string inputObjectName, string inputObjectFolder, int materialIndex, string customMaterialListLine)
        {
            Logger.WriteDebug("- [" + inputObjectName + "]: Reading materials...");
            string materialListFileName = Path.Combine(inputObjectFolder, "MaterialLists", inputObjectName + ".txt");

            // Parse the material list once per (file, customMaterialListLine) and reuse.
            string cacheKey = materialListFileName + "|" + customMaterialListLine;
            EQMaterialList? materialListData;
            if (CachedMaterialListByKey.TryGetValue(cacheKey, out materialListData) == false)
            {
                materialListData = new EQMaterialList();
                if (materialListData.LoadFromDisk(materialListFileName, customMaterialListLine) == false)
                {
                    Logger.WriteError("- [" + inputObjectName + "]: No material data found.");
                    return;
                }
                CachedMaterialListByKey.TryAdd(cacheKey, materialListData);
            }

            // Select the texture variation (with fallback to 0), matching the original behavior
            List<Material>? sourceMaterials = null;
            if (materialIndex >= materialListData.MaterialsByTextureVariation.Count)
            {
                if (materialListData.MaterialsByTextureVariation.Count > 0)
                {
                    sourceMaterials = materialListData.MaterialsByTextureVariation[0];
                    Logger.WriteDebug("- [" + inputObjectName + "]: materialIndex of value '" + materialIndex + "' exceeded count of MaterialsByTextureVariation of value '" + materialListData.MaterialsByTextureVariation.Count + "', so fell back to 0.");
                }
                else
                    Logger.WriteError("- [" + inputObjectName + "]: materialIndex of value '" + materialIndex + "' exceeded count of MaterialsByTextureVariation of value '" + materialListData.MaterialsByTextureVariation.Count + "'.");
            }
            else
            {
                sourceMaterials = materialListData.MaterialsByTextureVariation[materialIndex];
            }

            // Use a deep copy so per-template material changes don't cause problems with the cache
            if (sourceMaterials != null)
            {
                Materials = new List<Material>(sourceMaterials.Count);
                foreach (Material sourceMaterial in sourceMaterials)
                    Materials.Add(new Material(sourceMaterial));
            }
        }

        // TODO: Make this work with multiple meshes
        private void LoadCollisionMeshData(string inputObjectName, List<string> meshNames, string inputObjectFolder)
        {
            Logger.WriteDebug("- [" + inputObjectName + "]: Reading collision mesh data...");
            MeshData collisionMeshData = new MeshData();
            foreach (string meshName in meshNames)
            {
                string collisionMeshFileName = Path.Combine(inputObjectFolder, "Meshes", meshName + "_collision.txt");
                if (File.Exists(collisionMeshFileName) == false)
                {
                    Logger.WriteDebug("- [" + inputObjectName + "]: No collision mesh found, skipping.");
                    continue;
                }
                EQMesh meshData = new EQMesh();
                if (CachedCollisionMeshByFileName.TryGetValue(collisionMeshFileName, out EQMesh? cachedCollisionMesh) == true)
                    meshData = new EQMesh(cachedCollisionMesh);
                else if (meshData.LoadFromDisk(collisionMeshFileName) == true)
                    CachedCollisionMeshByFileName.TryAdd(collisionMeshFileName, new EQMesh(meshData));
                else
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
            Logger.WriteDebug("- [" + inputObjectName + "]: Reading skeleton data...");
            string skeletonFileName = Path.Combine(inputObjectFolder, "Skeletons", inputObjectName + ".txt");
            if (CachedSkeletonDataByFileName.TryGetValue(skeletonFileName, out EQSkeleton? cachedSkeleton) == true)
                SkeletonData = new EQSkeleton(cachedSkeleton);
            else
            {
                SkeletonData = new EQSkeleton();
                if (SkeletonData.LoadFromDisk(skeletonFileName) == false)
                {
                    Logger.WriteError("- [" + inputObjectName + "]: Issue loading skeleton data that should be at '" + skeletonFileName + "'");
                    return;
                }
                CachedSkeletonDataByFileName.TryAdd(skeletonFileName, new EQSkeleton(SkeletonData));
            }
        }

        public void LoadParticleCloudData(string inputObjectName, List<string> particleCloudNames, string inputObjectFolder)
        {
            Logger.WriteDebug("- [" + inputObjectName + "]: Reading particle cloud data...");
            foreach (string particleCloudName in particleCloudNames)
            {
                string particleCloudFileName = Path.Combine(inputObjectFolder, "Particles", particleCloudName + ".txt");
                if (ParticleCloudsByName.ContainsKey(particleCloudName) == true)
                    continue;
                if (CachedParticleCloudDataByFileName.TryGetValue(particleCloudFileName, out EQParticleCloud? cachedParticleCloud) == true)
                    ParticleCloudsByName.Add(particleCloudName, new EQParticleCloud(cachedParticleCloud)); // Clone
                else
                {
                    EQParticleCloud newParticleCloud = new EQParticleCloud();
                    if (newParticleCloud.LoadFromDisk(particleCloudFileName) == false)
                    {
                        Logger.WriteError("- [" + inputObjectName + "]: Issue loading particle cloud data that should be at '" + particleCloudFileName + "'");
                        return;
                    }
                    ParticleCloudsByName.Add(particleCloudName, newParticleCloud);
                    CachedParticleCloudDataByFileName.TryAdd(particleCloudFileName, new EQParticleCloud(newParticleCloud));
                }
            }
        }

        private void ConvertAnimatedVerticesToSkeleton(string inputObjectName)
        {
            Logger.WriteDebug("- [" + inputObjectName + "]: Converting animated vertices to skeleton data...");
            SkeletonData = new EQSkeleton();
            SkeletonData.LoadFromAnimatedVerticesData(ref MeshData);
        }

        private void GenerateAnimationFromAnimatedVertexSkeleton(string inputObjectName, MeshData meshData)
        {
            Logger.WriteDebug("- [" + inputObjectName + "]: Generating an animation based on an animated vertex skeleton...");
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
                    curFrame.XPosition = curPosOffset.X;
                    curFrame.ZPosition = curPosOffset.Z;
                    curFrame.YPosition = curPosOffset.Y;
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

        private void LoadAnimationData(string inputObjectName, string eqInputObjectFileName, string inputObjectFolder, string animationSupplementalName)
        {
            Logger.WriteDebug("- [" + inputObjectName + "]: Reading animation data...");

            // Animations are split by animation name
            Animations.Clear();
            string animationFolder = Path.Combine(inputObjectFolder, "Animations");
            DirectoryInfo animationDirectoryInfo = new DirectoryInfo(animationFolder);
            FileInfo[] animationFileInfos = animationDirectoryInfo.GetFiles(eqInputObjectFileName + "_*.txt");
            foreach(FileInfo animationFileInfo in animationFileInfos)
            {
                string animationFileName = Path.GetFileNameWithoutExtension(animationFileInfo.FullName);
                string animationName = animationFileName.Split("_")[1];
                EQAnimation curEQAnimation = new EQAnimation();
                if (curEQAnimation.LoadFromDisk(animationFileInfo.FullName))
                    Animations.Add(animationName, curEQAnimation.Animation);
                else
                    Logger.WriteError("- [" + inputObjectName + "]: Could not load animation data that should be at '" + animationFileName + "'");
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
                        Animations.Add(animationName, curEQAnimation.Animation);
                    else
                        Logger.WriteError("- [" + inputObjectName + "]: Could not load animation data that should be at '" + animationFileName + "'");
                }
            }

            // Backfill "pos" frame for any skeleton bone that an animation never references.  Without this, such bones get an empty track and
            // the WoW client renders them at identity (no translation or rotation), collapsing that body part onto its parent joint.  Without this,
            // some models like freeport guards or pixie wings simply don't render properly
            if (Animations.ContainsKey("pos") == true && SkeletonData.BoneStructures.Count > 0)
            {
                Dictionary<string, Animation.BoneAnimationFrame> posFramesByBoneName = new Dictionary<string, Animation.BoneAnimationFrame>();
                foreach (Animation.BoneAnimationFrame posFrame in Animations["pos"].AnimationFrames)
                    if (posFramesByBoneName.ContainsKey(posFrame.GetBoneName()) == false)
                        posFramesByBoneName.Add(posFrame.GetBoneName(), posFrame);
                foreach (var animationByName in Animations)
                {
                    if (animationByName.Key == "pos")
                        continue;
                    Animation curAnimation = animationByName.Value;
                    HashSet<string> boneNamesInAnimation = new HashSet<string>();
                    foreach (Animation.BoneAnimationFrame animationFrame in curAnimation.AnimationFrames)
                        boneNamesInAnimation.Add(animationFrame.GetBoneName());
                    foreach (EQSkeleton.EQSkeletonBone skeletonBone in SkeletonData.BoneStructures)
                    {
                        if (boneNamesInAnimation.Contains(skeletonBone.BoneName) == true)
                            continue;
                        if (posFramesByBoneName.ContainsKey(skeletonBone.BoneName) == false)
                            continue;
                        Animation.BoneAnimationFrame newFrame = posFramesByBoneName[skeletonBone.BoneName];
                        newFrame.FrameIndex = 0;
                        newFrame.FramesMS = curAnimation.TotalTimeInMS;
                        curAnimation.AnimationFrames.Add(newFrame);
                        Logger.WriteDebug("- [" + inputObjectName + "]: Bone '" + skeletonBone.BoneName + "' had no frames in animation '" + animationByName.Key + "', so a bind pose frame was added for it");
                    }
                }
            }

            // Generate any appropriate reversals
            {
                Dictionary<string, Animation> newReverseAnimationsByName = new Dictionary<string, Animation>();
                foreach (Animation animation in Animations.Values)
                {
                    string newAnimationName = string.Empty;
                    switch (animation.EQAnimationType)
                    {
                        case EQAnimationType.l01Walk: newAnimationName = "l01r"; break;
                        case EQAnimationType.l02Run: newAnimationName = "l02r"; break;
                        case EQAnimationType.l08Crouch: newAnimationName = "l08r"; break;
                        case EQAnimationType.p02StandToSit: newAnimationName = "p02r"; break;
                        case EQAnimationType.p03ShuffleFeet: newAnimationName = "p03r"; break;
                        case EQAnimationType.p05KneelStart: newAnimationName = "p05r"; break;
                        default: continue;
                    }
                    AnimationType animationType = EQAnimation.DetermineAnimationType(newAnimationName);
                    EQAnimationType eqAnimationType = EQAnimation.DetermineEQAnimationType(newAnimationName);
                    Animation newReverseAnimation = animation.GenerateAsReversedVersion(newAnimationName, animationType, eqAnimationType);
                    newReverseAnimationsByName.Add(newAnimationName, newReverseAnimation);
                }
                foreach (var reverseAnimation in newReverseAnimationsByName)
                    Animations.Add(reverseAnimation.Key, reverseAnimation.Value);
            }
        }
    }
}
