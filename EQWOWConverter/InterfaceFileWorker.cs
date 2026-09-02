//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2026 Nathan Handley
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

using System.Text;

namespace EQWOWConverter
{
    internal class InterfaceFileWorker
    {
        // Only these subfolders of the client's Interface folder are pulled out
        private static readonly string[] INTERFACE_SUBFOLDERS_TO_EXTRACT = { "GlueXML", "FrameXML" };

        // Subfolders of Assets\CustomTextures\interface that go into the patch under Interface\<subfolder>
        private static readonly string[] INTERFACE_TEXTURE_SUBFOLDERS_TO_COPY = { "ICONS", "PlayerFrame", "TargetingFrame" };

        public void ExtractClientInterfaceFiles()
        {
            string wowExportPath = Configuration.PATH_EXPORT_FOLDER;

            Logger.WriteInfo("Extracting client interface files...");

            // Make sure the patches folder is correct
            string wowPatchesFolder = Path.Combine(Configuration.PATH_WORLDOFWARCRAFT_CLIENT_INSTALL_FOLDER, "Data", Configuration.PATCH_LOCALIZATION_STRING);
            if (Directory.Exists(wowPatchesFolder) == false)
                throw new Exception("WoW client patches folder does not exist at '" + wowPatchesFolder + "', did you set PATH_WORLDOFWARCRAFT_CLIENT_INSTALL_FOLDER?");

            // Filter away any patches this converter is generating
            List<string> generatedPatchFileNames = Configuration.GetGeneratedPatchFileNames();
            List<string> patchFileNames = new List<string>();
            patchFileNames.Add(Path.Combine(wowPatchesFolder, string.Concat("locale-", Configuration.PATCH_LOCALIZATION_STRING, ".MPQ")));
            patchFileNames.Add(Path.Combine(wowPatchesFolder, string.Concat("patch-", Configuration.PATCH_LOCALIZATION_STRING, ".MPQ")));
            string[] existingPatchFiles = Directory.GetFiles(wowPatchesFolder, "patch-*-*.MPQ");
            foreach (string existingPatchName in existingPatchFiles)
                if (generatedPatchFileNames.Contains(Path.GetFileName(existingPatchName).ToLower()) == false)
                    patchFileNames.Add(existingPatchName);

            // Make sure all of the files are not locked
            foreach (string patchFileName in patchFileNames)
                if (FileTool.IsFileLocked(patchFileName))
                    throw new Exception("Patch file named '" + patchFileName + "' was locked and in use by another application");

            // Clear out any previously extracted interface files
            Logger.WriteDebug("Deleting previously extracted interface files");
            string exportedInterfaceFolder = Path.Combine(wowExportPath, "ExportedInterfaceFiles");
            FileTool.CreateBlankDirectory(exportedInterfaceFolder, false);

            // Generate a script to extract the interface files
            Logger.WriteDebug("Generating script to extract interface files");
            string workingGeneratedScriptsFolder = Path.Combine(wowExportPath, "GeneratedWorkingScripts");
            if (Directory.Exists(workingGeneratedScriptsFolder) == false)
                Directory.CreateDirectory(workingGeneratedScriptsFolder);
            StringBuilder interfaceExtractScriptText = new StringBuilder();
            foreach (string interfaceSubfolder in INTERFACE_SUBFOLDERS_TO_EXTRACT)
            {
                string exportedInterfaceSubfolder = Path.Combine(exportedInterfaceFolder, interfaceSubfolder);
                Directory.CreateDirectory(exportedInterfaceSubfolder);
                foreach (string patchFileName in patchFileNames)
                    interfaceExtractScriptText.AppendLine("extract \"" + patchFileName + "\" Interface\\" + interfaceSubfolder + "\\* \"" + exportedInterfaceSubfolder + "\"");
            }
            string interfaceExtractionScriptFileName = Path.Combine(workingGeneratedScriptsFolder, "interfaceextract.txt");
            using (var interfaceExtractionScriptFile = new StreamWriter(interfaceExtractionScriptFileName))
                interfaceExtractionScriptFile.WriteLine(interfaceExtractScriptText.ToString());

            // Extract the files using the script
            Logger.WriteDebug("Extracting interface files");
            string mpqEditorFullPath = Path.Combine(Configuration.PATH_TOOLS_FOLDER, "ladikmpqeditor", "MPQEditor.exe");
            if (File.Exists(mpqEditorFullPath) == false)
                throw new Exception("Failed to extract interface files. '" + mpqEditorFullPath + "' does not exist. (Be sure to set your Configuration.PATH_TOOLS_FOLDER properly)");
            string args = "console \"" + interfaceExtractionScriptFileName + "\"";
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.Arguments = args;
            process.StartInfo.FileName = mpqEditorFullPath;
            process.Start();
            process.WaitForExit();

            Logger.WriteDebug("Extracting client interface files complete");
        }

