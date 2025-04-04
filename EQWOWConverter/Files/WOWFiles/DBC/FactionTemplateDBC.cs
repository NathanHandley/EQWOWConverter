﻿//  Author: Nathan Handley (nathanhandley@protonmail.com)
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
            // Determine flags
            int flags = 1; // 0x0001 - Respond to calls for help
            flags |= 32; // 0x0020 - Search for enemies (med priority)

            // Fill the row
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(creatureFaction.FactionTemplateID); // ID
            newRow.AddInt32(creatureFaction.FactionID); // Faction.ID
            newRow.AddInt32(flags); // Flags
            newRow.AddInt32(8); // FactionGroup.ID (lots of 0, 1, 8)
            newRow.AddInt32(0); // FriendGroup (bitmask field)
            if (creatureFaction.ForceAgro == false && creatureFaction.ReputationIndex < 0)
                newRow.AddInt32(0); // EnemyGroup (bitmask field)
            else
                newRow.AddInt32(1); // EnemyGroup (bitmask field) - 1 = All players (and pets)
            newRow.AddInt32(creatureFaction.EnemyFaction1); // Enemies 1
            newRow.AddInt32(creatureFaction.EnemyFaction2); // Enemies 2
            newRow.AddInt32(creatureFaction.EnemyFaction3); // Enemies 3
            newRow.AddInt32(creatureFaction.EnemyFaction4); // Enemies 4
            newRow.AddInt32(creatureFaction.FactionID); // Friend 1 (help self)
            newRow.AddInt32(0); // Friend 2
            newRow.AddInt32(0); // Friend 3
            newRow.AddInt32(0); // Friend 4
            Rows.Add(newRow);
        }
    }
}
