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
using EQWOWConverter.ObjectModels;
using EQWOWConverter.ObjectModels.Properties;
using EQWOWConverter.Spells;
using EQWOWConverter.Transports;
using EQWOWConverter.WOWFiles;
using EQWOWConverter.Zones;
using MySql.Data.MySqlClient;
using System.Text;

namespace EQWOWConverter
{
    internal class AssetConverter
    {
        public bool ConvertEQDataToWOW()
        {
            Logger.WriteInfo("Converting from EQ to WoW...");

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

            // Objects (must always come before zones)
            if (Configuration.GENERATE_OBJECTS == true)
            {
                if (ConvertEQObjectsToWOW() == false)
                    return false;
            }
            else
                Logger.WriteInfo("- Note: Object generation is set to false in the Configuration");

            // Zones
            List<Zone> zones;
            if (ConvertEQZonesToWOW(out zones) == false)
                return false;

            // Creatures
            List<CreatureModelTemplate> creatureModelTemplates = new List<CreatureModelTemplate>();
            List<CreatureTemplate> creatureTemplates = new List<CreatureTemplate>();
            List<CreatureSpawnPool> creatureSpawnPools = new List<CreatureSpawnPool>();
            if (Configuration.GENERATE_CREATURES_AND_SPAWNS == true)
            {
                if (ConvertCreatures(ref creatureModelTemplates, ref creatureTemplates, ref creatureSpawnPools, zones) == false)
                    return false;
            }
            else
                Logger.WriteInfo("- Note: Creature generation is set to false in the Configuration");

            // Transports (make sure it's after zones)
            if (Configuration.GENERATE_TRANSPORTS == true)
                ConvertTransports();
            else
                Logger.WriteInfo("- Note: Transport generation is set to false in the Configuration");

            // Icons
            CopyIconFiles();

            // Items
            Dictionary<int, List<ItemLootTemplate>> itemLootTemplatesByCreatureTemplateID;
            ConvertItemsAndLoot(creatureTemplates, out itemLootTemplatesByCreatureTemplateID);

            // Spells
            List<SpellTemplate> spellTemplates;
            GenerateSpells(out spellTemplates);

            // Copy the loading screens
            CreateLoadingScreens();

            // Copy the liquid material textures
            CreateLiquidMaterials();

            // Create the DBC files
            CreateDBCFiles(zones, creatureModelTemplates, spellTemplates);

            // Create the SQL Scripts (note: this must always be after DBC files)
            CreateSQLScript(zones, creatureTemplates, creatureModelTemplates, creatureSpawnPools, itemLootTemplatesByCreatureTemplateID);

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
            if (Configuration.DEPLOY_SERVER_FILES == true)
                DeployServerFiles();
            if (Configuration.DEPLOY_SERVER_SQL == true)
                DeployServerSQL();

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
            Logger.WriteDetail("Loading transport ships...");
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
                    Logger.WriteDetail("- [" + transportShip.MeshName + "]: Importing EQ transport ship object '" + transportShip.MeshName + "'");
                    curZone.LoadFromEQObject(transportShip.MeshName, charactersFolderRoot);
                    Logger.WriteDetail("- [" + transportShip.MeshName + "]: Importing EQ transport ship object '" + transportShip.MeshName + "' complete");

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
                            string sourceTextureFullPath = Path.Combine(charactersFolderRoot, "Textures", textureName + ".blp");
                            string outputTextureFullPath = Path.Combine(transportOutputTextureFolder, textureName + ".blp");
                            if (File.Exists(sourceTextureFullPath) == false)
                            {
                                Logger.WriteError("Could not copy texture '" + sourceTextureFullPath + "', it did not exist. Did you run blpconverter?");
                                continue;
                            }
                            File.Copy(sourceTextureFullPath, outputTextureFullPath, true);
                            Logger.WriteDetail("- [" + transportShip.Name + "]: Texture named '" + textureName + "' copied");
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
            Logger.WriteDetail("Loading transport lifts...");
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
                    if (transportLift.TriggerType == TransportLiftTriggerType.Automatic)
                        folderRoot = charactersFolderRoot;

                    // Load it
                    ObjectModel curObjectModel = new ObjectModel(transportLift.MeshName, new ObjectModelProperties(), ObjectModelType.StaticDoodad);
                    Logger.WriteDetail("- [" + transportLift.MeshName + "]: Importing EQ transport lift object '" + transportLift.MeshName + "'");
                    curObjectModel.LoadEQObjectFromFile(folderRoot, null, transportLift.MeshName);
                    Logger.WriteDetail("- [" + transportLift.MeshName + "]: Importing EQ transport lift object '" + transportLift.MeshName + "' complete");

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
            Logger.WriteDetail("Loading transport lift triggers...");
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
                    ObjectModel curObjectModel = new ObjectModel(transportLiftTrigger.MeshName, new ObjectModelProperties(), ObjectModelType.StaticDoodad);
                    Logger.WriteDetail("- [" + transportLiftTrigger.MeshName + "]: Importing EQ transport lift trigger object '" + transportLiftTrigger.MeshName + "'");
                    curObjectModel.LoadEQObjectFromFile(objectsFolderRoot, null, transportLiftTrigger.MeshName, transportLiftTrigger.AnimationType, transportLiftTrigger.AnimMod, transportLiftTrigger.AnimTimeInMS);
                    Logger.WriteDetail("- [" + transportLiftTrigger.MeshName + "]: Importing EQ transport lift trigger object '" + transportLiftTrigger.MeshName + "' complete");
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
                string sourceFullPath = Path.Combine(inputSoundFolder, sound.AudioFileNameNoExt + ".wav");
                string targetFullPath = Path.Combine(outputLiftSoundFolder, sound.AudioFileNameNoExt + ".wav");
                File.Copy(sourceFullPath, targetFullPath, true);
            }

            Logger.WriteDetail("Converting Transports complete.");
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

            // Clean out the target objects folder
            string exportMPQRootFolder = Path.Combine(wowExportPath, "MPQReady");
            string exportObjectsFolder = Path.Combine(exportMPQRootFolder, "World", "Everquest", "StaticDoodads");
            if (Directory.Exists(exportObjectsFolder))
                Directory.Delete(exportObjectsFolder, true);

            // Generate static EQ objects
            Logger.WriteInfo("Converting static EQ objects...");
            string staticObjectListFileName = Path.Combine(conditionedObjectFolderRoot, "static_objects.txt");
            int curProgress = 0;
            int curProgressOffset = Logger.GetConsolePriorRowCursorLeft();
            List<string> staticObjectList = FileTool.ReadAllStringLinesFromFile(staticObjectListFileName, false, true);
            foreach (string staticObjectName in staticObjectList)
            {
                curProgress++;
                string curObjectOutputFolder = Path.Combine(exportObjectsFolder, staticObjectName);

                // Load the EQ object
                ObjectModelProperties objectProperties = ObjectModelProperties.GetObjectPropertiesForObject(staticObjectName);
                ObjectModel curObject = new ObjectModel(staticObjectName, objectProperties, ObjectModelType.StaticDoodad);
                Logger.WriteDetail("- [" + staticObjectName + "]: Importing EQ static object '" + staticObjectName + "'");
                curObject.LoadEQObjectFromFile(conditionedObjectFolderRoot, null, staticObjectName);
                Logger.WriteDetail("- [" + staticObjectName + "]: Importing EQ static object '" + staticObjectName + "' complete");

                // Create the M2 and Skin
                string relativeMPQPath = Path.Combine("World", "Everquest", "StaticDoodads", staticObjectName);
                M2 objectM2 = new M2(curObject, relativeMPQPath);
                objectM2.WriteToDisk(curObject.Name, curObjectOutputFolder);

                // Place the related textures
                string objectTextureFolder = Path.Combine(conditionedObjectFolderRoot, "textures");
                ExportTexturesForObject(curObject, objectTextureFolder, curObjectOutputFolder);

                // Save it for use elsewhere
                ObjectModel.StaticObjectModelsByName.Add(curObject.Name, curObject);
                Logger.WriteCounter(curProgress, curProgressOffset, staticObjectList.Count);
            }

