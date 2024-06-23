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

        public BoundingBox(float bottomX, float bottomY, float bottomZ, float topX, float topY, float topZ, float addedBoundary)
        {
            bottomX -= addedBoundary;
            bottomY -= addedBoundary;
            bottomZ -= addedBoundary;
            topX += addedBoundary;
            topY += addedBoundary;
            topZ += addedBoundary;
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

        public static BoundingBox GenerateBoxFromVectors(List<ModelVertex> vertices, float minSize = 0)
        {
            BoundingBox boundingBox = new BoundingBox();
            bool isFirstVector = true;
            foreach (ModelVertex renderVert in vertices)
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
    }
}