using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using EQWOWConverter.EQObjects;
using EQWOWConverter.Common;
using Warcraft.WMO;
using Warcraft.WMO.GroupFile;
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

            // Go through the subfolders for each zone and make the zone files
            List<Zone> zones = new List<Zone>();
            DirectoryInfo zoneRootDirectoryInfo = new DirectoryInfo(zoneFolderRoot);
            DirectoryInfo[] zoneDirectoryInfos = zoneRootDirectoryInfo.GetDirectories();
            foreach (DirectoryInfo zoneDirectory in zoneDirectoryInfos)
            {
                string curZoneDirectory = Path.Combine(zoneFolderRoot, zoneDirectory.Name);
                Logger.WriteLine("Importing EQ zone '" + zoneDirectory.Name + "' at '" + curZoneDirectory);
                Zone curZone = new Zone(zoneDirectory.Name, curZoneDirectory);
                zones.Add(curZone);
            }

            Logger.WriteLine("!!!! Conversion of zones to WOW NYI");
            Logger.WriteLine("Conversion Successful");
            return true;
        }
    }
}
