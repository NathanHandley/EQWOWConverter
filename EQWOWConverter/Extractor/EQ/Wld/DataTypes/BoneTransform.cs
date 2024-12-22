using GlmSharp;

namespace LanternExtractor.EQ.Wld.DataTypes
{
    public class BoneTransform
    {
        public vec3 Translation { get; set; }
        public quat Rotation { get; set; }
        public float Scale { get; set; }
        public mat4 ModelMatrix;
    }
}