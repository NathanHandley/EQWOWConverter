using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal interface M2ByteSerializable
    {
        public void AddToByteBuffer(ref List<byte> byteBuffer);
    }
}
