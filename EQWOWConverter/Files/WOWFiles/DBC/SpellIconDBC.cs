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
        public void AddSpellIconRow(int iconID)
        {
            int dbcID = GetDBCIDForSpellIconID(iconID);
            string textureFileName = "INTERFACE\\ICONS\\Spell_EQ_" + iconID.ToString();

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
            // There are 23 spell icons, so shift accordingly
            return Configuration.DBCID_SPELLICON_ID_START + 23 + iconID;
        }
    }
}
