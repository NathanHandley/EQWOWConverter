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

using EQWOWConverter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Zones
{
    internal class ZoneArea
    {
        private static UInt32 CURRENT_AREATABLEID = Configuration.CONFIG_DBCID_AREATABLE_START;

        public UInt32 DBCAreaTableID;
        public string DisplayName = string.Empty;
        public BoundingBox BoundingBox = new BoundingBox();
        public string MusicFileNameNoExtDay = string.Empty;
        public string MusicFileNameNoExtNight = string.Empty;
        public ZoneAreaMusic? AreaMusic = null;

        public ZoneArea(string displayName, BoundingBox boundingBox, string musicFileNameDay, string musicFileNameNight)
        {
            DBCAreaTableID = CURRENT_AREATABLEID;
            CURRENT_AREATABLEID++;
            DisplayName = displayName;
            MusicFileNameNoExtDay = musicFileNameDay;
            MusicFileNameNoExtNight = musicFileNameNight;

            // Scale and rotate
            BoundingBox.TopCorner.X = boundingBox.BottomCorner.X * -Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            BoundingBox.TopCorner.Y = boundingBox.BottomCorner.Y * -Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            BoundingBox.TopCorner.Z = boundingBox.TopCorner.Z * Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            BoundingBox.BottomCorner.X = boundingBox.TopCorner.X * -Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            BoundingBox.BottomCorner.Y = boundingBox.TopCorner.Y * -Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            BoundingBox.BottomCorner.Z = boundingBox.BottomCorner.Z * Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
        }
    }
}
