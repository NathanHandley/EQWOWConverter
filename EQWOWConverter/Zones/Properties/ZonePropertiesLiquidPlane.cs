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
    internal class ZonePropertiesLiquidPlane
    {
        public LiquidType LiquidType = LiquidType.None;
        public string MaterialName = string.Empty;
        public PlaneAxisAlignedXY PlaneAxisAlignedXY;
        public BoundingBox BoundingBox = new BoundingBox();
        public float MinDepth;

        public ZonePropertiesLiquidPlane(LiquidType liquidType, string materialName, float nwCornerX, float nwCornerY, float seCornerX, float seCornerY,
            float nwCornerZ, float neCornerZ, float seCornerZ, float swCornerZ, float minDepth)
        {
            LiquidType = liquidType;
            MaterialName = materialName;
            MinDepth = minDepth;

            // Scale and save the coordinates, rotated
            nwCornerX *= -Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            nwCornerY *= -Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            seCornerX *= -Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            seCornerY *= -Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            nwCornerZ *= Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            neCornerZ *= Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            seCornerZ *= Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            swCornerZ *= Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            minDepth *= Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            // Note that the rotated coordinates will end with SE and NW flipping
            PlaneAxisAlignedXY = new PlaneAxisAlignedXY(seCornerX, seCornerY, nwCornerX, nwCornerY, nwCornerZ, neCornerZ, seCornerZ, swCornerZ);

            // Generate bounding box
            RegenerateBoundingBox();
        }

        public ZonePropertiesLiquidPlane(LiquidType liquidType, string materialName, float nwCornerX, float nwCornerY, float seCornerX, float seCornerY,
            float allCornersZ, float minDepth)
        {
            LiquidType = liquidType;
            MaterialName = materialName;
            MinDepth = minDepth;

            // Scale and save the coordinates, rotated
            nwCornerX *= -Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            nwCornerY *= -Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            seCornerX *= -Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            seCornerY *= -Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            allCornersZ *= Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            // Note that the rotated coordinates will end with SE and NW flipping
            PlaneAxisAlignedXY = new PlaneAxisAlignedXY(seCornerX, seCornerY, nwCornerX, nwCornerY, allCornersZ);

            // Generate bounding box
            RegenerateBoundingBox();
        }

        public void RegenerateBoundingBox()
        {
            float minZ;
            float maxZ;
            if (PlaneAxisAlignedXY.IsZAxisAligned == true)
                maxZ = PlaneAxisAlignedXY.NWCornerZ;
            else
                maxZ = MathF.Max(MathF.Max(MathF.Max(PlaneAxisAlignedXY.NWCornerZ, PlaneAxisAlignedXY.NECornerZ), PlaneAxisAlignedXY.SECornerZ), PlaneAxisAlignedXY.SWCornerZ);
            minZ = maxZ - MinDepth;
            BoundingBox = new BoundingBox(PlaneAxisAlignedXY.SECornerXY.X, PlaneAxisAlignedXY.SECornerXY.Y, minZ, PlaneAxisAlignedXY.NWCornerXY.X, PlaneAxisAlignedXY.NWCornerXY.Y, maxZ);
            //BoundingBox.ApplyModelToWorldCoordinatesTranslation();
        }
    }
}
