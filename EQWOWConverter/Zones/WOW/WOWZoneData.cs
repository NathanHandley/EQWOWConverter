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
using EQWOWConverter.Objects;
using EQWOWConverter.Zones.WOW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Zones
{
    internal class WOWZoneData
    {
        private string ShortName = string.Empty;
        private bool IsLoaded = false;
        private static UInt32 CURRENT_WMOID = Configuration.CONFIG_DBCID_WMOID_START;
        private static UInt32 CURRENT_AREAID = Configuration.CONFIG_DBCID_AREAID_START;
        private static int CURRENT_MAPID = Configuration.CONFIG_DBCID_MAPID_START;

        public List<WorldModelObject> WorldObjects = new List<WorldModelObject>();
        public List<WOWObjectModelData> GeneratedZoneObjects = new List<WOWObjectModelData>();
        public List<Material> Materials = new List<Material>();
        public ColorARGB AmbientLight = new ColorARGB();
        public List<LightInstance> LightInstances = new List<LightInstance>();
        public List<WorldModelObjectDoodadInstance> DoodadInstances = new List<WorldModelObjectDoodadInstance>();
        public BoundingBox BoundingBox = new BoundingBox();
        public Fog FogSettings = new Fog();
        public UInt32 AreaID;
        public UInt32 WMOID;
        public int MapID;
        public int LoadingScreenID;
        public ZoneProperties ZoneProperties;

        public Vector3 SafePosition = new Vector3();

        public WOWZoneData(ZoneProperties zoneProperties)
        {
            // Gen/Update IDs
            AreaID = CURRENT_AREAID;
            CURRENT_AREAID++;
            WMOID = CURRENT_WMOID;
            CURRENT_WMOID++;
            MapID = CURRENT_MAPID;
            CURRENT_MAPID++;
            ZoneProperties = zoneProperties;
        }

        public void LoadFromEQZone(EQZoneData eqZoneData)
        {
            if (IsLoaded == true)
                return;
            ShortName = ZoneProperties.ShortName;
            Materials = eqZoneData.Materials;
            AmbientLight = new ColorARGB(eqZoneData.AmbientLight.A, eqZoneData.AmbientLight.R, eqZoneData.AmbientLight.G, AmbientLight.B);
            LightInstances = eqZoneData.LightInstances; // TODO: Factor for scale

            // Change face orientation for culling differences between EQ and WoW
            List<TriangleFace> triangleFaces = new List<TriangleFace>();
            foreach (TriangleFace eqFace in eqZoneData.TriangleFaces)
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
            List<TextureCoordinates> textureCoords = new List<TextureCoordinates>();
            foreach (TextureCoordinates uv in eqZoneData.TextureCoords)
            {
                TextureCoordinates curTextureCoords = new TextureCoordinates(uv.X, uv.Y * -1);
                textureCoords.Add(curTextureCoords);
            }

            // Adjust verticies for world scale and rotate around the Z axis 180 degrees
            List<Vector3> verticies = new List<Vector3>();
            foreach (Vector3 vertex in eqZoneData.Verticies)
            {
                vertex.X *= Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
                vertex.X = -vertex.X;
                vertex.Y *= Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
                vertex.Y = -vertex.Y;
                vertex.Z *= Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
                verticies.Add(vertex);
            }

            // Add object instances
            if (ZoneProperties.ShortName.Length > 0 && Configuration.CONFIG_DISABLE_OBJECT_INSTANCES_BY_ZONE_SHORTNAMES.Contains(ZoneProperties.ShortName) == false)
            {
                foreach (ObjectInstance objectInstance in eqZoneData.ObjectInstances)
                {
                    WorldModelObjectDoodadInstance doodadInstance = new WorldModelObjectDoodadInstance();
                    doodadInstance.ObjectName = objectInstance.ModelName;
                    doodadInstance.Position.X = objectInstance.Position.X * Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
                    // Invert Z and Y because of mapping differences
                    doodadInstance.Position.Z = objectInstance.Position.Y * Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
                    doodadInstance.Position.Y = objectInstance.Position.Z * Configuration.CONFIG_EQTOWOW_WORLD_SCALE;

                    // Also rotate the X and Y positions around Z axis 180 degrees
                    doodadInstance.Position.X = -doodadInstance.Position.X;
                    doodadInstance.Position.Y = -doodadInstance.Position.Y;

                    // Calculate the rotation
                    float rotateYaw = Convert.ToSingle(Math.PI / 180) * -objectInstance.Rotation.Z;
                    float rotatePitch = Convert.ToSingle(Math.PI / 180) * objectInstance.Rotation.X;
                    float rotateRoll = Convert.ToSingle(Math.PI / 180) * objectInstance.Rotation.Y;
                    System.Numerics.Quaternion rotationQ = System.Numerics.Quaternion.CreateFromYawPitchRoll(rotateYaw, rotatePitch, rotateRoll);
                    doodadInstance.Orientation.X = rotationQ.X;
                    doodadInstance.Orientation.Y = rotationQ.Y;
                    doodadInstance.Orientation.Z = rotationQ.Z;
                    doodadInstance.Orientation.W = -rotationQ.W; // Flip the sign for handedness

                    // Scale is confirmed to always have the same value in x, y, z
                    doodadInstance.Scale = objectInstance.Scale.X;

                    // Add it
                    DoodadInstances.Add(doodadInstance);
                }
            }

            // Generate world objects
            List<Vector3> normals = eqZoneData.Normals;
            List<ColorRGBA> vertexColors = eqZoneData.VertexColors;

            WorldObjects.Clear();

            // Build liquid wmos first
            foreach (ZonePropertiesLiquidVolume liquidVolume in ZoneProperties.LiquidVolumes)
            {
                // Generate and add the world model object
                WorldModelObject curWorldModelObject = new WorldModelObject(liquidVolume.VolumeBox, WorldModelObjectType.LiquidVolume);
                WorldObjects.Add(curWorldModelObject);
            }

            // Determine which materials are animated and create objects to represent them
            foreach(Material material in Materials)
                if (material.IsAnimated())
                    GenerateAndAddObjectInstanceForZoneMaterial(material, triangleFaces, verticies, normals, vertexColors, textureCoords);

            // If this can be generated as a single WMO, just do that
            if (triangleFaces.Count <= Configuration.CONFIG_WOW_MAX_FACES_PER_WMOGROUP)
            {
                List<string> materialNames = new List<string>();
                foreach(Material material in Materials)
                    materialNames.Add(material.Name);
                GenerateWorldModelObjectByMaterials(materialNames, triangleFaces, verticies, normals, vertexColors, textureCoords);
            }
            // Otherwise, break into parts
            else
            {
                // Generate the world groups by splitting the map down into subregions as needed
                BoundingBox fullBoundingBox = BoundingBox.GenerateBoxFromVectors(verticies, Configuration.CONFIG_EQTOWOW_ADDED_BOUNDARY_AMOUNT);
                List<string> materialNames = new List<string>();
                foreach (Material material in Materials)
                    materialNames.Add(material.Name);
                GenerateWorldModelObjectsByXYRegion(fullBoundingBox, materialNames, triangleFaces, verticies, normals, vertexColors, textureCoords);
            }

            // Save the loading screen
            switch (ZoneProperties.Continent)
            {
                case ZoneContinent.Antonica:
                case ZoneContinent.Faydwer:
                case ZoneContinent.Development:
                case ZoneContinent.Odus:
                    {
                        LoadingScreenID = Configuration.CONFIG_DBCID_LOADINGSCREENID_START;
                    } break;
                case ZoneContinent.Kunark:
                    {
                        LoadingScreenID = Configuration.CONFIG_DBCID_LOADINGSCREENID_START + 1;
                    }
                    break;
                case ZoneContinent.Velious:
                    {
                        LoadingScreenID = Configuration.CONFIG_DBCID_LOADINGSCREENID_START + 2;
                    }
                    break;                
                default:
                    {
                        Logger.WriteLine("Error setting loading screen, as the passed continent was not handled");
                    } break;
            }

            // Rebuild the bounding box
            BoundingBox = BoundingBox = BoundingBox.GenerateBoxFromVectors(verticies, Configuration.CONFIG_EQTOWOW_ADDED_BOUNDARY_AMOUNT);
            IsLoaded = true;
        }

        private void GenerateWorldModelObjectsByXYRegion(BoundingBox boundingBox, List<string> materialNames, List<TriangleFace> faces, List<Vector3> verticies, List<Vector3> normals,
            List<ColorRGBA> vertexColors, List<TextureCoordinates> textureCoords)
        {
            // If there are too many triangles to fit in a single box, cut the box into two and generate two child world model objects
            if (faces.Count > Configuration.CONFIG_WOW_MAX_FACES_PER_WMOGROUP)
            {
                // Create two new bounding boxes
                SplitBox splitBox = SplitBox.GenerateXYSplitBoxFromBoundingBox(boundingBox);

                // Calculate what triangles fit into these boxes
                List<TriangleFace> aBoxTriangles = new List<TriangleFace>();
                List<TriangleFace> bBoxTriangles = new List<TriangleFace>();

                foreach (TriangleFace triangle in faces)
                {
                    // Get center point
                    Vector3 v1 = verticies[triangle.V1];
                    Vector3 v2 = verticies[triangle.V2];
                    Vector3 v3 = verticies[triangle.V3];
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
                GenerateWorldModelObjectsByXYRegion(splitBox.BoxA, materialNames, aBoxTriangles, verticies, normals, vertexColors, textureCoords);
                GenerateWorldModelObjectsByXYRegion(splitBox.BoxB, materialNames, bBoxTriangles, verticies, normals, vertexColors, textureCoords);
            }
            else
                GenerateWorldModelObjectByMaterials(materialNames, faces, verticies, normals, vertexColors, textureCoords);
        }

         private void GenerateWorldModelObjectByMaterials(List<string> materialNames, List<TriangleFace> triangleFaces, List<Vector3> verticies, List<Vector3> normals,
            List<ColorRGBA> vertexColors, List<TextureCoordinates> textureCoords)
        {
            List<UInt32> materialIDs = new List<UInt32>();
            bool materialFound = false;

            // Get the related materials
            foreach (string materialName in materialNames)
            {
                foreach (Material material in Materials)
                {
                    if (material.Name == materialName)
                    {
                        materialIDs.Add(material.Index);
                        materialFound = true;
                        break;
                    }
                }
                if (materialFound == false)
                {
                    Logger.WriteLine("Error generating world model object, as material named '" + materialName +"' could not be found");
                    return;
                }
            }

            // Build a list of faces specific to these materials, controlling for overflow
            bool facesLeftToProcess = true;
            while (facesLeftToProcess)
            {
                facesLeftToProcess = false;
                List<TriangleFace> facesInGroup = new List<TriangleFace>();
                SortedSet<int> faceIndexesToDelete = new SortedSet<int>();
                for (int i = 0; i < triangleFaces.Count; i++)
                {
                    // Skip anything not matching the material
                    if (materialIDs.Contains(Convert.ToUInt32(triangleFaces[i].MaterialIndex)) == false)
                        continue;

                    // Save it
                    facesInGroup.Add(triangleFaces[i]);
                    faceIndexesToDelete.Add(i);

                    // Only go up to a maximum to avoid overflowing the model arrays
                    if (facesInGroup.Count >= Configuration.CONFIG_WOW_MAX_FACES_PER_WMOGROUP)
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

        private void GenerateAndAddObjectInstanceForZoneMaterial(Material material, List<TriangleFace> allTriangleFaces, List<Vector3> allVerticies, 
            List<Vector3> allNormals, List<ColorRGBA> allVertexColors, List<TextureCoordinates> allTextureCoordinates)
        {
            // Extract out copies of the geometry data specific to this material
            List<TriangleFace> modelTriangleFaces = new List<TriangleFace>();
            List<Vector3> modelVerticies = new List<Vector3>();
            List<Vector3> modelNormals = new List<Vector3>();
            List<ColorRGBA> modelVertexColors = new List<ColorRGBA>();
            List<TextureCoordinates> modelTextureCoordinates = new List<TextureCoordinates>();
            Dictionary<int, int> oldNewVertexIndicies = new Dictionary<int, int>();
            for (int i = 0; i < allTriangleFaces.Count; i++)
            {
                // Skip faces not matching the material
                if (allTriangleFaces[i].MaterialIndex != material.Index)
                    continue;

                // Make sure to reset the material ID to 0 since these objects are single material based
                TriangleFace curTriangleFace = new TriangleFace(allTriangleFaces[i]);
                curTriangleFace.MaterialIndex = 0;

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
                    int newVertIndex = modelVerticies.Count;
                    oldNewVertexIndicies.Add(oldVertIndex, newVertIndex);
                    curTriangleFace.V1 = newVertIndex;

                    // Add objects
                    modelVerticies.Add(allVerticies[oldVertIndex]);
                    modelTextureCoordinates.Add(allTextureCoordinates[oldVertIndex]);
                    modelNormals.Add(allNormals[oldVertIndex]);
                    if (allVertexColors.Count != 0)
                        modelVertexColors.Add(allVertexColors[oldVertIndex]);
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
                    int newVertIndex = modelVerticies.Count;
                    oldNewVertexIndicies.Add(oldVertIndex, newVertIndex);
                    curTriangleFace.V2 = newVertIndex;

                    // Add objects
                    modelVerticies.Add(allVerticies[oldVertIndex]);
                    modelTextureCoordinates.Add(allTextureCoordinates[oldVertIndex]);
                    modelNormals.Add(allNormals[oldVertIndex]);
                    if (allVertexColors.Count != 0)
                        modelVertexColors.Add(allVertexColors[oldVertIndex]);
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
                    int newVertIndex = modelVerticies.Count;
                    oldNewVertexIndicies.Add(oldVertIndex, newVertIndex);
                    curTriangleFace.V3 = newVertIndex;

                    // Add objects
                    modelVerticies.Add(allVerticies[oldVertIndex]);
                    modelTextureCoordinates.Add(allTextureCoordinates[oldVertIndex]);
                    modelNormals.Add(allNormals[oldVertIndex]);
                    if (allVertexColors.Count != 0)
                        modelVertexColors.Add(allVertexColors[oldVertIndex]);
                }

                // Save this updated triangle
                modelTriangleFaces.Add(curTriangleFace);
            }

            // Generate the object
            string name = "ZO_" + ShortName + "_" + material.Name;
            WOWObjectModelData newObject = new WOWObjectModelData();
            newObject.LoadFromZoneData(name, material, modelTriangleFaces, modelVerticies, modelNormals, modelVertexColors, modelTextureCoordinates);
            GeneratedZoneObjects.Add(newObject);

            // Add as a doodad
            WorldModelObjectDoodadInstance doodadInstance = new WorldModelObjectDoodadInstance();
            doodadInstance.ObjectName = name;
            doodadInstance.Position = new Vector3(0, 0, 0);
            DoodadInstances.Add(doodadInstance);
        }

        private void GenerateWorldModelObjectFromFaces(List<TriangleFace> faces, List<Vector3> verticies, List<Vector3> normals,
            List<ColorRGBA> vertexColors, List<TextureCoordinates> textureCoords)
        {
            // Since the face list is likely to not include all faces, rebuild the render object lists
            List<Vector3> condensedVerticies = new List<Vector3>();
            List<Vector3> condensedNormals = new List<Vector3>();
            List<ColorRGBA> condensedVertexColors = new List<ColorRGBA>();
            List<TextureCoordinates> condensedTextureCoords = new List<TextureCoordinates>();
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
                condensedNormals, condensedVertexColors, remappedTriangleFaces, Materials, DoodadInstances, ZoneProperties);
            WorldObjects.Add(curWorldModelObject);
        }
    }
}
