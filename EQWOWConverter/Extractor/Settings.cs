//  This file is part of a project licensed under the GNU General Public License (GPL).
//  Portions of this file incorporate code originally licensed under the MIT License.
// 
//  Original MIT License Notice:
//  Copyright(c) 2024 LanternEQ
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
// 
//  Additional code in this file and the project as a whole is licensed under the
//  GNU General Public License.
//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2025 Nathan Handley
//
//  This program is free software: you can redistribute it and/or modify it under the
//  terms of the GNU General Public License as published by the Free Software Foundation,
//  either version 3 of the License, or (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful, but WITHOUT ANY
//  WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A
//  PARTICULAR PURPOSE.  See the GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using System;
using System.IO;
using LanternExtractor.Infrastructure;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor
{
    /// <summary>
    /// Simple class that parses settings for the extractor
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// The logger reference for debug output
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The OS path to the settings file
        /// </summary>
        private readonly string _settingsFilePath;

        /// <summary>
        /// The OS path to the EverQuest directory
        /// </summary>
        public string EverQuestDirectory { get; set; }

        /// <summary>
        /// Extract data from the WLD file
        /// If false, we just extract the S3D contents
        /// </summary>
        public bool RawS3dExtract { get; set; }

        /// <summary>
        /// Adds group separation in the zone mesh export
        /// </summary>
        public bool ExportZoneMeshGroups { get; set; }

        /// <summary>
        /// Exports hidden geometry like zone boundaries
        /// </summary>
        public bool ExportHiddenGeometry { get; set; }

        /// <summary>
        /// Sets the desired model export format
        /// </summary>
        public ModelExportFormat ModelExportFormat { get; set; }
        
        public bool ExportCharactersToSingleFolder { get; set; }
        
        public bool ExportEquipmentToSingleFolder { get; set; }
        
        public bool ExportSoundsToSingleFolder { get; set; }

        /// <summary>
        /// Exports all OBJ frames for all animations
        /// </summary>
        public bool ExportAllAnimationFrames { get; set; }

        /// <summary>
        /// Exports all OBJ frames for all animations
        /// </summary>
        public bool ExportZoneWithObjects { get; set; }

        /// <summary>
        /// Export vertex colors with glTF model. Default behavior of glTF renderers
        /// is to mix the vertex color with the base color, which will not look right.
        /// Only turn this on if you intend to do some post-processing that
        /// requires vertex colors being present.
        /// </summary>
        public bool ExportGltfVertexColors { get; set; }

        /// <summary>
        /// Exports glTF models in .GLB file format. GLB packages the .glTF json, the
        /// associated .bin, and all of the model's texture images into one file. This will
        /// take up more space since textures can't be shared, however, it will make models
        /// more portable.
        /// </summary>
        public bool ExportGltfInGlbFormat { get; set; }

        /// <summary>
        /// Additional files that should be copied when extracting with `all` or `clientdata`
        /// </summary>
        public string ClientDataToCopy { get; set; }
        
        /// <summary>
        /// If enabled, XMI files will be copied to the 'Exports/Music' folder
        /// </summary>
        public bool CopyMusic { get; set; }

        /// <summary>
        /// The verbosity of the logger
        /// </summary>
        public int LoggerVerbosity { get; private set; }

        /// <summary>
        /// Constructor which caches the settings file path and the logger
        /// Also sets defaults for the settings in the case the file isn't found
        /// </summary>
        /// <param name="settingsFilePath">The OS path to the settings file</param>
        /// <param name="logger">A reference to the logger for debug info</param>
        public Settings(string settingsFilePath, ILogger logger)
        {
            _settingsFilePath = settingsFilePath;
            _logger = logger;

            EverQuestDirectory = "C:/EverQuest/";
            RawS3dExtract = false;
            ExportZoneMeshGroups = false;
            ExportHiddenGeometry = false;
            LoggerVerbosity = 0;
        }


        //public void Initialize()
        //{
        //    string settingsText;

        //    try
        //    {
        //        settingsText = File.ReadAllText(_settingsFilePath);
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError("Error loading settings file: " + e.Message);
        //        return;
        //    }

        //    var parsedSettings = TextParser.ParseTextToDictionary(settingsText, '=', '#');

        //    if (parsedSettings == null)
        //    {
        //        return;
        //    }

        //    if (parsedSettings.TryGetValue("EverQuestDirectory", out var setting))
        //    {
        //        EverQuestDirectory = setting;

        //        // Ensure the path ends with a /
        //        EverQuestDirectory = Path.GetFullPath(EverQuestDirectory + "/");
        //    }

        //    if (parsedSettings.TryGetValue("RawS3DExtract", out var parsedSetting))
        //    {
        //        RawS3dExtract = Convert.ToBoolean(parsedSetting);
        //    }

        //    if (parsedSettings.TryGetValue("ExportZoneMeshGroups", out var setting1))
        //    {
        //        ExportZoneMeshGroups = Convert.ToBoolean(setting1);
        //    }

        //    if (parsedSettings.TryGetValue("ExportHiddenGeometry", out var parsedSetting1))
        //    {
        //        ExportHiddenGeometry = Convert.ToBoolean(parsedSetting1);
        //    }

        //    if (parsedSettings.TryGetValue("ExportZoneWithObjects", out var setting2))
        //    {
        //        ExportZoneWithObjects = Convert.ToBoolean(setting2);
        //    }

        //    if (parsedSettings.TryGetValue("ModelExportFormat", out var parsedSetting2))
        //    {
        //        var exportFormatSetting = (ModelExportFormat)Convert.ToInt32(parsedSetting2);
        //        ModelExportFormat = exportFormatSetting;
        //    }

        //    if (parsedSettings.TryGetValue("ExportCharacterToSingleFolder", out var setting3))
        //    {
        //        ExportCharactersToSingleFolder = Convert.ToBoolean(setting3);
        //    }

        //    if (parsedSettings.TryGetValue("ExportEquipmentToSingleFolder", out var parsedSetting3))
        //    {
        //        ExportEquipmentToSingleFolder = Convert.ToBoolean(parsedSetting3);
        //    }
            
        //    if (parsedSettings.TryGetValue("ExportSoundsToSingleFolder", out var setting4))
        //    {
        //        ExportSoundsToSingleFolder = Convert.ToBoolean(setting4);
        //    }

        //    if (parsedSettings.TryGetValue("ExportAllAnimationFrames", out var parsedSetting4))
        //    {
        //        ExportAllAnimationFrames = Convert.ToBoolean(parsedSetting4);
        //    }

        //    if (parsedSettings.TryGetValue("ExportGltfVertexColors", out var setting5))
        //    {
        //        ExportGltfVertexColors = Convert.ToBoolean(setting5);
        //    }

        //    if (parsedSettings.TryGetValue("ExportGltfInGlbFormat", out var parsedSetting5))
        //    {
        //        ExportGltfInGlbFormat = Convert.ToBoolean(parsedSetting5);
        //    }

        //    if (parsedSettings.TryGetValue("ClientDataToCopy", out var setting6))
        //    {
        //        ClientDataToCopy = setting6;
        //    }
            
        //    if (parsedSettings.TryGetValue("ClientDataToCopy", out var parsedSetting6))
        //    {
        //        ClientDataToCopy = parsedSetting6;
        //    }
            
        //    if (parsedSettings.TryGetValue("CopyMusic", out var setting7))
        //    {
        //        CopyMusic = Convert.ToBoolean(setting7);
        //    }

        //    if (parsedSettings.TryGetValue("LoggerVerbosity", out var parsedSetting7))
        //    {
        //        LoggerVerbosity = Convert.ToInt32(parsedSetting7);
        //    }
        //}
    }
}
