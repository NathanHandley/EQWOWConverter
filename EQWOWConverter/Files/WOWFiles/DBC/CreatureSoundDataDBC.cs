﻿//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2024 Nathan Handley
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

using EQWOWConverter.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal class CreatureSoundDataDBC : DBCFile
    {
        public void AddRow(int id, int loopSoundID)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt(id); // ID
            newRow.AddInt(0); // Exertion SoundEntriesDBC.ID
            newRow.AddInt(0); // Exertion Critical SoundEntriesDBC.ID
            newRow.AddInt(0); // Injury SoundEntriesDBC.ID
            newRow.AddInt(0); // Injury Critical SoundEntriesDBC.ID
            newRow.AddInt(0); // Injury Crushing Blow SoundEntriesDBC.ID
            newRow.AddInt(0); // Death SoundEntriesDBC.ID
            newRow.AddInt(0); // Stun SoundEntriesDBC.ID
            newRow.AddInt(0); // Stand SoundEntriesDBC.ID
            newRow.AddInt(0); // Footstep sound in FootstepTerrainLookupDBC.ID
            newRow.AddInt(0); // Agro SoundEntriesDBC.ID
            newRow.AddInt(0); // Wing Flap SoundEntriesDBC.ID
            newRow.AddInt(0); // Wing Glide SoundEntriesDBC.ID
            newRow.AddInt(0); // Alert SoundEntriesDBC.ID
            newRow.AddInt(0); // Fidget 1 SoundEntriesDBC.ID
            newRow.AddInt(0); // Fidget 2 SoundEntriesDBC.ID
            newRow.AddInt(0); // Fidget 3 SoundEntriesDBC.ID
            newRow.AddInt(0); // Fidget 4 SoundEntriesDBC.ID
            newRow.AddInt(0); // Fidget 5 SoundEntriesDBC.ID
            newRow.AddInt(0); // Custom Attack 1 SoundEntriesDBC.ID
            newRow.AddInt(0); // Custom Attack 2 SoundEntriesDBC.ID
            newRow.AddInt(0); // Custom Attack 3 SoundEntriesDBC.ID
            newRow.AddInt(0); // Custom Attack 4 SoundEntriesDBC.ID
            newRow.AddInt(0); // NPC Sonud SoundEntriesDBC.ID
            newRow.AddInt(loopSoundID); // Loop Sound (idle?) SoundEntriesDBC.ID
            newRow.AddInt(0); // Creature impact type
            newRow.AddInt(0); // Jump Start ID SoundEntriesDBC.ID
            newRow.AddInt(0); // Jump End ID SoundEntriesDBC.ID
            newRow.AddInt(0); // Pet Attack ID SoundEntriesDBC.ID
            newRow.AddInt(0); // Pet Order ID SoundEntriesDBC.ID
            newRow.AddInt(0); // Pet Dismiss ID SoundEntriesDBC.ID
            newRow.AddFloat(0); // Fidget Delay Seconds Min (Time / Interval? 30 seconds?)
            newRow.AddFloat(0); // Fidget Delay Seconds Max (Time / Interval? 60 seconds?)
            newRow.AddInt(0); // Birth SoundEntriesDBC.ID
            newRow.AddInt(0); // Spell Cast Directed SoundEntriesDBC.ID
            newRow.AddInt(0); // Submerge SoundEntriesDBC.ID
            newRow.AddInt(0); // Submerged SoundEntriesDBC.ID
            newRow.AddInt(0); // Creature Sound Data ID Pet (?) SoundEntriesDBC.ID
            Rows.Add(newRow);
        }
    }
}