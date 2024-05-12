using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.ModelObjects
{
    internal enum ModelAnimationFlags : UInt32
    {
        Set0x80OnLoad                   = 0x01, // ? found this in the 010 Editor template
        Unknown1                        = 0x02,
        Unknown2                        = 0x04,
        Unknown3                        = 0x08,
        LoadLowPrioritySequence         = 0x10,
        AnimationInM2                   = 0x20,
        AliasedWithFollowupAnimation    = 0x40,
        BlendedAnimation                = 0x80,
        SequenceStored0x                = 0x100,
        BlindTimeInAndOut               = 0x200
    }
}
