using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWObjects
{
    internal enum WMOMaterialFlags : UInt32
    {
        DisableLighting = 0x1,
        DisableFogShadow = 0x2,
        Unculled = 0x4,
        ExteriorLit = 0x8,
        LightAtNight = 0x10,
        IsWindow = 0x20,
        ClampTextureS = 0x40,
        ClampTextureT = 0x80,
        Unused = 0x100
    }
}
