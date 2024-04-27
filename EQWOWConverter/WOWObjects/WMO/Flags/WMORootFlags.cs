using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWObjects
{
    internal enum WMORootFlags : UInt32
    {
        DoNotAttenuateVerticesBasedOnDistanceToPortal = 0x01,
        UseUnifiedRenderingPath = 0x02,
        UseLiquidTypeDBCID = 0x04,
        DoNotFixVertexColorAlpha = 0x08,
    }
}
