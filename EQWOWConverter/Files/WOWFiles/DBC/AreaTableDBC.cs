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

using EQWOWConverter.Zones;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using System.Xml.Linq;

namespace EQWOWConverter.WOWFiles
{
    internal class AreaTableDBC : DBCFile
    {
        private static int CURRENT_AREABIT = Configuration.CONFIG_DBCID_AREATABLE_AREABIT_BLOCK_1_START;

        public void AddRow(int id, int mapID, int parentAreaID, ZoneAreaMusic? areaMusic, ZoneAreaAmbientSound? areaSound, string areaName)
        {
            // AreaBit must always be unique
            int areaBit = CURRENT_AREABIT;
            CURRENT_AREABIT++;
            if (CURRENT_AREABIT == Configuration.CONFIG_DBCID_AREATABLE_AREABIT_BLOCK_1_END + 1)
                CURRENT_AREABIT = Configuration.CONFIG_DBCID_AREATABLE_AREABIT_BLOCK_2_START;
            else if (CURRENT_AREABIT == Configuration.CONFIG_DBCID_AREATABLE_AREABIT_BLOCK_2_END + 1)
                CURRENT_AREABIT = Configuration.CONFIG_DBCID_AREATABLE_AREABIT_BLOCK_3_START;
            else if (CURRENT_AREABIT > Configuration.CONFIG_DBCID_AREATABLE_AREABIT_BLOCK_3_END)
                Logger.WriteError("Areabit is too high, as it is over '" + Configuration.CONFIG_DBCID_AREATABLE_AREABIT_BLOCK_3_END + "'");

            // Music
            int zoneMusicID = 0;
            if (areaMusic != null)
                zoneMusicID = areaMusic.DBCID;

            // Ambience
            int ambienceID = 0;
            if (areaSound != null)
                ambienceID = areaSound.DBCID;

            DBCRow newRow = new DBCRow();
            newRow.AddInt32(id);
            newRow.AddInt32(mapID); // Continent ID
            newRow.AddInt32(parentAreaID);
            newRow.AddInt32(areaBit);
            newRow.AddPackedFlags(0); // Flags
            newRow.AddInt32(0); // Sound Provider Preferences
            newRow.AddInt32(0); // Sound Provider Preferences - Underwater
            newRow.AddInt32(ambienceID);
            newRow.AddInt32(zoneMusicID);
            newRow.AddInt32(0); // IntroSound
            newRow.AddInt32(0); // Exploration Level
            newRow.AddStringLang(areaName);
            newRow.AddInt32(0); // Faction Group Mask
            newRow.AddInt32(0); // LiquidTypeID 1
            newRow.AddInt32(0); // LiquidTypeID 2
            newRow.AddInt32(0); // LiquidTypeID 3
            newRow.AddInt32(0); // LiquidTypeID 4
            newRow.AddFloat(-5000f); // Minimum Elevation
            newRow.AddFloat(0); // Ambient Multiplier
            newRow.AddInt32(0); // LightID
            Rows.Add(newRow);
        }
    }
}
