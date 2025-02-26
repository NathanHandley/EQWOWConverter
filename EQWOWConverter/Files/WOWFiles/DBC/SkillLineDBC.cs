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

using Google.Protobuf.WellKnownTypes;
using System.Xml.Linq;
using System;

namespace EQWOWConverter.WOWFiles
{
    internal class SkillLineDBC : DBCFile
    {
        public void AddRow(int id, string name)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(id); // ID
            newRow.AddInt32(7); // CategoryID (7 = class)
            newRow.AddInt32(0); // SkillCostsID
            newRow.AddStringLang(name); // DisplayName
            newRow.AddStringLang(""); // Description
            newRow.AddInt32(1); // SpellIconID
            newRow.AddStringLang(""); // AlternateVerb
            newRow.AddInt32(0); // CanLink
            Rows.Add(newRow);
        }
    }
}
