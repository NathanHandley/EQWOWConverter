using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Common
{
    internal class Vector3
    {
        public float X;
        public float Y;
        public float Z;

        public List<byte> ToBytes()
        {
            List<byte> returnBytes = new List<byte>();
            break here too
            returnBytes.AddRange(BitConverter.GetBytes(X));
            returnBytes.AddRange(BitConverter.GetBytes(Y));
            returnBytes.AddRange(BitConverter.GetBytes(Z));
            return returnBytes;
        }
    }
}
