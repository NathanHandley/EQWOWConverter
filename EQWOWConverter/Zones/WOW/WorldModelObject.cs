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

using EQWOWConverter.Common;
using EQWOWConverter.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Zones
{
    internal class WorldModelObject
    {
        private static UInt32 CURRENT_WMOGROUPID = Configuration.CONFIG_DBCID_WMOGROUPID_START;

        public UInt32 WMOGroupID;
        public List<Vector3> Verticies = new List<Vector3>();
        public List<TextureUv> TextureCoords = new List<TextureUv>();
        public List<Vector3> Normals = new List<Vector3>();
        public List<ColorRGBA> VertexColors = new List<ColorRGBA>();
        public List<TriangleFace> TriangleFaces = new List<TriangleFace>();
        public List<WorldModelRenderBatch> RenderBatches = new List<WorldModelRenderBatch>();
        public BoundingBox BoundingBox = new BoundingBox();
        public BSPTree BSPTree;

        public WorldModelObject(List<Vector3> verticies, List<TextureUv> textureCoords, List<Vector3> normals, List<ColorRGBA> vertexColors, 
            List<TriangleFace> triangleFaces, List<Material> materials)
        {
            Verticies = verticies;
            TextureCoords = textureCoords;
            Normals = normals;
            VertexColors = vertexColors;
            TriangleFaces = triangleFaces;
            CalculateBoundingBox();
            GenerateRenderBatches(materials);
            WMOGroupID = CURRENT_WMOGROUPID;
            CURRENT_WMOGROUPID++;
            BSPTree = new BSPTree(BoundingBox, Verticies, TriangleFaces);
        }

        private void GenerateRenderBatches(List<Material> materials)
        {
            // Reorder the faces and related objects
            SortRenderObjects();

            // Build a render batch per material
            Dictionary<int, WorldModelRenderBatch> renderBatchesByMaterialID = new Dictionary<int, WorldModelRenderBatch>();
            for (int i = 0; i < TriangleFaces.Count; i++)
            {
                // Work off material index
                int curMaterialIndex = TriangleFaces[i].MaterialIndex;

                // Skip materials that shouldn't be rendered
                // TODO: Handle this better for invisible collision
                if (materials[curMaterialIndex].MaterialType == MaterialType.Invisible)
                    continue;
                if (materials[curMaterialIndex].AnimationTextures.Count == 0)
                    continue;

                // Create a new one if this is the first instance of the material
                if (renderBatchesByMaterialID.ContainsKey(curMaterialIndex) == false)
                {
                    renderBatchesByMaterialID.Add(curMaterialIndex, new WorldModelRenderBatch());
                    renderBatchesByMaterialID[curMaterialIndex].MaterialIndex = Convert.ToByte(curMaterialIndex);
                    renderBatchesByMaterialID[curMaterialIndex].FirstTriangleFaceIndex = Convert.ToUInt32(i * 3);
                    renderBatchesByMaterialID[curMaterialIndex].NumOfFaceIndicies = 3;
                    renderBatchesByMaterialID[curMaterialIndex].FirstVertexIndex = TriangleFaces[i].GetSmallestIndex();
                    renderBatchesByMaterialID[curMaterialIndex].LastVertexIndex = TriangleFaces[i].GetLargestIndex();
                }
                // Otherwise add to an existing
                else
                {
                    renderBatchesByMaterialID[curMaterialIndex].NumOfFaceIndicies += 3;
                    int curFaceMinIndex = TriangleFaces[i].GetSmallestIndex();
                    if (curFaceMinIndex < renderBatchesByMaterialID[curMaterialIndex].FirstVertexIndex)
                        renderBatchesByMaterialID[curMaterialIndex].FirstVertexIndex = Convert.ToUInt16(curFaceMinIndex);
                    int curFaceMaxIndex = TriangleFaces[i].GetLargestIndex();
                    if (curFaceMaxIndex > renderBatchesByMaterialID[curMaterialIndex].LastVertexIndex)
                        renderBatchesByMaterialID[curMaterialIndex].LastVertexIndex = Convert.ToUInt16(curFaceMaxIndex);
                }
            }

            // Store the new render batches
            RenderBatches.Clear();
            foreach (var renderBatch in renderBatchesByMaterialID)
            {
                renderBatch.Value.BoundingBox = new BoundingBox(BoundingBox);
                RenderBatches.Add(renderBatch.Value);
            }
        }

        private void SortRenderObjects()
        {
            TriangleFaces.Sort();

            // Reorder the verticies / texcoords / normals / vertcolors to match the sorted triangle faces
            List<Vector3> sortedVerticies = new List<Vector3>();
            List<TextureUv> sortedTextureCoords = new List<TextureUv>();
            List<Vector3> sortedNormals = new List<Vector3>();
            List<ColorRGBA> sortedVertexColors = new List<ColorRGBA>();
            List<TriangleFace> sortedTriangleFaces = new List<TriangleFace>();
            Dictionary<int, int> oldNewVertexIndicies = new Dictionary<int, int>();
            for (int i = 0; i < TriangleFaces.Count; i++)
            {
                TriangleFace curTriangleFace = TriangleFaces[i];

                // Face vertex 1
                if (oldNewVertexIndicies.ContainsKey(curTriangleFace.V1))
                {
                    // This index was aready remapped
                    curTriangleFace.V1 = oldNewVertexIndicies[curTriangleFace.V1];
                }
                else
                {
                    // Store new mapping
                    int oldVertIndex = curTriangleFace.V1;
                    int newVertIndex = sortedVerticies.Count;
                    oldNewVertexIndicies.Add(oldVertIndex, newVertIndex);
                    curTriangleFace.V1 = newVertIndex;

                    // Add objects
                    sortedVerticies.Add(Verticies[oldVertIndex]);
                    sortedTextureCoords.Add(TextureCoords[oldVertIndex]);
                    sortedNormals.Add(Normals[oldVertIndex]);
                    if (VertexColors.Count != 0)
                        sortedVertexColors.Add(VertexColors[oldVertIndex]);
                }

                // Face vertex 2
                if (oldNewVertexIndicies.ContainsKey(curTriangleFace.V2))
                {
                    // This index was aready remapped
                    curTriangleFace.V2 = oldNewVertexIndicies[curTriangleFace.V2];
                }
                else
                {
                    // Store new mapping
                    int oldVertIndex = curTriangleFace.V2;
                    int newVertIndex = sortedVerticies.Count;
                    oldNewVertexIndicies.Add(oldVertIndex, newVertIndex);
                    curTriangleFace.V2 = newVertIndex;

                    // Add objects
                    sortedVerticies.Add(Verticies[oldVertIndex]);
                    sortedTextureCoords.Add(TextureCoords[oldVertIndex]);
                    sortedNormals.Add(Normals[oldVertIndex]);
                    if (VertexColors.Count != 0)
                        sortedVertexColors.Add(VertexColors[oldVertIndex]);
                }

                // Face vertex 3
                if (oldNewVertexIndicies.ContainsKey(curTriangleFace.V3))
                {
                    // This index was aready remapped
                    curTriangleFace.V3 = oldNewVertexIndicies[curTriangleFace.V3];
                }
                else
                {
                    // Store new mapping
                    int oldVertIndex = curTriangleFace.V3;
                    int newVertIndex = sortedVerticies.Count;
                    oldNewVertexIndicies.Add(oldVertIndex, newVertIndex);
                    curTriangleFace.V3 = newVertIndex;

                    // Add objects
                    sortedVerticies.Add(Verticies[oldVertIndex]);
                    sortedTextureCoords.Add(TextureCoords[oldVertIndex]);
                    sortedNormals.Add(Normals[oldVertIndex]);
                    if (VertexColors.Count != 0)
                        sortedVertexColors.Add(VertexColors[oldVertIndex]);
                }

                // Save this updated triangle
                sortedTriangleFaces.Add(curTriangleFace);
            }

            // Save the sorted values
            Verticies = sortedVerticies;
            TextureCoords = sortedTextureCoords;
            Normals = sortedNormals;
            VertexColors = sortedVertexColors;
            TriangleFaces = sortedTriangleFaces;
        }

        private void CalculateBoundingBox()
        {
            BoundingBox = new BoundingBox();
            foreach (Vector3 renderVert in Verticies)
            {
                if (renderVert.X < BoundingBox.BottomCorner.X)
                    BoundingBox.BottomCorner.X = renderVert.X;
                if (renderVert.Y < BoundingBox.BottomCorner.Y)
                    BoundingBox.BottomCorner.Y = renderVert.Y;
                if (renderVert.Z < BoundingBox.BottomCorner.Z)
                    BoundingBox.BottomCorner.Z = renderVert.Z;

                if (renderVert.X > BoundingBox.TopCorner.X)
                    BoundingBox.TopCorner.X = renderVert.X;
                if (renderVert.Y > BoundingBox.TopCorner.Y)
                    BoundingBox.TopCorner.Y = renderVert.Y;
                if (renderVert.Z > BoundingBox.TopCorner.Z)
                    BoundingBox.TopCorner.Z = renderVert.Z;
            }
        }
    }
}
