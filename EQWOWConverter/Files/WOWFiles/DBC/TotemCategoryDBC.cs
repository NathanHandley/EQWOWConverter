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
    internal class TotemCategoryDBC : DBCFile
    {
        private static UInt32 CUR_ID = Configuration.DBCID_TOTEMCATEGORY_ID_START;

        public void AddRow(UInt32 id, string name, int categoryID, int categoryMask)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddUInt32(id); // ID;
            newRow.AddStringLang(name); // Name_Lang
            newRow.AddInt32(categoryID); // TotemCategoryType
            newRow.AddInt32(categoryMask); // TotemCategoryMask
            Rows.Add(newRow);
        }

        public static UInt32 GetUniqueID()
        {
            UInt32 newID = CUR_ID;
            CUR_ID++;
            return newID;
        }
    }
}