            // Generate skeletal EQ objects
            Logger.WriteInfo("Converting skeletal EQ objects...");
            string skeletalObjectListFileName = Path.Combine(conditionedObjectFolderRoot, "skeletal_objects.txt");
            curProgress = 0;
            curProgressOffset = Logger.GetConsolePriorRowCursorLeft();
            List<string> skeletalObjectList = FileTool.ReadAllStringLinesFromFile(skeletalObjectListFileName, false, true);
            foreach (string skeletalObjectName in skeletalObjectList)
            {
                curProgress++;
                string curObjectOutputFolder = Path.Combine(exportObjectsFolder, skeletalObjectName);

                // Load the EQ object
                ObjectModelProperties objectProperties = ObjectModelProperties.GetObjectPropertiesForObject(skeletalObjectName);
                ObjectModel curObject = new ObjectModel(skeletalObjectName, objectProperties, ObjectModelType.Skeletal);
                Logger.WriteDetail("- [" + skeletalObjectName + "]: Importing EQ skeletal object '" + skeletalObjectName + "'");
                curObject.LoadEQObjectFromFile(conditionedObjectFolderRoot, null);
                Logger.WriteDetail("- [" + skeletalObjectName + "]: Importing EQ skeletal object '" + skeletalObjectName + "' complete");

                // Create the M2 and Skin
                string relativeMPQPath = Path.Combine("World", "Everquest", "StaticDoodads", skeletalObjectName);
                M2 objectM2 = new M2(curObject, relativeMPQPath);
                objectM2.WriteToDisk(curObject.Name, curObjectOutputFolder);

                // Place the related textures
                string objectTextureFolder = Path.Combine(conditionedObjectFolderRoot, "textures");
                ExportTexturesForObject(curObject, objectTextureFolder, curObjectOutputFolder);

                // Save it for use elsewhere
                ObjectModel.StaticObjectModelsByName.Add(curObject.Name, curObject);
                Logger.WriteCounter(curProgress, curProgressOffset, skeletalObjectList.Count);
            }

            return true;
        }

        // TODO: Condense above
        public bool ConvertEQZonesToWOW(out List<Zone> zones)
        {
            Logger.WriteInfo("Converting EQ zones to WOW zones...");

            if (Configuration.GENERATE_ONLY_LISTED_ZONE_SHORTNAMES.Count > 0)
            {
                Logger.WriteInfo("- Note: GENERATE_ONLY_LISTED_ZONE_SHORTNAMES has values: ", false);
                foreach (string zoneShortName in Configuration.GENERATE_ONLY_LISTED_ZONE_SHORTNAMES)
                    Logger.WriteInfo(zoneShortName + " ", false, false);
                Logger.WriteInfo(string.Empty, true, false);
            }

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

            // Go through the subfolders for each zone and convert to wow zone
            DirectoryInfo zoneRootDirectoryInfo = new DirectoryInfo(inputZoneFolder);
            DirectoryInfo[] zoneDirectoryInfos = zoneRootDirectoryInfo.GetDirectories();
            zones = new List<Zone>();
            foreach (DirectoryInfo zoneDirectory in zoneDirectoryInfos)
            {
                // Skip any disabled expansions
                if (Configuration.GENERATE_EQ_EXPANSION_ID < 1 && Configuration.ZONE_KUNARK_ZONE_SHORTNAMES.Contains(zoneDirectory.Name))
                    continue;
                if (Configuration.GENERATE_EQ_EXPANSION_ID < 2 && Configuration.ZONE_VELIOUS_ZONE_SHORTNAMES.Contains(zoneDirectory.Name))
                    continue;
                if (Configuration.GENERATE_ONLY_LISTED_ZONE_SHORTNAMES.Count > 0 == true &&
                    Configuration.GENERATE_ONLY_LISTED_ZONE_SHORTNAMES.Contains(zoneDirectory.Name) == false)
                    continue;

                // Grab the zone properties to generate IDs, even for zones not being output
                Logger.WriteInfo(" - Processing zone '" + zoneDirectory.Name + "'");
                string relativeZoneObjectsPath = Path.Combine("World", "Everquest", "ZoneObjects", zoneDirectory.Name);
                ZoneProperties zoneProperties = ZoneProperties.GetZonePropertiesForZone(zoneDirectory.Name);

                // Create only zones configured to do so
                if (Configuration.GENERATE_ONLY_LISTED_ZONE_SHORTNAMES.Count == 0 ||
                    Configuration.GENERATE_ONLY_LISTED_ZONE_SHORTNAMES.Contains(zoneProperties.ShortName))
                {
                    Zone curZone = new Zone(zoneDirectory.Name, zoneProperties);
                    Logger.WriteDetail("- [" + curZone.ShortName + "]: Converting zone '" + curZone.ShortName + "' into a wow zone...");
                    string curZoneDirectory = Path.Combine(inputZoneFolder, zoneDirectory.Name);
                    curZone.LoadFromEQZone(zoneDirectory.Name, curZoneDirectory);

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

                    Logger.WriteDetail("- [" + curZone.ShortName + "]: Converting of zone '" + curZone.ShortName + "' complete");

                    zones.Add(curZone);
                }
                else
                    Logger.WriteDetail("For zone '" + zoneProperties.ShortName + "', skipped wow file generation since it wasn't in GENERATE_UPDATE_INCLUDED_ZONE_SHORTNAMES");
            }
            return true;
        }

