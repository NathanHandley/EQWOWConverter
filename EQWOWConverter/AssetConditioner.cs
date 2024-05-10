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
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EQWOWConverter.Common;
using EQWOWConverter.WOWFiles;
using EQWOWConverter.Zones;
using static System.Net.Mime.MediaTypeNames;

namespace EQWOWConverter
{
    internal class AssetConditioner
    {
        public AssetConditioner() { }

        private static uint objectMeshesCondensed = 0;
        private static uint objectMaterialsCondensed = 0;
        private static uint objectTexturesCondensed = 0;

        public bool ConditionEQOutput(string eqExportsRawPath, string eqExportsCondensedPath)
        {
            Logger.WriteLine("Conditioning Raw EQ Data...");

            // Reset counters
            objectMeshesCondensed = 0;
            objectMaterialsCondensed = 0;
            objectTexturesCondensed = 0;

            // Make sure the raw path exists
            if (Directory.Exists(eqExportsRawPath) == false)
            {
                Logger.WriteLine("ERROR - Raw input path of '" + eqExportsRawPath + "' does not exist.");
                Logger.WriteLine("Conditioning Failed!");
                return false;
            }

            // Create base folder
            FileTool.CreateBlankDirectory(eqExportsCondensedPath, false);

            // Delete/Recreate the output folders
            string outputObjectsFolderRoot = Path.Combine(eqExportsCondensedPath, "objects");
            string outputObjectsTexturesFolderRoot = Path.Combine(outputObjectsFolderRoot, "textures");
            string outputObjectsMeshesFolderRoot = Path.Combine(outputObjectsFolderRoot, "meshes");
            string outputObjectsMaterialsFolderRoot = Path.Combine(outputObjectsFolderRoot, "materiallists");
            string outputCharactersFolderRoot = Path.Combine(eqExportsCondensedPath, "characters");
            string outputSoundsFolderRoot = Path.Combine(eqExportsCondensedPath, "sounds");
            string outputMusicFolderRoot = Path.Combine(eqExportsCondensedPath, "music");
            string outputEquipmentFolderRoot = Path.Combine(eqExportsCondensedPath, "equipment");
            string outputMiscImagesFolderRoot = Path.Combine(eqExportsCondensedPath, "miscimages");
            string outputZoneFolderRoot = Path.Combine(eqExportsCondensedPath, "zones");
            string tempFolderRoot = Path.Combine(eqExportsCondensedPath, "temp");
            FileTool.CreateBlankDirectory(outputObjectsFolderRoot, false);
            FileTool.CreateBlankDirectory(outputObjectsTexturesFolderRoot, false);
            FileTool.CreateBlankDirectory(outputObjectsMeshesFolderRoot, false);
            FileTool.CreateBlankDirectory(outputObjectsMaterialsFolderRoot, false);
            FileTool.CreateBlankDirectory(outputCharactersFolderRoot, false);
            FileTool.CreateBlankDirectory(outputSoundsFolderRoot, false);
            FileTool.CreateBlankDirectory(outputMusicFolderRoot, false);
            FileTool.CreateBlankDirectory(outputEquipmentFolderRoot, false);
            FileTool.CreateBlankDirectory(outputMiscImagesFolderRoot, false);
            FileTool.CreateBlankDirectory(outputZoneFolderRoot, false);
            FileTool.CreateBlankDirectory(tempFolderRoot, false);

            // Iterate through each exported directory and process objects and zones
            string[] topDirectories = Directory.GetDirectories(eqExportsRawPath);
            foreach (string topDirectory in topDirectories)
            {
                // Get just the folder name itself for later
                string topDirectoryFolderNameOnly = topDirectory.Split('\\').Last();

                // Bring in the objects of this directory
                FileTool.CopyDirectoryAndContents(topDirectory, tempFolderRoot, true, true);

                // If it's the character, music, equipment, or sound folder then copy it as-is
                if (topDirectoryFolderNameOnly == "characters" || topDirectoryFolderNameOnly == "sounds" || topDirectoryFolderNameOnly == "music" || topDirectoryFolderNameOnly == "equipment")
                {
                    Logger.WriteLine("- [" + topDirectoryFolderNameOnly + "] Copying special folder containing these objects");
                    string outputFolder = Path.Combine(eqExportsCondensedPath, topDirectoryFolderNameOnly);
                    FileTool.CopyDirectoryAndContents(tempFolderRoot, outputFolder, true, true);
                    continue;
                }

                // If it's a bmpwad (contains misc images) then copy that into the miscimages folder
                if (topDirectoryFolderNameOnly.StartsWith("bmpwad"))
                {
                    Logger.WriteLine("- [" + topDirectoryFolderNameOnly + "] Copying special folder containing these objects, and resizing loading screens");
                    string outputMiscImagesFolder = Path.Combine(eqExportsCondensedPath, "miscimages");
                    FileTool.CopyDirectoryAndContents(tempFolderRoot, outputMiscImagesFolder, false, true);

                    // Resize the loading screens
                    if (topDirectoryFolderNameOnly == "bmpwad")
                    {
                        string inputLoadingScreenFileName = Path.Combine(tempFolderRoot, "logo03.png");
                        string outputLoadingScreenFileName = Path.Combine(outputMiscImagesFolder, "logo03resized.png");
                        GenerateResizedImage(inputLoadingScreenFileName, outputLoadingScreenFileName, 1024, 1024);
                    }
                    else if (topDirectoryFolderNameOnly == "bmpwad4")
                    {
                        string inputLoadingScreenFileName = Path.Combine(tempFolderRoot, "eqkload.png");
                        string outputLoadingScreenFileName = Path.Combine(outputMiscImagesFolder, "eqkloadresized.png");
                        GenerateResizedImage(inputLoadingScreenFileName, outputLoadingScreenFileName, 1024, 1024);
                    }
                    else if (topDirectoryFolderNameOnly == "bmpwad5")
                    {
                        string inputLoadingScreenFileName = Path.Combine(tempFolderRoot, "eqvload.png");
                        string outputLoadingScreenFileName = Path.Combine(outputMiscImagesFolder, "eqvloadresized.png");
                        GenerateResizedImage(inputLoadingScreenFileName, outputLoadingScreenFileName, 1024, 1024);
                    }
                    continue;
                }

                // Determine what objects exist in this folder
                string tempZoneFolder = Path.Combine(tempFolderRoot, "Zone");
                string tempObjectsFolder = Path.Combine(tempFolderRoot, "Objects");
                bool topDirectoryHasZone = Directory.Exists(tempZoneFolder);
                bool topDirectoryHasObjects = Directory.Exists(tempObjectsFolder);

                // Process objects
                if (topDirectoryHasObjects)
                {
                    // Process the object textures
                    ProcessAndCopyObjectTextures(topDirectoryFolderNameOnly, tempObjectsFolder, outputObjectsTexturesFolderRoot);

                    // Process the object materials
                    ProcessAndCopyObjectMaterials(topDirectoryFolderNameOnly, tempObjectsFolder, outputObjectsMaterialsFolderRoot);

                    // Process the object meshes
                    ProcessAndCopyObjectMeshes(topDirectoryFolderNameOnly, tempObjectsFolder, tempZoneFolder, outputObjectsMeshesFolderRoot);
                }

                // Process Zones
                if (topDirectoryHasZone)
                {
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

                    // Copy files that were missing in the original folders for some reason
                    if (topDirectoryFolderNameOnly == "fearplane")
                    {
                        Logger.WriteLine("- [" + topDirectoryFolderNameOnly + "] Copying texture file 'maywall' not found in the original zone folder...");
                        string inputFileName = Path.Combine(tempObjectsFolder, "Textures", "maywall.png");
                        string outputFileName = Path.Combine(outputZoneFolder, "Textures", "maywall.png");
                        File.Copy(inputFileName, outputFileName, true);
                    }
                    else if (topDirectoryFolderNameOnly == "oasis")
                    {
                        Logger.WriteLine("- [" + topDirectoryFolderNameOnly + "] Copying texture file 'canwall1' not found in the original zone folder...");
                        string inputFileName = Path.Combine(tempObjectsFolder, "Textures", "canwall1.png");
                        string outputFileName = Path.Combine(outputZoneFolder, "Textures", "canwall1.png");
                        File.Copy(inputFileName, outputFileName, true);
                    }
                    else if (topDirectoryFolderNameOnly == "swampofnohope")
                    {
                        Logger.WriteLine("- [" + topDirectoryFolderNameOnly + "] Copying texture file 'kruphse3' not found in the original zone folder...");
                        string inputFileName = Path.Combine(tempObjectsFolder, "Textures", "kruphse3.png");
                        string outputFileName = Path.Combine(outputZoneFolder, "Textures", "kruphse3.png");
                        File.Copy(inputFileName, outputFileName, true);
                    }
                }
            }

            // Clean up the temp folder and exit
            Directory.Delete(tempFolderRoot, true);
            Logger.WriteLine("Conditioning completed for model data.  Object Meshes condensed: '" + objectMeshesCondensed + "', Object Textures condensed: '" + objectTexturesCondensed + "', Object Materials Condensed: '" + objectMaterialsCondensed + "'");
            return true;
        }

