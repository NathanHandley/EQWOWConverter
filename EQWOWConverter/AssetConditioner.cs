//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2025 Nathan Handley
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

using EQWOWConverter.Common;
using EQWOWConverter.EQFiles;
using EQWOWConverter.GameObjects;
using System.Drawing;
using System.Text;

namespace EQWOWConverter
{
    internal class AssetConditioner
    {
        private static List<string> PNGtoBLPFilesToConvert = new List<string>();
        private static readonly object PNGtoBLPLock = new object();

        public bool ConditionEQOutput()
        {
            LogCounter progressCounter = new LogCounter("Conditioning EQ folders...");

            // Make sure the raw path exists
            string eqExportsRawPath = Configuration.PATH_EQEXPORTSRAW_FOLDER;
            if (Directory.Exists(eqExportsRawPath) == false)
            {
                Logger.WriteError("Error - Raw input path of '" + eqExportsRawPath + "' does not exist.");
                Logger.WriteError("Conditioning Failed!");
                return false;
            }

            // Create the output folders
            string eqExportsCondensedPath = Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER;
            string outputObjectsFolderRoot = Path.Combine(eqExportsCondensedPath, "objects");
            string outputObjectsTexturesFolderRoot = Path.Combine(outputObjectsFolderRoot, "textures");
            string outputObjectsMeshesFolderRoot = Path.Combine(outputObjectsFolderRoot, "meshes");
            string outputObjectsMaterialsFolderRoot = Path.Combine(outputObjectsFolderRoot, "materiallists");
            string outputObjectsAnimationsFolderRoot = Path.Combine(outputObjectsFolderRoot, "animations");
            string outputObjectsSkeletonsFolderRoot = Path.Combine(outputObjectsFolderRoot, "skeletons");
            string outputCharactersFolderRoot = Path.Combine(eqExportsCondensedPath, "characters");
            string outputSoundsFolderRoot = Path.Combine(eqExportsCondensedPath, "sounds");
            string outputMusicFolderRoot = Path.Combine(eqExportsCondensedPath, "music");
            string outputEquipmentFolderRoot = Path.Combine(eqExportsCondensedPath, "equipment");
            string outputMiscImagesFolderRoot = Path.Combine(eqExportsCondensedPath, "miscimages");
            string outputZoneFolderRoot = Path.Combine(eqExportsCondensedPath, "zones");
            string outputLiquidSurfacesFolderRoot = Path.Combine(eqExportsCondensedPath, "liquidsurfaces");
            string tempFolderRoot = Path.Combine(eqExportsCondensedPath, "temp");
            if (Directory.Exists(eqExportsCondensedPath))
                Directory.Delete(eqExportsCondensedPath, true);
            Directory.CreateDirectory(eqExportsCondensedPath);
            Directory.CreateDirectory(outputObjectsFolderRoot);
            Directory.CreateDirectory(outputObjectsTexturesFolderRoot);
            Directory.CreateDirectory(outputObjectsMeshesFolderRoot);
            Directory.CreateDirectory(outputObjectsMaterialsFolderRoot);
            Directory.CreateDirectory(outputObjectsAnimationsFolderRoot);
            Directory.CreateDirectory(outputObjectsSkeletonsFolderRoot);
            Directory.CreateDirectory(outputZoneFolderRoot);
            Directory.CreateDirectory(outputLiquidSurfacesFolderRoot);

            // Keep a store of all objects found
            SortedSet<string> staticObjectNames = new SortedSet<string>();
            SortedSet<string> skeletalObjectNames = new SortedSet<string>();

            // Read in the game objects by zone for reference updating
            Dictionary<string, List<string>> staticGameObjectSourceModelNamesByZoneShortName = GameObject.GetSourceStaticModelNamesByZoneShortName();
            Dictionary<string, List<string>> skeletalGameObjectSourceModelNamesByZoneShortName = GameObject.GetSourceSkeletalModelNamesByZoneShortName();

            // Iterate through each exported directory and process objects and zones
            string[] topDirectories = Directory.GetDirectories(eqExportsRawPath);
            foreach (string topDirectory in topDirectories)
            {
                // Get just the folder name itself for later
                string topDirectoryFolderNameOnly = topDirectory.Split('\\').Last();

                // Bring in the objects of this directory
                FileTool.CopyDirectoryAndContents(topDirectory, tempFolderRoot, true, true);

                // Skip some the following folders
                if (topDirectoryFolderNameOnly == "sky" || topDirectoryFolderNameOnly == "frontend" || topDirectoryFolderNameOnly == "video")
                    continue;

                // If it's the character, music, equipment, clientdata, or sound folder then copy it as-is
                if (topDirectoryFolderNameOnly == "characters" || topDirectoryFolderNameOnly == "sounds" || topDirectoryFolderNameOnly == "music" || topDirectoryFolderNameOnly == "equipment" || topDirectoryFolderNameOnly == "clientdata")
                {
                    Logger.WriteDebug("- [" + topDirectoryFolderNameOnly + "] Copying special folder containing these objects");
                    string outputFolder = Path.Combine(eqExportsCondensedPath, topDirectoryFolderNameOnly);
                    FileTool.CopyDirectoryAndContents(tempFolderRoot, outputFolder, true, true);
                    continue;
                }

                // If it's a bmpwad (contains misc images) then copy that into the miscimages folder
                if (topDirectoryFolderNameOnly.StartsWith("bmpwad"))
                {
                    Logger.WriteDebug("- [" + topDirectoryFolderNameOnly + "] Copying special folder containing these objects, and resizing loading screens");
                    string outputMiscImagesFolder = Path.Combine(eqExportsCondensedPath, "miscimages");
                    FileTool.CopyDirectoryAndContents(tempFolderRoot, outputMiscImagesFolder, false, true);

                    // Resize the loading screens
                    if (topDirectoryFolderNameOnly == "bmpwad")
                    {
                        string inputLoadingScreenFileName = Path.Combine(tempFolderRoot, "logo03.png");
                        string outputLoadingScreenFileName = Path.Combine(outputMiscImagesFolder, "logo03resized.png");
                        ImageTool.GenerateResizedImage(inputLoadingScreenFileName, outputLoadingScreenFileName, 1024, 1024);
                    }
                    else if (topDirectoryFolderNameOnly == "bmpwad4")
                    {
                        string inputLoadingScreenFileName = Path.Combine(tempFolderRoot, "eqkload.png");
                        string outputLoadingScreenFileName = Path.Combine(outputMiscImagesFolder, "eqkloadresized.png");
                        ImageTool.GenerateResizedImage(inputLoadingScreenFileName, outputLoadingScreenFileName, 1024, 1024);
                    }
                    else if (topDirectoryFolderNameOnly == "bmpwad5")
                    {
                        string inputLoadingScreenFileName = Path.Combine(tempFolderRoot, "eqvload.png");
                        string outputLoadingScreenFileName = Path.Combine(outputMiscImagesFolder, "eqvloadresized.png");
                        ImageTool.GenerateResizedImage(inputLoadingScreenFileName, outputLoadingScreenFileName, 1024, 1024);
                    }
                    continue;
                }

                // Everything else is a zone
                string tempZoneFolder = Path.Combine(tempFolderRoot, "Zone");
                string tempObjectsFolder = Path.Combine(tempFolderRoot, "Objects");

                // Copy the core zone folder
                string outputZoneFolder = Path.Combine(eqExportsCondensedPath, "zones", topDirectoryFolderNameOnly);
                FileTool.CopyDirectoryAndContents(tempZoneFolder, outputZoneFolder, true, true);

                // Bring over any object vertex colors
                string tempZoneObjectVertexColorFolder = Path.Combine(tempObjectsFolder, "VertexColors");
                if (Directory.Exists(tempZoneObjectVertexColorFolder))
                {
                    string outputZoneObjectVertexColorFolder = Path.Combine(outputZoneFolder, "StaticObjectVertexColors");
                    FileTool.CopyDirectoryAndContents(tempZoneObjectVertexColorFolder, outputZoneObjectVertexColorFolder, true, true);
                }

                // Track renames
                Dictionary<string, string> oldToNewObjectRenames = new Dictionary<string, string>();
                
                // Process actor objects
                string actorSkeletalFile = Path.Combine(tempObjectsFolder, "actors_skeletal.txt");
                if (File.Exists(actorSkeletalFile))
                {
                    foreach(string line in FileTool.ReadAllStringLinesFromFile(actorSkeletalFile, false, true))
                    {
                        if (line.Contains("#") == true)
                            continue;
                        string inputObjectName = line.Split(",")[0];
                        if (oldToNewObjectRenames.ContainsKey(inputObjectName))
                            inputObjectName = oldToNewObjectRenames[inputObjectName];
                        string outputObjectName;
                        ProcessAndCopyObject(topDirectory, tempObjectsFolder, tempZoneFolder, inputObjectName, outputObjectsFolderRoot, true, out outputObjectName);
                        if (skeletalObjectNames.Contains(outputObjectName) == false)
                            skeletalObjectNames.Add(outputObjectName);
                        if (oldToNewObjectRenames.ContainsKey(inputObjectName) == false)
                            oldToNewObjectRenames.Add(inputObjectName, outputObjectName);
                    }
                }
                Dictionary<string, string> skeletalGameObjectNameMap = new Dictionary<string, string>();
                if (skeletalGameObjectSourceModelNamesByZoneShortName.ContainsKey(topDirectoryFolderNameOnly) == true)
                {
                    foreach (string sourceModelName in skeletalGameObjectSourceModelNamesByZoneShortName[topDirectoryFolderNameOnly])
                    {
                        // If the object wasn't mapped yet, process it
                        if (oldToNewObjectRenames.ContainsKey(sourceModelName) == false)
                        {
                            string outputObjectName;
                            ProcessAndCopyObject(topDirectory, tempObjectsFolder, tempZoneFolder, sourceModelName, outputObjectsFolderRoot, true, out outputObjectName);
                            if (sourceModelName != outputObjectName)
                                oldToNewObjectRenames.Add(sourceModelName, outputObjectName);
                        }
                        skeletalGameObjectNameMap.Add(sourceModelName, oldToNewObjectRenames[sourceModelName]);
                    }
                }

                // Clear the rename cache
                oldToNewObjectRenames.Clear();

                // Process static objects
                string actorStaticFile = Path.Combine(tempObjectsFolder, "actors_static.txt");
                if (File.Exists(actorStaticFile))
                {
                    foreach (string line in FileTool.ReadAllStringLinesFromFile(actorStaticFile, false, true))
                    {
                        if (line.Contains("#") == true)
                            continue;
                        string inputObjectName = line.Split(",")[0];
                        if (oldToNewObjectRenames.ContainsKey(inputObjectName))
                            inputObjectName = oldToNewObjectRenames[inputObjectName];
                        string outputObjectName;
                        ProcessAndCopyObject(topDirectory, tempObjectsFolder, tempZoneFolder, inputObjectName, outputObjectsFolderRoot, false, out outputObjectName);
                        if (staticObjectNames.Contains(outputObjectName) == false)
                            staticObjectNames.Add(outputObjectName);
                        if (oldToNewObjectRenames.ContainsKey(inputObjectName) == false)
                            oldToNewObjectRenames.Add(inputObjectName, outputObjectName);
                    }
                }
                Dictionary<string, string> staticGameObjectNameMap = new Dictionary<string, string>();
                if (staticGameObjectSourceModelNamesByZoneShortName.ContainsKey(topDirectoryFolderNameOnly) == true)
                {
                    foreach (string sourceModelName in staticGameObjectSourceModelNamesByZoneShortName[topDirectoryFolderNameOnly])
                    {
                        // Special logic for these objects that will show up in future folders
                        if (topDirectoryFolderNameOnly == "highkeep" && sourceModelName == "bbboard") // It's in qeynos
                        {
                            staticGameObjectNameMap.Add(sourceModelName, sourceModelName);
                            continue;
                        }

                        // If the object wasn't mapped yet, process it
                        if (oldToNewObjectRenames.ContainsKey(sourceModelName) == false)
                        {
                            string outputObjectName;
                            ProcessAndCopyObject(topDirectory, tempObjectsFolder, tempZoneFolder, sourceModelName, outputObjectsFolderRoot, false, out outputObjectName);
                            if (sourceModelName != outputObjectName)
                                oldToNewObjectRenames.Add(sourceModelName, outputObjectName);
                        }
                        staticGameObjectNameMap.Add(sourceModelName, oldToNewObjectRenames[sourceModelName]);
                    }
                }

                // Copy the object instances again in case it changed
                string sourceObjectInstancesFile = Path.Combine(tempZoneFolder, "object_instances.txt");
                string targetObjectInstancesFile = Path.Combine(outputZoneFolder, "object_instances.txt");
                FileTool.CopyFile(sourceObjectInstancesFile, targetObjectInstancesFile);

                // Generate the object lists
                string outputStaticObjectListFileName = Path.Combine(outputObjectsFolderRoot, "static_objects.txt");
                using (var outputFile = new StreamWriter(outputStaticObjectListFileName))
                    foreach (string line in staticObjectNames)
                        outputFile.WriteLine(line);
                string outputSkeletalObjectListFileName = Path.Combine(outputObjectsFolderRoot, "skeletal_objects.txt");
                using (var outputFile = new StreamWriter(outputSkeletalObjectListFileName))
                    foreach (string line in skeletalObjectNames)
                        outputFile.WriteLine(line);

                // Generate an object lists for game objects, but put it in the zone folder
                string outputStaticGameObjectObjectMapFileName = Path.Combine(outputZoneFolder, "gameobject_static_map.txt");
                using (var outputFile = new StreamWriter(outputStaticGameObjectObjectMapFileName))
                    foreach (var curGameObjectNameMap in staticGameObjectNameMap)
                        outputFile.WriteLine(string.Concat(curGameObjectNameMap.Key, ",", curGameObjectNameMap.Value));
                string outputSkeletalGameObjectObjectMapFileName = Path.Combine(outputZoneFolder, "gameobject_skeletal_map.txt");
                using (var outputFile = new StreamWriter(outputSkeletalGameObjectObjectMapFileName))
                    foreach (var curGameObjectNameMap in skeletalGameObjectNameMap)
                        outputFile.WriteLine(string.Concat(curGameObjectNameMap.Key, ",", curGameObjectNameMap.Value));

                // Copy files that were missing in the original folders for some reason
                if (topDirectoryFolderNameOnly == "fearplane")
                {
                    Logger.WriteDebug("- [" + topDirectoryFolderNameOnly + "] Copying texture file 'maywall' not found in the original zone folder...");
                    string inputFileName = Path.Combine(tempObjectsFolder, "Textures", "maywall.png");
                    string outputFileName = Path.Combine(outputZoneFolder, "Textures", "maywall.png");
                    FileTool.CopyFile(inputFileName, outputFileName);
                }
                else if (topDirectoryFolderNameOnly == "oasis")
                {
                    Logger.WriteDebug("- [" + topDirectoryFolderNameOnly + "] Copying texture file 'canwall1' not found in the original zone folder...");
                    string inputFileName = Path.Combine(tempObjectsFolder, "Textures", "canwall1.png");
                    string outputFileName = Path.Combine(outputZoneFolder, "Textures", "canwall1.png");
                    FileTool.CopyFile(inputFileName, outputFileName);
                }
                else if (topDirectoryFolderNameOnly == "swampofnohope")
                {
                    Logger.WriteDebug("- [" + topDirectoryFolderNameOnly + "] Copying texture file 'kruphse3' not found in the original zone folder...");
                    string inputFileName = Path.Combine(tempObjectsFolder, "Textures", "kruphse3.png");
                    string outputFileName = Path.Combine(outputZoneFolder, "Textures", "kruphse3.png");
                    FileTool.CopyFile(inputFileName, outputFileName);
                }

                progressCounter.Write(1);
            }

            // Clean up the temp folder
            Directory.Delete(tempFolderRoot, true);

            // Save the texture coordinates in the material lists
            Logger.WriteInfo("Saving texture sizes in material lists...");
            topDirectories = Directory.GetDirectories(eqExportsCondensedPath);
            foreach (string topDirectory in topDirectories)
            {
                // Get just the folder name itself for later
                string topDirectoryFolderNameOnly = topDirectory.Split('\\').Last();

                // Different logic based on type of folder
                if (topDirectoryFolderNameOnly == "zones")
                {
                    string[] zoneDirectories = Directory.GetDirectories(topDirectory);
                    foreach (string zoneDirectory in zoneDirectories)
                    {
                        // Get just the zone folder
                        string zoneDirectoryFolderNameOnly = topDirectory.Split('\\').Last();
                        ResizeTexturesAndSaveCoordinatesInMaterialLists(zoneDirectoryFolderNameOnly, zoneDirectory);
                    }
                }
                else if (topDirectoryFolderNameOnly == "characters" || topDirectoryFolderNameOnly == "objects" || topDirectoryFolderNameOnly == "equipment")
                {
                    ResizeTexturesAndSaveCoordinatesInMaterialLists(topDirectoryFolderNameOnly, topDirectory);
                }
            }

            // Replace the custom textures
            ReplaceCustomTextures(outputCharactersFolderRoot, outputObjectsFolderRoot);

            // Convert music
            ConditionMusicFiles(outputMusicFolderRoot);

            // Create icons
            CreateIndividualIconFiles();

            // Generate the liquid surfaces
            Logger.WriteInfo("Generating liquid surface materials...");
            for (int i = 1; i <= 30; i++)
            {
                string curFileName = "eqclear." + i + ".png";
                ImageTool.GenerateNewTransparentImage(Path.Combine(outputLiquidSurfacesFolderRoot, curFileName), 256, 256);
            }

            Logger.WriteInfo("Conditioning complete.");
            return true;
        }

