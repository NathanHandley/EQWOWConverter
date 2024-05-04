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
        private static readonly UInt32 CONFIG_START_WMOID = 7000; // Reserving 7000-7200
        private static UInt32 CURRENTWMOID = CONFIG_START_WMOID;

        public string Name { get; } = string.Empty;
        public EQZoneData EQZoneData = new EQZoneData();
        public WOWZoneData WOWZoneData = new WOWZoneData(0);

        public Zone(string name)
        {
            Name = name;
        }

        public void LoadEQZoneData(string inputZoneFolderName, string inputZoneFolderFullPath)
        {
            // Clear any old data and reload
            EQZoneData = new EQZoneData();
            EQZoneData.LoadDataFromDisk(inputZoneFolderName, inputZoneFolderFullPath);
        }
        
        public void PopulateWOWZoneDataFromEQZoneData()
        {
            // Clear any old data and reload
            WOWZoneData = new WOWZoneData(CURRENTWMOID);
            CURRENTWMOID++;
            List<string> texturesToGroupIsolate = new List<string>();
            WOWZoneData.LoadFromEQZone(EQZoneData, texturesToGroupIsolate);
        }
    }
}
