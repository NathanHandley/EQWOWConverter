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

using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using EQWOWConverter.WOWFiles;
using EQWOWConverter.Zones;
using EQWOWConverter.Common;
using EQWOWConverter.ObjectModels;
using Vector3 = EQWOWConverter.Common.Vector3;
using EQWOWConverter.WOWFiles.DBC;
using EQWOWConverter.ObjectModels.Properties;
using EQWOWConverter.Files.WOWFiles;
using static System.Net.Mime.MediaTypeNames;

namespace EQWOWConverter
{
    internal class AssetConverter
    {
        public bool ConvertEQDataToWOW(string eqExportsConditionedPath, string wowExportPath)
        {
            Logger.WriteInfo("Converting from EQ to WoW...");

            // Make sure the root path exists
            if (Directory.Exists(eqExportsConditionedPath) == false)
            {
                Logger.WriteError("Error - Conditioned path of '" + eqExportsConditionedPath + "' does not exist.");
                Logger.WriteError("Conversion Failed!");
                return false;
            }

            // Extract DBC files
            ExtractClientDBCFiles(wowExportPath);

            // Convert the data
            if (Configuration.CONFIG_GENERATE_OBJECTS == true)
            {
                if (ConvertEQObjectsToWOW(eqExportsConditionedPath, wowExportPath) == false)
                    return false;
            }
            else
            {
                Logger.WriteInfo("Note: Object generation is set to false in the Configuration");
            }
            if (ConvertEQZonesToWOW(eqExportsConditionedPath, wowExportPath) == false)
                return false;

            Logger.WriteInfo("Conversion of data complete");
            return true;
        }

        // TODO: Condense above
        public bool ConvertEQObjectsToWOW(string eqExportsConditionedPath, string wowExportPath)
        {
            Logger.WriteInfo("Converting EQ objects to WOW objects...");

            // Make sure the object folder path exists
            string objectFolderRoot = Path.Combine(eqExportsConditionedPath, "objects");
            if (Directory.Exists(objectFolderRoot) == false)
                Directory.CreateDirectory(objectFolderRoot);

            // Clean out the objects folder
            string exportMPQRootFolder = Path.Combine(wowExportPath, "MPQReady");
            string exportObjectsFolder = Path.Combine(exportMPQRootFolder, "World", "Everquest", "Objects");
            if (Directory.Exists(exportObjectsFolder))
                Directory.Delete(exportObjectsFolder, true);

            // Go through all of the object meshes and process them one at a time
            string objectMeshFolderRoot = Path.Combine(objectFolderRoot, "meshes");
            DirectoryInfo objectMeshDirectoryInfo = new DirectoryInfo(objectMeshFolderRoot);
            FileInfo[] objectMeshFileInfos = objectMeshDirectoryInfo.GetFiles();
            List<ObjectModel> staticObjects = new List<ObjectModel>();
            foreach(FileInfo objectMeshFileInfo in objectMeshFileInfos)
            {
                string staticObjectMeshNameNoExt = Path.GetFileNameWithoutExtension(objectMeshFileInfo.FullName);
                string curStaticObjectOutputFolder = Path.Combine(exportObjectsFolder, staticObjectMeshNameNoExt);

                // Skip the collision mesh files
                if (objectMeshFileInfo.Name.Contains("_collision"))
                    continue;

                // Load the EQ object
                ObjectModelProperties objectProperties = ObjectModelProperties.GetObjectPropertiesForObject(staticObjectMeshNameNoExt);
                ObjectModel curObject = new ObjectModel(staticObjectMeshNameNoExt, objectProperties);
                Logger.WriteDetail("- [" + staticObjectMeshNameNoExt + "]: Importing EQ static object '" + staticObjectMeshNameNoExt + "'");
                curObject.LoadEQObjectData(objectFolderRoot);
                Logger.WriteDetail("- [" + staticObjectMeshNameNoExt + "]: Importing EQ static object '" + staticObjectMeshNameNoExt + "' complete");

                // Covert to WOW static object
                string relativeMPQPath = Path.Combine("World", "Everquest", "Objects", staticObjectMeshNameNoExt);
                CreateWoWObjectFromEQObject(curObject, curStaticObjectOutputFolder, relativeMPQPath);

                // Place the related textures
                string objectTextureFolder = Path.Combine(objectFolderRoot, "textures");
                ExportTexturesForObject(curObject, objectTextureFolder, curStaticObjectOutputFolder);

                // Save the object
                staticObjects.Add(curObject);
            }

            // Create the DBC update scripts
            // TODO

            // Create the Azeroth Core Scripts
            // TODO?

            return true;
        }

