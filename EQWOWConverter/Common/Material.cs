using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Common
{
    internal class Material
    {
        public uint Index;
        public string Name = string.Empty;
        public List<string> AnimationTextures = new List<string>();
        public uint AnimationDelayMs;
    }
}
