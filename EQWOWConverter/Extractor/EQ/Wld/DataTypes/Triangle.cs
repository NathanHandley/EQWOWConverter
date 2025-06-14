namespace LanternExtractor.EQ.Wld.DataTypes
{
    public class Triangle
    {
        public bool IsSolid { get; set; }
        public int Index1 { get; set; }
        public int Index2 { get; set; }
        public int Index3 { get; set; }
        public int MaterialIndex { get; set; } // Only used for legacy triangle - should create separate class
    }
}
