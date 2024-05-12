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
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Common
{
    internal class Vector3 : ByteSerializable
    {
        public float X;
        public float Y;
        public float Z;

        public Vector3()
        {

        }

        public Vector3(Vector3 vector)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }

        // Note that this is not a good practice in most instances outside of this use case.  Since the float data is
        // being generated and read in a fixed way, this equal comparison works fine.  But, if this is to be used
        // outside of this engine, a different approach using abs and epsilon should be used
        public bool IsEqualForIndexComparisons(Vector3 otherVector)
        {
            // Itself is always a match
            if (otherVector == this)
                return true;
            if (X != otherVector.X)
                return false;
            if (Y != otherVector.Y)
                return false;
            if (Z != otherVector.Z)
                return false;

            return true;
        }

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
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
    }
}
