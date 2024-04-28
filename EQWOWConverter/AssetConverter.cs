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
using EQWOWConverter.WOWObjects;
using EQWOWConverter.Common;
using Vector3 = EQWOWConverter.Common.Vector3;

namespace EQWOWConverter
{
    internal class AssetConverter
    {
        public static bool ConvertEQZonesToWOW(string eqExportsCondensedPath, string wowExportPath)
        {
            // TODO: Move this to a config
            UInt32 curWMOID = 7000; // Reserving 7000-7200

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

            // Go through the subfolders for each zone and convert to wow zone
            DirectoryInfo zoneRootDirectoryInfo = new DirectoryInfo(zoneFolderRoot);
            DirectoryInfo[] zoneDirectoryInfos = zoneRootDirectoryInfo.GetDirectories();
            foreach (DirectoryInfo zoneDirectory in zoneDirectoryInfos)
            {
                // For now, skip any zone that isn't the arena
                if (zoneDirectory.Name != "arena")
                    continue;

                // Load the EQ zone
                string curZoneDirectory = Path.Combine(zoneFolderRoot, zoneDirectory.Name);
                Logger.WriteLine("- [" + zoneDirectory.Name + "]: Importing EQ zone '" + zoneDirectory.Name + "' at '" + curZoneDirectory);
                Zone curZone = new Zone(zoneDirectory.Name, curZoneDirectory, curWMOID);
                curWMOID++;
                Logger.WriteLine("- [" + zoneDirectory.Name + "]: Importing of EQ zone '" + zoneDirectory.Name + "' complete");

                // Convert to WOW zone
                CreateWoWZoneFromEQZone(curZone, wowExportPath);
            }

            // Update the 
            Logger.WriteLine(" TODO: WMOAreaTable.dbc ");
            Logger.WriteLine(" TODO: AreaTable.dbc ");

            Logger.WriteLine("Conversion Successful");
            return true;
        }

        public static void CreateWoWZoneFromEQZone(Zone zone, string wowExportPath)
        {
            Logger.WriteLine("- [" + zone.Name + "]: Converting zone '" + zone.Name + "' into a wow zone...");

            // Create the zone WMO objects
            WMO zoneWMO = new WMO(zone, wowExportPath);

            // Create the WDT
            WDT zoneWDT = new WDT(zone, zoneWMO.RootFileRelativePathWithFileName);

            // Output the files
            zoneWMO.WriteToDisk(wowExportPath);
            zoneWDT.WriteToDisk(wowExportPath);

            Logger.WriteLine("- [" + zone.Name + "]: Converting of zone '" + zone.Name + "' complete");
        }
    }
}
