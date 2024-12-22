using System.Collections.Generic;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld.Fragments
{
    /// <summary>
    /// GenericFragment (0xXX)
    /// Used for unknown fragments.
    /// Left the functions here for debugging purposes.
    /// </summary>
    class Generic : WldFragment
    {
        public override void Initialize(int index, int size, byte[] data,
            List<WldFragment> fragments,
            Dictionary<int, string> stringHash, bool isNewWldFormat, ILogger logger)
        {
            base.Initialize(index, size, data, fragments, stringHash, isNewWldFormat, logger);
        }

        public override void OutputInfo(ILogger logger)
        {
            base.OutputInfo(logger);
        }
    }
}