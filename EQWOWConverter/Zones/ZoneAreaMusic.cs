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

using EQWOWConverter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Zones
{
    internal class ZoneAreaMusic
    {
        private static int CURRENT_ZONEMUSICSTARTID = Configuration.DBCID_ZONEMUSIC_START;

        public int DBCID;
        public string Name;
        public string FileNameNoExtDay = string.Empty;
        public string FileNameNoExtNight = string.Empty;
        public Sound? DaySound;
        public Sound? NightSound;

        public ZoneAreaMusic(string zoneDBCName, Sound? daySound, Sound? nightSound, string fileNameNoExtDay, string fileNameNoExtNight)
        {
            DBCID = CURRENT_ZONEMUSICSTARTID;
            CURRENT_ZONEMUSICSTARTID++;
            Name = zoneDBCName;
            DaySound = daySound;
            NightSound = nightSound;
            FileNameNoExtDay = fileNameNoExtDay;
            FileNameNoExtNight = fileNameNoExtNight;
        }
    }
}
