using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Intentially ignoring 'vertex colors'.  Come back if this is needed.

namespace EQWOWConverter
{
    internal class EQAssetConditioner
    {
        public EQAssetConditioner() { }

        public bool CondenseAll(string eqExportsRawPath, string eqExportsCondensedPath)
        {
            Console.WriteLine("Condensing Raw EQ Data...");

            // Make sure the raw path exists
            if (Directory.Exists(eqExportsRawPath) == false)
            {
                Console.WriteLine("ERROR - Raw input path of '" + eqExportsRawPath + "' does not exist.");
                Console.WriteLine("Condensing Failed!");
                return false;
            }

            // Create base folder
            Directory.CreateDirectory(eqExportsCondensedPath);

            // Objects
            if (CondenseObjects(eqExportsRawPath, eqExportsCondensedPath) == false)
            {
                Console.WriteLine("Condensing of objects failed!");
                return false;
            }

            Console.WriteLine("Condensing completed");
            return true;
        }
        
        private bool CondenseObjects(string eqExportsRawPath, string eqExportsCondensedPath)
        {
            Console.WriteLine("Condensing Objects Started...");

            // Delete/Recreate the objects subfolder
            string objectsOutputFolder = Path.Combine(eqExportsCondensedPath, "objects");
            if (Directory.Exists(objectsOutputFolder))
                Directory.Delete(objectsOutputFolder, true);
            Directory.CreateDirectory(objectsOutputFolder);

            // Iterate through each source top folder
            string[] topDirectories = Directory.GetDirectories(eqExportsRawPath);
            foreach (string topDirectory in topDirectories)
            {
                // Folder is the zone name
                string topFolder = topDirectory.Split('\\').Last();
                Console.WriteLine(" - Processing objects in folder '" + topFolder + "'");

                // Check for objects folder
                string objectFolder = Path.Combine(topDirectory, "Objects\\Textures");
                if (Directory.Exists(objectFolder) == false)
                {
                    Console.WriteLine(" - No objects in folder, skipping to next");
                    continue;
                }

                // Iterate through every object file
                string[] filesInObjectRoot = Directory.GetFiles(objectFolder);
                foreach(string file in filesInObjectRoot)
                {
                    string fileNameOnly = Path.GetFileName(file);
                    string sourceObjectFile = Path.Combine(objectFolder, fileNameOnly);
                    string targetObjectFile = Path.Combine(objectsOutputFolder, fileNameOnly);

                    // Compare the files if the destination file already exist
                    if (File.Exists(targetObjectFile) == true)
                    {
                        if (FileTool.AreFilesTheSame(targetObjectFile, sourceObjectFile) == false)
                            Console.WriteLine("- Error! File named '" + fileNameOnly + "' already exists and it's different.  Skipping.");
//                        else
//                            Console.WriteLine("- Skipping file '" + fileNameOnly + "' as it already exists");
                        continue;
                    }

                    // Copy the file
                    File.Copy(sourceObjectFile, targetObjectFile, true); 
                }
            }

            Console.WriteLine("Condensing Objects Ended (Success)");
            return true;
        }
    }
}
