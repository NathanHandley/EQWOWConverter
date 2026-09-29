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
    internal class SpellIconDBC : DBCFile
    {
        public static readonly int SPELL_ICON_COUNT = 23;
        public static readonly int ITEM_ICON_COUNT = 751;

        // spelgems.png gem grid is 11 columns by 12 rows, starting at memicon 2005.  Each column is one spell icon, and rows cycle through the 6 backdrop colors (purple, blue, green, yellow, red, orange)
        private static readonly int MEMICON_GRID_START = 2005;
        private static readonly int MEMICON_GRID_COLUMNS = 11;
        private static readonly int MEMICON_GRID_ROWS = 12;
        private static readonly int[] MEMICON_SPELL_ICON_IDS_BY_COLUMN_UPPER_ROWS = { 0, 5, 10, 15, 19, 1, 6, 11, 16, 20, 4 };
        private static readonly int[] MEMICON_SPELL_ICON_IDS_BY_COLUMN_LOWER_ROWS = { 2, 7, 12, 17, 3, 8, 13, 21, 22, 18, 9 };

        public void AddSpellIconRow(int iconID)
        {
            int dbcID = GetDBCIDForSpellIconID(iconID);
            string textureFileName = "INTERFACE\\ICONS\\Spell_EQ_" + iconID.ToString();

            DBCRow newRow = new DBCRow();
            newRow.AddInt32(dbcID);
            newRow.AddString(textureFileName);
            Rows.Add(newRow);
        }

        public void AddSpellGemIconRow(int iconID, int backdropIndex)
        {
            int dbcID = GetDBCIDForSpellGemIconID(iconID, backdropIndex);
            string textureFileName = "INTERFACE\\ICONS\\" + ImageTool.GetSpellGemIconFileNameNoExt(iconID, backdropIndex);

            DBCRow newRow = new DBCRow();
            newRow.AddInt32(dbcID);
            newRow.AddString(textureFileName);
            Rows.Add(newRow);
        }

        public void AddItemIconRow(int iconID)
        {
            int dbcID = GetDBCIDForItemIconID(iconID);
            string textureFileName = "INTERFACE\\ICONS\\INV_EQ_" + iconID.ToString();

            DBCRow newRow = new DBCRow();
            newRow.AddInt32(dbcID);
            newRow.AddString(textureFileName);
            Rows.Add(newRow);
        }

        public static int GetDBCIDForSpellIconID(int iconID)
        {
            return Configuration.DBCID_SPELLICON_ID_START + iconID;
        }

        public static int GetDBCIDForItemIconID(int iconID)
        {
            // Item icons come after the spell icons
            return Configuration.DBCID_SPELLICON_ID_START + SPELL_ICON_COUNT + iconID;
        }

        public static int GetDBCIDForSpellGemIconID(int iconID, int backdropIndex)
        {
            // Spell gem icons come after the item icons
            return Configuration.DBCID_SPELLICON_ID_START + SPELL_ICON_COUNT + ITEM_ICON_COUNT + (iconID * ImageTool.SPELL_GEM_BACKDROP_COUNT) + backdropIndex;
        }

        // Returns false if the memicon doesn't map to a spell gem
        public static bool TryGetSpellGemForMemIconID(int memIconID, out int iconID, out int backdropIndex)
        {
            iconID = 0;
            backdropIndex = 0;
            int gridIndex = memIconID - MEMICON_GRID_START;
            if (gridIndex < 0 || gridIndex >= MEMICON_GRID_COLUMNS * MEMICON_GRID_ROWS)
                return false;
            int row = gridIndex / MEMICON_GRID_COLUMNS;
            int column = gridIndex % MEMICON_GRID_COLUMNS;
            if (row < ImageTool.SPELL_GEM_BACKDROP_COUNT)
                iconID = MEMICON_SPELL_ICON_IDS_BY_COLUMN_UPPER_ROWS[column];
            else
                iconID = MEMICON_SPELL_ICON_IDS_BY_COLUMN_LOWER_ROWS[column];
            backdropIndex = row % ImageTool.SPELL_GEM_BACKDROP_COUNT;
            return true;
        }
    }
}
