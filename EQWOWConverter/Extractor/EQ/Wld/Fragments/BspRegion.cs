using System.Collections.Generic;
using GlmSharp;
using LanternExtractor.EQ.Wld.DataTypes;
using LanternExtractor.Infrastructure;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld.Fragments
{
    /// <summary>
    /// BspRegion (0x22)
    /// Internal Name: None
    /// Leaf nodes in the BSP tree. Can contain references to Mesh fragments.
    /// This fragment's PVS (potentially visible set) data is unhandled.
    /// </summary>
    public class BspRegion : WldFragment
    {
        /// <summary>
        /// Does this fragment contain geometry?
        /// </summary>
        public bool ContainsPolygons { get; private set; }

        /// <summary>
        /// A reference to the mesh fragment
        /// </summary>
        public Mesh Mesh { get; private set; }

        public LegacyMesh LegacyMesh { get; private set; }

        public BspRegionType RegionType { get; private set; }

        public List<vec3> RegionVertices = new List<vec3>();

        public override void Initialize(int index, int size, byte[] data,
            List<WldFragment> fragments,
            Dictionary<int, string> stringHash, bool isNewWldFormat, ILogger logger)
        {
            base.Initialize(index, size, data, fragments, stringHash, isNewWldFormat, logger);
            Name = stringHash[-Reader.ReadInt32()];

            // Flags
            int flags = Reader.ReadInt32();

            BitAnalyzer ba = new BitAnalyzer(flags);
            var hasSphere = ba.IsBitSet(0);
            var hasReverbVolume = ba.IsBitSet(1);
            var hasReverbOffset = ba.IsBitSet(2);
            var regionFog = ba.IsBitSet(3);
            var enableGoraud2 = ba.IsBitSet(4);
            var encodedVisibility = ba.IsBitSet(5);
            var hasLegacyMeshReference = ba.IsBitSet(6);
            var hasByteEntries = ba.IsBitSet(7);
            var hasMeshReference = ba.IsBitSet(8);

            ContainsPolygons = hasMeshReference || hasLegacyMeshReference;

            // Always 0
            int ambientLight = Reader.ReadInt32();
            int numRegionVertex = Reader.ReadInt32();
            int numProximalRegions = Reader.ReadInt32();

            // Always 0
            int numRenderVertices = Reader.ReadInt32();
            int numWalls = Reader.ReadInt32();
            int numObstacles = Reader.ReadInt32();

            // Always 0
            int numCuttingObstacles = Reader.ReadInt32();
            int numVisNode = Reader.ReadInt32();
            int numVisList = Reader.ReadInt32();

            for (int i = 0; i < numRegionVertex; i++)
            {
                RegionVertices.Add(new vec3(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle()));
            }

            var proximalRegions = new List<(int, float)>();
            for (int i = 0; i < numProximalRegions; i++)
            {
                proximalRegions.Add((Reader.ReadInt32(), Reader.ReadSingle()));
            }

            var renderVertices = new List<vec3>();
            for (int i = 0; i < numRenderVertices; i++)
            {
                renderVertices.Add(new vec3(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle()));
            }

            var walls = new List<RegionWall>();
            for (int i = 0; i < numWalls; i++)
            {
                var wall = new RegionWall();

                wall.Flags = Reader.ReadInt32();
                var wallBa = new BitAnalyzer(wall.Flags);
                var isFloor = wallBa.IsBitSet(0);
                var isRenderable = wallBa.IsBitSet(1);

                wall.NumVertices = Reader.ReadInt32();
                wall.VertexList = new List<int>();
                for (int v = 0; v <  wall.NumVertices; v++)
                {
                    wall.VertexList.Add(Reader.ReadInt32());
                }

                if (isRenderable)
                {
                    wall.RenderMethod = new RenderMethod
                    {
                        Flags = Reader.ReadInt32()
                    };

                    wall.RenderInfo = RenderInfo.Parse(Reader, fragments);
                    wall.NormalAbcd = new vec4(
                        Reader.ReadSingle(),
                        Reader.ReadSingle(),
                        Reader.ReadSingle(),
                        Reader.ReadSingle()
                    );
                }

                walls.Add(wall);
            }

            var obstacles = new List<RegionObstacle>();
            for (int i = 0; i < numObstacles; i++)
            {
                var obstacle = new RegionObstacle();
                obstacle.Flags = Reader.ReadInt32();

                var obstacleBa = new BitAnalyzer(obstacle.Flags);
                var isFloor = obstacleBa.IsBitSet(0);
                var isGeometryCutting = obstacleBa.IsBitSet(1);
                var hasUserData = obstacleBa.IsBitSet(2);

                obstacle.NextRegion = Reader.ReadInt32();
                obstacle.ObstacleType = (RegionObstacleType) Reader.ReadInt32();

                if (obstacle.ObstacleType == RegionObstacleType.EdgePolygon ||
                    obstacle.ObstacleType == RegionObstacleType.EdgePolygonNormalAbcd)
                {
                    obstacle.NumVertices = Reader.ReadInt32();
                }

                obstacle.VertextList = new List<int>();
                for (int v = 0; v < obstacle.NumVertices; v++)
                {
                    obstacle.VertextList.Add(Reader.ReadInt32());
                }

                if (obstacle.ObstacleType == RegionObstacleType.EdgePolygonNormalAbcd)
                {
                    obstacle.NormalAbcd = new vec4(
                        Reader.ReadSingle(),
                        Reader.ReadSingle(),
                        Reader.ReadSingle(),
                        Reader.ReadSingle()
                    );
                }

                if (obstacle.ObstacleType == RegionObstacleType.EdgeWall)
                {
                    obstacle.EdgeWall = Reader.ReadInt32();
                }

                if (hasUserData)
                {
                    obstacle.UserDataSize = Reader.ReadInt32();
                    obstacle.UserData = Reader.ReadBytes(obstacle.UserDataSize);
                }

                obstacles.Add(obstacle);
            }

            var visNodes = new List<RegionVisNode>();
            for (int i = 0; i < numVisNode; i++)
            {
                var visNode = new RegionVisNode
                {
                    NormalAbcd = new vec4(
                        Reader.ReadSingle(),
                        Reader.ReadSingle(),
                        Reader.ReadSingle(),
                        Reader.ReadSingle()
                    ),
                    VisListIndex = Reader.ReadInt32(),
                    FrontTree = Reader.ReadInt32(),
                    BackTree = Reader.ReadInt32()
                };
                visNodes.Add(visNode);
            }

            var visLists = new List<RegionVisList>();
            for (int i = 0; i < numVisList; i++)
            {
                var visList = new RegionVisList
                {
                    RangeCount = Reader.ReadInt16()
                };

                visList.Ranges = new List<int>();
                for (int r = 0; r < visList.RangeCount; r++)
                {
                    int range = hasByteEntries ? Reader.ReadByte() : Reader.ReadInt16();
                    visList.Ranges.Add(range);
                }

                visLists.Add(visList);
            }

            vec4 sphere;
            if (hasSphere)
            {
                sphere = new vec4(
                    Reader.ReadSingle(),
                    Reader.ReadSingle(),
                    Reader.ReadSingle(),
                    Reader.ReadSingle()
                );
            }

            float reverbVolume;
            if (hasReverbVolume)
            {
                reverbVolume = Reader.ReadSingle();
            }

            int reverbOffset;
            if (hasReverbOffset)
            {
                reverbOffset = Reader.ReadInt32();
            }

            var userDataSize = Reader.ReadInt32();
            var userData = Reader.ReadBytes(userDataSize);

            // Get the mesh reference index and link to it
            if (ContainsPolygons)
            {
                int meshReference = Reader.ReadInt32() - 1;

                if (hasMeshReference)
                {
                    Mesh = fragments[meshReference] as Mesh;
                }
                else if (hasLegacyMeshReference)
                {
                    LegacyMesh = fragments[meshReference] as LegacyMesh;
                }
            }
        }

        public void SetRegionFlag(BspRegionType bspRegionType)
        {
            RegionType = bspRegionType;
        }

        public override void OutputInfo(ILogger logger)
        {
            base.OutputInfo(logger);
            logger.LogInfo("-----");
            logger.LogInfo("BspRegion: Contains polygons: " + ContainsPolygons);

            if (ContainsPolygons)
            {
                int meshIndex = Mesh?.Index ?? LegacyMesh?.Index ?? 0;
                logger.LogInfo("BspRegion: Mesh index: " + meshIndex);
            }
        }
    }
}
