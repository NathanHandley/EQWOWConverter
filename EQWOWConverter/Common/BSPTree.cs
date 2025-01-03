﻿//  Author: Nathan Handley 2024 (nathanhandley@protonmail.com)
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EQWOWConverter.Common
{
    //internal class BSPTree
    //{
    //    public List<BSPNode> Nodes = new List<BSPNode>();
    //    public List<UInt32> FaceTriangleIndices = new List<UInt32>();

    //    // Generate on create
    //    public BSPTree(List<UInt32> triangleFacesIndices)
    //    {
    //        // Create a root node that is a leaf node with all of the triangles
    //        foreach (UInt32 faceIndex in triangleFacesIndices)
    //            FaceTriangleIndices.Add(faceIndex);
    //        BSPNode rootNode = new BSPNode();
    //        Nodes.Add(rootNode);

    //        // Update leaf value
    //        Nodes[0].SetValues(BSPNodeFlag.Leaf, -1, -1, Convert.ToUInt16(FaceTriangleIndices.Count), 0, 0.0f);
    //    }   
    //}

    internal class BSPTree
    {
        public List<BSPNode> Nodes = new List<BSPNode>();
        public List<UInt32> FaceTriangleIndices = new List<UInt32>();
        private List<int> NodesToProcess = new List<int>();

        // Blank create for WMO groups that don't need one
        public BSPTree()
        {
            BSPNode rootNode = new BSPNode();
            Nodes.Add(rootNode);

            // Update leaf value
            Nodes[0].SetValues(BSPNodeFlag.Leaf, -1, -1, Convert.ToUInt16(FaceTriangleIndices.Count), 0, 0.0f);
        }

        // Generate on create
        public BSPTree(MeshData collisionMeshData)
        {
            // Create a root node
            List<UInt32> faceIndices = new List<UInt32>();
            for (uint i = 0; i < collisionMeshData.TriangleFaces.Count; i++)
                faceIndices.Add(i);
            BoundingBox boundingBox = BoundingBox.GenerateBoxFromVectors(collisionMeshData.Vertices, Configuration.CONFIG_GENERATE_ADDED_BOUNDARY_AMOUNT);
            BSPNode rootNode = new BSPNode(true, boundingBox, faceIndices);
            Nodes.Add(rootNode);
            NodesToProcess.Add(0);

            // Loop through nodes until there are none to process
            while (NodesToProcess.Count > 0)
            {
                List<int> nodesToProcessCopy = new List<int>(NodesToProcess);
                NodesToProcess.Clear();
                foreach (int nodeIndexToProcess in nodesToProcessCopy)
                    ProcessNode(nodeIndexToProcess, collisionMeshData.TriangleFaces, collisionMeshData.Vertices);
            }
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
                        float planeSplitDistance = (box.TopCorner.Z + box.BottomCorner.Z) * 0.5f;
                        splitBox.PlaneDistance = planeSplitDistance;
                        splitBox.BoxA.TopCorner.Z = planeSplitDistance;
                        splitBox.BoxB.BottomCorner.Z = planeSplitDistance;
                    }
                    break;
                case BSPNodeFlag.YZPlane:
                    {
                        float planeSplitDistance = (box.TopCorner.X + box.BottomCorner.X) * 0.5f;
                        splitBox.PlaneDistance = planeSplitDistance;
                        splitBox.BoxA.TopCorner.X = planeSplitDistance;
                        splitBox.BoxB.BottomCorner.X = planeSplitDistance;
                    }
                    break;
                case BSPNodeFlag.XZPlane:
                    {
                        float planeSplitDistance = (box.TopCorner.Y + box.BottomCorner.Y) * 0.5f;
                        splitBox.PlaneDistance = planeSplitDistance;
                        splitBox.BoxA.TopCorner.Y = planeSplitDistance;
                        splitBox.BoxB.BottomCorner.Y = planeSplitDistance;
                    }
                    break;
                default:
                    {
                        Logger.WriteError("BSPTree.GenerateSplitBox Error!  Invalid planeSplitType provided.");
                    }
                    break;
            }
            return splitBox;
        }
        private void ProcessNode(int nodeIndex, List<TriangleFace> allTriangleFaces, List<Vector3> allVertices)
        {
            BoundingBox curNodeBoundingBox = new BoundingBox(Nodes[nodeIndex].TreeGenBoundingBox);
            float xDistance = curNodeBoundingBox.GetXDistance();
            float yDistance = curNodeBoundingBox.GetYDistance();
            float zDistance = curNodeBoundingBox.GetZDistance();
            float totalDistance = xDistance + yDistance + zDistance;
            // If this node already breached the minimim split OR uses a bounding box that is too small, then terminate as a leaf
            if (Nodes[nodeIndex].TreeGenFaceIndices.Count <= Configuration.CONFIG_ZONE_BTREE_MIN_SPLIT_SIZE || totalDistance < Configuration.CONFIG_ZONE_BTREE_MIN_BOX_SIZE_TOTAL)
            {
                // Store the faces on the master list
                UInt32 curFaceStartIndex = Convert.ToUInt32(FaceTriangleIndices.Count);
                foreach (UInt32 faceIndex in Nodes[nodeIndex].TreeGenFaceIndices)
                    FaceTriangleIndices.Add(faceIndex);
                // Update leaf values
                Nodes[nodeIndex].SetValues(BSPNodeFlag.Leaf, -1, -1, Convert.ToUInt16(Nodes[nodeIndex].TreeGenFaceIndices.Count), curFaceStartIndex, 0.0f);
                // No more processing
                Nodes[nodeIndex].ClearTreeGenData();
                return;
            }
            // Calculate plane type and split the box long way
            BSPNodeFlag planeSplitType;
            if (xDistance >= yDistance && (xDistance > zDistance))
                planeSplitType = BSPNodeFlag.YZPlane;
            else if (yDistance >= xDistance && (yDistance > zDistance))
                planeSplitType = BSPNodeFlag.XZPlane;
            else
                planeSplitType = BSPNodeFlag.XYPlane;
            SplitBox splitBox = GenerateSplitBox(curNodeBoundingBox, planeSplitType);
            Nodes[nodeIndex].Flags = planeSplitType;
            Nodes[nodeIndex].PlaneDistance = splitBox.PlaneDistance;
            // Store face Indices that collide with each box, and either update the node half or create appropriate nodes to reflect
            List<UInt32> boxAFaceIndices = new List<UInt32>();
            foreach (UInt32 boxFaceIndex in Nodes[nodeIndex].TreeGenFaceIndices)
            {
                Vector3 point1 = allVertices[allTriangleFaces[Convert.ToInt32(boxFaceIndex)].V1];
                Vector3 point2 = allVertices[allTriangleFaces[Convert.ToInt32(boxFaceIndex)].V2];
                Vector3 point3 = allVertices[allTriangleFaces[Convert.ToInt32(boxFaceIndex)].V3];
                if (splitBox.BoxA.DoesIntersectTriangle(point1, point2, point3))
                    boxAFaceIndices.Add(boxFaceIndex);
            }
            if (boxAFaceIndices.Count == 0)
                Nodes[nodeIndex].ChildANodeIndex = -1;
            else
            {
                BSPNode newChildNode = new BSPNode(true, splitBox.BoxA, boxAFaceIndices);
                Nodes[nodeIndex].ChildANodeIndex = Convert.ToInt16(Nodes.Count);
                NodesToProcess.Add(Nodes.Count);
                Nodes.Add(newChildNode);
            }
            List<UInt32> boxBFaceIndices = new List<UInt32>();
            foreach (UInt32 boxFaceIndex in Nodes[nodeIndex].TreeGenFaceIndices)
            {
                Vector3 point1 = allVertices[allTriangleFaces[Convert.ToInt32(boxFaceIndex)].V1];
                Vector3 point2 = allVertices[allTriangleFaces[Convert.ToInt32(boxFaceIndex)].V2];
                Vector3 point3 = allVertices[allTriangleFaces[Convert.ToInt32(boxFaceIndex)].V3];
                if (splitBox.BoxB.DoesIntersectTriangle(point1, point2, point3))
                    boxBFaceIndices.Add(boxFaceIndex);
            }
            if (boxBFaceIndices.Count == 0)
                Nodes[nodeIndex].ChildBNodeIndex = -1;
            else
            {
                BSPNode newChildNode = new BSPNode(true, splitBox.BoxB, boxBFaceIndices);
                Nodes[nodeIndex].ChildBNodeIndex = Convert.ToInt16(Nodes.Count);
                NodesToProcess.Add(Nodes.Count);
                Nodes.Add(newChildNode);
            }
            // No more processing
            Nodes[nodeIndex].ClearTreeGenData();
            return;
        }
    }
}