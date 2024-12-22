using LanternExtractor.EQ.Wld.Fragments;

namespace LanternExtractor.EQ.Wld.DataTypes
{
    public class BspNode
    {
        public float NormalX { get; set; }
        public float NormalY { get; set; }
        public float NormalZ { get; set; }
        public float SplitDistance { get; set; }
        public int RegionId { get; set; }
        public int LeftNode { get; set; }
        public int RightNode { get; set; }
        public BspRegion Region { get; set; }
    }
}