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
    internal class FactionDBC : DBCFile
    {
        public static int CURRENT_ID = Configuration.CONFIG_DBCID_FACTION_ID_START;

        public void AddRow()
        {
            int curID = CURRENT_ID;
            CURRENT_ID++;
            DBCRow newRow = new DBCRow();
            newRow.AddInt(0); // ID
            newRow.AddInt(0); // ReputationIndex (Must be a unique number, max 127, no gain -1)
            newRow.AddInt(0); // Reputation Race Mask 1
            newRow.AddInt(0); // Reputation Race Mask 2
            newRow.AddInt(0); // Reputation Race Mask 3
            newRow.AddInt(0); // Reputation Race Mask 4
            newRow.AddInt(0); // Reputation Class Mask 1
            newRow.AddInt(0); // Reputation Class Mask 2
            newRow.AddInt(0); // Reputation Class Mask 3
            newRow.AddInt(0); // Reputation Class Mask 4
            newRow.AddInt(0); // Reputation Base 1 (Used by Race/Class Mask 1)
            newRow.AddInt(0); // Reputation Base 2 (Used by Race/Class Mask 2)
            newRow.AddInt(0); // Reputation Base 3 (Used by Race/Class Mask 3)
            newRow.AddInt(0); // Reputation Base 4 (Used by Race/Class Mask 4)
            newRow.AddInt(0); // Reputation Flags 1 (similar to above?)
            newRow.AddInt(0); // Reputation Flags 2
            newRow.AddInt(0); // Reputation Flags 3
            newRow.AddInt(0); // Reputation Flags 4
            newRow.AddInt(0); // Parent Faction ID (Faction.ID)
            newRow.AddFloat(0); // ParentFactionMod 1
            newRow.AddFloat(0); // ParentFactionMod 2
            newRow.AddInt(0); // Parent Faction Cap 1
            newRow.AddInt(0); // Parent Faction Cap 2
            newRow.AddStringLang(string.Empty); // Name
            newRow.AddStringLang(string.Empty); // Description
            Rows.Add(newRow);
        }
    }
}
