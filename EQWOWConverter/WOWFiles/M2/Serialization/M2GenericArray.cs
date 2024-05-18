using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EQWOWConverter.WOWFiles
{
    internal class M2GenericArray<T>
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
    }
}