        // TODO: Delete?
        //public bool UpdateImageReferences(string eqExportsCondensedPath)
        //{
        //    Logger.WriteLine("Updating image references...");

        //    // Make sure the raw path exists
        //    if (Directory.Exists(eqExportsCondensedPath) == false)
        //    {
        //        Logger.WriteLine("ERROR - Condensed path of '" + eqExportsCondensedPath + "' does not exist.");
        //        Logger.WriteLine("Conditioning Failed!");
        //        return false;
        //    }

        //    // Delete any .png files
        //    FileTool.DeleteFilesInDirectory(eqExportsCondensedPath, "*.png", true);

        //    Logger.WriteLine("Image reference updates complete");
        //    return true;
        //}

        // TODO: Delete this?
        // TODO: Rewrite to be quick.  Approach:
        // TBD: Delete
        // 1) Generate 'vertexkey' for verticies (Vector3) by using the input row string literal 
        // 2) Make a Dictionary<string, SortedSet<int>> with key as the vertexkey, value array is triangleface index which has a vert sharing the key
        // 3) 
        // ...
        // 2) Make a Dictionary<string, SortedSet<int>> with key as the vertexkey, and set be matching vertex index
        // 3) Populate with Verticies
        // 4) Pivot table into another Dictionary<int, SortedSet<int>> with key as each vertex index, and value set as indexes that share the same vertexkey
        // 5) 
        //public bool GenerateAssociationMaps(string eqExportsCondensedPath)
        //{
        //    // Make sure the raw path exists
        //    if (Directory.Exists(eqExportsCondensedPath) == false)
        //    {
        //        Logger.WriteLine("ERROR - Condensed path of '" + eqExportsCondensedPath + "' does not exist.");
        //        Logger.WriteLine("Association Maps Generation Failured");
        //        return false;
        //    }

