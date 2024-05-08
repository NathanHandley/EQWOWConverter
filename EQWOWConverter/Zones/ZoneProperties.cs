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
using EQWOWConverter.Common;

namespace EQWOWConverter.Zones
{
    internal class ZoneProperties
    {
        public string DescriptiveName = string.Empty;
        public ColorRGB FogColor = new ColorRGB();
        public int FogMinClip = -1;
        public int FogMaxClip = -1;
        public bool DoShowSky = true;
        public Vector3 SafePosition = new Vector3();
        public List<List<string>>MaterialGroupsByTextureNames = new List<List<string>>();

        public void SetFogProperties(int red, int green, int blue, int minClip, int maxClip)
        {
            FogColor.R = red;
            FogColor.G = green;
            FogColor.B = blue;
            FogMinClip = minClip;
            FogMaxClip = maxClip;
        }
        public void SetSafePosition(float x, float y, float z)
        {
            SafePosition.X = x;
            SafePosition.Y = y;
            SafePosition.Z = z;
        }

        public void AddMaterialGrouping(params string[] textureNames)
        {
            List<string> grouping = new List<string>();
            foreach (string textureName in textureNames)
                grouping.Add(textureName);
            MaterialGroupsByTextureNames.Add(grouping);
        }
    }
}
