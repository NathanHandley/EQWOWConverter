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

        public bool ConditionAll(string eqExportsRawPath, string eqExportsCondensedPath)
        {
            Console.WriteLine("Conditioning Raw EQ Data...");

            // Make sure the raw path exists
            if (Directory.Exists(eqExportsRawPath) == false)
            {
                Console.WriteLine("ERROR - Raw input path of '" + eqExportsRawPath + "' does not exist.");
                Console.WriteLine("Conditioning Failed!");
                return false;
            }

            // Create base folder
            FileTool.CreateBlankDirectory(eqExportsCondensedPath);

            // Delete/Recreate the output folders
            string outputObjectsFolderRoot = Path.Combine(eqExportsCondensedPath, "objects");
            string outputCharactersFolderRoot = Path.Combine(eqExportsCondensedPath, "characters");
            string outputZoneFolderRoot = Path.Combine(eqExportsCondensedPath, "zones");
            string tempFolderRoot = Path.Combine(eqExportsCondensedPath, "temp");
            FileTool.CreateBlankDirectory(outputObjectsFolderRoot);
            FileTool.CreateBlankDirectory(outputCharactersFolderRoot);
            FileTool.CreateBlankDirectory(outputZoneFolderRoot);
            FileTool.CreateBlankDirectory(tempFolderRoot);

            // Iterate through each exported directory
            string[] topDirectories = Directory.GetDirectories(eqExportsRawPath);
            foreach (string topDirectory in topDirectories)
            {
                // Get just the folder name itself for later
                string topDirectoryFolderNameOnly = topDirectory.Split('\\').Last();

                // Bring in the objects of this directory
                FileTool.CopyDirectoryAndContents(topDirectory, tempFolderRoot, true, true);

                // Handle collisions
                // TODO:

                string tempZoneFolder = Path.Combine(tempFolderRoot, "Zone");
                if (Directory.Exists(tempZoneFolder))
                {
                    // Copy the zone folder over
                    string outputZoneFolder = Path.Combine(eqExportsCondensedPath, "zones", topDirectoryFolderNameOnly);
                    FileTool.CopyDirectoryAndContents(tempZoneFolder, outputZoneFolder, true, true);
                }
            }

            // Clean up the temp folder and exit
            Directory.Delete(tempFolderRoot, true);
            Console.WriteLine("Conditioning completed");
            return true;
        }

        /*
        private bool CondenseObjects(string eqExportsRawPath, string eqExportsCondensedPath)
        {
            // Iterate through each source top folder
            string[] topDirectories = Directory.GetDirectories(eqExportsRawPath);
            foreach (string topDirectory in topDirectories)
            {
                string topFolder = topDirectory.Split('\\').Last();
                Console.WriteLine(" - Processing objects in folder '" + topFolder + "'");

                // Check for objects folder
                string objectFolder = Path.Combine(topDirectory, "Objects");
                if (Directory.Exists(objectFolder) == false)
                {
                    Console.WriteLine(" - No objects in folder, skipping to next");
                    continue;
                }

                // Copy the contents to the Temp folder for work
                FileTool.CopyDirectoryAndContents(objectFolder, tempFolder, true, true);

                // Check for collision textures, and rename where required

                // Check for collision object




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
        */
    }
}
