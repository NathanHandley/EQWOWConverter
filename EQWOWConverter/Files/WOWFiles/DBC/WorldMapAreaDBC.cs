//  Author: Nathan Handley (nathanhandley@protonmail.com)
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

namespace EQWOWConverter.WOWFiles;

internal class WorldMapAreaDBC : DBCFile
{
    public void AddRow(int rowID, int zoneMapID, int areaTableID, string mapFolderNameNoExt, float locationLeft,
        float locationRight, float locationTop, float locationBottom)
    {
        DBCRow newRow = new DBCRow();

        newRow.AddInt32(rowID); // ID
        newRow.AddInt32(zoneMapID); // MapID
        newRow.AddInt32(areaTableID); // AreaID
        newRow.AddString(mapFolderNameNoExt); // AreaName, name of the map folder and files
        newRow.AddFloat(locationLeft); // LocLeft, position on the left
        newRow.AddFloat(locationRight); // LocRight, position on the right
        newRow.AddFloat(locationTop); // LocTop, position at the top
        newRow.AddFloat(locationBottom); // LocBottom, position at the bottom
        newRow.AddInt32(-1); // DisplayMapID, Virtual Map ID (Map.dbc.id)
        newRow.AddInt32(0); // DefaultDungeonFloor, Dungeon Map / Floor ID (DungeonMap.dbc.id)
        newRow.AddInt32(0); // ParentWorldMapID

        // Set the sort
        newRow.SortValue1 = rowID;

        Rows.Add(newRow);
    }


    protected override void OnPostLoadDataFromDisk()
    {
        // Convert any raw data rows to actual data rows (which should be all of them)
        foreach (DBCRow row in Rows)
        {
            // This shouldn't be possible, but control for it just in case
            if (row.SourceRawBytes.Count == 0)
            {
                Logger.WriteError("WorldMapAreaDBC had no source raw bytes when converting a row in OnPostLoadDataFromDisk");
                continue;
            }

            // Fill every field
            int byteCursor = 0;
            row.AddIntFromSourceRawBytes(ref byteCursor);
            row.AddIntFromSourceRawBytes(ref byteCursor);
            row.AddIntFromSourceRawBytes(ref byteCursor);
            row.AddStringFromSourceRawBytes(ref byteCursor, StringBlock);
            row.AddFloatFromSourceRawBytes(ref byteCursor);
            row.AddFloatFromSourceRawBytes(ref byteCursor);
            row.AddFloatFromSourceRawBytes(ref byteCursor);
            row.AddFloatFromSourceRawBytes(ref byteCursor);
            row.AddIntFromSourceRawBytes(ref byteCursor);
            row.AddIntFromSourceRawBytes(ref byteCursor);
            row.AddIntFromSourceRawBytes(ref byteCursor);

            // Attach the sort rows
            row.SortValue1 = ((DBCRow.DBCFieldInt32)row.AddedFields[0]).Value; // ID

            // Purge raw data
            row.SourceRawBytes.Clear();
        }
    }
}
