using System.Collections.Generic;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld.Fragments
{
    /// <summary>
    /// MeshReference (0x2D)
    /// Internal name: None
    /// Contains a reference to either a Mesh and LegacyMesh fragment.
    /// This fragment is referenced from a Skeleton fragment.
    /// </summary>
    public class MeshReference : WldFragment
    {
        public Mesh Mesh { get; private set; }
        
        public LegacyMesh LegacyMesh { get; private set; }
        
        public override void Initialize(int index, int size, byte[] data,
            List<WldFragment> fragments,
            Dictionary<int, string> stringHash, bool isNewWldFormat, ILogger logger)
        {
            base.Initialize(index, size, data, fragments, stringHash, isNewWldFormat, logger);
            Name = stringHash[-Reader.ReadInt32()];
            int reference = Reader.ReadInt32() - 1;
            Mesh = fragments[reference] as Mesh;

            if (Mesh != null)
            {
                return;
            }
            
            LegacyMesh = fragments[reference] as LegacyMesh;

            if (LegacyMesh != null)
            {
                return;
            }
            
            logger.LogError("No mesh reference found for id: " + reference);
        }

        public override void OutputInfo(ILogger logger)
        {
            base.OutputInfo(logger);

            if (Mesh != null)
            {
                logger.LogInfo("-----");
                logger.LogInfo("0x2D: Mesh reference: " + Mesh.Index);
            }
        }
    }
}