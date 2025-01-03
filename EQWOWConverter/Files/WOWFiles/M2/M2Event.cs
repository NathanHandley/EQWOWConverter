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
using EQWOWConverter.ObjectModels;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal class M2Event : IOffsetByteSerializable
    {
        string Identifier = string.Empty;
        public int Data = 0;
        public UInt32 ParentBoneID = 0;
        public Vector3 Position = new Vector3();
        public ObjectModelAnimationInterpolationType InterpolationType = ObjectModelAnimationInterpolationType.None;
        public UInt16 GlobalSequenceID = 65535;
        public List<ObjectModelTrackSequenceTimestamps> Timestamps = new List<ObjectModelTrackSequenceTimestamps>();
        public UInt32 TimestampsOffset = 0;

        public void PopulateAsIdleSoundDSL(Sound sound)
        {
            Identifier = "$DSL";
            Data = sound.DBCID;
            Timestamps.Add(new ObjectModelTrackSequenceTimestamps());
            Timestamps[0].AddTimestamp(0);
        }

        public void PopulateAsDeathThudDTH(ObjectModel wowObjectModel)
        {
            Identifier = "$DTH";
            ParentBoneID = Convert.ToUInt32(wowObjectModel.GetFirstBoneIndexForEQBoneNames("dth"));
            Timestamps.Add(new ObjectModelTrackSequenceTimestamps());

            // TODO: Make this work for more than "disarmed"
            for (int i = 0; i < wowObjectModel.ModelAnimations.Count; i++)
            {
                Timestamps.Add(new ObjectModelTrackSequenceTimestamps());
            }
        }

        public void PopulateAsHandleCombatAnimCAH(ObjectModel wowObjectModel)
        {
            Identifier = "$CAH";
            ParentBoneID = Convert.ToUInt32(wowObjectModel.GetFirstBoneIndexForEQBoneNames("cah"));

            // TODO: Make this work for more than "unarmed"
            // TODO: Figure out how to time the strike with the animation
            for (int i = 0; i < wowObjectModel.ModelAnimations.Count; i++)
            {
                Timestamps.Add(new ObjectModelTrackSequenceTimestamps());
                if (wowObjectModel.AnimationLookups[Convert.ToInt16(AnimationType.AttackUnarmed)] != i)
                    Timestamps[i].AddTimestamp(0);
            }
        }

        public void PopulateAsPlayWeaponSwooshSoundCSS(ObjectModel wowObjectModel)
        {
            Identifier = "$CSS";
            ParentBoneID = Convert.ToUInt32(wowObjectModel.GetFirstBoneIndexForEQBoneNames("css"));

            // TODO: Make this work for more than "unarmed"
            // TODO: Figure out how to time the strike with the animation
            for (int i = 0; i < wowObjectModel.ModelAnimations.Count; i++)
            {
                Timestamps.Add(new ObjectModelTrackSequenceTimestamps());
                if (wowObjectModel.AnimationLookups[Convert.ToInt16(AnimationType.AttackUnarmed)] != i)
                    Timestamps[i].AddTimestamp(0);
            }
        }

        public void PopulateAsPlayCombatActionAnimKitCPP(ObjectModel wowObjectModel)
        {
            Identifier = "$CPP";
            ParentBoneID = Convert.ToUInt32(wowObjectModel.GetFirstBoneIndexForEQBoneNames("cpp"));

            // TODO: Make this work for more than "unarmed"
            // TODO: Figure out how to time the strike with the animation
            for (int i = 0; i < wowObjectModel.ModelAnimations.Count; i++)
            {
                Timestamps.Add(new ObjectModelTrackSequenceTimestamps());
                if (wowObjectModel.AnimationLookups[Convert.ToInt16(AnimationType.AttackUnarmed)] != i)
                    Timestamps[i].AddTimestamp(0);
            }
        }

        public void PopulateAsPlayWoundAnimKitHIT(ObjectModel wowObjectModel)
        {
            Identifier = "$HIT";
            ParentBoneID = Convert.ToUInt32(wowObjectModel.GetFirstBoneIndexForEQBoneNames("hit"));

            for (int i = 0; i < wowObjectModel.ModelAnimations.Count; i++)
            {
                Timestamps.Add(new ObjectModelTrackSequenceTimestamps());
                Timestamps[i].AddTimestamp(0);
            }
        }

        public void PopulateAsPlayFidgetSoundFD1(ObjectModel wowObjectModel)
        {
            Identifier = "$FD1";
            ParentBoneID = Convert.ToUInt32(wowObjectModel.GetFirstBoneIndexForEQBoneNames("fd1"));

            for (int i = 0; i < wowObjectModel.ModelAnimations.Count; i++)
            {
                Timestamps.Add(new ObjectModelTrackSequenceTimestamps());

                // Always animation at index 2
                if (i == 2)
                    Timestamps[i].AddTimestamp(0);
            }
        }

        public void PopulateAsPlayFidgetSoundFD2(ObjectModel wowObjectModel)
        {
            Identifier = "$FD2";
            ParentBoneID = Convert.ToUInt32(wowObjectModel.GetFirstBoneIndexForEQBoneNames("fd2"));

            for (int i = 0; i < wowObjectModel.ModelAnimations.Count; i++)
            {
                Timestamps.Add(new ObjectModelTrackSequenceTimestamps());

                // Always animation at index 3
                if (i == 3)
                    Timestamps[i].AddTimestamp(0);
            }
        }

        public void PopulateAsHandleFootfallAnimEventFSD(ObjectModel wowObjectModel)
        {
            Identifier = "$FSD";
            ParentBoneID = Convert.ToUInt32(wowObjectModel.GetFirstBoneIndexForEQBoneNames("fsd"));

            for (int i = 0; i < wowObjectModel.ModelAnimations.Count; i++)
            {
                Timestamps.Add(new ObjectModelTrackSequenceTimestamps());

                // Set for the movement animations
                if (wowObjectModel.ModelAnimations[i].AnimationType == AnimationType.Walk ||
                    wowObjectModel.ModelAnimations[i].AnimationType == AnimationType.Run ||
                    wowObjectModel.ModelAnimations[i].AnimationType == AnimationType.RunLeft ||
                    wowObjectModel.ModelAnimations[i].AnimationType == AnimationType.RunRight ||
                    wowObjectModel.ModelAnimations[i].AnimationType == AnimationType.Walkbackwards)
                {
                    Timestamps[i].AddTimestamp(0);
                }
            }
        }
        
        public UInt32 GetHeaderSize()
        {
            UInt32 size = 0;
            size += 4;  // Identifier
            size += 4;  // Data
            size += 4;  // ParentBoneID
            size += 12; // Position
            size += 2;  // Timer InterpolationType
            size += 2;  // Timer GlobalSequenceID
            size += 4;  // Timer Number of timestamps
            size += 4;  // Timer Offset of timestamps
            return size;
        }

        public List<Byte> GetHeaderBytes()
        {
            if (Identifier.Length != 4)
                throw new Exception("M2Event could not return header bytes since the Identifier wasn't 4 characters");

            List<byte> returnBytes = new List<byte>();
            returnBytes.Add((byte)Identifier[0]);
            returnBytes.Add((byte)Identifier[1]);
            returnBytes.Add((byte)Identifier[2]);
            returnBytes.Add((byte)Identifier[3]);
            returnBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(Data)));
            returnBytes.AddRange(BitConverter.GetBytes(ParentBoneID));
            returnBytes.AddRange(Position.ToBytes());
            returnBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(InterpolationType)));
            returnBytes.AddRange(BitConverter.GetBytes(GlobalSequenceID));
            returnBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(Timestamps.Count)));
            returnBytes.AddRange(BitConverter.GetBytes(TimestampsOffset));
            return returnBytes;
        }

        public void AddDataBytes(ref List<byte> byteBuffer)
        {
            // Don't add anything if there's no data to add
            if (Timestamps.Count == 0)
                return;

            // Set space aside for timestamp headers
            TimestampsOffset = Convert.ToUInt32(byteBuffer.Count);
            UInt32 timestampHeaderBlockSize = 0;
            foreach (ObjectModelTrackSequenceTimestamps timestamp in Timestamps)
                timestampHeaderBlockSize += timestamp.GetHeaderSize();

            // Reserve the space for all headers in the byte buffer
            UInt32 totalSubHeaderSpaceToReserve = timestampHeaderBlockSize;
            for (int i = 0; i < totalSubHeaderSpaceToReserve; i++)
                byteBuffer.Add(0);

            // Add timestamp data
            foreach (ObjectModelTrackSequenceTimestamps timestamp in Timestamps)
            {
                timestamp.DataOffset = Convert.ToUInt32(byteBuffer.Count);
                byteBuffer.AddRange(timestamp.GetDataBytes());
            }

            // Write the timestamp header data
            List<byte> timestampHeaderBytes = new List<byte>();
            foreach (ObjectModelTrackSequenceTimestamps timestamp in Timestamps)
                timestampHeaderBytes.AddRange(timestamp.GetHeaderBytes());
            for (int i = 0; i < totalSubHeaderSpaceToReserve; i++)
                byteBuffer[i + Convert.ToInt32(TimestampsOffset)] = timestampHeaderBytes[i];
        }
    }
}
