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
    internal class AreaTriggerDBC : DBCFile
    {
        public static int CURRENT_AREATRIGGER_ID = Configuration.DBCID_AREATRIGGER_ID_START;
        private static readonly object AreaTriggerLock = new object();
        public static int GetGeneratedAreaTriggerID()
        {
            int generatedID;
            lock (AreaTriggerLock)
            {
                generatedID = CURRENT_AREATRIGGER_ID;
                CURRENT_AREATRIGGER_ID++;
            }
            return generatedID;
        }

        public void AddRow(int areaTriggerID, int mapID, float positionX, float positionY, float positionZ,
            float boxLength, float boxWidth, float boxHeight, float boxOrientation)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(areaTriggerID);
            newRow.AddInt32(mapID);
            newRow.AddFloat(positionX);
            newRow.AddFloat(positionY);
            newRow.AddFloat(positionZ);
            newRow.AddFloat(0); // Radius
            newRow.AddFloat(boxLength);
            newRow.AddFloat(boxWidth);
            newRow.AddFloat(boxHeight);
            newRow.AddFloat(boxOrientation);

            // Set the sort
            newRow.SortValue1 = mapID;
            newRow.SortValue2 = areaTriggerID;

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
                    Logger.WriteError("AreaTriggerDBC had no source raw bytes when converting a row in OnPostLoadDataFromDisk");
                    continue;
                }

                // Fill every field
                int byteCursor = 0;
                row.AddIntFromSourceRawBytes(ref byteCursor); // AreaTriggerID
                row.AddIntFromSourceRawBytes(ref byteCursor); // MapID
                row.AddFloatFromSourceRawBytes(ref byteCursor); // PositionX
                row.AddFloatFromSourceRawBytes(ref byteCursor); // PositionY
                row.AddFloatFromSourceRawBytes(ref byteCursor); // PositionZ
                row.AddFloatFromSourceRawBytes(ref byteCursor); // Radius
                row.AddFloatFromSourceRawBytes(ref byteCursor); // BoxLength
                row.AddFloatFromSourceRawBytes(ref byteCursor); // BoxWidth
                row.AddFloatFromSourceRawBytes(ref byteCursor); // BoxHeight
                row.AddFloatFromSourceRawBytes(ref byteCursor); // BoxYaw

                // Attach the sort rows
                row.SortValue1 = ((DBCRow.DBCFieldInt32)row.AddedFields[1]).Value; // MapID
                row.SortValue2 = ((DBCRow.DBCFieldInt32)row.AddedFields[0]).Value; // AreaTriggerID

                // Purge raw data
                row.SourceRawBytes.Clear();
            }
        }
    }
}
