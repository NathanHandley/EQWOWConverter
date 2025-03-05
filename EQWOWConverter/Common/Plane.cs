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

namespace EQWOWConverter.Common
{
    internal class Plane : IByteSerializable
    {
        Vector3 Normal = new Vector3();
        float Distance = 0f;

        public Plane(float x, float y, float z, float distance)
        {
            Normal = new Vector3(x, y, z);
            Distance = distance;
        }

        public UInt32 GetBytesSize()
        {
            return 16;
        }

        public List<Byte> ToBytes()
        {
            List<Byte> bytes = new List<Byte>();
            bytes.AddRange(BitConverter.GetBytes(Normal.X));
            bytes.AddRange(BitConverter.GetBytes(Normal.Y));
            bytes.AddRange(BitConverter.GetBytes(Normal.Z));
            bytes.AddRange(BitConverter.GetBytes(Distance));
            return bytes;
        }
    }
}
