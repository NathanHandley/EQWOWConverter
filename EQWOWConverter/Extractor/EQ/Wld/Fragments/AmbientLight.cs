using System.Collections.Generic;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld.Fragments
{
    /// <summary>
    /// AmbientLight (0x2A)
    /// Internal name: _AMBIENTLIGHT
    /// Defines the ambient light for a group of regions. This fragment is found in the Trilogy client but is UNUSED.
    /// </summary>
    class AmbientLight : WldFragment
    {
        /// <summary>
        /// A reference to a 0x13 light source reference fragment which defines the light for the regions
        /// </summary>
        public LightSourceReference LightReference { get; private set; }

        /// <summary>
        /// The regions that the light reference apply to
        /// </summary>
        public List<int> Regions { get; private set; }

        public override void Initialize(int index, int size, byte[] data,
            List<WldFragment> fragments,
            Dictionary<int, string> stringHash, bool isNewWldFormat, ILogger logger)
        {
            base.Initialize(index, size, data, fragments, stringHash, isNewWldFormat, logger);
            Name = stringHash[-Reader.ReadInt32()];
            int reference = Reader.ReadInt32() - 1;
            LightReference = fragments[reference] as LightSourceReference;
            int flags = Reader.ReadInt32();
            int regionCount = Reader.ReadInt32();

            Regions = new List<int>();
            for (int i = 0; i < regionCount; ++i)
            {
                int regionId = Reader.ReadInt32();
                Regions.Add(regionId);
            }
        }

        public override void OutputInfo(ILogger logger)
        {
            base.OutputInfo(logger);
            logger.LogInfo("-----");
            logger.LogInfo("AmbientLight: Reference: " + (LightReference.Index + 1));
            logger.LogInfo("AmbientLight: Regions: " + Regions.Count);
        }
    }
}