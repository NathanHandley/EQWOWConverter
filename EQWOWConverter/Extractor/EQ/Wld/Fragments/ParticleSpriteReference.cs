using System.Collections.Generic;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld.Fragments
{
    /// <summary>
    /// ParticleSpriteReference (0x27)
    /// Internal name: None
    /// References a ParticleSprite (0x26).
    /// </summary>
    public class ParticleSpriteReference : WldFragment
    {
        public ParticleSprite ParticleSprite { get; private set; }
        public int Flags { get; private set; }

        public override void Initialize(int index, int size, byte[] data, List<WldFragment> fragments, Dictionary<int, string> stringHash,
            bool isNewWldFormat, ILogger logger)
        {
            base.Initialize(index, size, data, fragments, stringHash, isNewWldFormat, logger);
            Name = stringHash[-Reader.ReadInt32()];
            int fragmentRef = Reader.ReadInt32();
            Flags = Reader.ReadInt32();
            ParticleSprite = fragments[fragmentRef - 1] as ParticleSprite;
        }
    }
}