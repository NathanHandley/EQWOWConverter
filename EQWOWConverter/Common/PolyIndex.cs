using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Common
{
    internal class PolyIndex
    {
        public int MaterialIndex;
        public int V1;
        public int V2;
        public int V3;

        // WARNING, these are converting to short int from int, remediate TODO:
        public List<byte> ToBytes()
        {
            List<byte> returnBytes = new List<byte>();
            returnBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(V1)));
            returnBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(V2)));
            returnBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(V3)));
            return returnBytes;
        }
    }
}
