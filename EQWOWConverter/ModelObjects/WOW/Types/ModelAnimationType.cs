using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.ModelObjects.WOW.Types
{
    internal enum ModelAnimationType : UInt16
    {
        Stand           = 0,
        Death           = 0,
        Spell           = 2,
        Stop            = 3,
        Walk            = 4,
        Run             = 5,
        Dead            = 6,
        Rise            = 7,
        StandWound      = 8,
        CombatWound     = 9,
        CombatCritical  = 10,
        ShuffleLeft     = 11,
        ShuffleRight    = 12,
        AttackUnarmed   = 16,
        Attack1H        = 17,
        Attack2H        = 18,  
        ReadyUnarmed    = 25,
        Ready1H         = 26,
        Ready2H         = 27,
        SpelLCast       = 32,
        Fall            = 40,
        Swim            = 42,
        SwimLeft        = 43,
        SwimRight       = 44,
        SwimBackwards   = 45,           
    }
}
