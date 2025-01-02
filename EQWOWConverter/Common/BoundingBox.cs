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

using EQWOWConverter.ObjectModels;
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

        public BoundingBox(Vector3 centerPosition, float apothem)
        {
            BottomCorner.X = centerPosition.X - apothem;
            BottomCorner.Y = centerPosition.Y - apothem;
            BottomCorner.Z = centerPosition.Z - apothem;
            TopCorner.X = centerPosition.X + apothem;
            TopCorner.Y = centerPosition.Y + apothem;
            TopCorner.Z = centerPosition.Z + apothem;
        }

        public BoundingBox(BoundingBox box)
        {
            TopCorner = new Vector3(box.TopCorner);
            BottomCorner = new Vector3(box.BottomCorner);
        }

        public BoundingBox(Vector3 topCorner, Vector3 bottomCorner)
        {
            TopCorner = new Vector3(topCorner);
            BottomCorner = new Vector3(bottomCorner);
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

        public float FurthestPointDistanceFromCenterXOnly()
        {
            System.Numerics.Vector3 cornerSystem = new System.Numerics.Vector3(TopCorner.X, 0, 0);
            Vector3 center = GetCenter();
            System.Numerics.Vector3 centerSystem = new System.Numerics.Vector3(center.X, 0, 0);
            return System.Numerics.Vector3.Distance(cornerSystem, centerSystem);
        }

        public float FurthestPointDistanceFromCenterYOnly()
        {
            System.Numerics.Vector3 cornerSystem = new System.Numerics.Vector3(0, TopCorner.Y, 0);
            Vector3 center = GetCenter();
            System.Numerics.Vector3 centerSystem = new System.Numerics.Vector3(0, center.Y, 0);
            return System.Numerics.Vector3.Distance(cornerSystem, centerSystem);
        }

        public bool ContainsPoint(Vector3 point)
        {
            if (point.X < BottomCorner.X)
                return false;
            if (point.Y < BottomCorner.Y)
                return false;
            if (point.Z < BottomCorner.Z)
                return false;
            if (point.X > TopCorner.X)
                return false;
            if (point.Y > TopCorner.Y)
                return false;
            if (point.Z > TopCorner.Z)
                return false;
            return true;
        }

        public bool ContainsBox(BoundingBox other)
        {
            float edgePad = 0.001f;
            if (TopCorner.X + edgePad < other.TopCorner.X)
                return false;
            if (TopCorner.Y + edgePad < other.TopCorner.Y)
                return false;
            if (TopCorner.Z + edgePad < other.TopCorner.Z)
                return false;
            if (other.BottomCorner.X + edgePad < BottomCorner.X)
                return false;
            if (other.BottomCorner.Y + edgePad < BottomCorner.Y)
                return false;
            if (other.BottomCorner.Z + edgePad < BottomCorner.Z)
                return false;
            return true;
        }

        public static BoundingBox GenerateBoxFromBoxes(List<BoundingBox> boxes)
        {
            if (boxes.Count == 0)
                throw new Exception("GenerateBoxFromBoxes had no boxes");
            BoundingBox boundingBox = new BoundingBox(boxes[0]);
            for (int i = 1; i < boxes.Count; i++)
            {
                BoundingBox box = boxes[i];
                if (box.TopCorner.X > boundingBox.TopCorner.X)
                    boundingBox.TopCorner.X = box.TopCorner.X;
                if (box.TopCorner.Y > boundingBox.TopCorner.Y)
                    boundingBox.TopCorner.Y = box.TopCorner.Y;
                if (box.TopCorner.Z > boundingBox.TopCorner.Z)
                    boundingBox.TopCorner.Z = box.TopCorner.Z;

                if (box.BottomCorner.X < boundingBox.BottomCorner.X)
                    boundingBox.BottomCorner.X = box.BottomCorner.X;
                if (box.BottomCorner.Y < boundingBox.BottomCorner.Y)
                    boundingBox.BottomCorner.Y = box.BottomCorner.Y;
                if (box.BottomCorner.Z < boundingBox.BottomCorner.Z)
                    boundingBox.BottomCorner.Z = box.BottomCorner.Z;
            }

            return boundingBox;
        }

        public static BoundingBox GenerateBoxFromVectors(List<Vector3> vertices, float addedBoundary)
        {
            BoundingBox boundingBox = new BoundingBox();
            bool isFirstVector = true;
            foreach (Vector3 renderVert in vertices)
            {
                if (isFirstVector)
                {
                    boundingBox.BottomCorner = new Vector3(renderVert);
                    boundingBox.TopCorner = new Vector3(renderVert);
                    isFirstVector = false;
                }
                else
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
            }
            boundingBox.BottomCorner.X -= addedBoundary;
            boundingBox.BottomCorner.Y -= addedBoundary;
            boundingBox.BottomCorner.Z -= addedBoundary;
            boundingBox.TopCorner.X += addedBoundary;
            boundingBox.TopCorner.Y += addedBoundary;
            boundingBox.TopCorner.Z += addedBoundary;
            return boundingBox;
        }

        public static BoundingBox GenerateBoxFromVectors(List<ObjectModelVertex> vertices, float minSize = 0)
        {
            BoundingBox boundingBox = new BoundingBox();
            bool isFirstVector = true;
            foreach (ObjectModelVertex renderVert in vertices)
            {
                if (isFirstVector)
                {
                    boundingBox.BottomCorner = new Vector3(renderVert.Position);
                    boundingBox.TopCorner = new Vector3(renderVert.Position);
                    isFirstVector = false;
                }
                else
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
            }

            // Enforce a min size if needed
            if (minSize > 0)
            {
                if (boundingBox.GetXDistance() < minSize)
                {
                    float amountToAdd = (minSize - boundingBox.GetXDistance()) * 0.5f;
                    boundingBox.TopCorner.X += amountToAdd;
                    boundingBox.BottomCorner.X -= amountToAdd;
                }
                if (boundingBox.GetYDistance() < minSize)
                {
                    float amountToAdd = (minSize - boundingBox.GetYDistance()) * 0.5f;
                    boundingBox.TopCorner.Y += amountToAdd;
                    boundingBox.BottomCorner.Y -= amountToAdd;
                }
                if (boundingBox.GetZDistance() < minSize)
                {
                    float amountToAdd = (minSize - boundingBox.GetZDistance()) * 0.5f;
                    boundingBox.TopCorner.Z += amountToAdd;
                    boundingBox.BottomCorner.Z -= amountToAdd;
                }
            }

            return boundingBox;
        }

        public bool DoesIntersectBox(BoundingBox other)
        {
            // It's an intersection if either box contains the other
            if (ContainsBox(other) == true)
                return true;
            if (other.ContainsBox(this) == true)
                return true;

            // Otherwise, check boundaries
            float edgePad = 0.001f;
            if (BottomCorner.X + edgePad > other.TopCorner.X)
                return false;
            if (BottomCorner.Y + edgePad > other.TopCorner.Y)
                return false;
            if (BottomCorner.Z + edgePad > other.TopCorner.Z)
                return false;
            if (other.BottomCorner.X + edgePad > TopCorner.X)
                return false;
            if (other.BottomCorner.Y + edgePad > TopCorner.Y)
                return false;
            if (other.BottomCorner.Z + edgePad > TopCorner.Z)
                return false;
            return true;
        }

        public static void SplitBoundingIntersect(BoundingBox box1, BoundingBox box2, out List<BoundingBox> intersecting, out List<BoundingBox> box1SubBoxes, out List<BoundingBox> box2SubBoxes)
        {
            intersecting = new List<BoundingBox>();
            box1SubBoxes = new List<BoundingBox>();
            box2SubBoxes = new List<BoundingBox>();

            // Exit if nothing intersects
            if (box1.DoesIntersectBox(box2) == false)
            {
                box1SubBoxes.Add(box1);
                box2SubBoxes.Add(box2);
                return;
            }

            // Calculate the intersection box
            Vector3 intersectionTop = Vector3.GetMin(box1.TopCorner, box2.TopCorner);
            Vector3 intersectionBottom = Vector3.GetMax(box1.BottomCorner, box2.BottomCorner);
            intersecting.Add(new BoundingBox(intersectionTop, intersectionBottom));

            // Create a box for any possible protusion
            // X
            if (box1.TopCorner.X > box2.TopCorner.X)
                box1SubBoxes.Add(new BoundingBox(box1.TopCorner, new Vector3(intersectionTop.X, box1.BottomCorner.Y, box1.BottomCorner.Z)));
            if (box2.TopCorner.X > box1.TopCorner.X)
                box2SubBoxes.Add(new BoundingBox(box2.TopCorner, new Vector3(intersectionTop.X, box2.BottomCorner.Y, box2.BottomCorner.Z)));
            if (box1.BottomCorner.X < box2.BottomCorner.X)
                box1SubBoxes.Add(new BoundingBox(new Vector3(intersectionBottom.X, box1.TopCorner.Y, box1.TopCorner.Z), box1.BottomCorner));
            if (box2.BottomCorner.X < box1.BottomCorner.X)
                box2SubBoxes.Add(new BoundingBox(new Vector3(intersectionBottom.X, box2.TopCorner.Y, box2.TopCorner.Z), box2.BottomCorner));

            // Y
            if (box1.TopCorner.Y > box2.TopCorner.Y)
                box1SubBoxes.Add(new BoundingBox(new Vector3(intersectionTop.X, box1.TopCorner.Y, box1.TopCorner.Z), new Vector3(intersectionBottom.X, intersectionTop.Y, box1.BottomCorner.Z)));
            if (box2.TopCorner.Y > box1.TopCorner.Y)
                box2SubBoxes.Add(new BoundingBox(new Vector3(intersectionTop.X, box2.TopCorner.Y, box2.TopCorner.Z), new Vector3(intersectionBottom.X, intersectionTop.Y, box2.BottomCorner.Z)));
            if (box1.BottomCorner.Y < box2.BottomCorner.Y)
                box1SubBoxes.Add(new BoundingBox(new Vector3(intersectionTop.X, intersectionBottom.Y, box1.TopCorner.Z), new Vector3(intersectionBottom.X, box1.BottomCorner.Y, box1.BottomCorner.Z)));
            if (box2.BottomCorner.Y < box1.BottomCorner.Y)
                box2SubBoxes.Add(new BoundingBox(new Vector3(intersectionTop.X, intersectionBottom.Y, box2.TopCorner.Z), new Vector3(intersectionBottom.X, box2.BottomCorner.Y, box2.BottomCorner.Z)));

            // Z
            if (box1.TopCorner.Z > box2.TopCorner.Z)
                box1SubBoxes.Add(new BoundingBox(new Vector3(intersectionTop.X, intersectionTop.Y, box1.TopCorner.Z), new Vector3(intersectionBottom.X, intersectionBottom.Y, intersectionTop.Z)));
            if (box2.TopCorner.Z > box1.TopCorner.Z)
                box2SubBoxes.Add(new BoundingBox(new Vector3(intersectionTop.X, intersectionTop.Y, box2.TopCorner.Z), new Vector3(intersectionBottom.X, intersectionBottom.Y, intersectionTop.Z)));
            if (box1.BottomCorner.Z < box2.BottomCorner.Z)
                box1SubBoxes.Add(new BoundingBox(new Vector3(intersectionTop.X, intersectionTop.Y, intersectionBottom.Z), new Vector3(intersectionBottom.X, intersectionBottom.Y, box1.BottomCorner.Z)));
            if (box2.BottomCorner.Z < box1.BottomCorner.Z)
                box2SubBoxes.Add(new BoundingBox(new Vector3(intersectionTop.X, intersectionTop.Y, intersectionBottom.Z), new Vector3(intersectionBottom.X, intersectionBottom.Y, box2.BottomCorner.Z)));
        }
    }
}