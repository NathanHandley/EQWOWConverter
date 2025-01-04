//  Author: Nathan Handley 2024 (nathanhandley@protonmail.com)
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
    internal class BSPTree
    {
        public List<BSPNode> Nodes = new List<BSPNode>();
        public List<UInt32> FaceTriangleIndices = new List<UInt32>();
        private List<BSPNode> NodeProcessQueue = new List<BSPNode>();

        // Blank create for WMO groups that don't need one
        public BSPTree()
        {
            BSPNode rootNode = new BSPNode(0, 0);
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
            BSPNode rootNode = new BSPNode(0, boundingBox, faceIndices, 0);
            Nodes.Add(rootNode);
            NodeProcessQueue.Add(rootNode);

            // Loop through nodes until there are none to process (and can't have more than Int16.Max-2 nodes)
            int maxNodeCount = 32760;
            if (Configuration.CONFIG_GENERATE_ZONE_COLLISION_QUICK_FOR_DEBUG == true)
                maxNodeCount = 16;
            while (NodeProcessQueue.Count > 0 && Nodes.Count < maxNodeCount)
            {
                // Pop the last and process it
                NodeProcessQueue.Sort();
                BSPNode curNode = NodeProcessQueue[NodeProcessQueue.Count - 1];
                NodeProcessQueue.RemoveAt(NodeProcessQueue.Count - 1);
                ProcessNode(curNode.NodeID, collisionMeshData.TriangleFaces, collisionMeshData.Vertices);
            }

            // Set all remaining nodes as leaf nodes
            foreach(BSPNode node in NodeProcessQueue)
            {
                UInt32 curFaceStartIndex = Convert.ToUInt32(FaceTriangleIndices.Count);
                foreach (UInt32 faceIndex in node.TreeGenFaceIndices)
                    FaceTriangleIndices.Add(faceIndex);
                node.SetValues(BSPNodeFlag.Leaf, -1, -1, Convert.ToUInt16(node.TreeGenFaceIndices.Count), curFaceStartIndex, 0.0f);
            }
        }

        private void ProcessNode(int nodeIndex, List<TriangleFace> allTriangleFaces, List<Vector3> allVertices)
        {
            BSPNode curNode = Nodes[nodeIndex];
            BoundingBox curNodeBoundingBox = new BoundingBox(curNode.TreeGenBoundingBox);
            float xDistance = curNodeBoundingBox.GetXDistance();
            float yDistance = curNodeBoundingBox.GetYDistance();
            float zDistance = curNodeBoundingBox.GetZDistance();
            float totalDistance = xDistance + yDistance + zDistance;
            curNode.TotalDistance = totalDistance;

            // If this node already breached the minimim split OR uses a bounding box that is too small, then terminate as a leaf
            if (curNode.TreeGenFaceIndices.Count <= Configuration.CONFIG_ZONE_BTREE_MIN_SPLIT_SIZE
                || totalDistance < Configuration.CONFIG_ZONE_BTREE_MIN_BOX_SIZE_TOTAL
                || curNode.Depth >= Configuration.CONFIG_ZONE_BTREE_MAX_NODE_GEN_DEPTH)
            {
                // Store the faces on the master list
                UInt32 curFaceStartIndex = Convert.ToUInt32(FaceTriangleIndices.Count);
                foreach (UInt32 faceIndex in curNode.TreeGenFaceIndices)
                    FaceTriangleIndices.Add(faceIndex);

                // Close out the leaf
                curNode.SetValues(BSPNodeFlag.Leaf, -1, -1, Convert.ToUInt16(curNode.TreeGenFaceIndices.Count), curFaceStartIndex, 0.0f);
                curNode.ClearTreeGenData();
                return;
            }

            // Calculate plane type and split the box long way
            SplitBox splitBox = SplitBox.GenerateXYZSplitBox(curNodeBoundingBox);
            if (splitBox.SplitAxis == AxisType.XAxis)
                curNode.Flags = BSPNodeFlag.YZPlane;
            else if (splitBox.SplitAxis == AxisType.YAxis)
                curNode.Flags = BSPNodeFlag.XZPlane;
            else
                curNode.Flags = BSPNodeFlag.XYPlane;
            curNode.PlaneDistance = splitBox.PlaneDistance;

            // Store face Indices that collide with each box, and either update the node half or create appropriate nodes to reflect
            List<UInt32> boxAFaceIndices = new List<UInt32>();
            foreach (UInt32 boxFaceIndex in curNode.TreeGenFaceIndices)
            {
                Vector3 point1 = allVertices[allTriangleFaces[Convert.ToInt32(boxFaceIndex)].V1];
                Vector3 point2 = allVertices[allTriangleFaces[Convert.ToInt32(boxFaceIndex)].V2];
                Vector3 point3 = allVertices[allTriangleFaces[Convert.ToInt32(boxFaceIndex)].V3];
                if (splitBox.BoxA.DoesIntersectTriangle(point1, point2, point3))
                    boxAFaceIndices.Add(boxFaceIndex);
            }
            if (boxAFaceIndices.Count == 0)
                curNode.ChildANodeIndex = -1;
            else
            {
                BSPNode newChildNode = new BSPNode(Nodes.Count, splitBox.BoxA, boxAFaceIndices, curNode.Depth + 1);
                curNode.ChildANodeIndex = Convert.ToInt16(Nodes.Count);
                NodeProcessQueue.Add(newChildNode);
                Nodes.Add(newChildNode);
            }
            List<UInt32> boxBFaceIndices = new List<UInt32>();
            foreach (UInt32 boxFaceIndex in curNode.TreeGenFaceIndices)
            {
                Vector3 point1 = allVertices[allTriangleFaces[Convert.ToInt32(boxFaceIndex)].V1];
                Vector3 point2 = allVertices[allTriangleFaces[Convert.ToInt32(boxFaceIndex)].V2];
                Vector3 point3 = allVertices[allTriangleFaces[Convert.ToInt32(boxFaceIndex)].V3];
                if (splitBox.BoxB.DoesIntersectTriangle(point1, point2, point3))
                    boxBFaceIndices.Add(boxFaceIndex);
            }
            if (boxBFaceIndices.Count == 0)
                curNode.ChildBNodeIndex = -1;
            else
            {
                BSPNode newChildNode = new BSPNode(Nodes.Count, splitBox.BoxB, boxBFaceIndices, curNode.Depth + 1);
                curNode.ChildBNodeIndex = Convert.ToInt16(Nodes.Count);
                NodeProcessQueue.Add(newChildNode);
                Nodes.Add(newChildNode);                
            }

            // No more processing
            curNode.ClearTreeGenData();
            return;
        }
    }
}