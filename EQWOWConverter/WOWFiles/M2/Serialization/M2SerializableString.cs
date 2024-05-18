using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EQWOWConverter.WOWFiles
{
    internal class M2SerializableString : M2ByteSerializable
    {
        private UInt32 Length = 0;
        private UInt32 Offset = 0;
        public string StringValue;

        public M2SerializableString(string stringValue)
        {
            StringValue = stringValue + "\0";
            Length = Convert.ToUInt32(StringValue.Length);
        }

        public void AddToByteBuffer(ref List<byte> byteBuffer)
        {
            Offset = Convert.ToUInt32(byteBuffer.Count);
            byteBuffer.AddRange(Encoding.ASCII.GetBytes(StringValue));
        }
    }
}
