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
using EQWOWConverter.Objects;
using Vector3 = EQWOWConverter.Common.Vector3;
using EQWOWConverter.WOWFiles.DBC;
using EQWOWConverter.ModelObjects;

namespace EQWOWConverter
{
    internal class AssetConverter
    {
        public static bool ConvertEQObjectsToWOW(string eqExportsConditionedPath, string wowExportPath)
        {
            Logger.WriteLine("Converting EQ objects to WOW zones...");

            // Make sure the root path exists
            if (Directory.Exists(eqExportsConditionedPath) == false)
            {
                Logger.WriteLine("ERROR - Conditioned path of '" + eqExportsConditionedPath + "' does not exist.");
                Logger.WriteLine("Conversion Failed!");
                return false;
            }

            // Make sure the object folder path exists
            string objectFolderRoot = Path.Combine(eqExportsConditionedPath, "objects");
            if (Directory.Exists(objectFolderRoot) == false)
            {
                Logger.WriteLine("ERROR - Object folder that should be at path '" + objectFolderRoot + "' does not exist.");
                Logger.WriteLine("Conversion Failed!");
                return false;
            }

            // Clean out the objects folder
            string exportMPQRootFolder = Path.Combine(wowExportPath, "MPQReady");
            string exportObjectsFolder = Path.Combine(exportMPQRootFolder, "World", "Everquest", "Objects");
            if (Directory.Exists(exportObjectsFolder))
                Directory.Delete(exportObjectsFolder, true);

            // Go through all of the object meshes and process them one at a time
            string objectMeshFolderRoot = Path.Combine(objectFolderRoot, "meshes");
            DirectoryInfo objectMeshDirectoryInfo = new DirectoryInfo(objectMeshFolderRoot);
            FileInfo[] objectMeshFileInfos = objectMeshDirectoryInfo.GetFiles();
            List<ModelObject> staticObjects = new List<ModelObject>();
            foreach(FileInfo objectMeshFileInfo in objectMeshFileInfos)
            {
                string staticObjectMeshNameNoExt = Path.GetFileNameWithoutExtension(objectMeshFileInfo.FullName);
                string curStaticObjectOutputFolder = Path.Combine(exportObjectsFolder, staticObjectMeshNameNoExt);

                // Skip the collision mesh files
                if (objectMeshFileInfo.Name.Contains("_collision"))
                    continue;

                // Restrict to one object for testing
                //if (staticObjectMeshNameNoExt != "oggrug")
                //    continue;

                // Load the EQ object
                ModelObject curObject = new ModelObject(staticObjectMeshNameNoExt);
                Logger.WriteLine("- [" + staticObjectMeshNameNoExt + "]: Importing EQ static object '" + staticObjectMeshNameNoExt + "'");
                curObject.LoadEQObjectData(staticObjectMeshNameNoExt, objectFolderRoot);
                Logger.WriteLine("- [" + staticObjectMeshNameNoExt + "]: Importing EQ static object '" + staticObjectMeshNameNoExt + "' complete");

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

            Logger.WriteLine("Conversion Successful");
            return true;
        }

        public static bool ConvertEQZonesToWOW(string eqExportsConditionedPath, string wowExportPath)
        {
            Logger.WriteLine("Converting EQ zones to WOW zones...");

            // Make sure the root path exists
            if (Directory.Exists(eqExportsConditionedPath) == false)
            {
                Logger.WriteLine("ERROR - Conditioned path of '" + eqExportsConditionedPath + "' does not exist.");
                Logger.WriteLine("Conversion Failed!");
                return false;
            }

            // Make sure the zone folder path exists
            string zoneFolderRoot = Path.Combine(eqExportsConditionedPath, "zones");
            if (Directory.Exists(zoneFolderRoot) == false)
            {
                Logger.WriteLine("ERROR - Zone folder that should be at path '" + zoneFolderRoot + "' does not exist.");
                Logger.WriteLine("Conversion Failed!");
                return false;
            }

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

            // Generate folder name for objects
            string exportObjectsFolderRelative = Path.Combine("World", "Everquest", "Objects");

            // Go through the subfolders for each zone and convert to wow zone
            DirectoryInfo zoneRootDirectoryInfo = new DirectoryInfo(zoneFolderRoot);
            DirectoryInfo[] zoneDirectoryInfos = zoneRootDirectoryInfo.GetDirectories();
            List<Zone> zones = new List<Zone>();
            foreach (DirectoryInfo zoneDirectory in zoneDirectoryInfos)
            {
                // Skip any disabled expansions
                if (Configuration.CONFIG_GENERATE_KUNARK_ZONES == false && Configuration.CONFIG_LOOKUP_KUNARK_ZONE_SHORTNAMES.Contains(zoneDirectory.Name))
                    continue;
                if (Configuration.CONFIG_GENERATE_VELIOUS_ZONES == false && Configuration.CONFIG_LOOKUP_VELIOUS_ZONE_SHORTNAMES.Contains(zoneDirectory.Name))
                    continue;

                //if (zoneDirectory.Name != "freportw")
                //    continue;

                // Load the EQ zone
                Zone curZone = new Zone(zoneDirectory.Name);
                Logger.WriteLine("- [" + zoneDirectory.Name + "]: Importing EQ zone '" + zoneDirectory.Name);
                string curZoneDirectory = Path.Combine(zoneFolderRoot, zoneDirectory.Name);
                curZone.LoadEQZoneData(zoneDirectory.Name, curZoneDirectory);
                Logger.WriteLine("- [" + zoneDirectory.Name + "]: Importing of EQ zone '" + zoneDirectory.Name + "' complete");

                // Convert to WOW zone
                CreateWoWZoneFromEQZone(curZone, exportMPQRootFolder, exportObjectsFolderRelative);

                // Place the related textures
                ExportTexturesForZone(curZone, curZoneDirectory, exportMPQRootFolder);

                zones.Add(curZone);
            }

            // Copy the loading screens
            CreateLoadingScreens(eqExportsConditionedPath, exportMPQRootFolder);

            // Create the DBC update scripts
            CreateDBCUpdateScripts(zones, wowExportPath);

            // Create the Azeroth Core Scripts
            CreateAzerothCoreScripts(zones, wowExportPath);

            Logger.WriteLine("Conversion Successful");
            return true;
        }

        public static void CreateWoWZoneFromEQZone(Zone zone, string exportMPQRootFolder, string exportObjectsFolder)
        {
            Logger.WriteLine("- [" + zone.ShortName + "]: Converting zone '" + zone.ShortName + "' into a wow zone...");

            // Capture any zone properties
            ZoneProperties zoneProperties = ZoneProperties.GetZonePropertiesForZone(zone.ShortName);

            // Generate the WOW zone data first
            zone.PopulateWOWZoneDataFromEQZoneData(zoneProperties);

            // Create the zone WMO objects
            WMO zoneWMO = new WMO(zone, exportMPQRootFolder, exportObjectsFolder);
            zoneWMO.WriteToDisk(exportMPQRootFolder);

            // Create the WDT
            WDT zoneWDT = new WDT(zone, zoneWMO.RootFileRelativePathWithFileName);
            zoneWDT.WriteToDisk(exportMPQRootFolder);

            // Create the WDL
            WDL zoneWDL = new WDL(zone);
            zoneWDL.WriteToDisk(exportMPQRootFolder);

            Logger.WriteLine("- [" + zone.ShortName + "]: Converting of zone '" + zone.ShortName + "' complete");
        }

        public static void CreateWoWObjectFromEQObject(ModelObject modelObject, string exportMPQObjectRootFolder, string mpqObjectPathRelative)
        {
            Logger.WriteLine("- [" + modelObject.Name + "]: Converting object '" + modelObject.Name + "' into a wow object...");

            // Generate the object
            modelObject.PopulateWOWModelObjectDataFromEQModelObjectData();

            // Create the M2 and Skin
            M2 objectM2 = new M2(modelObject, mpqObjectPathRelative);
            objectM2.WriteToDisk(modelObject.Name, exportMPQObjectRootFolder);

            Logger.WriteLine("- [" + modelObject.Name + "]: Converting of object '" + modelObject.Name + "' complete");
        }

        public static void CreateLoadingScreens(string eqExportsConditionedPath, string exportMPQRootFolder)
        {
            Logger.WriteLine("Copying loading screens");
            string loadingScreensTextureFolder = Path.Combine(exportMPQRootFolder, "Interface", "Glues", "LoadingScreens");
            Directory.CreateDirectory(loadingScreensTextureFolder);

            // Classic
            string classicInputFile = Path.Combine(eqExportsConditionedPath, "miscimages", "logo03resized.blp");
            string classicOutputFile = Path.Combine(loadingScreensTextureFolder, "LoadingScreenEQClassic.blp");
            File.Copy(classicInputFile, classicOutputFile);

            // Kunark
            string kunarkInputFile = Path.Combine(eqExportsConditionedPath, "miscimages", "eqkloadresized.blp");
            string kunarkOutputFile = Path.Combine(loadingScreensTextureFolder, "LoadingScreenEQKunark.blp");
            File.Copy(kunarkInputFile, kunarkOutputFile);

            // Velious
            string veliousInputFile = Path.Combine(eqExportsConditionedPath, "miscimages", "eqvloadresized.blp");
            string veliousOutputFile = Path.Combine(loadingScreensTextureFolder, "LoadingScreenEQVelious.blp");
            File.Copy(veliousInputFile, veliousOutputFile);
        }

        public static void CreateDBCUpdateScripts(List<Zone> zones, string wowExportPath)
        {
            Logger.WriteLine("Creating DBC Update Scripts...");

            string dbcUpdateScriptFolder = Path.Combine(wowExportPath, "DBCUpdateScripts");

            // Populate the loading screens script
            LoadingScreensDBC loadingScreensDBC = new LoadingScreensDBC();
            loadingScreensDBC.AddRow(Configuration.CONFIG_DBCID_LOADINGSCREENID_START, "EQAntonica", "Interface\\Glues\\LoadingScreens\\LoadingScreenEQClassic.blp");
            loadingScreensDBC.AddRow(Configuration.CONFIG_DBCID_LOADINGSCREENID_START + 1, "EQAntonica", "Interface\\Glues\\LoadingScreens\\LoadingScreenEQKunark.blp");
            loadingScreensDBC.AddRow(Configuration.CONFIG_DBCID_LOADINGSCREENID_START + 2, "EQAntonica", "Interface\\Glues\\LoadingScreens\\LoadingScreenEQVelious.blp");

            // Create the map-level DBC update scripts
            AreaTableDBC areaTableDBC = new AreaTableDBC();
            MapDBC mapDBC = new MapDBC();
            MapDifficultyDBC difficultyDBC = new MapDifficultyDBC();
            WMOAreaTableDBC wmoAreaTableDBC = new WMOAreaTableDBC();
            AreaTriggerDBC areaTriggerDBC = new AreaTriggerDBC();
            foreach (Zone zone in zones)
            {
                areaTableDBC.AddRow(Convert.ToInt32(zone.WOWZoneData.AreaID), zone.DescriptiveName);
                mapDBC.AddRow(zone.WOWZoneData.MapID, "EQ_" + zone.ShortName, zone.DescriptiveName, Convert.ToInt32(zone.WOWZoneData.AreaID), zone.WOWZoneData.LoadingScreenID);
                difficultyDBC.AddRow(zone.WOWZoneData.MapID);
                foreach (WorldModelObject wmo in zone.WOWZoneData.WorldObjects)
                {
                    wmoAreaTableDBC.AddRow(Convert.ToInt32(zone.WOWZoneData.WMOID), Convert.ToInt32(wmo.WMOGroupID),
                        Convert.ToInt32(zone.WOWZoneData.AreaID), zone.DescriptiveName);
                }
                foreach (ZonePropertiesLineBox zoneLine in ZoneProperties.GetZonePropertiesForZone(zone.ShortName).ZoneLineBoxes)
                {
                    areaTriggerDBC.AddRow(zoneLine.AreaTriggerID, zone.WOWZoneData.MapID, zoneLine.BoxPosition.X, zoneLine.BoxPosition.Y,
                        zoneLine.BoxPosition.Z, zoneLine.BoxLength, zoneLine.BoxWidth, zoneLine.BoxHeight, zoneLine.BoxOrientation);
                }
            }

            // Output them
            areaTableDBC.WriteToDisk(dbcUpdateScriptFolder);
            mapDBC.WriteToDisk(dbcUpdateScriptFolder);
            difficultyDBC.WriteToDisk(dbcUpdateScriptFolder);
            wmoAreaTableDBC.WriteToDisk(dbcUpdateScriptFolder);
            loadingScreensDBC.WriteToDisk(dbcUpdateScriptFolder);
            areaTriggerDBC.WriteToDisk(dbcUpdateScriptFolder);

            Logger.WriteLine("DBC Update Scripts created successfully");
        }

        public static void CreateAzerothCoreScripts(List<Zone> zones, string wowExportPath)
        {
            Logger.WriteLine("Creating AzerothCore SQL Scripts...");

            string sqlScriptFolder = Path.Combine(wowExportPath, "AzerothCoreSQLScripts");

            // Create a list of zone IDs by short names, used for lookups
            Dictionary<string, int> zoneMapIDsByShortName = new Dictionary<string, int>();
            foreach (Zone zone in zones)
            {
                if (zoneMapIDsByShortName.ContainsKey(zone.ShortName) == true)
                {
                    Logger.WriteLine("Error!  More than one map had the same short name of '" + zone.ShortName + "'");
                    continue;
                }
                zoneMapIDsByShortName[zone.ShortName] = zone.WOWZoneData.MapID;
            }

            // Create the SQL Scripts
            GameTeleSQL gameTeleSQL = new GameTeleSQL();
            InstanceTemplateSQL instanceTemplateSQL = new InstanceTemplateSQL();
            AreaTriggerSQL areaTriggerSQL = new AreaTriggerSQL();
            AreaTriggerTeleportSQL areaTriggerTeleportSQL = new AreaTriggerTeleportSQL();

            foreach (Zone zone in zones)
            {
                // Teleport scripts to safe positions (add a record for both descriptive and short name if they are different)
                gameTeleSQL.AddRow(Convert.ToInt32(zone.WOWZoneData.MapID), zone.DescriptiveNameOnlyLetters, zone.WOWZoneData.SafePosition.Y, zone.WOWZoneData.SafePosition.Y, zone.WOWZoneData.SafePosition.Z);
                if (zone.DescriptiveNameOnlyLetters.ToLower() != zone.ShortName.ToLower())
                    gameTeleSQL.AddRow(Convert.ToInt32(zone.WOWZoneData.MapID), zone.ShortName, zone.WOWZoneData.SafePosition.Y, zone.WOWZoneData.SafePosition.Y, zone.WOWZoneData.SafePosition.Z);

                // Instance list
                instanceTemplateSQL.AddRow(Convert.ToInt32(zone.WOWZoneData.MapID));

                // Zone lines
                foreach(ZonePropertiesLineBox zoneLine in ZoneProperties.GetZonePropertiesForZone(zone.ShortName).ZoneLineBoxes)
                {
                    if (zoneMapIDsByShortName.ContainsKey(zoneLine.TargetZoneShortName) == false)
                    {
                        Logger.WriteLine("Error!  When attempting to map a zone line, there was no zone with short name '" + zoneLine.TargetZoneShortName + "'");
                        continue;
                    }

                    // Area Trigger Teleport
                    int areaTriggerID = zoneLine.AreaTriggerID;
                    string descriptiveName = "EQ " + zone.ShortName + " - " + zoneLine.TargetZoneShortName + " zone line";
                    int targetMapId = zoneMapIDsByShortName[zoneLine.TargetZoneShortName];
                    float targetPositionX = zoneLine.TargetZonePosition.X;
                    float targetPositionY = zoneLine.TargetZonePosition.Y;
                    float targetPositionZ = zoneLine.TargetZonePosition.Z;
                    float targetOrientation = zoneLine.TargetZoneOrientation;
                    areaTriggerTeleportSQL.AddRow(areaTriggerID, descriptiveName, targetMapId, targetPositionX, targetPositionY, targetPositionZ, targetOrientation);

                    // Area Trigger
                    areaTriggerSQL.AddRow(zoneLine.AreaTriggerID, zone.WOWZoneData.MapID, zoneLine.BoxPosition.X, zoneLine.BoxPosition.Y,
                        zoneLine.BoxPosition.Z, zoneLine.BoxLength, zoneLine.BoxWidth, zoneLine.BoxHeight, zoneLine.BoxOrientation);
                }
            }

            // Output them
            gameTeleSQL.WriteToDisk(sqlScriptFolder);
            instanceTemplateSQL.WriteToDisk(sqlScriptFolder);
            areaTriggerSQL.WriteToDisk(sqlScriptFolder);
            areaTriggerTeleportSQL.WriteToDisk(sqlScriptFolder);
            
            Logger.WriteLine("AzerothCore SQL Scripts created successfully");
        }

        public static void ExportTexturesForZone(Zone zone, string zoneInputFolder, string wowExportPath)
        {
            Logger.WriteLine("- [" + zone.ShortName + "]: Exporting textures for zone '" + zone.ShortName + "'...");

            // Create the folder to output
            string zoneOutputTextureFolder = Path.Combine(wowExportPath, "World", "Everquest", "ZoneTextures", zone.ShortName);
            if (Directory.Exists(zoneOutputTextureFolder) == false)
                FileTool.CreateBlankDirectory(zoneOutputTextureFolder, true);

            // Go through every texture to move and put it there
            foreach (Material material in zone.WOWZoneData.Materials)
            {
                if (material.AnimationTextures.Count == 0)
                    continue;
                string textureName = material.AnimationTextures[0];
                string sourceTextureFullPath = Path.Combine(zoneInputFolder, "Textures", textureName + ".blp");
                string outputTextureFullPath = Path.Combine(zoneOutputTextureFolder, textureName + ".blp");
                File.Copy(sourceTextureFullPath, outputTextureFullPath, true);
                Logger.WriteLine("- [" + zone.ShortName + "]: Texture named '" + textureName + "' copied");
            }

            Logger.WriteLine("- [" + zone.ShortName + "]: Texture output for zone '" + zone.ShortName + "' complete");
        }

        public static void ExportTexturesForObject(ModelObject modelObject, string objectTextureInputFolder, string objectExportPath)
        {
            Logger.WriteLine("- [" + modelObject.Name + "]: Exporting textures for object '" + modelObject.Name + "'...");

            // Go through every texture and copy it
            foreach(ModelTexture texture in modelObject.WOWModelObjectData.ModelTextures)
            {
                string inputTextureName = Path.Combine(objectTextureInputFolder, texture.TextureName + ".blp");
                string outputTextureName = Path.Combine(objectExportPath, texture.TextureName + ".blp");
                if (Path.Exists(inputTextureName) == false)
                {
                    Logger.WriteLine("- [" + modelObject.Name + "]: ERROR Texture named '" + texture.TextureName + ".blp' not found.  Did you run blpconverter?");
                    return;
                }
                File.Copy(inputTextureName, outputTextureName, true);
                Logger.WriteLine("- [" + modelObject.Name + "]: Texture named '" + texture.TextureName + ".blp' copied");
            }

            Logger.WriteLine("- [" + modelObject.Name + "]: Texture output for object '" + modelObject.Name + "' complete");
        }
    }
}
