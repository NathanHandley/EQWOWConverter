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

using EQWOWConverter.Common;
using EQWOWConverter.EQFiles;
using EQWOWConverter.GameObjects;
using EQWOWConverter.ObjectModels;
using EQWOWConverter.ObjectModels.Properties;
using System.Text.RegularExpressions;

namespace EQWOWConverter.Zones
{
    internal class Zone
    {
        public string ShortName = string.Empty;
        public string DescriptiveName = string.Empty;        
        public string DescriptiveNameOnlyLetters = string.Empty;
        public ZoneEQData EQZoneData = new ZoneEQData();
        private bool IsLoaded = false;
        public List<ZoneModelObject> ZoneObjectModels = new List<ZoneModelObject>();
        public List<ObjectModel> GeneratedZoneObjects = new List<ObjectModel>();
        public List<Material> Materials = new List<Material>();
        public List<LightInstance> LightInstances = new List<LightInstance>();
        public List<ZoneDoodadInstance> DoodadInstances = new List<ZoneDoodadInstance>();
        public BoundingBox AllGeometryBoundingBox = new BoundingBox();
        public BoundingBox RenderedGeometryBoundingBox = new BoundingBox();
        public List<BoundingBox> AnimatedMaterialGeometryBoundingBoxes = new List<BoundingBox>();
        public int LoadingScreenID;
        public ZoneProperties ZoneProperties;
        public Dictionary<string, Sound> MusicSoundsByFileNameNoExt = new Dictionary<string, Sound>();
        public List<ZoneAreaMusic> ZoneAreaMusics = new List<ZoneAreaMusic>();
        public Dictionary<string, Sound> AmbientSoundsByFileNameNoExt = new Dictionary<string, Sound>();
        public List<ZoneAreaAmbientSound> ZoneAreaAmbientSounds = new List<ZoneAreaAmbientSound>();
        public ZoneArea DefaultArea;
        public List<ZoneArea> SubAreas = new List<ZoneArea>();
        public List<SoundInstance> SoundInstances = new List<SoundInstance>();
        public List<ObjectModel> SoundInstanceObjectModels = new List<ObjectModel>();

        public Zone(string shortName, string descriptiveName)
        {
            ShortName = shortName;
            SetDescriptiveName(descriptiveName);
            DefaultArea = new ZoneArea("", "");
            ZoneProperties = new ZoneProperties();
        }

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

        public void LoadFromEQZone(string inputZoneFolderName, string inputZoneFolderFullPath, List<GameObject> nonInteractiveGameObjects)
        {
            if (IsLoaded == true)
            {
                Logger.WriteInfo("LoadFromEQZone called for zone '" + ShortName + "' when the zone was already loaded");
                return;
            }

            // Load the EQ data
            EQZoneData.LoadDataFromDisk(inputZoneFolderName, inputZoneFolderFullPath);

            // Get and convert/translate the EverQuest mesh data
            MeshData renderMeshData = EQZoneData.RenderMeshData;
            renderMeshData.ApplyEQToWoWGeometryTranslationsAndScale(true, Configuration.GENERATE_WORLD_SCALE);
            renderMeshData.ApplyEQToWoWVertexColor(ZoneProperties.VertexColorIntensityOverride);
            MeshData collisionMeshData = EQZoneData.CollisionMeshData;
            collisionMeshData.ApplyEQToWoWGeometryTranslationsAndScale(true, Configuration.GENERATE_WORLD_SCALE);

            // Remove any discard parts of the geometry
            RemoveDiscardedGeometry(ref renderMeshData, ref collisionMeshData);

            // Update the materials
            Materials = EQZoneData.Materials;
            foreach (Material material in Materials)
                if (ZoneProperties.AlwaysBrightMaterialsByName.Contains(material.Name) == true)
                    material.AlwaysBrightOverride = true;
            if (ShortName == "kedge")
            {
                // Kedge Keep needs a dummy material in order for it to reder
                Material dummyMaterial = new Material(Materials[0]);
                dummyMaterial.Name = "Dummy";
                dummyMaterial.MaterialType = MaterialType.Diffuse;
                dummyMaterial.TextureNames.Clear();
                dummyMaterial.TextureNames.Add("kc1an1");
                dummyMaterial.AnimationDelayMs = 0;
                Materials.Add(dummyMaterial);
            }

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
            GenerateDoodadInstances(EQZoneData.ObjectInstances, nonInteractiveGameObjects, renderMeshData);

            // Generate the collidable areas (zone areas, liquid)
            GenerateCollidableWorldObjectModels(renderMeshData, collisionMeshData);

            // Generate the render objects
            GenerateRenderWorldObjectModels(renderMeshData);

            // Bind doodads to wmos
            AssociateDoodadsWithWMOs();

            // Save the loading screen
            SetLoadingScreen();

            // Rebuild the bounding boxes
            List<BoundingBox> allBoundingBoxes = new List<BoundingBox>();
            List<BoundingBox> allRenderedBoundingBoxes = new List<BoundingBox>();
            foreach (ZoneModelObject zoneObject in ZoneObjectModels)
            {
                allBoundingBoxes.Add(zoneObject.BoundingBox);
                if (zoneObject.WMOType == ZoneObjectModelType.Rendered)
                    allRenderedBoundingBoxes.Add(zoneObject.BoundingBox);
            }
            AllGeometryBoundingBox = BoundingBox.GenerateBoxFromBoxes(allBoundingBoxes);
            allRenderedBoundingBoxes.AddRange(AnimatedMaterialGeometryBoundingBoxes);
            RenderedGeometryBoundingBox = BoundingBox.GenerateBoxFromBoxes(allRenderedBoundingBoxes);

            // Set any area parent relationships
            SetAreaParentRelationships();

            // If set, generate a shadowbox
            if (ZoneProperties.HasShadowBox == true)
            {
                ZoneModelObject curWorldObjectModel = new ZoneModelObject(Convert.ToUInt16(ZoneObjectModels.Count), DefaultArea.DBCAreaTableID);
                curWorldObjectModel.LoadAsShadowBox(Materials, AllGeometryBoundingBox, ZoneProperties);
                ZoneObjectModels.Add(curWorldObjectModel);
            }

            // Completely loaded
            IsLoaded = true;
        }

