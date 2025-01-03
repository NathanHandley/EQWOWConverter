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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EQWOWConverter.WOWFiles
{
    internal class MapDifficultyDBC : DBCFile
    {
        public void AddRow(int mapID, int mapDifficultyID)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt(mapDifficultyID);
            newRow.AddInt(mapID);
            newRow.AddInt(0); // Difficulty, not 100% sure what this should be
            newRow.AddStringLang(""); // Rejection Message
            newRow.AddInt(0); // Raid Duration
            newRow.AddInt(0); // Max Players
            newRow.AddString(""); // Difficulty String (?)
            Rows.Add(newRow);
        }
    }
}
