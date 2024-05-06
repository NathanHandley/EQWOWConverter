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
    internal class BoundingBox
    {
        public Vector3 TopCornerHighRes = new Vector3();
        public Vector3 BottomCornerHigHRes = new Vector3();

        public BoundingBox()
        {

        }

        public BoundingBox(BoundingBox box)
        {
            TopCornerHighRes = new Vector3(box.TopCornerHighRes);
            BottomCornerHigHRes = new Vector3(box.BottomCornerHigHRes);
        }

        public List<byte> ToBytesHighRes()
        {
            List<byte> returnBytes = new List<byte>();
            returnBytes.AddRange(BitConverter.GetBytes(BottomCornerHigHRes.X));
            returnBytes.AddRange(BitConverter.GetBytes(BottomCornerHigHRes.Y));
            returnBytes.AddRange(BitConverter.GetBytes(BottomCornerHigHRes.Z));
            returnBytes.AddRange(BitConverter.GetBytes(TopCornerHighRes.X));
            returnBytes.AddRange(BitConverter.GetBytes(TopCornerHighRes.Y));
            returnBytes.AddRange(BitConverter.GetBytes(TopCornerHighRes.Z));
            return returnBytes;
        }

        public List<byte> ToBytesLowRes()
        {
            List<byte> returnBytes = new List<byte>();
            Int16 TopX = Convert.ToInt16(Math.Round(TopCornerHighRes.X, 0, MidpointRounding.AwayFromZero));
            Int16 TopY = Convert.ToInt16(Math.Round(TopCornerHighRes.Y, 0, MidpointRounding.AwayFromZero));
            Int16 TopZ = Convert.ToInt16(Math.Round(TopCornerHighRes.Z, 0, MidpointRounding.AwayFromZero));
            Int16 BottomX = Convert.ToInt16(Math.Round(BottomCornerHigHRes.X, 0, MidpointRounding.AwayFromZero));
            Int16 BottomY = Convert.ToInt16(Math.Round(BottomCornerHigHRes.Y, 0, MidpointRounding.AwayFromZero));
            Int16 BottomZ = Convert.ToInt16(Math.Round(BottomCornerHigHRes.Z, 0, MidpointRounding.AwayFromZero));
            returnBytes.AddRange(BitConverter.GetBytes(BottomX));
            returnBytes.AddRange(BitConverter.GetBytes(BottomY));
            returnBytes.AddRange(BitConverter.GetBytes(BottomZ));
            returnBytes.AddRange(BitConverter.GetBytes(TopX));
            returnBytes.AddRange(BitConverter.GetBytes(TopY));
            returnBytes.AddRange(BitConverter.GetBytes(TopZ));
            return returnBytes;
        }

        public float GetXDistance()
        {
            return Math.Abs(TopCornerHighRes.X - BottomCornerHigHRes.X);
        }

        public float GetYDistance()
        {
            return Math.Abs(TopCornerHighRes.Y - BottomCornerHigHRes.Y);
        }

        public float GetZDistance()
        {
            return Math.Abs(TopCornerHighRes.Z - BottomCornerHigHRes.Z);
        }

        public bool DoesIntersectTriangle(Vector3 point1, Vector3 point2, Vector3 point3)
        {
            // Verticies contained in the box are given collisions, and should be checked first
            if (IsPointInside(point1) || IsPointInside(point2) || IsPointInside(point3))
                return true;

            // Edge 1
            Vector3 line1Min = GetMinsForLine(point1, point2);
            Vector3 line1Max = GetMaxForLine(point1, point2);
            if (DoesLineCastOverlapExist(line1Min, line1Max) == false)
                return false;

            // Edge 2
            Vector3 line2Min = GetMinsForLine(point2, point3);
            Vector3 line2Max = GetMaxForLine(point2, point3);
            if (DoesLineCastOverlapExist(line2Min, line2Max) == false)
                return false;

            // Edge 3
            Vector3 line3Min = GetMinsForLine(point3, point1);
            Vector3 line3Max = GetMaxForLine(point3, point1);
            if (DoesLineCastOverlapExist(line3Min, line3Max) == false)
                return false;

            return true;
        }

        private bool DoesLineCastOverlapExist(Vector3 line1Min, Vector3 line1Max)
        {
            // Only works because this is an axis aligned box
            if (line1Min.X > TopCornerHighRes.X)
                return false;
            else if (line1Min.Y > TopCornerHighRes.Y)
                return false;
            else if (line1Min.Z > TopCornerHighRes.Z)
                return false;
            else if (line1Max.X < BottomCornerHigHRes.X)
                return false;
            else if (line1Max.Y <  BottomCornerHigHRes.Y)
                return false;
            else if (line1Max.Z <  BottomCornerHigHRes.Z)
                return false;
            return true;
        }

        private Vector3 GetMinsForLine(Vector3 point1, Vector3 point2)
        {
            Vector3 returnVector = new Vector3();
            if (point1.X < point2.X)
                returnVector.X = point1.X;
            else
                returnVector.X = point2.X;
            if (point1.Y < point2.Y)
                returnVector.Y = point1.Y;
            else
                returnVector.Y = point2.Y;
            if (point1.Z < point2.Z)
                returnVector.Z = point1.Z;
            else
                returnVector.Z = point2.Z;
            return returnVector;
        }

        private Vector3 GetMaxForLine(Vector3 point1, Vector3 point2)
        {
            Vector3 returnVector = new Vector3();
            if (point1.X > point2.X)
                returnVector.X = point1.X;
            else
                returnVector.X = point2.X;
            if (point1.Y > point2.Y)
                returnVector.Y = point1.Y;
            else
                returnVector.Y = point2.Y;
            if (point1.Z > point2.Z)
                returnVector.Z = point1.Z;
            else
                returnVector.Z = point2.Z;
            return returnVector;
        }

        private bool IsPointInside(Vector3 point)
        {
            return point.X >= BottomCornerHigHRes.X && point.X <= TopCornerHighRes.X &&
                   point.Y >= BottomCornerHigHRes.Y && point.Y <= TopCornerHighRes.Y &&
                   point.Z >= BottomCornerHigHRes.Z && point.Z <= TopCornerHighRes.Z;
        }
    }
}