        //    // Make sure the zone folder path exists
        //    string zoneFolderRoot = Path.Combine(eqExportsCondensedPath, "zones");
        //    if (Directory.Exists(zoneFolderRoot) == false)
        //    {
        //        Logger.WriteLine("ERROR - Zone folder that should be at path '" + zoneFolderRoot + "' does not exist.");
        //        Logger.WriteLine("Association Maps Generation Failured");
        //        return false;
        //    }

        //    // Go through the subfolders for each zone generate the maps
        //    DirectoryInfo zoneRootDirectoryInfo = new DirectoryInfo(zoneFolderRoot);
        //    DirectoryInfo[] zoneDirectoryInfos = zoneRootDirectoryInfo.GetDirectories();
        //    foreach (DirectoryInfo zoneDirectory in zoneDirectoryInfos)
        //    {
        //        // Load the EQ zone
        //        Zone curZone = new Zone(zoneDirectory.Name);
        //        Logger.WriteLine("- [" + zoneDirectory.Name + "]: Starting association map generation for '" + zoneDirectory.Name + "...");
        //        string curZoneDirectory = Path.Combine(zoneFolderRoot, zoneDirectory.Name);
        //        curZone.LoadEQZoneData(zoneDirectory.Name, curZoneDirectory);

        //        // Generate vertex map
        //        Logger.WriteLine("- [" + zoneDirectory.Name + "]: Generating vertex association map...");

