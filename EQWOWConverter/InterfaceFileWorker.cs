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
    }
}
