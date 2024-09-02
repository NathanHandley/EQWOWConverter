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
        private static UInt32 CURRENT_AREATABLEID = Configuration.CONFIG_DBCID_AREATABLE_ID_START;

        public UInt32 DBCAreaTableID;
        public string DisplayName = string.Empty;
        public List<BoundingBox> BoundingBoxes = new List<BoundingBox>();
        public string MusicFileNameNoExtDay = string.Empty;
        public string MusicFileNameNoExtNight = string.Empty;
        public float MusicVolume;
        public ZoneAreaMusic? AreaMusic = null;        
        public string AmbientSoundFileNameNoExtDay = string.Empty;
        public string AmbientSoundFileNameNoExtNight = string.Empty;
        public float AmbientSoundVolume;
        public ZoneAreaAmbientSound? AreaAmbientSound = null;

        public ZoneArea(string displayName, BoundingBox boundingBox, string musicFileNameNoExtDay, string musicFileNameNoExtNight,
            float musicVolume, string ambientSoundFileNameNoExtDay, string ambientSoundFileNameNoExtNight, float ambientVolume)
        {
            DBCAreaTableID = CURRENT_AREATABLEID;
            CURRENT_AREATABLEID++;
            DisplayName = displayName;
            MusicFileNameNoExtDay = musicFileNameNoExtDay;
            MusicFileNameNoExtNight = musicFileNameNoExtNight;
            AmbientSoundFileNameNoExtDay = ambientSoundFileNameNoExtDay;
            AmbientSoundFileNameNoExtNight = ambientSoundFileNameNoExtNight;
            MusicVolume = musicVolume;
            AmbientSoundVolume = ambientVolume;
            AddBoundingBox(boundingBox);
        }

        public void AddBoundingBox(BoundingBox boundingBox)
        {
            // Scale and rotate
            BoundingBox newBox = new BoundingBox();
            newBox.TopCorner.X = boundingBox.BottomCorner.X * -Configuration.CONFIG_GENERATE_WORLD_SCALE;
            newBox.TopCorner.Y = boundingBox.BottomCorner.Y * -Configuration.CONFIG_GENERATE_WORLD_SCALE;
            newBox.TopCorner.Z = boundingBox.TopCorner.Z * Configuration.CONFIG_GENERATE_WORLD_SCALE;
            newBox.BottomCorner.X = boundingBox.TopCorner.X * -Configuration.CONFIG_GENERATE_WORLD_SCALE;
            newBox.BottomCorner.Y = boundingBox.TopCorner.Y * -Configuration.CONFIG_GENERATE_WORLD_SCALE;
            newBox.BottomCorner.Z = boundingBox.BottomCorner.Z * Configuration.CONFIG_GENERATE_WORLD_SCALE;
            BoundingBoxes.Add(newBox);
        }
    }
}
