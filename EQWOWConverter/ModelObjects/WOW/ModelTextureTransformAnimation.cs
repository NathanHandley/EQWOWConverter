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

using EQWOWConverter.Common;
using EQWOWConverter.WOWFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.ModelObjects
{
    internal class ModelTextureTransformAnimation : IByteSerializable
    {
        public UInt32 GetBytesSize()
        {
            // Temp, hard coded
            Logger.WriteLine("WARNING ModelTextureTransformAnimation.GetBytesSize is NYI");
            UInt32 size = 0;
            size += 2;  // InterpolationType
            size += 2;  // GlobalSequenceID
            size += 4;  // Number of timestamps
            size += 4;  // Offset of timestamps
            size += 4;  // Number of values
            size += 4;  // Offset of values

            size += 2;  // InterpolationType
            size += 2;  // GlobalSequenceID
            size += 4;  // Number of timestamps
            size += 4;  // Offset of timestamps
            size += 4;  // Number of values
            size += 4;  // Offset of values

            size += 2;  // InterpolationType
            size += 2;  // GlobalSequenceID
            size += 4;  // Number of timestamps
            size += 4;  // Offset of timestamps
            size += 4;  // Number of values
            size += 4;  // Offset of values
            return size;
        }

        public List<byte> ToBytes()
        {
            List<byte> bytes = new List<byte>();
            // Temp, hard coded
            Logger.WriteLine("WARNING ModelTextureTransformAnimation.ToBytes is NYI");

            // Translation
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(0)));
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(65535)));
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            // Rotation
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(0)));
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(65535)));
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            // Scaling
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(0)));
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(65535)));
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
            return bytes;
        }
    }
}
