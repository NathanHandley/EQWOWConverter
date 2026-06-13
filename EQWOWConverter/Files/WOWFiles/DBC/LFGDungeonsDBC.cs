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
    }
}
