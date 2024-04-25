using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWObjects
{
    internal enum WMOGroupFlags : UInt32
    {
        HasBSPTree          = 0x1,
        HasLightMap         = 0x2,
        HasVertexColors     = 0x4,
        IsOutdoors          = 0x8,
        Unknown1            = 0x10,
        Unknown2            = 0x20,
        UseExteriorLighting = 0x40,
        IsUnreachable       = 0x80,
        Unknown3            = 0x100,
        HasLights           = 0x200,
        Unknown4            = 0x400,
        HasDoodads          = 0x800,
        HasWater            = 0x1000,
        IsIndoors           = 0x2000,
        Unknown5            = 0x4000,
        Unknown6            = 0x8000,
        AlwaysDraw          = 0x10000,
        Unknown7            = 0x20000,
        DoShowSkybox        = 0x40000,
        WaterIsOcean        = 0x80000,
        Unknown8            = 0x100000,
        AllowsMount         = 0x200000,
        Unknown9            = 0x400000,
        Unknown10           = 0x800000,
        UseMOCV2ForTexBlend = 0x1000000,
        Has2MOTV            = 0x2000000,
        IsAntiPortal        = 0x4000000,
        Unknown11           = 0x8000000,
        Unknown12           = 0x10000000,
        Unknown13           = 0x20000000,
        Unknown14           = 0x40000000, // Is this the flag for 3 MOTV?
        Unknown15           = 0x80000000
    }
}
