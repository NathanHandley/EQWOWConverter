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

        // Generate on create
        public BSPTree(List<UInt32> triangleFacesIndices)
        {
            // Create a root node that is a leaf node with all of the triangles
            foreach (UInt32 faceIndex in triangleFacesIndices)
                FaceTriangleIndices.Add(faceIndex);
            BSPNode rootNode = new BSPNode();
            Nodes.Add(rootNode);

            // Update leaf value
            Nodes[0].SetValues(BSPNodeFlag.Leaf, -1, -1, Convert.ToUInt16(FaceTriangleIndices.Count), 0, 0.0f);
        }   
    }
}