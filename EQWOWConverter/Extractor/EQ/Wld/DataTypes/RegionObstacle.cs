using System.Collections.Generic;
using GlmSharp;

namespace LanternExtractor.EQ.Wld.DataTypes
{
    public class RegionObstacle
    {
        /// bit 0 - is a FLOOR
        /// bit 1 - is a GEOMETRYCUTTINGOBSTACLE
        /// bit 2 - has USERDATA %s
        public int Flags { get; set; }

        /// NEXTREGION %d
        public int NextRegion { get; set; }

        /// XY_VERTEX 0 %d
        /// XYZ_VERTEX 0 %d
        /// XY_LINE 0 %d %d
        /// XY_EDGE 0 %d %d
        /// XYZ_EDGE 0 %d %d
        /// PLANE 0 %d
        /// EDGEPOLYGON 0
        /// EDGEWALL 0 %d
        public RegionObstacleType ObstacleType { get; set; }

        // NUMVERTICES %d
        public int NumVertices { get; set; }

        /// VERTEXLIST %d ...%d
        public List<int> VertextList { get; set; }

        /// NORMALABCD %f %f %f %f
        public vec4 NormalAbcd { get; set; }

        /// EDGEWALL 0 %d
        /// Binary values are 0 based. "EDGEWALL 0 1" becomes edge_wall[0]
        public int EdgeWall { get; set; }

        /// Length of USERDATA string
        public int UserDataSize { get; set; }

        /// USERDATA %s
        public byte[] UserData { get; set; }
    }
}
