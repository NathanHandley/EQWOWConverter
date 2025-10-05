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
            newRow.AddInt32(id); // ID
            newRow.AddInt32(0); // DisplayRaceID
            newRow.AddInt32(0); // DisplaySexID
            newRow.AddInt32(0); // SkinID
            newRow.AddInt32(0); // FaceID
            newRow.AddInt32(0); // HairStyleID
            newRow.AddInt32(0); // HairColorID
            newRow.AddInt32(0); // FacialHairID
            newRow.AddInt32(0); // Helm ItemDisplayInfo.ID
            newRow.AddInt32(0); // Shoulder ItemDisplayInfo.ID
            newRow.AddInt32(0); // Shirt ItemDisplayInfo.ID
            newRow.AddInt32(0); // Cuirass ItemDisplayInfo.ID
            newRow.AddInt32(0); // Belt ItemDisplayInfo.ID
            newRow.AddInt32(0); // Legs ItemDisplayInfo.ID
            newRow.AddInt32(0); // Boots ItemDisplayInfo.ID
            newRow.AddInt32(0); // Wrist ItemDisplayInfo.ID
            newRow.AddInt32(0); // Gloves ItemDisplayInfo.ID
            newRow.AddInt32(0); // Tabard ItemDisplayInfo.ID
            newRow.AddInt32(0); // Cape ItemDisplayInfo.ID
            newRow.AddInt32(0); // CanEquipWeapons (0 = no, 1 = yes)
            newRow.AddString(string.Empty); // BakeName
            Rows.Add(newRow);
        }
    }
}
