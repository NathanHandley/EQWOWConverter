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
        public ZoneContinent Continent;
        public ColorRGB FogColor = new ColorRGB();
        public int FogMinClip = -1;
        public int FogMaxClip = -1;
        public bool DoShowSky = true;
        public Vector3 SafePosition = new Vector3();
        public float SafeOrientation = 0;
        public List<List<string>>MaterialGroupsByName = new List<List<string>>(); // To Delete

        public void SetBaseZoneProperties(string descriptiveName, float safeX, float safeY, float safeZ, float safeOrientation, ZoneContinent continent)
        {
            DescriptiveName = descriptiveName;
            Continent = continent;
            SafePosition.X = safeX;
            SafePosition.Y = safeY;
            SafePosition.Z = safeZ;
            SafeOrientation = safeOrientation;
        }

        public void SetFogProperties(int red, int green, int blue, int minClip, int maxClip)
        {
            FogColor.R = red;
            FogColor.G = green;
            FogColor.B = blue;
            FogMinClip = minClip;
            FogMaxClip = maxClip;
        }

        //public void AddMaterialGrouping(params string[] materialNames)
        //{
        //    List<string> grouping = new List<string>();
        //    foreach (string materialName in materialNames)
        //        grouping.Add(materialName);
        //    MaterialGroupsByName.Add(grouping);
        //}
    }
}
