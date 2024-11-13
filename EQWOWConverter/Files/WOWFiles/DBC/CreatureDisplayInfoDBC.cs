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

using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EQWOWConverter.WOWFiles
{
    internal class CreatureDisplayInfoDBC : DBCFile
    {
        public void AddRow(int id, int displayID)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt(id); // ID
            newRow.AddInt(displayID); // CreatureModelData.ID
            newRow.AddInt(0); // CreatureSoundData.ID
            newRow.AddInt(0); // CreatureDisplayInfoExtra.ID
            newRow.AddFloat(1); // Model Scale
            newRow.AddInt(255); // Opacity (255 opaque, 0 transparent)
            newRow.AddString(string.Empty); // Texture1 (texture for 1st geoset)
            newRow.AddString(string.Empty); // Texture2 (texture for 2nd geoset)
            newRow.AddString(string.Empty); // Texture2 (texture for 3rd geoset)
            newRow.AddString(string.Empty); // Portrait Texture
            newRow.AddInt(1); // Blood level (appears to reference UnitBloodLevels.dbc, but actually uses CreatureModelData)
            newRow.AddInt(0); // Blood ID (appears to reference UnitBlood.dbc, but that doesn't exist...)
            newRow.AddInt(0); // References NPCSounds.dbc, used for interactions like "hello" when clicking
            newRow.AddInt(0); // ParticleColor.dbc reference, typically zero
            newRow.AddInt(0); // CreatureGeosetData.  Example: 0x00200000 will select geoset 2 out of group 600 (602)
            newRow.AddInt(0); // Unsure, but set for gyrocopters, catapults, rocketmounts, siegevehicles
            Rows.Add(newRow);
        }
    }
}
