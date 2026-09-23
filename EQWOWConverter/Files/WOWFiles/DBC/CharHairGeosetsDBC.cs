//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2026 Nathan Handley
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
    internal class CharHairGeosetsDBC : DBCFile
    {
        public void AddRow(int id, int raceID, int sexID, int variationIndex, int geosetID)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(id); // ID
            newRow.AddInt32(raceID); // RaceID (ChrRaces.dbc)
            newRow.AddInt32(sexID); // SexID (0 = male, 1 = female)
            newRow.AddInt32(variationIndex); // VariationID (hair style byte)
            newRow.AddInt32(geosetID); // GeosetID (group 0 geoset the style shows: the EQ head meshes)
            newRow.AddInt32(0); // Showscalp
            newRow.SortValue1 = id;
            Rows.Add(newRow);
        }

        protected override void OnPostLoadDataFromDisk()
        {
            // Rows stay as raw bytes, but they need sort values so the stock rows keep a stable order against the added rows
            foreach (DBCRow row in Rows)
            {
                if (row.SourceRawBytes.Count >= 4)
                    row.SortValue1 = BitConverter.ToInt32(row.SourceRawBytes.ToArray(), 0);
            }
        }
    }
}