        public void GenerateSpellParticleSpriteSheets()
        {
            Logger.WriteInfo("Generating spell particle sprite sheets...");

            // Load in the known unique sprite names out of spells.eff
            string spellsEFFFileFullPath = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "clientdata", "spells.eff");
            if (Path.Exists(spellsEFFFileFullPath) == false)
            {
                Logger.WriteError("Could not find spells.eff data that should be at ", spellsEFFFileFullPath, ", did you not run the extraction step?");
                return;
            }
            EQSpellsEFF eqSpellsEFF = new EQSpellsEFF();
            eqSpellsEFF.LoadFromDisk(spellsEFFFileFullPath);

            // Create a list of sprite chains
            Dictionary<string, List<string>> spriteChainsBySpriteRoot = new Dictionary<string, List<string>>();
            foreach (string rootSpriteName in eqSpellsEFF.UniqueSpriteNames)
            {
                // Skip if this file doesn't exist, since there wouldn't be subsequent anyway
                string sourceTextureFolder = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "equipment", "Textures");
                string spriteFileNameFullPath = Path.Combine(sourceTextureFolder, string.Concat(rootSpriteName, ".png"));
                if (File.Exists(spriteFileNameFullPath) == false)
                    continue;

                // Separate the number from the text
                string rootTextName = rootSpriteName.Substring(0, rootSpriteName.Length - 2);
                string rootNumberPartString = rootSpriteName.Substring(rootSpriteName.Length - 2);
                int rootNumber = int.Parse(rootNumberPartString);

