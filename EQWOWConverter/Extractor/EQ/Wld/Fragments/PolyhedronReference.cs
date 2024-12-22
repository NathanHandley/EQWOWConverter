using System.Collections.Generic;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld.Fragments
{
    /// <summary>
    /// PolyhedronReference (0x18)
    /// Internal Name: None
    /// Need to figure this fragment out.
    /// </summary>
    public class PolyhedronReference : WldFragment
    {
        public Polyhedron Polyhedron;

        public override void Initialize(int index, int size, byte[] data,
            List<WldFragment> fragments,
            Dictionary<int, string> stringHash, bool isNewWldFormat, ILogger logger)
        {
            base.Initialize(index, size, data, fragments, stringHash, isNewWldFormat, logger);
            Name = stringHash[-Reader.ReadInt32()];
            Polyhedron = fragments[Reader.ReadInt32() - 1] as Polyhedron;
            float params1 = Reader.ReadSingle();
        }

        public override void OutputInfo(ILogger logger)
        {
            base.OutputInfo(logger);
            logger.LogInfo("-----");
        }
    }
}
