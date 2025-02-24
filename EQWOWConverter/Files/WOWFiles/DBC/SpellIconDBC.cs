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
        public void AddRow(int iconID)
        {
            int dbcID = GetDBCIDForIconID(iconID);
            string textureFileName = "INTERFACE\\ICONS\\Spell_EQ_" + iconID.ToString();

            DBCRow newRow = new DBCRow();
            newRow.AddInt32(dbcID);
            newRow.AddString(textureFileName);
            Rows.Add(newRow);
        }

        public static int GetDBCIDForIconID(int iconID)
        {
            return Configuration.CONFIG_DBCID_SPELLICON_ID_START + iconID;
        }
    }
}
