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

namespace EQWOWConverter.WOWFiles
{
    internal class WMOAreaTableDBC : DBCFile
    {
        private static int CURRENT_ID = Configuration.CONFIG_DBCID_WMOAREATABLE_ID_START;

        public void AddRow(int wmoID, int wmoGroupID, int zoneMusic, int areaTableID, string areaName)
        {
            // Generate a new ID
            int ID = CURRENT_ID;
            CURRENT_ID++;

            DBCRow newRow = new DBCRow();
            newRow.AddInt32(ID);
            newRow.AddInt32(wmoID);
            newRow.AddInt32(0); // NameSetID
            newRow.AddInt32(wmoGroupID);
            newRow.AddInt32(0); // SoundProviderPref
            newRow.AddInt32(0); // SoundProviderPref - Underwater
            newRow.AddInt32(0); // AmbienceID
            newRow.AddInt32(zoneMusic);
            newRow.AddInt32(0); // Intro Sound
            newRow.AddPackedFlags(0); // Flags
            newRow.AddInt32(areaTableID);
            newRow.AddStringLang(areaName);            
            Rows.Add(newRow);
        }      
    }
}
