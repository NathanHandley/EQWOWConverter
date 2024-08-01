﻿//  Author: Nathan Handley (nathanhandley@protonmail.com)
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EQWOWConverter.Zones
{
    internal class Zone
    {
        public string DescriptiveName = string.Empty;
        public string ShortName { get; } = string.Empty;
        public string DescriptiveNameOnlyLetters = string.Empty;
        public ZoneEQData EQZoneData = new ZoneEQData();
        public ZoneWOWData WOWZoneData;

        public Zone(string shortName, ZoneProperties zoneProperties)
        {
            WOWZoneData = new ZoneWOWData(zoneProperties);
            ShortName = shortName;
            if (zoneProperties.DescriptiveName != string.Empty)
                SetDescriptiveName(zoneProperties.DescriptiveName);
            else
                DescriptiveNameOnlyLetters = shortName;
        }

        public void LoadEQZoneData(string inputZoneFolderName, string inputZoneFolderFullPath)
        {
            // Load
            EQZoneData.LoadDataFromDisk(inputZoneFolderName, inputZoneFolderFullPath);
        }
        
        public void PopulateWOWZoneDataFromEQZoneData()
        {
            WOWZoneData.LoadFromEQZone(EQZoneData);
        }

        public void SetDescriptiveName(string name)
        {
            DescriptiveName = name;
            DescriptiveNameOnlyLetters = name;
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            DescriptiveNameOnlyLetters = rgx.Replace(DescriptiveNameOnlyLetters, "");
            DescriptiveNameOnlyLetters = DescriptiveNameOnlyLetters.Replace(" ", "");
        }
    }
}
