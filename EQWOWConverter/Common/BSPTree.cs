//  Author: Nathan Handley 2024 (nathanhandley@protonmail.com)
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
using System.Xml.Linq;

namespace EQWOWConverter.Common
{
    internal class BSPTree
    {
        private static readonly UInt16 CONFIG_MIN_SPLIT_SIZE = 2000;

        private class SplitBox
        {
            public BoundingBox BoxA = new BoundingBox();
            public BoundingBox BoxB = new BoundingBox();
            public float PlaneDistance;
        }

        public List<BSPNode> Nodes = new List<BSPNode>();
        public List<UInt16> FaceIndicies = new List<UInt16>();

        // Generate on create
        public BSPTree(BoundingBox boundingBox, List<Vector3> verticies, List<TriangleFace> triangleFaces)
        {
            // Spawn out from the root node
            List<UInt32> faceIndicies = new List<UInt32>();
            for (uint i = 0; i < triangleFaces.Count; i++)
                faceIndicies.Add(i);
            AddNode(boundingBox, faceIndicies, triangleFaces, verticies);
        }

        private SplitBox GenerateSplitBox(BoundingBox box, BSPNodeFlag planeSplitType)
        {
            SplitBox splitBox = new SplitBox();
            splitBox.BoxA = new BoundingBox(box);
            splitBox.BoxB = new BoundingBox(box);
            switch (planeSplitType)
            {
                case BSPNodeFlag.XYPlane:
                    {
                        float planeSplitDistance = (box.TopCornerHighRes.Z + box.BottomCornerHigHRes.Z) * 0.5f;
                        splitBox.PlaneDistance = planeSplitDistance;
                        splitBox.BoxA.TopCornerHighRes.Z = planeSplitDistance;
                        splitBox.BoxB.BottomCornerHigHRes.Z = planeSplitDistance;
                    } break;
                case BSPNodeFlag.YZPlane:
                    {
                        float planeSplitDistance = (box.TopCornerHighRes.X + box.BottomCornerHigHRes.X) * 0.5f;
                        splitBox.PlaneDistance = planeSplitDistance;
                        splitBox.BoxA.TopCornerHighRes.X = planeSplitDistance;
                        splitBox.BoxB.BottomCornerHigHRes.X = planeSplitDistance;
                    } break;
                case BSPNodeFlag.XZPlane:
                    {
                        float planeSplitDistance = (box.TopCornerHighRes.Y + box.BottomCornerHigHRes.Y) * 0.5f;
                        splitBox.PlaneDistance = planeSplitDistance;
                        splitBox.BoxA.TopCornerHighRes.Y = planeSplitDistance;
                        splitBox.BoxB.BottomCornerHigHRes.Y = planeSplitDistance;
                    } break;
                default:
                    {
                        Logger.WriteLine("BSPTree.GenerateSplitBox Error!  Invalid planeSplitType provided.");
                    } break;
            }
            return splitBox;
        }

        private UInt16 AddNode(BoundingBox boundingBox, List<UInt32> boxFacesIndicies, List<TriangleFace> allTriangleFaces, List<Vector3> allVerticies)
        {
            // If we reach the minimum split size, terminate this branch
            if (boxFacesIndicies.Count <= CONFIG_MIN_SPLIT_SIZE)
            {
                // Create this final leaf
                BSPNode leafNode = new BSPNode(BSPNodeFlag.Leaf, -1, -1,
                    Convert.ToUInt16(boxFacesIndicies.Count), Convert.ToUInt16(FaceIndicies.Count), 0.0f);
                foreach (UInt32 faceIndex in boxFacesIndicies)
                    FaceIndicies.Add(Convert.ToUInt16(faceIndex));
                Nodes.Add(leafNode);
                return Convert.ToUInt16(Nodes.Count - 1);
            }

            // Calculate plane type and split the box long way
            BSPNodeFlag planeSplitType;
            if (boundingBox.GetXDistance() > boundingBox.GetYDistance() && (boundingBox.GetXDistance() > boundingBox.GetZDistance()))
                planeSplitType = BSPNodeFlag.YZPlane;
            else if (boundingBox.GetYDistance() > boundingBox.GetXDistance() && (boundingBox.GetYDistance() > boundingBox.GetZDistance()))
                planeSplitType = BSPNodeFlag.XZPlane;
            else
                planeSplitType = BSPNodeFlag.XYPlane;
            SplitBox splitBox = GenerateSplitBox(boundingBox, planeSplitType);

            // Store face indicies that collide with each box
            List<UInt32> boxAFaceIndicies = new List<UInt32>();
            foreach(UInt32 boxFaceIndex in boxFacesIndicies)
            {
                Vector3 point1 = allVerticies[allTriangleFaces[Convert.ToInt32(boxFaceIndex)].V1];
                Vector3 point2 = allVerticies[allTriangleFaces[Convert.ToInt32(boxFaceIndex)].V2];
                Vector3 point3 = allVerticies[allTriangleFaces[Convert.ToInt32(boxFaceIndex)].V3];
                if (splitBox.BoxA.DoesIntersectTriangle(point1, point2, point3))
                    boxAFaceIndicies.Add(boxFaceIndex);

            }
            List<UInt32> boxBFaceIndicies = new List<UInt32>();
            foreach (UInt32 boxFaceIndex in boxFacesIndicies)
            {
                Vector3 point1 = allVerticies[allTriangleFaces[Convert.ToInt32(boxFaceIndex)].V1];
                Vector3 point2 = allVerticies[allTriangleFaces[Convert.ToInt32(boxFaceIndex)].V2];
                Vector3 point3 = allVerticies[allTriangleFaces[Convert.ToInt32(boxFaceIndex)].V3];
                if (splitBox.BoxB.DoesIntersectTriangle(point1, point2, point3))
                    boxBFaceIndicies.Add(boxFaceIndex);
            }

            // Create this split node
            Int16 faceCountBoxA = Convert.ToInt16((boxAFaceIndicies.Count == 0) ? -1 : AddNode(splitBox.BoxA, boxAFaceIndicies, allTriangleFaces, allVerticies));
            Int16 faceCountBoxB = Convert.ToInt16((boxBFaceIndicies.Count == 0) ? -1 : AddNode(splitBox.BoxB, boxBFaceIndicies, allTriangleFaces, allVerticies));
            BSPNode newNode = new BSPNode(planeSplitType, faceCountBoxA, faceCountBoxB, 0, 0, splitBox.PlaneDistance);
            Nodes.Add(newNode);
            return Convert.ToUInt16(Nodes.Count - 1);
        }        
    }
}