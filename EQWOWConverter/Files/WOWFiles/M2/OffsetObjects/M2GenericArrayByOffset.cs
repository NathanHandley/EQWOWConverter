using EQWOWConverter.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EQWOWConverter.WOWFiles
{
    internal class M2GenericArrayByOffset<T> : IOffsetByteSerializable where T : IByteSerializable
    {
        private UInt32 Count = 0;
        private UInt32 Offset = 0;
        private List<T> Elements = new List<T>();

        public void Add(T element)
        {
            Elements.Add(element);
            Count = Convert.ToUInt32(Elements.Count);
        }

        public void AddArray(List<T> elementArray)
        {
            for (int i = 0; i < elementArray.Count; i++)
                Add(elementArray[i]);
        }

        public List<Byte> GetHeaderBytes()
        {
            List<byte> returnBytes = new List<byte>();
            returnBytes.AddRange(BitConverter.GetBytes(Count));
            returnBytes.AddRange(BitConverter.GetBytes(Offset));
            return returnBytes;
        }

        public void AddDataBytes(ref List<byte> byteBuffer)
        {
            if (Count == 0)
                return;
            Offset = Convert.ToUInt32(byteBuffer.Count);
            for (int i = 0; i < Elements.Count; ++i)
                byteBuffer.AddRange(Elements[i].ToBytes());
        }
    }
}