        //        // Find alike verticies and group their indicies
        //        Dictionary<int, SortedSet<int>> alikeVertexIndiciesByVertexIndex = new Dictionary<int, SortedSet<int>>();
        //        for (int i = 0; i < curZone.EQZoneData.Verticies.Count; i++)
        //        {
        //            if (alikeVertexIndiciesByVertexIndex.ContainsKey(i) == false)
        //                alikeVertexIndiciesByVertexIndex.Add(i, new SortedSet<int>());
        //            for (int j = i + 1; j < curZone.EQZoneData.Verticies.Count; j++)
        //            {
        //                if (curZone.EQZoneData.Verticies[i].IsEqualForIndexComparisons(curZone.EQZoneData.Verticies[j]))
        //                {
        //                    // On match, store in both lists
        //                    alikeVertexIndiciesByVertexIndex[i].Add(j);
        //                    if (alikeVertexIndiciesByVertexIndex.ContainsKey(j) == false)
        //                        alikeVertexIndiciesByVertexIndex.Add(j, new SortedSet<int>());
        //                    alikeVertexIndiciesByVertexIndex[j].Add(i);
        //                }
        //            }
        //        }

        //        // Put vertex map into an output string array and write it
        //        StringBuilder outputVertexMap = new StringBuilder();
        //        outputVertexMap.AppendLine("# First value is the core index, and all following are associated (the same value in the vertex array)");
        //        foreach(var alikeVertexIndicies in alikeVertexIndiciesByVertexIndex)
        //        {
        //            outputVertexMap.Append(alikeVertexIndicies.Key.ToString());
        //            foreach(int refIndex in alikeVertexIndicies.Value)
        //                outputVertexMap.Append("," + refIndex.ToString());
        //            outputVertexMap.AppendLine(string.Empty);
        //        }
        //        string vertexMapFilePath = Path.Combine(curZoneDirectory, "map_verticies.txt");
        //        File.WriteAllText(vertexMapFilePath, outputVertexMap.ToString());

        //        // Generate the face triangle map
        //        Logger.WriteLine("- [" + zoneDirectory.Name + "]: Generating face incidies association map...");

        //        // Find alike faces and group their indicies
        //        Dictionary<int, SortedSet<int>> connectedFaceIndiciesByFaceIndex = new Dictionary<int, SortedSet<int>>();
        //        for (int i = 0; i < curZone.EQZoneData.TriangleFaces.Count; i++)
        //        {
        //            if (connectedFaceIndiciesByFaceIndex.ContainsKey(i) == false)
        //                connectedFaceIndiciesByFaceIndex.Add(i, new SortedSet<int>());
        //            TriangleFace leftFace = curZone.EQZoneData.TriangleFaces[i];
        //            for (int j = i + 1; j < curZone.EQZoneData.TriangleFaces.Count; j++)
        //            {
        //                TriangleFace rightFace = curZone.EQZoneData.TriangleFaces[j];
        //                bool facesConnect = false;
        //                if (leftFace.SharesIndexWith(rightFace) == true)
        //                    facesConnect = true;
        //                else
        //                {
        //                    if (rightFace.ContainsIndex(alikeVertexIndiciesByVertexIndex[leftFace.V1]))
        //                        facesConnect = true;
        //                    else if (rightFace.ContainsIndex(alikeVertexIndiciesByVertexIndex[leftFace.V2]))
        //                        facesConnect = true;
        //                    else if (rightFace.ContainsIndex(alikeVertexIndiciesByVertexIndex[leftFace.V3]))
        //                        facesConnect = true;
        //                }

        //                if (facesConnect == true)
        //                {
        //                    // On match, store in both lists
        //                    connectedFaceIndiciesByFaceIndex[i].Add(j);
        //                    if (connectedFaceIndiciesByFaceIndex.ContainsKey(j) == false)
        //                        connectedFaceIndiciesByFaceIndex.Add(j, new SortedSet<int>());
        //                    connectedFaceIndiciesByFaceIndex[j].Add(i);
        //                }
        //            }
        //        }

        //        // Put face map into an output string array and write it
        //        StringBuilder outputFaceMap = new StringBuilder();
        //        outputFaceMap.AppendLine("# First value is the core index, and all following are associated (the same value in the face indicies array)");
        //        foreach (var alikeFaceIndicies in connectedFaceIndiciesByFaceIndex)
        //        {
        //            outputFaceMap.Append(alikeFaceIndicies.Key.ToString());
        //            foreach (int refIndex in alikeFaceIndicies.Value)
        //                outputFaceMap.Append("," + refIndex.ToString());
        //            outputFaceMap.AppendLine(string.Empty);
        //        }
        //        string faceMapFilePath = Path.Combine(curZoneDirectory, "map_indicies.txt");
        //        File.WriteAllText(faceMapFilePath, outputFaceMap.ToString());
        //    }
        //    return true;
        //}

