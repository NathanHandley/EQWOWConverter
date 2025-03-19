//  Author: Nathan Handley (nathanhandley@protonmail.com)
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

namespace EQWOWConverter.Common
{
    internal class MeshData
    {
        public int AnimatedVerticesDelayInMS = 0;
        public List<Vector3> Vertices = new List<Vector3>();
        public List<Vector3> Normals = new List<Vector3>();
        public List<TextureCoordinates> TextureCoordinates = new List<TextureCoordinates>();
        public List<TriangleFace> TriangleFaces = new List<TriangleFace>();
        public List<ColorRGBA> VertexColors = new List<ColorRGBA>();
        public List<byte> BoneIDs = new List<byte>(); // Note: Only single associations, but WOW can support up to 4 w/weights
        public List<AnimatedVertexFrames> AnimatedVertexFramesByVertexIndex = new List<AnimatedVertexFrames>();

        public MeshData() { }

        public MeshData(MeshData meshData)
        {
            Vertices = new List<Vector3>(meshData.Vertices.Count);
            foreach (Vector3 vertex in meshData.Vertices)
                Vertices.Add(new Vector3(vertex));
            Normals = new List<Vector3>(meshData.Normals.Count);
            foreach (Vector3 normal in meshData.Normals)
                Normals.Add(new Vector3(normal));
            TextureCoordinates = new List<TextureCoordinates>(meshData.TextureCoordinates.Count);
            foreach(TextureCoordinates textureCoordinate in meshData.TextureCoordinates)
                TextureCoordinates.Add(new TextureCoordinates(textureCoordinate));
            TriangleFaces = new List<TriangleFace>(meshData.TriangleFaces.Count);
            foreach(TriangleFace triangleFace in meshData.TriangleFaces)
                TriangleFaces.Add(new TriangleFace(triangleFace));
            VertexColors = new List<ColorRGBA>(meshData.VertexColors.Count);
            foreach (ColorRGBA colorRGBA in meshData.VertexColors)
                VertexColors.Add(new ColorRGBA(colorRGBA));
            BoneIDs = new List<byte>(meshData.BoneIDs.Count);
            foreach (byte boneID in meshData.BoneIDs)
                BoneIDs.Add(boneID);
            AnimatedVertexFramesByVertexIndex = new List<AnimatedVertexFrames>(meshData.AnimatedVertexFramesByVertexIndex.Count);
            foreach(AnimatedVertexFrames frames in meshData.AnimatedVertexFramesByVertexIndex)
            {
                AnimatedVertexFramesByVertexIndex.Add(new AnimatedVertexFrames());
                foreach (Vector3 frameVertex in frames.VertexOffsetFrames)
                    AnimatedVertexFramesByVertexIndex[AnimatedVertexFramesByVertexIndex.Count - 1].VertexOffsetFrames.Add(new Vector3(frameVertex));
            }
        }

