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

        public bool ConditionAllModels(string eqExportsRawPath, string eqExportsCondensedPath)
        {
            Console.WriteLine("Conditioning Raw EQ Model Data (Zones, Characters, Objects)...");

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
            string outputObjectsTexturesFolderRoot = Path.Combine(outputObjectsFolderRoot, "Textures");
            string outputCharactersFolderRoot = Path.Combine(eqExportsCondensedPath, "characters");
            string outputCharactersTexturesFolderRoot = Path.Combine(outputCharactersFolderRoot, "Textures");
            string outputZoneFolderRoot = Path.Combine(eqExportsCondensedPath, "zones");
            string tempFolderRoot = Path.Combine(eqExportsCondensedPath, "temp");
            FileTool.CreateBlankDirectory(outputObjectsFolderRoot);
            FileTool.CreateBlankDirectory(outputObjectsTexturesFolderRoot);
            FileTool.CreateBlankDirectory(outputCharactersFolderRoot);
            FileTool.CreateBlankDirectory(outputCharactersTexturesFolderRoot);
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

                // Determine what objects exist in this folder
                string tempZoneFolder = Path.Combine(tempFolderRoot, "Zone");
                string tempObjectsFolder = Path.Combine(tempFolderRoot, "Objects");
                string tempCharactersFolder = Path.Combine(tempFolderRoot, "Characters");
                bool topDirectoryHasZone = Directory.Exists(tempZoneFolder);
                bool topDirectoryHasObjects = Directory.Exists(tempObjectsFolder);
                bool topDirectoryHasCharacters = Directory.Exists(tempCharactersFolder);


                // Handle collisions
                // TODO:

                // What needs to happen (Objects)
                // - If an object file name (.mtl, .obj) already exists, compare the files + related textures
                //  - If everything matches, we move onto the next
                //  - If something is different, add a suffix, incriment, and reattempt comparison
                // - If an object 'changed' names

                // Process objects
                if (topDirectoryHasObjects)
                {
                    // Delete vertex color data (Hopefully we won't need this...)
                    string objectVertexDataFolder = Path.Combine(tempObjectsFolder, "VertexColors");
                    if (Directory.Exists(objectVertexDataFolder))
                        Directory.Delete(objectVertexDataFolder, true);

                    // Look for texture collisions for different texture files
                    string tempObjectTextureFolderName = Path.Combine(tempObjectsFolder, "Textures");
                    string[] objectTexturesFiles = Directory.GetFiles(tempObjectTextureFolderName);
                    foreach (string objectTextureFile in objectTexturesFiles)
                    {
                        // Calculate the full paths for comparison
                        string objectTextureFileNameOnly = Path.GetFileName(objectTextureFile);
                        string sourceObjectFile = Path.Combine(tempObjectTextureFolderName, objectTextureFileNameOnly);
                        string targetObjectFile = Path.Combine(outputObjectsTexturesFolderRoot, objectTextureFileNameOnly);

                        // Loop until there is no unresolved file collision
                        bool doesUnresolvedFileCollisionExist = false;
                        do
                        {
                            // Compare the files if the destination file already exist
                            doesUnresolvedFileCollisionExist = false;
                            if (File.Exists(targetObjectFile) == true)
                            {
                                // If the files collide but are not the exact same, create a new version
                                if (FileTool.AreFilesTheSame(targetObjectFile, sourceObjectFile) == false)
                                {
                                    // Update the file name
                                    string newObjectTextureFileNameOnly = Path.GetFileNameWithoutExtension(objectTextureFile) + "alt1" + Path.GetExtension(objectTextureFile);
                                    Console.WriteLine("- Object Texture Collision with name '" + objectTextureFileNameOnly + "' but different contents so renaming to '" + newObjectTextureFileNameOnly + "'");
                                    File.Move(objectTextureFile, Path.Combine(tempObjectTextureFolderName, newObjectTextureFileNameOnly));

                                    // Update texture references in material files
                                    string[] objectMaterialFiles = Directory.GetFiles(tempObjectsFolder, "*.mtl");
                                    foreach (string objectMaterialFile in objectMaterialFiles)
                                    {
                                        string fileText = File.ReadAllText(objectMaterialFile);
                                        fileText = fileText.Replace(objectTextureFileNameOnly, newObjectTextureFileNameOnly);
                                        File.WriteAllText(objectMaterialFile, fileText);
                                    }

                                    // Continue loop using this new file name as a base
                                    objectTextureFileNameOnly = newObjectTextureFileNameOnly;
                                    sourceObjectFile = Path.Combine(tempObjectTextureFolderName, objectTextureFileNameOnly);
                                    targetObjectFile = Path.Combine(outputObjectsTexturesFolderRoot, objectTextureFileNameOnly);                                    
                                    doesUnresolvedFileCollisionExist = true;
                                }
                            }
                        }
                        while (doesUnresolvedFileCollisionExist);

                        // Copy the file
                        File.Copy(sourceObjectFile, targetObjectFile, true);
                    }
                }




                // Copy folders if they exist
                if (topDirectoryHasZone)
                {
                    string outputZoneFolder = Path.Combine(eqExportsCondensedPath, "zones", topDirectoryFolderNameOnly);
                    FileTool.CopyDirectoryAndContents(tempZoneFolder, outputZoneFolder, true, true);
                }
                if (topDirectoryHasObjects)
                    FileTool.CopyDirectoryAndContents(tempObjectsFolder, outputObjectsFolderRoot, false, true);
                if (topDirectoryHasCharacters)
                    FileTool.CopyDirectoryAndContents(tempCharactersFolder, outputCharactersFolderRoot, false, true);
            }

            // Clean up the temp folder and exit
            Directory.Delete(tempFolderRoot, true);
            Console.WriteLine("Conditioning completed for model data");
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



/*
                if (topDirectoryHasObjects)
        {
            // Delete vertex color data (Hopefully we won't need this...)
            string objectVertexDataFolder = Path.Combine(tempObjectsFolder, "VertexColors");
            if (Directory.Exists(objectVertexDataFolder))
                Directory.Delete(objectVertexDataFolder, true);

            // Look for texture collisions for different texture files
            string tempObjectTextureFolderName = Path.Combine(tempObjectsFolder, "Textures");
            string[] objectTexturesFiles = Directory.GetFiles(tempObjectTextureFolderName);
            foreach (string objectTextureFile in objectTexturesFiles)
            {
                // Calculate the full paths for comparison
                string objectTextureFileNameOnlyOriginal = Path.GetFileName(objectTextureFile);
                string sourceObjectFile = Path.Combine(tempObjectTextureFolderName, objectTextureFileNameOnlyOriginal);
                string targetObjectFile = Path.Combine(outputObjectsTexturesFolderRoot, objectTextureFileNameOnlyOriginal);

                // Compare the files if the destination file already exist
                if (File.Exists(targetObjectFile) == true)
                {
                    // If the files collide but are not the exact same, create a new version
                    if (FileTool.AreFilesTheSame(targetObjectFile, sourceObjectFile) == false)
                    {
                        Console.WriteLine("- Object Texture Collision with name '" + objectTextureFileNameOnlyOriginal + "' but different contents.");
                        string newObjectTextureFileName = Path.GetFileNameWithoutExtension(objectTextureFile) + "alt1" + Path.GetExtension(objectTextureFile);
                    }
                }

                // Copy the file
                File.Copy(sourceObjectFile, targetObjectFile, true);
            }
        }
        */
}
}
