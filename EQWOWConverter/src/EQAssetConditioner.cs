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

                // Process objects
                if (topDirectoryHasObjects)
                {
                    // Delete vertex color data (Hopefully we won't need this...)
                    string objectVertexDataFolder = Path.Combine(tempObjectsFolder, "VertexColors");
                    if (Directory.Exists(objectVertexDataFolder))
                        Directory.Delete(objectVertexDataFolder, true);

                    // Process the object textures
                    ProcessAndCopyObjectTextures(tempObjectsFolder, outputObjectsTexturesFolderRoot);

                    // Process the object files
                    ProcessAndCopyObjectFiles(tempObjectsFolder, outputObjectsFolderRoot, tempZoneFolder);
                }

                // Process characters
                if (topDirectoryHasCharacters)
                {
                    // TODO
                    //if (topDirectoryHasCharacters)
                    //    FileTool.CopyDirectoryAndContents(tempCharactersFolder, outputCharactersFolderRoot, false, true);
                }

                // Process Zones
                if (topDirectoryHasZone)
                {
                    string outputZoneFolder = Path.Combine(eqExportsCondensedPath, "zones", topDirectoryFolderNameOnly);
                    FileTool.CopyDirectoryAndContents(tempZoneFolder, outputZoneFolder, true, true);
                }
            }

            // Clean up the temp folder and exit
            Directory.Delete(tempFolderRoot, true);
            Console.WriteLine("Conditioning completed for model data");
            return true;
        }

        private void ProcessAndCopyObjectTextures(string tempObjectsFolder, string outputObjectsTexturesFolderRoot)
        {
            // Look for texture collisions for different texture files
            string tempObjectTextureFolderName = Path.Combine(tempObjectsFolder, "Textures");
            string[] objectTexturesFiles = Directory.GetFiles(tempObjectTextureFolderName);
            foreach (string objectTextureFile in objectTexturesFiles)
            {
                // Calculate the full paths for comparison
                string objectTextureFileNameOnly = Path.GetFileName(objectTextureFile);
                string originalObjectTextureFileNameOnlyNoExtension = Path.GetFileNameWithoutExtension(objectTextureFile);
                string sourceObjectTextureFile = Path.Combine(tempObjectTextureFolderName, objectTextureFileNameOnly);
                string targetObjectTextureFile = Path.Combine(outputObjectsTexturesFolderRoot, objectTextureFileNameOnly);

                // Loop until there is no unresolved file collision
                bool doesUnresolvedFileCollisionExist;
                uint altIteration = 1;
                do
                {
                    // Compare the files if the destination file already exist
                    doesUnresolvedFileCollisionExist = false;
                    if (File.Exists(targetObjectTextureFile) == true)
                    {
                        // If the files collide but are not the exact same, create a new version
                        if (FileTool.AreFilesTheSame(targetObjectTextureFile, sourceObjectTextureFile) == false)
                        {
                            // Update the file name
                            string newObjectTextureFileNameOnly = originalObjectTextureFileNameOnlyNoExtension + "alt" + altIteration.ToString() + Path.GetExtension(objectTextureFile);
                            altIteration++;
                            Console.WriteLine("- Object Texture Collision with name '" + objectTextureFileNameOnly + "' but different contents so renaming to '" + newObjectTextureFileNameOnly + "'");
                            File.Move(objectTextureFile, Path.Combine(tempObjectTextureFolderName, newObjectTextureFileNameOnly));

                            // Update texture references in material files
                            string[] objectMaterialFiles = Directory.GetFiles(tempObjectsFolder, "*.mtl");
                            foreach (string objectMaterialFile in objectMaterialFiles)
                            {
                                string fileText = File.ReadAllText(objectMaterialFile);
                                if (fileText.Contains(objectTextureFileNameOnly))
                                {
                                    Console.WriteLine("- Object material file '" + objectMaterialFile + "' contained texture '" + objectTextureFileNameOnly + "' which was renamed to '" + newObjectTextureFileNameOnly + "'. Updating material file...");
                                    fileText = fileText.Replace(objectTextureFileNameOnly, newObjectTextureFileNameOnly);
                                    File.WriteAllText(objectMaterialFile, fileText);
                                }
                            }

                            // Continue loop using this new file name as a base
                            objectTextureFileNameOnly = newObjectTextureFileNameOnly;
                            sourceObjectTextureFile = Path.Combine(tempObjectTextureFolderName, objectTextureFileNameOnly);
                            targetObjectTextureFile = Path.Combine(outputObjectsTexturesFolderRoot, objectTextureFileNameOnly);
                            doesUnresolvedFileCollisionExist = true;
                        }
                    }
                }
                while (doesUnresolvedFileCollisionExist);

                // Copy the file
                File.Copy(sourceObjectTextureFile, targetObjectTextureFile, true);
            }
        }

        // Object files are composed of .obj, _collision.obj, and .mtl
        private void ProcessAndCopyObjectFiles(string tempObjectsFolder, string outputObjectsFolder, string tempZoneFolder)
        {
            // Go through all of the object obj files
            string[] objectFiles = Directory.GetFiles(tempObjectsFolder, "*.obj");
            foreach (string objectFile in objectFiles)
            {
                // Skip just collision files
                if (objectFile.Contains("_collision"))
                    continue;

                // Get the related object files
                string objectFileBaseNoExtension = Path.GetFileNameWithoutExtension(objectFile);
                string originalObjectFileBaseNoExtension = objectFileBaseNoExtension;
                string objectOBJFileName = Path.GetFileName(objectFile);
                string objectOBJCollisionFileName = Path.GetFileName(objectFileBaseNoExtension + "_collision" + ".obj");
                string objectMTLFileName = objectFileBaseNoExtension + ".mtl";

                // Calculate related object paths
                string objectSourceOBJFullPath = Path.Combine(tempObjectsFolder, objectOBJFileName);
                string objectSourceOBJCollisionFullPath = Path.Combine(tempObjectsFolder, objectOBJCollisionFileName);
                string objectSourceMTLFullPath = Path.Combine(tempObjectsFolder, objectMTLFileName);
                string objectTargetOBJFullPath = Path.Combine(outputObjectsFolder, objectOBJFileName);
                string objectTargetOBJCollisionFullPath = Path.Combine(outputObjectsFolder, objectOBJCollisionFileName);
                string objectTargetMTLFullPath = Path.Combine(outputObjectsFolder, objectMTLFileName);

                // Loop until there is no unresolved file collision
                bool doesUnresolvedFileCollisionExist;
                uint altIteration = 1;
                do
                {
                    // Compare the files if the destination file already exist
                    doesUnresolvedFileCollisionExist = false;
                    if (File.Exists(objectSourceOBJFullPath) && File.Exists(objectTargetOBJFullPath) &&
                        FileTool.AreFilesTheSame(objectSourceOBJFullPath, objectTargetOBJFullPath) == false)
                    {
                        Console.WriteLine("- Object OBJ file '" + objectOBJFileName + "' had a collision with a file that was different.  Alternate version will be made and associated...");
                        doesUnresolvedFileCollisionExist = true;
                    }
                    if (File.Exists(objectSourceOBJCollisionFullPath) && File.Exists(objectTargetOBJCollisionFullPath) &&
                        FileTool.AreFilesTheSame(objectSourceOBJCollisionFullPath, objectTargetOBJCollisionFullPath) == false)
                    {
                        Console.WriteLine("- Object Collision OBJ file '" + objectOBJCollisionFileName + "' had a collision with a file that was different.  Alternate version will be made and associated...");
                        doesUnresolvedFileCollisionExist = true;
                    }
                    if (File.Exists(objectSourceMTLFullPath) && File.Exists(objectTargetMTLFullPath) &&
                        FileTool.AreFilesTheSame(objectSourceMTLFullPath, objectTargetMTLFullPath) == false)
                    {
                        Console.WriteLine("- Object MTL file '" + objectMTLFileName + "' had a collision with a file that was different.  Alternate version will be made and associated...");
                        doesUnresolvedFileCollisionExist = true;
                    }

                    // Handle renames if there was a collision that wasn't the same
                    if (doesUnresolvedFileCollisionExist == true)
                    {
                        string newObjectFileBaseNoExtension = originalObjectFileBaseNoExtension + "alt" + altIteration.ToString();
                        altIteration++;
                        Console.WriteLine("- New Object file base name is '" + newObjectFileBaseNoExtension);

                        // Rename the core files
                        string newObjectFileName = newObjectFileBaseNoExtension + ".obj";
                        string newObjectCollisionFileName = newObjectFileBaseNoExtension + "_collision.obj";
                        string newObjectMaterialFileName = newObjectFileBaseNoExtension + ".mtl";
                        if (File.Exists(objectSourceOBJFullPath))
                            File.Move(objectSourceOBJFullPath, Path.Combine(tempObjectsFolder, newObjectFileName));
                        if (File.Exists(objectSourceOBJCollisionFullPath))
                            File.Move(objectSourceOBJCollisionFullPath, Path.Combine(tempObjectsFolder, newObjectCollisionFileName));
                        if (File.Exists(objectSourceMTLFullPath))
                            File.Move(objectSourceMTLFullPath, Path.Combine(tempObjectsFolder, newObjectMaterialFileName));

                        // Update comparison references
                        objectSourceOBJFullPath = Path.Combine(tempObjectsFolder, newObjectFileName);
                        objectSourceOBJCollisionFullPath = Path.Combine(tempObjectsFolder, newObjectCollisionFileName);
                        objectSourceMTLFullPath = Path.Combine(tempObjectsFolder, newObjectMaterialFileName);
                        objectTargetOBJFullPath = Path.Combine(outputObjectsFolder, newObjectFileName);
                        objectTargetOBJCollisionFullPath = Path.Combine(outputObjectsFolder, newObjectCollisionFileName);
                        objectTargetMTLFullPath = Path.Combine(outputObjectsFolder, newObjectMaterialFileName);

                        // Update material file reference inside the obj file
                        if (File.Exists(objectSourceOBJFullPath))
                        {
                            string fileText = File.ReadAllText(objectSourceOBJFullPath);
                            if (fileText.Contains(objectMTLFileName))
                            {
                                Console.WriteLine("- Object OBJ named '" + objectOBJFileName + "' has reference to material file '" + objectMTLFileName + "', changing to '" + newObjectMaterialFileName + "'...");
                                fileText = fileText.Replace(objectMTLFileName, newObjectMaterialFileName);
                                File.WriteAllText(objectSourceOBJFullPath, fileText);
                            }
                        }

                        // Update object references in the zone file, if it exists
                        string zoneObjectInstancesFile = Path.Combine(tempZoneFolder, "object_instances.txt");
                        if (File.Exists(zoneObjectInstancesFile))
                        {
                            string zoneName = string.Empty;
                            string[] zoneFiles = Directory.GetFiles(tempZoneFolder, "*.obj");
                            if (zoneFiles.Length > 0)
                                zoneName = Path.GetFileNameWithoutExtension(zoneFiles[0]);
                            Console.WriteLine("- Zone object_instances file for zone '" + zoneName + "' was found in container with object '" + objectOBJFileName + "', so updating references...");
                            string fileText = File.ReadAllText(zoneObjectInstancesFile);
                            fileText = fileText.Replace(objectFileBaseNoExtension + ",", newObjectFileBaseNoExtension + ",");
                            File.WriteAllText(zoneObjectInstancesFile, fileText);
                        }

                        // Update the base file names
                        objectFileBaseNoExtension = newObjectFileBaseNoExtension;
                        objectOBJFileName = newObjectFileName;
                        objectOBJCollisionFileName = newObjectCollisionFileName;
                        objectMTLFileName = newObjectMaterialFileName;
                    }
                }
                while (doesUnresolvedFileCollisionExist);

                // Copy the files
                if (File.Exists(objectSourceOBJFullPath))
                    File.Copy(objectSourceOBJFullPath, objectTargetOBJFullPath, true);
                if (File.Exists(objectSourceOBJCollisionFullPath))
                    File.Copy(objectSourceOBJCollisionFullPath, objectTargetOBJCollisionFullPath, true);
                if (File.Exists(objectSourceMTLFullPath))
                    File.Copy(objectSourceMTLFullPath, objectTargetMTLFullPath, true);
            }
        }
    }
}
