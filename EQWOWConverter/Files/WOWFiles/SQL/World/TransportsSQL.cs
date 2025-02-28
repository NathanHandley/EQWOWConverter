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
        public override string DeleteRowSQL()
        {
            return "";
        }

        public void AddRow()
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("GUID", 0);
            newRow.AddInt("Entry", 0);
            newRow.AddString("Name", string.Empty);
            newRow.AddString("ScriptName", 64, string.Empty);
            Rows.Add(newRow);
        }
    }
}
