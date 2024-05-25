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

using EQWOWConverter.ModelObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Common
{
    internal class BoundingBox
    {
        public Vector3 TopCorner = new Vector3();
        public Vector3 BottomCorner = new Vector3();

        public BoundingBox()
        {

        }

        public BoundingBox(float bottomX, float bottomY, float bottomZ, float topX, float topY, float topZ)
        {
            BottomCorner = new Vector3(bottomX, bottomY, bottomZ);
            TopCorner = new Vector3(topX, topY, topZ);
        }

        public BoundingBox(BoundingBox box)
        {
            TopCorner = new Vector3(box.TopCorner);
            BottomCorner = new Vector3(box.BottomCorner);
        }

        public List<byte> ToBytesHighRes()
        {
            List<byte> returnBytes = new List<byte>();
            returnBytes.AddRange(BitConverter.GetBytes(BottomCorner.X));
            returnBytes.AddRange(BitConverter.GetBytes(BottomCorner.Y));
            returnBytes.AddRange(BitConverter.GetBytes(BottomCorner.Z));
            returnBytes.AddRange(BitConverter.GetBytes(TopCorner.X));
            returnBytes.AddRange(BitConverter.GetBytes(TopCorner.Y));
            returnBytes.AddRange(BitConverter.GetBytes(TopCorner.Z));
            return returnBytes;
        }

        public List<byte> ToBytesForWDT()
        {
            List<byte> returnBytes = new List<byte>();
            returnBytes.AddRange(BitConverter.GetBytes(BottomCorner.Y));
            returnBytes.AddRange(BitConverter.GetBytes(BottomCorner.Z));
            returnBytes.AddRange(BitConverter.GetBytes(BottomCorner.X));
            returnBytes.AddRange(BitConverter.GetBytes(TopCorner.Y));
            returnBytes.AddRange(BitConverter.GetBytes(TopCorner.Z));
            returnBytes.AddRange(BitConverter.GetBytes(TopCorner.X));
            return returnBytes;
        }

        public List<byte> ToBytesLowRes()
        {
            List<byte> returnBytes = new List<byte>();
            Int16 TopX = Convert.ToInt16(Math.Round(TopCorner.X, 0, MidpointRounding.AwayFromZero));
            Int16 TopY = Convert.ToInt16(Math.Round(TopCorner.Y, 0, MidpointRounding.AwayFromZero));
            Int16 TopZ = Convert.ToInt16(Math.Round(TopCorner.Z, 0, MidpointRounding.AwayFromZero));
            Int16 BottomX = Convert.ToInt16(Math.Round(BottomCorner.X, 0, MidpointRounding.AwayFromZero));
            Int16 BottomY = Convert.ToInt16(Math.Round(BottomCorner.Y, 0, MidpointRounding.AwayFromZero));
            Int16 BottomZ = Convert.ToInt16(Math.Round(BottomCorner.Z, 0, MidpointRounding.AwayFromZero));
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
            return Math.Abs(TopCorner.X - BottomCorner.X);
        }

        public float GetYDistance()
        {
            return Math.Abs(TopCorner.Y - BottomCorner.Y);
        }

        public float GetZDistance()
        {
            return Math.Abs(TopCorner.Z - BottomCorner.Z);
        }

        public Vector3 GetCenter()
        {
            Vector3 vector3 = new Vector3();
            vector3.X = (TopCorner.X + BottomCorner.X) / 2;
            vector3.Y = (TopCorner.Y + BottomCorner.Y) / 2;
            vector3.Z = (TopCorner.Z + BottomCorner.Z) / 2;
            return vector3;            
        }
        
        public float FurthestPointDistanceFromCenter()
        {
            System.Numerics.Vector3 cornerSystem = new System.Numerics.Vector3(TopCorner.X, TopCorner.Y, TopCorner.Z);
            Vector3 center = GetCenter();
            System.Numerics.Vector3 centerSystem = new System.Numerics.Vector3(center.X, center.Y, center.Z);
            return System.Numerics.Vector3.Distance(cornerSystem, centerSystem);
        }

        public bool DoesIntersectTriangle(Vector3 point1, Vector3 point2, Vector3 point3)
        {
            // Verticies contained in the box are given collisions, and should be checked first
            if (IsPointInside(point1) || IsPointInside(point2) || IsPointInside(point3))
                return true;

            // Test each line of the triangle for collision
            if (DoesLineCollide(point1, point2))
                return true;
            if (DoesLineCollide(point1, point3))
                return true;
            if (DoesLineCollide(point2, point3))
                return true;

            return false;
        }

        public bool IsPointInside(Vector3 point)
        {
            return point.X >= BottomCorner.X && point.X <= TopCorner.X &&
                   point.Y >= BottomCorner.Y && point.Y <= TopCorner.Y &&
                   point.Z >= BottomCorner.Z && point.Z <= TopCorner.Z;
        }

        public static BoundingBox GenerateBoxFromVectors(List<Vector3> verticies)
        {
            BoundingBox boundingBox = new BoundingBox();
            foreach (Vector3 renderVert in verticies)
            {
                if (renderVert.X < boundingBox.BottomCorner.X)
                    boundingBox.BottomCorner.X = renderVert.X;
                if (renderVert.Y < boundingBox.BottomCorner.Y)
                    boundingBox.BottomCorner.Y = renderVert.Y;
                if (renderVert.Z < boundingBox.BottomCorner.Z)
                    boundingBox.BottomCorner.Z = renderVert.Z;

                if (renderVert.X > boundingBox.TopCorner.X)
                    boundingBox.TopCorner.X = renderVert.X;
                if (renderVert.Y > boundingBox.TopCorner.Y)
                    boundingBox.TopCorner.Y = renderVert.Y;
                if (renderVert.Z > boundingBox.TopCorner.Z)
                    boundingBox.TopCorner.Z = renderVert.Z;
            }
            return boundingBox;
        }

        public static BoundingBox GenerateBoxFromVectors(List<ModelVertex> verticies, float minSize = 0)
        {
            BoundingBox boundingBox = new BoundingBox();
            foreach (ModelVertex renderVert in verticies)
            {
                if (renderVert.Position.X < boundingBox.BottomCorner.X)
                    boundingBox.BottomCorner.X = renderVert.Position.X;
                if (renderVert.Position.Y < boundingBox.BottomCorner.Y)
                    boundingBox.BottomCorner.Y = renderVert.Position.Y;
                if (renderVert.Position.Z < boundingBox.BottomCorner.Z)
                    boundingBox.BottomCorner.Z = renderVert.Position.Z;

                if (renderVert.Position.X > boundingBox.TopCorner.X)
                    boundingBox.TopCorner.X = renderVert.Position.X;
                if (renderVert.Position.Y > boundingBox.TopCorner.Y)
                    boundingBox.TopCorner.Y = renderVert.Position.Y;
                if (renderVert.Position.Z > boundingBox.TopCorner.Z)
                    boundingBox.TopCorner.Z = renderVert.Position.Z;
            }

            // Enforce a min size if needed
            if (minSize > 0)
            {
                float amountToAddX = (boundingBox.TopCorner.X - boundingBox.BottomCorner.X);
                boundingBox.TopCorner.X += amountToAddX;
                boundingBox.BottomCorner.X -= amountToAddX;
                float amountToAddY = (boundingBox.TopCorner.Y - boundingBox.BottomCorner.Y);
                boundingBox.TopCorner.Y += amountToAddY;
                boundingBox.BottomCorner.Y -= amountToAddY;
                float amountToAddZ = (boundingBox.TopCorner.Z - boundingBox.BottomCorner.Z);
                boundingBox.TopCorner.Z += amountToAddZ;
                boundingBox.BottomCorner.Z -= amountToAddZ;
            }

            return boundingBox;
        }

        // The following three methods were adapted from an answer found here: https://stackoverflow.com/questions/3235385/given-a-bounding-box-and-a-line-two-points-determine-if-the-line-intersects-t
        private bool DoesLineCollide(Vector3 point1, Vector3 point2)
        {
            Vector3 collidePoint = new Vector3();
            if (GetIntersection(point1.X - TopCorner.X, point2.X - TopCorner.X, point1, point2, ref collidePoint) && InBox(collidePoint, 1))
                return true;
            if (GetIntersection(point1.Y - TopCorner.Y, point2.Y - TopCorner.Y, point1, point2, ref collidePoint) && InBox(collidePoint, 2))
                return true;
            if (GetIntersection(point1.Z - TopCorner.Z, point2.Z - TopCorner.Z, point1, point2, ref collidePoint) && InBox(collidePoint, 3))
                return true;
            if (GetIntersection(point1.X - BottomCorner.X, point2.X - BottomCorner.X, point1, point2, ref collidePoint) && InBox(collidePoint, 1))
                return true;
            if (GetIntersection(point1.Y - BottomCorner.Y, point2.Y - BottomCorner.Y, point1, point2, ref collidePoint) && InBox(collidePoint, 2))
                return true;
            if (GetIntersection(point1.Z - BottomCorner.Z, point2.Z - BottomCorner.Z, point1, point2, ref collidePoint) && InBox(collidePoint, 3))
                return true;
            return false;
        }
        bool InBox(Vector3 collidePoint, int testAxis)
        {
            if (testAxis == 1 && collidePoint.Z > TopCorner.Z && collidePoint.Z < BottomCorner.Z && collidePoint.Y > TopCorner.Y && collidePoint.Y < BottomCorner.Y)
                return true;
            else if (testAxis == 2 && collidePoint.Z > TopCorner.Z && collidePoint.Z < BottomCorner.Z && collidePoint.X > TopCorner.X && collidePoint.X < BottomCorner.X)
                return true;
            else if (testAxis == 3 && collidePoint.X > TopCorner.X && collidePoint.X < BottomCorner.X && collidePoint.Y > TopCorner.Y && collidePoint.Y < BottomCorner.Y)
                return true;
            return false;
        }
        bool GetIntersection(float distance1, float distance2, Vector3 point1, Vector3 point2, ref Vector3 collidePoint)
        {
            if ((distance1 * distance2) >= 0.0f) return false;
            if (distance1 == distance2) return false;
            collidePoint = point1 + (point2 - point1) * (-distance1 / (distance2 - distance1));
            return true;
        }
    }
}