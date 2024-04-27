using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWObjects
{
    internal enum WMOPolyMaterialFlags : byte
    {
        Unknown1            = 0x01,
        NoCameraCollide     = 0x02,
        Detail              = 0x04,
        Collision           = 0x08, // Reads as collision, but wowdev says it turns of rendering of water ripple...?
        Hint                = 0x10,
        Render              = 0x20,
        CullObjects         = 0x40,
        CollideHit          = 0x80
    }
}