        private void GenerateResizedImage(string inputFilePath, string outputFilePath, int newWidth, int newHeight)
        {
            Bitmap inputImage = new Bitmap(inputFilePath);
            Bitmap outputImage = new Bitmap(newWidth, newHeight);
            outputImage.SetResolution(outputImage.HorizontalResolution, outputImage.VerticalResolution);
            using (var graphics = Graphics.FromImage(outputImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                using (ImageAttributes wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    Rectangle outputRectangle = new Rectangle(0, 0, newWidth, newHeight);
                    graphics.DrawImage(inputImage, outputRectangle, 0, 0, inputImage.Width, inputImage.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            outputImage.Save(outputFilePath);
            inputImage.Dispose();
        }

        private void ProcessAndCopyObjectTextures(string topDirectory, string tempObjectsFolder, string outputObjectsTexturesFolderRoot)
        {
            // Look for texture collisions for different texture files
            string tempObjectTextureFolderName = Path.Combine(tempObjectsFolder, "Textures");
            string tempMaterialsFolderName = Path.Combine(tempObjectsFolder, "MaterialLists");
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
                            Logger.WriteLine("- [" + topDirectory + "] Object Texture Collision with name '" + objectTextureFileNameOnly + "' but different contents so renaming to '" + newObjectTextureFileNameOnly + "'");
                            File.Move(sourceObjectTextureFile, Path.Combine(tempObjectTextureFolderName, newObjectTextureFileNameOnly));

                            // Update texture references in material files
                            string[] objectMaterialFiles = Directory.GetFiles(tempMaterialsFolderName);
                            foreach (string objectMaterialFile in objectMaterialFiles)
                            {
                                string fileText = File.ReadAllText(objectMaterialFile);
                                string objectTextureFileNameNoExtension = Path.GetFileNameWithoutExtension(objectTextureFileNameOnly);
                                if (fileText.Contains(":" + objectTextureFileNameNoExtension))
                                {
                                    string newObjectTextureFileNameNoExtension = Path.GetFileNameWithoutExtension(newObjectTextureFileNameOnly);
                                    Logger.WriteLine("- [" + topDirectory + "] Object material file '" + objectMaterialFile + "' contained texture '" + objectTextureFileNameNoExtension + "' which was renamed to '" + newObjectTextureFileNameNoExtension + "'. Updating material file...");
                                    fileText = fileText.Replace(":" + objectTextureFileNameNoExtension, ":" + newObjectTextureFileNameNoExtension);
                                    File.WriteAllText(objectMaterialFile, fileText);
                                }
                            }

                            // Continue loop using this new file name as a base
                            objectTextureFileNameOnly = newObjectTextureFileNameOnly;
                            sourceObjectTextureFile = Path.Combine(tempObjectTextureFolderName, objectTextureFileNameOnly);
                            targetObjectTextureFile = Path.Combine(outputObjectsTexturesFolderRoot, objectTextureFileNameOnly);
                            doesUnresolvedFileCollisionExist = true;
                        }
                        else
                        {
                            objectTexturesCondensed++;
                        }
                    }
                }
                while (doesUnresolvedFileCollisionExist);

                // Copy the file
                File.Copy(sourceObjectTextureFile, targetObjectTextureFile, true);
            }
        }