        public void LoadAsTransportShip(ObjectModel shipModel)
        {
            if (IsLoaded == true)
            {
                Logger.WriteInfo("LoadFromEQCharacterData called for zone '" + ShortName + "' when the zone was already loaded");
                return;
            }

            // Need a dummy material so the WMO doesn't crash
            Material dummyMaterial = new Material(shipModel.ModelMaterials[0].Material);
            Materials.Add(dummyMaterial);

            // Set up collision data
            MeshData renderMeshData = shipModel.GetMeshDataByPose(true, EQAnimationType.p01StandPassive, EQAnimationType.l01Walk, EQAnimationType.posStandPose);

            // Create a doodad instance
            ZoneDoodadInstance doodadInstance = new ZoneDoodadInstance(ZoneDoodadInstanceType.StaticObject);
            doodadInstance.ObjectName = shipModel.Name;
            DoodadInstances.Add(doodadInstance);

            // Generate the collidable areas (zone areas, liquid)
            GenerateCollidableWorldObjectModels(renderMeshData, renderMeshData, true);

            // Bind doodads to wmos
            AssociateDoodadsWithWMOs();

            // Rebuild the bounding boxes
            List<BoundingBox> allBoundingBoxes = new List<BoundingBox>();
            foreach (ZoneModelObject zoneObject in ZoneObjectModels)
                allBoundingBoxes.Add(zoneObject.BoundingBox);
            AllGeometryBoundingBox = BoundingBox.GenerateBoxFromBoxes(allBoundingBoxes);

            // Set any area parent relationships
            SetAreaParentRelationships();

            // Completely loaded
            IsLoaded = true;
        }

        private void RemoveDiscardedGeometry(ref MeshData renderMeshData, ref MeshData collisionMeshData)
        {
            foreach (BoundingBox discardBox in ZoneProperties.DiscardGeometryBoxes)
            {
                MeshData discardedRenderMeshData;
                MeshData keptRenderMeshData;
                MeshData.GetSplitMeshDataWithClipping(renderMeshData, discardBox, out discardedRenderMeshData, out keptRenderMeshData);
                renderMeshData = keptRenderMeshData;

                MeshData discardedCollisionMeshData;
                MeshData keptCollisionMeshData;
                MeshData.GetSplitMeshDataWithClipping(collisionMeshData, discardBox, out discardedCollisionMeshData, out keptCollisionMeshData);
                collisionMeshData = keptCollisionMeshData;
            }
        }

