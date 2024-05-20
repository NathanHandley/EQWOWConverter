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
using EQWOWConverter.ModelObjects.WOW;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.ModelObjects
{
    internal class ModelTrackSequences<T> where T : IByteSerializable
    {
        public ModelAnimationInterpolationType InterpolationType = ModelAnimationInterpolationType.None;
        public UInt16 GlobalSequenceID = 65535;
        public List<ModelTrackSequenceTimestamps> Timestamps = new List<ModelTrackSequenceTimestamps>();
        public List<ModelTrackSequenceValues<T>> Values = new List<ModelTrackSequenceValues<T>>();
        public UInt32 TimestampsOffset = 0;
        public UInt32 ValuesOffset = 0;

        /// <summary>
        /// Adds a new track sequence, and returns the index
        /// </summary>
        public int AddSequence()
        {
            Timestamps.Add(new ModelTrackSequenceTimestamps());
            Values.Add(new ModelTrackSequenceValues<T>());
            return Timestamps.Count - 1;
        }

        /// <summary>
        /// Adds a value to the pair related for a sequence
        /// </summary>
        public void AddValueToSequence(int sequenceID, UInt32 timestamp, T value)
        {
            if (sequenceID >= Timestamps.Count)
            {
                Logger.WriteLine("ERROR AddValueToSequence out of range exception!");
                return;
            }
            Timestamps[sequenceID].AddTimestamp(timestamp);
            Values[sequenceID].AddValue(value);
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
