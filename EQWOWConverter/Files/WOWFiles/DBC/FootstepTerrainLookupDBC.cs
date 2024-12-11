//  Author: Nathan Handley (nathanhandley@protonmail.com)
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

using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EQWOWConverter.WOWFiles
{
    internal class FootstepTerrainLookupDBC : DBCFile
    {
        public static int CURRENT_ID = Configuration.CONFIG_DBCID_FOOTSTEPTERRAINLOOKUP_ID_START;

        public void AddRow(int creatureFootstepID, int soundID)
        {
            // 10 rows are created for every creaturefootstep record to cover values within TerrainType.dbc
            for (int terrainTypeSoundID = 0; terrainTypeSoundID < 10; terrainTypeSoundID++)
            {
                int curID = CURRENT_ID;
                CURRENT_ID++;
                DBCRow newRow = new DBCRow();
                newRow.AddInt(curID); // ID
                newRow.AddInt(creatureFootstepID);
                newRow.AddInt(terrainTypeSoundID); // TerrainType.ID
                newRow.AddInt(soundID);
                newRow.AddInt(0); // SoundIDSplash
                Rows.Add(newRow);
            }
        }
    }
}