        // TODO: Condense above
        public bool ConvertEQZonesToWOW(string eqExportsConditionedPath, string wowExportPath)
        {
            Logger.WriteInfo("Converting EQ zones to WOW zones...");

            // Make sure the zone folder path exists
            string zoneFolderRoot = Path.Combine(eqExportsConditionedPath, "zones");
            if (Directory.Exists(zoneFolderRoot) == false)
                Directory.CreateDirectory(zoneFolderRoot);
            string inputMusicFolderRoot = Path.Combine(eqExportsConditionedPath, "music");

            // Clean out the zone and interface folders
            string exportMPQRootFolder = Path.Combine(wowExportPath, "MPQReady");
            string exportMapsFolder = Path.Combine(exportMPQRootFolder, "World", "Maps");
            if (Directory.Exists(exportMapsFolder))
                Directory.Delete(exportMapsFolder, true);
            string exportWMOFolder = Path.Combine(exportMPQRootFolder, "World", "wmo");
            if (Directory.Exists(exportWMOFolder))
                Directory.Delete(exportWMOFolder, true);
            string zoneTexturesFolder = Path.Combine(exportMPQRootFolder, "World", "Everquest", "ZoneTextures");
            if (Directory.Exists(zoneTexturesFolder))
                Directory.Delete(zoneTexturesFolder, true);
            string exportInterfaceFolder = Path.Combine(exportMPQRootFolder, "Interface");
            if (Directory.Exists(exportInterfaceFolder))
                Directory.Delete(exportInterfaceFolder, true);
            string exportMusicFolder = Path.Combine(exportMPQRootFolder, "Sound", "Music");
            if (Directory.Exists(exportMusicFolder))
                Directory.Delete(exportMusicFolder, true);

            // Generate folder name for objects
            string exportObjectsFolderRelative = Path.Combine("World", "Everquest", "Objects");

            // Load shared environment settings
            ZoneProperties.CommonOutdoorEnvironmentProperties.SetAsOutdoors(77, 120, 143, ZoneFogType.Clear, true, 0.5f, 1.0f, ZoneSkySpecialType.None);

            // Go through the subfolders for each zone and convert to wow zone
            DirectoryInfo zoneRootDirectoryInfo = new DirectoryInfo(zoneFolderRoot);
            DirectoryInfo[] zoneDirectoryInfos = zoneRootDirectoryInfo.GetDirectories();
            List<Zone> zones = new List<Zone>();
            foreach (DirectoryInfo zoneDirectory in zoneDirectoryInfos)
            {
                // Skip any disabled expansions
                if (Configuration.CONFIG_GENERATE_KUNARK_ZONES == false && Configuration.CONFIG_GENERATE_KUNARK_ZONE_SHORTNAMES.Contains(zoneDirectory.Name))
                    continue;
                if (Configuration.CONFIG_GENERATE_VELIOUS_ZONES == false && Configuration.CONFIG_GENERATE_VELIOUS_ZONE_SHORTNAMES.Contains(zoneDirectory.Name))
                    continue;

                // Load the EQ zone
                ZoneProperties zoneProperties = ZoneProperties.GetZonePropertiesForZone(zoneDirectory.Name);
                Zone curZone = new Zone(zoneDirectory.Name, zoneProperties);
                Logger.WriteDetail("- [" + zoneDirectory.Name + "]: Importing EQ zone '" + zoneDirectory.Name);
                string curZoneDirectory = Path.Combine(zoneFolderRoot, zoneDirectory.Name);
                curZone.LoadEQZoneData(zoneDirectory.Name, curZoneDirectory);
                Logger.WriteDetail("- [" + zoneDirectory.Name + "]: Importing of EQ zone '" + zoneDirectory.Name + "' complete");

                // Convert to WOW zone
                CreateWoWZoneFromEQZone(curZone, exportMPQRootFolder, exportObjectsFolderRelative);

                // Place the related textures
                ExportTexturesForZone(curZone, curZoneDirectory, exportMPQRootFolder, exportObjectsFolderRelative);

                // Place the related music files
                ExportMusicForZone(curZone, inputMusicFolderRoot, exportMPQRootFolder);

                zones.Add(curZone);
            }

            // Copy the loading screens
            CreateLoadingScreens(eqExportsConditionedPath, exportMPQRootFolder);

            // Copy the liquid material textures
            CreateLiquidMaterials(eqExportsConditionedPath, exportMPQRootFolder);

            // Create the DBC update scripts
            CreateDBCUpdateScripts(zones, wowExportPath);

            // Create the DBC files
            CreateDBCFiles(zones, wowExportPath);

            // Create the Azeroth Core Scripts
            CreateAzerothCoreScripts(zones, wowExportPath);

            return true;
        }

