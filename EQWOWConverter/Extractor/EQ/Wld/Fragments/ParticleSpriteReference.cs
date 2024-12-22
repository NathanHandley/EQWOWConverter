using System.Collections.Generic;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld.Fragments
{
    /// <summary>
    /// ParticleSpriteReference (0x27)
    /// Internal name: None
    /// Assumed to reference a particle sprite fragment.
    /// </summary>
    public class ParticleSpriteReference : WldFragment
    {
        private ParticleSprite _reference;
        public override void Initialize(int index, int size, byte[] data, List<WldFragment> fragments, Dictionary<int, string> stringHash,
            bool isNewWldFormat, ILogger logger)
        {
            base.Initialize(index, size, data, fragments, stringHash, isNewWldFormat, logger);
            Name = stringHash[-Reader.ReadInt32()];
            int fragmentRef = Reader.ReadInt32();
            int value08 = Reader.ReadInt32(); // always 0
            _reference = fragments[fragmentRef - 1] as ParticleSprite;
        }
    }
}