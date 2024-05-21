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

namespace EQWOWConverter.Common
{
    internal class Quaternion : IByteSerializable
    {
        public float X = 0;
        public float Y = 0;
        public float Z = 0;
        public float W = 0;

        public Quaternion() { }
        public Quaternion(float x, float y, float z, float w)
        {
            X = x; 
            Y = y; 
            Z = z; 
            W = w;
        }

        public UInt32 GetBytesSize()
        {
            return 16;
        }

        public List<Byte> ToBytes()
        {
            List<Byte> bytes = new List<Byte>();
            bytes.AddRange(BitConverter.GetBytes(X));
            bytes.AddRange(BitConverter.GetBytes(Y));
            bytes.AddRange(BitConverter.GetBytes(Z));
            bytes.AddRange(BitConverter.GetBytes(W));
            return bytes;
        }

        public static Quaternion GenerateQuaternionRotation(float x, float y, float z)
        {
            // Convert to radians
            float radiansX = MathF.PI * x / 180.0f;
            float radiansY = MathF.PI * y / 180.0f;
            float radiansZ = MathF.PI * z / 180.0f;

            // Create quaternions for each axis rotation (using .Net library)
            System.Numerics.Quaternion qZ = System.Numerics.Quaternion.CreateFromAxisAngle(System.Numerics.Vector3.UnitZ, radiansZ);
            System.Numerics.Quaternion qX = System.Numerics.Quaternion.CreateFromAxisAngle(System.Numerics.Vector3.UnitX, radiansX);
            System.Numerics.Quaternion qY = System.Numerics.Quaternion.CreateFromAxisAngle(System.Numerics.Vector3.UnitY, radiansY);

            // Combine the quaternions in the Z, X, Y order
            System.Numerics.Quaternion combined = qY * qX * qZ;

            // Generate and return the quaternion
            System.Numerics.Quaternion rotated = combined * System.Numerics.Quaternion.Identity;
            Quaternion returnResult = new Quaternion();
            returnResult.X = rotated.X;
            returnResult.Y = rotated.Y;
            returnResult.Z = rotated.Z;
            returnResult.W = rotated.W;

            return returnResult;
        }
    }
}
