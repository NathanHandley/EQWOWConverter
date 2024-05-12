using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.ModelObjects.WOW
{
    internal enum ModelAnimationInterpolationType : UInt16
    {
        None                = 0,
        Linear              = 1,
        CubicBezierSpline   = 2,
        CubicHermiteSpline  = 3
    }
}
