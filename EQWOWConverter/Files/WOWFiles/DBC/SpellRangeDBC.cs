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
    internal class SpellRangeDBC : DBCFile
    {
        private static int CUR_DBCID = Configuration.DBCID_SPELLRANGE_ID_START;
        private static readonly object SpellRangeDBCLock = new object();

        public void AddRow(int dbcID, float range)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(dbcID); // ID
            newRow.AddFloat(0); // RangeMin_1
            newRow.AddFloat(0); // RangeMin_2
            newRow.AddFloat(range); // RangeMax_1
            newRow.AddFloat(range); // RangeMax_2
            newRow.AddPackedFlags(0); // Flags
            string description = string.Concat("EQ Range ", range.ToString());
            newRow.AddStringLang(description); // DisplayName
            newRow.AddStringLang(description); // DisplayName_Short
            Rows.Add(newRow);
        }

        public static int GenerateDBCID()
        {
            lock (SpellRangeDBCLock)
            {
                int newDBCID = CUR_DBCID;
                CUR_DBCID++;
                return newDBCID;
            }
        }
    }
}
