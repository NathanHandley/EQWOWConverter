//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2026 Nathan Handley
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
    internal class LockDBC : DBCFile
    {
        public void AddRowForItemKeys(int lockID, int keyItemWOWID, int altKeyItemWOWID)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(lockID); // ID
            newRow.AddInt32(1); // Type_1 (1 = requires item)
            newRow.AddInt32(altKeyItemWOWID > 0 ? 1 : 0); // Type_2
            newRow.AddInt32(0); // Type_3
            newRow.AddInt32(0); // Type_4
            newRow.AddInt32(0); // Type_5
            newRow.AddInt32(0); // Type_6
            newRow.AddInt32(0); // Type_7
            newRow.AddInt32(0); // Type_8
            newRow.AddInt32(keyItemWOWID); // Index_1 (the key item)
            newRow.AddInt32(altKeyItemWOWID > 0 ? altKeyItemWOWID : 0); // Index_2 (the alternate key item)
            newRow.AddInt32(0); // Index_3
            newRow.AddInt32(0); // Index_4
            newRow.AddInt32(0); // Index_5
            newRow.AddInt32(0); // Index_6
            newRow.AddInt32(0); // Index_7
            newRow.AddInt32(0); // Index_8
            newRow.AddInt32(0); // Skill_1
            newRow.AddInt32(0); // Skill_2
            newRow.AddInt32(0); // Skill_3
            newRow.AddInt32(0); // Skill_4
            newRow.AddInt32(0); // Skill_5
            newRow.AddInt32(0); // Skill_6
            newRow.AddInt32(0); // Skill_7
            newRow.AddInt32(0); // Skill_8
            newRow.AddInt32(1); // Action_1 (1 = open, which the game's key-locked doors all use on their key slots)
            newRow.AddInt32(altKeyItemWOWID > 0 ? 1 : 0); // Action_2
            newRow.AddInt32(0); // Action_3
            newRow.AddInt32(0); // Action_4
            newRow.AddInt32(0); // Action_5
            newRow.AddInt32(0); // Action_6
            newRow.AddInt32(0); // Action_7
            newRow.AddInt32(0); // Action_8
            Rows.Add(newRow);
        }
    }
}
