using System.Collections.Generic;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld.Fragments
{
    /// <summary>
    /// MeshAnimatedVerticesReference (0x2F)
    /// Internal name: None
    /// References a LegacyMeshAnimatedVertices or MeshAnimatedVertices fragment.
    /// This fragment is referenced from the Mesh fragment, if it's animated.
    /// </summary>
    public class MeshAnimatedVerticesReference : WldFragment
    {
        public LegacyMeshAnimatedVertices LegacyMeshAnimatedVertices { get; set; }
        public MeshAnimatedVertices MeshAnimatedVertices { get; set; }

        public override void Initialize(int index, int size, byte[] data,
            List<WldFragment> fragments,
            Dictionary<int, string> stringHash, bool isNewWldFormat, ILogger logger)
        {
            base.Initialize(index, size, data, fragments, stringHash, isNewWldFormat, logger);
            Name = stringHash[-Reader.ReadInt32()];

            var fragmentId = Reader.ReadInt32() - 1;
            MeshAnimatedVertices = fragments[fragmentId] as MeshAnimatedVertices;
            LegacyMeshAnimatedVertices = fragments[fragmentId] as LegacyMeshAnimatedVertices;
            int flags = Reader.ReadInt32();
        }

        public IAnimatedVertices GetAnimatedVertices()
        {
            return MeshAnimatedVertices as IAnimatedVertices ?? LegacyMeshAnimatedVertices;
        }

        public override void OutputInfo(ILogger logger)
        {
            base.OutputInfo(logger);
            logger.LogInfo("-----");
        }
    }
}
