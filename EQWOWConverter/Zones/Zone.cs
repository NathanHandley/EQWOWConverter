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
        public Dictionary<string, Sound> AmbientSoundsByFileNameNoExt = new Dictionary<string, Sound>();
        public List<ZoneAreaAmbientSound> ZoneAreaAmbientSounds = new List<ZoneAreaAmbientSound>();
        public ZoneArea DefaultArea;
        public List<ZoneArea> SubAreas = new List<ZoneArea>();
        public List<SoundInstance> SoundInstances = new List<SoundInstance>();
        public List<ObjectModel> SoundInstanceObjectModels = new List<ObjectModel>();

        public Zone(string shortName, ZoneProperties zoneProperties)
        {
            ZoneProperties = zoneProperties;
            ShortName = shortName;
            if (zoneProperties.DescriptiveName != string.Empty)
                SetDescriptiveName(zoneProperties.DescriptiveName);
            else
                DescriptiveNameOnlyLetters = shortName;
            DefaultArea = zoneProperties.DefaultZoneArea;
            SubAreas = zoneProperties.ZoneAreas;
        }

        public void LoadEQZoneData(string inputZoneFolderName, string inputZoneFolderFullPath)
        {
            // Load
            EQZoneData.LoadDataFromDisk(inputZoneFolderName, inputZoneFolderFullPath);
        } 

        public void LoadFromEQZone()
        {
            if (IsLoaded == true)
            {
                Logger.WriteInfo("LoadFromEQZone called for zone '" + ShortName + "' when the zone was already loaded");
                return;
            }

            // Clear any prior world object model data
            ZoneObjectModels.Clear();

            // Get and convert/translate the EverQuest mesh data
            MeshData renderMeshData = new MeshData(EQZoneData.RenderMeshData);
            renderMeshData.ApplyEQToWoWGeometryTranslationsAndScale(true, Configuration.CONFIG_GENERATE_WORLD_SCALE);
            renderMeshData.ApplyEQToWoWVertexColor(ZoneProperties);
            MeshData collisionMeshData = new MeshData(EQZoneData.CollisionMeshData);
            collisionMeshData.ApplyEQToWoWGeometryTranslationsAndScale(true, Configuration.CONFIG_GENERATE_WORLD_SCALE);

            // Update the materials
            Materials = EQZoneData.Materials;
            foreach (Material material in Materials)
                if (ZoneProperties.AlwaysBrightMaterialsByName.Contains(material.Name) == true)
                    material.AlwaysBrightOverride = true;

            // Make the zonewide music if needed
            if (DefaultArea.MusicFileNameNoExtDay != string.Empty || DefaultArea.MusicFileNameNoExtNight != string.Empty)
                DefaultArea.AreaMusic = GenerateZoneAreaMusic(DefaultArea.MusicFileNameNoExtDay, DefaultArea.MusicFileNameNoExtNight, DefaultArea.MusicLoop, DefaultArea.MusicVolume);

            // Build light instances
            GenerateLightInstances(EQZoneData.LightInstances);

            // Process Sound Instances
            ProcessSoundInstances();

            // Associate liquids to their respective zone areas 
            AssignLiquidsToAreas();

            // Create doodad instances
            GenerateDoodadInstances(EQZoneData.ObjectInstances, renderMeshData);

            // Generate the collidable areas (zone areas, liquid)
            GenerateCollidableWorldObjectModels(renderMeshData, collisionMeshData);

            // Generate the render objects
            GenerateRenderWorldObjectModels(renderMeshData);

            // Bind doodads to wmos
            AssociateDoodadsWithWMOs();

            // Save the loading screen
            SetLoadingScreen();

            // Rebuild the bounding box
            List<BoundingBox> allBoundingBoxes = new List<BoundingBox>();
            foreach(ZoneObjectModel zoneObject in ZoneObjectModels)
                allBoundingBoxes.Add(zoneObject.BoundingBox);
            BoundingBox = BoundingBox.GenerateBoxFromBoxes(allBoundingBoxes);

            // Set any area parent relationships
            SetAreaParentRelationships();

            // If set, generate a shadowbox
            if (ZoneProperties.HasShadowBox == true)
            {
                ZoneObjectModel curWorldObjectModel = new ZoneObjectModel(Convert.ToUInt16(ZoneObjectModels.Count), DefaultArea.DBCAreaTableID);
                curWorldObjectModel.LoadAsShadowBox(Materials, BoundingBox, ZoneProperties);
                ZoneObjectModels.Add(curWorldObjectModel);
            }

            // Completely loaded
            IsLoaded = true;
        }

        private void AssignLiquidsToAreas()
        {
            // Go through each liquid and find which area they belong to, and split if they fall onto a boundary
            List<ZoneLiquidGroup> liquidGroupsToAssign = ZoneProperties.LiquidGroups;

            while (liquidGroupsToAssign.Count > 0)
            {
                // Pop the first liquid and work with that
                ZoneLiquidGroup curLiquidGroup = liquidGroupsToAssign[0];
                liquidGroupsToAssign.RemoveAt(0);

                // See if it fits in any sub areas
                bool liquidWasAssignedToSubArea = false;
                foreach (ZoneArea area in SubAreas)
                {
                    // If fully contained, make it directly assigned
                    if (area.MaxBoundingBox.ContainsBox(curLiquidGroup.BoundingBox) == true)
                    {
                        area.LiquidGroups.Add(curLiquidGroup);
                        liquidWasAssignedToSubArea = true;
                        break;
                    }

                    // Try every box in the area
                    foreach (BoundingBox areaBox in area.BoundingBoxes)
                    {
                        // If it intersects, split it up
                        if (areaBox.DoesIntersectBox(curLiquidGroup.BoundingBox) == true)
                        {
                            // Create two groups, one for what goes to this subarea and one that goes back on the pile
                            ZoneLiquidGroup subAreaLiquidGroup = new ZoneLiquidGroup();
                            ZoneLiquidGroup remainderLiquidGroup = new ZoneLiquidGroup();
                            foreach (ZoneLiquid liquidChunk in curLiquidGroup.GetLiquidChunks())
                            {
                                if (areaBox.ContainsBox(liquidChunk.BoundingBox) == true)
                                {
                                    subAreaLiquidGroup.AddLiquidChunk(liquidChunk);
                                    continue;
                                }
                                if (areaBox.DoesIntersectBox(liquidChunk.BoundingBox) == true)
                                {
                                    List<BoundingBox> intersectingBox;
                                    List<BoundingBox> areaOnlyBoxes;
                                    List<BoundingBox> liquidOnlyBoxes;
                                    BoundingBox.SplitBoundingIntersect(areaBox, liquidChunk.BoundingBox, out intersectingBox, out areaOnlyBoxes, out liquidOnlyBoxes);
                                    if (intersectingBox.Count == 0)
                                        throw new Exception("Could not split up a liquid volume, as no intersecting box was found");

                                    // Save the intersection box into the area as a new chunk
                                    ZoneLiquid intersectionLiquid = liquidChunk.GeneratePartialFromBoundingBox(intersectingBox[0]);
                                    subAreaLiquidGroup.AddLiquidChunk(intersectionLiquid);

                                    // The rest go to the remainder group if they were liquid
                                    foreach (BoundingBox box in liquidOnlyBoxes)
                                    {
                                        ZoneLiquid liquidBox = liquidChunk.GeneratePartialFromBoundingBox(box);
                                        remainderLiquidGroup.AddLiquidChunk(liquidBox);
                                    }
                                }
                            }
                            if (subAreaLiquidGroup.GetLiquidChunks().Count > 0)
                                area.LiquidGroups.Add(subAreaLiquidGroup);
                            if (remainderLiquidGroup.GetLiquidChunks().Count > 0)
                                liquidGroupsToAssign.Add(remainderLiquidGroup);
                        }
                    }
                }

                // If no where else, give it to the default area
                if (liquidWasAssignedToSubArea == false)
                    DefaultArea.LiquidGroups.Add(curLiquidGroup);
            }
        }

        private void AssociateDoodadsWithWMOs()
        {
            // Attach the doodads to the nearest wmo
            for (int di = 0; di < DoodadInstances.Count; di++)
            {
                ZoneDoodadInstance doodadInstance = DoodadInstances[di];

                int curZoneObjectModelIndex = 0;
                float currentDistance = 1000000;
                for (int i = 0; i < ZoneObjectModels.Count; i++)
                {
                    ZoneObjectModel curZoneObjectModel = ZoneObjectModels[i];
                    if (curZoneObjectModel.WMOType != ZoneObjectModelType.Rendered)
                        continue;

                    float thisDistance = doodadInstance.Position.GetDistance(curZoneObjectModel.BoundingBox.GetCenter());
                    if (thisDistance < currentDistance)
                    {
                        currentDistance = thisDistance;
                        curZoneObjectModelIndex = i;
                    }
                }
                ZoneObjectModels[curZoneObjectModelIndex].DoodadInstances.Add(di, doodadInstance);
            }
        }

        private void GenerateDoodadInstances(List<ObjectInstance> eqObjectInstances, MeshData renderMeshData)
        {
            // Create doodad instances from EQ object instances
            foreach (ObjectInstance objectInstance in eqObjectInstances)
            {
                ZoneDoodadInstance doodadInstance = new ZoneDoodadInstance(ZoneDoodadInstanceType.StaticObject);
                doodadInstance.ObjectName = objectInstance.ModelName;
                doodadInstance.Position.X = objectInstance.Position.X * Configuration.CONFIG_GENERATE_WORLD_SCALE;
                // Invert Z and Y because of mapping differences
                doodadInstance.Position.Z = objectInstance.Position.Y * Configuration.CONFIG_GENERATE_WORLD_SCALE;
                doodadInstance.Position.Y = objectInstance.Position.Z * Configuration.CONFIG_GENERATE_WORLD_SCALE;

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

            // Determine which materials are animated or transparent and create objects to represent them
            foreach (Material material in Materials)
                if ((material.IsAnimated() || material.HasTransparency()) && material.IsRenderable())
                    GenerateAndAddDoodadsForZoneMaterial(material, renderMeshData);

            // If enabled, show light instances as torches for debugging
            if (Configuration.CONFIG_LIGHT_INSTANCES_DRAWN_AS_TORCHES == true)
            {
                foreach (LightInstance lightInstance in LightInstances)
                {
                    ZoneDoodadInstance doodadInstance = new ZoneDoodadInstance(ZoneDoodadInstanceType.StaticObject);
                    doodadInstance.ObjectName = "torch";
                    doodadInstance.Position = lightInstance.Position;
                    DoodadInstances.Add(doodadInstance);
                }
            }
        }

        private void GenerateLightInstances(List<LightInstance> eqLightInstances)
        {
            // Add light instances
            if (Configuration.CONFIG_LIGHT_INSTANCES_ENABLED == true)
            {
                LightInstances = eqLightInstances;

                // Correct light instance data
                foreach (LightInstance lightInstance in LightInstances)
                {
                    Vector3 originalPosition = new Vector3(lightInstance.Position);
                    lightInstance.Position.X = originalPosition.X * Configuration.CONFIG_GENERATE_WORLD_SCALE;
                    // Invert Z and Y because of mapping differences
                    lightInstance.Position.Z = originalPosition.Y * Configuration.CONFIG_GENERATE_WORLD_SCALE;
                    lightInstance.Position.Y = originalPosition.Z * Configuration.CONFIG_GENERATE_WORLD_SCALE;

                    // Also rotate the X and Y positions around Z axis 180 degrees
                    lightInstance.Position.X = -lightInstance.Position.X;
                    lightInstance.Position.Y = -lightInstance.Position.Y;
                }
            }
        }

        private void GenerateCollidableWorldObjectModels(MeshData renderMeshData, MeshData collisionMeshData)
        {
            if (Configuration.CONFIG_ZONE_COLLISION_ENABLED == false)
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

            // Build collision areas based on zone areas
            foreach (ZoneArea subArea in ZoneProperties.ZoneAreas)
            {
                // Generate areas for each liquid first since those need to be fully contained
                foreach (ZoneLiquidGroup liquidGroup in subArea.LiquidGroups)
                {
                    // Clip out all geoemetry for the liquid group and use that same collision geometry for every liquid entry
                    MeshData liquidGroupMeshData;
                    MeshData remainderMeshData;
                    MeshData.GetSplitMeshDataWithClipping(collisionMeshData, liquidGroup.BoundingBox, out liquidGroupMeshData, out remainderMeshData);
                    collisionMeshData = remainderMeshData;
                    foreach (ZoneLiquid liquid in liquidGroup.GetLiquidChunks())
                    {
                        MeshData liquidChunkMeshData;
                        MeshData.GetSplitMeshDataWithClipping(liquidGroupMeshData, liquid.BoundingBox, out liquidChunkMeshData, out remainderMeshData);
                        GenerateCollisionWorldObjectModelsForCollidableArea(liquidChunkMeshData, subArea, liquid);
                        liquidGroupMeshData = remainderMeshData;
                    }
                    collisionMeshData.AddMeshData(liquidGroupMeshData);
                }
                // Areas can have multiple boxes, so merge them up
                MeshData areaMeshDataFull = new MeshData();
                foreach (BoundingBox areaSubBoundingBox in subArea.BoundingBoxes)
                {
                    MeshData areaMeshDataBox;
                    MeshData remainderMeshData;
                    MeshData.GetSplitMeshDataWithClipping(collisionMeshData, areaSubBoundingBox, out areaMeshDataBox, out remainderMeshData);
                    collisionMeshData = remainderMeshData;
                    areaMeshDataFull.AddMeshData(areaMeshDataBox);
                }
                GenerateCollisionWorldObjectModelsForCollidableArea(areaMeshDataFull, subArea, null);
            }
            foreach (ZoneLiquidGroup liquidGroup in DefaultArea.LiquidGroups)
            {
                // Clip out all geoemetry for the liquid group and use that same collision geometry for every liquid entry
                MeshData liquidGroupMeshData;
                MeshData remainderMeshData;
                MeshData.GetSplitMeshDataWithClipping(collisionMeshData, liquidGroup.BoundingBox, out liquidGroupMeshData, out remainderMeshData);
                collisionMeshData = remainderMeshData;
                foreach (ZoneLiquid liquid in liquidGroup.GetLiquidChunks())
                {
                    MeshData liquidChunkMeshData;
                    MeshData.GetSplitMeshDataWithClipping(liquidGroupMeshData, liquid.BoundingBox, out liquidChunkMeshData, out remainderMeshData);
                    GenerateCollisionWorldObjectModelsForCollidableArea(liquidChunkMeshData, DefaultArea, liquid);
                    liquidGroupMeshData = remainderMeshData;
                }
                collisionMeshData.AddMeshData(liquidGroupMeshData);
            }
            DefaultArea.AddBoundingBox(BoundingBox.GenerateBoxFromVectors(collisionMeshData.Vertices, Configuration.CONFIG_GENERATE_ADDED_BOUNDARY_AMOUNT), false);
            GenerateCollisionWorldObjectModelsForCollidableArea(collisionMeshData, DefaultArea, null);
        }

        private void GenerateCollisionWorldObjectModelsForCollidableArea(MeshData collisionMeshData, ZoneArea zoneArea, ZoneLiquid? liquid)
        {
            // Create a music if needed
            ZoneAreaMusic? areaMusic = null;
            if (zoneArea.MusicFileNameNoExtDay != string.Empty || zoneArea.MusicFileNameNoExtNight != string.Empty)
            {
                areaMusic = GenerateZoneAreaMusic(zoneArea.MusicFileNameNoExtDay, zoneArea.MusicFileNameNoExtNight, zoneArea.MusicLoop, zoneArea.MusicVolume);
                zoneArea.AreaMusic = areaMusic;
            }

            // Create sound ambience if needed
            ZoneAreaAmbientSound? areaAmbientSound = null;
            if (zoneArea.AmbientSoundFileNameNoExtDay != string.Empty || zoneArea.AmbientSoundFileNameNoExtNight != string.Empty)
            {
                areaAmbientSound = GenerateZoneAreaAmbientSound(zoneArea.AmbientSoundFileNameNoExtDay, zoneArea.AmbientSoundFileNameNoExtNight);
                zoneArea.AreaAmbientSound = areaAmbientSound;
            }

            // Break the geometry into as many parts as limited by the system
            BoundingBox fullBoundingBox = BoundingBox.GenerateBoxFromVectors(collisionMeshData.Vertices, Configuration.CONFIG_GENERATE_ADDED_BOUNDARY_AMOUNT);
            List<MeshData> meshDataChunks = collisionMeshData.GetMeshDataChunks(fullBoundingBox, collisionMeshData.TriangleFaces, Configuration.CONFIG_ZONE_MAX_BTREE_FACES_PER_WMOGROUP);

            // Force a data chunk if there is still liquid
            if (meshDataChunks.Count == 0 && liquid != null)
                meshDataChunks.Add(new MeshData());

            // Create a group for each chunk
            foreach (MeshData meshDataChunk in meshDataChunks)
            {
                ZoneObjectModel curWorldObjectModel = new ZoneObjectModel(Convert.ToUInt16(ZoneObjectModels.Count), zoneArea.DBCAreaTableID);
                meshDataChunk.CondenseAndRenumberVertexIndices();
                curWorldObjectModel.LoadAsCollidableArea(meshDataChunk, zoneArea.DisplayName, areaMusic, liquid, ZoneProperties);
                ZoneObjectModels.Add(curWorldObjectModel);
            }
        }

        private void GenerateRenderWorldObjectModels(MeshData renderMeshData)
        {
            // Reduce meshdata to what will actually be rendered
            MeshData staticMeshData = renderMeshData.GetMeshDataExcludingNonRenderedAndAnimatedMaterials(Materials.ToArray());

            // If set, show the area box
            if (Configuration.CONFIG_ZONE_DRAW_COLLIDABLE_SUB_AREAS_AS_BOXES == true)
            {
                foreach(ZoneArea zoneArea in SubAreas)
                {
                    if (zoneArea.DisplayName != ZoneProperties.DescriptiveName)
                    {
                        foreach(BoundingBox areaBox in zoneArea.BoundingBoxes)
                        {
                            ZoneBox zoneBox = new ZoneBox(areaBox, Materials, ShortName, 0, MeshBoxRenderType.Both);
                            staticMeshData.AddMeshData(zoneBox.MeshData);
                        }
                    }
                }
            }

            // Break the geometry into as many parts as limited by the system
            BoundingBox fullBoundingBox = BoundingBox.GenerateBoxFromVectors(staticMeshData.Vertices, Configuration.CONFIG_GENERATE_ADDED_BOUNDARY_AMOUNT);
            List<MeshData> meshDataChunks = staticMeshData.GetMeshDataChunks(fullBoundingBox, staticMeshData.TriangleFaces, Configuration.CONFIG_ZONE_MAX_FACES_PER_WMOGROUP);

            // Create a group for each chunk
            foreach (MeshData meshDataChunk in meshDataChunks)
            {
                ZoneObjectModel curWorldObjectModel = new ZoneObjectModel(Convert.ToUInt16(ZoneObjectModels.Count), DefaultArea.DBCAreaTableID);
                meshDataChunk.CondenseAndRenumberVertexIndices();
                curWorldObjectModel.LoadAsRendered(meshDataChunk, Materials, LightInstances, ZoneProperties);
                ZoneObjectModels.Add(curWorldObjectModel);
            }
        }

        private ZoneAreaMusic GenerateZoneAreaMusic(string musicFileNameDay, string musicFileNameNight, bool loop, float volume)
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
            Sound? daySound = GenerateOrGetAreaSound(musicFileNameDay, ref MusicSoundsByFileNameNoExt, "EQ Music ", SoundType.ZoneMusic, loop, volume);
            Sound? nightSound = GenerateOrGetAreaSound(musicFileNameNight, ref MusicSoundsByFileNameNoExt, "EQ Music ", SoundType.ZoneMusic, loop, volume);

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

        private ZoneAreaAmbientSound GenerateZoneAreaAmbientSound(string soundFileNameDay, string soundFileNameNight)
        {
            // Reuse if exists
            foreach (ZoneAreaAmbientSound areaAmbientSound in ZoneAreaAmbientSounds)
                if (areaAmbientSound.FileNameNoExtDay == soundFileNameDay && areaAmbientSound.FileNameNoExtNight == soundFileNameNight)
                    return areaAmbientSound;

            // Error if both are blank names
            if (soundFileNameDay == string.Empty && soundFileNameNight == string.Empty)
            {
                string errorMessage = "GenerateAmbientSound failed for '" + ShortName + "' because both the day and night file names were blank";
                Logger.WriteError(errorMessage);
                throw new Exception(errorMessage);
            }

            // Generate new sounds if needed
            Sound? daySound = GenerateOrGetAreaSound(soundFileNameDay, ref AmbientSoundsByFileNameNoExt, "EQ Ambient ", SoundType.ZoneAmbience, true);
            Sound? nightSound = GenerateOrGetAreaSound(soundFileNameNight, ref AmbientSoundsByFileNameNoExt, "EQ Ambient ", SoundType.ZoneAmbience, true);

            // Generate the ambient sounds
            ZoneAreaAmbientSound newAmbientSound = new ZoneAreaAmbientSound(daySound, nightSound, soundFileNameDay, soundFileNameNight);
            ZoneAreaAmbientSounds.Add(newAmbientSound);

            // Return it
            return newAmbientSound;
        }

        private Sound? GenerateOrGetAreaSound(string soundFileName, ref Dictionary<string, Sound> existingLookup, string soundNamePrefix, SoundType soundType, bool loop, float volume = 1f)
        {
            Sound? returnSound = null;
            if (soundFileName != string.Empty)
            {
                if (existingLookup.ContainsKey(soundFileName))
                    returnSound = existingLookup[soundFileName];
                else
                {
                    string curSoundName = soundNamePrefix + soundFileName;
                    returnSound = new Sound(curSoundName, soundFileName, soundType, 8, 45, loop, volume); // 8 and 45 are default values for music and ambience in the DBCs
                    existingLookup.Add(soundFileName, returnSound);
                }
            }
            return returnSound;
        }

        private void SetLoadingScreen()
        {
            switch (ZoneProperties.Continent)
            {
                case ZoneContinentType.Antonica:
                case ZoneContinentType.Faydwer:
                case ZoneContinentType.Development:
                case ZoneContinentType.Odus:
                    {
                        LoadingScreenID = Configuration.CONFIG_DBCID_LOADINGSCREEN_ID_START;
                    }
                    break;
                case ZoneContinentType.Kunark:
                    {
                        LoadingScreenID = Configuration.CONFIG_DBCID_LOADINGSCREEN_ID_START + 1;
                    }
                    break;
                case ZoneContinentType.Velious:
                    {
                        LoadingScreenID = Configuration.CONFIG_DBCID_LOADINGSCREEN_ID_START + 2;
                    }
                    break;
                default:
                    {
                        Logger.WriteError("Error setting loading screen, as the passed continent was not handled");
                    }
                    break;
            }
        }

        private void GenerateAndAddDoodadsForZoneMaterial(Material material, MeshData allMeshData)
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
                string name = material.UniqueName + "_" + i.ToString();
                ObjectModel newObject = new ObjectModel(name, ObjectModelProperties.GetObjectPropertiesForObject(""), ObjectModelType.ZoneModel);
                newObject.Load(name, new List<Material> { new Material(material) }, curMeshData, new List<Vector3>(), new List<TriangleFace>());
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
                ZoneDoodadInstance doodadInstance = new ZoneDoodadInstance(ZoneDoodadInstanceType.ZoneMaterial);
                doodadInstance.ObjectName = name;
                doodadInstance.Position = curPosition;
                //doodadInstance.Color = vertexColorAverage; // Not yet working
                //doodadInstance.Flags |= DoodadInstanceFlags.UseInteriorLighting; // Not yet working
                DoodadInstances.Add(doodadInstance);
            }
        }

        private void SetAreaParentRelationships()
        {
            // For any parent matches, set the parent ID.  Otherwise, set the default id
            foreach(ZoneArea zoneAreaToSet in SubAreas)
            {
                bool matchFound = false;
                foreach (ZoneArea zoneAreaToCheck in SubAreas)
                {
                    if (zoneAreaToSet.ParentAreaDisplayName == zoneAreaToCheck.DisplayName)
                    {
                        zoneAreaToSet.DBCParentAreaTableID = zoneAreaToCheck.DBCAreaTableID;
                        matchFound = true;
                        continue;
                    }
                }
                if (matchFound == false)
                {
                    zoneAreaToSet.DBCParentAreaTableID = DefaultArea.DBCAreaTableID;
                    zoneAreaToSet.ParentAreaDisplayName = DefaultArea.ParentAreaDisplayName;
                }
            }
        }

        private void ProcessSoundInstances()
        {
            // 3D Sounds
            foreach (SoundInstance soundInstance3D in EQZoneData.Sound3DInstances)
            {
                if (soundInstance3D.SoundFileNameDayNoExt.Trim() == string.Empty)
                    Logger.WriteDetail("For zone '" + ShortName + "', skipping 3D sound instance which has no file name for the day sound");
                else if (soundInstance3D.SoundFileNameDayNoExt != soundInstance3D.SoundFileNameNightNoExt)
                    Logger.WriteDetail("For zone '" + ShortName + "', skipping 3D sound instance which has mismatched day and night of '" + soundInstance3D.SoundFileNameDayNoExt + "' and '" + soundInstance3D.SoundFileNameNightNoExt + "'");
                else
                    ProcessSoundInstance(soundInstance3D);
            }

            // 2D Sounds
            // TODO: Make these non-directional...?
            foreach (SoundInstance soundInstance2D in EQZoneData.Sound2DInstances)
            {
                if (ZoneProperties.Enabled2DSoundInstancesByDaySoundName.Contains(soundInstance2D.SoundFileNameDayNoExt))
                    ProcessSoundInstance(soundInstance2D);
            }
        }

        private void ProcessSoundInstance(SoundInstance soundInstance)
        {
            // Create the sound
            float radius = soundInstance.Radius * Configuration.CONFIG_GENERATE_WORLD_SCALE;
            float minDistance = radius;
            if (soundInstance.Is2DSound)
                minDistance *= Configuration.CONFIG_AUDIO_SOUNDINSTANCE_2D_MIN_DISTANCE_MOD;
            else
                minDistance *= Configuration.CONFIG_AUDIO_SOUNDINSTANCE_3D_MIN_DISTANCE_MOD;

            soundInstance.Sound = new Sound(soundInstance.GenerateDBCName(ShortName, SoundInstances.Count), soundInstance.SoundFileNameDayNoExt,
                SoundType.GameObject, minDistance, radius, true);

            //  Flip Y and Z
            float yPosition = soundInstance.Position.Z;
            soundInstance.Position.Z = soundInstance.Position.Y;
            soundInstance.Position.Y = yPosition;

            // Apply world scale to position
            soundInstance.Position.X *= Configuration.CONFIG_GENERATE_WORLD_SCALE;
            soundInstance.Position.Y *= Configuration.CONFIG_GENERATE_WORLD_SCALE;
            soundInstance.Position.Z *= Configuration.CONFIG_GENERATE_WORLD_SCALE;

            // Generate a unique ID
            soundInstance.GenerateGameObjectIDs();

            // Add it
            SoundInstances.Add(soundInstance);

            // Generate a model to represent it for emitting
            string objectModelName = soundInstance.GenerateModelName(ShortName, SoundInstances.Count);
            Material material = new Material("default", "default", 0, MaterialType.Diffuse, new List<string> { Configuration.CONFIG_AUDIO_SOUNDINSTANCE_RENDEROBJECT_MATERIAL_NAME }, 0, 64, 64, true);
            MeshData objectModelMesh = new MeshData();
            BoundingBox objectModelBoundingBox = new BoundingBox(new Vector3(0, 0, 0), Configuration.CONFIG_AUDIO_SOUNDINSTANCE_RENDEROBJECT_BOX_SIZE);
            if (Configuration.CONFIG_AUDIO_SOUNDINSTANCE_DRAW_AS_BOX == true)
                objectModelMesh.GenerateAsBox(objectModelBoundingBox, Convert.ToUInt16(material.Index), MeshBoxRenderType.Both);
            ObjectModel soundInstanceObjectModel = new ObjectModel(objectModelName, ObjectModelProperties.GetObjectPropertiesForObject("SoundInstance"), ObjectModelType.SoundInstance);
            soundInstanceObjectModel.Load(objectModelName, new List<Material>() { material }, objectModelMesh, new List<Vector3>(), new List<TriangleFace>());
            soundInstanceObjectModel.SoundIdleLoop = soundInstance.Sound;
            SoundInstanceObjectModels.Add(soundInstanceObjectModel);

            // Make it a doodad
            ZoneDoodadInstance doodadInstance = new ZoneDoodadInstance(ZoneDoodadInstanceType.SoundInstance);
            doodadInstance.ObjectName = objectModelName;
            doodadInstance.Position.X = soundInstance.Position.X;
            doodadInstance.Position.Z = soundInstance.Position.Z;
            doodadInstance.Position.Y = soundInstance.Position.Y;
            doodadInstance.Position.X = -doodadInstance.Position.X; // Rotate around Z axis 180 degrees
            doodadInstance.Position.Y = -doodadInstance.Position.Y; // Rotate around Z axis 180 degrees
            DoodadInstances.Add(doodadInstance);
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