        private void AssignLiquidsToAreas()
        {
            // Helper for stepping through and splitting water up in an area
            void SegmentAndAssignToArea(ref ZoneArea area, ref ZoneLiquidGroup curLiquidGroup)
            {
                // If fully contained OR a forced assignment, make it directly assigned
                if (area.MaxBoundingBox.ContainsBox(curLiquidGroup.BoundingBox) == true || area.DisplayName == curLiquidGroup.ForcedAreaAssignmentName)
                {
                    ZoneLiquidGroup fullLiquidGroup = new ZoneLiquidGroup();
                    foreach(ZoneLiquid liquidChunk in curLiquidGroup.GetLiquidChunks())
                        fullLiquidGroup.AddLiquidChunk(liquidChunk);
                    area.LiquidGroups.Add(fullLiquidGroup);
                    curLiquidGroup.ClearLiquidChunks();
                    return;
                }

                // If it's directly assigned to an area but not this one, skip
                if (curLiquidGroup.ForcedAreaAssignmentName.Length > 0)
                    return;

                // Do nothing if there is no intersection
                if (area.MaxBoundingBox.DoesIntersectBox(curLiquidGroup.BoundingBox, Configuration.GENERATE_FLOAT_EPSILON) == false)
                    return;

                // Try every box in the area
                foreach (BoundingBox areaBox in area.BoundingBoxes)
                {
                    // If it intersects, split it up
                    ZoneLiquidGroup liquidChunksOutsideAreaBox = new ZoneLiquidGroup();
                    ZoneLiquidGroup liquidChunksInsideAreaBox = new ZoneLiquidGroup();
                    if (areaBox.DoesIntersectBox(curLiquidGroup.BoundingBox, Configuration.GENERATE_FLOAT_EPSILON) == true)
                    { 
                        // Process all chunks
                        for (int i = curLiquidGroup.GetLiquidChunks().Count - 1; i >= 0; i--)
                        {
                            // Pop off the current chunk and work with it
                            ZoneLiquid liquidChunk = curLiquidGroup.GetLiquidChunks()[i];
                            curLiquidGroup.DeleteLiquidChunkAtIndex(i);

                            // Ignore the chunk if there's no intersection
                            if (areaBox.DoesIntersectBox(liquidChunk.BoundingBox, Configuration.GENERATE_FLOAT_EPSILON) == false)
                            {
                                liquidChunksOutsideAreaBox.AddLiquidChunk(liquidChunk);
                                continue;
                            }

                            // Save the chunk in the area if it can fully consume it
                            if (areaBox.ContainsBox(liquidChunk.BoundingBox) == true)
                            {
                                liquidChunksInsideAreaBox.AddLiquidChunk(liquidChunk);
                                continue;
                            }

                            // Split it up if there in as intersection
                            List<BoundingBox> intersectingBox;
                            List<BoundingBox> areaOnlyBoxes;
                            List<BoundingBox> liquidOnlyBoxes;
                            BoundingBox.SplitBoundingIntersect(areaBox, liquidChunk.BoundingBox, out intersectingBox, out areaOnlyBoxes, out liquidOnlyBoxes);
                            if (intersectingBox.Count == 0)
                                throw new Exception("Could not split up a liquid volume, as no intersecting box was found");

                            // Save the intersection box into the area as a new chunk
                            ZoneLiquid intersectionLiquid = liquidChunk.GeneratePartialFromScaledTransformedBoundingBox(intersectingBox[0]);
                            liquidChunksInsideAreaBox.AddLiquidChunk(intersectionLiquid);

                            // Put the derived chunks back into the cur liquid group for futher processing in this cycle
                            foreach (BoundingBox box in liquidOnlyBoxes)
                            {
                                ZoneLiquid liquidBox = liquidChunk.GeneratePartialFromScaledTransformedBoundingBox(box);
                                liquidChunksOutsideAreaBox.AddLiquidChunk(liquidBox);
                            }
                        }
                    }

                    // Save a group for any chunks that landed inside the box
                    if (liquidChunksInsideAreaBox.GetLiquidChunks().Count > 0)
                        area.LiquidGroups.Add(liquidChunksInsideAreaBox);

                    // Save back the remainder liquid chunks to the working group
                    if (liquidChunksOutsideAreaBox.GetLiquidChunks().Count > 0)
                        foreach (ZoneLiquid liquidChunk in liquidChunksOutsideAreaBox.GetLiquidChunks())
                            curLiquidGroup.AddLiquidChunk(liquidChunk);
                }
            }

            // Go through each liquid and find which area they belong to, and split if they fall onto a boundary
            List<ZoneLiquidGroup> liquidGroupsToAssign = ZoneProperties.LiquidGroups;
            while (liquidGroupsToAssign.Count > 0)
            {
                // Pop the first liquid and work with that
                ZoneLiquidGroup curLiquidGroup = liquidGroupsToAssign[0];
                liquidGroupsToAssign.RemoveAt(0);

                // See if it fits in any sub areas
                for (int i = 0; i < SubAreas.Count; i++)
                {
                    ZoneArea area = SubAreas[i];
                    SegmentAndAssignToArea(ref area, ref curLiquidGroup);
                }

                // If no where else, give it to the default area
                if (curLiquidGroup.GetLiquidChunks().Count > 0)
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
                    ZoneModelObject curZoneObjectModel = ZoneObjectModels[i];
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

        private void GenerateDoodadInstances(List<ObjectInstance> eqObjectInstances, List<GameObject> nonInteractiveGameObjects, MeshData renderMeshData)
        {
            // Add the game objects to the object instance list
            foreach (GameObject nonInteractiveGameObject in nonInteractiveGameObjects)
            {
                ObjectInstance goObjectInstance = new ObjectInstance();
                goObjectInstance.ModelName = nonInteractiveGameObject.GenerateModelFileNameNoExt();
                goObjectInstance.Position = nonInteractiveGameObject.Position;
                goObjectInstance.Scale = new Vector3(nonInteractiveGameObject.Scale, nonInteractiveGameObject.Scale, nonInteractiveGameObject.Scale);

                // "Heading" and "Incline" in EQ is -512 to 512 instead of -360 to 360
                float rotationDegrees = ((nonInteractiveGameObject.EQHeading / 512) * -360f); // Reverse for orientation handiness difference
                float tiltInDegrees = (nonInteractiveGameObject.EQIncline / 512) * 360;
                goObjectInstance.Rotation = new Vector3(0, rotationDegrees, tiltInDegrees);
                
                eqObjectInstances.Add(goObjectInstance);
            }

            // Create doodad instances from EQ object instances
            foreach (ObjectInstance objectInstance in eqObjectInstances)
            {
                string modelName = objectInstance.ModelName;
                ObjectModelProperties objectProperties = ObjectModelProperties.GetObjectPropertiesForObject(modelName.ToLower());

                // Handle model swaps
                if (objectProperties.AlternateModelSwapName.Length > 0)
                    modelName = objectProperties.AlternateModelSwapName;

                // Skip any invalid instances
                if (ObjectModel.StaticObjectModelsByName.ContainsKey(modelName) == false)
                {
                    Logger.WriteDebug("WARNING (or maybe Error): Could not generate doodad instance since model '" + modelName + "' does not exist.  Either is was missing on export, or you need to generate objects");
                    continue;
                }

                ZoneDoodadInstance doodadInstance = new ZoneDoodadInstance(ZoneDoodadInstanceType.StaticObject);
                doodadInstance.ObjectName = modelName;
                doodadInstance.Position.X = objectInstance.Position.X * Configuration.GENERATE_WORLD_SCALE;
                // Invert Z and Y because of mapping differences
                doodadInstance.Position.Z = objectInstance.Position.Y * Configuration.GENERATE_WORLD_SCALE;
                doodadInstance.Position.Y = objectInstance.Position.Z * Configuration.GENERATE_WORLD_SCALE;

                // Also rotate the X and Y positions around Z axis 180 degrees
                doodadInstance.Position.X = -doodadInstance.Position.X;
                doodadInstance.Position.Y = -doodadInstance.Position.Y;

                // Skip it if it's in a discarded geometry section
                bool skipDoodad = false;
                foreach (BoundingBox discardGeometryBox in ZoneProperties.DiscardGeometryBoxes)
                {
                    if (discardGeometryBox.ContainsPoint(doodadInstance.Position) == true)
                    {
                        skipDoodad = true;
                        continue;
                    }
                }
                // Also skip if it's a special case for map generation
                if (Configuration.WORLDMAP_DEBUG_GENERATION_MODE_ENABLED == true)
                {
                    foreach (BoundingBox discardGeometryBox in ZoneProperties.DiscardGeometryBoxesMapGenOnly)
                    {
                        if (discardGeometryBox.ContainsPoint(doodadInstance.Position) == true)
                        {
                            skipDoodad = true;
                            continue;
                        }
                    }
                    if (objectProperties.IncludeInMinimapGeneration == false)
                    {
                        skipDoodad = true;
                        continue;
                    }
                }
                if (skipDoodad == true)
                    continue;

                // Calculate the rotation (WMO)
                float rotateYaw = Convert.ToSingle(Math.PI / 180) * -objectInstance.Rotation.Z;
                float rotatePitch = Convert.ToSingle(Math.PI / 180) * objectInstance.Rotation.X;
                float rotateRoll = Convert.ToSingle(Math.PI / 180) * objectInstance.Rotation.Y;
                System.Numerics.Quaternion rotationQ = System.Numerics.Quaternion.CreateFromYawPitchRoll(rotateYaw, rotatePitch, rotateRoll);
                doodadInstance.WMOOrientation.X = rotationQ.X;
                doodadInstance.WMOOrientation.Y = rotationQ.Y;
                doodadInstance.WMOOrientation.Z = rotationQ.Z;
                doodadInstance.WMOOrientation.W = -rotationQ.W; // Flip the sign for handedness

                // Calculate the rotation (ADT)
                doodadInstance.ADTRotation.X = objectInstance.Rotation.Z;
                doodadInstance.ADTRotation.Y = -1 * objectInstance.Rotation.Y;
                doodadInstance.ADTRotation.Z = objectInstance.Rotation.X;

                // Scale is confirmed to always have the same value in x, y, z
                doodadInstance.Scale = objectInstance.Scale.X;

                // Add it
                DoodadInstances.Add(doodadInstance);
            }

            // Determine which materials are animated or transparent and create objects to represent them
            if (Configuration.WORLDMAP_DEBUG_GENERATION_MODE_ENABLED == false)
            {
                foreach (Material material in Materials)
                    if ((material.IsAnimated() || material.HasTransparency()) && material.IsRenderable())
                        GenerateAndAddDoodadsForZoneMaterial(material, renderMeshData);
            }            

            // If enabled, show light instances as torches for debugging
            if (Configuration.LIGHT_INSTANCES_DRAWN_AS_TORCHES == true)
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
            if (Configuration.LIGHT_INSTANCES_ENABLED == true)
            {
                LightInstances = eqLightInstances;

                // Correct light instance data
                foreach (LightInstance lightInstance in LightInstances)
                {
                    Vector3 originalPosition = new Vector3(lightInstance.Position);
                    lightInstance.Position.X = originalPosition.X * Configuration.GENERATE_WORLD_SCALE;
                    // Invert Z and Y because of mapping differences
                    lightInstance.Position.Z = originalPosition.Y * Configuration.GENERATE_WORLD_SCALE;
                    lightInstance.Position.Y = originalPosition.Z * Configuration.GENERATE_WORLD_SCALE;

                    // Also rotate the X and Y positions around Z axis 180 degrees
                    lightInstance.Position.X = -lightInstance.Position.X;
                    lightInstance.Position.Y = -lightInstance.Position.Y;
                }
            }
        }

        private void GenerateCollidableWorldObjectModels(MeshData renderMeshData, MeshData collisionMeshData, bool forceSkipObjectCollision = false)
        {
            if (Configuration.ZONE_COLLISION_ENABLED == false)
                return;

            // Special logic for making all render geometry collidable
            if (Configuration.ZONE_ENABLE_COLLISION_ON_ALL_ZONE_RENDER_MATERIALS == true)
            {
                collisionMeshData = new MeshData(renderMeshData);
            }

            // Determine if preset collision mesh data should be used, or if the render data should be used to generate it
            else if (collisionMeshData.Vertices.Count == 0 || collisionMeshData.TriangleFaces.Count == 0)
            {
                Logger.WriteDebug("For zone '" + ShortName + "', collision is generated from rendermesh");
                collisionMeshData = new MeshData(renderMeshData);
            }
            else
            {
                Logger.WriteDebug("For zone '" + ShortName + "', collision is generated from defined collision mesh");
            }

            // Grab the collision data from the doodads and bake into the zone's collision map
            if (Configuration.GENERATE_OBJECTS == true && ObjectModel.StaticObjectModelsByName.Count != 0 && forceSkipObjectCollision == false)
            {
                MeshData allObjectMeshData = new MeshData();
                foreach (ZoneDoodadInstance doodadInstance in DoodadInstances)
                {
                    // Only work with the static ones for the bake-in
                    if (doodadInstance.DoodadType != ZoneDoodadInstanceType.StaticObject)
                        continue;

                    // Get the object mesh data
                    ObjectModel curObject = ObjectModel.StaticObjectModelsByName[doodadInstance.ObjectName];
                    MeshData objectMeshData = new MeshData();
                    objectMeshData.Vertices = new List<Vector3>(curObject.CollisionPositions);
                    objectMeshData.TriangleFaces = new List<TriangleFace>(curObject.CollisionTriangles);

                    // Fill out placeholders (TODO: Figure out how to remove this)
                    for (int i = 0; i < objectMeshData.Vertices.Count; i++)
                        objectMeshData.TextureCoordinates.Add(new TextureCoordinates(0, 0));
                    for (int i = 0; i < objectMeshData.Vertices.Count; i++)
                        objectMeshData.VertexColors.Add(new ColorRGBA(0, 0, 0, 0));

                    // Perform transformatinos
                    objectMeshData.ApplyRotationOnVertices(doodadInstance.WMOOrientation);
                    objectMeshData.ApplyScaleOnVertices(doodadInstance.Scale);
                    objectMeshData.ApplyTranslationOnVertices(doodadInstance.Position);

                    // Add it to the collision map
                    allObjectMeshData.AddMeshData(objectMeshData);
                }

                // Append it
                collisionMeshData.AddMeshData(allObjectMeshData);
            }

            // Create normals for all all collision data
            collisionMeshData.Normals.Clear();
            for (int i = 0; i < collisionMeshData.Vertices.Count; i++)
                collisionMeshData.Normals.Add(new Vector3());
            foreach (TriangleFace face in collisionMeshData.TriangleFaces)
            {
                // Get the vertex positions for the triangle
                Vector3 v1 = collisionMeshData.Vertices[face.V1];
                Vector3 v2 = collisionMeshData.Vertices[face.V2];
                Vector3 v3 = collisionMeshData.Vertices[face.V3];
                Vector3 normal = Vector3.CalculateNormalFromVectors(v1, v2, v3);

                // Accumulate the face normal into the vertex normals
                collisionMeshData.Normals[face.V1] += normal;
                collisionMeshData.Normals[face.V2] += normal;
                collisionMeshData.Normals[face.V3] += normal;
            }

            // Constrain the maximum collision area, if set
            if (ZoneProperties.CollisionMaxZ != 0)
            {
                float scaledCollisionMaxZ = ZoneProperties.CollisionMaxZ * Configuration.GENERATE_WORLD_SCALE;
                BoundingBox removalBoundingArea = BoundingBox.GenerateBoxFromVectors(collisionMeshData.Vertices, Configuration.GENERATE_ADDED_BOUNDARY_AMOUNT);
                removalBoundingArea.BottomCorner.Z = scaledCollisionMaxZ;
                if (removalBoundingArea.BottomCorner.Z > removalBoundingArea.TopCorner.Z)
                    Logger.WriteError(string.Concat("Error attempting to constrain max collision area for zone ", ZoneProperties.ShortName, " as the Z value was higher than the top"));
                else
                {
                    MeshData discardedMeshData;
                    MeshData keptMeshData;
                    MeshData.GetSplitMeshDataWithClipping(collisionMeshData, removalBoundingArea, out discardedMeshData, out keptMeshData);
                    collisionMeshData = keptMeshData;
                }
            }

            // Helper for clipping operations below
            void GenerateLiquidCollisionAreas(ZoneArea zoneArea, ZoneLiquidGroup liquidGroup)
            {
                // Clip out all geoemetry for the liquid group and use that same collision geometry for every liquid entry
                MeshData liquidMeshData;
                MeshData remainderMeshData;
                MeshData.GetSplitMeshDataWithClipping(collisionMeshData, liquidGroup.BoundingBox, out liquidMeshData, out remainderMeshData);
                collisionMeshData = remainderMeshData;
                if (liquidGroup.GetLiquidChunks().Count == 1)
                    GenerateCollisionWorldObjectModelsForCollidableArea(liquidMeshData, zoneArea, liquidGroup.GetLiquidChunks()[0]);
                else
                {
                    foreach (ZoneLiquid liquid in liquidGroup.GetLiquidChunks())
                    {
                        MeshData liquidChunkMeshData;
                        MeshData.GetSplitMeshDataWithClipping(liquidMeshData, liquid.BoundingBox, out liquidChunkMeshData, out remainderMeshData);
                        GenerateCollisionWorldObjectModelsForCollidableArea(liquidChunkMeshData, zoneArea, liquid);
                        liquidMeshData = remainderMeshData;
                    }
                    collisionMeshData.AddMeshData(liquidMeshData);
                }
            }

            // Build collision areas based on zone areas
            foreach (ZoneArea subArea in ZoneProperties.ZoneAreas)
            {
                // Generate collision areas for each liquid group in the area
                foreach (ZoneLiquidGroup liquidGroup in subArea.LiquidGroups)
                    GenerateLiquidCollisionAreas(subArea, liquidGroup);

                // Pull out the most mesh data needed to make this sub area, and create if there is only one subbox
                MeshData maxAreaMeshData;
                MeshData maxAreaRemainderMeshData;
                MeshData.GetSplitMeshDataWithClipping(collisionMeshData, subArea.MaxBoundingBox, out maxAreaMeshData, out maxAreaRemainderMeshData);
                collisionMeshData = maxAreaRemainderMeshData;
                if (subArea.BoundingBoxes.Count == 1)
                    GenerateCollisionWorldObjectModelsForCollidableArea(maxAreaMeshData, subArea, null);
                else
                {
                    // Areas can have multiple boxes, so collect it up
                    MeshData areaMeshDataFull = new MeshData();
                    foreach (BoundingBox areaSubBoundingBox in subArea.BoundingBoxes)
                    {
                        MeshData areaMeshDataBox;
                        MeshData remainderMeshData;
                        MeshData.GetSplitMeshDataWithClipping(maxAreaMeshData, areaSubBoundingBox, out areaMeshDataBox, out remainderMeshData);
                        maxAreaMeshData = remainderMeshData;
                        areaMeshDataFull.AddMeshData(areaMeshDataBox);
                    }
                    GenerateCollisionWorldObjectModelsForCollidableArea(areaMeshDataFull, subArea, null);
                    collisionMeshData.AddMeshData(maxAreaMeshData);
                }
            }

            // Generate collision areas for each liquid group in the default area
            foreach (ZoneLiquidGroup liquidGroup in DefaultArea.LiquidGroups)
                GenerateLiquidCollisionAreas(DefaultArea, liquidGroup);

            // Everything left goes to the default area
            DefaultArea.AddBoundingBox(BoundingBox.GenerateBoxFromVectors(collisionMeshData.Vertices, Configuration.GENERATE_ADDED_BOUNDARY_AMOUNT), false);
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
            List<MeshData> meshDataChunks = MeshData.GetGeometrySplitIntoCubiods(collisionMeshData, Configuration.ZONE_SPLIT_COLLISION_CUBOID_MAX_EDGE_LEGNTH, Configuration.ZONE_MAX_FACES_PER_WMOGROUP);

            // Build the bounding box, which must always be at least as high as the liquid
            BoundingBox boundingBox = BoundingBox.GenerateBoxFromVectors(collisionMeshData.Vertices, Configuration.GENERATE_ADDED_BOUNDARY_AMOUNT);
            if (liquid != null)
                boundingBox.TopCorner.Z = float.Max(boundingBox.TopCorner.Z, liquid.BoundingBox.TopCorner.Z);

            // Force a data chunk if there is still liquid
            if (meshDataChunks.Count == 0 && liquid != null)
            {
                meshDataChunks.Add(new MeshData());
                boundingBox = new BoundingBox(liquid.BoundingBox);
            }

            // Create a group for each chunk
            foreach (MeshData meshDataChunk in meshDataChunks)
            {
                // If there are multiple chunks and liquid, break up the liquid
                ZoneLiquid? chunkLiquid = liquid;
                if (meshDataChunks.Count > 1 && chunkLiquid != null)
                {
                    // If liquid and geometry are both here, then the geometry's bounding top must be at least as high as the liquid
                    boundingBox = BoundingBox.GenerateBoxFromVectors(meshDataChunk.Vertices, Configuration.GENERATE_ADDED_BOUNDARY_AMOUNT);
                    boundingBox.TopCorner.Z = float.Max(boundingBox.TopCorner.Z, chunkLiquid.BoundingBox.TopCorner.Z);

                    // Find out the intersecting liquid areas
                    List<BoundingBox> intersectingBox;
                    List<BoundingBox> chunkOnlyBoxes;
                    List<BoundingBox> liquidOnlyBoxes;
                    BoundingBox.SplitBoundingIntersect(boundingBox, chunkLiquid.BoundingBox, out intersectingBox, out chunkOnlyBoxes, out liquidOnlyBoxes);

                    // Generate and use only the intersecting subsection of the liquid
                    chunkLiquid = chunkLiquid.GeneratePartialFromScaledTransformedBoundingBox(intersectingBox[0]);
                }

                ZoneModelObject curWorldObjectModel = new ZoneModelObject(Convert.ToUInt16(ZoneObjectModels.Count), zoneArea.DBCAreaTableID);
                meshDataChunk.CondenseAndRenumberVertexIndices();
                curWorldObjectModel.LoadAsCollidableArea(meshDataChunk, boundingBox, zoneArea.DisplayName, areaMusic, chunkLiquid);
                ZoneObjectModels.Add(curWorldObjectModel);
            }
        }

        private void GenerateRenderWorldObjectModels(MeshData renderMeshData)
        {
            bool excludeAnimatedAndTransparent = true;
            if (Configuration.WORLDMAP_DEBUG_GENERATION_MODE_ENABLED == true)
                excludeAnimatedAndTransparent = false;

            // Reduce meshdata to what will actually be rendered
            MeshData staticMeshData = renderMeshData.GetMeshDataExcludingNonRenderedAndAnimatedMaterials(true, excludeAnimatedAndTransparent, Materials.ToArray());

            // If set, show the area box
            if (Configuration.ZONE_DRAW_COLLIDABLE_SUB_AREAS_AS_BOXES == true)
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
            List<MeshData> meshDataChunks = MeshData.GetGeometrySplitIntoCubiods(staticMeshData, 0, Configuration.ZONE_MAX_FACES_PER_WMOGROUP);

            // Create a group for each chunk
            foreach (MeshData meshDataChunk in meshDataChunks)
            {
                ZoneModelObject curWorldObjectModel = new ZoneModelObject(Convert.ToUInt16(ZoneObjectModels.Count), DefaultArea.DBCAreaTableID);
                meshDataChunk.CondenseAndRenumberVertexIndices();
                curWorldObjectModel.LoadAsRendered(meshDataChunk, Materials);
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
                        LoadingScreenID = Configuration.DBCID_LOADINGSCREEN_ID_START;
                    }
                    break;
                case ZoneContinentType.Kunark:
                    {
                        LoadingScreenID = Configuration.DBCID_LOADINGSCREEN_ID_START + 1;
                    }
                    break;
                case ZoneContinentType.Velious:
                    {
                        LoadingScreenID = Configuration.DBCID_LOADINGSCREEN_ID_START + 2;
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

            if (curMaterialMeshData.Vertices.Count == 0)
            {
                Logger.WriteDebug("GenerateAndAddDoodadsForZoneMaterial in zone ", ShortName, " had no mesh vertices for material index ", material.Index.ToString(), ", so skipping.");
                return;
            }
            AnimatedMaterialGeometryBoundingBoxes.Add(curMeshBoundingBox);

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
                ObjectModel newObject = new ObjectModel(name, ObjectModelProperties.GetObjectPropertiesForObject(""), ObjectModelType.ZoneModel, Configuration.GENERATE_OBJECT_MODEL_MIN_BOUNDARY_BOX_SIZE);
                newObject.Load(new List<Material> { new Material(material) }, curMeshData, new List<Vector3>(), new List<TriangleFace>());
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
                    Logger.WriteDebug("For zone '" + ShortName + "', skipping 3D sound instance which has no file name for the day sound");
                else if (soundInstance3D.SoundFileNameDayNoExt != soundInstance3D.SoundFileNameNightNoExt)
                    Logger.WriteDebug("For zone '" + ShortName + "', skipping 3D sound instance which has mismatched day and night of '" + soundInstance3D.SoundFileNameDayNoExt + "' and '" + soundInstance3D.SoundFileNameNightNoExt + "'");
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
            float radius = soundInstance.Radius * Configuration.GENERATE_WORLD_SCALE;
            float minDistance = radius;
            if (soundInstance.Is2DSound)
                minDistance *= Configuration.AUDIO_SOUNDINSTANCE_2D_MIN_DISTANCE_MOD;
            else
                minDistance *= Configuration.AUDIO_SOUNDINSTANCE_3D_MIN_DISTANCE_MOD;

            soundInstance.Sound = new Sound(soundInstance.GenerateDBCName(ShortName, SoundInstances.Count), soundInstance.SoundFileNameDayNoExt,
                SoundType.GameObject, minDistance, radius, true);

            //  Flip Y and Z
            float yPosition = soundInstance.Position.Z;
            soundInstance.Position.Z = soundInstance.Position.Y;
            soundInstance.Position.Y = yPosition;

            // Apply world scale to position
            soundInstance.Position.X *= Configuration.GENERATE_WORLD_SCALE;
            soundInstance.Position.Y *= Configuration.GENERATE_WORLD_SCALE;
            soundInstance.Position.Z *= Configuration.GENERATE_WORLD_SCALE;

            // Add it
            SoundInstances.Add(soundInstance);

            // Generate a model to represent it for emitting
            string objectModelName = soundInstance.GenerateModelName(ShortName, SoundInstances.Count);
            Material material = new Material("default", "default", 0, MaterialType.Diffuse, new List<string> { Configuration.AUDIO_SOUNDINSTANCE_RENDEROBJECT_MATERIAL_NAME }, 0, 64, 64, true);
            MeshData objectModelMesh = new MeshData();
            if (Configuration.AUDIO_SOUNDINSTANCE_DRAW_AS_BOX == true)
            {
                BoundingBox objectModelBoundingBox = new BoundingBox(new Vector3(0, 0, 0), Configuration.AUDIO_SOUNDINSTANCE_RENDEROBJECT_BOX_SIZE);
                objectModelMesh.GenerateAsBox(objectModelBoundingBox, Convert.ToUInt16(material.Index), MeshBoxRenderType.Both);
            }
            ObjectModel soundInstanceObjectModel = new ObjectModel(objectModelName, ObjectModelProperties.GetObjectPropertiesForObject("SoundInstance"), ObjectModelType.SoundInstance, Configuration.GENERATE_OBJECT_MODEL_MIN_BOUNDARY_BOX_SIZE);
            soundInstanceObjectModel.Load(new List<Material>() { material }, objectModelMesh, new List<Vector3>(), new List<TriangleFace>());
            if (soundInstance.Sound != null)
                soundInstanceObjectModel.SoundsByAnimationType.Add(AnimationType.Stand, soundInstance.Sound);
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
