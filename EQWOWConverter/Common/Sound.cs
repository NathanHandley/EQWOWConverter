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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Common
{
    internal class Sound
    {
        public int Id = 0;
        public string Name = string.Empty;
        public string AudioFileName = string.Empty;
        public SoundType Type = SoundType.None;
        public float Volume = 1f;
        public bool Loop = false;
        public float MinDistance = 8f; // Default for zone music
        public float DistanceCutoff = 45f; // Default for zone music

        public Sound(int id, string name, string audioFileName, SoundType type)
        {
            Id = id;
            Name = name;
            AudioFileName = audioFileName;
            Type = type;
        }
    }
}