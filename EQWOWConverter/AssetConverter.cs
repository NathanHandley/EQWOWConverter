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
using EQWOWConverter.GameObjects;
using EQWOWConverter.Items;
using EQWOWConverter.ObjectModels;
using EQWOWConverter.ObjectModels.Properties;
using EQWOWConverter.Player;
using EQWOWConverter.Quests;
using EQWOWConverter.Spells;
using EQWOWConverter.Tradeskills;
using EQWOWConverter.Transports;
using EQWOWConverter.WOWFiles;
using EQWOWConverter.Zones;
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

            // Clean output folder
            string exportMPQRootFolder = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "MPQReady");
            if (Directory.Exists(exportMPQRootFolder) == true)
                Directory.Delete(exportMPQRootFolder, true);   

            // Extract DBC files
            DBCFileWorker dbcFileWorker = new DBCFileWorker();
            if (Configuration.GENERATE_EXTRACT_DBC_FILES == true)
                dbcFileWorker.ExtractClientDBCFiles();
            else
                Logger.WriteInfo("- Note: DBC File Extraction is set to false in the Configuration");

            // Extract minimap metadata
            if (Configuration.GENERATE_MINIMAPS == true)
                ExtractMinimapMD5TranslateFile();

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
            Dictionary<int, CreatureTemplate> creatureTemplatesByEQID = CreatureTemplate.GetCreatureTemplateListByEQID();
            List<CreatureSpawnPool> creatureSpawnPools = new List<CreatureSpawnPool>();
            Task creaturesAndSpawnsTask = Task.Factory.StartNew(() =>
            {
                Logger.WriteInfo("<+> Thread [Creatures, Transports, and Spawns] Started");

                // Creatures
                CreatureRace.GenerateAllSounds();
                if (Configuration.GENERATE_CREATURES_AND_SPAWNS == true)
                {
                    if (Configuration.CREATURE_SPAWN_AND_WAYPOINT_DEBUG_MODE == true)
                        ConvertCreaturesForDebug(creatureTemplatesByEQID, ref creatureSpawnPools);
                    else
                        ConvertCreatures(creatureTemplatesByEQID, ref creatureSpawnPools);
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

            // Thread 3: Items, Spells, Tradeskills
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
                GenerateSpells(out spellTemplates, itemTemplatesByEQDBID, ref creatureTemplatesByEQID); // Remove the 'ref'

                // Update class-specific references from spell scrolls
                if (Configuration.SPELLS_LEARNABLE_FROM_ITEMS_ENABLED == true)
                {
                    Dictionary<int, SpellTemplate> spellTemplatesByEQID = SpellTemplate.GetSpellTemplatesByEQID();
                    foreach (var itemTemplateByWOWEntryID in itemTemplatesByWOWEntryID)
                    {
                        if (itemTemplateByWOWEntryID.Value.EQScrollSpellID > 0 && spellTemplatesByEQID.ContainsKey(itemTemplateByWOWEntryID.Value.EQScrollSpellID) == true)
                            itemTemplateByWOWEntryID.Value.PopulateClassSpecificVersionsForSpellScrolls(spellTemplatesByEQID[itemTemplateByWOWEntryID.Value.EQScrollSpellID].LearnScrollPropertiesByClassType);
                    }
                }                

                // Tradeskills
                GenerateTradeskills(itemTemplatesByEQDBID, ref spellTemplates, out tradeskillRecipes);

                Logger.WriteInfo("<-> Thread [Items, Spells, Tradeskills] Ended");
            }, TaskCreationOptions.LongRunning);
            creaturesAndSpawnsTask.Wait();
            itemsSpellsTradeskillsTask.Wait();

            // Creature model files
            List<CreatureTemplate> creatureTemplates = CreatureTemplate.GetCreatureTemplateListByWOWID().Values.ToList();
            List<CreatureModelTemplate> creatureModelTemplates = new List<CreatureModelTemplate>();
            Task creatureModelFilesTask = Task.Factory.StartNew(() =>
            {
                Logger.WriteInfo("<+> Thread [Creature Models] Started");

                GenerateCreatureModelFiles(creatureTemplates, ref creatureModelTemplates);

                Logger.WriteInfo("<-> Thread [Creature Models] Ended");
            }, TaskCreationOptions.LongRunning);
            if (Configuration.CORE_ENABLE_MULTITHREADING == false)
                creatureModelFilesTask.Wait();

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

            // Assign item spell effects
            AssignItemSpellEffects(ref itemTemplatesByWOWEntryID);

            // Quests Finish-up
            if (Configuration.GENERATE_QUESTS == true)
                ConvertQuests(ref questTemplates, ref creatureTemplates);

            // Generate item graphics
            CreateItemGraphics(ref itemTemplatesByEQDBID);

            // Make sure threads are done
            if (Configuration.CORE_ENABLE_MULTITHREADING == true)
            {
                zoneAndObjectTask.Wait();
                creatureModelFilesTask.Wait();
            }

            // Generate creature spell logic
            // Note: Should be one of the last things so that the spells are only valid ones
            Dictionary<int, SpellTemplate> spellTemplatesByEQID = SpellTemplate.GetSpellTemplatesByEQID(); // Manage this better since spell templates is pulled above
            ConvertCreatureSpellAI(ref creatureTemplates, spellTemplatesByEQID);

            // Create the DBC files
            dbcFileWorker.CreateDBCFiles(zones, creatureModelTemplates, spellTemplates);

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

                // Create or update the MPQs
                string exportMPQFileName = Path.Combine(Configuration.PATH_EXPORT_FOLDER, Configuration.PATH_CLIENT_PATCH_LOC_FILE_NAME_NO_EXT + ".mpq");
                if (Configuration.GENERATE_ONLY_LISTED_ZONE_SHORTNAMES.Count == 0 || File.Exists(exportMPQFileName) == false)
                    CreateMainPatchMPQ();
                else
                    UpdateMainPatchMPQ();
                if (Configuration.GENERATE_MINIMAPS == true)
                    CreateMinimapPatchMPQ();

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
                SQLScriptWorker sqlWorker = new SQLScriptWorker();
                sqlWorker.CreateSQLScripts(zones, creatureTemplates, creatureModelTemplates, creatureSpawnPools, 
                    itemLootTemplatesByCreatureTemplateID, questTemplates, tradeskillRecipes, spellTemplates);

                if (Configuration.DEPLOY_SERVER_FILES == true)
                    DeployServerFiles();
                else
                    Logger.WriteInfo("- Note: DEPLOY_SERVER_FILES set false in the Configuration");
                if (Configuration.DEPLOY_SERVER_SQL == true)
                    sqlWorker.DeployServerSQL();
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
                    ExportTexturesForObject(curObjectModel, new List<string>() { objectTextureFolder }, curStaticObjectOutputFolder);

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
                    ExportTexturesForObject(curObjectModel, new List<string>() { objectTextureFolder }, curStaticObjectOutputFolder);

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
                ExportTexturesForObject(curObject, new List<string>() { objectTextureFolder }, curObjectOutputFolder);

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
                ExportTexturesForObject(curObject, new List<string>() { objectTextureFolder }, curObjectOutputFolder);

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
            questTemplates = QuestTemplate.GetQuestTemplates();

            // Work through each of the quest templates
            Dictionary<string, ZoneProperties> zonePropertiesByShortName = ZoneProperties.GetZonePropertyListByShortName();
            foreach (QuestTemplate questTemplate in questTemplates)
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
                for (int i = 0; i < questTemplate.RewardItems.Count; i++)
                {
                    if (itemTemplatesByEQDBID.ContainsKey(questTemplate.RewardItems[i].itemIDEQ) == false)
                        questTemplate.IsValidQuest = false;
                    else
                        itemTemplatesByEQDBID[questTemplate.RewardItems[i].itemIDEQ].IsRewardedFromQuest = true;
                }   
            }
        }

        public void ConvertQuests(ref List<QuestTemplate> questTemplates, ref List<CreatureTemplate> creatureTemplates)
        {
            Dictionary<string, ZoneProperties> zonePropertiesByShortName = ZoneProperties.GetZonePropertyListByShortName();
            Dictionary<int, CreatureTemplate> creatureTemplatesByEQID = CreatureTemplate.GetCreatureTemplateListByEQID();
            foreach (QuestTemplate questTemplate in questTemplates)
            {
                // Skip any quests that are in zones we're not processing
                if (zonePropertiesByShortName.ContainsKey(questTemplate.ZoneShortName.ToLower()) == false)
                {
                    Logger.WriteDebug(string.Concat("Ignoring quest with id '", questTemplate.QuestIDWOW, ", as the zone '", questTemplate.ZoneShortName, "' is not being generated"));
                    questTemplate.IsValidQuest = false;
                    continue;
                }

                // Skip any quests where the items cannot be obtained
                if (questTemplate.HasInvalidItems == true)
                {
                    questTemplate.IsValidQuest = false;
                    continue;
                }
                SortedDictionary<int, ItemTemplate> itemTemplatesByWOWEntryID = ItemTemplate.GetItemTemplatesByWOWEntryID();
                if (questTemplate.AreRequiredItemsPlayerObtainable(itemTemplatesByWOWEntryID) == false)
                {
                    questTemplate.IsValidQuest = false;
                    continue;
                }

                // If needed, split out rewards based on class-specific versions.  Divide the chances accordingly
                bool hasRandomReward = false;
                List<QuestTemplate.QuestItemReference> expandedQuestRewardItems = new List<QuestTemplate.QuestItemReference>();
                foreach(QuestTemplate.QuestItemReference questRewardItemReference in questTemplate.RewardItems)
                {
                    if (itemTemplatesByWOWEntryID.ContainsKey(questRewardItemReference.itemIDWOW) == false)
                    {
                        Logger.WriteError("Quest ", questTemplate.QuestIDWOW.ToString(), " has an invalid reward item reference with wowid of ", questRewardItemReference.itemIDWOW.ToString());
                        questTemplate.IsValidQuest = false;
                        break;
                    }
                    ItemTemplate questRewardItem = itemTemplatesByWOWEntryID[questRewardItemReference.itemIDWOW];
                    if (questRewardItem.ClassSpecificItemVersionsByWOWItemTemplateID.Count > 0)
                    {
                        foreach (int classSpecificItemIDWOW in questRewardItem.ClassSpecificItemVersionsByWOWItemTemplateID.Values)
                        {
                            QuestTemplate.QuestItemReference classSpecificReference = new QuestTemplate.QuestItemReference();
                            classSpecificReference.itemIDParentWOW = questRewardItem.WOWEntryID;
                            classSpecificReference.itemIDWOW = classSpecificItemIDWOW;
                            classSpecificReference.itemIDEQ = questRewardItemReference.itemIDEQ;
                            classSpecificReference.itemCount = questRewardItemReference.itemCount;
                            if (questRewardItemReference.itemChance == 100)
                                classSpecificReference.itemChance = 100;
                            else
                                classSpecificReference.itemChance = questRewardItemReference.itemChance / questRewardItem.ClassSpecificItemVersionsByWOWItemTemplateID.Values.Count;
                            expandedQuestRewardItems.Add(classSpecificReference);
                        }
                    }
                    else
                        expandedQuestRewardItems.Add(questRewardItemReference);
                    if (questRewardItemReference.itemChance != 100)
                        hasRandomReward = true;
                }
                if (questTemplate.IsValidQuest == false)
                    continue;
                questTemplate.RewardItems.Clear();

                // For chance-rewards or reward counts greater than 4, stick them in a reward bag.  Otherwise directly attach back to the quest
                if (hasRandomReward == true || expandedQuestRewardItems.Count > 4)
                {
                    if (questTemplate.MultiRewardContainerWOWItemID <= 0)
                    {
                        Logger.WriteError("Quest template ", questTemplate.QuestIDWOW.ToString(), " had multiple rewards but no reward_container_wowid, skipping");
                        questTemplate.IsValidQuest = false;
                        continue;
                    }

                    string containerName = string.Concat(questTemplate.QuestgiverName.Replace("_", " ").Replace("#", ""), "'s Reward");
                    questTemplate.RandomAwardContainerItemTemplate = ItemTemplate.CreateQuestRandomItemContainer(containerName, expandedQuestRewardItems, questTemplate.MultiRewardContainerWOWItemID);
                    questTemplate.RewardItems.Clear();
                    if (questTemplate.RandomAwardContainerItemTemplate != null)
                    {
                        QuestTemplate.QuestItemReference questItemReference = new QuestTemplate.QuestItemReference();
                        questItemReference.itemIDWOW = questTemplate.RandomAwardContainerItemTemplate.WOWEntryID;
                        questItemReference.itemIDEQ = questTemplate.RandomAwardContainerItemTemplate.EQItemID;
                        questItemReference.itemCount = 1;
                        questItemReference.itemChance = 100;
                        questTemplate.RewardItems.Add(questItemReference);
                    }
                }
                else
                {
                    questTemplate.RewardItems = expandedQuestRewardItems;
                }

                // Spell scrolls can also be a required item, so update the references as needed
                foreach (QuestTemplate.QuestItemReference itemReference in questTemplate.RequiredItems)
                {
                    if (itemTemplatesByWOWEntryID.ContainsKey(itemReference.itemIDWOW) == false)
                    {
                        Logger.WriteError("Quest ", questTemplate.QuestIDWOW.ToString(), " has an invalid required item reference with wowid of ", itemReference.itemIDWOW.ToString());
                        questTemplate.IsValidQuest = false;
                        continue;
                    }
                    ItemTemplate requiredItemTemplate = itemTemplatesByWOWEntryID[itemReference.itemIDWOW];
                    if (requiredItemTemplate.ClassSpecificItemVersionsByWOWItemTemplateID.Count > 0)
                    {
                        if (questTemplate.RewardItems.Count > 0)
                        {
                            // If the first reward is class specific, use that as the class reference.
                            ItemTemplate firstRewardItemTemplate = itemTemplatesByWOWEntryID[questTemplate.RewardItems[0].itemIDWOW];
                            bool matchFound = false;
                            foreach (var classSpecificItem in requiredItemTemplate.ClassSpecificItemVersionsByWOWItemTemplateID)
                            {
                                if (firstRewardItemTemplate.AllowedClassTypes.Contains(classSpecificItem.Key) == true)
                                {
                                    itemReference.itemIDWOW = classSpecificItem.Value;
                                    matchFound = true;
                                    break;
                                }
                            }
                            if (matchFound == false)
                                itemReference.itemIDWOW = requiredItemTemplate.ClassSpecificItemVersionsByWOWItemTemplateID.First().Value;
                        }
                        else
                        {
                            bool validItemFound = false;
                            foreach (var classSpecificItem in requiredItemTemplate.ClassSpecificItemVersionsByWOWItemTemplateID)
                            {
                                if (itemTemplatesByWOWEntryID.ContainsKey(classSpecificItem.Value) == true)
                                {
                                    itemReference.itemIDWOW = itemTemplatesByWOWEntryID[classSpecificItem.Value].WOWEntryID;
                                    validItemFound = true;
                                    break;
                                }
                            }
                            if (validItemFound == false)
                            {
                                Logger.WriteError("Quest ", questTemplate.QuestIDWOW.ToString(), " has no valid class specific item reference for wowid of ", itemReference.itemIDWOW.ToString());
                                questTemplate.IsValidQuest = false;
                                continue;
                            }                                
                        }
                        itemReference.itemIDParentWOW = requiredItemTemplate.WOWEntryID;
                    }
                    else
                    {
                        // Also ensure that the 'essence container' versions are not being used as a required item
                        if (requiredItemTemplate.NonEssenceWOWEntryID != 0)
                            itemReference.itemIDWOW = requiredItemTemplate.NonEssenceWOWEntryID;
                    }
                }

                // Pull up the related creature(s) that are quest givers and mark them as quest givers
                // NOTE: Always do this last (before the add(questTemplate)
                List<CreatureTemplate> questgiverCreatureTemplates = CreatureTemplate.GetCreatureTemplatesForSpawnZonesAndName(questTemplate.ZoneShortName, questTemplate.QuestgiverName);
                if (questgiverCreatureTemplates.Count == 0)
                {
                    Logger.WriteDebug(string.Concat("Could not map quest to creature template with zone '", questTemplate.ZoneShortName, "' and name '", questTemplate.QuestgiverName, "'"));
                    questTemplate.IsValidQuest = false;
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
                        if (reaction.ReactionType == QuestReactionType.Emote || reaction.ReactionType == QuestReactionType.Say || reaction.ReactionType == QuestReactionType.Yell)
                            foreach (CreatureTemplate creatureTemplate in questgiverCreatureTemplates)
                                creatureTemplate.HasSmartScript = true;
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
            string exportMPQFileName = Path.Combine(Configuration.PATH_EXPORT_FOLDER, Configuration.PATH_CLIENT_PATCH_LOC_FILE_NAME_NO_EXT + ".mpq");
            string relativeStaticDoodadsPath = Path.Combine("World", "Everquest", "StaticDoodads");

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

            // Generate the zone
            Zone curZone = new Zone(zoneShortName, zoneProperties);
            Logger.WriteDebug("- [" + curZone.ShortName + "]: Converting zone '" + curZone.ShortName + "' into a wow zone...");
            string curZoneDirectory = Path.Combine(inputZoneFolder, zoneShortName);
            curZone.LoadFromEQZone(zoneShortName, curZoneDirectory, GameObject.GetDoodadGameObjectsForZoneShortname(zoneShortName));

            // Create the zone WMO objects
            WMO zoneWMO = new WMO(curZone, exportMPQRootFolder, "WORLD\\EVERQUEST\\ZONETEXTURES", relativeStaticDoodadsPath, relativeZoneObjectsPath, false);
            zoneWMO.WriteToDisk();

            // Calculate range of ADTs to generate based on zone dimensions
            // Note: Coordinate system differs between ADT and WMO, so the top/bottom are traded (and sign inverted)
            float tileLength = 1600f / 3f; // Comes out to 533.333 repeat, doing the math here to make it be as exact as possible
            float worldNorth = curZone.BoundingBox.BottomCorner.X * -1f;
            float worldWest = curZone.BoundingBox.BottomCorner.Y * -1f;
            float worldSouth = curZone.BoundingBox.TopCorner.X * -1f;
            float worldEast = curZone.BoundingBox.TopCorner.Y * -1f;
            int tileXMin = 31 - Convert.ToInt32(MathF.Truncate(MathF.Abs(worldWest) / tileLength));
            int tileXMax = 32 + Convert.ToInt32(MathF.Truncate(MathF.Abs(worldEast) / tileLength));
            int tileYMin = 31 - Convert.ToInt32(MathF.Truncate(MathF.Abs(worldNorth) / tileLength));
            int tileYMax = 32 + Convert.ToInt32(MathF.Truncate(MathF.Abs(worldSouth) / tileLength));

            // Control for minimums
            tileXMin = Math.Min(31, tileXMin);
            tileXMax = Math.Max(32, tileXMax);
            tileYMin = Math.Min(31, tileYMin);
            tileYMax = Math.Max(32, tileYMax);

            // Create the WDT
            WDT zoneWDT = new WDT(curZone, zoneWMO.RootFileRelativePathWithFileName, tileXMin, tileXMax, tileYMin, tileYMax);
            zoneWDT.WriteToDisk(exportMPQRootFolder);

            // Create the ADT
            for (UInt32 y = Convert.ToUInt32(tileYMin); y <= Convert.ToUInt32(tileYMax); y++)
                for (UInt32 x = Convert.ToUInt32(tileXMin); x <= Convert.ToUInt32(tileXMax); x++)
                {
                    ADT zoneADT = new ADT(curZone, zoneWMO.RootFileRelativePathWithFileName, x, y, (curZone.BoundingBox.TopCorner.Z + 5f));
                    zoneADT.WriteToDisk(exportMPQRootFolder);
                }

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

        public void GenerateCreatureModelFiles(List<CreatureTemplate> creatureTemplates, ref List<CreatureModelTemplate> creatureModelTemplates)
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
                return;
            }
            string inputObjectTextureFolder = Path.Combine(charactersFolderRoot, "Textures");
            string generatedTexturesFolderPath = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "GeneratedCreatureTextures");

            // Generate the creatures
            LogCounter progressionCounter = new LogCounter("Creating creature model files...");
            CreatureModelTemplate.CreateCreatureModelTemplatesFromCreatureTemplates(creatureTemplates);
            foreach (var modelTemplatesByRaceID in CreatureModelTemplate.AllTemplatesByRaceID)
            {
                foreach (CreatureModelTemplate modelTemplate in modelTemplatesByRaceID.Value)
                {
                    modelTemplate.CreateModelFiles(charactersFolderRoot, inputObjectTextureFolder, exportAnimatedObjectsFolder, generatedTexturesFolderPath);
                    creatureModelTemplates.Add(modelTemplate);
                    progressionCounter.Write(1);
                }
            }
        }

        public bool ConvertCreaturesForDebug(Dictionary<int, CreatureTemplate> creatureTemplatesByEQID, ref List<CreatureSpawnPool> creatureSpawnPools)
        {
            Logger.WriteInfo("Converting EQ Creatures (skeletal objects) to WOW creature objects for DEBUG WAYPOINTS...");

            // Get a list of valid zone names
            Dictionary<string, int> mapIDsByShortName = new Dictionary<string, int>();
            Dictionary<int, ZoneProperties> zonePropertiesByMapID = new Dictionary<int, ZoneProperties>();
            Dictionary<int, int> defaultAreaIDsByMapID = new Dictionary<int, int>();
            foreach (var zoneProperties in ZoneProperties.GetZonePropertyListByShortName())
            {
                mapIDsByShortName.Add(zoneProperties.Value.ShortName.ToLower().Trim(), zoneProperties.Value.DBCMapID);
                zonePropertiesByMapID.Add(zoneProperties.Value.DBCMapID, zoneProperties.Value);
                defaultAreaIDsByMapID.Add(zoneProperties.Value.DBCMapID, Convert.ToInt32(zoneProperties.Value.DefaultZoneArea.DBCAreaTableID));
            }

            // Create creature templates for every spawn instance
            List<CreatureSpawnInstance> creatureSpawnInstances = CreatureSpawnInstance.GetSpawnInstanceListByID().Values.ToList();
            foreach (CreatureSpawnInstance creatureSpawnInstance in creatureSpawnInstances)
            {
                if (mapIDsByShortName.ContainsKey(creatureSpawnInstance.ZoneShortName.ToLower().Trim()) == true)
                {
                    CreatureTemplate.SpawnWaypointDebugCreateCreatureTemplate(creatureSpawnInstance.ID, 0, 0, creatureSpawnInstance.SpawnXPosition, creatureSpawnInstance.SpawnYPosition,
                        creatureSpawnInstance.SpawnZPosition, creatureSpawnInstance.ZoneShortName, mapIDsByShortName[creatureSpawnInstance.ZoneShortName.ToLower().Trim()],
                        defaultAreaIDsByMapID[mapIDsByShortName[creatureSpawnInstance.ZoneShortName.ToLower().Trim()]]);
                }
            }

            // Generate a creature template for every grid entry
            foreach (CreaturePathGridEntry gridEntry in CreaturePathGridEntry.GetInitialPathGridEntries())
            {
                if (mapIDsByShortName.ContainsKey(gridEntry.ZoneShortName.ToLower().Trim()) == true)
                    CreatureTemplate.SpawnWaypointDebugCreateCreatureTemplate(0, gridEntry.GridID, gridEntry.Number, gridEntry.NodeX, gridEntry.NodeY, gridEntry.NodeZ, gridEntry.ZoneShortName,
                        mapIDsByShortName[gridEntry.ZoneShortName.ToLower().Trim()], defaultAreaIDsByMapID[mapIDsByShortName[gridEntry.ZoneShortName.ToLower().Trim()]]);
            }

            // Copy all of the sound files
            Logger.WriteInfo("Copying creature sound files...");
            string inputSoundFolderRoot = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "sounds");
            string exportCreatureSoundsDirectory = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "MPQReady", "Sound", "Creature", "Everquest");
            if (Directory.Exists(exportCreatureSoundsDirectory) == true)
                Directory.Delete(exportCreatureSoundsDirectory, true);
            FileTool.CreateBlankDirectory(exportCreatureSoundsDirectory, false);
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

        public bool ConvertCreatures(Dictionary<int, CreatureTemplate> creatureTemplatesByEQID, ref List<CreatureSpawnPool> creatureSpawnPools)
        {
            Logger.WriteInfo("Converting EQ Creatures (skeletal objects) to WOW creature objects...");

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
                        if (creatureTemplatesByEQID.ContainsKey(spawnEntry.EQCreatureTemplateID))
                        {
                            curSpawnPool.AddCreatureTemplate(creatureTemplatesByEQID[spawnEntry.EQCreatureTemplateID], spawnEntry.Chance);

                            // If  this is a limited spawn, limit the group
                            // TODO: This should be handled differently, as only specific creature templates should be limited
                            if (creatureTemplatesByEQID[spawnEntry.EQCreatureTemplateID].LimitOneInSpawnPool == true)
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

        public void ConvertCreatureSpellAI(ref List<CreatureTemplate> creatureTemplates, Dictionary<int, SpellTemplate> spellTemplatesByEQID)
        {
            Logger.WriteInfo("Converting Creature Spell AI...");

            // Load the creature spell reference data
            List<CreatureSpellList> creatureSpellLists = CreatureSpellList.GetCreatureSpellLists();
            SortedDictionary<int, List<CreatureSpellEntry>> creatureSpellEntriesByListID = CreatureSpellEntry.GetCreatureSpellEntriesByListID();

            // Cull the lists down to only list entries that have valid spells within them
            foreach (List<CreatureSpellEntry> creatureSpellEntries in creatureSpellEntriesByListID.Values)
                for (int i = creatureSpellEntries.Count - 1;  i >= 0; i--)
                    if (spellTemplatesByEQID.ContainsKey(creatureSpellEntries[i].EQSpellID) == false)
                        creatureSpellEntries.RemoveAt(i);

            // Create a mapping of spell list for faster lookup
            Dictionary<int, CreatureSpellList> creatureSpellListsByID = new Dictionary<int, CreatureSpellList>();
            foreach (CreatureSpellList creatureSpellList in creatureSpellLists)
                creatureSpellListsByID.Add(creatureSpellList.ID, creatureSpellList);

            // Get a list of possible spells cast by the creature
            foreach (CreatureTemplate creatureTemplate in creatureTemplates)
            {
                List<CreatureSpellEntry> allValidSpellEntries = new List<CreatureSpellEntry>();
                
                // Skip any creature with no spell list as they'd have no AI
                if (creatureTemplate.CreatureSpellListID == 0)
                    continue;
                if (creatureSpellListsByID.ContainsKey(creatureTemplate.CreatureSpellListID) == false)
                {
                    Logger.WriteError("For creature template eqid ", creatureTemplate.EQCreatureTemplateID.ToString(), " there was no creature spell list with id ", creatureTemplate.CreatureSpellListID.ToString());
                    continue;
                }

                // Store all of the entries
                CreatureSpellList creatureSpellList = creatureSpellListsByID[creatureTemplate.CreatureSpellListID];
                if (creatureSpellEntriesByListID.ContainsKey(creatureTemplate.CreatureSpellListID) == true)
                {
                    foreach (CreatureSpellEntry spellEntry in creatureSpellEntriesByListID[creatureTemplate.CreatureSpellListID])
                        allValidSpellEntries.Add(spellEntry);
                }

                // Save On-Attack information
                if (creatureSpellList.AttackProcID > 0 && spellTemplatesByEQID.ContainsKey(creatureSpellList.AttackProcID))
                {
                    int eqSpellID = creatureSpellList.AttackProcID;
                    int procChance = creatureSpellList.ProcChance;
                    creatureTemplate.AttackEQSpellIDAndProcChance.Add((eqSpellID, procChance));
                }

                // Handle parent mappings
                if (creatureSpellList.ParentListID != 0)
                {
                    if (creatureSpellListsByID.ContainsKey(creatureSpellList.ParentListID) == false)
                        Logger.WriteDebug("For creature template eqid ", creatureTemplate.EQCreatureTemplateID.ToString(), " there was no creature spell list for parent with id ", creatureSpellList.ParentListID.ToString());
                    else
                    {
                        CreatureSpellList creatureParentSpellList = creatureSpellListsByID[creatureSpellList.ParentListID];

                        // Add the parent entries
                        if (creatureSpellEntriesByListID.ContainsKey(creatureSpellList.ParentListID) == true)
                        {
                            foreach (CreatureSpellEntry spellEntry in creatureSpellEntriesByListID[creatureSpellList.ParentListID])
                                allValidSpellEntries.Add(spellEntry);
                        }

                        // Add the On-Attack
                        if (creatureParentSpellList.AttackProcID > 0 && spellTemplatesByEQID.ContainsKey(creatureParentSpellList.AttackProcID))
                        {
                            int eqSpellID = creatureParentSpellList.AttackProcID;
                            int procChance = creatureParentSpellList.ProcChance;
                            creatureTemplate.AttackEQSpellIDAndProcChance.Add((eqSpellID, procChance));
                        }
                    }
                }

                // Calculate a true minimum recast delay by factoring in spell cast and/or aura time
                for (int i = 0; i < allValidSpellEntries.Count; i++)
                {
                    CreatureSpellEntry curEntry = allValidSpellEntries[i];
                    SpellTemplate spellTemplate = spellTemplatesByEQID[allValidSpellEntries[i].EQSpellID];
                    int originalRecastDelayInMS = allValidSpellEntries[i].OriginalRecastDelayInMS;
                    curEntry.CalculatedMinimumDelayInMS = Math.Max(Math.Max(originalRecastDelayInMS, spellTemplate.AuraDuration.GetBuffDurationForLevel(creatureTemplate.Level)), Convert.ToInt32(spellTemplate.RecoveryTimeInMS));
                    curEntry.BuffDurationInMS = spellTemplate.AuraDuration.GetBuffDurationForLevel(creatureTemplate.Level);
                    allValidSpellEntries[i] = curEntry;
                }

                // Put all of the spells in their appropriate buckets
                foreach (CreatureSpellEntry spellEntry in allValidSpellEntries)
                {
                    // Skip any that aren't in the right level band
                    if (spellEntry.MinLevel > creatureTemplate.Level || spellEntry.MaxLevel < creatureTemplate.Level)
                        continue;

                    bool addedToList = false;
                    if ((spellEntry.TypeFlags & 8) == 8) // Buff
                    {
                        if (spellEntry.BuffDurationInMS >= Configuration.CREATURE_SPELL_OOC_BUFF_MIN_DURATION_IN_MS)
                        {
                            creatureTemplate.CreatureSpellEntriesOutOfCombatBuff.Add(spellEntry);
                            addedToList = true;
                        }
                    }
                    else if ((spellEntry.TypeFlags & 2) == 2) // Heal
                    {
                        // Keep the strongest direct heal
                        if (creatureTemplate.CreatureSpellEntriesHeal.Count == 0)
                            creatureTemplate.CreatureSpellEntriesHeal.Add(spellEntry);
                        else
                        {
                            int mappedHealAmount = spellTemplatesByEQID[creatureTemplate.CreatureSpellEntriesHeal[0].EQSpellID].HighestDirectHealAmountInSpellEffect;
                            int candidateHealAmount = spellTemplatesByEQID[spellEntry.EQSpellID].HighestDirectHealAmountInSpellEffect;
                            if (candidateHealAmount > mappedHealAmount)
                                creatureTemplate.CreatureSpellEntriesHeal[0] = spellEntry;
                        }

                        addedToList = true;
                    }

                    if (addedToList == false)
                        creatureTemplate.CreatureSpellEntriesCombat.Add(spellEntry);
                }

                // Mark if it'll have any smart scripts
                if (creatureTemplate.AttackEQSpellIDAndProcChance.Count > 0 || creatureTemplate.CreatureSpellEntriesCombat.Count > 0 ||
                        creatureTemplate.CreatureSpellEntriesHeal.Count > 0 || creatureTemplate.CreatureSpellEntriesOutOfCombatBuff.Count > 0)
                    creatureTemplate.HasSmartScript = true;

                // Sort then set recasts (where needed) to make sure it cycles
                creatureTemplate.CreatureSpellEntriesCombat.Sort();
                creatureTemplate.CreatureSpellEntriesOutOfCombatBuff.Sort();
            }

            Logger.WriteDebug("Converting Creature Spell AI complete.");
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
                int itemGroupID = 1;
                List<ItemTemplate> heldMeleeItemTemplatesThatAlwaysDrop = new List<ItemTemplate>();
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
                            if (itemDropEntry.Chance == 100 && curItemTemplate.IsHeldInHandsNonRanged() == true && creatureTemplate.Race.CanHoldVisualItems == true)
                                heldMeleeItemTemplatesThatAlwaysDrop.Add(curItemTemplate);

                            // Items that have class specific copies need to be added for each copy, otherwise just one
                            if (curItemTemplate.ClassSpecificItemVersionsByWOWItemTemplateID.Count > 0)
                            {
                                foreach (var classSpecificItem in curItemTemplate.ClassSpecificItemVersionsByWOWItemTemplateID)
                                {
                                    ItemLootTemplate newItemLootTemplate = new ItemLootTemplate();
                                    newItemLootTemplate.CreatureTemplateEntryID = creatureTemplate.WOWCreatureTemplateID;
                                    newItemLootTemplate.ItemTemplateEntryID = classSpecificItem.Value;
                                    newItemLootTemplate.Chance = itemDropEntry.Chance;
                                    newItemLootTemplate.Comment = string.Concat(creatureTemplate.Name, " - ", curItemTemplate.Name, " (", classSpecificItem.Key.ToString(), ")");
                                    newItemLootTemplate.GroupID = itemGroupID;
                                    newItemLootTemplate.MinCount = Math.Max(lootTableEntry.MinDrop, 1);
                                    newItemLootTemplate.MaxCount = Math.Max(lootTableEntry.MinDrop, 1);
                                    itemLootTemplates.Add(newItemLootTemplate);
                                }
                            }
                            else
                            {                                
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
                itemTemplate.ItemDisplayInfo = ItemDisplayInfo.CreateItemDisplayInfo(itemTemplate.EQItemDisplayFileName, iconName,
                    itemTemplate.InventoryType, itemTemplate.EQArmorMaterialType, itemTemplate.ColorPacked, ItemEquipUnitType.Player);
                if (itemTemplate.WOWEntryIDForCreatureEquip != 0)
                {
                    itemTemplate.ItemDisplayInfoForCreatureEquip = ItemDisplayInfo.CreateItemDisplayInfo(itemTemplate.EQItemDisplayFileName, iconName,
                        itemTemplate.InventoryType, itemTemplate.EQArmorMaterialType, itemTemplate.ColorPacked, ItemEquipUnitType.Creature);
                }
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

        public void GenerateSpells(out List<SpellTemplate> spellTemplates, SortedDictionary<int, ItemTemplate> itemTemplatesByEQDBID, 
            ref Dictionary<int, CreatureTemplate> creatureTemplatesByEQID)
        {
            Logger.WriteInfo("Generating spells...");

            // Create the spell visual data
            SpellVisual.GenerateWOWSpellVisualData();

            // Load spell templates
            SpellTemplate.LoadSpellTemplates(itemTemplatesByEQDBID, ZoneProperties.GetZonePropertyListByShortName(), ref creatureTemplatesByEQID);
            spellTemplates = SpellTemplate.GetSpellTemplatesByEQID().Values.ToList();

            // Add any custom spells
            GenerateCustomSpells(ref spellTemplates);

            // Generate item-bound spells
            foreach (ItemTemplate itemTemplate in itemTemplatesByEQDBID.Values)
            {
                // Generate the poison weapon enchantment spells
                if (itemTemplate.WOWProcEnchantEffectIDEQ > 0 && itemTemplate.WOWProcEnchantSpellIDWOW > 0)
                {
                    SpellTemplate? enchantSpellTemplate;
                    SpellTemplate.GenerateItemEnchantSpellIfNotCreated(itemTemplate.Name, itemTemplate.WOWProcEnchantEffectIDEQ, itemTemplate.WOWProcEnchantSpellIDWOW, out enchantSpellTemplate);
                    if (enchantSpellTemplate != null)
                        spellTemplates.Add(enchantSpellTemplate);
                }

                // Generate focus items
                if (itemTemplate.FocusType != ItemFocusType.None && itemTemplate.FocusValue > 0)
                {
                    SpellTemplate? enchantSpellTemplate;
                    bool isNewSpell;
                    SpellTemplate.GenerateFocusSpellIfNotCreated(itemTemplate.Name, itemTemplate.IconID, itemTemplate.FocusType, itemTemplate.FocusValue, out enchantSpellTemplate, out isNewSpell);
                    if (enchantSpellTemplate != null)
                    {
                        if (itemTemplate.WOWSpellID1 > 0)
                        {
                            Logger.WriteError("Attempted to add focus spell effect to item with WOWID of ", itemTemplate.WOWEntryID.ToString(), " but it already had a WOWSpellID1"); ;
                            continue;
                        }
                        itemTemplate.WOWSpellID1 = enchantSpellTemplate.WOWSpellID;
                        itemTemplate.WOWSpellTrigger1 = 1; // On Equip
                        itemTemplate.WOWSpellPPMRate1 = 0;
                        itemTemplate.WOWSpellCharges1 = 0; // Unlimited
                        itemTemplate.WOWSpellCooldown1 = -1; // Use spell's default
                        itemTemplate.WOWSpellCategory1 = 0; // No category (no shared)
                        itemTemplate.WOWSpellCategoryCooldown1 = -1; // Default
                        if (isNewSpell)
                            spellTemplates.Add(enchantSpellTemplate);
                    }
                }
            }

            // Copy the spell sounds
            string inputSoundFolder = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "sounds");
            string exportMPQRootFolder = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "MPQReady");
            string outputSpellSoundFolder = Path.Combine(exportMPQRootFolder, "Sound", "Spells", "Everquest");
            if (Directory.Exists(outputSpellSoundFolder) == true)
                Directory.Delete(outputSpellSoundFolder, true);
            FileTool.CreateBlankDirectory(outputSpellSoundFolder, true);
            foreach (Sound sound in SpellVisual.SoundsByFileNameNoExt.Values)
            {
                string sourceFullPath = Path.Combine(inputSoundFolder, string.Concat(sound.AudioFileNameNoExt, ".wav"));
                string targetFullPath = Path.Combine(outputSpellSoundFolder, string.Concat(sound.AudioFileNameNoExt, ".wav"));
                FileTool.CopyFile(sourceFullPath, targetFullPath);
            }

            // Output the objects & textures
            List<string> sourceTextureFolders = new List<string>();
            sourceTextureFolders.Add(Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "spellspritesheets"));
            sourceTextureFolders.Add(Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "equipment", "Textures"));
            string outputObjectFolder = Path.Combine(exportMPQRootFolder, "SPELLS", "Everquest");
            if (Directory.Exists(outputObjectFolder) == true)
                Directory.Delete(outputObjectFolder, true);
            foreach (ObjectModel objectModel in SpellVisual.GetAllEmitterObjectModels())
            {
                // Write the M2 and Skin
                string relativeMPQPath = Path.Combine("SPELLS", "Everquest", objectModel.Name);
                M2 objectM2 = new M2(objectModel, relativeMPQPath);
                string outputFolder = Path.Combine(exportMPQRootFolder, relativeMPQPath);
                objectM2.WriteToDisk(objectModel.Name, outputFolder);

                // Output the textures
                ExportTexturesForObject(objectModel, sourceTextureFolders, outputFolder);
            }

            Logger.WriteDebug("Generating spells complete.");
        }

        public void GenerateCustomSpells(ref List<SpellTemplate> spellTemplates)
        {
            // Custom Gate
            SpellTemplate gateSpellTemplate = new SpellTemplate();
            gateSpellTemplate.Name = "Gate";
            gateSpellTemplate.WOWSpellID = Configuration.SPELLS_GATECUSTOM_SPELLDBC_ID;
            gateSpellTemplate.EQSpellID = SpellTemplate.GenerateUniqueEQSpellID();
            gateSpellTemplate.Description = "Opens a magical portal that returns you to your bind point in Norrath.";
            if (Configuration.SPELLS_GATE_TETHER_ENABLED == true)
            {
                gateSpellTemplate.Description = string.Concat(gateSpellTemplate.Description, " You will have 30 minutes where you can return to your gate point after casting it.");
                gateSpellTemplate.AuraDescription = "You are tethered to the location where you gated. Click off before the buff wears off to return there. The tether will fail if you attempt return while in combat.";
                gateSpellTemplate.AuraDuration.SetFixedDuration(1800000); // 30 minutes
                gateSpellTemplate.WOWSpellEffects.Add(new SpellEffectWOW(SpellWOWEffectType.ApplyAura, SpellWOWAuraType.Dummy, 0, 0, 0, 0, (int)SpellDummyType.Gate, 0));
            }
            else
                gateSpellTemplate.WOWSpellEffects.Add(new SpellEffectWOW(SpellWOWEffectType.Dummy, SpellWOWAuraType.None, 0, 0, 0, 0, (int)SpellDummyType.Gate, 0));
            gateSpellTemplate.SpellIconID = SpellIconDBC.GetDBCIDForSpellIconID(22);
            gateSpellTemplate.CastTimeInMS = 5000;
            gateSpellTemplate.RecoveryTimeInMS = 8000;
            gateSpellTemplate.WOWSpellEffects[0].ImplicitTargetA = SpellWOWTargetType.UnitCaster;
            gateSpellTemplate.SpellVisualID1 = Convert.ToUInt32(SpellVisual.GetSpellVisual(9, SpellVisualType.Beneficial).SpellVisualDBCID); // Gate
            gateSpellTemplate.PlayerLearnableByClassTrainer = true;
            gateSpellTemplate.AllowCastInCombat = false;
            gateSpellTemplate.SkillLine = Configuration.DBCID_SKILLLINE_ALTERATION_ID;
            spellTemplates.Add(gateSpellTemplate);

            // Custom Bind Affinity (Self)
            SpellTemplate bindAffinitySelfSpellTemplate = new SpellTemplate();
            bindAffinitySelfSpellTemplate.Name = "Bind Affinity (Self)";
            bindAffinitySelfSpellTemplate.WOWSpellID = Configuration.SPELLS_BINDCUSTOM_SPELLDBC_ID;
            bindAffinitySelfSpellTemplate.EQSpellID = SpellTemplate.GenerateUniqueEQSpellID();
            bindAffinitySelfSpellTemplate.Description = "Binds the soul of the caster to their current location. Only works in Norrath.";
            bindAffinitySelfSpellTemplate.SpellIconID = SpellIconDBC.GetDBCIDForSpellIconID(21);
            bindAffinitySelfSpellTemplate.CastTimeInMS = 6000;
            bindAffinitySelfSpellTemplate.RecoveryTimeInMS = 12000;
            bindAffinitySelfSpellTemplate.SpellVisualID1 = Convert.ToUInt32(SpellVisual.GetSpellVisual(14, SpellVisualType.Beneficial).SpellVisualDBCID); // Bind
            bindAffinitySelfSpellTemplate.PlayerLearnableByClassTrainer = true;
            bindAffinitySelfSpellTemplate.AllowCastInCombat = false;
            bindAffinitySelfSpellTemplate.SkillLine = Configuration.DBCID_SKILLLINE_ALTERATION_ID;
            bindAffinitySelfSpellTemplate.WOWSpellEffects.Add(new SpellEffectWOW(SpellWOWEffectType.Dummy, SpellWOWAuraType.Dummy, 0, 0, 0, 0, (int)SpellDummyType.BindSelf, 0));
            bindAffinitySelfSpellTemplate.WOWSpellEffects[0].ImplicitTargetA = SpellWOWTargetType.UnitCaster;
            spellTemplates.Add(bindAffinitySelfSpellTemplate);

            // Phase aura 1 (Day)
            SpellTemplate dayPhaseSpellTemplate = new SpellTemplate();
            dayPhaseSpellTemplate.Name = "EQ Phase Day";
            dayPhaseSpellTemplate.Category = 0;
            dayPhaseSpellTemplate.WOWSpellID = Configuration.SPELLS_DAYPHASE_SPELLDBC_ID;
            dayPhaseSpellTemplate.EQSpellID = SpellTemplate.GenerateUniqueEQSpellID();
            dayPhaseSpellTemplate.Description = "Able to see day EQ creatures";
            dayPhaseSpellTemplate.SpellIconID = 253;
            dayPhaseSpellTemplate.WOWSpellEffects.Add(new SpellEffectWOW(SpellWOWEffectType.ApplyAura, SpellWOWAuraType.Phase, 0, 0, 1, -1, 2, 0));
            dayPhaseSpellTemplate.SchoolMask = 1;
            dayPhaseSpellTemplate.SkillLine = Configuration.DBCID_SKILLLINE_ALTERATION_ID;
            dayPhaseSpellTemplate.AuraDuration.IsInfinite = true;
            spellTemplates.Add(dayPhaseSpellTemplate);

            // Phase aura 2 (Night)
            SpellTemplate nightPhaseSpellTemplate = new SpellTemplate();
            nightPhaseSpellTemplate.Name = "EQ Phase Day";
            nightPhaseSpellTemplate.Category = 0;
            nightPhaseSpellTemplate.WOWSpellID = Configuration.SPELLS_NIGHTPHASE_SPELLDBC_ID;
            nightPhaseSpellTemplate.EQSpellID = SpellTemplate.GenerateUniqueEQSpellID();
            nightPhaseSpellTemplate.Description = "Able to see night EQ creatures";
            nightPhaseSpellTemplate.SpellIconID = 253;
            nightPhaseSpellTemplate.WOWSpellEffects.Add(new SpellEffectWOW(SpellWOWEffectType.ApplyAura, SpellWOWAuraType.Phase, 0, 0, 1, -1, 4, 0));
            nightPhaseSpellTemplate.SchoolMask = 1;
            nightPhaseSpellTemplate.SkillLine = Configuration.DBCID_SKILLLINE_ALTERATION_ID;
            nightPhaseSpellTemplate.AuraDuration.IsInfinite = true;
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
                        if (curItemTemplate.WOWSpellID1 != 0)
                        {
                            Logger.WriteError(string.Concat("Unable to attach spell to combiner item ", itemID, " as that item already had a spell attached"));
                            continue;
                        }
                        curItemTemplate.WOWSpellID1 = curSpellTemplate.WOWSpellID;
                        curItemTemplate.Description = recipe.GetGeneratedDescription(itemTemplatesByWOWEntryID);

                        // These can't be containers anymore
                        curItemTemplate.ClassID = 12; // Quest
                        curItemTemplate.SubClassID = 12; // Quest
                        curItemTemplate.InventoryType = ItemWOWInventoryType.NoEquip;
                        curItemTemplate.WOWSpellCooldown1 = 1;
                        curItemTemplate.WOWSpellCategoryCooldown1 = 1000;
                        curItemTemplate.WOWItemMaterialType = -1;
                    }
                }
                else
                    curSpellTemplate.Name = recipe.Name;

                // Create the produced item
                ItemTemplate? resultItemTemplate = null;
                Int32 effectDieSides = 0;
                Int32 effectBasePoints = 0;
                if (recipe.ProducedItemCountsByWOWItemID.Count == 0)
                {
                    Logger.WriteError(string.Concat("Could not convert item template with id ", recipe.EQID, " due to there being no produced items"));
                    continue;
                }
                else if (recipe.ProducedItemCountsByWOWItemID.Count > 1)
                {
                    if (recipe.ProducedMultiContainerWOWID <= 0)
                    {
                        Logger.WriteError("Recipe ", recipe.EQID.ToString(), " had multiple produced items but no produced_multi_container_wowid, skipping.");
                        continue;
                    }
                    string containerName = string.Concat(recipe.Name, " Items");
                    resultItemTemplate = ItemTemplate.CreateMultiItemTradeskillContainer(containerName, recipe.ProducedItemCountsByWOWItemID, recipe.ProducedMultiContainerWOWID);
                    recipe.ProducedFilledContainer = resultItemTemplate;
                }
                else
                {
                    resultItemTemplate = itemTemplatesByWOWEntryID[recipe.ProducedItemCountsByWOWItemID.Keys.First()];
                    effectDieSides = 1;
                    effectBasePoints = recipe.ProducedItemCountsByWOWItemID.Values.First() - 1;
                }
                if (resultItemTemplate == null)
                {
                    Logger.WriteError(string.Concat("Could not convert item template with id ", recipe.EQID, " as the result item template is NULL"));
                    continue;
                }

                // Assign every component item as reagents
                foreach (var item in recipe.ComponentItemCountsByWOWItemID)
                {
                    int count = item.Value;
                    int wowItemID = item.Key;

                    // For class-specific item variations, replace the added reagent with one that is aligned to the output, or the first
                    ItemTemplate curItemTemplate = itemTemplatesByWOWEntryID[wowItemID];
                    if (curItemTemplate.ClassSpecificItemVersionsByWOWItemTemplateID.Count > 0)
                    {
                        ItemTemplate firstProducedItemTemplate = itemTemplatesByWOWEntryID[recipe.ProducedItemCountsByWOWItemID.Keys.First()];
                        bool matchFound = false;
                        int parentItemID = wowItemID;
                        foreach (var classSpecificItem in curItemTemplate.ClassSpecificItemVersionsByWOWItemTemplateID)
                        {
                            if (firstProducedItemTemplate.AllowedClassTypes.Contains(classSpecificItem.Key) == true)
                            {
                                wowItemID = classSpecificItem.Value;
                                matchFound = true;
                                break;
                            }
                        }
                        if (matchFound == false)
                            wowItemID = curItemTemplate.ClassSpecificItemVersionsByWOWItemTemplateID.First().Value;
                        SpellTemplate.Reagent newReagent = new SpellTemplate.Reagent(wowItemID, count);
                        newReagent.ParentWOWItemTemplateEntryID = parentItemID;
                        curSpellTemplate.Reagents.Add(newReagent);
                    }
                    else
                    {
                        if (curItemTemplate.NonEssenceWOWEntryID != 0)
                            curSpellTemplate.Reagents.Add(new SpellTemplate.Reagent(curItemTemplate.NonEssenceWOWEntryID, count));
                        else
                            curSpellTemplate.Reagents.Add(new SpellTemplate.Reagent(wowItemID, count));
                    }
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
                curSpellTemplate.WOWSpellEffects.Add(new SpellEffectWOW(SpellWOWEffectType.CreateItem, SpellWOWAuraType.None, 0, Convert.ToUInt32(resultItemTemplate.WOWEntryID), effectDieSides, effectBasePoints, 0, 0));
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

        public void ExtractMinimapMD5TranslateFile()
        {
            string wowExportPath = Configuration.PATH_EXPORT_FOLDER;

            Logger.WriteInfo("Extracting client MD5 Translate file for Minimap...");

            // Make sure the patches folder is correct
            string wowPatchesFolderRoot = Path.Combine(Configuration.PATH_WORLDOFWARCRAFT_CLIENT_INSTALL_FOLDER, "Data");
            if (Directory.Exists(wowPatchesFolderRoot) == false)
                throw new Exception("WoW client patches folder does not exist at '" + wowPatchesFolderRoot + "', did you set PATH_WORLDOFWARCRAFT_CLIENT_INSTALL_FOLDER?");
            string wowPatchesFolderLoc = Path.Combine(wowPatchesFolderRoot, "enUS");

            // Get a list of valid patch files (it's done this way to ensure sorting order is exactly right). Also ignore existing patch file
            List<string> patchFileNames = new List<string>();
            patchFileNames.Add(Path.Combine(wowPatchesFolderLoc, "patch-enUS.MPQ"));
            string[] existingPatchFiles = Directory.GetFiles(wowPatchesFolderLoc, "patch-*-*.MPQ");
            foreach (string existingPatchName in existingPatchFiles)
                if (existingPatchName.Contains(Configuration.PATH_CLIENT_PATCH_LOC_FILE_NAME_NO_EXT) == false && existingPatchName.Contains(Configuration.PATH_CLIENT_PATCH_LOC_FILE_NAME_NO_EXT) == false)
                    patchFileNames.Add(existingPatchName);
            patchFileNames.Add(Path.Combine(wowPatchesFolderRoot, "patch.MPQ"));
            existingPatchFiles = Directory.GetFiles(wowPatchesFolderRoot, "patch-*.MPQ");
            foreach (string existingPatchName in existingPatchFiles)
                if (existingPatchName.Contains(Configuration.PATH_CLIENT_PATCH_LOC_FILE_NAME_NO_EXT) == false && existingPatchName.Contains(Configuration.PATH_CLIENT_PATCH_LOC_FILE_NAME_NO_EXT) == false)
                    patchFileNames.Add(existingPatchName);

            // Make sure all of the files are not locked
            foreach (string patchFileName in patchFileNames)
                if (FileTool.IsFileLocked(patchFileName))
                    throw new Exception("Patch file named '" + patchFileName + "' was locked and in use by another application");

            // Clear out any previously extracted minimap files
            Logger.WriteDebug("Deleting previously extracted MD5 Translate file");
            string exportedMD5Folder = Path.Combine(wowExportPath, "ExportedMD5File");
            FileTool.CreateBlankDirectory(exportedMD5Folder, false);

            // Generate a script to extract the MD5 file
            Logger.WriteDebug("Generating script to extract MD5 file");
            string workingGeneratedScriptsFolder = Path.Combine(wowExportPath, "GeneratedWorkingScripts");
            FileTool.CreateBlankDirectory(workingGeneratedScriptsFolder, true);
            StringBuilder md5ExtractScriptText = new StringBuilder();
            foreach (string patchFileName in patchFileNames)
                md5ExtractScriptText.AppendLine("extract \"" + patchFileName + "\" textures\\minimap\\md5translate.trs \"" + exportedMD5Folder + "\"");
            string md5ExtractionScriptFileName = Path.Combine(workingGeneratedScriptsFolder, "md5extract.txt");
            using (var md5ExtractionScriptFile = new StreamWriter(md5ExtractionScriptFileName))
                md5ExtractionScriptFile.WriteLine(md5ExtractScriptText.ToString());

            // Extract the file using the script
            Logger.WriteDebug("Extracting MD5 file");
            string mpqEditorFullPath = Path.Combine(Configuration.PATH_TOOLS_FOLDER, "ladikmpqeditor", "MPQEditor.exe");
            if (File.Exists(mpqEditorFullPath) == false)
                throw new Exception("Failed to extract MD5 file. '" + mpqEditorFullPath + "' does not exist. (Be sure to set your Configuration.PATH_TOOLS_FOLDER properly)");
            string args = "console \"" + md5ExtractionScriptFileName + "\"";
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.Arguments = args;
            process.StartInfo.FileName = mpqEditorFullPath;
            process.Start();
            process.WaitForExit();

            Logger.WriteDebug("Extracting client MD5 Translate file complete");
        }

        public void CreateMinimapPatchMPQ()
        {
            //Logger.WriteInfo("Building minimap patch MPQ...");

            //// Make sure the output folder exists
            //if (Directory.Exists(Configuration.PATH_EXPORT_FOLDER) == false)
            //    throw new Exception("Export folder '" + Configuration.PATH_EXPORT_FOLDER + "' did not exist, make sure you set PATH_EXPORT_FOLDER");

            //// Delete the old patch file, if it exists
            //Logger.WriteDebug("Deleting old minimap patch file if it exists");
            //string outputPatchFileName = Path.Combine(Configuration.PATH_EXPORT_FOLDER, Configuration.PATH_CLIENT_PATCH_FILE_NAME_NO_EXT + ".MPQ");
            //if (File.Exists(outputPatchFileName) == true)
            //    File.Delete(outputPatchFileName);

            //TODO: HERE




            //Logger.WriteDebug("Building minimap patch MPQ complete");
        }

        public void CreateMainPatchMPQ()
        {
            Logger.WriteInfo("Building main patch MPQ...");

            // Make sure the output folder exists
            if (Directory.Exists(Configuration.PATH_EXPORT_FOLDER) == false)
                throw new Exception("Export folder '" + Configuration.PATH_EXPORT_FOLDER + "' did not exist, make sure you set PATH_EXPORT_FOLDER");

            // Delete the old patch file, if it exists
            Logger.WriteDebug("Deleting old patch file if it exists");
            string outputPatchFileName = Path.Combine(Configuration.PATH_EXPORT_FOLDER, Configuration.PATH_CLIENT_PATCH_LOC_FILE_NAME_NO_EXT + ".MPQ");
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

            Logger.WriteDebug("Building main patch MPQ complete");
        }

        public void UpdateMainPatchMPQ()
        {
            Logger.WriteInfo("Updating main patch MPQ...");
            string exportMPQFileName = Path.Combine(Configuration.PATH_EXPORT_FOLDER, Configuration.PATH_CLIENT_PATCH_LOC_FILE_NAME_NO_EXT + ".mpq");
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

            // Spell Sounds
            string relativeSpellSoundsPath = Path.Combine("Sound", "Spells", "Everquest");
            string fullSpellSoundsPath = Path.Combine(mpqReadyFolder, relativeSpellSoundsPath);
            mpqUpdateScriptText.AppendLine("add \"" + exportMPQFileName + "\" \"" + fullSpellSoundsPath + "\" \"" + relativeSpellSoundsPath + "\" /r");

            // Spell Particle Emitters
            string relativeSpellEmittersPath = Path.Combine("SPELLS", "Everquest");
            string fullSpellEmittersPath = Path.Combine(mpqReadyFolder, relativeSpellEmittersPath);
            mpqUpdateScriptText.AppendLine("add \"" + exportMPQFileName + "\" \"" + fullSpellEmittersPath + "\" \"" + relativeSpellEmittersPath + "\" /r");

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

            Logger.WriteDebug("Building main patch MPQ complete");
        }

        public void DeployClient()
        {
            Logger.WriteInfo("Deploying to client...");

            // Make sure a patch was created
            string sourcePatchFileNameAndPath = Path.Combine(Configuration.PATH_EXPORT_FOLDER, Configuration.PATH_CLIENT_PATCH_LOC_FILE_NAME_NO_EXT + ".MPQ");
            if (File.Exists(sourcePatchFileNameAndPath) == false)
            {
                Logger.WriteError("Failed to deploy to client. Patch at '" + sourcePatchFileNameAndPath + "' did not exist");
                return;
            }

            // Delete the old one if it's already deployed on the client
            string targetPatchFileNameAndPath = Path.Combine(Configuration.PATH_WORLDOFWARCRAFT_CLIENT_INSTALL_FOLDER, "Data", "enUS", Configuration.PATH_CLIENT_PATCH_LOC_FILE_NAME_NO_EXT + ".MPQ");
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
            string folderToDelete = Path.Combine(Configuration.PATH_WORLDOFWARCRAFT_CLIENT_INSTALL_FOLDER, "Cache", "WDB");
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
            if (Directory.Exists(Configuration.DEPLOY_SERVER_DBC_FOLDER_LOCATION) == false)
            {
                Logger.WriteError("Could not deploy DBC files to the server, no target folder existed at '" + Configuration.DEPLOY_SERVER_DBC_FOLDER_LOCATION + "'. Check that you set Configuration.PATH_DEPLOY_SERVER_DBC_FILES_FOLDER properly");
                return;
            }

            // Deploy the DBC files
            Logger.WriteDebug("Deploying DBC files to the server...");
            string[] dbcFiles = Directory.GetFiles(sourceServerDBCFolder);
            foreach (string dbcFile in dbcFiles)
            {
                string targetFileName = Path.Combine(Configuration.DEPLOY_SERVER_DBC_FOLDER_LOCATION, Path.GetFileName(dbcFile));
                FileTool.CopyFile(dbcFile, targetFileName);
            }


            Logger.WriteDebug("Deploying files to server complete");
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

        public void AssignItemSpellEffects(ref SortedDictionary<int, ItemTemplate> itemTemplatesByWOWEntryID)
        {
            Logger.WriteInfo("Assigning item spell effects...");

            // Attach any spells to 
            Dictionary<int, SpellTemplate> spellTemplatesByEQID = SpellTemplate.GetSpellTemplatesByEQID();

            foreach(ItemTemplate itemTemplate in itemTemplatesByWOWEntryID.Values)
            {
                // Rogue Poison Weapon Enchants
                if (itemTemplate.EQClickType == 6)
                {
                    if (spellTemplatesByEQID.ContainsKey(itemTemplate.WOWProcEnchantEffectIDEQ) == false)
                        Logger.WriteDebug("Could not map spell with eqid ", itemTemplate.WOWProcEnchantEffectIDEQ.ToString(), " to item ", itemTemplate.Name, " (", itemTemplate.WOWEntryID.ToString(), ") as the spell didn't exist");
                    else
                    {
                        itemTemplate.WOWSpellID1 = itemTemplate.WOWProcEnchantSpellIDWOW;
                        itemTemplate.WOWSpellTrigger1 = 0; // Use (click)
                        itemTemplate.WOWSpellCharges1 = itemTemplate.MaxCharges *= -1; // *= -1 makes it expendable
                        itemTemplate.WOWSpellCooldown1 = -1; // Use spell's default
                        itemTemplate.WOWSpellCategory1 = 0; // No category (no shared)
                        itemTemplate.WOWSpellCategoryCooldown1 = -1; // Default
                    }
                }

                // Worn
                else if (itemTemplate.EQWornEffectSpellID > 0)
                {
                    if (spellTemplatesByEQID.ContainsKey(itemTemplate.EQWornEffectSpellID) == false)
                        Logger.WriteDebug("Could not map spell with eqid ", itemTemplate.EQWornEffectSpellID.ToString(), " to item ", itemTemplate.Name, " (", itemTemplate.WOWEntryID.ToString(), ") as the spell didn't exist");
                    else
                    {
                        SpellTemplate curSpellTemplate = spellTemplatesByEQID[itemTemplate.EQWornEffectSpellID];
                        if (curSpellTemplate.WOWSpellIDWorn <= 0)
                            Logger.WriteError("Could not map spell with eqid ", itemTemplate.EQWornEffectSpellID.ToString(), " to item ", itemTemplate.Name, " (", itemTemplate.WOWEntryID.ToString(), ") as the spell did not have a value in 'wow_worn-id'");
                        else
                        {
                            itemTemplate.WOWSpellID1 = curSpellTemplate.WOWSpellIDWorn;
                            itemTemplate.WOWSpellTrigger1 = 1; // On Equip
                            itemTemplate.WOWSpellPPMRate1 = 0;
                            itemTemplate.WOWSpellCharges1 = 0; // Unlimited
                            itemTemplate.WOWSpellCooldown1 = -1; // Use spell's default
                            itemTemplate.WOWSpellCategory1 = 0; // No category (no shared)
                            itemTemplate.WOWSpellCategoryCooldown1 = -1; // Default
                        }
                    }
                }

                // Proc
                else if (itemTemplate.EQCombatProcSpellEffectID > 0)
                {
                    if (spellTemplatesByEQID.ContainsKey(itemTemplate.EQCombatProcSpellEffectID) == false)
                        Logger.WriteDebug("Could not map spell with eqid ", itemTemplate.EQCombatProcSpellEffectID.ToString(), " to item ", itemTemplate.Name, " (", itemTemplate.WOWEntryID.ToString(), ") as the spell didn't exist");
                    else
                    {
                        // Remap any special type

                        itemTemplate.WOWSpellID1 = spellTemplatesByEQID[itemTemplate.EQCombatProcSpellEffectID].WOWSpellID;
                        itemTemplate.WOWSpellTrigger1 = 2; // Chance on Hit
                        itemTemplate.WOWSpellPPMRate1 = Configuration.ITEMS_WEAPON_EFFECT_PPM_BASE_RATE; // TODO: Make this varied?
                        itemTemplate.WOWSpellCharges1 = 0; // Unlimited
                        itemTemplate.WOWSpellCooldown1 = -1; // Use spell's default
                        itemTemplate.WOWSpellCategory1 = 0; // No category (no shared)
                        itemTemplate.WOWSpellCategoryCooldown1 = -1; // Default
                    }
                }

                // Clicky
                else if (itemTemplate.EQClickSpellEffectID > 0)
                {
                    if (spellTemplatesByEQID.ContainsKey(itemTemplate.EQClickSpellEffectID) == false)
                        Logger.WriteDebug("Could not map spell with eqid ", itemTemplate.EQClickSpellEffectID.ToString(), " to item ", itemTemplate.Name, " (", itemTemplate.WOWEntryID.ToString(), ") as the spell didn't exist");
                    else
                    {
                        itemTemplate.WOWSpellID1 = spellTemplatesByEQID[itemTemplate.EQClickSpellEffectID].WOWSpellID;
                        itemTemplate.WOWSpellTrigger1 = 0; // Use (click)
                        switch (itemTemplate.EQClickType)
                        {
                            case 4: // Must equip to click
                            case 1: // Clickable from inventory with level requirement (note: Can't make worn gear clickable from inventory)
                            case 5: // Clickable from inventory with level, class, race requirements (nuance not yet implemented)
                                {
                                    itemTemplate.WOWSpellCharges1 = itemTemplate.MaxCharges;
                                    if (itemTemplate.ClassID == 0 && itemTemplate.SubClassID == 1) // Potions are expendable
                                        itemTemplate.WOWSpellCharges1 *= -1;
                                } break;
                            case 3: // Expendable
                                {
                                    itemTemplate.WOWSpellCharges1 = itemTemplate.MaxCharges * -1;
                                } break;
                            default:
                                {
                                    Logger.WriteError("Unhandled EQClickType of ", itemTemplate.EQClickType.ToString());
                                } break;
                        }
                        itemTemplate.WOWSpellCooldown1 = -1; // Use spell's default
                        itemTemplate.WOWSpellCategory1 = 0; // No category (no shared)
                        itemTemplate.WOWSpellCategoryCooldown1 = -1; // Default
                    }
                }

                // Remap gate
                if (itemTemplate.WOWSpellID1 == 36)
                    itemTemplate.WOWSpellID1 = Configuration.SPELLS_GATECUSTOM_SPELLDBC_ID;

                // Remap bind
                if (itemTemplate.WOWSpellID1 == 35)
                    itemTemplate.WOWSpellID1 = Configuration.SPELLS_BINDCUSTOM_SPELLDBC_ID;
            }
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

        public void ExportTexturesForObject(ObjectModel wowObjectModelData, List<string> objectTextureInputFolders, string objectExportPath)
        {
            Logger.WriteDebug("- [" + wowObjectModelData.Name + "]: Exporting textures for object '" + wowObjectModelData.Name + "'...");

            // Go through every texture and copy it
            foreach(ObjectModelTexture texture in wowObjectModelData.ModelTextures)
            {
                string outputTextureName = Path.Combine(objectExportPath, texture.TextureName + ".blp");
                string inputTextureName = string.Empty;
                bool inputTextureFound = false;
                foreach (string objectTextureInputFolder in  objectTextureInputFolders)
                {
                    inputTextureName = Path.Combine(objectTextureInputFolder, texture.TextureName + ".blp");
                    if (Path.Exists(inputTextureName) == true)
                    {
                        inputTextureFound = true;
                        break;
                    }
                }
                if (inputTextureFound == false)
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
