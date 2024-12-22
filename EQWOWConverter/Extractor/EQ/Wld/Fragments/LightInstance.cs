using System.Collections.Generic;
using GlmSharp;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld.Fragments
{
    /// <summary>
    /// LightInstance (0x28)
    /// Internal name: None
    /// Defines the position and radius of a light.
    /// </summary>
    class LightInstance : WldFragment
    {
        /// <summary>
        /// The light reference (0x1C) this fragment refers to
        /// </summary>
        public LightSourceReference LightReference { get; private set; }

        /// <summary>
        /// The position of the light
        /// </summary>
        public vec3 Position { get; private set; }

        /// <summary>
        /// The radius of the light
        /// </summary>
        public float Radius { get; private set; }

        public override void Initialize(int index, int size, byte[] data,
            List<WldFragment> fragments,
            Dictionary<int, string> stringHash, bool isNewWldFormat, ILogger logger)
        {
            base.Initialize(index, size, data, fragments, stringHash, isNewWldFormat, logger);
            Name = stringHash[-Reader.ReadInt32()];
            LightReference = fragments[Reader.ReadInt32() - 1] as LightSourceReference;
            int flags = Reader.ReadInt32();
            Position = new vec3(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle());
            Radius = Reader.ReadSingle();
        }

        public override void OutputInfo(ILogger logger)
        {
            base.OutputInfo(logger);
            logger.LogInfo("-----");
            logger.LogInfo("LightInstance: Reference: " + (LightReference.Index + 1));
            logger.LogInfo("LightInstance: Position: " + Position);
            logger.LogInfo("LightInstance: Radius: " + Radius);
        }
    }
}