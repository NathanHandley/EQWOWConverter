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
    internal class ZoneLiquid
    {
        public ZoneLiquidType LiquidType = ZoneLiquidType.None;
        public ZoneLiquidShapeType LiquidShape = ZoneLiquidShapeType.Plane;
        public string MaterialName = string.Empty; // Unused, consider removing
        public BoundingBox BoundingBox = new BoundingBox();
        public float MinDepth;
        public float HighZ;
        public float LowZ;
        public ZoneLiquidSlantType SlantType = ZoneLiquidSlantType.None;
        public Vector2 NWCornerXY = new Vector2();
        public Vector2 SECornerXY = new Vector2();

        public ZoneLiquid()
        {

        }

        public ZoneLiquid(ZoneLiquid other)
        {
            LiquidType = other.LiquidType;
            MaterialName = other.MaterialName;
            MinDepth = other.MinDepth;
            HighZ = other.HighZ;
            LowZ = other.LowZ;
            SlantType = other.SlantType;
            NWCornerXY = new Vector2(other.NWCornerXY);
            SECornerXY = new Vector2(other.SECornerXY);
            BoundingBox = new BoundingBox(other.BoundingBox);
            LiquidShape = other.LiquidShape;
        }

        public ZoneLiquid(ZoneLiquidType liquidType, string materialName, float nwCornerX, float nwCornerY, float seCornerX, float seCornerY,
            float highZ, float lowZ, ZoneLiquidSlantType slantType, float minDepth)
        {
            LiquidType = liquidType;
            MaterialName = materialName;

            // Add additional height for ripple rendering
            highZ += Configuration.CONFIG_LIQUID_SURFACE_ADD_Z_HEIGHT;
            lowZ += Configuration.CONFIG_LIQUID_SURFACE_ADD_Z_HEIGHT;

            // Scale and save the coordinates, rotated
            nwCornerX *= -Configuration.CONFIG_GENERATE_WORLD_SCALE;
            nwCornerY *= -Configuration.CONFIG_GENERATE_WORLD_SCALE;
            seCornerX *= -Configuration.CONFIG_GENERATE_WORLD_SCALE;
            seCornerY *= -Configuration.CONFIG_GENERATE_WORLD_SCALE;
            highZ *= Configuration.CONFIG_GENERATE_WORLD_SCALE;
            lowZ *= Configuration.CONFIG_GENERATE_WORLD_SCALE;
            minDepth *= Configuration.CONFIG_GENERATE_WORLD_SCALE;

            MinDepth = minDepth;           
            HighZ = highZ;
            LowZ = lowZ;
            SlantType = slantType;
            SECornerXY.X = seCornerX;
            SECornerXY.Y = seCornerY;
            NWCornerXY.X = nwCornerX;
            NWCornerXY.Y = nwCornerY;

            // Generate bounding box
            RegenerateBoundingBox();
        }

        public ZoneLiquid(ZoneLiquidType liquidType, string materialName, float nwCornerX, float nwCornerY, float seCornerX, float seCornerY,
            float allCornersZ, float minDepth, ZoneLiquidShapeType shapeType)
        {
            LiquidType = liquidType;
            MaterialName = materialName;
            LiquidShape = shapeType;

            // Add additional height for ripple rendering
            allCornersZ += Configuration.CONFIG_LIQUID_SURFACE_ADD_Z_HEIGHT;
            
            // Scale and save the coordinates, rotated
            nwCornerX *= -Configuration.CONFIG_GENERATE_WORLD_SCALE;
            nwCornerY *= -Configuration.CONFIG_GENERATE_WORLD_SCALE;
            seCornerX *= -Configuration.CONFIG_GENERATE_WORLD_SCALE;
            seCornerY *= -Configuration.CONFIG_GENERATE_WORLD_SCALE;
            allCornersZ *= Configuration.CONFIG_GENERATE_WORLD_SCALE;
            minDepth *= Configuration.CONFIG_GENERATE_WORLD_SCALE;

            MinDepth = minDepth;
            HighZ = allCornersZ;
            LowZ = allCornersZ;

            // Swap corners due to model space differences for water
            SECornerXY.X = seCornerX;
            SECornerXY.Y = seCornerY;
            NWCornerXY.X = nwCornerX;
            NWCornerXY.Y = nwCornerY;

            // Generate bounding box
            RegenerateBoundingBox();
        }

        public void RegenerateBoundingBox()
        {
            float minZ = LowZ - MinDepth;
            float maxZ = HighZ;
            float minX = MathF.Min(SECornerXY.X, NWCornerXY.X);
            float maxX = MathF.Max(SECornerXY.X, NWCornerXY.X);
            float minY = MathF.Min(SECornerXY.Y, NWCornerXY.Y);
            float maxY = MathF.Max(SECornerXY.Y, NWCornerXY.Y);
            BoundingBox = new BoundingBox(minX, minY, minZ, maxX, maxY, maxZ);
        }

        public List<ZoneLiquid> SplitIntoSizeRestictedChunks(int maximumXYSizePerChunk)
        {
            List<ZoneLiquid> dividedPlanes = new List<ZoneLiquid> { new ZoneLiquid(this) };

            if (maximumXYSizePerChunk <= 0)
            {
                dividedPlanes.Add(this);
                Logger.WriteError("ZonePropertiesLiquidPlane maximumXYSizePerChunk is less than or zero.");
                return dividedPlanes;
            }
            if (SlantType != ZoneLiquidSlantType.None)
            {
                Logger.WriteError("ZonePropertiesLiquidPlane is not z axis aligned but is being split.  There will be issues.");
            }

            bool doSplitFurther = true;
            while (doSplitFurther)
            {
                doSplitFurther = false;
                List<ZoneLiquid> newPlanes = new List<ZoneLiquid>();
                foreach (ZoneLiquid curPlane in dividedPlanes)
                {
                    if (curPlane.GetXDistance() >= maximumXYSizePerChunk || curPlane.GetYDistance() >= maximumXYSizePerChunk)
                    {
                        ZoneLiquid newPlane = new ZoneLiquid(curPlane);
                        if (curPlane.GetXDistance() > curPlane.GetYDistance())
                        {
                            float planeSplitDistance = (curPlane.NWCornerXY.X + curPlane.SECornerXY.X) * 0.5f;
                            curPlane.SECornerXY.X = planeSplitDistance;
                            newPlane.NWCornerXY.X = planeSplitDistance;
                        }
                        else
                        {
                            float planeSplitDistance = (curPlane.NWCornerXY.Y + curPlane.SECornerXY.Y) * 0.5f;
                            curPlane.SECornerXY.Y = planeSplitDistance;
                            newPlane.NWCornerXY.Y = planeSplitDistance;
                        }
                        doSplitFurther = true;
                        curPlane.RegenerateBoundingBox();
                        newPlane.RegenerateBoundingBox();
                        newPlanes.Add(newPlane);
                    }
                }
                foreach (ZoneLiquid newPlane in newPlanes)
                    dividedPlanes.Add(newPlane);
            }

            return dividedPlanes;
        }

        private float GetXDistance()
        {
            if (NWCornerXY.X > SECornerXY.X)
                return MathF.Abs(NWCornerXY.X - SECornerXY.X);
            else
                return MathF.Abs(SECornerXY.X - NWCornerXY.X);
        }

        private float GetYDistance()
        {
            if (NWCornerXY.Y > SECornerXY.Y)
                return MathF.Abs(NWCornerXY.Y - SECornerXY.Y);
            else
                return MathF.Abs(SECornerXY.Y - NWCornerXY.Y);
        }

        public ZoneLiquid GeneratePartialFromBoundingBox(BoundingBox boundingBox)
        {
            if (boundingBox.DoesIntersectBox(BoundingBox) == false)
                throw new Exception("Attempted to generate a partial zone liquid plane but the passed bounding box didn't intersect with it");
            if (SlantType != ZoneLiquidSlantType.None)
                Logger.WriteDetail("Warning!  Unhandled slanting type for ZoneLiquid::GeneratePartialFromBoundingBox, which could cause errors");

            ZoneLiquid newLiquid = new ZoneLiquid(LiquidType, MaterialName, boundingBox.TopCorner.X, boundingBox.TopCorner.Y, boundingBox.BottomCorner.X, boundingBox.BottomCorner.Y,
                boundingBox.TopCorner.Z, (boundingBox.BottomCorner.Z - boundingBox.TopCorner.Z) * -1, LiquidShape);

            return newLiquid;
        }
    }
}
