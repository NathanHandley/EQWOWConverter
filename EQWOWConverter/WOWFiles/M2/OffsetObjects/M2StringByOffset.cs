using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EQWOWConverter.WOWFiles
{
    internal class M2StringByOffset : IOffsetByteSerializable
    {
        private UInt32 Length = 0;
        private UInt32 Offset = 0;
        public string StringValue;

        public M2StringByOffset(string stringValue)
        {
            StringValue = stringValue + "\0";
            Length = Convert.ToUInt32(StringValue.Length);
        }

        public void AddToByteBuffer(ref List<byte> byteBuffer)
        {
            Offset = Convert.ToUInt32(byteBuffer.Count);
            byteBuffer.AddRange(Encoding.ASCII.GetBytes(StringValue));
        }

        public List<Byte> GetHeaderBytes()
        {
            List<byte> returnBytes = new List<byte>();
            returnBytes.AddRange(BitConverter.GetBytes(Length));
            returnBytes.AddRange(BitConverter.GetBytes(Offset));
            return returnBytes;
        }

        public void AddDataBytes(ref List<byte> byteBuffer)
        {
            Offset = Convert.ToUInt32(byteBuffer.Count);
            byteBuffer.AddRange(Encoding.ASCII.GetBytes(StringValue));
        }
    }
}
