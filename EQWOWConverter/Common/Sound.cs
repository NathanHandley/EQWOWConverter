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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Common
{
    internal class Sound
    {
        private static int CURRENT_SOUNDENTRY_ID = Configuration.CONFIG_DBCID_SOUNDENTRIES_ID_START;

        public int DBCID;
        public string Name = string.Empty;
        public string AudioFileNameNoExt = string.Empty;
        public SoundType Type = SoundType.None;
        public float Volume;
        public bool Loop = false;
        public float MinDistance;
        public float DistanceCutoff;

        public Sound(string name, string audioFileName, float volume, SoundType type, float minDistance, float distanceCutoff, bool loop)
        {
            DBCID = CURRENT_SOUNDENTRY_ID;
            CURRENT_SOUNDENTRY_ID++;
            Name = name;
            AudioFileNameNoExt = audioFileName;
            Volume = volume;
            Type = type;
            MinDistance = minDistance;
            DistanceCutoff = distanceCutoff;
            Loop = loop;
        }
    }
}