        public bool ConvertCreatures(ref List<CreatureModelTemplate> creatureModelTemplates, ref List<CreatureTemplate> creatureTemplates, 
            ref List<CreatureSpawnPool> creatureSpawnPools, List<Zone> zones)
        {
            string eqExportsConditionedPath = Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER;
            string wowExportPath = Configuration.PATH_EXPORT_FOLDER;

            Logger.WriteInfo("Converting EQ Creatures (skeletal objects) to WOW creature objects...");

            if (Configuration.CREATURE_ADD_ENTITY_ID_TO_NAME == true)
                Logger.WriteInfo("- Note: CREATURE_ADD_ENTITY_ID_TO_NAME is set to TRUE");

            // Recreate the output folder to clean it out
            string exportMPQRootFolder = Path.Combine(wowExportPath, "MPQReady");
            string exportAnimatedObjectsFolder = Path.Combine(exportMPQRootFolder, "Creature", "Everquest");
            if (Directory.Exists(exportAnimatedObjectsFolder))
                Directory.Delete(exportAnimatedObjectsFolder, true);
            Directory.CreateDirectory(exportAnimatedObjectsFolder);

            // Generate templates
            Dictionary<int, CreatureTemplate> creatureTemplatesByID = CreatureTemplate.GetCreatureTemplateListByEQID();
            creatureTemplates = creatureTemplatesByID.Values.ToList();

            // Create all of the models and related model files
            Logger.WriteInfo("Creating creature model files...");

            // For the counter
            int curProgress = 0;
            int curProgressOffset = Logger.GetConsolePriorRowCursorLeft();
            
            CreatureModelTemplate.CreateAllCreatureModelTemplates(creatureTemplates);
            foreach (var modelTemplatesByRaceID in CreatureModelTemplate.AllTemplatesByRaceID)
            {
                foreach (CreatureModelTemplate modelTemplate in modelTemplatesByRaceID.Value)
                {
                    modelTemplate.CreateModelFiles();
                    creatureModelTemplates.Add(modelTemplate);
                    curProgress++;
                    Logger.WriteCounter(curProgress, curProgressOffset);
                }
            }

            // Get a list of valid zone names
            Dictionary<string, int> mapIDsByShortName = new Dictionary<string, int>();
            Dictionary<int, Zone> zonesByMapID = new Dictionary<int, Zone>();
            foreach (Zone zone in zones)
            {
                mapIDsByShortName.Add(zone.ShortName.ToLower().Trim(), zone.ZoneProperties.DBCMapID);
                zonesByMapID.Add(zone.ZoneProperties.DBCMapID, zone);
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
                        spawnInstance.AreaID = Convert.ToInt32(zonesByMapID[spawnInstance.MapID].DefaultArea.DBCAreaTableID);
                        curSpawnPool.AddSpawnInstance(spawnInstance);
                    }
                }

                // Add spawns that are valid spawns
                if (creatureSpawnEntriesByGroupID.ContainsKey(spawnGroup.Key))
                {
                    foreach (CreatureSpawnEntry spawnEntry in creatureSpawnEntriesByGroupID[spawnGroup.Key])
                    {
                        if (creatureTemplatesByID.ContainsKey(spawnEntry.EQCreatureTemplateID))
                            curSpawnPool.AddCreatureTemplate(creatureTemplatesByID[spawnEntry.EQCreatureTemplateID], spawnEntry.Chance);
                    }
                }

                // Make sure there is at least one element
                if (curSpawnPool.CreatureSpawnInstances.Count == 0)
                {
                    Logger.WriteDetail("Invalid creature spawn pool with groupID '" + spawnGroup.Key+ "', as there are no creature spawn instances. Skipping.");
                    continue;
                }
                if (curSpawnPool.CreatureTemplates.Count == 0)
                {
                    Logger.WriteDetail("Invalid creature spawn pool with groupID '" + spawnGroup.Key+ "', as there are no valid creature templates. Skipping.");
                    continue;
                }

                // Validate the chances
                if (curSpawnPool.DoChancesAddTo100() == false)
                {
                    Logger.WriteDetail("Invalid creature spawn pool with groupID '" + spawnGroup.Key + "', as chances did not add to 100. Rebalancing.");
                    curSpawnPool.BalanceChancesTo100();
                }

