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

namespace EQWOWConverter.WOWFiles
{
    internal class WMOAreaTableDBC : DBCFile
    {
        private static int CURRENT_ID = Configuration.DBCID_WMOAREATABLE_ID_START;

        public void AddRow(int wmoID, int wmoGroupID, int zoneMusic, int areaTableID, string areaName)
        {
            // Generate a new ID
            int ID = CURRENT_ID;
            CURRENT_ID++;

            DBCRow newRow = new DBCRow();
            newRow.AddInt32(ID);
            newRow.AddInt32(wmoID);
            newRow.AddInt32(0); // NameSetID
            newRow.AddInt32(wmoGroupID);
            newRow.AddInt32(0); // SoundProviderPref
            newRow.AddInt32(0); // SoundProviderPref - Underwater
            newRow.AddInt32(0); // AmbienceID
            newRow.AddInt32(zoneMusic);
            newRow.AddInt32(0); // Intro Sound
            newRow.AddPackedFlags(0); // Flags
            newRow.AddInt32(areaTableID);
            newRow.AddStringLang(areaName);

            // Also set the fields for sorting
            newRow.SortValue1 = wmoID;
            newRow.SortValue2 = 0; // NameSetID
            newRow.SortValue3 = wmoGroupID;

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
                    Logger.WriteError("WMOAreaTableDBC had no source raw bytes when converting a row in OnPostLoadDataFromDisk");
                    continue;
                }

                // Fill every field
                int byteCursor = 0;
                row.AddIntFromSourceRawBytes(ref byteCursor);
                row.AddIntFromSourceRawBytes(ref byteCursor);
                row.AddIntFromSourceRawBytes(ref byteCursor);
                row.AddIntFromSourceRawBytes(ref byteCursor);
                row.AddIntFromSourceRawBytes(ref byteCursor);
                row.AddIntFromSourceRawBytes(ref byteCursor);
                row.AddIntFromSourceRawBytes(ref byteCursor);
                row.AddIntFromSourceRawBytes(ref byteCursor);
                row.AddIntFromSourceRawBytes(ref byteCursor);
                row.AddPackedFlagsFromSourceRawBytes(ref byteCursor);
                row.AddIntFromSourceRawBytes(ref byteCursor);
                row.AddStringLangFromSourceRawBytes(ref byteCursor, StringBlock);

                // Attach the sort rows
                row.SortValue1 = ((DBCRow.DBCFieldInt32)row.AddedFields[1]).Value;
                row.SortValue2 = ((DBCRow.DBCFieldInt32)row.AddedFields[2]).Value;
                row.SortValue3 = ((DBCRow.DBCFieldInt32)row.AddedFields[3]).Value;

                // Purge raw data
                row.SourceRawBytes.Clear();
            }
        }
    }
}