        private void ProcessAndCopyObjectMaterials(string topDirectory, string tempObjectsFolder, string outputObjectsMaterialsFolderRoot)
        {
            // Look for material collisions for different material files
            string tempObjectMaterialsFolderName = Path.Combine(tempObjectsFolder, "MaterialLists");
            string tempObjectMeshesFolderName = Path.Combine(tempObjectsFolder, "Meshes");            
            string[] objectMaterialFiles = Directory.GetFiles(tempObjectMaterialsFolderName);
            foreach (string objectMaterialFile in objectMaterialFiles)
            {
                // Calculate the full paths for comparison
                string objectMaterialFileNameOnly = Path.GetFileName(objectMaterialFile);
                string originalObjectMaterialFileNameOnlyNoExtension = Path.GetFileNameWithoutExtension(objectMaterialFile);
                string sourceObjectMaterialFile = Path.Combine(tempObjectMaterialsFolderName, objectMaterialFileNameOnly);
                string targetObjectMaterialFile = Path.Combine(outputObjectsMaterialsFolderRoot, objectMaterialFileNameOnly);

                // Loop until there is no unresolved file collision
                bool doesUnresolvedFileCollisionExist;
                uint altIteration = 1;
                do
                {
                    // Compare the files if the destination file already exist
                    doesUnresolvedFileCollisionExist = false;
                    if (File.Exists(targetObjectMaterialFile) == true)
                    {
                        // If the files collide but are not the exact same, create a new version
                        if (FileTool.AreFilesTheSame(targetObjectMaterialFile, sourceObjectMaterialFile) == false)
                        {
                            // Update the file name
                            string newObjectMaterialFileNameOnly = originalObjectMaterialFileNameOnlyNoExtension + "alt" + altIteration.ToString() + Path.GetExtension(objectMaterialFile);
                            altIteration++;
                            Logger.WriteLine("- [" + topDirectory + "] Object Material Collision with name '" + objectMaterialFileNameOnly + "' but different contents so renaming to '" + newObjectMaterialFileNameOnly + "'");
                            File.Move(sourceObjectMaterialFile, Path.Combine(tempObjectMaterialsFolderName, newObjectMaterialFileNameOnly));

                            // Update material references in mesh files
                            string[] objectMeshFiles = Directory.GetFiles(tempObjectMeshesFolderName);
                            foreach (string objectMeshFile in objectMeshFiles)
                            {
                                string fileText = File.ReadAllText(objectMeshFile);
                                string objectMaterialFileNameNoExtension = Path.GetFileNameWithoutExtension(objectMaterialFileNameOnly);
                                if (fileText.Contains("," + objectMaterialFileNameNoExtension))
                                {
                                    string newObjectMaterialFileNameNoExtension = Path.GetFileNameWithoutExtension(newObjectMaterialFileNameOnly);
                                    Logger.WriteLine("- [" + topDirectory + "] Object mesh file '" + objectMeshFile + "' contained material '" + objectMaterialFileNameNoExtension + "' which was renamed to '" + newObjectMaterialFileNameNoExtension + "'. Updating mesh file...");
                                    fileText = fileText.Replace("," + objectMaterialFileNameNoExtension, "," + newObjectMaterialFileNameNoExtension);
                                    File.WriteAllText(objectMeshFile, fileText);
                                }
                            }

                            // Continue loop using this new file name as a base
                            objectMaterialFileNameOnly = newObjectMaterialFileNameOnly;
                            sourceObjectMaterialFile = Path.Combine(tempObjectMaterialsFolderName, objectMaterialFileNameOnly);
                            targetObjectMaterialFile = Path.Combine(outputObjectsMaterialsFolderRoot, objectMaterialFileNameOnly);
                            doesUnresolvedFileCollisionExist = true;
                        }
                        else
                        {
                            objectMaterialsCondensed++;
                        }
                    }
                }
                while (doesUnresolvedFileCollisionExist);

                // Copy the file
                File.Copy(sourceObjectMaterialFile, targetObjectMaterialFile, true);
            }
        }

