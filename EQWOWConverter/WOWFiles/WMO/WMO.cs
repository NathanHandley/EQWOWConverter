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

using EQWOWConverter.Common;
using EQWOWConverter.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EQWOWConverter.WOWFiles
{
    internal class WMO
    {
        public WMORoot RootObject;
        public List<WMOGroup> GroupObjects = new List<WMOGroup>();
        private string BaseFileName;
        private string FullWMOFolderPath;
        public string RootFileRelativePathWithFileName;

        public WMO(Zone gameMap, string baseFolderPath)
        {
            BaseFileName = gameMap.Name;

            // Create root object
            RootObject = new WMORoot(gameMap);

            // Create the groups
            for(int i = 0; i < gameMap.RenderMesh.TextureAlignedSubMeshes.Count; i++)
                GroupObjects.Add(new WMOGroup(gameMap, RootObject, i));

            // Generate the root file name
            FullWMOFolderPath = Path.Combine(baseFolderPath,"World", "wmo", "Everquest");
            RootFileRelativePathWithFileName = Path.Combine("World", "wmo", "Everquest", BaseFileName + ".wmo");
        }

        public void WriteToDisk(string baseFolderPath)
        {
            FileTool.CreateBlankDirectory(FullWMOFolderPath, true);
            string RootFileFullPathAndFileName = Path.Combine(FullWMOFolderPath, BaseFileName + ".wmo");
            File.WriteAllBytes(RootFileFullPathAndFileName, RootObject.RootBytes.ToArray());
            
            UInt16 curGroupIndex = 0;
            foreach (WMOGroup group in GroupObjects)
            {
                string wmoGroupFileName;
                if (curGroupIndex < 10)
                    wmoGroupFileName = Path.Combine(FullWMOFolderPath, BaseFileName + "_00" + curGroupIndex + ".wmo");
                else if (curGroupIndex < 100)
                    wmoGroupFileName = Path.Combine(FullWMOFolderPath, BaseFileName + "_0" + curGroupIndex + ".wmo");
                else
                    wmoGroupFileName = Path.Combine(FullWMOFolderPath, BaseFileName + "_" + curGroupIndex + ".wmo");
                File.WriteAllBytes(wmoGroupFileName, group.GroupBytes.ToArray());
                curGroupIndex++;
            }
        }
    }
}
