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
using EQWOWConverter.WOWFiles;
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
        public Dictionary<string, Sound> MusicSoundsByFileNameNoExt = new Dictionary<string, Sound>();
        public List<ZoneAreaMusic> ZoneAreaMusics = new List<ZoneAreaMusic>();
        public ZoneArea DefaultArea;
        public List<ZoneArea> SubAreas = new List<ZoneArea>();

        public Zone(string shortName, ZoneProperties zoneProperties)
        {
            ZoneProperties = zoneProperties;
            ShortName = shortName;
            if (zoneProperties.DescriptiveName != string.Empty)
                SetDescriptiveName(zoneProperties.DescriptiveName);
            else
                DescriptiveNameOnlyLetters = shortName;
            DefaultArea = new ZoneArea(DescriptiveName, new BoundingBox(), zoneProperties.ZonewideMusicFileNameDay, zoneProperties.ZonewideMusicFileNameNight);
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

            // Make the zonewide music if needed
            if (DefaultArea.MusicFileNameNoExtDay != string.Empty || DefaultArea.MusicFileNameNoExtNight != string.Empty)
                DefaultArea.AreaMusic = GenerateZoneAreaMusic(DefaultArea.MusicFileNameNoExtDay, DefaultArea.MusicFileNameNoExtNight);

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

            // Create the root object
            ZoneObjectModel rootModel = new ZoneObjectModel(Convert.ToUInt16(ZoneObjectModels.Count));
            rootModel.LoadAsRoot(ZoneProperties);
            ZoneObjectModels.Add(rootModel);

            // Get and convert/translate the mesh data
            MeshData renderMeshData = new MeshData(EQZoneData.RenderMeshData);
            renderMeshData.ApplyEQToWoWGeometryTranslationsAndWorldScale();
            renderMeshData.ApplyEQToWoWVertexColor(ZoneProperties);
            MeshData collisionMeshData = new MeshData(EQZoneData.CollisionMeshData);
            collisionMeshData.ApplyEQToWoWGeometryTranslationsAndWorldScale();

            // Build liquid wmos
            GenerateLiquidWorldObjectModels(renderMeshData, ZoneProperties);

            // Determine which materials are animated or transparent and create objects to represent them
            foreach (Material material in Materials)
                if ((material.IsAnimated() || material.HasTransparency()) && material.IsRenderable())
                    GenerateAndAddObjectInstancesForZoneMaterial(material, renderMeshData);

            // Create the areas
            GenerateWorldObjectModelsForAllCollidableAreas(renderMeshData, collisionMeshData, ZoneProperties);

            // Attach all doodads to the root
            rootModel.CreateZoneWideDoodadAssociations(DoodadInstances);

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

            // If set, show the area box around music instances
            if (Configuration.CONFIG_AUDIO_MUSIC_DRAW_MUSIC_AREAS_AS_BOXES == true)
            {
                for (int i = 0; i < ZoneObjectModels.Count; i++)
                {
                    ZoneObjectModel wmo = ZoneObjectModels[i];
                    if (wmo.ZoneMusic != null)
                    {
                        //ZoneBox zoneBox = new ZoneBox(wmo.BoundingBox, Materials, ShortName, 0, ZoneBoxRenderType.Both);
                        ZoneBox zoneBox = new ZoneBox(SubAreas[0].BoundingBox, Materials, ShortName, 0, ZoneBoxRenderType.Both);
                        GenerateRenderedWorldObjectModel(zoneBox.MeshData.TriangleFaces, zoneBox.MeshData);
                    }
                }
            }

            // Rebuild the bounding box
            List<BoundingBox> allBoundingBoxes = new List<BoundingBox>();
            foreach(ZoneObjectModel zoneObject in ZoneObjectModels)
                allBoundingBoxes.Add(zoneObject.BoundingBox);
            BoundingBox = BoundingBox.GenerateBoxFromBoxes(allBoundingBoxes);
            rootModel.BoundingBox = new BoundingBox(BoundingBox);

            // If set, generate a shadowbox
            if (ZoneProperties.HasShadowBox == true)
            {
                ZoneObjectModel curWorldObjectModel = new ZoneObjectModel(Convert.ToUInt16(ZoneObjectModels.Count));
                curWorldObjectModel.LoadAsShadowBox(Materials, BoundingBox, ZoneProperties);
                ZoneObjectModels.Add(curWorldObjectModel);
            }

            // Completely loaded
            IsLoaded = true;
        }

        private void GenerateRenderWorldObjectModels(MeshData allMeshData, List<Material> allMaterials)
        {
            // Reduce meshdata to what will actually be rendered
            MeshData staticMeshData = allMeshData.GetMeshDataExcludingNonRenderedAndAnimatedMaterials(allMaterials.ToArray());

            // If this can be generated as a single WMO, just do that
            if (staticMeshData.TriangleFaces.Count <= Configuration.CONFIG_WOW_MAX_FACES_PER_WMOGROUP)
                GenerateRenderedWorldObjectModel(staticMeshData.TriangleFaces, staticMeshData);
            // Otherwise, break into parts
            else
            {
                // Generate the world groups by splitting the map down into subregions as needed
                BoundingBox fullBoundingBox = BoundingBox.GenerateBoxFromVectors(staticMeshData.Vertices, Configuration.CONFIG_EQTOWOW_ADDED_BOUNDARY_AMOUNT);
                GenerateWorldObjectModelsByXYRegion(fullBoundingBox, staticMeshData.TriangleFaces, staticMeshData, Configuration.CONFIG_WOW_MAX_FACES_PER_WMOGROUP, ZoneObjectModelType.Rendered);
            }
        }

        private void GenerateWorldObjectModelsForAllCollidableAreas(MeshData renderMeshData, MeshData collisionMeshData, ZoneProperties zoneProperties)
        {
            if (Configuration.CONFIG_WORLD_MODEL_OBJECT_COLLISION_AND_MUSIC_ENABLED == false)
                return;

            // Determine if preset collision mesh data should be used, or if the render data should be used to generate it
            if (collisionMeshData.Vertices.Count == 0 || collisionMeshData.TriangleFaces.Count == 0)
            {
                Logger.WriteDetail("For zone '" + ShortName + "', collision is generated from rendermesh");
                collisionMeshData = new MeshData(renderMeshData);
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
            }

            // Generate sub-areas
            foreach(ZoneArea subArea in zoneProperties.ZoneAreas)
            {
                MeshData areaMeshData;
                MeshData remainderMeshData;
                MeshData.GetSplitMeshDataWithClipping(collisionMeshData, subArea.BoundingBox, out areaMeshData, out remainderMeshData);
                collisionMeshData = remainderMeshData;
                GenerateWorldObjectModelsForCollidableArea(areaMeshData, subArea);
                SubAreas.Add(subArea);
            }

            // Remainder is the primary area
            DefaultArea.BoundingBox = BoundingBox.GenerateBoxFromVectors(collisionMeshData.Vertices, Configuration.CONFIG_EQTOWOW_ADDED_BOUNDARY_AMOUNT);
            GenerateWorldObjectModelsForCollidableArea(collisionMeshData, DefaultArea);
        }

        private void GenerateWorldObjectModelsForCollidableArea(MeshData collisionMeshData, ZoneArea zoneArea)
        {
            // Create a music if needed
            ZoneAreaMusic? areaMusic = null;
            if (zoneArea.MusicFileNameNoExtDay != string.Empty || zoneArea.MusicFileNameNoExtNight != string.Empty)
            {
                areaMusic = GenerateZoneAreaMusic(zoneArea.MusicFileNameNoExtDay, zoneArea.MusicFileNameNoExtNight);
                zoneArea.AreaMusic = areaMusic;
            }

            // Break the geometry into as many parts as limited by the system
            BoundingBox fullBoundingBox = BoundingBox.GenerateBoxFromVectors(collisionMeshData.Vertices, Configuration.CONFIG_EQTOWOW_ADDED_BOUNDARY_AMOUNT);
            List<MeshData> meshDataChunks = collisionMeshData.GetMeshDataChunks(fullBoundingBox, collisionMeshData.TriangleFaces, Configuration.CONFIG_WOW_MAX_BTREE_FACES_PER_WMOGROUP);

            // Create a group for each chunk
            foreach(MeshData meshDataChunk in meshDataChunks)
            {
                ZoneObjectModel curWorldObjectModel = new ZoneObjectModel(Convert.ToUInt16(ZoneObjectModels.Count));
                meshDataChunk.CondenseAndRenumberVertexIndices();
                curWorldObjectModel.LoadAsCollidableArea(meshDataChunk, zoneArea.DBCAreaTableID, zoneArea.DisplayName, areaMusic, ZoneProperties);
                ZoneObjectModels.Add(curWorldObjectModel);
            }
        }

        private ZoneAreaMusic GenerateZoneAreaMusic(string musicFileNameDay, string musicFileNameNight)
        {
            // Reuse if exists
            foreach (ZoneAreaMusic areaMusic in ZoneAreaMusics)
                if (areaMusic.FileNameNoExtDay == musicFileNameDay && areaMusic.FileNameNoExtNight == musicFileNameNight)
                    return areaMusic;

            // Error if both are blank names
            if (musicFileNameDay == string.Empty &&  musicFileNameNight == string.Empty)
            {
                string errorMessage = "GenerateZoneAreaMusic failed for '" + ShortName + "' because both the day and night file names were blank";
                Logger.WriteError(errorMessage);
                throw new Exception(errorMessage);
            }

            // Generate new sounds if needed
            Sound? daySound = null;
            if (musicFileNameDay != string.Empty)
            {
                if (MusicSoundsByFileNameNoExt.ContainsKey(musicFileNameDay))
                    daySound = MusicSoundsByFileNameNoExt[musicFileNameDay];
                else
                {
                    string curSoundName = "EQ Music " + musicFileNameDay;
                    daySound = new Sound(curSoundName, musicFileNameDay, SoundType.ZoneMusic);
                    MusicSoundsByFileNameNoExt.Add(musicFileNameDay, daySound);
                }
            }
            Sound? nightSound = null;
            if (musicFileNameNight != string.Empty)
            {
                if (MusicSoundsByFileNameNoExt.ContainsKey(musicFileNameNight))
                    nightSound = MusicSoundsByFileNameNoExt[musicFileNameNight];
                else
                {
                    string curSoundName = "EQ Music " + musicFileNameNight;
                    nightSound = new Sound(curSoundName, musicFileNameNight, SoundType.ZoneMusic);
                    MusicSoundsByFileNameNoExt.Add(musicFileNameNight, nightSound);
                }
            }

            // Generate the music
            string musicName = "Zone-" + ShortName;
            if (ZoneAreaMusics.Count > 9)
                musicName += ZoneAreaMusics.Count.ToString();
            else
                musicName += "0" + ZoneAreaMusics.Count.ToString();
            ZoneAreaMusic newMusic = new ZoneAreaMusic(musicName, daySound, nightSound, musicFileNameDay, musicFileNameNight);
            ZoneAreaMusics.Add(newMusic);

            // Return it
            return newMusic;
        }

        private void GenerateLiquidWorldObjectModels(MeshData meshData, ZoneProperties zoneProperties)
        {
            // Volumes
            foreach (ZoneLiquidVolume liquidVolume in zoneProperties.LiquidVolumes)
            {
                ZoneObjectModel curWorldObjectModel = new ZoneObjectModel(Convert.ToUInt16(ZoneObjectModels.Count));
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
                        ZoneObjectModel curWorldObjectModel = new ZoneObjectModel(Convert.ToUInt16(ZoneObjectModels.Count));
                        curWorldObjectModel.LoadAsLiquidPlane(curLiquidPlane.LiquidType, curLiquidPlane, planeMaterial, curLiquidPlane.BoundingBox, zoneProperties);
                        ZoneObjectModels.Add(curWorldObjectModel);
                    }
                }
                else
                {
                    ZoneObjectModel curWorldObjectModel = new ZoneObjectModel(Convert.ToUInt16(ZoneObjectModels.Count));
                    curWorldObjectModel.LoadAsLiquidPlane(liquidPlane.LiquidType, liquidPlane, planeMaterial, liquidPlane.BoundingBox, zoneProperties);
                    ZoneObjectModels.Add(curWorldObjectModel);
                }
            }
        }

        private void GenerateWorldObjectModelsByXYRegion(BoundingBox boundingBox, List<TriangleFace> faces, MeshData meshData, int maxFacesPerWMOGroup, 
            ZoneObjectModelType wmoType)
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
                GenerateWorldObjectModelsByXYRegion(splitBox.BoxA, aBoxTriangles, meshData, maxFacesPerWMOGroup, wmoType);
                GenerateWorldObjectModelsByXYRegion(splitBox.BoxB, bBoxTriangles, meshData, maxFacesPerWMOGroup, wmoType);
            }
            else
            {
                if (wmoType == ZoneObjectModelType.Rendered)
                    GenerateRenderedWorldObjectModel(faces, meshData);
            }
        }

        private void GenerateRenderedWorldObjectModel(List<TriangleFace> facesToInclude, MeshData meshData)
        {
            // Generate a world model object if there are any vertices
            MeshData extractedMeshData = meshData.GetMeshDataForFaces(facesToInclude);
            if (extractedMeshData.Vertices.Count > 0)
            {
                ZoneObjectModel curWorldObjectModel = new ZoneObjectModel(Convert.ToUInt16(ZoneObjectModels.Count));
                curWorldObjectModel.LoadAsRendered(extractedMeshData, Materials, LightInstances, ZoneProperties);
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