                // Add it
                spawnPoolsByGroupID.Add(spawnGroup.Key, curSpawnPool);
            }
            creatureSpawnPools = spawnPoolsByGroupID.Values.ToList();

            // Make a list of path grid entries
            Dictionary<int, Dictionary<int, List<CreaturePathGridEntry>>> creaturePathGridEntriesByIDAndMapID = new Dictionary<int, Dictionary<int, List<CreaturePathGridEntry>>>();
            foreach (CreaturePathGridEntry creaturePathGridEntry in CreaturePathGridEntry.GetPathGridEntries())
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

            // Sort and renumber grid IDs
            foreach(var pathGridSetByGridID in creaturePathGridEntriesByIDAndMapID)
            {
                foreach (var pathGridSetByMapID in creaturePathGridEntriesByIDAndMapID[pathGridSetByGridID.Key])
                {
                    // Sort the values from smallest to largest
                    pathGridSetByMapID.Value.Sort();

                    // Renumber the elements so that it starts from 1 and incriments by 1
                    int curID = 1;
                    foreach(CreaturePathGridEntry curEntry in pathGridSetByMapID.Value)
                    {
                        curEntry.Number = curID;
                        curID++;
                    }
                }
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
                            Logger.WriteDetail("CreatureSpawnInstance with ID '" + creatureSpawnInstance.ID + "' could not find a PathGridEntry with ID '" + creatureSpawnInstance.PathGridID + "'");
                            continue;
                        }
                        if (creaturePathGridEntriesByIDAndMapID[creatureSpawnInstance.PathGridID].ContainsKey(creatureSpawnInstance.MapID) == false)
                        {
                            Logger.WriteDetail("CreatureSpawnInstance with ID '" + creatureSpawnInstance.ID + "' could not find a PathGridEntry with ID '" + creatureSpawnInstance.PathGridID + "' and mapID of '" + creatureSpawnInstance.MapID + "'");
                            continue;
                        }

                        foreach (CreaturePathGridEntry creaturePathGridEntry in creaturePathGridEntriesByIDAndMapID[creatureSpawnInstance.PathGridID][creatureSpawnInstance.MapID])
                            creatureSpawnInstance.PathGridEntries.Add(creaturePathGridEntry);
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
                    File.Copy(sourceSoundFileName, targetSoundFileName, true);
            }

            Logger.WriteInfo("Creature generation complete.");
            return true;
        }

        public void ConvertItemsAndLoot(List<CreatureTemplate> creatureTemplates, out Dictionary<int, List<ItemLootTemplate>> itemLootTemplatesByCreatureTemplateID)
        {
            Logger.WriteInfo("Converting items and loot...");
            
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
                    Logger.WriteDetail("For creature template '" + creatureTemplate.EQCreatureTemplateID + "' named '" + creatureTemplate.Name + "', lootTableID of '" + creatureTemplate.EQLootTableID + "' was not found");
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
                        Logger.WriteDetail("ItemLootTable with ID '" + lootTableEntry.LootTableID + "' references ItemLootDrop with ID '" + lootTableEntry.LootDropID + "', but it did not exist");
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
                                Logger.WriteDetail("ItemDropEntry with ID '" + itemDropEntry.LootDropID + "' references ItemID of '" + itemDropEntry.ItemIDEQ + "', but it did not exist");
                                continue;
                            }
                            ItemTemplate curItemTemplate = itemTemplatesByEQDBID[itemDropEntry.ItemIDEQ];
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

        public void GenerateSpells(out List<SpellTemplate> spellTemplates)
        {
            Logger.WriteInfo("Generating spells...");
            spellTemplates = new List<SpellTemplate>();

            Logger.WriteDetail("Creating custom spells");
            List<ClassType> casterClassTypes = new List<ClassType> { ClassType.Priest, ClassType.Shaman, ClassType.Mage, ClassType.Druid, ClassType.Warlock };
            List<ClassType> meleeClassTypes = new List<ClassType> { ClassType.Warrior, ClassType.Paladin, ClassType.Hunter, ClassType.Rogue, ClassType.DeathKnight };

            // Gate
            SpellTemplate gateSpellTemplate = new SpellTemplate();
            gateSpellTemplate.Name = "Gate";
            gateSpellTemplate.ID = Configuration.SPELLS_GATE_SPELLDBC_ID;
            gateSpellTemplate.Description = "Opens a magical portal that returns you to your bind point in Norrath.";
            gateSpellTemplate.SpellIconID = SpellIconDBC.GetDBCIDForIconID(22);
            gateSpellTemplate.CastTimeInMS = 5000;
            gateSpellTemplate.RecoveryTimeInMS = 8000;
            gateSpellTemplate.TargetType = SpellTargetType.SelfSingle;
            gateSpellTemplate.SpellVisualID1 = 220; // Taken from astral recall / hearthstone
            gateSpellTemplate.PlayerLearnableByClassTrainer = true;
            spellTemplates.Add(gateSpellTemplate);            

            // Bind Affinity (Self)
            SpellTemplate bindAffinitySelfSpellTemplate = new SpellTemplate();
            bindAffinitySelfSpellTemplate.Name = "Bind Affinity (Self)";
            bindAffinitySelfSpellTemplate.ID = Configuration.SPELLS_BINDSELF_SPELLDBC_ID;
            bindAffinitySelfSpellTemplate.Description = "Binds the soul of the caster to their current location. Only works in Norrath.";
            bindAffinitySelfSpellTemplate.SpellIconID = SpellIconDBC.GetDBCIDForIconID(21);
            bindAffinitySelfSpellTemplate.CastTimeInMS = 6000;
            bindAffinitySelfSpellTemplate.RecoveryTimeInMS = 12000;
            bindAffinitySelfSpellTemplate.TargetType = SpellTargetType.SelfSingle;
            bindAffinitySelfSpellTemplate.SpellVisualID1 = 99; // Taken from soulstone
            bindAffinitySelfSpellTemplate.PlayerLearnableByClassTrainer = true;
            spellTemplates.Add(bindAffinitySelfSpellTemplate);

            // Bind Affinity
            SpellTemplate bindAffinitySpellTemplate = new SpellTemplate();
            bindAffinitySpellTemplate.Name = "Bind Affinity";
            bindAffinitySpellTemplate.ID = Configuration.SPELLS_BINDANY_SPELLDBC_ID;
            bindAffinitySpellTemplate.Description = "Binds the soul of the target to their current location. Only works in Norrath.";
            bindAffinitySpellTemplate.SpellIconID = SpellIconDBC.GetDBCIDForIconID(21);
            bindAffinitySpellTemplate.CastTimeInMS = 6000;
            bindAffinitySpellTemplate.RecoveryTimeInMS = 12000;
            bindAffinitySpellTemplate.RangeIndexDBCID = 4; // Medium Range - 30 yards
            bindAffinitySpellTemplate.TargetType = SpellTargetType.AllyGroupedSingle;
            bindAffinitySpellTemplate.SpellVisualID1 = 99; // Taken from soulstone
            bindAffinitySpellTemplate.PlayerLearnableByClassTrainer = true;
            spellTemplates.Add(bindAffinitySpellTemplate);

            Logger.WriteDetail("Generating spells completed.");
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
            Logger.WriteDetail("Deleting previously extracted DBC files");
            string exportedDBCFolder = Path.Combine(wowExportPath, "ExportedDBCFiles");
            FileTool.CreateBlankDirectory(exportedDBCFolder, false);

            // Generate a script to extract the DBC files
            Logger.WriteDetail("Generating script to extract DBC files");
            string workingGeneratedScriptsFolder = Path.Combine(wowExportPath, "GeneratedWorkingScripts");
            FileTool.CreateBlankDirectory(workingGeneratedScriptsFolder, true);
            StringBuilder dbcExtractScriptText = new StringBuilder();
            foreach (string patchFileName in patchFileNames)
                dbcExtractScriptText.AppendLine("extract \"" + patchFileName + "\" DBFilesClient\\* \"" + exportedDBCFolder + "\"");
            string dbcExtractionScriptFileName = Path.Combine(workingGeneratedScriptsFolder, "dbcextract.txt");
            using (var dbcExtractionScriptFile = new StreamWriter(dbcExtractionScriptFileName))
                dbcExtractionScriptFile.WriteLine(dbcExtractScriptText.ToString());

            // Extract the files using the script
            Logger.WriteDetail("Extracting DBC files");
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

            Logger.WriteDetail("Extracting client DBC files complete");
        }

        public void CreatePatchMPQ()
        {
            Logger.WriteInfo("Building patch MPQ...");

            // Make sure the output folder exists
            if (Directory.Exists(Configuration.PATH_EXPORT_FOLDER) == false)
                throw new Exception("Export folder '" + Configuration.PATH_EXPORT_FOLDER + "' did not exist, make sure you set PATH_EXPORT_FOLDER");

            // Delete the old patch file, if it exists
            Logger.WriteDetail("Deleting old patch file if it exists");
            string outputPatchFileName = Path.Combine(Configuration.PATH_EXPORT_FOLDER, Configuration.PATH_PATCH_NEW_FILE_NAME_NO_EXT + ".MPQ");
            if (File.Exists(outputPatchFileName) == true)
                File.Delete(outputPatchFileName);

            // Generate a script to generate the mpq file
            Logger.WriteDetail("Generating script to generate MPQ file");
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
            Logger.WriteDetail("Generating MPQ file");
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

            Logger.WriteDetail("Building patch MPQ complete");
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
            Logger.WriteDetail("Generating script to update the MPQ file");
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
            string relativeItemIconsPath = Path.Combine("Interface", "ICONS");
            string fullItemIconsPath = Path.Combine(mpqReadyFolder, relativeItemIconsPath);
            mpqUpdateScriptText.AppendLine("add \"" + exportMPQFileName + "\" \"" + fullItemIconsPath + "\" \"" + relativeItemIconsPath + "\" /r");

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
            Logger.WriteDetail("Updating MPQ file");
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

            Logger.WriteDetail("Building patch MPQ complete");
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
                        Logger.WriteDetail(ex.StackTrace.ToString());
                    Logger.WriteError("Deploying to client failed");
                    return;
                }
            }

            // Copy it
            File.Copy(sourcePatchFileNameAndPath, targetPatchFileNameAndPath, true);

            Logger.WriteDetail("Deploying to client complete");
        }

        public void ClearClientCache()
        {
            Logger.WriteInfo("Clearing client cache...");

            // If there is a folder, delete it
            string folderToDelete = Path.Combine(Configuration.PATH_WOW_ENUS_CLIENT_FOLDER, "Cache", "WDB");
            if (Directory.Exists(folderToDelete) == true)
            {
                Logger.WriteDetail("Client cache WDB folder found, so deleting...");
                Directory.Delete(folderToDelete, true);
                Logger.WriteDetail("Client cache WDB deleted.");
            }
            else
                Logger.WriteDetail("No client cache WDB folder detected");

            Logger.WriteDetail("Clearing client cache complete");
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
            Logger.WriteDetail("Deploying DBC files to the server...");
            string[] dbcFiles = Directory.GetFiles(sourceServerDBCFolder);
            foreach (string dbcFile in dbcFiles)
            {
                string targetFileName = Path.Combine(Configuration.PATH_DEPLOY_SERVER_DBC_FILES_FOLDER, Path.GetFileName(dbcFile));
                File.Copy(dbcFile, targetFileName, true);
            }


            Logger.WriteDetail("Deploying files to server complete");
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
                    Logger.WriteDetail(ex.StackTrace.ToString());
                Logger.WriteError("Deploying sql to server failed.");
            }

            Logger.WriteDetail("Deploying sql to server complete");
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
                File.Copy(classicInputFile, classicOutputFile);

            // Kunark
            string kunarkInputFile = Path.Combine(eqExportsConditionedPath, "miscimages", "eqkloadresized.blp");
            string kunarkOutputFile = Path.Combine(loadingScreensTextureFolder, "LoadingScreenEQKunark.blp");
            if (File.Exists(kunarkInputFile) == false)
                Logger.WriteError("Could not find texture '" + kunarkInputFile + "', it did not exist. Did you run blpconverter?");
            else
                File.Copy(kunarkInputFile, kunarkOutputFile);

            // Velious
            string veliousInputFile = Path.Combine(eqExportsConditionedPath, "miscimages", "eqvloadresized.blp");
            string veliousOutputFile = Path.Combine(loadingScreensTextureFolder, "LoadingScreenEQVelious.blp");
            if (File.Exists(veliousInputFile) == false)
                Logger.WriteError("Could not find texture '" + veliousInputFile + "', it did not exist. Did you run blpconverter?");
            else
                File.Copy(veliousInputFile, veliousOutputFile);
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
            SpellIconDBC spellIconDBC = new SpellIconDBC();
            spellIconDBC.LoadFromDisk(dbcInputFolder, "SpellIcon.dbc");
            TaxiPathDBC taxiPathDBC = new TaxiPathDBC();
            taxiPathDBC.LoadFromDisk(dbcInputFolder, "TaxiPath.dbc");
            TaxiPathNodeDBC taxiPathNodeDBC = new TaxiPathNodeDBC();
            taxiPathNodeDBC.LoadFromDisk(dbcInputFolder, "TaxiPathNode.dbc");
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
                areaTableDBC.AddRow(Convert.ToInt32(zone.DefaultArea.DBCAreaTableID), zoneProperties.DBCMapID, 0, zone.DefaultArea.AreaMusic, zone.DefaultArea.AreaAmbientSound, zone.DefaultArea.DisplayName);
                foreach (ZoneArea subArea in zoneProperties.ZoneAreas)
                    areaTableDBC.AddRow(Convert.ToInt32(subArea.DBCAreaTableID), zoneProperties.DBCMapID, Convert.ToInt32(subArea.DBCParentAreaTableID), subArea.AreaMusic, subArea.AreaAmbientSound, subArea.DisplayName);

                // AreaTrigger
                foreach (ZonePropertiesZoneLineBox zoneLine in zoneProperties.ZoneLineBoxes)
                    areaTriggerDBC.AddRow(zoneLine.AreaTriggerID, zoneProperties.DBCMapID, zoneLine.BoxPosition.X, zoneLine.BoxPosition.Y,
                        zoneLine.BoxPosition.Z, zoneLine.BoxLength, zoneLine.BoxWidth, zoneLine.BoxHeight, zoneLine.BoxOrientation);

                // Item data
                foreach (ItemTemplate itemTemplate in ItemTemplate.GetItemTemplatesByEQDBIDs().Values)
                    itemDBC.AddRow(itemTemplate);
                foreach (ItemDisplayInfo itemDisplayInfo in ItemDisplayInfo.ItemDisplayInfos)
                    itemDisplayInfoDBC.AddRow(itemDisplayInfo);

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

            // Graveyards
            foreach(ZonePropertiesGraveyard graveyard in ZonePropertiesGraveyard.GetAllGraveyards())
            {
                int mapID = ZoneProperties.GetZonePropertiesForZone(graveyard.LocationShortName).DBCMapID;
                worldSafeLocsDBC.AddRow(graveyard, mapID);
            }

            // SkillLine
            //skillLineDBC.AddRow(Configuration.DBCID_SKILLLINE_ALTERATION_ID, "Alteration");            

            // Spells
            for(int i = 0; i < 23; i++)
                spellIconDBC.AddRow(i);
            foreach (SpellTemplate spellTemplate in spellTemplates)
            {
                spellDBC.AddRow(spellTemplate);
                skillLineAbilityDBC.AddRow(SkillLineAbilityDBC.GenerateID(), Configuration.DBCID_SKILLLINE_ALTERATION_ID, spellTemplate.ID);
            }
            foreach (var spellCastTimeDBCIDByCastTime in SpellTemplate.SpellCastTimeDBCIDsByCastTime)
                spellCastTimesDBC.AddRow(spellCastTimeDBCIDByCastTime.Value, spellCastTimeDBCIDByCastTime.Key);

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
                            Logger.WriteDetail("Skipping transport ship since zone '" + touchedZone + "' isn't being converted");
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
                    gameObjectDisplayInfoDBC.AddRow(m2ByGameObjectID.Key, relativeObjectFileName.ToLower(), m2ByGameObjectID.Value.ObjectModel.BoundingBox);
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
                    gameObjectDisplayInfoDBC.AddRow(m2ByGameObjectID.Key, relativeObjectFileName.ToLower(), m2ByGameObjectID.Value.ObjectModel.BoundingBox, openSoundEntryID, closeSoundEntryID);
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

            // Save the files
            areaTableDBC.SaveToDisk(dbcOutputClientFolder);
            areaTableDBC.SaveToDisk(dbcOutputServerFolder);
            areaTriggerDBC.SaveToDisk(dbcOutputClientFolder);
            areaTriggerDBC.SaveToDisk(dbcOutputServerFolder);
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
            spellIconDBC.SaveToDisk(dbcOutputClientFolder);
            spellIconDBC.SaveToDisk(dbcOutputServerFolder);
            taxiPathDBC.SaveToDisk(dbcOutputClientFolder);
            taxiPathDBC.SaveToDisk(dbcOutputServerFolder);
            taxiPathNodeDBC.SaveToDisk(dbcOutputClientFolder);
            taxiPathNodeDBC.SaveToDisk(dbcOutputServerFolder);
            transportAnimationDBC.SaveToDisk(dbcOutputClientFolder);
            transportAnimationDBC.SaveToDisk(dbcOutputServerFolder);
            worldSafeLocsDBC.SaveToDisk(dbcOutputClientFolder);
            worldSafeLocsDBC.SaveToDisk(dbcOutputServerFolder);            
            wmoAreaTableDBC.SaveToDisk(dbcOutputClientFolder);
            wmoAreaTableDBC.SaveToDisk(dbcOutputServerFolder);
            zoneMusicDBC.SaveToDisk(dbcOutputClientFolder);
            zoneMusicDBC.SaveToDisk(dbcOutputServerFolder);

            Logger.WriteDetail("Creating DBC Files complete");
        }

        public void CreateSQLScript(List<Zone> zones, List<CreatureTemplate> creatureTemplates, List<CreatureModelTemplate> creatureModelTemplates,
            List<CreatureSpawnPool> creatureSpawnPools, Dictionary<int, List<ItemLootTemplate>> itemLootTemplatesByCreatureTemplateID)
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
            CreatureLootTableSQL creatureLootTableSQL = new CreatureLootTableSQL();
            CreatureModelInfoSQL creatureModelInfoSQL = new CreatureModelInfoSQL();
            CreatureTemplateSQL creatureTemplateSQL = new CreatureTemplateSQL();
            CreatureTemplateModelSQL creatureTemplateModelSQL = new CreatureTemplateModelSQL();
            GameGraveyardSQL gameGraveyardSQL = new GameGraveyardSQL();
            GameTeleSQL gameTeleSQL = new GameTeleSQL();
            GossipMenuSQL gossipMenuSQL = new GossipMenuSQL();
            GossipMenuOptionSQL gossipMenuOptionSQL = new GossipMenuOptionSQL();
            GraveyardZoneSQL graveyardZoneSQL = new GraveyardZoneSQL();
            GameObjectSQL gameObjectSQL = new GameObjectSQL();
            GameObjectTemplateSQL gameObjectTemplateSQL = new GameObjectTemplateSQL();
            GameObjectTemplateAddonSQL gameObjectTemplateAddonSQL = new GameObjectTemplateAddonSQL();
            InstanceTemplateSQL instanceTemplateSQL = new InstanceTemplateSQL();
            ItemTemplateSQL itemTemplateSQL = new ItemTemplateSQL();
            ModEverquestCreatureOnkillReputationSQL modEverquestCreatureOnkillReputationSQL = new ModEverquestCreatureOnkillReputationSQL();
            NPCTextSQL npcTextSQL = new NPCTextSQL();
            NPCTrainerSQL npcTrainerSQL = new NPCTrainerSQL();            
            NPCVendorSQL npcVendorSQL = new NPCVendorSQL();
            PoolCreatureSQL poolCreatureSQL = new PoolCreatureSQL();
            PoolPoolSQL poolPoolSQL = new PoolPoolSQL();
            PoolTemplateSQL poolTemplateSQL = new PoolTemplateSQL();
            //SmartScriptsSQL smartScriptsSQL = new SmartScriptsSQL();
            TransportsSQL transportsSQL = new TransportsSQL();
            WaypointDataSQL waypointDataSQL = new WaypointDataSQL();

            // Zones
            foreach (Zone zone in zones)
            {
                // Instance list
                instanceTemplateSQL.AddRow(Convert.ToInt32(zone.ZoneProperties.DBCMapID));

                // Teleport scripts to safe positions (add a record for both descriptive and short name if they are different)
                gameTeleSQL.AddRow(Convert.ToInt32(zone.ZoneProperties.DBCMapID), zone.DescriptiveNameOnlyLetters, zone.SafePosition.Y, zone.SafePosition.Y, zone.SafePosition.Z);
                if (zone.DescriptiveNameOnlyLetters.ToLower() != zone.ShortName.ToLower())
                    gameTeleSQL.AddRow(Convert.ToInt32(zone.ZoneProperties.DBCMapID), zone.ShortName, zone.SafePosition.Y, zone.SafePosition.Y, zone.SafePosition.Z);

                // Zone lines
                foreach (ZonePropertiesZoneLineBox zoneLine in ZoneProperties.GetZonePropertiesForZone(zone.ShortName).ZoneLineBoxes)
                {
                    if (ZoneProperties.ZonePropertyListByShortName.ContainsKey(zoneLine.TargetZoneShortName) == false)
                    {
                        Logger.WriteError("Error!  When attempting to map a zone line, there was no zone with short name '" + zoneLine.TargetZoneShortName + "'");
                        continue;
                    }

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
                        npcTrainerSQL.AddRowForClassTrainer(SpellClassTrainerAbility.GetTrainerSpellsIDForWOWClassTrainer(creatureTemplate.ClassTrainerType), creatureTemplate.WOWCreatureTemplateID);

                        // Associate the menu
                        creatureTemplate.GossipMenuID = classTrainerMenuIDs[creatureTemplate.ClassTrainerType];
                    }

                    // Create the records
                    creatureTemplateSQL.AddRow(creatureTemplate, scale);
                    creatureTemplateModelSQL.AddRow(creatureTemplate.WOWCreatureTemplateID, creatureTemplate.ModelTemplate.DBCCreatureDisplayID, scale);
                    
                    // If it's a vendor, add the vendor records too
                    if (creatureTemplate.MerchantID != 0 && vendorItems.ContainsKey(creatureTemplate.MerchantID))
                    {
                        foreach(CreatureVendorItem vendorItem in vendorItems[creatureTemplate.MerchantID])
                        {
                            if (ItemTemplate.GetItemTemplatesByEQDBIDs().ContainsKey(vendorItem.EQItemID) == false)
                            {
                                Logger.WriteError("Attempted to add a merchant item with EQItemID '" + vendorItem.EQItemID + "' to merchant '" + creatureTemplate.MerchantID + "', but the EQItemID did not exist");
                                continue;
                            }

                            npcVendorSQL.AddRow(creatureTemplate.WOWCreatureTemplateID, ItemTemplate.GetItemTemplatesByEQDBIDs()[vendorItem.EQItemID].WOWEntryID, vendorItem.Slot);
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
                    if (spawnInstance.PathGridEntries.Count > 0)
                    {
                        int waypointGUID = creatureGUID * 1000;
                        creatureAddonSQL.AddRow(creatureGUID, waypointGUID);
                        foreach (CreaturePathGridEntry pathGridEntry in spawnInstance.PathGridEntries)
                            waypointDataSQL.AddRow(waypointGUID, pathGridEntry.Number, pathGridEntry.NodeX, pathGridEntry.NodeY, pathGridEntry.NodeZ, pathGridEntry.PauseInSec * 1000);
                        creatureSQL.AddRow(creatureGUID, creatureTemplate.WOWCreatureTemplateID, spawnInstance.MapID, spawnInstance.AreaID, spawnInstance.AreaID,
                            spawnInstance.SpawnXPosition, spawnInstance.SpawnYPosition, spawnInstance.SpawnZPosition, spawnInstance.Orientation, CreatureMovementType.Path);
                    }
                    else
                    {
                        creatureSQL.AddRow(creatureGUID, creatureTemplate.WOWCreatureTemplateID, spawnInstance.MapID, spawnInstance.AreaID, spawnInstance.AreaID,
                            spawnInstance.SpawnXPosition, spawnInstance.SpawnYPosition, spawnInstance.SpawnZPosition, spawnInstance.Orientation, CreatureMovementType.None);
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
                            if (spawnInstance.PathGridEntries.Count > 0)
                            {
                                int waypointGUID = creatureGUID * 1000;
                                creatureAddonSQL.AddRow(creatureGUID, waypointGUID);
                                foreach (CreaturePathGridEntry pathGridEntry in spawnInstance.PathGridEntries)
                                    waypointDataSQL.AddRow(waypointGUID, pathGridEntry.Number, pathGridEntry.NodeX, pathGridEntry.NodeY, pathGridEntry.NodeZ, pathGridEntry.PauseInSec * 1000);
                                creatureSQL.AddRow(creatureGUID, creatureTemplate.WOWCreatureTemplateID, spawnInstance.MapID, spawnInstance.AreaID, spawnInstance.AreaID,
                                    spawnInstance.SpawnXPosition, spawnInstance.SpawnYPosition, spawnInstance.SpawnZPosition, spawnInstance.Orientation, CreatureMovementType.Path);
                            }
                            else
                            {
                                creatureSQL.AddRow(creatureGUID, creatureTemplate.WOWCreatureTemplateID, spawnInstance.MapID, spawnInstance.AreaID, spawnInstance.AreaID,
                                    spawnInstance.SpawnXPosition, spawnInstance.SpawnYPosition, spawnInstance.SpawnZPosition, spawnInstance.Orientation, CreatureMovementType.None);
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
                        if (spawnInstance.PathGridEntries.Count > 0)
                        {
                            int waypointGUID = creatureGUID * 1000;
                            creatureAddonSQL.AddRow(creatureGUID, waypointGUID);
                            foreach (CreaturePathGridEntry pathGridEntry in spawnInstance.PathGridEntries)
                                waypointDataSQL.AddRow(waypointGUID, pathGridEntry.Number, pathGridEntry.NodeX, pathGridEntry.NodeY, pathGridEntry.NodeZ, pathGridEntry.PauseInSec * 1000);
                            creatureSQL.AddRow(creatureGUID, creatureTemplate.WOWCreatureTemplateID, spawnInstance.MapID, spawnInstance.AreaID, spawnInstance.AreaID,
                                spawnInstance.SpawnXPosition, spawnInstance.SpawnYPosition, spawnInstance.SpawnZPosition, spawnInstance.Orientation, CreatureMovementType.Path);
                        }
                        else
                        {
                            creatureSQL.AddRow(creatureGUID, creatureTemplate.WOWCreatureTemplateID, spawnInstance.MapID, spawnInstance.AreaID, spawnInstance.AreaID,
                                spawnInstance.SpawnXPosition, spawnInstance.SpawnYPosition, spawnInstance.SpawnZPosition, spawnInstance.Orientation, CreatureMovementType.None);
                        }
                    }
                }
            }

            // Items
            foreach (ItemTemplate itemTemplate in ItemTemplate.GetItemTemplatesByEQDBIDs().Values)
                itemTemplateSQL.AddRow(itemTemplate);
            foreach (var itemLootTemplateByCreatureTemplateID in itemLootTemplatesByCreatureTemplateID.Values)
                foreach (ItemLootTemplate itemLootTemplate in itemLootTemplateByCreatureTemplateID)
                    creatureLootTableSQL.AddRow(itemLootTemplate);

            // Graveyards
            foreach (ZonePropertiesGraveyard graveyard in ZonePropertiesGraveyard.GetAllGraveyards())
            {
                // Should be one for each ghost zone
                foreach(string zoneShortName in graveyard.GhostZoneShortNames)
                {
                    string zoneShortNameLower = zoneShortName.ToLower();
                    int ghostZoneAreaID = Convert.ToInt32(ZoneProperties.GetZonePropertiesForZone(zoneShortNameLower).DefaultZoneArea.DBCAreaTableID);
                    graveyardZoneSQL.AddRow(graveyard, ghostZoneAreaID);
                }

                ZoneProperties curZoneProperties = ZoneProperties.GetZonePropertiesForZone(graveyard.LocationShortName);
                int mapID = curZoneProperties.DBCMapID;
                gameGraveyardSQL.AddRow(graveyard, mapID);

                // And there should be one spirit healer per graveyard
                int spiritHealerGUID = CreatureTemplate.GenerateCreatureSQLGUID();
                int zoneAreaID = Convert.ToInt32(curZoneProperties.DefaultZoneArea.DBCAreaTableID);
                creatureSQL.AddRow(spiritHealerGUID, Configuration.ZONE_GRAVEYARD_SPIRIT_HEALER_CREATURETEMPLATE_ID, mapID, zoneAreaID, zoneAreaID, 
                    graveyard.SpiritHealerX, graveyard.SpiritHealerY, graveyard.SpiritHealerZ, graveyard.SpiritHealerOrientation, CreatureMovementType.None);
            }         

            // Trainer Abilities
            foreach (ClassType classType in Enum.GetValues(typeof(ClassType)))
            {
                if (classType == ClassType.All || classType == ClassType.None)
                    continue;

                int lineID = SpellClassTrainerAbility.GetTrainerSpellsIDForWOWClassTrainer(classType);
                foreach (SpellClassTrainerAbility trainerAbility in SpellClassTrainerAbility.GetTrainerSpellsForClass(classType))
                    npcTrainerSQL.AddRowForClassAbility(lineID, trainerAbility);
            }

            // Transports
            if (Configuration.GENERATE_TRANSPORTS == true)
            {
                Dictionary<string, int> mapIDsByShortName = new Dictionary<string, int>();
                foreach (Zone zone in zones)
                    mapIDsByShortName.Add(zone.ShortName.ToLower().Trim(), zone.ZoneProperties.DBCMapID);
                foreach (TransportShip transportShip in TransportShip.GetAllTransportShips())
                {
                    // Only add this transport ship if the full path is zones that are loaded
                    bool zonesAreLoaded = true;
                    foreach (string touchedZone in transportShip.GetTouchedZonesSplitOut())
                    {
                        if (mapIDsByShortName.ContainsKey(touchedZone.ToLower().Trim()) == false)
                        {
                            zonesAreLoaded = false;
                            Logger.WriteDetail("Skipping transport ship since zone '" + touchedZone + "' isn't being converted");
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
                        Logger.WriteDetail("Skipping transport lift since zone '" + transportLift.SpawnZoneShortName + "' isn't being converted");
                        continue;
                    }
                    int areaID = 0;
                    foreach (Zone zone in zones)
                        if (zone.ShortName.ToLower().Trim() == transportLift.SpawnZoneShortName.ToLower().Trim())
                            areaID = Convert.ToInt32(zone.DefaultArea.DBCAreaTableID);

                    string name = "Lift EQ (" + transportLift.Name + ")";
                    string longName = transportLift.SpawnZoneShortName + " (" + name + ")";
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
                        Logger.WriteDetail("Skipping transport lift trigger since zone '" + transportLiftTrigger.SpawnZoneShortName + "' isn't being converted");
                        continue;
                    }
                    int areaID = 0;
                    foreach (Zone zone in zones)
                        if (zone.ShortName.ToLower().Trim() == transportLiftTrigger.SpawnZoneShortName.ToLower().Trim())
                            areaID = Convert.ToInt32(zone.DefaultArea.DBCAreaTableID);

                    string name = transportLiftTrigger.Name;
                    string longName = "EQ Lift Trigger " + transportLiftTrigger.SpawnZoneShortName + " (" + name + ")";
                    int mapID = mapIDsByShortName[transportLiftTrigger.SpawnZoneShortName.ToLower().Trim()];
                    gameObjectTemplateSQL.AddRowForTransportLiftTrigger(transportLiftTrigger.GameObjectTemplateID, transportLiftTrigger.GameObjectDisplayInfoID, name, transportLiftTrigger.ResetTimeInMS);
                    gameObjectTemplateAddonSQL.AddRowForLiftTrigger(transportLiftTrigger.GameObjectTemplateID);
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
            gameGraveyardSQL.SaveToDisk("game_graveyard", SQLFileType.World);
            gameObjectSQL.SaveToDisk("gameobject", SQLFileType.World);
            gameObjectTemplateSQL.SaveToDisk("gameobject_template", SQLFileType.World);
            gameObjectTemplateAddonSQL.SaveToDisk("gameobject_template_addon", SQLFileType.World);
            gameTeleSQL.SaveToDisk("game_tele", SQLFileType.World);
            gossipMenuSQL.SaveToDisk("gossip_menu", SQLFileType.World);
            gossipMenuOptionSQL.SaveToDisk("gossip_menu_option", SQLFileType.World);
            graveyardZoneSQL.SaveToDisk("graveyard_zone", SQLFileType.World);
            instanceTemplateSQL.SaveToDisk("instance_template", SQLFileType.World);
            itemTemplateSQL.SaveToDisk("item_template", SQLFileType.World);
            modEverquestCreatureOnkillReputationSQL.SaveToDisk("mod_everquest_creature_onkill_reputation", SQLFileType.World);
            npcTextSQL.SaveToDisk("npc_text", SQLFileType.World);
            npcTrainerSQL.SaveToDisk("npc_trainer", SQLFileType.World);
            npcVendorSQL.SaveToDisk("npc_vendor", SQLFileType.World);
            poolCreatureSQL.SaveToDisk("pool_creature", SQLFileType.World);
            poolPoolSQL.SaveToDisk("pool_pool", SQLFileType.World);
            poolTemplateSQL.SaveToDisk("pool_template", SQLFileType.World);
            //smartScriptsSQL.SaveToDisk("smart_scripts", SQLFileType.World);
            transportsSQL.SaveToDisk("transports", SQLFileType.World);
            waypointDataSQL.SaveToDisk("waypoint_data", SQLFileType.World);
        }

        public void ExportTexturesForZone(Zone zone, string zoneInputFolder, string wowExportPath, string relativeZoneMaterialDoodadsPath,
            string inputObjectTextureFolder)
        {
            Logger.WriteDetail("- [" + zone.ShortName + "]: Exporting textures for zone '" + zone.ShortName + "'...");

            // Create the folder to output
            string zoneOutputTextureFolder = Path.Combine(wowExportPath, "World", "Everquest", "ZoneTextures", zone.ShortName);
            if (Directory.Exists(zoneOutputTextureFolder) == false)
                FileTool.CreateBlankDirectory(zoneOutputTextureFolder, true);

            // Go through every texture to move and put it there
            foreach (Material material in zone.Materials)
            {
                foreach (string textureName in material.TextureNames)
                {
                    string sourceTextureFullPath = Path.Combine(zoneInputFolder, "Textures", textureName + ".blp");
                    string outputTextureFullPath = Path.Combine(zoneOutputTextureFolder, textureName + ".blp");
                    if (File.Exists(sourceTextureFullPath) == false)
                    {
                        Logger.WriteError("Could not copy texture '" + sourceTextureFullPath + "', it did not exist. Did you run blpconverter?");
                        continue;
                    }
                    File.Copy(sourceTextureFullPath, outputTextureFullPath, true);
                    Logger.WriteDetail("- [" + zone.ShortName + "]: Texture named '" + textureName + "' copied");
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
                    File.Copy(sourceTextureFullPath, outputTextureFullPath, true);
                    Logger.WriteDetail("- [" + zone.ShortName + "]: Texture named '" + texture.TextureName + "' copied");
                }
            }

            // Finally copy the textures for any sound instance objects
            string soundInstanceInputTextureFullPath = Path.Combine(inputObjectTextureFolder, Configuration.AUDIO_SOUNDINSTANCE_RENDEROBJECT_MATERIAL_NAME + ".blp");
            foreach (ObjectModel zoneObject in zone.SoundInstanceObjectModels)
            {
                string soundInstanceOutputTextureFullPath = Path.Combine(wowExportPath, relativeZoneMaterialDoodadsPath, zoneObject.Name, Configuration.AUDIO_SOUNDINSTANCE_RENDEROBJECT_MATERIAL_NAME + ".blp");
                if (File.Exists(soundInstanceInputTextureFullPath) == false)
                {
                    Logger.WriteError("Could not copy texture '" + soundInstanceInputTextureFullPath + "', it did not exist. Did you run blpconverter?");
                    continue;
                }
                File.Copy(soundInstanceInputTextureFullPath, soundInstanceOutputTextureFullPath, true);
            }

            Logger.WriteDetail("- [" + zone.ShortName + "]: Texture output for zone '" + zone.ShortName + "' complete");
        }

        public void ExportMusicForZone(Zone zone, string musicInputFolder, string wowExportPath)
        {
            Logger.WriteDetail("- [" + zone.ShortName + "]: Exporting music for zone '" + zone.ShortName + "'...");

            if (zone.ZoneAreaMusics.Count == 0)
            {
                Logger.WriteDetail("- [" + zone.ShortName + "]: No music found for this zone");
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
                File.Copy(sourceFullPath, targetFullPath, true);
                Logger.WriteDetail("- [" + zone.ShortName + "]: Music named '" + musicSoundByFileName.Value.AudioFileNameNoExt + "' copied");
            }
            Logger.WriteDetail("- [" + zone.ShortName + "]: Music output for zone '" + zone.ShortName + "' complete");
        }

        public void ExportAmbientSoundForZone(Zone zone, string soundInputFolder, string wowExportPath)
        {
            Logger.WriteDetail("- [" + zone.ShortName + "]: Exporting ambient sound for zone '" + zone.ShortName + "'...");

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
                    Logger.WriteDetail("Skipping sound file named '" + ambientSoundByFileName + "' since it is just silence");
                    continue;
                }
                string sourceFullPath = Path.Combine(soundInputFolder, ambientSoundByFileName.Value.AudioFileNameNoExt + ".wav");
                string targetFullPath = Path.Combine(zoneOutputAmbienceFolder, ambientSoundByFileName.Value.AudioFileNameNoExt + ".wav");
                if (File.Exists(sourceFullPath) == false)
                {
                    Logger.WriteError("Could not copy ambient sound file '" + sourceFullPath + "', as it did not exist");
                    continue;
                }
                File.Copy(sourceFullPath, targetFullPath, true);
                Logger.WriteDetail("- [" + zone.ShortName + "]: Ambient sound named '" + ambientSoundByFileName.Value.AudioFileNameNoExt + "' copied");
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
                    File.Copy(sourceFullPath, targetFullPath, true);
                    Logger.WriteDetail("- [" + zone.ShortName + "]: Sound instance sound named '" + curSound.AudioFileNameNoExt + "' copied");
                }
            }

            Logger.WriteDetail("- [" + zone.ShortName + "]: Ambient sound output for zone '" + zone.ShortName + "' complete");
        }

        public void ExportTexturesForObject(ObjectModel wowObjectModelData, string objectTextureInputFolder, string objectExportPath)
        {
            Logger.WriteDetail("- [" + wowObjectModelData.Name + "]: Exporting textures for object '" + wowObjectModelData.Name + "'...");

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
                File.Copy(inputTextureName, outputTextureName, true);
                Logger.WriteDetail("- [" + wowObjectModelData.Name + "]: Texture named '" + texture.TextureName + ".blp' copied");
            }

            Logger.WriteDetail("- [" + wowObjectModelData.Name + "]: Texture output for object '" + wowObjectModelData.Name + "' complete");
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
            SortedDictionary<int, ItemTemplate> itemTemplates = ItemTemplate.GetItemTemplatesByEQDBIDs();       
            string itemIconInputFolder = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "itemicons");
            foreach(ItemDisplayInfo itemDisplayInfo in ItemDisplayInfo.ItemDisplayInfos)
            {
                string sourceIconFile = Path.Combine(itemIconInputFolder, itemDisplayInfo.IconFileNameNoExt + ".blp");
                string outputIconFile = Path.Combine(iconOutputFolder, itemDisplayInfo.IconFileNameNoExt + ".blp");
                File.Copy(sourceIconFile, outputIconFile, true);
            }

            // Spells
            string spellIconInputFolder = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "spellicons");
            string[] spellIconFiles = Directory.GetFiles(spellIconInputFolder, "*.blp");
            foreach(string spellIconFile in spellIconFiles)
            {
                string sourceIconFile = spellIconFile;
                string outputIconFile = Path.Combine(iconOutputFolder, Path.GetFileNameWithoutExtension(spellIconFile) + ".blp");
                File.Copy(sourceIconFile, outputIconFile, true);
            }
        }
    }
}
