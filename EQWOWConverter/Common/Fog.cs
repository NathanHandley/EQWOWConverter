using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Common
{
    internal class Fog
    {
        public bool HasInfiniteRadius = false;
        public Vector3 Position = new Vector3();
        public float NearRadius = 0;
        public float FarRadius = 0;
        public float NormalStartScalar = 0.25f;
        public float NormalEnd = 444.4445f;
        public float UnderwaterStartScalar = -0.5f;
        public float UnderwaterEnd = 222.2222f;
    }
}