        public void ExtractClientDBCFiles(string wowExportPath)
        {
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

            Logger.WriteInfo("Extracting client DBC files complete");
        }

        public void CreateAndReplaceGeneratedPatchMPQ(string wowExportPath)
        {
            Logger.WriteInfo("Building patch MPQ...");

            // Make sure the patches folder is correct
            string wowPatchesFolder = Path.Combine(Configuration.CONFIG_PATH_WOW_ENUS_CLIENT_FOLDER, "Data", "enUS");
            if (Directory.Exists(wowPatchesFolder) == false)
                throw new Exception("WoW client patches folder does not exist at '" + wowPatchesFolder + "', did you set CONFIG_PATH_WOW_ENUS_CLIENT_FOLDER?");

            // Delete the old patch file, if it exists
            Logger.WriteDetail("Deleting old patch file if it exists");
            string outputPatchFileName = Path.Combine(wowPatchesFolder, Configuration.CONFIG_PATH_PATCH_NEW_FILE_NAME_NO_EXT + ".MPQ");
            if (File.Exists(outputPatchFileName) == true)
                File.Delete(outputPatchFileName);

            // Generate a script to generate the mpq file
            Logger.WriteDetail("Generating script to generate MPQ file");
            string mpqReadyFolder = Path.Combine(wowExportPath, "MPQReady");
            if (Directory.Exists(mpqReadyFolder) == false)
                throw new Exception("There was no MPQReady folder inside of '" + wowExportPath + "'");
            string workingGeneratedScriptsFolder = Path.Combine(wowExportPath, "GeneratedWorkingScripts");
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

            Logger.WriteInfo("Building patch MPQ complete");
        }

        public void CreateWoWZoneFromEQZone(Zone zone, string exportMPQRootFolder, string relativeExportObjectsFolder)
        {
            Logger.WriteDetail("- [" + zone.ShortName + "]: Converting zone '" + zone.ShortName + "' into a wow zone...");

            // Generate the WOW zone data first
            zone.LoadFromEQZone();

            // Create the zone WMO objects
            WMO zoneWMO = new WMO(zone, exportMPQRootFolder, relativeExportObjectsFolder);
            zoneWMO.WriteToDisk(exportMPQRootFolder);

            // Create the WDT
            WDT zoneWDT = new WDT(zone, zoneWMO.RootFileRelativePathWithFileName);
            zoneWDT.WriteToDisk(exportMPQRootFolder);

            // Create the WDL
            WDL zoneWDL = new WDL(zone);
            zoneWDL.WriteToDisk(exportMPQRootFolder);

            // Create the zone-specific object files
            foreach(ObjectModel zoneObject in zone.GeneratedZoneObjects)
            {
                // Recreate the folder if needed
                string curZoneObjectRelativePath = Path.Combine(relativeExportObjectsFolder, zoneObject.Name);
                string curZoneObjectFolder = Path.Combine(exportMPQRootFolder, curZoneObjectRelativePath);
                if (Directory.Exists(curZoneObjectFolder))
                    Directory.Delete(curZoneObjectFolder, true);
                Directory.CreateDirectory(curZoneObjectFolder);

                // Build this zone object M2 Data
                M2 objectM2 = new M2(zoneObject, curZoneObjectRelativePath);
                objectM2.WriteToDisk(zoneObject.Name, curZoneObjectFolder);
            }

            Logger.WriteDetail("- [" + zone.ShortName + "]: Converting of zone '" + zone.ShortName + "' complete");
        }

