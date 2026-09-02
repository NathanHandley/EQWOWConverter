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
    internal class SpellCastTimesDBC : DBCFile
    {
        public void AddRow(int dbcID, int castTime)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(dbcID); // ID
            newRow.AddInt32(castTime); // Base Casting Time in milliseconds
            newRow.AddInt32(0); // Amount of milliseconds deducted per spell level
            newRow.AddInt32(0); // Minimum cast time in milliseconds
            Rows.Add(newRow);
        }

        public Dictionary<int, int> GetBaseCastTimeInMSByDBCID()
        {
            Dictionary<int, int> baseCastTimeInMSByDBCID = new Dictionary<int, int>();
            foreach (DBCRow row in Rows)
            {
                if (row.SourceRawBytes.Count >= 8)
                {
                    int dbcID = row.SourceRawBytes[0] | (row.SourceRawBytes[1] << 8) | (row.SourceRawBytes[2] << 16) | (row.SourceRawBytes[3] << 24);
                    int castTimeInMS = row.SourceRawBytes[4] | (row.SourceRawBytes[5] << 8) | (row.SourceRawBytes[6] << 16) | (row.SourceRawBytes[7] << 24);
                    baseCastTimeInMSByDBCID[dbcID] = castTimeInMS;
                }
                else if (row.AddedFields.Count >= 2 && row.AddedFields[0] is DBCRow.DBCFieldInt32 idField && row.AddedFields[1] is DBCRow.DBCFieldInt32 castTimeField)
                    baseCastTimeInMSByDBCID[idField.Value] = castTimeField.Value;
            }
            return baseCastTimeInMSByDBCID;
        }
    }
}
