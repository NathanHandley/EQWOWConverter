﻿//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2025 Nathan Handley
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

using EQWOWConverter.Zones;

namespace EQWOWConverter.WOWFiles
{
    internal class WorldSafeLocsDBC : DBCFile
    {
        public void AddRow(ZonePropertiesGraveyard graveyard, int mapID)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(graveyard.WorldSafeLocsDBCID); // ID
            newRow.AddInt32(mapID); // Continent (MapID)
            newRow.AddFloat(MathF.Round(graveyard.RespawnX, 6)); // X
            newRow.AddFloat(MathF.Round(graveyard.RespawnY, 6)); // Y
            newRow.AddFloat(MathF.Round(graveyard.RespawnZ, 6)); // Z
            newRow.AddStringLang(graveyard.Description); // Description            
            Rows.Add(newRow);
        }
    }
}
