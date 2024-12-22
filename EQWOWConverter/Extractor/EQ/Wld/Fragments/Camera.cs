using System.Collections.Generic;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld.Fragments
{
    /// <summary>
    /// Camera (0x08)
    /// Internal Name: CAMERA_DUMMY
    /// Unknown fragment purpose. Contains 26 parameters. It's here in case someone wants to take a look.
    /// </summary>
    class Camera : WldFragment
    {
        public override void Initialize(int index, int size, byte[] data,
            List<WldFragment> fragments,
            Dictionary<int, string> stringHash, bool isNewWldFormat, ILogger logger)
        {
            base.Initialize(index, size, data, fragments, stringHash, isNewWldFormat, logger);
            Name = stringHash[-Reader.ReadInt32()];

            // 26 fields - unknown what they reference
            int params0 = Reader.ReadInt32();
            int params1 = Reader.ReadInt32();
            int params2 = Reader.ReadInt32();
            int params3 = Reader.ReadInt32();
            int params4 = Reader.ReadInt32();
            float params5 = Reader.ReadSingle();
            float params6 = Reader.ReadSingle();
            int params7 = Reader.ReadInt32();
            float params8 = Reader.ReadSingle();
            float params9 = Reader.ReadSingle();
            int params10 = Reader.ReadInt32();
            float params11 = Reader.ReadSingle();
            float params12 = Reader.ReadSingle();
            int params13 = Reader.ReadInt32();
            float params14 = Reader.ReadSingle();
            float params15 = Reader.ReadSingle();
            int params16 = Reader.ReadInt32();
            int params17 = Reader.ReadInt32();
            int params18 = Reader.ReadInt32();
            int params19 = Reader.ReadInt32();
            int params20 = Reader.ReadInt32();
            int params21 = Reader.ReadInt32();
            int params22 = Reader.ReadInt32();
            int params23 = Reader.ReadInt32();
            int params24 = Reader.ReadInt32();
            int params25 = Reader.ReadInt32();
        }
    }
}