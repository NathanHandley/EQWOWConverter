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
using Vector3 = EQWOWConverter.Common.Vector3;
using EQWOWConverter.WOWFiles.DBC;

namespace EQWOWConverter
{
    internal class AssetConverter
    {
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

            // Clean out the mpq folder
            string exportMPQRootFolder = Path.Combine(wowExportPath, "MPQReady");
            if (Directory.Exists(exportMPQRootFolder))
                Directory.Delete(exportMPQRootFolder, true);

            // Go through the subfolders for each zone and convert to wow zone
            DirectoryInfo zoneRootDirectoryInfo = new DirectoryInfo(zoneFolderRoot);
            DirectoryInfo[] zoneDirectoryInfos = zoneRootDirectoryInfo.GetDirectories();
            List<Zone> zones = new List<Zone>();
            foreach (DirectoryInfo zoneDirectory in zoneDirectoryInfos)
            {
                //if (zoneDirectory.Name != "unrest")
                //    continue;

                // Load the EQ zone
                Zone curZone = new Zone(zoneDirectory.Name);
                Logger.WriteLine("- [" + zoneDirectory.Name + "]: Importing EQ zone '" + zoneDirectory.Name);
                string curZoneDirectory = Path.Combine(zoneFolderRoot, zoneDirectory.Name);
                curZone.LoadEQZoneData(zoneDirectory.Name, curZoneDirectory);
                Logger.WriteLine("- [" + zoneDirectory.Name + "]: Importing of EQ zone '" + zoneDirectory.Name + "' complete");

                // Convert to WOW zone
                CreateWoWZoneFromEQZone(curZone, exportMPQRootFolder);

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

        public static void CreateWoWZoneFromEQZone(Zone zone, string exportMPQRootFolder)
        {
            Logger.WriteLine("- [" + zone.ShortName + "]: Converting zone '" + zone.ShortName + "' into a wow zone...");

            // Capture any zone properties
            ZoneProperties zoneProperties = GetZonePropertiesForZone(zone.ShortName);

            // Generate the WOW zone data first
            zone.PopulateWOWZoneDataFromEQZoneData(zoneProperties);

            // Create the zone WMO objects
            WMO zoneWMO = new WMO(zone, exportMPQRootFolder);
            zoneWMO.WriteToDisk(exportMPQRootFolder);

            // Create the WDT
            WDT zoneWDT = new WDT(zone, zoneWMO.RootFileRelativePathWithFileName);
            zoneWDT.WriteToDisk(exportMPQRootFolder);

            // Create the WDL
            WDL zoneWDL = new WDL(zone);
            zoneWDL.WriteToDisk(exportMPQRootFolder);

            Logger.WriteLine("- [" + zone.ShortName + "]: Converting of zone '" + zone.ShortName + "' complete");
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
            foreach (Zone zone in zones)
            {
                areaTableDBC.AddRow(Convert.ToInt32(zone.WOWZoneData.AreaID), zone.DescriptiveName);
                mapDBC.AddRow(zone.WOWZoneData.MapID, "EQ_" + zone.ShortName, zone.DescriptiveName, Convert.ToInt32(zone.WOWZoneData.AreaID), zone.WOWZoneData.LoadingScreenID);
                difficultyDBC.AddRow(zone.WOWZoneData.MapID);
                foreach(WorldModelObject wmo in zone.WOWZoneData.WorldObjects)
                {
                    wmoAreaTableDBC.AddRow(Convert.ToInt32(zone.WOWZoneData.WMOID), Convert.ToInt32(wmo.WMOGroupID),
                        Convert.ToInt32(zone.WOWZoneData.AreaID), zone.DescriptiveName);
                }
            }

            // Output them
            areaTableDBC.WriteToDisk(dbcUpdateScriptFolder);
            mapDBC.WriteToDisk(dbcUpdateScriptFolder);
            difficultyDBC.WriteToDisk(dbcUpdateScriptFolder);
            wmoAreaTableDBC.WriteToDisk(dbcUpdateScriptFolder);
            loadingScreensDBC.WriteToDisk(dbcUpdateScriptFolder);

            Logger.WriteLine("DBC Update Scripts created successfully");
        }

        public static void CreateAzerothCoreScripts(List<Zone> zones, string wowExportPath)
        {
            Logger.WriteLine("Creating AzerothCore SQL Scripts...");

            string sqlScriptFolder = Path.Combine(wowExportPath, "AzerothCoreSQLScripts");

            // Create the SQL Scripts
            GameTeleSQL gameTeleSQL = new GameTeleSQL();
            InstanceTemplateSQL instanceTemplateSQL = new InstanceTemplateSQL();

            foreach (Zone zone in zones)
            {
                gameTeleSQL.AddRow(Convert.ToInt32(zone.WOWZoneData.MapID), zone.DescriptiveNameOnlyLetters,
                    zone.WOWZoneData.SafePosition.Y, zone.WOWZoneData.SafePosition.Y, zone.WOWZoneData.SafePosition.Z);
                instanceTemplateSQL.AddRow(Convert.ToInt32(zone.WOWZoneData.MapID));
            }

            // Output them
            gameTeleSQL.WriteToDisk(sqlScriptFolder);
            instanceTemplateSQL.WriteToDisk(sqlScriptFolder);
            
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

        private static ZoneProperties GetZonePropertiesForZone(string zoneShortName)
        {
            ZoneProperties zoneProperties = new ZoneProperties();

            switch (zoneShortName)
            {
                case "airplane":
                    {
                        zoneProperties.SetBaseZoneProperties("Plane of Sky", 542.45f, 1384.6f, -650f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(0, 0, 0, 500, 2000);                        
                    }
                    break;
                case "akanon":
                    {
                        zoneProperties.SetBaseZoneProperties("Ak'Anon", -35f, 47f, 4f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(30, 60, 30, 10, 600);                        
                    }
                    break;
                case "arena":
                    {
                        zoneProperties.SetBaseZoneProperties("The Arena", 460.9f, -41.4f, -7.38f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(100, 100, 100, 10, 1500);                        
                    }
                    break;
                case "befallen":
                    {
                        zoneProperties.SetBaseZoneProperties("Befallen", 35.22f, -75.27f, 2.19f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(30, 30, 90, 10, 175);
                    }
                    break;
                case "beholder":
                    {
                        zoneProperties.SetBaseZoneProperties("Gorge of King Xorbb", -21.44f, -512.23f, 45.13f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(240, 180, 150, 10, 600);
                    }
                    break;
                case "blackburrow":
                    {
                        zoneProperties.SetBaseZoneProperties("Blackburrow", 38.92f, -158.97f, 3.75f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(50, 100, 90, 10, 700);
                    }
                    break;
                case "burningwood":
                    {
                        zoneProperties.SetBaseZoneProperties("Burning Wood", -820f, -4942f, 200.31f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(235, 235, 235, 60, 400);
                    }
                    break;
                case "butcher":
                    {
                        zoneProperties.SetBaseZoneProperties("Butcherblock Mountains", -700f, 2550f, 2.9f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(150, 170, 140, 10, 1000);
                    }
                    break;
                case "cabeast":
                    {
                        zoneProperties.SetBaseZoneProperties("East Cabilis", -416f, 1343f, 4f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(150, 120, 80, 40, 300);
                    }
                    break;
                case "cabwest":
                    {
                        zoneProperties.SetBaseZoneProperties("West Cabilis", 790f, 165f, 3.75f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(150, 120, 80, 40, 300);
                    }
                    break;
                case "cauldron":
                    {
                        zoneProperties.SetBaseZoneProperties("Dagnor's Cauldron", 320f, 2815f, 473f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(100, 100, 140, 10, 1000);
                    }
                    break;
                case "cazicthule":
                    {
                        zoneProperties.SetBaseZoneProperties("Cazic Thule", -80f, 80f, 5.5f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(50, 80, 20, 10, 450);
                    }
                    break;
                case "charasis":
                    {
                        zoneProperties.SetBaseZoneProperties("Howling Stones", 0f, 0f, -4.25f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(160, 180, 200, 50, 400);
                    }
                    break;
                case "chardok":
                    {
                        zoneProperties.SetBaseZoneProperties("Chardok", 859f, 119f, 106f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(90, 53, 6, 30, 300);
                    }
                    break;
                case "citymist":
                    {
                        zoneProperties.SetBaseZoneProperties("City of Mist", -734f, 28f, 3.75f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(90, 110, 60, 50, 275);
                    }
                    break;
                case "cobaltscar":
                    {
                        zoneProperties.SetBaseZoneProperties("Cobalt Scar", 895f, -939f, 318f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(180, 180, 180, 200, 1800);
                    }
                    break;
                case "commons":
                    {
                        zoneProperties.SetBaseZoneProperties("West Commonlands", -1334.24f, 209.57f, -51.47f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);                        
                    }
                    break;
                case "crushbone":
                    {
                        zoneProperties.SetBaseZoneProperties("Crushbone", 158f, -644f, 4f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(90, 90, 190, 10, 400);
                    }
                    break;
                case "crystal":
                    {
                        zoneProperties.SetBaseZoneProperties("Crystal Caverns", 303f, 487f, -74f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(0, 0, 0, 0, 0);
                    }
                    break;
                case "dalnir":
                    {
                        zoneProperties.SetBaseZoneProperties("Dalnir", 90f, 8f, 3.75f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(20, 10, 25, 30, 210);
                    }
                    break;
                case "dreadlands":
                    {
                        zoneProperties.SetBaseZoneProperties("Dreadlands", 9565.05f, 2806.04f, 1045.2f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(235, 235, 235, 200, 600);
                    }
                    break;
                case "droga":
                    {
                        zoneProperties.SetBaseZoneProperties("Temple of Droga", 294.11f, 1371.43f, 3.75f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(0, 15, 0, 100, 300);
                    }
                    break;
                case "eastkarana":
                    {
                        zoneProperties.SetBaseZoneProperties("Eastern Karana", 0f, 0f, 3.5f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                    }
                    break;
                case "eastwastes":
                    {
                        zoneProperties.SetBaseZoneProperties("Eastern Wastes", -4296f, -5049f, 147f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(200, 200, 200, 200, 1800);
                    }
                    break;
                case "ecommons":
                    {
                        zoneProperties.SetBaseZoneProperties("East Commonlands", -1485f, 9.2f, -51f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                    }
                    break;
                case "emeraldjungle":
                    {
                        zoneProperties.SetBaseZoneProperties("Emerald Jungle", 4648.06f, -1222.97f, 0f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(200, 235, 210, 60, 200);
                    }
                    break;
                case "erudnext":
                    {
                        zoneProperties.SetBaseZoneProperties("Erudin Docks", -309.75f, 109.64f, 23.75f, 0, ZoneContinent.Odus);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 550);
                    }
                    break;
                case "erudnint":
                    {
                        zoneProperties.SetBaseZoneProperties("Erudin Palace", 807f, 712f, 22f, 0, ZoneContinent.Odus);
                        zoneProperties.SetFogProperties(0, 0, 0, 0, 0);
                    }
                    break;
                case "erudsxing":
                    {
                        zoneProperties.SetBaseZoneProperties("Erud's Crossing", 795f, -1766.9f, 12.36f, 0, ZoneContinent.Odus);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                    }
                    break;
                case "everfrost":
                    {
                        zoneProperties.SetBaseZoneProperties("Everfrost Peaks", 682.74f, 3139.01f, -60.16f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 230, 255, 10, 500);
                    }
                    break;
                case "fearplane":
                    {
                        zoneProperties.SetBaseZoneProperties("Plane of Fear", 1282.09f, -1139.03f, 1.67f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(255, 50, 10, 10, 1000);
                    }
                    break;
                case "feerrott":
                    {
                        zoneProperties.SetBaseZoneProperties("The Feerrott", 902.6f, 1091.7f, 28f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(60, 90, 30, 10, 175);
                    }
                    break;
                case "felwithea":
                    {
                        zoneProperties.SetBaseZoneProperties("North Felwithe", 94f, -25f, 3.75f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(100, 130, 100, 10, 300);
                    }
                    break;
                case "felwitheb":
                    {
                        zoneProperties.SetBaseZoneProperties("South Felwithe", -790f, 320f, -10.25f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(100, 130, 100, 10, 300);
                    }
                    break;
                case "fieldofbone":
                    {
                        zoneProperties.SetBaseZoneProperties("Field of Bone", 1617f, -1684f, -54.78f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(235, 235, 235, 200, 800);
                    }
                    break;
                case "firiona":
                    {
                        zoneProperties.SetBaseZoneProperties("Firiona Vie", 1439.96f, -2392.06f, -2.65f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(235, 235, 235, 200, 800);
                    }
                    break;
                case "freporte":
                    {
                        zoneProperties.SetBaseZoneProperties("East Freeport", -648f, -1097f, -52.2f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(230, 200, 200, 10, 450);
                    }
                    break;
                case "freportn":
                    {
                        zoneProperties.SetBaseZoneProperties("North Freeport", 211f, -296f, 4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(230, 200, 200, 10, 450);
                    }
                    break;
                case "freportw":
                    {
                        zoneProperties.SetBaseZoneProperties("West Freeport", 181f, 335f, -24f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(230, 200, 200, 10, 450);
                    }
                    break;
                case "frontiermtns":
                    {
                        zoneProperties.SetBaseZoneProperties("Frontier Mountains", -4262f, -633f, 113.24f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(235, 235, 235, 200, 800);
                    }
                    break;
                case "frozenshadow":
                    {
                        zoneProperties.SetBaseZoneProperties("Tower of Frozen Shadow", 200f, 120f, 0f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(25, 25, 25, 10, 350);
                    }
                    break;
                case "gfaydark":
                    {
                        zoneProperties.SetBaseZoneProperties("Greater Faydark", 10f, -20f, 0f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(0, 128, 64, 10, 300);
                    }
                    break;
                case "greatdivide":
                    {
                        zoneProperties.SetBaseZoneProperties("The Great Divide", -965f, -7720f, -557f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(160, 160, 172, 200, 1800);
                    }
                    break;
                case "grobb":
                    {
                        zoneProperties.SetBaseZoneProperties("Grobb", 0f, -100f, 4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(0, 0, 0, 500, 2000);
                    }
                    break;
                case "growthplane":
                    {
                        zoneProperties.SetBaseZoneProperties("Plane of Growth", 3016f, -2522f, -19f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(0, 50, 100, 60, 1200);
                    }
                    break;
                case "gukbottom":
                    {
                        zoneProperties.SetBaseZoneProperties("Lower Guk", -217f, 1197f, -81.78f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(50, 45, 20, 10, 140);
                    }
                    break;
                case "guktop":
                    {
                        zoneProperties.SetBaseZoneProperties("Upper Guk", 7f, -36f, 4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(40, 45, 20, 10, 140);                       
                    }
                    break;
                case "halas":
                    {
                        zoneProperties.SetBaseZoneProperties("Halas", 0f, 0f, 3.75f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 230, 255, 10, 300);
                    }
                    break;
                case "hateplane":
                    {
                        zoneProperties.SetBaseZoneProperties("Plane of Hate", -353.08f, -374.8f, 3.75f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(128, 128, 128, 30, 200);
                    }
                    break;
                case "highkeep":
                    {
                        zoneProperties.SetBaseZoneProperties("High Keep", 88f, -16f, 4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(0, 0, 0, 0, 0);
                    }
                    break;
                case "highpass":
                    {
                        zoneProperties.SetBaseZoneProperties("Highpass Hold", -104f, -14f, 4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 200, 10, 400);
                    }
                    break;
                case "hole":
                    {
                        zoneProperties.SetBaseZoneProperties("The Hole", -1049.98f, 640.04f, -77.22f, 0, ZoneContinent.Odus);
                        zoneProperties.SetFogProperties(10, 10, 10, 200, 500);
                    }
                    break;
                case "iceclad":
                    {
                        zoneProperties.SetBaseZoneProperties("Iceclad Ocean", 340f, 5330f, -17f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(200, 200, 200, 200, 1800);
                    }
                    break;
                case "innothule":
                    {
                        zoneProperties.SetBaseZoneProperties("Innothule Swamp", -588f, -2192f, -25f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(170, 160, 90, 10, 500);
                    }
                    break;
                case "kael":
                    {
                        zoneProperties.SetBaseZoneProperties("Kael Drakkal", -633f, -47f, 128f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(10, 10, 50, 20, 500);
                    }
                    break;
                case "kaesora":
                    {
                        zoneProperties.SetBaseZoneProperties("Kaesora", 40f, 370f, 99.72f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(0, 10, 0, 20, 200);
                    }
                    break;
                case "kaladima":
                    {
                        zoneProperties.SetBaseZoneProperties("North Kaladim", -2f, -18f, 3.75f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(70, 50, 20, 10, 175);
                    }
                    break;
                case "kaladimb":
                    {
                        zoneProperties.SetBaseZoneProperties("South Kaladim", -267f, 414f, 3.75f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(70, 50, 20, 10, 175);
                    }
                    break;
                case "karnor":
                    {
                        zoneProperties.SetBaseZoneProperties("Karnor's Castle", 0f, 0f, 4f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(50, 20, 20, 10, 350);
                    }
                    break;
                case "kedge":
                    {
                        zoneProperties.SetBaseZoneProperties("Kedge Keep", 99.96f, 14.02f, 31.75f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(10, 10, 10, 25, 25);
                    }
                    break;
                case "kerraridge":
                    {
                        zoneProperties.SetBaseZoneProperties("Kerra Island", -859.97f, 474.96f, 23.75f, 0, ZoneContinent.Odus);
                        zoneProperties.SetFogProperties(220, 220, 200, 10, 600);
                    }
                    break;
                case "kithicor":
                    {
                        zoneProperties.SetBaseZoneProperties("Kithicor Forest", 3828f, 1889f, 459f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(120, 140, 100, 10, 200);
                    }
                    break;
                case "kurn":
                    {
                        zoneProperties.SetBaseZoneProperties("Kurn's Tower", 77.72f, -277.64f, 3.75f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(50, 50, 20, 10, 200);
                    }
                    break;
                case "lakeofillomen":
                    {
                        zoneProperties.SetBaseZoneProperties("Lake of Ill Omen", -5383.07f, 5747.14f, 68.27f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(235, 235, 235, 200, 800);
                    }
                    break;
                case "lakerathe":
                    {
                        zoneProperties.SetBaseZoneProperties("Lake Rathetear", 1213f, 4183f, 4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                    }
                    break;
                case "lavastorm":
                    {
                        zoneProperties.SetBaseZoneProperties("Lavastorm Mountains", 153.45f, -1842.79f, -16.37f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(255, 50, 10, 10, 800);
                    }
                    break;
                case "lfaydark":
                    {
                        zoneProperties.SetBaseZoneProperties("Lesser Faydark", -1769.93f, -108.08f, -1.11f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(230, 255, 200, 10, 300);
                    }
                    break;
                case "load":
                    {
                        zoneProperties.SetBaseZoneProperties("Loading Area", -316f, 5f, 8.2f, 0, ZoneContinent.Development);
                        zoneProperties.SetFogProperties(0, 0, 0, 500, 2000);
                    }
                    break;
                case "mischiefplane":
                    {
                        zoneProperties.SetBaseZoneProperties("Plane of Mischief", -395f, -1410f, 115f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(210, 235, 210, 60, 600);
                    }
                    break;
                case "mistmoore":
                    {
                        zoneProperties.SetBaseZoneProperties("Mistmoore Castle", 123f, -295f, -177f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(60, 30, 90, 10, 250);
                    }
                    break;
                case "misty":
                    {
                        zoneProperties.SetBaseZoneProperties("Misty Thicket", 0f, 0f, 2.43f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(100, 120, 50, 10, 500);
                    }
                    break;
                case "najena":
                    {
                        zoneProperties.SetBaseZoneProperties("Najena", -22.6f, 229.1f, -41.8f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(30, 0, 40, 10, 110);
                    }
                    break;
                case "necropolis":
                    {
                        zoneProperties.SetBaseZoneProperties("Dragon Necropolis", 2000f, -100f, 5f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(35, 50, 35, 10, 2000);
                    }
                    break;
                case "nektulos":
                    {
                        zoneProperties.SetBaseZoneProperties("Nektulos Forest", -259f, -1201f, -5f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(80, 90, 70, 10, 400);
                    }
                    break;
                case "neriaka":
                    {
                        zoneProperties.SetBaseZoneProperties("Neriak Foreign Quarter", 156.92f, -2.94f, 31.75f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(10, 0, 60, 10, 250);
                    }
                    break;
                case "neriakb":
                    {
                        zoneProperties.SetBaseZoneProperties("Neriak Commons", -499.91f, 2.97f, -10.25f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(10, 0, 60, 10, 250);
                    }
                    break;
                case "neriakc":
                    {
                        zoneProperties.SetBaseZoneProperties("Neriak Third Gate", -968.96f, 891.92f, -52.22f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(10, 0, 60, 10, 250);
                    }
                    break;
                case "northkarana":
                    {
                        zoneProperties.SetBaseZoneProperties("Northern Karana", -382f, -284f, -7f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                    }
                    break;
                case "nro":
                    {
                        zoneProperties.SetBaseZoneProperties("Northern Desert of Ro", 299.12f, 3537.9f, -24.5f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(250, 250, 180, 10, 800);
                    }
                    break;
                case "nurga":
                    {
                        zoneProperties.SetBaseZoneProperties("Mines of Nurga", 150f, -1062f, -107f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(0, 15, 0, 100, 300);
                    }
                    break;
                case "oasis":
                    {
                        zoneProperties.SetBaseZoneProperties("Oasis of Marr", 903.98f, 490.03f, 6.4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(250, 250, 180, 10, 800);
                    }
                    break;
                case "oggok":
                    {
                        zoneProperties.SetBaseZoneProperties("Oggok", -99f, -345f, 4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(130, 140, 80, 10, 300);
                    }
                    break;
                case "oot":
                    {
                        zoneProperties.SetBaseZoneProperties("Ocean of Tears", -9200f, 390f, 6f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                    }
                    break;
                case "overthere":
                    {
                        zoneProperties.SetBaseZoneProperties("The Overthere", -4263f, -241f, 235f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(235, 235, 235, 200, 800);
                    }
                    break;
                case "paineel":
                    {
                        zoneProperties.SetBaseZoneProperties("Paineel", 200f, 800f, 3.39f, 0, ZoneContinent.Odus);
                        zoneProperties.SetFogProperties(150, 150, 150, 200, 850);
                    }
                    break;
                case "paw":
                    {
                        zoneProperties.SetBaseZoneProperties("Splitpaw Lair", -7.9f, -79.3f, 4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(30, 25, 10, 10, 180);
                    }
                    break;
                case "permafrost":
                    {
                        zoneProperties.SetBaseZoneProperties("Permafrost", 0f, 0f, 3.75f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(25, 35, 45, 10, 180);
                    }
                    break;
                case "qcat":
                    {
                        zoneProperties.SetBaseZoneProperties("Qeynos Catacombs", -315f, 214f, -38f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(0, 0, 0, 500, 2000);
                    }
                    break;
                case "qey2hh1":
                    {
                        zoneProperties.SetBaseZoneProperties("Western Karana", -638f, 12f, -4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                    }
                    break;
                case "qeynos":
                    {
                        zoneProperties.SetBaseZoneProperties("South Qeynos", 186.46f, 14.29f, 3.75f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 210, 10, 450);
                    }
                    break;
                case "qeynos2":
                    {
                        zoneProperties.SetBaseZoneProperties("North Qeynos", 114f, 678f, 4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 450);
                    }
                    break;
                case "qeytoqrg":
                    {
                        zoneProperties.SetBaseZoneProperties("Qeynos Hills", 196.7f, 5100.9f, -1f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(0, 0, 0, 500, 2000);
                    }
                    break;
                case "qrg":
                    {
                        zoneProperties.SetBaseZoneProperties("Surefall Glade", 136.9f, -65.9f, 4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(180, 175, 183, 10, 450);
                    }
                    break;
                case "rathemtn":
                    {
                        zoneProperties.SetBaseZoneProperties("Rathe Mountains", 1831f, 3825f, 29.03f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                    }
                    break;
                case "rivervale":
                    {
                        zoneProperties.SetBaseZoneProperties("Rivervale", 45.3f, 1.6f, 3.8f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 210, 200, 10, 400);
                    }
                    break;
                case "runnyeye":
                    {
                        zoneProperties.SetBaseZoneProperties("Runnyeye Citadel", -21.85f, -108.88f, 3.75f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(75, 150, 25, 10, 600);
                    }
                    break;
                case "sebilis":
                    {
                        zoneProperties.SetBaseZoneProperties("Old Sebilis", 0f, 235f, 40f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(20, 10, 60, 50, 400);
                    }
                    break;
                case "sirens":
                    {
                        zoneProperties.SetBaseZoneProperties("Siren's Grotto", -33f, 196f, 4f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(30, 100, 130, 10, 500);
                    }
                    break;
                case "skyfire":
                    {
                        zoneProperties.SetBaseZoneProperties("Skyfire Mountains", -3931.32f, -1139.25f, 39.76f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(235, 200, 200, 200, 600);
                    }
                    break;
                case "skyshrine":
                    {
                        zoneProperties.SetBaseZoneProperties("Skyshrine", -730f, -210f, 0f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(50, 0, 200, 100, 600);
                    }
                    break;
                case "sleeper":
                    {
                        zoneProperties.SetBaseZoneProperties("Sleeper's Tomb", 0f, 0f, 5f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(80, 80, 220, 200, 800);
                    }
                    break;
                case "soldunga":
                    {
                        zoneProperties.SetBaseZoneProperties("Solusek's Eye", -485.77f, -476.04f, 73.72f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(180, 30, 30, 10, 100);
                    }
                    break;
                case "soldungb":
                    {
                        zoneProperties.SetBaseZoneProperties("Nagafen's Lair", -262.7f, -423.99f, -108.22f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(180, 30, 30, 10, 350);
                    }
                    break;
                case "soltemple":
                    {
                        zoneProperties.SetBaseZoneProperties("The Temple of Solusek Ro", 7.5f, 268.8f, 3f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(180, 5, 5, 50, 500);
                    }
                    break;
                case "southkarana":
                    {
                        zoneProperties.SetBaseZoneProperties("Southern Karana", 1293.66f, 2346.69f, -5.77f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                    }
                    break;
                case "sro":
                    {
                        zoneProperties.SetBaseZoneProperties("Southern Desert of Ro", 286f, 1265f, 79f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(250, 250, 180, 10, 800);
                    }
                    break;
                case "steamfont":
                    {
                        zoneProperties.SetBaseZoneProperties("Steamfont Mountains", -272.86f, 159.86f, -21.4f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                    }
                    break;
                case "stonebrunt":
                    {
                        zoneProperties.SetBaseZoneProperties("Stonebrunt Mountains", -1643.01f, -3427.84f, -6.57f, 0, ZoneContinent.Odus);
                        zoneProperties.SetFogProperties(235, 235, 235, 10, 800);
                    }
                    break;
                case "swampofnohope":
                    {
                        zoneProperties.SetBaseZoneProperties("Swamp of No Hope", -1830f, -1259.9f, 27.1f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(210, 200, 210, 60, 400);
                    }
                    break;
                case "templeveeshan":
                    {
                        zoneProperties.SetBaseZoneProperties("Temple of Veeshan", -499f, -2086f, -36f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(60, 10, 10, 30, 300);
                    }
                    break;
                case "thurgadina":
                    {
                        zoneProperties.SetBaseZoneProperties("Thurgadin", 0f, -1222f, 0f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(25, 25, 25, 100, 300);
                    }
                    break;
                case "thurgadinb":
                    {
                        zoneProperties.SetBaseZoneProperties("Thurgadin Mines", 0f, 250f, 0f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(25, 25, 25, 100, 300);
                    }
                    break;
                case "timorous":
                    {
                        zoneProperties.SetBaseZoneProperties("Timorous Deep", 2194f, -5392f, 4f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(225, 225, 230, 100, 700);
                    }
                    break;
                case "tox":
                    {
                        zoneProperties.SetBaseZoneProperties("Toxxulia Forest", 203f, 2295f, -45f, 0, ZoneContinent.Odus);
                        zoneProperties.SetFogProperties(220, 200, 30, 50, 250);
                    }
                    break;
                case "trakanon":
                    {
                        zoneProperties.SetBaseZoneProperties("Trakanon's Teeth", 1485.86f, 3868.29f, -340.59f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(210, 235, 213, 60, 250);
                    }
                    break;
                case "tutorial":
                    {
                        zoneProperties.SetBaseZoneProperties("Tutorial", 0f, 0f, 0f, 0, ZoneContinent.Development);
                        zoneProperties.SetFogProperties(0, 0, 0, 500, 2000);
                    }
                    break;
                case "unrest":
                    {
                        zoneProperties.SetBaseZoneProperties("The Estate of Unrest", 52f, -38f, 3.75f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(40, 10, 60, 10, 300);
                    }
                    break;
                case "veeshan":
                    {
                        zoneProperties.SetBaseZoneProperties("Veeshan's Peak", 1682f, 41f, 28f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(20, 0, 0, 100, 1200);
                    }
                    break;
                 case "velketor":
                    {
                        zoneProperties.SetBaseZoneProperties("Velketor's Labyrinth", -65f, 581f, -152f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(10, 130, 130, 10, 500);
                    }
                    break;
                case "wakening":
                    {
                        zoneProperties.SetBaseZoneProperties("Wakening Land", -5000f, -673f, -195f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(254, 254, 254, 60, 600);
                    }
                    break;
                case "warrens":
                    {
                        zoneProperties.SetBaseZoneProperties("The Warrens", -930f, 748f, -37.22f, 0, ZoneContinent.Odus);
                        zoneProperties.SetFogProperties(0, 15, 0, 100, 300);
                    }
                    break;
                case "warslikswood":
                    {
                        zoneProperties.SetBaseZoneProperties("Warsliks Woods", -467.95f, -1428.95f, 197.31f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(210, 235, 210, 60, 600);
                    }
                    break;
                case "westwastes":
                    {
                        zoneProperties.SetFogProperties(128, 128, 160, 200, 1800);
                        zoneProperties.SetBaseZoneProperties("Western Wastes", - 3499f, -4099f, -16.66f, 0, ZoneContinent.Velious);
                    }
                    break;
                default:
                    {
                        Logger.WriteLine("GetZonePropertiesForZone error!  No known short name of '" + zoneShortName + "'");
                    } break;
            }

            return zoneProperties;
        }
    }
}
