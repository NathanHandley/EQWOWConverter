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

using EQWOWConverter.Items;

namespace EQWOWConverter.WOWFiles
{
    internal class CharStartOutfitDBC : DBCFile
    {
        private static int CUR_ID = 1;

        public void AddRowsForSexes(byte raceID, byte classID, List<ItemTemplate> itemTemplates)
        {
            AddRow(raceID, classID, 0, itemTemplates);
            AddRow(raceID, classID, 1, itemTemplates);
        }

        protected void AddRow(byte raceID, byte classID, byte sexID, List<ItemTemplate> itemTemplates)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(CUR_ID); // ID
            newRow.AddByte(raceID); // RaceID
            newRow.AddByte(classID); // ClassID
            newRow.AddByte(sexID); // SexID
            newRow.AddByte(0); // OutfitID
            for (int i = 0; i < 24; i++) // ItemID x 24
            {
                if (itemTemplates.Count <= i)
                    newRow.AddInt32(-1);
                else
                    newRow.AddInt32(itemTemplates[i].WOWEntryID);
            }
            for (int i = 0; i < 24; i++) // DisplayItemID x 24
            {
                if (itemTemplates.Count <= i)
                    newRow.AddInt32(-1);
                else
                {
                    ItemDisplayInfo? itemDisplayInfo = itemTemplates[i].ItemDisplayInfo;
                    if (itemDisplayInfo != null)
                        newRow.AddInt32(itemDisplayInfo.ItemDisplayInfoDBCID); // DisplayItemID
                    else
                        newRow.AddInt32(-1); // DisplayItemID
                }
            }
            for (int i = 0; i < 24; i++) // InventoryType x 24
            {
                if (itemTemplates.Count <= i)
                    newRow.AddInt32(-1);
                else
                    newRow.AddInt32(Convert.ToInt32(itemTemplates[i].InventoryType)); // InventoryType
            }

            // Sort
            newRow.SortValue1 = classID;
            newRow.SortValue2 = raceID;
            newRow.SortValue3 = sexID;
            Rows.Add(newRow);

            CUR_ID++;
        }

        protected override void OnPostLoadDataFromDisk()
        {
            // Clear all of the old rows
            Rows.Clear();
        }
    }
}