        public void GenerateInterfaceFiles()
        {
            string stockInterfaceFolder = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "ExportedInterfaceFiles");
            string customInterfaceFolder = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "CustomInterface");
            string actionsFileName = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "InterfaceGenActions.csv");
            string outputInterfaceFolder = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "MPQReady", "Interface");
            GenerateInterfaceFiles(stockInterfaceFolder, customInterfaceFolder, actionsFileName, outputInterfaceFolder);

            // Textures the generated interface files reference
            string customTexturesFolder = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "CustomTextures", "interface");
            foreach (string textureSubfolder in INTERFACE_TEXTURE_SUBFOLDERS_TO_COPY)
            {
                string sourceFolder = Path.Combine(customTexturesFolder, textureSubfolder);
                if (Directory.Exists(sourceFolder) == false)
                {
                    Logger.WriteError("Custom interface texture folder '" + sourceFolder + "' did not exist, so nothing was copied from it");
                    continue;
                }
                string targetFolder = Path.Combine(outputInterfaceFolder, textureSubfolder);
                if (Directory.Exists(targetFolder) == false)
                    Directory.CreateDirectory(targetFolder);
                Logger.WriteDebug("Copying custom interface textures from '" + sourceFolder + "'");
                foreach (string sourceFileName in Directory.GetFiles(sourceFolder))
                    FileTool.CopyFile(sourceFileName, Path.Combine(targetFolder, Path.GetFileName(sourceFileName)));
            }
        }

        public void GenerateInterfaceFiles(string stockInterfaceFolder, string customInterfaceFolder, string actionsFileName, string outputInterfaceFolder)
        {
            Logger.WriteInfo("Generating client interface files...");

            if (Directory.Exists(stockInterfaceFolder) == false)
            {
                Logger.WriteError("Failed to generate interface files, as '" + stockInterfaceFolder + "' did not exist (is GENERATE_EXTRACT_INTERFACE_FILES false?)");
                return;
            }

            // Files that are this project's own get copied as-is
            foreach (string interfaceSubfolder in INTERFACE_SUBFOLDERS_TO_EXTRACT)
            {
                string customSubfolder = Path.Combine(customInterfaceFolder, interfaceSubfolder);
                if (Directory.Exists(customSubfolder) == false)
                    continue;
                string outputSubfolder = Path.Combine(outputInterfaceFolder, interfaceSubfolder);
                if (Directory.Exists(outputSubfolder) == false)
                    Directory.CreateDirectory(outputSubfolder);
                foreach (string customFileName in Directory.GetFiles(customSubfolder))
                {
                    Logger.WriteDebug("Copying custom interface file '" + customFileName + "'");
                    File.Copy(customFileName, Path.Combine(outputSubfolder, Path.GetFileName(customFileName)), true);
                }
            }

            // Stock files with changes get rebuilt from the actions.  The Data column is code and comes last, so it may hold the delimiter and unpaired double quotes, which the reader has to be told to allow for
            Dictionary<string, List<Dictionary<string, string>>> actionsByRelativeFileName = new Dictionary<string, List<Dictionary<string, string>>>();
            if (File.Exists(actionsFileName) == false)
                Logger.WriteError("Interface actions file '" + actionsFileName + "' did not exist, so no stock interface files will be changed");
            else
            {
                foreach (Dictionary<string, string> columns in FileTool.ReadAllRowsFromFileWithHeader(actionsFileName, "|", false, true))
                {
                    string relativeFileName = Path.Combine(columns["Folder"], columns["File"]);
                    if (actionsByRelativeFileName.ContainsKey(relativeFileName) == false)
                        actionsByRelativeFileName.Add(relativeFileName, new List<Dictionary<string, string>>());
                    actionsByRelativeFileName[relativeFileName].Add(columns);
                }
            }
            foreach (var actionsForFile in actionsByRelativeFileName)
            {
                string stockFileName = Path.Combine(stockInterfaceFolder, actionsForFile.Key);
                string outputFileName = Path.Combine(outputInterfaceFolder, actionsForFile.Key);
                if (File.Exists(stockFileName) == false)
                {
                    Logger.WriteError("Could not generate interface file '" + actionsForFile.Key + "', as the stock file '" + stockFileName + "' did not exist");
                    continue;
                }
                string? outputFolder = Path.GetDirectoryName(outputFileName);
                if (string.IsNullOrEmpty(outputFolder) == false && Directory.Exists(outputFolder) == false)
                    Directory.CreateDirectory(outputFolder);
                Logger.WriteDebug("Generating interface file '" + actionsForFile.Key + "' from the stock file and " + actionsForFile.Value.Count + " actions");
                ApplyInterfaceGenActions(stockFileName, outputFileName, actionsForFile.Value);
            }

            Logger.WriteDebug("Generating client interface files complete");
        }

        private void ApplyInterfaceGenActions(string stockFileName, string outputFileName, List<Dictionary<string, string>> actions)
        {
            string[] stockLines = File.ReadAllText(stockFileName, Encoding.UTF8).Split("\r\n");
            int lineCount = stockLines.Length;

            HashSet<int> deletedLineNumbers = new HashSet<int>();
            Dictionary<int, string> replacementsByLineNumber = new Dictionary<int, string>();
            Dictionary<int, SortedDictionary<int, string>> insertsByStepByAfterLineNumber = new Dictionary<int, SortedDictionary<int, string>>();
            foreach (Dictionary<string, string> action in actions)
            {
                string step = action["Step"];
                string line = action["Line"];
                string data = action["Data"];
                switch (action["Action"].ToLower())
                {
                    case "delete":
                        {
                            int startLine;
                            int endLine;
                            string[] rangeParts = line.Split('-', 2);
                            if (int.TryParse(rangeParts[0], out startLine) == false || (rangeParts.Length == 2 && int.TryParse(rangeParts[1], out endLine) == false))
                            {
                                Logger.WriteError("Interface action step " + step + " has an invalid delete line '" + line + "', skipping it");
                                continue;
                            }
                            if (rangeParts.Length == 1)
                                endLine = startLine;
                            else
                                endLine = int.Parse(rangeParts[1]);
                            if (startLine < 1 || endLine > lineCount || startLine > endLine)
                            {
                                Logger.WriteError("Interface action step " + step + " deletes lines " + line + " which is outside 1-" + lineCount + " of the stock file, skipping it");
                                continue;
                            }
                            for (int lineNumber = startLine; lineNumber <= endLine; lineNumber++)
                                deletedLineNumbers.Add(lineNumber);
                        } break;
                    case "replace":
                        {
                            int lineNumber;
                            if (int.TryParse(line, out lineNumber) == false || lineNumber < 1 || lineNumber > lineCount)
                            {
                                Logger.WriteError("Interface action step " + step + " replaces line '" + line + "' which is outside 1-" + lineCount + " of the stock file, skipping it");
                                continue;
                            }
                            replacementsByLineNumber[lineNumber] = data;
                        } break;
                    case "insertafter":
                        {
                            int lineNumber;
                            int stepNumber;
                            if (int.TryParse(line, out lineNumber) == false || lineNumber < 0 || lineNumber > lineCount)
                            {
                                Logger.WriteError("Interface action step " + step + " inserts after line '" + line + "' which is outside 0-" + lineCount + " of the stock file, skipping it");
                                continue;
                            }
                            if (int.TryParse(step, out stepNumber) == false)
                            {
                                Logger.WriteError("Interface action step '" + step + "' is not a number, skipping it");
                                continue;
                            }
                            if (insertsByStepByAfterLineNumber.ContainsKey(lineNumber) == false)
                                insertsByStepByAfterLineNumber.Add(lineNumber, new SortedDictionary<int, string>());
                            insertsByStepByAfterLineNumber[lineNumber][stepNumber] = data;
                        } break;
                    case "copy": break; // The stock file ships as-is.  Listing it is what gets it written, nothing needs changing
                    default:
                        {
                            Logger.WriteError("Interface action step " + step + " has an unknown action '" + action["Action"] + "', skipping it");
                        } break;
                }
            }
            foreach (int deletedLineNumber in deletedLineNumbers)
                if (replacementsByLineNumber.ContainsKey(deletedLineNumber) == true)
                    Logger.WriteError("Interface actions for '" + stockFileName + "' both delete and replace line " + deletedLineNumber + ", the delete wins");

            List<string> outputLines = new List<string>();
            if (insertsByStepByAfterLineNumber.ContainsKey(0) == true)
                outputLines.AddRange(insertsByStepByAfterLineNumber[0].Values);
            for (int i = 0; i < lineCount; i++)
            {
                int lineNumber = i + 1;
                if (deletedLineNumbers.Contains(lineNumber) == true)
                {
                    // Dropped
                }
                else if (replacementsByLineNumber.ContainsKey(lineNumber) == true)
                    outputLines.Add(replacementsByLineNumber[lineNumber]);
                else
                    outputLines.Add(stockLines[i]);
                if (insertsByStepByAfterLineNumber.ContainsKey(lineNumber) == true)
                    outputLines.AddRange(insertsByStepByAfterLineNumber[lineNumber].Values);
            }

            File.WriteAllText(outputFileName, string.Join("\r\n", outputLines), new UTF8Encoding(false));
        }
    }
}
