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
    internal class SoundInstance
    {
        public Vector3 Position = new Vector3();
        public bool Is2DSound = false;
        public int Radius = 0;
        public string SoundNameDay = string.Empty;
        public string SoundNameNight = string.Empty;
        public float VolumeDay = 0f;
        public float VolumeNight = 0f;
        public int CooldownInMSDay = 0;
        public int CooldownInMSNight = 0;
        public int CooldownInMSRandom = 0;
        // public Multiplier -- Unsure what this is
    }
}
