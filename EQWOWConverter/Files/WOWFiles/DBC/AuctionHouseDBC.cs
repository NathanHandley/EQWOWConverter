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
    internal class AuctionHouseDBC : DBCFile
    {
        // Row IDs in the stock AuctionHouse.dbc
        public static readonly int HOUSE_ID_BLACKWATER = 7;

        // Field positions after OnPostLoadDataFromDisk fills the row
        private static readonly int FIELD_INDEX_ID = 0;
        private static readonly int FIELD_INDEX_DEPOSIT_PERCENT = 2;
        private static readonly int FIELD_INDEX_CUT_PERCENT = 3;

        public void SetDepositAndCutPercent(int houseID, int depositPercent, int cutPercent)
        {
            foreach (DBCRow row in Rows)
            {
                if (((DBCRow.DBCFieldInt32)row.AddedFields[FIELD_INDEX_ID]).Value != houseID)
                    continue;
                ((DBCRow.DBCFieldInt32)row.AddedFields[FIELD_INDEX_DEPOSIT_PERCENT]).Value = depositPercent;
                ((DBCRow.DBCFieldInt32)row.AddedFields[FIELD_INDEX_CUT_PERCENT]).Value = cutPercent;
                Logger.WriteDebug(string.Concat("Set AuctionHouse.dbc house ", houseID.ToString(), " deposit to ", depositPercent.ToString(), "% and cut to ", cutPercent.ToString(), "%"));
                return;
            }
            Logger.WriteError(string.Concat("Could not set deposit and cut for auction house ID ", houseID.ToString(), " as no row with that ID exists in AuctionHouse.dbc"));
        }

        protected override void OnPostLoadDataFromDisk()
        {
            // Convert any raw data rows to actual data rows (which should be all of them)
            foreach (DBCRow row in Rows)
            {
                // This shouldn't be possible, but control for it just in case
                if (row.SourceRawBytes.Count == 0)
                {
                    Logger.WriteError("AuctionHouseDBC had no source raw bytes when converting a row in OnPostLoadDataFromDisk");
                    continue;
                }

                // Fill every field
                int byteCursor = 0;
                row.AddIntFromSourceRawBytes(ref byteCursor); // ID
                row.AddIntFromSourceRawBytes(ref byteCursor); // FactionID
                row.AddIntFromSourceRawBytes(ref byteCursor); // DepositRate
                row.AddIntFromSourceRawBytes(ref byteCursor); // ConsignmentRate
                row.AddStringLangFromSourceRawBytes(ref byteCursor, StringBlock); // Name_Lang

                // Sort by ID
                row.SortValue1 = ((DBCRow.DBCFieldInt32)row.AddedFields[FIELD_INDEX_ID]).Value;

                // Purge raw data
                row.SourceRawBytes.Clear();
            }
        }
    }
}
