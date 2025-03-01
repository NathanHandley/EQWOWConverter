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
    internal class TaxiPathDBC : DBCFile
    {
        private static int CUR_ID = Configuration.DBCID_TAXIPATH_ID_START;

        public void AddRow(int id)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(id); // ID
            newRow.AddInt32(0); // FromTaxiNode
            newRow.AddInt32(0); // ToTaxiNode
            newRow.AddInt32(0); // Cost
            Rows.Add(newRow);
        }

        public static int GenerateID()
        {
            int id = CUR_ID;
            CUR_ID++;
            return id;
        }
    }
}
