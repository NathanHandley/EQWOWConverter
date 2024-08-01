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
    internal class ZoneLiquidVolume
    {
        public ZoneLiquidType LiquidType { get; set; }
        public BoundingBox BoundingBox = new BoundingBox();
        public ZoneLiquidPlane LiquidPlane;

        public ZoneLiquidVolume(ZoneLiquidType liquidType, float nwCornerX, float nwCornerY, float seCornerX, float seCornerY, float highZ, float lowZ)
        {
            LiquidType = liquidType;

            // Note that the rotated coordinates will end with SE and NW flipping
            LiquidPlane = new ZoneLiquidPlane(liquidType, "", nwCornerX, nwCornerY, seCornerX, seCornerY, highZ, lowZ, ZoneLiquidSlantType.None, highZ-lowZ);

            // Generate bounding box
            RegenerateBoundingBox();
        }

        public void RegenerateBoundingBox()
        {
            float minZ = LiquidPlane.LowZ;
            float maxZ = LiquidPlane.HighZ;
            BoundingBox = new BoundingBox(LiquidPlane.SECornerXY.X, LiquidPlane.SECornerXY.Y, minZ, LiquidPlane.NWCornerXY.X, LiquidPlane.NWCornerXY.Y, maxZ);
        }

    }
}
