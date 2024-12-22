using System.Collections.Generic;
using GlmSharp;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld.Fragments
{
    /// <summary>
    /// MeshAnimatedVertices (0x37)
    /// Internal name: _DMTRACKDEF
    /// Contains a list of frames each containing a position for each vertex.
    /// The frame vertices are cycled through, animating the model.
    /// </summary>
    public class MeshAnimatedVertices : WldFragment, IAnimatedVertices
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
            int vertexCount = Reader.ReadInt16();
            int frameCount = Reader.ReadInt16();
            Delay = Reader.ReadInt16();
            int param2 = Reader.ReadInt16();

            float scale = 1.0f / (1 << Reader.ReadInt16());

            Frames = new List<List<vec3>>();
            for (int i = 0; i < frameCount; ++i)
            {
                var positions = new List<vec3>();

                for (int j = 0; j < vertexCount; ++j)
                {
                    float x = Reader.ReadInt16() * scale;
                    float y = Reader.ReadInt16() * scale;
                    float z = Reader.ReadInt16() * scale;

                    positions.Add(new vec3(x, y, z));
                }

                Frames.Add(positions);
            }
        }

        public override void OutputInfo(ILogger logger)
        {
            base.OutputInfo(logger);
            logger.LogInfo("-----");
            logger.LogInfo("MeshAnimatedVertices: Frame count: " + Frames.Count);
        }
    }
}
