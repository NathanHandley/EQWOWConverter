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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Zones
{
    internal class ZoneArea
    {
        private static UInt32 CURRENT_AREATABLEID = Configuration.CONFIG_DBCID_AREATABLE_ID_START;

        public UInt32 DBCAreaTableID;
        public UInt32 DBCParentAreaTableID = 0; // Zero is no parent
        public string DisplayName = string.Empty;
        public string ParentAreaDisplayName = string.Empty;
        public List<BoundingBox> BoundingBoxes = new List<BoundingBox>();
        public List<ZoneLiquidGroup> LiquidGroups = new List<ZoneLiquidGroup>();
        public BoundingBox MaxBoundingBox = new BoundingBox();
        public string MusicFileNameNoExtDay = string.Empty;
        public string MusicFileNameNoExtNight = string.Empty;
        public bool MusicLoop = false;
        public float MusicVolume = 1f;
        public ZoneAreaMusic? AreaMusic = null;        
        public string AmbientSoundFileNameNoExtDay = string.Empty;
        public string AmbientSoundFileNameNoExtNight = string.Empty;
        public ZoneAreaAmbientSound? AreaAmbientSound = null;

        public ZoneArea(string displayName, string parentAreaDisplayName)
        {
            DBCAreaTableID = CURRENT_AREATABLEID;
            CURRENT_AREATABLEID++;
            DisplayName = displayName;
            ParentAreaDisplayName = parentAreaDisplayName;
        }

        public void AddBoundingBox(BoundingBox boundingBox, bool scaleAndRotate)
        {
            // Scale and rotate
            if (scaleAndRotate == true)
            {
                BoundingBox newBox = new BoundingBox();
                newBox.TopCorner.X = boundingBox.BottomCorner.X * -Configuration.CONFIG_GENERATE_WORLD_SCALE;
                newBox.TopCorner.Y = boundingBox.BottomCorner.Y * -Configuration.CONFIG_GENERATE_WORLD_SCALE;
                newBox.TopCorner.Z = boundingBox.TopCorner.Z * Configuration.CONFIG_GENERATE_WORLD_SCALE;
                newBox.BottomCorner.X = boundingBox.TopCorner.X * -Configuration.CONFIG_GENERATE_WORLD_SCALE;
                newBox.BottomCorner.Y = boundingBox.TopCorner.Y * -Configuration.CONFIG_GENERATE_WORLD_SCALE;
                newBox.BottomCorner.Z = boundingBox.BottomCorner.Z * Configuration.CONFIG_GENERATE_WORLD_SCALE;
                BoundingBoxes.Add(newBox);
            }
            else
            {
                BoundingBoxes.Add(new BoundingBox(boundingBox));
            }

            MaxBoundingBox = BoundingBox.GenerateBoxFromBoxes(BoundingBoxes);
        }

        public void SetAmbientSound(string ambientSoundFileNameNoExtDay, string ambientSoundFileNameNoExtNight)
        {            
            AmbientSoundFileNameNoExtDay = ambientSoundFileNameNoExtDay;
            AmbientSoundFileNameNoExtNight = ambientSoundFileNameNoExtNight;
        }

        public void SetMusic(string musicFileNameNoExtDay, string musicFileNameNoExtNight, bool loopMusic, float musicVolume = 1f)
        {
            MusicFileNameNoExtDay = musicFileNameNoExtDay;
            MusicFileNameNoExtNight = musicFileNameNoExtNight;
            loopMusic = true;
            MusicVolume = musicVolume;
        }
    }
}
