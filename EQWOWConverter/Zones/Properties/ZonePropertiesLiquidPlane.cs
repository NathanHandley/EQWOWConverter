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
        public BoundingBox BoundingBox;

        public ZonePropertiesLiquidPlane(LiquidType liquidType, string materialName, float nwCornerX, float nwCornerY, float seCornerX, float seCornerY,
            float nwCornerZ, float neCornerZ, float seCornerZ, float swCornerZ, float minDepth)
        {
            LiquidType = liquidType;
            MaterialName = materialName;
            PlaneAxisAlignedXY = new PlaneAxisAlignedXY(nwCornerX, nwCornerY, seCornerX, seCornerY, nwCornerZ, neCornerZ, seCornerZ, swCornerZ);

            // Generate bounding box

            float minZ = MathF.Min(MathF.Min(MathF.Min(nwCornerZ, neCornerZ), seCornerZ), swCornerZ) - minDepth;
            float maxZ = MathF.Max(MathF.Max(MathF.Max(nwCornerZ, neCornerZ), seCornerZ), swCornerZ);


            BoundingBox = new BoundingBox(seCornerX, seCornerY, minZ, nwCornerX, nwCornerY, maxZ);// <- Breaks water


            //BoundingBox = new BoundingBox(seCornerX, seCornerY, minZ, nwCornerX, nwCornerY, maxZ, Configuration.CONFIG_EQTOWOW_ADDED_BOUNDARY_AMOUNT);
            BoundingBox secondBox = new BoundingBox(-377, -1300, -87, -5, -787, 150);
        }
    }
}
