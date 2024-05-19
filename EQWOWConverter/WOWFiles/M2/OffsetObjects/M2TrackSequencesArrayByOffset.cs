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
using EQWOWConverter.ModelObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal class M2TrackSequencesArrayByOffset<T> where T : ByteSerializable
    {
        private List<M2TrackSequences<T>> TrackSequences = new List<M2TrackSequences<T>>();
        private UInt32 Count = 0;
        private UInt32 Offset = 0;

        public void Add(ModelTrackSequences<T> modelTrackSequences)
        {
            TrackSequences.Add(new M2TrackSequences<T>(modelTrackSequences));
            Count = Convert.ToUInt32(TrackSequences.Count);
        }

        public void AddArray(List<ModelTrackSequences<T>> trackSequences)
        {
            foreach(var trackSequence in trackSequences)
                Add(trackSequence);
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
            // Don't add anything if there's nothing
            if (Count == 0)
            {
                Offset = 0;
                return;
            }
            else Offset = Convert.ToUInt32(byteBuffer.Count);

            // Calculate the space needed for the track sequence headers and reserve it
            int trackSequencesHeaderStartOffset = byteBuffer.Count;
            UInt32 allTrackSequenceHeaderBytesCount = 0;
            foreach (var trackSequence in TrackSequences)
                allTrackSequenceHeaderBytesCount += trackSequence.GetHeaderSize();
            for (int i = 0; i < allTrackSequenceHeaderBytesCount; i++)
                byteBuffer.Add(0);

            // Add the data bytes
            foreach (var trackSequence in TrackSequences)
                trackSequence.AddDataBytes(ref byteBuffer);

            // Write the header data into the byte buffer
                List<byte> headerBytes = new List<byte>();
            foreach(var trackSequence in TrackSequences)
                headerBytes.AddRange(trackSequence.GetHeaderBytes());
            for (int i = 0; i < headerBytes.Count; i++)
                byteBuffer[i + trackSequencesHeaderStartOffset] = headerBytes[i];
        }
    }
}
