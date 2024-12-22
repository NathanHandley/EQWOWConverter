using System.Collections.Generic;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld.Fragments
{
    /// <summary>
    /// Fragment07 (0x07)
    /// Internal Name: None
    /// Only found in gequip files. References a 0x06 fragment.
    /// This fragment can be referenced by an actor fragment.
    /// </summary>
    public class Fragment07 : WldFragment
    {
        private Fragment06 _fragment06;
        
        public override void Initialize(int index, int size, byte[] data, List<WldFragment> fragments, Dictionary<int, string> stringHash,
            bool isNewWldFormat, ILogger logger)
        {
            base.Initialize(index, size, data, fragments, stringHash, isNewWldFormat, logger);
            Name = stringHash[-Reader.ReadInt32()];
            _fragment06 = fragments[Reader.ReadInt32() - 1] as Fragment06;
            int value08 = Reader.ReadInt32(); // always 0
        }
    }
}