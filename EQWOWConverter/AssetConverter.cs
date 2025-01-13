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
using EQWOWConverter.WOWFiles;
using EQWOWConverter.WOWFiles.DBC;
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
            if (Directory.Exists(Configuration.CONFIG_PATH_EQEXPORTSCONDITIONED_FOLDER) == false)
            {
                Logger.WriteError("Error - Conditioned path of '" + Configuration.CONFIG_PATH_EQEXPORTSCONDITIONED_FOLDER + "' does not exist.");
                Logger.WriteError("Conversion Failed!");
                return false;
            }

            // Extract
            if (Configuration.CONFIG_EXTRACT_DBC_FILES == true)
                ExtractClientDBCFiles();
            else
                Logger.WriteInfo("- Note: DBC File Extraction is set to false in the Configuration");

            // Objects (must always come before zones)
            if (Configuration.CONFIG_GENERATE_OBJECTS == true)
            {
                if (ConvertEQObjectsToWOW() == false)
                    return false;
            }
            else
            {
                Logger.WriteInfo("- Note: Object generation is set to false in the Configuration");
            }

            // Zones
            List<Zone> zones;
            if (ConvertEQZonesToWOW(out zones) == false)
                return false;

            // Creatures
            List<CreatureModelTemplate> creatureModelTemplates = new List<CreatureModelTemplate>();
            List<CreatureTemplate> creatureTemplates = new List<CreatureTemplate>();
            List<CreatureSpawnPool> creatureSpawnPools = new List<CreatureSpawnPool>();
            if (Configuration.CONFIG_GENERATE_CREATURES_AND_SPAWNS == true)
            {
                if (ConvertCreatures(ref creatureModelTemplates, ref creatureTemplates, ref creatureSpawnPools, zones) == false)
                    return false;
            }
            else
            {
                Logger.WriteInfo("- Note: Creature generation is set to false in the Configuration");
            }

            // Items
            CreateItems();

            // Copy the loading screens
            CreateLoadingScreens();

            // Copy the liquid material textures
            CreateLiquidMaterials();

            // Create the DBC files
            CreateDBCFiles(zones, creatureModelTemplates);

            // Create the SQL Scripts (note: this must always be after DBC files)
            CreateSQLScript(zones, creatureTemplates, creatureModelTemplates, creatureSpawnPools);

            // Create or update the MPQ
            string exportMPQFileName = Path.Combine(Configuration.CONFIG_PATH_EXPORT_FOLDER, Configuration.CONFIG_PATH_PATCH_NEW_FILE_NAME_NO_EXT + ".mpq");
            if (Configuration.CONFIG_GENERATE_UPDATE_BUILD_INCLUDED_ZONE_SHORTNAMES.Count == 0 || File.Exists(exportMPQFileName) == false)
                CreatePatchMPQ();
            else
                UpdatePatchMPQ();

            // Deploy 
            if (Configuration.CONFIG_DEPLOY_CLIENT_FILES == true)
                DeployClient();
            if (Configuration.CONFIG_DEPLOY_SERVER_FILES == true)
                DeployServerFiles();
            if (Configuration.CONFIG_DEPLOY_SERVER_SQL == true)
                DeployServerSQL();

            Logger.WriteInfo("Conversion of data complete");
            return true;
        }

        public bool ConvertEQObjectsToWOW()
        {
            string eqExportsConditionedPath = Configuration.CONFIG_PATH_EQEXPORTSCONDITIONED_FOLDER;
            string wowExportPath = Configuration.CONFIG_PATH_EXPORT_FOLDER;

            Logger.WriteInfo("Converting EQ objects to WOW objects...");

            // Make sure the object folder path exists
            string objectFolderRoot = Path.Combine(eqExportsConditionedPath, "objects");
            if (Directory.Exists(objectFolderRoot) == false)
                Directory.CreateDirectory(objectFolderRoot);

            // Clean out the objects folder
            string exportMPQRootFolder = Path.Combine(wowExportPath, "MPQReady");
            string exportObjectsFolder = Path.Combine(exportMPQRootFolder, "World", "Everquest", "StaticDoodads");
            if (Directory.Exists(exportObjectsFolder))
                Directory.Delete(exportObjectsFolder, true);

            // For the counter
            int curProgress = 0;
            int curProgressOffset = Logger.GetConsolePriorRowCursorLeft();

            // Go through all of the object meshes and process them one at a time
            string objectMeshFolderRoot = Path.Combine(objectFolderRoot, "meshes");
            DirectoryInfo objectMeshDirectoryInfo = new DirectoryInfo(objectMeshFolderRoot);
            FileInfo[] objectMeshFileInfos = objectMeshDirectoryInfo.GetFiles();
            foreach (FileInfo objectMeshFileInfo in objectMeshFileInfos)
            {
                curProgress++;
                string staticObjectMeshNameNoExt = Path.GetFileNameWithoutExtension(objectMeshFileInfo.FullName);
                string curStaticObjectOutputFolder = Path.Combine(exportObjectsFolder, staticObjectMeshNameNoExt);

                // Skip the collision mesh files
                if (objectMeshFileInfo.Name.Contains("_collision"))
                {
                    Logger.WriteCounter(curProgress, curProgressOffset, objectMeshFileInfos.Length);
                    continue;
                }

                // Load the EQ object
                ObjectModelProperties objectProperties = ObjectModelProperties.GetObjectPropertiesForObject(staticObjectMeshNameNoExt);
                ObjectModel curObject = new ObjectModel(staticObjectMeshNameNoExt, objectProperties, ObjectModelType.SimpleDoodad);
                Logger.WriteDetail("- [" + staticObjectMeshNameNoExt + "]: Importing EQ static object '" + staticObjectMeshNameNoExt + "'");
                curObject.LoadStaticEQObjectFromFile(objectFolderRoot, staticObjectMeshNameNoExt);
                Logger.WriteDetail("- [" + staticObjectMeshNameNoExt + "]: Importing EQ static object '" + staticObjectMeshNameNoExt + "' complete");

                // Create the M2 and Skin
                string relativeMPQPath = Path.Combine("World", "Everquest", "StaticDoodads", staticObjectMeshNameNoExt);
                M2 objectM2 = new M2(curObject, relativeMPQPath);
                objectM2.WriteToDisk(curObject.Name, curStaticObjectOutputFolder);

                // Place the related textures
                string objectTextureFolder = Path.Combine(objectFolderRoot, "textures");
                ExportTexturesForObject(curObject, objectTextureFolder, curStaticObjectOutputFolder);

                // Save it for use elsewhere
                ObjectModel.StaticObjectModelsByName.Add(curObject.Name, curObject);
                Logger.WriteCounter(curProgress, curProgressOffset, objectMeshFileInfos.Length);
            }
            return true;
        }

        // TODO: Condense above
        public bool ConvertEQZonesToWOW(out List<Zone> zones)
        {
            Logger.WriteInfo("Converting EQ zones to WOW zones...");

            if (Configuration.CONFIG_GENERATE_UPDATE_BUILD_INCLUDED_ZONE_SHORTNAMES.Count > 0)
            {
                Logger.WriteInfo("- Note: CONFIG_GENERATE_UPDATE_BUILD_INCLUDED_ZONE_SHORTNAMES has values: ", false);
                foreach (string zoneShortName in Configuration.CONFIG_GENERATE_UPDATE_BUILD_INCLUDED_ZONE_SHORTNAMES)
                    Logger.WriteInfo(zoneShortName + " ", false, false);
                Logger.WriteInfo(string.Empty, true, false);
            }

            if (Configuration.CONFIG_GENERATE_UPDATE_BUILD_ONLY_HAVE_INCLUDED_ZONES_FUNCTIONAL == true)
                Logger.WriteInfo("- Note: CONFIG_GENERATE_UPDATE_BUILD_ONLY_HAVE_INCLUDED_ZONES_FUNCTIONAL is TRUE");

            // Build paths
            string inputZoneFolder = Path.Combine(Configuration.CONFIG_PATH_EQEXPORTSCONDITIONED_FOLDER, "zones");
            string inputSoundFolderRoot = Path.Combine(Configuration.CONFIG_PATH_EQEXPORTSCONDITIONED_FOLDER, "sounds");
            string inputMusicFolderRoot = Path.Combine(Configuration.CONFIG_PATH_EQEXPORTSCONDITIONED_FOLDER, "music");
            string inputObjectTexturesFolder = Path.Combine(Configuration.CONFIG_PATH_EQEXPORTSCONDITIONED_FOLDER, "objects", "textures");
            string exportMPQRootFolder = Path.Combine(Configuration.CONFIG_PATH_EXPORT_FOLDER, "MPQReady");
            string exportMapsFolder = Path.Combine(exportMPQRootFolder, "World", "Maps");
            string exportWMOFolder = Path.Combine(exportMPQRootFolder, "World", "wmo");
            string exportZonesTexturesFolder = Path.Combine(exportMPQRootFolder, "World", "Everquest", "ZoneTextures");
            string exportZonesObjectsFolder = Path.Combine(exportMPQRootFolder, "World", "Everquest", "ZoneObjects");
            string exportInterfaceFolder = Path.Combine(exportMPQRootFolder, "Interface");
            string exportMusicFolder = Path.Combine(exportMPQRootFolder, "Sound", "Music");
            string exportMPQFileName = Path.Combine(Configuration.CONFIG_PATH_EXPORT_FOLDER, Configuration.CONFIG_PATH_PATCH_NEW_FILE_NAME_NO_EXT + ".mpq");
            string relativeStaticDoodadsPath = Path.Combine("World", "Everquest", "StaticDoodads");

            // Clear folders if it's a fresh build
            if (Configuration.CONFIG_GENERATE_UPDATE_BUILD_INCLUDED_ZONE_SHORTNAMES.Count == 0
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
                if (Configuration.CONFIG_GENERATE_KUNARK_ZONES == false && Configuration.CONFIG_GENERATE_KUNARK_ZONE_SHORTNAMES.Contains(zoneDirectory.Name))
                    continue;
                if (Configuration.CONFIG_GENERATE_VELIOUS_ZONES == false && Configuration.CONFIG_GENERATE_VELIOUS_ZONE_SHORTNAMES.Contains(zoneDirectory.Name))
                    continue;
                if (Configuration.CONFIG_GENERATE_UPDATE_BUILD_ONLY_HAVE_INCLUDED_ZONES_FUNCTIONAL == true &&
                    Configuration.CONFIG_GENERATE_UPDATE_BUILD_INCLUDED_ZONE_SHORTNAMES.Contains(zoneDirectory.Name) == false)
                    continue;

                Logger.WriteInfo(" - Processing zone '" + zoneDirectory.Name + "'");

                // Load the EQ zone
                string relativeZoneObjectsPath = Path.Combine("World", "Everquest", "ZoneObjects", zoneDirectory.Name);
                ZoneProperties zoneProperties = ZoneProperties.GetZonePropertiesForZone(zoneDirectory.Name);
                Zone curZone = new Zone(zoneDirectory.Name, zoneProperties);
                Logger.WriteDetail("- [" + zoneDirectory.Name + "]: Importing EQ zone '" + zoneDirectory.Name);
                string curZoneDirectory = Path.Combine(inputZoneFolder, zoneDirectory.Name);
                curZone.LoadEQZoneData(zoneDirectory.Name, curZoneDirectory);
                Logger.WriteDetail("- [" + zoneDirectory.Name + "]: Importing of EQ zone '" + zoneDirectory.Name + "' complete");

                // Convert to WOW zone
                CreateWoWZoneFromEQZone(curZone, exportMPQRootFolder, relativeStaticDoodadsPath, relativeZoneObjectsPath);

                // Copy/move files if needed
                if (Configuration.CONFIG_GENERATE_UPDATE_BUILD_INCLUDED_ZONE_SHORTNAMES.Count == 0 ||
                    Configuration.CONFIG_GENERATE_UPDATE_BUILD_INCLUDED_ZONE_SHORTNAMES.Contains(zoneDirectory.Name))
                {
                    // Place the related textures
                    ExportTexturesForZone(curZone, curZoneDirectory, exportMPQRootFolder, relativeZoneObjectsPath, inputObjectTexturesFolder);

                    // Place the related music files
                    ExportMusicForZone(curZone, inputMusicFolderRoot, exportMPQRootFolder);

                    // Place the related ambience files
                    ExportAmbientSoundForZone(curZone, inputSoundFolderRoot, exportMPQRootFolder);
                }
                else
                    Logger.WriteDetail("For zone '" + zoneDirectory.Name + "', skipped texture and music copy since it wasn't in CONFIG_GENERATE_UPDATE_INCLUDED_ZONE_SHORTNAMES");

                zones.Add(curZone);
            }
            return true;
        }

        public bool ConvertCreatures(ref List<CreatureModelTemplate> creatureModelTemplates, ref List<CreatureTemplate> creatureTemplates, 
            ref List<CreatureSpawnPool> creatureSpawnPools, List<Zone> zones)
        {
            string eqExportsConditionedPath = Configuration.CONFIG_PATH_EQEXPORTSCONDITIONED_FOLDER;
            string wowExportPath = Configuration.CONFIG_PATH_EXPORT_FOLDER;

            Logger.WriteInfo("Converting EQ Creatures (skeletal objects) to WOW creature objects...");

            if (Configuration.CONFIG_CREATURE_ADD_ENTITY_ID_TO_NAME == true)
                Logger.WriteInfo("- Note: CONFIG_CREATURE_ADD_ENTITY_ID_TO_NAME is set to TRUE");

            // Recreate the output folder to clean it out
            string exportMPQRootFolder = Path.Combine(wowExportPath, "MPQReady");
            string exportAnimatedObjectsFolder = Path.Combine(exportMPQRootFolder, "Creature", "Everquest");
            if (Directory.Exists(exportAnimatedObjectsFolder))
                Directory.Delete(exportAnimatedObjectsFolder, true);
            Directory.CreateDirectory(exportAnimatedObjectsFolder);

            // Generate templates
            Dictionary<int, CreatureTemplate> creatureTemplatesByID = CreatureTemplate.GetCreatureTemplateList();
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
                        if (creatureTemplatesByID.ContainsKey(spawnEntry.CreatureTemplateID))
                            curSpawnPool.AddCreatureTemplate(creatureTemplatesByID[spawnEntry.CreatureTemplateID], spawnEntry.Chance);
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
            string inputSoundFolderRoot = Path.Combine(Configuration.CONFIG_PATH_EQEXPORTSCONDITIONED_FOLDER, "sounds");
            string exportCreatureSoundsDirectory = Path.Combine(Configuration.CONFIG_PATH_EXPORT_FOLDER, "MPQReady", "Sound", "Creature", "Everquest");
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

        public void ExtractClientDBCFiles()
        {
            string wowExportPath = Configuration.CONFIG_PATH_EXPORT_FOLDER;

            Logger.WriteInfo("Extracting client DBC files...");

            // Make sure the patches folder is correct
            string wowPatchesFolder = Path.Combine(Configuration.CONFIG_PATH_WOW_ENUS_CLIENT_FOLDER, "Data", "enUS");
            if (Directory.Exists(wowPatchesFolder) == false)
                throw new Exception("WoW client patches folder does not exist at '" + wowPatchesFolder + "', did you set CONFIG_PATH_WOW_ENUS_CLIENT_FOLDER?");

            // Get a list of valid patch files (it's done this way to ensure sorting order is exactly right). Also ignore existing patch file
            List<string> patchFileNames = new List<string>();
            patchFileNames.Add(Path.Combine(wowPatchesFolder, "patch-enUS.MPQ"));
            string[] existingPatchFiles = Directory.GetFiles(wowPatchesFolder, "patch-*-*.MPQ");
            foreach (string existingPatchName in existingPatchFiles)
                if (existingPatchName.Contains(Configuration.CONFIG_PATH_PATCH_NEW_FILE_NAME_NO_EXT) == false)
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
            string mpqEditorFullPath = Path.Combine(Configuration.CONFIG_PATH_TOOLS_FOLDER, "ladikmpqeditor", "MPQEditor.exe");
            if (File.Exists(mpqEditorFullPath) == false)
                throw new Exception("Failed to extract DBC files. '" + mpqEditorFullPath + "' does not exist. (Be sure to set your Configuration.CONFIG_PATH_TOOLS_FOLDER properly)");
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
            if (Directory.Exists(Configuration.CONFIG_PATH_EXPORT_FOLDER) == false)
                throw new Exception("Export folder '" + Configuration.CONFIG_PATH_EXPORT_FOLDER + "' did not exist, make sure you set CONFIG_PATH_EXPORT_FOLDER");

            // Delete the old patch file, if it exists
            Logger.WriteDetail("Deleting old patch file if it exists");
            string outputPatchFileName = Path.Combine(Configuration.CONFIG_PATH_EXPORT_FOLDER, Configuration.CONFIG_PATH_PATCH_NEW_FILE_NAME_NO_EXT + ".MPQ");
            if (File.Exists(outputPatchFileName) == true)
                File.Delete(outputPatchFileName);

            // Generate a script to generate the mpq file
            Logger.WriteDetail("Generating script to generate MPQ file");
            string mpqReadyFolder = Path.Combine(Configuration.CONFIG_PATH_EXPORT_FOLDER, "MPQReady");
            if (Directory.Exists(mpqReadyFolder) == false)
                throw new Exception("There was no MPQReady folder inside of '" + Configuration.CONFIG_PATH_EXPORT_FOLDER + "'");
            string workingGeneratedScriptsFolder = Path.Combine(Configuration.CONFIG_PATH_EXPORT_FOLDER, "GeneratedWorkingScripts");
            FileTool.CreateBlankDirectory(workingGeneratedScriptsFolder, true);
            StringBuilder mpqCreateScriptText = new StringBuilder();
            mpqCreateScriptText.AppendLine("new \"" + outputPatchFileName + "\" 65536");
            mpqCreateScriptText.AppendLine("add \"" + outputPatchFileName + "\" \"" + mpqReadyFolder + "\\*\" /auto /r");
            string mpqNewScriptFileName = Path.Combine(workingGeneratedScriptsFolder, "mpqnew.txt");
            using (var mpqNewScriptFile = new StreamWriter(mpqNewScriptFileName))
                mpqNewScriptFile.WriteLine(mpqCreateScriptText.ToString());

            // Generate the new MPQ using the script
            Logger.WriteDetail("Generating MPQ file");
            string mpqEditorFullPath = Path.Combine(Configuration.CONFIG_PATH_TOOLS_FOLDER, "ladikmpqeditor", "MPQEditor.exe");
            if (File.Exists(mpqEditorFullPath) == false)
                throw new Exception("Failed to generate MPQ file. '" + mpqEditorFullPath + "' does not exist. (Be sure to set your Configuration.CONFIG_PATH_TOOLS_FOLDER properly)");
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
            string exportMPQFileName = Path.Combine(Configuration.CONFIG_PATH_EXPORT_FOLDER, Configuration.CONFIG_PATH_PATCH_NEW_FILE_NAME_NO_EXT + ".mpq");
            if (File.Exists(exportMPQFileName) == false)
            {
                Logger.WriteError("Attempted to update the patch MPQ, but it didn't exist at '" + exportMPQFileName + "'");
                return;
            }

            // Generate a script to update the new MPQ
            Logger.WriteDetail("Generating script to update the MPQ file");
            string mpqReadyFolder = Path.Combine(Configuration.CONFIG_PATH_EXPORT_FOLDER, "MPQReady");
            if (Directory.Exists(mpqReadyFolder) == false)
                throw new Exception("There was no MPQReady folder inside of '" + Configuration.CONFIG_PATH_EXPORT_FOLDER + "'");
            string workingGeneratedScriptsFolder = Path.Combine(Configuration.CONFIG_PATH_EXPORT_FOLDER, "GeneratedWorkingScripts");
            FileTool.CreateBlankDirectory(workingGeneratedScriptsFolder, true);
            StringBuilder mpqUpdateScriptText = new StringBuilder();
            
            // Zones
            foreach(string zoneName in Configuration.CONFIG_GENERATE_UPDATE_BUILD_INCLUDED_ZONE_SHORTNAMES)
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

            // Objects
            if (Configuration.CONFIG_GENERATE_OBJECTS == true)
            {                
                string relativeStaticDoodadsPath = Path.Combine("World", "Everquest", "StaticDoodads");
                string fullStaticDoodadsPath = Path.Combine(mpqReadyFolder, relativeStaticDoodadsPath);
                mpqUpdateScriptText.AppendLine("add \"" + exportMPQFileName + "\" \"" + fullStaticDoodadsPath + "\" \"" + relativeStaticDoodadsPath + "\" /r");
            }

            // Creatures
            if (Configuration.CONFIG_GENERATE_CREATURES_AND_SPAWNS == true)
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
            string mpqEditorFullPath = Path.Combine(Configuration.CONFIG_PATH_TOOLS_FOLDER, "ladikmpqeditor", "MPQEditor.exe");
            if (File.Exists(mpqEditorFullPath) == false)
                throw new Exception("Failed to update MPQ file. '" + mpqEditorFullPath + "' does not exist. (Be sure to set your Configuration.CONFIG_PATH_TOOLS_FOLDER properly)");
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
            string sourcePatchFileNameAndPath = Path.Combine(Configuration.CONFIG_PATH_EXPORT_FOLDER, Configuration.CONFIG_PATH_PATCH_NEW_FILE_NAME_NO_EXT + ".MPQ");
            if (File.Exists(sourcePatchFileNameAndPath) == false)
            {
                Logger.WriteError("Failed to deploy to client. Patch at '" + sourcePatchFileNameAndPath + "' did not exist");
                return;
            }

            // Delete the old one if it's already deployed on the client
            string targetPatchFileNameAndPath = Path.Combine(Configuration.CONFIG_PATH_WOW_ENUS_CLIENT_FOLDER, "Data", "enUS", Configuration.CONFIG_PATH_PATCH_NEW_FILE_NAME_NO_EXT + ".MPQ");
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

        public void DeployServerFiles()
        {
            Logger.WriteInfo("Deploying files to server...");

            // Make sure source and target paths are good for DBC files
            string sourceServerDBCFolder = Path.Combine(Configuration.CONFIG_PATH_EXPORT_FOLDER, "DBCFilesServer");
            if (Directory.Exists(sourceServerDBCFolder) == false)
            {
                Logger.WriteError("Could not deploy DBC files to the server, no folder existed at '" + sourceServerDBCFolder + "'");
                return;
            }
            if (Directory.Exists(Configuration.CONFIG_PATH_DEPLOY_SERVER_DBC_FILES_FOLDER) == false)
            {
                Logger.WriteError("Could not deploy DBC files to the server, no target folder existed at '" + Configuration.CONFIG_PATH_DEPLOY_SERVER_DBC_FILES_FOLDER + "'. Check that you set Configuration.CONFIG_PATH_DEPLOY_SERVER_DBC_FILES_FOLDER properly");
                return;
            }

            // Deploy the DBC files
            Logger.WriteDetail("Deploying DBC files to the server...");
            string[] dbcFiles = Directory.GetFiles(sourceServerDBCFolder);
            foreach (string dbcFile in dbcFiles)
            {
                string targetFileName = Path.Combine(Configuration.CONFIG_PATH_DEPLOY_SERVER_DBC_FILES_FOLDER, Path.GetFileName(dbcFile));
                File.Copy(dbcFile, targetFileName, true);
            }


            Logger.WriteDetail("Deploying files to server complete");
        }
        
        public void DeployServerSQL()
        {
            Logger.WriteInfo("Deploying sql to server...");

            // Verify there is a scripts folder
            string sqlScriptFolder = Path.Combine(Configuration.CONFIG_PATH_EXPORT_FOLDER, "SQLScripts");
            if (Directory.Exists(sqlScriptFolder) == false)
            {
                Logger.WriteError("Could not deploy SQL scripts to server. Path '" + sqlScriptFolder + "' did not exist");
                return;
            }

            // Deploy all of the scripts
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Configuration.CONFIG_DEPLOY_SQL_CONNECTION_STRING_WORLD))
                {
                    connection.Open();
                    string[] sqlFiles = Directory.GetFiles(sqlScriptFolder);
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

        public void CreateWoWZoneFromEQZone(Zone zone, string exportMPQRootFolder, string relativeStaticDoodadsFolder,
            string relativeZoneObjectsFolder)
        {
            Logger.WriteDetail("- [" + zone.ShortName + "]: Converting zone '" + zone.ShortName + "' into a wow zone...");

            // Generate the WOW zone data first, always do it since IDs are based on it
            zone.LoadFromEQZone();

            if (Configuration.CONFIG_GENERATE_UPDATE_BUILD_INCLUDED_ZONE_SHORTNAMES.Count == 0 ||
                Configuration.CONFIG_GENERATE_UPDATE_BUILD_INCLUDED_ZONE_SHORTNAMES.Contains(zone.ShortName))
            {
                // Create the zone WMO objects
                WMO zoneWMO = new WMO(zone, exportMPQRootFolder, relativeStaticDoodadsFolder, relativeZoneObjectsFolder);
                zoneWMO.WriteToDisk(exportMPQRootFolder);

                // Create the WDT
                WDT zoneWDT = new WDT(zone, zoneWMO.RootFileRelativePathWithFileName);
                zoneWDT.WriteToDisk(exportMPQRootFolder);

                // Create the WDL
                WDL zoneWDL = new WDL(zone);
                zoneWDL.WriteToDisk(exportMPQRootFolder);

                // Create the zone-specific generated object files
                foreach (ObjectModel zoneObject in zone.GeneratedZoneObjects)
                {
                    // Recreate the folder if needed
                    string curZoneObjectRelativePath = Path.Combine(relativeZoneObjectsFolder, zoneObject.Name);
                    string curZoneObjectFolder = Path.Combine(exportMPQRootFolder, curZoneObjectRelativePath);
                    if (Directory.Exists(curZoneObjectFolder))
                        Directory.Delete(curZoneObjectFolder, true);
                    Directory.CreateDirectory(curZoneObjectFolder);

                    // Build this zone object M2 Data
                    M2 objectM2 = new M2(zoneObject, curZoneObjectRelativePath);
                    objectM2.WriteToDisk(zoneObject.Name, curZoneObjectFolder);
                }

                // Create the zone-specific sound instance objects
                foreach (ObjectModel zoneSoundInstanceObject in zone.SoundInstanceObjectModels)
                {
                    // Recreate the folder if needed
                    string curZoneObjectRelativePath = Path.Combine(relativeZoneObjectsFolder, zoneSoundInstanceObject.Name);
                    string curZoneObjectFolder = Path.Combine(exportMPQRootFolder, curZoneObjectRelativePath);
                    if (Directory.Exists(curZoneObjectFolder))
                        Directory.Delete(curZoneObjectFolder, true);
                    Directory.CreateDirectory(curZoneObjectFolder);

                    // Build this zone object M2 Data
                    M2 objectM2 = new M2(zoneSoundInstanceObject, curZoneObjectRelativePath);
                    objectM2.WriteToDisk(zoneSoundInstanceObject.Name, curZoneObjectFolder);
                }
            }
            else
                Logger.WriteDetail("For zone '" + zone.ShortName + "', skipped wow file generation since it wasn't in CONFIG_GENERATE_UPDATE_INCLUDED_ZONE_SHORTNAMES");

            Logger.WriteDetail("- [" + zone.ShortName + "]: Converting of zone '" + zone.ShortName + "' complete");
        }

        public void CreateLoadingScreens()
        {
            string eqExportsConditionedPath = Configuration.CONFIG_PATH_EQEXPORTSCONDITIONED_FOLDER;
            string exportMPQRootFolder = Path.Combine(Configuration.CONFIG_PATH_EXPORT_FOLDER, "MPQReady");

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
            string eqExportsConditionedPath = Configuration.CONFIG_PATH_EQEXPORTSCONDITIONED_FOLDER;
            string exportMPQRootFolder = Path.Combine(Configuration.CONFIG_PATH_EXPORT_FOLDER, "MPQReady");

            Logger.WriteInfo("Copying liquid material textures");

            string sourceTextureFolder = Path.Combine(eqExportsConditionedPath, "liquidsurfaces");
            string targetTextureFolder = Path.Combine(exportMPQRootFolder, "XTEXTURES", "everquest");
            if (Directory.Exists(targetTextureFolder) == true)
                Directory.Delete(targetTextureFolder, true);
            Directory.CreateDirectory(targetTextureFolder);
            FileTool.CopyDirectoryAndContents(sourceTextureFolder, targetTextureFolder, true, true, "*.blp");
        }

        public void CreateDBCFiles(List<Zone> zones, List<CreatureModelTemplate> creatureModelTemplates)
        {
            string wowExportPath = Configuration.CONFIG_PATH_EXPORT_FOLDER;

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
            FootstepTerrainLookupDBC footstepTerrainLookupDBC = new FootstepTerrainLookupDBC();
            footstepTerrainLookupDBC.LoadFromDisk(dbcInputFolder, "FootstepTerrainLookup.dbc");
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
            SoundAmbienceDBC soundAmbienceDBC = new SoundAmbienceDBC();
            soundAmbienceDBC.LoadFromDisk(dbcInputFolder, "SoundAmbience.dbc");
            SoundEntriesDBC soundEntriesDBC = new SoundEntriesDBC();
            soundEntriesDBC.LoadFromDisk(dbcInputFolder, "SoundEntries.dbc");
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
            loadingScreensDBC.AddRow(Configuration.CONFIG_DBCID_LOADINGSCREEN_ID_START, "EQAntonica", "Interface\\Glues\\LoadingScreens\\LoadingScreenEQClassic.blp");
            loadingScreensDBC.AddRow(Configuration.CONFIG_DBCID_LOADINGSCREEN_ID_START + 1, "EQKunark", "Interface\\Glues\\LoadingScreens\\LoadingScreenEQKunark.blp");
            loadingScreensDBC.AddRow(Configuration.CONFIG_DBCID_LOADINGSCREEN_ID_START + 2, "EQVelious", "Interface\\Glues\\LoadingScreens\\LoadingScreenEQVelious.blp");

            // Creatures sounds
            Dictionary<string, int> creatureFootstepIDBySoundNames = new Dictionary<string, int>();
            int curCreatureFootstepID = Configuration.CONFIG_DBCID_FOOTSTEPTERRAINLOOKUP_CREATUREFOOTSTEPID_START;
            foreach (CreatureModelTemplate creatureModelTemplate in creatureModelTemplates)
            {
                creatureDisplayInfoDBC.AddRow(creatureModelTemplate.DBCCreatureDisplayID, creatureModelTemplate.DBCCreatureModelDataID);
                string relativeModelPath = "Creature\\Everquest\\" + creatureModelTemplate.GetCreatureModelFolderName() + "\\" + creatureModelTemplate.GenerateFileName() + ".mdx";
                creatureModelDataDBC.AddRow(creatureModelTemplate, relativeModelPath);
                creatureSoundDataDBC.AddRow(creatureModelTemplate.DBCCreatureSoundDataID, creatureModelTemplate.Race, CreatureRace.FootstepIDBySoundName[creatureModelTemplate.Race.SoundWalkingName]);
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
                foreach (ItemTemplate itemTemplate in ItemTemplate.ItemTemplatesByEQDBID.Values)
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
                string creatureSoundsDirectory = "Sound\\Creature\\Everquest";
                foreach (var sound in CreatureRace.SoundsBySoundName)
                {
                    soundEntriesDBC.AddRow(sound.Value, sound.Value.Name, creatureSoundsDirectory);
                }

                // WMOAreaTable (Header than groups)
                wmoAreaTableDBC.AddRow(Convert.ToInt32(zoneProperties.DBCWMOID), Convert.ToInt32(-1), 0, Convert.ToInt32(zone.DefaultArea.DBCAreaTableID), zone.DescriptiveName); // Header record
                foreach (ZoneObjectModel wmo in zone.ZoneObjectModels)
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
            footstepTerrainLookupDBC.SaveToDisk(dbcOutputClientFolder);
            footstepTerrainLookupDBC.SaveToDisk(dbcOutputServerFolder);
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
            soundAmbienceDBC.SaveToDisk(dbcOutputClientFolder);
            soundAmbienceDBC.SaveToDisk(dbcOutputServerFolder);
            soundEntriesDBC.SaveToDisk(dbcOutputClientFolder);
            soundEntriesDBC.SaveToDisk(dbcOutputServerFolder);
            wmoAreaTableDBC.SaveToDisk(dbcOutputClientFolder);
            wmoAreaTableDBC.SaveToDisk(dbcOutputServerFolder);
            zoneMusicDBC.SaveToDisk(dbcOutputClientFolder);
            zoneMusicDBC.SaveToDisk(dbcOutputServerFolder);

            Logger.WriteDetail("Creating DBC Files complete");
        }

        public void CreateSQLScript(List<Zone> zones, List<CreatureTemplate> creatureTemplates, List<CreatureModelTemplate> creatureModelTemplates,
            List<CreatureSpawnPool> creatureSpawnPools)
        {
            Logger.WriteInfo("Creating SQL Scripts...");

            // Clean the folder
            string sqlScriptFolder = Path.Combine(Configuration.CONFIG_PATH_EXPORT_FOLDER, "SQLScripts");
            if (Directory.Exists(sqlScriptFolder))
                Directory.Delete(sqlScriptFolder, true);

            // Create the SQL Scripts
            AreaTriggerSQL areaTriggerSQL = new AreaTriggerSQL();
            AreaTriggerTeleportSQL areaTriggerTeleportSQL = new AreaTriggerTeleportSQL();
            CreatureSQL creatureSQL = new CreatureSQL();
            CreatureAddonSQL creatureAddonSQL = new CreatureAddonSQL();
            CreatureModelInfoSQL creatureModelInfoSQL = new CreatureModelInfoSQL();
            CreatureTemplateSQL creatureTemplateSQL = new CreatureTemplateSQL();
            CreatureTemplateModelSQL creatureTemplateModelSQL = new CreatureTemplateModelSQL();
            GameTeleSQL gameTeleSQL = new GameTeleSQL();
            InstanceTemplateSQL instanceTemplateSQL = new InstanceTemplateSQL();
            ItemTemplateSQL itemTemplateSQL = new ItemTemplateSQL();
            PoolCreatureSQL poolCreatureSQL = new PoolCreatureSQL();
            PoolPoolSQL poolPoolSQL = new PoolPoolSQL();
            PoolTemplateSQL poolTemplateSQL = new PoolTemplateSQL();
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

            // Creature Templates
            foreach (CreatureTemplate creatureTemplate in creatureTemplates)
            {
                if (creatureTemplate.ModelTemplate == null)
                    Logger.WriteError("Error generating azeroth core scripts since model template was null for creature template '" + creatureTemplate.Name + "'");
                else
                {
                    // Calculate the scale
                    float scale = creatureTemplate.Size * creatureTemplate.Race.SpawnSizeMod;

                    // Create the records
                    creatureTemplateSQL.AddRow(creatureTemplate, scale);
                    creatureTemplateModelSQL.AddRow(creatureTemplate.SQLCreatureTemplateID, creatureTemplate.ModelTemplate.DBCCreatureDisplayID, scale);
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
                        creatureSQL.AddRow(creatureGUID, creatureTemplate.SQLCreatureTemplateID, spawnInstance.MapID, spawnInstance.AreaID, spawnInstance.AreaID,
                            spawnInstance.SpawnXPosition, spawnInstance.SpawnYPosition, spawnInstance.SpawnZPosition, spawnInstance.Orientation, CreatureMovementType.Path);
                    }
                    else
                    {
                        creatureSQL.AddRow(creatureGUID, creatureTemplate.SQLCreatureTemplateID, spawnInstance.MapID, spawnInstance.AreaID, spawnInstance.AreaID,
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
                                creatureSQL.AddRow(creatureGUID, creatureTemplate.SQLCreatureTemplateID, spawnInstance.MapID, spawnInstance.AreaID, spawnInstance.AreaID,
                                    spawnInstance.SpawnXPosition, spawnInstance.SpawnYPosition, spawnInstance.SpawnZPosition, spawnInstance.Orientation, CreatureMovementType.Path);
                            }
                            else
                            {
                                creatureSQL.AddRow(creatureGUID, creatureTemplate.SQLCreatureTemplateID, spawnInstance.MapID, spawnInstance.AreaID, spawnInstance.AreaID,
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
                            creatureSQL.AddRow(creatureGUID, creatureTemplate.SQLCreatureTemplateID, spawnInstance.MapID, spawnInstance.AreaID, spawnInstance.AreaID,
                                spawnInstance.SpawnXPosition, spawnInstance.SpawnYPosition, spawnInstance.SpawnZPosition, spawnInstance.Orientation, CreatureMovementType.Path);
                        }
                        else
                        {
                            creatureSQL.AddRow(creatureGUID, creatureTemplate.SQLCreatureTemplateID, spawnInstance.MapID, spawnInstance.AreaID, spawnInstance.AreaID,
                                spawnInstance.SpawnXPosition, spawnInstance.SpawnYPosition, spawnInstance.SpawnZPosition, spawnInstance.Orientation, CreatureMovementType.None);
                        }
                    }
                }
            }

            // Items
            foreach (ItemTemplate itemTemplate in ItemTemplate.ItemTemplatesByEQDBID.Values)
                itemTemplateSQL.AddRow(itemTemplate);

            // Output them
            areaTriggerSQL.SaveToDisk("areatrigger");
            areaTriggerTeleportSQL.SaveToDisk("areatrigger_teleport");
            creatureSQL.SaveToDisk("creature");
            creatureAddonSQL.SaveToDisk("creature_addon");
            creatureModelInfoSQL.SaveToDisk("creature_model_info");
            creatureTemplateSQL.SaveToDisk("creature_template");
            creatureTemplateModelSQL.SaveToDisk("creature_template_model");
            gameTeleSQL.SaveToDisk("game_tele");
            instanceTemplateSQL.SaveToDisk("instance_template");
            itemTemplateSQL.SaveToDisk("item_template");
            poolCreatureSQL.SaveToDisk("pool_creature");
            poolPoolSQL.SaveToDisk("pool_pool");
            poolTemplateSQL.SaveToDisk("pool_template");
            waypointDataSQL.SaveToDisk("waypoint_data");
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
            string soundInstanceInputTextureFullPath = Path.Combine(inputObjectTextureFolder, Configuration.CONFIG_AUDIO_SOUNDINSTANCE_RENDEROBJECT_MATERIAL_NAME + ".blp");
            foreach (ObjectModel zoneObject in zone.SoundInstanceObjectModels)
            {
                string soundInstanceOutputTextureFullPath = Path.Combine(wowExportPath, relativeZoneMaterialDoodadsPath, zoneObject.Name, Configuration.CONFIG_AUDIO_SOUNDINSTANCE_RENDEROBJECT_MATERIAL_NAME + ".blp");
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

        public void CreateItems()
        {
            Logger.WriteInfo("Creating items...");
            ItemTemplate.PopulateItemTemplateListFromDisk();
            Logger.WriteInfo("Copying item icon files...");

            // Clear and create the directory
            string iconOutputFolder = Path.Combine(Configuration.CONFIG_PATH_EXPORT_FOLDER, "MPQReady", "Interface", "ICONS");
            if (Directory.Exists(iconOutputFolder) == true)
                Directory.Delete(iconOutputFolder, true);
            Directory.CreateDirectory(iconOutputFolder);

            // Copy all of the icons
            string iconInputFolder = Path.Combine(Configuration.CONFIG_PATH_EQEXPORTSCONDITIONED_FOLDER, "itemicons");
            foreach(ItemDisplayInfo itemDisplayInfo in ItemDisplayInfo.ItemDisplayInfos)
            {
                string sourceIconFile = Path.Combine(iconInputFolder, itemDisplayInfo.IconFileNameNoExt + ".blp");
                string outputIconFile = Path.Combine(iconOutputFolder, itemDisplayInfo.IconFileNameNoExt + ".blp");
                File.Copy(sourceIconFile, outputIconFile, true);
            }
        }
    }
}
