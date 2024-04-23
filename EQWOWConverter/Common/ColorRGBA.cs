using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Common
{
    internal class ColorRGBA
    {
        public byte R = 0;
        public byte G = 0;
        public byte B = 0;
        public byte A = 0;

        public List<byte> ToBytes()
        {
            List<byte> returnBytes = new List<byte>();
            returnBytes.Add(R);
            returnBytes.Add(G);
            returnBytes.Add(B);
            returnBytes.Add(A);
            return returnBytes;
        }
    }
}
