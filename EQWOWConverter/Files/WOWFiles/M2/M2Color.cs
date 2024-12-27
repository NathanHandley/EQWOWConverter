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
using EQWOWConverter.ObjectModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal class M2Color : IOffsetByteSerializable
    {
        M2TrackSequences<ColorRGBf> ColorTracks = new M2TrackSequences<ColorRGBf>();
        M2TrackSequences<Fixed16> AlphaTracks = new M2TrackSequences<Fixed16>(); // 32767 = Opaque, 0 = Transparent
        
        public M2Color(ColorRGBf color, int numOfAnimations)
        {
            ColorTracks.TrackSequences.AddSequence();
            ColorTracks.TrackSequences.AddValueToLastSequence(0, color);
            AlphaTracks.TrackSequences.AddSequence();
            AlphaTracks.TrackSequences.AddValueToLastSequence(0, new Fixed16(32767));
        }

        public UInt32 GetHeaderSize()
        {
            UInt32 size = 0;
            size += ColorTracks.GetHeaderSize();
            size += AlphaTracks.GetHeaderSize();
            return size;
        }

        public List<byte> GetHeaderBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(ColorTracks.GetHeaderBytes());
            bytes.AddRange(AlphaTracks.GetHeaderBytes());
            return bytes;
        }

        public void AddDataBytes(ref List<byte> byteBuffer)
        {
            ColorTracks.AddDataBytes(ref byteBuffer);
            AlphaTracks.AddDataBytes(ref byteBuffer);
        }
    }
}
