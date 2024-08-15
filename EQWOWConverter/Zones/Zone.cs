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
using EQWOWConverter.ObjectModels;
using EQWOWConverter.ObjectModels.Properties;
using EQWOWConverter.Zones.WOW;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EQWOWConverter.Zones
{
    internal class Zone
    {
        public string ShortName = string.Empty;
        public string DescriptiveName = string.Empty;        
        public string DescriptiveNameOnlyLetters = string.Empty;
        public ZoneEQData EQZoneData = new ZoneEQData();
        private bool IsLoaded = false;
        public List<ZoneObjectModel> ZoneObjectModels = new List<ZoneObjectModel>();
        public List<ObjectModel> GeneratedZoneObjects = new List<ObjectModel>();
        public List<Material> Materials = new List<Material>();
        public List<LightInstance> LightInstances = new List<LightInstance>();
        public List<ZoneDoodadInstance> DoodadInstances = new List<ZoneDoodadInstance>();
        public BoundingBox BoundingBox = new BoundingBox();
        public int LoadingScreenID;
        public ZoneProperties ZoneProperties;
        public Vector3 SafePosition = new Vector3();
        public Dictionary<int, Sound> ZoneMusicSoundsByIndex = new Dictionary<int, Sound>();

        public Zone(string shortName, ZoneProperties zoneProperties)
        {
            ZoneProperties = zoneProperties;
            ShortName = shortName;
            if (zoneProperties.DescriptiveName != string.Empty)
                SetDescriptiveName(zoneProperties.DescriptiveName);
            else
                DescriptiveNameOnlyLetters = shortName;
        }

        public void LoadEQZoneData(string inputZoneFolderName, string inputZoneFolderFullPath)
        {
            // Load
            EQZoneData.LoadDataFromDisk(inputZoneFolderName, inputZoneFolderFullPath);
        } 

        public void LoadFromEQZone()
        {
            if (IsLoaded == true)
                return;
            ShortName = ZoneProperties.ShortName;
            Materials = EQZoneData.Materials;

            // Update the materials
            foreach (Material material in Materials)
                if (ZoneProperties.AlwaysBrightMaterialsByName.Contains(material.Name) == true)
                    material.AlwaysBrightOverride = true;

            // Add object instances
            foreach (ObjectInstance objectInstance in EQZoneData.ObjectInstances)
            {
                ZoneDoodadInstance doodadInstance = new ZoneDoodadInstance();
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

            // Add light instances
            if (Configuration.CONFIG_LIGHT_INSTANCES_ENABLED == true)
            {
                LightInstances = EQZoneData.LightInstances;

                // Correct light instance data
                foreach (LightInstance lightInstance in LightInstances)
                {
                    Vector3 originalPosition = new Vector3(lightInstance.Position);
                    lightInstance.Position.X = originalPosition.X * Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
                    // Invert Z and Y because of mapping differences
                    lightInstance.Position.Z = originalPosition.Y * Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
                    lightInstance.Position.Y = originalPosition.Z * Configuration.CONFIG_EQTOWOW_WORLD_SCALE;

                    // Also rotate the X and Y positions around Z axis 180 degrees
                    lightInstance.Position.X = -lightInstance.Position.X;
                    lightInstance.Position.Y = -lightInstance.Position.Y;

                    // If enabled, show light instances as torches for debugging
                    if (Configuration.CONFIG_LIGHT_INSTANCES_DRAWN_AS_TORCHES == true)
                    {
                        ZoneDoodadInstance doodadInstance = new ZoneDoodadInstance();
                        doodadInstance.ObjectName = "torch";
                        doodadInstance.Position = lightInstance.Position;
                        DoodadInstances.Add(doodadInstance);
                    }
                }
            }

            ZoneObjectModels.Clear();

            // Get and convert/translate the mesh data
            MeshData renderMeshData = new MeshData(EQZoneData.RenderMeshData);
            renderMeshData.ApplyEQToWoWGeometryTranslationsAndWorldScale();
            renderMeshData.ApplyEQToWoWVertexColor(ZoneProperties);
            MeshData collisionMeshData = new MeshData(EQZoneData.CollisionMeshData);
            collisionMeshData.ApplyEQToWoWGeometryTranslationsAndWorldScale();

            // Build liquid wmos
            GenerateLiquidWorldObjectModels(renderMeshData, ZoneProperties);

            // Build music wmos
            GenerateMusicWorldObjects(EQZoneData.MusicInstances, ZoneProperties);

            // Determine which materials are animated or transparent and create objects to represent them
            foreach (Material material in Materials)
                if ((material.IsAnimated() || material.HasTransparency()) && material.IsRenderable())
                    GenerateAndAddObjectInstancesForZoneMaterial(material, renderMeshData);

            // Create collision data for the map
            if (Configuration.CONFIG_WORLD_MODEL_OBJECT_COLLISION_ENABLED == true)
            {
                if (collisionMeshData.Vertices.Count == 0 || collisionMeshData.TriangleFaces.Count == 0)
                {
                    Logger.WriteDetail("For zone '" + ShortName + "', collision is generated from rendermesh");
                    collisionMeshData = new MeshData(renderMeshData);
                    GenerateCollisionWorldObjectModels(collisionMeshData);
                }
                else
                {
                    Logger.WriteDetail("For zone '" + ShortName + "', collision is generated from defined collision mesh");
                    if (collisionMeshData.Normals.Count == 0)
                        for (int i = 0; i < collisionMeshData.Vertices.Count; i++)
                            collisionMeshData.Normals.Add(new Vector3(0, 0, 0));
                    if (collisionMeshData.TextureCoordinates.Count == 0)
                        for (int i = 0; i < collisionMeshData.Vertices.Count; i++)
                            collisionMeshData.TextureCoordinates.Add(new TextureCoordinates(0, 0));
                    if (collisionMeshData.VertexColors.Count == 0 && renderMeshData.VertexColors.Count != 0)
                        for (int i = 0; i < collisionMeshData.Vertices.Count; i++)
                            collisionMeshData.VertexColors.Add(new ColorRGBA(0, 0, 0, 0));
                    GenerateCollisionWorldObjectModels(collisionMeshData);
                }
            }

            // Generate the render objects
            GenerateRenderWorldObjectModels(renderMeshData, Materials);

            // Save the loading screen
            switch (ZoneProperties.Continent)
            {
                case ZoneContinentType.Antonica:
                case ZoneContinentType.Faydwer:
                case ZoneContinentType.Development:
                case ZoneContinentType.Odus:
                    {
                        LoadingScreenID = Configuration.CONFIG_DBCID_LOADINGSCREENID_START;
                    }
                    break;
                case ZoneContinentType.Kunark:
                    {
                        LoadingScreenID = Configuration.CONFIG_DBCID_LOADINGSCREENID_START + 1;
                    }
                    break;
                case ZoneContinentType.Velious:
                    {
                        LoadingScreenID = Configuration.CONFIG_DBCID_LOADINGSCREENID_START + 2;
                    }
                    break;
                default:
                    {
                        Logger.WriteError("Error setting loading screen, as the passed continent was not handled");
                    }
                    break;
            }

            // Rebuild the bounding box
            List<Vector3> boundingVectors = new List<Vector3>();
            boundingVectors.AddRange(collisionMeshData.Vertices);
            boundingVectors.AddRange(renderMeshData.Vertices);
            BoundingBox = BoundingBox.GenerateBoxFromVectors(boundingVectors, Configuration.CONFIG_EQTOWOW_ADDED_BOUNDARY_AMOUNT);

            // If set, generate a shadowbox
            if (ZoneProperties.HasShadowBox == true)
            {
                ZoneObjectModel curWorldObjectModel = new ZoneObjectModel();
                curWorldObjectModel.LoadAsShadowBox(Materials, BoundingBox, ZoneProperties);
                ZoneObjectModels.Add(curWorldObjectModel);
            }

            // Completely loaded
            IsLoaded = true;
        }

        private void GenerateCollisionWorldObjectModels(MeshData collisionMeshData)
        {
            // Reduce the collision triangle incidies since they won't map to actual render indicies anymore
            collisionMeshData.CondenseAndRenumberVertexIndices();

            // If this can be generated as a single WMO, just do that
            if (collisionMeshData.TriangleFaces.Count <= Configuration.CONFIG_WOW_MAX_BTREE_FACES_PER_WMOGROUP)
            {
                ZoneObjectModel curWorldObjectModel = new ZoneObjectModel();
                curWorldObjectModel.LoadAsCollision(collisionMeshData, DoodadInstances, ZoneProperties);
                ZoneObjectModels.Add(curWorldObjectModel);
            }
            // Otherwise, break into parts
            else
            {
                // Generate the world groups by splitting the map down into subregions as needed
                BoundingBox fullBoundingBox = BoundingBox.GenerateBoxFromVectors(collisionMeshData.Vertices, Configuration.CONFIG_EQTOWOW_ADDED_BOUNDARY_AMOUNT);
                GenerateWorldObjectModelsByXYRegion(fullBoundingBox, collisionMeshData.TriangleFaces, collisionMeshData, Configuration.CONFIG_WOW_MAX_BTREE_FACES_PER_WMOGROUP, true);
            }
        }

        private void GenerateRenderWorldObjectModels(MeshData allMeshData, List<Material> allMaterials)
        {
            // Reduce meshdata to what will actually be rendered
            MeshData staticMeshData = allMeshData.GetMeshDataExcludingNonRenderedAndAnimatedMaterials(allMaterials.ToArray());

            // If this can be generated as a single WMO, just do that
            if (staticMeshData.TriangleFaces.Count <= Configuration.CONFIG_WOW_MAX_FACES_PER_WMOGROUP)
                GenerateWorldObjectModel(staticMeshData.TriangleFaces, staticMeshData);
            // Otherwise, break into parts
            else
            {
                // Generate the world groups by splitting the map down into subregions as needed
                BoundingBox fullBoundingBox = BoundingBox.GenerateBoxFromVectors(staticMeshData.Vertices, Configuration.CONFIG_EQTOWOW_ADDED_BOUNDARY_AMOUNT);
                GenerateWorldObjectModelsByXYRegion(fullBoundingBox, staticMeshData.TriangleFaces, staticMeshData, Configuration.CONFIG_WOW_MAX_FACES_PER_WMOGROUP, false);
            }
        }

        public void GenerateLiquidWorldObjectModels(MeshData meshData, ZoneProperties zoneProperties)
        {
            // Volumes
            foreach (ZoneLiquidVolume liquidVolume in zoneProperties.LiquidVolumes)
            {
                ZoneObjectModel curWorldObjectModel = new ZoneObjectModel();
                curWorldObjectModel.LoadAsLiquidVolume(liquidVolume.LiquidType, liquidVolume.LiquidPlane, liquidVolume.BoundingBox, zoneProperties);
                ZoneObjectModels.Add(curWorldObjectModel);
            }

            // Planes
            foreach (ZoneLiquidPlane liquidPlane in zoneProperties.LiquidPlanes)
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
                    List<ZoneLiquidPlane> liquidPlaneChunks = liquidPlane.SplitIntoSizeRestictedChunks(Configuration.CONFIG_EQTOWOW_LIQUID_SURFACE_MAX_XY_DIMENSION);
                    foreach (ZoneLiquidPlane curLiquidPlane in liquidPlaneChunks)
                    {
                        ZoneObjectModel curWorldObjectModel = new ZoneObjectModel();
                        curWorldObjectModel.LoadAsLiquidPlane(curLiquidPlane.LiquidType, curLiquidPlane, planeMaterial, curLiquidPlane.BoundingBox, zoneProperties);
                        ZoneObjectModels.Add(curWorldObjectModel);
                    }
                }
                else
                {
                    ZoneObjectModel curWorldObjectModel = new ZoneObjectModel();
                    curWorldObjectModel.LoadAsLiquidPlane(liquidPlane.LiquidType, liquidPlane, planeMaterial, liquidPlane.BoundingBox, zoneProperties);
                    ZoneObjectModels.Add(curWorldObjectModel);
                }
            }
        }

        public void GenerateMusicWorldObjects(List<MusicInstance> musicInstances, ZoneProperties zoneProperties)
        {
            // Skip if there are no valid index references
            if (zoneProperties.ValidMusicInstanceTrackIndexes.Count == 0)
                return;

            int curSoundEntryDBCID = zoneProperties.DBCSoundEntryZoneMusicStartID;
            int curZoneMusicDBCID = zoneProperties.DBCZoneMusicStartID;
            int curZoneMusicIndex = 1;
            foreach(MusicInstance musicInstance in musicInstances)
            {
                // Create day sound
                Sound? dayMusicSound = null;
                if (zoneProperties.ValidMusicInstanceTrackIndexes.Contains(musicInstance.DayIndex) == true)
                {
                    if (ZoneMusicSoundsByIndex.ContainsKey(musicInstance.DayIndex) == false)
                    {
                        string curSoundName = "EQ " + zoneProperties.ShortName + " music " + musicInstance.DayIndex.ToString();
                        string audioFileName = zoneProperties.ShortName + "-";
                        if (musicInstance.DayIndex > 9)
                            audioFileName += musicInstance.DayIndex.ToString() + ".mp3";
                        else
                            audioFileName += "0" + musicInstance.DayIndex.ToString() + ".mp3";
                        dayMusicSound = new Sound(curSoundEntryDBCID, curSoundName, audioFileName, SoundType.ZoneMusic);
                        curSoundEntryDBCID++;
                        ZoneMusicSoundsByIndex.Add(musicInstance.DayIndex, dayMusicSound);
                    }
                    else
                        dayMusicSound = ZoneMusicSoundsByIndex[musicInstance.DayIndex];
                }

                // Create night sound
                Sound? nightMusicSound = null;
                if (zoneProperties.ValidMusicInstanceTrackIndexes.Contains(musicInstance.NightIndex) == true)
                {
                    if (ZoneMusicSoundsByIndex.ContainsKey(musicInstance.NightIndex) == false)
                    {
                        string curSoundName = "EQ " + zoneProperties.ShortName + " music " + musicInstance.NightIndex.ToString();
                        string audioFileName = zoneProperties.ShortName + "-";
                        if (musicInstance.NightIndex > 9)
                            audioFileName += musicInstance.NightIndex.ToString() + ".mp3";
                        else
                            audioFileName += "0" + musicInstance.NightIndex.ToString() + ".mp3";
                        nightMusicSound = new Sound(curSoundEntryDBCID, curSoundName, audioFileName, SoundType.ZoneMusic);
                        curSoundEntryDBCID++;
                        ZoneMusicSoundsByIndex.Add(musicInstance.NightIndex, nightMusicSound);
                    }
                    else
                        nightMusicSound = ZoneMusicSoundsByIndex[musicInstance.NightIndex];
                }

                if (nightMusicSound == null && dayMusicSound == null)
                    continue;
                if (nightMusicSound == null)
                    nightMusicSound = dayMusicSound;
                if (dayMusicSound == null)
                    dayMusicSound = nightMusicSound;

                // Create the zone music record data
                ZoneObjectModel musicWorldObjectModel = new ZoneObjectModel();
                string curZoneMusicName = "Zone-" + zoneProperties.ShortName;
                if (curZoneMusicIndex > 9)
                    curZoneMusicName += curZoneMusicIndex.ToString();
                else
                    curZoneMusicName += "0" + curZoneMusicIndex.ToString();

                // Create the WMO
                musicWorldObjectModel.LoadAsMusic(musicInstance, curZoneMusicDBCID, curZoneMusicName, dayMusicSound, nightMusicSound);
                ZoneObjectModels.Add(musicWorldObjectModel);
                curZoneMusicDBCID++;
                curZoneMusicIndex++;
            }
        }

        private void GenerateWorldObjectModelsByXYRegion(BoundingBox boundingBox, List<TriangleFace> faces, MeshData meshData, int maxFacesPerWMOGroup, bool isCollisionMesh)
        {
            // If there are too many triangles to fit in a single box, cut the box into two and generate two child world model objects
            if (faces.Count > maxFacesPerWMOGroup)
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
                GenerateWorldObjectModelsByXYRegion(splitBox.BoxA, aBoxTriangles, meshData, maxFacesPerWMOGroup, isCollisionMesh);
                GenerateWorldObjectModelsByXYRegion(splitBox.BoxB, bBoxTriangles, meshData, maxFacesPerWMOGroup, isCollisionMesh);
            }
            else
            {
                if (isCollisionMesh == false)
                    GenerateWorldObjectModel(faces, meshData);
                else
                {
                    MeshData extractedMeshData = meshData.GetMeshDataForFaces(faces);
                    ZoneObjectModel curWorldObjectModel = new ZoneObjectModel();
                    curWorldObjectModel.LoadAsCollision(extractedMeshData, DoodadInstances, ZoneProperties);
                    ZoneObjectModels.Add(curWorldObjectModel);
                }
            }
        }

        private void GenerateWorldObjectModel(List<TriangleFace> facesToInclude, MeshData meshData)
        {
            // Generate a world model object if there are any vertices
            MeshData extractedMeshData = meshData.GetMeshDataForFaces(facesToInclude);
            if (extractedMeshData.Vertices.Count > 0)
            {
                ZoneObjectModel curWorldObjectModel = new ZoneObjectModel();
                curWorldObjectModel.LoadAsRendered(extractedMeshData, Materials, DoodadInstances, LightInstances, ZoneProperties);
                ZoneObjectModels.Add(curWorldObjectModel);
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

                List<Vector3> collisionVertices = new List<Vector3>();

                // Generate the object
                string name = "ZO_" + ShortName + "_" + material.UniqueName + "_" + i.ToString();
                ObjectModel newObject = new ObjectModel(name, ObjectModelProperties.GetObjectPropertiesForObject(""));
                newObject.Load(name, new List<Material> { new Material(material) }, curMeshData, new List<Vector3>(), new List<TriangleFace>(), false);
                GeneratedZoneObjects.Add(newObject);

                // Note: Below doesn't seem to work yet
                // Calculate the average vertex color
                //int totalR = 0;
                //int totalG = 0;
                //int totalB = 0;
                //foreach(ColorRGBA vertexColor in curMeshData.VertexColors)
                //{
                //    totalR += vertexColor.R;
                //    totalG += vertexColor.G;
                //    totalB += vertexColor.B;
                //}
                //ColorRGBA vertexColorAverage = new ColorRGBA();
                //vertexColorAverage.R = Convert.ToByte(totalR / curMeshData.VertexColors.Count);
                //vertexColorAverage.G = Convert.ToByte(totalG / curMeshData.VertexColors.Count);
                //vertexColorAverage.B = Convert.ToByte(totalB / curMeshData.VertexColors.Count);
                //vertexColorAverage.A = 0; // Alpha 0 keeps directional, alpha 255 is from center

                // Add as a doodad
                ZoneDoodadInstance doodadInstance = new ZoneDoodadInstance();
                doodadInstance.ObjectName = name;
                doodadInstance.Position = curPosition;
                //doodadInstance.Color = vertexColorAverage; // Not yet working
                //doodadInstance.Flags |= DoodadInstanceFlags.UseInteriorLighting; // Not yet working
                DoodadInstances.Add(doodadInstance);
            }
        }

        public void SetDescriptiveName(string name)
        {
            DescriptiveName = name;
            DescriptiveNameOnlyLetters = name;
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            DescriptiveNameOnlyLetters = rgx.Replace(DescriptiveNameOnlyLetters, "");
            DescriptiveNameOnlyLetters = DescriptiveNameOnlyLetters.Replace(" ", "");
        }
    }
}
