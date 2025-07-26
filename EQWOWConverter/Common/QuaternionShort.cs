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
    // This is a 'short' version of the quaternion that is used for bones
    internal class QuaternionShort : IByteSerializable
    {
        public float X = 0;
        public float Y = 0;
        public float Z = 0;
        public float W = 1;

        public QuaternionShort() { }

        public QuaternionShort(QuaternionShort other)
        {
            X = other.X;
            Y = other.Y;
            Z = other.Z;
            W = other.W;
        }

        public QuaternionShort(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        private UInt16 GetFloatAsShort(float value)
        {
            return Convert.ToUInt16((value + 1f) * 32767);
        }

        public UInt32 GetBytesSize()
        {
            return 8;
        }

        public void RecalculateToShortest()
        {
            // Normalize it
            System.Numerics.Quaternion currentRotation = new System.Numerics.Quaternion(X, Y, Z, W);
            System.Numerics.Quaternion.Normalize(currentRotation);

            // Use SLEPRT to find shortest
            System.Numerics.Quaternion shortestRotation = System.Numerics.Quaternion.Slerp(System.Numerics.Quaternion.Identity, currentRotation, 1.0f);
            X = shortestRotation.X;
            Y = shortestRotation.Y;
            Z = shortestRotation.Z;
            W = shortestRotation.W;
        }

        public void RecalculateToShortestFromOther(QuaternionShort otherQuaternion)
        {
            // Create and normalize the other
            System.Numerics.Quaternion otherRotation = new System.Numerics.Quaternion(otherQuaternion.X,
                otherQuaternion.Y, otherQuaternion.Z, otherQuaternion.W);
            System.Numerics.Quaternion.Normalize(otherRotation);

            // Normalize this
            System.Numerics.Quaternion currentRotation = new System.Numerics.Quaternion(X, Y, Z, W);
            System.Numerics.Quaternion.Normalize(currentRotation);

            // Use SLEPRT to find shortest
            System.Numerics.Quaternion shortestRotation = System.Numerics.Quaternion.Slerp(otherRotation, currentRotation, 1.0f);
            X = shortestRotation.X;
            Y = shortestRotation.Y;
            Z = shortestRotation.Z;
            W = shortestRotation.W;
        }

        public List<Byte> ToBytes()
        {
            List<Byte> bytes = new List<Byte>();
            bytes.AddRange(BitConverter.GetBytes(GetFloatAsShort(X)));
            bytes.AddRange(BitConverter.GetBytes(GetFloatAsShort(Y)));
            bytes.AddRange(BitConverter.GetBytes(GetFloatAsShort(Z)));
            bytes.AddRange(BitConverter.GetBytes(GetFloatAsShort(W)));
            return bytes;
        }

        // Generates a list of quaternions at equidistant locations around a circle
        public static List<QuaternionShort> GetQuaternionsInCircle(int numOfCircleDivisions)
        {
            List<QuaternionShort> rotations = new List<QuaternionShort>();
            if (numOfCircleDivisions <= 1)
            {
                rotations.Add(new QuaternionShort());
                return rotations;
            }

            // Generate quaternions for each mark around the circle
            float angleIncrement = 360.0f / numOfCircleDivisions;
            for (int i = 0; i < numOfCircleDivisions; i++)
            {
                float angleDegrees = i * angleIncrement;
                float angleRadians = angleDegrees * (float)Math.PI / 180.0f;
                float halfAngle = angleRadians / 2.0f;

                // Rotation axis is up Z
                float sinHalfAngle = (float)Math.Sin(halfAngle);
                float cosHalfAngle = (float)Math.Cos(halfAngle);

                rotations.Add(new QuaternionShort(0.0f, 0.0f, sinHalfAngle, cosHalfAngle));
            }

            return rotations;
        }
    }
}
