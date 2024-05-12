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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.ModelObjects
{
    internal class ModelAnimation
    {
        UInt16 ID = 0; // This correlates to AnimationData.dbc.  0 is standing
        UInt16 SubAnimationID = 0; // wowdev also refers to this as variationIndex
        UInt32 DurationInMS = 10000;
        float MoveSpeed = 0f;
        ModelAnimationFlags Flags = 0;
        Int16 PlayFrequency = 32767; // Always make this add up to 32767 for animations of same type
        UInt16 Padding = 0;
        UInt32 ReplayMin = 0;
        UInt32 ReplayMax = 0;
        UInt32 BlendTime = 150; 
        BoundingBox BoundingBox = new BoundingBox();
        float BoundingRadius = 0f;
        Int16 NextAnimation = -1; // aka, variationNext
        UInt16 AliasNext = 0; // Id in the list of animations if this is an alias (?)

        public List<byte> ToBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(ID));
            bytes.AddRange(BitConverter.GetBytes(SubAnimationID));
            bytes.AddRange(BitConverter.GetBytes(DurationInMS));
            bytes.AddRange(BitConverter.GetBytes(MoveSpeed));
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(Flags)));
            bytes.AddRange(BitConverter.GetBytes(PlayFrequency));
            bytes.AddRange(BitConverter.GetBytes(Padding));
            bytes.AddRange(BitConverter.GetBytes(ReplayMin));
            bytes.AddRange(BitConverter.GetBytes(ReplayMax));
            bytes.AddRange(BitConverter.GetBytes(BlendTime));
            bytes.AddRange(BoundingBox.ToBytesHighRes());
            bytes.AddRange(BitConverter.GetBytes(BoundingRadius));
            bytes.AddRange(BitConverter.GetBytes(NextAnimation));
            bytes.AddRange(BitConverter.GetBytes(AliasNext));
            return bytes;
        }
    }
}
