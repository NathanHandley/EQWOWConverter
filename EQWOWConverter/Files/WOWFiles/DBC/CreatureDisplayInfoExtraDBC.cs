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
    internal class CreatureDisplayInfoExtraDBC : DBCFile
    {
        public void AddRow(int id)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(id); // ID" Type = "int" IsIndex = "true" />
            newRow.AddInt32(0); // DisplayRaceID" Type = "int" />
            newRow.AddInt32(0); // DisplaySexID" Type = "int" />
            newRow.AddInt32(0); // SkinID" Type = "int" />
            newRow.AddInt32(0); // FaceID" Type = "int" />
            newRow.AddInt32(0); // HairStyleID" Type = "int" />
            newRow.AddInt32(0); // HairColorID" Type = "int" />
            newRow.AddInt32(0); // FacialHairID" Type = "int" />
            newRow.AddInt32(0); // NPCItemDisplay" Type = "int" ArraySize = "11" />
            newRow.AddInt32(0); // NPCItemDisplay" Type = "int" ArraySize = "11" />
            newRow.AddInt32(0); // NPCItemDisplay" Type = "int" ArraySize = "11" />
            newRow.AddInt32(0); // NPCItemDisplay" Type = "int" ArraySize = "11" />
            newRow.AddInt32(0); // NPCItemDisplay" Type = "int" ArraySize = "11" />
            newRow.AddInt32(0); // NPCItemDisplay" Type = "int" ArraySize = "11" />
            newRow.AddInt32(0); // NPCItemDisplay" Type = "int" ArraySize = "11" />
            newRow.AddInt32(0); // NPCItemDisplay" Type = "int" ArraySize = "11" />
            newRow.AddInt32(0); // NPCItemDisplay" Type = "int" ArraySize = "11" />
            newRow.AddInt32(0); // NPCItemDisplay" Type = "int" ArraySize = "11" />
            newRow.AddInt32(0); // NPCItemDisplay" Type = "int" ArraySize = "11" />
            newRow.AddInt32(0); // Flags" Type = "int" />
            newRow.AddString(string.Empty); // BakeName" Type = "string" />
            Rows.Add(newRow);
        }
    }
}
