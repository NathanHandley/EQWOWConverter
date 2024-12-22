using GlmSharp;

namespace LanternExtractor.EQ.Wld.DataTypes
{
    public class ZonelineInfo
    {
        public ZonelineType Type { get; set; }
        public int Index { get; set; }
        public vec3 Position { get; set; }
        public int Heading { get; set; }
        public int ZoneIndex { get; set; }
    }
}