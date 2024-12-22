using System.Collections.Generic;
using LanternExtractor.Infrastructure;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld.Fragments
{
    /// <summary>
    /// BitmapInfo (0x04)
    /// Internal name: _SPRITE
    /// This fragment contains a reference to a 0x03 fragment and information about animation.
    /// </summary>
    public class BitmapInfo : WldFragment
    {
        /// <summary>
        /// Is the texture animated?
        /// </summary>
        public bool IsAnimated { get; private set; }

        /// <summary>
        /// The bitmap names referenced. 
        /// </summary>
        public List<BitmapName> BitmapNames { get; private set; }

        /// <summary>
        /// The number of milliseconds before the next texture is swapped.
        /// </summary>
        public int AnimationDelayMs { get; private set; }

        public override void Initialize(int index, int size, byte[] data,
            List<WldFragment> fragments,
            Dictionary<int, string> stringHash, bool isNewWldFormat, ILogger logger)
        {
            base.Initialize(index, size, data, fragments, stringHash, isNewWldFormat, logger);
            Name = stringHash[-Reader.ReadInt32()];
            int flags = Reader.ReadInt32();
            var bitAnalyzer = new BitAnalyzer(flags);
            IsAnimated = bitAnalyzer.IsBitSet(3);
            int bitmapCount = Reader.ReadInt32();
            
            BitmapNames = new List<BitmapName>();

            if (IsAnimated)
            {
                AnimationDelayMs = Reader.ReadInt32();
            }

            for (int i = 0; i < bitmapCount; ++i)
            {
                BitmapNames.Add(fragments[Reader.ReadInt32() - 1] as BitmapName);
            }
        }

        public override void OutputInfo(ILogger logger)
        {
            base.OutputInfo(logger);
            logger.LogInfo("-----");
            logger.LogInfo("BitmapInfo: Animated: " + IsAnimated);

            if (IsAnimated)
            {
                logger.LogInfo("BitmapInfo: Animation delay: " + AnimationDelayMs + "ms");
            }

            string references = string.Empty;

            for (var i = 0; i < BitmapNames.Count; i++)
            {
                if (i != 0)
                {
                    references += ", ";
                }

                BitmapName bitmapName = BitmapNames[i];
                references += bitmapName.Index + 1;
            }

            logger.LogInfo("BitmapInfo: Reference(s): " + references);
        }
    }
}