                // Start a chain of sprites by going through in sequence until a sprite isn't found
                spriteChainsBySpriteRoot.Add(rootSpriteName, new List<string>());
                spriteChainsBySpriteRoot[rootSpriteName].Add(rootSpriteName);
                bool spriteFound = true;
                while (spriteFound)
                {
                    rootNumber++;
                    string nextSpriteName = string.Concat(rootTextName, rootNumber.ToString());
                    if (rootNumber < 10)
                        nextSpriteName = string.Concat(rootTextName, "0", rootNumber.ToString());
                    string nextSpriteFullFileName = Path.Combine(sourceTextureFolder, string.Concat(nextSpriteName, ".png"));
                    if (File.Exists(nextSpriteFullFileName) == false)
                    {
                        spriteFound = false;
                        continue;
                    }
                    spriteChainsBySpriteRoot[rootSpriteName].Add(nextSpriteName);
                }
            }

            Logger.WriteInfo("Generating spell particle sprite sheets complete...");
        }

        private void ResizeTexturesAndSaveCoordinatesInMaterialLists(string topFolderName, string workingRootFolderPath)
        {
            string materialListFolder = Path.Combine(workingRootFolderPath, "MaterialLists");
            string textureFolder = Path.Combine(workingRootFolderPath, "Textures");
            string[] materialListFiles = Directory.GetFiles(materialListFolder);
            foreach (string materialListFile in materialListFiles)
            {
                string materialFileText = FileTool.ReadAllDataFromFile(materialListFile);
                string[] materialFileRows = materialFileText.Split(Environment.NewLine);
                string[] materialFileRowsForWrite = materialFileText.Split(Environment.NewLine);
                EQMaterialList materialListData = new EQMaterialList();
                materialListData.LoadFromDisk(materialListFile);
                int rowIndex = -1;
                foreach (string materialFileRow in materialFileRows)
                {
                    rowIndex++;
                    // Skip blank lines and comments
                    if (materialFileRow.Length == 0 || materialFileRow.StartsWith("#"))
                        continue;

                    // Grab material details
                    string[] blocks = materialFileRow.Split(",");
                    UInt32 materialIndex = UInt32.Parse(blocks[0]);
                    Material curMaterial = materialListData.MaterialsByTextureVariation[0][Convert.ToInt32(materialIndex)];

                    // Get texture dimensions, if there is one
                    if (curMaterial.TextureNames.Count > 0)
                    {
                        string textureFullPath = Path.Combine(textureFolder, curMaterial.TextureNames[0] + ".png");
                        Bitmap textureinputImage = new Bitmap(textureFullPath);
                        int imageHeight = textureinputImage.Height;
                        int imageWidth = textureinputImage.Width;
                        textureinputImage.Dispose();

                        // Throw an error if this texture isn't a power of 2
                        HashSet<int> validDimensions = new HashSet<int>() { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024 };
                        if (validDimensions.Contains(imageHeight) == false)
                            Logger.WriteError("Texture '" + textureFullPath + "' height is invalid with value '" + imageHeight + "', it must be a power of 2 and <= 1024");
                        if (validDimensions.Contains(imageWidth) == false)
                            Logger.WriteError("Texture '" + textureFullPath + "' width is invalid with value '" + imageWidth + "', it must be a power of 2 and <= 1024");

                        // Resize if it's too small
                        if (imageHeight < 8 || imageWidth < 8)
                        {
                            Logger.WriteDebug("Expanding texture at '" + textureFullPath + "' which has a height of '" + imageHeight + "' and width of '" + imageWidth + "'");
                            if (imageHeight < 8)
                                imageHeight = 8;
                            if (imageWidth < 8)
                                imageWidth = 8;
                            ImageTool.GenerateResizedImage(textureFullPath, textureFullPath, imageWidth, imageHeight);
                        }

                        // Write this to the end of the row
                        materialFileRowsForWrite[rowIndex] = materialFileRow + "," + imageWidth.ToString() + "," + imageHeight.ToString();
                    }
                }

                // Update the material file
                StringBuilder newMaterialListFileContents = new StringBuilder();
                foreach (string materialFileRow in materialFileRowsForWrite)
                    newMaterialListFileContents.AppendLine(materialFileRow);
                File.WriteAllText(materialListFile, newMaterialListFileContents.ToString());
            }
        }

        public void ReplaceCustomTextures(string charactersDirectory, string objectsDirectory)
        {
            Logger.WriteInfo("Performing texture replacements with any custom textures...");

            // Characters
            string characterTexturesFolder = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "CustomTextures", "character");
            if (Directory.Exists(characterTexturesFolder) == false)
            {
                Logger.WriteError("Failed to perform character texture replacements, as '" + characterTexturesFolder + "' does not exist. (Be sure to set your Configuration.PATH_TOOLS_FOLDER properly)");
            }
            else
            {
                Logger.WriteInfo("Performing custom character texture replacements...");
                string[] customTextures = Directory.GetFiles(characterTexturesFolder);
                foreach (string customTexture in customTextures)
                {
                    string targetFileName = Path.Combine(charactersDirectory, "Textures", Path.GetFileName(customTexture));
                    Logger.WriteDebug("Replacing or placing character object texture '" + customTexture + "'");
                    File.Copy(customTexture, targetFileName, true);
                }
                Logger.WriteInfo("Character custom texture replacements complete");
            }

            // Objects
            string objectTexturesFolder = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "CustomTextures", "object");
            if (Directory.Exists(objectTexturesFolder) == false)
            {
                Logger.WriteError("Failed to perform object texture replacements, as '" + objectTexturesFolder + "' does not exist. (Be sure to set your Configuration.PATH_TOOLS_FOLDER properly)");
            }
            else
            {
                Logger.WriteInfo("Performing custom object texture replacements...");
                string[] customTextures = Directory.GetFiles(objectTexturesFolder);
                foreach (string customTexture in customTextures)
                {
                    string targetFileName = Path.Combine(objectsDirectory, "textures", Path.GetFileName(customTexture));
                    Logger.WriteDebug(string.Concat("Replacing or placing custom object texture '", customTexture, "'"));
                    FileTool.CopyFile(customTexture, targetFileName);
                }
                Logger.WriteInfo("Object custom texture replacements complete");
            }

            Logger.WriteInfo("Texture replacements complete");
        }

        public void ConditionMusicFiles(string musicDirectory)
        {
            LogCounter progressCounter = new LogCounter("Converting music files...");
            if (Path.Exists(musicDirectory) == false)
            {
                Logger.WriteError("Failed to process music files.  The music directory at '" + musicDirectory + "' does not exist.");
                return;
            }

            // Delete previously created files, if any
            string[] priorCreatedMusicFiles = Directory.GetFiles(musicDirectory, "*.mp3");
            foreach (string priorCreatedMusicFile in priorCreatedMusicFiles)
                File.Delete(priorCreatedMusicFile);
            priorCreatedMusicFiles = Directory.GetFiles(musicDirectory, "*.mid");
            foreach (string priorCreatedMusicFile in priorCreatedMusicFiles)
                File.Delete(priorCreatedMusicFile);
            priorCreatedMusicFiles = Directory.GetFiles(musicDirectory, "*.wav");
            foreach (string priorCreatedMusicFile in priorCreatedMusicFiles)
                File.Delete(priorCreatedMusicFile);

            string[] musicXMIFiles = Directory.GetFiles(musicDirectory, "*.xmi");
            Logger.WriteDebug("There are '" + musicXMIFiles.Count() + "' music .xmi files to process into .mid");

            // Establish paths to tool files
            string ssplayerFileFullPath = Path.Combine(Configuration.PATH_TOOLS_FOLDER, "ssplayer", "ssplayer.exe");
            if (File.Exists(ssplayerFileFullPath) == false)
            {
                Logger.WriteError("Failed to process music files. '" + ssplayerFileFullPath + "' does not exist. (Be sure to set your Configuration.PATH_TOOLS_FOLDER properly)");
                return;
            }
            string fluidsynthFileFullPath = Path.Combine(Configuration.PATH_TOOLS_FOLDER, "fluidsynth", "fluidsynth.exe");
            if (File.Exists(fluidsynthFileFullPath) == false)
            {
                Logger.WriteError("Failed to process music files. '" + fluidsynthFileFullPath + "' does not exist. (Be sure to set your Configuration.PATH_TOOLS_FOLDER properly)");
                return;
            }
            string soundfontFileFullPath = Path.Combine(Configuration.PATH_TOOLS_FOLDER, "soundfont", Configuration.AUDIO_SOUNDFONT_FILE_NAME);
            if (File.Exists(soundfontFileFullPath) == false)
            {
                Logger.WriteError("Failed to process music files. '" + soundfontFileFullPath + "' does not exist. (Be sure to set your Configuration.PATH_TOOLS_FOLDER properly)");
                return;
            }
            string ffmpegFileFullPath = Path.Combine(Configuration.PATH_TOOLS_FOLDER, "ffmpeg", "ffmpeg.exe");
            if (File.Exists(ffmpegFileFullPath) == false)
            {
                Logger.WriteError("Failed to process music files. '" + ffmpegFileFullPath + "' does not exist. (Be sure to set your Configuration.PATH_TOOLS_FOLDER properly)");
                return;
            }

            // Process all of the xmi files, which contain the midi information
            foreach (string musicXMIFile in musicXMIFiles)
            {
                // Extract XMI into 1 or more .mid
                Logger.WriteDebug("Extracting XMI file at '" + musicXMIFile + "'");
                string args = "-extractall \"" + musicXMIFile + "\"";
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.Arguments = args;
                process.StartInfo.FileName = ssplayerFileFullPath;
                process.Start();
                process.WaitForExit();
                Logger.WriteDebug(process.StandardOutput.ReadToEnd());
                Console.Title = "EverQuest to WoW Converter";

                // Go through each .mid to complete the conversion
                string[] musicMidiFiles = Directory.GetFiles(musicDirectory, "*.mid");
                Logger.WriteDebug("There are '" + musicMidiFiles.Count() + "' music .mid files created from '" + musicXMIFile + "'");
                foreach (string musicMidiFile in musicMidiFiles)
                {
                    // Create the .wav
                    string tempWavFileName = Path.Combine(musicDirectory, Path.GetFileNameWithoutExtension(musicMidiFile) + ".wav");
                    Logger.WriteDebug("Converting .mid file at '" + musicMidiFile + "' to .wav");
                    args = "-F \"" + tempWavFileName + "\" -ni \"" + soundfontFileFullPath + "\" \"" + musicMidiFile + "\" -g " + Configuration.AUDIO_MUSIC_CONVERSION_GAIN_AMOUNT.ToString();
                    process = new System.Diagnostics.Process();
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.Arguments = args;
                    process.StartInfo.FileName = fluidsynthFileFullPath;
                    process.Start();
                    process.WaitForExit();
                    Logger.WriteDebug(process.StandardOutput.ReadToEnd());

                    // Create the .mp3
                    string mp3FileName = Path.Combine(musicDirectory, Path.GetFileNameWithoutExtension(musicMidiFile) + ".mp3");
                    Logger.WriteDebug("Converting .wav file at '" + tempWavFileName + "' to .mp3");
                    args = "-i \"" + tempWavFileName + "\" -acodec mp3 \"" + mp3FileName + "\" -hide_banner -loglevel error";
                    process = new System.Diagnostics.Process();
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.Arguments = args;
                    process.StartInfo.FileName = ffmpegFileFullPath;
                    process.Start();
                    process.WaitForExit();
                    Logger.WriteDebug(process.StandardOutput.ReadToEnd());

                    // Delete the temp files
                    File.Delete(musicMidiFile);
                    File.Delete(tempWavFileName);

                    progressCounter.Write(1);
                }
            }
        }

        public void CreateIndividualIconFiles()
        {
            Logger.WriteInfo("Creating icon image files...");

            string miscImagesFolder = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "miscimages");
            if (Directory.Exists(miscImagesFolder) == false)
            {
                Logger.WriteError("Icon Image creation failed.  Could not find the miscimages folder at '', ensure you have exported the EQ content");
                return;
            }

            // Item Icons
            Logger.WriteDebug("Creating item icons.");
            string itemIconsFolder = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "itemicons");
            try
            {
                if (Directory.Exists(itemIconsFolder) == true)
                    Directory.Delete(itemIconsFolder, true);
                Directory.CreateDirectory(itemIconsFolder);
                string curIconImageSourceFile = Path.Combine(miscImagesFolder, "dragitem01.png");
                ImageTool.GenerateItemIconImagesFromFile(curIconImageSourceFile, 192, 0, itemIconsFolder, 40, 40, ImageTool.IconSeriesDirection.AlongY, 12, "INV_EQ_", 0);
                curIconImageSourceFile = Path.Combine(miscImagesFolder, "dragitem02.png");
                ImageTool.GenerateItemIconImagesFromFile(curIconImageSourceFile, 192, 192, itemIconsFolder, 40, 40, ImageTool.IconSeriesDirection.AlongY, 12, "INV_EQ_", 0);
                curIconImageSourceFile = Path.Combine(miscImagesFolder, "dragitem03.png");
                ImageTool.GenerateItemIconImagesFromFile(curIconImageSourceFile, 192, 384, itemIconsFolder, 40, 40, ImageTool.IconSeriesDirection.AlongY, 12, "INV_EQ_", 0);
                curIconImageSourceFile = Path.Combine(miscImagesFolder, "dragitem04.png");
                ImageTool.GenerateItemIconImagesFromFile(curIconImageSourceFile, 175, 576, itemIconsFolder, 40, 40, ImageTool.IconSeriesDirection.AlongY, 12, "INV_EQ_", 0);
            }
            catch (Exception)
            {
                Logger.WriteError("Error occurred while creating icon files.  Item icons threw an exception");
                return;
            }

            // Spell Icons
            Logger.WriteDebug("Creating spell icons.");
            string spellIconsFolder = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "spellicons");
            try
            {
                if (Directory.Exists(spellIconsFolder) == true)
                    Directory.Delete(spellIconsFolder, true);
                Directory.CreateDirectory(spellIconsFolder);
                string curIconImageSourceFile = Path.Combine(miscImagesFolder, "spelicon.png");
                ImageTool.GenerateItemIconImagesFromFile(curIconImageSourceFile, 19, 0, spellIconsFolder, 40, 40, ImageTool.IconSeriesDirection.AlongX, 5, "Spell_EQ_", 0);
                ImageTool.GenerateItemIconImagesFromFile(curIconImageSourceFile, 4, 19, spellIconsFolder, 40, 40, ImageTool.IconSeriesDirection.AlongX, 5, "Spell_EQ_", 20);
            }
            catch (Exception)
            {
                Logger.WriteError("Error occurred while creating icon files.  Spell icons threw an exception");
                return;
            }

            Logger.WriteInfo("Creating icon files complete.");
        }

        public bool ConvertPNGFilesToBLP()
        {
            // Make sure the tool is there
            string blpConverterFullPath = Path.Combine(Configuration.PATH_TOOLS_FOLDER, "blpconverter", "BLPConverter.exe");
            if (File.Exists(blpConverterFullPath) == false)
            {
                Logger.WriteError("Failed to convert images files. '" + blpConverterFullPath + "' does not exist. (Be sure to set your Configuration.PATH_TOOLS_FOLDER properly)");
                return false;
            }

            // Build paths and store in a process array
            List<string> textureFoldersToProcess = new List<string>();
            string characterTexturesFolder = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "characters", "Textures");
            if (Directory.Exists(characterTexturesFolder) == false)
            {
                Logger.WriteError("Failed to convert png files to blp, as the character textures folder did not exist at '" + characterTexturesFolder + "'");
                return false;
            }
            textureFoldersToProcess.Add(characterTexturesFolder);
            string equipmentTexturesFolder = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "equipment", "Textures");
            if (Directory.Exists(characterTexturesFolder) == false)
            {
                Logger.WriteError("Failed to convert png files to blp, as the equipment textures folder did not exist at '" + equipmentTexturesFolder + "'");
                return false;
            }
            textureFoldersToProcess.Add(equipmentTexturesFolder);
            string liquidSurfacesFolder = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "liquidsurfaces");
            if (Directory.Exists(liquidSurfacesFolder) == false)
            {
                Logger.WriteError("Failed to convert png files to blp, as the liquid surfaces folder did not exist at '" + liquidSurfacesFolder + "'");
                return false;
            }
            textureFoldersToProcess.Add(liquidSurfacesFolder);
            string miscImagesFolder = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "miscimages");
            if (Directory.Exists(miscImagesFolder) == false)
            {
                Logger.WriteError("Failed to convert png files to blp, as the misc images folder did not exist at '" + miscImagesFolder + "'");
                return false;
            }
            textureFoldersToProcess.Add(miscImagesFolder);
            string objectTexturesFolder = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "objects", "textures");
            if (Directory.Exists(objectTexturesFolder) == false)
            {
                Logger.WriteError("Failed to convert png files to blp, as the object textures folder did not exist at '" + objectTexturesFolder + "'");
                return false;
            }
            textureFoldersToProcess.Add(objectTexturesFolder);
            string itemIconFolder = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "itemicons");
            if (Directory.Exists(itemIconFolder) == false)
            {
                Logger.WriteError("Failed to convert png files to blp, as the itemicons folder did not exist at '" + itemIconFolder + "'");
                return false;
            }
            textureFoldersToProcess.Add(itemIconFolder);
            string spellIconFolder = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "spellicons");
            if (Directory.Exists(spellIconFolder) == false)
            {
                Logger.WriteError("Failed to convert png files to blp, as the itemicons folder did not exist at '" + spellIconFolder + "'");
                return false;
            }
            textureFoldersToProcess.Add(spellIconFolder);
            string zonesRootFolder = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "zones");
            if (Directory.Exists(zonesRootFolder) == false)
            {
                Logger.WriteError("Failed to convert png files to blp, as the zones root folder did not exist at '" + zonesRootFolder + "'");
                return false;
            }
            string[] zoneDirectories = Directory.GetDirectories(zonesRootFolder);
            foreach (string zoneDirectory in zoneDirectories)
            {
                string curZoneTextureFolder = Path.Combine(zoneDirectory, "Textures");
                if (Directory.Exists(curZoneTextureFolder) == false)
                {
                    Logger.WriteError("Failed to convert png files to blp, as the zone '" + curZoneTextureFolder + "' had no Textures folder");
                    return false;
                }
                textureFoldersToProcess.Add(curZoneTextureFolder);
            }

            // Get all the individual files to process
            Logger.WriteInfo("Building list of png files to convert...");
            foreach(string folderToProcess in textureFoldersToProcess)
            {
                string[] curFolderPNGFiles = Directory.GetFiles(folderToProcess, "*.png");
                foreach(string curPngFile in curFolderPNGFiles)
                {
                    Logger.WriteDebug("Adding file '" + curPngFile + "' for conversion");
                    PNGtoBLPFilesToConvert.Add(curPngFile);
                }
            }

            LogCounter progressCounter = new LogCounter("Converting png files to blp files...", 0, PNGtoBLPFilesToConvert.Count);
            progressCounter.Write(0);
            if (Configuration.CORE_ENABLE_MULTITHREADING == true)
            {
                int taskCount = Configuration.CORE_PNGTOBLPCONVERSION_THREAD_COUNT;
                Task[] tasks = new Task[taskCount];
                for (int i = 0; i < taskCount; i++)
                {
                    int iCopy = i + 1;
                    tasks[i] = Task.Factory.StartNew(() =>
                    {
                        PNGToBLPConversionThreadWorker(iCopy, blpConverterFullPath, progressCounter);
                    });
                }
                Task.WaitAll(tasks);
            }
            else
            {
                PNGToBLPConversionThreadWorker(1, blpConverterFullPath, progressCounter);
            }

            return true;
        }

        private void PNGToBLPConversionThreadWorker(int threadID, string blpConverterFullPath, LogCounter progressCounter)
        {
            Logger.WriteInfo(string.Concat("<+> Thread [PNG to BLP Subworker ", threadID.ToString(), "] Started"));

            bool moreToProcess = true;
            while (moreToProcess)
            {
                // Create the batch
                List<string> fileNameBatch = new List<string>();
                lock (PNGtoBLPLock)
                {
                    if (PNGtoBLPFilesToConvert.Count == 0)
                    {
                        moreToProcess = false;
                        continue;
                    }
                    else
                    {
                        int batchSize = Math.Min(Configuration.GENERATE_BLPCONVERTBATCHSIZE, PNGtoBLPFilesToConvert.Count);
                        fileNameBatch = PNGtoBLPFilesToConvert.Take(batchSize).ToList();
                        PNGtoBLPFilesToConvert.RemoveRange(0, batchSize);
                    }
                }

                // Convert to a parameter
                StringBuilder curFileArgListSB = new StringBuilder();
                for (int i = 0; i < fileNameBatch.Count; i++)
                {
                    string curFile = fileNameBatch[i];
                    curFileArgListSB.Append(" \"");
                    curFileArgListSB.Append(curFile);
                    curFileArgListSB.Append("\"");
                }

                // Convert them
                Logger.WriteDebug("Converting png files '" + curFileArgListSB.ToString() + "'");
                string args = "/M /FBLP_DXT5 " + curFileArgListSB.ToString();
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.Arguments = args;
                process.StartInfo.FileName = blpConverterFullPath;
                process.Start();
                //process.WaitForExit();
                Logger.WriteDebug(process.StandardOutput.ReadToEnd());
                Console.Title = "EverQuest to WoW Converter";
                curFileArgListSB.Clear();
                progressCounter.Write(fileNameBatch.Count);
            }

            Logger.WriteInfo(string.Concat("<-> Thread [PNG to BLP Subworker ", threadID.ToString(), "] Ended"));
        }

        private void RenameMeshInSkeletonFile(string skeletonFileFullName, string oldMeshName, string newMeshName)
        {
            // Ensure file exists
            if (File.Exists(skeletonFileFullName) == false)
            {
                Logger.WriteError("Failed to rename mesh in skeleton file, as file '" + skeletonFileFullName + "' did not exist");
                return;
            }

            // Read all of the rows
            List<string> fileLines = FileTool.ReadAllStringLinesFromFile(skeletonFileFullName, false, true);

            // Output the rows, replacing the string
            string replaceStringOld = "," + oldMeshName + ",";
            string replaceStringNew = "," + newMeshName + ",";
            using (var outputFile = new StreamWriter(skeletonFileFullName))
                foreach (string line in fileLines)
                    outputFile.WriteLine(line.Replace(replaceStringOld, replaceStringNew));
        }

        private int GetUniqueIDForAnyIncompatibleFileCollision(string sourceFileFullPath, string targetFolder, int startUniqueIDToAdd)
        {
            // Pull out just the file name
            string sourceFileNameNoExtension = Path.GetFileNameWithoutExtension(sourceFileFullPath);

            // Loop until there is no unresolved file collision
            bool doesUnresolvedFileCollisionExist = false;
            int calculatedUniqueIDToAdd = startUniqueIDToAdd;
            do
            {
                // Generate the name for comparison
                string testTargetFileName;
                if (calculatedUniqueIDToAdd == 0)
                    testTargetFileName = Path.Combine(targetFolder, sourceFileNameNoExtension + Path.GetExtension(sourceFileFullPath));
                else
                    testTargetFileName = Path.Combine(targetFolder, sourceFileNameNoExtension + "alt" + calculatedUniqueIDToAdd.ToString() + Path.GetExtension(sourceFileFullPath));

                // Compare the files if the destination file already exist
                doesUnresolvedFileCollisionExist = false;
                if (File.Exists(testTargetFileName) == true)
                {
                    // If the files collide but are not the exact same, create a new version
                    if (FileTool.AreFilesTheSame(sourceFileFullPath, testTargetFileName) == false)
                    {
                        doesUnresolvedFileCollisionExist = true;
                        calculatedUniqueIDToAdd++;
                    }
                }
            }
            while (doesUnresolvedFileCollisionExist);
            return calculatedUniqueIDToAdd;
        }

        private void UpdateMaterialListNameInMesh(string sourceMeshFileFullPath, string oldMaterialListName, string newMaterialListName)
        {
            // Read each line of the mesh file looking for material name
            List<string> inputMeshFileRows = FileTool.ReadAllStringLinesFromFile(sourceMeshFileFullPath, false, false);
            List<string> outputMeshFileRows = new List<string>();
            for (int mi = 0; mi < inputMeshFileRows.Count; mi++)
            {
                string inputMeshLine = inputMeshFileRows[mi];
                if (inputMeshLine.Contains("ml,"))
                    inputMeshLine = inputMeshLine.Replace(oldMaterialListName, newMaterialListName);
                outputMeshFileRows.Add(inputMeshLine);
            }
            // Write the file
            using (var outputMaterialList = new StreamWriter(sourceMeshFileFullPath))
                foreach (string block in outputMeshFileRows)
                    outputMaterialList.WriteLine(block);
        }

        private void ProcessAndCopyMaterialTextures(string topDirectory, string tempObjectsFolder, string materialListFullPath, string outputObjectsFolder)
        {
            string sourceTextureFolder = Path.Combine(tempObjectsFolder, "textures");
            string targetTextureFolder = Path.Combine(outputObjectsFolder, "textures");

            // Read each line of the material file looking for texture names
            List<string> inMaterialListFileRows = FileTool.ReadAllStringLinesFromFile(materialListFullPath, false, false);
            List<string> outMaterialListFileRows = new List<string>();
            for (int mi = 0; mi < inMaterialListFileRows.Count; mi++)
            {
                string materialLine = inMaterialListFileRows[mi];

                // Comment rows start with a pound
                if (materialLine.StartsWith("#"))
                {
                    outMaterialListFileRows.Add(materialLine);
                    continue;
                }

                // Remove any spaces or pound signs
                materialLine = materialLine.Replace("#", "");
                materialLine = materialLine.Replace(" ", "");

                // Look for the section that has texture names
                string[] workingRowBlocks = materialLine.Split(",");
                for (int i = 0; i < workingRowBlocks.Length; i++)
                {
                    string block = workingRowBlocks[i];

                    // Texture rows have the colon
                    if (block.Contains(':'))
                    {
                        // Break it up, skipping the first since that's just the material name
                        string[] workingTextureBlocks = block.Split(":");
                        for (int j = 1; j < workingTextureBlocks.Length; j++)
                        {
                            string textureNameNoExt = workingTextureBlocks[j];

                            // Rename anything that needs to be renamed
                            string sourceTextureFileNameAndPath = Path.Combine(sourceTextureFolder, textureNameNoExt + ".png");
                            int generatedID = GetUniqueIDForAnyIncompatibleFileCollision(sourceTextureFileNameAndPath, targetTextureFolder, 0);
                            if (generatedID > 0)
                            {
                                textureNameNoExt = textureNameNoExt + "alt" + generatedID.ToString();
                                Logger.WriteDebug("- [" + topDirectory + "] Texture collision occurred so renaming '" + workingTextureBlocks[j] + "' to '" + textureNameNoExt + "' for MaterialList '" + materialListFullPath + "'");
                                workingTextureBlocks[j] = textureNameNoExt;
                            }

                            // Copy the texture
                            string targetTextureFileNameAndPath = Path.Combine(targetTextureFolder, textureNameNoExt + ".png");
                            FileTool.CopyFile(sourceTextureFileNameAndPath, targetTextureFileNameAndPath);
                        }

                        // Put it back together
                        StringBuilder composedTextureBlockSB = new StringBuilder();
                        for (int j = 0; j < workingTextureBlocks.Length; j++)
                        {
                            composedTextureBlockSB.Append(workingTextureBlocks[j]);
                            if (j < workingTextureBlocks.Length - 1)
                                composedTextureBlockSB.Append(":");
                        }
                        block = composedTextureBlockSB.ToString();
                    }

                    workingRowBlocks[i] = block;
                }

                // Re-combine and add this row
                StringBuilder composedLineSB = new StringBuilder();
                for (int i = 0; i < workingRowBlocks.Length; i++)
                {
                    composedLineSB.Append(workingRowBlocks[i]);
                    if (i < workingRowBlocks.Length - 1)
                        composedLineSB.Append(",");
                }
                outMaterialListFileRows.Add(composedLineSB.ToString());
            }

            // Write the file
            using (var outputMaterialList = new StreamWriter(materialListFullPath))
                foreach (string block in outMaterialListFileRows)
                    outputMaterialList.WriteLine(block);
        }

        private void ProcessAndCopyObject(string topDirectory, string tempObjectsFolder, string tempZoneFolder, string objectName, string outputObjectsFolder, bool isSkeletal, out string revisedObjectName)
        {
            string tempObjectMaterialList = Path.Combine(tempObjectsFolder, "MaterialLists", objectName + ".txt");

            // Handle texture name collisions
            ProcessAndCopyMaterialTextures(topDirectory, tempObjectsFolder, tempObjectMaterialList, outputObjectsFolder);

            // Different logic for skeletal vs non-skeletal
            if (isSkeletal)
            {
                // Collect source folder names                
                string tempObjectAnimationFile = Path.Combine(tempObjectsFolder, "Animations", objectName + "_pos.txt");
                string tempObjectSkeletonFile = Path.Combine(tempObjectsFolder, "Skeletons", objectName + ".txt");
                List<string> tempMeshNames = new List<string>();
                foreach (string line in FileTool.ReadAllStringLinesFromFile(tempObjectSkeletonFile, false, true))
                {
                    if (line.Contains("#") || line.StartsWith("root"))
                        continue;
                    string meshName = line.Split(",")[2];
                    if (meshName.Length > 0)
                        tempMeshNames.Add(meshName);
                }

                // Determine if any renames are required
                Dictionary<string, int> generatedIDByOriginalMeshName = new Dictionary<string, int>();
                foreach(string meshName in tempMeshNames)
                {
                    string tempObjectMesh = Path.Combine(tempObjectsFolder, "Meshes", meshName + ".txt");
                    int generatedMeshID = GetUniqueIDForAnyIncompatibleFileCollision(tempObjectMesh, Path.Combine(outputObjectsFolder, "Meshes"), 0);
                    if (generatedMeshID != 0)
                        generatedIDByOriginalMeshName.Add(meshName, generatedMeshID);
                }
                int generatedID = 0;
                generatedID = GetUniqueIDForAnyIncompatibleFileCollision(tempObjectMaterialList, Path.Combine(outputObjectsFolder, "MaterialLists"), generatedID);
                generatedID = GetUniqueIDForAnyIncompatibleFileCollision(tempObjectAnimationFile, Path.Combine(outputObjectsFolder, "Animations"), generatedID);
                generatedID = GetUniqueIDForAnyIncompatibleFileCollision(tempObjectSkeletonFile, Path.Combine(outputObjectsFolder, "Skeletons"), generatedID);

                // Update mesh names in the skeleton file, if needed
                foreach (var idByOriginalMeshName in generatedIDByOriginalMeshName)
                {
                    string newMeshName = idByOriginalMeshName.Key + "alt" + generatedID.ToString();
                    RenameMeshInSkeletonFile(tempObjectSkeletonFile, idByOriginalMeshName.Key, newMeshName);
                }

                // Generate a new name
                revisedObjectName = objectName;
                if (generatedID > 0)
                    revisedObjectName = objectName + "alt" + generatedID.ToString();

                // Copy the files, factoring for renames
                foreach (string meshName in tempMeshNames)
                {
                    string targetMeshName = meshName;
                    if (generatedIDByOriginalMeshName.ContainsKey(meshName))
                        targetMeshName = meshName + "alt" + generatedID.ToString();
                    string tempObjectMesh = Path.Combine(tempObjectsFolder, "Meshes", meshName + ".txt");
                    string tempObjectMeshCollision = Path.Combine(tempObjectsFolder, "Meshes", meshName + "_collision.txt");
                    string outputMeshFileName = Path.Combine(outputObjectsFolder, "Meshes", string.Concat(targetMeshName, ".txt"));
                    FileTool.CopyFile(tempObjectMesh, outputMeshFileName);
                    if (File.Exists(tempObjectMeshCollision))
                        FileTool.CopyFile(tempObjectMeshCollision, Path.Combine(outputObjectsFolder, "Meshes", string.Concat(targetMeshName, "_collision.txt")));

                    // If renamed, also update the material list reference
                    if (generatedID > 0)
                        UpdateMaterialListNameInMesh(outputMeshFileName, objectName, revisedObjectName);
                }
                FileTool.CopyFile(tempObjectMaterialList, Path.Combine(outputObjectsFolder, "MaterialLists", revisedObjectName + ".txt"));                
                FileTool.CopyFile(tempObjectAnimationFile, Path.Combine(outputObjectsFolder, "Animations", revisedObjectName + "_pos.txt"));
                FileTool.CopyFile(tempObjectSkeletonFile, Path.Combine(outputObjectsFolder, "Skeletons", revisedObjectName + ".txt"));
            }
            else
            {
                // Collect source folder names
                string tempObjectMesh = Path.Combine(tempObjectsFolder, "Meshes", objectName + ".txt");
                string tempObjectMeshCollision = Path.Combine(tempObjectsFolder, "Meshes", objectName + "_collision.txt");

                // Determine if any renames are required
                int generatedID = GetUniqueIDForAnyIncompatibleFileCollision(tempObjectMesh, Path.Combine(outputObjectsFolder, "Meshes"), 0);
                generatedID = GetUniqueIDForAnyIncompatibleFileCollision(tempObjectMaterialList, Path.Combine(outputObjectsFolder, "MaterialLists"), generatedID);

                // Copy the files, factoring for rename
                revisedObjectName = objectName;
                if (generatedID > 0)
                    revisedObjectName = objectName + "alt" + generatedID.ToString();
                string outputMeshFileName = Path.Combine(outputObjectsFolder, "Meshes", string.Concat(revisedObjectName, ".txt"));
                FileTool.CopyFile(tempObjectMesh, outputMeshFileName);
                FileTool.CopyFile(tempObjectMaterialList, Path.Combine(outputObjectsFolder, "MaterialLists", string.Concat(revisedObjectName, ".txt")));
                if (File.Exists(tempObjectMeshCollision))
                    FileTool.CopyFile(tempObjectMeshCollision, Path.Combine(outputObjectsFolder, "Meshes", string.Concat(revisedObjectName, "_collision.txt")));

                // If renamed, also update the material list reference
                if (generatedID > 0)
                    UpdateMaterialListNameInMesh(outputMeshFileName, objectName, revisedObjectName);
            }
            
            // If the object itself was renamed, update references
            if (objectName != revisedObjectName)
            {
                // Update the instances file
                string tempObjectInstancesFile = Path.Combine(tempZoneFolder, "object_instances.txt");
                if (File.Exists(tempObjectInstancesFile) == false)
                    Logger.WriteDebug("- [" + topDirectory + "] No object_instances file to update");
                else
                {
                    EQObjectInstances eqObjectInstances = new EQObjectInstances();
                    if (eqObjectInstances.LoadFromDisk(tempObjectInstancesFile) == false)
                        Logger.WriteError("- [" + topDirectory + "] Issue loading object instances file '" + tempObjectInstancesFile + "'");
                    else
                    {
                        bool meshFoundInFile = false;
                        foreach (ObjectInstance objectInstance in eqObjectInstances.ObjectInstances)
                        {
                            if (objectInstance.ModelName == objectName)
                            {
                                meshFoundInFile = true;
                                break;
                            }
                        }
                        if (meshFoundInFile == true)
                        {
                            string fileText = FileTool.ReadAllDataFromFile(tempObjectInstancesFile);
                            if (fileText.Contains(objectName + ","))
                            {
                                string newObjectMeshFileNameNoExtension = Path.GetFileNameWithoutExtension(revisedObjectName);
                                Logger.WriteDebug("- [" + topDirectory + "] Zone object_instances file '" + tempObjectInstancesFile + "' contained mesh '" + objectName + "' which was renamed to '" + revisedObjectName + "'. Updating object_instances file...");
                                fileText = fileText.Replace(objectName + ",", newObjectMeshFileNameNoExtension + ",");
                                File.WriteAllText(tempObjectInstancesFile, fileText);
                            }
                        }
                    }
                }
            }            
        }
    }
}
