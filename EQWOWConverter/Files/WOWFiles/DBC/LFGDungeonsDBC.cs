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
    internal class LFGDungeonsDBC : DBCFile
    {
        private static int CUR_ID = Configuration.DBCID_LFGDUNGEONS_ID_START;

        public void AddRow(string name, int minLevel, int targetLevel, int targetLevelMin, int targetLevelMax, int mapID, bool isRaid, int groupID)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(CUR_ID); // ID
            newRow.AddStringLang(name); // Name
            newRow.AddInt32(minLevel); // MinLevel
            newRow.AddInt32(100); // MaxLevel
            newRow.AddInt32(targetLevel); // Target_Level
            newRow.AddInt32(targetLevelMin); // Target_Level_Min
            newRow.AddInt32(targetLevelMax); // Target_Level_Max
            newRow.AddInt32(mapID); // MapID
            newRow.AddInt32(0); // Difficulty
            newRow.AddInt32(isRaid == true ? 0 : 3); // Flags (I think 3 is cross realm?)
            newRow.AddInt32(isRaid == true ? 2 : 1); // TypeID  (2 = raid, 1 = dungeon)
            newRow.AddInt32(-1); // Faction
            newRow.AddString(""); //TextureFilename
            newRow.AddInt32(0); // ExpansionLevel
            newRow.AddInt32(0); // Order_Index
            newRow.AddInt32(groupID); // Group_Id
            newRow.AddStringLang(""); // Description_Lang

            // Sort by ID
            newRow.SortValue1 = CUR_ID;

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
                    Logger.WriteError("LFGDungeonGroupDBC had no source raw bytes when converting a row in OnPostLoadDataFromDisk");
                    continue;
                }

                // Fill every field
                int byteCursor = 0;
                row.AddIntFromSourceRawBytes(ref byteCursor); // ID
                row.AddStringLangFromSourceRawBytes(ref byteCursor, StringBlock); // Name
                row.AddIntFromSourceRawBytes(ref byteCursor); // MinLevel
                row.AddIntFromSourceRawBytes(ref byteCursor); // MaxLevel
                row.AddIntFromSourceRawBytes(ref byteCursor); // Target_Level
                row.AddIntFromSourceRawBytes(ref byteCursor); // Target_Level_Min
                row.AddIntFromSourceRawBytes(ref byteCursor); // Target_Level_Max
                row.AddIntFromSourceRawBytes(ref byteCursor); // MapID
                row.AddIntFromSourceRawBytes(ref byteCursor); // Difficulty
                row.AddIntFromSourceRawBytes(ref byteCursor); // Flags (I think 3 is cross realm?)
                row.AddIntFromSourceRawBytes(ref byteCursor); // TypeID  (2 = raid, 1 = dungeon)
                row.AddIntFromSourceRawBytes(ref byteCursor); // Faction
                row.AddStringFromSourceRawBytes(ref byteCursor, StringBlock); //TextureFilename
                row.AddIntFromSourceRawBytes(ref byteCursor); // ExpansionLevel
                row.AddIntFromSourceRawBytes(ref byteCursor); // Order_Index
                row.AddIntFromSourceRawBytes(ref byteCursor); // Group_Id
                row.AddStringLangFromSourceRawBytes(ref byteCursor, StringBlock); // Description_Lang

                // Sort by ID
                row.SortValue1 = ((DBCRow.DBCFieldInt32)row.AddedFields[0]).Value;

                // Purge raw data
                row.SourceRawBytes.Clear();
            }
        }
    }
}
