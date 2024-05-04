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
            foreach (DirectoryInfo zoneDirectory in zoneDirectoryInfos)
            {
                // Only do erudes crossing
               // if (zoneDirectory.Name != "erudsxing" && zoneDirectory.Name != "crystal")
               //     continue;

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
            }

            // Update the 
            Logger.WriteLine(" TODO: WMOAreaTable.dbc ");
            Logger.WriteLine(" TODO: AreaTable.dbc ");

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

        public static void ExportTexturesForZone(Zone zone, string zoneInputFolder, string wowExportPath)
        {
            Logger.WriteLine("- [" + zone.Name + "]: Exporting textures for zone '" + zone.Name + "'...");

            // Create the folder to output
            string zoneOutputTextureFolder = Path.Combine(wowExportPath, "World", "Everquest", "ZoneTextures", zone.Name);
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
