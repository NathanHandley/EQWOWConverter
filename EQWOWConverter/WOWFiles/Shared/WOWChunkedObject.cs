//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2024 Nathan Handley
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal class WOWChunkedObject
    {
        protected List<byte> WrapInChunk(string token, byte[] dataBlock)
        {
            if (token.Length != 4)
                Logger.WriteLine("Error, WrapInChunk has a token that isn't a length of 4 (value = '" + token + "')");
            List<byte> wrappedChunk = new List<byte>();
            wrappedChunk.Add((byte)token[3]);
            wrappedChunk.Add((byte)token[2]);
            wrappedChunk.Add((byte)token[1]);
            wrappedChunk.Add((byte)token[0]);
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

        protected byte GetPackedFlags(params byte[] flags)
        {
            byte packedFlags = 0;
            foreach (byte flag in flags)
            {
                packedFlags |= flag;
            }
            return packedFlags;
        }
    }
}
