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

namespace EQWOWConverter.WOWFiles
{
    internal class MapDBC : DBCFile
    {
        public void AddRow(int id, string directory, string mapName, int areaTableID, int loadingScreenID)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(id); // MapID / Primary Key
            newRow.AddString(directory);
            newRow.AddInt32(0); // Instance Type (0 - None, 1 - Party, 2 - Raid, 3 - PVP, 4 - Arena)
            newRow.AddPackedFlags(0); // Flags
            newRow.AddPackedFlags(0); // PVP
            newRow.AddStringLang(mapName);
            newRow.AddInt32(areaTableID);
            newRow.AddStringLang(string.Empty); // Alliance Map Description
            newRow.AddStringLang(string.Empty); // Horde Map Description
            newRow.AddInt32(loadingScreenID);
            newRow.AddFloat(1f); // Minimap Icon Scaling
            newRow.AddInt32(1); // Corpse Map ID (references first column, or -1 for none. 0 is eastern kingdoms (westfall))
            newRow.AddFloat(0); // CorpseX - "This is listed as the x-coordinate of the instance entrance" on wowdev... why?
            newRow.AddFloat(0); // CorpseY - "This is listed as the y-coordinate of the instance entrance" on wowdev... why?
            newRow.AddInt32(-1); // TimeOfDayOverride - This is -1 for everywhere except Orgimmar and Dalaran
            newRow.AddInt32(0); // ExpansionID (0 - Vanilla, 1 - BC, 2 - WOTLK)
            newRow.AddInt32(0); // RaidOffset (?)
            newRow.AddInt32(0); // Max Players (0 if no max?)

            // Set sorting row
            newRow.SortValue1 = id; // MapID

            Rows.Add(newRow);
        }
    }
}
