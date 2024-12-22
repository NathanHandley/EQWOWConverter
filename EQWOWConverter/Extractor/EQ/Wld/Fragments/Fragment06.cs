using System.Collections.Generic;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld.Fragments
{
    /// <summary>
    /// Fragment06 (0x06)
    /// Internal Name: None
    /// Only found in gequip files. Seems to represent 2d sprites in the world (coins).
    /// </summary>
    public class Fragment06 : WldFragment
    {
        public override void Initialize(int index, int size, byte[] data, List<WldFragment> fragments, Dictionary<int, string> stringHash,
            bool isNewWldFormat, ILogger logger)
        {
            base.Initialize(index, size, data, fragments, stringHash, isNewWldFormat, logger);
            Name = stringHash[-Reader.ReadInt32()];
            int value = Reader.ReadInt32();
        }
    }
}