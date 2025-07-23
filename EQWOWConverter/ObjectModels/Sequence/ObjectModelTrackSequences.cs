//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2025 Nathan Handley
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

namespace EQWOWConverter.ObjectModels
{
    internal class ObjectModelTrackSequences<T> where T : IByteSerializable
    {
        public ObjectModelAnimationInterpolationType InterpolationType = ObjectModelAnimationInterpolationType.None;
        public UInt16 GlobalSequenceID = 65535;
        public List<ObjectModelTrackSequenceTimestamps> Timestamps = new List<ObjectModelTrackSequenceTimestamps>();
        public List<ObjectModelTrackSequenceValues<T>> Values = new List<ObjectModelTrackSequenceValues<T>>();
        public UInt32 TimestampsOffset = 0;
        public UInt32 ValuesOffset = 0;

        public void Clear()
        {
            Timestamps.Clear();
            Values.Clear();
        }

        public int AddSequence()
        {
            Timestamps.Add(new ObjectModelTrackSequenceTimestamps());
            Values.Add(new ObjectModelTrackSequenceValues<T>());
            return Timestamps.Count - 1;
        }

        public void AddValueToSequence(int sequenceID, UInt32 timestamp, T value)
        {
            if (sequenceID >= Timestamps.Count)
            {
                Logger.WriteError("Error AddValueToSequence out of range exception!");
                return;
            }
            Timestamps[sequenceID].AddTimestamp(timestamp);
            Values[sequenceID].AddValue(value);
        }

        public void AddValueToLastSequence(UInt32 timestamp, T value)
        {
            if (Timestamps.Count == 0)
            {
                Logger.WriteError("Error AddValueToLastSequence failed because there are no sequences");
                return;
            }
            Timestamps[Timestamps.Count-1].AddTimestamp(timestamp);
            Values[Timestamps.Count-1].AddValue(value);
        }

        public void ReplicateFirstValueToEnd(int sequenceID, UInt32 timestamp)
        {
            if (Timestamps[sequenceID].Timestamps.Count == 0)
            {
                Logger.WriteError("Error ReplicateFirstValueToEnd failed because there was no first value");
                return;
            }
            Timestamps[sequenceID].AddTimestamp(timestamp);
            Values[sequenceID].AddValue(Values[sequenceID].Values[0]);
        }

        public UInt32 GetHeaderSize()
        {
            UInt32 size = 0;
            size += 2;  // InterpolationType
            size += 2;  // GlobalSequenceID
            size += 4;  // Number of timestamps
            size += 4;  // Offset of timestamps
            size += 4;  // Number of values
            size += 4;  // Offset of values
            return size;
        }
    }
}
