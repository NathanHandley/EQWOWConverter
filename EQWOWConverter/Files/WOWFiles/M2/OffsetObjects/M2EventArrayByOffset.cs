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
using EQWOWConverter.Common;

namespace EQWOWConverter.WOWFiles
{
    internal class M2EventArrayByOffset : IOffsetByteSerializable
    {
        private UInt32 Offset = 0;
        private List<M2Event> Events = new List<M2Event>();

        public void AddIdleSoundEvent(Sound sound)
        {
            M2Event newEvent = new M2Event();
            newEvent.PopulateAsIdleSound(sound);
            Events.Add(newEvent);
        }

        public List<Byte> GetHeaderBytes()
        {
            List<byte> returnBytes = new List<byte>();
            returnBytes.AddRange(BitConverter.GetBytes(Events.Count));
            returnBytes.AddRange(BitConverter.GetBytes(Offset));
            return returnBytes;
        }

        public void AddDataBytes(ref List<byte> byteBuffer)
        {
            // Update header bytes
            if (Events.Count == 0)
            {
                Offset = 0;
                return;
            }
            Offset = Convert.ToUInt32(byteBuffer.Count);

            // Calculate the space needed for the event headers and reserve it
            int eventHeaderStartOffset = byteBuffer.Count;
            UInt32 eventHeaderBytesCount = 0;
            foreach (var eventElement in Events)
                eventHeaderBytesCount += eventElement.GetHeaderSize();
            for (int i = 0; i < eventHeaderBytesCount; i++)
                byteBuffer.Add(0);

            // Add the data bytes
            foreach (var eventElement in Events)
                eventElement.AddDataBytes(ref byteBuffer);

            // Write the header data into the byte buffer
            List<byte> headerBytes = new List<byte>();
            foreach (var eventElement in Events)
                headerBytes.AddRange(eventElement.GetHeaderBytes());
            for (int i = 0; i < headerBytes.Count; i++)
                byteBuffer[i + eventHeaderStartOffset] = headerBytes[i];
        }
    }
}
