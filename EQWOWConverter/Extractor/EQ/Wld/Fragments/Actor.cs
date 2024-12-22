using System.Collections.Generic;
using LanternExtractor.EQ.Wld.DataTypes;
using LanternExtractor.Infrastructure;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld.Fragments
{
    /// <summary>
    /// Actor (0x14)
    /// Internal name: _ACTORDEF
    /// Information about an actor that can be spawned into the world.
    /// An actor will have one of five types and will reference another fragment.
    /// </summary>
    class Actor : WldFragment
    {
        /// <summary>
        /// Mesh reference (optional)
        /// </summary>
        public MeshReference MeshReference { get; private set; }

        /// <summary>
        /// Skeleton track reference (optional)
        /// </summary>
        public SkeletonHierarchyReference SkeletonReference { get; private set; }

        /// <summary>
        /// Camera reference (optional)
        /// </summary>
        public CameraReference CameraReference { get; private set; }

        /// <summary>
        /// Camera reference (optional)
        /// </summary>
        public ParticleSpriteReference ParticleSpriteReference { get; private set; }

        public Fragment07 Fragment07;

        public ActorType ActorType;

        public string ReferenceName;

        public override void Initialize(int index, int size, byte[] data,
            List<WldFragment> fragments,
            Dictionary<int, string> stringHash, bool isNewWldFormat, ILogger logger)
        {
            base.Initialize(index, size, data, fragments, stringHash, isNewWldFormat, logger);
            Name = stringHash[-Reader.ReadInt32()];
            int flags = Reader.ReadInt32();

            BitAnalyzer ba = new BitAnalyzer(flags);

            bool params1Exist = ba.IsBitSet(0);
            bool params2Exist = ba.IsBitSet(1);
            bool fragment2MustContainZero = ba.IsBitSet(7);

            // Is an index in the string hash
            int fragment1 = Reader.ReadInt32();

            // For objects, SPRITECALLBACK - and it's the same reference value
            string stringValue = stringHash[-fragment1];

            // 1 for both static and animated objects
            int size1 = Reader.ReadInt32();

            // The number of components (meshes, skeletons, camera references) the actor has
            // In all Trilogy files, there is only ever 1
            int componentCount = Reader.ReadInt32();

            // 0 for both static and animated objects
            int fragment2 = Reader.ReadInt32();

            if (params1Exist)
            {
                int params1 = Reader.ReadInt32();
            }

            if (params2Exist)
            {
                Reader.BaseStream.Position += 7 * sizeof(int);
            }

            // Size 1 entries
            for (int i = 0; i < size1; ++i)
            {
                // Always 1
                int dataPairCount = Reader.ReadInt32();

                // Unknown purpose
                // Always 0 and 1.00000002E+30
                for (int j = 0; j < dataPairCount; ++j)
                {
                    int value = Reader.ReadInt32();
                    int value2 = Reader.ReadInt16();
                    int value3 = Reader.ReadInt16();
                }
            }

            if (componentCount > 1)
            {
                logger.LogWarning("Actor: More than one component references");
            }

            // Can contain either a skeleton reference (animated), mesh reference (static) or a camera reference
            for (int i = 0; i < componentCount; ++i)
            {
                int fragmentIndex = Reader.ReadInt32() - 1;
                var fragment = fragments[fragmentIndex];

                SkeletonReference = fragment as SkeletonHierarchyReference;

                if (SkeletonReference != null)
                {
                    SkeletonReference.SkeletonHierarchy.IsAssigned = true;
                    break;
                }

                MeshReference = fragment as MeshReference;

                if (MeshReference != null && MeshReference.Mesh != null)
                {
                    MeshReference.Mesh.IsHandled = true;
                    break;
                }

                if (MeshReference != null && MeshReference.LegacyMesh != null)
                {
                    break;
                }

                // This only exists in the main zone WLD
                CameraReference = fragment as CameraReference;

                if (CameraReference != null)
                {
                    break;
                }

                ParticleSpriteReference = fragment as ParticleSpriteReference;

                if (ParticleSpriteReference != null)
                {
                    break;
                }

                Fragment07 = fragment as Fragment07;

                if (Fragment07 != null)
                {
                    break;
                }

                logger.LogError($"Actor: Cannot link fragment with index {fragmentIndex}");
            }

            // Always 0 in qeynos2 objects
            int name3Bytes = Reader.ReadInt32();

            CalculateActorType(logger);
        }

        private void CalculateActorType(ILogger logger)
        {
            if (CameraReference != null)
            {
                ActorType = ActorType.Camera;
                ReferenceName = CameraReference.Name;
            }
            else if (SkeletonReference != null)
            {
                ActorType = ActorType.Skeletal;
            }
            else if (MeshReference != null)
            {
                ActorType = ActorType.Static;

                if (MeshReference != null)
                {
                    ReferenceName = MeshReference.Name;
                }
            }
            else if (ParticleSpriteReference != null)
            {
                ActorType = ActorType.Particle;
                ReferenceName = ParticleSpriteReference.Name;
            }
            else if (Fragment07 != null)
            {
                ActorType = ActorType.Sprite;
                ReferenceName = Fragment07.Name;
            }
            else
            {
                logger.LogError("Cannot determine actor type!");
            }
        }

        public override void OutputInfo(ILogger logger)
        {
            base.OutputInfo(logger);
            logger.LogInfo("-----");
        }

        public void AssignSkeletonReference(SkeletonHierarchy skeleton, ILogger logger)
        {
            SkeletonReference = new SkeletonHierarchyReference
            {
                SkeletonHierarchy = skeleton
            };

            CalculateActorType(logger);
            skeleton.IsAssigned = true;
        }
    }
}
