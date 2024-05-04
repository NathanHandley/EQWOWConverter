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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Zones
{
    internal class WOWZoneData
    {
        private static UInt32 CURRENT_WMOID = 7000; // Reserving 7000-7122
        private static UInt32 CURRENT_AREAID = 6000; // Reserving 6000-6122
        private static int CURRENT_MAPID = 750;  // Reserving 750-872

        public List<WorldModelObject> WorldObjects = new List<WorldModelObject>();
        public List<Material> Materials = new List<Material>();
        public ColorRGBA AmbientLight = new ColorRGBA();
        public List<LightInstance> LightInstances = new List<LightInstance>();
        public AxisAlignedBox BoundingBox = new AxisAlignedBox();
        public Fog FogSettings = new Fog();
        public List<string> TextureNames = new List<string>();
        public UInt32 AreaID;
        public UInt32 WMOID;
        public int MapID;

        public WOWZoneData()
        {
            // Gen/Update IDs
            AreaID = CURRENT_AREAID;
            CURRENT_AREAID++;
            WMOID = CURRENT_WMOID;
            CURRENT_WMOID++;
            MapID = CURRENT_MAPID;
            CURRENT_MAPID++;
        }
        
        // TODO: Delete texturesToGroupIsolate
        public void LoadFromEQZone(EQZoneData eqZoneData, List<string> texturesToGroupIsolate)
        {
            Materials = eqZoneData.Materials;
            AmbientLight = eqZoneData.AmbientLight;
            LightInstances = eqZoneData.LightInstances;

            // Create a list of the textures
            // NOTE: Only the first texture in an material is captured
            // TODO: Handle animated textures
            foreach (Material material in Materials)
            {
                if (material.AnimationTextures.Count > 0)
                    TextureNames.Add(material.AnimationTextures[0]);
            }

            // Change face orientation for culling differences between EQ and WoW
            List<TriangleFace> triangleFaces = new List<TriangleFace>();
            foreach(TriangleFace eqFace in eqZoneData.TriangleFaces)
            {
                TriangleFace newFace = new TriangleFace();
                newFace.MaterialIndex = eqFace.MaterialIndex;

                // Rotate the verticies for culling differences
                newFace.V1 = eqFace.V3;
                newFace.V2 = eqFace.V2;
                newFace.V3 = eqFace.V1;               

                // Add it
                triangleFaces.Add(newFace);
            }

            // Change texture mapping differences between EQ and WoW
            List<TextureUv> textureCoords = new List<TextureUv>();
            foreach (TextureUv uv in eqZoneData.TextureCoords)
            {
                TextureUv curTextureCoords = new TextureUv(uv.X, uv.Y * -1);
                textureCoords.Add(curTextureCoords);
            }

            // Create world model objects by identifying connected triangles and grouping them
            List<Vector3> verticies = eqZoneData.Verticies;
            List<Vector3> normals = eqZoneData.Normals;
            List<ColorRGBA> vertexColors = eqZoneData.VertexColors;

            // Extract out isolated texture groups first, creating world objects
            WorldObjects.Clear();
            foreach (string texture in TextureNames)
            {
                Int32 materialID = -2;
                // Get the related material
                foreach (Material material in Materials)
                {
                    if (material.AnimationTextures.Count > 0 && material.AnimationTextures[0].ToUpper() == texture.ToUpper())
                    {
                        materialID = Convert.ToInt32(material.Index);
                        break;
                    }
                }
                if (materialID == -2)
                {
                    Logger.WriteLine(" ERROR! Texture name '" + texture + " 'passed to isolate did not exist in the zone");
                    continue;
                }

                // Build a list of faces specific to this material, controlling for overflow
                bool facesLeftToProcess = true;
                while (facesLeftToProcess)
                {
                    facesLeftToProcess = false;
                    List<TriangleFace> facesInGroup = new List<TriangleFace>();
                    SortedSet<int> faceIndexesToDelete = new SortedSet<int>();
                    for (int i = 0; i < triangleFaces.Count; i++)
                    {
                        // Skip anything not matching the material
                        if (triangleFaces[i].MaterialIndex != materialID)
                            continue;

                        // Save it
                        facesInGroup.Add(triangleFaces[i]);
                        faceIndexesToDelete.Add(i);

                        // Only go up to a maximum to avoid overflowing the model arrays
                        if (facesInGroup.Count >= 60000)
                        {
                            facesLeftToProcess = true;
                            break;
                        }
                    }

                    // Purge the faces from the original list
                    foreach (int faceIndex in faceIndexesToDelete.Reverse())
                        triangleFaces.RemoveAt(faceIndex);

                    // Generate the world model object
                    GenerateWorldModelObjectFromFaces(facesInGroup, verticies, normals, vertexColors, textureCoords);
                }
            }

            // Rebuild the bounding box
            CalculateBoundingBox();
        }

        private void GenerateWorldModelObjectFromFaces(List<TriangleFace> faces, List<Vector3> verticies, List<Vector3> normals,
            List<ColorRGBA> vertexColors, List<TextureUv> textureCoords)
        {
            // Since the face list is likely to not include all faces, rebuild the render object lists
            List<Vector3> condensedVerticies = new List<Vector3>();
            List<Vector3> condensedNormals = new List<Vector3>();
            List<ColorRGBA> condensedVertexColors = new List<ColorRGBA>();
            List<TextureUv> condensedTextureCoords = new List<TextureUv>();
            List<TriangleFace> remappedTriangleFaces = new List<TriangleFace>();
            Dictionary<int, int> oldNewVertexIndicies = new Dictionary<int, int>();
            for (int i = 0; i < faces.Count; i++)
            {
                TriangleFace curTriangleFace = faces[i];

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
                    int newVertIndex = condensedVerticies.Count;
                    oldNewVertexIndicies.Add(oldVertIndex, newVertIndex);
                    curTriangleFace.V1 = newVertIndex;

                    // Add objects
                    condensedVerticies.Add(verticies[oldVertIndex]);
                    condensedTextureCoords.Add(textureCoords[oldVertIndex]);
                    condensedNormals.Add(normals[oldVertIndex]);
                    if (vertexColors.Count != 0)
                        condensedVertexColors.Add(vertexColors[oldVertIndex]);
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
                    int newVertIndex = condensedVerticies.Count;
                    oldNewVertexIndicies.Add(oldVertIndex, newVertIndex);
                    curTriangleFace.V2 = newVertIndex;

                    // Add objects
                    condensedVerticies.Add(verticies[oldVertIndex]);
                    condensedTextureCoords.Add(textureCoords[oldVertIndex]);
                    condensedNormals.Add(normals[oldVertIndex]);
                    if (vertexColors.Count != 0)
                        condensedVertexColors.Add(vertexColors[oldVertIndex]);
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
                    int newVertIndex = condensedVerticies.Count;
                    oldNewVertexIndicies.Add(oldVertIndex, newVertIndex);
                    curTriangleFace.V3 = newVertIndex;

                    // Add objects
                    condensedVerticies.Add(verticies[oldVertIndex]);
                    condensedTextureCoords.Add(textureCoords[oldVertIndex]);
                    condensedNormals.Add(normals[oldVertIndex]);
                    if (vertexColors.Count != 0)
                        condensedVertexColors.Add(vertexColors[oldVertIndex]);
                }

                // Save this updated triangle
                remappedTriangleFaces.Add(curTriangleFace);
            }

            // Generate and add the world model object
            WorldModelObject curWorldModelObject = new WorldModelObject(condensedVerticies, condensedTextureCoords, 
                condensedNormals, condensedVertexColors, remappedTriangleFaces, Materials);
            WorldObjects.Add(curWorldModelObject);
        }

        private void CalculateBoundingBox()
        {
            // Calculate it by using the bounding box of all WorldModelObjects
            BoundingBox = new AxisAlignedBox();
            foreach(WorldModelObject worldModelObject in WorldObjects)
            {
                if (worldModelObject.BoundingBox.TopCorner.X > BoundingBox.TopCorner.X)
                    BoundingBox.TopCorner.X = worldModelObject.BoundingBox.TopCorner.X;
                if (worldModelObject.BoundingBox.TopCorner.Y > BoundingBox.TopCorner.Y)
                    BoundingBox.TopCorner.Y = worldModelObject.BoundingBox.TopCorner.Y;
                if (worldModelObject.BoundingBox.TopCorner.Z > BoundingBox.TopCorner.Z)
                    BoundingBox.TopCorner.Z = worldModelObject.BoundingBox.TopCorner.Z;

                if (worldModelObject.BoundingBox.BottomCorner.X < BoundingBox.BottomCorner.X)
                    BoundingBox.BottomCorner.X = worldModelObject.BoundingBox.BottomCorner.X;
                if (worldModelObject.BoundingBox.BottomCorner.Y < BoundingBox.BottomCorner.Y)
                    BoundingBox.BottomCorner.Y = worldModelObject.BoundingBox.BottomCorner.Y;
                if (worldModelObject.BoundingBox.BottomCorner.Z < BoundingBox.BottomCorner.Z)
                    BoundingBox.BottomCorner.Z = worldModelObject.BoundingBox.BottomCorner.Z;
            }
        }
    }
}
