using System;
using System.Collections.Generic;
using GlmSharp;
using LanternExtractor.EQ.Wld.DataTypes;
using LanternExtractor.Infrastructure;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld.Fragments
{
    /// <summary>
    /// LegacyMesh (0x2C)
    /// Internal name: _DMSPRITEDEF
    /// This fragment is only found in the gequip archives and while it exists and is functional, it is not used.
    /// It looks like an earlier version of the Mesh fragment with fewer data points.
    /// </summary>
    public class LegacyMesh : WldFragment
    {
        public vec3 Center { get; private set; }
        public List<vec3> Vertices = new List<vec3>();
        public List<vec2> TexCoords = new List<vec2>();
        public List<vec3> Normals = new List<vec3>();
        public List<Polygon> Polygons = new List<Polygon>();
        public List<ivec2> VertexTex = new List<ivec2>();
        public List<Color> Colors = new List<Color>();
        public List<RenderGroup> RenderGroups = new List<RenderGroup>();
        public MaterialList MaterialList;
        public PolyhedronReference PolyhedronReference;
        public Dictionary<int, MobVertexPiece> MobPieces { get; private set; }
        /// <summary>
        /// The animated vertex fragment (0x2E or 0x37) reference
        /// </summary>
        public MeshAnimatedVerticesReference AnimatedVerticesReference { get; private set; }

        /// <summary>
        /// Set to true if there are non solid polygons in the mesh
        /// This means we export collision separately (e.g. trees, fire)
        /// </summary>
        public bool ExportSeparateCollision { get; private set; }

        public override void Initialize(int index, int size, byte[] data, List<WldFragment> fragments, Dictionary<int, string> stringHash,
            bool isNewWldFormat, ILogger logger)
        {
            base.Initialize(index, size, data, fragments, stringHash, isNewWldFormat, logger);
            Name = stringHash[-Reader.ReadInt32()];

            // TODO: investigate flags further
            // looks like some flags will zero and 1.0 fields if they are missing.
            // 0x1 (bit0) center offset
            // 0x2 (bit1) bounding radius?
            // 0x200 (bit9)
            // 0x400 (bit10) colors?
            // 0x800 (bit11) RenderGroups
            // 0x1000 (bit12) VertexTex
            // 0x2000 (bit13)
            // 0x4000 (bit14) shown in ghidra as 0x40 bounding box?
            // 0x8000 (bit15) shown in ghidra as 0x80
            int flags = Reader.ReadInt32();
            BitAnalyzer ba = new BitAnalyzer(flags);

            int vertexCount = Reader.ReadInt32();
            int texCoordCount = Reader.ReadInt32();
            int normalsCount = Reader.ReadInt32();
            int colorsCount = Reader.ReadInt32(); // size4
            int polygonCount = Reader.ReadInt32();
            int size6 = Reader.ReadInt16();
            int fragment1Maybe = Reader.ReadInt16();
            int vertexPieceCount = Reader.ReadInt32(); // -1
            MaterialList = fragments[Reader.ReadInt32() - 1] as MaterialList;
            int meshAnimation = Reader.ReadInt32();

            // Vertex animation only
            if (meshAnimation != 0)
            {
                AnimatedVerticesReference = fragments[meshAnimation - 1] as MeshAnimatedVerticesReference;
            }

            float something1 = Reader.ReadSingle();

            // This might also be able to take a sphere (0x16) or sphere list (0x1a) collision volume
            var polyhedronReference = Reader.ReadInt32();
            if (polyhedronReference > 0)
            {
                PolyhedronReference = fragments[polyhedronReference - 1] as PolyhedronReference;
                var sphereFragment = fragments[polyhedronReference - 1] as Fragment16;
                if (sphereFragment != null)
                {
                    System.Console.WriteLine(sphereFragment.Name);
                }
                ExportSeparateCollision = true;
            }

            Center = new vec3(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle());
            if (!ba.IsBitSet(0))
            {
                Center = vec3.Zero;
            }

            float boundingRadiusMaybe = Reader.ReadSingle();
            if (!ba.IsBitSet(1))
            {
                boundingRadiusMaybe = 1.0f;
            }

            for (int i = 0; i < vertexCount; ++i)
            {
                Vertices.Add(new vec3(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle()));
            }

            for (int i = 0; i < texCoordCount; ++i)
            {
                TexCoords.Add(new vec2(Reader.ReadSingle(), Reader.ReadSingle()));
            }

            for (int i = 0; i < normalsCount; ++i)
            {
                Normals.Add(new vec3(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle()));
            }

            // I don't think this is colors
            // count seems to match the vertexCount when present
            // found in gequip.t3d
            for (int i = 0; i < colorsCount; ++i)
            {
                // byte 0 seems to always be 1
                // byte 1 seems to always be 0,1,2
                // byte 2 seems to be between 0 and polygonCount - 1
                // byte 3 seems to always be 0
                var unkBytes = Reader.ReadBytes(4);
            }

            // faces
            for (int i = 0; i < polygonCount; ++i)
            {
                int flag = Reader.ReadInt16();

                int unk1 = Reader.ReadInt16();
                int materialIndex = Reader.ReadInt16();
                int unk3 = Reader.ReadInt16();
                int unk4 = Reader.ReadInt16();

                int i1 = Reader.ReadInt16();
                int i2 = Reader.ReadInt16();
                int i3 = Reader.ReadInt16();
                Polygons.Add(new Polygon
                {
                    IsSolid = true,
                    Vertex1 = i1,
                    Vertex2 = i2,
                    Vertex3 = i3,
                    MaterialIndex = materialIndex
                });
            }

            // meshops
            for (int i = 0; i < size6; ++i)
            {
                int datatype = Reader.ReadInt32();

                if (datatype != 4)
                {
                    int vertexIndex = Reader.ReadInt32();
                    int data6Param1 = Reader.ReadInt16();
                    int data6Param2 = Reader.ReadInt16();
                }
                else
                {
                    float offset = Reader.ReadSingle();
                    int something = Reader.ReadInt32();
                }
            }

            MobPieces = new Dictionary<int, MobVertexPiece>();
            int mobStart = 0;
            for (int i = 0; i < vertexPieceCount; ++i)
            {
                var mobVertexPiece = new MobVertexPiece
                {
                    Count = Reader.ReadInt16(),
                    Start = Reader.ReadInt16()
                };

                mobStart += mobVertexPiece.Count;

                MobPieces[mobVertexPiece.Start] = mobVertexPiece;
            }

            if (ba.IsBitSet(9))
            {
                int size8 = Reader.ReadInt32();

                Reader.BaseStream.Position += size8 * 4;
            }

            // found in qrg R1. count matches vertex count
            // this might be vertex colors?
            if (ba.IsBitSet(10))
            {
                int unkCount = Reader.ReadInt32();
                for (int i = 0; i < unkCount; i++)
                {
                    var colorBytes = BitConverter.GetBytes(Reader.ReadInt32());
                    int b = colorBytes[0];
                    int g = colorBytes[1];
                    int r = colorBytes[2];
                    int a = colorBytes[3];

                    Colors.Add(new Color( r, g, b, a));
                }
            }

            if (ba.IsBitSet(11))
            {
                int polygonTexCount = Reader.ReadInt32();

                for (int i = 0; i < polygonTexCount; ++i)
                {
                    RenderGroups.Add(new RenderGroup
                    {
                        PolygonCount = Reader.ReadInt16(),
                        MaterialIndex = Reader.ReadInt16()
                    });
                }
            }

            if (ba.IsBitSet(12))
            {
                int vertexTexCount = Reader.ReadInt32();

                for (int i = 0; i < vertexTexCount; ++i)
                {
                    VertexTex.Add(new ivec2
                    {
                        x = Reader.ReadInt16(),
                        y = Reader.ReadInt16()
                    });
                }
            }

            // TODO: Research: Instead of controlling the presence, the fields might be zeroed if the bit isn't set
            // in highkeep, seems to only be set on HANGLANT, TIKI, TORCHPOINT (but not on brazier1)
            // I think this might be the position of the lightdef; for a light pole the third float is like 7.66 which seems like a reasonable height
            if (ba.IsBitSet(13))
            {
                var params31 = Reader.ReadSingle();
                var params32 = Reader.ReadSingle();
                var params33 = Reader.ReadSingle();

                var bp = 1;
            }

            // Bounding Box?
            if (ba.IsBitSet(14))
            {
                var vertex1 = new vec3(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle());
                var vertex2 = new vec3(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle());
            }

            if (ba.IsBitSet(15))
            {
                var b15 = 1;
            }

            // In some rare cases, the number of uvs does not match the number of vertices
            if (vertexCount != texCoordCount)
            {
                int difference = vertexCount - texCoordCount;

                for (int i = 0; i < difference; ++i)
                {
                    TexCoords.Add(new vec2(0.0f, 0.0f));
                }
            }
        }
    }
}