        public void GenerateAsBox(BoundingBox boundingBox, int materialIndex, MeshBoxRenderType renderType)
        {
            // Clear prior data
            Vertices.Clear();
            Normals.Clear();
            TextureCoordinates.Clear();
            TriangleFaces.Clear();
            VertexColors.Clear();
            BoneIDs.Clear();

            // Set temp values
            float highX = boundingBox.TopCorner.X;
            float lowX = boundingBox.BottomCorner.X;
            float highY = boundingBox.TopCorner.Y;
            float lowY = boundingBox.BottomCorner.Y;
            float highZ = boundingBox.TopCorner.Z;
            float lowZ = boundingBox.BottomCorner.Z;

            // Side 1
            int quadFaceStartVert = Vertices.Count;
            Vertices.Add(new Vector3(highX, lowY, highZ));
            TextureCoordinates.Add(new TextureCoordinates(1, 1));
            Vertices.Add(new Vector3(highX, lowY, lowZ));
            TextureCoordinates.Add(new TextureCoordinates(1, 0));
            Vertices.Add(new Vector3(lowX, lowY, lowZ));
            TextureCoordinates.Add(new TextureCoordinates(0, 0));
            Vertices.Add(new Vector3(lowX, lowY, highZ));
            TextureCoordinates.Add(new TextureCoordinates(0, 1));
            if (renderType == MeshBoxRenderType.Outward || renderType == MeshBoxRenderType.Both)
            {
                TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 1, quadFaceStartVert, quadFaceStartVert + 3));
                TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 1, quadFaceStartVert + 3, quadFaceStartVert + 2));
            }
            if (renderType == MeshBoxRenderType.Inward || renderType == MeshBoxRenderType.Both)
            {
                TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 3, quadFaceStartVert, quadFaceStartVert + 1));
                TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 2, quadFaceStartVert + 3, quadFaceStartVert + 1));
            }

            // Side 2
            quadFaceStartVert = Vertices.Count;
            Vertices.Add(new Vector3(highX, highY, highZ));
            TextureCoordinates.Add(new TextureCoordinates(1, 1));
            Vertices.Add(new Vector3(lowX, highY, highZ));
            TextureCoordinates.Add(new TextureCoordinates(0, 1));
            Vertices.Add(new Vector3(lowX, highY, lowZ));
            TextureCoordinates.Add(new TextureCoordinates(0, 0));
            Vertices.Add(new Vector3(highX, highY, lowZ));
            TextureCoordinates.Add(new TextureCoordinates(1, 0));
            if (renderType == MeshBoxRenderType.Outward || renderType == MeshBoxRenderType.Both)
            {
                TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 1, quadFaceStartVert, quadFaceStartVert + 3));
                TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 1, quadFaceStartVert + 3, quadFaceStartVert + 2));
            }
            if (renderType == MeshBoxRenderType.Inward || renderType == MeshBoxRenderType.Both)
            {
                TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 3, quadFaceStartVert, quadFaceStartVert + 1));
                TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 2, quadFaceStartVert + 3, quadFaceStartVert + 1));
            }

            // Side 3
            quadFaceStartVert = Vertices.Count;
            Vertices.Add(new Vector3(highX, highY, highZ));
            TextureCoordinates.Add(new TextureCoordinates(1, 1));
            Vertices.Add(new Vector3(highX, highY, lowZ));
            TextureCoordinates.Add(new TextureCoordinates(1, 0));
            Vertices.Add(new Vector3(highX, lowY, lowZ));
            TextureCoordinates.Add(new TextureCoordinates(0, 0));
            Vertices.Add(new Vector3(highX, lowY, highZ));
            TextureCoordinates.Add(new TextureCoordinates(0, 1));
            if (renderType == MeshBoxRenderType.Outward || renderType == MeshBoxRenderType.Both)
            {
                TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 1, quadFaceStartVert, quadFaceStartVert + 3));
                TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 1, quadFaceStartVert + 3, quadFaceStartVert + 2));
            }
            if (renderType == MeshBoxRenderType.Inward || renderType == MeshBoxRenderType.Both)
            {
                TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 3, quadFaceStartVert, quadFaceStartVert + 1));
                TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 2, quadFaceStartVert + 3, quadFaceStartVert + 1));
            }

            // Side 4
            quadFaceStartVert = Vertices.Count;
            Vertices.Add(new Vector3(lowX, highY, highZ));
            TextureCoordinates.Add(new TextureCoordinates(1, 1));
            Vertices.Add(new Vector3(lowX, lowY, highZ));
            TextureCoordinates.Add(new TextureCoordinates(0, 1));
            Vertices.Add(new Vector3(lowX, lowY, lowZ));
            TextureCoordinates.Add(new TextureCoordinates(0, 0));
            Vertices.Add(new Vector3(lowX, highY, lowZ));
            TextureCoordinates.Add(new TextureCoordinates(1, 0));
            if (renderType == MeshBoxRenderType.Outward || renderType == MeshBoxRenderType.Both)
            {
                TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 1, quadFaceStartVert, quadFaceStartVert + 3));
                TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 1, quadFaceStartVert + 3, quadFaceStartVert + 2));
            }
            if (renderType == MeshBoxRenderType.Inward || renderType == MeshBoxRenderType.Both)
            {
                TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 3, quadFaceStartVert, quadFaceStartVert + 1));
                TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 2, quadFaceStartVert + 3, quadFaceStartVert + 1));
            }

            // Top
            quadFaceStartVert = Vertices.Count;
            Vertices.Add(new Vector3(highX, highY, highZ));
            TextureCoordinates.Add(new TextureCoordinates(1, 1));
            Vertices.Add(new Vector3(highX, lowY, highZ));
            TextureCoordinates.Add(new TextureCoordinates(1, 0));
            Vertices.Add(new Vector3(lowX, lowY, highZ));
            TextureCoordinates.Add(new TextureCoordinates(0, 0));
            Vertices.Add(new Vector3(lowX, highY, highZ));
            TextureCoordinates.Add(new TextureCoordinates(0, 1));
            if (renderType == MeshBoxRenderType.Outward || renderType == MeshBoxRenderType.Both)
            {
                TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 1, quadFaceStartVert, quadFaceStartVert + 3));
                TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 1, quadFaceStartVert + 3, quadFaceStartVert + 2));
            }
            if (renderType == MeshBoxRenderType.Inward || renderType == MeshBoxRenderType.Both)
            {
                TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 3, quadFaceStartVert, quadFaceStartVert + 1));
                TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 2, quadFaceStartVert + 3, quadFaceStartVert + 1));
            }

            // Bottom
            quadFaceStartVert = Vertices.Count;
            Vertices.Add(new Vector3(highX, highY, lowZ));
            TextureCoordinates.Add(new TextureCoordinates(1, 1));
            Vertices.Add(new Vector3(lowX, highY, lowZ));
            TextureCoordinates.Add(new TextureCoordinates(0, 1));
            Vertices.Add(new Vector3(lowX, lowY, lowZ));
            TextureCoordinates.Add(new TextureCoordinates(0, 0));
            Vertices.Add(new Vector3(highX, lowY, lowZ));
            TextureCoordinates.Add(new TextureCoordinates(1, 0));
            
            if (renderType == MeshBoxRenderType.Outward || renderType == MeshBoxRenderType.Both)
            {
                TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 1, quadFaceStartVert, quadFaceStartVert + 3));
                TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 1, quadFaceStartVert + 3, quadFaceStartVert + 2));
            }
            if (renderType == MeshBoxRenderType.Inward || renderType == MeshBoxRenderType.Both)
            {
                TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 3, quadFaceStartVert, quadFaceStartVert + 1));
                TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 2, quadFaceStartVert + 3, quadFaceStartVert + 1));
            }

            // Fill in the blanks
            for(int i = 0; i < Vertices.Count; i++)
            {
                Normals.Add(new Vector3(0, 0, 0));
                VertexColors.Add(new ColorRGBA(0, 0, 0));
                BoneIDs.Add(0);
            }
        }

        public void ApplyRotationOnVertices(Quaternion rotation)
        {
            foreach (Vector3 vertex in Vertices)
                vertex.Rotate(rotation);
        }

        public void ApplyTranslationOnVertices(Vector3 translation)
        {
            foreach(Vector3 vertex in Vertices)
            {
                vertex.X += translation.X;
                vertex.Y += translation.Y;
                vertex.Z += translation.Z;
            }
        }

        public void ApplyScaleOnVertices(float scale)
        {
            foreach (Vector3 vertex in Vertices)
                vertex.Scale(scale);
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
                BoneIDs = newMeshData.BoneIDs;
                AnimatedVertexFramesByVertexIndex = newMeshData.AnimatedVertexFramesByVertexIndex;
            }
        }

        public void ApplyEQToWoWGeometryTranslationsAndScale(bool rotateZAxis, float scale)
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
                vertex.X *= scale;
                vertex.Y *= scale;
                vertex.Z *= scale;
                if (rotateZAxis == true)
                {
                    vertex.X *= -1;
                    vertex.Y *= -1;
                }
            }
            // Flip Y on the texture coordinates
            foreach (TextureCoordinates textureCoordinate in TextureCoordinates)
            {
                textureCoordinate.Y *= -1;
            }
        }

        public void ApplyEQToWoWVertexColor(double vertexColorIntensityOverride)
        {
            double intensityAmount = Configuration.LIGHT_DEFAULT_VERTEX_COLOR_INTENSITY;
            if (vertexColorIntensityOverride >= 0)
                intensityAmount = vertexColorIntensityOverride;

            // Vertex colors are reduced for external areas due to the natural zone light
            foreach (ColorRGBA vertexColor in VertexColors)
            {
                vertexColor.R = Convert.ToByte(Convert.ToDouble(vertexColor.R) * intensityAmount);
                vertexColor.G = Convert.ToByte(Convert.ToDouble(vertexColor.G) * intensityAmount);
                vertexColor.B = Convert.ToByte(Convert.ToDouble(vertexColor.B) * intensityAmount);
                //vertexColor.A = vertexColor.A;
            }
        }

        public void AddMeshData(MeshData meshDataToAdd)
        {
            // Increase the vertex information for the mesh data being added in since the vertices array will increase
            int indexValueToAdd = Vertices.Count;
            foreach(TriangleFace face in meshDataToAdd.TriangleFaces)
            {
                face.V1 += indexValueToAdd;
                face.V2 += indexValueToAdd;
                face.V3 += indexValueToAdd;
            }

            // Add the data to the arrays
            Vertices.AddRange(meshDataToAdd.Vertices);
            Normals.AddRange(meshDataToAdd.Normals);
            TextureCoordinates.AddRange(meshDataToAdd.TextureCoordinates);
            VertexColors.AddRange(meshDataToAdd.VertexColors);
            TriangleFaces.AddRange(meshDataToAdd.TriangleFaces);
            BoneIDs.AddRange(meshDataToAdd.BoneIDs);
            AnimatedVertexFramesByVertexIndex.AddRange(meshDataToAdd.AnimatedVertexFramesByVertexIndex);
            if (AnimatedVerticesDelayInMS == 0)
                AnimatedVerticesDelayInMS = meshDataToAdd.AnimatedVerticesDelayInMS;
        }

        public void RemoveInvalidMaterialReferences(List<Material> startingMaterialList, out List<Material> newMaterialList)
        {
            // Figure out what materials have geometry, and remap
            Dictionary<int, int> oldNewMaterialMappings = new Dictionary<int, int>();
            newMaterialList = new List<Material>();
            for (int i = 0; i <  startingMaterialList.Count; i++)
            {
                bool matchFound = false;
                for (int j = 0; j < TriangleFaces.Count; j++)
                {
                    if (TriangleFaces[j].MaterialIndex == startingMaterialList[i].Index)
                    {
                        // Match found, keep it
                        Material materialCopy = new Material(startingMaterialList[i]);
                        materialCopy.Index = Convert.ToUInt32(newMaterialList.Count);
                        oldNewMaterialMappings[i] = Convert.ToInt32(materialCopy.Index);
                        newMaterialList.Add(materialCopy);
                        matchFound = true;
                        break;
                    }
                }
                if (matchFound == false)
                {
                    // No match found, so delete it
                    oldNewMaterialMappings[i] = -1;
                }
            }

            // Update all of the material mappings in TriangleFaces
            foreach(TriangleFace face in TriangleFaces)
            {
                int curMaterialIndex = face.MaterialIndex;
                face.MaterialIndex = oldNewMaterialMappings[curMaterialIndex];
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
                    if (BoneIDs.Count != 0)
                        extractedMeshData.BoneIDs.Add(BoneIDs[oldVertIndex]);
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
                    if (BoneIDs.Count != 0)
                        extractedMeshData.BoneIDs.Add(BoneIDs[oldVertIndex]);
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
                    if (BoneIDs.Count != 0)
                        extractedMeshData.BoneIDs.Add(BoneIDs[oldVertIndex]);
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
                    if (BoneIDs.Count != 0)
                        extractedMeshData.BoneIDs.Add(BoneIDs[oldVertIndex]);
                    if (AnimatedVertexFramesByVertexIndex.Count != 0)
                        extractedMeshData.AnimatedVertexFramesByVertexIndex.Add(AnimatedVertexFramesByVertexIndex[oldVertIndex]);
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
                    if (BoneIDs.Count != 0)
                        extractedMeshData.BoneIDs.Add(BoneIDs[oldVertIndex]);
                    if (AnimatedVertexFramesByVertexIndex.Count != 0)
                        extractedMeshData.AnimatedVertexFramesByVertexIndex.Add(AnimatedVertexFramesByVertexIndex[oldVertIndex]);
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
                    if (BoneIDs.Count != 0)
                        extractedMeshData.BoneIDs.Add(BoneIDs[oldVertIndex]);
                    if (AnimatedVertexFramesByVertexIndex.Count != 0)
                        extractedMeshData.AnimatedVertexFramesByVertexIndex.Add(AnimatedVertexFramesByVertexIndex[oldVertIndex]);
                }

                // Save this updated triangle
                extractedMeshData.TriangleFaces.Add(curTriangleFace);
            }
            return extractedMeshData;
        }

        public void CondenseAndRenumberVertexIndices()
        {
            // Reorder the vertices / texcoords / normals / to match the sorted triangle faces
            List<Vector3> sortedVertices = new List<Vector3>(Vertices.Count);
            List<TriangleFace> sortedTriangleFaces = new List<TriangleFace>(TriangleFaces.Count);
            List<Vector3> sortedNormals = new List<Vector3>(Normals.Count);
            List<TextureCoordinates> sortedTextureCoordinates = new List<TextureCoordinates>(TextureCoordinates.Count);
            List<ColorRGBA> sortedVertexColors = new List<ColorRGBA>(VertexColors.Count);
            List<byte> sortedBoneIndexes = new List<byte>(BoneIDs.Count);
            List<AnimatedVertexFrames> sortedAnimatedVertexFrames = new List<AnimatedVertexFrames>(AnimatedVertexFramesByVertexIndex.Count);

            Dictionary<int, int> oldNewVertexIndices = new Dictionary<int, int>();
            int curSortedVertexCount = 0;
            for (int i = 0; i < TriangleFaces.Count; i++)
            {
                TriangleFace curTriangleFace = TriangleFaces[i];

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
                    int newVertIndex = curSortedVertexCount;
                    oldNewVertexIndices.Add(oldVertIndex, newVertIndex);
                    curTriangleFace.V1 = newVertIndex;
                    sortedVertices.Add(Vertices[oldVertIndex]);
                    curSortedVertexCount++;
                    if (Normals.Count != 0)
                        sortedNormals.Add(Normals[newVertIndex]);
                    if (TextureCoordinates.Count != 0)
                        sortedTextureCoordinates.Add(TextureCoordinates[oldVertIndex]);
                    if (VertexColors.Count != 0)
                        sortedVertexColors.Add(VertexColors[newVertIndex]);
                    if (BoneIDs.Count != 0)
                        sortedBoneIndexes.Add(BoneIDs[newVertIndex]);
                    if (AnimatedVertexFramesByVertexIndex.Count != 0)
                        sortedAnimatedVertexFrames.Add(AnimatedVertexFramesByVertexIndex[newVertIndex]);
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
                    int newVertIndex = curSortedVertexCount;
                    oldNewVertexIndices.Add(oldVertIndex, newVertIndex);
                    curTriangleFace.V2 = newVertIndex;
                    sortedVertices.Add(Vertices[oldVertIndex]);
                    curSortedVertexCount++;
                    if (Normals.Count != 0)
                        sortedNormals.Add(Normals[newVertIndex]);
                    if (TextureCoordinates.Count != 0)
                        sortedTextureCoordinates.Add(TextureCoordinates[oldVertIndex]);
                    if (VertexColors.Count != 0)
                        sortedVertexColors.Add(VertexColors[newVertIndex]);
                    if (BoneIDs.Count != 0)
                        sortedBoneIndexes.Add(BoneIDs[newVertIndex]);
                    if (AnimatedVertexFramesByVertexIndex.Count != 0)
                        sortedAnimatedVertexFrames.Add(AnimatedVertexFramesByVertexIndex[newVertIndex]);
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
                    int newVertIndex = curSortedVertexCount;
                    oldNewVertexIndices.Add(oldVertIndex, newVertIndex);
                    curTriangleFace.V3 = newVertIndex;
                    sortedVertices.Add(Vertices[oldVertIndex]);
                    curSortedVertexCount++;
                    if (Normals.Count != 0)
                        sortedNormals.Add(Normals[newVertIndex]);
                    if (TextureCoordinates.Count != 0)
                        sortedTextureCoordinates.Add(TextureCoordinates[oldVertIndex]);
                    if (VertexColors.Count != 0)
                        sortedVertexColors.Add(VertexColors[newVertIndex]);
                    if (BoneIDs.Count != 0)
                        sortedBoneIndexes.Add(BoneIDs[newVertIndex]);
                    if (AnimatedVertexFramesByVertexIndex.Count != 0)
                        sortedAnimatedVertexFrames.Add(AnimatedVertexFramesByVertexIndex[newVertIndex]);
                }

                // Save this updated triangle
                sortedTriangleFaces.Add(curTriangleFace);
            }
            TriangleFaces = sortedTriangleFaces;
            Vertices = sortedVertices;
        }

        public void SortDataByMaterialAndBones()
        {
            // Sort triangles by material
            SortTriangleFacesByMaterial();

            // Stop if no bone IDs
            if (BoneIDs.Count == 0)
                return;

            // Determine the material IDs and grab the indices by them
            SortedDictionary<int, HashSet<int>> VertexIndicesByMaterialID = new SortedDictionary<int, HashSet<int>>();
            foreach (TriangleFace triangleFace in TriangleFaces)
            {
                if (VertexIndicesByMaterialID.ContainsKey(triangleFace.MaterialIndex) == false)
                    VertexIndicesByMaterialID.Add(triangleFace.MaterialIndex, new HashSet<int>());
                VertexIndicesByMaterialID[triangleFace.MaterialIndex].Add(triangleFace.V1);
                VertexIndicesByMaterialID[triangleFace.MaterialIndex].Add(triangleFace.V2);
                VertexIndicesByMaterialID[triangleFace.MaterialIndex].Add(triangleFace.V3);
            }

            // Pre-populate the vertex index lookup
            Dictionary<int, int> oldNewVertexIndices = new Dictionary<int, int>();
            for (int i = 0; i < Vertices.Count; i++)
                oldNewVertexIndices.Add(i, i);

            // Begin sorted lists of elements
            List<Vector3> sortedVertices = new List<Vector3>();
            List<Vector3> sortedNormals = new List<Vector3>();
            List<TextureCoordinates> sortedTextureCoordinates = new List<TextureCoordinates>();
            List<ColorRGBA> sortedVertexColors = new List<ColorRGBA>();
            List<byte> sortedBoneIndexes = new List<byte>();
            List<AnimatedVertexFrames> sortedAnimatedVertexFrames = new List<AnimatedVertexFrames>();

            // Go through vertices on a material-by-material basis
            foreach (var vertexIndicesForMaterial in VertexIndicesByMaterialID)
            {
                // Build the list of boneIDs that can be found for this material
                int curMaterialIndex = vertexIndicesForMaterial.Key;
                SortedSet<byte> boneIDsInMaterial = new SortedSet<byte>();
                foreach(TriangleFace triangleFace in TriangleFaces)
                {
                    if (triangleFace.MaterialIndex == curMaterialIndex)
                    {
                        if (boneIDsInMaterial.Contains(BoneIDs[triangleFace.V1]) == false)
                            boneIDsInMaterial.Add(BoneIDs[triangleFace.V1]);
                        if (boneIDsInMaterial.Contains(BoneIDs[triangleFace.V2]) == false)
                            boneIDsInMaterial.Add(BoneIDs[triangleFace.V2]);
                        if (boneIDsInMaterial.Contains(BoneIDs[triangleFace.V3]) == false)
                            boneIDsInMaterial.Add(BoneIDs[triangleFace.V3]);
                    }
                }

                // Look at each bone and update references
                foreach(byte boneId in boneIDsInMaterial)
                {
                    // Iterate through vertices and save off bone matches
                    foreach(int oldIndex in vertexIndicesForMaterial.Value)
                    {
                        if (BoneIDs[oldIndex] == boneId)
                        {
                            // Store the lookup map
                            int newIndex = sortedVertices.Count;
                            oldNewVertexIndices[oldIndex] = newIndex;

                            // Create copied values
                            sortedVertices.Add(Vertices[oldIndex]);
                            sortedNormals.Add(Normals[oldIndex]);
                            sortedTextureCoordinates.Add(TextureCoordinates[oldIndex]);
                            if (VertexColors.Count != 0)
                                sortedVertexColors.Add(VertexColors[oldIndex]);
                            if (BoneIDs.Count != 0)
                                sortedBoneIndexes.Add(BoneIDs[oldIndex]);
                            if (AnimatedVertexFramesByVertexIndex.Count != 0)
                                sortedAnimatedVertexFrames.Add(AnimatedVertexFramesByVertexIndex[oldIndex]);
                        }
                    }
                }
            }

            // Update the triangle references
            List<TriangleFace> updatedTriangleFaces = new List<TriangleFace>();
            foreach (TriangleFace curFace in TriangleFaces)
            {
                int materialIndex = curFace.MaterialIndex;
                int v1 = oldNewVertexIndices[curFace.V1];
                int v2 = oldNewVertexIndices[curFace.V2];
                int v3 = oldNewVertexIndices[curFace.V3];
                TriangleFace newFace = new TriangleFace(materialIndex, v1, v2, v3);
                updatedTriangleFaces.Add(newFace);
            }

            // Save everything
            TriangleFaces = updatedTriangleFaces;
            Vertices = sortedVertices;
            Normals = sortedNormals;
            TextureCoordinates = sortedTextureCoordinates;
            VertexColors = sortedVertexColors;
            BoneIDs = sortedBoneIndexes;
            AnimatedVertexFramesByVertexIndex = sortedAnimatedVertexFrames;
        }

        public void SortTriangleFacesByMaterial()
        {
            // Sort triangles first by material
            TriangleFaces.Sort();

            // Reorder the vertices / texcoords / normals / to match the sorted triangle faces
            List<Vector3> sortedVertices = new List<Vector3>();
            List<Vector3> sortedNormals = new List<Vector3>();
            List<TextureCoordinates> sortedTextureCoordinates = new List<TextureCoordinates>();
            List<TriangleFace> sortedTriangleFaces = new List<TriangleFace>();
            List<ColorRGBA> sortedVertexColors = new List<ColorRGBA>();
            List<byte> sortedBoneIndexes = new List<byte>();
            List<AnimatedVertexFrames> sortedAnimatedVertexFrames = new List<AnimatedVertexFrames>();

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
                    if (BoneIDs.Count != 0)
                        sortedBoneIndexes.Add(BoneIDs[oldVertIndex]);
                    if (AnimatedVertexFramesByVertexIndex.Count != 0)
                        sortedAnimatedVertexFrames.Add(AnimatedVertexFramesByVertexIndex[oldVertIndex]);
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
                    if (BoneIDs.Count != 0)
                        sortedBoneIndexes.Add(BoneIDs[oldVertIndex]);
                    if (AnimatedVertexFramesByVertexIndex.Count != 0)
                        sortedAnimatedVertexFrames.Add(AnimatedVertexFramesByVertexIndex[oldVertIndex]);
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
                    if (BoneIDs.Count != 0)
                        sortedBoneIndexes.Add(BoneIDs[oldVertIndex]);
                    if (AnimatedVertexFramesByVertexIndex.Count != 0)
                        sortedAnimatedVertexFrames.Add(AnimatedVertexFramesByVertexIndex[oldVertIndex]);
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
            BoneIDs = sortedBoneIndexes;
            AnimatedVertexFramesByVertexIndex = sortedAnimatedVertexFrames;
        }

        // TODO: Look into collapsing into the following method since there is a lot of shared code
        public void SplitIntoChunks(MeshData allMeshData, BoundingBox curBoundingBox, List<TriangleFace> curTriangleFaces, Material curMaterial, ref List<Vector3> chunkPositions, ref List<MeshData> chunkMeshDatas)
        {
            // Calculate the true number of triangles that will be made
            int finalTriangleCount = curTriangleFaces.Count * curMaterial.NumOfAnimationFrames();

            // If the box is too big, cut it up
            if (curBoundingBox.FurthestPointDistanceFromCenterXOnly() >= Configuration.ZONE_MATERIAL_TO_OBJECT_SPLIT_MIN_XY_CENTER_TO_EDGE_DISTANCE
                || curBoundingBox.FurthestPointDistanceFromCenterYOnly() >= Configuration.ZONE_MATERIAL_TO_OBJECT_SPLIT_MIN_XY_CENTER_TO_EDGE_DISTANCE
                || finalTriangleCount >= Configuration.ZONE_MAX_FACES_PER_ZONE_MATERIAL_OBJECT)
            {
                // Create two new bounding boxes based on the longest edge
                SplitBox splitBox = SplitBox.GenerateXYSplitBox(curBoundingBox);

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

        public static List<MeshData> GetGeometrySplitIntoCubiods(MeshData meshData, float maxSpanPerCubiod = -1, int maxFaceCountPerCubiod = -1)
        {
            // Put self into the queue as a single item
            List<MeshData> pendingChunkQueue = new List<MeshData>() { meshData };
            
            // Process all chunks until they are within the max span
            List<MeshData> finishedChunks = new List<MeshData>();
            while (pendingChunkQueue.Count > 0)
            {
                // Pop off the last meshdata to work with it
                MeshData curMeshData = pendingChunkQueue[pendingChunkQueue.Count - 1];
                pendingChunkQueue.RemoveAt(pendingChunkQueue.Count - 1);

                // If a max span is violated, split the box into two and add both to the pending chunk queue
                BoundingBox curMeshBox = BoundingBox.GenerateBoxFromVectors(curMeshData.Vertices, Configuration.GENERATE_ADDED_BOUNDARY_AMOUNT);
                if ((maxSpanPerCubiod > 0 && (curMeshBox.GetXDistance() > maxSpanPerCubiod || curMeshBox.GetYDistance() > maxSpanPerCubiod || curMeshBox.GetZDistance() > maxSpanPerCubiod))
                    || (maxFaceCountPerCubiod > 0 && curMeshData.TriangleFaces.Count > maxFaceCountPerCubiod))
                {
                    SplitBox splitBox = SplitBox.GenerateXYZSplitBox(curMeshBox);
                    MeshData boxAMeshData;
                    MeshData boxBMeshData;
                    GetSplitMeshDataWithClipping(curMeshData, splitBox.BoxA, out boxAMeshData, out boxBMeshData);
                    pendingChunkQueue.Add(boxAMeshData);
                    pendingChunkQueue.Add(boxBMeshData);
                }

                // Otherwise, it's finished
                else
                    finishedChunks.Add(curMeshData);
            }

            return finishedChunks;
        }

        public static void GetSplitMeshDataWithClipping(MeshData meshToExtractFrom, BoundingBox extractionArea,
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

            // Process known intersecting triangles crossing the high X line
            MeshData newWorkingInsideIntersectingMeshDataHighX = new MeshData();
            foreach (TriangleFace triangleFace in intersectingMeshData.TriangleFaces)
                SplitTriangleByX(triangleFace, extractionArea.TopCorner.X, ref remainderMeshData, ref newWorkingInsideIntersectingMeshDataHighX, intersectingMeshData);
            remainderMeshData.CondenseAndRenumberVertexIndices();

            // Processing triangles crossing the low x line
            MeshData newWorkingInsideIntersectingMeshDataLowX = new MeshData();
            foreach (TriangleFace triangleFace in newWorkingInsideIntersectingMeshDataHighX.TriangleFaces)
                SplitTriangleByX(triangleFace, extractionArea.BottomCorner.X, ref newWorkingInsideIntersectingMeshDataLowX, ref remainderMeshData, newWorkingInsideIntersectingMeshDataHighX);
            remainderMeshData.CondenseAndRenumberVertexIndices();

            // Processing triangles crossing the high y line
            MeshData newWorkingInsideIntersectingMeshDataHighY = new MeshData();
            foreach (TriangleFace triangleFace in newWorkingInsideIntersectingMeshDataLowX.TriangleFaces)
                SplitTriangleByY(triangleFace, extractionArea.TopCorner.Y, ref remainderMeshData, ref newWorkingInsideIntersectingMeshDataHighY, newWorkingInsideIntersectingMeshDataLowX);
            remainderMeshData.CondenseAndRenumberVertexIndices();

            // Processing triangles crossing the low y line
            MeshData newWorkingInsideIntersectingMeshDataLowY = new MeshData();
            foreach (TriangleFace triangleFace in newWorkingInsideIntersectingMeshDataHighY.TriangleFaces)
                SplitTriangleByY(triangleFace, extractionArea.BottomCorner.Y, ref newWorkingInsideIntersectingMeshDataLowY, ref remainderMeshData, newWorkingInsideIntersectingMeshDataHighY);
            remainderMeshData.CondenseAndRenumberVertexIndices();

            // Processing triangles crossing the high z line
            MeshData newWorkingInsideIntersectingMeshDataHighZ = new MeshData();
            foreach (TriangleFace triangleFace in newWorkingInsideIntersectingMeshDataLowY.TriangleFaces)
                SplitTriangleByZ(triangleFace, extractionArea.TopCorner.Z, ref remainderMeshData, ref newWorkingInsideIntersectingMeshDataHighZ, newWorkingInsideIntersectingMeshDataLowY);
            remainderMeshData.CondenseAndRenumberVertexIndices();

            // Processing triangles crossing the low z line
            MeshData newWorkingInsideIntersectingMeshDataLowZ = new MeshData();
            foreach (TriangleFace triangleFace in newWorkingInsideIntersectingMeshDataHighZ.TriangleFaces)
                SplitTriangleByZ(triangleFace, extractionArea.BottomCorner.Z, ref newWorkingInsideIntersectingMeshDataLowZ, ref remainderMeshData, newWorkingInsideIntersectingMeshDataHighZ);
            remainderMeshData.CondenseAndRenumberVertexIndices();

            // Finalize the extracted data
            extractedMeshData.AddMeshData(newWorkingInsideIntersectingMeshDataLowZ);
            extractedMeshData.CondenseAndRenumberVertexIndices();
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

            MeshData newPositiveMeshData = new MeshData();
            MeshData newNegativeMeshData = new MeshData();

            // Starting clockwise, check for intersections and ensure all vertices are added in order
            // V1
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
            // V1->V2 Intersection Test
            if (v1.X >= xLine && v2.X < xLine || v1.X < xLine && v2.X >= xLine)
                CheckIntersectWithXPlaneAndGenerateVertData(v1, v2, n1, n2, t1, t2, c1, c2, xLine, ref newPositiveMeshData, ref newNegativeMeshData);
            // V2
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
            // V2->V3 Intersection Test
            if (v2.X >= xLine && v3.X < xLine || v2.X < xLine && v3.X >= xLine)
                CheckIntersectWithXPlaneAndGenerateVertData(v2, v3, n2, n3, t2, t3, c2, c3, xLine, ref newPositiveMeshData, ref newNegativeMeshData);
            // V3
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
            // V3->V1 Intersection Test
            if (v3.X >= xLine && v1.X < xLine || v3.X < xLine && v1.X >= xLine)
                CheckIntersectWithXPlaneAndGenerateVertData(v3, v1, n3, n1, t3, t1, c3, c1, xLine, ref newPositiveMeshData, ref newNegativeMeshData);

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

            MeshData newPositiveMeshData = new MeshData();
            MeshData newNegativeMeshData = new MeshData();

            // Starting clockwise, check for intersections and ensure all vertices are added in order
            // V1
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
            // V1->V2 Intersection Test
            if (v1.Y >= yLine && v2.Y < yLine || v1.Y < yLine && v2.Y >= yLine)
                CheckIntersectWithYPlaneAndGenerateVertData(v1, v2, n1, n2, t1, t2, c1, c2, yLine, ref newPositiveMeshData, ref newNegativeMeshData);         
            // V2
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
            // V2->V3 Intersection Test
            if (v2.Y >= yLine && v3.Y < yLine || v2.Y < yLine && v3.Y >= yLine)
                CheckIntersectWithYPlaneAndGenerateVertData(v2, v3, n2, n3, t2, t3, c2, c3, yLine, ref newPositiveMeshData, ref newNegativeMeshData);
            // V3
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
            // V3->V1 Intersection Test
            if (v3.Y >= yLine && v1.Y < yLine || v3.Y < yLine && v1.Y >= yLine)
                CheckIntersectWithYPlaneAndGenerateVertData(v3, v1, n3, n1, t3, t1, c3, c1, yLine, ref newPositiveMeshData, ref newNegativeMeshData);

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

        private static void SplitTriangleByZ(TriangleFace triangle, float zLine, ref MeshData positiveMeshData,
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

            MeshData newPositiveMeshData = new MeshData();
            MeshData newNegativeMeshData = new MeshData();

            // Starting clockwise, check for intersections and ensure all vertices are added in order
            // V1
            if (v1.Z >= zLine)
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
            // V1->V2 Intersection Test
            if (v1.Z >= zLine && v2.Z < zLine || v1.Z < zLine && v2.Z >= zLine)
                CheckIntersectWithZPlaneAndGenerateVertData(v1, v2, n1, n2, t1, t2, c1, c2, zLine, ref newPositiveMeshData, ref newNegativeMeshData);
            // V2
            if (v2.Z >= zLine)
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
            // V2->V3 Intersection Test
            if (v2.Z >= zLine && v3.Z < zLine || v2.Z < zLine && v3.Z >= zLine)
                CheckIntersectWithZPlaneAndGenerateVertData(v2, v3, n2, n3, t2, t3, c2, c3, zLine, ref newPositiveMeshData, ref newNegativeMeshData);
            // V3
            if (v3.Z >= zLine)
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
            // V3->V1 Intersection Test
            if (v3.Z >= zLine && v1.Z < zLine || v3.Z < zLine && v1.Z >= zLine)
                CheckIntersectWithZPlaneAndGenerateVertData(v3, v1, n3, n1, t3, t1, c3, c1, zLine, ref newPositiveMeshData, ref newNegativeMeshData);

            // Generate the triangles
            CreateTrianglesAndSaveData(triangle.MaterialIndex, newNegativeMeshData, ref negativeMeshData);
            CreateTrianglesAndSaveData(triangle.MaterialIndex, newPositiveMeshData, ref positiveMeshData);
        }

        private static void CheckIntersectWithZPlaneAndGenerateVertData(Vector3 v1, Vector3 v2, Vector3 n1, Vector3 n2, TextureCoordinates t1, TextureCoordinates t2,
            ColorRGBA c1, ColorRGBA c2, float zLine, ref MeshData positiveMeshData, ref MeshData negativeMeshData)
        {
            // Do nothing if the verts are on the same side
            if (v1.Z >= zLine && v2.Z >= zLine)
                return;
            if (v1.Z < zLine && v2.Z < zLine)
                return;

            // If intersection occurs, add the interpolated data to the lists
            float t = (zLine - v1.Z) / (v2.Z - v1.Z);

            // Vertex
            Vector3 vertexIntersection = new Vector3(v1.X + t * (v2.X - v1.X), v1.Y + t * (v2.Y - v1.Y), zLine);
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

            // If zero, then nothing to do
            if (newMeshData.Vertices.Count == 0)
                return;
            // If there are 3 verts, then it's one triangle in order
            if (newMeshData.Vertices.Count == 3)
                workingMeshData.TriangleFaces.Add(new TriangleFace(materialIndex, startIndex, startIndex + 1, startIndex + 2));
            // Otherwise, it's 4 and two triangles are to be spawned
            else if (newMeshData.Vertices.Count == 4)
            {
                workingMeshData.TriangleFaces.Add(new TriangleFace(materialIndex, startIndex, startIndex + 1, startIndex + 2));
                workingMeshData.TriangleFaces.Add(new TriangleFace(materialIndex, startIndex + 2, startIndex + 3, startIndex));
            }
            else
            {
                throw new Exception("MeshData CreateTrianglesAndSaveData had a vertices count of '" + newMeshData.Vertices.Count + "'");
            }
            workingMeshData.Vertices.AddRange(newMeshData.Vertices);
            workingMeshData.Normals.AddRange(newMeshData.Normals);
            workingMeshData.TextureCoordinates.AddRange(newMeshData.TextureCoordinates);
            workingMeshData.VertexColors.AddRange(newMeshData.VertexColors);
        }
    }
}
