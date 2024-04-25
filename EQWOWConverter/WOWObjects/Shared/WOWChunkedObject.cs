using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWObjects
{
    internal class WOWChunkedObject
    {
        protected List<byte> WrapInChunk(string token, byte[] dataBlock)
        {
            if (token.Length != 4)
                Logger.WriteLine("Error, WrapInChunk has a token that isn't a length of 4 (value = '" + token + "')");
            List<byte> wrappedChunk = new List<byte>();
            wrappedChunk.AddRange(Encoding.ASCII.GetBytes(token));
            wrappedChunk.AddRange(BitConverter.GetBytes(Convert.ToUInt32(dataBlock.Length)));
            wrappedChunk.AddRange(dataBlock);
            return wrappedChunk;
        }

        protected UInt32 GetPackedFlags(params UInt32[] flags)
        {
            UInt32 packedFlags = 0;
            foreach (UInt32 flag in flags)
            {
                packedFlags |= flag;
            }
            return packedFlags;
        }
    }
}
