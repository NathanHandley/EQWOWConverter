using System.Collections.Generic;
using GlmSharp;
using LanternExtractor.EQ.Wld.DataTypes;
using LanternExtractor.Infrastructure;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld.Fragments
{
    /// <summary>
    /// Polyhedron (0x17)
    /// Internal Name: _POLYHDEF
    /// </summary>
    public class Polyhedron : WldFragment
    {
        // POLYHEDRONDEFINITION
        // BOUNDINGRADIUS %f
        // SCALEFACTOR %f
        // NUMVERTICES %d
        // XYZ %f %f %f
        // NUMFACES %d
        // FACE
        //     NORMALABCD %f %f %f %f
        //     NUMVERTICES %d
        //     VERTEXLIST %d ...%d
        // ENDFACE
        // ENDPOLYHEDRONDEFINITION

        public float BoundingRadius { get; set; }
        public float ScaleFactor { get; set; }
        public List<vec3> Vertices { get; set; }
        public List<Polygon> Faces { get; set; }

        public override void Initialize(int index, int size, byte[] data,
            List<WldFragment> fragments,
            Dictionary<int, string> stringHash, bool isNewWldFormat, ILogger logger)
        {
            base.Initialize(index, size, data, fragments, stringHash, isNewWldFormat, logger);
            Name = stringHash[-Reader.ReadInt32()];
            int flags = Reader.ReadInt32();

            var ba = new BitAnalyzer(flags);
            var hasScaleFactor = ba.IsBitSet(0);
            var hasNormalAbcd = ba.IsBitSet(1);

            int vertexCount = Reader.ReadInt32();
            int faceCount = Reader.ReadInt32();
            float boundingRadius = Reader.ReadSingle();

            if (hasScaleFactor)
            {
                ScaleFactor = Reader.ReadSingle();
            }
            else
            {
                ScaleFactor = 1.0f;
            }

            Vertices = new List<vec3>();
            for (var i = 0; i < vertexCount; i++)
            {
                var vertex = new vec3(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle());
                Vertices.Add(vertex);
            }

            Faces = new List<Polygon>();
            for (var i = 0; i < faceCount; i++)
            {
                var faceVertexCount = Reader.ReadInt32();
                var faceVertices = new List<int>();
                for (var v = 0; v < faceVertexCount; v++)
                {
                    faceVertices.Add(Reader.ReadInt32());
                }

                if (hasNormalAbcd)
                {
                    var normalAbcd = new vec4(
                        Reader.ReadSingle(),
                        Reader.ReadSingle(),
                        Reader.ReadSingle(),
                        Reader.ReadSingle()
                    );
                }

                // 4 vertices will result in 2 triangles
                var polygonCount = faceVertexCount - 2;
                for (var f = 0; f < polygonCount; f++)
                {
                    var polygon = new Polygon
                    {
                        IsSolid = true,
                        Vertex1 = faceVertices[0],
                        Vertex2 = faceVertices[f + 1],
                        Vertex3 = faceVertices[f + 2],
                    };
                    Faces.Add(polygon);
                }
            }
        }
    }
}
