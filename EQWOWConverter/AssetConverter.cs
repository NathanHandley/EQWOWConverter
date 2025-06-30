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
using EQWOWConverter.Creatures;
using EQWOWConverter.Items;
using EQWOWConverter.GameObjects;
using EQWOWConverter.ObjectModels;
using EQWOWConverter.ObjectModels.Properties;
using EQWOWConverter.Player;
using EQWOWConverter.Quests;
using EQWOWConverter.Spells;
using EQWOWConverter.Tradeskills;
using EQWOWConverter.Transports;
using EQWOWConverter.WOWFiles;
using EQWOWConverter.Zones;
using MySql.Data.MySqlClient;
using System.Text;

namespace EQWOWConverter
{
    internal class AssetConverter
    {
        private static List<string> ZoneShortNamesToProcess = new List<string>();
        private static readonly object ZoneLock = new object();

        public bool ConvertEQDataToWOW()
        {
            Logger.WriteInfo("Converting from EQ to WoW...");
            Logger.WriteInfo("- Note: CORE_ENABLE_MULTITHREADING is " + Configuration.CORE_ENABLE_MULTITHREADING.ToString());

            // Verify Input Path
            if (Directory.Exists(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER) == false)
            {
                Logger.WriteError("Error - Conditioned path of '" + Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER + "' does not exist.");
                Logger.WriteError("Conversion Failed!");
                return false;
            }

            // Extract
            if (Configuration.EXTRACT_DBC_FILES == true)
                ExtractClientDBCFiles();
            else
                Logger.WriteInfo("- Note: DBC File Extraction is set to false in the Configuration");

            // Thread 1: Objects and Zones
            List<Zone> zones = new List<Zone>();
            Task zoneAndObjectTask = Task.Factory.StartNew(() =>
            {
                Logger.WriteInfo("<+> Thread [Zone and Objects] Started");

                // Objects (must always come before zones)
                if (Configuration.GENERATE_OBJECTS == true)
                    ConvertEQObjectsToWOW();

                // Zones
                ConvertEQZonesToWOW(out zones);
                Logger.WriteInfo("<-> Thread [Zone and Objects] Ended");
            }, TaskCreationOptions.LongRunning);
            if (Configuration.CORE_ENABLE_MULTITHREADING == false)
                zoneAndObjectTask.Wait();

            // Thread 2: Creatures, Transports and Spawns
            List<CreatureTemplate> creatureTemplates = new List<CreatureTemplate>();
            Dictionary<int, CreatureTemplate> creatureTemplatesByEQID = new Dictionary<int, CreatureTemplate>();
            List<CreatureModelTemplate> creatureModelTemplates = new List<CreatureModelTemplate>();
            List<CreatureSpawnPool> creatureSpawnPools = new List<CreatureSpawnPool>();
            Task creaturesAndSpawnsTask = Task.Factory.StartNew(() =>
            {
                Logger.WriteInfo("<+> Thread [Creatures, Transports, and Spawns] Started");

                // Creatures                
                if (Configuration.GENERATE_CREATURES_AND_SPAWNS == true)
                {
                    ConvertCreatures(ref creatureModelTemplates, ref creatureTemplates, ref creatureSpawnPools);
                    creatureTemplatesByEQID = CreatureTemplate.GetCreatureTemplateListByEQID();
                }
                else
                    Logger.WriteInfo("- Note: Creature generation is set to false in the Configuration");

                // Transports (make sure it's after zones)
                if (Configuration.GENERATE_TRANSPORTS == true)
                    ConvertTransports();
                else
                    Logger.WriteInfo("- Note: Transport generation is set to false in the Configuration");

                Logger.WriteInfo("<-> Thread [Creatures, Transports, and Spawns] Ended");
            }, TaskCreationOptions.LongRunning);
            if (Configuration.CORE_ENABLE_MULTITHREADING == false)
                creaturesAndSpawnsTask.Wait();

            // Thread 3: Items, Spells, Tradeskills, GameObjects
            List<SpellTemplate> spellTemplates = new List<SpellTemplate>();
            List<TradeskillRecipe> tradeskillRecipes = new List<TradeskillRecipe>();
            SortedDictionary<int, ItemTemplate> itemTemplatesByEQDBID = new SortedDictionary<int, ItemTemplate>();
            Task itemsSpellsTradeskillsTask = Task.Factory.StartNew(() =>
            {
                Logger.WriteInfo("<+> Thread [Items, Spells, Tradeskills] Started");

                // Generate item templates
                Logger.WriteInfo("Generating item templates...");
                itemTemplatesByEQDBID = ItemTemplate.GetItemTemplatesByEQDBIDs();
                SortedDictionary<int, ItemTemplate> itemTemplatesByWOWEntryID = ItemTemplate.GetItemTemplatesByWOWEntryID();
                foreach (var classRaceProperties in PlayerClassRaceProperties.GetClassRacePropertiesByRaceAndClassID())
                    foreach (int itemID in classRaceProperties.Value.StartItemIDs)
                        if (itemTemplatesByWOWEntryID.ContainsKey(itemID) == true)
                            itemTemplatesByWOWEntryID[itemID].IsGivenAsStartItem = true;

                // Spells                                        
                GenerateSpells(out spellTemplates);

                // Tradeskills
                GenerateTradeskills(itemTemplatesByEQDBID, ref spellTemplates, out tradeskillRecipes);

                Logger.WriteInfo("<-> Thread [Items, Spells, Tradeskills] Ended");
            }, TaskCreationOptions.LongRunning);
            if (Configuration.CORE_ENABLE_MULTITHREADING == false)
                itemsSpellsTradeskillsTask.Wait();

            // Wait for some of the threads above to complete
            if (Configuration.CORE_ENABLE_MULTITHREADING == true)
            {
                creaturesAndSpawnsTask.Wait();
                itemsSpellsTradeskillsTask.Wait();
            }            

            // Loot
            Dictionary<int, List<ItemLootTemplate>> itemLootTemplatesByCreatureTemplateID;
            ConvertLoot(creatureTemplates, out itemLootTemplatesByCreatureTemplateID);

            // Update vendor references for future culling
            Dictionary<int, List<CreatureVendorItem>> vendorItems = CreatureVendorItem.GetCreatureVendorItemsByMerchantIDs();
            foreach (var vendorItemList in vendorItems.Values)
                foreach (CreatureVendorItem vendorItem in vendorItemList)
                    if (itemTemplatesByEQDBID.ContainsKey(vendorItem.EQItemID) == true)
                        itemTemplatesByEQDBID[vendorItem.EQItemID].IsSoldByVendor = true;

            // Quests
            List<QuestTemplate> questTemplates = new List<QuestTemplate>();
            if (Configuration.GENERATE_QUESTS == true)
                ConvertQuestItems(itemTemplatesByEQDBID, out questTemplates);
            else
                Logger.WriteInfo("- Note: Quest generation is set to false in the Configuration");

            // If there are any neutral creatures that should be interactive, remap
            RemapDefaultFactionsForInteractiveCreatures(ref creatureTemplates);

            // If there are any non player obtainable things (spells, items), clear them out
            SortedDictionary<int, ItemTemplate> itemTemplatesByWOWEntryID = ItemTemplate.GetItemTemplatesByWOWEntryID();
            ClearNonPlayerObtainableItemsAndRecipes(ref tradeskillRecipes, ref itemTemplatesByWOWEntryID);

            // Finish the items
            CreateItemGraphics(ref itemTemplatesByEQDBID);

            // Quests Finish-up
            if (Configuration.GENERATE_QUESTS == true)
                ConvertQuests(itemTemplatesByEQDBID, ref questTemplates, ref creatureTemplates);

            // Make sure zones are done
            if (Configuration.CORE_ENABLE_MULTITHREADING == true)
                zoneAndObjectTask.Wait();

            // Create the DBC files
            CreateDBCFiles(zones, creatureModelTemplates, spellTemplates);

            // Thread 1: Client
            Task clientBuildAndDeployTask = Task.Factory.StartNew(() =>
            {
                Logger.WriteInfo("<+> Thread [Client Build and Deploy] Started");

                // Icons
                CopyIconFiles();

                // Copy the liquid material textures
                CreateLiquidMaterials();

                // Copy the loading screens
                CreateLoadingScreens();

                // Create or update the MPQ
                string exportMPQFileName = Path.Combine(Configuration.PATH_EXPORT_FOLDER, Configuration.PATH_PATCH_NEW_FILE_NAME_NO_EXT + ".mpq");
                if (Configuration.GENERATE_ONLY_LISTED_ZONE_SHORTNAMES.Count == 0 || File.Exists(exportMPQFileName) == false)
                    CreatePatchMPQ();
                else
                    UpdatePatchMPQ();

                // Deploy 
                if (Configuration.DEPLOY_CLIENT_FILES == true)
                {
                    DeployClient();
                    if (Configuration.DEPLOY_CLEAR_CACHE_ON_CLIENT_DEPLOY == true)
                        ClearClientCache();
                }
                else
                    Logger.WriteInfo("- Note: DEPLOY_CLIENT_FILES set false in the Configuration");

                Logger.WriteInfo("<-> Thread [Client Build and Deploy] Ended");
            }, TaskCreationOptions.LongRunning);
            if (Configuration.CORE_ENABLE_MULTITHREADING == false)
                clientBuildAndDeployTask.Wait();

            // Thead 2: Server
            Task serverDeployTask = Task.Factory.StartNew(() =>
            {
                Logger.WriteInfo("<+> Thread [Server Build and Deploy] Started");

                // Create the SQL Scripts (note: this must always be after DBC files)
                CreateSQLScript(zones, creatureTemplates, creatureModelTemplates, creatureSpawnPools, itemLootTemplatesByCreatureTemplateID, questTemplates, tradeskillRecipes);

                if (Configuration.DEPLOY_SERVER_FILES == true)
                    DeployServerFiles();
                else
                    Logger.WriteInfo("- Note: DEPLOY_SERVER_FILES set false in the Configuration");
                if (Configuration.DEPLOY_SERVER_SQL == true)
                    DeployServerSQL();
                else
                    Logger.WriteInfo("- Note: DEPLOY_SERVER_SQL set false in the Configuration");

                Logger.WriteInfo("<-> Thread [Server Build and Deploy] Ended");
            }, TaskCreationOptions.LongRunning);
            if (Configuration.CORE_ENABLE_MULTITHREADING == false)
                serverDeployTask.Wait();

            // Wait for threads above to complete
            if (Configuration.CORE_ENABLE_MULTITHREADING == true)
            {
                clientBuildAndDeployTask.Wait();
                serverDeployTask.Wait();
            }

            Logger.WriteInfo("Conversion of data complete");
            return true;
        }

        public void ConvertTransports()
        {
            Logger.WriteInfo("Converting Transports...");
            string eqExportsConditionedPath = Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER;
            string exportMPQRootFolder = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "MPQReady");
            string relativeStaticDoodadsPath = Path.Combine("World", "Everquest", "StaticDoodads");

            // Ships
            Logger.WriteDebug("Loading transport ships...");
            List<TransportShip> transportShips = TransportShip.GetAllTransportShips();
            Dictionary<string, int> gameObjectDisplayInfoIDsByMeshName = new Dictionary<string, int>();
            Dictionary<string, Zone> transportShipZoneModelsByMeshName = new Dictionary<string, Zone>();
            string charactersFolderRoot = Path.Combine(eqExportsConditionedPath, "characters");
            string objectsFolderRoot = Path.Combine(eqExportsConditionedPath, "objects");
            foreach (TransportShip transportShip in transportShips)
            {
                // Load the mesh if it hasen't been yet
                if (transportShipZoneModelsByMeshName.ContainsKey(transportShip.MeshName) == false)
                {
                    // Load it
                    Zone curZone = new Zone(transportShip.MeshName, transportShip.Name);
                    Logger.WriteDebug("- [" + transportShip.MeshName + "]: Importing EQ transport ship object '" + transportShip.MeshName + "'");
                    curZone.LoadFromEQObject(transportShip.MeshName, charactersFolderRoot);
                    Logger.WriteDebug("- [" + transportShip.MeshName + "]: Importing EQ transport ship object '" + transportShip.MeshName + "' complete");

                    // Convert to WMO
                    string relativeTransportObjectsPath = Path.Combine("World", "Everquest", "TransportObjects", transportShip.MeshName);
                    WMO transportWMO = new WMO(curZone, exportMPQRootFolder, "WORLD\\EVERQUEST\\TRANSPORTTEXTURES", relativeStaticDoodadsPath, relativeTransportObjectsPath, true);
                    transportWMO.WriteToDisk();

                    // Copy the textures
                    string transportOutputTextureFolder = Path.Combine(exportMPQRootFolder, "World", "Everquest", "TransportTextures", transportShip.MeshName);
                    if (Directory.Exists(transportOutputTextureFolder) == false)
                        FileTool.CreateBlankDirectory(transportOutputTextureFolder, true);
                    foreach (Material material in curZone.Materials)
                    {
                        foreach (string textureName in material.TextureNames)
                        {
                            string sourceTextureFullPath = Path.Combine(charactersFolderRoot, "Textures", string.Concat(textureName, ".blp"));
                            string outputTextureFullPath = Path.Combine(transportOutputTextureFolder, string.Concat(textureName, ".blp"));
                            if (File.Exists(sourceTextureFullPath) == false)
                            {
                                Logger.WriteError("Could not copy texture '" + sourceTextureFullPath + "', it did not exist. Did you run blpconverter?");
                                continue;
                            }
                            FileTool.CopyFile(sourceTextureFullPath, outputTextureFullPath);
                            Logger.WriteDebug(string.Concat("- [", transportShip.Name, "]: Texture named '", textureName, "' copied"));
                        }
                    }

                    int gameObjectDisplayInfoID = GameObjectDisplayInfoDBC.GenerateID();
                    transportShipZoneModelsByMeshName.Add(transportShip.MeshName, curZone);
                    gameObjectDisplayInfoIDsByMeshName.Add(transportShip.MeshName, gameObjectDisplayInfoID);
                    TransportShip.TransportShipWMOsByGameObjectDisplayInfoID.Add(gameObjectDisplayInfoID, transportWMO);
                }
                transportShip.GameObjectDisplayInfoID = gameObjectDisplayInfoIDsByMeshName[transportShip.MeshName];
            }

            // Lifts
            Logger.WriteDebug("Loading transport lifts...");
            List<TransportLift> transportLifts = TransportLift.GetAllTransportLifts();
            gameObjectDisplayInfoIDsByMeshName.Clear();
            Dictionary<string, ObjectModel> transportLiftObjectModelsByMeshName = new Dictionary<string, ObjectModel>();
            foreach (TransportLift transportLift in transportLifts)
            {
                // Load the object mesh if it hasn't been yet
                if (transportLiftObjectModelsByMeshName.ContainsKey(transportLift.MeshName) == false)
                {
                    // Different lift types have different source mesh data
                    string folderRoot = objectsFolderRoot;
                    if (transportLift.IsSourceSkeletal == true)
                        folderRoot = charactersFolderRoot;

                    // Load it
                    ObjectModel curObjectModel = new ObjectModel(transportLift.MeshName, new ObjectModelProperties(), ObjectModelType.StaticDoodad, Configuration.GENERATE_OBJECT_MODEL_MIN_BOUNDARY_BOX_SIZE);
                    Logger.WriteDebug("- [" + transportLift.MeshName + "]: Importing EQ transport lift object '" + transportLift.MeshName + "'");
                    curObjectModel.LoadEQObjectFromFile(folderRoot, transportLift.MeshName);
                    Logger.WriteDebug("- [" + transportLift.MeshName + "]: Importing EQ transport lift object '" + transportLift.MeshName + "' complete");

                    // Create the M2 and Skin
                    string relativeMPQPath = Path.Combine("World", "Everquest", "Transports", transportLift.MeshName);
                    M2 objectM2 = new M2(curObjectModel, relativeMPQPath);
                    string curStaticObjectOutputFolder = Path.Combine(exportMPQRootFolder, "World", "Everquest", "Transports", transportLift.MeshName);
                    objectM2.WriteToDisk(curObjectModel.Name, curStaticObjectOutputFolder);

                    // Place the related textures
                    string objectTextureFolder = Path.Combine(folderRoot, "textures");
                    ExportTexturesForObject(curObjectModel, objectTextureFolder, curStaticObjectOutputFolder);

                    // Store it
                    int gameObjectDisplayInfoID = GameObjectDisplayInfoDBC.GenerateID();
                    transportLiftObjectModelsByMeshName.Add(transportLift.MeshName, curObjectModel);
                    gameObjectDisplayInfoIDsByMeshName.Add(transportLift.MeshName, gameObjectDisplayInfoID);
                    TransportLift.ObjectModelM2ByMeshGameObjectDisplayID.Add(gameObjectDisplayInfoID, objectM2);
                }
                transportLift.GameObjectDisplayInfoID = gameObjectDisplayInfoIDsByMeshName[transportLift.MeshName];
                TransportLiftPathNode.UpdateNodesWithGameObjectEntryIDByPathGroup(transportLift.PathGroupID, transportLift.GameObjectTemplateID);
            }

            // Lift Triggers
            Logger.WriteDebug("Loading transport lift triggers...");
            List<TransportLiftTrigger> transportLiftTriggers = TransportLiftTrigger.GetAllTransportLiftTriggers();
            gameObjectDisplayInfoIDsByMeshName.Clear();
            string inputSoundFolder = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "sounds");
            Dictionary<string, ObjectModel> transportLiftTriggerObjectModelsByMeshName = new Dictionary<string, ObjectModel>();
            foreach (TransportLiftTrigger transportLiftTrigger in transportLiftTriggers)
            {
                // Load the object mesh if it hasn't been yet
                if (transportLiftTriggerObjectModelsByMeshName.ContainsKey(transportLiftTrigger.MeshName) == false)
                {
                    // Load it
                    ObjectModelProperties objectProperties = new ObjectModelProperties(transportLiftTrigger.AnimationType, transportLiftTrigger.AnimMod, transportLiftTrigger.AnimTimeInMS, true, true);
                    ObjectModel curObjectModel = new ObjectModel(transportLiftTrigger.MeshName, objectProperties, ObjectModelType.StaticDoodad, Configuration.GENERATE_OBJECT_MODEL_MIN_BOUNDARY_BOX_SIZE);
                    Logger.WriteDebug("- [" + transportLiftTrigger.MeshName + "]: Importing EQ transport lift trigger object '" + transportLiftTrigger.MeshName + "'");
                    curObjectModel.LoadEQObjectFromFile(objectsFolderRoot, transportLiftTrigger.MeshName);
                    Logger.WriteDebug("- [" + transportLiftTrigger.MeshName + "]: Importing EQ transport lift trigger object '" + transportLiftTrigger.MeshName + "' complete");
                    if (transportLiftTrigger.OpenSound != null)
                        curObjectModel.SoundsByAnimationType.Add(AnimationType.Open, transportLiftTrigger.OpenSound);
                    if (transportLiftTrigger.CloseSound != null)
                        curObjectModel.SoundsByAnimationType.Add(AnimationType.Close, transportLiftTrigger.CloseSound);

                    // Create the M2 and Skin
                    string relativeMPQPath = Path.Combine("World", "Everquest", "Transports", transportLiftTrigger.MeshName);
                    M2 objectM2 = new M2(curObjectModel, relativeMPQPath);
                    string curStaticObjectOutputFolder = Path.Combine(exportMPQRootFolder, "World", "Everquest", "Transports", transportLiftTrigger.MeshName);
                    objectM2.WriteToDisk(curObjectModel.Name, curStaticObjectOutputFolder);

                    // Place the related textures
                    string objectTextureFolder = Path.Combine(objectsFolderRoot, "textures");
                    ExportTexturesForObject(curObjectModel, objectTextureFolder, curStaticObjectOutputFolder);

                    // Store it
                    int gameObjectDisplayInfoID = GameObjectDisplayInfoDBC.GenerateID();
                    transportLiftTriggerObjectModelsByMeshName.Add(transportLiftTrigger.MeshName, curObjectModel);
                    gameObjectDisplayInfoIDsByMeshName.Add(transportLiftTrigger.MeshName, gameObjectDisplayInfoID);
                    TransportLiftTrigger.ObjectModelM2ByMeshGameObjectDisplayID.Add(gameObjectDisplayInfoID, objectM2);
                }

                transportLiftTrigger.GameObjectDisplayInfoID = gameObjectDisplayInfoIDsByMeshName[transportLiftTrigger.MeshName];
            }
            foreach(Sound sound in TransportLiftTrigger.AllSoundsBySoundName.Values)
            {
                string outputLiftSoundFolder = Path.Combine(exportMPQRootFolder, "Sound", "EQTransports");
                if (Directory.Exists(outputLiftSoundFolder) == false)
                    FileTool.CreateBlankDirectory(outputLiftSoundFolder, true);
                string sourceFullPath = Path.Combine(inputSoundFolder, string.Concat(sound.AudioFileNameNoExt, ".wav"));
                string targetFullPath = Path.Combine(outputLiftSoundFolder, string.Concat(sound.AudioFileNameNoExt, ".wav"));
                FileTool.CopyFile(sourceFullPath, targetFullPath);
            }

