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
        public ColorRGBA AmbientLight = new ColorRGBA();
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
            AmbientLight = new ColorRGBA(eqZoneData.AmbientLight.R, eqZoneData.AmbientLight.G, eqZoneData.AmbientLight.B, AmbientLight.A);
            LightInstances = eqZoneData.LightInstances; // TODO: Factor for scale

            MeshData meshData = new MeshData(eqZoneData.MeshData);
            meshData.ApplyEQToWoWGeometryTranslationsAndWorldScale();

            // Add object instances
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

            WorldObjects.Clear();

            // Build liquid wmos first
            GenerateLiquidWorldModelObjects(meshData, ZoneProperties);

            // Determine which materials are animated or transparent and create objects to represent them
            foreach (Material material in Materials)
                if ((material.IsAnimated() || material.HasTransparency()) && material.IsRenderable())
                {
                    MeshData allMeshData = new MeshData();
                    GenerateAndAddObjectInstancesForZoneMaterial(material, meshData);
                }

            // If this can be generated as a single WMO, just do that
            if (meshData.TriangleFaces.Count <= Configuration.CONFIG_WOW_MAX_FACES_PER_WMOGROUP)
            {
                List<string> materialNames = new List<string>();
                foreach(Material material in Materials)
                    materialNames.Add(material.UniqueName);
                GenerateWorldModelObjectByMaterials(materialNames, meshData.TriangleFaces, meshData);
            }
            // Otherwise, break into parts
            else
            {
                // Generate the world groups by splitting the map down into subregions as needed
                BoundingBox fullBoundingBox = BoundingBox.GenerateBoxFromVectors(meshData.Vertices, Configuration.CONFIG_EQTOWOW_ADDED_BOUNDARY_AMOUNT);
                List<string> materialNames = new List<string>();
                foreach (Material material in Materials)
                    materialNames.Add(material.UniqueName);
                GenerateWorldModelObjectsByXYRegion(fullBoundingBox, materialNames, meshData.TriangleFaces, meshData);
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
                        Logger.WriteError("Error setting loading screen, as the passed continent was not handled");
                    } break;
            }

            // Rebuild the bounding box
            BoundingBox = BoundingBox = BoundingBox.GenerateBoxFromVectors(meshData.Vertices, Configuration.CONFIG_EQTOWOW_ADDED_BOUNDARY_AMOUNT);
            IsLoaded = true;
        }

        public void GenerateLiquidWorldModelObjects(MeshData meshData, ZoneProperties zoneProperties)
        {
            // Volumes
            foreach (ZonePropertiesLiquidVolume liquidVolume in zoneProperties.LiquidVolumes)
            {
                WorldModelObject curWorldModelObject = new WorldModelObject();
                curWorldModelObject.LoadAsLiquidVolume(liquidVolume.LiquidType, liquidVolume.VolumeBox);
                WorldObjects.Add(curWorldModelObject);
            }

            // Planes
            foreach (ZonePropertiesLiquidPlane liquidPlane in zoneProperties.LiquidPlanes)
            {
                Material planeMaterial = new Material();
                bool materialFound = false;
                foreach (Material material in Materials)
                {
                    if (liquidPlane.MaterialName == material.Name)
                    {
                        planeMaterial = material;
                        materialFound = true;
                        break;
                    }
                }
                if (materialFound == false)
                {
                    Logger.WriteError("In generating liquidplane for wmo '" + ShortName + "', unable to find material named '" + liquidPlane.MaterialName + "'");
                    if (Materials.Count > 0)
                        planeMaterial = new Material(Materials[0]);
                }

                // Create the object, constraining to max size if needed
                if (liquidPlane.BoundingBox.GetYDistance() >= Configuration.CONFIG_EQTOWOW_LIQUID_SURFACE_MAX_XY_DIMENSION ||
                    liquidPlane.BoundingBox.GetXDistance() >= Configuration.CONFIG_EQTOWOW_LIQUID_SURFACE_MAX_XY_DIMENSION)
                {
                    List<ZonePropertiesLiquidPlane> liquidPlaneChunks = liquidPlane.SplitIntoSizeRestictedChunks(Configuration.CONFIG_EQTOWOW_LIQUID_SURFACE_MAX_XY_DIMENSION);
                    foreach (ZonePropertiesLiquidPlane curLiquidPlane in liquidPlaneChunks)
                    {
                        WorldModelObject curWorldModelObject = new WorldModelObject();
                        curWorldModelObject.LoadAsLiquidPlane(curLiquidPlane.LiquidType, curLiquidPlane.PlaneAxisAlignedXY, planeMaterial, curLiquidPlane.BoundingBox);
                        WorldObjects.Add(curWorldModelObject);
                    }
                }
                else
                {
                    WorldModelObject curWorldModelObject = new WorldModelObject();
                    curWorldModelObject.LoadAsLiquidPlane(liquidPlane.LiquidType, liquidPlane.PlaneAxisAlignedXY, planeMaterial, liquidPlane.BoundingBox);
                    WorldObjects.Add(curWorldModelObject);
                }
            }
        }

        private void GenerateWorldModelObjectsByXYRegion(BoundingBox boundingBox, List<string> materialNames, List<TriangleFace> faces, MeshData meshData)
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
                    Vector3 v1 = meshData.Vertices[triangle.V1];
                    Vector3 v2 = meshData.Vertices[triangle.V2];
                    Vector3 v3 = meshData.Vertices[triangle.V3];
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
                GenerateWorldModelObjectsByXYRegion(splitBox.BoxA, materialNames, aBoxTriangles, meshData);
                GenerateWorldModelObjectsByXYRegion(splitBox.BoxB, materialNames, bBoxTriangles, meshData);
            }
            else
            {
                GenerateWorldModelObjectByMaterials(materialNames, faces, meshData);
            }
        }

         private void GenerateWorldModelObjectByMaterials(List<string> materialNames, List<TriangleFace> faceToProcess, MeshData meshData)
        {
            List<UInt32> materialIDs = new List<UInt32>();
            bool materialFound = false;

            // Get the related materials
            foreach (string materialName in materialNames)
            {
                foreach (Material material in Materials)
                {
                    if (material.UniqueName == materialName)
                    {
                        materialIDs.Add(material.Index);
                        materialFound = true;
                        break;
                    }
                }
                if (materialFound == false)
                {
                    Logger.WriteError("Error generating world model object, as material named '" + materialName +"' could not be found");
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
                for (int i = 0; i < faceToProcess.Count; i++)
                {
                    // Skip anything not matching the material
                    if (materialIDs.Contains(Convert.ToUInt32(faceToProcess[i].MaterialIndex)) == false)
                        continue;

                    // Save it
                    facesInGroup.Add(faceToProcess[i]);
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
                    faceToProcess.RemoveAt(faceIndex);

                // Generate a world model object if there are any verticies
                MeshData extractedMeshData = meshData.GetMeshDataForFaces(facesInGroup);
                if (extractedMeshData.Vertices.Count > 0)
                {
                    WorldModelObject curWorldModelObject = new WorldModelObject();
                    curWorldModelObject.LoadAsRendered(extractedMeshData, Materials, DoodadInstances, ZoneProperties);
                    WorldObjects.Add(curWorldModelObject);
                }
            }
        }

        private void GenerateAndAddObjectInstancesForZoneMaterial(Material material, MeshData allMeshData)
        {
            List<Vector3> meshPositions = new List<Vector3>();
            List<MeshData> meshDatas = new List<MeshData>();

            // Grab only this material
            MeshData curMaterialMeshData = allMeshData.GetMeshDataForMaterials(material);

            // Generate a bounding box for the mesh data
            BoundingBox curMeshBoundingBox = BoundingBox.GenerateBoxFromVectors(curMaterialMeshData.Vertices, 0.0f);

            // Split the zone into chunks if it passes a threashold
            curMaterialMeshData.SplitIntoChunks(curMaterialMeshData, curMeshBoundingBox, curMaterialMeshData.TriangleFaces, 
                material, ref meshPositions, ref meshDatas);

            // Create the objects
            for (int i = 0; i < meshDatas.Count; i++)
            {
                Vector3 curPosition = meshPositions[i];
                MeshData curMeshData = meshDatas[i];

                List<Vector3> collisionVerticies = new List<Vector3>();

                // Generate the object
                string name = "ZO_" + ShortName + "_" + material.UniqueName + "_" + i.ToString();
                WOWObjectModelData newObject = new WOWObjectModelData();
                newObject.Load(name, new List<Material> { new Material(material) }, curMeshData, new List<Vector3>(), new List<TriangleFace>(), false);
                GeneratedZoneObjects.Add(newObject);

                // Add as a doodad
                WorldModelObjectDoodadInstance doodadInstance = new WorldModelObjectDoodadInstance();
                doodadInstance.ObjectName = name;
                doodadInstance.Position = curPosition;
                DoodadInstances.Add(doodadInstance);
            }
        }
    }
}
