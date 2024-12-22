using System.Collections.Generic;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld.Fragments
{
    /// <summary>
    /// CameraReference (0x09)
    /// Internal Name: None
    /// References a Camera fragment.
    /// </summary>
    class CameraReference : WldFragment
    {
        public Camera Camera { get; private set; }

        public override void Initialize(int index, int size, byte[] data,
            List<WldFragment> fragments,
            Dictionary<int, string> stringHash, bool isNewWldFormat, ILogger logger)
        {
            base.Initialize(index, size, data, fragments, stringHash, isNewWldFormat, logger);
            Name = stringHash[-Reader.ReadInt32()];
            Camera = fragments[Reader.ReadInt32() - 1] as Camera;

            // Usually 0
            int flags = Reader.ReadInt32();
        }

        public override void OutputInfo(ILogger logger)
        {
            base.OutputInfo(logger);
            logger.LogInfo("-----");
            logger.LogInfo("CameraReference: Reference: " + (Camera.Index + 1));
        }
    }
}