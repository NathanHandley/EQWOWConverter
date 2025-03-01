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
    internal class TransportsSQL : SQLFile
    {
        private static int CUR_GUID = Configuration.SQL_TRANSPORTS_GUID_START;

        public override string DeleteRowSQL()
        {
            return "DELETE FROM transports WHERE guid >= " + Configuration.SQL_TRANSPORTS_GUID_START.ToString() + " AND guid <= " + Configuration.SQL_TRANSPORTS_GUID_END.ToString() + ";";
        }

        public void AddRow(int entry, string name)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("GUID", CUR_GUID);
            newRow.AddInt("Entry", entry);
            newRow.AddString("Name", name);
            newRow.AddString("ScriptName", 64, string.Empty);
            Rows.Add(newRow);
            CUR_GUID++;
        }
    }
}