        private void ProcessAndCopyObjectMeshes(string topDirectory, string tempObjectsFolder, string tempZoneFolder, string outputObjectsMeshesFolderRoot)
        {
            // Look for mesh collisions for different mesh files
            string tempObjectMeshesFolderName = Path.Combine(tempObjectsFolder, "Meshes");
            string[] objectMeshFiles = Directory.GetFiles(tempObjectMeshesFolderName);
            foreach (string objectMeshFile in objectMeshFiles)
            {
                // Skip collision meshes
                if (objectMeshFile.Contains("_collision.txt"))
                    continue;

                // Calculate the full paths for comparison
                string objectMeshFileNameOnly = Path.GetFileName(objectMeshFile);
                string originalObjectMeshFileNameOnlyNoExtension = Path.GetFileNameWithoutExtension(objectMeshFile);
                string sourceObjectMeshFile = Path.Combine(tempObjectMeshesFolderName, objectMeshFileNameOnly);
                string targetObjectMeshFile = Path.Combine(outputObjectsMeshesFolderRoot, objectMeshFileNameOnly);

                // Loop until there is no unresolved file collision
                bool doesUnresolvedFileCollisionExist;
                uint altIteration = 1;
                do
                {
                    // Compare the files if the destination file already exist
                    doesUnresolvedFileCollisionExist = false;
                    if (File.Exists(targetObjectMeshFile) == true)
                    {
                        // If the files collide but are not the exact same, create a new version
                        if (FileTool.AreFilesTheSame(targetObjectMeshFile, sourceObjectMeshFile) == false)
                        {
                            // Update the file name
                            string newObjectMeshFileNameOnly = originalObjectMeshFileNameOnlyNoExtension + "alt" + altIteration.ToString() + Path.GetExtension(objectMeshFile);
                            altIteration++;
                            Logger.WriteLine("- [" + topDirectory + "] Object Mesh Collision with name '" + objectMeshFileNameOnly + "' but different contents so renaming to '" + newObjectMeshFileNameOnly + "'");
                            File.Move(sourceObjectMeshFile, Path.Combine(tempObjectMeshesFolderName, newObjectMeshFileNameOnly));

                            // Also update the collision file, if there was one
                            string collisionMeshFileName = Path.GetFileNameWithoutExtension(objectMeshFileNameOnly) + "_collision.txt";
                            string collisionMeshFilePath = Path.Combine(tempObjectMeshesFolderName, collisionMeshFileName);
                            if (File.Exists(collisionMeshFilePath))
                            {
                                string newCollisionMeshFilePath = Path.Combine(tempObjectMeshesFolderName, originalObjectMeshFileNameOnlyNoExtension + "alt" + altIteration.ToString() + "_collision.txt");
                                Logger.WriteLine("- [" + topDirectory + "] Object Mesh also had a collision mesh with name '" + collisionMeshFileName + "', so changing that as well");
                                File.Move(collisionMeshFilePath, newCollisionMeshFilePath);
                            }

                            // Update mesh references in zone object instances file
                            string zoneObjectInstancesFile = Path.Combine(tempZoneFolder, "object_instances.txt");
                            if (File.Exists(zoneObjectInstancesFile) == false)
                            {
                                Logger.WriteLine("- [" + topDirectory + "] No object_instances file to update");
                            }
                            else
                            {
                                string fileText = File.ReadAllText(zoneObjectInstancesFile);
                                string objectMeshFileNameNoExtension = Path.GetFileNameWithoutExtension(objectMeshFileNameOnly);
                                if (fileText.Contains(objectMeshFileNameNoExtension + ","))
                                {
                                    string newObjectMeshFileNameNoExtension = Path.GetFileNameWithoutExtension(newObjectMeshFileNameOnly);
                                    Logger.WriteLine("- [" + topDirectory + "] Zone object_instances file '" + zoneObjectInstancesFile + "' contained mesh '" + objectMeshFileNameNoExtension + "' which was renamed to '" + newObjectMeshFileNameNoExtension + "'. Updating object_instances file...");
                                    fileText = fileText.Replace(objectMeshFileNameNoExtension + ",", newObjectMeshFileNameNoExtension + ",");
                                    File.WriteAllText(zoneObjectInstancesFile, fileText);
                                }
                            }

                            // Continue loop using this new file name as a base
                            objectMeshFileNameOnly = newObjectMeshFileNameOnly;
                            sourceObjectMeshFile = Path.Combine(tempObjectMeshesFolderName, objectMeshFileNameOnly);
                            targetObjectMeshFile = Path.Combine(outputObjectsMeshesFolderRoot, objectMeshFileNameOnly);
                            doesUnresolvedFileCollisionExist = true;
                        }
                        else
                        {
                            objectMeshesCondensed++;
                        }
                    }
                }
                while (doesUnresolvedFileCollisionExist);

                // Copy the files
                File.Copy(sourceObjectMeshFile, targetObjectMeshFile, true);
                string collisionMeshFileNameOnly = Path.GetFileNameWithoutExtension(sourceObjectMeshFile) + "_collision.txt";
                string sourceCollisionMeshFile = Path.Combine(tempObjectMeshesFolderName, collisionMeshFileNameOnly);
                if (File.Exists(sourceCollisionMeshFile))
                {
                    string targetCollisionMeshFile = Path.Combine(outputObjectsMeshesFolderRoot, collisionMeshFileNameOnly);
                    File.Copy(sourceCollisionMeshFile, targetCollisionMeshFile, true);
                }
            }
        }
    }
}
