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

using EQWOWConverter.Zones;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public void DeleteInvalidTriangles()
        {
            List<TriangleFace> keepTriangleFaces = new List<TriangleFace>();
            foreach (TriangleFace triangle in TriangleFaces)
            {
                // Remove triangles that don't have 3 unique indices
                if (triangle.V1 == triangle.V2 || triangle.V2 == triangle.V3 || triangle.V1 == triangle.V3)
                    continue;

                // Remove triangles that have indices beyond the vertex list
                if (triangle.V1 >= Vertices.Count)
                    continue;
                if (triangle.V2 >= Vertices.Count)
                    continue;
                if (triangle.V3 >= Vertices.Count)
                    continue;

                // This is a good one, keep it
                keepTriangleFaces.Add(triangle);
            }

            // Only rebuild if things are different, and there's at least one
            if (keepTriangleFaces.Count != TriangleFaces.Count)
            {
                Logger.WriteDetail("DeleteInvalidTriangles found a mesh with invalid triangles, changing count from '" + TriangleFaces.Count + "' to '" + keepTriangleFaces.Count + "'");
                MeshData newMeshData = GetMeshDataForFaces(keepTriangleFaces);
                newMeshData.CondenseAndRenumberVertexIndices();
                Vertices = newMeshData.Vertices;
                Normals = newMeshData.Normals;
                TextureCoordinates = newMeshData.TextureCoordinates;
                TriangleFaces = newMeshData.TriangleFaces;
                VertexColors = newMeshData.VertexColors;
            }
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

        public void ApplyEQToWoWVertexColor(ZoneProperties zoneProperties)
        {
            double intensityAmount = Configuration.CONFIG_LIGHT_DEFAULT_VERTEX_COLOR_INTENSITY;
            if (zoneProperties.VertexColorIntensityOverride >= 0)
                intensityAmount = zoneProperties.VertexColorIntensityOverride;

            // Vertex colors are reduced for external areas due to the natural zone light
            foreach (ColorRGBA vertexColor in VertexColors)
            {
                vertexColor.R = Convert.ToByte(Convert.ToDouble(vertexColor.R) * intensityAmount);
                vertexColor.G = Convert.ToByte(Convert.ToDouble(vertexColor.G) * intensityAmount);
                vertexColor.B = Convert.ToByte(Convert.ToDouble(vertexColor.B) * intensityAmount);
                //vertexColor.A = vertexColor.A;
            }
        }

        public MeshData GetMeshDataForMaterials(params Material[] materials)
        {
            // Extract out copies of the geometry data specific to these materials
            MeshData extractedMeshData = new MeshData();
            Dictionary<int, int> oldNewVertexIndices = new Dictionary<int, int>();
            HashSet<int> materialIDs = new HashSet<int>();
            foreach (Material material in materials)
                materialIDs.Add(Convert.ToInt32(material.Index));
            for (int i = 0; i < TriangleFaces.Count; i++)
            {
                // Skip faces not matching a material in the set
                if (materialIDs.Contains(TriangleFaces[i].MaterialIndex) == false)
                    continue;
                int curMaterialID = TriangleFaces[i].MaterialIndex;

                // Make the new face, and keep the material ID
                TriangleFace curTriangleFace = new TriangleFace(TriangleFaces[i]);
                curTriangleFace.MaterialIndex = Convert.ToInt32(curMaterialID);

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

        public MeshData GetMeshDataExcludingNonRenderedAndAnimatedMaterials(params Material[] allMaterials)
        {
            // Extract out copies of the geometry data specific to data that would be considered zone static and rendered materials
            List<Material> includedMaterials = new List<Material>();
            foreach (Material material in allMaterials)
            {
                if (material.IsAnimated())
                    continue;
                if (material.HasTransparency())
                    continue;
                if (material.IsRenderable() == false)
                    continue;
                includedMaterials.Add(material);
            }
            if (includedMaterials.Count == 0)
                return new MeshData();
            else
                return GetMeshDataForMaterials(includedMaterials.ToArray());
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
                    if (TextureCoordinates.Count != 0)
                        extractedMeshData.TextureCoordinates.Add(TextureCoordinates[oldVertIndex]);
                    if (Normals.Count != 0)
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
                    if (TextureCoordinates.Count != 0)
                        extractedMeshData.TextureCoordinates.Add(TextureCoordinates[oldVertIndex]);
                    if (Normals.Count != 0)
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
                    if (TextureCoordinates.Count != 0)
                        extractedMeshData.TextureCoordinates.Add(TextureCoordinates[oldVertIndex]);
                    if (Normals.Count != 0)
                        extractedMeshData.Normals.Add(Normals[oldVertIndex]);
                    if (VertexColors.Count != 0)
                        extractedMeshData.VertexColors.Add(VertexColors[oldVertIndex]);
                }

                // Save this updated triangle
                extractedMeshData.TriangleFaces.Add(curTriangleFace);
            }
            return extractedMeshData;
        }

        public void CondenseAndRenumberVertexIndices()
        {
            // Reorder the vertices / texcoords / normals / to match the sorted triangle faces
            List<Vector3> sortedVertices = new List<Vector3>();
            List<TriangleFace> sortedTriangleFaces = new List<TriangleFace>();
            List<Vector3> sortedNormals = new List<Vector3>();
            List<TextureCoordinates> sortedTextureCoordinates = new List<TextureCoordinates>();
            List<ColorRGBA> sortedVertexColors = new List<ColorRGBA>();
            Dictionary<int, int> oldNewVertexIndices = new Dictionary<int, int>();
            for (int i = 0; i < TriangleFaces.Count; i++)
            {
                TriangleFace curTriangleFace = new TriangleFace(TriangleFaces[i]);

                // Delete any that use the same index three times
                if (curTriangleFace.V1 == curTriangleFace.V2 && curTriangleFace.V1 == curTriangleFace.V3)
                    continue;

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
                    sortedVertices.Add(Vertices[oldVertIndex]);
                    if (Normals.Count != 0)
                        sortedNormals.Add(Normals[newVertIndex]);
                    if (TextureCoordinates.Count != 0)
                        sortedTextureCoordinates.Add(TextureCoordinates[oldVertIndex]);
                    if (VertexColors.Count != 0)
                        sortedVertexColors.Add(VertexColors[newVertIndex]);
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
                    sortedVertices.Add(Vertices[oldVertIndex]);
                    if (Normals.Count != 0)
                        sortedNormals.Add(Normals[newVertIndex]);
                    if (TextureCoordinates.Count != 0)
                        sortedTextureCoordinates.Add(TextureCoordinates[oldVertIndex]);
                    if (VertexColors.Count != 0)
                        sortedVertexColors.Add(VertexColors[newVertIndex]);
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
                    sortedVertices.Add(Vertices[oldVertIndex]);
                    if (Normals.Count != 0)
                        sortedNormals.Add(Normals[newVertIndex]);
                    if (TextureCoordinates.Count != 0)
                        sortedTextureCoordinates.Add(TextureCoordinates[oldVertIndex]);
                    if (VertexColors.Count != 0)
                        sortedVertexColors.Add(VertexColors[newVertIndex]);
                }

                // Save this updated triangle
                sortedTriangleFaces.Add(curTriangleFace);
            }
            TriangleFaces = sortedTriangleFaces;
            Vertices = sortedVertices;
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

        // TODO: Look into collapsing into the following method since there is a lot of shared code
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

        public List<MeshData> GetMeshDataChunks(BoundingBox boundingBox, List<TriangleFace> faces, int maxFaceCountPerChunk)
        {
            List<MeshData> returnMeshChunks = new List<MeshData>();

            // If there are too many triangles to fit in a single box, cut the box into two and generate two child world model objects
            if (faces.Count > maxFaceCountPerChunk)
            {
                // Create two new bounding boxes
                SplitBox splitBox = SplitBox.GenerateXYSplitBoxFromBoundingBox(boundingBox);

                // Calculate what triangles fit into these boxes
                List<TriangleFace> aBoxTriangles = new List<TriangleFace>();
                List<TriangleFace> bBoxTriangles = new List<TriangleFace>();

                foreach (TriangleFace triangle in faces)
                {
                    // Skip any faces that aren't actually triangles
                    if (triangle.V1 == triangle.V2 || triangle.V2 == triangle.V3 || triangle.V1 == triangle.V3)
                        continue;

                    // Get center point
                    Vector3 v1 = Vertices[triangle.V1];
                    Vector3 v2 = Vertices[triangle.V2];
                    Vector3 v3 = Vertices[triangle.V3];
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
                returnMeshChunks.AddRange(GetMeshDataChunks(splitBox.BoxA, aBoxTriangles, maxFaceCountPerChunk));
                returnMeshChunks.AddRange(GetMeshDataChunks(splitBox.BoxB, bBoxTriangles, maxFaceCountPerChunk));
            }
            else
            {
                MeshData newMeshChunk = GetMeshDataForFaces(faces);
                if (newMeshChunk.TriangleFaces.Count > 0)
                {
                    newMeshChunk.CondenseAndRenumberVertexIndices();
                    returnMeshChunks.Add(newMeshChunk);
                }
            }
            return returnMeshChunks;
        }
        
        public static void GetSplitMeshData(MeshData meshToExtractFrom, BoundingBox extractionArea,
            out MeshData extractedMeshData, out MeshData remainderMeshData)
        {
            // Divide all triangles into the two groups, those in the extraction area and those that are not
            List<TriangleFace> extractedFaces = new List<TriangleFace>();
            List<TriangleFace> remainderFaces = new List<TriangleFace>();
            foreach (TriangleFace face in meshToExtractFrom.TriangleFaces)
            {
                // Skip any faces that aren't actually triangles
                if (face.V1 == face.V2 || face.V2 == face.V3 || face.V1 == face.V3)
                    continue;

                // Get center point
                Vector3 faceV1 = meshToExtractFrom.Vertices[face.V1];
                Vector3 faceV2 = meshToExtractFrom.Vertices[face.V2];
                Vector3 faceV3 = meshToExtractFrom.Vertices[face.V3];
                Vector3 faceCenter = new Vector3((faceV1.X + faceV2.X + faceV3.X) / 3, (faceV1.Y + faceV2.Y + faceV3.Y) / 3, (faceV1.Z + faceV2.Z + faceV3.Z) / 3);

                // Determine if it's within the extraction region and split accordingly
                if (extractionArea.ContainsPoint(faceCenter))
                    extractedFaces.Add(face);
                else
                    remainderFaces.Add(face);
            }

            // Create the new mesh objects
            extractedMeshData = new MeshData(meshToExtractFrom).GetMeshDataForFaces(extractedFaces);
            remainderMeshData = new MeshData(meshToExtractFrom).GetMeshDataForFaces(remainderFaces);
        }

        public static void GetSplitMeshDataWithXYClipping(MeshData meshToExtractFrom, BoundingBox extractionArea,
            out MeshData extractedMeshData, out MeshData remainderMeshData)
        {
            // Divide all triangles into the two groups, those in the extraction area and those that are not
            List<TriangleFace> extractedFaces = new List<TriangleFace>();
            List<TriangleFace> intersectingFaces = new List<TriangleFace>();
            List<TriangleFace> remainderFaces = new List<TriangleFace>();
            foreach (TriangleFace face in meshToExtractFrom.TriangleFaces)
            {
                // Skip any faces that aren't actually triangles
                if (face.V1 == face.V2 || face.V2 == face.V3 || face.V1 == face.V3)
                    continue;

                // Test all 3 points
                Vector3 faceV1 = meshToExtractFrom.Vertices[face.V1];
                bool faceV1InExtraction = extractionArea.ContainsPoint(faceV1);
                Vector3 faceV2 = meshToExtractFrom.Vertices[face.V2];
                bool faceV2InExtraction = extractionArea.ContainsPoint(faceV2);
                Vector3 faceV3 = meshToExtractFrom.Vertices[face.V3];
                bool faceV3InExtraction = extractionArea.ContainsPoint(faceV3);

                // Sort into three buckets based on where the points lay
                if (faceV1InExtraction && faceV2InExtraction && faceV3InExtraction)
                    extractedFaces.Add(face);
                else if (!faceV1InExtraction && !faceV2InExtraction && !faceV3InExtraction)
                    remainderFaces.Add(face);
                else
                    intersectingFaces.Add(face);
            }

            // Create the new mesh objects for categorized faces so far
            extractedMeshData = new MeshData(meshToExtractFrom).GetMeshDataForFaces(extractedFaces);
            extractedMeshData.CondenseAndRenumberVertexIndices();
            remainderMeshData = new MeshData(meshToExtractFrom).GetMeshDataForFaces(remainderFaces);
            remainderMeshData.CondenseAndRenumberVertexIndices();
            MeshData intersectingMeshData = new MeshData(meshToExtractFrom).GetMeshDataForFaces(intersectingFaces);
            intersectingMeshData.CondenseAndRenumberVertexIndices();

            foreach (TriangleFace triangleFace in intersectingMeshData.TriangleFaces)
                SplitTriangleByY(triangleFace, extractionArea.TopCorner.Y, ref remainderMeshData, ref extractedMeshData, intersectingMeshData);


            // Process known intersecting triangles crossing the high X line
            //MeshData newWorkingInsideIntersectingMeshDataHighX = new MeshData();
            //foreach (TriangleFace triangleFace in intersectingMeshData.TriangleFaces)
            //    SplitTriangleByX(triangleFace, extractionArea.TopCorner.X, ref remainderMeshData, ref newWorkingInsideIntersectingMeshDataHighX, intersectingMeshData);

            //// Processing triangles crossing the low x line
            //MeshData newWorkingInsideIntersectingMeshDataLowX = new MeshData();
            //foreach (TriangleFace triangleFace in newWorkingInsideIntersectingMeshDataHighX.TriangleFaces)
            //    SplitTriangleByX(triangleFace, extractionArea.BottomCorner.X, ref newWorkingInsideIntersectingMeshDataLowX, ref remainderMeshData, newWorkingInsideIntersectingMeshDataHighX);

            //// Processing triangles crossing the high y line
            //MeshData newWorkingInsideIntersectingMeshDataHighY = new MeshData();
            //foreach (TriangleFace triangleFace in newWorkingInsideIntersectingMeshDataLowX.TriangleFaces)
            //    SplitTriangleByY(triangleFace, extractionArea.TopCorner.Y, ref remainderMeshData, ref newWorkingInsideIntersectingMeshDataHighY, newWorkingInsideIntersectingMeshDataLowX);

            //// Processing remaining triangles crossing the low y line
            //foreach (TriangleFace triangleFace in newWorkingInsideIntersectingMeshDataLowX.TriangleFaces)
            //    SplitTriangleByY(triangleFace, extractionArea.BottomCorner.Y, ref extractedMeshData, ref remainderMeshData, newWorkingInsideIntersectingMeshDataHighY);
        }

        private static void SplitTriangleByX(TriangleFace triangle, float xLine, ref MeshData positiveMeshData, 
            ref MeshData negativeMeshData, MeshData sourceMeshData)
        {
            // Get the face data
            Vector3 v1 = sourceMeshData.Vertices[triangle.V1];
            Vector3 v2 = sourceMeshData.Vertices[triangle.V2];
            Vector3 v3 = sourceMeshData.Vertices[triangle.V3];
            Vector3 n1 = sourceMeshData.Normals[triangle.V1];
            Vector3 n2 = sourceMeshData.Normals[triangle.V2];
            Vector3 n3 = sourceMeshData.Normals[triangle.V3];
            TextureCoordinates t1 = sourceMeshData.TextureCoordinates[triangle.V1];
            TextureCoordinates t2 = sourceMeshData.TextureCoordinates[triangle.V2];
            TextureCoordinates t3 = sourceMeshData.TextureCoordinates[triangle.V3];
            ColorRGBA c1 = sourceMeshData.VertexColors[triangle.V1];
            ColorRGBA c2 = sourceMeshData.VertexColors[triangle.V2];
            ColorRGBA c3 = sourceMeshData.VertexColors[triangle.V3];

            // Collect the points on each side of the line
            MeshData newPositiveMeshData = new MeshData();
            MeshData newNegativeMeshData = new MeshData();
            if (v1.X >= xLine)
            {
                newPositiveMeshData.Vertices.Add(v1);
                newPositiveMeshData.Normals.Add(n1);
                newPositiveMeshData.TextureCoordinates.Add(t1);
                newPositiveMeshData.VertexColors.Add(c1);
            }
            else
            {
                newNegativeMeshData.Vertices.Add(v1);
                newNegativeMeshData.Normals.Add(n1);
                newNegativeMeshData.TextureCoordinates.Add(t1);
                newNegativeMeshData.VertexColors.Add(c1);
            }
            if (v2.X >= xLine)
            {
                newPositiveMeshData.Vertices.Add(v2);
                newPositiveMeshData.Normals.Add(n2);
                newPositiveMeshData.TextureCoordinates.Add(t2);
                newPositiveMeshData.VertexColors.Add(c2);
            }
            else
            {
                newNegativeMeshData.Vertices.Add(v2);
                newNegativeMeshData.Normals.Add(n2);
                newNegativeMeshData.TextureCoordinates.Add(t2);
                newNegativeMeshData.VertexColors.Add(c2);
            }
            if (v3.X >= xLine)
            {
                newPositiveMeshData.Vertices.Add(v3);
                newPositiveMeshData.Normals.Add(n3);
                newPositiveMeshData.TextureCoordinates.Add(t3);
                newPositiveMeshData.VertexColors.Add(c3);
            }
            else
            {
                newNegativeMeshData.Vertices.Add(v3);
                newNegativeMeshData.Normals.Add(n3);
                newNegativeMeshData.TextureCoordinates.Add(t3);
                newNegativeMeshData.VertexColors.Add(c3);
            }

            // Add intersection data if the verts didn't all fall on one line
            if (newNegativeMeshData.Vertices.Count != 0 && newPositiveMeshData.Vertices.Count != 0)
            {
                CheckIntersectWithXPlaneAndGenerateVertData(v1, v2, n1, n2, t1, t2, c1, c2, xLine, ref newPositiveMeshData, ref newNegativeMeshData);
                CheckIntersectWithXPlaneAndGenerateVertData(v2, v3, n2, n3, t2, t3, c2, c3, xLine, ref newPositiveMeshData, ref newNegativeMeshData);
                CheckIntersectWithXPlaneAndGenerateVertData(v3, v1, n3, n1, t3, t1, c3, c1, xLine, ref newPositiveMeshData, ref newNegativeMeshData);
            }           

            // Generate the triangles
            CreateTrianglesAndSaveData(triangle.MaterialIndex, newNegativeMeshData, ref negativeMeshData);
            CreateTrianglesAndSaveData(triangle.MaterialIndex, newPositiveMeshData, ref positiveMeshData);
        }

        private static void CheckIntersectWithXPlaneAndGenerateVertData(Vector3 v1, Vector3 v2, Vector3 n1, Vector3 n2, TextureCoordinates t1, TextureCoordinates t2, 
            ColorRGBA c1, ColorRGBA c2, float xLine, ref MeshData positiveMeshData, ref MeshData negativeMeshData)
        {
            // Do nothing if the verts are on the same side
            if (v1.X >= xLine && v2.X >= xLine)
                return;
            if (v1.X < xLine && v2.X < xLine)
                return;

            // If intersection occurs, add the interpolated data to the lists
            float t = (xLine - v1.X) / (v2.X - v1.X);
            
            // Vertex
            Vector3 vertexIntersection = new Vector3(xLine, v1.Y + t * (v2.Y - v1.Y), v1.Z + t * (v2.Z - v1.Z));
            positiveMeshData.Vertices.Add(vertexIntersection);
            negativeMeshData.Vertices.Add(vertexIntersection);
            
            // Normal
            Vector3 normalIntersection = new Vector3(n1.X + t * (n2.X - n1.X), n1.Y + t * (n2.Y - n1.Y), n1.Z + t * (n2.Z - n1.Z));
            positiveMeshData.Normals.Add(normalIntersection);
            negativeMeshData.Normals.Add(normalIntersection);
            
            // Texture Coordinates
            TextureCoordinates textureCoordinates = new TextureCoordinates(t1.X + t * (t2.X - t1.X), t1.Y + t * (t2.Y - t1.Y));
            positiveMeshData.TextureCoordinates.Add(textureCoordinates);
            negativeMeshData.TextureCoordinates.Add(textureCoordinates);

            // Vertex Color
            byte r = Convert.ToByte(MathF.Min(Convert.ToSingle(c1.R) + t * (Convert.ToSingle(c2.R) - Convert.ToSingle(c1.R)), 255f));
            byte g = Convert.ToByte(MathF.Min(Convert.ToSingle(c1.G) + t * (Convert.ToSingle(c2.G) - Convert.ToSingle(c1.G)), 255f));
            byte b = Convert.ToByte(MathF.Min(Convert.ToSingle(c1.B) + t * (Convert.ToSingle(c2.B) - Convert.ToSingle(c1.B)), 255f));
            byte a = Convert.ToByte(MathF.Min(Convert.ToSingle(c1.A) + t * (Convert.ToSingle(c2.A) - Convert.ToSingle(c1.A)), 255f));
            ColorRGBA vertexColor = new ColorRGBA(r, g, b, a);
            positiveMeshData.VertexColors.Add(vertexColor);
            negativeMeshData.VertexColors.Add(vertexColor);
        }

        private static void SplitTriangleByY(TriangleFace triangle, float yLine, ref MeshData positiveMeshData,
            ref MeshData negativeMeshData, MeshData sourceMeshData)
        {
            // Get the face data
            Vector3 v1 = sourceMeshData.Vertices[triangle.V1];
            Vector3 v2 = sourceMeshData.Vertices[triangle.V2];
            Vector3 v3 = sourceMeshData.Vertices[triangle.V3];
            Vector3 n1 = sourceMeshData.Normals[triangle.V1];
            Vector3 n2 = sourceMeshData.Normals[triangle.V2];
            Vector3 n3 = sourceMeshData.Normals[triangle.V3];
            TextureCoordinates t1 = sourceMeshData.TextureCoordinates[triangle.V1];
            TextureCoordinates t2 = sourceMeshData.TextureCoordinates[triangle.V2];
            TextureCoordinates t3 = sourceMeshData.TextureCoordinates[triangle.V3];
            ColorRGBA c1 = sourceMeshData.VertexColors[triangle.V1];
            ColorRGBA c2 = sourceMeshData.VertexColors[triangle.V2];
            ColorRGBA c3 = sourceMeshData.VertexColors[triangle.V3];

            // Collect the points on each side of the line
            MeshData newPositiveMeshData = new MeshData();
            MeshData newNegativeMeshData = new MeshData();
            if (v1.Y >= yLine)
            {
                newPositiveMeshData.Vertices.Add(v1);
                newPositiveMeshData.Normals.Add(n1);
                newPositiveMeshData.TextureCoordinates.Add(t1);
                newPositiveMeshData.VertexColors.Add(c1);
            }
            else
            {
                newNegativeMeshData.Vertices.Add(v1);
                newNegativeMeshData.Normals.Add(n1);
                newNegativeMeshData.TextureCoordinates.Add(t1);
                newNegativeMeshData.VertexColors.Add(c1);
            }
            if (v2.Y >= yLine)
            {
                newPositiveMeshData.Vertices.Add(v2);
                newPositiveMeshData.Normals.Add(n2);
                newPositiveMeshData.TextureCoordinates.Add(t2);
                newPositiveMeshData.VertexColors.Add(c2);
            }
            else
            {
                newNegativeMeshData.Vertices.Add(v2);
                newNegativeMeshData.Normals.Add(n2);
                newNegativeMeshData.TextureCoordinates.Add(t2);
                newNegativeMeshData.VertexColors.Add(c2);
            }
            if (v3.Y >= yLine)
            {
                newPositiveMeshData.Vertices.Add(v3);
                newPositiveMeshData.Normals.Add(n3);
                newPositiveMeshData.TextureCoordinates.Add(t3);
                newPositiveMeshData.VertexColors.Add(c3);
            }
            else
            {
                newNegativeMeshData.Vertices.Add(v3);
                newNegativeMeshData.Normals.Add(n3);
                newNegativeMeshData.TextureCoordinates.Add(t3);
                newNegativeMeshData.VertexColors.Add(c3);
            }

            // Add intersection data if the verts didn't all fall on one line
            if (newNegativeMeshData.Vertices.Count != 0 && newPositiveMeshData.Vertices.Count != 0)
            {
                CheckIntersectWithYPlaneAndGenerateVertData(v1, v2, n1, n2, t1, t2, c1, c2, yLine, ref newPositiveMeshData, ref newNegativeMeshData);
                CheckIntersectWithYPlaneAndGenerateVertData(v2, v3, n2, n3, t2, t3, c2, c3, yLine, ref newPositiveMeshData, ref newNegativeMeshData);
                CheckIntersectWithYPlaneAndGenerateVertData(v3, v1, n3, n1, t3, t1, c3, c1, yLine, ref newPositiveMeshData, ref newNegativeMeshData);
            }

            // Generate the triangles
            CreateTrianglesAndSaveData(triangle.MaterialIndex, newNegativeMeshData, ref negativeMeshData);
            CreateTrianglesAndSaveData(triangle.MaterialIndex, newPositiveMeshData, ref positiveMeshData);
        }

        private static void CheckIntersectWithYPlaneAndGenerateVertData(Vector3 v1, Vector3 v2, Vector3 n1, Vector3 n2, TextureCoordinates t1, TextureCoordinates t2,
            ColorRGBA c1, ColorRGBA c2, float yLine, ref MeshData positiveMeshData, ref MeshData negativeMeshData)
        {
            // Do nothing if the verts are on the same side
            if (v1.Y >= yLine && v2.Y >= yLine)
                return;
            if (v1.Y < yLine && v2.Y < yLine)
                return;

            // If intersection occurs, add the interpolated data to the lists
            float t = (yLine - v1.Y) / (v2.Y - v1.Y);

            // Vertex
            Vector3 vertexIntersection = new Vector3(v1.X + t * (v2.X - v1.X), yLine, v1.Z + t * (v2.Z - v1.Z));
            positiveMeshData.Vertices.Add(vertexIntersection);
            negativeMeshData.Vertices.Add(vertexIntersection);

            // Normal
            Vector3 normalIntersection = new Vector3(n1.X + t * (n2.X - n1.X), n1.Y + t * (n2.Y - n1.Y), n1.Z + t * (n2.Z - n1.Z));
            positiveMeshData.Normals.Add(normalIntersection);
            negativeMeshData.Normals.Add(normalIntersection);

            // Texture Coordinates
            TextureCoordinates textureCoordinates = new TextureCoordinates(t1.X + t * (t2.X - t1.X), t1.Y + t * (t2.Y - t1.Y));
            positiveMeshData.TextureCoordinates.Add(textureCoordinates);
            negativeMeshData.TextureCoordinates.Add(textureCoordinates);

            // Vertex Color
            byte r = Convert.ToByte(MathF.Min(Convert.ToSingle(c1.R) + t * (Convert.ToSingle(c2.R) - Convert.ToSingle(c1.R)), 255f));
            byte g = Convert.ToByte(MathF.Min(Convert.ToSingle(c1.G) + t * (Convert.ToSingle(c2.G) - Convert.ToSingle(c1.G)), 255f));
            byte b = Convert.ToByte(MathF.Min(Convert.ToSingle(c1.B) + t * (Convert.ToSingle(c2.B) - Convert.ToSingle(c1.B)), 255f));
            byte a = Convert.ToByte(MathF.Min(Convert.ToSingle(c1.A) + t * (Convert.ToSingle(c2.A) - Convert.ToSingle(c1.A)), 255f));
            ColorRGBA vertexColor = new ColorRGBA(r, g, b, a);
            positiveMeshData.VertexColors.Add(vertexColor);
            negativeMeshData.VertexColors.Add(vertexColor);
        }

        private static void CreateTrianglesAndSaveData(int materialIndex, MeshData newMeshData, ref MeshData workingMeshData)
        {
            int startIndex = workingMeshData.Vertices.Count;
            for (int i = 0; i < newMeshData.Vertices.Count-2; i++)
                workingMeshData.TriangleFaces.Add(new TriangleFace(materialIndex, startIndex + i, startIndex + i + 1, startIndex + i + 2));
            workingMeshData.Vertices.AddRange(newMeshData.Vertices);
            workingMeshData.Normals.AddRange(newMeshData.Normals);
            workingMeshData.TextureCoordinates.AddRange(newMeshData.TextureCoordinates);
            workingMeshData.VertexColors.AddRange(newMeshData.VertexColors);
        }
    }
}
