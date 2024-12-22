using System.Collections.Generic;
using GlmSharp;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld.Fragments
{
    /// <summary>
    /// ObjectInstance (0x15)
    /// Internal name: None
    /// Information about a single instance of an object.
    /// </summary>
    public class ObjectInstance : WldFragment
    {
        /// <summary>
        /// The name of the object model
        /// </summary>
        public string ObjectName { get; private set; }

        /// <summary>
        /// The instance position in the world
        /// </summary>
        public vec3 Position { get; private set; }

        /// <summary>
        /// The instance rotation in the world
        /// </summary>
        public vec3 Rotation { get; private set; }

        /// <summary>
        /// The instance scale in the world
        /// </summary>
        public vec3 Scale { get; private set; }

        /// <summary>
        /// The vertex colors lighting data for this instance
        /// </summary>
        public VertexColors Colors;

        public override void Initialize(int index, int size, byte[] data,
            List<WldFragment> fragments,
            Dictionary<int, string> stringHash, bool isNewWldFormat, ILogger logger)
        {
            base.Initialize(index, size, data, fragments, stringHash, isNewWldFormat, logger);
            Name = stringHash[-Reader.ReadInt32()];

            // in main zone, points to 0x16, in object wld, it contains the object name
            int reference = Reader.ReadInt32();

            ObjectName = reference < 0 ? stringHash[-reference].Replace("_ACTORDEF", "").ToLower() : string.Empty;

            // Main zone: 0x2E, Objects: 0x32E
            int flags = Reader.ReadInt32();

            // Fragment reference
            // In main zone, it points to a 0x16 fragment
            // In objects.wld, it is 0
            int unknown2 = Reader.ReadInt32();

            Position = new vec3(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle());

            // Rotation is strange. There is never any x rotation (roll)
            // The z rotation is negated
            float value0 = Reader.ReadSingle();
            float value1 = Reader.ReadSingle();
            float value2 = Reader.ReadSingle();

            float modifier = 1.0f / 512.0f * 360.0f;

            Rotation = new vec3(0f, value1 * modifier, -(value0 * modifier));

            // Only scale y is used
            float scaleX, scaleY, scaleZ;
            scaleX = Reader.ReadSingle();
            scaleY = Reader.ReadSingle();
            scaleZ = Reader.ReadSingle();

            Scale = new vec3(scaleY, scaleY, scaleY);

            int colorFragment = Reader.ReadInt32();

            if (colorFragment != 0)
            {
                Colors = (fragments[colorFragment - 1] as VertexColorsReference)?.VertexColors;
            }
        }

        public override void OutputInfo(ILogger logger)
        {
            base.OutputInfo(logger);
            logger.LogInfo("----------");
            logger.LogInfo($"{GetType()}: Name: " + ObjectName);
            logger.LogInfo($"{GetType()}: Position: " + Position);
            logger.LogInfo($"{GetType()}: Rotation: " + Rotation);
            logger.LogInfo($"{GetType()}: Scale: " + Scale);
        }
    }
}
