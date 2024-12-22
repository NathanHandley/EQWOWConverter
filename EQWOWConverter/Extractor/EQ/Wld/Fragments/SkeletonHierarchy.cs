using System;
using System.Collections.Generic;
using System.Linq;
using GlmSharp;
using LanternExtractor.EQ.Wld.DataTypes;
using LanternExtractor.EQ.Wld.Helpers;
using LanternExtractor.Infrastructure;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld.Fragments
{
    /// <summary>
    /// SkeletonHierarchy (0x10)
    /// Internal name: _HS_DEF
    /// Describes the layout of a complete skeleton and which pieces connect to eachother
    /// </summary>
    public class SkeletonHierarchy : WldFragment
    {
        public List<Mesh> Meshes { get; private set; }
        public List<LegacyMesh> AlternateMeshes { get; private set; }
        public List<SkeletonBone> Skeleton { get; set; }

        private PolyhedronReference _fragment18Reference;

        public string ModelBase { get; set; }
        public bool IsAssigned { get; set; }
        private Dictionary<string, SkeletonBone> SkeletonPieceDictionary { get; set; }

        public Dictionary<string, Animation> Animations = new Dictionary<string, Animation>();

        public Dictionary<int, string> BoneMappingClean = new Dictionary<int, string>();
        public Dictionary<int, string> BoneMapping = new Dictionary<int, string>();

        public float BoundingRadius;

        public List<Mesh> SecondaryMeshes = new List<Mesh>();
        public List<LegacyMesh> SecondaryAlternateMeshes = new List<LegacyMesh>();

        private bool _hasBuiltData;

        public override void Initialize(int index, int size, byte[] data,
            List<WldFragment> fragments,
            Dictionary<int, string> stringHash, bool isNewWldFormat, ILogger logger)
        {
            base.Initialize(index, size, data, fragments, stringHash, isNewWldFormat, logger);

            Skeleton = new List<SkeletonBone>();
            Meshes = new List<Mesh>();
            AlternateMeshes = new List<LegacyMesh>();
            SkeletonPieceDictionary = new Dictionary<string, SkeletonBone>();

            Name = stringHash[-Reader.ReadInt32()];
            ModelBase = FragmentNameCleaner.CleanName(this, true);

            // Always 2 when used in main zone, and object files.
            // This means, it has a bounding radius
            // Some differences in character + model archives
            // Confirmed
            int flags = Reader.ReadInt32();

            var ba = new BitAnalyzer(flags);

            bool hasUnknownParams = ba.IsBitSet(0);
            bool hasBoundingRadius = ba.IsBitSet(1);
            bool hasMeshReferences = ba.IsBitSet(9);

            // Number of bones in the skeleton
            int boneCount = Reader.ReadInt32();

            // Fragment 18 reference
            int fragment18Reference = Reader.ReadInt32() - 1;

            if (fragment18Reference > 0)
            {
                _fragment18Reference = fragments[fragment18Reference] as PolyhedronReference;
            }

            // Three sequential DWORDs
            // This will never be hit for object animations.
            if (hasUnknownParams)
            {
                Reader.BaseStream.Position += 3 * sizeof(int);
            }

            // This is the sphere radius checked against the frustum to cull this object
            if (hasBoundingRadius)
            {
                BoundingRadius = Reader.ReadSingle();
            }

            for (int i = 0; i < boneCount; ++i)
            {
                // An index into the string has to get this bone's name
                int boneNameIndex = Reader.ReadInt32();
                string boneName = string.Empty;

                if (stringHash.ContainsKey(-boneNameIndex))
                {
                    boneName = stringHash[-boneNameIndex];
                }

                // Always 0 for object bones
                // Confirmed
                int boneFlags = Reader.ReadInt32();

                // Reference to a bone track
                // Confirmed - is never a bad reference
                int trackReferenceIndex = Reader.ReadInt32() - 1;

                TrackFragment track = fragments[trackReferenceIndex] as TrackFragment;
                AddPoseTrack(track, boneName);

                var pieceNew = new SkeletonBone
                {
                    Index = i,
                    Track = track,
                    Name = boneName
                };

                pieceNew.Track.IsPoseAnimation = true;
                pieceNew.AnimationTracks = new Dictionary<string, TrackFragment>();

                BoneMappingClean[i] = Animation.CleanBoneAndStripBase(boneName, ModelBase);
                BoneMapping[i] = boneName;

                if (pieceNew.Track == null)
                {
                    logger.LogError("Unable to link track reference!");
                }

                int meshReferenceIndex = Reader.ReadInt32() - 1;

                if (meshReferenceIndex < 0)
                {
                    string name = stringHash[-meshReferenceIndex - 1];
                }
                else if (meshReferenceIndex != 0)
                {
                    pieceNew.MeshReference = fragments[meshReferenceIndex] as MeshReference;

                    if (pieceNew.MeshReference == null)
                    {
                        pieceNew.ParticleCloud = fragments[meshReferenceIndex] as ParticleCloud;
                    }

                    if (pieceNew.Name == "root")
                    {
                        pieceNew.Name = FragmentNameCleaner.CleanName(pieceNew.MeshReference.Mesh);
                    }
                }

                int childCount = Reader.ReadInt32();

                pieceNew.Children = new List<int>();

                for (int j = 0; j < childCount; ++j)
                {
                    int childIndex = Reader.ReadInt32();
                    pieceNew.Children.Add(childIndex);
                }

                Skeleton.Add(pieceNew);

                if (pieceNew.Name != "")
                {
                    if (!SkeletonPieceDictionary.ContainsKey(pieceNew.Name))
                    {
                        SkeletonPieceDictionary.Add(pieceNew.Name, pieceNew);
                    }
                }
            }

            // Read in mesh references
            // All meshes will have vertex bone assignments
            if (hasMeshReferences)
            {
                int size2 = Reader.ReadInt32();

                for (int i = 0; i < size2; ++i)
                {
                    int meshRefIndex = Reader.ReadInt32() - 1;

                    MeshReference meshRef = fragments[meshRefIndex] as MeshReference;

                    if (meshRef?.Mesh != null)
                    {
                        if (Meshes.All(x => x.Name != meshRef.Mesh.Name))
                        {
                            Meshes.Add(meshRef.Mesh);
                            meshRef.Mesh.IsHandled = true;
                        }
                    }

                    if (meshRef?.LegacyMesh != null)
                    {
                        if (AlternateMeshes.All(x => x.Name != meshRef.LegacyMesh.Name))
                        {
                            AlternateMeshes.Add(meshRef.LegacyMesh);
                        }
                    }
                }

                Meshes = Meshes.OrderBy(x => x.Name).ToList();

                List<int> unknown = new List<int>();

                for (int i = 0; i < size2; ++i)
                {
                    unknown.Add(Reader.ReadInt32());
                }
            }
        }

        public void BuildSkeletonData(bool stripModelBase)
        {
            if (_hasBuiltData)
            {
                return;
            }

            BuildSkeletonTreeData(0, Skeleton, null, string.Empty,
                string.Empty, string.Empty, stripModelBase);
            _hasBuiltData = true;
        }

        public override void OutputInfo(ILogger logger)
        {
            base.OutputInfo(logger);
            logger.LogInfo("-----");
            logger.LogInfo("0x10: Skeleton pieces: " + Skeleton.Count);
        }

        private void AddPoseTrack(TrackFragment track, string pieceName)
        {
            if (!Animations.ContainsKey("pos"))
            {
                Animations["pos"] = new Animation();
            }

            Animations["pos"].AddTrack(track, pieceName, Animation.CleanBoneName(pieceName),
                Animation.CleanBoneAndStripBase(pieceName, ModelBase));
            track.TrackDefFragment.IsAssigned = true;
            track.IsProcessed = true;
            track.IsPoseAnimation = true;
        }

        public void AddTrackDataEquipment(TrackFragment track, string boneName, bool isDefault = false)
        {
            string animationName = string.Empty;
            string modelName = string.Empty;
            string pieceName = string.Empty;

            string cleanedName = FragmentNameCleaner.CleanName(track, true);

            if (isDefault)
            {
                animationName = "pos";
                modelName = ModelBase;
                cleanedName = cleanedName.Replace(ModelBase, String.Empty);
                pieceName = cleanedName == string.Empty ? "root" : cleanedName;
            }
            else
            {
                if (cleanedName.Length <= 3)
                {
                    return;
                }

                animationName = cleanedName.Substring(0, 3);
                cleanedName = cleanedName.Remove(0, 3);

                if (cleanedName.Length < 3)
                {
                    return;
                }

                modelName = ModelBase;
                pieceName = boneName;

                if (pieceName == string.Empty)
                {
                    pieceName = "root";
                }
            }

            track.SetTrackData(modelName, animationName, pieceName);

            if (Animations.ContainsKey(track.AnimationName))
            {
                if (modelName == ModelBase && ModelBase != Animations[animationName].AnimModelBase)
                {
                    Animations.Remove(animationName);
                }

                if (modelName != ModelBase && ModelBase == Animations[animationName].AnimModelBase)
                {
                    return;
                }
            }

            if (!Animations.ContainsKey(track.AnimationName))
            {
                Animations[track.AnimationName] = new Animation();
            }

            Animations[track.AnimationName].AddTrack(track, track.PieceName, Animation.CleanBoneName(track.PieceName),
                Animation.CleanBoneAndStripBase(track.PieceName, ModelBase));
            track.TrackDefFragment.IsAssigned = true;
            track.IsProcessed = true;
        }

        public void AddTrackData(TrackFragment track, bool isDefault = false)
        {
            string animationName = string.Empty;
            string modelName = string.Empty;
            string pieceName = string.Empty;

            string cleanedName = FragmentNameCleaner.CleanName(track, true);

            if (isDefault)
            {
                animationName = "pos";
                modelName = ModelBase;
                cleanedName = cleanedName.Replace(ModelBase, String.Empty);
                pieceName = cleanedName == string.Empty ? "root" : cleanedName;
            }
            else
            {
                if (cleanedName.Length <= 3)
                {
                    return;
                }

                animationName = cleanedName.Substring(0, 3);
                cleanedName = cleanedName.Remove(0, 3);

                if (cleanedName.Length < 3)
                {
                    return;
                }

                modelName = cleanedName.Substring(0, 3);
                cleanedName = cleanedName.Remove(0, 3);
                pieceName = cleanedName;

                if (pieceName == string.Empty)
                {
                    pieceName = "root";
                }
            }

            track.SetTrackData(modelName, animationName, pieceName);

            if (Animations.ContainsKey(track.AnimationName))
            {
                if (modelName == ModelBase && ModelBase != Animations[animationName].AnimModelBase)
                {
                    Animations.Remove(animationName);
                }

                if (modelName != ModelBase && ModelBase == Animations[animationName].AnimModelBase)
                {
                    return;
                }
            }

            if (!Animations.ContainsKey(track.AnimationName))
            {
                Animations[track.AnimationName] = new Animation();
            }

            Animations[track.AnimationName]
                .AddTrack(track, track.Name, Animation.CleanBoneName(track.PieceName),
                    Animation.CleanBoneAndStripBase(track.PieceName, ModelBase));
            track.TrackDefFragment.IsAssigned = true;
            track.IsProcessed = true;
        }

        private void BuildSkeletonTreeData(int index, List<SkeletonBone> treeNodes, SkeletonBone parent,
            string runningName, string runningNameCleaned, string runningIndex, bool stripModelBase)
        {
            SkeletonBone bone = treeNodes[index];
            bone.Parent = parent;
            bone.CleanedName = CleanBoneName(bone.Name, stripModelBase);
            BoneMappingClean[index] = bone.CleanedName;

            if (bone.Name != string.Empty)
            {
                runningIndex += bone.Index + "/";
            }

            runningName += bone.Name;
            runningNameCleaned += bone.CleanedName;

            bone.FullPath = runningName;
            bone.CleanedFullPath = runningNameCleaned;

            if (bone.Children.Count == 0)
            {
                return;
            }

            runningName += "/";
            runningNameCleaned += "/";

            foreach (var childNode in bone.Children)
            {
                BuildSkeletonTreeData(childNode, treeNodes, bone, runningName, runningNameCleaned, runningIndex,
                    stripModelBase);
            }
        }

        private string CleanBoneName(string nodeName, bool stripModelBase)
        {
            nodeName = nodeName.Replace("_DAG", "");
            nodeName = nodeName.ToLower();
            if (stripModelBase)
            {
                nodeName = nodeName.Replace(ModelBase, string.Empty);
            }

            nodeName += nodeName.Length == 0 ? "root" : string.Empty;
            return nodeName;
        }

        public void AddAdditionalMesh(Mesh mesh)
        {
            if (Meshes.Any(x => x.Name == mesh.Name)
                || SecondaryMeshes.Any(x => x.Name == mesh.Name))
            {
                return;
            }

            if (mesh.MobPieces.Count == 0)
            {
                return;
            }

            SecondaryMeshes.Add(mesh);
            SecondaryMeshes = SecondaryMeshes.OrderBy(x => x.Name).ToList();
        }

        public void AddAdditionalAlternateMesh(LegacyMesh mesh)
        {
            if (AlternateMeshes.Any(x => x.Name == mesh.Name)
                || SecondaryAlternateMeshes.Any(x => x.Name == mesh.Name))
            {
                return;
            }

            if (mesh.MobPieces.Count == 0)
            {
                return;
            }

            SecondaryAlternateMeshes.Add(mesh);
            SecondaryAlternateMeshes = SecondaryAlternateMeshes.OrderBy(x => x.Name).ToList();
        }

        public bool IsValidSkeleton(string trackName, out string boneName)
        {
            string track = trackName.Substring(3);

            if (trackName == ModelBase)
            {
                boneName = ModelBase;
                return true;
            }

            foreach (var bone in Skeleton)
            {
                string cleanBoneName = bone.Name.Replace("_DAG", string.Empty).ToLower();
                if (cleanBoneName == track)
                {
                    boneName = bone.Name.ToLower();
                    return true;
                }
            }

            boneName = string.Empty;
            return false;
        }

        public mat4 GetBoneMatrix(int boneIndex, string animName, int frame)
        {
            if (!Animations.ContainsKey(animName))
            {
                return mat4.Identity;
            }

            if (frame < 0 || frame >= Animations[animName].FrameCount)
            {
                return mat4.Identity;
            }

            var currentBone = Skeleton[boneIndex];

            mat4 boneMatrix = mat4.Identity;

            while (currentBone != null)
            {
                if (!Animations[animName].TracksCleanedStripped.ContainsKey(currentBone.CleanedName))
                {
                    break;
                }

                var track = Animations[animName].TracksCleanedStripped[currentBone.CleanedName].TrackDefFragment;
                int realFrame = frame >= track.Frames.Count ? 0 : frame;
                currentBone = Skeleton[boneIndex].Parent;

                float scaleValue = track.Frames[realFrame].Scale;
                var scaleMat = mat4.Scale(scaleValue, scaleValue, scaleValue);

                var rotationMatrix = new mat4(track.Frames[realFrame].Rotation);

                var translation = track.Frames[realFrame].Translation;
                var translateMat = mat4.Translate(translation);

                var modelMatrix = translateMat * rotationMatrix * scaleMat;

                boneMatrix = modelMatrix * boneMatrix;

                if (currentBone != null)
                {
                    boneIndex = currentBone.Index;
                }
            }

            return boneMatrix;
        }

        public void RenameNodeBase(string newBase)
        {
            foreach (var node in Skeleton)
            {
                node.Name = node.Name.Replace(ModelBase.ToUpper(), newBase.ToUpper());
            }

            var newNameMapping = new Dictionary<int, string>();
            foreach (var node in BoneMapping)
            {
                newNameMapping[node.Key] = node.Value.Replace(ModelBase.ToUpper(), newBase.ToUpper());
            }

            BoneMapping = newNameMapping;

            ModelBase = newBase;
        }
    }
}
