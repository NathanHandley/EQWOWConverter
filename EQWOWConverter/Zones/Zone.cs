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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Zones
{
    internal class Zone
    {
        public static readonly float WORLD_SCALE = 0.35f;

        public string Name { get; } = string.Empty;
        public string ShortName { get; } = string.Empty;
        public EQZoneData EQZoneData = new EQZoneData();
        public WOWZoneData WOWZoneData = new WOWZoneData();

        public Zone(string name, string shortName)
        {
            Name = name;
            ShortName = shortName;
        }

        public void LoadEQZoneData(string inputZoneFolderName, string inputZoneFolderFullPath)
        {
            // Clear any old data and reload
            EQZoneData = new EQZoneData();
            EQZoneData.LoadDataFromDisk(inputZoneFolderName, inputZoneFolderFullPath);
        }
        
        public void PopulateWOWZoneDataFromEQZoneData()
        {
            List<string> texturesToGroupIsolate = new List<string>();
            WOWZoneData.LoadFromEQZone(EQZoneData, texturesToGroupIsolate, WORLD_SCALE);
        }
    }
}
