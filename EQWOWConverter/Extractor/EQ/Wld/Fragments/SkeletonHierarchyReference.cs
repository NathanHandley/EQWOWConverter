using System.Collections.Generic;
using System.IO;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld.Fragments
{
    /// <summary>
    /// SkeletonHierarchyReference (0x11)
    /// Internal name: None
    /// A reference to a skeleton track fragment (0x12)
    /// </summary>
    class SkeletonHierarchyReference : WldFragment
    {
        public SkeletonHierarchy SkeletonHierarchy { get; set; }

        public override void Initialize(int index, int size, byte[] data,
            List<WldFragment> fragments,
            Dictionary<int, string> stringHash, bool isNewWldFormat, ILogger logger)
        {
            base.Initialize(index, size, data, fragments, stringHash, isNewWldFormat, logger);

            var reader = new BinaryReader(new MemoryStream(data));
            
            // Reference is usually 0
            // Confirmed
            Name = stringHash[-reader.ReadInt32()];

            int reference = reader.ReadInt32();

            SkeletonHierarchy = fragments[reference - 1] as SkeletonHierarchy;

            if (SkeletonHierarchy == null)
            {
                logger.LogError("Bad skeleton hierarchy reference");
            }

            int params1 = reader.ReadInt32();

            // Params are 0
            // Confirmed
            if (params1 != 0)
            {
                
            }
            
            // Confirmed end
            if (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                
            }
        }

        public override void OutputInfo(ILogger logger)
        {
            base.OutputInfo(logger);

            if (SkeletonHierarchy != null)
            {
                logger.LogInfo("-----");
                logger.LogInfo("0x11: Skeleton track reference: " + SkeletonHierarchy.Index + 1);
            }
        }
    }
}