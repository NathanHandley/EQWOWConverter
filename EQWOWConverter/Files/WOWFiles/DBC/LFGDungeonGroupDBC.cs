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
    internal class LFGDungeonGroupDBC : DBCFile
    {
        public void AddRow(int id, string name, int orderIndex, bool isRaidType)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(id); // ID
            newRow.AddStringLang(name); // Name
            newRow.AddInt32(orderIndex); // Order_Index
            newRow.AddInt32(0); // Parent_Group_Id
            newRow.AddInt32(isRaidType == true ? 2 : 1); // Typeid (1 = normal, 2 = raid, 5 = heroic)

            // Sort by ID
            newRow.SortValue1 = id;

            Rows.Add(newRow);
        }
    }
}
