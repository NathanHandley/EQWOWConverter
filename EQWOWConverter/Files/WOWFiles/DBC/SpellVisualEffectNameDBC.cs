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
    internal class SpellVisualEffectNameDBC : DBCFile
    {
        public void AddRow(int id, string name, string relativeFileName)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(id); // ID
            newRow.AddString(name); // Name (not actually used anywhere)
            newRow.AddString(relativeFileName); // FileName (example: "spells\holyzone.mdx")
            newRow.AddFloat(1); // AreaEffectSize (mostly 0 or 1, but there are some other values like 2 and 4)
            newRow.AddFloat(1); // Scale (lots of 1)
            newRow.AddFloat(0.01f); // MinAllowedScale (Lots of 0.01)
            newRow.AddFloat(100); // MaxAllowedScale (mostly 100, but see some 10)
            Rows.Add(newRow);
        }
    }
}
