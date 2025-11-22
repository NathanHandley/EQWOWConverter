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

namespace EQWOWConverter.Zones
{
    internal class ZoneDoodadInstance
    {
        public ZoneDoodadInstanceType DoodadType;
        public string ObjectName = string.Empty;
        public UInt32 WMOObjectNameOffset = 0;
        public UInt32 ADTObjectNameIndex = 0;
        public ZoneDoodadInstanceFlags Flags = ZoneDoodadInstanceFlags.AcceptProjectedTexture; // Not yet implemented
        public Vector3 Position = new Vector3();
        public Quaternion Orientation = new Quaternion();
        public float Scale = 1.0f;
        public ColorRGBA Color = new ColorRGBA();

        public ZoneDoodadInstance(ZoneDoodadInstanceType type)
        {
            DoodadType = type;
        }

        public List<byte> ToBytesForWMO()
        {
            List<byte> returnBytes = new List<byte>();

            // The flags and name offset share a UInt32, effectively making it UInt24 and UInt8
            returnBytes.AddRange(BitConverter.GetBytes(WMOObjectNameOffset));
            returnBytes.AddRange(Position.ToBytes());
            returnBytes.AddRange(Orientation.ToBytes());
            returnBytes.AddRange(BitConverter.GetBytes(Scale));
            returnBytes.AddRange(Color.ToBytesBGRA());
            return returnBytes;
        }

        public List<byte> ToBytesForADT(UInt32 uniqueID)
        {
            // Note that models have to be translated to map space, which has a different origin point
            float centerPointValue = 51200f / 3f; // 64 x 533.333/2
            Vector3 positionVector = new Vector3(centerPointValue + Position.X, Position.Y, centerPointValue + Position.Z);
            List<byte> returnBytes = new List<byte>();
            returnBytes.AddRange(BitConverter.GetBytes(ADTObjectNameIndex));
            returnBytes.AddRange(BitConverter.GetBytes(uniqueID));
            returnBytes.AddRange(positionVector.ToBytes());
            returnBytes.AddRange(Orientation.ToMDDFEulerDegrees().ToBytes());
            returnBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(1024))); // Scale, 1024 = 1.0f
            returnBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(0))); // Flags, always zero
            return returnBytes;
        }
    }
}
