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

using System.IO;
using System.Linq;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ
{
    public static class MusicCopier
    {
        private const string MusicDirectory = "music";

        public static void Copy(string shortname, string folderName, ILogger logger, Settings settings)
        {
            if (shortname != "music" && shortname != "all")
            {
                return;
            }
            
            if (!settings.CopyMusic)
            {
                return;
            }

            var xmiFiles = Directory.GetFiles(settings.EverQuestDirectory, "*.*", SearchOption.AllDirectories)
                .Where(EqFileHelper.IsMusicFile).ToList();
            var destinationFolder = Path.Combine(folderName, MusicDirectory);

            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }

            foreach (var xmi in xmiFiles)
            {
                var fileName = Path.GetFileName(xmi);
                var destination = Path.Combine(destinationFolder, fileName);
                if (File.Exists(destination))
                {
                    continue;
                }

                File.Copy(xmi, destination);
            }
        }
    }
}