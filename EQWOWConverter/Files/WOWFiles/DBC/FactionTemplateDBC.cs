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

using EQWOWConverter.Creatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EQWOWConverter.WOWFiles
{
    internal class FactionTemplateDBC : DBCFile
    {
        public void AddRow(CreatureFaction creatureFaction)
        {
            // Fill the row
            DBCRow newRow = new DBCRow();
            newRow.AddInt(creatureFaction.FactionTemplateID); // ID
            newRow.AddInt(creatureFaction.FactionID); // Faction.ID
            newRow.AddInt(33); // Flags (Copied from Netherwing, revisit)
            newRow.AddInt(0); // FactionGroup.ID (lots of 0, 1, 8)
            newRow.AddInt(0); // FriendGroup (bitmask field)
            newRow.AddInt(0); // EnemyGroup (bitmask field)
            newRow.AddInt(0); // Enemies 1
            newRow.AddInt(0); // Enemies 2
            newRow.AddInt(0); // Enemies 3
            newRow.AddInt(0); // Enemies 4
            newRow.AddInt(creatureFaction.FactionID); // Friend 1 (help self)
            newRow.AddInt(0); // Friend 2
            newRow.AddInt(0); // Friend 3
            newRow.AddInt(0); // Friend 4
            Rows.Add(newRow);
        }
    }
}
