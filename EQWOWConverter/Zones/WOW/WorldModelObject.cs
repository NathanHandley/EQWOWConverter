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
using EQWOWConverter.Zones.WOW;
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
        public WorldModelObjectType WMOType = WorldModelObjectType.Rendered;
        public List<Material> Materials = new List<Material>();
        public List<Vector3> Verticies = new List<Vector3>();
        public List<TextureCoordinates> TextureCoords = new List<TextureCoordinates>();
        public List<Vector3> Normals = new List<Vector3>();
        public List<ColorBGRA> VertexColors = new List<ColorBGRA>();
        public List<TriangleFace> TriangleFaces = new List<TriangleFace>();
        public List<WorldModelRenderBatch> RenderBatches = new List<WorldModelRenderBatch>();
        public Dictionary<int, WorldModelObjectDoodadInstance> DoodadInstances = new Dictionary<int, WorldModelObjectDoodadInstance>();
        public BoundingBox BoundingBox = new BoundingBox();
        public BSPTree BSPTree;

        public WorldModelObject(BoundingBox boundingBox, WorldModelObjectType wmoType)
        {
            WMOType = wmoType;
            BoundingBox = boundingBox;
            BSPTree = new BSPTree(boundingBox, new List<UInt32>());
        }

        public WorldModelObject(List<Vector3> verticies, List<TextureCoordinates> textureCoords, List<Vector3> normals, List<ColorRGBA> vertexColors, 
            List<TriangleFace> triangleFaces, List<Material> materials, List<WorldModelObjectDoodadInstance> zoneWideDoodadInstances,
            ZoneProperties zoneProperties)
        {
            Verticies = verticies;
            TextureCoords = textureCoords;
            Materials = materials;
            Normals = normals;
            foreach(var vertexColor in vertexColors)
                VertexColors.Add(new ColorBGRA(vertexColor.B, vertexColor.G, vertexColor.R, vertexColor.A));
            TriangleFaces = triangleFaces;
            BoundingBox = BoundingBox.GenerateBoxFromVectors(Verticies, Configuration.CONFIG_EQTOWOW_ADDED_BOUNDARY_AMOUNT);
            List<UInt32> collisionTriangleIndicies;
            GenerateRenderBatches(materials, zoneProperties, out collisionTriangleIndicies);
            WMOGroupID = CURRENT_WMOGROUPID;
            CURRENT_WMOGROUPID++;
            BSPTree = new BSPTree(BoundingBox, collisionTriangleIndicies);
            CreateDoodadAssociations(zoneWideDoodadInstances);
        }

        private void GenerateRenderBatches(List<Material> materials, ZoneProperties zoneProperties, out List<UInt32> collisionTriangleIncidies)
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
                if (materials[curMaterialIndex].IsRenderable() == false)
                    continue;
                if (materials[curMaterialIndex].IsAnimated() == true)
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

            // Construct the collision triangle indicies
            collisionTriangleIncidies = new List<UInt32>();
            for (int i = 0; i < TriangleFaces.Count; ++i)
            {
                Material curMaterial = materials[TriangleFaces[i].MaterialIndex];
                if (zoneProperties.NonCollisionMaterialNames.Contains(curMaterial.Name) == false)
                    collisionTriangleIncidies.Add(Convert.ToUInt32(i));
            }

            // Store the new render batches
            RenderBatches.Clear();
            foreach (var renderBatch in renderBatchesByMaterialID)
            {
                renderBatch.Value.BoundingBox = new BoundingBox(BoundingBox);
                RenderBatches.Add(renderBatch.Value);
            }
        }

        private void CreateDoodadAssociations(List<WorldModelObjectDoodadInstance> zoneWidedoodadInstances)
        {
            // Associate any doodads that have a position inside of this wmo bounding box
            for (int i = 0; i < zoneWidedoodadInstances.Count; i++)
            {
                WorldModelObjectDoodadInstance doodadInstance = zoneWidedoodadInstances[i];
                if (BoundingBox.IsPointInside(doodadInstance.Position))
                    DoodadInstances.Add(i, doodadInstance);
            }
        }

        private void SortRenderObjects()
        {
            TriangleFaces.Sort();

            // Reorder the verticies / texcoords / normals / vertcolors to match the sorted triangle faces
            List<Vector3> sortedVerticies = new List<Vector3>();
            List<TextureCoordinates> sortedTextureCoords = new List<TextureCoordinates>();
            List<Vector3> sortedNormals = new List<Vector3>();
            List<ColorBGRA> sortedVertexColors = new List<ColorBGRA>();
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
    }
}
