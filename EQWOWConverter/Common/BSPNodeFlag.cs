using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Common
{
    internal enum BSPNodeFlag : UInt16
    {
        YZPlane     = 0x0,
        XZPlane     = 0x1,
        XYPlane     = 0x2,
        AxisMask    = 0x3,
        Leaf        = 0x4,
        NoChild     = 0xFFFF
    }
}
