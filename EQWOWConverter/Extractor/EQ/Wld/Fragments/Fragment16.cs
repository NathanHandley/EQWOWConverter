using System.Collections.Generic;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld.Fragments
{
    /// <summary>
    /// Fragment16 (0x16)
    /// Internal Name: None
    /// An unknown fragment. Found in zone files.
    /// </summary>
    class Fragment16 : WldFragment
    {
        public override void Initialize(int index, int size, byte[] data,
            List<WldFragment> fragments,
            Dictionary<int, string> stringHash, bool isNewWldFormat, ILogger logger)
        {
            base.Initialize(index, size, data, fragments, stringHash, isNewWldFormat, logger);
            Name = stringHash[-Reader.ReadInt32()];

            // Should be 0.1
            float unknown = Reader.ReadSingle();
        }
    }
}