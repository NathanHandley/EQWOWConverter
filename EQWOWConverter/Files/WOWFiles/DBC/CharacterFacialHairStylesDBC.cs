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
    internal class CharacterFacialHairStylesDBC : DBCFile
    {
        public void AddRow(int raceID, int sexID, int variationIndex)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(raceID); // RaceID (ChrRaces.dbc)
            newRow.AddInt32(sexID); // SexID (0 = male, 1 = female)
            newRow.AddInt32(variationIndex); // VariationID (facial hair byte)
            newRow.AddInt32(0); // Geoset 1
            newRow.AddInt32(0); // Geoset 2
            newRow.AddInt32(0); // Geoset 3
            newRow.AddInt32(0); // Geoset 4
            newRow.AddInt32(0); // Geoset 5
            newRow.SortValue1 = 1; // After all stock rows
            newRow.SortValue2 = (raceID * 1000) + (sexID * 100) + variationIndex;
            Rows.Add(newRow);
        }

        protected override void OnPostLoadDataFromDisk()
        {
            // Stock rows keep their original file order (the client scans character DBCs in file order, see CharSectionsDBC) and the added rows go after them, grouped by (race, sex, variation)
            for (int i = 0; i < Rows.Count; i++)
            {
                Rows[i].SortValue1 = 0;
                Rows[i].SortValue2 = i;
            }
        }
    }
}
