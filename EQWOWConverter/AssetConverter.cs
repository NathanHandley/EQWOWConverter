using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warcraft.WMO;
using Warcraft.WMO.GroupFile;

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

            // For now, let's test by grabbing a specific zone.  west freeport
            string zonePath = Path.Combine(eqExportsCondensedPath, "zones", "freportw");
            return ConvertZone("freportw", zonePath);
        }

        private static bool ConvertZone(string zoneName, string zonePath)
        {
            Logger.WriteLine("Converting zone '" + zoneName + "' at '" + zonePath);
            if (Directory.Exists(zonePath) == false)
            {
                Logger.WriteLine("ERROR - Could not find path at '" + zonePath + "'");
                return false;
            }

            Logger.WriteLine("Conversion of '" + zoneName + "' complete");
            return true;
        }
    }
}
