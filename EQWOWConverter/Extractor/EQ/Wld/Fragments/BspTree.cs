using System.Collections.Generic;
using LanternExtractor.EQ.Wld.DataTypes;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld.Fragments
{
    /// <summary>
    /// BSP Tree (0x21)
    /// Internal Name: None
    /// Binary tree with each leaf node containing a BspRegion fragment.
    /// </summary>
    class BspTree : WldFragment
    {
        /// <summary>
        /// The BSP nodes contained within the tree
        /// </summary>
        public List<BspNode> Nodes { get; private set; }

        public override void Initialize(int index, int size, byte[] data,
            List<WldFragment> fragments,
            Dictionary<int, string> stringHash, bool isNewWldFormat, ILogger logger)
        {
            base.Initialize(index, size, data, fragments, stringHash, isNewWldFormat, logger);
            Name = stringHash[-Reader.ReadInt32()];
            int nodeCount = Reader.ReadInt32();
            Nodes = new List<BspNode>();

            for (int i = 0; i < nodeCount; ++i)
            {
                Nodes.Add(new BspNode
                {
                    NormalX = Reader.ReadSingle(),
                    NormalY = Reader.ReadSingle(),
                    NormalZ = Reader.ReadSingle(),
                    SplitDistance = Reader.ReadSingle(),
                    RegionId = Reader.ReadInt32(),
                    LeftNode = Reader.ReadInt32() - 1,
                    RightNode = Reader.ReadInt32() - 1
                });
            }
        }

        /// <summary>
        /// Links BSP nodes to their corresponding BSP Regions
        /// The RegionId is not a fragment index but instead an index in a list of BSP Regions
        /// </summary>
        /// <param name="fragments">BSP region fragments</param>
        public void LinkBspRegions(List<BspRegion> fragments)
        {
            foreach (var node in Nodes)
            {
                if (node.RegionId == 0)
                {
                    continue;
                }

                node.Region = fragments[node.RegionId - 1];
            }
        }

        public override void OutputInfo(ILogger logger)
        {
            base.OutputInfo(logger);
            logger.LogInfo("-----");
            logger.LogInfo("BSPTree: Node count: " + Nodes.Count);
        }
    }
}