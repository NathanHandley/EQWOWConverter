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
                //if (zoneDirectory.Name != "iceclad")
                //    continue;

                // Load the EQ zone
                Zone curZone = new Zone(zoneDirectory.Name, zoneDirectory.Name);
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
            Logger.WriteLine("- [" + zone.Name + "]: Converting zone '" + zone.Name + "' into a wow zone...");

            // Generate the WOW zone data first
            zone.PopulateWOWZoneDataFromEQZoneData();

            // Create the zone WMO objects
            WMO zoneWMO = new WMO(zone, exportMPQRootFolder);
            zoneWMO.WriteToDisk(exportMPQRootFolder);

            // Create the WDT
            WDT zoneWDT = new WDT(zone, zoneWMO.RootFileRelativePathWithFileName);
            zoneWDT.WriteToDisk(exportMPQRootFolder);

            // Create the WDL
            WDL zoneWDL = new WDL(zone);
            zoneWDL.WriteToDisk(exportMPQRootFolder);

            Logger.WriteLine("- [" + zone.Name + "]: Converting of zone '" + zone.Name + "' complete");
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
                areaTableDBC.AddRow(Convert.ToInt32(zone.WOWZoneData.AreaID), zone.Name);
                mapDBC.AddRow(zone.WOWZoneData.MapID, "EQ_" + zone.ShortName, zone.Name, Convert.ToInt32(zone.WOWZoneData.AreaID));
                difficultyDBC.AddRow(zone.WOWZoneData.MapID);
                foreach(WorldModelObject wmo in zone.WOWZoneData.WorldObjects)
                {
                    wmoAreaTableDBC.AddRow(Convert.ToInt32(zone.WOWZoneData.WMOID), Convert.ToInt32(wmo.WMOGroupID),
                        Convert.ToInt32(zone.WOWZoneData.AreaID), zone.Name);
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
                gameTeleSQL.AddRow(Convert.ToInt32(zone.WOWZoneData.MapID), zone.Name);
                instanceTemplateSQL.AddRow(Convert.ToInt32(zone.WOWZoneData.MapID));
            }

            // Output them
            gameTeleSQL.WriteToDisk(sqlScriptFolder);
            instanceTemplateSQL.WriteToDisk(sqlScriptFolder);
            
            Logger.WriteLine("AzerothCore SQL Scripts created successfully");
        }

        public static void ExportTexturesForZone(Zone zone, string zoneInputFolder, string wowExportPath)
        {
            Logger.WriteLine("- [" + zone.Name + "]: Exporting textures for zone '" + zone.Name + "'...");

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
                Logger.WriteLine("- [" + zone.Name + "]: Texture named '" + textureName + "' copied");
            }

            Logger.WriteLine("- [" + zone.Name + "]: Texture output for zone '" + zone.Name + "' complete");
        }
    }
}
