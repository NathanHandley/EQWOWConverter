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
    internal class ZonePropertiesLiquidVolume
    {
        public LiquidType LiquidType { get; set; }
        public BoundingBox BoundingBox = new BoundingBox();
        public PlaneAxisAlignedXY PlaneAxisAlignedXY;

        public ZonePropertiesLiquidVolume(LiquidType liquidType, float nwCornerX, float nwCornerY, float seCornerX, float seCornerY, float highZ, float lowZ)
        {
            LiquidType = liquidType;

            // Scale and save the coordinates, rotated
            nwCornerX *= -Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            nwCornerY *= -Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            seCornerX *= -Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            seCornerY *= -Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            highZ *= Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            lowZ *= Configuration.CONFIG_EQTOWOW_WORLD_SCALE;

            // Note that the rotated coordinates will end with SE and NW flipping
            PlaneAxisAlignedXY = new PlaneAxisAlignedXY(seCornerX, seCornerY, nwCornerX, nwCornerY, highZ, lowZ, LiquidSlantType.None);

            // Generate bounding box
            RegenerateBoundingBox();
        }

        public void RegenerateBoundingBox()
        {
            float minZ = PlaneAxisAlignedXY.LowZ;
            float maxZ = PlaneAxisAlignedXY.HighZ;
            BoundingBox = new BoundingBox(PlaneAxisAlignedXY.SECornerXY.X, PlaneAxisAlignedXY.SECornerXY.Y, minZ, PlaneAxisAlignedXY.NWCornerXY.X, PlaneAxisAlignedXY.NWCornerXY.Y, maxZ);
        }

    }
}
