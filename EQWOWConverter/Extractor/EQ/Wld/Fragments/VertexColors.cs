using System;
using System.Collections.Generic;
using LanternExtractor.EQ.Wld.DataTypes;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld.Fragments
{
    /// <summary>
    /// VertexColors (0x32)
    /// Internal name: _DMT
    /// A list of colors, one per vertex, representing baked lighting data for an object.
    /// Vertex color data for zone meshes are baked into the mesh as they are unique.
    /// </summary>
    public class VertexColors : WldFragment
    {
        /// <summary>
        /// The vertex colors corresponding with each vertex
        /// </summary>
        public List<Color> Colors { get; private set; }

        public override void Initialize(int index, int size, byte[] data,
            List<WldFragment> fragments,
            Dictionary<int, string> stringHash, bool isNewWldFormat, ILogger logger)
        {
            base.Initialize(index, size, data, fragments, stringHash, isNewWldFormat, logger);
            Name = stringHash[-Reader.ReadInt32()];
            int unknown = Reader.ReadInt32();
            int colorCount = Reader.ReadInt32();

            // Typically contains 1
            int unknown2 = Reader.ReadInt32();

            // Typically contains 200
            int unknown3 = Reader.ReadInt32();

            // Typically contains 0
            int unknown4 = Reader.ReadInt32();

            Colors = new List<Color>();

            for (int i = 0; i < colorCount; ++i)
            {
                byte[] colorBytes = BitConverter.GetBytes(Reader.ReadInt32());
                int b = colorBytes[0];
                int g = colorBytes[1];
                int r = colorBytes[2];
                int a = colorBytes[3];

                Colors.Add(new Color (r, g, b, a));
            }
        }

        public override void OutputInfo(ILogger logger)
        {
            base.OutputInfo(logger);
            logger.LogInfo("-----");
            logger.LogInfo("VertexColors: Vertex color count: " + Colors.Count);
        }
    }
}