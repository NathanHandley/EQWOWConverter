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
    internal class TaxiPathNodeDBC : DBCFile
    {
        private static int CUR_ID = Configuration.DBCID_TAXIPATHNODE_ID_START;

        public void AddRow(int pathID, int nodeIndex, int mapID, float posX, float posY, float posZ,
            int delayInSeconds)
        {
            // Generate flags
            int flags = 0;
            if (delayInSeconds > 0)
                flags += 2;

            // Calculate the delay
            int delayInSecCalc = delayInSecCalc = Convert.ToInt32(MathF.Round(Convert.ToSingle(delayInSeconds) * Configuration.TRANSPORT_PAUSE_MULTIPLIER));            

            DBCRow newRow = new DBCRow();
            newRow.AddInt32(CUR_ID); // ID
            newRow.AddInt32(pathID); // PathID
            newRow.AddInt32(nodeIndex); // NodeIndex
            newRow.AddInt32(mapID); // ContinentID
            newRow.AddFloat(posX); // LocX
            newRow.AddFloat(posY); // LocY
            newRow.AddFloat(posZ); // LocZ
            newRow.AddInt32(flags); // Flags
            newRow.AddInt32(delayInSecCalc); // Delay in seconds
            newRow.AddInt32(0); // ArrivalEventID
            newRow.AddInt32(0); // DepatureEventID

            // Set sorting
            newRow.SortValue1 = pathID;
            newRow.SortValue2 = nodeIndex;

            Rows.Add(newRow);

            CUR_ID++;
        }

        protected override void OnPostLoadDataFromDisk()
        {
            // Convert any raw data rows to actual data rows (which should be all of them)
            foreach (DBCRow row in Rows)
            {
                // This shouldn't be possible, but control for it just in case
                if (row.SourceRawBytes.Count == 0)
                {
                    Logger.WriteError("TaxiPathNodeDBC had no source raw bytes when converting a row in OnPostLoadDataFromDisk");
                    continue;
                }

                // Fill every field
                int byteCursor = 0;
                row.AddIntFromSourceRawBytes(ref byteCursor);
                row.AddIntFromSourceRawBytes(ref byteCursor);
                row.AddIntFromSourceRawBytes(ref byteCursor);
                row.AddIntFromSourceRawBytes(ref byteCursor);
                row.AddFloatFromSourceRawBytes(ref byteCursor);
                row.AddFloatFromSourceRawBytes(ref byteCursor);
                row.AddFloatFromSourceRawBytes(ref byteCursor);
                row.AddIntFromSourceRawBytes(ref byteCursor);
                row.AddIntFromSourceRawBytes(ref byteCursor);
                row.AddIntFromSourceRawBytes(ref byteCursor);
                row.AddIntFromSourceRawBytes(ref byteCursor);

                // Attach the sort rows
                row.SortValue1 = ((DBCRow.DBCFieldInt32)row.AddedFields[1]).Value; // PathID
                row.SortValue2 = ((DBCRow.DBCFieldInt32)row.AddedFields[2]).Value; // NodeIndex

                // Purge raw data
                row.SourceRawBytes.Clear();
            }
        }
    }
}
