﻿//  Author: Nathan Handley (nathanhandley@protonmail.com)
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

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Common
{
    internal class Vector3 : IByteSerializable
    {
        public float X;
        public float Y;
        public float Z;

        public static readonly Vector3 Right = new Vector3(1, 0, 0);
        public static readonly Vector3 Up = new Vector3(0, 1, 0);
        public static readonly Vector3 Forward = new Vector3(0, 0, 1);

        public Vector3()
        {

        }

        public Vector3(Vector3 vector)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public UInt32 GetBytesSize()
        {
            return 12;
        }

        public List<byte> ToBytes()
        {
            List<byte> returnBytes = new List<byte>();
            returnBytes.AddRange(BitConverter.GetBytes(X));
            returnBytes.AddRange(BitConverter.GetBytes(Y));
            returnBytes.AddRange(BitConverter.GetBytes(Z));
            return returnBytes;
        }

        public static Vector3 operator -(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static Vector3 operator +(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static Vector3 operator *(Vector3 v1, float val)
        {
            return new Vector3(v1.X * val, v1.Y * val, v1.Z * val);
        }

        public static Vector3 operator /(Vector3 v1, float scalar)
        {
            return new Vector3(v1.X / scalar, v1.Y / scalar, v1.Z / scalar);
        }

        public static float Dot(Vector3 v1, Vector3 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }

        public static Vector3 Cross(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.Y * v2.Z - v1.Z * v2.Y,
                               v1.Z * v2.X - v1.X * v2.Z,
                               v1.X * v2.Y - v1.Y * v2.X);
        }

        public float GetDistance(Vector3 otherVector)
        {
            float dx = otherVector.X - X;
            float dy = otherVector.Y - Y;
            float dz = otherVector.Z - Z;
            return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public static Vector3 GetMin(Vector3 v1, Vector3 v2)
        {
            return new Vector3(Math.Min(v1.X, v2.X), Math.Min(v1.Y, v2.Y), Math.Min(v1.Z, v2.Z));
        }

        public static Vector3 GetMax(Vector3 v1, Vector3 v2)
        {
            return new Vector3(Math.Max(v1.X, v2.X), Math.Max(v1.Y, v2.Y), Math.Max(v1.Z, v2.Z));
        }

        public void Rotate(float rotationX, float rotationY, float rotationZ, float rotationW)
        {
            // Calculate intermediate values
            float x2 = rotationX + rotationX, y2 = rotationY + rotationY, z2 = rotationZ + rotationZ;
            float xx = rotationX * x2, yy = rotationY * y2, zz = rotationZ * z2;
            float xy = rotationX * y2, xz = rotationX * z2, yz = rotationY * z2;
            float wx = rotationW * x2, wy = rotationW * y2, wz = rotationW * z2;

            // Apply rotation matrix
            float transformedX = (1 - (yy + zz)) * X + (xy - wz) * Y + (xz + wy) * Z;
            float transformedY = (xy + wz) * X + (1 - (xx + zz)) * Y + (yz - wx) * Z;
            float transformedZ = (xz - wy) * X + (yz + wx) * Y + (1 - (xx + yy)) * Z;
            X = transformedX;
            Y = transformedY;
            Z = transformedZ;
        }

        public void Rotate(Quaternion rotation)
        {
            Rotate(rotation.X, rotation.Y, rotation.Z, rotation.W);
        }

        public void Rotate(QuaternionShort rotation)
        {
            Rotate(rotation.X, rotation.Y, rotation.Z, rotation.W);
        }

        public void Scale(float scale)
        {
            X *= scale;
            Y *= scale;
            Z *= scale;
        }

        public float GetMagnitude()
        {
            return MathF.Sqrt(X * X + Y * Y + Z * Z);
        }

        public static Vector3 GetNormalized(Vector3 v)
        {
            float magnitude = v.GetMagnitude();
            if (magnitude > 0)
            {
                return v / magnitude;
            }
            else
            {
                // Return a zero vector if the magnitude is zero to avoid division by zero
                return new Vector3(0, 0, 0);
            }
        }

        public float GetLengthSquared()
        {
            return X * X + Y * Y + Z * Z;
        }
    }
}