        public void CreateWoWObjectFromEQObject(ObjectModel modelObject, string exportMPQObjectRootFolder, string mpqObjectPathRelative)
        {
            Logger.WriteDetail("- [" + modelObject.Name + "]: Converting object '" + modelObject.Name + "' into a wow object...");

            // Generate the object
            modelObject.PopulateObjectModelFromEQObjectModelData();

            // Create the M2 and Skin
            M2 objectM2 = new M2(modelObject, mpqObjectPathRelative);
            objectM2.WriteToDisk(modelObject.Name, exportMPQObjectRootFolder);

            Logger.WriteDetail("- [" + modelObject.Name + "]: Converting of object '" + modelObject.Name + "' complete");
        }

        public void CreateLoadingScreens(string eqExportsConditionedPath, string exportMPQRootFolder)
        {
            Logger.WriteInfo("Copying loading screens");
            string loadingScreensTextureFolder = Path.Combine(exportMPQRootFolder, "Interface", "Glues", "LoadingScreens");
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

        public void CreateLiquidMaterials(string eqExportsConditionedPath, string exportMPQRootFolder)
        {
            Logger.WriteInfo("Copying liquid material textures");

            string sourceTextureFolder = Path.Combine(eqExportsConditionedPath, "liquidsurfaces");
            string targetTextureFolder = Path.Combine(exportMPQRootFolder, "XTEXTURES", "everquest");
            Directory.CreateDirectory(targetTextureFolder);
            FileTool.CopyDirectoryAndContents(sourceTextureFolder, targetTextureFolder, true, true, "*.blp");
        }

        public void CreateDBCUpdateScripts(List<Zone> zones, string wowExportPath)
        {
            Logger.WriteInfo("Creating DBC Update Scripts...");

            // Clear prior folders
            string dbcPatchUpdateScriptFolder = Path.Combine(wowExportPath, "DBCUpdateScripts-Patch");
            if (Directory.Exists(dbcPatchUpdateScriptFolder) == true)
                Directory.Delete(dbcPatchUpdateScriptFolder, true);
            Directory.CreateDirectory(dbcPatchUpdateScriptFolder);
            string dbcServerUpdateScriptFolder = Path.Combine(wowExportPath, "DBCUpdateScripts-Server");
            if (Directory.Exists(dbcServerUpdateScriptFolder) == true)
                Directory.Delete(dbcServerUpdateScriptFolder, true);
            Directory.CreateDirectory(dbcServerUpdateScriptFolder);

            // Populate the loading screens script
            LoadingScreensDBC loadingScreensDBC = new LoadingScreensDBC();
            loadingScreensDBC.AddRow(Configuration.CONFIG_DBCID_LOADINGSCREENID_START, "EQAntonica", "Interface\\Glues\\LoadingScreens\\LoadingScreenEQClassic.blp");
            loadingScreensDBC.AddRow(Configuration.CONFIG_DBCID_LOADINGSCREENID_START + 1, "EQAntonica", "Interface\\Glues\\LoadingScreens\\LoadingScreenEQKunark.blp");
            loadingScreensDBC.AddRow(Configuration.CONFIG_DBCID_LOADINGSCREENID_START + 2, "EQAntonica", "Interface\\Glues\\LoadingScreens\\LoadingScreenEQVelious.blp");

            // Create the map-level DBC update scripts
            LightFloatBandDBC lightFloatBandDBC = new LightFloatBandDBC();
            LightIntBandDBC lightIntBandDBC = new LightIntBandDBC();
            LightParamsDBC lightParamsDBC = new LightParamsDBC();
            LiquidTypeDBC liquidTypeDBC = new LiquidTypeDBC();
            MapDifficultyDBC mapDifficultyDBC = new MapDifficultyDBC();
            SoundEntriesDBC soundEntriesDBC = new SoundEntriesDBC();
            WMOAreaTableDBC wmoAreaTableDBC = new WMOAreaTableDBC();
            ZoneMusicDBC zoneMusicDBC = new ZoneMusicDBC();

            // Save the common outdoor properties
            lightParamsDBC.AddRow(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersClearWeather);
            lightIntBandDBC.AddRows(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersClearWeather);
            lightFloatBandDBC.AddRows(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersClearWeather);
            lightParamsDBC.AddRow(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersClearWeatherUnderwater);
            lightIntBandDBC.AddRows(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersClearWeatherUnderwater);
            lightFloatBandDBC.AddRows(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersClearWeatherUnderwater);
            lightParamsDBC.AddRow(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersStormyWeather);
            lightIntBandDBC.AddRows(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersStormyWeather);
            lightFloatBandDBC.AddRows(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersStormyWeather);
            lightParamsDBC.AddRow(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersStormyWeatherUnderwater);
            lightIntBandDBC.AddRows(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersStormyWeatherUnderwater);
            lightFloatBandDBC.AddRows(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersStormyWeatherUnderwater);

            // Output the zonewide light properties for all possible known zones
            foreach (var zonePropertiesByShortname in ZoneProperties.ZonePropertyListByShortName)
            {
                // Default to the outdoor one
                ZoneProperties zoneProperties = zonePropertiesByShortname.Value;                
                if (zoneProperties.CustomZonewideEnvironmentProperties != null)
                {
                    ZoneEnvironmentSettings curZoneEnvironmentSettings = zoneProperties.CustomZonewideEnvironmentProperties;

                    lightParamsDBC.AddRow(curZoneEnvironmentSettings.ParamatersClearWeather);
                    lightIntBandDBC.AddRows(curZoneEnvironmentSettings.ParamatersClearWeather);
                    lightFloatBandDBC.AddRows(curZoneEnvironmentSettings.ParamatersClearWeather);

                    lightParamsDBC.AddRow(curZoneEnvironmentSettings.ParamatersClearWeatherUnderwater);
                    lightIntBandDBC.AddRows(curZoneEnvironmentSettings.ParamatersClearWeatherUnderwater);
                    lightFloatBandDBC.AddRows(curZoneEnvironmentSettings.ParamatersClearWeatherUnderwater);

                    lightParamsDBC.AddRow(curZoneEnvironmentSettings.ParamatersStormyWeather);
                    lightIntBandDBC.AddRows(curZoneEnvironmentSettings.ParamatersStormyWeather);
                    lightFloatBandDBC.AddRows(curZoneEnvironmentSettings.ParamatersStormyWeather);

                    lightParamsDBC.AddRow(curZoneEnvironmentSettings.ParamatersStormyWeatherUnderwater);
                    lightIntBandDBC.AddRows(curZoneEnvironmentSettings.ParamatersStormyWeatherUnderwater);
                    lightFloatBandDBC.AddRows(curZoneEnvironmentSettings.ParamatersStormyWeatherUnderwater);
                }
            }

            // Zone-specific records
            foreach (Zone zone in zones)
            {
                ZoneProperties zoneProperties = zone.ZoneProperties;

                // Map & Map Difficulty
                mapDifficultyDBC.AddRow(zoneProperties.DBCMapID, zoneProperties.DBCMapDifficultyID);
                
                // WMOAreaTable (Header than groups)
                wmoAreaTableDBC.AddRow(Convert.ToInt32(zoneProperties.DBCWMOID), Convert.ToInt32(-1), 0, Convert.ToInt32(zone.DefaultArea.DBCAreaTableID), zone.DescriptiveName); // Header record
                foreach (ZoneObjectModel wmo in zone.ZoneObjectModels)
                    wmoAreaTableDBC.AddRow(Convert.ToInt32(zoneProperties.DBCWMOID), Convert.ToInt32(wmo.WMOGroupID), 0, Convert.ToInt32(wmo.AreaTableID), wmo.DisplayName);

                // SoundEntries
                string musicDirectory = "Sound\\Music\\Everquest\\" + zoneProperties.ShortName;
                foreach (var sound in zone.MusicSoundsByFileNameNoExt)
                {
                    string fileNameWithExt = sound.Value.AudioFileNameNoExt + ".mp3";
                    soundEntriesDBC.AddRow(sound.Value, fileNameWithExt, musicDirectory);
                }

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

            // Output them for both patch and server
            mapDifficultyDBC.WriteToDisk(dbcPatchUpdateScriptFolder);
            mapDifficultyDBC.WriteToDisk(dbcServerUpdateScriptFolder);
            liquidTypeDBC.WriteToDisk(dbcPatchUpdateScriptFolder);
            liquidTypeDBC.WriteToDisk(dbcServerUpdateScriptFolder);
            lightFloatBandDBC.WriteToDisk(dbcPatchUpdateScriptFolder);
            lightFloatBandDBC.WriteToDisk(dbcServerUpdateScriptFolder);
            lightIntBandDBC.WriteToDisk(dbcPatchUpdateScriptFolder);
            lightIntBandDBC.WriteToDisk(dbcServerUpdateScriptFolder);
            lightParamsDBC.WriteToDisk(dbcPatchUpdateScriptFolder);
            lightParamsDBC.WriteToDisk(dbcServerUpdateScriptFolder);
            loadingScreensDBC.WriteToDisk(dbcPatchUpdateScriptFolder);
            loadingScreensDBC.WriteToDisk(dbcServerUpdateScriptFolder);
            soundEntriesDBC.WriteToDisk(dbcPatchUpdateScriptFolder);
            soundEntriesDBC.WriteToDisk(dbcServerUpdateScriptFolder);
            wmoAreaTableDBC.WriteToDisk(dbcPatchUpdateScriptFolder);
            wmoAreaTableDBC.WriteToDisk(dbcServerUpdateScriptFolder);
            zoneMusicDBC.WriteToDisk(dbcPatchUpdateScriptFolder);
            zoneMusicDBC.WriteToDisk(dbcServerUpdateScriptFolder);
        }

        public void CreateDBCFiles(List<Zone> zones, string wowExportPath)
        {
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
            LightDBC lightDBC = new LightDBC();
            lightDBC.LoadFromDisk(dbcInputFolder, "Light.dbc");
            MapDBC mapDBCClient = new MapDBC();
            mapDBCClient.LoadFromDisk(dbcInputFolder, "Map.dbc");
            MapDBC mapDBCServer = new MapDBC();
            mapDBCServer.LoadFromDisk(dbcInputFolder, "Map.dbc");

            // Zone-specific records
            foreach (Zone zone in zones)
            {
                ZoneProperties zoneProperties = zone.ZoneProperties;

                // AreaTable
                areaTableDBC.AddRow(Convert.ToInt32(zone.DefaultArea.DBCAreaTableID), 0, zone.DefaultArea.AreaMusic, zone.DefaultArea.DisplayName);
                foreach (ZoneArea subArea in zoneProperties.ZoneAreas)
                    areaTableDBC.AddRow(Convert.ToInt32(subArea.DBCAreaTableID), Convert.ToInt32(zone.DefaultArea.DBCAreaTableID), subArea.AreaMusic, subArea.DisplayName);

                // AreaTrigger
                foreach (ZonePropertiesZoneLineBox zoneLine in zoneProperties.ZoneLineBoxes)
                    areaTriggerDBC.AddRow(zoneLine.AreaTriggerID, zoneProperties.DBCMapID, zoneLine.BoxPosition.X, zoneLine.BoxPosition.Y,
                        zoneLine.BoxPosition.Z, zoneLine.BoxLength, zoneLine.BoxWidth, zoneLine.BoxHeight, zoneLine.BoxOrientation);

                // Light Tables
                if (zoneProperties.CustomZonewideEnvironmentProperties != null)
                {
                    ZoneEnvironmentSettings curZoneEnvironmentSettings = zoneProperties.CustomZonewideEnvironmentProperties;

                    lightDBC.AddRow(zoneProperties.DBCMapID, curZoneEnvironmentSettings);
                }
                else
                {
                    lightDBC.AddRow(zoneProperties.DBCMapID, ZoneProperties.CommonOutdoorEnvironmentProperties);
                }

                // Map
                mapDBCClient.AddRow(zoneProperties.DBCMapID, "EQ_" + zone.ShortName, zone.DescriptiveName, 0, zone.LoadingScreenID);
                mapDBCServer.AddRow(zoneProperties.DBCMapID, "EQ_" + zone.ShortName, zone.DescriptiveName, Convert.ToInt32(zone.DefaultArea.DBCAreaTableID), zone.LoadingScreenID);
            }

            // Save the files
            areaTableDBC.SaveToDisk(dbcOutputClientFolder);
            areaTableDBC.SaveToDisk(dbcOutputServerFolder);
            areaTriggerDBC.SaveToDisk(dbcOutputClientFolder);
            areaTriggerDBC.SaveToDisk(dbcOutputServerFolder);
            lightDBC.SaveToDisk(dbcOutputClientFolder);
            lightDBC.SaveToDisk(dbcOutputServerFolder);
            mapDBCClient.SaveToDisk(dbcOutputClientFolder);
            mapDBCServer.SaveToDisk(dbcOutputServerFolder);

            Logger.WriteInfo("Creating DBC Files complete");
        }

        public void CreateAzerothCoreScripts(List<Zone> zones, string wowExportPath)
        {
            Logger.WriteInfo("Creating AzerothCore SQL Scripts...");

            string sqlScriptFolder = Path.Combine(wowExportPath, "AzerothCoreSQLScripts");

            // Create the SQL Scripts
            GameTeleSQL gameTeleSQL = new GameTeleSQL();
            InstanceTemplateSQL instanceTemplateSQL = new InstanceTemplateSQL();
            AreaTriggerSQL areaTriggerSQL = new AreaTriggerSQL();
            AreaTriggerTeleportSQL areaTriggerTeleportSQL = new AreaTriggerTeleportSQL();

            foreach (Zone zone in zones)
            {
                // Teleport scripts to safe positions (add a record for both descriptive and short name if they are different)
                gameTeleSQL.AddRow(Convert.ToInt32(zone.ZoneProperties.DBCMapID), zone.DescriptiveNameOnlyLetters, zone.SafePosition.Y, zone.SafePosition.Y, zone.SafePosition.Z);
                if (zone.DescriptiveNameOnlyLetters.ToLower() != zone.ShortName.ToLower())
                    gameTeleSQL.AddRow(Convert.ToInt32(zone.ZoneProperties.DBCMapID), zone.ShortName, zone.SafePosition.Y, zone.SafePosition.Y, zone.SafePosition.Z);

                // Instance list
                instanceTemplateSQL.AddRow(Convert.ToInt32(zone.ZoneProperties.DBCMapID));

                // Zone lines
                foreach(ZonePropertiesZoneLineBox zoneLine in ZoneProperties.GetZonePropertiesForZone(zone.ShortName).ZoneLineBoxes)
                {
                    if (ZoneProperties.ZonePropertyListByShortName.ContainsKey(zoneLine.TargetZoneShortName) == false)
                    {
                        Logger.WriteError("Error!  When attempting to map a zone line, there was no zone with short name '" + zoneLine.TargetZoneShortName + "'");
                        continue;
                    }

                    // Area Trigger Teleport
                    int areaTriggerID = zoneLine.AreaTriggerID;
                    string descriptiveName = "EQ " + zone.ShortName + " - " + zoneLine.TargetZoneShortName + " zone line";
                    int targetMapId = ZoneProperties.GetZonePropertiesForZone(zoneLine.TargetZoneShortName).DBCMapID;
                    float targetPositionX = zoneLine.TargetZonePosition.X;
                    float targetPositionY = zoneLine.TargetZonePosition.Y;
                    float targetPositionZ = zoneLine.TargetZonePosition.Z;
                    float targetOrientation = zoneLine.TargetZoneOrientation;
                    areaTriggerTeleportSQL.AddRow(areaTriggerID, descriptiveName, targetMapId, targetPositionX, targetPositionY, targetPositionZ, targetOrientation);

                    // Area Trigger
                    areaTriggerSQL.AddRow(zoneLine.AreaTriggerID, zone.ZoneProperties.DBCMapID, zoneLine.BoxPosition.X, zoneLine.BoxPosition.Y,
                        zoneLine.BoxPosition.Z, zoneLine.BoxLength, zoneLine.BoxWidth, zoneLine.BoxHeight, zoneLine.BoxOrientation);
                }
            }

            // Output them
            gameTeleSQL.WriteToDisk(sqlScriptFolder);
            instanceTemplateSQL.WriteToDisk(sqlScriptFolder);
            areaTriggerSQL.WriteToDisk(sqlScriptFolder);
            areaTriggerTeleportSQL.WriteToDisk(sqlScriptFolder);
        }

        public void ExportTexturesForZone(Zone zone, string zoneInputFolder, string wowExportPath, string objectExportPath)
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
                    string outputTextureFullPath = Path.Combine(wowExportPath, objectExportPath, zoneObject.Name, texture.TextureName + ".blp");
                    if (File.Exists(sourceTextureFullPath) == false)
                    {
                        Logger.WriteError("Could not copy texture '" + sourceTextureFullPath + "', it did not exist. Did you run blpconverter?");
                        continue;
                    }
                    File.Copy(sourceTextureFullPath, outputTextureFullPath, true);
                    Logger.WriteDetail("- [" + zone.ShortName + "]: Texture named '" + texture.TextureName + "' copied");
                }
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
    }
}
