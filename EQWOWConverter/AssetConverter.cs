using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using EQWOWConverter.EQObjects;
using EQWOWConverter.Common;
using Vector3 = EQWOWConverter.Common.Vector3;

namespace EQWOWConverter
{
    internal class AssetConverter
    {
        public static bool ConvertEQZonesToWOW(string eqExportsCondensedPath)
        {
            // Temp for now, just use one zone

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

            // Go through the subfolders for each zone and load them in for processing
            List<EQZone> zones = new List<EQZone>();
            DirectoryInfo zoneRootDirectoryInfo = new DirectoryInfo(zoneFolderRoot);
            DirectoryInfo[] zoneDirectoryInfos = zoneRootDirectoryInfo.GetDirectories();
            Logger.WriteLine("Loading EQ zone files into memory...");
            foreach (DirectoryInfo zoneDirectory in zoneDirectoryInfos)
            {
                string curZoneDirectory = Path.Combine(zoneFolderRoot, zoneDirectory.Name);
                Logger.WriteLine("- [" + zoneDirectory.Name + "]: Importing EQ zone '" + zoneDirectory.Name + "' at '" + curZoneDirectory);
                EQZone curZone = new EQZone(zoneDirectory.Name, curZoneDirectory);
                zones.Add(curZone);
                Logger.WriteLine("- [" + zoneDirectory.Name + "]: Importing of EQ zone '" + zoneDirectory.Name + "' complete");
            }
            Logger.WriteLine("EQ zone load complete.");

            // Make respective warcraft maps for the zones
            Logger.WriteLine("Converting EQ zones into WOW zones");
            foreach (EQZone zone in zones)
            {
                CreateWoWZoneFromEQZone(zone);
            }
            Logger.WriteLine("EQ zones converted into WOW zones complete");

            Logger.WriteLine("Conversion Successful");
            return true;
        }

        public static void CreateWoWZoneFromEQZone(EQZone zone)
        {
            Logger.WriteLine("- [" + zone.Name + "]: Converting zone '" + zone.Name + "' into a wow zone...");

            





            Logger.WriteLine("- [" + zone.Name + "]: Converting of zone '" + zone.Name + "' complete");
        }
    }
}
