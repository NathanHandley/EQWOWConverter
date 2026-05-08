using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld.Fragments
{
    /// <summary>
    /// ParticleSprite (0x26) — also known as BlitSpriteDef.
    /// Internal name: _SPB
    /// Wraps a BitmapInfoReference (the texture / animated frames) plus a render method
    /// (blend mode) used when drawing the particle quad.
    /// </summary>
    public class ParticleSprite : WldFragment
    {
        // Texture (or animated texture series) used by particles emitted from this sprite.
        public BitmapInfoReference BitmapReference { get; private set; }

        // Flags field. Always 0 in Trilogy data.
        public int Flags { get; private set; }

        // Render method / blend mode. Same encoding as Material's render method
        // (TransparentAdditive, TransparentMasked, etc.). Determines how the
        // particle quad is composited.
        public int RenderMethod { get; private set; }

        public override void Initialize(int index, int size, byte[] data, List<WldFragment> fragments, Dictionary<int, string> stringHash,
            bool isNewWldFormat, ILogger logger)
        {
            base.Initialize(index, size, data, fragments, stringHash, isNewWldFormat, logger);
            Name = stringHash[-Reader.ReadInt32()];
            Flags = Reader.ReadInt32();
            int fragmentRef = Reader.ReadInt32();
            RenderMethod = Reader.ReadInt32();
            BitmapReference = fragments[fragmentRef - 1] as BitmapInfoReference;
        }
    }
}