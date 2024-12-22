using GlmSharp;

namespace LanternExtractor.EQ.Wld.DataTypes
{
    public class RegionVisNode
    {
        /// NORMALABCD %f %f %f %f
        public vec4 NormalAbcd { get; set; }

        /// VISLISTINDEX %d
        public int VisListIndex { get; set; }

        /// FRONTTREE %d
        public int FrontTree { get; set; }

        /// BACKTREE %d
        public int BackTree { get; set; }
    }
}
