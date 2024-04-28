using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWObjects
{
    internal enum WDTHeaderFlags : uint
    {
        HasGlobalMapObject      = 0x01,
        UseADTMCCV              = 0x02,
        UseTerrainShaders       = 0x04,
        DoodadRefsSortedBySize  = 0x08
    }
}
