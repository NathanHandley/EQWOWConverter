using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter
{
    internal class EQAssetConditioner
    {
        public EQAssetConditioner() { }

        public bool CondenseObjects(string eqExportsRawPath, string eqExportsCondensedPath)
        {
            Console.WriteLine("Condensing Objects Started...");

            // Make sure the raw path exists
            if (Directory.Exists(eqExportsRawPath) == false)
            {
                Console.WriteLine("ERROR - Raw input path of '" + eqExportsRawPath + "' does not exist.");
                Console.WriteLine("Condensing Objects Ended (Failed)");
                return false;
            }

            // Create base folder
            Directory.CreateDirectory(eqExportsCondensedPath);

            // Delete/Recreate the objects subfolder
            string objectsOutputFolder = Path.Combine(eqExportsCondensedPath, "objects");
            if (Directory.Exists(objectsOutputFolder))
                Directory.Delete(objectsOutputFolder, true);
            Directory.CreateDirectory(objectsOutputFolder);

            // Iterate through each zone folder
            string[] zoneDirectories = Directory.GetDirectories(eqExportsRawPath);
            foreach (string zoneDirectory in zoneDirectories)
            {
                // Folder is the zone name
                string zoneName = zoneDirectory.Split('\\').Last();
                Console.WriteLine(" - Processing objects for zone '" + zoneName + "'");

                // Check for objects folder
                string zoneObjectFolder = Path.Combine(zoneDirectory, "Objects");
                if (Directory.Exists(zoneObjectFolder) == false)
                {
                    Console.WriteLine(" - No objects in zone, skipping to next");
                    continue;
                }

                // Iterate through the Objects by name (minus extension)
                // TODO: HERE
            }

            Console.WriteLine("Condensing Objects Ended (Success)");
            return true;
        }
    }
}
