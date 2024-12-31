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
//  Copyright (c) 2024 Nathan Handley
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EQWOWConverter;
using LanternExtractor.EQ;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor
{
    static class LanternExtractor
    {
        private static Settings _settings;
        private static ILogger _logger;

        public static bool ProcessRequest(string everquestDirectory, string extractDirectory)
        {
            _logger = new TextFileLogger();
            _settings = new Settings("settings.txt", _logger);
            _logger.SetVerbosity((LogVerbosity)_settings.LoggerVerbosity);

            // Set properties
            _settings.EverQuestDirectory = Path.GetFullPath(everquestDirectory) + "/";
            _settings.RawS3dExtract = false;
            _settings.ModelExportFormat = ModelExportFormat.Intermediate;
            _settings.ExportZoneMeshGroups = false;
            _settings.ExportHiddenGeometry = true;
            _settings.ExportCharactersToSingleFolder = true;
            _settings.ExportEquipmentToSingleFolder = true;
            _settings.ExportSoundsToSingleFolder = true;
            _settings.ClientDataToCopy = "spells.eff,spdat.eff";
            _settings.CopyMusic = true;

            extractDirectory = Path.GetFullPath(extractDirectory) + "/";

            string archiveName = "all";
            List<string> eqFiles = EqFileHelper.GetValidEqFilePaths(_settings.EverQuestDirectory, archiveName);
            eqFiles.Sort();

            if (eqFiles.Count == 0 && !EqFileHelper.IsSpecialCaseExtraction(archiveName))
            {
                Logger.WriteError("Extractor Error - No valid EQ files found for: '" + archiveName + "' at path: " +
                                  _settings.EverQuestDirectory);
                return false;
            }

            Logger.WriteInfo("Extractor extracting files...");

            // For the counter
            int curProgress = 0;
            int curProgressOffset = Logger.GetConsolePriorRowCursorLeft();
            Logger.WriteCounter(curProgress, curProgressOffset);
            
            foreach (var file in eqFiles)
            {
                ArchiveExtractor.Extract(file, extractDirectory, _logger, _settings);
                curProgress++;
                Logger.WriteCounter(curProgress, curProgressOffset);
            }

            Logger.WriteInfo("Extractor copying client data...");
            ClientDataCopier.Copy(archiveName, extractDirectory, _logger, _settings);
            Logger.WriteInfo("Extractor copying music...");
            MusicCopier.Copy(archiveName, extractDirectory, _logger, _settings);

            return true;
        }
    }
}
