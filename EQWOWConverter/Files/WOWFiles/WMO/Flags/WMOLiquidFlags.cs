using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Files.WOWFiles
{
    internal enum WMOLiquidFlags : byte
    {
        None = 0,
        LegacyLiquidType = 0x04,
        DisableRender = 0x08,   // Don't render it
        Unknown1 = 0x10,        // ?
        Unknown2 = 0x20,        // ?
        IsFishable = 0x40,      // Can fish
        Shared = 0x80           // Not sure what this is
    }
}
