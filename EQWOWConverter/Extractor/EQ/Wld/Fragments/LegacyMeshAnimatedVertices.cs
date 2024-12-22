using System.Collections.Generic;
using GlmSharp;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld.Fragments
{
    /// <summary>
    /// LegactMeshAnimatedVertices (0x2E)
    /// Internal name: _DMTRACKDEF
    /// Contains a list of frames each containing a position for each vertex.
    /// The frame vertices are cycled through, animating the model.
    /// </summary>
    public class LegacyMeshAnimatedVertices : WldFragment, IAnimatedVertices
    {
        /// <summary>
        /// The model frames
        /// </summary>
        public List<List<vec3>> Frames { get; set; }

        /// <summary>
        /// The delay between the vertex swaps
        /// </summary>
        public int Delay { get; set; }

        public override void Initialize(int index, int size, byte[] data,
            List<WldFragment> fragments,
            Dictionary<int, string> stringHash, bool isNewWldFormat, ILogger logger)
        {
            base.Initialize(index, size, data, fragments, stringHash, isNewWldFormat, logger);

            Name = stringHash[-Reader.ReadInt32()];
            int flags = Reader.ReadInt32();
            int vertexCount = Reader.ReadInt32();
            int frameCount = Reader.ReadInt32();
            Delay = Reader.ReadInt32();
            int param1 = Reader.ReadInt32();

            Frames = new List<List<vec3>>();
            for (var i = 0; i < frameCount; i++)
            {
                var positions = new List<vec3>();

                for (var v = 0; v < vertexCount; v++)
                {
                    positions.Add(
                        new vec3(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle())
                    );
                }

                Frames.Add(positions);
            }
        }

        public override void OutputInfo(ILogger logger)
        {
            base.OutputInfo(logger);
            logger.LogInfo("-----");
            logger.LogInfo("LegacyMeshAnimatedVertices: Frame count: " + Frames.Count);
        }
    }
}
