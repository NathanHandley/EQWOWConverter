using System.Collections.Generic;
using GlmSharp;

namespace LanternExtractor.EQ.Wld.DataTypes
{
    public class RegionWall
    {
        /// bit 0 - has FLOOR (is floor?)
        /// bit 1 - has RENDERMETHOD and NORMALABCD (is renderable?)
        public int Flags { get; set; }

        /// NUMVERTICES %d
        public int NumVertices { get; set; }

        /// RENDERMETHOD ...
        public RenderMethod RenderMethod { get; set; }

        /// RENDERINFO
        public RenderInfo RenderInfo { get; set; }

        /// NORMALABCD %f %f %f %f
        public vec4 NormalAbcd { get; set; }

        /// VERTEXLIST %d ...%d
        /// Binary values are 0 based. "VERTEXLIST 1" becomes vertex_list[0]
        public List<int> VertexList { get; set; }
    }
}
