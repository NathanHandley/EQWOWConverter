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
    internal class MeshData
    {
        public List<Vector3> Vertices = new List<Vector3>();
        public List<Vector3> Normals = new List<Vector3>();
        public List<TextureCoordinates> TextureCoordinates = new List<TextureCoordinates>();
        public List<TriangleFace> TriangleFaces = new List<TriangleFace>();
        public List<ColorRGBA> VertexColors = new List<ColorRGBA>();

        public MeshData() { }

        public MeshData(MeshData meshData)
        {
            foreach (Vector3 vertex in meshData.Vertices)
                Vertices.Add(new Vector3(vertex));
            foreach(Vector3 normal in meshData.Normals)
                Normals.Add(new Vector3(normal));
            foreach(TextureCoordinates textureCoordinate in meshData.TextureCoordinates)
                TextureCoordinates.Add(new TextureCoordinates(textureCoordinate));
            foreach(TriangleFace triangleFace in meshData.TriangleFaces)
                TriangleFaces.Add(new TriangleFace(triangleFace));
            foreach (ColorRGBA colorRGBA in meshData.VertexColors)
                VertexColors.Add(new ColorRGBA(colorRGBA));
        }

        public void ApplyEQToWoWGeometryTranslationsAndWorldScale()
        {
            // Change face indices for winding over differences
            foreach (TriangleFace triangleFace in TriangleFaces)
            {
                int swapFaceVertexIndex = triangleFace.V3;
                triangleFace.V3 = triangleFace.V1;
                triangleFace.V1 = swapFaceVertexIndex;
            }
            // Perform vertex world scaling and 180 Z-Axis degree rotation
            foreach (Vector3 vertex in Vertices)
            {
                vertex.X *= -1 * Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
                vertex.Y *= -1 * Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
                vertex.Z *= Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            }
            // Flip Y on the texture coordinates
            foreach (TextureCoordinates textureCoordinate in TextureCoordinates)
            {
                textureCoordinate.Y *= -1;
            }
        }

        public MeshData GetMeshDataForMaterial(Material material)
        {
            // Extract out copies of the geometry data specific to this material
            MeshData extractedMeshData = new MeshData();
            Dictionary<int, int> oldNewVertexIndices = new Dictionary<int, int>();
            for (int i = 0; i < TriangleFaces.Count; i++)
            {
                // Skip faces not matching the material
                if (TriangleFaces[i].MaterialIndex != material.Index)
                    continue;

                // Make the new face, and keep the material ID
                TriangleFace curTriangleFace = new TriangleFace(TriangleFaces[i]);
                curTriangleFace.MaterialIndex = Convert.ToInt32(material.Index);

                // Face vertex 1
                if (oldNewVertexIndices.ContainsKey(curTriangleFace.V1))
                {
                    // This index was aready remapped
                    curTriangleFace.V1 = oldNewVertexIndices[curTriangleFace.V1];
                }
                else
                {
                    // Store new mapping
                    int oldVertIndex = curTriangleFace.V1;
                    int newVertIndex = extractedMeshData.Vertices.Count;
                    oldNewVertexIndices.Add(oldVertIndex, newVertIndex);
                    curTriangleFace.V1 = newVertIndex;

                    // Add objects
                    extractedMeshData.Vertices.Add(Vertices[oldVertIndex]);
                    extractedMeshData.TextureCoordinates.Add(TextureCoordinates[oldVertIndex]);
                    extractedMeshData.Normals.Add(Normals[oldVertIndex]);
                    if (VertexColors.Count != 0)
                        extractedMeshData.VertexColors.Add(VertexColors[oldVertIndex]);
                }

                // Face vertex 2
                if (oldNewVertexIndices.ContainsKey(curTriangleFace.V2))
                {
                    // This index was aready remapped
                    curTriangleFace.V2 = oldNewVertexIndices[curTriangleFace.V2];
                }
                else
                {
                    // Store new mapping
                    int oldVertIndex = curTriangleFace.V2;
                    int newVertIndex = extractedMeshData.Vertices.Count;
                    oldNewVertexIndices.Add(oldVertIndex, newVertIndex);
                    curTriangleFace.V2 = newVertIndex;

                    // Add objects
                    extractedMeshData.Vertices.Add(Vertices[oldVertIndex]);
                    extractedMeshData.TextureCoordinates.Add(TextureCoordinates[oldVertIndex]);
                    extractedMeshData.Normals.Add(Normals[oldVertIndex]);
                    if (VertexColors.Count != 0)
                        extractedMeshData.VertexColors.Add(VertexColors[oldVertIndex]);
                }

                // Face vertex 3
                if (oldNewVertexIndices.ContainsKey(curTriangleFace.V3))
                {
                    // This index was aready remapped
                    curTriangleFace.V3 = oldNewVertexIndices[curTriangleFace.V3];
                }
                else
                {
                    // Store new mapping
                    int oldVertIndex = curTriangleFace.V3;
                    int newVertIndex = extractedMeshData.Vertices.Count;
                    oldNewVertexIndices.Add(oldVertIndex, newVertIndex);
                    curTriangleFace.V3 = newVertIndex;

                    // Add objects
                    extractedMeshData.Vertices.Add(Vertices[oldVertIndex]);
                    extractedMeshData.TextureCoordinates.Add(TextureCoordinates[oldVertIndex]);
                    extractedMeshData.Normals.Add(Normals[oldVertIndex]);
                    if (VertexColors.Count != 0)
                        extractedMeshData.VertexColors.Add(VertexColors[oldVertIndex]);
                }

                // Save this updated triangle
                extractedMeshData.TriangleFaces.Add(curTriangleFace);
            }
            return extractedMeshData;
        }

        public MeshData GetMeshDataForFaces(List<TriangleFace> faces)
        {
            // Since the face list is likely to not include all faces, rebuild the render object lists
            MeshData extractedMeshData = new MeshData();
            Dictionary<int, int> oldNewVertexIndices = new Dictionary<int, int>();
            for (int i = 0; i < faces.Count; i++)
            {
                TriangleFace curTriangleFace = faces[i];

                // Face vertex 1
                if (oldNewVertexIndices.ContainsKey(curTriangleFace.V1))
                {
                    // This index was aready remapped
                    curTriangleFace.V1 = oldNewVertexIndices[curTriangleFace.V1];
                }
                else
                {
                    // Store new mapping
                    int oldVertIndex = curTriangleFace.V1;
                    int newVertIndex = extractedMeshData.Vertices.Count;
                    oldNewVertexIndices.Add(oldVertIndex, newVertIndex);
                    curTriangleFace.V1 = newVertIndex;

                    // Add objects
                    extractedMeshData.Vertices.Add(Vertices[oldVertIndex]);
                    extractedMeshData.TextureCoordinates.Add(TextureCoordinates[oldVertIndex]);
                    extractedMeshData.Normals.Add(Normals[oldVertIndex]);
                    if (VertexColors.Count != 0)
                        extractedMeshData.VertexColors.Add(VertexColors[oldVertIndex]);
                }

                // Face vertex 2
                if (oldNewVertexIndices.ContainsKey(curTriangleFace.V2))
                {
                    // This index was aready remapped
                    curTriangleFace.V2 = oldNewVertexIndices[curTriangleFace.V2];
                }
                else
                {
                    // Store new mapping
                    int oldVertIndex = curTriangleFace.V2;
                    int newVertIndex = extractedMeshData.Vertices.Count;
                    oldNewVertexIndices.Add(oldVertIndex, newVertIndex);
                    curTriangleFace.V2 = newVertIndex;

                    // Add objects
                    extractedMeshData.Vertices.Add(Vertices[oldVertIndex]);
                    extractedMeshData.TextureCoordinates.Add(TextureCoordinates[oldVertIndex]);
                    extractedMeshData.Normals.Add(Normals[oldVertIndex]);
                    if (VertexColors.Count != 0)
                        extractedMeshData.VertexColors.Add(VertexColors[oldVertIndex]);
                }

                // Face vertex 3
                if (oldNewVertexIndices.ContainsKey(curTriangleFace.V3))
                {
                    // This index was aready remapped
                    curTriangleFace.V3 = oldNewVertexIndices[curTriangleFace.V3];
                }
                else
                {
                    // Store new mapping
                    int oldVertIndex = curTriangleFace.V3;
                    int newVertIndex = extractedMeshData.Vertices.Count;
                    oldNewVertexIndices.Add(oldVertIndex, newVertIndex);
                    curTriangleFace.V3 = newVertIndex;

                    // Add objects
                    extractedMeshData.Vertices.Add(Vertices[oldVertIndex]);
                    extractedMeshData.TextureCoordinates.Add(TextureCoordinates[oldVertIndex]);
                    extractedMeshData.Normals.Add(Normals[oldVertIndex]);
                    if (VertexColors.Count != 0)
                        extractedMeshData.VertexColors.Add(VertexColors[oldVertIndex]);
                }

                // Save this updated triangle
                extractedMeshData.TriangleFaces.Add(curTriangleFace);
            }
            return extractedMeshData;
        }

        public void SortDataByMaterial()
        {
            // Sort triangles first
            TriangleFaces.Sort();

            // Reorder the vertices / texcoords / normals / to match the sorted triangle faces
            List<Vector3> sortedVertices = new List<Vector3>();
            List<Vector3> sortedNormals = new List<Vector3>();
            List<TextureCoordinates> sortedTextureCoordinates = new List<TextureCoordinates>();
            List<TriangleFace> sortedTriangleFaces = new List<TriangleFace>();
            List<ColorRGBA> sortedVertexColors = new List<ColorRGBA>();
            Dictionary<int, int> oldNewVertexIndices = new Dictionary<int, int>();
            for (int i = 0; i < TriangleFaces.Count; i++)
            {
                TriangleFace curTriangleFace = TriangleFaces[i];

                // Face vertex 1
                if (oldNewVertexIndices.ContainsKey(curTriangleFace.V1))
                {
                    // This index was aready remapped
                    curTriangleFace.V1 = oldNewVertexIndices[curTriangleFace.V1];
                }
                else
                {
                    // Store new mapping
                    int oldVertIndex = curTriangleFace.V1;
                    int newVertIndex = sortedVertices.Count;
                    oldNewVertexIndices.Add(oldVertIndex, newVertIndex);
                    curTriangleFace.V1 = newVertIndex;

                    // Add vertices
                    sortedVertices.Add(Vertices[oldVertIndex]);
                    sortedNormals.Add(Normals[oldVertIndex]);
                    sortedTextureCoordinates.Add(TextureCoordinates[oldVertIndex]);
                    if (VertexColors.Count != 0)
                        sortedVertexColors.Add(VertexColors[oldVertIndex]);
                }

                // Face vertex 2
                if (oldNewVertexIndices.ContainsKey(curTriangleFace.V2))
                {
                    // This index was aready remapped
                    curTriangleFace.V2 = oldNewVertexIndices[curTriangleFace.V2];
                }
                else
                {
                    // Store new mapping
                    int oldVertIndex = curTriangleFace.V2;
                    int newVertIndex = sortedVertices.Count;
                    oldNewVertexIndices.Add(oldVertIndex, newVertIndex);
                    curTriangleFace.V2 = newVertIndex;

                    // Add vertices
                    sortedVertices.Add(Vertices[oldVertIndex]);
                    sortedNormals.Add(Normals[oldVertIndex]);
                    sortedTextureCoordinates.Add(TextureCoordinates[oldVertIndex]);
                    if (VertexColors.Count != 0)
                        sortedVertexColors.Add(VertexColors[oldVertIndex]);
                }

                // Face vertex 3
                if (oldNewVertexIndices.ContainsKey(curTriangleFace.V3))
                {
                    // This index was aready remapped
                    curTriangleFace.V3 = oldNewVertexIndices[curTriangleFace.V3];
                }
                else
                {
                    // Store new mapping
                    int oldVertIndex = curTriangleFace.V3;
                    int newVertIndex = sortedVertices.Count;
                    oldNewVertexIndices.Add(oldVertIndex, newVertIndex);
                    curTriangleFace.V3 = newVertIndex;

                    // Add vertices
                    sortedVertices.Add(Vertices[oldVertIndex]);
                    sortedNormals.Add(Normals[oldVertIndex]);
                    sortedTextureCoordinates.Add(TextureCoordinates[oldVertIndex]);
                    if (VertexColors.Count != 0)
                        sortedVertexColors.Add(VertexColors[oldVertIndex]);
                }

                // Save this updated triangle
                sortedTriangleFaces.Add(curTriangleFace);
            }

            // Save the sorted values
            TriangleFaces = sortedTriangleFaces;
            Vertices = sortedVertices;
            Normals = sortedNormals;
            TextureCoordinates = sortedTextureCoordinates;
            VertexColors = sortedVertexColors;
        }

        public void SplitIntoChunks(MeshData allMeshData, BoundingBox curBoundingBox, List<TriangleFace> curTriangleFaces, Material curMaterial, ref List<Vector3> chunkPositions, ref List<MeshData> chunkMeshDatas)
        {
            // Calculate the true number of triangles that will be made
            int finalTriangleCount = curTriangleFaces.Count * curMaterial.NumOfAnimationFrames();

            // If the box is too big, cut it up
            if (curBoundingBox.FurthestPointDistanceFromCenterXOnly() >= Configuration.CONFIG_EQTOWOW_ZONE_MATERIAL_TO_OBJECT_SPLIT_MIN_XY_CENTER_TO_EDGE_DISTANCE
                || curBoundingBox.FurthestPointDistanceFromCenterYOnly() >= Configuration.CONFIG_EQTOWOW_ZONE_MATERIAL_TO_OBJECT_SPLIT_MIN_XY_CENTER_TO_EDGE_DISTANCE
                || finalTriangleCount >= Configuration.CONFIG_EQTOWOW_ZONE_MATERIAL_TO_OBJECT_SPLIT_MAX_FACE_TRIANGLE_COUNT)
            {
                // Create two new bounding boxes based on the longest edge
                SplitBox splitBox = SplitBox.GenerateXYSplitBoxFromBoundingBox(curBoundingBox);

                // Calculate what triangles fit into these boxes
                List<TriangleFace> aBoxTriangles = new List<TriangleFace>();
                List<TriangleFace> bBoxTriangles = new List<TriangleFace>();

                foreach (TriangleFace triangle in curTriangleFaces)
                {
                    // Get center point
                    Vector3 v1 = allMeshData.Vertices[triangle.V1];
                    Vector3 v2 = allMeshData.Vertices[triangle.V2];
                    Vector3 v3 = allMeshData.Vertices[triangle.V3];
                    Vector3 center = new Vector3((v1.X + v2.X + v3.X) / 3, (v1.Y + v2.Y + v3.Y) / 3, (v1.Z + v2.Z + v3.Z) / 3);

                    // Align to the first box if it is inside it (only based on xy), otherwise put in the other box
                    // and don't do if/else since there is intentional overlap
                    if (center.X >= splitBox.BoxA.BottomCorner.X && center.X <= splitBox.BoxA.TopCorner.X &&
                        center.Y >= splitBox.BoxA.BottomCorner.Y && center.Y <= splitBox.BoxA.TopCorner.Y)
                    {
                        aBoxTriangles.Add(new TriangleFace(triangle));
                    }
                    if (center.X >= splitBox.BoxB.BottomCorner.X && center.X <= splitBox.BoxB.TopCorner.X &&
                        center.Y >= splitBox.BoxB.BottomCorner.Y && center.Y <= splitBox.BoxB.TopCorner.Y)
                    {
                        bBoxTriangles.Add(new TriangleFace(triangle));
                    }
                }

                // Generate for the two sub boxes
                SplitIntoChunks(allMeshData, splitBox.BoxA, aBoxTriangles, curMaterial, ref chunkPositions, ref chunkMeshDatas);
                SplitIntoChunks(allMeshData, splitBox.BoxB, bBoxTriangles, curMaterial, ref chunkPositions, ref chunkMeshDatas);
            }
            else if (curTriangleFaces.Count > 0)
            {
                // If no more splits, create a complete chunk mesh
                MeshData newMeshData = new MeshData(allMeshData.GetMeshDataForFaces(curTriangleFaces));

                // Use the bounding box as the center position
                Vector3 newMeshPosition = new Vector3(curBoundingBox.GetCenter());
                chunkPositions.Add(newMeshPosition);

                // Subtract the center from the position data so it aligns propertly
                foreach (Vector3 vertex in newMeshData.Vertices)
                {
                    vertex.X -= newMeshPosition.X;
                    vertex.Y -= newMeshPosition.Y;
                    vertex.Z -= newMeshPosition.Z;
                }

                // Save the mesh
                chunkMeshDatas.Add(newMeshData);
            }
        }
    }
}
