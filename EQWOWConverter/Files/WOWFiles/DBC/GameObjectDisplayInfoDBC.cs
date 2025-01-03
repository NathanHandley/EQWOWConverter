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
using EQWOWConverter.Zones;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EQWOWConverter.WOWFiles
{
    internal class GameObjectDisplayInfoDBC : DBCFile
    {
        public void AddRow(int id, string modelNameAndRelativePath, int loopSoundEntryID, BoundingBox geoBoundingBox)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt(id);
            newRow.AddString(modelNameAndRelativePath);
            newRow.AddInt(0); // Stand SoundEntries.ID
            newRow.AddInt(0); // Open SoundEntries.ID
            newRow.AddInt(loopSoundEntryID); // Loop SoundEntries.ID
            newRow.AddInt(0); // Close SoundEntries.ID
            newRow.AddInt(0); // Destroy SoundEntries.ID
            newRow.AddInt(0); // Opened SoundEntries.ID
            newRow.AddInt(0); // Custom0 SoundEntries.ID
            newRow.AddInt(0); // Custom1 SoundEntries.ID
            newRow.AddInt(0); // Custom2 SoundEntries.ID
            newRow.AddInt(0); // Custom3 SoundEntries.ID
            newRow.AddFloat(geoBoundingBox.BottomCorner.X); // GeoBox Min X
            newRow.AddFloat(geoBoundingBox.BottomCorner.Y); // GeoBox Min Y
            newRow.AddFloat(geoBoundingBox.BottomCorner.Z); // GeoBox Min Z
            newRow.AddFloat(geoBoundingBox.TopCorner.X); // GeoBox Max X
            newRow.AddFloat(geoBoundingBox.TopCorner.Y); // GeoBox Max Y
            newRow.AddFloat(geoBoundingBox.TopCorner.Z); // GeoBox Max Z
            newRow.AddInt(0); // ObjectEffectPackageID (?)
            Rows.Add(newRow);
        }
    }
}