            Logger.WriteDebug("Converting Transports complete.");
        }

        public bool ConvertEQObjectsToWOW()
        {
            string eqExportsConditionedPath = Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER;
            string wowExportPath = Configuration.PATH_EXPORT_FOLDER;

            Logger.WriteInfo("Converting EQ objects to WOW objects...");

            // Make sure the object folder path exists
            string conditionedObjectFolderRoot = Path.Combine(eqExportsConditionedPath, "objects");
            if (Directory.Exists(conditionedObjectFolderRoot) == false)
            {
                Logger.WriteError("Failed to convert objects, as there is no object folder at '" + conditionedObjectFolderRoot + "', did you run the conditoning step?");
                return false;
            }
            string conditionedEquipmentFolderRoot = Path.Combine(eqExportsConditionedPath, "equipment");

            // Clean out the target objects folder
            string exportMPQRootFolder = Path.Combine(wowExportPath, "MPQReady");
            string exportObjectsFolder = Path.Combine(exportMPQRootFolder, "World", "Everquest", "StaticDoodads");
            if (Directory.Exists(exportObjectsFolder))
                Directory.Delete(exportObjectsFolder, true);

            // Generate static EQ objects
            string staticObjectListFileName = Path.Combine(conditionedObjectFolderRoot, "static_objects.txt");
            List<string> staticObjectList = FileTool.ReadAllStringLinesFromFile(staticObjectListFileName, false, true);
            LogCounter staticObjectProgressCounter = new LogCounter("Converting static EQ objects...", 0, staticObjectList.Count);
            foreach (string staticObjectName in staticObjectList)
            {
                staticObjectProgressCounter.AddToProgress(1);
                string curObjectOutputFolder = Path.Combine(exportObjectsFolder, staticObjectName);

                // Load the EQ object
                ObjectModelProperties objectProperties = ObjectModelProperties.GetObjectPropertiesForObject(staticObjectName);
                ObjectModel curObject = new ObjectModel(staticObjectName, objectProperties, ObjectModelType.StaticDoodad, Configuration.GENERATE_OBJECT_MODEL_MIN_BOUNDARY_BOX_SIZE);
                Logger.WriteDebug("- [" + staticObjectName + "]: Importing EQ static object '" + staticObjectName + "'");
                curObject.LoadEQObjectFromFile(conditionedObjectFolderRoot, staticObjectName);
                Logger.WriteDebug("- [" + staticObjectName + "]: Importing EQ static object '" + staticObjectName + "' complete");

                // Create the M2 and Skin
                string relativeMPQPath = Path.Combine("World", "Everquest", "StaticDoodads", staticObjectName);
                M2 objectM2 = new M2(curObject, relativeMPQPath);
                objectM2.WriteToDisk(curObject.Name, curObjectOutputFolder);

                // Place the related textures
                string objectTextureFolder = Path.Combine(conditionedObjectFolderRoot, "textures");
                ExportTexturesForObject(curObject, objectTextureFolder, curObjectOutputFolder);

                // Save it for use elsewhere
                ObjectModel.StaticObjectModelsByName.Add(curObject.Name, curObject);
                staticObjectProgressCounter.Write();
            }

            // Generate skeletal EQ objects
            string skeletalObjectListFileName = Path.Combine(conditionedObjectFolderRoot, "skeletal_objects.txt");
            List<string> skeletalObjectList = FileTool.ReadAllStringLinesFromFile(skeletalObjectListFileName, false, true);
            LogCounter skeletalObjectProgressCounter = new LogCounter("Converting skeletal EQ objects...", 0, skeletalObjectList.Count);            
            foreach (string skeletalObjectName in skeletalObjectList)
            {
                skeletalObjectProgressCounter.AddToProgress(1);
                string curObjectOutputFolder = Path.Combine(exportObjectsFolder, skeletalObjectName);

                // Load the EQ object
                ObjectModelProperties objectProperties = ObjectModelProperties.GetObjectPropertiesForObject(skeletalObjectName);
                ObjectModel curObject = new ObjectModel(skeletalObjectName, objectProperties, ObjectModelType.StaticDoodad, Configuration.GENERATE_OBJECT_MODEL_MIN_BOUNDARY_BOX_SIZE);
                Logger.WriteDebug("- [" + skeletalObjectName + "]: Importing EQ skeletal object '" + skeletalObjectName + "'");
                curObject.LoadEQObjectFromFile(conditionedObjectFolderRoot, skeletalObjectName);
                Logger.WriteDebug("- [" + skeletalObjectName + "]: Importing EQ skeletal object '" + skeletalObjectName + "' complete");

                // Create the M2 and Skin
                string relativeMPQPath = Path.Combine("World", "Everquest", "StaticDoodads", skeletalObjectName);
                M2 objectM2 = new M2(curObject, relativeMPQPath);
                objectM2.WriteToDisk(curObject.Name, curObjectOutputFolder);

                // Place the related textures
                string objectTextureFolder = Path.Combine(conditionedObjectFolderRoot, "textures");
                ExportTexturesForObject(curObject, objectTextureFolder, curObjectOutputFolder);

                // Save it for use elsewhere
                ObjectModel.StaticObjectModelsByName.Add(curObject.Name, curObject);
                skeletalObjectProgressCounter.Write();
            }

            // Update all game object model references
            Logger.WriteInfo("Updating game object model references...");
            Dictionary<string, List<GameObject>> interactiveGameObjectsByZoneShortname = GameObject.GetNonDoodadGameObjectsByZoneShortNames();
            foreach (var interactiveGameObjectsInZone in interactiveGameObjectsByZoneShortname)
            {
                string curZoneFolder = Path.Combine(eqExportsConditionedPath, "zones", interactiveGameObjectsInZone.Key);
                string curZoneStaticObjectNameMapFileName = Path.Combine(curZoneFolder, "gameobject_static_map.txt");
                Dictionary<string, string> staticObjectNameMap = new Dictionary<string, string>();
                foreach(string row in FileTool.ReadAllStringLinesFromFile(curZoneStaticObjectNameMapFileName, false, true))
                    staticObjectNameMap.Add(row.Split(",")[0], row.Split(",")[1]);
                string curZoneSkeletalObjectNameMapFileName = Path.Combine(curZoneFolder, "gameobject_skeletal_map.txt");
                Dictionary<string, string> skeletalObjectNameMap = new Dictionary<string, string>();
                foreach (string row in FileTool.ReadAllStringLinesFromFile(curZoneStaticObjectNameMapFileName, false, true))
                    skeletalObjectNameMap.Add(row.Split(",")[0], row.Split(",")[1]);
                foreach(GameObject curObject in interactiveGameObjectsInZone.Value)
                {
                    if (curObject.ModelIsInEquipmentFolder == true || curObject.ObjectType == GameObjects.GameObjectType.Mailbox)
                    {
                        // Never need to remap equipment-based models
                        curObject.ModelName = curObject.OriginalModelName;
                    }
                    else if (curObject.ModelIsSkeletal == true)
                    {
                        if (skeletalObjectNameMap.ContainsKey(curObject.OriginalModelName) == false)
                        {
                            Logger.WriteError(string.Concat("Unable to find original model name ", curObject.OriginalModelName, " in zone ", interactiveGameObjectsInZone.Key));
                            continue;
                        }
                        curObject.ModelName = skeletalObjectNameMap[curObject.OriginalModelName];
                    }
                    else
                    {
                        if (staticObjectNameMap.ContainsKey(curObject.OriginalModelName) == false)
                        {
                            Logger.WriteError(string.Concat("Unable to find original model name ", curObject.OriginalModelName, " in zone ", interactiveGameObjectsInZone.Key));
                            continue;
                        }
                        curObject.ModelName = staticObjectNameMap[curObject.OriginalModelName];
                    }
                }
            }
            Dictionary<string, List<GameObject>> nonInteractiveGameObjectsByZoneShortname = GameObject.GetDoodadGameObjectsByZoneShortNames();
            foreach (var nonInteractiveGameObjectsInZone in nonInteractiveGameObjectsByZoneShortname)
            {
                string curZoneFolder = Path.Combine(eqExportsConditionedPath, "zones", nonInteractiveGameObjectsInZone.Key);
                string curZoneStaticObjectNameMapFileName = Path.Combine(curZoneFolder, "gameobject_static_map.txt");
                Dictionary<string, string> staticObjectNameMap = new Dictionary<string, string>();
                foreach (string row in FileTool.ReadAllStringLinesFromFile(curZoneStaticObjectNameMapFileName, false, true))
                    staticObjectNameMap.Add(row.Split(",")[0], row.Split(",")[1]);
                string curZoneSkeletalObjectNameMapFileName = Path.Combine(curZoneFolder, "gameobject_skeletal_map.txt");
                Dictionary<string, string> skeletalObjectNameMap = new Dictionary<string, string>();
                foreach (string row in FileTool.ReadAllStringLinesFromFile(curZoneStaticObjectNameMapFileName, false, true))
                    skeletalObjectNameMap.Add(row.Split(",")[0], row.Split(",")[1]);
                foreach (GameObject curObject in nonInteractiveGameObjectsInZone.Value)
                {
                    if (curObject.ModelIsInEquipmentFolder == true)
                    {
                        // Never need to remap equipment-based models
                        curObject.ModelName = curObject.OriginalModelName;
                    }
                    else if (curObject.ModelIsSkeletal == true)
                    {
                        if (skeletalObjectNameMap.ContainsKey(curObject.OriginalModelName) == false)
                        {
                            Logger.WriteError(string.Concat("Unable to find original model name ", curObject.OriginalModelName, " in zone ", nonInteractiveGameObjectsInZone.Key));
                            continue;
                        }
                        curObject.ModelName = skeletalObjectNameMap[curObject.OriginalModelName];
                    }
                    else
                    {
                        if (staticObjectNameMap.ContainsKey(curObject.OriginalModelName) == false)
                        {
                            Logger.WriteError(string.Concat("Unable to find original model name ", curObject.OriginalModelName, " in zone ", nonInteractiveGameObjectsInZone.Key));
                            continue;
                        }
                        curObject.ModelName = staticObjectNameMap[curObject.OriginalModelName];
                    }
                }
            }

            // Load the models for GameObjects
            GameObject.LoadModelObjectsForNonDoodadGameObjects();

            // Generate the skeletal non-interactive zone game objects 
            List<GameObject> nonInteractiveGameObjects = GameObject.GetAllDoodadGameObjects();
            List<string> loadedNonInteractiveGameObjectNames = new List<string>();
            LogCounter gameObjectProgressCounter = new LogCounter("Converting non-interactive Game Objects...", 0, nonInteractiveGameObjects.Count);
            foreach (GameObject nonInteractiveGameObject in nonInteractiveGameObjects)
            {
                gameObjectProgressCounter.AddToProgress(1);
                string modelFileName = nonInteractiveGameObject.GenerateModelFileNameNoExt();
                if (loadedNonInteractiveGameObjectNames.Contains(modelFileName))
                {
                    gameObjectProgressCounter.Write();
                    continue;
                }

                // Determine folder to load from
                string modelDataRootFolder = conditionedObjectFolderRoot;
                if (nonInteractiveGameObject.ModelIsInEquipmentFolder == true)
                    modelDataRootFolder = conditionedEquipmentFolderRoot;

                // Load the object
                Logger.WriteDebug("- [" + modelFileName + "]: Importing Game Object object '" + modelFileName + "'");
                ObjectModel curObjectModel;
                switch (nonInteractiveGameObject.OpenType)
                {
                    case GameObjectOpenType.TYPE0:
                    case GameObjectOpenType.TYPE55:
                    case GameObjectOpenType.TYPE56:
                    case GameObjectOpenType.TYPE57:
                    case GameObjectOpenType.TYPE58:
                        {
                            ObjectModelProperties objectProperties = new ObjectModelProperties();
                            objectProperties.DoGenerateCollisionFromMeshData = nonInteractiveGameObject.HasColission;
                            objectProperties.RenderingEnabled = nonInteractiveGameObject.RenderingEnabled;
                            curObjectModel = new ObjectModel(modelFileName, objectProperties, ObjectModelType.StaticDoodad, Configuration.GENERATE_OBJECT_MODEL_MIN_BOUNDARY_BOX_SIZE);
                            curObjectModel.LoadEQObjectFromFile(modelDataRootFolder, nonInteractiveGameObject.ModelName);
                        } break;
                    case GameObjectOpenType.TYPE105:
                        {
                            ObjectModelProperties objectProperties = new ObjectModelProperties(ActiveDoodadAnimType.OnIdleRotateAroundYClockwise, 0, 9000, nonInteractiveGameObject.HasColission, nonInteractiveGameObject.RenderingEnabled);
                            curObjectModel = new ObjectModel(modelFileName, objectProperties, ObjectModelType.StaticDoodad, Configuration.GENERATE_OBJECT_MODEL_MIN_BOUNDARY_BOX_SIZE);
                            curObjectModel.LoadEQObjectFromFile(modelDataRootFolder, nonInteractiveGameObject.ModelName);
                        } break;
                    case GameObjectOpenType.TYPE100:
                    case GameObjectOpenType.TYPE101:
                        {
                            ObjectModelProperties objectProperties = new ObjectModelProperties(ActiveDoodadAnimType.OnIdleRotateAroundZCounterclockwise, 0, 9000, nonInteractiveGameObject.HasColission, nonInteractiveGameObject.RenderingEnabled);
                            curObjectModel = new ObjectModel(modelFileName, objectProperties, ObjectModelType.StaticDoodad, Configuration.GENERATE_OBJECT_MODEL_MIN_BOUNDARY_BOX_SIZE);
                            curObjectModel.LoadEQObjectFromFile(modelDataRootFolder, nonInteractiveGameObject.ModelName);
                        } break;
                    case GameObjectOpenType.TYPE59: // TODO: mischiefplane (POMTORCH2000), frontiermtns (FRONTROCK102B)
                    case GameObjectOpenType.TYPE106: // TODO: Droga, DNWINCH102
                    case GameObjectOpenType.TYPE156: // TODO: Sleeper, SLFF200
                    default:
                        {
                            Logger.WriteError("Error loading non interactive game object named " + modelFileName + " due to unhandled OpenType " + nonInteractiveGameObject.OpenType);
                            continue;
                        }
                }
                Logger.WriteDebug("- [" + modelFileName + "]: Importing Game Object '" + modelFileName + "' complete");

                // Create the M2 and Skin
                string curObjectOutputFolder = Path.Combine(exportObjectsFolder, modelFileName);
                string relativeMPQPath = Path.Combine("World", "Everquest", "StaticDoodads", modelFileName);
                M2 objectM2 = new M2(curObjectModel, relativeMPQPath);
                objectM2.WriteToDisk(modelFileName, curObjectOutputFolder);

                // Place the related textures
                string inputTextureFolder = Path.Combine(modelDataRootFolder, "textures");
                foreach (ObjectModelTexture texture in curObjectModel.ModelTextures)
                {
                    string inputTextureName = Path.Combine(inputTextureFolder, texture.TextureName + ".blp");
                    string outputTextureName = Path.Combine(curObjectOutputFolder, texture.TextureName + ".blp");
                    if (Path.Exists(inputTextureName) == false)
                    {
                        Logger.WriteError("- [" + curObjectModel.Name + "]: Error Texture named '" + texture.TextureName + ".blp' not found.  Did you run blpconverter?");
                        continue;
                    }
                    FileTool.CopyFile(inputTextureName, outputTextureName);
                    Logger.WriteDebug("- [" + curObjectModel.Name + "]: Texture named '" + texture.TextureName + ".blp' copied");
                }

                // Save it for use elsewhere
                ObjectModel.StaticObjectModelsByName.Add(curObjectModel.Name, curObjectModel);
                loadedNonInteractiveGameObjectNames.Add(curObjectModel.Name);
                gameObjectProgressCounter.Write();
            }

            // Game Object Sounds
            string inputSoundFolder = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "sounds");
            foreach (Sound sound in GameObject.AllSoundsBySoundName.Values)
            {
                string outputGameObjectSoundFolder = Path.Combine(exportMPQRootFolder, "Sound", "GameObjects");
                if (Directory.Exists(outputGameObjectSoundFolder) == false)
                    FileTool.CreateBlankDirectory(outputGameObjectSoundFolder, true);
                string sourceFullPath = Path.Combine(inputSoundFolder, string.Concat(sound.AudioFileNameNoExt, ".wav"));
                string targetFullPath = Path.Combine(outputGameObjectSoundFolder, string.Concat(sound.AudioFileNameNoExt, ".wav"));
                FileTool.CopyFile(sourceFullPath, targetFullPath);
            }

            return true;
        }

        public void ConvertQuestItems(SortedDictionary<int, ItemTemplate> itemTemplatesByEQDBID, out List<QuestTemplate> questTemplates)
        {
            Logger.WriteInfo("Converting quest items...");

            // Build the return quest templates
            questTemplates = new List<QuestTemplate>();

            // Work through each of the quest templates
            Dictionary<string, ZoneProperties> zonePropertiesByShortName = ZoneProperties.GetZonePropertyListByShortName();
            foreach (QuestTemplate questTemplate in QuestTemplate.GetQuestTemplates())
            {
                // Skip any quests that are in zones we're not processing
                if (zonePropertiesByShortName.ContainsKey(questTemplate.ZoneShortName.ToLower()) == false)
                {
                    Logger.WriteDebug(string.Concat("Ignoring quest with id '", questTemplate.QuestIDWOW, ", as the zone '", questTemplate.ZoneShortName, "' is not being generated"));
                    continue;
                }

                // Confirm the items are good and store the IDs
                if (questTemplate.MapWOWItemIDs(itemTemplatesByEQDBID) == false)
                {
                    Logger.WriteDebug(string.Concat("Could not map item IDs for quest '", questTemplate.QuestIDWOW, "'"));
                    continue;
                }

                // Mark all of the rewards so they get included in the final output
                foreach (int eqItemTemplateID in questTemplate.RewardItemEQIDs)
                    itemTemplatesByEQDBID[eqItemTemplateID].IsRewardedFromQuest = true;
            }
        }

        public void ConvertQuests(SortedDictionary<int, ItemTemplate> itemTemplatesByEQDBID, ref List<QuestTemplate> questTemplates, ref List<CreatureTemplate> creatureTemplates)
        {
            Dictionary<string, ZoneProperties> zonePropertiesByShortName = ZoneProperties.GetZonePropertyListByShortName();
            Dictionary<int, CreatureTemplate> creatureTemplatesByEQID = CreatureTemplate.GetCreatureTemplateListByEQID();
            foreach (QuestTemplate questTemplate in QuestTemplate.GetQuestTemplates())
            {
                // Skip any quests that are in zones we're not processing
                if (zonePropertiesByShortName.ContainsKey(questTemplate.ZoneShortName.ToLower()) == false)
                {
                    Logger.WriteDebug(string.Concat("Ignoring quest with id '", questTemplate.QuestIDWOW, ", as the zone '", questTemplate.ZoneShortName, "' is not being generated"));
                    continue;
                }

                // Skip any quests where the items cannot be obtained
                if (questTemplate.HasInvalidItems == true)
                    continue;
                SortedDictionary<int, ItemTemplate> itemTemplatesByWOWEntryID = ItemTemplate.GetItemTemplatesByWOWEntryID();
                if (questTemplate.AreRequiredItemsPlayerObtainable(itemTemplatesByWOWEntryID) == false)
                    continue;

                // If there is a random award, handle it
                if (questTemplate.RewardItemEQIDs.Count > 0 && questTemplate.RewardItemChances[0] < 100)
                {
                    string containerName = string.Concat(questTemplate.QuestgiverName.Replace("_", " ").Replace("#", ""), "'s Reward");
                    questTemplate.RandomAwardContainerItemTemplate = ItemTemplate.CreateQuestRandomItemContainer(containerName, questTemplate.RewardItemEQIDs, questTemplate.RewardItemChances, questTemplate.RewardItemCounts);
                    questTemplate.RewardItemWOWIDs.Clear();
                    questTemplate.RewardItemEQIDs.Clear();
                    questTemplate.RewardItemCounts.Clear();
                    questTemplate.RewardItemChances.Clear();
                    if (questTemplate.RandomAwardContainerItemTemplate != null)
                    {
                        questTemplate.RewardItemWOWIDs.Add(questTemplate.RandomAwardContainerItemTemplate.WOWEntryID);
                        questTemplate.RewardItemEQIDs.Add(questTemplate.RandomAwardContainerItemTemplate.EQItemID);
                        questTemplate.RewardItemCounts.Add(1);
                        questTemplate.RewardItemChances.Add(100);
                    }
                }

                // Pull up the related creature(s) that are quest givers and mark them as quest givers
                // NOTE: Always do this last (before the add(questTemplate)
                List<CreatureTemplate> questgiverCreatureTemplates = CreatureTemplate.GetCreatureTemplatesForSpawnZonesAndName(questTemplate.ZoneShortName, questTemplate.QuestgiverName);
                if (questgiverCreatureTemplates.Count == 0)
                {
                    Logger.WriteDebug(string.Concat("Could not map quest to creature template with zone '", questTemplate.ZoneShortName, "' and name '", questTemplate.QuestgiverName, "'"));
                    continue;
                }
                foreach (CreatureTemplate creatureTemplate in questgiverCreatureTemplates)
                {
                    if (questTemplate.QuestgiverWOWCreatureTemplateIDs.Contains(creatureTemplate.WOWCreatureTemplateID) == true)
                        continue;

                    questTemplate.QuestgiverWOWCreatureTemplateIDs.Add(creatureTemplate.WOWCreatureTemplateID);
                    creatureTemplate.IsQuestGiver = true;
                    if (questTemplate.HasMinimumFactionRequirement == true)
                        questTemplate.QuestgiverWOWFactionID = CreatureFaction.GetWOWFactionIDForEQFactionID(creatureTemplate.EQFactionID);

                    // If this quest giver is aligned to an otherwise otherwise only-attackable reputation, realign to "Norrath Settlers)
                    if (creatureTemplate.WOWFactionTemplateID == 2300 || creatureTemplate.WOWFactionTemplateID == 2301 || creatureTemplate.WOWFactionTemplateID == 2302 || creatureTemplate.WOWFactionTemplateID == 2337)
                        creatureTemplate.WOWFactionTemplateID = 2313;
                }

                // Add the default area id for quest sorting
                questTemplate.AreaID = Convert.ToInt32(zonePropertiesByShortName[questTemplate.ZoneShortName.ToLower()].DefaultZoneArea.DBCAreaTableID);

                // If there are any reactions related to talking, mark the template as using smart scripts
                if (questTemplate.Reactions.Count > 0)
                    foreach (QuestReaction reaction in questTemplate.Reactions)
                        if (reaction.type == QuestReactionType.Emote || reaction.type == QuestReactionType.Say || reaction.type == QuestReactionType.Yell)
                            foreach (CreatureTemplate creatureTemplate in questgiverCreatureTemplates)
                                creatureTemplate.HasSmartScript = true;

                questTemplates.Add(questTemplate);
            }

            Logger.WriteDebug("Converting quests done");
        }

        public void RemapDefaultFactionsForInteractiveCreatures(ref List<CreatureTemplate> creatureTemplates)
        {
            Logger.WriteDebug("Remapping interactive neutral creature templates's faction template started");
            foreach(CreatureTemplate creatureTemplate in creatureTemplates)
            {
                if (creatureTemplate.WOWFactionTemplateID != Configuration.CREATURE_FACTION_TEMPLATE_NEUTRAL)
                    continue;
                if (creatureTemplate.IsBanker || creatureTemplate.IsQuestGiver || creatureTemplate.MerchantID != 0 || creatureTemplate.ClassTrainerType != ClassType.None)
                {
                    creatureTemplate.WOWFactionTemplateID = Configuration.CREATURE_FACTION_TEMPLATE_NEUTRAL_INTERACTIVE;
                }
            }
            Logger.WriteDebug("Remapping interactive neutral creature templates's faction template done");
        }

        public bool ConvertEQZonesToWOW(out List<Zone> zones)
        {
            // Build paths
            string inputZoneFolder = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "zones");
            string inputSoundFolderRoot = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "sounds");
            string inputMusicFolderRoot = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "music");
            string inputObjectTexturesFolder = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "objects", "textures");
            string exportMPQRootFolder = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "MPQReady");
            string exportMapsFolder = Path.Combine(exportMPQRootFolder, "World", "Maps");
            string exportWMOFolder = Path.Combine(exportMPQRootFolder, "World", "wmo");
            string exportZonesTexturesFolder = Path.Combine(exportMPQRootFolder, "World", "Everquest", "ZoneTextures");
            string exportZonesObjectsFolder = Path.Combine(exportMPQRootFolder, "World", "Everquest", "ZoneObjects");
            string exportInterfaceFolder = Path.Combine(exportMPQRootFolder, "Interface");
            string exportMusicFolder = Path.Combine(exportMPQRootFolder, "Sound", "Music");
            string exportMPQFileName = Path.Combine(Configuration.PATH_EXPORT_FOLDER, Configuration.PATH_PATCH_NEW_FILE_NAME_NO_EXT + ".mpq");
            string relativeStaticDoodadsPath = Path.Combine("World", "Everquest", "StaticDoodads");

            // Clear folders if it's a fresh build
            if (Configuration.GENERATE_ONLY_LISTED_ZONE_SHORTNAMES.Count == 0
                || File.Exists(exportMPQFileName) == false)
            {
                if (Directory.Exists(exportMapsFolder))
                    Directory.Delete(exportMapsFolder, true);
                if (Directory.Exists(exportWMOFolder))
                    Directory.Delete(exportWMOFolder, true);
                if (Directory.Exists(exportZonesTexturesFolder))
                    Directory.Delete(exportZonesTexturesFolder, true);
                if (Directory.Exists(exportZonesObjectsFolder))
                    Directory.Delete(exportZonesObjectsFolder, true);
                if (Directory.Exists(exportMusicFolder))
                    Directory.Delete(exportMusicFolder, true);
            }

            // Generate folders
            if (Directory.Exists(exportMapsFolder) == false)
                Directory.CreateDirectory(exportMapsFolder);
            if (Directory.Exists(exportWMOFolder) == false)
                Directory.CreateDirectory(exportWMOFolder);
            if (Directory.Exists(exportZonesTexturesFolder) == false)
                Directory.CreateDirectory(exportZonesTexturesFolder);
            if (Directory.Exists(exportZonesObjectsFolder) == false)
                Directory.CreateDirectory(exportZonesObjectsFolder);
            if (Directory.Exists(exportMusicFolder) == false)
                Directory.CreateDirectory(exportMusicFolder);

            // Load shared environment settings
            ZoneProperties.CommonOutdoorEnvironmentProperties.SetAsOutdoors(77, 120, 143, ZoneFogType.Clear, true, 0.5f, 1.0f, ZoneSkySpecialType.None);

            // Get the list of zones to process
            DirectoryInfo zoneRootDirectoryInfo = new DirectoryInfo(inputZoneFolder);
            DirectoryInfo[] zoneDirectoryInfos = zoneRootDirectoryInfo.GetDirectories();
            Dictionary<string, ZoneProperties> zonePropertiesListByShortName = ZoneProperties.GetZonePropertyListByShortName();
            foreach (DirectoryInfo zoneDirectory in zoneDirectoryInfos)
            {
                if (zonePropertiesListByShortName.ContainsKey(zoneDirectory.Name))
                    ZoneShortNamesToProcess.Add(zoneDirectory.Name);
            }
            LogCounter progressCounter = new LogCounter("Converting EQ zones to WOW zones...", 0, ZoneShortNamesToProcess.Count);
            progressCounter.Write(0);

            if (Configuration.GENERATE_ONLY_LISTED_ZONE_SHORTNAMES.Count > 0)
            {
                Logger.WriteInfo("- Note: GENERATE_ONLY_LISTED_ZONE_SHORTNAMES has values: ", false);
                foreach (string zoneShortName in Configuration.GENERATE_ONLY_LISTED_ZONE_SHORTNAMES)
                    Logger.WriteInfo(zoneShortName + " ", false, false);
                Logger.WriteInfo(string.Empty, true, false);
            }

            List<Zone> workingZones = new List<Zone>();
            if (Configuration.CORE_ENABLE_MULTITHREADING == true)
            {
                int taskCount = Configuration.CORE_ZONEGEN_THREAD_COUNT;
                if (ZoneShortNamesToProcess.Count < taskCount)
                    taskCount = ZoneShortNamesToProcess.Count;
                Task<List<Zone>>[] tasks = new Task<List<Zone>>[taskCount];
                for (int i = 0; i < taskCount; i++)
                {
                    int iCopy = i + 1;
                    tasks[i] = Task.Factory.StartNew(() =>
                    {
                        return ZoneThreadWorker(iCopy, inputZoneFolder, exportMPQRootFolder, relativeStaticDoodadsPath, inputObjectTexturesFolder, inputMusicFolderRoot, inputSoundFolderRoot, progressCounter);
                    });
                }
                Task.WaitAll(tasks);
                foreach(var task in tasks)
                    workingZones.AddRange(task.Result);
            }
            else
            {
                List<Zone> processedZones = ZoneThreadWorker(1, inputZoneFolder, exportMPQRootFolder, relativeStaticDoodadsPath, inputObjectTexturesFolder, inputMusicFolderRoot, inputSoundFolderRoot, progressCounter);
                lock (ZoneLock)
                    workingZones.AddRange(processedZones);
            }

            zones = workingZones;
            return true;
        }

        private List<Zone> ZoneThreadWorker(int threadID, string inputZoneFolder, string exportMPQRootFolder, string relativeStaticDoodadsPath, 
            string inputObjectTexturesFolder, string inputMusicFolderRoot, string inputSoundFolderRoot, LogCounter progressCounter)
        {
            Logger.WriteInfo(string.Concat("<+> Thread [Zone Subworker ", threadID.ToString(), "] Started"));
            List<Zone> processedZones = new List<Zone>();
            bool moreToProcess = true;
            while (moreToProcess)
            {
                // Grab the zone to work with
                string zoneShortNameToProcess = string.Empty;
                lock (ZoneLock)
                {
                    if (ZoneShortNamesToProcess.Count > 0)
                    {
                        zoneShortNameToProcess = ZoneShortNamesToProcess.First();
                        ZoneShortNamesToProcess.RemoveAt(0);
                    }
                }

                // Zone was found, so continue processing
                if (zoneShortNameToProcess != string.Empty)
                {
                    Zone curZone = ConvertZone(zoneShortNameToProcess, inputZoneFolder, exportMPQRootFolder, relativeStaticDoodadsPath, inputObjectTexturesFolder, inputMusicFolderRoot, inputSoundFolderRoot, progressCounter);
                    processedZones.Add(curZone);
                }
                else
                    moreToProcess = false;
            }
            Logger.WriteInfo(string.Concat("<-> Thread [Zone Subworker ", threadID.ToString(), "] Ended"));
            return processedZones;
        }

        private Zone ConvertZone(string zoneShortName, string inputZoneFolder, string exportMPQRootFolder, string relativeStaticDoodadsPath, string inputObjectTexturesFolder,
            string inputMusicFolderRoot, string inputSoundFolderRoot, LogCounter progressCounter)
        {
            // Grab the zone properties to generate IDs, even for zones not being output
            Logger.WriteDebug(" - Processing zone '" + zoneShortName + "'");
            string relativeZoneObjectsPath = Path.Combine("World", "Everquest", "ZoneObjects", zoneShortName);
            ZoneProperties zoneProperties = ZoneProperties.GetZonePropertiesForZone(zoneShortName);

            // Grab any game objects that 

            // Generate the zone
            Zone curZone = new Zone(zoneShortName, zoneProperties);
            Logger.WriteDebug("- [" + curZone.ShortName + "]: Converting zone '" + curZone.ShortName + "' into a wow zone...");
            string curZoneDirectory = Path.Combine(inputZoneFolder, zoneShortName);
            curZone.LoadFromEQZone(zoneShortName, curZoneDirectory, GameObject.GetDoodadGameObjectsForZoneShortname(zoneShortName));

            // Create the zone WMO objects
            WMO zoneWMO = new WMO(curZone, exportMPQRootFolder, "WORLD\\EVERQUEST\\ZONETEXTURES", relativeStaticDoodadsPath, relativeZoneObjectsPath, false);
            zoneWMO.WriteToDisk();

            // Create the WDT
            WDT zoneWDT = new WDT(curZone, zoneWMO.RootFileRelativePathWithFileName);
            zoneWDT.WriteToDisk(exportMPQRootFolder);

            // Create the WDL
            WDL zoneWDL = new WDL(curZone);
            zoneWDL.WriteToDisk(exportMPQRootFolder);

            // Create the zone-specific generated object files
            foreach (ObjectModel zoneObject in curZone.GeneratedZoneObjects)
            {
                // Recreate the folder if needed
                string curZoneObjectRelativePath = Path.Combine(relativeZoneObjectsPath, zoneObject.Name);
                string curZoneObjectFolder = Path.Combine(exportMPQRootFolder, curZoneObjectRelativePath);
                if (Directory.Exists(curZoneObjectFolder))
                    Directory.Delete(curZoneObjectFolder, true);
                Directory.CreateDirectory(curZoneObjectFolder);

                // Build this zone object M2 Data
                M2 objectM2 = new M2(zoneObject, curZoneObjectRelativePath);
                objectM2.WriteToDisk(zoneObject.Name, curZoneObjectFolder);
            }

            // Create the zone-specific sound instance objects
            foreach (ObjectModel zoneSoundInstanceObject in curZone.SoundInstanceObjectModels)
            {
                // Recreate the folder if needed
                string curZoneObjectRelativePath = Path.Combine(relativeZoneObjectsPath, zoneSoundInstanceObject.Name);
                string curZoneObjectFolder = Path.Combine(exportMPQRootFolder, curZoneObjectRelativePath);
                if (Directory.Exists(curZoneObjectFolder))
                    Directory.Delete(curZoneObjectFolder, true);
                Directory.CreateDirectory(curZoneObjectFolder);

                // Build this zone object M2 Data
                M2 objectM2 = new M2(zoneSoundInstanceObject, curZoneObjectRelativePath);
                objectM2.WriteToDisk(zoneSoundInstanceObject.Name, curZoneObjectFolder);
            }

            // Place the related textures
            ExportTexturesForZone(curZone, curZoneDirectory, exportMPQRootFolder, relativeZoneObjectsPath, inputObjectTexturesFolder);

            // Place the related music files
            ExportMusicForZone(curZone, inputMusicFolderRoot, exportMPQRootFolder);

            // Place the related ambience files
            ExportAmbientSoundForZone(curZone, inputSoundFolderRoot, exportMPQRootFolder);

            Logger.WriteDebug("- [" + curZone.ShortName + "]: Converting of zone '" + curZone.ShortName + "' complete");

            progressCounter.Write(1);
            return curZone;
        }

        public bool ConvertCreatures(ref List<CreatureModelTemplate> creatureModelTemplates, ref List<CreatureTemplate> creatureTemplates, 
            ref List<CreatureSpawnPool> creatureSpawnPools)
        {           
            // Generate folder paths
            string workingTexturePath = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "GeneratedCreatureTextures");
            if (Directory.Exists(workingTexturePath))
                Directory.Delete(workingTexturePath, true);
            Directory.CreateDirectory(workingTexturePath);
            string wowExportPath = Configuration.PATH_EXPORT_FOLDER;
            string exportMPQRootFolder = Path.Combine(wowExportPath, "MPQReady");
            string exportAnimatedObjectsFolder = Path.Combine(exportMPQRootFolder, "Creature", "Everquest");
            if (Directory.Exists(exportAnimatedObjectsFolder))
                Directory.Delete(exportAnimatedObjectsFolder, true); // BUG: Race condition on this delete
            Directory.CreateDirectory(exportAnimatedObjectsFolder);
            string eqExportsConditionedPath = Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER;
            string charactersFolderRoot = Path.Combine(eqExportsConditionedPath, "characters");
            if (Directory.Exists(charactersFolderRoot) == false)
            {
                Logger.WriteError("Failed to create characters because could not find the characters folder root at '" + charactersFolderRoot + "'");
                return false;
            }
            string inputObjectTextureFolder = Path.Combine(charactersFolderRoot, "Textures");
            string generatedTexturesFolderPath = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "GeneratedCreatureTextures");

            Logger.WriteInfo("Converting EQ Creatures (skeletal objects) to WOW creature objects...");

            // Generate templates
            Dictionary<int, CreatureTemplate> creatureTemplatesByID = CreatureTemplate.GetCreatureTemplateListByEQID();
            creatureTemplates = creatureTemplatesByID.Values.ToList();

            // Create all of the models and related model files
            LogCounter progressionCounter = new LogCounter("Creating creature model files...");
            CreatureModelTemplate.CreateAllCreatureModelTemplates(creatureTemplates);
            foreach (var modelTemplatesByRaceID in CreatureModelTemplate.AllTemplatesByRaceID)
            {
                foreach (CreatureModelTemplate modelTemplate in modelTemplatesByRaceID.Value)
                {
                    modelTemplate.CreateModelFiles(charactersFolderRoot, inputObjectTextureFolder, exportAnimatedObjectsFolder, generatedTexturesFolderPath);
                    creatureModelTemplates.Add(modelTemplate);
                    progressionCounter.Write(1);
                }
            }

            // Get a list of valid zone names
            Dictionary<string, int> mapIDsByShortName = new Dictionary<string, int>();
            Dictionary<int, ZoneProperties> zonePropertiesByMapID = new Dictionary<int, ZoneProperties>();
            foreach (var zoneProperties in ZoneProperties.GetZonePropertyListByShortName())
            {
                mapIDsByShortName.Add(zoneProperties.Value.ShortName.ToLower().Trim(), zoneProperties.Value.DBCMapID);
                zonePropertiesByMapID.Add(zoneProperties.Value.DBCMapID, zoneProperties.Value);
            }

            // Group spawn entries (creature template relationships) by group ID
            Logger.WriteInfo("Associating creatures with spawn locations...");
            List<CreatureSpawnEntry> spawnEntries = CreatureSpawnEntry.GetSpawnEntryList();
            Dictionary<int, List<CreatureSpawnEntry>> creatureSpawnEntriesByGroupID = new Dictionary<int, List<CreatureSpawnEntry>>();
            foreach (CreatureSpawnEntry entry in spawnEntries)
            {
                if (creatureSpawnEntriesByGroupID.ContainsKey(entry.SpawnGroupID) == false)
                    creatureSpawnEntriesByGroupID.Add(entry.SpawnGroupID, new List<CreatureSpawnEntry>());
                creatureSpawnEntriesByGroupID[entry.SpawnGroupID].Add(entry);
            }

            // Group all spawn instances (locations) by group ID
            List<CreatureSpawnInstance> creatureSpawnInstances = CreatureSpawnInstance.GetSpawnInstanceListByID().Values.ToList();
            Dictionary<int, List<CreatureSpawnInstance>> creatureSpawnInstancesByGroupID = new Dictionary<int, List<CreatureSpawnInstance>>();
            foreach (CreatureSpawnInstance instance in creatureSpawnInstances)
            {
                if (creatureSpawnInstancesByGroupID.ContainsKey(instance.SpawnGroupID) == false)
                    creatureSpawnInstancesByGroupID.Add(instance.SpawnGroupID, new List<CreatureSpawnInstance>());
                creatureSpawnInstancesByGroupID[instance.SpawnGroupID].Add(instance);
            }

            // Go through each spawn group and generate pools
            Logger.WriteInfo("Generating creature spawn pools...");
            Dictionary<int, CreatureSpawnGroup> spawnGroupsByGroupID = CreatureSpawnGroup.GetSpawnGroupsByGroupID();
            Dictionary<int, CreatureSpawnPool> spawnPoolsByGroupID = new Dictionary<int, CreatureSpawnPool>();
            foreach (var spawnGroup in spawnGroupsByGroupID)
            {
                // Confirm there are related instances
                if (creatureSpawnInstancesByGroupID.ContainsKey(spawnGroup.Key) == false)
                    continue;

                // Make the new pool
                CreatureSpawnPool curSpawnPool = new CreatureSpawnPool(spawnGroup.Value);

                // Add instances that are valid zones
                foreach (CreatureSpawnInstance spawnInstance in creatureSpawnInstancesByGroupID[spawnGroup.Key])
                {
                    if (mapIDsByShortName.ContainsKey(spawnInstance.ZoneShortName.ToLower().Trim()))
                    {
                        spawnInstance.MapID = mapIDsByShortName[spawnInstance.ZoneShortName.ToLower().Trim()];
                        spawnInstance.AreaID = Convert.ToInt32(zonePropertiesByMapID[spawnInstance.MapID].DefaultZoneArea.DBCAreaTableID);
                        curSpawnPool.AddSpawnInstance(spawnInstance);
                    }
                }

                // Add spawns that are valid spawns
                if (creatureSpawnEntriesByGroupID.ContainsKey(spawnGroup.Key))
                {
                    foreach (CreatureSpawnEntry spawnEntry in creatureSpawnEntriesByGroupID[spawnGroup.Key])
                    {
                        if (creatureTemplatesByID.ContainsKey(spawnEntry.EQCreatureTemplateID))
                        {
                            curSpawnPool.AddCreatureTemplate(creatureTemplatesByID[spawnEntry.EQCreatureTemplateID], spawnEntry.Chance);

                            // If  this is a limited spawn, limit the group
                            // TODO: This should be handled differently, as only specific creature templates should be limited
                            if (creatureTemplatesByID[spawnEntry.EQCreatureTemplateID].LimitOneInSpawnPool == true)
                                spawnGroup.Value.SpawnLimit = 1;
                        }
                    }
                }

                // Make sure there is at least one element
                if (curSpawnPool.CreatureSpawnInstances.Count == 0)
                {
                    Logger.WriteDebug("Invalid creature spawn pool with groupID '" + spawnGroup.Key+ "', as there are no creature spawn instances. Skipping.");
                    continue;
                }
                if (curSpawnPool.CreatureTemplates.Count == 0)
                {
                    Logger.WriteDebug("Invalid creature spawn pool with groupID '" + spawnGroup.Key+ "', as there are no valid creature templates. Skipping.");
                    continue;
                }

                // Validate the chances
                if (curSpawnPool.DoChancesAddTo100() == false)
                {
                    Logger.WriteDebug("Invalid creature spawn pool with groupID '" + spawnGroup.Key + "', as chances did not add to 100. Rebalancing.");
                    curSpawnPool.BalanceChancesTo100();
                }

                // Add it
                spawnPoolsByGroupID.Add(spawnGroup.Key, curSpawnPool);
            }
            creatureSpawnPools = spawnPoolsByGroupID.Values.ToList();

            // Make a list of path grid entries
            Dictionary<int, Dictionary<int, List<CreaturePathGridEntry>>> creaturePathGridEntriesByIDAndMapID = new Dictionary<int, Dictionary<int, List<CreaturePathGridEntry>>>();
            foreach (CreaturePathGridEntry creaturePathGridEntry in CreaturePathGridEntry.GetInitialPathGridEntries())
            {
                // Skip non-viable maps
                if (mapIDsByShortName.ContainsKey(creaturePathGridEntry.ZoneShortName.ToLower().Trim()) == false)
                    continue;
                int mapID = mapIDsByShortName[creaturePathGridEntry.ZoneShortName.ToLower().Trim()];

                // Add it
                if (creaturePathGridEntriesByIDAndMapID.ContainsKey(creaturePathGridEntry.GridID) == false)
                    creaturePathGridEntriesByIDAndMapID.Add(creaturePathGridEntry.GridID, new Dictionary<int, List<CreaturePathGridEntry>>());
                if (creaturePathGridEntriesByIDAndMapID[creaturePathGridEntry.GridID].ContainsKey(mapID) == false)
                    creaturePathGridEntriesByIDAndMapID[creaturePathGridEntry.GridID].Add(mapID, new List<CreaturePathGridEntry>());
                creaturePathGridEntriesByIDAndMapID[creaturePathGridEntry.GridID][mapID].Add(creaturePathGridEntry);
            }

            // Associate path grid entries with relevant spawn instances
            Logger.WriteInfo("Relating creatures to spawn instances...");
            foreach (CreatureSpawnPool creatureSpawnPool in creatureSpawnPools)
            {
                foreach (CreatureSpawnInstance creatureSpawnInstance in creatureSpawnPool.CreatureSpawnInstances)
                {
                    if (creatureSpawnInstance.PathGridID != 0)
                    {
                        if (creaturePathGridEntriesByIDAndMapID.ContainsKey(creatureSpawnInstance.PathGridID) == false)
                        {
                            Logger.WriteDebug("CreatureSpawnInstance with ID '" + creatureSpawnInstance.ID + "' could not find a PathGridEntry with ID '" + creatureSpawnInstance.PathGridID + "'");
                            continue;
                        }
                        if (creaturePathGridEntriesByIDAndMapID[creatureSpawnInstance.PathGridID].ContainsKey(creatureSpawnInstance.MapID) == false)
                        {
                            Logger.WriteDebug("CreatureSpawnInstance with ID '" + creatureSpawnInstance.ID + "' could not find a PathGridEntry with ID '" + creatureSpawnInstance.PathGridID + "' and mapID of '" + creatureSpawnInstance.MapID + "'");
                            continue;
                        }

                        creatureSpawnInstance.SetPathGridEntries(creaturePathGridEntriesByIDAndMapID[creatureSpawnInstance.PathGridID][creatureSpawnInstance.MapID]);
                    }
                }
            }

            // Copy all of the sound files
            Logger.WriteInfo("Copying creature sound files...");
            string inputSoundFolderRoot = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "sounds");
            string exportCreatureSoundsDirectory = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "MPQReady", "Sound", "Creature", "Everquest");
            if (Directory.Exists(exportCreatureSoundsDirectory) == true)
                Directory.Delete(exportCreatureSoundsDirectory, true);
            FileTool.CreateBlankDirectory(exportCreatureSoundsDirectory, false);
            CreatureRace.GenerateAllSounds();
            foreach (var sound in CreatureRace.SoundsBySoundName)
            {
                string sourceSoundFileName = Path.Combine(inputSoundFolderRoot, sound.Value.Name);
                string targetSoundFileName = Path.Combine(exportCreatureSoundsDirectory, sound.Value.Name);
                if (File.Exists(targetSoundFileName) == false)
                    FileTool.CopyFile(sourceSoundFileName, targetSoundFileName);
            }

            Logger.WriteInfo("Creature generation complete.");
            return true;
        }

        public void ConvertLoot(List<CreatureTemplate> creatureTemplates, out Dictionary<int, List<ItemLootTemplate>> itemLootTemplatesByCreatureTemplateID)
        {
            Logger.WriteInfo("Converting loot tables...");

            // Generate item templates
            SortedDictionary<int, ItemTemplate> itemTemplatesByEQDBID = ItemTemplate.GetItemTemplatesByEQDBIDs();

            // Build item loot templates
            itemLootTemplatesByCreatureTemplateID = new Dictionary<int, List<ItemLootTemplate>>();
            Dictionary<int, ItemLootDrop> itemLootDropsByEQID = ItemLootDrop.GetItemLootDropsByEQID();
            Dictionary<int, ItemLootTable> itemLootTablesByEQID = ItemLootTable.GetItemLootTablesByEQID();
            foreach(CreatureTemplate creatureTemplate in creatureTemplates)
            {
                // Skip creatures with no loot table
                if (creatureTemplate.EQLootTableID == 0)
                    continue;
                if (itemLootTablesByEQID.ContainsKey(creatureTemplate.EQLootTableID) == false)
                {
                    // In review, these errors have to do with future expansions that this code won't support
                    Logger.WriteDebug("For creature template '" + creatureTemplate.EQCreatureTemplateID + "' named '" + creatureTemplate.Name + "', lootTableID of '" + creatureTemplate.EQLootTableID + "' was not found");
                    continue;
                }
                ItemLootTable curItemLootTable = itemLootTablesByEQID[creatureTemplate.EQLootTableID];

                // Set money
                creatureTemplate.MoneyMinInCopper = curItemLootTable.MinMoney;
                creatureTemplate.MoneyMaxInCopper = curItemLootTable.MaxMoney;

                // Create the item loot template records
                List<ItemLootTemplate> itemLootTemplates = new List<ItemLootTemplate>();
                int itemGroupID = 0;
                foreach (ItemLootTableEntry lootTableEntry in curItemLootTable.ItemLootTableEntries)
                {
                    // Skip invalid rows
                    if (itemLootDropsByEQID.ContainsKey(lootTableEntry.LootDropID) == false)
                    {
                        // In review, these errors have to do with future expansions that this code won't support
                        Logger.WriteDebug("ItemLootTable with ID '" + lootTableEntry.LootTableID + "' references ItemLootDrop with ID '" + lootTableEntry.LootDropID + "', but it did not exist");
                        continue;
                    }

                    // Calculate the minimum number of times this entry should show up
                    int numOfDropCopies = 1;
                    if (lootTableEntry.Multiplier > 1)
                    {
                        int calcMultiplier = (lootTableEntry.Multiplier * lootTableEntry.Probability) / 100;
                        numOfDropCopies = int.Max(calcMultiplier, lootTableEntry.MultiplierMin);
                        numOfDropCopies = int.Max(numOfDropCopies, 1);
                    }

                    // Output loot table entries for each copy determined above
                    ItemLootDrop curItemLootDrop = itemLootDropsByEQID[lootTableEntry.LootDropID];
                    for (int i = 0; i < numOfDropCopies; i++)
                    {
                        foreach (ItemLootDropEntry itemDropEntry in curItemLootDrop.ItemLootDropEntries)
                        {
                            if (itemTemplatesByEQDBID.ContainsKey(itemDropEntry.ItemIDEQ) == false)
                            {
                                // In review, these errors have to do with items in future expansions
                                Logger.WriteDebug("ItemDropEntry with ID '" + itemDropEntry.LootDropID + "' references ItemID of '" + itemDropEntry.ItemIDEQ + "', but it did not exist");
                                continue;
                            }
                            ItemTemplate curItemTemplate = itemTemplatesByEQDBID[itemDropEntry.ItemIDEQ];
                            curItemTemplate.IsDroppedByCreature = true;
                            ItemLootTemplate newItemLootTemplate = new ItemLootTemplate();
                            newItemLootTemplate.CreatureTemplateEntryID = creatureTemplate.WOWCreatureTemplateID;
                            newItemLootTemplate.ItemTemplateEntryID = curItemTemplate.WOWEntryID;
                            newItemLootTemplate.Chance = itemDropEntry.Chance;
                            newItemLootTemplate.Comment = creatureTemplate.Name + " - " + curItemTemplate.Name;
                            newItemLootTemplate.GroupID = itemGroupID;
                            newItemLootTemplate.MinCount = Math.Max(lootTableEntry.MinDrop, 1);
                            newItemLootTemplate.MaxCount = Math.Max(lootTableEntry.MinDrop, 1);
                            itemLootTemplates.Add(newItemLootTemplate);
                        }
                        itemGroupID++;
                    }
                }

                if (itemLootTemplates.Count > 0)
                {
                    itemLootTemplatesByCreatureTemplateID.Add(creatureTemplate.WOWCreatureTemplateID, itemLootTemplates);
                    creatureTemplate.WOWLootID = creatureTemplate.WOWCreatureTemplateID;
                }
            }

            Logger.WriteInfo("Item and loot conversion complete.");
        }
        private void CreateItemGraphics(ref SortedDictionary<int, ItemTemplate> itemTemplatesByEQDBID)
        {
            Logger.WriteInfo("Creating item graphics started");

            // Generating the item display information
            Logger.WriteInfo("Generating item display info...");
            foreach (ItemTemplate itemTemplate in itemTemplatesByEQDBID.Values)
            {
                string iconName = "INV_EQ_" + (itemTemplate.IconID).ToString();
                itemTemplate.ItemDisplayInfo = ItemDisplayInfo.CreateItemDisplayInfo("eq_" + itemTemplate.EQItemDisplayFileName, iconName,
                    itemTemplate.InventoryType, itemTemplate.EQArmorMaterialType, itemTemplate.ColorPacked);
            }

            // Build output directory
            Logger.WriteInfo("Generating and copying blp files for equipment...");
            string outputFolderPathRoot = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "MPQReady", "ITEM", "TEXTURECOMPONENTS");
            if (Directory.Exists(outputFolderPathRoot) == true)
                Directory.Delete(outputFolderPathRoot, true);
            Directory.CreateDirectory(outputFolderPathRoot);

            // Convert and copy all of the BLP files
            LogCounter progressionCounter = new LogCounter("Converting and copying equipment textures... ");
            Task equipTexConv1Task = Task.Factory.StartNew(() =>
            {
                Logger.WriteInfo("<+> Thread [Equipment Texture Subworker 1] Started");
                ConvertAndCopyEquipmentTextures("ArmLowerTexture", progressionCounter);
                ConvertAndCopyEquipmentTextures("ArmUpperTexture", progressionCounter);
                ConvertAndCopyEquipmentTextures("LegLowerTexture", progressionCounter);
                ConvertAndCopyEquipmentTextures("LegUpperTexture", progressionCounter);
                ConvertAndCopyEquipmentTextures("FootTexture", progressionCounter);
                Logger.WriteInfo("<-> Thread [Equipment Texture Subworker 1] Ended");
            }, TaskCreationOptions.LongRunning);
            if (Configuration.CORE_ENABLE_MULTITHREADING == false)
                equipTexConv1Task.Wait();
            Task equipTexConv2Task = Task.Factory.StartNew(() =>
            {
                Logger.WriteInfo("<+> Thread [Equipment Texture Subworker 2] Started");
                ConvertAndCopyEquipmentTextures("TorsoLowerTexture", progressionCounter);
                ConvertAndCopyEquipmentTextures("TorsoUpperTexture", progressionCounter);
                ConvertAndCopyEquipmentTextures("HandTexture", progressionCounter);
                Logger.WriteInfo("<-> Thread [Equipment Texture Subworker 2] Ended");
            }, TaskCreationOptions.LongRunning);
            if (Configuration.CORE_ENABLE_MULTITHREADING == false)
                equipTexConv2Task.Wait();
            if (Configuration.CORE_ENABLE_MULTITHREADING == true)
            {
                equipTexConv1Task.Wait();
                equipTexConv2Task.Wait();
            }
            Logger.WriteInfo("Creating item graphics ended");
        }

        private void ConvertAndCopyEquipmentTextures(string subfolderName, LogCounter progressionCounter)
        {
            Logger.WriteDebug("Converting and copying equipment textures for '" + subfolderName + "'... ");
            string workingFolder = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "GeneratedEquipmentTextures", subfolderName);
            string outputFolder = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "MPQReady", "ITEM", "TEXTURECOMPONENTS", subfolderName);
            if (Directory.Exists(workingFolder) == false)
            {
                Logger.WriteInfo("Note: Not copying armor texture data due to there not being any for '" + subfolderName + "'");
                return;
            }
            if (Directory.Exists(outputFolder) == true)
                Directory.Delete(outputFolder, true);
            Directory.CreateDirectory(outputFolder);
            string[] pngFileNamesWithPath = Directory.GetFiles(workingFolder, "*.png");

            progressionCounter.Write();
            ImageTool.ConvertPNGTexturesToBLP(pngFileNamesWithPath.ToList(), ImageTool.ImageAssociationType.Clothing, progressionCounter);
            FileTool.CopyDirectoryAndContents(workingFolder, outputFolder, true, false, "*.blp");
        }

        public void GenerateSpells(out List<SpellTemplate> spellTemplates)
        {
            Logger.WriteInfo("Generating spells...");

            // Load spell templates
            spellTemplates = SpellTemplate.GetSpellTemplatesByEQID().Values.ToList();

            // Add any custom spells
            GenerateCustomSpells(ref spellTemplates);

            Logger.WriteDebug("Generating spells complete.");
        }

        public void GenerateCustomSpells(ref List<SpellTemplate> spellTemplates)
        {
            Logger.WriteDebug("Creating custom spells");
            List<ClassType> casterClassTypes = new List<ClassType> { ClassType.Priest, ClassType.Shaman, ClassType.Mage, ClassType.Druid, ClassType.Warlock };
            List<ClassType> meleeClassTypes = new List<ClassType> { ClassType.Warrior, ClassType.Paladin, ClassType.Hunter, ClassType.Rogue, ClassType.DeathKnight };

            // Gate
            SpellTemplate gateSpellTemplate = new SpellTemplate();
            gateSpellTemplate.Name = "Gate";
            gateSpellTemplate.WOWSpellID = Configuration.SPELLS_GATE_SPELLDBC_ID;
            gateSpellTemplate.Description = "Opens a magical portal that returns you to your bind point in Norrath.";
            if (Configuration.SPELLS_GATE_TETHER_ENABLED == true)
            {
                gateSpellTemplate.Description = string.Concat(gateSpellTemplate.Description, " You will have 30 minutes where you can return to your gate point after casting it.");
                gateSpellTemplate.AuraDescription = "You are tethered to the location where you gated. Right click before the buff wears off to return there. The tether will fail if you attempt return while in combat.";
                gateSpellTemplate.SpellDurationInMS = 1800000; // 30 minutes
                gateSpellTemplate.EffectType1 = SpellWOWEffectType.ApplyAura;
                gateSpellTemplate.EffectAura1 = 4; // Dummy
            }
            gateSpellTemplate.SpellIconID = SpellIconDBC.GetDBCIDForSpellIconID(22);
            gateSpellTemplate.CastTimeInMS = 5000;
            gateSpellTemplate.RecoveryTimeInMS = 8000;
            gateSpellTemplate.WOWTargetType = SpellWOWTargetType.Self;
            gateSpellTemplate.SpellVisualID1 = 220; // Taken from astral recall / hearthstone
            gateSpellTemplate.PlayerLearnableByClassTrainer = true;
            gateSpellTemplate.AllowCastInCombat = false;
            gateSpellTemplate.SkillLine = Configuration.DBCID_SKILLLINE_ALTERATION_ID;
            spellTemplates.Add(gateSpellTemplate);

            // Bind Affinity (Self)
            SpellTemplate bindAffinitySelfSpellTemplate = new SpellTemplate();
            bindAffinitySelfSpellTemplate.Name = "Bind Affinity (Self)";
            bindAffinitySelfSpellTemplate.WOWSpellID = Configuration.SPELLS_BINDSELF_SPELLDBC_ID;
            bindAffinitySelfSpellTemplate.Description = "Binds the soul of the caster to their current location. Only works in Norrath.";
            bindAffinitySelfSpellTemplate.SpellIconID = SpellIconDBC.GetDBCIDForSpellIconID(21);
            bindAffinitySelfSpellTemplate.CastTimeInMS = 6000;
            bindAffinitySelfSpellTemplate.RecoveryTimeInMS = 12000;
            bindAffinitySelfSpellTemplate.WOWTargetType = SpellWOWTargetType.Self;
            bindAffinitySelfSpellTemplate.SpellVisualID1 = 99; // Taken from soulstone
            bindAffinitySelfSpellTemplate.PlayerLearnableByClassTrainer = true;
            bindAffinitySelfSpellTemplate.AllowCastInCombat = false;
            bindAffinitySelfSpellTemplate.SkillLine = Configuration.DBCID_SKILLLINE_ALTERATION_ID;
            spellTemplates.Add(bindAffinitySelfSpellTemplate);

            // Bind Affinity
            SpellTemplate bindAffinitySpellTemplate = new SpellTemplate();
            bindAffinitySpellTemplate.Name = "Bind Affinity";
            bindAffinitySpellTemplate.WOWSpellID = Configuration.SPELLS_BINDANY_SPELLDBC_ID;
            bindAffinitySpellTemplate.Description = "Binds the soul of the target to their current location. Only works in Norrath.";
            bindAffinitySpellTemplate.SpellIconID = SpellIconDBC.GetDBCIDForSpellIconID(21);
            bindAffinitySpellTemplate.CastTimeInMS = 6000;
            bindAffinitySpellTemplate.RecoveryTimeInMS = 12000;
            bindAffinitySpellTemplate.SpellRange = 30;
            bindAffinitySpellTemplate.WOWTargetType = SpellWOWTargetType.TargetParty;
            bindAffinitySpellTemplate.SpellVisualID1 = 99; // Taken from soulstone
            bindAffinitySpellTemplate.PlayerLearnableByClassTrainer = true;
            bindAffinitySpellTemplate.AllowCastInCombat = false;
            bindAffinitySpellTemplate.SkillLine = Configuration.DBCID_SKILLLINE_ALTERATION_ID;
            spellTemplates.Add(bindAffinitySpellTemplate);

            // Phase aura 1 (Day)
            SpellTemplate dayPhaseSpellTemplate = new SpellTemplate();
            dayPhaseSpellTemplate.Name = "EQ Phase Day";
            dayPhaseSpellTemplate.Category = 0;
            dayPhaseSpellTemplate.InterruptFlags = 0;
            dayPhaseSpellTemplate.WOWSpellID = Configuration.SPELLS_DAYPHASE_SPELLDBC_ID;
            dayPhaseSpellTemplate.Description = "Able to see day EQ creatures";
            dayPhaseSpellTemplate.SpellIconID = 253;
            dayPhaseSpellTemplate.EffectType1 = SpellWOWEffectType.ApplyAura;
            dayPhaseSpellTemplate.EffectDieSides1 = 1;
            dayPhaseSpellTemplate.EffectAura1 = 261; // Phase Aura
            dayPhaseSpellTemplate.EffectMiscValue1 = 2;
            dayPhaseSpellTemplate.EffectBasePoints1 = -1;
            dayPhaseSpellTemplate.SchoolMask = 1;
            dayPhaseSpellTemplate.SkillLine = Configuration.DBCID_SKILLLINE_ALTERATION_ID;
            spellTemplates.Add(dayPhaseSpellTemplate);

            // Phase aura 2 (Night)
            SpellTemplate nightPhaseSpellTemplate = new SpellTemplate();
            nightPhaseSpellTemplate.Name = "EQ Phase Day";
            nightPhaseSpellTemplate.Category = 0;
            nightPhaseSpellTemplate.InterruptFlags = 0;
            nightPhaseSpellTemplate.WOWSpellID = Configuration.SPELLS_NIGHTPHASE_SPELLDBC_ID;
            nightPhaseSpellTemplate.Description = "Able to see night EQ creatures";
            nightPhaseSpellTemplate.EffectType1 = SpellWOWEffectType.ApplyAura;
            nightPhaseSpellTemplate.EffectDieSides1 = 1;
            nightPhaseSpellTemplate.SpellIconID = 253;
            nightPhaseSpellTemplate.EffectAura1 = 261; // Phase Aura
            nightPhaseSpellTemplate.EffectMiscValue1 = 4;
            nightPhaseSpellTemplate.EffectBasePoints1 = -1;
            nightPhaseSpellTemplate.SchoolMask = 1;
            nightPhaseSpellTemplate.SkillLine = Configuration.DBCID_SKILLLINE_ALTERATION_ID;
            spellTemplates.Add(nightPhaseSpellTemplate);

            Logger.WriteDebug("Generating custom spells completed.");
        }

        public void GenerateTradeskills(SortedDictionary<int, ItemTemplate> itemTemplatesByEQDBID, ref List<SpellTemplate> spellTemplates,
            out List<TradeskillRecipe> tradeskillRecipes)
        {
            Logger.WriteInfo("Converting tradeskills...");

            // Populate the tradeskill data
            TradeskillRecipe.PopulateTradeskillRecipes(itemTemplatesByEQDBID);

            // Create spells for the recipe actions
            Dictionary<string, int> recipeNameCounts = new Dictionary<string, int>();
            SortedDictionary<int, ItemTemplate> itemTemplatesByWOWEntryID = ItemTemplate.GetItemTemplatesByWOWEntryID();
            tradeskillRecipes = TradeskillRecipe.GetAllRecipes();
            foreach (TradeskillRecipe recipe in tradeskillRecipes)
            {
                SpellTemplate curSpellTemplate = new SpellTemplate();
                curSpellTemplate.CastTimeInMS = Configuration.TRADESKILL_CAST_TIME_IN_MS;
                curSpellTemplate.WOWSpellID = recipe.SpellID;

                // "None" recipes aren't regular recipes, but rather (typically) quest item combines
                if (recipe.Type == TradeskillType.None)
                {
                    curSpellTemplate.Name = string.Concat("Create ", recipe.Name);

                    // Attach the spell to every combiner item
                    foreach (int itemID in recipe.CombinerWOWItemIDs)
                    {
                        if (itemTemplatesByWOWEntryID.ContainsKey(itemID) == false)
                        {
                            Logger.WriteError(string.Concat("Unable to attach spell to combiner item ", itemID, " as it did not exist"));
                            continue;
                        }
                        ItemTemplate curItemTemplate = itemTemplatesByWOWEntryID[itemID];
                        if (curItemTemplate.SpellID1 != 0)
                        {
                            Logger.WriteError(string.Concat("Unable to attach spell to combiner item ", itemID, " as that item already had a spell attached"));
                            continue;
                        }
                        curItemTemplate.SpellID1 = curSpellTemplate.WOWSpellID;
                        curItemTemplate.Description = recipe.GetGeneratedDescription(itemTemplatesByWOWEntryID);

                        // These can't be containers anymore
                        curItemTemplate.ClassID = 12; // Quest
                        curItemTemplate.SubClassID = 12; // Quest
                        curItemTemplate.InventoryType = ItemWOWInventoryType.NoEquip;
                        curItemTemplate.SpellCooldown1 = 1;
                        curItemTemplate.SpellCategoryCooldown1 = 1000;
                        curItemTemplate.WOWItemMaterialType = -1;
                    }
                }
                else
                    curSpellTemplate.Name = recipe.Name;
            
                // Assign every component item as reagents
                foreach (var item in recipe.ComponentItemCountsByWOWItemID)
                    curSpellTemplate.Reagents.Add(new SpellTemplate.Reagent(item.Key, item.Value));

                // Create the produced item
                ItemTemplate? resultItemTemplate = null;
                if (recipe.ProducedItemCountsByWOWItemID.Count == 0)
                {
                    Logger.WriteError(string.Concat("Could not convert item template with id ", recipe.EQID, " due to there being no produced items"));
                    continue;
                }
                else if (recipe.ProducedItemCountsByWOWItemID.Count > 1)
                {
                    string containerName = string.Concat(recipe.Name, " Items");
                    resultItemTemplate = ItemTemplate.CreateTradeskillMultiItemContainer(containerName, recipe.ProducedItemCountsByWOWItemID);
                    recipe.ProducedFilledContainer = resultItemTemplate;
                }
                else
                {
                    resultItemTemplate = itemTemplatesByWOWEntryID[recipe.ProducedItemCountsByWOWItemID.Keys.First()];
                    curSpellTemplate.EffectDieSides1 = 1;
                    curSpellTemplate.EffectBasePoints1 = recipe.ProducedItemCountsByWOWItemID.Values.First() - 1;
                }
                if (resultItemTemplate == null)
                {
                    Logger.WriteError(string.Concat("Could not convert item template with id ", recipe.EQID, " as the result item template is NULL"));
                    continue;
                }

                // Avoid name collisions
                if (recipeNameCounts.ContainsKey(curSpellTemplate.Name) == true)
                {
                    recipeNameCounts[curSpellTemplate.Name]++;
                    string altName = string.Concat(curSpellTemplate.Name, " (Recipe ", recipeNameCounts[curSpellTemplate.Name], ")");
                    curSpellTemplate.Name = altName;
                }
                else
                    recipeNameCounts.Add(curSpellTemplate.Name, 1);

                curSpellTemplate.EffectType1 = SpellWOWEffectType.CreateItem;
                curSpellTemplate.EffectItemType1 = Convert.ToUInt32(resultItemTemplate.WOWEntryID);
                curSpellTemplate.SpellIconID = SpellIconDBC.GetDBCIDForItemIconID(resultItemTemplate.IconID);
                curSpellTemplate.SchoolMask = 1; // "Normal"
                curSpellTemplate.TradeskillRecipe = recipe;
                curSpellTemplate.SkillLine = recipe.SkillLineWOW;
                curSpellTemplate.RequiredTotemID1 = recipe.RequiredTotemID1;
                curSpellTemplate.RequiredTotemID2 = recipe.RequiredTotemID2;
                curSpellTemplate.SpellFocusID = Convert.ToUInt32(recipe.RequiredFocus);
                recipe.SetSpellVisualData(curSpellTemplate);
                spellTemplates.Add(curSpellTemplate);
            }

            // Generate spell trainer abilities
            SpellTrainerAbility.PopulateTradeskillAbilities(TradeskillRecipe.GetRecipesByTradeskillType());
            
            Logger.WriteDebug("Converting tradeskills completed.");
        }

        public void ExtractClientDBCFiles()
        {
            string wowExportPath = Configuration.PATH_EXPORT_FOLDER;

            Logger.WriteInfo("Extracting client DBC files...");

            // Make sure the patches folder is correct
            string wowPatchesFolder = Path.Combine(Configuration.PATH_WOW_ENUS_CLIENT_FOLDER, "Data", "enUS");
            if (Directory.Exists(wowPatchesFolder) == false)
                throw new Exception("WoW client patches folder does not exist at '" + wowPatchesFolder + "', did you set PATH_WOW_ENUS_CLIENT_FOLDER?");

            // Get a list of valid patch files (it's done this way to ensure sorting order is exactly right). Also ignore existing patch file
            List<string> patchFileNames = new List<string>();
            patchFileNames.Add(Path.Combine(wowPatchesFolder, "patch-enUS.MPQ"));
            string[] existingPatchFiles = Directory.GetFiles(wowPatchesFolder, "patch-*-*.MPQ");
            foreach (string existingPatchName in existingPatchFiles)
                if (existingPatchName.Contains(Configuration.PATH_PATCH_NEW_FILE_NAME_NO_EXT) == false)
                    patchFileNames.Add(existingPatchName);

            // Make sure all of the files are not locked
            foreach (string patchFileName in patchFileNames)
                if (FileTool.IsFileLocked(patchFileName))
                    throw new Exception("Patch file named '" + patchFileName + "' was locked and in use by another application");

            // Clear out any previously extracted DBC files
            Logger.WriteDebug("Deleting previously extracted DBC files");
            string exportedDBCFolder = Path.Combine(wowExportPath, "ExportedDBCFiles");
            FileTool.CreateBlankDirectory(exportedDBCFolder, false);

            // Generate a script to extract the DBC files
            Logger.WriteDebug("Generating script to extract DBC files");
            string workingGeneratedScriptsFolder = Path.Combine(wowExportPath, "GeneratedWorkingScripts");
            FileTool.CreateBlankDirectory(workingGeneratedScriptsFolder, true);
            StringBuilder dbcExtractScriptText = new StringBuilder();
            foreach (string patchFileName in patchFileNames)
                dbcExtractScriptText.AppendLine("extract \"" + patchFileName + "\" DBFilesClient\\* \"" + exportedDBCFolder + "\"");
            string dbcExtractionScriptFileName = Path.Combine(workingGeneratedScriptsFolder, "dbcextract.txt");
            using (var dbcExtractionScriptFile = new StreamWriter(dbcExtractionScriptFileName))
                dbcExtractionScriptFile.WriteLine(dbcExtractScriptText.ToString());

            // Extract the files using the script
            Logger.WriteDebug("Extracting DBC files");
            string mpqEditorFullPath = Path.Combine(Configuration.PATH_TOOLS_FOLDER, "ladikmpqeditor", "MPQEditor.exe");
            if (File.Exists(mpqEditorFullPath) == false)
                throw new Exception("Failed to extract DBC files. '" + mpqEditorFullPath + "' does not exist. (Be sure to set your Configuration.PATH_TOOLS_FOLDER properly)");
            string args = "console \"" + dbcExtractionScriptFileName + "\"";
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.Arguments = args;
            process.StartInfo.FileName = mpqEditorFullPath;
            process.Start();
            process.WaitForExit();

            Logger.WriteDebug("Extracting client DBC files complete");
        }

        public void CreatePatchMPQ()
        {
            Logger.WriteInfo("Building patch MPQ...");

            // Make sure the output folder exists
            if (Directory.Exists(Configuration.PATH_EXPORT_FOLDER) == false)
                throw new Exception("Export folder '" + Configuration.PATH_EXPORT_FOLDER + "' did not exist, make sure you set PATH_EXPORT_FOLDER");

            // Delete the old patch file, if it exists
            Logger.WriteDebug("Deleting old patch file if it exists");
            string outputPatchFileName = Path.Combine(Configuration.PATH_EXPORT_FOLDER, Configuration.PATH_PATCH_NEW_FILE_NAME_NO_EXT + ".MPQ");
            if (File.Exists(outputPatchFileName) == true)
                File.Delete(outputPatchFileName);

            // Generate a script to generate the mpq file
            Logger.WriteDebug("Generating script to generate MPQ file");
            string mpqReadyFolder = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "MPQReady");
            if (Directory.Exists(mpqReadyFolder) == false)
                throw new Exception("There was no MPQReady folder inside of '" + Configuration.PATH_EXPORT_FOLDER + "'");
            string workingGeneratedScriptsFolder = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "GeneratedWorkingScripts");
            FileTool.CreateBlankDirectory(workingGeneratedScriptsFolder, true);
            StringBuilder mpqCreateScriptText = new StringBuilder();
            mpqCreateScriptText.AppendLine("new \"" + outputPatchFileName + "\" 65536");
            mpqCreateScriptText.AppendLine("add \"" + outputPatchFileName + "\" \"" + mpqReadyFolder + "\\*\" /auto /r");
            string mpqNewScriptFileName = Path.Combine(workingGeneratedScriptsFolder, "mpqnew.txt");
            using (var mpqNewScriptFile = new StreamWriter(mpqNewScriptFileName))
                mpqNewScriptFile.WriteLine(mpqCreateScriptText.ToString());

            // Generate the new MPQ using the script
            Logger.WriteDebug("Generating MPQ file");
            string mpqEditorFullPath = Path.Combine(Configuration.PATH_TOOLS_FOLDER, "ladikmpqeditor", "MPQEditor.exe");
            if (File.Exists(mpqEditorFullPath) == false)
                throw new Exception("Failed to generate MPQ file. '" + mpqEditorFullPath + "' does not exist. (Be sure to set your Configuration.PATH_TOOLS_FOLDER properly)");
            string args = "console \"" + mpqNewScriptFileName + "\"";
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.Arguments = args;
            process.StartInfo.FileName = mpqEditorFullPath;
            process.Start();
            process.WaitForExit();

            Logger.WriteDebug("Building patch MPQ complete");
        }

        public void UpdatePatchMPQ()
        {
            Logger.WriteInfo("Updating patch MPQ...");
            string exportMPQFileName = Path.Combine(Configuration.PATH_EXPORT_FOLDER, Configuration.PATH_PATCH_NEW_FILE_NAME_NO_EXT + ".mpq");
            if (File.Exists(exportMPQFileName) == false)
            {
                Logger.WriteError("Attempted to update the patch MPQ, but it didn't exist at '" + exportMPQFileName + "'");
                return;
            }

            // Generate a script to update the new MPQ
            Logger.WriteDebug("Generating script to update the MPQ file");
            string mpqReadyFolder = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "MPQReady");
            if (Directory.Exists(mpqReadyFolder) == false)
                throw new Exception("There was no MPQReady folder inside of '" + Configuration.PATH_EXPORT_FOLDER + "'");
            string workingGeneratedScriptsFolder = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "GeneratedWorkingScripts");
            FileTool.CreateBlankDirectory(workingGeneratedScriptsFolder, true);
            StringBuilder mpqUpdateScriptText = new StringBuilder();
            
            // Zones
            foreach(string zoneName in Configuration.GENERATE_ONLY_LISTED_ZONE_SHORTNAMES)
            {
                // Add zone-specific folders
                // ZoneObjects
                string relativeZoneObjectsPath = Path.Combine("World", "Everquest", "ZoneObjects", zoneName);
                string fullZoneObjectsPath = Path.Combine(mpqReadyFolder, relativeZoneObjectsPath);
                mpqUpdateScriptText.AppendLine("add \"" + exportMPQFileName + "\" \"" + fullZoneObjectsPath + "\" \"" + relativeZoneObjectsPath + "\" /r");

                // ZoneTextures
                string relativeZoneTexturesPath = Path.Combine("World", "Everquest", "ZoneTextures", zoneName);
                string fullZoneZoneTexturesPath = Path.Combine(mpqReadyFolder, relativeZoneTexturesPath);
                mpqUpdateScriptText.AppendLine("add \"" + exportMPQFileName + "\" \"" + fullZoneZoneTexturesPath + "\" \"" + relativeZoneTexturesPath + "\" /r");

                // Maps
                string relativeZoneMapsPath = Path.Combine("World", "Maps", "EQ_" + zoneName);
                string fullZoneMapsPath = Path.Combine(mpqReadyFolder, relativeZoneMapsPath);
                mpqUpdateScriptText.AppendLine("add \"" + exportMPQFileName + "\" \"" + fullZoneMapsPath + "\" \"" + relativeZoneMapsPath + "\" /r");

                // WMO
                string relativeZoneWmoPath = Path.Combine("World", "wmo", "Everquest", zoneName);
                string fullZoneWmoPath = Path.Combine(mpqReadyFolder, relativeZoneWmoPath);
                mpqUpdateScriptText.AppendLine("add \"" + exportMPQFileName + "\" \"" + fullZoneWmoPath + "\" \"" + relativeZoneWmoPath + "\" /r");

                // Music
                string relativeZoneMusicPath = Path.Combine("Sound", "Music", "Everquest", zoneName);
                string fullZoneMusicPath = Path.Combine(mpqReadyFolder, relativeZoneMusicPath);
                mpqUpdateScriptText.AppendLine("add \"" + exportMPQFileName + "\" \"" + fullZoneMusicPath + "\" \"" + relativeZoneMusicPath + "\" /r");
            }

            // Transports
            if (Configuration.GENERATE_TRANSPORTS == true)
            {
                foreach (TransportShip transportShip in TransportShip.GetAllTransportShips())
                {
                    // WMO
                    string relativeTransportWmoPath = Path.Combine("World", "wmo", "Everquest", transportShip.MeshName);
                    string fullTransportWmoPath = Path.Combine(mpqReadyFolder, relativeTransportWmoPath);
                    mpqUpdateScriptText.AppendLine("add \"" + exportMPQFileName + "\" \"" + fullTransportWmoPath + "\" \"" + relativeTransportWmoPath + "\" /r");

                    // ZoneTextures
                    string relativeTransportTexturesPath = Path.Combine("World", "Everquest", "TransportTextures", transportShip.MeshName);
                    string fullTransportTexturesPath = Path.Combine(mpqReadyFolder, relativeTransportTexturesPath);
                    mpqUpdateScriptText.AppendLine("add \"" + exportMPQFileName + "\" \"" + fullTransportTexturesPath + "\" \"" + relativeTransportTexturesPath + "\" /r");
                }
                if (TransportLift.GetAllTransportLifts().Count > 0)
                {
                    // Transport lift models
                    string relativeTransportsPath = Path.Combine("World", "Everquest", "Transports");
                    string fullTransportsPath = Path.Combine(mpqReadyFolder, relativeTransportsPath);
                    mpqUpdateScriptText.AppendLine("add \"" + exportMPQFileName + "\" \"" + fullTransportsPath + "\" \"" + relativeTransportsPath + "\" /r");

                    // Transport lift sounds
                    string relativeDoodadSoundsPath = Path.Combine("Sound", "EQTransports");
                    string fullDoodadSoundsPath = Path.Combine(mpqReadyFolder, relativeDoodadSoundsPath);
                    mpqUpdateScriptText.AppendLine("add \"" + exportMPQFileName + "\" \"" + fullDoodadSoundsPath + "\" \"" + relativeDoodadSoundsPath + "\" /r");
                }
            }

            // Objects
            if (Configuration.GENERATE_OBJECTS == true)
            {                
                string relativeStaticDoodadsPath = Path.Combine("World", "Everquest", "StaticDoodads");
                string fullStaticDoodadsPath = Path.Combine(mpqReadyFolder, relativeStaticDoodadsPath);
                mpqUpdateScriptText.AppendLine("add \"" + exportMPQFileName + "\" \"" + fullStaticDoodadsPath + "\" \"" + relativeStaticDoodadsPath + "\" /r");

                string relativeGameObjectsPath = Path.Combine("World", "Everquest", "GameObjects");
                string fullGameObjectsPath = Path.Combine(mpqReadyFolder, relativeGameObjectsPath);
                mpqUpdateScriptText.AppendLine("add \"" + exportMPQFileName + "\" \"" + fullGameObjectsPath + "\" \"" + relativeGameObjectsPath + "\" /r");
            }

            // Creatures
            if (Configuration.GENERATE_CREATURES_AND_SPAWNS == true)
            {   
                string relativeCreaturePath = Path.Combine("Creature", "Everquest");
                string fullCreaturePath = Path.Combine(mpqReadyFolder, relativeCreaturePath);
                mpqUpdateScriptText.AppendLine("add \"" + exportMPQFileName + "\" \"" + fullCreaturePath + "\" \"" + relativeCreaturePath + "\" /r");

                string relativeCreatureSoundsPath = Path.Combine("Sound", "Creature", "Everquest");
                string fullCreatureSoundPath = Path.Combine(mpqReadyFolder, relativeCreatureSoundsPath);
                mpqUpdateScriptText.AppendLine("add \"" + exportMPQFileName + "\" \"" + fullCreatureSoundPath + "\" \"" + relativeCreatureSoundsPath + "\" /r");
            }

            // Items
            string relativeItemsPath = Path.Combine("Item");
            string fullItemsPath = Path.Combine(mpqReadyFolder, relativeItemsPath);
            mpqUpdateScriptText.AppendLine("add \"" + exportMPQFileName + "\" \"" + fullItemsPath + "\" \"" + relativeItemsPath + "\" /r");

            // Icons
            string relativeIconsPath = Path.Combine("Interface", "ICONS");
            string fullIconsPath = Path.Combine(mpqReadyFolder, relativeIconsPath);
            mpqUpdateScriptText.AppendLine("add \"" + exportMPQFileName + "\" \"" + fullIconsPath + "\" \"" + relativeIconsPath + "\" /r");

            // Ambient Sounds
            string relativeAmbientSoundsPath = Path.Combine("Sound", "Ambience", "Everquest");
            string fullAmbientSoundsPath = Path.Combine(mpqReadyFolder, relativeAmbientSoundsPath);
            mpqUpdateScriptText.AppendLine("add \"" + exportMPQFileName + "\" \"" + fullAmbientSoundsPath + "\" \"" + relativeAmbientSoundsPath + "\" /r");

            // Loading Screens
            string relativeLoadingScreensPath = Path.Combine("Interface", "Glues", "LoadingScreens");
            string fullLoadingScreensPath = Path.Combine(mpqReadyFolder, relativeLoadingScreensPath);
            mpqUpdateScriptText.AppendLine("add \"" + exportMPQFileName + "\" \"" + fullLoadingScreensPath + "\" \"" + relativeLoadingScreensPath + "\" /r");

            // Liquid Materials
            string relativeLiquidMaterialsPath = Path.Combine("XTEXTURES", "everquest");
            string fullLiquidMaterialsPath = Path.Combine(mpqReadyFolder, relativeLiquidMaterialsPath);
            mpqUpdateScriptText.AppendLine("add \"" + exportMPQFileName + "\" \"" + fullLiquidMaterialsPath + "\" \"" + relativeLiquidMaterialsPath + "\" /r");

            // DBC Files
            string relativeDBCClientPath = Path.Combine("DBFilesClient");
            string fullDBCClientPath = Path.Combine(mpqReadyFolder, relativeDBCClientPath);
            mpqUpdateScriptText.AppendLine("add \"" + exportMPQFileName + "\" \"" + fullDBCClientPath + "\" \"" + relativeDBCClientPath + "\" /r");

            // Finally, compact it
            mpqUpdateScriptText.AppendLine("compact \"" + exportMPQFileName + "\" /r");

            // Output the script to disk
            string mpqUpdateScriptFileName = Path.Combine(workingGeneratedScriptsFolder, "mpqupdate.txt");
            using (var mpqUpdateScriptFile = new StreamWriter(mpqUpdateScriptFileName))
                mpqUpdateScriptFile.WriteLine(mpqUpdateScriptText.ToString());

            // Update the MPQ using the script
            Logger.WriteDebug("Updating MPQ file");
            string mpqEditorFullPath = Path.Combine(Configuration.PATH_TOOLS_FOLDER, "ladikmpqeditor", "MPQEditor.exe");
            if (File.Exists(mpqEditorFullPath) == false)
                throw new Exception("Failed to update MPQ file. '" + mpqEditorFullPath + "' does not exist. (Be sure to set your Configuration.PATH_TOOLS_FOLDER properly)");
            string args = "console \"" + mpqUpdateScriptFileName + "\"";
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.Arguments = args;
            process.StartInfo.FileName = mpqEditorFullPath;
            process.Start();
            process.WaitForExit();

            Logger.WriteDebug("Building patch MPQ complete");
        }

        public void DeployClient()
        {
            Logger.WriteInfo("Deploying to client...");

            // Make sure a patch was created
            string sourcePatchFileNameAndPath = Path.Combine(Configuration.PATH_EXPORT_FOLDER, Configuration.PATH_PATCH_NEW_FILE_NAME_NO_EXT + ".MPQ");
            if (File.Exists(sourcePatchFileNameAndPath) == false)
            {
                Logger.WriteError("Failed to deploy to client. Patch at '" + sourcePatchFileNameAndPath + "' did not exist");
                return;
            }

            // Delete the old one if it's already deployed on the client
            string targetPatchFileNameAndPath = Path.Combine(Configuration.PATH_WOW_ENUS_CLIENT_FOLDER, "Data", "enUS", Configuration.PATH_PATCH_NEW_FILE_NAME_NO_EXT + ".MPQ");
            if (File.Exists(targetPatchFileNameAndPath) == true)
            {
                try
                {
                    File.Delete(targetPatchFileNameAndPath);
                }
                catch (Exception ex)
                {
                    Logger.WriteError("Failed to delete the file at '" + targetPatchFileNameAndPath + "', it may be in use (client running, open in MPQ editor, etc)");
                    if (ex.StackTrace != null)
                        Logger.WriteDebug(ex.StackTrace.ToString());
                    Logger.WriteError("Deploying to client failed");
                    return;
                }
            }

            // Copy it
            FileTool.CopyFile(sourcePatchFileNameAndPath, targetPatchFileNameAndPath);

            Logger.WriteDebug("Deploying to client complete");
        }

        public void ClearClientCache()
        {
            Logger.WriteInfo("Clearing client cache...");

            // If there is a folder, delete it
            string folderToDelete = Path.Combine(Configuration.PATH_WOW_ENUS_CLIENT_FOLDER, "Cache", "WDB");
            if (Directory.Exists(folderToDelete) == true)
            {
                Logger.WriteDebug("Client cache WDB folder found, so deleting...");
                Directory.Delete(folderToDelete, true);
                Logger.WriteDebug("Client cache WDB deleted.");
            }
            else
                Logger.WriteDebug("No client cache WDB folder detected");

            Logger.WriteDebug("Clearing client cache complete");
        }

        public void DeployServerFiles()
        {
            Logger.WriteInfo("Deploying files to server...");

            // Make sure source and target paths are good for DBC files
            string sourceServerDBCFolder = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "DBCFilesServer");
            if (Directory.Exists(sourceServerDBCFolder) == false)
            {
                Logger.WriteError("Could not deploy DBC files to the server, no folder existed at '" + sourceServerDBCFolder + "'");
                return;
            }
            if (Directory.Exists(Configuration.PATH_DEPLOY_SERVER_DBC_FILES_FOLDER) == false)
            {
                Logger.WriteError("Could not deploy DBC files to the server, no target folder existed at '" + Configuration.PATH_DEPLOY_SERVER_DBC_FILES_FOLDER + "'. Check that you set Configuration.PATH_DEPLOY_SERVER_DBC_FILES_FOLDER properly");
                return;
            }

            // Deploy the DBC files
            Logger.WriteDebug("Deploying DBC files to the server...");
            string[] dbcFiles = Directory.GetFiles(sourceServerDBCFolder);
            foreach (string dbcFile in dbcFiles)
            {
                string targetFileName = Path.Combine(Configuration.PATH_DEPLOY_SERVER_DBC_FILES_FOLDER, Path.GetFileName(dbcFile));
                FileTool.CopyFile(dbcFile, targetFileName);
            }


            Logger.WriteDebug("Deploying files to server complete");
        }
        
        public void DeployServerSQL()
        {
            Logger.WriteInfo("Deploying sql to server...");

            // Deploy all of the scripts
            try
            {
                // Character scripts
                string charactersSQLScriptFolder = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "SQLScripts", SQLFileType.Characters.ToString());
                if (Directory.Exists(charactersSQLScriptFolder) == false)
                {
                    Logger.WriteError("Could not deploy SQL scripts to server. Path '" + charactersSQLScriptFolder + "' did not exist");
                    return;
                }
                using (MySqlConnection connection = new MySqlConnection(Configuration.DEPLOY_SQL_CONNECTION_STRING_CHARACTERS))
                {
                    connection.Open();
                    string[] sqlFiles = Directory.GetFiles(charactersSQLScriptFolder);
                    foreach (string sqlFile in sqlFiles)
                    {
                        MySqlCommand command = new MySqlCommand();
                        command.Connection = connection;
                        command.CommandTimeout = 288000;
                        command.CommandText = FileTool.ReadAllDataFromFile(sqlFile);
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }

                // World scripts
                string worldSQLScriptFolder = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "SQLScripts", SQLFileType.World.ToString());
                if (Directory.Exists(worldSQLScriptFolder) == false)
                {
                    Logger.WriteError("Could not deploy SQL scripts to server. Path '" + worldSQLScriptFolder + "' did not exist");
                    return;
                }
                using (MySqlConnection connection = new MySqlConnection(Configuration.DEPLOY_SQL_CONNECTION_STRING_WORLD))
                {
                    connection.Open();
                    string[] sqlFiles = Directory.GetFiles(worldSQLScriptFolder);
                    foreach(string sqlFile in sqlFiles)
                    {
                        MySqlCommand command = new MySqlCommand();
                        command.Connection = connection;
                        command.CommandTimeout = 288000;
                        command.CommandText = FileTool.ReadAllDataFromFile(sqlFile);
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteError("Error occurred when executing a script: '" + ex.Message + "'");
                if (ex.StackTrace != null)
                    Logger.WriteDebug(ex.StackTrace.ToString());
                Logger.WriteError("Deploying sql to server failed.");
            }

            Logger.WriteDebug("Deploying sql to server complete");
        }

        public void CreateLoadingScreens()
        {
            string eqExportsConditionedPath = Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER;
            string exportMPQRootFolder = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "MPQReady");

            Logger.WriteInfo("Copying loading screens");
            string loadingScreensTextureFolder = Path.Combine(exportMPQRootFolder, "Interface", "Glues", "LoadingScreens");
            if (Directory.Exists(loadingScreensTextureFolder) == true)
                Directory.Delete(loadingScreensTextureFolder, true);
            Directory.CreateDirectory(loadingScreensTextureFolder);

            // Classic
            string classicInputFile = Path.Combine(eqExportsConditionedPath, "miscimages", "logo03resized.blp");
            string classicOutputFile = Path.Combine(loadingScreensTextureFolder, "LoadingScreenEQClassic.blp");
            if (File.Exists(classicInputFile) == false)
                Logger.WriteError("Could not find texture '" + classicInputFile + "', it did not exist. Did you run blpconverter?");
            else
                FileTool.CopyFile(classicInputFile, classicOutputFile);

            // Kunark
            string kunarkInputFile = Path.Combine(eqExportsConditionedPath, "miscimages", "eqkloadresized.blp");
            string kunarkOutputFile = Path.Combine(loadingScreensTextureFolder, "LoadingScreenEQKunark.blp");
            if (File.Exists(kunarkInputFile) == false)
                Logger.WriteError("Could not find texture '" + kunarkInputFile + "', it did not exist. Did you run blpconverter?");
            else
                FileTool.CopyFile(kunarkInputFile, kunarkOutputFile);

            // Velious
            string veliousInputFile = Path.Combine(eqExportsConditionedPath, "miscimages", "eqvloadresized.blp");
            string veliousOutputFile = Path.Combine(loadingScreensTextureFolder, "LoadingScreenEQVelious.blp");
            if (File.Exists(veliousInputFile) == false)
                Logger.WriteError("Could not find texture '" + veliousInputFile + "', it did not exist. Did you run blpconverter?");
            else
                FileTool.CopyFile(veliousInputFile, veliousOutputFile);
        }

        public void CreateLiquidMaterials()
        {
            string eqExportsConditionedPath = Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER;
            string exportMPQRootFolder = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "MPQReady");

            Logger.WriteInfo("Copying liquid material textures");

            string sourceTextureFolder = Path.Combine(eqExportsConditionedPath, "liquidsurfaces");
            string targetTextureFolder = Path.Combine(exportMPQRootFolder, "XTEXTURES", "everquest");
            if (Directory.Exists(targetTextureFolder) == true)
                Directory.Delete(targetTextureFolder, true);
            Directory.CreateDirectory(targetTextureFolder);
            FileTool.CopyDirectoryAndContents(sourceTextureFolder, targetTextureFolder, true, true, "*.blp");
        }

        public void ClearNonPlayerObtainableItemsAndRecipes(ref List<TradeskillRecipe> tradeskillRecipes, ref SortedDictionary<int, ItemTemplate> itemTemplatesByWOWEntryID)
        {
            Logger.WriteInfo("Removing non-player obtainable items and recipes...");

            // Remove invalid items and refresh the item list
            List<ItemTemplate> itemTemplatesToRemove = new List<ItemTemplate>();
            foreach(ItemTemplate itemTemplate in itemTemplatesByWOWEntryID.Values)
            {
                if (itemTemplate.IsPlayerObtainable() == false)
                    itemTemplatesToRemove.Add(itemTemplate);
            }
            foreach (ItemTemplate itemTemplate in itemTemplatesToRemove)
                ItemTemplate.RemoveItemTemplate(itemTemplate);
            itemTemplatesByWOWEntryID = ItemTemplate.GetItemTemplatesByWOWEntryID();

            // Remove invalid tradeskill recipes (and related spell trainer ability)
            bool moreToRemove = true;
            while (moreToRemove)
            {
                moreToRemove = false;
                for (int i = tradeskillRecipes.Count - 1; i >= 0; i--)
                {
                    TradeskillRecipe recipe = tradeskillRecipes[i];
                    bool hasInvalidItems = false;
                    foreach (var countByItemID in recipe.ComponentItemCountsByWOWItemID)
                    {
                        if (itemTemplatesByWOWEntryID.ContainsKey(countByItemID.Key) == true)
                        {
                            if (itemTemplatesByWOWEntryID[countByItemID.Key].IsPlayerObtainable() == false)
                            {
                                Logger.WriteWarning(string.Concat("TradeskillRecipe with ID ", recipe.EQID, " had a reagant with ID ", countByItemID.Key, " which is not player obtainable"));
                                hasInvalidItems = true;
                            }
                        }
                        else
                        {
                            Logger.WriteWarning(string.Concat("TradeskillRecipe with ID ", recipe.EQID, " had an invalid reagant with ID ", countByItemID.Key));
                            hasInvalidItems = true;
                        }
                    }
                    if (hasInvalidItems == true)
                    {
                        moreToRemove = true;

                        // Clear any produced items, if needed.  Need to check any other items to see if other tradeskills produce them to be sure
                        if (recipe.ProducedFilledContainer != null)
                            ItemTemplate.RemoveItemTemplate(recipe.ProducedFilledContainer);
                        foreach (int producedItemID in recipe.ProducedItemCountsByWOWItemID.Keys)
                            if (itemTemplatesByWOWEntryID.ContainsKey(producedItemID) == true)
                                itemTemplatesByWOWEntryID[producedItemID].NumOfTradeskillsThatCreateIt--;

                        // Remove any trainer ability associated
                        if (recipe.Type != TradeskillType.Unknown && recipe.Type != TradeskillType.None)
                            SpellTrainerAbility.RemoveTrainerAbilityUsingTradeskillRecipe(recipe);

                        // Remove the recipe
                        TradeskillRecipe.RemoveRecipe(recipe);
                    }
                }

                // Remove invalid items again and refresh the list
                itemTemplatesToRemove.Clear();
                foreach (ItemTemplate itemTemplate in itemTemplatesByWOWEntryID.Values)
                {
                    if (itemTemplate.IsPlayerObtainable() == false)
                        itemTemplatesToRemove.Add(itemTemplate);
                }
                foreach (ItemTemplate itemTemplate in itemTemplatesToRemove)
                    ItemTemplate.RemoveItemTemplate(itemTemplate);
            }
        }

        public void CreateDBCFiles(List<Zone> zones, List<CreatureModelTemplate> creatureModelTemplates, List<SpellTemplate> spellTemplates)
        {
            string wowExportPath = Configuration.PATH_EXPORT_FOLDER;

            Logger.WriteInfo("Creating DBC Files...");

            // Make sure there is a folder of files
            string dbcInputFolder = Path.Combine(wowExportPath, "ExportedDBCFiles");
            if (Directory.Exists(dbcInputFolder) == false)
            {
                Logger.WriteError("Failed to create DBC files, as '" + dbcInputFolder + "' did not exist");
                return;
            }

            // Clear prior folders
            string dbcOutputClientFolder = Path.Combine(wowExportPath, "MPQReady", "DBFilesClient");
            if (Directory.Exists(dbcOutputClientFolder) == true)
                Directory.Delete(dbcOutputClientFolder, true);
            Directory.CreateDirectory(dbcOutputClientFolder);
            string dbcOutputServerFolder = Path.Combine(wowExportPath, "DBCFilesServer");
            if (Directory.Exists(dbcOutputServerFolder) == true)
                Directory.Delete(dbcOutputServerFolder, true);
            Directory.CreateDirectory(dbcOutputServerFolder);

            // Load the files
            AreaTableDBC areaTableDBC = new AreaTableDBC();
            areaTableDBC.LoadFromDisk(dbcInputFolder, "AreaTable.dbc");
            AreaTriggerDBC areaTriggerDBC = new AreaTriggerDBC();
            areaTriggerDBC.LoadFromDisk(dbcInputFolder, "AreaTrigger.dbc");
            CharStartOutfitDBC charStartOutfitDBC = new CharStartOutfitDBC();
            if (Configuration.PLAYER_USE_EQ_START_ITEMS == true)
                charStartOutfitDBC.LoadFromDisk(dbcInputFolder, "CharStartOutfit.dbc");
            CreatureDisplayInfoDBC creatureDisplayInfoDBC = new CreatureDisplayInfoDBC();
            creatureDisplayInfoDBC.LoadFromDisk(dbcInputFolder, "CreatureDisplayInfo.dbc");
            CreatureModelDataDBC creatureModelDataDBC = new CreatureModelDataDBC();
            creatureModelDataDBC.LoadFromDisk(dbcInputFolder, "CreatureModelData.dbc");
            CreatureSoundDataDBC creatureSoundDataDBC = new CreatureSoundDataDBC();
            creatureSoundDataDBC.LoadFromDisk(dbcInputFolder, "CreatureSoundData.dbc");
            FactionDBC factionDBC = new FactionDBC();
            factionDBC.LoadFromDisk(dbcInputFolder, "Faction.dbc");
            FactionTemplateDBC factionTemplateDBC = new FactionTemplateDBC();
            factionTemplateDBC.LoadFromDisk(dbcInputFolder, "FactionTemplate.dbc");
            FootstepTerrainLookupDBC footstepTerrainLookupDBC = new FootstepTerrainLookupDBC();
            footstepTerrainLookupDBC.LoadFromDisk(dbcInputFolder, "FootstepTerrainLookup.dbc");
            GameObjectDisplayInfoDBC gameObjectDisplayInfoDBC = new GameObjectDisplayInfoDBC();
            gameObjectDisplayInfoDBC.LoadFromDisk(dbcInputFolder, "GameObjectDisplayInfo.dbc");
            ItemDBC itemDBC = new ItemDBC();
            itemDBC.LoadFromDisk(dbcInputFolder, "Item.dbc");
            ItemDisplayInfoDBC itemDisplayInfoDBC = new ItemDisplayInfoDBC();
            itemDisplayInfoDBC.LoadFromDisk(dbcInputFolder, "ItemDisplayInfo.dbc");
            LightDBC lightDBC = new LightDBC();
            lightDBC.LoadFromDisk(dbcInputFolder, "Light.dbc");
            LightFloatBandDBC lightFloatBandDBC = new LightFloatBandDBC();
            lightFloatBandDBC.LoadFromDisk(dbcInputFolder, "LightFloatBand.dbc");
            LightIntBandDBC lightIntBandDBC = new LightIntBandDBC();
            lightIntBandDBC.LoadFromDisk(dbcInputFolder, "LightIntBand.dbc");
            LightParamsDBC lightParamsDBC = new LightParamsDBC();
            lightParamsDBC.LoadFromDisk(dbcInputFolder, "LightParams.dbc");
            LiquidTypeDBC liquidTypeDBC = new LiquidTypeDBC();
            liquidTypeDBC.LoadFromDisk(dbcInputFolder, "LiquidType.dbc");
            LoadingScreensDBC loadingScreensDBC = new LoadingScreensDBC();
            loadingScreensDBC.LoadFromDisk(dbcInputFolder, "LoadingScreens.dbc");
            MapDBC mapDBCClient = new MapDBC();
            mapDBCClient.LoadFromDisk(dbcInputFolder, "Map.dbc");
            MapDBC mapDBCServer = new MapDBC();
            mapDBCServer.LoadFromDisk(dbcInputFolder, "Map.dbc");
            MapDifficultyDBC mapDifficultyDBC = new MapDifficultyDBC();
            mapDifficultyDBC.LoadFromDisk(dbcInputFolder, "MapDifficulty.dbc");
            //SkillLineDBC skillLineDBC = new SkillLineDBC();
            //skillLineDBC.LoadFromDisk(dbcInputFolder, "SkillLine.dbc");
            SkillLineAbilityDBC skillLineAbilityDBC = new SkillLineAbilityDBC();
            skillLineAbilityDBC.LoadFromDisk(dbcInputFolder, "SkillLineAbility.dbc");
            SoundAmbienceDBC soundAmbienceDBC = new SoundAmbienceDBC();
            soundAmbienceDBC.LoadFromDisk(dbcInputFolder, "SoundAmbience.dbc");
            SoundEntriesDBC soundEntriesDBC = new SoundEntriesDBC();
            soundEntriesDBC.LoadFromDisk(dbcInputFolder, "SoundEntries.dbc");
            SpellDBC spellDBC = new SpellDBC();
            spellDBC.LoadFromDisk(dbcInputFolder, "Spell.dbc");
            SpellCastTimesDBC spellCastTimesDBC = new SpellCastTimesDBC();
            spellCastTimesDBC.LoadFromDisk(dbcInputFolder, "SpellCastTimes.dbc");
            SpellDurationDBC spellDurationDBC = new SpellDurationDBC();
            spellDurationDBC.LoadFromDisk(dbcInputFolder, "SpellDuration.dbc");
            SpellIconDBC spellIconDBC = new SpellIconDBC();
            spellIconDBC.LoadFromDisk(dbcInputFolder, "SpellIcon.dbc");
            SpellRangeDBC spellRangeDBC = new SpellRangeDBC();
            spellRangeDBC.LoadFromDisk(dbcInputFolder, "SpellRange.dbc");
            TaxiPathDBC taxiPathDBC = new TaxiPathDBC();
            taxiPathDBC.LoadFromDisk(dbcInputFolder, "TaxiPath.dbc");
            TaxiPathNodeDBC taxiPathNodeDBC = new TaxiPathNodeDBC();
            taxiPathNodeDBC.LoadFromDisk(dbcInputFolder, "TaxiPathNode.dbc");
            TotemCategoryDBC totemCategoryDBC = new TotemCategoryDBC();
            totemCategoryDBC.LoadFromDisk(dbcInputFolder, "TotemCategory.dbc");
            TransportAnimationDBC transportAnimationDBC = new TransportAnimationDBC();
            transportAnimationDBC.LoadFromDisk(dbcInputFolder, "TransportAnimation.dbc");
            WorldSafeLocsDBC worldSafeLocsDBC = new WorldSafeLocsDBC();
            worldSafeLocsDBC.LoadFromDisk(dbcInputFolder, "WorldSafeLocs.dbc");
            WMOAreaTableDBC wmoAreaTableDBC = new WMOAreaTableDBC();
            wmoAreaTableDBC.LoadFromDisk(dbcInputFolder, "WMOAreaTable.dbc");
            ZoneMusicDBC zoneMusicDBC = new ZoneMusicDBC();
            zoneMusicDBC.LoadFromDisk(dbcInputFolder, "ZoneMusic.dbc");

            // Save common light parameters
            lightFloatBandDBC.AddRows(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersClearWeather);
            lightFloatBandDBC.AddRows(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersClearWeatherUnderwater);
            lightFloatBandDBC.AddRows(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersStormyWeather);
            lightFloatBandDBC.AddRows(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersStormyWeatherUnderwater);
            lightIntBandDBC.AddRows(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersClearWeather);
            lightIntBandDBC.AddRows(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersClearWeatherUnderwater);
            lightIntBandDBC.AddRows(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersStormyWeather);
            lightIntBandDBC.AddRows(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersStormyWeatherUnderwater);
            lightParamsDBC.AddRow(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersClearWeather);
            lightParamsDBC.AddRow(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersClearWeatherUnderwater);
            lightParamsDBC.AddRow(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersStormyWeather);
            lightParamsDBC.AddRow(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersStormyWeatherUnderwater);

            // Liquid is common
            liquidTypeDBC.AddRows();

            // LoadingScreens is common
            loadingScreensDBC.AddRow(Configuration.DBCID_LOADINGSCREEN_ID_START, "EQAntonica", "Interface\\Glues\\LoadingScreens\\LoadingScreenEQClassic.blp");
            loadingScreensDBC.AddRow(Configuration.DBCID_LOADINGSCREEN_ID_START + 1, "EQKunark", "Interface\\Glues\\LoadingScreens\\LoadingScreenEQKunark.blp");
            loadingScreensDBC.AddRow(Configuration.DBCID_LOADINGSCREEN_ID_START + 2, "EQVelious", "Interface\\Glues\\LoadingScreens\\LoadingScreenEQVelious.blp");

            // Creatures sounds
            Dictionary<string, int> creatureFootstepIDBySoundNames = new Dictionary<string, int>();
            int curCreatureFootstepID = Configuration.DBCID_FOOTSTEPTERRAINLOOKUP_CREATUREFOOTSTEPID_START;
            foreach (CreatureModelTemplate creatureModelTemplate in creatureModelTemplates)
            {
                creatureDisplayInfoDBC.AddRow(creatureModelTemplate.DBCCreatureDisplayID, creatureModelTemplate.DBCCreatureModelDataID);
                string relativeModelPath = "Creature\\Everquest\\" + creatureModelTemplate.GetCreatureModelFolderName() + "\\" + creatureModelTemplate.GenerateFileName() + ".mdx";
                creatureModelDataDBC.AddRow(creatureModelTemplate, relativeModelPath);
                creatureSoundDataDBC.AddRow(creatureModelTemplate.DBCCreatureSoundDataID, creatureModelTemplate.Race, CreatureRace.FootstepIDBySoundName[creatureModelTemplate.Race.SoundWalkingName]);
            }
            string creatureSoundsDirectory = "Sound\\Creature\\Everquest";
            foreach (var sound in CreatureRace.SoundsBySoundName)
                soundEntriesDBC.AddRow(sound.Value, sound.Value.Name, creatureSoundsDirectory);

            // Faction
            foreach (CreatureFaction creatureFaction in CreatureFaction.GetCreatureFactionsByFactionID().Values)
            {
                factionDBC.AddRow(creatureFaction);
                if (creatureFaction.Name != Configuration.CREATURE_FACTION_ROOT_NAME)
                    factionTemplateDBC.AddRow(creatureFaction);
            }

            // Footstep Terrain Lookup (for creatures)
            foreach(var footstepIDBySoundID in CreatureRace.FootstepIDBySoundID)
                footstepTerrainLookupDBC.AddRow(footstepIDBySoundID.Value, footstepIDBySoundID.Key);

            // Zone-specific records
            foreach (Zone zone in zones)
            {
                ZoneProperties zoneProperties = zone.ZoneProperties;

                // AreaTable
                areaTableDBC.AddRow(Convert.ToInt32(zone.DefaultArea.DBCAreaTableID), zoneProperties.DBCMapID, 0, zone.DefaultArea.AreaMusic, zone.DefaultArea.AreaAmbientSound, zone.DefaultArea.DisplayName, zoneProperties.IsRestingZoneWide);
                foreach (ZoneArea subArea in zoneProperties.ZoneAreas)
                    areaTableDBC.AddRow(Convert.ToInt32(subArea.DBCAreaTableID), zoneProperties.DBCMapID, Convert.ToInt32(subArea.DBCParentAreaTableID), subArea.AreaMusic, subArea.AreaAmbientSound, subArea.DisplayName, zoneProperties.IsRestingZoneWide);

                // AreaTrigger
                foreach (ZonePropertiesZoneLineBox zoneLine in zoneProperties.ZoneLineBoxes)
                    areaTriggerDBC.AddRow(zoneLine.AreaTriggerID, zoneProperties.DBCMapID, zoneLine.BoxPosition.X, zoneLine.BoxPosition.Y,
                        zoneLine.BoxPosition.Z, zoneLine.BoxLength, zoneLine.BoxWidth, zoneLine.BoxHeight, zoneLine.BoxOrientation);

                // Light Tables
                if (zoneProperties.CustomZonewideEnvironmentProperties != null)
                {
                    ZoneEnvironmentSettings curZoneEnvironmentSettings = zoneProperties.CustomZonewideEnvironmentProperties;

                    lightDBC.AddRow(zoneProperties.DBCMapID, curZoneEnvironmentSettings);

                    lightFloatBandDBC.AddRows(curZoneEnvironmentSettings.ParamatersClearWeather);
                    lightFloatBandDBC.AddRows(curZoneEnvironmentSettings.ParamatersClearWeatherUnderwater);
                    lightFloatBandDBC.AddRows(curZoneEnvironmentSettings.ParamatersStormyWeather);
                    lightFloatBandDBC.AddRows(curZoneEnvironmentSettings.ParamatersStormyWeatherUnderwater);

                    lightIntBandDBC.AddRows(curZoneEnvironmentSettings.ParamatersClearWeather);
                    lightIntBandDBC.AddRows(curZoneEnvironmentSettings.ParamatersClearWeatherUnderwater);
                    lightIntBandDBC.AddRows(curZoneEnvironmentSettings.ParamatersStormyWeather);
                    lightIntBandDBC.AddRows(curZoneEnvironmentSettings.ParamatersStormyWeatherUnderwater);

                    lightParamsDBC.AddRow(curZoneEnvironmentSettings.ParamatersClearWeather);
                    lightParamsDBC.AddRow(curZoneEnvironmentSettings.ParamatersClearWeatherUnderwater);
                    lightParamsDBC.AddRow(curZoneEnvironmentSettings.ParamatersStormyWeather);
                    lightParamsDBC.AddRow(curZoneEnvironmentSettings.ParamatersStormyWeatherUnderwater);
                }
                else
                {
                    lightDBC.AddRow(zoneProperties.DBCMapID, ZoneProperties.CommonOutdoorEnvironmentProperties);
                }

                // Map
                mapDBCClient.AddRow(zoneProperties.DBCMapID, "EQ_" + zone.ShortName, zone.DescriptiveName, 0, zone.LoadingScreenID);
                mapDBCServer.AddRow(zoneProperties.DBCMapID, "EQ_" + zone.ShortName, zone.DescriptiveName, Convert.ToInt32(zone.DefaultArea.DBCAreaTableID), zone.LoadingScreenID);

                // Map Difficulty
                mapDifficultyDBC.AddRow(zoneProperties.DBCMapID, zoneProperties.DBCMapDifficultyID);

                // Sound Ambience
                foreach (ZoneAreaAmbientSound zoneAreaAmbient in zone.ZoneAreaAmbientSounds)
                {
                    int soundIDDay = 0;
                    if (zoneAreaAmbient.DaySound != null)
                        soundIDDay = zoneAreaAmbient.DaySound.DBCID;
                    int soundIDNight = 0;
                    if (zoneAreaAmbient.NightSound != null)
                        soundIDNight = zoneAreaAmbient.NightSound.DBCID;
                    soundAmbienceDBC.AddRow(zoneAreaAmbient.DBCID, soundIDDay, soundIDNight);
                }

                // SoundEntries
                string musicDirectory = "Sound\\Music\\Everquest\\" + zoneProperties.ShortName;
                foreach (var sound in zone.MusicSoundsByFileNameNoExt)
                {
                    string fileNameWithExt = sound.Value.AudioFileNameNoExt + ".mp3";
                    soundEntriesDBC.AddRow(sound.Value, fileNameWithExt, musicDirectory);
                }
                string ambientSoundsDirectory = "Sound\\Ambience\\Everquest";
                foreach (var sound in zone.AmbientSoundsByFileNameNoExt)
                {
                    string fileNameWithExt = sound.Value.AudioFileNameNoExt + ".wav";
                    soundEntriesDBC.AddRow(sound.Value, fileNameWithExt, ambientSoundsDirectory);
                }
                foreach (SoundInstance soundInstance in zone.SoundInstances)
                {
                    if (soundInstance.Sound == null)
                        Logger.WriteError("Could not create SoundEntry.dbc record for sound instance '" + soundInstance.SoundFileNameDayNoExt + "' since the Sound was null");
                    else
                    {
                        string fileNameWithExt = soundInstance.Sound.AudioFileNameNoExt + ".wav";
                        soundEntriesDBC.AddRow(soundInstance.Sound, fileNameWithExt, ambientSoundsDirectory);
                    }
                }

                // WMOAreaTable (Header than groups)
                wmoAreaTableDBC.AddRow(Convert.ToInt32(zoneProperties.DBCWMOID), Convert.ToInt32(-1), 0, Convert.ToInt32(zone.DefaultArea.DBCAreaTableID), zone.DescriptiveName); // Header record
                foreach (ZoneModelObject wmo in zone.ZoneObjectModels)
                    wmoAreaTableDBC.AddRow(Convert.ToInt32(zoneProperties.DBCWMOID), Convert.ToInt32(wmo.WMOGroupID), 0, Convert.ToInt32(wmo.AreaTableID), wmo.DisplayName);

                // ZoneMusic
                foreach (ZoneAreaMusic zoneMusic in zone.ZoneAreaMusics)
                {
                    int soundIDDay = 0;
                    if (zoneMusic.DaySound != null)
                        soundIDDay = zoneMusic.DaySound.DBCID;
                    int soundIDNight = 0;
                    if (zoneMusic.NightSound != null)
                        soundIDNight = zoneMusic.NightSound.DBCID;
                    zoneMusicDBC.AddRow(zoneMusic.DBCID, zoneMusic.Name, soundIDDay, soundIDNight);
                }
            }

            // GameObjects
            if (Configuration.GENERATE_OBJECTS == true)
            {
                Dictionary<(string, GameObjectOpenType), int> gameObjectDisplayInfoIDsByModelNameAndOpenType = GameObject.GetGameObjectDisplayInfoIDsByModelNameAndOpenType();
                Dictionary<(string, GameObjectOpenType), ObjectModel> gameObjectModelsByNameAndOpenType = GameObject.GetNonDoodadObjectModelsByNameAndOpenType();
                Dictionary<(string, GameObjectOpenType), Sound> openSoundsByModelNameAndOpenType = GameObject.OpenSoundsByModelNameAndOpenType;
                Dictionary<(string, GameObjectOpenType), Sound> closeSoundsByModelNameAndOpenType = GameObject.CloseSoundsByModelNameAndOpenType;
                foreach (ValueTuple<string, GameObjectOpenType> nameAndOpenType in gameObjectDisplayInfoIDsByModelNameAndOpenType.Keys)
                {
                    // Sounds
                    int openSoundEntryID = 0;
                    int closeSoundEntryID = 0;
                    if (openSoundsByModelNameAndOpenType.ContainsKey(nameAndOpenType) == true)
                    {
                        openSoundEntryID = openSoundsByModelNameAndOpenType[nameAndOpenType].DBCID;
                        closeSoundEntryID = closeSoundsByModelNameAndOpenType[nameAndOpenType].DBCID;
                    }

                    // Display info
                    string fileName = string.Concat(nameAndOpenType.Item1, "_", nameAndOpenType.Item2.ToString());
                    string relativeObjectFileName = Path.Combine("World", "Everquest", "GameObjects", fileName, fileName + ".mdx");
                    gameObjectDisplayInfoDBC.AddRow(gameObjectDisplayInfoIDsByModelNameAndOpenType[nameAndOpenType],
                        relativeObjectFileName.ToLower(),
                        gameObjectModelsByNameAndOpenType[nameAndOpenType].VisibilityBoundingBox, 
                        openSoundEntryID, closeSoundEntryID);
                }
                string soundDirectoryRelative = Path.Combine("Sound", "GameObjects");
                foreach (Sound sound in GameObject.AllSoundsBySoundName.Values)
                    soundEntriesDBC.AddRow(sound, sound.AudioFileNameNoExt + ".wav", soundDirectoryRelative);
            }

            // Graveyards
            Dictionary<string, ZoneProperties> zonePropertiesByShortName = ZoneProperties.GetZonePropertyListByShortName();
            foreach (ZonePropertiesGraveyard graveyard in ZonePropertiesGraveyard.GetAllGraveyards())
            {
                if (zonePropertiesByShortName.ContainsKey(graveyard.LocationShortName) == true)
                {
                    int mapID = zonePropertiesByShortName[graveyard.LocationShortName].DBCMapID;
                    worldSafeLocsDBC.AddRow(graveyard, mapID);
                }
            }

            // Character start data
            // Must come before "Item Data"
            SortedDictionary<int, ItemTemplate> itemTemplatesByWOWEntry = ItemTemplate.GetItemTemplatesByWOWEntryID();
            if (Configuration.PLAYER_USE_EQ_START_ITEMS == true)
            {
                // Create the non-eq items to be used
                ItemTemplate itemHearthstone = new ItemTemplate(6948, ItemWOWInventoryType.NoEquip);
                itemHearthstone.IsGivenAsStartItem = true;
                itemHearthstone.IsExistingItemAlready = true;
                ItemTemplate itemTotem = new ItemTemplate(46978, ItemWOWInventoryType.NoEquip);
                itemTotem.IsGivenAsStartItem = true;
                itemTotem.IsExistingItemAlready = true;

                // Populate for all combinations, all races
                foreach (var classRaceProperties in PlayerClassRaceProperties.GetClassRacePropertiesByRaceAndClassID())
                {
                    // Grab all of the items
                    List<ItemTemplate> startingItems = new List<ItemTemplate>();
                    foreach (int itemID in classRaceProperties.Value.StartItemIDs)
                    {
                        if (itemID == 46978)
                            startingItems.Add(itemTotem);
                        else if (itemTemplatesByWOWEntry.ContainsKey(itemID) == false)
                            Logger.WriteError(string.Concat("Failed to pull startup item with wow entry id '", itemID, "' since it did not exist"));
                        else
                            startingItems.Add(itemTemplatesByWOWEntry[itemID]);
                    }

                    // Add the hearthstone if configured to do so
                    if (Configuration.PLAYER_ADD_HEARTHSTONE_IF_USE_EQ_START_ITEMS == true)
                        startingItems.Add(itemHearthstone);

                    // Add the rows
                    charStartOutfitDBC.AddRowsForSexes(Convert.ToByte(classRaceProperties.Value.RaceID), Convert.ToByte(classRaceProperties.Value.ClassID), startingItems);
                }
            }

            // Item data
            // Note: Must come AFTER character start data
            foreach (ItemTemplate itemTemplate in itemTemplatesByWOWEntry.Values)
            {
                if (itemTemplate.IsExistingItemAlready == true)
                    continue;
                itemDBC.AddRow(itemTemplate);
            }
            foreach (ItemDisplayInfo itemDisplayInfo in ItemDisplayInfo.ItemDisplayInfos)
                itemDisplayInfoDBC.AddRow(itemDisplayInfo);

            // SkillLine
            //skillLineDBC.AddRow(Configuration.DBCID_SKILLLINE_ALTERATION_ID, "Alteration");            

            // Spells
            for (int i = 0; i < 23; i++)
                spellIconDBC.AddSpellIconRow(i);
            for (int i = 0; i < 751; i++)
                spellIconDBC.AddItemIconRow(i);
            foreach (SpellTemplate spellTemplate in spellTemplates)
            {
                spellDBC.AddRow(spellTemplate);
                if (spellTemplate.SkillLine != 0)
                    skillLineAbilityDBC.AddRow(SkillLineAbilityDBC.GenerateID(), spellTemplate);
            }
            foreach (var spellCastTimeDBCIDByCastTime in SpellTemplate.SpellCastTimeDBCIDsByCastTime)
                spellCastTimesDBC.AddRow(spellCastTimeDBCIDByCastTime.Value, spellCastTimeDBCIDByCastTime.Key);
            foreach (var spellRangeDBCIDByRange in SpellTemplate.SpellRangeDBCIDsBySpellRange)
                spellRangeDBC.AddRow(spellRangeDBCIDByRange.Value, spellRangeDBCIDByRange.Key);
            foreach (var spellDurationDBCIDByDurationInMS in SpellTemplate.SpellDurationDBCIDsByDurationInMS)
                spellDurationDBC.AddRow(spellDurationDBCIDByDurationInMS.Value, spellDurationDBCIDByDurationInMS.Key);

            // Transports
            if (Configuration.GENERATE_TRANSPORTS == true)
            {
                foreach (var transportWMOByID in TransportShip.TransportShipWMOsByGameObjectDisplayInfoID)
                {
                    gameObjectDisplayInfoDBC.AddRow(transportWMOByID.Key, transportWMOByID.Value.RootFileRelativePathWithFileName.ToLower(), transportWMOByID.Value.BoundingBox);
                    wmoAreaTableDBC.AddRow(Convert.ToInt32(transportWMOByID.Value.Zone.ZoneProperties.DBCWMOID), Convert.ToInt32(-1), 0, 0, transportWMOByID.Value.Zone.DescriptiveName); // Header record
                    foreach (ZoneModelObject wmo in transportWMOByID.Value.Zone.ZoneObjectModels)
                        wmoAreaTableDBC.AddRow(Convert.ToInt32(transportWMOByID.Value.Zone.ZoneProperties.DBCWMOID), Convert.ToInt32(wmo.WMOGroupID), 0, 0, wmo.DisplayName);
                }
                Dictionary<string, int> mapIDsByShortName = new Dictionary<string, int>();
                foreach (Zone zone in zones)
                    mapIDsByShortName.Add(zone.ShortName.ToLower().Trim(), zone.ZoneProperties.DBCMapID);
                HashSet<int> validTransportGroupIDs = new HashSet<int>();
                foreach (TransportShip curTransportShip in TransportShip.GetAllTransportShips())
                {
                    // Only add this transport ship if the full path is zones that are loaded
                    bool zonesAreLoaded = true;
                    foreach (string touchedZone in curTransportShip.GetTouchedZonesSplitOut())
                    {
                        if (mapIDsByShortName.ContainsKey(touchedZone.ToLower().Trim()) == false)
                        {
                            zonesAreLoaded = false;
                            Logger.WriteDebug("Skipping transport ship since zone '" + touchedZone + "' isn't being converted");
                            break;
                        }
                    }
                    if (zonesAreLoaded == false)
                        continue;
                    validTransportGroupIDs.Add(curTransportShip.PathGroupID);

                    // TODO: Make loading screen configurable
                    mapDBCClient.AddRow(Convert.ToInt32(curTransportShip.MapID), curTransportShip.MeshName, curTransportShip.Name, 0, Configuration.DBCID_LOADINGSCREEN_ID_START);
                    mapDBCServer.AddRow(Convert.ToInt32(curTransportShip.MapID), curTransportShip.MeshName, curTransportShip.Name, 0, Configuration.DBCID_LOADINGSCREEN_ID_START);
                    taxiPathDBC.AddRow(curTransportShip.TaxiPathID);
                }
                foreach (TransportShipPathNode shipNode in TransportShipPathNode.GetAllPathNodesSorted())
                {
                    if (shipNode.WOWPathID == 0)
                        continue;
                    if (validTransportGroupIDs.Contains(shipNode.PathGroup) == false)
                        continue;
                    int mapID = mapIDsByShortName[shipNode.MapShortName.ToLower().Trim()];
                    taxiPathNodeDBC.AddRow(shipNode.WOWPathID, shipNode.StepNumber, mapID, shipNode.XPosition, shipNode.YPosition,
                        shipNode.ZPosition, shipNode.PauseTimeInSec);
                }
                foreach (var m2ByGameObjectID in TransportLift.ObjectModelM2ByMeshGameObjectDisplayID)
                {
                    string relativeObjectFileName = Path.Combine("World", "Everquest", "Transports", m2ByGameObjectID.Value.ObjectModel.Name, m2ByGameObjectID.Value.ObjectModel.Name + ".mdx");
                    gameObjectDisplayInfoDBC.AddRow(m2ByGameObjectID.Key, relativeObjectFileName.ToLower(), m2ByGameObjectID.Value.ObjectModel.VisibilityBoundingBox);
                }
                foreach (var m2ByGameObjectID in TransportLiftTrigger.ObjectModelM2ByMeshGameObjectDisplayID)
                {
                    string relativeObjectFileName = Path.Combine("World", "Everquest", "Transports", m2ByGameObjectID.Value.ObjectModel.Name, m2ByGameObjectID.Value.ObjectModel.Name + ".mdx");
                    int openSoundEntryID = 0;
                    if (m2ByGameObjectID.Value.ObjectModel.SoundsByAnimationType.ContainsKey(AnimationType.Open) == true)
                        openSoundEntryID = m2ByGameObjectID.Value.ObjectModel.SoundsByAnimationType[AnimationType.Open].DBCID;
                    int closeSoundEntryID = 0;
                    if (m2ByGameObjectID.Value.ObjectModel.SoundsByAnimationType.ContainsKey(AnimationType.Close) == true)
                        closeSoundEntryID = m2ByGameObjectID.Value.ObjectModel.SoundsByAnimationType[AnimationType.Close].DBCID;
                    gameObjectDisplayInfoDBC.AddRow(m2ByGameObjectID.Key, relativeObjectFileName.ToLower(), m2ByGameObjectID.Value.ObjectModel.VisibilityBoundingBox, openSoundEntryID, closeSoundEntryID);
                }
                foreach (TransportLiftPathNode pathNode in TransportLiftPathNode.GetAllPathNodesSorted())
                {
                    // Only add nodes for zones that are loaded
                    if (mapIDsByShortName.ContainsKey(pathNode.ZoneShortName.ToLower()) == false)
                        continue;
                    transportAnimationDBC.AddRow(pathNode.GameObjectTemplateEntryID, pathNode.TimestampInMS, pathNode.XPositionOffset, pathNode.YPositionOffset, pathNode.ZPositionOffset, pathNode.AnimationSequenceID);
                }
                string soundDirectoryRelative = Path.Combine("Sound", "EQTransports");
                foreach (Sound sound in TransportLiftTrigger.AllSoundsBySoundName.Values)
                    soundEntriesDBC.AddRow(sound, sound.AudioFileNameNoExt + ".wav", soundDirectoryRelative);
            }

            // Tradeskills
            int curTradeskillTotemCategoryID = Configuration.TRADESKILL_TOTEM_CATEGORY_START;
            totemCategoryDBC.AddRow(Convert.ToUInt32(Configuration.TRADESKILL_TOTEM_CATEGORY_DBCID_ENGINEERING_TOOLBOX), "Toolbox", curTradeskillTotemCategoryID, 1);
            curTradeskillTotemCategoryID++;
            totemCategoryDBC.AddRow(Convert.ToUInt32(Configuration.TRADESKILL_TOTEM_CATEGORY_DBCID_TAILORING), "Sewing Kit", curTradeskillTotemCategoryID, 1);
            curTradeskillTotemCategoryID++;
            totemCategoryDBC.AddRow(Convert.ToUInt32(Configuration.TRADESKILL_TOTEM_CATEGORY_DBCID_JEWELCRAFTING), "Jeweler's Kit", curTradeskillTotemCategoryID, 1);
            curTradeskillTotemCategoryID++;
            totemCategoryDBC.AddRow(Convert.ToUInt32(Configuration.TRADESKILL_TOTEM_CATEGORY_DBCID_ALCHEMY), "Medicine Bag", curTradeskillTotemCategoryID, 1);
            curTradeskillTotemCategoryID++;
            totemCategoryDBC.AddRow(Convert.ToUInt32(Configuration.TRADESKILL_TOTEM_CATEGORY_DBCID_ENGINEERING_FLETCHING), "Fletching Kit", curTradeskillTotemCategoryID, 1);
            Dictionary<string, UInt32> tradeskillTotems = TradeskillRecipe.GetTotemIDsByItemName();
            int curTradeskillTotemCategoryMaskValue = 1;
            int curTradeskillTotemCategoryMaskCount = 0;
            foreach (var tradeskillTotemData in tradeskillTotems)
            {
                totemCategoryDBC.AddRow(tradeskillTotemData.Value, tradeskillTotemData.Key, curTradeskillTotemCategoryID, curTradeskillTotemCategoryMaskValue);
                curTradeskillTotemCategoryMaskValue *= 2;
                curTradeskillTotemCategoryMaskCount++;
                if (curTradeskillTotemCategoryMaskCount > 10)
                {
                    curTradeskillTotemCategoryMaskValue = 1;
                    curTradeskillTotemCategoryMaskCount = 0;
                    curTradeskillTotemCategoryID++;
                }
            }

            // Save the files
            areaTableDBC.SaveToDisk(dbcOutputClientFolder);
            areaTableDBC.SaveToDisk(dbcOutputServerFolder);
            areaTriggerDBC.SaveToDisk(dbcOutputClientFolder);
            areaTriggerDBC.SaveToDisk(dbcOutputServerFolder);
            if (Configuration.PLAYER_USE_EQ_START_ITEMS == true)
            {
                charStartOutfitDBC.SaveToDisk(dbcOutputClientFolder);
                charStartOutfitDBC.SaveToDisk(dbcOutputServerFolder);
            }
            creatureDisplayInfoDBC.SaveToDisk(dbcOutputClientFolder);
            creatureDisplayInfoDBC.SaveToDisk(dbcOutputServerFolder);
            creatureModelDataDBC.SaveToDisk(dbcOutputClientFolder);
            creatureModelDataDBC.SaveToDisk(dbcOutputServerFolder);
            creatureSoundDataDBC.SaveToDisk(dbcOutputClientFolder);
            creatureSoundDataDBC.SaveToDisk(dbcOutputServerFolder);
            factionDBC.SaveToDisk(dbcOutputClientFolder);
            factionDBC.SaveToDisk(dbcOutputServerFolder);
            factionTemplateDBC.SaveToDisk(dbcOutputClientFolder);
            factionTemplateDBC.SaveToDisk(dbcOutputServerFolder);
            footstepTerrainLookupDBC.SaveToDisk(dbcOutputClientFolder);
            footstepTerrainLookupDBC.SaveToDisk(dbcOutputServerFolder);
            gameObjectDisplayInfoDBC.SaveToDisk(dbcOutputClientFolder);
            gameObjectDisplayInfoDBC.SaveToDisk(dbcOutputServerFolder);
            itemDBC.SaveToDisk(dbcOutputClientFolder);
            itemDBC.SaveToDisk(dbcOutputServerFolder);
            itemDisplayInfoDBC.SaveToDisk(dbcOutputClientFolder);
            itemDisplayInfoDBC.SaveToDisk(dbcOutputServerFolder);
            lightDBC.SaveToDisk(dbcOutputClientFolder);
            lightDBC.SaveToDisk(dbcOutputServerFolder);
            lightFloatBandDBC.SaveToDisk(dbcOutputClientFolder);
            lightFloatBandDBC.SaveToDisk(dbcOutputServerFolder);
            lightIntBandDBC.SaveToDisk(dbcOutputClientFolder);
            lightIntBandDBC.SaveToDisk(dbcOutputServerFolder);
            lightParamsDBC.SaveToDisk(dbcOutputClientFolder);
            lightParamsDBC.SaveToDisk(dbcOutputServerFolder);
            liquidTypeDBC.SaveToDisk(dbcOutputClientFolder);
            liquidTypeDBC.SaveToDisk(dbcOutputServerFolder);
            loadingScreensDBC.SaveToDisk(dbcOutputClientFolder);
            loadingScreensDBC.SaveToDisk(dbcOutputServerFolder);
            mapDBCClient.SaveToDisk(dbcOutputClientFolder);
            mapDBCServer.SaveToDisk(dbcOutputServerFolder);
            mapDifficultyDBC.SaveToDisk(dbcOutputClientFolder);
            mapDifficultyDBC.SaveToDisk(dbcOutputServerFolder);
            //skillLineDBC.SaveToDisk(dbcOutputClientFolder);
            //skillLineDBC.SaveToDisk(dbcOutputServerFolder);
            skillLineAbilityDBC.SaveToDisk(dbcOutputClientFolder);
            skillLineAbilityDBC.SaveToDisk(dbcOutputServerFolder);
            soundAmbienceDBC.SaveToDisk(dbcOutputClientFolder);
            soundAmbienceDBC.SaveToDisk(dbcOutputServerFolder);
            soundEntriesDBC.SaveToDisk(dbcOutputClientFolder);
            soundEntriesDBC.SaveToDisk(dbcOutputServerFolder);
            spellDBC.SaveToDisk(dbcOutputClientFolder);
            spellDBC.SaveToDisk(dbcOutputServerFolder);
            spellCastTimesDBC.SaveToDisk(dbcOutputClientFolder);
            spellCastTimesDBC.SaveToDisk(dbcOutputServerFolder);
            spellDurationDBC.SaveToDisk(dbcOutputClientFolder);
            spellDurationDBC.SaveToDisk(dbcOutputServerFolder);
            spellIconDBC.SaveToDisk(dbcOutputClientFolder);
            spellIconDBC.SaveToDisk(dbcOutputServerFolder);
            spellRangeDBC.SaveToDisk(dbcOutputClientFolder);
            spellRangeDBC.SaveToDisk(dbcOutputServerFolder);
            taxiPathDBC.SaveToDisk(dbcOutputClientFolder);
            taxiPathDBC.SaveToDisk(dbcOutputServerFolder);
            taxiPathNodeDBC.SaveToDisk(dbcOutputClientFolder);
            taxiPathNodeDBC.SaveToDisk(dbcOutputServerFolder);
            totemCategoryDBC.SaveToDisk(dbcOutputClientFolder);
            totemCategoryDBC.SaveToDisk(dbcOutputServerFolder);
            transportAnimationDBC.SaveToDisk(dbcOutputClientFolder);
            transportAnimationDBC.SaveToDisk(dbcOutputServerFolder);
            worldSafeLocsDBC.SaveToDisk(dbcOutputClientFolder);
            worldSafeLocsDBC.SaveToDisk(dbcOutputServerFolder);            
            wmoAreaTableDBC.SaveToDisk(dbcOutputClientFolder);
            wmoAreaTableDBC.SaveToDisk(dbcOutputServerFolder);
            zoneMusicDBC.SaveToDisk(dbcOutputClientFolder);
            zoneMusicDBC.SaveToDisk(dbcOutputServerFolder);

            Logger.WriteDebug("Creating DBC Files complete");
        }

        public void CreateSQLScript(List<Zone> zones, List<CreatureTemplate> creatureTemplates, List<CreatureModelTemplate> creatureModelTemplates,
            List<CreatureSpawnPool> creatureSpawnPools, Dictionary<int, List<ItemLootTemplate>> itemLootTemplatesByCreatureTemplateID, List<QuestTemplate> questTemplates,
            List<TradeskillRecipe> tradeskillRecipes)
        {
            Logger.WriteInfo("Creating SQL Scripts...");

            // Clean the folder
            string sqlScriptFolder = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "SQLScripts");
            if (Directory.Exists(sqlScriptFolder))
                Directory.Delete(sqlScriptFolder, true);

            // Create the SQL Scripts
            // Characters
            ModEverquestCharacterHomebindSQL modEverquestCharacterHomebindSQL = new ModEverquestCharacterHomebindSQL();            
            // World
            AreaTriggerSQL areaTriggerSQL = new AreaTriggerSQL();
            AreaTriggerTeleportSQL areaTriggerTeleportSQL = new AreaTriggerTeleportSQL();
            BroadcastTextSQL broadcastTextSQL = new BroadcastTextSQL();
            CreatureSQL creatureSQL = new CreatureSQL();
            CreatureAddonSQL creatureAddonSQL = new CreatureAddonSQL();
            CreatureLootTemplateSQL creatureLootTableSQL = new CreatureLootTemplateSQL();
            CreatureModelInfoSQL creatureModelInfoSQL = new CreatureModelInfoSQL();
            CreatureQuestEnderSQL creatureQuestEnderSQL = new CreatureQuestEnderSQL();
            CreatureQuestStarterSQL creatureQuestStarterSQL = new CreatureQuestStarterSQL();
            CreatureTemplateSQL creatureTemplateSQL = new CreatureTemplateSQL();
            CreatureTemplateModelSQL creatureTemplateModelSQL = new CreatureTemplateModelSQL();
            CreatureTextSQL creatureTextSQL = new CreatureTextSQL();
            GameEventSQL gameEventSQL = new GameEventSQL();
            GameGraveyardSQL gameGraveyardSQL = new GameGraveyardSQL();
            GameTeleSQL gameTeleSQL = new GameTeleSQL();
            GameWeatherSQL gameWeatherSQL = new GameWeatherSQL();
            GossipMenuSQL gossipMenuSQL = new GossipMenuSQL();
            GossipMenuOptionSQL gossipMenuOptionSQL = new GossipMenuOptionSQL();
            GraveyardZoneSQL graveyardZoneSQL = new GraveyardZoneSQL();
            GameObjectSQL gameObjectSQL = new GameObjectSQL();
            GameObjectTemplateSQL gameObjectTemplateSQL = new GameObjectTemplateSQL();
            GameObjectTemplateAddonSQL gameObjectTemplateAddonSQL = new GameObjectTemplateAddonSQL();
            InstanceTemplateSQL instanceTemplateSQL = new InstanceTemplateSQL();
            ItemLootTemplateSQL itemLootTemplateSQL = new ItemLootTemplateSQL();
            ItemTemplateSQL itemTemplateSQL = new ItemTemplateSQL();
            ModEverquestCreatureOnkillReputationSQL modEverquestCreatureOnkillReputationSQL = new ModEverquestCreatureOnkillReputationSQL();
            ModEverquestQuestCompleteReputationSQL modEverquestQuestCompleteReputationSQL = new ModEverquestQuestCompleteReputationSQL();
            NPCTextSQL npcTextSQL = new NPCTextSQL();
            NPCTrainerSQL npcTrainerSQL = new NPCTrainerSQL();            
            NPCVendorSQL npcVendorSQL = new NPCVendorSQL();
            PlayerCreateInfoSQL playerCreateInfoSQL = new PlayerCreateInfoSQL();
            PoolCreatureSQL poolCreatureSQL = new PoolCreatureSQL();
            PoolPoolSQL poolPoolSQL = new PoolPoolSQL();
            PoolTemplateSQL poolTemplateSQL = new PoolTemplateSQL();
            QuestTemplateSQL questTemplateSQL = new QuestTemplateSQL();
            QuestTemplateAddonSQL questTemplateAddonSQL = new QuestTemplateAddonSQL();
            SmartScriptsSQL smartScriptsSQL = new SmartScriptsSQL();
            TransportsSQL transportsSQL = new TransportsSQL();
            WaypointDataSQL waypointDataSQL = new WaypointDataSQL();

            // Zones
            Dictionary<string, ZoneProperties> zonePropertiesByShortName = ZoneProperties.GetZonePropertyListByShortName();
            foreach (Zone zone in zones)
            {
                // Instance list
                instanceTemplateSQL.AddRow(Convert.ToInt32(zone.ZoneProperties.DBCMapID));

                // Teleport scripts to safe positions (add a record for both descriptive and short name if they are different)
                gameTeleSQL.AddRow(Convert.ToInt32(zone.ZoneProperties.DBCMapID), zone.DescriptiveNameOnlyLetters, zone.SafePosition.Y, zone.SafePosition.Y, zone.SafePosition.Z);
                if (zone.DescriptiveNameOnlyLetters.ToLower() != zone.ShortName.ToLower())
                    gameTeleSQL.AddRow(Convert.ToInt32(zone.ZoneProperties.DBCMapID), zone.ShortName, zone.SafePosition.Y, zone.SafePosition.Y, zone.SafePosition.Z);

                // Weather data
                if (Configuration.ZONE_WEATHER_ENABLED == true)
                    gameWeatherSQL.AddRow(Convert.ToInt32(zone.ZoneProperties.DefaultZoneArea.DBCAreaTableID), zone.ZoneProperties.RainChanceWinter, zone.ZoneProperties.SnowChanceWinter,
                        zone.ZoneProperties.RainChanceSpring, zone.ZoneProperties.SnowChanceSpring, zone.ZoneProperties.RainChanceSummer, zone.ZoneProperties.SnowChanceSummer,
                        zone.ZoneProperties.RainChanceFall, zone.ZoneProperties.RainChanceWinter);

                // Zone lines
                foreach (ZonePropertiesZoneLineBox zoneLine in ZoneProperties.GetZonePropertiesForZone(zone.ShortName).ZoneLineBoxes)
                {
                    // Skip invalid zone lines
                    if (zonePropertiesByShortName.ContainsKey(zoneLine.TargetZoneShortName.ToLower()) == false)
                        continue;

                    // Area Trigger
                    areaTriggerSQL.AddRow(zoneLine.AreaTriggerID, zone.ZoneProperties.DBCMapID, zoneLine.BoxPosition.X, zoneLine.BoxPosition.Y,
                        zoneLine.BoxPosition.Z, zoneLine.BoxLength, zoneLine.BoxWidth, zoneLine.BoxHeight, zoneLine.BoxOrientation);

                    // Area Trigger Teleport
                    int areaTriggerID = zoneLine.AreaTriggerID;
                    string descriptiveName = "EQ " + zone.ShortName + " - " + zoneLine.TargetZoneShortName + " zone line";
                    int targetMapId = ZoneProperties.GetZonePropertiesForZone(zoneLine.TargetZoneShortName).DBCMapID;
                    float targetPositionX = zoneLine.TargetZonePosition.X;
                    float targetPositionY = zoneLine.TargetZonePosition.Y;
                    float targetPositionZ = zoneLine.TargetZonePosition.Z;
                    float targetOrientation = zoneLine.TargetZoneOrientation;
                    areaTriggerTeleportSQL.AddRow(areaTriggerID, descriptiveName, targetMapId, targetPositionX, targetPositionY, targetPositionZ, targetOrientation);
                }
            }
            Dictionary<string, int> mapIDsByShortName = new Dictionary<string, int>();
            foreach (Zone zone in zones)
                mapIDsByShortName.Add(zone.ShortName.ToLower().Trim(), zone.ZoneProperties.DBCMapID);

            // Pre-generate class trainer menus
            Dictionary<ClassType, int> classTrainerMenuIDs = new Dictionary<ClassType, int>();
            foreach (ClassType classType in Enum.GetValues(typeof(ClassType)))
            {
                if (classType == ClassType.All || classType == ClassType.None)
                    continue;

                // Base menu
                int gossipMenuID = GossipMenuSQL.GenerateUniqueMenuID();
                classTrainerMenuIDs.Add(classType, gossipMenuID);
                gossipMenuSQL.AddRow(gossipMenuID, Configuration.CREATURE_CLASS_TRAINER_NPC_TEXT_ID);

                // Menu options
                gossipMenuOptionSQL.AddRowForClassTrainer(gossipMenuID, 0, 3, "I would like to train.", 
                    Configuration.CREATURE_CLASS_TRAINER_TRAIN_BROADCAST_TEXT_ID, 5, 16, 0);
                gossipMenuOptionSQL.AddRowForClassTrainer(gossipMenuID, 1, 0, "I wish to unlearn my talents", 
                    Configuration.CREATURE_CLASS_TRAINER_UNLEARN_BROADCAST_TEXT_ID, 16, 16, Configuration.CREATURE_CLASS_TRAINER_UNLEARN_MENU_ID);
                gossipMenuOptionSQL.AddRowForClassTrainer(gossipMenuID, 2, 0, "I wish to know about Dual Talent Specialization.", 
                    Configuration.CREATURE_CLASS_TRAINER_DUALTALENT_BROADCAST_TEXT_ID, 20, 1, Configuration.CREATURE_CLASS_TRAINER_DUALTALENT_MENU_ID);
            }

            // Creature Templates
            Dictionary<int, List<CreatureVendorItem>> vendorItems = CreatureVendorItem.GetCreatureVendorItemsByMerchantIDs();
            foreach (CreatureTemplate creatureTemplate in creatureTemplates)
            {
                if (creatureTemplate.ModelTemplate == null)
                    Logger.WriteError("Error generating azeroth core scripts since model template was null for creature template '" + creatureTemplate.Name + "'");
                else
                {
                    // Calculate the scale
                    float scale = creatureTemplate.Size * creatureTemplate.Race.SpawnSizeMod;

                    // Class Trainers
                    if (creatureTemplate.ClassTrainerType != ClassType.All && creatureTemplate.ClassTrainerType != ClassType.None)
                    {
                        // Trainers need a line in the npc trainers table
                        npcTrainerSQL.AddRowForTrainerReference(SpellTrainerAbility.GetTrainerSpellsReferenceLineIDForWOWClassTrainer(creatureTemplate.ClassTrainerType), creatureTemplate.WOWCreatureTemplateID);

                        // Associate the menu
                        creatureTemplate.GossipMenuID = classTrainerMenuIDs[creatureTemplate.ClassTrainerType];
                    }

                    // Profession Trainers
                    if (creatureTemplate.TradeskillTrainerType != TradeskillType.None && creatureTemplate.TradeskillTrainerType != TradeskillType.Unknown)
                    {
                        // Trainers need a line in the npc trainers table
                        npcTrainerSQL.AddRowForTrainerReference(SpellTrainerAbility.GetTrainerSpellsReferenceLineIDForWOWTradeskillTrainer(creatureTemplate.TradeskillTrainerType), creatureTemplate.WOWCreatureTemplateID);
                    }

                    // Riding trainers
                    if (Configuration.CREATURE_RIDING_TRAINERS_ENABLED == true && creatureTemplate.IsRidingTrainer == true)
                    {
                        npcTrainerSQL.AddRowForTrainerReference(202010, creatureTemplate.WOWCreatureTemplateID); // Same as Binjy Featherwhistle
                        if (Configuration.CREATURE_RIDING_TRAINERS_ALSO_TEACH_FLY == true)
                            npcTrainerSQL.AddRowForTrainerReference(202011, creatureTemplate.WOWCreatureTemplateID); // Same as Hargen Bronzewing
                    }

                    // Determine the display id
                    int displayID = creatureTemplate.ModelTemplate.DBCCreatureDisplayID;
                    if (creatureTemplate.IsNonNPC == true)
                        displayID = 11686; // Dranei totem

                    // Create the records
                    creatureTemplateSQL.AddRow(creatureTemplate, scale);
                    creatureTemplateModelSQL.AddRow(creatureTemplate.WOWCreatureTemplateID, displayID, scale);
                    
                    // If it's a vendor, add the vendor records too
                    if (creatureTemplate.MerchantID != 0 && vendorItems.ContainsKey(creatureTemplate.MerchantID))
                    {
                        foreach(CreatureVendorItem vendorItem in vendorItems[creatureTemplate.MerchantID])
                        {
                            if (vendorItem.WOWItemID != -1)
                                npcVendorSQL.AddRow(creatureTemplate.WOWCreatureTemplateID, vendorItem.WOWItemID, vendorItem.Slot);
                            else
                            {
                                if (ItemTemplate.GetItemTemplatesByEQDBIDs().ContainsKey(vendorItem.EQItemID) == false)
                                {
                                    Logger.WriteError("Attempted to add a merchant item with EQItemID '" + vendorItem.EQItemID + "' to merchant '" + creatureTemplate.MerchantID + "', but the EQItemID did not exist");
                                    continue;
                                }

                                npcVendorSQL.AddRow(creatureTemplate.WOWCreatureTemplateID, ItemTemplate.GetItemTemplatesByEQDBIDs()[vendorItem.EQItemID].WOWEntryID, vendorItem.Slot);
                            }
                        }
                    }                      

                    // Kill rewards
                    foreach (CreatureFactionKillReward creatureFactionKillReward in creatureTemplate.CreatureFactionKillRewards)
                        modEverquestCreatureOnkillReputationSQL.AddRow(creatureTemplate.WOWCreatureTemplateID, creatureFactionKillReward);
                }
            }

            // Creature models
            foreach (CreatureModelTemplate creatureModelTemplate in creatureModelTemplates)
                creatureModelInfoSQL.AddRow(creatureModelTemplate.DBCCreatureDisplayID, Convert.ToInt32(creatureModelTemplate.GenderType));

            // Creature and Spawn Pools
            foreach(CreatureSpawnPool spawnPool in creatureSpawnPools)
            {
                // For single element single location pools, just create a creature record
                if (spawnPool.CreatureSpawnInstances.Count == 1 && spawnPool.CreatureTemplates.Count == 1)
                {
                    CreatureTemplate creatureTemplate = spawnPool.CreatureTemplates[0];
                    CreatureSpawnInstance spawnInstance = spawnPool.CreatureSpawnInstances[0];
                    int creatureGUID = CreatureTemplate.GenerateCreatureSQLGUID();
                    List<CreaturePathGridEntry> pathGridEntries = spawnInstance.GetPathGridEntries();
                    string comment = string.Concat(creatureTemplate.Name, " - EQ Group: ", spawnPool.CreatureSpawnGroup.ID, ", EQ NPC ID: ", creatureTemplate.EQCreatureTemplateID, ", EQ Instance ID: ", spawnInstance.ID);
                    if (pathGridEntries.Count > 0)
                    {
                        int waypointGUID = creatureGUID * 1000;
                        creatureAddonSQL.AddRow(creatureGUID, waypointGUID, creatureTemplate.DefaultEmoteID);
                        foreach (CreaturePathGridEntry pathGridEntry in pathGridEntries)
                            waypointDataSQL.AddRow(waypointGUID, pathGridEntry.Number, pathGridEntry.NodeX, pathGridEntry.NodeY, pathGridEntry.NodeZ, pathGridEntry.PauseInSec * 1000);
                        creatureSQL.AddRow(creatureGUID, creatureTemplate.WOWCreatureTemplateID, spawnInstance.MapID, spawnInstance.AreaID, spawnInstance.AreaID,
                            spawnInstance.SpawnXPosition, spawnInstance.SpawnYPosition, spawnInstance.SpawnZPosition, spawnInstance.Orientation, CreatureMovementType.Path,
                            spawnPool.CreatureSpawnGroup.RoamDistance, spawnInstance.SpawnDay, spawnInstance.SpawnNight, comment);
                    }
                    else
                    {
                        creatureAddonSQL.AddRow(creatureGUID, 0, creatureTemplate.DefaultEmoteID);
                        CreatureMovementType movementType = CreatureMovementType.None;
                        if (spawnPool.CreatureSpawnGroup.RoamDistance > 1)
                            movementType = CreatureMovementType.Random;
                        creatureSQL.AddRow(creatureGUID, creatureTemplate.WOWCreatureTemplateID, spawnInstance.MapID, spawnInstance.AreaID, spawnInstance.AreaID,
                            spawnInstance.SpawnXPosition, spawnInstance.SpawnYPosition, spawnInstance.SpawnZPosition, spawnInstance.Orientation, movementType,
                            spawnPool.CreatureSpawnGroup.RoamDistance, spawnInstance.SpawnDay, spawnInstance.SpawnNight, comment);
                    }
                }

                // Create a pool pools if there are multiple locations
                else if (spawnPool.CreatureSpawnInstances.Count > 1)
                {
                    // Create the mother pool template
                    int motherPoolTemplateID = CreatureSpawnPool.GetPoolTemplateSQLID();
                    List<string> motherPoolNames = new List<string>();
                    foreach(CreatureTemplate template in spawnPool.CreatureTemplates)
                        if (motherPoolNames.Contains(template.Name) == false)
                            motherPoolNames.Add(template.Name);
                    string motherPoolDescription = "(" + spawnPool.CreatureSpawnGroup.ID + ")";
                    foreach (string name in motherPoolNames)
                        motherPoolDescription += ", " + name;
                    poolTemplateSQL.AddRow(motherPoolTemplateID, motherPoolDescription + " - Master Pool", spawnPool.GetMaxSpawnCount());

                    // Create by instance groups
                    for (int spawnInstanceIndex = 0; spawnInstanceIndex < spawnPool.CreatureSpawnInstances.Count; spawnInstanceIndex++)
                    {
                        CreatureSpawnInstance spawnInstance = spawnPool.CreatureSpawnInstances[spawnInstanceIndex];

                        // Create the pool pool
                        int poolPoolTemplateID = CreatureSpawnPool.GetPoolTemplateSQLID();
                        string poolPoolDescription = motherPoolDescription + " - " + spawnInstanceIndex.ToString();
                        poolTemplateSQL.AddRow(poolPoolTemplateID, poolPoolDescription, 1);
                        poolPoolSQL.AddRow(poolPoolTemplateID, motherPoolTemplateID, 0, poolPoolDescription);

                        // Create the creature records
                        for (int creatureTemplateIndex = 0; creatureTemplateIndex < spawnPool.CreatureTemplates.Count; creatureTemplateIndex++)
                        {
                            CreatureTemplate creatureTemplate = spawnPool.CreatureTemplates[creatureTemplateIndex];
                            int chance = spawnPool.CreatureTemplateChances[creatureTemplateIndex];
                            int creatureGUID = CreatureTemplate.GenerateCreatureSQLGUID();
                            poolCreatureSQL.AddRow(creatureGUID, poolPoolTemplateID, chance, creatureTemplate.Name);
                            List<CreaturePathGridEntry> pathGridEntries = spawnInstance.GetPathGridEntries();
                            string comment = string.Concat(creatureTemplate.Name, " - EQ Group: ", spawnPool.CreatureSpawnGroup.ID, ", EQ NPC ID: ", creatureTemplate.EQCreatureTemplateID, ", EQ Instance ID: ", spawnInstance.ID);
                            if (pathGridEntries.Count > 0)
                            {
                                int waypointGUID = creatureGUID * 1000;
                                creatureAddonSQL.AddRow(creatureGUID, waypointGUID, creatureTemplate.DefaultEmoteID);
                                foreach (CreaturePathGridEntry pathGridEntry in pathGridEntries)
                                    waypointDataSQL.AddRow(waypointGUID, pathGridEntry.Number, pathGridEntry.NodeX, pathGridEntry.NodeY, pathGridEntry.NodeZ, pathGridEntry.PauseInSec * 1000);
                                creatureSQL.AddRow(creatureGUID, creatureTemplate.WOWCreatureTemplateID, spawnInstance.MapID, spawnInstance.AreaID, spawnInstance.AreaID,
                                    spawnInstance.SpawnXPosition, spawnInstance.SpawnYPosition, spawnInstance.SpawnZPosition, spawnInstance.Orientation, CreatureMovementType.Path,
                                    spawnPool.CreatureSpawnGroup.RoamDistance, spawnInstance.SpawnDay, spawnInstance.SpawnNight, comment);
                            }
                            else
                            {
                                creatureAddonSQL.AddRow(creatureGUID, 0, creatureTemplate.DefaultEmoteID);
                                CreatureMovementType movementType = CreatureMovementType.None;
                                if (spawnPool.CreatureSpawnGroup.RoamDistance > 1)
                                    movementType = CreatureMovementType.Random;
                                creatureSQL.AddRow(creatureGUID, creatureTemplate.WOWCreatureTemplateID, spawnInstance.MapID, spawnInstance.AreaID, spawnInstance.AreaID,
                                    spawnInstance.SpawnXPosition, spawnInstance.SpawnYPosition, spawnInstance.SpawnZPosition, spawnInstance.Orientation, movementType,
                                    spawnPool.CreatureSpawnGroup.RoamDistance, spawnInstance.SpawnDay, spawnInstance.SpawnNight, comment);
                            }
                        }
                    }
                }

                // No mother pool needed
                else if (spawnPool.CreatureSpawnInstances.Count == 1 && spawnPool.CreatureTemplates.Count > 0)
                {
                    CreatureSpawnInstance spawnInstance = spawnPool.CreatureSpawnInstances[0];

                    // Make the pool description
                    List<string> poolNames = new List<string>();
                    foreach (CreatureTemplate template in spawnPool.CreatureTemplates)
                        if (poolNames.Contains(template.Name) == false)
                            poolNames.Add(template.Name);
                    string poolDescription = "(" + spawnPool.CreatureSpawnGroup.ID + ")";
                    foreach (string name in poolNames)
                        poolDescription += ", " + name;
                    
                    // Create the pool template
                    int poolTemplateID = CreatureSpawnPool.GetPoolTemplateSQLID();
                    poolTemplateSQL.AddRow(poolTemplateID, poolDescription, spawnPool.GetMaxSpawnCount());

                    // Create the creature records
                    for (int creatureTemplateIndex = 0; creatureTemplateIndex < spawnPool.CreatureTemplates.Count; creatureTemplateIndex++)
                    {
                        CreatureTemplate creatureTemplate = spawnPool.CreatureTemplates[creatureTemplateIndex];
                        int chance = spawnPool.CreatureTemplateChances[creatureTemplateIndex];
                        int creatureGUID = CreatureTemplate.GenerateCreatureSQLGUID();
                        poolCreatureSQL.AddRow(creatureGUID, poolTemplateID, chance, creatureTemplate.Name);
                        List<CreaturePathGridEntry> pathGridEntries = spawnInstance.GetPathGridEntries();
                        string comment = string.Concat(creatureTemplate.Name, " - EQ Group: ", spawnPool.CreatureSpawnGroup.ID, ", EQ NPC ID: ", creatureTemplate.EQCreatureTemplateID, ", EQ Instance ID: ", spawnInstance.ID);
                        if (pathGridEntries.Count > 0)
                        {
                            int waypointGUID = creatureGUID * 1000;
                            creatureAddonSQL.AddRow(creatureGUID, waypointGUID, creatureTemplate.DefaultEmoteID);
                            foreach (CreaturePathGridEntry pathGridEntry in pathGridEntries)
                                waypointDataSQL.AddRow(waypointGUID, pathGridEntry.Number, pathGridEntry.NodeX, pathGridEntry.NodeY, pathGridEntry.NodeZ, pathGridEntry.PauseInSec * 1000);
                            creatureSQL.AddRow(creatureGUID, creatureTemplate.WOWCreatureTemplateID, spawnInstance.MapID, spawnInstance.AreaID, spawnInstance.AreaID,
                                spawnInstance.SpawnXPosition, spawnInstance.SpawnYPosition, spawnInstance.SpawnZPosition, spawnInstance.Orientation, CreatureMovementType.Path, 
                                spawnPool.CreatureSpawnGroup.RoamDistance, spawnInstance.SpawnDay, spawnInstance.SpawnNight, comment);
                        }
                        else
                        {
                            creatureAddonSQL.AddRow(creatureGUID, 0, creatureTemplate.DefaultEmoteID);
                            CreatureMovementType movementType = CreatureMovementType.None;
                            if (spawnPool.CreatureSpawnGroup.RoamDistance > 1)
                                movementType = CreatureMovementType.Random;
                            creatureSQL.AddRow(creatureGUID, creatureTemplate.WOWCreatureTemplateID, spawnInstance.MapID, spawnInstance.AreaID, spawnInstance.AreaID,
                                spawnInstance.SpawnXPosition, spawnInstance.SpawnYPosition, spawnInstance.SpawnZPosition, spawnInstance.Orientation, movementType,
                                spawnPool.CreatureSpawnGroup.RoamDistance, spawnInstance.SpawnDay, spawnInstance.SpawnNight, comment);
                        }
                    }
                }
            }

            // Game Events
            DateTime eventEnd = new DateTime(2025, 12, 30, 23, 0, 0);
            DateTime dayStart = new DateTime(2012, 10, 29, 6, 0, 0);
            gameEventSQL.AddRow(Configuration.SQL_GAMEEVENT_ID_DAY, dayStart, eventEnd, 1440, 840, "EQ Day");
            DateTime nightStart = new DateTime(2016, 10, 28, 20, 0, 0);
            gameEventSQL.AddRow(Configuration.SQL_GAMEEVENT_ID_NIGHT, nightStart, eventEnd, 1440, 600, "EQ Night");

            // Graveyards
            foreach (ZonePropertiesGraveyard graveyard in ZonePropertiesGraveyard.GetAllGraveyards())
            {
                // Should be one for each ghost zone
                foreach (string zoneShortName in graveyard.GhostZoneShortNames)
                {
                    string zoneShortNameLower = zoneShortName.ToLower();
                    if (zonePropertiesByShortName.ContainsKey(zoneShortNameLower) == true)
                    {
                        int ghostZoneAreaID = Convert.ToInt32(ZoneProperties.GetZonePropertiesForZone(zoneShortNameLower).DefaultZoneArea.DBCAreaTableID);
                        graveyardZoneSQL.AddRow(graveyard, ghostZoneAreaID);
                    }   
                }

                if (zonePropertiesByShortName.ContainsKey(graveyard.LocationShortName) == true)
                {
                    ZoneProperties curZoneProperties = ZoneProperties.GetZonePropertiesForZone(graveyard.LocationShortName);
                    int mapID = curZoneProperties.DBCMapID;
                    gameGraveyardSQL.AddRow(graveyard, mapID);

                    // And there should be one spirit healer per graveyard
                    int spiritHealerGUID = CreatureTemplate.GenerateCreatureSQLGUID();
                    int zoneAreaID = Convert.ToInt32(curZoneProperties.DefaultZoneArea.DBCAreaTableID);
                    creatureSQL.AddRow(spiritHealerGUID, Configuration.ZONE_GRAVEYARD_SPIRIT_HEALER_CREATURETEMPLATE_ID, mapID, zoneAreaID, zoneAreaID,
                        graveyard.SpiritHealerX, graveyard.SpiritHealerY, graveyard.SpiritHealerZ, graveyard.SpiritHealerOrientation, CreatureMovementType.None, 0, true, true, string.Empty);
                }
            }

            // Game Objects
            if (Configuration.GENERATE_OBJECTS == true)
            {
                Dictionary<string, List<GameObject>> gameObjectsByZoneShortNames = GameObject.GetNonDoodadGameObjectsByZoneShortNames();
                foreach(var gameObjectByShortName in gameObjectsByZoneShortNames)
                {
                    // Skip invalid objects (zones not loaded)
                    if (zonePropertiesByShortName.ContainsKey(gameObjectByShortName.Key) == false)
                        continue;
                    int areaID = 0;
                    foreach (Zone zone in zones)
                        if (zone.ShortName.ToLower().Trim() == gameObjectByShortName.Key)
                            areaID = Convert.ToInt32(zone.DefaultArea.DBCAreaTableID);

                    foreach (GameObject gameObject in gameObjectByShortName.Value)
                    {
                        string comment = string.Concat("EQ ", gameObject.ObjectType.ToString(), " ", gameObject.ZoneShortName, " (", gameObject.ID, ")");
                        string name = gameObject.DisplayName;
                        if (name.Length == 0)
                            name = comment;
                        int mapID = mapIDsByShortName[gameObjectByShortName.Key];
                        gameObjectSQL.AddRow(gameObject.GameObjectGUID, gameObject.GameObjectTemplateEntryID, mapID, areaID, gameObject.Position, gameObject.Orientation, comment);
                        gameObjectTemplateSQL.AddRowForGameObject(name, gameObject);
                        gameObjectTemplateAddonSQL.AddRowNoDespawn(gameObject.GameObjectTemplateEntryID);

                        // Attach any smart scripts
                        if (gameObject.TriggerGameObjectGUID != 0)
                        {
                            string scriptComment = string.Concat("EQ GameObject GUID ", gameObject.GameObjectGUID, " Chain Activates GUID ", gameObject.TriggerGameObjectGUID);
                            smartScriptsSQL.AddRowForGameObjectStateTriggerEvent(gameObject.GameObjectTemplateEntryID, gameObject.TriggerGameObjectGUID, gameObject.TriggerGameObjectTemplateEntryID, scriptComment);
                        }
                    }
                }
            }

            // Items
            foreach (ItemTemplate itemTemplate in ItemTemplate.GetItemTemplatesByEQDBIDs().Values)
            {
                if (itemTemplate.IsExistingItemAlready == true)
                    continue;
                itemTemplateSQL.AddRow(itemTemplate);
            }
            foreach (var itemLootTemplateByCreatureTemplateID in itemLootTemplatesByCreatureTemplateID.Values)
                foreach (ItemLootTemplate itemLootTemplate in itemLootTemplateByCreatureTemplateID)
                    creatureLootTableSQL.AddRow(itemLootTemplate);

            // Player start properties
            if (Configuration.PLAYER_USE_EQ_START_LOCATION == true && zonePropertiesByShortName.Count > 0)
            {
                // Restrict to loaded zones
                Dictionary<string, int> areaIDsByShortName = new Dictionary<string, int>();
                foreach (Zone zone in zones)
                {
                    areaIDsByShortName.Add(zone.ShortName.ToLower().Trim(), Convert.ToInt32(zone.DefaultArea.DBCAreaTableID));
                }
                foreach (var classRaceProperties in PlayerClassRaceProperties.GetClassRacePropertiesByRaceAndClassID())
                {
                    string startZoneShortName = classRaceProperties.Value.StartZoneShortName;
                    if (mapIDsByShortName.ContainsKey(classRaceProperties.Value.StartZoneShortName) == false)
                    {
                        Logger.WriteDebug(string.Concat("Could not map player location for zone short name '", classRaceProperties.Value.StartZoneShortName, "' since no zone was loaded with that shortname.  Using a fallback"));
                        startZoneShortName = mapIDsByShortName.First().Key;
                    }

                    playerCreateInfoSQL.AddRow(classRaceProperties.Key.Item1, classRaceProperties.Key.Item2, mapIDsByShortName[startZoneShortName],
                        areaIDsByShortName[startZoneShortName], classRaceProperties.Value.StartPositionX, classRaceProperties.Value.StartPositionY,
                        classRaceProperties.Value.StartPositionZ, classRaceProperties.Value.StartOrientation);
                }
            }

            // Quests
            Dictionary<int, int> creatureTextGroupIDsByCreatureTemplateID = new Dictionary<int, int>();
            SortedDictionary<int, ItemTemplate> itemTemplatesByWOWEntryID = ItemTemplate.GetItemTemplatesByWOWEntryID();
            foreach (QuestTemplate questTemplate in questTemplates)
            {
                // Skip quests where items needed to complete it are not available
                if (questTemplate.AreRequiredItemsPlayerObtainable(itemTemplatesByWOWEntryID) == false)
                    continue;

                string firstQuestName = questTemplate.Name;
                int firstQuestID = questTemplate.QuestIDWOW;
                string repeatQuestName = string.Concat(questTemplate.Name, " (repeat)");
                int repeatQuestID = firstQuestID + Configuration.SQL_QUEST_TEMPLATE_ID_REPEATABLE_SHIFT;

                questTemplateSQL.AddRow(questTemplate, firstQuestID, firstQuestName);              
                questTemplateSQL.AddRow(questTemplate, repeatQuestID, repeatQuestName);
                questTemplateAddonSQL.AddRow(questTemplate, firstQuestID, 0, false);
                questTemplateAddonSQL.AddRow(questTemplate, repeatQuestID, firstQuestID, true);

                // Quest NPCs
                Dictionary<int, CreatureTemplate> creatureTemplateByWOWID = CreatureTemplate.GetCreatureTemplateListByWOWID();
                foreach (int creatureTemplateID in questTemplate.QuestgiverWOWCreatureTemplateIDs)
                {
                    creatureQuestStarterSQL.AddRow(firstQuestID, creatureTemplateID);
                    creatureQuestStarterSQL.AddRow(repeatQuestID, creatureTemplateID);
                    creatureQuestEnderSQL.AddRow(firstQuestID, creatureTemplateID);
                    creatureQuestEnderSQL.AddRow(repeatQuestID, creatureTemplateID);

                    // Reward say/yell/emote actions
                    foreach (QuestReaction reaction in questTemplate.Reactions)
                    {
                        if (reaction.type == QuestReactionType.Emote || reaction.type == QuestReactionType.Say || reaction.type == QuestReactionType.Yell)
                        {
                            // Broadcast Text
                            int broadcastID = BroadcastTextSQL.GenerateUniqueID();
                            broadcastTextSQL.AddRow(broadcastID, reaction.ReactionValue, reaction.ReactionValue);

                            // Creature Text
                            int creatureTextGroupID = 0;
                            if (creatureTextGroupIDsByCreatureTemplateID.ContainsKey(creatureTemplateID) == true)
                            {
                                creatureTextGroupIDsByCreatureTemplateID[creatureTemplateID]+=2;
                                creatureTextGroupID = creatureTextGroupIDsByCreatureTemplateID[creatureTemplateID];
                            }
                            else
                                creatureTextGroupIDsByCreatureTemplateID.Add(creatureTemplateID, 0);
                            string comment = string.Concat("EQ ", creatureTemplateByWOWID[creatureTemplateID].Name, " Quest ", reaction.type.ToString());
                            int messageType = 12; // Default to say
                            switch (reaction.type)
                            {
                                case QuestReactionType.Say: messageType = 12; break;
                                case QuestReactionType.Emote: messageType = 16; break;
                                case QuestReactionType.Yell: messageType = 14; break;
                                default: Logger.WriteError("Unhandled quest reaction type of " + reaction.type); break;
                            }
                            creatureTextSQL.AddRow(creatureTemplateID, creatureTextGroupID, messageType, reaction.ReactionValue, broadcastID, Configuration.QUESTS_TEXT_DURATION_IN_MS, comment);
                            creatureTextSQL.AddRow(creatureTemplateID, creatureTextGroupID + 1, messageType, reaction.ReactionValue, broadcastID, Configuration.QUESTS_TEXT_DURATION_IN_MS, comment);

                            // Smart Script
                            smartScriptsSQL.AddRowForQuestCompleteTalkEvent(creatureTemplateID, creatureTextGroupID, firstQuestID, comment);
                            smartScriptsSQL.AddRowForQuestCompleteTalkEvent(creatureTemplateID, creatureTextGroupID + 1, repeatQuestID, comment);
                        }
                    }
                }

                // Reputation rewards
                foreach (QuestCompletionFactionReward completionReputation in questTemplate.questCompletionFactionRewards)
                {
                    modEverquestQuestCompleteReputationSQL.AddRow(firstQuestID, completionReputation);
                    modEverquestQuestCompleteReputationSQL.AddRow(repeatQuestID, completionReputation);
                }

                // Loot templates for containers
                if (questTemplate.RandomAwardContainerItemTemplate != null)
                {
                    for (int i = 0; i < questTemplate.RandomAwardContainerItemTemplate.ContainedWOWItemTemplateIDs.Count; i++)
                    {
                        int curWOWItemTemplateID = questTemplate.RandomAwardContainerItemTemplate.ContainedWOWItemTemplateIDs[i];
                        float curItemChance = questTemplate.RandomAwardContainerItemTemplate.ContainedItemChances[i];
                        string comment = string.Concat(questTemplate.RandomAwardContainerItemTemplate.Name, " - ", itemTemplatesByWOWEntryID[curWOWItemTemplateID].Name);
                        itemLootTemplateSQL.AddRow(questTemplate.RandomAwardContainerItemTemplate.WOWEntryID, curWOWItemTemplateID, 1, curItemChance, 1, comment);
                    }
                }
            }

            // Trainer Abilities - Class
            foreach (ClassType classType in Enum.GetValues(typeof(ClassType)))
            {
                if (classType == ClassType.All || classType == ClassType.None)
                    continue;

                int lineID = SpellTrainerAbility.GetTrainerSpellsReferenceLineIDForWOWClassTrainer(classType);
                foreach (SpellTrainerAbility trainerAbility in SpellTrainerAbility.GetTrainerSpellsForClass(classType))
                    npcTrainerSQL.AddRowForTrainerAbility(lineID, trainerAbility);
            }

            // Trainer Abilities - Tradeskills
            foreach (TradeskillType tradeskillType in Enum.GetValues(typeof(TradeskillType)))
            {
                if (tradeskillType == TradeskillType.Unknown || tradeskillType == TradeskillType.None)
                    continue;

                int lineID = SpellTrainerAbility.GetTrainerSpellsReferenceLineIDForWOWTradeskillTrainer(tradeskillType);
                npcTrainerSQL.AddDevelopmentSkillsForTradeskill(lineID, tradeskillType);
                foreach (SpellTrainerAbility trainerAbility in SpellTrainerAbility.GetTrainerSpellsForTradeskill(tradeskillType))
                    npcTrainerSQL.AddRowForTrainerAbility(lineID, trainerAbility);
            }

            // Tradeskills
            foreach (TradeskillRecipe recipe in tradeskillRecipes)
            {
                // Multi-item containers
                if (recipe.ProducedFilledContainer != null)
                {
                    for (int i = 0; i < recipe.ProducedFilledContainer.ContainedWOWItemTemplateIDs.Count; i++)
                    {
                        int curWOWItemTemplateID = recipe.ProducedFilledContainer.ContainedWOWItemTemplateIDs[i];
                        int curItemCount = recipe.ProducedFilledContainer.ContainedtemCounts[i];
                        string comment = string.Concat(recipe.ProducedFilledContainer.Name, " - ", itemTemplatesByWOWEntryID[curWOWItemTemplateID].Name);
                        itemLootTemplateSQL.AddRow(recipe.ProducedFilledContainer.WOWEntryID, curWOWItemTemplateID, i + 1, 100, curItemCount, comment);
                    }
                }
            }

            // Transports
            if (Configuration.GENERATE_TRANSPORTS == true)
            {
                foreach (TransportShip transportShip in TransportShip.GetAllTransportShips())
                {
                    // Only add this transport ship if the full path is zones that are loaded
                    bool zonesAreLoaded = true;
                    foreach (string touchedZone in transportShip.GetTouchedZonesSplitOut())
                    {
                        if (mapIDsByShortName.ContainsKey(touchedZone.ToLower().Trim()) == false)
                        {
                            zonesAreLoaded = false;
                            Logger.WriteDebug("Skipping transport ship since zone '" + touchedZone + "' isn't being converted");
                            break;
                        }
                    }
                    if (zonesAreLoaded == false)
                        continue;
                    string name = "Ship EQ (" + transportShip.Name + ")";
                    string longName = transportShip.TouchedZones + " (" + name + ")";
                    transportsSQL.AddRow(transportShip.WOWGameObjectTemplateID, longName);
                    gameObjectTemplateSQL.AddRowForTransportShip(transportShip.WOWGameObjectTemplateID, transportShip.GameObjectDisplayInfoID, name,
                        transportShip.TaxiPathID, Convert.ToInt32(transportShip.MapID));
                    gameObjectTemplateAddonSQL.AddRowForTransport(transportShip.WOWGameObjectTemplateID);
                }
                foreach (TransportLift transportLift in TransportLift.GetAllTransportLifts())
                {
                    // Only add this transport lift if the zone is loaded
                    if (mapIDsByShortName.ContainsKey(transportLift.SpawnZoneShortName.ToLower().Trim()) == false)
                    {
                        Logger.WriteDebug("Skipping transport lift since zone '" + transportLift.SpawnZoneShortName + "' isn't being converted");
                        continue;
                    }
                    int areaID = 0;
                    foreach (Zone zone in zones)
                        if (zone.ShortName.ToLower().Trim() == transportLift.SpawnZoneShortName.ToLower().Trim())
                            areaID = Convert.ToInt32(zone.DefaultArea.DBCAreaTableID);

                    string name = "Lift EQ (" + transportLift.Name + ")";
                    int mapID = mapIDsByShortName[transportLift.SpawnZoneShortName.ToLower().Trim()];
                    gameObjectTemplateSQL.AddRowForTransportLift(transportLift.GameObjectTemplateID, transportLift.GameObjectDisplayInfoID, name, transportLift.EndTimestamp);
                    gameObjectTemplateAddonSQL.AddRowForTransport(transportLift.GameObjectTemplateID);
                    gameObjectSQL.AddRow(transportLift.GameObjectGUID, transportLift.GameObjectTemplateID, mapID, areaID, new Vector3(transportLift.SpawnX, transportLift.SpawnY, transportLift.SpawnZ), transportLift.Orientation);
                }
                foreach (TransportLiftTrigger transportLiftTrigger in TransportLiftTrigger.GetAllTransportLiftTriggers())
                {
                    // Only add this transport lift if the zone is loaded
                    if (mapIDsByShortName.ContainsKey(transportLiftTrigger.SpawnZoneShortName.ToLower().Trim()) == false)
                    {
                        Logger.WriteDebug("Skipping transport lift trigger since zone '" + transportLiftTrigger.SpawnZoneShortName + "' isn't being converted");
                        continue;
                    }
                    int areaID = 0;
                    foreach (Zone zone in zones)
                        if (zone.ShortName.ToLower().Trim() == transportLiftTrigger.SpawnZoneShortName.ToLower().Trim())
                            areaID = Convert.ToInt32(zone.DefaultArea.DBCAreaTableID);

                    string name = transportLiftTrigger.Name;
                    int mapID = mapIDsByShortName[transportLiftTrigger.SpawnZoneShortName.ToLower().Trim()];
                    gameObjectTemplateSQL.AddRowForTransportLiftTrigger(transportLiftTrigger.GameObjectTemplateID, transportLiftTrigger.GameObjectDisplayInfoID, name, transportLiftTrigger.ResetTimeInMS);
                    gameObjectTemplateAddonSQL.AddRowNoDespawn(transportLiftTrigger.GameObjectTemplateID);
                    gameObjectSQL.AddRow(transportLiftTrigger.GameObjectGUID, transportLiftTrigger.GameObjectTemplateID, mapID, areaID, new Vector3(transportLiftTrigger.SpawnX, transportLiftTrigger.SpawnY,
                        transportLiftTrigger.SpawnZ), transportLiftTrigger.Orientation);
                }
            }

            // Output them
            // Characters
            modEverquestCharacterHomebindSQL.SaveToDisk("mod_everquest_character_homebind", SQLFileType.Characters);
            // World
            areaTriggerSQL.SaveToDisk("areatrigger", SQLFileType.World);
            areaTriggerTeleportSQL.SaveToDisk("areatrigger_teleport", SQLFileType.World);
            broadcastTextSQL.SaveToDisk("broadcast_text", SQLFileType.World);
            creatureSQL.SaveToDisk("creature", SQLFileType.World);
            creatureAddonSQL.SaveToDisk("creature_addon", SQLFileType.World);
            creatureLootTableSQL.SaveToDisk("creature_loot_template", SQLFileType.World);
            creatureModelInfoSQL.SaveToDisk("creature_model_info", SQLFileType.World);
            creatureTemplateSQL.SaveToDisk("creature_template", SQLFileType.World);
            creatureTemplateModelSQL.SaveToDisk("creature_template_model", SQLFileType.World);
            creatureTextSQL.SaveToDisk("creature_text", SQLFileType.World);
            gameEventSQL.SaveToDisk("game_event", SQLFileType.World);
            gameGraveyardSQL.SaveToDisk("game_graveyard", SQLFileType.World);
            gameObjectSQL.SaveToDisk("gameobject", SQLFileType.World);
            gameObjectTemplateSQL.SaveToDisk("gameobject_template", SQLFileType.World);
            gameObjectTemplateAddonSQL.SaveToDisk("gameobject_template_addon", SQLFileType.World);
            gameTeleSQL.SaveToDisk("game_tele", SQLFileType.World);
            gameWeatherSQL.SaveToDisk("game_weather", SQLFileType.World);
            gossipMenuSQL.SaveToDisk("gossip_menu", SQLFileType.World);
            gossipMenuOptionSQL.SaveToDisk("gossip_menu_option", SQLFileType.World);
            graveyardZoneSQL.SaveToDisk("graveyard_zone", SQLFileType.World);
            instanceTemplateSQL.SaveToDisk("instance_template", SQLFileType.World);
            itemLootTemplateSQL.SaveToDisk("item_loot_template", SQLFileType.World);
            itemTemplateSQL.SaveToDisk("item_template", SQLFileType.World);
            modEverquestCreatureOnkillReputationSQL.SaveToDisk("mod_everquest_creature_onkill_reputation", SQLFileType.World);
            modEverquestQuestCompleteReputationSQL.SaveToDisk("mod_everquest_quest_complete_reputation", SQLFileType.World);
            npcTextSQL.SaveToDisk("npc_text", SQLFileType.World);
            npcTrainerSQL.SaveToDisk("npc_trainer", SQLFileType.World);
            npcVendorSQL.SaveToDisk("npc_vendor", SQLFileType.World);
            if (Configuration.PLAYER_USE_EQ_START_LOCATION == true)
                playerCreateInfoSQL.SaveToDisk("playercreateinfo", SQLFileType.World);
            poolCreatureSQL.SaveToDisk("pool_creature", SQLFileType.World);
            poolPoolSQL.SaveToDisk("pool_pool", SQLFileType.World);
            poolTemplateSQL.SaveToDisk("pool_template", SQLFileType.World);
            smartScriptsSQL.SaveToDisk("smart_scripts", SQLFileType.World);
            transportsSQL.SaveToDisk("transports", SQLFileType.World);
            waypointDataSQL.SaveToDisk("waypoint_data", SQLFileType.World);
            if (Configuration.GENERATE_QUESTS == true)
            {
                creatureQuestEnderSQL.SaveToDisk("creature_questender", SQLFileType.World);
                creatureQuestStarterSQL.SaveToDisk("creature_queststarter", SQLFileType.World);
                questTemplateSQL.SaveToDisk("quest_template", SQLFileType.World);
                questTemplateAddonSQL.SaveToDisk("quest_template_addon", SQLFileType.World);
            }
        }

        public void ExportTexturesForZone(Zone zone, string zoneInputFolder, string wowExportPath, string relativeZoneMaterialDoodadsPath,
            string inputObjectTextureFolder)
        {
            Logger.WriteDebug(string.Concat("- [", zone.ShortName, "]: Exporting textures for zone '", zone.ShortName, "'..."));

            // Create the folder to output
            string zoneOutputTextureFolder = Path.Combine(wowExportPath, "World", "Everquest", "ZoneTextures", zone.ShortName);
            if (Directory.Exists(zoneOutputTextureFolder) == false)
                FileTool.CreateBlankDirectory(zoneOutputTextureFolder, true);

            // Go through every texture to move and put it there
            foreach (Material material in zone.Materials)
            {
                foreach (string textureName in material.TextureNames)
                {
                    string sourceTextureFullPath = Path.Combine(zoneInputFolder, "Textures", string.Concat(textureName, ".blp"));
                    string outputTextureFullPath = Path.Combine(zoneOutputTextureFolder, string.Concat(textureName, ".blp"));
                    if (File.Exists(sourceTextureFullPath) == false)
                    {
                        Logger.WriteError("Could not copy texture '" + sourceTextureFullPath + "', it did not exist. Did you run blpconverter?");
                        continue;
                    }
                    FileTool.CopyFile(sourceTextureFullPath, outputTextureFullPath);
                    Logger.WriteDebug(string.Concat("- [", zone.ShortName, "]: Texture named '", textureName, "' copied"));
                }
            }

            // Also copy textures for the zone specific objects
            foreach (ObjectModel zoneObject in zone.GeneratedZoneObjects)
            {
                foreach(ObjectModelTexture texture in zoneObject.ModelTextures)
                {
                    string sourceTextureFullPath = Path.Combine(zoneInputFolder, "Textures", texture.TextureName + ".blp");
                    string outputTextureFullPath = Path.Combine(wowExportPath, relativeZoneMaterialDoodadsPath, zoneObject.Name, texture.TextureName + ".blp");
                    if (File.Exists(sourceTextureFullPath) == false)
                    {
                        Logger.WriteError("Could not copy texture '" + sourceTextureFullPath + "', it did not exist. Did you run blpconverter?");
                        continue;
                    }
                    FileTool.CopyFile(sourceTextureFullPath, outputTextureFullPath);
                    Logger.WriteDebug(string.Concat("- [", zone.ShortName, "]: Texture named '", texture.TextureName, "' copied"));
                }
            }

            // Finally copy the textures for any sound instance objects
            if (Configuration.AUDIO_SOUNDINSTANCE_DRAW_AS_BOX == true)
            {
                string soundInstanceInputTextureFullPath = Path.Combine(inputObjectTextureFolder, Configuration.AUDIO_SOUNDINSTANCE_RENDEROBJECT_MATERIAL_NAME + ".blp");
                foreach (ObjectModel zoneObject in zone.SoundInstanceObjectModels)
                {
                    string soundInstanceOutputTextureFullPath = Path.Combine(wowExportPath, relativeZoneMaterialDoodadsPath, zoneObject.Name, Configuration.AUDIO_SOUNDINSTANCE_RENDEROBJECT_MATERIAL_NAME + ".blp");
                    if (File.Exists(soundInstanceInputTextureFullPath) == false)
                    {
                        Logger.WriteError("Could not copy texture '" + soundInstanceInputTextureFullPath + "', it did not exist. Did you run blpconverter?");
                        continue;
                    }
                    FileTool.CopyFile(soundInstanceInputTextureFullPath, soundInstanceOutputTextureFullPath);
                }
            }

            Logger.WriteDebug(string.Concat("- [", zone.ShortName, "]: Texture output for zone '", zone.ShortName, "' complete"));
        }

        public void ExportMusicForZone(Zone zone, string musicInputFolder, string wowExportPath)
        {
            Logger.WriteDebug(string.Concat("- [", zone.ShortName, "]: Exporting music for zone '", zone.ShortName, "'..."));

            if (zone.ZoneAreaMusics.Count == 0)
            {
                Logger.WriteDebug("- [" + zone.ShortName + "]: No music found for this zone");
                return;
            }

            // Create the folder to output
            string zoneOutputMusicFolder = Path.Combine(wowExportPath, "Sound", "Music", "Everquest", zone.ShortName);
            if (Directory.Exists(zoneOutputMusicFolder) == false)
                FileTool.CreateBlankDirectory(zoneOutputMusicFolder, true);

            // Go through every music sound object and put there if needed
            foreach(var musicSoundByFileName in zone.MusicSoundsByFileNameNoExt)
            {
                string sourceFullPath = Path.Combine(musicInputFolder, musicSoundByFileName.Value.AudioFileNameNoExt + ".mp3");
                string targetFullPath = Path.Combine(zoneOutputMusicFolder, musicSoundByFileName.Value.AudioFileNameNoExt + ".mp3");
                if (File.Exists(sourceFullPath) == false)
                {
                    Logger.WriteError("Could not copy music file '" + sourceFullPath + "', as it did not exist");
                    continue;
                }
                FileTool.CopyFile(sourceFullPath, targetFullPath);
                Logger.WriteDebug(string.Concat("- [", zone.ShortName, "]: Music named '", musicSoundByFileName.Value.AudioFileNameNoExt, "' copied"));
            }
            Logger.WriteDebug(string.Concat("- [", zone.ShortName, "]: Music output for zone '", zone.ShortName, "' complete"));
        }

        public void ExportAmbientSoundForZone(Zone zone, string soundInputFolder, string wowExportPath)
        {
            Logger.WriteDebug("- [" + zone.ShortName + "]: Exporting ambient sound for zone '" + zone.ShortName + "'...");

            // Create the folder to output
            string zoneOutputAmbienceFolder = Path.Combine(wowExportPath, "Sound", "Ambience", "Everquest");
            if (Directory.Exists(zoneOutputAmbienceFolder) == false)
                FileTool.CreateBlankDirectory(zoneOutputAmbienceFolder, true);

            // Go through every sound object and put there if needed
            foreach (var ambientSoundByFileName in zone.AmbientSoundsByFileNameNoExt)
            {
                // Skip the sound file for silence
                if (ambientSoundByFileName.Key.ToUpper() == "SILENCE")
                {
                    Logger.WriteDebug("Skipping sound file named '" + ambientSoundByFileName + "' since it is just silence");
                    continue;
                }
                string sourceFullPath = Path.Combine(soundInputFolder, ambientSoundByFileName.Value.AudioFileNameNoExt + ".wav");
                string targetFullPath = Path.Combine(zoneOutputAmbienceFolder, ambientSoundByFileName.Value.AudioFileNameNoExt + ".wav");
                if (File.Exists(sourceFullPath) == false)
                {
                    Logger.WriteError("Could not copy ambient sound file '" + sourceFullPath + "', as it did not exist");
                    continue;
                }
                FileTool.CopyFile(sourceFullPath, targetFullPath);
                Logger.WriteDebug("- [" + zone.ShortName + "]: Ambient sound named '" + ambientSoundByFileName.Value.AudioFileNameNoExt + "' copied");
            }
            foreach (SoundInstance soundInstance in zone.SoundInstances)
            {
                Sound? curSound = soundInstance.Sound;
                if (curSound == null)
                    Logger.WriteError("Could not copy sound file for '" + soundInstance.SoundFileNameDayNoExt + "' since the Sound object was null");
                else
                {
                    string sourceFullPath = Path.Combine(soundInputFolder, curSound.AudioFileNameNoExt + ".wav");
                    string targetFullPath = Path.Combine(zoneOutputAmbienceFolder, curSound.AudioFileNameNoExt + ".wav");
                    if (File.Exists(sourceFullPath) == false)
                    {
                        Logger.WriteError("Could not copy sound instance sound file '" + sourceFullPath + "', as it did not exist");
                        continue;
                    }
                    FileTool.CopyFile(sourceFullPath, targetFullPath);
                    Logger.WriteDebug("- [" + zone.ShortName + "]: Sound instance sound named '" + curSound.AudioFileNameNoExt + "' copied");
                }
            }

            Logger.WriteDebug("- [" + zone.ShortName + "]: Ambient sound output for zone '" + zone.ShortName + "' complete");
        }

        public void ExportTexturesForObject(ObjectModel wowObjectModelData, string objectTextureInputFolder, string objectExportPath)
        {
            Logger.WriteDebug("- [" + wowObjectModelData.Name + "]: Exporting textures for object '" + wowObjectModelData.Name + "'...");

            // Go through every texture and copy it
            foreach(ObjectModelTexture texture in wowObjectModelData.ModelTextures)
            {
                string inputTextureName = Path.Combine(objectTextureInputFolder, texture.TextureName + ".blp");
                string outputTextureName = Path.Combine(objectExportPath, texture.TextureName + ".blp");
                if (Path.Exists(inputTextureName) == false)
                {
                    Logger.WriteError("- [" + wowObjectModelData.Name + "]: Error Texture named '" + texture.TextureName + ".blp' not found.  Did you run blpconverter?");
                    return;
                }
                FileTool.CopyFile(inputTextureName, outputTextureName);
                Logger.WriteDebug("- [" + wowObjectModelData.Name + "]: Texture named '" + texture.TextureName + ".blp' copied");
            }

            Logger.WriteDebug("- [" + wowObjectModelData.Name + "]: Texture output for object '" + wowObjectModelData.Name + "' complete");
        }

        public void CopyIconFiles()
        {
            Logger.WriteInfo("Copying icon files...");

            // Clear and create the directory
            string iconOutputFolder = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "MPQReady", "Interface", "ICONS");
            if (Directory.Exists(iconOutputFolder) == true)
                Directory.Delete(iconOutputFolder, true);
            Directory.CreateDirectory(iconOutputFolder);

            // Items
            string itemIconInputFolder = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "itemicons");
            string[] sourceItemIconFilePaths = Directory.GetFiles(itemIconInputFolder, "*.blp");
            foreach (string inputIconFile in sourceItemIconFilePaths)
            {
                string outputIconFile = Path.Combine(iconOutputFolder, Path.GetFileName(inputIconFile));
                FileTool.CopyFile(inputIconFile, outputIconFile);
            }

            // Spells
            string spellIconInputFolder = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "spellicons");
            string[] sourceSpellIconFilePaths = Directory.GetFiles(spellIconInputFolder, "*.blp");
            foreach (string inputIconFile in sourceSpellIconFilePaths)
            {
                string outputIconFile = Path.Combine(iconOutputFolder, Path.GetFileName(inputIconFile));
                FileTool.CopyFile(inputIconFile, outputIconFile);
            }
        }
    }
}
