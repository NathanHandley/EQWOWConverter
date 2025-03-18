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
using EQWOWConverter.EQFiles;
using EQWOWConverter.ObjectModels.Properties;
using EQWOWConverter.Creatures;

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
        public List<string> GeneratedTextureNames = new List<string>();
        public BoundingBox BoundingBox = new BoundingBox();
        public float BoundingSphereRadius = 0f;
        public Dictionary<AnimationType, Sound> SoundsByAnimationType = new Dictionary<AnimationType, Sound>();
        public float ModelScalePreWorldScale = 1f;
        public float ModelLiftPreWorldScale = 0f;
        public int NumOfFidgetSounds = 0;
        public Vector3 PortraitCameraPosition = new Vector3();
        public Vector3 PortraitCameraTargetPosition = new Vector3();
        public MeshData MeshData = new MeshData();

        public List<Vector3> CollisionPositions = new List<Vector3>();
        public List<Vector3> CollisionFaceNormals = new List<Vector3>();
        public List<TriangleFace> CollisionTriangles = new List<TriangleFace>();
        public BoundingBox CollisionBoundingBox = new BoundingBox();
        public float CollisionSphereRaidus = 0f;

        private CreatureModelTemplate? CreatureModelTemplate = null;

        public static Dictionary<string, ObjectModel> StaticObjectModelsByName = new Dictionary<string, ObjectModel>();

        public ObjectModel(string name, ObjectModelProperties objectProperties, ObjectModelType modelType, float modelScale = 1, float modelLift = 0)
        {
            Name = name;
            Properties = objectProperties;
            ModelType = modelType;
            ModelScalePreWorldScale = modelScale;
            ModelLiftPreWorldScale = modelLift;
        }

        public void LoadStaticEQObjectFromFile(string inputRootFolder, string meshName, ActiveDoodadAnimType? activeDoodadAnimationType = null, 
            float activeDoodadAnimModValue = 0, int activeDoodadAnimTimeInMS = 0)
        {
            // Clear any old data and reload it
            EQObjectModelData = new ObjectModelEQData();
            EQObjectModelData.LoadAllStaticObjectDataFromDisk(Name, inputRootFolder, meshName);

            if (EQObjectModelData.CollisionVertices.Count == 0)
                Load(Name, EQObjectModelData.Materials, EQObjectModelData.MeshData, new List<Vector3>(), new List<TriangleFace>(), activeDoodadAnimationType, 
                    activeDoodadAnimModValue, activeDoodadAnimTimeInMS);
            else
                Load(Name, EQObjectModelData.Materials, EQObjectModelData.MeshData, EQObjectModelData.CollisionVertices, EQObjectModelData.CollisionTriangleFaces, 
                    activeDoodadAnimationType, activeDoodadAnimModValue, activeDoodadAnimTimeInMS);
        }

        public void LoadSkeletalEQObjectFromFile(string inputRootFolder, CreatureModelTemplate? creatureModelTemplate)
        {
            // Clear any old data and reload it
            EQObjectModelData = new ObjectModelEQData();
            EQObjectModelData.LoadAllAnimateObjectDataFromDisk(Name, inputRootFolder, creatureModelTemplate);

            // Save the template
            CreatureModelTemplate = creatureModelTemplate;

            // Load it
            if (EQObjectModelData.CollisionVertices.Count == 0)
                Load(Name, EQObjectModelData.Materials, EQObjectModelData.MeshData, new List<Vector3>(), new List<TriangleFace>());
            else
                Load(Name, EQObjectModelData.Materials, EQObjectModelData.MeshData, EQObjectModelData.CollisionVertices, EQObjectModelData.CollisionTriangleFaces);
        }

        // TODO: Vertex Colors
        public void Load(string name, List<Material> initialMaterials, MeshData meshData, List<Vector3> collisionVertices,
            List<TriangleFace> collisionTriangleFaces, ActiveDoodadAnimType? activeDoodadAnimationType = null, float activeDoodadAnimModValue = 0, int activeDoodadAnimTimeInMS = 0)
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
            meshData.SortDataByMaterialAndBones();

            // Perform EQ->WoW translations if this is coming from a raw EQ object
            if (ModelType == ObjectModelType.Skeletal || ModelType == ObjectModelType.StaticDoodad)
            {
                float scaleAmount = ModelScalePreWorldScale * Configuration.GENERATE_WORLD_SCALE;
                if (ModelType == ObjectModelType.Skeletal)
                    scaleAmount = ModelScalePreWorldScale * Configuration.GENERATE_CREATURE_SCALE;

                // Regular
                meshData.ApplyEQToWoWGeometryTranslationsAndScale((ModelType != ObjectModelType.Skeletal), scaleAmount);

                // If there is any collision data, also translate that too
                if (collisionVertices.Count > 0)
                {
                    MeshData collisionMeshData = new MeshData();
                    collisionMeshData.TriangleFaces = collisionTriangleFaces;
                    collisionMeshData.Vertices = collisionVertices;
                    collisionMeshData.ApplyEQToWoWGeometryTranslationsAndScale((ModelType != ObjectModelType.Skeletal), scaleAmount);
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
            ModelReplaceableTextureLookups.Add(0);

            // Build the bones and animation structures
            ProcessBonesAndAnimation(activeDoodadAnimationType, activeDoodadAnimModValue, activeDoodadAnimTimeInMS);

            // Create a global sequence if there is none
            if (GlobalLoopSequenceLimits.Count == 0)
                GlobalLoopSequenceLimits.Add(0);

            // Store the final state mesh data
            MeshData = meshData;
        }

        private void ProcessBonesAndAnimation(ActiveDoodadAnimType? activeDoodadAnimationType = null, float activeDoodadAnimModValue = 0, int activeDoodadAnimTimeInMS = 0)
        {
            // Static types
            if ((ModelType != ObjectModelType.Skeletal) || EQObjectModelData.Animations.Count == 0)
            {
                ModelBoneKeyLookups.Add(-1);

                // Create a base bone
                ModelBones.Add(new ObjectModelBone());
                ModelBones[0].BoneNameEQ = "root";

                if (activeDoodadAnimationType == null)
                {
                    // Make one animation (standing) for normal static objects
                    ModelAnimations.Add(new ObjectModelAnimation());
                    ModelAnimations[0].BoundingBox = new BoundingBox(BoundingBox);
                    ModelAnimations[0].BoundingRadius = BoundingSphereRadius;
                }
                else
                {
                    // For lift triggers, there is animation build-out that occurs specific to the behavior of it
                    BuildAnimationsForActiveDoodad(activeDoodadAnimationType, activeDoodadAnimModValue, activeDoodadAnimTimeInMS);
                }
            }

            // Skeletal
            else 
            {
                // Build the skeleton
                if (BuildSkeletonBonesAndLookups() == false)
                {
                    Logger.WriteError("Could not build skeleton information for object '" + Name + "'");

                    // Make one animation (standing)
                    ModelBones.Add(new ObjectModelBone());
                    ModelAnimations.Add(new ObjectModelAnimation());
                    ModelAnimations[0].BoundingBox = new BoundingBox(BoundingBox);
                    ModelAnimations[0].BoundingRadius = BoundingSphereRadius;
                    return;
                }

                if (CreateAndSetAnimations() == false)
                {
                    Logger.WriteError("Could not create and set animations for object '" + Name + "'");

                    // Make one animation (standing)
                    ModelAnimations.Add(new ObjectModelAnimation());
                    ModelAnimations[0].BoundingBox = new BoundingBox(BoundingBox);
                    ModelAnimations[0].BoundingRadius = BoundingSphereRadius;
                    return;
                }

                if (ModelType == ObjectModelType.Skeletal)
                {
                    // Fill out the nameplate bone translation
                    if (CreatureModelTemplate != null && CreatureModelTemplate.Race.NameplateAddedHeight > Configuration.GENERATE_FLOAT_EPSILON)
                    {
                        // Set the adjustment vector
                        ObjectModelBone nameplateBone = GetBoneWithName("nameplate");
                        Vector3 adjustmentVector = new Vector3(0, 0, CreatureModelTemplate.Race.NameplateAddedHeight);
                        int raceID = CreatureModelTemplate.Race.ID;

                        // These races project forward instead of up due to a rotation
                        if (raceID == 31 || raceID == 66 || raceID == 126)
                            adjustmentVector = new Vector3(0, CreatureModelTemplate.Race.NameplateAddedHeight, 0);

                        // These races project to their right instead of up due to a rotation
                        if (raceID == 107 || raceID == 135 || raceID == 154)
                            adjustmentVector = new Vector3(0, CreatureModelTemplate.Race.NameplateAddedHeight, 0);

                        // These races project to their right, but rotated the other way
                        if (raceID == 162 || raceID == 68)
                            adjustmentVector = new Vector3(CreatureModelTemplate.Race.NameplateAddedHeight * -1, 0, 0);

                        // Update all of the track sequences
                        for (int i = 0; i < nameplateBone.TranslationTrack.Values.Count; i++)
                            nameplateBone.TranslationTrack.AddValueToSequence(i, 0, adjustmentVector);
                    }
                }

                // Create bone lookups on a per submesh basis (which are grouped by material)
                // Note: Vertices are sorted by material and then by bone index already, so we can trust that here
                List<int> vertexMaterialIDs = new List<int>();
                for (int vertexIndex = 0; vertexIndex < ModelVertices.Count; vertexIndex++)
                    vertexMaterialIDs.Add(-1);
                foreach (TriangleFace modelTriangle in ModelTriangles)
                {
                    vertexMaterialIDs[modelTriangle.V1] = modelTriangle.MaterialIndex;
                    vertexMaterialIDs[modelTriangle.V2] = modelTriangle.MaterialIndex;
                    vertexMaterialIDs[modelTriangle.V3] = modelTriangle.MaterialIndex;
                }
                int currentMaterialID = -1;
                for (int vertexIndex = 0; vertexIndex < ModelVertices.Count; vertexIndex++)
                {
                    // Expand list if new material encountered
                    if (currentMaterialID != vertexMaterialIDs[vertexIndex])
                    {
                        currentMaterialID = vertexMaterialIDs[vertexIndex];
                        BoneLookupsByMaterialIndex.Add(currentMaterialID, new List<short>());
                    }

                    // Add lookup if new bone is encountered
                    byte curBoneIndex = ModelVertices[vertexIndex].BoneIndicesTrue[0];
                    if (BoneLookupsByMaterialIndex[currentMaterialID].Contains(curBoneIndex) == false)
                        BoneLookupsByMaterialIndex[currentMaterialID].Add(curBoneIndex);

                    // Add a lookup reference based on the lookup index
                    ModelVertices[vertexIndex].BoneIndicesLookup[0] = Convert.ToByte(BoneLookupsByMaterialIndex[currentMaterialID].Count - 1);
                }

                if (ModelType == ObjectModelType.Skeletal)
                {
                    // Set the portrait camera locations
                    SetupPortraitCamera();
                }
            }
        }

        private void BuildAnimationsForActiveDoodad(ActiveDoodadAnimType? activeDoodadAnimationType, float activeDoodadAnimModValue, int activeDoodadAnimTimeInMS)
        {
            // Associate all of the verts with the single bone that should already be set up
            if (ModelBones.Count == 0)
            {
                Logger.WriteError("Failed building animations for activeDoodad, as there are no bones");
                return;
            }
            for (int vertexIndex = 0; vertexIndex < ModelVertices.Count; vertexIndex++)
                ModelVertices[vertexIndex].BoneIndicesLookup[0] = 0;

            // Lay out the bone tracks for the 5 animations
            for (int i = 0; i < 5; i++)
            {
                ModelBones[0].ScaleTrack.AddSequence();
                ModelBones[0].RotationTrack.AddSequence();
                ModelBones[0].TranslationTrack.AddSequence();
            }
            ModelBones[0].ScaleTrack.InterpolationType = ObjectModelAnimationInterpolationType.Linear;
            ModelBones[0].RotationTrack.InterpolationType = ObjectModelAnimationInterpolationType.Linear;
            ModelBones[0].TranslationTrack.InterpolationType = ObjectModelAnimationInterpolationType.Linear;

            // Scale the mod value if translation
            switch (activeDoodadAnimationType)
            {
                case ActiveDoodadAnimType.SlideUpDown: activeDoodadAnimModValue *= Configuration.GENERATE_WORLD_SCALE; break;
                default: break; // nothing
            }

            // Open
            ObjectModelAnimation animationOpen = new ObjectModelAnimation();
            animationOpen.AnimationType = AnimationType.Open;
            animationOpen.BoundingBox = new BoundingBox(BoundingBox);
            animationOpen.BoundingRadius = BoundingSphereRadius;
            animationOpen.DurationInMS = Convert.ToUInt32(activeDoodadAnimTimeInMS);
            switch (activeDoodadAnimationType)
            {
                case ActiveDoodadAnimType.SlideUpDown:
                    {
                        ModelBones[0].TranslationTrack.AddValueToSequence(0, 0, new Vector3(0, 0, 0));
                        ModelBones[0].TranslationTrack.AddValueToSequence(0, Convert.ToUInt32(activeDoodadAnimTimeInMS), new Vector3(0, 0, activeDoodadAnimModValue));
                    } break;
                case ActiveDoodadAnimType.RotateAroundZ:
                    {
                        ModelBones[0].RotationTrack.AddValueToSequence(0, 0, new QuaternionShort());
                        ModelBones[0].RotationTrack.AddValueToSequence(0, Convert.ToUInt32(activeDoodadAnimTimeInMS), new QuaternionShort(0, 0, activeDoodadAnimModValue, 1));
                    } break;
                default: Logger.WriteError("BuildAnimationsForActiveDoodad failed due to unhandled ActiveDoodadAnimType of '" + activeDoodadAnimationType + "'"); return;
            }
            ModelAnimations.Add(animationOpen);

            // Opened
            ObjectModelAnimation animationOpened = new ObjectModelAnimation();
            animationOpened.AnimationType = AnimationType.Opened;
            animationOpened.BoundingBox = new BoundingBox(BoundingBox);
            animationOpened.BoundingRadius = BoundingSphereRadius;
            animationOpened.DurationInMS = Convert.ToUInt32(activeDoodadAnimTimeInMS);
            switch (activeDoodadAnimationType)
            {
                case ActiveDoodadAnimType.SlideUpDown:
                    {
                        ModelBones[0].TranslationTrack.AddValueToSequence(1, Convert.ToUInt32(activeDoodadAnimTimeInMS), new Vector3(0, 0, activeDoodadAnimModValue));
                    } break;
                case ActiveDoodadAnimType.RotateAroundZ:
                    {
                        ModelBones[0].RotationTrack.AddValueToSequence(1, Convert.ToUInt32(activeDoodadAnimTimeInMS), new QuaternionShort(0, 0, activeDoodadAnimModValue, 1));
                    }
                    break;
                default: Logger.WriteError("BuildAnimationsForActiveDoodad failed due to unhandled ActiveDoodadAnimType of '" + activeDoodadAnimationType + "'"); return;
            }
            ModelAnimations.Add(animationOpened);

            // Close
            ObjectModelAnimation animationClose = new ObjectModelAnimation();
            animationClose.AnimationType = AnimationType.Close;
            animationClose.BoundingBox = new BoundingBox(BoundingBox);
            animationClose.BoundingRadius = BoundingSphereRadius;
            animationClose.DurationInMS = Convert.ToUInt32(activeDoodadAnimTimeInMS);
            switch (activeDoodadAnimationType)
            {
                case ActiveDoodadAnimType.SlideUpDown:
                    {
                        ModelBones[0].TranslationTrack.AddValueToSequence(2, 0, new Vector3(0, 0, activeDoodadAnimModValue));
                        ModelBones[0].TranslationTrack.AddValueToSequence(2, Convert.ToUInt32(activeDoodadAnimTimeInMS), new Vector3(0, 0, 0));
                    }
                    break;
                case ActiveDoodadAnimType.RotateAroundZ:
                    {
                        ModelBones[0].RotationTrack.AddValueToSequence(2, 0, new QuaternionShort(0, 0, activeDoodadAnimModValue * -1, 1));
                        ModelBones[0].RotationTrack.AddValueToSequence(2, Convert.ToUInt32(activeDoodadAnimTimeInMS), new QuaternionShort());
                    }
                    break;
                default: Logger.WriteError("BuildAnimationsForActiveDoodad failed due to unhandled ActiveDoodadAnimType of '" + activeDoodadAnimationType + "'"); return;
            }
            ModelAnimations.Add(animationClose);

            // Stand
            ObjectModelAnimation animationStand = new ObjectModelAnimation();
            animationStand.AnimationType = AnimationType.Stand;
            animationStand.BoundingBox = new BoundingBox(BoundingBox);
            animationStand.BoundingRadius = BoundingSphereRadius;
            animationStand.DurationInMS = Convert.ToUInt32(activeDoodadAnimTimeInMS);
            ModelAnimations.Add(animationStand);

            // Closed
            ObjectModelAnimation animationClosed = new ObjectModelAnimation();
            animationClosed.AnimationType = AnimationType.Closed;
            animationClosed.BoundingBox = new BoundingBox(BoundingBox);
            animationClosed.BoundingRadius = BoundingSphereRadius;
            animationClosed.DurationInMS = Convert.ToUInt32(activeDoodadAnimTimeInMS);
            ModelAnimations.Add(animationClosed);    
        }

        private bool BuildSkeletonBonesAndLookups()
        {
            if (EQObjectModelData.SkeletonData.BoneStructures.Count == 0)
            {
                Logger.WriteError("Could not build skeleton information for object '" + Name + "' due to no EQ bone structures found");
                return false;
            }

            // Build out bones
            ModelBones.Clear();

            // Add a "main" bone in the start.  Note, all bone indices will need to increase by 1 as a result
            ObjectModelBone mainBone = new ObjectModelBone();
            mainBone.BoneNameEQ = "main";
            mainBone.ScaleTrack.InterpolationType = ObjectModelAnimationInterpolationType.None;
            mainBone.RotationTrack.InterpolationType = ObjectModelAnimationInterpolationType.None;
            mainBone.TranslationTrack.InterpolationType = ObjectModelAnimationInterpolationType.None;
            ModelBones.Add(mainBone);

            // Since a 'main' bone was added to the start, all other bone indices needs to be increased by 1
            foreach (ObjectModelVertex vertex in ModelVertices)
                vertex.BoneIndicesTrue[0]++;

            // First block in the bones themselves
            foreach (EQSkeleton.EQSkeletonBone eqBone in EQObjectModelData.SkeletonData.BoneStructures)
            {
                ObjectModelBone curBone = new ObjectModelBone();
                curBone.BoneNameEQ = eqBone.BoneName;
                if (curBone.BoneNameEQ == "root")
                {
                    curBone.ScaleTrack.InterpolationType = ObjectModelAnimationInterpolationType.None;
                    curBone.RotationTrack.InterpolationType = ObjectModelAnimationInterpolationType.None;
                    curBone.TranslationTrack.InterpolationType = ObjectModelAnimationInterpolationType.None;
                    curBone.ParentBone = 0;
                    curBone.ParentBoneNameEQ = "main";
                }
                else
                {
                    curBone.ScaleTrack.InterpolationType = ObjectModelAnimationInterpolationType.Linear;
                    curBone.RotationTrack.InterpolationType = ObjectModelAnimationInterpolationType.Linear;
                    curBone.TranslationTrack.InterpolationType = ObjectModelAnimationInterpolationType.Linear;
                }
                curBone.KeyBoneID = -1;
                ModelBones.Add(curBone);
            }

            // Next assign all children, accounting for main bone
            foreach (EQSkeleton.EQSkeletonBone eqBone in EQObjectModelData.SkeletonData.BoneStructures)
            {
                if (GetFirstBoneIndexForEQBoneNames(eqBone.BoneName) == -1)
                {
                    Logger.WriteError("Could not find a bone with name '" + eqBone.BoneName + "' for object '" + Name + "'");
                    continue;
                }
                int parentBoneIndex = GetFirstBoneIndexForEQBoneNames(eqBone.BoneName);
                for (int childIndex = 0; childIndex < eqBone.Children.Count; childIndex++)
                {
                    int modChildIndex = eqBone.Children[childIndex] + 1;
                    if (modChildIndex >= ModelBones.Count)
                    {
                        Logger.WriteError("Invalid bone index when trying to assign children for '" + eqBone.BoneName + "' for object '" + Name + "'");
                        continue;
                    }
                    ObjectModelBone childBone = ModelBones[modChildIndex];
                    childBone.ParentBone = Convert.ToInt16(parentBoneIndex);
                    childBone.ParentBoneNameEQ = ModelBones[parentBoneIndex].BoneNameEQ;
                }
            }

            // Set bone lookups
            ModelBoneKeyLookups.Clear();

            // Block out 27 key bones with blank
            for (int i = 0; i < 27; i++)
                ModelBoneKeyLookups.Add(-1);

            // Set root as a key
            SetKeyBone(KeyBoneType.Root);

            if (ModelType == ObjectModelType.Skeletal)
            {
                // Create the nameplate bone
                GenerateNameplateBone();

                // Create the bones required for events
                CreateEventBone("dth"); // DeathThud
                //CreateEventBone("cah"); // HandleCombatAnim
                CreateEventBone("css"); // PlayWeaponSwoosh
                //CreateEventBone("cpp"); // PlayCombatActionAnim
                CreateEventBone("fd1"); // PlayFidgetSound1
                CreateEventBone("fd2"); // PlayFidgetSound2
                CreateEventBone("fsd"); // HandleFootfallAnimEvent
                CreateEventBone("hit"); // PlayWoundAnimKit

                // Set any key bones
                SetKeyBone(KeyBoneType.Jaw);
                SetKeyBone(KeyBoneType._Breath);
                SetKeyBone(KeyBoneType._Name);
                SetKeyBone(KeyBoneType.CCH);
            }

            return true;
        }

        public void GenerateNameplateBone()
        {
            // Grab the base bone for it
            int initialBoneID = GetFirstBoneIndexForEQBoneNames("head_point", "he", "root");

            // Create the nameplate bone for it
            ObjectModelBone nameplateBone = new ObjectModelBone("nameplate", Convert.ToInt16(initialBoneID));
            ModelBones.Add(nameplateBone);
        }

        public bool CreateAndSetAnimations()
        {
            // Set the various animations (note: Do not change the order of the first 4)
            FindAndSetAnimationForType(AnimationType.Stand);
            
            if (ModelType == ObjectModelType.Skeletal)
            {
                FindAndSetAnimationForType(AnimationType.Stand); // Stand mid-idle
                FindAndSetAnimationForType(AnimationType.Stand, EQAnimationType.o01StandIdle); // Idle 1 / Fidget            
                FindAndSetAnimationForType(AnimationType.Stand, EQAnimationType.o01StandIdle); // Idle 2 / Fidget
                FindAndSetAnimationForType(AnimationType.AttackUnarmed);
                FindAndSetAnimationForType(AnimationType.Walk);
                FindAndSetAnimationForType(AnimationType.Run);
                FindAndSetAnimationForType(AnimationType.ShuffleLeft);
                FindAndSetAnimationForType(AnimationType.ShuffleRight);
                FindAndSetAnimationForType(AnimationType.Swim);
                FindAndSetAnimationForType(AnimationType.Death);
                FindAndSetAnimationForType(AnimationType.CombatWound);
                FindAndSetAnimationForType(AnimationType.CombatCritical);

                // Update the stand/fidget animation timers so that there is a fidget sometimes
                if (ModelAnimations.Count > 2 && ModelAnimations[1].AnimationType == AnimationType.Stand)
                {
                    // Update timers
                    int fidgetSliceAll = Convert.ToInt32(32767 * (Convert.ToDouble(Configuration.CREATURE_FIDGET_TIME_PERCENT) / 100));
                    int nonFidgetSliceAll = 32767 - fidgetSliceAll;
                    int nonFidgetSlice1 = nonFidgetSliceAll / 2;
                    int nonFidgetSlice2 = nonFidgetSliceAll - nonFidgetSlice1;
                    int fidgetSlice1 = fidgetSliceAll / 2;
                    int fidgetSlice2 = fidgetSliceAll - fidgetSlice1;
                    ModelAnimations[0].PlayFrequency = Convert.ToInt16(nonFidgetSlice1);
                    ModelAnimations[1].PlayFrequency = Convert.ToInt16(nonFidgetSlice2);
                    ModelAnimations[2].PlayFrequency = Convert.ToInt16(fidgetSlice1);
                    ModelAnimations[3].PlayFrequency = Convert.ToInt16(fidgetSlice2);

                    // Link animations
                    ModelAnimations[0].NextAnimation = 2;
                    ModelAnimations[1].NextAnimation = 3;
                    ModelAnimations[2].NextAnimation = 1;
                    ModelAnimations[3].NextAnimation = 0;
                }
            }

            // For every bone that has animation frames, add the first one to the end to close the animation sequences
            foreach (ObjectModelBone bone in ModelBones)
            {
                for (int i = 0; i < ModelAnimations.Count; i++)
                {
                    if (bone.ScaleTrack.Timestamps[i].Timestamps.Count > 1)
                    {
                        bone.ScaleTrack.Timestamps[i].Timestamps.Add(Convert.ToUInt32(ModelAnimations[i].DurationInMS));
                        bone.ScaleTrack.Values[i].Values.Add(bone.ScaleTrack.Values[i].Values[0]);
                    }
                    if (bone.RotationTrack.Timestamps[i].Timestamps.Count > 1)
                    {
                        QuaternionShort curRotation = new QuaternionShort(bone.RotationTrack.Values[i].Values[0]);
                        QuaternionShort priorRotation = new QuaternionShort(bone.RotationTrack.Values[i].Values[bone.RotationTrack.Values[i].Values.Count-1]);
                        curRotation.RecalculateToShortestFromOther(priorRotation);
                        bone.RotationTrack.Timestamps[i].Timestamps.Add(Convert.ToUInt32(ModelAnimations[i].DurationInMS));
                        bone.RotationTrack.Values[i].Values.Add(curRotation);
                    }
                    if (bone.TranslationTrack.Timestamps[i].Timestamps.Count > 1)
                    {
                        bone.TranslationTrack.Timestamps[i].Timestamps.Add(Convert.ToUInt32(ModelAnimations[i].DurationInMS));
                        bone.TranslationTrack.Values[i].Values.Add(bone.TranslationTrack.Values[i].Values[0]);
                    }
                }
            }


            if (ModelAnimations.Count == 0)
                Logger.WriteError("Zero animations for skeletal model object '" + Name + "', so it will crash if you try to load it");

            // Set the animation lookups
            SetAllAnimationLookups();

            return true;
        }

        public void SetupPortraitCamera()
        {
            if (ModelAnimations.Count == 0)
            {
                Logger.WriteError("Could not set the portrait camera because there were not enough animations");
                return;
            }

            // Use the 'name' bone since it points at the head
            int headBoneID = GetFirstBoneIndexForEQBoneNames("head_point", "he", "pe", "root");
            if (headBoneID == -1)
            {
                Logger.WriteError("Could not set the portrait camera because the head bone was indexed at -1");
                return;
            }

            // Using the standing animation location, make this the base target point for the camera
            ObjectModelBone headBone = ModelBones[headBoneID];
            ObjectModelBone curBone = headBone;
            while (curBone.TranslationTrack.Values[0].Values.Count == 0 && curBone.ParentBone != -1)
                curBone = ModelBones[curBone.ParentBone];
            if (curBone.TranslationTrack.Values[0].Values.Count == 0)
            {
                Logger.WriteError("Could not set the portrait camera because a valid head bone could not be located");
                return;
            }
            Vector3 headLocation = new Vector3();

            // Add all of the transformation data to get the actual head position
            bool moreToProcess = true;
            while (moreToProcess)
            {
                // Rotate
                if (curBone.RotationTrack.Values[0].Values.Count > 0)
                    headLocation.Rotate(curBone.RotationTrack.Values[0].Values[0]);

                // Scale
                if (curBone.ScaleTrack.Values[0].Values.Count > 0)
                    headLocation.Scale(curBone.ScaleTrack.Values[0].Values[0].X);

                // Translate
                if (curBone.TranslationTrack.Values[0].Values.Count > 0)
                    headLocation += curBone.TranslationTrack.Values[0].Values[0];

                if (curBone.ParentBone != -1)
                {
                    curBone = ModelBones[curBone.ParentBone];
                    moreToProcess = true;
                }
                else
                    moreToProcess = false;
            }
            PortraitCameraTargetPosition = headLocation;
            PortraitCameraPosition = headLocation;
            if (CreatureModelTemplate != null)
            {
                PortraitCameraPosition += CreatureModelTemplate.Race.CameraPositionMod;
                PortraitCameraTargetPosition += CreatureModelTemplate.Race.CameraTargetPositionMod;
            }            
        }

        public void FindAndSetAnimationForType(AnimationType animationType, EQAnimationType overrideEQAnimationType = EQAnimationType.Unknown)
        {
            Logger.WriteDetail("Seeking animation to build to wow type '" + animationType.ToString() + "' for object '" + Name + "'");

            // Determine what animations can work
            List<EQAnimationType> compatibleAnimationTypes = new List<EQAnimationType>();
            if (overrideEQAnimationType == EQAnimationType.Unknown )
                compatibleAnimationTypes = ObjectModelAnimation.GetPrioritizedCompatibleEQAnimationTypes(animationType);
            else
                compatibleAnimationTypes.Add(overrideEQAnimationType);
            foreach(EQAnimationType compatibleAnimationType in compatibleAnimationTypes)
            {
                // Look for a match, and process it if found
                foreach(var animation in EQObjectModelData.Animations)
                {
                    if (animation.Value.EQAnimationType == compatibleAnimationType)
                    {
                        // Capture and set this animation
                        Logger.WriteDetail("Found usable candidate, setting to eq type '" + animation.Key + "' for object '" + Name + "'");

                        // Create the base animation object
                        ObjectModelAnimation newAnimation = new ObjectModelAnimation();
                        newAnimation.DurationInMS = Convert.ToUInt32(animation.Value.TotalTimeInMS);
                        newAnimation.AnimationType = animationType;
                        newAnimation.EQAnimationType = animation.Value.EQAnimationType;
                        newAnimation.BoundingBox = new BoundingBox(BoundingBox);
                        newAnimation.BoundingRadius = BoundingSphereRadius;
                        newAnimation.AliasNext = Convert.ToUInt16(ModelAnimations.Count); // The next animation is itself, so it's a loop (TODO: Change this)
                        ModelAnimations.Add(newAnimation);

                        // Create an animation track sequence for each bone
                        foreach (ObjectModelBone bone in ModelBones)
                        {
                            bone.ScaleTrack.AddSequence();
                            bone.RotationTrack.AddSequence();
                            bone.TranslationTrack.AddSequence();
                        }

                        // Add the animation-bone transformations to the bone objects for each frame
                        Dictionary<string, int> curTimestampsByBoneName = new Dictionary<string, int>();
                        for (int i = 0; i < animation.Value.AnimationFrames.Count; i++)
                        {
                            Animation.BoneAnimationFrame animationFrame = animationFrame = animation.Value.AnimationFrames[i];
                            if (DoesBoneExistForName(animationFrame.GetBoneName()) == false)
                            {
                                Logger.WriteDetail("For object '" + Name + "' skipping bone with name '" + animationFrame.GetBoneName() + "' when mapping animation since it couldn't be found");
                                continue;
                            }
                            
                            ObjectModelBone curBone = GetBoneWithName(animationFrame.GetBoneName());

                            // Root always just has one frame
                            if (animationFrame.GetBoneName() == "root")
                            {
                                curBone.ScaleTrack.AddValueToLastSequence(0, new Vector3(1, 1, 1));
                                curBone.RotationTrack.AddValueToLastSequence(0, new QuaternionShort());
                                curBone.TranslationTrack.AddValueToLastSequence(0, new Vector3(0, 0, 0));
                            }
                            // Regular bones append the animation frame
                            else
                            {
                                // Format and transform the animation frame values from EQ to WoW
                                Vector3 frameTranslation = new Vector3(animationFrame.XPosition * Configuration.GENERATE_CREATURE_SCALE * ModelScalePreWorldScale,
                                                                       animationFrame.YPosition * Configuration.GENERATE_CREATURE_SCALE * ModelScalePreWorldScale,
                                                                       animationFrame.ZPosition * Configuration.GENERATE_CREATURE_SCALE * ModelScalePreWorldScale);
                                Vector3 frameScale = new Vector3(animationFrame.Scale, animationFrame.Scale, animationFrame.Scale);
                                QuaternionShort frameRotation = new QuaternionShort(-animationFrame.XRotation,
                                                                                    -animationFrame.YRotation,
                                                                                    -animationFrame.ZRotation,
                                                                                    animationFrame.WRotation);

                                // SLERP the rotation to fix it on translation to WoW.  If there's a previous frame, base off that
                                if (curBone.RotationTrack.Values.Count > 0 && curBone.RotationTrack.Values[curBone.RotationTrack.Values.Count-1].Values.Count > 0)
                                {
                                    QuaternionShort priorRotation = curBone.RotationTrack.Values[curBone.RotationTrack.Values.Count - 1].Values[curBone.RotationTrack.Values[curBone.RotationTrack.Values.Count - 1].Values.Count - 1];
                                    frameRotation.RecalculateToShortestFromOther(priorRotation);
                                }
                                else
                                    frameRotation.RecalculateToShortest();

                                // For bones that connect to root, add the height mod
                                if (curBone.ParentBoneNameEQ == "root")
                                    frameTranslation.Z += ModelLiftPreWorldScale * Configuration.GENERATE_CREATURE_SCALE;

                                // Calculate the frame start time
                                UInt32 curTimestamp = 0;
                                if (curTimestampsByBoneName.ContainsKey(animationFrame.GetBoneName()) == false)
                                    curTimestampsByBoneName.Add(animationFrame.GetBoneName(), 0);
                                else
                                    curTimestamp = Convert.ToUInt32(curTimestampsByBoneName[animationFrame.GetBoneName()]);

                                // Add the values
                                curBone.ScaleTrack.AddValueToLastSequence(curTimestamp, frameScale);
                                curBone.RotationTrack.AddValueToLastSequence(curTimestamp, frameRotation);
                                curBone.TranslationTrack.AddValueToLastSequence(curTimestamp, frameTranslation);

                                // Increment the frame start for next
                                curTimestampsByBoneName[animationFrame.GetBoneName()] += animationFrame.FramesMS;
                            }
                        }

                        // Animation set, so exit
                        return;
                    }
                }
            }

            Logger.WriteDetail("No animation candidate was found for object '" + Name + "'");
        }

        public int GetFirstBoneIndexForEQBoneNames(params string[] eqBoneNames)
        {
            foreach (string eqBoneName in eqBoneNames)
            {
                for (int i = 0; i < ModelBones.Count; i++)
                {
                    ObjectModelBone curBone = ModelBones[i];
                    if (curBone.BoneNameEQ == eqBoneName)
                        return i;
                }
            }
            return -1;
        }

        public int GetBoneIndexForAttachmentType(ObjectModelAttachmentType attachmentType)
        {
            // Default to root bone
            int returnValue = -1;
            if (ModelBoneKeyLookups.Count >= 27)
                returnValue = ModelBoneKeyLookups[26];

            switch (attachmentType)
            {
                case ObjectModelAttachmentType.Chest:
                case ObjectModelAttachmentType.ChestBloodFront:
                    {
                        returnValue = GetFirstBoneIndexForEQBoneNames("ch", "pe", "root");
                    } break;
                case ObjectModelAttachmentType.ChestBloodBack:
                    {
                        returnValue = GetFirstBoneIndexForEQBoneNames("ch", "pe", "root");
                    } break;
                case ObjectModelAttachmentType.MouthBreath:
                    {
                        returnValue = GetFirstBoneIndexForEQBoneNames("ja", "head_point", "he", "ch", "pe", "root");
                    } break;
                case ObjectModelAttachmentType.GroundBase:
                    {
                        returnValue = GetFirstBoneIndexForEQBoneNames("root");
                    } break;
                case ObjectModelAttachmentType.PlayerName:
                    {
                        returnValue = GetFirstBoneIndexForEQBoneNames("nameplate");
                    }
                    break;
                case ObjectModelAttachmentType.HeadTop:
                    {
                        returnValue = GetFirstBoneIndexForEQBoneNames("head_point", "pe", "root");
                    } break;
                case ObjectModelAttachmentType.HandLeft_ItemVisual2:
                case ObjectModelAttachmentType.SpellLeftHand:
                    {
                        returnValue = GetFirstBoneIndexForEQBoneNames("l_point", "pe", "root");
                    } break;
                case ObjectModelAttachmentType.HandRight_ItemVisual1:
                case ObjectModelAttachmentType.SpellRightHand:
                    {
                        returnValue = GetFirstBoneIndexForEQBoneNames("r_point", "pe", "root");
                    } break;
                default:
                    {
                        Logger.WriteError("GetBoneIndexForAttachmentType error - Unhandled attachment type of '" + attachmentType.ToString() + "' for object model '" + Name + "'");
                    } break;
            }

            return returnValue;
        }

        public int GetBoneIndexForKeyBoneType(KeyBoneType keyBoneType)
        {
            // Default to root bone
            int returnValue = -1;
            if (ModelBoneKeyLookups.Count >= 27)
                returnValue = ModelBoneKeyLookups[26];

            switch (keyBoneType)
            {
                case KeyBoneType.Root:
                    {
                        returnValue = GetFirstBoneIndexForEQBoneNames("root");
                    } break;
                case KeyBoneType.Jaw:
                    {
                        returnValue = GetFirstBoneIndexForEQBoneNames("ja", "head_point", "he", "ch", "pe", "root");
                    } break;
                case KeyBoneType._Breath:
                    {
                        returnValue = GetFirstBoneIndexForEQBoneNames("ja", "head_point", "he", "ch", "pe", "root");
                    } break;
                case KeyBoneType._Name:
                    {
                        returnValue = GetFirstBoneIndexForEQBoneNames("nameplate");
                    } break;
                case KeyBoneType.CCH:
                    {
                        returnValue = GetFirstBoneIndexForEQBoneNames("head_point", "root"); // Complete guess as I don't know what CCH is
                    } break;
                default:
                    {
                        Logger.WriteError("GetBoneIndexForKeyBoneType error - Unhandled key bone type of '" + keyBoneType.ToString() + "' for object model '" + Name + "'");
                    }
                    break;
            }

            return returnValue;
        }

        private void CreateEventBone(string eventBoneName)
        {
            // Get parent bone
            int parentBoneID = -1;
            switch (eventBoneName.ToLower())
            {
                case "cah":
                case "css":
                case "cpp":
                case "fd1":
                case "fd2":
                case "fsd":
                case "hit":
                    {
                        // For now, let's just use root
                        // TODO: Use something other than root?
                        parentBoneID = GetFirstBoneIndexForEQBoneNames("root"); 
                    } break;
                default: parentBoneID = -1; break;
            }

            // Create the bone
            ObjectModelBone eventBone = new ObjectModelBone(eventBoneName, Convert.ToInt16(parentBoneID));
            ModelBones.Add(eventBone);
        }

        private void SetAllAnimationLookups()
        {
            // Set the animations through 49 (Attack Rifle)
            AnimationLookups.Clear();
            for (Int16 i = 0; i <= 49; i++)
                AnimationLookups.Add(-1);
            SetFirstUnusedAnimationIndexForAnimationType(AnimationType.Stand);
            if (ModelType == ObjectModelType.Skeletal)
            {
                SetFirstUnusedAnimationIndexForAnimationType(AnimationType.Walk);
                SetFirstUnusedAnimationIndexForAnimationType(AnimationType.Run);
                SetFirstUnusedAnimationIndexForAnimationType(AnimationType.CombatWound);
                SetFirstUnusedAnimationIndexForAnimationType(AnimationType.CombatCritical);
                SetFirstUnusedAnimationIndexForAnimationType(AnimationType.StandWound);
                SetFirstUnusedAnimationIndexForAnimationType(AnimationType.AttackUnarmed);
                SetFirstUnusedAnimationIndexForAnimationType(AnimationType.Death);
                SetFirstUnusedAnimationIndexForAnimationType(AnimationType.Swim);
            }
        }

        private void SetFirstUnusedAnimationIndexForAnimationType(AnimationType animationType)
        {
            List<EQAnimationType> validEQAnimationTypes = ObjectModelAnimation.GetPrioritizedCompatibleEQAnimationTypes(animationType);
            int firstAnimationIndex = GetFirstAnimationIndexForEQAnimationTypes(validEQAnimationTypes.ToArray());
            if (firstAnimationIndex == -1)
                return;
            for (int i = 0; i < AnimationLookups.Count; i++)
            {
                if (AnimationLookups[i] == firstAnimationIndex)
                    return;
            }
            AnimationLookups[Convert.ToInt32(animationType)] = Convert.ToInt16(firstAnimationIndex);
        }

        private int GetFirstAnimationIndexForEQAnimationTypes(params EQAnimationType[] eqAnimationTypes)
        {
            foreach (EQAnimationType eqAnimationType in eqAnimationTypes)
            {
                for (int i = 0; i < ModelAnimations.Count; i++)
                {
                    ObjectModelAnimation curAnimation = ModelAnimations[i];
                    if (curAnimation.EQAnimationType == eqAnimationType)
                        return i;
                }
            }
            return -1;
        }

        private void SetKeyBone(KeyBoneType keyBoneType)
        {
            int boneIndex = GetBoneIndexForKeyBoneType(keyBoneType);
            ModelBoneKeyLookups[Convert.ToInt32(keyBoneType)] = Convert.ToInt16(boneIndex);
            if (boneIndex != -1)
                ModelBones[boneIndex].KeyBoneID = Convert.ToInt32(keyBoneType);
        }
              
        private ObjectModelBone GetBoneWithName(string name)
        {
            foreach (ObjectModelBone bone in ModelBones)
                if (bone.BoneNameEQ == name)
                    return bone;

            Logger.WriteError("No bone named '" + name + "' for object '" + Name + "'");
            return new ObjectModelBone();
        }

        private bool DoesBoneExistForName(string name)
        {
            foreach (ObjectModelBone bone in ModelBones)
                if (bone.BoneNameEQ == name)
                    return true;
            return false;
        }

        private void GenerateModelVertices(MeshData meshData, List<Vector3> collisionVertices, List<TriangleFace> collisionTriangleFaces)
        {
            if (Configuration.OBJECT_STATIC_RENDER_AS_COLLISION == true && (ModelType == ObjectModelType.Skeletal || ModelType == ObjectModelType.StaticDoodad))
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
            // Purge any invalid material references
            // TODO: Look into making this work for non-skeletal
            if (ModelType == ObjectModelType.Skeletal)
            {
                List<Material> updatedMaterialList;
                meshData.RemoveInvalidMaterialReferences(initialMaterials, out updatedMaterialList);
                initialMaterials = updatedMaterialList;
            }

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

            // Perform color tinting if this was a creature and had colors set
            if (CreatureModelTemplate != null && CreatureModelTemplate.ColorTint != null)
            {
                CreatureTemplateColorTint colorTint = CreatureModelTemplate.ColorTint;

                // Head
                if (colorTint.HelmColor != null)
                {
                    SetMaterialColorByTextureNameFragment("helm", colorTint.ID, colorTint.HelmColor);
                    SetMaterialColorByTextureNameFragment("chain", colorTint.ID, colorTint.HelmColor);
                }

                // Chest
                if (colorTint.ChestColor != null)
                {
                    SetMaterialColorByTextureNameFragmentAtPosition("ch", 3, colorTint.ID, colorTint.ChestColor);
                    SetMaterialColorByTextureNameFragment("clk", colorTint.ID, colorTint.ChestColor);
                }

                // Arms
                if (colorTint.ArmsColor != null)
                    SetMaterialColorByTextureNameFragmentAtPosition("ua", 3, colorTint.ID, colorTint.ArmsColor);

                // Bracer
                if (colorTint.BracerColor != null)
                    SetMaterialColorByTextureNameFragmentAtPosition("fa", 3, colorTint.ID, colorTint.BracerColor);

                // Hands
                if (colorTint.HandsColor != null)
                    SetMaterialColorByTextureNameFragmentAtPosition("hn", 3, colorTint.ID, colorTint.HandsColor);

                // Legs
                if (colorTint.LegsColor != null)
                    SetMaterialColorByTextureNameFragmentAtPosition("lg", 3, colorTint.ID, colorTint.LegsColor);

                // Feet
                if (colorTint.FeetColor != null)
                    SetMaterialColorByTextureNameFragmentAtPosition("ft", 3, colorTint.ID, colorTint.FeetColor);
            }

            // Generate model textures
            foreach(ObjectModelMaterial modelMaterial in ModelMaterials)
            {
                ObjectModelTexture newModelTexture = new ObjectModelTexture();
                newModelTexture.TextureName = modelMaterial.Material.TextureNames[0];
                ModelTextures.Add(newModelTexture);
            }
        }

        private void SetMaterialColorByTextureNameFragment(string textureNameFragmentToMatch, int colorID, ColorRGBA color)
        {
            foreach (ObjectModelMaterial objectModelMaterial in ModelMaterials)
            {
                if (objectModelMaterial.Material.TextureNames.Count > 0 &&
                    objectModelMaterial.Material.TextureNames[0].Contains(textureNameFragmentToMatch) == true)
                {
                    string newTextureName = objectModelMaterial.Material.TextureNames[0] + "_c" + colorID;
                    ImageTool.GenerateColoredCreatureTexture(objectModelMaterial.Material.TextureNames[0], newTextureName, color);
                    objectModelMaterial.Material.TextureNames[0] = newTextureName;
                    if (GeneratedTextureNames.Contains(newTextureName) == false)
                        GeneratedTextureNames.Add(newTextureName);
                }
            }
        }

        private void SetMaterialColorByTextureNameFragmentAtPosition(string textureNameFragmentToMatch, int positionOffset, int colorID, ColorRGBA color)
        {
            foreach (ObjectModelMaterial objectModelMaterial in ModelMaterials)
            {
                if (objectModelMaterial.Material.TextureNames.Count > 0 &&
                    objectModelMaterial.Material.TextureNames[0].Length >= (textureNameFragmentToMatch.Length + positionOffset) &&
                    objectModelMaterial.Material.TextureNames[0].Substring(positionOffset, textureNameFragmentToMatch.Length) == textureNameFragmentToMatch)
                {
                    string newTextureName = objectModelMaterial.Material.TextureNames[0] + "_c" + colorID;
                    ImageTool.GenerateColoredCreatureTexture(objectModelMaterial.Material.TextureNames[0], newTextureName, color);
                    objectModelMaterial.Material.TextureNames[0] = newTextureName;
                    if (GeneratedTextureNames.Contains(newTextureName) == false)
                        GeneratedTextureNames.Add(newTextureName);
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
                        float extendDistance = Configuration.OBJECT_STATIC_LADDER_EXTEND_DISTANCE * Configuration.GENERATE_WORLD_SCALE;
                        float stepDistance = Configuration.OBJECT_STATIC_LADDER_STEP_DISTANCE * Configuration.GENERATE_WORLD_SCALE;

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
            if (collisionVertices.Count == 0 && (ModelType == ObjectModelType.Skeletal || ModelType == ObjectModelType.StaticDoodad))
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
                        Logger.WriteDetail("Attempted to build collision data for object '" + Name + "', but could not find material with ID '" + face.MaterialIndex + "'");
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
            BoundingBox = BoundingBox.GenerateBoxFromVectors(ModelVertices, Configuration.OBJECT_STATIC_MIN_BOUNDING_BOX_SIZE);
            BoundingSphereRadius = BoundingBox.FurthestPointDistanceFromCenter();
            CollisionBoundingBox = BoundingBox.GenerateBoxFromVectors(CollisionPositions, Configuration.GENERATE_ADDED_BOUNDARY_AMOUNT);
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
                    if (meshData.BoneIDs.Count > 0)
                        meshData.BoneIDs.Add(meshData.BoneIDs[vi]);
                    if (meshData.VertexColors.Count > 0)
                        meshData.VertexColors.Add(new ColorRGBA(meshData.VertexColors[vi]));
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
