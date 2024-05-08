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

namespace EQWOWConverter
{
    internal class AssetConverter
    {
        public static bool ConvertEQZonesToWOW(string eqExportsCondensedPath, string wowExportPath)
        {
            Logger.WriteLine("Converting EQ zones to WOW zones...");

            // Make sure the root path exists
            if (Directory.Exists(eqExportsCondensedPath) == false)
            {
                Logger.WriteLine("ERROR - Condensed path of '" + eqExportsCondensedPath + "' does not exist.");
                Logger.WriteLine("Conversion Failed!");
                return false;
            }

            // Make sure the zone folder path exists
            string zoneFolderRoot = Path.Combine(eqExportsCondensedPath, "zones");
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
                //if (zoneDirectory.Name != "gfaydark")
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

        public static void CreateDBCUpdateScripts(List<Zone> zones, string wowExportPath)
        {
            Logger.WriteLine("Creating DBC Update Scripts...");

            string dbcUpdateScriptFolder = Path.Combine(wowExportPath, "DBCUpdateScripts");

            // Create the DBC update scripts
            AreaTableDBC areaTableDBC = new AreaTableDBC();
            MapDBC mapDBC = new MapDBC();
            MapDifficultyDBC difficultyDBC = new MapDifficultyDBC();
            WMOAreaTableDBC wmoAreaTableDBC = new WMOAreaTableDBC();
            foreach (Zone zone in zones)
            {
                areaTableDBC.AddRow(Convert.ToInt32(zone.WOWZoneData.AreaID), zone.DescriptiveName);
                mapDBC.AddRow(zone.WOWZoneData.MapID, "EQ_" + zone.ShortName, zone.DescriptiveName, Convert.ToInt32(zone.WOWZoneData.AreaID));
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
            foreach (string textureName in zone.WOWZoneData.TextureNames)
            {
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
                        zoneProperties.DescriptiveName = "Plane of Sky";
                        zoneProperties.SetFogProperties(0, 0, 0, 500, 2000);
                        zoneProperties.SetSafePosition(542.45f, 1384.6f, -650f);
                    }
                    break;
                case "akanon":
                    {
                        zoneProperties.DescriptiveName = "Ak'Anon";
                        zoneProperties.SetFogProperties(30, 60, 30, 10, 600);
                        zoneProperties.SetSafePosition(-35f, 47f, 4f);
                    }
                    break;
                case "arena":
                    {
                        zoneProperties.DescriptiveName = "The Arena";
                        zoneProperties.SetFogProperties(100, 100, 100, 10, 1500);
                        zoneProperties.SetSafePosition(460.9f, -41.4f, -7.38f);
                    }
                    break;
                case "befallen":
                    {
                        zoneProperties.DescriptiveName = "Befallen";
                        zoneProperties.SetFogProperties(30, 30, 90, 10, 175);
                        zoneProperties.SetSafePosition(35.22f, -75.27f, 2.19f);
                    }
                    break;
                case "beholder":
                    {
                        zoneProperties.DescriptiveName = "Gorge of King Xorbb";
                        zoneProperties.SetFogProperties(240, 180, 150, 10, 600);
                        zoneProperties.SetSafePosition(-21.44f, -512.23f, 45.13f);
                    }
                    break;
                case "blackburrow":
                    {
                        zoneProperties.DescriptiveName = "Blackburrow";
                        zoneProperties.SetFogProperties(50, 100, 90, 10, 700);
                        zoneProperties.SetSafePosition(38.92f, -158.97f, 3.75f);
                    }
                    break;
                case "burningwood":
                    {
                        zoneProperties.DescriptiveName = "Burning Wood";
                        zoneProperties.SetFogProperties(235, 235, 235, 60, 400);
                        zoneProperties.SetSafePosition(-820f, -4942f, 200.31f);
                    }
                    break;
                case "butcher":
                    {
                        zoneProperties.DescriptiveName = "Butcherblock Mountains";
                        zoneProperties.SetFogProperties(150, 170, 140, 10, 1000);
                        zoneProperties.SetSafePosition(-700f, 2550f, 2.9f);
                    }
                    break;
                case "cabeast":
                    {
                        zoneProperties.DescriptiveName = "East Cabilis";
                        zoneProperties.SetFogProperties(150, 120, 80, 40, 300);
                        zoneProperties.SetSafePosition(-416f, 1343f, 4f);
                    }
                    break;
                case "cabwest":
                    {
                        zoneProperties.DescriptiveName = "West Cabilis";
                        zoneProperties.SetFogProperties(150, 120, 80, 40, 300);
                        zoneProperties.SetSafePosition(790f, 165f, 3.75f);
                    }
                    break;
                case "cauldron":
                    {
                        zoneProperties.DescriptiveName = "Dagnor's Cauldron";
                        zoneProperties.SetFogProperties(100, 100, 140, 10, 1000);
                        zoneProperties.SetSafePosition(320f, 2815f, 473f);
                    }
                    break;
                case "cazicthule":
                    {
                        zoneProperties.DescriptiveName = "Cazic Thule";
                        zoneProperties.SetFogProperties(50, 80, 20, 10, 450);
                        zoneProperties.SetSafePosition(-80f, 80f, 5.5f);
                    }
                    break;
                case "charasis":
                    {
                        zoneProperties.DescriptiveName = "Howling Stones";
                        zoneProperties.SetFogProperties(160, 180, 200, 50, 400);
                        zoneProperties.SetSafePosition(0f, 0f, -4.25f);
                    }
                    break;
                case "chardok":
                    {
                        zoneProperties.DescriptiveName = "Chardok";
                        zoneProperties.SetFogProperties(90, 53, 6, 30, 300);
                        zoneProperties.SetSafePosition(859f, 119f, 106f);
                    }
                    break;
                case "citymist":
                    {
                        zoneProperties.DescriptiveName = "City of Mist";
                        zoneProperties.SetFogProperties(90, 110, 60, 50, 275);
                        zoneProperties.SetSafePosition(-734f, 28f, 3.75f);
                    }
                    break;
                case "cobaltscar":
                    {
                        zoneProperties.DescriptiveName = "Cobalt Scar";
                        zoneProperties.SetFogProperties(180, 180, 180, 200, 1800);
                        zoneProperties.SetSafePosition(895f, -939f, 318f);
                    }
                    break;
                case "commons":
                    {
                        zoneProperties.DescriptiveName = "West Commonlands";
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                        zoneProperties.SetSafePosition(-1334.24f, 209.57f, -51.47f);
                    }
                    break;
                case "crushbone":
                    {
                        zoneProperties.DescriptiveName = "Crushbone";
                        zoneProperties.SetFogProperties(90, 90, 190, 10, 400);
                        zoneProperties.SetSafePosition(158f, -644f, 4f);
                    }
                    break;
                case "crystal":
                    {
                        zoneProperties.DescriptiveName = "Crystal Caverns";
                        zoneProperties.SetFogProperties(0, 0, 0, 0, 0);
                        zoneProperties.SetSafePosition(303f, 487f, -74f);
                    }
                    break;
                case "dalnir":
                    {
                        zoneProperties.DescriptiveName = "Dalnir";
                        zoneProperties.SetFogProperties(20, 10, 25, 30, 210);
                        zoneProperties.SetSafePosition(90f, 8f, 3.75f);
                    }
                    break;
                case "dreadlands":
                    {
                        zoneProperties.DescriptiveName = "Dreadlands";
                        zoneProperties.SetFogProperties(235, 235, 235, 200, 600);
                        zoneProperties.SetSafePosition(9565.05f, 2806.04f, 1045.2f);
                    }
                    break;
                case "droga":
                    {
                        zoneProperties.DescriptiveName = "Temple of Droga";
                        zoneProperties.SetFogProperties(0, 15, 0, 100, 300);
                        zoneProperties.SetSafePosition(294.11f, 1371.43f, 3.75f);
                    }
                    break;
                case "eastkarana":
                    {
                        zoneProperties.DescriptiveName = "Eastern Karana";
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                        zoneProperties.SetSafePosition(0f, 0f, 3.5f);
                    }
                    break;
                case "eastwastes":
                    {
                        zoneProperties.DescriptiveName = "Eastern Wastes";
                        zoneProperties.SetFogProperties(200, 200, 200, 200, 1800);
                        zoneProperties.SetSafePosition(-4296f, -5049f, 147f);
                    }
                    break;
                case "ecommons":
                    {
                        zoneProperties.DescriptiveName = "East Commonlands";
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                        zoneProperties.SetSafePosition(-1485f, 9.2f, -51f);
                    }
                    break;
                case "emeraldjungle":
                    {
                        zoneProperties.DescriptiveName = "Emerald Jungle";
                        zoneProperties.SetFogProperties(200, 235, 210, 60, 200);
                        zoneProperties.SetSafePosition(4648.06f, -1222.97f, 0f);
                    }
                    break;
                case "erudnext":
                    {
                        zoneProperties.DescriptiveName = "Erudin Docks";
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 550);
                        zoneProperties.SetSafePosition(-309.75f, 109.64f, 23.75f);
                    }
                    break;
                case "erudnint":
                    {
                        zoneProperties.DescriptiveName = "Erudin Palace";
                        zoneProperties.SetFogProperties(0, 0, 0, 0, 0);
                        zoneProperties.SetSafePosition(807f, 712f, 22f);
                    }
                    break;
                case "erudsxing":
                    {
                        zoneProperties.DescriptiveName = "Erud's Crossing";
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                        zoneProperties.SetSafePosition(795f, -1766.9f, 12.36f);
                    }
                    break;
                case "everfrost":
                    {
                        zoneProperties.DescriptiveName = "Everfrost Peaks";
                        zoneProperties.SetFogProperties(200, 230, 255, 10, 500);
                        zoneProperties.SetSafePosition(682.74f, 3139.01f, -60.16f);
                    }
                    break;
                case "fearplane":
                    {
                        zoneProperties.DescriptiveName = "Plane of Fear";
                        zoneProperties.SetFogProperties(255, 50, 10, 10, 1000);
                        zoneProperties.SetSafePosition(1282.09f, -1139.03f, 1.67f);
                    }
                    break;
                case "feerrott":
                    {
                        zoneProperties.DescriptiveName = "The Feerrott";
                        zoneProperties.SetFogProperties(60, 90, 30, 10, 175);
                        zoneProperties.SetSafePosition(902.6f, 1091.7f, 28f);
                    }
                    break;
                case "felwithea":
                    {
                        zoneProperties.DescriptiveName = "North Felwithe";
                        zoneProperties.SetFogProperties(100, 130, 100, 10, 300);
                        zoneProperties.SetSafePosition(94f, -25f, 3.75f);
                    }
                    break;
                case "felwitheb":
                    {
                        zoneProperties.DescriptiveName = "South Felwithe";
                        zoneProperties.SetFogProperties(100, 130, 100, 10, 300);
                        zoneProperties.SetSafePosition(-790f, 320f, -10.25f);
                    }
                    break;
                case "fieldofbone":
                    {
                        zoneProperties.DescriptiveName = "Field of Bone";
                        zoneProperties.SetFogProperties(235, 235, 235, 200, 800);
                        zoneProperties.SetSafePosition(1617f, -1684f, -54.78f);
                    }
                    break;
                case "firiona":
                    {
                        zoneProperties.DescriptiveName = "Firiona Vie";
                        zoneProperties.SetFogProperties(235, 235, 235, 200, 800);
                        zoneProperties.SetSafePosition(1439.96f, -2392.06f, -2.65f);
                    }
                    break;
                case "freporte":
                    {
                        zoneProperties.DescriptiveName = "East Freeport";
                        zoneProperties.SetFogProperties(230, 200, 200, 10, 450);
                        zoneProperties.SetSafePosition(-648f, -1097f, -52.2f);
                    }
                    break;
                case "freportn":
                    {
                        zoneProperties.DescriptiveName = "North Freeport";
                        zoneProperties.SetFogProperties(230, 200, 200, 10, 450);
                        zoneProperties.SetSafePosition(211f, -296f, 4f);
                    }
                    break;
                case "freportw":
                    {
                        zoneProperties.DescriptiveName = "West Freeport";
                        zoneProperties.SetFogProperties(230, 200, 200, 10, 450);
                        zoneProperties.SetSafePosition(181f, 335f, -24f);
                    }
                    break;
                case "frontiermtns":
                    {
                        zoneProperties.DescriptiveName = "Frontier Mountains";
                        zoneProperties.SetFogProperties(235, 235, 235, 200, 800);
                        zoneProperties.SetSafePosition(-4262f, -633f, 113.24f);
                    }
                    break;
                case "frozenshadow":
                    {
                        zoneProperties.DescriptiveName = "Tower of Frozen Shadow";
                        zoneProperties.SetFogProperties(25, 25, 25, 10, 350);
                        zoneProperties.SetSafePosition(200f, 120f, 0f);
                    }
                    break;
                case "gfaydark":
                    {
                        zoneProperties.DescriptiveName = "Greater Faydark";
                        zoneProperties.SetFogProperties(0, 128, 64, 10, 300);
                        zoneProperties.SetSafePosition(10f, -20f, 0f);
                        zoneProperties.AddMaterialGrouping("rockw2", "sfrg4", "sfrg5", "sfrg6", "sfrg7", "sfrg8", "sfrg9", "sfrg10", "sgrass",
                            "spath", "spath45", "spathend", "spathlh", "spathrh", "spatht", "spathtol", "spathtor", "spathy1", "xgrass1",
                            "grastran", "citystone", "citywall", "1dirtfloor");
                    }
                    break;
                case "greatdivide":
                    {
                        zoneProperties.DescriptiveName = "The Great Divide";
                        zoneProperties.SetFogProperties(160, 160, 172, 200, 1800);
                        zoneProperties.SetSafePosition(-965f, -7720f, -557f);
                    }
                    break;
                case "grobb":
                    {
                        zoneProperties.DescriptiveName = "Grobb";
                        zoneProperties.SetFogProperties(0, 0, 0, 500, 2000);
                        zoneProperties.SetSafePosition(0f, -100f, 4f);
                    }
                    break;
                case "growthplane":
                    {
                        zoneProperties.DescriptiveName = "Plane of Growth";
                        zoneProperties.SetFogProperties(0, 50, 100, 60, 1200);
                        zoneProperties.SetSafePosition(3016f, -2522f, -19f);
                    }
                    break;
                case "gukbottom":
                    {
                        zoneProperties.DescriptiveName = "Lower Guk";
                        zoneProperties.SetFogProperties(50, 45, 20, 10, 140);
                        zoneProperties.SetSafePosition(-217f, 1197f, -81.78f);
                    }
                    break;
                case "guktop":
                    {
                        zoneProperties.DescriptiveName = "Upper Guk";
                        zoneProperties.SetFogProperties(40, 45, 20, 10, 140);
                        zoneProperties.SetSafePosition(7f, -36f, 4f);
                    }
                    break;
                case "halas":
                    {
                        zoneProperties.DescriptiveName = "Halas";
                        zoneProperties.SetFogProperties(200, 230, 255, 10, 300);
                        zoneProperties.SetSafePosition(0f, 0f, 3.75f);
                    }
                    break;
                case "hateplane":
                    {
                        zoneProperties.DescriptiveName = "Plane of Hate";
                        zoneProperties.SetFogProperties(128, 128, 128, 30, 200);
                        zoneProperties.SetSafePosition(-353.08f, -374.8f, 3.75f);
                    }
                    break;
                case "highkeep":
                    {
                        zoneProperties.DescriptiveName = "High Keep";
                        zoneProperties.SetFogProperties(0, 0, 0, 0, 0);
                        zoneProperties.SetSafePosition(88f, -16f, 4f);
                    }
                    break;
                case "highpass":
                    {
                        zoneProperties.DescriptiveName = "Highpass Hold";
                        zoneProperties.SetFogProperties(200, 200, 200, 10, 400);
                        zoneProperties.SetSafePosition(-104f, -14f, 4f);
                    }
                    break;
                case "hole":
                    {
                        zoneProperties.DescriptiveName = "The Hole";
                        zoneProperties.SetFogProperties(10, 10, 10, 200, 500);
                        zoneProperties.SetSafePosition(-1049.98f, 640.04f, -77.22f);
                    }
                    break;
                case "iceclad":
                    {
                        zoneProperties.DescriptiveName = "Iceclad Ocean";
                        zoneProperties.SetFogProperties(200, 200, 200, 200, 1800);
                        zoneProperties.SetSafePosition(340f, 5330f, -17f);
                    }
                    break;
                case "innothule":
                    {
                        zoneProperties.DescriptiveName = "Innothule Swamp";
                        zoneProperties.SetFogProperties(170, 160, 90, 10, 500);
                        zoneProperties.SetSafePosition(-588f, -2192f, -25f);
                    }
                    break;
                case "kael":
                    {
                        zoneProperties.DescriptiveName = "Kael Drakkal";
                        zoneProperties.SetFogProperties(10, 10, 50, 20, 500);
                        zoneProperties.SetSafePosition(-633f, -47f, 128f);
                    }
                    break;
                case "kaesora":
                    {
                        zoneProperties.DescriptiveName = "Kaesora";
                        zoneProperties.SetFogProperties(0, 10, 0, 20, 200);
                        zoneProperties.SetSafePosition(40f, 370f, 99.72f);
                    }
                    break;
                case "kaladima":
                    {
                        zoneProperties.DescriptiveName = "North Kaladim";
                        zoneProperties.SetFogProperties(70, 50, 20, 10, 175);
                        zoneProperties.SetSafePosition(-2f, -18f, 3.75f);
                    }
                    break;
                case "kaladimb":
                    {
                        zoneProperties.DescriptiveName = "South Kaladim";
                        zoneProperties.SetFogProperties(70, 50, 20, 10, 175);
                        zoneProperties.SetSafePosition(-267f, 414f, 3.75f);
                    }
                    break;
                case "karnor":
                    {
                        zoneProperties.DescriptiveName = "Karnor's Castle";
                        zoneProperties.SetFogProperties(50, 20, 20, 10, 350);
                        zoneProperties.SetSafePosition(0f, 0f, 4f);
                    }
                    break;
                case "kedge":
                    {
                        zoneProperties.DescriptiveName = "Kedge Keep";
                        zoneProperties.SetFogProperties(10, 10, 10, 25, 25);
                        zoneProperties.SetSafePosition(99.96f, 14.02f, 31.75f);
                    }
                    break;
                case "kerraridge":
                    {
                        zoneProperties.DescriptiveName = "Kerra Island";
                        zoneProperties.SetFogProperties(220, 220, 200, 10, 600);
                        zoneProperties.SetSafePosition(-859.97f, 474.96f, 23.75f);
                    }
                    break;
                case "kithicor":
                    {
                        zoneProperties.DescriptiveName = "Kithicor Forest";
                        zoneProperties.SetFogProperties(120, 140, 100, 10, 200);
                        zoneProperties.SetSafePosition(3828f, 1889f, 459f);
                    }
                    break;
                case "kurn":
                    {
                        zoneProperties.DescriptiveName = "Kurn's Tower";
                        zoneProperties.SetFogProperties(50, 50, 20, 10, 200);
                        zoneProperties.SetSafePosition(77.72f, -277.64f, 3.75f);
                    }
                    break;
                case "lakeofillomen":
                    {
                        zoneProperties.DescriptiveName = "Lake of Ill Omen";
                        zoneProperties.SetFogProperties(235, 235, 235, 200, 800);
                        zoneProperties.SetSafePosition(-5383.07f, 5747.14f, 68.27f);
                    }
                    break;
                case "lakerathe":
                    {
                        zoneProperties.DescriptiveName = "Lake Rathetear";
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                        zoneProperties.SetSafePosition(1213f, 4183f, 4f);
                    }
                    break;
                case "lavastorm":
                    {
                        zoneProperties.DescriptiveName = "Lavastorm Mountains";
                        zoneProperties.SetFogProperties(255, 50, 10, 10, 800);
                        zoneProperties.SetSafePosition(153.45f, -1842.79f, -16.37f);
                    }
                    break;
                case "lfaydark":
                    {
                        zoneProperties.DescriptiveName = "Lesser Faydark";
                        zoneProperties.SetFogProperties(230, 255, 200, 10, 300);
                        zoneProperties.SetSafePosition(-1769.93f, -108.08f, -1.11f);
                    }
                    break;
                case "load":
                    {
                        zoneProperties.DescriptiveName = "Loading Area";
                        zoneProperties.SetFogProperties(0, 0, 0, 500, 2000);
                        zoneProperties.SetSafePosition(-316f, 5f, 8.2f);
                    }
                    break;
                case "mischiefplane":
                    {
                        zoneProperties.DescriptiveName = "Plane of Mischief";
                        zoneProperties.SetFogProperties(210, 235, 210, 60, 600);
                        zoneProperties.SetSafePosition(-395f, -1410f, 115f);
                    }
                    break;
                case "mistmoore":
                    {
                        zoneProperties.DescriptiveName = "Mistmoore Castle";
                        zoneProperties.SetFogProperties(60, 30, 90, 10, 250);
                        zoneProperties.SetSafePosition(123f, -295f, -177f);
                    }
                    break;
                case "misty":
                    {
                        zoneProperties.DescriptiveName = "Misty Thicket";
                        zoneProperties.SetFogProperties(100, 120, 50, 10, 500);
                        zoneProperties.SetSafePosition(0f, 0f, 2.43f);
                    }
                    break;
                case "najena":
                    {
                        zoneProperties.DescriptiveName = "Najena";
                        zoneProperties.SetFogProperties(30, 0, 40, 10, 110);
                        zoneProperties.SetSafePosition(-22.6f, 229.1f, -41.8f);
                    }
                    break;
                case "necropolis":
                    {
                        zoneProperties.DescriptiveName = "Dragon Necropolis";
                        zoneProperties.SetFogProperties(35, 50, 35, 10, 2000);
                        zoneProperties.SetSafePosition(2000f, -100f, 5f);
                    }
                    break;
                case "nektulos":
                    {
                        zoneProperties.DescriptiveName = "Nektulos Forest";
                        zoneProperties.SetFogProperties(80, 90, 70, 10, 400);
                        zoneProperties.SetSafePosition(-259f, -1201f, -5f);
                    }
                    break;
                case "neriaka":
                    {
                        zoneProperties.DescriptiveName = "Neriak Foreign Quarter";
                        zoneProperties.SetFogProperties(10, 0, 60, 10, 250);
                        zoneProperties.SetSafePosition(156.92f, -2.94f, 31.75f);
                    }
                    break;
                case "neriakb":
                    {
                        zoneProperties.DescriptiveName = "Neriak Commons";
                        zoneProperties.SetFogProperties(10, 0, 60, 10, 250);
                        zoneProperties.SetSafePosition(-499.91f, 2.97f, -10.25f);
                    }
                    break;
                case "neriakc":
                    {
                        zoneProperties.DescriptiveName = "Neriak Third Gate";
                        zoneProperties.SetFogProperties(10, 0, 60, 10, 250);
                        zoneProperties.SetSafePosition(-968.96f, 891.92f, -52.22f);
                    }
                    break;
                case "northkarana":
                    {
                        zoneProperties.DescriptiveName = "Northern Karana";
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                        zoneProperties.SetSafePosition(-382f, -284f, -7f);
                    }
                    break;
                case "nro":
                    {
                        zoneProperties.DescriptiveName = "Northern Desert of Ro";
                        zoneProperties.SetFogProperties(250, 250, 180, 10, 800);
                        zoneProperties.SetSafePosition(299.12f, 3537.9f, -24.5f);
                    }
                    break;
                case "nurga":
                    {
                        zoneProperties.DescriptiveName = "Mines of Nurga";
                        zoneProperties.SetFogProperties(0, 15, 0, 100, 300);
                        zoneProperties.SetSafePosition(150f, -1062f, -107f);
                    }
                    break;
                case "oasis":
                    {
                        zoneProperties.DescriptiveName = "Oasis of Marr";
                        zoneProperties.SetFogProperties(250, 250, 180, 10, 800);
                        zoneProperties.SetSafePosition(903.98f, 490.03f, 6.4f);
                    }
                    break;
                case "oggok":
                    {
                        zoneProperties.DescriptiveName = "Oggok";
                        zoneProperties.SetFogProperties(130, 140, 80, 10, 300);
                        zoneProperties.SetSafePosition(-99f, -345f, 4f);
                    }
                    break;
                case "oot":
                    {
                        zoneProperties.DescriptiveName = "Ocean of Tears";
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                        zoneProperties.SetSafePosition(-9200f, 390f, 6f);
                    }
                    break;
                case "overthere":
                    {
                        zoneProperties.DescriptiveName = "The Overthere";
                        zoneProperties.SetFogProperties(235, 235, 235, 200, 800);
                        zoneProperties.SetSafePosition(-4263f, -241f, 235f);
                    }
                    break;
                case "paineel":
                    {
                        zoneProperties.DescriptiveName = "Paineel";
                        zoneProperties.SetFogProperties(150, 150, 150, 200, 850);
                        zoneProperties.SetSafePosition(200f, 800f, 3.39f);
                    }
                    break;
                case "paw":
                    {
                        zoneProperties.DescriptiveName = "Splitpaw Lair";
                        zoneProperties.SetFogProperties(30, 25, 10, 10, 180);
                        zoneProperties.SetSafePosition(-7.9f, -79.3f, 4f);
                    }
                    break;
                case "permafrost":
                    {
                        zoneProperties.DescriptiveName = "Permafrost";
                        zoneProperties.SetFogProperties(25, 35, 45, 10, 180);
                        zoneProperties.SetSafePosition(0f, 0f, 3.75f);
                    }
                    break;
                case "qcat":
                    {
                        zoneProperties.DescriptiveName = "Qeynos Catacombs";
                        zoneProperties.SetFogProperties(0, 0, 0, 500, 2000);
                        zoneProperties.SetSafePosition(-315f, 214f, -38f);
                    }
                    break;
                case "qey2hh1":
                    {
                        zoneProperties.DescriptiveName = "Western Karana";
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                        zoneProperties.SetSafePosition(-638f, 12f, -4f);
                    }
                    break;
                case "qeynos":
                    {
                        zoneProperties.DescriptiveName = "South Qeynos";
                        zoneProperties.SetFogProperties(200, 200, 210, 10, 450);
                        zoneProperties.SetSafePosition(186.46f, 14.29f, 3.75f);
                    }
                    break;
                case "qeynos2":
                    {
                        zoneProperties.DescriptiveName = "North Qeynos";
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 450);
                        zoneProperties.SetSafePosition(114f, 678f, 4f);
                    }
                    break;
                case "qeytoqrg":
                    {
                        zoneProperties.DescriptiveName = "Qeynos Hills";
                        zoneProperties.SetFogProperties(0, 0, 0, 500, 2000);
                        zoneProperties.SetSafePosition(196.7f, 5100.9f, -1f);
                    }
                    break;
                case "qrg":
                    {
                        zoneProperties.DescriptiveName = "Surefall Glade";
                        zoneProperties.SetFogProperties(180, 175, 183, 10, 450);
                        zoneProperties.SetSafePosition(136.9f, -65.9f, 4f);
                    }
                    break;
                case "rathemtn":
                    {
                        zoneProperties.DescriptiveName = "Rathe Mountains";
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                        zoneProperties.SetSafePosition(1831f, 3825f, 29.03f);
                    }
                    break;
                case "rivervale":
                    {
                        zoneProperties.DescriptiveName = "Rivervale";
                        zoneProperties.SetFogProperties(200, 210, 200, 10, 400);
                        zoneProperties.SetSafePosition(45.3f, 1.6f, 3.8f);
                    }
                    break;
                case "runnyeye":
                    {
                        zoneProperties.DescriptiveName = "Runnyeye Citadel";
                        zoneProperties.SetFogProperties(75, 150, 25, 10, 600);
                        zoneProperties.SetSafePosition(-21.85f, -108.88f, 3.75f);
                    }
                    break;
                case "sebilis":
                    {
                        zoneProperties.DescriptiveName = "Old Sebilis";
                        zoneProperties.SetFogProperties(20, 10, 60, 50, 400);
                        zoneProperties.SetSafePosition(0f, 235f, 40f);
                    }
                    break;
                case "sirens":
                    {
                        zoneProperties.DescriptiveName = "Siren's Grotto";
                        zoneProperties.SetFogProperties(30, 100, 130, 10, 500);
                        zoneProperties.SetSafePosition(-33f, 196f, 4f);
                    }
                    break;
                case "skyfire":
                    {
                        zoneProperties.DescriptiveName = "Skyfire Mountains";
                        zoneProperties.SetFogProperties(235, 200, 200, 200, 600);
                        zoneProperties.SetSafePosition(-3931.32f, -1139.25f, 39.76f);
                    }
                    break;
                case "skyshrine":
                    {
                        zoneProperties.DescriptiveName = "Skyshrine";
                        zoneProperties.SetFogProperties(50, 0, 200, 100, 600);
                        zoneProperties.SetSafePosition(-730f, -210f, 0f);
                    }
                    break;
                case "sleeper":
                    {
                        zoneProperties.DescriptiveName = "Sleeper's Tomb";
                        zoneProperties.SetFogProperties(80, 80, 220, 200, 800);
                        zoneProperties.SetSafePosition(0f, 0f, 5f);
                    }
                    break;
                case "soldunga":
                    {
                        zoneProperties.DescriptiveName = "Solusek's Eye";
                        zoneProperties.SetFogProperties(180, 30, 30, 10, 100);
                        zoneProperties.SetSafePosition(-485.77f, -476.04f, 73.72f);
                    }
                    break;
                case "soldungb":
                    {
                        zoneProperties.DescriptiveName = "Nagafen's Lair";
                        zoneProperties.SetFogProperties(180, 30, 30, 10, 350);
                        zoneProperties.SetSafePosition(-262.7f, -423.99f, -108.22f);
                    }
                    break;
                case "soltemple":
                    {
                        zoneProperties.DescriptiveName = "The Temple of Solusek Ro";
                        zoneProperties.SetFogProperties(180, 5, 5, 50, 500);
                        zoneProperties.SetSafePosition(7.5f, 268.8f, 3f);
                    }
                    break;
                case "southkarana":
                    {
                        zoneProperties.DescriptiveName = "Southern Karana";
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                        zoneProperties.SetSafePosition(1293.66f, 2346.69f, -5.77f);
                    }
                    break;
                case "sro":
                    {
                        zoneProperties.DescriptiveName = "Southern Desert of Ro";
                        zoneProperties.SetFogProperties(250, 250, 180, 10, 800);
                        zoneProperties.SetSafePosition(286f, 1265f, 79f);
                    }
                    break;
                case "steamfont":
                    {
                        zoneProperties.DescriptiveName = "Steamfont Mountains";
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                        zoneProperties.SetSafePosition(-272.86f, 159.86f, -21.4f);
                    }
                    break;
                case "stonebrunt":
                    {
                        zoneProperties.DescriptiveName = "Stonebrunt Mountains";
                        zoneProperties.SetFogProperties(235, 235, 235, 10, 800);
                        zoneProperties.SetSafePosition(-1643.01f, -3427.84f, -6.57f);
                    }
                    break;
                case "swampofnohope":
                    {
                        zoneProperties.DescriptiveName = "Swamp of No Hope";
                        zoneProperties.SetFogProperties(210, 200, 210, 60, 400);
                        zoneProperties.SetSafePosition(-1830f, -1259.9f, 27.1f);
                    }
                    break;
                case "templeveeshan":
                    {
                        zoneProperties.DescriptiveName = "Temple of Veeshan";
                        zoneProperties.SetFogProperties(60, 10, 10, 30, 300);
                        zoneProperties.SetSafePosition(-499f, -2086f, -36f);
                    }
                    break;
                case "thurgadina":
                    {
                        zoneProperties.DescriptiveName = "Thurgadin";
                        zoneProperties.SetFogProperties(25, 25, 25, 100, 300);
                        zoneProperties.SetSafePosition(0f, -1222f, 0f);
                    }
                    break;
                case "thurgadinb":
                    {
                        zoneProperties.DescriptiveName = "Thurgadin Mines";
                        zoneProperties.SetFogProperties(25, 25, 25, 100, 300);
                        zoneProperties.SetSafePosition(0f, 250f, 0f);
                    }
                    break;
                case "timorous":
                    {
                        zoneProperties.DescriptiveName = "Timorous Deep";
                        zoneProperties.SetFogProperties(225, 225, 230, 100, 700);
                        zoneProperties.SetSafePosition(2194f, -5392f, 4f);
                    }
                    break;
                case "tox":
                    {
                        zoneProperties.DescriptiveName = "Toxxulia Forest";
                        zoneProperties.SetFogProperties(220, 200, 30, 50, 250);
                        zoneProperties.SetSafePosition(203f, 2295f, -45f);
                    }
                    break;
                case "trakanon":
                    {
                        zoneProperties.DescriptiveName = "Trakanon's Teeth";
                        zoneProperties.SetFogProperties(210, 235, 213, 60, 250);
                        zoneProperties.SetSafePosition(1485.86f, 3868.29f, -340.59f);
                    }
                    break;
                case "tutorial":
                    {
                        zoneProperties.DescriptiveName = "Tutorial";
                        zoneProperties.SetFogProperties(0, 0, 0, 500, 2000);
                        zoneProperties.SetSafePosition(0f, 0f, 0f);
                    }
                    break;
                case "unrest":
                    {
                        zoneProperties.DescriptiveName = "The Estate of Unrest";
                        zoneProperties.SetFogProperties(40, 10, 60, 10, 300);
                        zoneProperties.SetSafePosition(52f, -38f, 3.75f);
                    }
                    break;
                case "veeshan":
                    {
                        zoneProperties.DescriptiveName = "Veeshan's Peak";
                        zoneProperties.SetFogProperties(20, 0, 0, 100, 1200);
                        zoneProperties.SetSafePosition(1682f, 41f, 28f);
                    }
                    break;
                 case "velketor":
                    {
                        zoneProperties.DescriptiveName = "Velketor's Labyrinth";
                        zoneProperties.SetFogProperties(10, 130, 130, 10, 500);
                        zoneProperties.SetSafePosition(-65f, 581f, -152f);
                    }
                    break;
                case "wakening":
                    {
                        zoneProperties.DescriptiveName = "Wakening Land";
                        zoneProperties.SetFogProperties(254, 254, 254, 60, 600);
                        zoneProperties.SetSafePosition(-5000f, -673f, -195f);
                    }
                    break;
                case "warrens":
                    {
                        zoneProperties.DescriptiveName = "The Warrens";
                        zoneProperties.SetFogProperties(0, 15, 0, 100, 300);
                        zoneProperties.SetSafePosition(-930f, 748f, -37.22f);
                    }
                    break;
                case "warslikswood":
                    {
                        zoneProperties.DescriptiveName = "Warsliks Woods";
                        zoneProperties.SetFogProperties(210, 235, 210, 60, 600);
                        zoneProperties.SetSafePosition(-467.95f, -1428.95f, 197.31f);
                    }
                    break;
                case "westwastes":
                    {
                        zoneProperties.DescriptiveName = "Western Wastes";
                        zoneProperties.SetFogProperties(128, 128, 160, 200, 1800);
                        zoneProperties.SetSafePosition(-3499f, -4099f, -16.66f);
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
