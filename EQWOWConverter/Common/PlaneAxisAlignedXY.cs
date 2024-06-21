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
    internal class PlaneAxisAlignedXY
    {
        public float NWCornerZ;
        public float NECornerZ;
        public float SECornerZ;
        public float SWCornerZ;
        public Vector2 NWCornerXY = new Vector2();
        public Vector2 SECornerXY = new Vector2();
        bool IsZAxisAligned = false;

        public PlaneAxisAlignedXY() { }

        public PlaneAxisAlignedXY(PlaneAxisAlignedXY other)
        {
            NWCornerZ = other.NWCornerZ;
            NECornerZ = other.NECornerZ;
            SECornerZ = other.SECornerZ;
            SWCornerZ = other.SWCornerZ;
            NWCornerXY = new Vector2(other.NWCornerXY);
            SECornerXY = new Vector2(other.SECornerXY);
        }

        public PlaneAxisAlignedXY(float nwCornerX, float nwCornerY, float seCornerX, float seCornerY,
            float nwCornerZ, float neCornerZ, float seCornerZ, float swCornerZ)
        {
            NWCornerXY.X = nwCornerX;
            NWCornerXY.Y = nwCornerY;
            SECornerXY.X = seCornerX;
            SECornerXY.Y = seCornerY;
            NWCornerZ = nwCornerZ;
            NECornerZ = neCornerZ;
            SECornerZ = seCornerZ;
            SWCornerZ = swCornerZ;            
        }

        public PlaneAxisAlignedXY(float nwCornerX, float nwCornerY, float seCornerX, float seCornerY,
            float allCornersZ)
        {
            NWCornerXY.X = nwCornerX;
            NWCornerXY.Y = nwCornerY;
            SECornerXY.X = seCornerX;
            SECornerXY.Y = seCornerY;
            NWCornerZ = allCornersZ;
            NECornerZ = allCornersZ;
            SECornerZ = allCornersZ;
            SWCornerZ = allCornersZ;
            IsZAxisAligned = true;
        }
    }
}
