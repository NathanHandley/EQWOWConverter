using System.Collections.Generic;
using GlmSharp;

namespace LanternExtractor.EQ.Wld.Fragments
{
    public interface IAnimatedVertices
    {
        List<List<vec3>> Frames { get; set; }
        int Delay { get; set; }
    }
}
